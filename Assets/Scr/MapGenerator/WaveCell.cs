using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

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

        private Random _random;

        public WaveCell(int x, int y, int[] options, Random random = null)
        {
            X = x;
            Y = y;
            Options = new List<int>(options);
            Value = -1;
            Check();

            _random = random ?? new Random();
        }
        
        public bool Collapse()
        {
            if (Options.Count == 0)
                return false;

            Value = Options[_random.Next(0, Options.Count)];
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