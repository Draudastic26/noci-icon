using System.Collections.Generic;
using UnityEngine;

namespace drstc.noci
{
    public enum CellState { Dead = 0, Alive = 1, Contour = 2 }

    public class Noci
    {
        /// <summary>
        /// The config defines the appearance of the icon.
        /// </summary>
        public NociConfig Config { get; private set; }

        /// <summary>
        /// Same seed will alway generate the same icons.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Counts the calles Rerolls. 
        /// </summary>
        public int RerollCount { get; private set; }

        private CellState[,] grid;
        private Vector2Int[] contourCoordinates;

        private int halfWidth;

        private Texture2D texture;

        private System.Random random;

        /// <summary>
        /// Noci constructor with a random seed.
        /// </summary>
        /// <param name="nociConfig">Defines the configuration</param>
        public Noci(NociConfig nociConfig) : this(nociConfig, Random.Range(int.MinValue, int.MaxValue))
        { }

        /// <summary>
        /// Noci constructor with seed.
        /// </summary>
        /// <param name="nociConfig">Defines the configuration</param>
        public Noci(NociConfig nociConfig, int seed)
        {
            Seed = seed;
            random = new System.Random(seed);
            RerollCount = 0;
            SetConfig(nociConfig);
        }

        /// <summary>
        /// Creates a sprite with the same content and dimensions as the texture.
        /// </summary>
        public Sprite GetSprite()
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        /// <summary>
        /// Returns the noci texture.
        /// </summary>
        public Texture2D GetTexture2D()
        {
            return texture;
        }

        /// <summary>
        /// Returns the noci texture in a scaleFactor times larger
        /// </summary>
        /// <param name="scaleFactor">The factor to increase the result texture</param>
        public Texture2D GetTexture2D(int scaleFactor)
        {
            if(scaleFactor < 1)
            {
                Debug.Log($"Scale factor can't be lower than 1. Will be set to 2");
                scaleFactor = 2;
            }

            var newTex = new Texture2D(texture.width * scaleFactor, texture.height * scaleFactor);
            newTex.filterMode = FilterMode.Point;

            var colors = GetColors(grid, newTex.width, newTex.height);

            newTex.SetPixels(colors);
            newTex.Apply();
            return newTex;
        }

        /// <summary>
        /// Will create another noci texture based on the given config. All based on the seed passed (or generated) in the constructor.
        /// </summary>
        public void Reroll()
        {
            RerollCount++;
            GenerateGrid();
            GenerateTexture();
        }

        /// <summary>
        /// Will create another noci texture based on the passed config. To access the new texture call GetTexture2d() again.
        /// </summary>
        /// <param name="nociConfig">Defines the configuration</param>
        public void SetConfig(NociConfig newConfig)
        {
            // Just regenerate grid, when dimension or iteration changed?
            var regenerateGrid = Config == null ||
                                 Config.Dimension != newConfig.Dimension ||
                                 Config.Iterations != newConfig.Iterations;

            Config = new NociConfig(newConfig);
            halfWidth = Config.Dimension.x / 2;

            if (regenerateGrid) GenerateGrid();

            GenerateTexture();
        }

        private void GenerateGrid()
        {
            // First create some noise with half x dimension
            grid = CreateHalfGridWithInitNoise();

            for (int i = 0; i < Config.Iterations; i++)
            {
                grid = EvaluateStates();
            }

            //Attention! Unfold changes dimension of grid
            grid = Unfold();

            contourCoordinates = GetContourCoordinates();
        }

        private void GenerateTexture()
        {
            if (Config.DrawContour) grid = SetContour();
            else grid = RemoveContour();

            texture = createTexture2D();
        }

        private CellState[,] CreateHalfGridWithInitNoise()
        {
            var newGrid = new CellState[halfWidth, Config.Dimension.y];

            // Skip index 0 in x and y to leave space for contour. Just skip last index in Y, since the grid is vertically complete and will just unfold horizontally 
            for (var x = 1; x < newGrid.GetLength(0); x++)
            {
                for (var y = 1; y < newGrid.GetLength(1) - 1; y++)
                {
                    // Simple random noise 50:50
                    newGrid[x, y] = (CellState)random.Next(0, 2);
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

        private Color[] GetColors(CellState[,] grid, int targetWidth, int targetHeight)
        {
            var colors = new Color[targetWidth * targetHeight];

            var dividerX = targetWidth / Config.Dimension.x;
            var divideY = targetWidth / Config.Dimension.y;

            for (var x = 0; x < targetWidth; x++)
            {
                for (var y = 0; y < targetHeight; y++)
                {
                    colors[targetWidth * y + x] = GetCellColor(grid[x / dividerX, y / divideY]);
                }
            }
            return colors;
        }

        private Color[] GetColors(CellState[,] grid)
        {
            return GetColors(grid, grid.GetLength(0), grid.GetLength(1));
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

            // Mirror/Unfold
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

        private Vector2Int[] GetContourCoordinates()
        {
            var contours = new List<Vector2Int>();

            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == CellState.Dead)
                    {
                        var neighbours = GetLivingNeighbours(x, y);
                        if (neighbours > 0)
                        {
                            contours.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }

            return contours.ToArray();
        }

        private CellState[,] SetContour()
        {
            var newGrid = grid.Clone() as CellState[,];
            for (var i = 0; i < contourCoordinates.Length; i++)
            {
                newGrid[contourCoordinates[i].x, contourCoordinates[i].y] = CellState.Contour;
            }
            return newGrid;
        }

        private CellState[,] RemoveContour()
        {
            var newGrid = grid.Clone() as CellState[,];

            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == CellState.Contour)
                    {
                        newGrid[x, y] = CellState.Dead;
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

        private Color GetCellColor(CellState state)
        {
            switch (state)
            {
                case CellState.Alive:
                    return Config.CellColor;
                case CellState.Dead:
                    // TODO: Maybe also should be configurable
                    return Color.clear;
                case CellState.Contour:
                    return Config.ContourColor;
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