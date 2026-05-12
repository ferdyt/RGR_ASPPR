using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4_ASPPR
{
    internal class InverseMatrixCalculator
    {
        public List<Matrix> iterations = new List<Matrix>();

        private void CalculateElements(Matrix matrix, int k)
        {
            int n = matrix.Rows;
            double[,] nextStep = new double[n, n];
            double pivot = matrix[k, k];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == k && j == k)
                    {
                        nextStep[i, j] = 1;
                    }
                    else if (i == k)
                    {
                        nextStep[i, j] = -matrix[i, j];
                    }
                    else if (j == k)
                    {
                        nextStep[i, j] = matrix[i, j];
                    }
                    else
                    {
                        nextStep[i, j] = matrix[i, j] * pivot - matrix[i, k] * matrix[k, j];
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = nextStep[i, j] / pivot;
                }
            }
        }

        public List<Matrix> Eliminate(Matrix matrix)
        {
            int n = matrix.Rows;

            for (int k = 0; k < n; k++)
            {
                if (matrix[k, k] == 0)
                {
                    continue;
                }

                CalculateElements(matrix, k);
                matrix.SwapHeaders(k, k);
                iterations.Add(matrix.Clone());
            }

            return iterations;
        }
    }
}
