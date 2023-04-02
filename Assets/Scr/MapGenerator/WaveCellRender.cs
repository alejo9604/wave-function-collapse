using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    public class WaveCellRender : MonoBehaviour
    {
        [SerializeField]
        private int _maxOptionsPerRow = 4;
        
        private SpriteRenderer _renderer;
        private Dictionary<int, SpriteRenderer> _options = new Dictionary<int, SpriteRenderer>();
        private WaveCollapseTileProvider _tileProvider;
        

        public void Init(WaveCollapseTileProvider tileProvider, float cellSize)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = null;
            _tileProvider = tileProvider;
            SetGrid(cellSize);
        }
        
        private void SetGrid(float cellSize)
        {
            for (int i = 0; i < _options.Count; i++)
            {
                Destroy(_options[i].gameObject);
            }
            _options.Clear();

            cellSize *= 0.8f;
            
            float innerSize = 0.8f / _maxOptionsPerRow;
            float innerSpacing = cellSize / _maxOptionsPerRow;
            Vector2 initPos = Vector2.zero;
            initPos.x = - cellSize/ 2f + innerSpacing/2f;
            initPos.y = cellSize/ 2f - innerSpacing/2f;
            int currentY = 0;

            int totalTiles = _tileProvider.TotalTiles;
            for (int i = 0; i < totalTiles; i++)
            {
                TileRenderData tileData = _tileProvider.GetTileRenderByIndex(i);
                if(tileData.rotation > 0)
                    continue;
                
                int xSpace = i % _maxOptionsPerRow;
                if (i > 0 && xSpace == 0)
                    currentY++;
                
                Vector2 pos = new Vector2(initPos.x + xSpace * innerSpacing, initPos.y - currentY * innerSpacing);
                GameObject c = new GameObject($"Option_{i}");
                c.transform.SetParent(transform);
                c.transform.localPosition = pos;
                c.transform.localScale = Vector3.one * innerSize;
                SpriteRenderer s = c.AddComponent<SpriteRenderer>();
                s.sortingOrder = 1;
                s.sprite = tileData.Sprite;
                _options.Add(tileData.id, s);
            }
        }
        
        public void Render(WaveCell cell)
        {
            if (cell.Collapsed)
                RenderCollapsed(cell);
            else
                RenderNoCollapsed(cell);
        }

        private void RenderCollapsed(WaveCell cell)
        {
            TileRenderData tileRenderData = cell.Value >= 0 ? _tileProvider.GetTileRenderById(cell.Value) : null;
            _renderer.sprite = tileRenderData?.Sprite;
            _renderer.transform.localEulerAngles = new Vector3(0f, 0f, tileRenderData?.rotation ?? 0);
            foreach (var o in _options.Values)
            {
                o.gameObject.SetActive(false);
            }
        }
        
        private void RenderNoCollapsed(WaveCell cell)
        {
            _renderer.sprite = null;
            _renderer.transform.localEulerAngles = Vector3.zero;
            foreach (int id in _options.Keys)
            {
                _options[id].gameObject.SetActive(cell.Options.Contains(id));
            }
        }
    }
}