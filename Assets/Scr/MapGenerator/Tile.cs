using System;
using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.MapGeneration
{
    [Serializable]
    public class Tile
    {
        public const int EDGE_UP = 0;
        public const int EDGE_DOWN = 1;
        public const int EDGE_LEFT = 2;
        public const int EDGE_RIGHT = 3;
        
        public int id;
        public string name;
        public Sprite sprite;
        public string[] edges;
        public List<int> validTilesUp;
        public List<int> validTilesDown;
        public List<int> validTilesLeft;
        public List<int> validTilesRight;

        public Tile(int id, string name, Sprite sprite, string[] edges)
        {
            this.id = id;
            this.name = name;
            this.sprite = sprite;
            this.edges = edges;
        }

        public Tile(int id, TileData tileData)
        {
            this.id = id;
            Bind(tileData);
        }

        public void GenerateValidTiles(List<Tile> tiles)
        {
            validTilesUp = new List<int>();
            validTilesDown = new List<int>();
            validTilesLeft = new List<int>();
            validTilesRight = new List<int>();
            
            foreach (Tile t in tiles)
            {
                if(t.id == id) continue;
                
                if(edges[EDGE_UP] == t.edges[EDGE_DOWN])
                    validTilesUp.Add(t.id);
                
                if(edges[EDGE_DOWN] == t.edges[EDGE_UP])
                    validTilesDown.Add(t.id);
                
                if(edges[EDGE_LEFT] == t.edges[EDGE_RIGHT])
                    validTilesLeft.Add(t.id);
                
                if(edges[EDGE_RIGHT] == t.edges[EDGE_LEFT])
                    validTilesRight.Add(t.id);
            }
        }
        
        private void Bind(TileData tileData)
        {
            name = tileData.name;
            sprite = tileData.Sprite;
            edges = new string[4];
            edges[EDGE_UP] = TileData.ToEdgeRestriction(tileData.Up);
            edges[EDGE_DOWN] = TileData.ToEdgeRestriction(tileData.Down);
            edges[EDGE_LEFT] = TileData.ToEdgeRestriction(tileData.Left);
            edges[EDGE_RIGHT] = TileData.ToEdgeRestriction(tileData.Right);
        }
    }
}