using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllieJoe.SudokuSolver.View;
using UnityEngine;

namespace AllieJoe.SudokuSolver
{
    public enum SudokuType {_4x4, _9x9}

    public class SudokuSolver : MonoBehaviour
    {
        [SerializeField] private SudokuType _type = SudokuType._4x4;
        [SerializeField] private SudokuRenderer _renderer;
        
        private Board _board;
        private Stack<SudokuSolverStep> _steps = new Stack<SudokuSolverStep>(); 

        private static int[,] MockBoard_9x9()
        {
            return new int[9, 9]
            {
                {0, 0, 0,   6, 0, 0,   0, 0, 3},
                {8, 0, 0,   0, 0, 5,   0, 0, 0},
                {0, 0, 0,   4, 0, 0,   5, 2, 0},
                {0, 0, 0,   0, 7, 2,   0, 0, 0},
                {0, 7, 6,   0, 0, 4,   0, 0, 0},
                {5, 4, 2,   3, 0, 0,   8, 0, 0},
                {0, 3, 8,   1, 4, 0,   0, 9, 5},
                {7, 0, 0,   0, 3, 0,   0, 0, 0},
                {0, 2, 0,   0, 6, 8,   3, 0, 7},
            };
        }
        
        private static int[,] MockBoard_4x4()
        {
            return new int[4,4]
            {
                {0, 4,   0, 0},
                {0, 2,   3, 0},
                {0, 0,   0, 3},
                {4, 3,   0, 2},
            };
        }

        private void Start()
        {
            SetBoard();
        }

        private void SetBoard()
        {
            if(_type == SudokuType._4x4)
                _board = new Board(MockBoard_4x4());
            else if(_type == SudokuType._9x9)
                _board = new Board(MockBoard_9x9());
            _renderer.CreateBoard(_board);
            _steps.Clear();
        }

        private  void Tick()
        {
            if(_board.Solved)
                return;
            
            (int x, int y) cellToCollapse = _board.FindMintEntropyCell();
            if (cellToCollapse.x < 0)
            {
                Debug.Log("Can't find a solution. Try again!!!"); 
                _renderer.UpdateBoard(_board);
                return;
            }

            bool success = Step(cellToCollapse.x, cellToCollapse.y);
            _renderer.UpdateBoard(_board);
            _renderer.HighlightCollapsedPiece(cellToCollapse.x, cellToCollapse.y);
            
            if(_board.Solved)
                Debug.Log("Solved!!!");
            else if(!success && _board.HasInvalidTiles())
                Debug.Log("Can't find a solution. Try again!!!"); 
        }


        private bool Step(int x, int y)
        {
            SudokuSolverStep step = new SudokuSolverStep(_board, x, y);
            step.Execute();
            if (step.Status == SudokuSolverStepStatus.Completed)
            {
                _steps.Push(step);
                return true;
            }
            
            //Backtrack until a good state
            SudokuSolverStepStatus currentStatus = SudokuSolverStepStatus.Abort;
            while (currentStatus == SudokuSolverStepStatus.Abort)
            {
                if (_steps.Count == 0)
                {
                    return false;
                }

                Debug.LogError("Backtracking...");
                SudokuSolverStep backtrackStep = _steps.Pop();
                backtrackStep.UndoStep();
                backtrackStep.Execute();
                currentStatus = backtrackStep.Status;
                if (currentStatus == SudokuSolverStepStatus.Completed)
                {
                    _steps.Push(backtrackStep);
                    break;
                }
            }
            
            return true;
        }


        #region CollapseAndPropagate
        private bool CollapseAndPropagate(int x, int y)
        {
            bool successCollapse = true;
            //BoardStateData boardState = _board.GetBoardState();
            Queue<BoardCell> pendingCollapsedCells = new Queue<BoardCell>();
            
            BoardCell cellToCollapse = _board.GetCell(x, y);
            if (!cellToCollapse.TryCollapse())
            {
                successCollapse = false;
                Debug.LogError($"Can't collapse {cellToCollapse.Pos}");
            }
            
            //boardState.SetCellCollapsed(cellToCollapse);
            
            int collapsedValue = cellToCollapse.Value;
            int collapseX = cellToCollapse.X;
            int collapseY = cellToCollapse.Y;
            for (int i = 0; i < _board.Size; i++)
            {
                //Add Horizontal line
                if (!TryUpdateCell(i, collapseY, collapsedValue, pendingCollapsedCells))
                {
                    Debug.LogError($"Can't update domain on {i}, {collapseY}");
                }
                //Add Vertical Line
                if(!TryUpdateCell(collapseX, i, collapsedValue, pendingCollapsedCells))
                {
                    Debug.LogError($"Can't update domain on {collapseX}, {i}");
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
        
        #endregion
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && _board != null)
                Tick();

            if (Input.GetKeyDown(KeyCode.R))
                SetBoard();

        }
        
    }

}