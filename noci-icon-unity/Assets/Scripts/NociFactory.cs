using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace drstc.nociincon
{
    public enum CellState { Dead = 0, Alive = 1, Contour = 2 }

    public class NociFactory
    {
        public NociConfig Config { get; private set; }

        private CellState[,] grid;

        private int halfWidth;

        private Texture2D texture;

        public NociFactory(NociConfig nociConfig)
        {
            Config = nociConfig;

            halfWidth = Config.Dimension.x / 2;

            // First create some noise with half x dimension
            grid = CreateHalfGridWithInitNoise();

            for (int i = 0; i < Config.Iterations; i++)
            {
                grid = EvaluateStates();
            }

            //Attention! Changes dimension of grid
            grid = Unfold();
            grid = SetContour();

            texture = createTexture2D();
        }

        public Sprite GetSprite()
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        public Texture2D GetTexture2D()
        {
            return texture;
        }

        private CellState[,] CreateHalfGridWithInitNoise()
        {
            var newGrid = new CellState[halfWidth, Config.Dimension.y];

            // Skip index 0 in x and y to leave space for contour. Just skip last index in Y
            for (var x = 1; x < newGrid.GetLength(0); x++)
            {
                for (var y = 1; y < newGrid.GetLength(1) - 1; y++)
                {
                    // Simple random noise 50:50
                    newGrid[x, y] = (CellState)Random.Range(0, 2);
                }
            }

            return newGrid;
        }

        private CellState[,] EvaluateStates()
        {
            var newGrid = grid.Clone() as CellState[,];

            for (var x = 1; x < grid.GetLength(0); x++)
            {
                for (var y = 1; y < grid.GetLength(1) - 1; y++)
                {
                    var neighbours = GetLivingNeighbours(x, y);

                    // living cell
                    if (TryGet(x, y) == CellState.Alive)
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
                    colors[width * y + x] = GetPixelColor(grid[x, y]);
                }
            }
            return colors;
        }

        private CellState[,] Unfold()
        {
            var newGrid = new CellState[Config.Dimension.x, Config.Dimension.y];

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

        private CellState[,] SetContour()
        {
            var newGrid = grid.Clone() as CellState[,];

            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == CellState.Dead)
                    {
                        var neighbours = GetLivingNeighbours(x, y);
                        if (neighbours > 0)
                        {
                            newGrid[x, y] = CellState.Contour;
                        }
                    }
                }
            }
            return newGrid;
        }

        private int GetLivingNeighbours(int x, int y)
        {
            var aliveCount = 0;
            aliveCount += (int)TryGet(x - 1, y);
            aliveCount += (int)TryGet(x, y + 1);
            aliveCount += (int)TryGet(x + 1, y);
            aliveCount += (int)TryGet(x, y - 1);
            return aliveCount;
        }

        private CellState TryGet(int x, int y)
        {
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return 0;
            else
                return grid[x, y];
        }

        private static Color GetPixelColor(CellState state)
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

        private Texture2D createTexture2D()
        {
            var newTex = new Texture2D(Config.Dimension.x, Config.Dimension.y);
            newTex.filterMode = FilterMode.Point;

            var colors = GetColors(grid);

            newTex.SetPixels(colors);
            newTex.Apply();
            return newTex;
        }
    }
}