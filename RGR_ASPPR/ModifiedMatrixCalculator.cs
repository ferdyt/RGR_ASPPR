using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4_ASPPR
{
    internal class ModifiedMatrixCalculator
    {
        public Matrix Calculate(Matrix matrix, int r, int s)
        {
            int rows = matrix.Rows;
            int cols = matrix.Columns;
            double[,] newMatrix = new double[rows, cols];
            double pivot = matrix[r, s];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == r && j == s)
                    { 
                        newMatrix[i, j] = 1;
                    }
                    else if (j == s)
                    {
                        newMatrix[i, j] = -matrix[i, j];
                    }
                    else if (i == r)
                    {
                        newMatrix[i, j] = matrix[i, j];
                    }
                    else
                    {
                        newMatrix[i, j] = matrix[i, j] * pivot - matrix[i, s] * matrix[r, j];
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double val = newMatrix[i, j] / pivot;
                    // Zero-out tiny rounding errors caused by IEEE-754 arithmetic
                    if (System.Math.Abs(val) < 1e-12)
                    {
                        val = 0.0;
                    }
                    matrix[i, j] = val;
                }
            }

            return matrix;
        }
    }
}
