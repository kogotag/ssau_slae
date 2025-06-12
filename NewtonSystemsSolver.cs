using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    /// <summary>
    /// Класс поиска решения системы нелинейных уравнений методом Ньютона.
    /// Предполагаем, что начальная точка находится в малой окрестности искомого решения.
    /// В противном случае метод может быть неустойчив
    /// </summary>
    class NewtonSystemsSolver
    {
        /// <summary>
        /// Массив функций
        /// </summary>
        private MathFunction[] functions;

        /// <summary>
        /// Размерность
        /// </summary>
        private int dim;

        /// <summary>
        /// "Новый" вектор — на конце текущей итерации
        /// </summary>
        private Matrix newVector;

        /// <summary>
        /// "Старый" вектор из предыдущей итерации
        /// </summary>
        private Matrix oldVector;

        /// <summary>
        /// Конструктор решателя
        /// </summary>
        /// <param name="functions">Массив функций</param>
        /// <param name="startingPoint">Начальная точка</param>
        /// <exception cref="ArgumentException">Массив функций должен быть непустым</exception>
        public NewtonSystemsSolver(MathFunction[] functions, Matrix startingPoint)
        {
            if (functions.Length == 0)
            {
                throw new ArgumentException("Задан пустой массив функций");
            }

            this.functions = functions;
            this.dim = functions.Length;
            this.newVector = startingPoint;
            this.oldVector = newVector;
        }

        /// <summary>
        /// Выполнение одной итерации. Получение "нового" вектора
        /// </summary>
        public void Iterate()
        {
            // Новый вектор становится старым
            oldVector = newVector;

            // Матрица Якоби
            Matrix jacobianMatrix = new Matrix(dim, dim);

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    // Частные производные вычислены двусторонним методом с шагом по умолчанию 0.01
                    jacobianMatrix.Set(i, j, functions[i].EvaluateDerivative(oldVector.MatrixColumnToArray(), j));
                }
            }

            // Матрица правых частей
            Matrix rightHandMatrix = new Matrix(dim, 1);

            for (int i = 0; i < dim; i++)
            {
                // Матрицу правых частей заполняем значения функций в старых координатах с противоположным знаком
                rightHandMatrix.Set(i, 0, -functions[i].Evaluate(oldVector.MatrixColumnToArray()));
            }

            // Ищем вектор смещения к новому вектору, решая СЛАУ методом Зейделя
            Matrix? deltas = new SeidelSLAE(jacobianMatrix, rightHandMatrix).Solve().GetUniqueSolution();
            if (deltas == null)
            {
                throw new Exception("deltas are null");
            }

            // Вычисляем новый вектор
            newVector = oldVector + deltas;
        }

        /// <summary>
        /// Получить решение системы уравнений методом Ньютона
        /// </summary>
        /// <param name="precision">Задаваемая точность. По умолчанию 0.1</param>
        /// <returns>Вектор-столбец, обращающий систему в тождество с заданной точностью</returns>
        public Matrix Solve(double precision = 0.1d)
        {
            // Матрица значений функций
            Matrix value = new Matrix(dim, 1);

            // Выполняем итерации, пока вектор значений функций дальше от нуля, чем заданная точность
            do
            {
                Iterate();
                for (int i = 0; i < dim; i++)
                {
                    value.Set(i, 0, functions[i].Evaluate(newVector.ColumnToArray()));
                }
            } while (value.VectorNorm() > precision);

            // Возвращаем конечный итерированный вектор как решение
            return newVector;
        }
    }
}