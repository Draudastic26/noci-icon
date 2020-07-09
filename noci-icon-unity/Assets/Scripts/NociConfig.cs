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
}