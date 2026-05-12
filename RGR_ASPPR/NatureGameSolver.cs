using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4_ASPPR
{
    public class NatureGameSolver
    {
        private static string GetStrategyNames(double[] values, double target)
        {
            var strategies = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                if (Math.Abs(values[i] - target) < 0.001)
                    strategies.Add($"A{i + 1}");
            }
            return string.Join(" або ", strategies);
        }

        internal static void SolveWaldCriterion(Matrix matrix)
        {
            Console.WriteLine("\nКритерiй Вальда:");
            double[] mins = new double[matrix.Rows];
            for (int i = 0; i < matrix.Rows; i++)
            {
                double min = double.MaxValue;
                for (int j = 0; j < matrix.Columns; j++)
                    if (matrix[i, j] < min) min = matrix[i, j];
                mins[i] = min;
                Console.WriteLine($"min в рядку {i + 1}: {min}");
            }

            double maxOfMins = mins.Max();
            Console.WriteLine($"\nМаксимальний елемент: {maxOfMins}");
            Console.WriteLine($"Оптимальнi стратегiї: {GetStrategyNames(mins, maxOfMins)}");
        }

        internal static void SolveOptimismCriterion(Matrix matrix)
        {
            Console.WriteLine("\nКритерiй максимаксу:");
            double[] maxs = new double[matrix.Rows];
            for (int i = 0; i < matrix.Rows; i++)
            {
                double max = double.MinValue;
                for (int j = 0; j < matrix.Columns; j++)
                    if (matrix[i, j] > max) max = matrix[i, j];
                maxs[i] = max;
                Console.WriteLine($"max в рядку {i + 1}: {max}");
            }

            double absoluteMax = maxs.Max();
            Console.WriteLine($"\nМаксимальний елемент: {absoluteMax}");
            Console.WriteLine($"Оптимальнi стратегiї: {GetStrategyNames(maxs, absoluteMax)}");
        }

        internal static void SolveHurwiczCriterion(Matrix matrix)
        {
            Console.WriteLine("\nКритерiй Гурвiца:");
            Console.Write("Коефiцiєнт y = ");
            if (!double.TryParse(Console.ReadLine(), out double y)) y = 0.3;

            double[] mins = new double[matrix.Rows];
            double[] maxs = new double[matrix.Rows];
            for (int i = 0; i < matrix.Rows; i++)
            {
                double rMin = double.MaxValue, rMax = double.MinValue;
                for (int j = 0; j < matrix.Columns; j++)
                {
                    if (matrix[i, j] < rMin) rMin = matrix[i, j];
                    if (matrix[i, j] > rMax) rMax = matrix[i, j];
                }
                mins[i] = rMin; maxs[i] = rMax;
            }

            for (int i = 0; i < matrix.Rows; i++) Console.WriteLine($"min в рядку {i + 1}: {mins[i]}");
            Console.WriteLine();
            for (int i = 0; i < matrix.Rows; i++) Console.WriteLine($"max в рядку {i + 1}: {maxs[i]}");
            Console.WriteLine();

            double[] s = new double[matrix.Rows];
            for (int i = 0; i < matrix.Rows; i++)
            {
                s[i] = y * mins[i] + (1 - y) * maxs[i];
                Console.WriteLine($"s{i + 1} = {y} * {mins[i]} + (1 - {y}) * {maxs[i]} = {s[i]:F1}");
            }

            double maxS = s.Max();
            Console.WriteLine($"\nМаксимальний елемент: {maxS:F1}");
            Console.WriteLine($"Оптимальнi стратегiї: {GetStrategyNames(s, maxS)}");
        }

        internal static void SolveSavageCriterion(Matrix matrix)
        {
            Console.WriteLine("\nКритерiй Севiджа:");
            double[,] R = new double[matrix.Rows, matrix.Columns];
            Console.WriteLine("Матриця ризикiв:");

            for (int j = 0; j < matrix.Columns; j++)
            {
                double maxCol = double.MinValue;
                for (int i = 0; i < matrix.Rows; i++)
                    if (matrix[i, j] > maxCol) maxCol = matrix[i, j];
                for (int i = 0; i < matrix.Rows; i++) R[i, j] = maxCol - matrix[i, j];
            }

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++) Console.Write($"{R[i, j]}  ");
                Console.WriteLine();
            }

            double[] maxRisks = new double[matrix.Rows];
            Console.WriteLine();
            for (int i = 0; i < matrix.Rows; i++)
            {
                double mR = double.MinValue;
                for (int j = 0; j < matrix.Columns; j++) if (R[i, j] > mR) mR = R[i, j];
                maxRisks[i] = mR;
                Console.WriteLine($"max в рядку {i + 1}: {mR}");
            }

            double minOfMax = maxRisks.Min();
            Console.WriteLine($"\nMiнiмальний елемент: {minOfMax}");
            Console.WriteLine($"Оптимальнi стратегiї: {GetStrategyNames(maxRisks, minOfMax)}");
        }

        internal static void SolveBayesCriterion(Matrix matrix)
        {
            Console.WriteLine("\nКритерiй Байєса:");
            int m = matrix.Columns;
            double[] p = new double[m];
            Console.WriteLine($"Введiть {m} ймовiрностей:");
            string[] input = Console.ReadLine().Split(' ');
            for (int j = 0; j < m; j++) p[j] = double.Parse(input[j]);

            string pList = string.Join("; ", p.Select((val, idx) => $"p{idx + 1} = {val}"));
            Console.WriteLine($"\nЙмовiрностi застосування природою своїх стратегiй: {pList};");

            double[] s = new double[matrix.Rows];
            for (int i = 0; i < matrix.Rows; i++)
            {
                string formula = "";
                double val = 0;
                for (int j = 0; j < m; j++)
                {
                    val += matrix[i, j] * p[j];
                    formula += $"{matrix[i, j]} * {p[j]}" + (j == m - 1 ? "" : " + ");
                }
                s[i] = val;
                Console.WriteLine($"s{i + 1} = {formula} = {s[i]:F2}");
            }

            double maxS = s.Max();
            Console.WriteLine($"\nМаксимальний елемент: {maxS:F2}");
            Console.WriteLine($"Оптимальнi стратегiї: {GetStrategyNames(s, maxS)}");
        }

        internal static void SolveLaplaceCriterion(Matrix matrix)
        {
            Console.WriteLine("\nКритерiй Лапласа:");
            double p = 1.0 / matrix.Columns;
            double[] s = new double[matrix.Rows];

            for (int i = 0; i < matrix.Rows; i++)
            {
                string formula = "";
                double val = 0;
                for (int j = 0; j < matrix.Columns; j++)
                {
                    val += matrix[i, j] * p;
                    formula += $"{matrix[i, j]} * {p}" + (j == matrix.Columns - 1 ? "" : " + ");
                }
                s[i] = val;
                Console.WriteLine($"s{i + 1} = {formula} = {s[i]:F2}");
            }

            double maxS = s.Max();
            Console.WriteLine($"\nМаксимальний елемент: {maxS:F2}");
            Console.WriteLine($"Оптимальнi стратегiї: {GetStrategyNames(s, maxS)}");
        }
    }
}
