using ssau_slae;
using System;
using System.Text;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestPolynoms();
            TestGauss();
            TestSeidel();
            TestNewton();
        }

        /// <summary>
        /// Проверка вспомогательной части программы - алгебра многочленов
        /// </summary>
        static void TestPolynoms()
        {
            Polynomial polynom1 = new Polynomial(
                new List<Monomial>()
                {
                    new Monomial(new Dictionary<int, int>{ { 0, 1} }, 1)
                }, -2d);

            Polynomial polynom2 = new Polynomial(
                new List<Monomial>()
                {
                    new Monomial(new Dictionary<int, int>{ { 1, 1} }, 1)
                }, -1d);

            Polynomial polynom3 = new Polynomial(
                new List<Monomial>()
                {
                    new Monomial(new Dictionary<int, int>{ { 0, 1} }, 1)
                }, -3d);

            Polynomial polynom4 = new Polynomial(
                new List<Monomial>()
                {
                    new Monomial(new Dictionary<int, int>{ { 1, 1} }, 1)
                }, 2d);

            Polynomial polynomsProduct = polynom1 * polynom2 * polynom3 * polynom4;

            Polynomial polynomsProductExpected = new Polynomial(
                new List<Monomial>()
                {
                    new Monomial(new Dictionary<int, int>{ {0, 2}, {1,2 } }, 1d),
                    new Monomial(new Dictionary<int, int>{ {0,2 }, {1,1 } }, 1d),
                    new Monomial(new Dictionary<int, int>{ {0,1 }, {1,2 } }, -5d),
                    new Monomial(new Dictionary<int, int>{ {0, 1}, {1,1 } }, -5d),
                    new Monomial(new Dictionary<int, int>{ {0,2 } }, -2d),
                    new Monomial(new Dictionary<int, int>{ {1,2 } }, 6d),
                    new Monomial(new Dictionary<int, int>{ {0,1 } }, 10d),
                    new Monomial(new Dictionary<int, int>{ {1,1 } }, 6d)
                }, -12d);

            bool passed = polynomsProduct.Equals(polynomsProductExpected);

            if (passed)
            {
                Console.WriteLine("Многочлены прошли тест");
            }
            else
            {
                Console.WriteLine(polynom1.ToString());
                Console.WriteLine("Умножить на");
                Console.WriteLine(polynom2.ToString());
                Console.WriteLine("Умножить на");
                Console.WriteLine(polynom3.ToString());
                Console.WriteLine("Умножить на");
                Console.WriteLine(polynom4.ToString());
                Console.WriteLine("Равно");
                Console.WriteLine(polynomsProduct.ToString());
                Console.WriteLine("А должно быть");
                Console.WriteLine(polynomsProductExpected.ToString());
            }
        }

        /// <summary>
        /// Проверка метода Ньютона заданное число раз
        /// </summary>
        static void TestNewton()
        {
            //Выполняем ряд тестов для метода Ньютона
            int testsPassed = 0;
            for (int i = 1; i <= 1000; i++)
            {
                bool passed = TestNewton(i);
                if (passed)
                {
                    testsPassed++;
                }
            }

            Console.WriteLine("\n" + "Итого пройдено " + testsPassed + " / 1000 тестов");
        }

        /// <summary>
        /// Проверка метода Ньютона
        /// </summary>
        /// <param name="iteration">Номер теста</param>
        /// <returns>true, если тест пройден; иначе false</returns>
        static bool TestNewton(int iteration = -1)
        {
            Random random = new Random();

            // Размерность системы
            int n = 5;

            // Ожидаемое решение
            // К сожалению, отделить корни от известного не удалось, поэтому наличие известного корня не позволяет проверить метод.
            // Тем не менее, он используется для построения функций-многочленов
            Matrix expectedSolution = new Matrix(new double[]
            {
                1d,
                1d,
                1d,
                1d,
                1d
            });

            // Началньая точка
            Matrix startingPoint = new Matrix(expectedSolution);

            // Сдвигаемся от известного корня на небольшое расстояние
            for (int i = 0; i < n; i++)
            {
                startingPoint.Set(i, 0, expectedSolution.Get(i, 0) + random.NextDouble() / 3);
            }

            // Генерируем случайные многочлены
            Polynomial[] polynoms = new Polynomial[n];
            for (int i = 0; i < n; i++)
            {
                polynoms[i] = GenRandPolynomial(random, n, expectedSolution);
            }

            // Конвертируем многочлены в функции
            MathFunction[] functions = new MathFunction[n];
            for (int i = 0; i < n; i++)
            {
                functions[i] = polynoms[i].ConvertToFunction();
            }

            // Создаём объект решения
            NewtonSystemsSolver solver = new NewtonSystemsSolver(functions, startingPoint);
            // Получаем решение методом Ньютона
            Matrix computedSolution = solver.Solve();

            StringBuilder message = new StringBuilder();
            if (iteration > 0)
            {
                message.Append("Тест метода Ньютона " + iteration + ": ");
            }

            Matrix value = new Matrix(n, 1);
            for (int i = 0; i < n; i++)
            {
                value.Set(i, 0, functions[i].Evaluate(computedSolution.ColumnToArray()));
            }

            bool passed = false;
            if (value.VectorNorm() < 0.1d)
            {
                message.Append("Решение удовлетворяет уравнениям");
                passed = true;
            }
            else
            {
                message.Append("Решения не удовлетворяют уравнениям");
            }

            Console.WriteLine(message.ToString());
            return passed;
        }

        /// <summary>
        /// Сгенерировать случайный многочлен с одним известным корнем и несколькими вторичными, удалёнными на значительное расстояние от того
        /// </summary>
        /// <param name="random">Объект генератора случайных чисел</param>
        /// <param name="dim">Размерность пространства</param>
        /// <param name="root">Известный корень</param>
        /// <returns>Многочлен</returns>
        static Polynomial GenRandPolynomial(Random random, int dim, Matrix root)
        {
            // Создаём список для известных нулей функции
            List<Matrix> roots = new List<Matrix>();
            roots.Add(root);

            // От искомого корня сдвигаемся на значительное расстояние
            for (int i = 0; i < dim; i++)
            {
                Matrix newRoot = new Matrix(root);
                for (int j = 0; j < dim; j++)
                {
                    newRoot.Set(j, 0, newRoot.Get(j, 0) + random.NextDouble() * 8d + 2d);
                }
            }

            // Список многочленов
            List<Polynomial> polynoms = new List<Polynomial>();

            // Создаём список многочленов-скобок вида (x_i-a), чтобы перемножать их и получать функции с известными корнями
            // Сначала делаем скобки для искомого корня
            for (int i = 0; i < dim; i++)
            {
                polynoms.Add(new Polynomial(new List<Monomial>()
                {
                    new Monomial(new Dictionary<int, int>{ { i, 1 } }, 1)
                }, -root.Get(i, 0)));
            }

            // Затем делаем скобки для вторичных корней
            foreach (Matrix secondaryRoot in roots)
            {
                for (int i = 0; i < random.Next(1, dim + 1); i++)
                {
                    polynoms.Add(new Polynomial(new List<Monomial>()
                    {
                        new Monomial(new Dictionary<int, int>{ { i, 1 } }, 1)
                    }, -secondaryRoot.Get(i, 0)));
                }
            }

            Polynomial finalPolynom = polynoms[0];

            // Перемножаем полиномы-скобки
            for (int i = 1; i < polynoms.Count; i++)
            {
                finalPolynom *= polynoms[i];
            }

            // Делаем из полученного полинома функцию
            return finalPolynom;
        }

        /// <summary>
        /// Проверка метода Зейделя
        /// </summary>
        static void TestSeidel()
        {
            //Выполняем ряд тестов для метода Гаусса
            int testsPassed = 0;
            for (int i = 1; i <= 1000; i++)
            {
                bool passed = TestSeidel(i);
                if (passed)
                {
                    testsPassed++;
                }
            }

            Console.WriteLine("\n" + "Итого пройдено " + testsPassed + " / 1000 тестов");
        }

        /// <summary>
        /// Тест, генерирующий некоторый вектор, предполагаемый решением будущего СЛАУ.
        /// К нему применяется невырожденное преобразование, полученное СЛАУ решается методом Зейделя.
        /// Решение должно совпасть с исходным вектором.
        /// </summary>
        /// <param name="iteration">Номер теста для удобства</param>
        /// <returns>true, если метод Зейделя отработал, как запланировано. Иначе false</returns>
        static bool TestSeidel(int iteration = -1)
        {
            Random random = new Random();
            int n = 3;
            // Случайный вектор с целыми координатами
            Matrix sol = GenRandIntVector(n, random);

            // Объект решения СЛАУ для сравнения с полученным решением
            SLAESolution expectedSolution = new SLAESolution(SLAESolution.SolutionType.UNIQUE_SOLUTION);
            expectedSolution.SetUniqueSolution(sol);

            // Получаем невырожденную матрицу перехода.
            // Используем её как матрицу коэффициентов, умножаем точное решение слева на матрицу перехода
            // для получения вектора правой части
            Matrix transformMatrix = NonSingularTransform3DMatrix(random);
            Matrix rightHandMatrix = sol.MultiplyLeft(transformMatrix);

            // Получаем решение методом Зейделя
            SeidelSLAE slae = new SeidelSLAE(transformMatrix, rightHandMatrix);
            SLAESolution computedSol = slae.Solve();

            // Готовим вывод результатов на экран
            StringBuilder message = new StringBuilder();
            if (iteration > 0)
            {
                message.Append("Тест метода Зейделя " + iteration + ": ");
            }

            bool passed = false;
            if (expectedSolution.EqualsPrecision(computedSol, 0.1))
            {
                message.Append("Решения совпали");
                passed = true;
            }
            else
            {
                message.AppendLine("Решения не совпали");
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
        /// Проверка работы метода Гаусса
        /// </summary>
        static void TestGauss()
        {
            //Выполняем ряд тестов для метода Гаусса
            int testsPassed = 0;
            for (int i = 1; i <= 1000; i++)
            {
                bool passed = TestGauss(i);
                if (passed)
                {
                    testsPassed++;
                }
            }

            Console.WriteLine("\n" + "Итого пройдено " + testsPassed + " / 1000 тестов");
        }

        /// <summary>
        /// Тест, генерирующий некоторый вектор, предполагаемый решением будущего СЛАУ.
        /// К нему применяется невырожденное преобразование, полученное СЛАУ решается методом Гаусса.
        /// Решение должно совпасть с исходным вектором.
        /// </summary>
        /// <param name="iteration">Номер теста для удобства</param>
        /// <returns>true, если метод Гаусса отработал, как запланировано. Иначе false</returns>
        static bool TestGauss(int iteration = -1)
        {
            Random random = new Random();
            int n = 3;
            // Случайный вектор с целыми координатами
            Matrix sol = GenRandIntVector(n, random);

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
            if (iteration > 0)
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
                message.AppendLine("Решения не совпали");
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
                    if (Math.Abs(matrix.Get(i, j)) < 1E-3)
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
                transformMatrix = GenRandIntDiagonalMatrix(3, random)
                    .MultiplyLeft(GetZRotationMatrix(Math.PI / 12 * random.Next(-12, 13)))
                    .MultiplyLeft(GetYRotationMatrix(Math.PI / 12 * random.Next(-12, 13)))
                    .MultiplyLeft(GetXRotationMatrix(Math.PI / 12 * random.Next(-12, 13)));
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
        static Matrix GenRandIntVector(int dim, Random random)
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
        static Matrix GenRandIntDiagonalMatrix(int dim, Random random)
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
        static Matrix GetXRotationMatrix(double angle)
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
        static Matrix GetYRotationMatrix(double angle)
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
        static Matrix GetZRotationMatrix(double angle)
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