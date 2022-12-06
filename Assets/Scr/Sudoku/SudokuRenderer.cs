using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AllieJoe.SudokuSolver.View
{
    public class SudokuRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform _boardParent;
        [SerializeField] private RectTransform _rowPrefab;
        [SerializeField] private SudokuPieceRenderer _piecePrefab;

        private int _size;
        private List<SudokuPieceRenderer> _pieceRenderers = new List<SudokuPieceRenderer>();
        
        public void CreateBoard(Board board)
        {
            Clear();
            _size = board.Size;
            for (int x = 0; x < _size; x++)
            {
                RectTransform row = Instantiate(_rowPrefab, _boardParent);
                for (int y = 0; y < _size; y++)
                {
                    SudokuPieceRenderer piece = Instantiate(_piecePrefab, row);
                    BoardCell c = board.GetCell(x, y);
                    piece.Init();
                    piece.SetData(c.Value, c.Domain);
                    piece.SetBorders(x, y, board.QuadrantSize);
                    _pieceRenderers.Add(piece);
                }
            }
            
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        
        public void UpdateBoard(Board board)
        {
            _size = board.Size;
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    BoardCell c = board.GetCell(x, y);
                    _pieceRenderers[x * _size + y].SetData(c.Value, c.Domain);
                }
            }
        }

        public void HighlightCollapsedPiece(int x, int y)
        {
            _pieceRenderers[x * _size + y].Highlight();
        }

        public void HighlightCompleted()
        {
            foreach (SudokuPieceRenderer sudokuPieceRenderer in _pieceRenderers)
            {
                sudokuPieceRenderer.HighlightCompleted();
            }
        }
        
        public void HighlightNoSolution()
        {
            foreach (SudokuPieceRenderer sudokuPieceRenderer in _pieceRenderers)
            {
                sudokuPieceRenderer.HighlightNoSolution();
            }
        }

        private void Clear()
        {
            _pieceRenderers.Clear();
            foreach (Transform t in _boardParent)
            {
                t.gameObject.SetActive(false);
                Destroy(t.gameObject);
            }
        }
    }
}