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
        private List<SpriteRenderer> _options = new List<SpriteRenderer>();
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
                s.sprite = _tileProvider.GetTileRenderByIndex(i).Sprite;
                _options.Add(s);
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
            _renderer.sprite = cell.Value >= 0 ? _tileProvider.GetTileRenderById(cell.Value).Sprite : null;
            foreach (var o in _options)
            {
                o.gameObject.SetActive(false);
            }
        }
        
        private void RenderNoCollapsed(WaveCell cell)
        {
            _renderer.sprite = null;
            int totalTiles = _tileProvider.TotalTiles;
            for (int i = 0; i < totalTiles; i++)
            {
                bool active = cell.Options.Contains(_tileProvider.GetTileRenderByIndex(i).id);
                _options[i].gameObject.SetActive(active);
            }
        }
    }
}