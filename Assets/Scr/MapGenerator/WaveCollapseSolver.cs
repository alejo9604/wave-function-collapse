using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class WaveCollapseSolver : MonoBehaviour
    {
        [SerializeField]
        private TilesSO _tileData;
        private List<Tile> _tiles;

        [Space(20)]
        public int Size = 4;

        private Cell[] _cells;
        
        private Cell GetCell(int x, int y) => _cells[x * Size + y];
        
        [Button]
        private void Init()
        {
            SetTiles();
            CreateCells();
        }

        [Button]
        private void Step()
        {
            Run();
            //TODO: Render
        }
        
        private void SetTiles()
        {
            _tiles = new List<Tile>();
            List<TileData> tileData = _tileData.Tiles;
            for (int i = 0; i < tileData.Count; i++)
            {
                if(tileData[i].Ignore)
                    continue;
                
                _tiles.Add(new Tile(i, tileData[i]));
            }
            
            for (int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].GenerateValidTiles(_tiles);
            }

        }

        private void CreateCells()
        {
            _cells = new Cell[Size * Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _cells[i * Size + j] = new Cell(i, j, _tiles.Count);
                }
            }
        }
        
        private void Run()
        {
            //TODO: Check if continue
            
            //Collapse random cell
            int lowestEntropy = GetLowestEntropyIndex();
            _cells[lowestEntropy].Collapse();
            
            //TODO: Propagate
        }

        private int GetLowestEntropyIndex()
        {
            int minEntropy = _cells.Min(c => c.Entropy);
            List<Cell> lowestEntropy = _cells.Where(c => c.Entropy == minEntropy).ToList();
            Cell randomCell = lowestEntropy[Random.Range(0, lowestEntropy.Count)];
            return randomCell.X + Size * randomCell.Y;
        }

    }
}