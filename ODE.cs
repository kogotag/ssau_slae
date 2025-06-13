using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    /// <summary>
    /// Класс для поиска решения системы обыкновенных дифференциальных уравнений методом Рунге-Кутты
    /// </summary>
    class ODE
    {
        // Массив функций
        private MathFunction[] functions;

        // Размерность
        private int dim;

        // Матрица для хранения решения
        private Matrix solution;

        // Номер текущей итерации
        private int iteration;

        // Общее количество итераций
        private int iterationCount;

        // Шаг
        private double step;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="functions">Массив функций</param>
        /// <param name="startConditions">Начальное положение</param>
        /// <param name="startTime">Начальное время</param>
        /// <param name="stopTime">Время остановки</param>
        /// <param name="step">Шаг интегрирования</param>
        public ODE(MathFunction[] functions, Matrix startConditions, double startTime, double stopTime, double step)
        {
            if (functions.Length == 0)
            {
                throw new ArgumentException("Задан пустой массив функций");
            }

            if (stopTime <= startTime)
            {
                throw new ArgumentException("stopTime должен быть больше startTime");
            }

            if (step <= 0d)
            {
                throw new ArgumentException("Шаг должен быть положительным");
            }

            this.functions = functions;
            this.dim = functions.Length;

            if (startConditions.GetRowsCount() != dim)
            {
                throw new ArgumentException("Размерность начального положения должна совпадать с размерностью пространства");
            }

            this.iterationCount = (int)Math.Floor((stopTime - startTime) / step);
            this.step = step;
            this.solution = new Matrix(dim + 1, iterationCount);
            for (int i = 0; i < dim; i++)
            {
                this.solution.Set(i, iteration, startConditions.Get(i, 0));
            }
            this.solution.Set(dim, iteration, startTime);
        }

        /// <summary>
        /// Функция, выполняющая одну итерацию вычислений. Вычисляет следующее положение системы по формулам Рунге-Кутты
        /// </summary>
        private void Iteration()
        {
            // Коэффициенты К1
            Matrix k1 = new Matrix(dim, 1);

            // Заполняем массив аргументов для вызова функций,
            // для первого коэффициента в аргументы записываем предыдущее состояние системы
            double[] args = new double[dim + 1];
            for (int i = 0; i <= dim; i++)
            {
                args[i] = solution.Get(i, iteration - 1);
            }

            // Вычисляем К1
            for (int i = 0; i < dim; i++)
            {
                k1.Set(i, 0, step * functions[i].Evaluate(args));
            }

            // Зная К1, задаём новые аргументы
            args = new double[dim + 1];
            for (int i = 0; i < dim; i++)
            {
                args[i] = solution.Get(i, iteration - 1) + 0.5d * k1.Get(i, 0);
            }
            args[dim] = solution.Get(dim, iteration - 1) + step * 0.5d;

            // Вычисляем К2
            Matrix k2 = new Matrix(dim, 1);
            for(int i = 0; i < dim; i++)
            {
                k2.Set(i, 0, step * functions[i].Evaluate(args));
            }

            // Зная К2, вычисляем новые аргументы
            args = new double[dim + 1];
            for (int i = 0; i < dim; i++)
            {
                args[i] = solution.Get(i, iteration - 1) + 0.5d * k2.Get(i, 0);
            }
            args[dim] = solution.Get(dim, iteration - 1) + step * 0.5d;

            // Вычисляем К3
            Matrix k3 = new Matrix(dim, 1);
            for (int i = 0; i < dim; i++)
            {
                k3.Set(i, 0, step * functions[i].Evaluate(args));
            }

            // Зная К3, вычисляем новые аргументы
            args = new double[dim + 1];
            for (int i = 0; i < dim; i++)
            {
                args[i] = solution.Get(i, iteration - 1) + k3.Get(i, 0);
            }
            args[dim] = solution.Get(dim, iteration - 1) + step;

            // Вычисляем К4
            Matrix k4 = new Matrix(dim, 1);
            for (int i = 0; i < dim; i++)
            {
                k4.Set(i, 0, step * functions[i].Evaluate(args));
            }

            // Зная все коэффициенты, вычисляем новое положение системы
            for (int i = 0; i < dim; i++)
            {
                solution.Set(i, iteration, solution.Get(i, iteration - 1) + 1d / 6 * (k1.Get(i, 0) + 2 * k2.Get(i, 0) + 2 * k3.Get(i, 0) + k4.Get(i, 0)));
            }
            solution.Set(dim, iteration, solution.Get(dim, iteration - 1) + step);
        }

        /// <summary>
        /// Получить решение методом Рунге-Кутты
        /// </summary>
        /// <returns>Матрица, содержащая решение</returns>
        public Matrix SolveRungeKutta45()
        {
            // Выполняем итерации подсчитанное число раз
            for(iteration = 1; iteration < iterationCount; iteration++)
            {
                Iteration();
            }

            // Возвращаем матрицу с решением
            return solution;
        }
    }
}
