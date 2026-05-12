using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Lab_4_ASPPR
{
    internal class SystemCalculator
    {
        public double[] Calculate(Matrix matrix, double[] constants)
        {
            double[] solution = new double[matrix.Rows];

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    solution[i] += Math.Round(matrix.Clone()[i, j] * constants[j], 2);
                }
            }

            for (int i = 0; i < solution.Length; i++)
            {
                solution[i] = Math.Round(solution[i], 2);
            }

            return solution;
        }
    }
}
