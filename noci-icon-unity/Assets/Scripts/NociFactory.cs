using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace drstc.nociincon
{
    public enum CellState { Dead = 0, Alive = 1, Contour = 2 }

    public class NociConfig
    {
        public int Dimension
        {
            get { return dimension; }
            set
            {
                var r = value % 2;
                var newValue = value;
                if (r != 0)
                {
                    newValue++;
                    Debug.Log($"Dimension is not even and set to {newValue}");
                }
                dimension = newValue;
            }
        }

        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        private int dimension;
        private int iterations;

        public NociConfig(int dimension, int iterations) => (Dimension, Iterations) = (dimension, iterations);
    }

    public static class NociFactory
    {
        private static NociConfig config;

        public static Sprite GetSprite(NociConfig nociConfig)
        {
            config = nociConfig;
            var halfDim = config.Dimension / 2;

            // First create some noise
            var grid = GetInitNoise();

            for (int i = 0; i < config.Iterations; i++)
            {
                grid = EvaluateStates(grid);
            }


            grid = GridFlip(grid);

            // DebugGrid(grid);

            grid = SetContour(grid);

            // DebugGrid(grid);


            var tex = createTexture2D(grid);
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        private static Texture2D createTexture2D(CellState[,] grid)
        {
            var tex = new Texture2D(config.Dimension, config.Dimension);
            tex.filterMode = FilterMode.Point;

            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    tex.SetPixel(x, y, GetColor(grid[x, y]));
                }
            }
            tex.Apply();
            return tex;
        }

        private static CellState[,] GridFlip(CellState[,] grid)
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

        private static CellState[,] GetInitNoise()
        {
            var halfDim = config.Dimension / 2;
            var grid = new CellState[halfDim, config.Dimension];

            // Simple random noise 50:50
            for (var i = 1; i < grid.GetLength(0); i++)
            {
                for (var j = 1; j < grid.GetLength(1) - 1; j++)
                {
                    grid[i, j] = (CellState)Random.Range(0, 2);
                }
            }

            return grid;
        }

        private static CellState[,] EvaluateStates(CellState[,] grid)
        {
            var newGrid = grid.Clone() as CellState[,];

            for (var i = 1; i < grid.GetLength(0) - 1; i++)
            {
                for (var j = 1; j < grid.GetLength(1) - 1; j++)
                {
                    var neighbours = GetLivingNeighbours(grid, i, j);

                    // living cell
                    if (TryGet(grid, i, j) == CellState.Alive)
                    {
                        if (neighbours < 2 || neighbours > 3)
                        {
                            newGrid[i, j] = CellState.Dead;
                        }
                    }
                    // dead cell                    
                    else
                    {
                        if (neighbours <= 1)
                        {
                            newGrid[i, j] = CellState.Alive;
                        }
                    }
                }
            }
            return newGrid;
        }

        private static CellState[,] SetContour(CellState[,] grid)
        {
            var newGrid = grid.Clone() as CellState[,];

            var path = "Assets/Sprites/NOCI_DEBUG/dgb";

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
            aliveCount += (int)TryGet(grid, x + 1, y + 1);
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

        private static void DebugGrid(CellState[,] grid)
        {
            var logArray = "";

            for (var y = 0; y < grid.GetLength(1); y++)
            {
                for (var x = 0; x < grid.GetLength(0); x++)
                {
                    logArray += (int)grid[x, y] + "\t";
                }
                logArray += "\n";
            }

            Debug.Log($"{logArray}");
        }

        public static void SaveTextureAsPNG(Texture2D texture, string fullPath)
        {
            byte[] _bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, _bytes);
            //Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + fullPath);
        }
    }
}