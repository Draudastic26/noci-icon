using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace drstc.nociincon
{
    public static class NociFactory
    {
        private static NociConfig config;

        public static Sprite GetSprite(NociConfig nociConfig)
        {
            config = nociConfig;

            // First create some noise with half x dimension
            var grid = GetGridWithInitNoise();

            for (int i = 0; i < config.Iterations; i++)
            {
                grid = EvaluateStates(grid);
            }

            grid = Unfold(grid);
            grid = SetContour(grid);

            var tex = createTexture2D(grid);
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        private static Texture2D createTexture2D(CellState[,] grid)
        {
            var tex = new Texture2D(config.Dimension, config.Dimension);
            tex.filterMode = FilterMode.Point;

            var colors = GetColors(grid);

            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        private static Color[] GetColors(CellState[,] grid)
        {
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);
            var colors = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    //tex.SetPixel(x, y, GetColor(grid[x, y]));
                    colors[width * y + x] = GetColor(grid[x, y]);
                }
            }
            return colors;
        }

        private static CellState[,] Unfold(CellState[,] grid)
        {
            var newGrid = new CellState[config.Dimension, config.Dimension];

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    newGrid[i, j] = grid[i, j];
                }
            }

            //Mirror
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                var newGridIndex = i + grid.GetLength(0);
                var gridIndex = grid.GetLength(0) - i - 1;
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    newGrid[newGridIndex, j] = grid[gridIndex, j];
                }
            }
            return newGrid;
        }

        private static CellState[,] GetGridWithInitNoise()
        {
            var halfDim = config.Dimension / 2;
            var grid = new CellState[halfDim, config.Dimension];

            var width = grid.GetLength(0);
            var height = grid.GetLength(1);

            // Simple random noise 50:50
            for (var x = 1; x < grid.GetLength(0); x++)
            {
                for (var y = 1; y < grid.GetLength(1) - 1; y++)
                {
                    grid[x, y] = (CellState)Random.Range(0, 2);
                }
            }

            return grid;
        }

        private static CellState[,] EvaluateStates(CellState[,] grid)
        {
            var newGrid = grid.Clone() as CellState[,];

            for (var x = 1; x < grid.GetLength(0); x++)
            {
                for (var y = 1; y < grid.GetLength(1) - 1; y++)
                {
                    var neighbours = GetLivingNeighbours(grid, x, y);

                    // living cell
                    if (TryGet(grid, x, y) == CellState.Alive)
                    {
                        if (neighbours < 2 || neighbours > 3)
                        {
                            newGrid[x, y] = CellState.Dead;
                        }
                    }
                    // dead cell                    
                    else
                    {
                        if (neighbours <= 1)
                        {
                            newGrid[x, y] = CellState.Alive;
                        }
                    }
                }
            }
            return newGrid;
        }

        private static CellState[,] SetContour(CellState[,] grid)
        {
            var newGrid = grid.Clone() as CellState[,];

            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == CellState.Dead)
                    {
                        var neighbours = GetLivingNeighbours(grid, x, y);
                        if (neighbours > 0)
                        {
                            newGrid[x, y] = CellState.Contour;
                        }
                    }
                }
            }
            return newGrid;
        }

        private static int GetLivingNeighbours(CellState[,] grid, int x, int y)
        {
            var aliveCount = 0;
            aliveCount += (int)TryGet(grid, x - 1, y);
            aliveCount += (int)TryGet(grid, x, y + 1);
            aliveCount += (int)TryGet(grid, x + 1, y);
            aliveCount += (int)TryGet(grid, x, y - 1);
            return aliveCount;
        }

        private static CellState TryGet(CellState[,] grid, int x, int y)
        {
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return 0;
            else
                return grid[x, y];
        }

        private static Color GetColor(CellState state)
        {
            switch (state)
            {
                case CellState.Alive:
                    return Color.white;
                case CellState.Dead:
                    return Color.clear;
                case CellState.Contour:
                    return Color.black;
                default:
                    return Color.magenta;
            }
        }
    }
}