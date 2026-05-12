using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4_ASPPR
{
    internal class RankCalculator
    {
        private void CalculateElements(Matrix matrix, int r, int s)
        {
            int rows = matrix.Rows;
            int cols = matrix.Columns;
            double pivot = matrix[r, s];
            double[,] nextStep = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == r && j == s)
                    {
                        nextStep[i, j] = 1;
                    }
                    else if (i == r)
                    {
                        nextStep[i, j] = -matrix[i, j];
                    }
                    else if (j == s)
                    {
                        nextStep[i, j] = matrix[i, j];
                    }
                    else
                    {
                        nextStep[i, j] = matrix[i, j] * pivot - matrix[i, s] * matrix[r, j];
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = nextStep[i, j] / pivot;
                }
            }
        }

        private int FindNonZeroRow(Matrix matrix, int k)
        {
            int rows = matrix.Rows;
            int bestRow = -1;
            double maxVal = 0;

            for (int i = k; i < rows; i++)
            {
                double currentVal = Math.Abs(matrix[i, k]);

                if (currentVal > maxVal)
                {
                    maxVal = currentVal;
                    bestRow = i;
                }
            }

            if (maxVal == 0) return -1;

            return bestRow;
        }

        private void SwapRows(Matrix matrix, int row1, int row2)
        {
            if (row1 == row2) return;

            int cols = matrix.Columns;

            for (int j = 0; j < cols; j++)
            {
                double temp = matrix[row1, j];
                matrix[row1, j] = matrix[row2, j];
                matrix[row2, j] = temp;
            }
        }

        public List<Matrix> CalculateRank(Matrix matrix)
        {
            int rows = matrix.Rows;
            int cols = matrix.Columns;

            int minIterations = Math.Min(rows, cols);
            List<Matrix> iterationsList = new List<Matrix>();

            for (int k = 0; k < minIterations; k++)
            {
                if (matrix[k, k] == 0)
                {
                    int swapRow = FindNonZeroRow(matrix, k);

                    if (swapRow != -1)
                    {
                        SwapRows(matrix, k, swapRow);
                    }
                    else
                    {
                        continue;
                    }
                }

                CalculateElements(matrix, k, k);

                iterationsList.Add(matrix.Clone());
                matrix.Rank++;
            }

            return iterationsList;
        }
    }
}
