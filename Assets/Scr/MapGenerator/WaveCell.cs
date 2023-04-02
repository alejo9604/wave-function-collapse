using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class WaveCell
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Collapsed { get; private set; }
        public List<int> Options { get; private set; }
        public int Value { get; private set; } = -1;

        public int Entropy { get; private set; } = int.MaxValue;

        public WaveCell(int x, int y, int[] options)
        {
            X = x;
            Y = y;
            Options = new List<int>(options);
            Value = -1;
            Check();
        }
        
        public bool Collapse()
        {
            if (Options.Count == 0)
                return false;

            Value = Options[Random.Range(0, Options.Count)];
            Options.Clear();
            Check();
            return true;
        }
        

        public void UpdateValidOptions(List<int> valid)
        {
            Options = Options.Where(x => valid.Any(y => y == x)).ToList();
            TryForceCollapse();
            Check();
        }

        private void TryForceCollapse()
        {
            if (Options.Count != 1)
                return;
            
            Value = Options[0];
            Options.Clear();
        }
        
        private void Check()
        {
            Entropy = Options.Count;
            Collapsed = Entropy == 0;
        }
    }
}