using Lab_4_ASPPR;
using System.Security.Cryptography;

// Constants
const string ERROR_SQUARE_MATRIX = "Помилка! Матриця повинна бути квадратною.";
const string ERROR_OUT_OF_BOUNDS = "Помилка! Рядок виходить за межi матрицi";
const string ERROR_NOT_NUMBER = "Помилка! '{0}' не є числом. Спробуйте ще раз.";
const string ERROR_INCONSISTENT = "Система обмежень є суперечливою";
const string ERROR_UNBOUNDED = "Цiльова функцiя необмежена";
const string ERROR_CANT_MODIFY_Z = "Помилка! Неможливо змiнити рядок Z.";
const string ERROR_INVALID_CHOICE = "Помилка! Введено некоректний вибiр. Спробуйте ще раз.";
const string ERROR_INVALID_NUMBER = "Помилка! Введено некоректне число. Спробуйте ще раз.";

// Global state
int choice = 0;
Matrix? optimalMatrix = null;
Matrix? withoutZeros = null;

// Helper methods for printing
void PrintIterations(List<Matrix> iterations)
{
    int count = 1;
    foreach (Matrix m in iterations)
    {
        Console.WriteLine($"\nРозв\'язок для {count} елементу дiагоналi:");
        m.PrintMatrix();
        count++;
    }
}

void PrintSolutions(double[] solutions)
{
    Console.WriteLine("\nРозв\'язок системи:");
    for (int i = 0; i < solutions.Length; i++)
    {
        Console.WriteLine($"X[{i + 1}]: {solutions[i]}");
    }
}

void PrintIntermediateTable(Matrix matrix, int row, int col, string format = "Промiжна таблиця: (елемент {0}, {1})")
{
    Console.WriteLine($"\n{format}", matrix.ColumnHeaders[col], matrix.RowHeaders[row]);
    matrix.PrintMatrix();
}

void PrintIntermediateTableWithDual(Matrix matrix, int row, int col)
{
    Console.WriteLine($"\nПромiжна таблиця: (елемент {matrix.ColumnHeaders[col]}, {matrix.RowHeaders[row]})");
    matrix.PrintMatrixWithDualHeaders();
}

// Matrix operations
void ReverseMatrix()
{
    Matrix matrix = InputManager.InputMatrix();
    matrix.InitializeHeaders();

    if (matrix.Rows != matrix.Columns)
    {
        Console.WriteLine(ERROR_SQUARE_MATRIX);
        return;
    }

    InverseMatrixCalculator eliminator = new InverseMatrixCalculator();
    List<Matrix> iterations = eliminator.Eliminate(matrix);
    PrintIterations(iterations);
}

void SystemSolution()
{
    Matrix matrix = InputManager.InputMatrix();
    double[] constants = InputManager.InputConstants();
    matrix.InitializeHeaders();

    InverseMatrixCalculator eliminator = new InverseMatrixCalculator();
    List<Matrix> iterations = eliminator.Eliminate(matrix);
    PrintIterations(iterations);

    SystemCalculator systemCalculator = new SystemCalculator();
    double[] solutions = systemCalculator.Calculate(matrix, constants);
    PrintSolutions(solutions);
}

void RankMatrix()
{
    Matrix matrix = InputManager.InputMatrix();
    matrix.InitializeHeaders();

    RankCalculator rankCalculator = new RankCalculator();
    List<Matrix> iterations = rankCalculator.CalculateRank(matrix);
    PrintIterations(iterations);

    Console.WriteLine("\nРанг матрицi: " + matrix.Rank);
}

// Matrix analysis helpers
int FindRowWithNegativeB(Matrix matrix)
{
    for (int i = 0; i < matrix.Rows - 1; i++)
    {
        if (matrix[i, matrix.Columns - 1] < 0)
            return i;
    }
    return -1;
}

int FindNegativeInRow(Matrix matrix, int row)
{
    for (int j = 0; j < matrix.Columns - 1; j++)
    {
        if (matrix[row, j] < 0)
            return j;
    }
    return -1;
}

int FindPositiveInRow(Matrix matrix, int row)
{
    for (int j = 0; j < matrix.Columns - 1; j++)
    {
        if (matrix[row, j] > 0)
            return j;
    }
    return -1;
}

int FindNegativeInZRow(Matrix matrix)
{
    int lastRow = matrix.Rows - 1;
    for (int j = 0; j < matrix.Columns - 1; j++)
    {
        if (matrix[lastRow, j] < 0)
            return j;
    }
    return -1;
}

int OptimalMinNotNegative(Matrix matrix, int col)
{
    int r = -1;
    double minValue = double.MaxValue;
    int lastCol = matrix.Columns - 1;

    for (int i = 0; i < matrix.Rows - 1; i++)
    {
        double element = matrix[i, col];
        double freeTerm = matrix[i, lastCol];

        if (element > 0)
        {
            double ratio = freeTerm / element;
            if (ratio < minValue)
            {
                minValue = ratio;
                r = i;
            }
        }
    }
    return r;
}

int MinNotNegative(Matrix matrix, int col)
{
    int r = -1;
    double minValue = double.MaxValue;
    int lastCol = matrix.Columns - 1;

    for (int i = 0; i < matrix.Rows - 1; i++)
    {
        double element = matrix[i, col];
        double freeTerm = matrix[i, lastCol];

        if (element != 0)
        {
            double ratio = freeTerm / element;
            if (ratio >= 0 && ratio < minValue)
            {
                minValue = ratio;
                r = i;
            }
        }
    }
    return r;
}

bool IsLastColumnNegative(Matrix matrix)
{
    for (int i = 0; i < matrix.Rows - 1; i++)
    {
        if (matrix[i, matrix.Columns - 1] < 0)
            return true;
    }
    return false;
}

// Result extraction methods
double[] GetResultXdouble(Matrix matrix)
{
    int maxIndex = 0;
    var allHeaders = matrix.RowHeaders.Concat(matrix.ColumnHeaders);
    foreach (var header in allHeaders)
    {
        if (header.StartsWith("x") && int.TryParse(header.Substring(1), out int index))
        {
            if (index > maxIndex) maxIndex = index;
        }
    }

    if (maxIndex == 0) return new double[0];

    double[] xValues = new double[maxIndex];
    for (int i = 0; i < matrix.Rows - 1; i++)
    {
        string header = matrix.RowHeaders[i];
        if (header.StartsWith("x") && int.TryParse(header.Substring(1), out int index))
        {
            xValues[index - 1] = matrix[i, matrix.Columns - 1];
        }
    }
    return xValues;
}

double[] GetResultXdoubleForFractional(Matrix matrix)
{
    double[] xValues = new double[matrix.Rows];
    for (int i = 0; i < matrix.Rows - 1; i++)
    {
        string header = matrix.RowHeaders[i];
        if (header.StartsWith("x"))
            xValues[i] = matrix[i, matrix.Columns - 1];
    }
    return xValues;
}

string GetResultX(Matrix matrix)
{
    double[] xValues = GetResultXdouble(matrix);
    return "X = (" + string.Join("; ", xValues.Select(v => v.ToString("F5"))) + ")";
}

double[] GetResultUdouble(Matrix matrix)
{
    int maxIndex = 0;
    var allHeaders = matrix.DualRowHeaders.Concat(matrix.DualColumnHeaders);
    foreach (var header in allHeaders)
    {
        if (header.StartsWith("u") && int.TryParse(header.Substring(1), out int index))
        {
            if (index > maxIndex) maxIndex = index;
        }
    }

    if (maxIndex == 0) return new double[0];

    double[] uValues = new double[maxIndex];
    for (int i = 0; i < matrix.Columns - 1; i++)
    {
        string header = matrix.DualColumnHeaders[i];
        if (header.StartsWith("u") && int.TryParse(header.Substring(1), out int index))
        {
            uValues[index - 1] = matrix[matrix.Rows - 1, i];
        }
    }
    return uValues;
}

string GetResultU(Matrix matrix)
{
    double[] uValues = GetResultUdouble(matrix);
    return "U = (" + string.Join("; ", uValues.Select(v => v.ToString("F5"))) + ")";
}

double[] GetRowValues(Matrix matrix, int row)
{
    double[] rowValues = new double[matrix.Columns];
    for (int i = 0; i < matrix.Columns; i++)
    {
        rowValues[i] = matrix[row, i];
    }
    return rowValues;
}

// Solution finding methods
Matrix FindOptimalSolution(Matrix matrix, bool useDualHeaders = false)
{
    ModifiedMatrixCalculator optimalSolution = new ModifiedMatrixCalculator();

    while (true)
    {
        int s = FindNegativeInZRow(matrix);

        if (s == -1)
        {
            Console.WriteLine("Оптимальний розв'язок знайдено");
            return matrix;
        }

        int r = OptimalMinNotNegative(matrix, s);

        if (r == -1)
        {
            Console.WriteLine(ERROR_UNBOUNDED);
            return null;
        }

        matrix = optimalSolution.Calculate(matrix, r, s);
        matrix.SwapHeaders(r, s);

        if (useDualHeaders)
        {
            matrix.SwapDualHeaders(r, s);
            PrintIntermediateTableWithDual(matrix, r, s);
        }
        else
        {
            PrintIntermediateTable(matrix, r, s);
        }
    }
}

Matrix FindReferenceSolution(Matrix matrix, bool useDualHeaders = false)
{
    if (matrix == null) return null;

    ModifiedMatrixCalculator referenceSolution = new ModifiedMatrixCalculator();

    while (IsLastColumnNegative(matrix))
    {
        int targetRow = FindRowWithNegativeB(matrix);
        if (targetRow == -1)
            break;

        int s = FindNegativeInRow(matrix, targetRow);
        if (s == -1)
        {
            Console.WriteLine(ERROR_INCONSISTENT);
            return null;
        }

        int r = MinNotNegative(matrix, s);
        if (r == -1)
        {
            Console.WriteLine("Неможливо знайти розв\'язувальний рядок");
            return null;
        }

        matrix = referenceSolution.Calculate(matrix, r, s);
        matrix.SwapHeaders(r, s);

        if (useDualHeaders)
        {
            matrix.SwapDualHeaders(r, s);
            PrintIntermediateTableWithDual(matrix, r, s);
        }
        else
        {
            Console.WriteLine("\nПромiжна таблиця");
            matrix.PrintMatrix();
        }
    }

    Console.WriteLine("Опорний розв\'язок знайдено");
    return matrix;
}

Matrix InputDeleteZeroRows(Matrix matrix, string? input, bool useDualHeaders = false)
{
    ModifiedMatrixCalculator eliminator = new ModifiedMatrixCalculator();
    matrix.InitializeHeaders();
    string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    int[] zeroRows = new int[parts.Length];

    for (int i = 0; i < parts.Length; i++)
    {
        if (!int.TryParse(parts[i], out int rowIndex))
        {
            Console.WriteLine(string.Format(ERROR_NOT_NUMBER, parts[i]));
            return null;
        }
        zeroRows[i] = rowIndex - 1;
    }

    for (int i = 0; i < zeroRows.Length; i++)
    {
        if (zeroRows[i] < 0 || zeroRows[i] >= matrix.Rows)
        {
            Console.WriteLine($"Помилка! Рядок {zeroRows[i] + 1} виходить за межi матрицi");
            return null;
        }

        if (matrix.RowHeaders[zeroRows[i]] != "Z")
        {
            matrix.RowHeaders[zeroRows[i]] = "0";
        }
        else
        {
            Console.WriteLine(ERROR_CANT_MODIFY_Z);
            return null;
        }
    }

    matrix.UpdateYHeaders();

    if (useDualHeaders)
        matrix.InitializeDualHeaders();

    int zeroRowIteration = 0;

    while (zeroRowIteration < zeroRows.Length)
    {
        // Отримуємо індекс рядка, який ми ПОЗНАЧИЛИ як "0"
        int r = zeroRows[zeroRowIteration];

        // Шукаємо позитивний елемент САМЕ в цьому рядку
        int column = FindPositiveInRow(matrix, r);

        // Якщо позитивних немає, спробуємо знайти будь-який ненульовий (крім вільного члена)
        if (column == -1)
        {
            for (int j = 0; j < matrix.Columns - 1; j++)
            {
                if (Math.Abs(matrix[r, j]) > 1e-9)
                {
                    column = j;
                    break;
                }
            }
        }

        // Якщо в рядку взагалі немає ненульових елементів (рядок лінійно залежний)
        if (column == -1)
        {
            // Можна просто видалити цей рядок з матриці
            // matrix = matrix.FilterRow(r); // якщо у вас є такий метод
            zeroRowIteration++;
            continue;
        }

        // ТЕПЕР ОБОВ'ЯЗКОВО ВИКОРИСТОВУЄМО 'r' як розв'язувальний рядок
        eliminator.Calculate(matrix, r, column);
        matrix.SwapHeaders(r, column);

        if (useDualHeaders)
            matrix.SwapDualHeaders(r, column);

        // Оскільки ми вибрали рядок 'r', де була мітка "0", 
        // тепер ця мітка ТОЧНО перейшла в ColumnHeaders[column]
        if (matrix.ColumnHeaders[column] == "0")
        {
            if (useDualHeaders)
            {
                matrix.AddOperationULog(r, column);
                // ... (ваш лог для dual headers)
            }
            matrix = matrix.FilterColumn(column);
            zeroRowIteration++;
        }

        if (useDualHeaders)
            PrintIntermediateTableWithDual(matrix, r, column);
        else
            PrintIntermediateTable(matrix, r, column);
    }

    return matrix;
}

void ShowReferenceSolution()
{
    Matrix matrix = InputManager.InputMatrix();
    if (matrix == null) return;
    matrix.InitializeHeaders();

    Matrix referenceSolution = FindReferenceSolution(matrix);
    optimalMatrix = referenceSolution;

    if (referenceSolution != null)
    {
        Console.WriteLine("\nОпорний розв\'язок:");
        Console.WriteLine(GetResultX(referenceSolution));
    }
}

void ShowOptimalSolution()
{
    if (optimalMatrix == null)
    {
        Console.WriteLine("Спочатку знайдіть опорний розв\'язок.");
        return;
    }

    Matrix optimalSolution = FindOptimalSolution(optimalMatrix.Clone());
    if (optimalSolution != null)
    {
        Console.WriteLine("\nОптимальний розв\'язок:");
        Console.WriteLine(GetResultX(optimalSolution));
        optimalSolution.ShowMaxZ();
    }
}

Matrix AddOnes(Matrix matrix)
{
    double[,] newArray = new double[matrix.Rows + 1, matrix.Columns + 1];

    for (int i = 0; i < matrix.Rows; i++)
    {
        for (int j = 0; j < matrix.Columns; j++)
        {
            newArray[i, j] = matrix[i, j];
        }
    }

    for (int i = 0; i < matrix.Rows + 1; i++)
        newArray[i, matrix.Columns] = 1;

    for (int j = 0; j < matrix.Columns + 1; j++)
        newArray[matrix.Rows, j] = -1;

    newArray[matrix.Rows, matrix.Columns] = 0;

    Matrix newMatrix = new Matrix(newArray);
    newMatrix.AdjustmentNumber = matrix.AdjustmentNumber;
    return newMatrix;
}

int[] BottomPrice(Matrix matrix)
{
    int minValue = int.MaxValue;
    int[] indexes = new int[2];

    for (int i = 0; i < matrix.Rows; i++)
    {
        for (int j = 0; j < matrix.Columns; j++)
        {
            if (matrix[i, j] < minValue)
            {
                minValue = (int)matrix[i, j];
                indexes[0] = i;
                indexes[1] = j;
            }
        }
    }
    return indexes;
}

int[] TopPrice(Matrix matrix)
{
    int maxValue = int.MinValue;
    int[] indexes = new int[2];

    for (int j = 0; j < matrix.Columns; j++)
    {
        for (int i = 0; i < matrix.Rows; i++)
        {
            if (matrix[i, j] > maxValue)
            {
                maxValue = (int)matrix[i, j];
                indexes[0] = i;
                indexes[1] = j;
            }
        }
    }
    return indexes;
}

int GetStrategy(double value, double[] strategies)
{
    double accumulated = 0;
    for (int i = 0; i < strategies.Length; i++)
    {
        accumulated += strategies[i];
        if (value <= accumulated)
            return i;
    }
    return strategies.Length - 1;
}

void PrintModelingResults(double[,] randomNumbers, double[] X, double[] Y, Matrix matrix)
{
    string rowTemplate = "{0,-10} | {1,-10} | {2,-10} | {3,-10} | {4,-10} | {5,-10} | {6,-10} | {7,-10}";

    Console.WriteLine(rowTemplate, "Номер", "Випадкове", "Стратегiя", "Випадкове", "Стратегiя", "Виграш A", "Накопич.", "Середнiй");
    Console.WriteLine(rowTemplate, "Партiї", "число A", "гравця A", "число B", "гравця B", "", "виграш A", "виграш A");
    Console.WriteLine(new string('-', 101));

    double cumulativeWinA = 0;
    double averageWinA = 0;
    int gamesCount = randomNumbers.GetLength(0);

    double[] newX = new double[X.Length];
    double[] newY = new double[Y.Length];

    for (int i = 0; i < gamesCount; i++)
    {
        int strategyA = GetStrategy(randomNumbers[i, 0], X);
        int strategyB = GetStrategy(randomNumbers[i, 1], Y);

        for (int j = 0; j < X.Length; j++)
        {
            if (strategyA == j) newX[j]++;
        }

        for (int j = 0; j < Y.Length; j++)
        {
            if (strategyB == j) newY[j]++;
        }

        double winA = matrix[strategyA, strategyB];
        cumulativeWinA += winA;
        averageWinA = Math.Round(cumulativeWinA / (i + 1), 5);

        Console.WriteLine(rowTemplate, i + 1, randomNumbers[i, 0], matrix.RowHeaders[strategyA], 
                         randomNumbers[i, 1], matrix.ColumnHeaders[strategyB], winA, cumulativeWinA, averageWinA);
    }

    for (int i = 0; i < newX.Length; i++)
        newX[i] /= gamesCount;

    for (int i = 0; i < newY.Length; i++)
        newY[i] /= gamesCount;

    Console.WriteLine("\nПiдсумковi стратегії:");
    Console.WriteLine("X = (" + string.Join("; ", newX.Select(v => v.ToString("F5"))) + ")");
    Console.WriteLine("Y = (" + string.Join("; ", newY.Select(v => v.ToString("F5"))) + ")");
    Console.WriteLine($"v = {averageWinA}");
}

void SolveMatrixGame()
{
    Matrix matrix = InputManager.InputMatrix();

    if (matrix.FindMinElement() < 0)
        matrix = matrix.DeleteNegativeElements();

    int[] bottomPrice = BottomPrice(matrix);
    int[] topPrice = TopPrice(matrix);

    Console.WriteLine($"Нижня цiна гри {matrix[bottomPrice[0], bottomPrice[1]]} (рядок {bottomPrice[0]}, стовпець {bottomPrice[1]})");
    Console.WriteLine($"Верхня цiна гри {matrix[topPrice[0], topPrice[1]]} (рядок {topPrice[0]}, стовпець {topPrice[1]})");

    if (!bottomPrice.SequenceEqual(topPrice))
    {
        Console.WriteLine("Гра не має чистої стратегiї.");

        matrix = AddOnes(matrix);
        matrix.InitializeHeaders();
        matrix.InitializeDualHeaders();
        Matrix initialMatrix = matrix.Clone();

        Console.WriteLine("\nПочаткова таблиця для розв\'язання гри:");
        matrix.PrintMatrixWithDualHeaders();

        Console.WriteLine("Розв\'язання симплекс методом...");
        matrix = FindOptimalSolution(matrix, true);
        matrix.PrintMatrixWithDualHeaders();
        Console.WriteLine(GetResultX(matrix));
        Console.WriteLine(GetResultU(matrix));
        matrix.ShowMinW();

        double v = matrix[matrix.Rows - 1, matrix.Columns - 1];
        v = 1 / v - matrix.AdjustmentNumber;

        double[] newX = GetResultXdouble(matrix).Select(x => x * v).ToArray();
        double[] newU = GetResultUdouble(matrix).Select(u => u * v).ToArray();

        Console.WriteLine("Стратегiї:");
        Console.WriteLine("Xo = (" + string.Join("; ", newU.Select(u => u.ToString("F5"))) + ")");
        Console.WriteLine("Yo = (" + string.Join("; ", newX.Select(x => x.ToString("F5"))) + ")");
        Console.WriteLine($"Цiна гри: {v}");

        Console.Write("Чи потрiбне моделювання? (y/n):");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            Console.Write("\nВведiть кiлькiсть партiй: ");
            if (int.TryParse(Console.ReadLine(), out int gamesCount) && gamesCount > 0)
            {
                byte[] bytes = new byte[8];
                double[,] randomNumbers = new double[gamesCount, 2];

                for (int i = 0; i < gamesCount; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        RandomNumberGenerator.Fill(bytes);
                        ulong ul = BitConverter.ToUInt64(bytes, 0);
                        double r = (double)ul / ulong.MaxValue;
                        randomNumbers[i, j] = Math.Round(Math.Abs(r % 1), 5);
                    }
                }

                Console.WriteLine("Результати моделювання:\n");
                PrintModelingResults(randomNumbers, newU, newX, initialMatrix);
            }
        }
    }
    else
    {
        matrix.InitializeHeaders();
        matrix.InitializeDualHeaders();
        matrix.PrintMatrixWithDualHeaders();
        Console.WriteLine($"Гра має чисту стратегiю з цiною {bottomPrice}.");
    }
}

// Integer solution
void FindIntSolution()
{
    int counter = 1;
    Matrix matrix = InputManager.InputMatrix();
    if (matrix == null) return;

    matrix.InitializeHeaders();

    while (true)
    {
        Console.WriteLine($"\nПОШУК ЦIЛОЧИСЕЛЬНОГО РОЗВ\'ЯЗКУ (крок {counter}):");

        Console.WriteLine("Пошук опорного розв\'язку...");
        matrix = FindReferenceSolution(matrix);
        if (matrix == null) return;
        matrix.PrintMatrix();
        Console.WriteLine(GetResultX(matrix));

        Console.WriteLine("\nПошук оптимального розв\'язку...");
        matrix = FindOptimalSolution(matrix);
        if (matrix == null) return;
        matrix.PrintMatrix();
        Console.WriteLine(GetResultX(matrix));
        matrix.ShowMaxZ();

        double[] xValues = GetResultXdoubleForFractional(matrix);

        if (xValues.All(v => Math.Abs(v - Math.Round(v)) < 1e-9))
        {
            Console.WriteLine("\nЦiлочисельний розв\'язок знайдено:");
            Console.WriteLine(GetResultX(matrix));
            matrix.ShowMaxZ();
            break;
        }

        int maxFractionalRow = GomoryAlgorithm.GetXWithMaxFractialPart(xValues);
        Console.WriteLine($"\nРядок з найбiльшою дробовою частиною: {matrix.RowHeaders[maxFractionalRow]}");

        double[] xRowValues = GetRowValues(matrix, maxFractionalRow);
        double[] sConstraints = GetSConstraints(xRowValues);

        Console.WriteLine("\nДодаткове обмеження:");
        matrix = matrix.AddSRow(sConstraints);
        Console.WriteLine($"s{matrix.SRowIndex} = " + string.Join("; ", sConstraints.Select(v => v.ToString("F2"))));

        Console.WriteLine($"\nПромiжна таблиця з додатковим обмеженням s{matrix.SRowIndex}:");
        matrix.PrintMatrix();

        counter++;
    }
}

double[] GetSConstraints(double[] xRowValues)
{
    for (int i = 0; i < xRowValues.Length - 1; i++)
        xRowValues[i] = -1 * (xRowValues[i] - Math.Floor(xRowValues[i]));

    xRowValues[xRowValues.Length - 1] = -Math.Abs(xRowValues[xRowValues.Length - 1] - Math.Floor(xRowValues[xRowValues.Length - 1]));

    return xRowValues;
}

// Dual problem
void FindDoubleSolution()
{
    Matrix matrix = InputManager.InputMatrix();
    if (matrix == null) return;

    matrix.InitializeHeaders();
    matrix.InitializeDualHeaders();

    Console.WriteLine("Чи є нуль-рядки? (y/n):");
    if (Console.ReadLine()?.Trim().ToLower() == "y")
    {
        Console.WriteLine("Вкажiть нульовi рядки через пробiл: ");
        string? zeroRowsInput = Console.ReadLine();
        matrix = InputDeleteZeroRows(matrix, zeroRowsInput, true);
        if (matrix == null) return;
    }

    Console.WriteLine("\nПочаткова таблиця:");
    matrix.PrintMatrixWithDualHeaders();

    Console.WriteLine("\nПошук опорного розв\'язку...");
    matrix = FindReferenceSolution(matrix, true);
    if (matrix == null) return;
    matrix.PrintMatrixWithDualHeaders();
    Console.WriteLine(GetResultX(matrix));
    Console.WriteLine(matrix.GetFinalUValue());

    Console.WriteLine("\nПошук оптимального розв\'язку...");
    matrix = FindOptimalSolution(matrix, true);
    if (matrix == null) return;
    matrix.PrintMatrixWithDualHeaders();
    Console.WriteLine(GetResultX(matrix));
    Console.WriteLine(matrix.GetFinalUValue());

    matrix.ShowMinW();
}

void SolveNatureGames()
{
    Matrix matrix = InputManager.InputMatrix();
    NatureGameSolver.SolveWaldCriterion(matrix);
    NatureGameSolver.SolveOptimismCriterion(matrix);
    NatureGameSolver.SolveHurwiczCriterion(matrix);
    NatureGameSolver.SolveSavageCriterion(matrix);
    NatureGameSolver.SolveBayesCriterion(matrix);
    NatureGameSolver.SolveLaplaceCriterion(matrix);
}

void PrintC(List<double[]> ZRows)
{
    Console.Write("\n\t");
    for (int j = 0; j < ZRows[0].Length - 1; j++)
    {
        Console.Write($"{"x" + (j + 1),8}");
    }
    Console.WriteLine();

    for (int i = 0; i < ZRows.Count; i++)
    {
        Console.Write($"C{i + 1}:\t");
        for (int j = 0; j < ZRows[0].Length - 1; j++)
        {
            Console.Write($"{ZRows[i][j],8:F2}");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

void PrintX(List<double[]> optimalSolutions)
{
    Console.Write("\n\t");
    for (int j = 0; j < optimalSolutions[0].Length; j++)
    {
        Console.Write($"{"x" + (j + 1),8}");
    }
    Console.WriteLine();

    for (int i = 0; i < optimalSolutions.Count; i++)
    {
        Console.Write($"X*{i + 1}:\t");
        for (int j = 0; j < optimalSolutions[0].Length; j++)
        {
            Console.Write($"{optimalSolutions[i][j],8:F2}");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

Matrix CalculateNonoptimalMatrix(List<double[]> ZRows, List<double[]> optimalSolutions)
{
    int n = optimalSolutions.Count; // Кількість критеріїв
    double[,] newMatrix = new double[n, n];

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            // Скалярний добуток Cj * Xi*
            double CjXi = ScalarProduct(ZRows[j], optimalSolutions[i]);

            // Скалярний добуток Cj * Xj*
            double CjXj = ScalarProduct(ZRows[j], optimalSolutions[j]);

            if (Math.Abs(CjXj) > 1e-10)
            {
                // Формула: q_ij = | (Cj*Xi - Cj*Xj) / Cj*Xj |
                double q_ij = Math.Abs((CjXi - CjXj) / CjXj);

                if (q_ij == 0)
                {
                    newMatrix[i, j] = q_ij;
                }
                else
                {
                    newMatrix[i, j] = -q_ij;
                }
            }
            else
            {
                newMatrix[i, j] = 0;
            }
        }
    }

    return new Matrix(newMatrix);
}

// Допоміжній метод для скалярного добутку векторів
double ScalarProduct(double[] vectorA, double[] vectorB)
{
    double sum = 0;
    for (int k = 0; k < vectorA.Length - 1; k++)
    {
        sum += vectorA[k] * vectorB[k];
    }
    return sum;
}

double[] CompromiseSolution(List<double[]> optimalSolutions, double[] vectorB)
{
    if (optimalSolutions == null || optimalSolutions.Count == 0) return null;
    if (optimalSolutions.Count != vectorB.Length)
    {
        throw new ArgumentException("Кількість вагових коефіцієнтів (vectorB) має збігатися з кількістю розв'язків.");
    }

    int numVariables = optimalSolutions[0].Length;
    int numSolutions = optimalSolutions.Count;
    double[] compromiseVector = new double[numVariables];

    for (int j = 0; j < numVariables; j++)
    {
        double sum = 0;
        for (int i = 0; i < numSolutions; i++)
        {
            sum += vectorB[i] * optimalSolutions[i][j];
        }
        compromiseVector[j] = sum;
    }

    return compromiseVector;
}

void SolveMultiCriteria()
{
    List<double[]> ZRows = new List<double[]>();
    List<double[]> SignedZRows = new List<double[]>();
    List<double[]> optimalSolutions = new List<double[]>();
    Matrix matrix = InputManager.InputMatrix();
    Console.WriteLine("Вкажiть нульовi рядки через пробiл: ");
    string? zeroRowsInput = Console.ReadLine();
    int criteriaCount = InputManager.InputCountOfCriteria();

    for (int i = 0; i < criteriaCount; i++)
    {
        Console.WriteLine("max/min:");
        bool choice = Console.ReadLine()?.Trim().ToLower() == "max" ? true : false;
        Console.WriteLine($"\nВведiть рядок Z для критерiю {i + 1}");
        double[] ZRow = InputManager.InputZRow(matrix.Columns);
        ZRows.Add(ZRow);

        if (choice)
        {
            double[] signedZRow = ZRow.Select(z => -z).ToArray();
            SignedZRows.Add(signedZRow);
        }
        else
        {
            SignedZRows.Add(ZRow);
        }
    }

    for (int i = 0;i < criteriaCount; i++)
    {
        Console.WriteLine($"Розв'язання Z{i} симплекс-методом");
        Matrix tempMatrix = matrix.AddZRow(SignedZRows[i]);
        tempMatrix.InitializeHeaders();
        tempMatrix = InputDeleteZeroRows(tempMatrix, zeroRowsInput);
        tempMatrix = FindReferenceSolution(tempMatrix);
        tempMatrix = FindOptimalSolution(tempMatrix);
        Console.WriteLine(GetResultX(tempMatrix));
        optimalSolutions.Add(GetResultXdouble(tempMatrix));
    }

    PrintX(optimalSolutions);
    PrintC(ZRows);
    Matrix nonOptimalMatrix = CalculateNonoptimalMatrix(ZRows, optimalSolutions);
    nonOptimalMatrix.InitializeHeaders();
    Console.WriteLine("Матриця програшу:");
    nonOptimalMatrix.PrintMatrix();
    double minValue = nonOptimalMatrix.FindMinElement();
    Console.WriteLine($"\nmax = |{minValue}| = {Math.Abs(minValue)}\n");
    nonOptimalMatrix = nonOptimalMatrix.DeleteNegativeElements();
    nonOptimalMatrix.InitializeDualHeaders();
    nonOptimalMatrix.PrintMatrixWithDualHeaders();

    int[] bottomPrice = BottomPrice(nonOptimalMatrix);
    int[] topPrice = TopPrice(nonOptimalMatrix);

    Console.WriteLine($"Нижня цiна гри {nonOptimalMatrix[bottomPrice[0], bottomPrice[1]]} (рядок {bottomPrice[0]}, стовпець {bottomPrice[1]})");
    Console.WriteLine($"Верхня цiна гри {nonOptimalMatrix[topPrice[0], topPrice[1]]} (рядок {topPrice[0]}, стовпець {topPrice[1]})");

    if (!bottomPrice.SequenceEqual(topPrice))
    {
        Console.WriteLine("Гра не має чистої стратегiї.");

        nonOptimalMatrix = AddOnes(nonOptimalMatrix);
        nonOptimalMatrix.InitializeHeaders();
        nonOptimalMatrix.InitializeDualHeaders();
        Matrix initialMatrix = nonOptimalMatrix.Clone();

        Console.WriteLine("\nПочаткова таблиця для розв\'язання гри:");
        nonOptimalMatrix.PrintMatrixWithDualHeaders();

        Console.WriteLine("Розв\'язання симплекс методом...");
        nonOptimalMatrix = FindOptimalSolution(nonOptimalMatrix, true);
        nonOptimalMatrix.PrintMatrixWithDualHeaders();
        Console.WriteLine("Стратегiя 1 гравця:");
        Console.WriteLine(GetResultU(nonOptimalMatrix));
        Console.WriteLine("Стратегiя 2 гравця:");
        Console.WriteLine(GetResultX(nonOptimalMatrix));
        nonOptimalMatrix.ShowMinW();

        double v = nonOptimalMatrix[nonOptimalMatrix.Rows - 1, nonOptimalMatrix.Columns - 1];
        v = 1 / v;
        double[] newX = GetResultXdouble(nonOptimalMatrix).Select(x => x * v).ToArray();
        double[] newU = GetResultUdouble(nonOptimalMatrix).Select(u => u * v).ToArray();
        Console.WriteLine($"Цiна гри: {v}");

        double[] compromiseVector = CompromiseSolution(optimalSolutions, GetResultUdouble(nonOptimalMatrix));
        Console.WriteLine("Компромiсне рiшення:");
        Console.WriteLine("(" + string.Join("; ", compromiseVector.Select(x => x.ToString("F5"))) + ")");
    }
    else
    {
        nonOptimalMatrix.InitializeHeaders();
        nonOptimalMatrix.InitializeDualHeaders();
        nonOptimalMatrix.PrintMatrixWithDualHeaders();
        Console.WriteLine($"Гра має чисту стратегiю з цiною {bottomPrice}.");
    }
}

// Main menu loop
while (true)
{
    Console.WriteLine("\nОберiть дiю:");
    Console.WriteLine("1 - Пошук оберненої матрицi");
    Console.WriteLine("2 - Пошук розв\'язку системи рiвнянь");
    Console.WriteLine("3 - Пошук рангу матрицi");
    Console.WriteLine("4 - Пошук опорного розв\'язку");
    Console.WriteLine("5 - Пошук оптимального розв\'язку");
    Console.WriteLine("6 - Видалення нульових рядкiв");
    Console.WriteLine("7 - Пошук цiлочисельного розв\'язку");
    Console.WriteLine("8 - Пошук розв\'язку для двоїстої задачi");
    Console.WriteLine("9 - Розв\'язання матричної гри");
    Console.WriteLine("10 - Iгри з природою");
    Console.WriteLine("11 - Розв'язок багатокритерiальної задачi");
    Console.WriteLine("0 - Вихiд");
    Console.Write("Ваш вибiр: ");

    try
    {
        choice = int.Parse(Console.ReadLine());
    }
    catch (FormatException)
    {
        Console.WriteLine(ERROR_INVALID_NUMBER);
        continue;
    }

    switch (choice)
    {
        case 0:
            return;
        case 1:
            ReverseMatrix();
            break;
        case 2:
            SystemSolution();
            break;
        case 3:
            RankMatrix();
            break;
        case 4:
            ShowReferenceSolution();
            break;
        case 5:
            ShowOptimalSolution();
            break;
        case 6:
            Console.WriteLine("Вкажiть нульовi рядки через пробiл: ");
            string? zeroRowsInput = Console.ReadLine();
            Matrix matrix = InputManager.InputMatrix();
            withoutZeros = InputDeleteZeroRows(matrix, zeroRowsInput, true);
            if (withoutZeros != null)
            {
                Console.WriteLine("Знайти опорний розв\'язок? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() == "y")
                {
                    optimalMatrix = FindReferenceSolution(withoutZeros.Clone());
                    if (optimalMatrix != null)
                    {
                        Console.WriteLine("\nОпорний розв\'язок:");
                        Console.WriteLine(GetResultX(optimalMatrix));
                    }
                }
                else
                {
                    Console.WriteLine("Повернення до головного меню.\n");
                }
            }
            break;
        case 7:
            FindIntSolution();
            break;
        case 8:
            FindDoubleSolution();
            break;
        case 9:
            SolveMatrixGame();
            break;
        case 10:
            SolveNatureGames();
            break;
        case 11:
            SolveMultiCriteria();
            break;
    }
}
