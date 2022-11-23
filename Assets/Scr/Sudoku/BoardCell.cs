using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AllieJoe.SudokuSolver
{
    public class BoardCell
    {
        public int Value { get; private set; }
        public int Entropy => Collapsed ? int.MaxValue : Domain.Count;
        public bool Collapsed { get; private set; }
        public bool CanCollapse => !Collapsed && Domain.Count <= 1;
        
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public (int, int) Pos => (X, Y);
        
        public List<int> Domain = new List<int>();

        public BoardCell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public BoardCell(int x, int y, int value, int domainSize) : this(x, y)
        {
            if (value > 0)
            {
                Collapse(value);
                return;
                
            }
            
            Value = 0;
            Collapsed = false;
            Domain = new List<int>();
            for (int i = 0; i < domainSize; i++)
            {
                Domain.Add(i + 1);
            }
        }

        public bool TryCollapse(bool random = true)
        {
            if (Domain.Count <= 0)
            {
                Debug.LogError("Trying to collapse with value not present on the Domain.");
                return false;
            }

            int valueToCollapse = random ? Domain[Random.Range(0, Domain.Count)] : Domain[0];
            Collapse(valueToCollapse);
            return true;
        }

        public bool TryUpdateDomain(int value)
        {
            if (Collapsed)
                return false;
            return Domain.Remove(value);
        }

        public void Bind(BoardCellData data)
        {
            X = data.X;
            Y = data.Y;
            Value = data.Value;
            Collapsed = data.Collapsed;
            Domain = data.Domain.ToList();
        }
        

        private void Collapse(int value)
        {
            Value = value;
            Collapsed = true;
            Domain.Clear();
            
        } 
    }
}