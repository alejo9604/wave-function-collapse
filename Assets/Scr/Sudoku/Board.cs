using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.SudokuSolver
{
    public class Board
    {
        public int Size { get; private set; }
        public int QuadrantSize { get; private set; }
        public bool Solved => remainCellsToCollapse == 0;

        private List<BoardCell> _cells;
        private int remainCellsToCollapse;

        public BoardCell GetCell(int x, int y)
        {
            return _cells[x * Size + y];
        }
        
        public int GetValue(int x, int y)
        {
            return GetCell(x,y).Value;
        }

        public Board(int[,] data)
        {
            Size = data.GetLength(0);
            QuadrantSize = (int) Mathf.Sqrt(Size);
            _cells = new List<BoardCell>(Size * Size);
            for (int x = 0; x < Size; x++) 
            {
                for (int y = 0; y  < Size; y ++)
                {
                    _cells.Add(new BoardCell(x, y, data[x, y], Size));
                }
            }

            SetInitialState();
        }

        private void SetInitialState()
        {
            remainCellsToCollapse = _cells.Count;
            List<BoardCell> collapsedCells = new List<BoardCell>();
            foreach (var c in _cells)
            {
                if (c.Collapsed)
                {
                    collapsedCells.Add(c);
                    remainCellsToCollapse--;
                }
            }

            BoardCell temp = null;
            foreach (var collapsed in collapsedCells)
            {
                (int x, int y) = collapsed.Pos;
                int collapsedValue = collapsed.Value;
                for (int i = 0; i < Size; i++)
                {
                    //Add Horizontal line
                    temp = GetCell(i, y);
                    temp.UpdateDomain(collapsedValue);
                    //Add Vertical Line
                    temp = GetCell(x, i);
                    temp.UpdateDomain(collapsedValue);
                }

                int initQuadrantX = Mathf.FloorToInt(x / (float) QuadrantSize) * QuadrantSize;
                int initQuadrantY = Mathf.FloorToInt(y / (float) QuadrantSize) * QuadrantSize;
                for (int i = initQuadrantX; i < initQuadrantX + QuadrantSize; i++)
                {
                    for (int j = initQuadrantY; j < initQuadrantY + QuadrantSize; j++)
                    {
                        temp = GetCell(i, j);
                        temp.UpdateDomain(collapsedValue);
                    }
                }
            }
        }

        public bool HasInvalidTiles()
        {
            return _cells.Exists(cell => !cell.Collapsed && cell.Entropy == 0);
        }

        public (int, int) FindMintEntropyCell()
        {
            int minEntropy = 100;
            List<BoardCell> minEntries = new List<BoardCell>();
            foreach (var c in _cells)
            {
                if(c.Collapsed) 
                    continue;
                
                if (c.Entropy == minEntropy)
                {
                    minEntries.Add(c);
                }
                else if (c.Entropy < minEntropy)
                {
                    minEntries.Clear();
                    minEntries.Add(c);
                    minEntropy = c.Entropy;
                }
            }

            if (minEntries.Count > 0)
                return minEntries[Random.Range(0, minEntries.Count)].Pos;
            else
                return (-1, -1);
        }

        public bool CollapseAndPropagate(int x, int y)
        {
            Queue<BoardCell> pendingCollapsedCells = new Queue<BoardCell>();
            pendingCollapsedCells.Enqueue(GetCell(x, y));

            bool successCollapse = true;
            while (pendingCollapsedCells.Count > 0)
            {
                BoardCell collapseCell = pendingCollapsedCells.Dequeue();
                if (!collapseCell.TryCollapse())
                {
                    successCollapse = false;
                    continue;
                }

                remainCellsToCollapse--;
                
                int collapsedValue = collapseCell.Value;
                int collapseX = collapseCell.X;
                int collapseY = collapseCell.Y;
                for (int i = 0; i < Size; i++)
                {
                    //Add Horizontal line
                    TryUpdateCell(i, collapseY, collapsedValue, pendingCollapsedCells);
                    //Add Vertical Line
                    TryUpdateCell(collapseX, i, collapsedValue, pendingCollapsedCells);
                }
            
                int initQuadrantX = Mathf.FloorToInt(collapseX / (float) QuadrantSize) * QuadrantSize;
                int initQuadrantY = Mathf.FloorToInt(collapseY / (float) QuadrantSize) * QuadrantSize;
                for (int i = initQuadrantX; i < initQuadrantX + QuadrantSize; i++)
                {
                    for (int j = initQuadrantY; j < initQuadrantY + QuadrantSize; j++)
                    {
                        TryUpdateCell(i, j, collapsedValue, pendingCollapsedCells);
                    }
                }
            }

            return successCollapse;
        }

        private void TryUpdateCell(int x, int y, int collapsedValue, Queue<BoardCell> pendingCollapsedCells = null)
        {
            BoardCell temp = GetCell(x, y);
            if (temp.UpdateDomain(collapsedValue) && temp.Entropy <= 1)
            {
                pendingCollapsedCells?.Enqueue(temp);
            }
        }

        public override string ToString()
        {
            string text = "";
            for (int x = 0; x < Size; x++)
            {
                text += "[";
                for (int y = 0; y  < Size; y ++)
                {
                    int v = GetValue(x, y);
                    text += v > 0 ? $"{GetValue(x, y)}," : "_,";
                }
                text += "]\n";
            }

            return text;
        }
    }
}