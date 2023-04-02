using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class WaveCollapseRender : MonoBehaviour
    {
        [SerializeField]
        private float _cameraBufferExpand = 1;
        
        [Space(20)]
        [SerializeField]
        private WaveCellRender _waveCellPrefab;
        [SerializeField]
        private bool _renderCellsOptions;
        private Vector2 _spacing;

        private List<WaveCellRender> _waveCells = new List<WaveCellRender>();
        private WaveCollapseTileProvider _tileProvider;
        public void CreateGrid(int size, WaveCell[] cells, TileRenderData[] tiles, Vector2 spacing)
        {
            Clear();
            
            _spacing = spacing;
            _tileProvider = new WaveCollapseTileProvider(tiles);
            for (int i = 0; i < cells.Length; i++)
            {
                WaveCell cell = cells[i];
                CreateCell(cell.X, cell.Y);
            }
            
            SetCamera(size);
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
            cell.name = $"Cell ({x}, {y})";
            cell.transform.SetParent(transform);
            cell.Init(_tileProvider, _spacing.x, _renderCellsOptions);
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

        private void SetCamera(int dimension)
        {
            Camera cam = Camera.main;
            
            Vector2 boxSize = _spacing * dimension;
            Vector2 offset = _spacing / 2f;
            offset.x *= -1; //Move backwards
            
            Vector3 center = boxSize / 2f;
            center.y *= -1; //Invert axis
            center += (Vector3) offset;
            center.z = -10;
            
            float vertical = boxSize.y + _cameraBufferExpand;
            float horizontal = (boxSize.x + _cameraBufferExpand) * cam.pixelHeight / cam.pixelWidth;

            float size = Mathf.Max(horizontal, vertical) * 0.5f;

            cam.transform.position = center;
            cam.orthographicSize = size;
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