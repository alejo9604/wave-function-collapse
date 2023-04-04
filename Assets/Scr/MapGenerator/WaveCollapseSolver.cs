using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllieJoe.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace AllieJoe.MapGeneration
{
    public enum LookUpDirection {UP, DOWN, LEFT, RIGHT }
    
    public class WaveCollapseSolver : MonoBehaviour
    {
        enum WaveState {Completed, Invalid, Pending}
        
        [SerializeField]
        private TilesSO _tileData;

        [Space(20)]
        public int Size = 4;
        [SerializeField]
        private WaveCollapseRender _render;

        [Space(20)]
        [SerializeField]
        [Range(0f, 1)]
        private float _speed = 0.15f;
        [SerializeField]
        private bool _randomSeed;
        [SerializeField]
        private int _seed;

        private List<Tile> _tiles;
        private Dictionary<int, Tile> _tilesDic;
        private WaveCell[] _cells;

        private Random _random;
        private Stack<WaveCell> _propagateStackCell;
        private WaveState _state;
        private bool _firstStep = true;
        
        private WaveCell GetCell(int x, int y) => _cells[x * Size + y];
        

        [Button]
        private void Init()
        {
            //Animation - quick handler
            StopAllCoroutines();

            if (_randomSeed)
                _seed = UnityEngine.Random.Range(0, int.MaxValue);
            _random = new Random(_seed);

            IDMapperGenerator.Reset();
            
            SetTiles();
            CreateCells();

            _propagateStackCell = new Stack<WaveCell>();

            TileRenderData[] tileRenderData = _tiles.Select(x => new TileRenderData {id = x.id, Sprite = x.sprite, rotation = x.rotation}).ToArray();
            _render.CreateGrid(Size, _cells, tileRenderData, _tileData.Spacing);
        }

        [Button]
        private void RunAnimation()
        {
            //Animation - quick handler
            StopAllCoroutines();
            StartCoroutine(RunAnimator());
        }
        
        [Button]
        private void SingleStep()
        {
            //Animation - quick handler
            StopAllCoroutines();
            Step();
        }


        private IEnumerator RunAnimator()
        {
            _state = WaveState.Pending;
            while (_state == WaveState.Pending)
            {
                Step();
                yield return new WaitForSeconds(_speed);
            }
        }
        
        private void Step()
        {
            Run();
            _render.RenderGrid(_cells);

            _firstStep = false;
        }
        
        private void SetTiles()
        {
            _tiles = new List<Tile>();
            _tilesDic = new Dictionary<int, Tile>();
            List<TileData> tileData = _tileData.Tiles;
            for (int i = 0; i < tileData.Count; i++)
            {
                if(tileData[i].Ignore)
                    continue;

                Tile tile = new Tile(i, tileData[i]);
                _tiles.Add(tile);
                _tilesDic.Add(i, tile);
            }


            int sourceTileCount = _tiles.Count;
            int id = _tiles[^1].id + 1;
            for (int i = 0; i < sourceTileCount; i++)
            {
                Tile tileToRotate = _tiles[i];
                List<string> prevRotations = new List<string>(4);
                prevRotations.Add(string.Join("", tileToRotate.edges));
                for (int j = 1; j < 4; j++)
                {
                    string[] rotatedTiles = _tiles[i].RotateEdges(j);
                    string directValue = string.Join("", rotatedTiles);
                    if (!prevRotations.Contains(directValue))
                    {
                        prevRotations.Add(directValue);
                        Tile tile = new Tile(id, tileToRotate.name, tileToRotate.sprite, rotatedTiles, 90 * j);
                        _tiles.Add(tile);
                        _tilesDic.Add(id, tile);

                        id++;
                    }
                }
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
                    _cells[i * Size + j] = new WaveCell(i, j, optionsID, _random);
                }
            }
        }
        
        private void Run()
        {
            _state = AllCollapsedOrInvalid();
            if (_state == WaveState.Completed)
            {
                Debug.Log("[WaveCollapseSolver] Completed!");
                return;
            }
            
            if (_state == WaveState.Invalid)
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
            if (_firstStep)
            {
                return _random.Next(0, _cells.Length);
            }
            
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

            return waveCellIndex[_random.Next(0, waveCellIndex.Count)];
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
            
            switch (direction)
            {
                case LookUpDirection.UP:
                    TryUpdateCellOptions(x, y - 1, GetValidOptions(cell, direction));
                    break;
                case LookUpDirection.DOWN:
                    TryUpdateCellOptions(x, y + 1, GetValidOptions(cell, direction));
                    break;
                case LookUpDirection.LEFT:
                    TryUpdateCellOptions(x - 1, y, GetValidOptions(cell, direction));
                    break;
                case LookUpDirection.RIGHT:
                    TryUpdateCellOptions(x + 1, y, GetValidOptions(cell, direction));
                    break;
                
            }
        }

        private List<int> GetValidOptions(WaveCell cell, LookUpDirection direction)
        {
            //If collapsed get the Tile selected
            if (cell.Value >= 0)
            {
                return _tilesDic[cell.Value].GetValidTilesByDirection(direction);
            }
            
            //No collapsed: use all options.
            List<int> validOptions = new List<int>();
            for (int i = 0; i < cell.Options.Count; i++)
            {
                Tile tile = _tilesDic[cell.Options[i]];
                validOptions.AddRange(tile.GetValidTilesByDirection(direction));
            }

            return validOptions;
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

            if (prevEntropy != cell.Entropy)
            {
                _propagateStackCell.Push(cell);
            }
        }
    }
}