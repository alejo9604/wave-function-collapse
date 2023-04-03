using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AllieJoe.MapGeneration.Tool
{
    public class WaveTileGenerator : MonoBehaviour
    {

        [SerializeField] 
        private List<Color> _options = new List<Color>();
        [SerializeField] 
        private Image _optionPrefab;
        [SerializeField] 
        private Image _imagePrefab;
        [SerializeField] 
        private int amount;

        [Space(20)]
        [SerializeField]
        private TilesSO _tilesSo;

        [Space(20)] 
        [SerializeField] 
        private Image _tileImage;

        [Space(20)] 
        [SerializeField] 
        private List<Sprite> _sprites = new List<Sprite>();
        [SerializeField]
        private List<WaveTileGeneratorConstrain> _tileConstrains;
        private int _selectedSprite = 0;

        private string folderPath;

        private void Start()
        {
            foreach (var t in _tileConstrains)
            {
                t.CreateColorImages(amount, _imagePrefab);
            }
            RenderOptions();
            RenderSelectedSprite();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Prev();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Next();
        }

        private void Next()
        {
            SetTileConstrains();
            _selectedSprite = (_selectedSprite + 1) % _sprites.Count;
            RenderSelectedSprite();
        }

        private void Prev()
        {
            SetTileConstrains();
            _selectedSprite = (_selectedSprite - 1) % _sprites.Count;
            RenderSelectedSprite();
        }

        private void RenderSelectedSprite()
        {
            _tileImage.sprite = _sprites[_selectedSprite];
            TileData tileData = _tilesSo.Tiles[_selectedSprite];
            
            _tileConstrains[0].SetColor(tileData.Up);
            _tileConstrains[1].SetColor(tileData.Right);
            _tileConstrains[2].SetColor(tileData.Down);
            _tileConstrains[3].SetColor(tileData.Left);
        }

        private void SetTileConstrains()
        {
            TileData tileData = _tilesSo.Tiles[_selectedSprite];
            
            tileData.Up = _tileConstrains[0].Color;
            tileData.Right = _tileConstrains[1].Color;
            tileData.Down = _tileConstrains[2].Color;
            tileData.Left = _tileConstrains[3].Color;
        }

        private void RenderOptions()
        {
            for (int i = 0; i < _tileConstrains.Count; i++)
            {
                _tileConstrains[i].SetOptions(_options, _optionPrefab);
            }
        }
        
        [Button]
        private void ResetTileSO()
        {
            _tilesSo.Tiles.Clear();
            for (int i = 0; i < _sprites.Count; i++)
            {
                _tilesSo.Tiles.Add(new TileData
                {
                    name = _sprites[i].name,
                    Sprite = _sprites[i],
                    Up = new Color[amount],
                    Right = new Color[amount],
                    Down = new Color[amount],
                    Left = new Color[amount]
                });
            }
        }
        
#if UNITY_EDITOR
        [Button]
        private void LoadTiles()
        {
            string path = EditorUtility.OpenFolderPanel("Select WaveTile set", folderPath, "");
            if (path.Length != 0)
            {
                string[] files = Directory.GetFiles(path);
                _sprites.Clear();

                foreach (var file in files)
                {
                    string filePath = "Assets" + file.Replace(Application.dataPath, "");
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);

                    if (sprite == null)
                        continue;

                    _sprites.Add(sprite);
                }
            }
        }
#endif
    }
}