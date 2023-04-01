using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class WaveCollapseRender : MonoBehaviour
    {
        [SerializeField]
        private WaveCellRender _waveCellPrefab;
        private Vector2 _spacing;

        private List<WaveCellRender> _waveCells = new List<WaveCellRender>();
        private WaveCollapseTileProvider _tileProvider;
        public void CreateGrid(WaveCell[] cells, TileRenderData[] tiles, Vector2 spacing)
        {
            Clear();
            
            _spacing = spacing;
            _tileProvider = new WaveCollapseTileProvider(tiles);
            for (int i = 0; i < cells.Length; i++)
            {
                WaveCell cell = cells[i];
                CreateCell(cell.X, cell.Y);
            }
        }

        public void RenderGrid(WaveCell[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                _waveCells[i].Render(cells[i]);
            }
        }
        
        private void CreateCell(int x, int y)
        {
            Vector2 pos = new Vector2(x * _spacing.x, - y * _spacing.y);
            WaveCellRender cell = Instantiate(_waveCellPrefab, pos, Quaternion.identity);
            cell.transform.SetParent(transform);
            cell.Init(_tileProvider, _spacing.x);
            _waveCells.Add(cell);
        }

        public void Clear()
        {
            for (int i = 0; i < _waveCells.Count; i++)
            {
                Destroy(_waveCells[i].gameObject);
            }
            _waveCells.Clear();
        }
    }

    public class WaveCollapseTileProvider
    {
        private TileRenderData[] _tiles;
        private Dictionary<int, TileRenderData> _options = new Dictionary<int, TileRenderData>();
        
        public WaveCollapseTileProvider(TileRenderData[] tiles)
        {
            _tiles = tiles;
            for (int i = 0; i < tiles.Length; i++)
            {
                _options.Add(tiles[i].id, tiles[i]);
            }
        }

        public int TotalTiles => _tiles.Length;

        public TileRenderData GetTileRenderByIndex(int index) => _tiles[index];

        public TileRenderData GetTileRenderById(int id) => _options[id];
    }
}