using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class WaveCollapseSolver : MonoBehaviour
    {
        enum LookUpDirection {UP, DOWN, LEFT, RIGHT }
        enum WaveState {Completed, Invalid, Pending}
        
        [SerializeField]
        private TilesSO _tileData;

        [Space(20)]
        public int Size = 4;
        [SerializeField]
        private WaveCollapseRender _render;

        private List<Tile> _tiles;
        private WaveCell[] _cells;
        private Stack<WaveCell> _propagateStackCell;
        
        private WaveCell GetCell(int x, int y) => _cells[x * Size + y];
        
        [Button]
        private void Init()
        {
            SetTiles();
            CreateCells();

            _propagateStackCell = new Stack<WaveCell>();

            TileRenderData[] tileRenderData = _tiles.Select(x => new TileRenderData {id = x.id, Sprite = x.sprite}).ToArray();
            _render.CreateGrid(_cells, tileRenderData, _tileData.Spacing);
        }

        [Button]
        private void Step()
        {
            Run();
            _render.RenderGrid(_cells);
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
            _cells = new WaveCell[Size * Size];
            int[] optionsID = _tiles.Select(x => x.id).ToArray();
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _cells[i * Size + j] = new WaveCell(i, j, optionsID);
                }
            }
        }
        
        private void Run()
        {
            WaveState state = AllCollapsedOrInvalid();
            if (state == WaveState.Completed)
            {
                Debug.Log("[WaveCollapseSolver] Completed!");
                return;
            }
            
            if (state == WaveState.Invalid)
            {
                Debug.Log("[WaveCollapseSolver] Invalid!");
                return;
            }

            int lowestEntropy = GetLowestEntropyIndex();
            
            WaveCell cell = _cells[lowestEntropy]; 
            cell.Collapse();

            Debug.Log($"[WaveCollapseSolver] Collapsed value: {cell.Value}");
            
            PropagateCollapsed(cell);
        }

        private WaveState AllCollapsedOrInvalid()
        {
            bool isAllCollapsed = true;
            foreach (WaveCell cell in _cells)
            {
                if (cell.Collapsed && cell.Value < 0)
                    return WaveState.Invalid;
                
                if (cell.Entropy > 0)
                    isAllCollapsed = false;
            }

            return isAllCollapsed ? WaveState.Completed : WaveState.Pending;
        }

        private int GetLowestEntropyIndex()
        {
            int minEntropy = int.MaxValue;
            List<int> waveCellIndex = new List<int>();
            for (int i = 0; i < _cells.Length; i++)
            {
                WaveCell cell = _cells[i];
                if(cell.Collapsed)
                    continue;
                
                if (cell.Entropy == minEntropy)
                {
                    waveCellIndex.Add(i);
                    continue;
                }
                
                if (cell.Entropy < minEntropy)
                {
                    waveCellIndex.Clear();
                    waveCellIndex.Add(i);
                    minEntropy = cell.Entropy;
                }
            }

            return waveCellIndex[Random.Range(0, waveCellIndex.Count)];
        }

        private void PropagateCollapsed(WaveCell collapsedCell)
        {
            _propagateStackCell.Push(collapsedCell);
            PropagateStack();
        }

        private void PropagateStack()
        {
            while (_propagateStackCell.Count > 0)
            {
                WaveCell cell = _propagateStackCell.Pop();
                
                CellLookUpAdjacent(cell, LookUpDirection.UP);
                CellLookUpAdjacent(cell, LookUpDirection.DOWN);
                CellLookUpAdjacent(cell, LookUpDirection.LEFT);
                CellLookUpAdjacent(cell, LookUpDirection.RIGHT);
            }
        }

        private void CellLookUpAdjacent(WaveCell cell, LookUpDirection direction)
        {
            int x = cell.X;
            int y = cell.Y;

            Tile selectedTile = _tiles.FirstOrDefault(tile => tile.id == cell.Value);
            
            switch (direction)
            {
                case LookUpDirection.UP:
                    TryUpdateCellOptions(x, y - 1, selectedTile.validTilesUp);
                    break;
                case LookUpDirection.DOWN:
                    TryUpdateCellOptions(x, y + 1, selectedTile.validTilesDown);
                    break;
                case LookUpDirection.LEFT:
                    TryUpdateCellOptions(x - 1, y, selectedTile.validTilesLeft);
                    break;
                case LookUpDirection.RIGHT:
                    TryUpdateCellOptions(x + 1, y, selectedTile.validTilesRight);
                    break;
                
            }
        }

        private void TryUpdateCellOptions(int x, int y, List<int> valid)
        {
            if(x < 0 || x >= Size || y < 0 || y >= Size)
                return;

            WaveCell cell = GetCell(x, y);
            
            if(cell.Collapsed)
                return;
            
            int prevEntropy = cell.Entropy;
            cell.UpdateValidOptions(valid);

            // if (prevEntropy != cell.Entropy)
            // {
            //     _propagateStackCell.Push(cell);
            // }
        }
    }
}