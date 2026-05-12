using System;
using System.Collections.Generic;
using System.Text;

namespace Lab_4_ASPPR
{
    internal class Matrix
    {
        private readonly double[,] _data;

        public int Rows { get; }
        public int Columns { get; }
        public int Rank { get; set; }
        public int SRowIndex { get; set; }

        public string[] RowHeaders { get; set; }
        public string[] ColumnHeaders { get; set; }

        public string[] DualRowHeaders { get; set; }
        public string[] DualColumnHeaders { get; set; }

        public double AdjustmentNumber { get; set; } = 0;

        public List<string[,]> OperationsULog { get; set; } = new List<string[,]>();

        public double GetMaxZ { get => this[this.Rows - 1, this.Columns - 1]; }
        public double GetMinW { get => this[this.Rows - 1, this.Columns - 1]; }

        public Matrix(double[,] matrix)
        {
            _data = matrix;
            Rows = matrix.GetLength(0);
            Columns = matrix.GetLength(1);
        }

        public double this[int row, int col]
        {
            get => _data[row, col];
            set => _data[row, col] = value;
        }

        public Matrix Clone()
        {
            int rows = Rows;
            int cols = Columns;
            double[,] newData = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    newData[i, j] = this[i, j];

            Matrix newMatrix = new Matrix(newData);

            newMatrix.Rank = this.Rank;

            if (this.RowHeaders != null)
            {
                newMatrix.RowHeaders = (string[])this.RowHeaders.Clone();
            }

            if (this.ColumnHeaders != null)
            {
                newMatrix.ColumnHeaders = (string[])this.ColumnHeaders.Clone();
            }

            return newMatrix;
        }

        public string GetFinalUValue()
        {
            // 1. Словник для збереження поточних значень усіх змінних (u, v, x, y)
            Dictionary<string, double> currentValues = new Dictionary<string, double>();

            // Додаємо значення з рядка Z поточної (фінальної) матриці
            // Це вільні змінні, які стоять у "шапці"
            for (int j = 0; j < this.Columns - 1; j++)
            {
                string header = this.DualColumnHeaders[j];
                currentValues[header] = this[this.Rows - 1, j];
            }

            // Додаємо 0 для базисних змінних (ті, що зліва у фінальній таблиці)
            for (int i = 0; i < this.Rows - 1; i++)
            {
                string header = this.DualRowHeaders[i];
                currentValues[header] = 0.0;
            }

            // 2. Зворотна підстановка по логу (від останньої операції до першої)
            for (int i = OperationsULog.Count - 1; i >= 0; i--)
            {
                string[,] logEntry = OperationsULog[i];
                int logRows = logEntry.GetLength(0); // Кількість рядків у лозі

                string targetVar = logEntry[0, 0];   // Змінна, яку ми виражаємо (напр. u3)
                double constantTerm = double.Parse(logEntry[logRows - 1, 1]); // Останній рядок - константа

                double calculatedValue = constantTerm;

                // Проходимо по всіх проміжних коефіцієнтах у лозі
                for (int r = 1; r < logRows - 1; r++)
                {
                    string varInFormula = logEntry[r, 0];
                    double coefficient = double.Parse(logEntry[r, 1]);

                    if (currentValues.ContainsKey(varInFormula))
                    {
                        calculatedValue += coefficient * currentValues[varInFormula];
                    }
                    // Якщо змінної немає в словнику, вона вважається базисною (0), 
                    // тому додавати нічого не треба.
                }

                // Зберігаємо (або оновлюємо) розраховане значення
                currentValues[targetVar] = calculatedValue;
            }

            // 3. Формуємо результуючий рядок U = (u1, u2, ..., un)
            // Шукаємо всі змінні, що починаються на 'u', та сортуємо їх за індексом
            var uKeys = currentValues.Keys
                .Where(k => k.StartsWith("u"))
                .OrderBy(k => int.Parse(k.Substring(1)))
                .Select(k => currentValues[k].ToString("F2"));

            return "U = (" + string.Join("; ", uKeys) + ")";
        }

        public double[] GetUValue()
        {
            // 1. Словник для збереження поточних значень усіх змінних (u, v, x, y)
            Dictionary<string, double> currentValues = new Dictionary<string, double>();

            // Додаємо значення з рядка Z поточної (фінальної) матриці
            // Це вільні змінні, які стоять у "шапці"
            for (int j = 0; j < this.Columns - 1; j++)
            {
                string header = this.DualColumnHeaders[j];
                currentValues[header] = this[this.Rows - 1, j];
            }

            // Додаємо 0 для базисних змінних (ті, що зліва у фінальній таблиці)
            for (int i = 0; i < this.Rows - 1; i++)
            {
                string header = this.DualRowHeaders[i];
                currentValues[header] = 0.0;
            }

            // 2. Зворотна підстановка по логу (від останньої операції до першої)
            for (int i = OperationsULog.Count - 1; i >= 0; i--)
            {
                string[,] logEntry = OperationsULog[i];
                int logRows = logEntry.GetLength(0); // Кількість рядків у лозі

                string targetVar = logEntry[0, 0];   // Змінна, яку ми виражаємо (напр. u3)
                double constantTerm = double.Parse(logEntry[logRows - 1, 1]); // Останній рядок - константа

                double calculatedValue = constantTerm;

                // Проходимо по всіх проміжних коефіцієнтах у лозі
                for (int r = 1; r < logRows - 1; r++)
                {
                    string varInFormula = logEntry[r, 0];
                    double coefficient = double.Parse(logEntry[r, 1]);

                    if (currentValues.ContainsKey(varInFormula))
                    {
                        calculatedValue += coefficient * currentValues[varInFormula];
                    }
                    // Якщо змінної немає в словнику, вона вважається базисною (0), 
                    // тому додавати нічого не треба.
                }

                // Зберігаємо (або оновлюємо) розраховане значення
                currentValues[targetVar] = calculatedValue;
            }

            // 3. Формуємо результуючий рядок U = (u1, u2, ..., un)
            // Шукаємо всі змінні, що починаються на 'u', та сортуємо їх за індексом
            var uKeys = currentValues.Keys
                .Where(k => k.StartsWith("u"))
                .OrderBy(k => int.Parse(k.Substring(1)))
                .Select(k => currentValues[k].ToString("F2"));

            double[] uValues = uKeys.Select(double.Parse).ToArray();

            return uValues;
        }

        public void AddOperationULog(int s, int r)
        {
            string[,] newLog = new string[this.Rows + 1, 2];

            newLog[0, 0] = this.DualColumnHeaders[r];
            newLog[0, 1] = "0";

            for (int i = 0; i < this.Rows - 1; i++)
            {
                newLog[i + 1, 0] = this.DualRowHeaders[i];
                newLog[i + 1, 1] = this[i, r].ToString("F2");
            }

            newLog[this.Rows, 0] = "1";
            newLog[this.Rows, 1] = this[this.Rows - 1, r].ToString("F2");

            OperationsULog.Add(newLog);
        }

        public void InitializeHeaders()
        {
            RowHeaders = new string[Rows];
            ColumnHeaders = new string[Columns];

            for (int i = 0; i < Rows - 1; i++)
            {
                RowHeaders[i] = "y" + (i + 1);
            }
            RowHeaders[Rows - 1] = "Z";

            for (int j = 0; j < Columns - 1; j++)
            {
                ColumnHeaders[j] = "x" + (j + 1);
            }
            ColumnHeaders[Columns - 1] = "1";
        }

        public void InitializeDualHeaders()
        {
            DualRowHeaders = new string[Rows];
            DualColumnHeaders = new string[Columns];
            for (int i = 0; i < Rows - 1; i++)
            {
                DualRowHeaders[i] = "u" + (i + 1);
            }
            DualRowHeaders[Rows - 1] = "1";
            for (int j = 0; j < Columns - 1; j++)
            {
                DualColumnHeaders[j] = "v" + (j + 1);
            }
            DualColumnHeaders[Columns - 1] = "W";
        }

        public void UpdateYHeaders()
        {
            int count = 1;

            for (int i = 0; i < Rows - 1; i++)
            {
                if (RowHeaders[i] == "0")
                {
                    continue;
                }

                RowHeaders[i] = "y" + count;
                count++;
            }
        }

        public void SwapHeaders(int r, int s)
        {
            string temp = this.RowHeaders[r];

            this.RowHeaders[r] = this.ColumnHeaders[s];
            this.ColumnHeaders[s] = temp;
        }

        public void SwapDualHeaders(int r, int s)
        {
            string temp = this.DualRowHeaders[r];

            this.DualRowHeaders[r] = this.DualColumnHeaders[s];
            this.DualColumnHeaders[s] = temp;
        }

        public Matrix AddSRow(double[] newRow)
        {
            int oldRows = this.Rows;
            int lastRowIndex = oldRows - 1;

            double[,] newData = new double[oldRows + 1, Columns];
            string[] newRowHeaders = new string[oldRows + 1];

            for (int i = 0; i < lastRowIndex; i++)
            {
                newRowHeaders[i] = this.RowHeaders[i];
                for (int j = 0; j < Columns; j++)
                {
                    newData[i, j] = this[i, j];
                }
            }

            int currentSRowNum = this.SRowIndex + 1;
            newRowHeaders[lastRowIndex] = "s" + currentSRowNum;
            for (int j = 0; j < Columns; j++)
            {
                newData[lastRowIndex, j] = newRow[j];
            }

            newRowHeaders[oldRows] = this.RowHeaders[lastRowIndex];
            for (int j = 0; j < Columns; j++)
            {
                newData[oldRows, j] = this[lastRowIndex, j];
            }

            Matrix newMatrix = new Matrix(newData)
            {
                Rank = this.Rank,
                SRowIndex = currentSRowNum,
                RowHeaders = newRowHeaders,
                ColumnHeaders = (string[])this.ColumnHeaders.Clone()
            };

            return newMatrix;
        }

        public void PrintMatrix()
        {
            Console.Write("\t");
            for (int j = 0; j < this.Columns; j++)
            {
                Console.Write($"{this.ColumnHeaders[j],8}");
            }
            Console.WriteLine();

            for (int i = 0; i < this.Rows; i++)
            {
                Console.Write($"{this.RowHeaders[i]}\t");
                for (int j = 0; j < this.Columns; j++)
                {
                    Console.Write($"{this[i, j],8:F2}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void PrintMatrixWithDualHeaders()
        {
            Console.Write("\t");
            for (int j = 0; j < this.Columns; j++)
            {
                Console.Write($"{this.DualColumnHeaders[j],8}");
            }
            Console.WriteLine();
            Console.Write("\t");
            for (int j = 0; j < this.Columns; j++)
            {
                Console.Write($"{this.ColumnHeaders[j],8}");
            }
            Console.WriteLine();

            for (int i = 0; i < this.Rows; i++)
            {
                Console.Write($"{this.DualRowHeaders[i]} {this.RowHeaders[i]}\t");
                for (int j = 0; j < this.Columns; j++)
                {
                    Console.Write($"{this[i, j],8:F2}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public Matrix DeleteNegativeElements()
        {
            double minValue = FindMinElement();
            minValue = Math.Abs(minValue);

            double[,] newArray = new double[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    newArray[i, j] = this[i, j] + minValue;
                }
            }

            Matrix newMatrix = new Matrix(newArray);
            newMatrix.AdjustmentNumber = minValue;
            newMatrix.RowHeaders = this.RowHeaders != null ? (string[])this.RowHeaders.Clone() : null;
            newMatrix.ColumnHeaders = this.ColumnHeaders != null ? (string[])this.ColumnHeaders.Clone() : null;
            return newMatrix;
        }

        public double FindMinElement()
        {
            double minValue = double.MaxValue;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (this[i, j] < minValue)
                        minValue = this[i, j];
                }
            }
            return minValue;
        }

        public void ShowMaxZ()
        {
            Console.WriteLine($"Max (Z) = {this.GetMaxZ}");
        }

        public void ShowMinW()
        {
            Console.WriteLine($"Max (Z) = Min (W) = {this.GetMinW}");
        }

        public Matrix FilterColumn(int columnToExclude)
        {
            double[,] newData = new double[Rows, Columns - 1];
            string[] newColumnHeaders = new string[Columns - 1];
            string[] newDualColumnHeaders = null;

            if (DualColumnHeaders != null)
                newDualColumnHeaders = new string[Columns - 1];

            for (int i = 0; i < Rows; i++)
            {
                int targetCol = 0;
                for (int j = 0; j < Columns; j++)
                {
                    if (j == columnToExclude) continue;

                    newData[i, targetCol] = _data[i, j];

                    if (i == 0 && ColumnHeaders != null)
                        newColumnHeaders[targetCol] = ColumnHeaders[j];

                    if (i == 0 && newDualColumnHeaders != null)
                        newDualColumnHeaders[targetCol] = DualColumnHeaders[j];

                    targetCol++;
                }
            }

            Matrix newMatrix = new Matrix(newData)
            {
                Rank = this.Rank,
                RowHeaders = this.RowHeaders != null ? (string[])this.RowHeaders.Clone() : null,
                ColumnHeaders = newColumnHeaders,
                DualRowHeaders = this.DualRowHeaders != null ? (string[])this.DualRowHeaders.Clone() : null,
                DualColumnHeaders = newDualColumnHeaders,
                OperationsULog = this.OperationsULog
            };

            return newMatrix;
        }

        public Matrix AddZRow(double[] zRow)
        {
            double[,] newData = new double[Rows + 1, Columns];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    newData[i, j] = this[i, j];
                }
            }
            int lastRowIndex = Rows;
            for (int j = 0; j < Columns; j++)
            {
                newData[lastRowIndex, j] = zRow[j];
            }
            Matrix newMatrix = new Matrix(newData);
            return newMatrix;
        }
    }
}
