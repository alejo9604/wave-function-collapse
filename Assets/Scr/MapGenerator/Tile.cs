using System;
using System.Collections.Generic;
using System.Linq;
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
        public float rotation;
        public string[] edges;
        public List<int> validTilesUp;
        public List<int> validTilesDown;
        public List<int> validTilesLeft;
        public List<int> validTilesRight;
        
        public Tile(int id, string name, Sprite sprite, string[] edges, float rotation)
        {
            this.id = id;
            this.name = name;
            this.sprite = sprite;
            this.edges = edges;
            this.rotation = rotation;
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

        public List<int> GetValidTilesByDirection(LookUpDirection direction)
        {
            switch (direction)
            {
                case LookUpDirection.UP:
                    return validTilesUp;
                case LookUpDirection.DOWN:
                    return validTilesDown;
                case LookUpDirection.LEFT:
                    return validTilesLeft;
                case LookUpDirection.RIGHT:
                    return validTilesRight;
            }

            return null;
        }
        
        public string[] RotateEdges(int num) //1 = 90, 2 = 180, 3 = 270
        {
            int len = edges.Length;
            string[] rotatedEdges = new string[len];
            for (int i = 0; i < len; i++)
            {
                rotatedEdges[i] = edges[(i - num + len) % len];
            }

            //TODO: This doesn't work. We need to reverse the internal chunk of value:
            // 'ABC XYZ FGJ' -> 'FGJ XYZ ABC' instead of 'JGF ZYX CBA' 
            if (num == 1 || num == 2)
            {
                rotatedEdges[EDGE_UP] = ReverseEdgeValue(rotatedEdges[EDGE_UP]);
                rotatedEdges[EDGE_DOWN] = ReverseEdgeValue(rotatedEdges[EDGE_DOWN]);
            }

            if (num == 2 || num == 3)
            {
                rotatedEdges[EDGE_LEFT] = ReverseEdgeValue(rotatedEdges[EDGE_LEFT]);
                rotatedEdges[EDGE_RIGHT] = ReverseEdgeValue(rotatedEdges[EDGE_RIGHT]);
            }

            return rotatedEdges;
        }

        private string ReverseEdgeValue(string edgeValue)
        {
            string[] values = Enumerable.Range(0, 3).Select(i => edgeValue.Substring(i * 6, 6)).ToArray();
            Array.Reverse(values);
            return string.Join("", values);
        }

        private void Bind(TileData tileData)
        {
            name = tileData.name;
            sprite = tileData.Sprite;
            rotation = 0;
            edges = new string[4];
            edges[EDGE_UP] = TileData.ToEdgeRestriction(tileData.Up);
            edges[EDGE_DOWN] = TileData.ToEdgeRestriction(tileData.Down);
            edges[EDGE_LEFT] = TileData.ToEdgeRestriction(tileData.Left);
            edges[EDGE_RIGHT] = TileData.ToEdgeRestriction(tileData.Right);
        }
    }
}