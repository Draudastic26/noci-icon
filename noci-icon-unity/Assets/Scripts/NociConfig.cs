using UnityEngine;

namespace drstc.nociincon
{
    public class NociConfig
    {
        public Vector2Int Dimension
        {
            get { return dimension; }
            set
            {
                var r = value.x % 2;
                var newValue = value;
                if (r != 0)
                {
                    newValue.x++;
                    Debug.Log($"Dimension.X value is not even and set to {newValue}");
                }
                dimension = newValue;
            }
        }

        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        private Vector2Int dimension;
        private int iterations;

        public NociConfig(Vector2Int dimension, int iterations) => (Dimension, Iterations) = (dimension, iterations);
    }
}