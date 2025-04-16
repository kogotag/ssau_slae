using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    class SeidelSLAE
    {
        /// <summary>
        /// Матрица коэффициентов СЛАУ
        /// </summary>
        private Matrix coefficientMatrix;

        /// <summary>
        /// Матрица правой части СЛАУ
        /// </summary>
        private Matrix rightHandMatrix;

        /// <summary>
        /// "Новый" вектор — на конце текущей итерации
        /// </summary>
        private Matrix newVector;

        /// <summary>
        /// "Старый" вектор из предыдущей итерации
        /// </summary>
        private Matrix oldVector;

        /// <summary>
        /// Конструктор СЛАУ для решения методом Зейделя
        /// </summary>
        /// <param name="coefficientMatrix">Матрица коэффициентов СЛАУ</param>
        /// <param name="rightHandMatrix">Матрица правой части СЛАУ</param>
        /// <exception cref="ArgumentNullException">Матрицы не должны быть null</exception>
        /// <exception cref="ArgumentException">Поддерживается решение только квадратных матриц.
        /// Количество строк матрицы коэффициентов должно быть равно количеству строк матрицы правой части</exception>
        public SeidelSLAE(Matrix coefficientMatrix, Matrix rightHandMatrix)
        {
            if (coefficientMatrix == null || rightHandMatrix == null)
            {
                throw new ArgumentNullException();
            }

            if (coefficientMatrix.GetRowsCount() != rightHandMatrix.GetRowsCount())
            {
                throw new ArgumentException("Количество строк матрицы коэффициентов должно быть равно количеству строк матрицы свободных членов");
            }

            if (coefficientMatrix.GetColumnsCount() != coefficientMatrix.GetRowsCount())
            {
                throw new ArgumentException("С помощью данного класса можно решать только системы, в которых количество переменных равно количеству уравнений");
            }

            this.coefficientMatrix = coefficientMatrix;
            this.rightHandMatrix = rightHandMatrix;
            this.newVector = new Matrix(rightHandMatrix.GetRowsCount(), 1);
            this.oldVector = newVector;
        }

        /// <summary>
        /// Конструктор СЛАУ для решения методом Зейделя
        /// </summary>
        /// <param name="coefficientMatrix">Матрица коэффициентов СЛАУ</param>
        /// <param name="rightHandMatrix">Матрица правой части СЛАУ</param>
        public SeidelSLAE(double[,] coefficientMatrix, double[,] rightHandMatrix)
            : this(new Matrix(coefficientMatrix), new Matrix(rightHandMatrix)) { }

        /// <summary>
        /// Метод Зейделя гарантированно работает для нормальных матриц,
        /// так что сделаем матрицу коэффициентов нормальной
        /// </summary>
        private void NormalizeSystem()
        {
            // Транспонированная матрица
            Matrix multiplier = coefficientMatrix.Transposed();

            // Нормализованная матрица коэффициентов
            coefficientMatrix = coefficientMatrix.MultiplyLeft(multiplier);

            // Соответствующая новой матрице коэффициентов, матрица правых частей
            rightHandMatrix = rightHandMatrix.MultiplyLeft(multiplier);
        }

        /// <summary>
        /// Выполнить итерацию метода Зейделя - получить "новый" вектор
        /// </summary>
        private void Iterate()
        {
            // Новый вектор становится старым
            oldVector = newVector;
            for (int i = 0; i < coefficientMatrix.GetRowsCount(); i++)
            {
                // Вычисляем новое значение по формуле Зейделя
                double val = 0;

                for (int j = 0; j < i - 1; j++)
                {
                    val += coefficientMatrix.Get(i, j) * newVector.Get(j, 0);
                }

                for (int j = i + 1; j < coefficientMatrix.GetRowsCount(); j++)
                {
                    val += coefficientMatrix.Get(i, j) * oldVector.Get(j, 0);
                }

                val -= rightHandMatrix.Get(i, 0);
                val *= -1d / coefficientMatrix.Get(i, i);

                // Записываем вычисленное значение в новый вектор
                newVector.Set(i, 0, val);
            }
        }

        /// <summary>
        /// Решение СЛАУ методом Зейделя
        /// </summary>
        /// <param name="precision">Задаваемая точность решения</param>
        /// <returns>Объект решения СЛАУ</returns>
        public SLAESolution Solve(double precision = 0.1d)
        {
            // Сначала делаем матрицу коэффициентов нормальной, не забывая при этом поменять матрицу правых частей
            NormalizeSystem();

            // Затем применяем итерационный метод до достижения требуемой точности
            do
            {
                Iterate();
            } while ((newVector - oldVector).VectorNorm() > precision);

            SLAESolution solution = new SLAESolution(SLAESolution.SolutionType.UNIQUE_SOLUTION);
            solution.SetUniqueSolution(newVector);

            return solution;
        }
    }
}
