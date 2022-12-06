using System.Collections.Generic;
using AllieJoe.SudokuSolver.View;
using AllieJoe.SudokuSolver.Helper;
using UnityEngine;

namespace AllieJoe.SudokuSolver
{
    public enum SudokuType {Random, _4x4, _9x9}

    public class SudokuSolver : MonoBehaviour
    {
        [SerializeField] private SudokuType _type = SudokuType._4x4;
        [SerializeField] private SudokuRenderer _renderer;
        
        [Range(1, 50)]
        public int K = 20;
        
        private Board _board;
        private Stack<SudokuSolverStep> _steps = new Stack<SudokuSolverStep>();

        private void Start()
        {
            SetBoard();
        }

        private void SetBoard()
        {
            if (_type == SudokuType.Random)
                _board = new Board(SudokuGenerator.Generate(9, K));
            else if(_type == SudokuType._4x4)
                _board = new Board(SudokuGenerator.MockBoard_4x4());
            else if(_type == SudokuType._9x9)
                _board = new Board(SudokuGenerator.MockBoard_9x9());
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
                Debug.LogError("Can't find a solution. Try again!!!"); 
        }


        private bool Step(int x, int y)
        {
            //Try to execute next move
            SudokuSolverStep step = new SudokuSolverStep(_board, x, y);
            step.Execute();
            if (step.Status == SudokuSolverStepStatus.Completed)
            {
                _steps.Push(step);
                return true;
            }
            
            //Backtrack until a good state
            SudokuSolverStepStatus currentStatus = SudokuSolverStepStatus.Error;
            while (currentStatus == SudokuSolverStepStatus.Error)
            {
                if (_steps.Count == 0)
                    return false;

                SudokuSolverStep backtrackStep = _steps.Pop();
                if(!backtrackStep.UndoStep())
                {
                    Debug.LogError("Failed to Undo Step. Aborting...");
                    return false;
                }
                
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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && _board != null)
                Tick();

            if (Input.GetKeyDown(KeyCode.R))
                SetBoard();

        }
        
    }

}