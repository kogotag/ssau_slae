using ssau_slae;
using System;
using System.Text;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Выполняем ряд тестов для метода Гаусса
            int gaussPassed = 0;
            for (int i = 1; i <= 1000; i++)
            {
                bool passed = testGauss(i);
                if (passed)
                {
                    gaussPassed++;
                }
            }

            Console.WriteLine("\n" + "Итого пройдено " + gaussPassed + " / 1000 тестов");
        }

        /// <summary>
        /// Тест, генерирующий некоторый вектор, предполагаемый решением будущего СЛАУ.
        /// К нему применяется невырожденное преобразование, полученное СЛАУ решается методом Гаусса.
        /// Решение должно совпасть с исходным вектором.
        /// </summary>
        /// <param name="iteration">Номер теста для удобства</param>
        /// <returns>true, если метод Гаусса отработал, как запланировано. Иначе false</returns>
        static bool testGauss(int iteration = -1)
        {
            Random random = new Random();
            int n = 3;
            // Случайный вектор с целыми координатами
            Matrix sol = genRandIntVector(n, random);

            // Объект решения СЛАУ для сравнения с полученным решением
            SLAESolution expectedSolution = new SLAESolution(SLAESolution.SolutionType.UNIQUE_SOLUTION);
            expectedSolution.SetUniqueSolution(sol);

            // Получаем невырожденную матрицу перехода.
            // Используем её как матрицу коэффициентов, умножаем точное решение слева на матрицу перехода
            // для получения вектора правой части
            Matrix transformMatrix = NonSingularTransform3DMatrix(random);
            Matrix rightHandMatrix = sol.MultiplyLeft(transformMatrix);

            // Получаем решение методом Гаусса
            SLAE slae = new SLAE(transformMatrix, rightHandMatrix);
            SLAESolution computedSol = slae.SolveGauss();

            // Готовим вывод результатов на экран
            StringBuilder message = new StringBuilder();
            if (iteration >= 0)
            {
                message.Append("Тест метода Гаусса " + iteration + ": ");
            }

            bool passed = false;
            if (expectedSolution.EqualsPrecision(computedSol, 0.1))
            {
                message.Append("Решения совпали");
                passed = true;
            }
            else
            {
                message.Append("Решения не совпали");
                message.AppendLine("\nСистема:");
                message.AppendLine(SLAE.BuildExpandedMatrix(transformMatrix, rightHandMatrix).ToString());
                message.AppendLine("Ожидаемое решение:");
                message.AppendLine(expectedSolution.ToString());
                message.AppendLine("Полученное решение:");
                message.AppendLine(computedSol.ToString());
                if (computedSol.GetSolutionType() == SLAESolution.SolutionType.UNIQUE_SOLUTION)
                {
                    message.AppendLine("Разница решений:");
                    Matrix diff = sol - computedSol.GetUniqueSolution();
                    message.AppendLine(diff.ToString());
                    message.AppendLine("Модуль разности решений:");
                    message.AppendLine(diff.VectorNorm().ToString());
                }
            }

            Console.WriteLine(message.ToString());
            return passed;
        }

        /// <summary>
        /// Этот метод проверяет, есть ли в матрице элементы очень низких порядков.
        /// Это нужно для корректной проверки метода Гаусса, так как большая разница в порядках элементов
        /// приводит к резкому увеличению погрешности
        /// </summary>
        /// <param name="matrix">Проверяемая матрица</param>
        /// <returns>true, если матрица не содержит элементов низких порядков</returns>
        static bool CheckTooLowOrderOfMagnitude(Matrix matrix)
        {
            for (int i = 0; i < matrix.GetRowsCount(); i++)
            {
                for (int j = 0; j < matrix.GetColumnsCount(); j++)
                {
                    if (Math.Abs(matrix.Get(i,j)) < 1E-3)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Случайная невырожденная матрица перехода 3 на 3. Генерируется путём последовательных
        /// преобразований растяжения/сжатия базисных векторов, поворотов вокруг осей.
        /// </summary>
        /// <param name="random">Генератор случайных чисел</param>
        /// <returns>Случайная невырожденная матрица перехода 3 на 3</returns>
        static Matrix NonSingularTransform3DMatrix(Random random)
        {
            Matrix transformMatrix = new Matrix(3, 3);
            do
            {
                transformMatrix = genRandIntDiagonalMatrix(3, random)
                    .MultiplyLeft(zRotationMatrix(Math.PI / 12 * random.Next(-12, 13)))
                    .MultiplyLeft(yRotationMatrix(Math.PI / 12 * random.Next(-12, 13)))
                    .MultiplyLeft(xRotationMatrix(Math.PI / 12 * random.Next(-12, 13)));
            } while (CheckTooLowOrderOfMagnitude(transformMatrix));

            return transformMatrix;
        }

        /// <summary>
        /// Случайный вектор произвольной размерности, элементами которого являются целые числа.
        /// Ни один элемент не будет равен 0
        /// </summary>
        /// <param name="dim">Размерность вектора</param>
        /// <param name="random">Генератор случайных чисел</param>
        /// <returns>Случайный вектор произвольной размерности, элементами которого являются целые числа</returns>
        static Matrix genRandIntVector(int dim, Random random)
        {
            Matrix res = new Matrix(dim, 1);
            for (int i = 0; i < dim; i++)
            {
                do
                {
                    res.Set(i, 0, random.Next(-100, 100));
                } while (res.Get(i, 0) == 0);
            }

            return res;
        }

        /// <summary>
        /// Случайная диагональная матрица, элементы главной диагонали которой являются целыми числами.
        /// Все элементы главной диагонали отличны от нуля
        /// </summary>
        /// <param name="dim">Размерность матрицы</param>
        /// <param name="random">Генератор случайных чисел</param>
        /// <returns>Случайная диагональная матрица</returns>
        static Matrix genRandIntDiagonalMatrix(int dim, Random random)
        {
            Matrix res = new Matrix(dim, dim);
            for (int i = 0; i < dim; i++)
            {
                do
                {
                    res.Set(i, i, random.Next(-100, 100));
                } while (res.Get(i, i) == 0);
            }

            return res;
        }

        /// <summary>
        /// Матрица поворота около оси x на выбранный угол
        /// </summary>
        /// <param name="angle">Угол поворота</param>
        /// <returns>Матрица поворота</returns>
        static Matrix xRotationMatrix(double angle)
        {
            Matrix res = new Matrix(3, 3);

            res.Set(0, 0, 1);
            res.Set(1, 1, Math.Cos(angle));
            res.Set(2, 2, res.Get(1, 1));
            res.Set(2, 1, Math.Sin(angle));
            res.Set(1, 2, -res.Get(2, 1));

            return res;
        }

        /// <summary>
        /// Матрица поворота около оси y на выбранный угол
        /// </summary>
        /// <param name="angle">Угол поворота</param>
        /// <returns>Матрица поворота</returns>
        static Matrix yRotationMatrix(double angle)
        {
            Matrix res = new Matrix(3, 3);

            res.Set(1, 1, 1);
            res.Set(0, 0, Math.Cos(angle));
            res.Set(2, 2, res.Get(0, 0));
            res.Set(2, 0, -Math.Sin(angle));
            res.Set(0, 2, -res.Get(2, 0));

            return res;
        }

        /// <summary>
        /// Матрица поворота около оси z на выбранный угол
        /// </summary>
        /// <param name="angle">Угол поворота</param>
        /// <returns>Матрица поворота</returns>
        static Matrix zRotationMatrix(double angle)
        {
            Matrix res = new Matrix(3, 3);

            res.Set(2, 2, 1);
            res.Set(0, 0, Math.Cos(angle));
            res.Set(1, 1, res.Get(0, 0));
            res.Set(1, 0, Math.Sin(angle));
            res.Set(0, 1, -res.Get(1, 0));

            return res;
        }
    }
}