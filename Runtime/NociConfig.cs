using UnityEngine;

namespace drstc.noci
{
    public class NociConfig
    {
        public const int MIN_DIMENSION = 6;

        public Vector2Int Dimension
        {
            get { return dimension; }
            set
            {
                if (value.x < MIN_DIMENSION)
                {
                    Debug.Log($"X dimension must be at least {MIN_DIMENSION}");
                    value.x = MIN_DIMENSION;
                }
                if (value.y < MIN_DIMENSION)
                {
                    Debug.Log($"Y dimension must be at least {MIN_DIMENSION}");
                    value.y = MIN_DIMENSION;
                }

                var r = value.x % 2;
                var newValue = value;
                if (r != 0)
                {
                    newValue.x++;
                    Debug.Log($"X dimension was not even, so it is set to {newValue}");
                }

                dimension = newValue;
            }
        }

        public int Iterations { get; set; }
        public bool DrawContour { get; set; }
        public Color CellColor { get; set; }
        public Color ContourColor { get; set; }

        private Vector2Int dimension;

        public NociConfig(Vector2Int dimension, int iterations, bool drawContour)
        {
            Dimension = dimension;
            Iterations = iterations;
            DrawContour = drawContour;
            CellColor = Color.white;
            ContourColor = Color.black;
        }

        public NociConfig(NociConfig copyConfig)
        {
            Dimension = copyConfig.Dimension;
            Iterations = copyConfig.Iterations;
            DrawContour = copyConfig.DrawContour;
            CellColor = copyConfig.CellColor;
            ContourColor = copyConfig.ContourColor;
        }
    }
}