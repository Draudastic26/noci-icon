using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace drstc.nociincon
{
    public class NociFactory
    {
        public int dimension;

        public int iterations;

        public NociFactory()
        {
            // ensure even dimension 
            this.dimension = 8;
            this.iterations = 10;
        }

        public Sprite GetSprite()
        {
            var halfDim = dimension / 2;

            // First create some noise
            var grid = GetInitNoise();

            for (int i = 0; i < iterations; i++)
            {
                EvaluateStates(grid);
            }
            var tex = new Texture2D(dimension, dimension);
            tex.filterMode = FilterMode.Point;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    tex.SetPixel(i, j, grid[i, j] == 0 ? Color.black : Color.white);
                }
            }

            tex.Apply();

            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        private int[,] GetInitNoise()
        {
            var halfDim = dimension / 2;
            var grid = new int[halfDim, dimension];

            // Simple random noise 50:50
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = Random.Range(0, 2);
                }
            }

            return grid;
        }

        private void EvaluateStates(int[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var neighbours = GetLivingNeighbours(grid, i, j);

                    // living cell
                    if (TryGet(grid, i, j) == 1)
                    {
                        if (neighbours < 2 || neighbours > 3)
                        {
                            grid[i, j] = 0;
                        }
                    }
                    // dead cell                    
                    else
                    {
                        if (neighbours <= 1)
                        {
                            grid[i, j] = 1;
                        }
                    }
                }
            }
        }

        private int GetLivingNeighbours(int[,] grid, int x, int y)
        {
            var aliveCount = 0;

            aliveCount += TryGet(grid, x - 1, y);
            aliveCount += TryGet(grid, x, y + 1);
            aliveCount += TryGet(grid, x + 1, y + 1);
            aliveCount += TryGet(grid, x, y - 1);

            return aliveCount;
        }

        private int TryGet(int[,] grid, int x, int y)
        {
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return 0;
            else
                return grid[x, y];
        }
    }
}