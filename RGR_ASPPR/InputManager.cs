using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4_ASPPR
{
    internal static class InputManager
    {
        public static Matrix AddZeroRowHeaders(Matrix matrix, int row)
        {
            string[] newRowHeaders = new string[matrix.Rows + 1];
            for (int i = 0; i < row; i++)
            {
                newRowHeaders[i] = matrix.RowHeaders[i];
            }
            newRowHeaders[row] = "0";
            for (int i = row; i < matrix.Rows; i++)
            {
                newRowHeaders[i + 1] = matrix.RowHeaders[i];
            }
            matrix.RowHeaders = newRowHeaders;
            return matrix;
        }

        public static Matrix InputMatrix()
        {
            int rows, cols;
            double[,] data;

            while (true)
            {
                try
                {
                    Console.Write("Введiть кiлькiсть рядкiв: ");
                    rows = int.Parse(Console.ReadLine());
                    Console.Write("Введiть кiлькiсть стовпцiв: ");
                    cols = int.Parse(Console.ReadLine());
                    data = new double[rows, cols];
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Помилка! Введено некоректне число. Спробуйте ще раз.");
                    continue;
                }
            }

            Console.WriteLine($"Введiть рядок матрицi:");
            for (int i = 0; i < rows; i++)
            {
                while (true)
                {
                    Console.Write($"Рядок {i + 1}: ");
                    string input = Console.ReadLine();
                    string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != cols)
                    {
                        Console.WriteLine($"Помилка! Ви ввели {parts.Length} чисел, а потрiбно {cols}. Спробуйте ще раз.");
                        continue;
                    }
                    try
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            data[i, j] = double.Parse(parts[j].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Помилка! Введено некоректне число. Спробуйте ще раз.");
                    }
                }
            }
            Matrix matrix = new Matrix(data);

            return matrix;
        }

        public static int InputCountOfCriteria()
        {
            int criteriaNum;
            Console.Write("Введiть кiлькiсть критерiв: ");
            while (true)
            {
                try
                {
                    criteriaNum = int.Parse(Console.ReadLine());
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Помилка! Введено некоректне число. Спробуйте ще раз.");
                    continue;
                }
            }
            return criteriaNum;
        }

        static public double[] InputZRow(int count)
        {
            count--; // Зменшуємо на 1, оскільки перший елемент буде "0"

            while (true)
            {
                Console.WriteLine($"Введiть {count} чисел для рядка Z через пробiл:");
                string input = Console.ReadLine();

                // Розділяємо рядок за пробілами, видаляючи зайві пробіли між числами
                string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != count)
                {
                    Console.WriteLine($"Помилка! Очiкувалося {count} чисел, але ви ввели {parts.Length}. Спробуйте ще раз.");
                    continue;
                }

                double[] zRow = new double[count + 1];
                zRow[count] = 0;
                bool success = true;

                for (int i = 0; i < count; i++)
                {
                    // Намагаємося розпарсити кожне число
                    if (!double.TryParse(parts[i].Replace(',', '.'),
                        System.Globalization.CultureInfo.InvariantCulture, out zRow[i]))
                    {
                        Console.WriteLine($"Помилка! '{parts[i]}' не є коректним числом.");
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    return zRow;
                }
            }
        }

        public static double[] InputConstants()
        {
            int constNum;
            double[] constants;

            Console.Write("Введiть кiлькiсть констант: ");
            while (true)
            {
                try
                {
                    constNum = int.Parse(Console.ReadLine());
                    constants = new double[constNum];
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Помилка! Введено некоректне число. Спробуйте ще раз.");
                    continue;
                }
            }
            for (int i = 0; i < constNum; i++)
            {
                while (true)
                {
                    Console.Write($"Константа {i + 1}: ");
                    string input = Console.ReadLine();
                    try
                    {
                        constants[i] = double.Parse(input.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Помилка! Введено некоректне число. Спробуйте ще раз.");
                    }
                }
            }

            return constants;
        }
    }
}
