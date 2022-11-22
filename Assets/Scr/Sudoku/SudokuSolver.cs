using System;
using System.Collections;
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
        }

        private void Tick()
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

            bool success = _board.CollapseAndPropagate(cellToCollapse.x, cellToCollapse.y);
            _renderer.UpdateBoard(_board);
            _renderer.HighlightCollapsedPiece(cellToCollapse.x, cellToCollapse.y);
            
            if(_board.Solved)
                Debug.Log("Solved!!!");
            else if(!success && _board.HasInvalidTiles())
                Debug.Log("Can't find a solution. Try again!!!"); 
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