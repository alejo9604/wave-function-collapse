using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Mathf = UnityEngine.Mathf;

namespace AllieJoe.SudokuSolver
{
    public enum SudokuSolverStepStatus {None, Completed, Fail, Abort}
    
    public class SudokuSolverStep
    {
        private const int TOTAL_ATTEMPTS = 9;
        
        private Board _board;
        private (int x, int y) _cellPos;

        private Queue<BoardCell> pendingCollapsedCells = null;//new Queue<BoardCell>();
        private bool useRandomCollapseValue = false;

        private BoardStateData _boardState;
        private BoardCellCollapseData _cellCollapsedData;
        private BoardCell _cellToCollapse;
        
        public SudokuSolverStepStatus Status { get; private set; }

        public SudokuSolverStep(Board board, int x, int y)
        {
            _board = board;
            _cellPos = (x, y);
        }

        public void Execute()
        {
            int attempts = 0;
            Status = SudokuSolverStepStatus.None;
            
            while (attempts < TOTAL_ATTEMPTS)
            {
                attempts++;
                if (Status == SudokuSolverStepStatus.Fail)
                {
                    UndoAndRemoveCollapseValue();
                }
                
                TryCollapseAndPropagate();

                if (Status == SudokuSolverStepStatus.Completed ||
                    Status == SudokuSolverStepStatus.Abort)
                {
                    break;
                }
            }

            if (Status == SudokuSolverStepStatus.Fail)
            {
                Status = SudokuSolverStepStatus.Abort;
            }

            if (Status == SudokuSolverStepStatus.Completed)
            {
                Debug.LogError($"Collapsing {_cellCollapsedData.X},{_cellCollapsedData.Y} to {_cellCollapsedData.ValueCollapsed}");
            }
        }
        
        public bool UndoStep()
        {
            Debug.LogError($"Undo step: board and removing {_cellCollapsedData.ValueCollapsed} from ({_cellCollapsedData.X},{_cellCollapsedData.Y}).");
            
            //Undo collapse
            BoardCellData cellDataCollapse = _boardState.Cells[_cellCollapsedData.X * _board.Size + _cellCollapsedData.Y];
            cellDataCollapse.Collapsed = false;
            cellDataCollapse.Value = 0;
            cellDataCollapse.Domain = _cellCollapsedData.Domain;
            
            RestoreBoard();
            return _cellToCollapse.TryUpdateDomain(_cellCollapsedData.ValueCollapsed);
        }

        private void TryCollapseAndPropagate()
        {
            _boardState = _board.GetBoardState();
            
            _cellToCollapse = _board.GetCell(_cellPos.x, _cellPos.y);
            
            _cellCollapsedData = new BoardCellCollapseData(_cellToCollapse);

            if (_cellToCollapse.Entropy == 0)
            {
                Status = SudokuSolverStepStatus.Abort;
                Debug.LogError($"Cell {_cellPos} without domain!");
                return;
            }

            bool success = CollapseAndPropagate();
            Status = success ? SudokuSolverStepStatus.Completed : SudokuSolverStepStatus.Fail;
        }

        private bool CollapseAndPropagate()
        {
            bool successCollapse = true;
            if (!_cellToCollapse.TryCollapse(useRandomCollapseValue))
            {
                successCollapse = false;
                Debug.LogError($"Can't collapse {_cellToCollapse.Pos}");
            }

            _cellCollapsedData.ValueCollapsed = _cellToCollapse.Value;
            
            int collapsedValue = _cellToCollapse.Value;
            int collapseX = _cellToCollapse.X;
            int collapseY = _cellToCollapse.Y;
            for (int i = 0; i < _board.Size; i++)
            {
                //Add Horizontal line
                if (!TryUpdateCell(i, collapseY, collapsedValue, pendingCollapsedCells))
                {
                    Debug.LogError($"Can't update domain on {i}, {collapseY}");
                    return false;
                }
                //Add Vertical Line
                if(!TryUpdateCell(collapseX, i, collapsedValue, pendingCollapsedCells))
                {
                    Debug.LogError($"Can't update domain on {collapseX}, {i}");
                    return false;
                }
            }

            int quadrantSize = _board.QuadrantSize;
            int initQuadrantX = Mathf.FloorToInt(collapseX / (float) quadrantSize) * quadrantSize;
            int initQuadrantY = Mathf.FloorToInt(collapseY / (float) quadrantSize) * quadrantSize;
            for (int i = initQuadrantX; i < initQuadrantX + quadrantSize; i++)
            {
                for (int j = initQuadrantY; j < initQuadrantY + quadrantSize; j++)
                {
                    if (!TryUpdateCell(i, j, collapsedValue, pendingCollapsedCells))
                    {
                        Debug.LogError($"Can't update domain on {i}, {j}");
                        return false;
                    }
                }
            }

            return successCollapse;
        }
        
        private bool TryUpdateCell(int x, int y, int collapsedValue, Queue<BoardCell> pendingCollapsedCells = null)
        {
            BoardCell temp = _board.GetCell(x, y);
            if (temp.TryUpdateDomain(collapsedValue) && temp.CanCollapse)
            {
                pendingCollapsedCells?.Enqueue(temp);
            }
            
            return temp.Entropy > 0;
        }

        private bool UndoAndRemoveCollapseValue()
        {
            Debug.LogError($"Restoring board and removing {_cellCollapsedData.ValueCollapsed} from ({_cellCollapsedData.X},{_cellCollapsedData.Y}).");
            RestoreBoard();
            return _cellToCollapse.TryUpdateDomain(_cellCollapsedData.ValueCollapsed);
        }

        private void RestoreBoard()
        {
            _board.Bind(_boardState);
        }

    }
}