using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class Cell
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Collapsed { get; private set; }
        public List<int> Options { get; private set; }
        public int Value { get; private set; } = -1;

        public int Entropy => Options.Count;

        public Cell(int x, int y, int options)
        {
            X = x;
            Y = y;
            for (int i = 0; i < options; i++)
            {
                Options.Add(i);
            }
            Value = -1;
            Check();
        }
        
        public bool Collapse()
        {
            if (Options.Count == 0)
                return false;

            Value = Options[Random.Range(0, Options.Count)];
            Collapsed = true;
            Options.Clear();
            return true;
        }

        public void Check()
        {
            Collapsed = Options.Count == 0;
        }
    }
}