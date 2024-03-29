using System;
using System.Linq;
using System.Collections.Generic;
using AllieJoe.Util;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace AllieJoe.MapGeneration
{
    [CreateAssetMenu(menuName = "Wave/Tile", fileName = "Wave Tile")]
    public class TilesSO : ScriptableObject
    {
        public Vector2 Spacing;
        [TableList(DrawScrollView = true, ShowIndexLabels = true)]
        public List<TileData> Tiles;

        private string folderPath;

        [Button]
        private void LoadTileAssets()
        {
#if UNITY_EDITOR
            List<TileData> tempTiles = new List<TileData>();
            string path = EditorUtility.OpenFolderPanel("Select WaveTile set", folderPath, "");
            if (path.Length != 0)
            {
                folderPath = path;
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    string filePath = "Assets" + file.Replace(Application.dataPath, "");
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);

                    if (sprite == null)
                        continue;

                    tempTiles.Add(new TileData
                    {
                        Sprite = sprite,
                        name = sprite.name
                    });
                }
            }

            if (tempTiles.Count > 0)
            {
                Tiles.Clear();
                Tiles = tempTiles;
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }

    [Serializable]
    public class TileData
    {
        
        [TableColumnWidth(57, Resizable = false)] [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite Sprite;

        [HideInInspector] public string name;

        [VerticalGroup("Constrains")] 
        public Color[] Up = new[] {Color.black};

        [VerticalGroup("Constrains")] 
        public Color[] Down = new[] {Color.black};

        [VerticalGroup("Constrains")] 
        public Color[] Left = new[] {Color.black};

        [VerticalGroup("Constrains")] 
        public Color[] Right = new[] {Color.black};

        [TableColumnWidth(50, Resizable = false)]
        public bool Ignore = false;
        
        public static string ToEdgeRestriction(Color[] restriction)
        {
            return restriction.Aggregate("", (s, color) => 
                $"{s}{IDMapperGenerator.MapValueToIncrementalID(ColorUtility.ToHtmlStringRGB(color))}");
        } 
    }
}