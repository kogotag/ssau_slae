using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    /// <summary>
    /// Класс для получения собственных чисел и собственных векторов симметричных матриц методом вращений Якоби
    /// </summary>
    class JacobiRotations
    {
        // Преобразуемая матрица
        private Matrix matrix;

        // Матрица собственных векторов
        private Matrix eigenVectorsMatrix;

        // Размерность
        private int dim;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sourceMatrix">Исходная симметрическая матрица оператора</param>
        /// <exception cref="ArgumentException">Матрица должна быть симметрической</exception>
        public JacobiRotations(Matrix sourceMatrix)
        {
            this.matrix = sourceMatrix;

            if (!sourceMatrix.IsSymmetrical())
            {
                throw new ArgumentException("Матрица должна быть симметрической");
            }

            this.dim = sourceMatrix.GetRowsCount();
            this.eigenVectorsMatrix = Matrix.Identity(dim);
        }

        /// <summary>
        /// Находим максимальный по модулю внедиагональный элемент
        /// </summary>
        /// <returns>Кортеж - номер строки, номер столбца, значение</returns>
        private (int row, int column, double val) FindMaxAbsElement()
        {
            (int row, int column, double val) element = (0, 0, Math.Abs(matrix.Get(0, 0)));

            for (int i = 0; i < dim; i++)
            {
                for (int j = i; j < dim; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (Math.Abs(matrix.Get(i, j)) > element.val)
                    {
                        element = (i, j, Math.Abs(matrix.Get(i, j)));
                    }
                }
            }

            element.val = matrix.Get(element.row, element.column);

            return element;
        }

        /// <summary>
        /// Вычисляем норму, по которой оцениваем, нужно ли заканчивать вычисление
        /// </summary>
        /// <returns>Число</returns>
        private double CalculateNorm()
        {
            double norm = 0d;
            for (int i = 0; i < matrix.GetRowsCount(); i++)
            {
                for (int j = i; j < matrix.GetColumnsCount(); j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    norm += matrix.Get(i, j) * matrix.Get(i, j);
                }
            }

            norm = Math.Sqrt(norm);

            return norm;
        }

        /// <summary>
        /// Вычисляем угол вращения
        /// </summary>
        /// <param name="maxAbsElement">Внедиагональный элемент, максимальный по модулю</param>
        /// <returns></returns>
        private double CalculateAngle((int row, int column, double val) maxAbsElement)
        {
            if (Math.Abs(matrix.Get(maxAbsElement.row, maxAbsElement.row) - matrix.Get(maxAbsElement.column, maxAbsElement.column)) < 0.1d)
            {
                // Если знаменатель близок к нулю, то угол равен пи / 4
                return Math.PI / 4;
            }

            return 0.5d * Math.Atan(2 * (maxAbsElement.val) / (matrix.Get(maxAbsElement.row, maxAbsElement.row) - matrix.Get(maxAbsElement.column, maxAbsElement.column)));
        }

        /// <summary>
        /// Итерация, преобразующая матрицу
        /// </summary>
        private void Iterate() {
            // Находим максимальный по модулю внедиагональный элемент
            (int row, int column, double val) maxAbsElement = FindMaxAbsElement();

            // Вычисляем угол вращения
            double angle = CalculateAngle(maxAbsElement);

            // Составляем матрицу вращения
            Matrix rotationMatrix = Matrix.Identity(dim);
            rotationMatrix.Set(maxAbsElement.row, maxAbsElement.row, Math.Cos(angle));
            rotationMatrix.Set(maxAbsElement.column, maxAbsElement.column, Math.Cos(angle));
            rotationMatrix.Set(maxAbsElement.row, maxAbsElement.column, -Math.Sin(angle));
            rotationMatrix.Set(maxAbsElement.column, maxAbsElement.row, Math.Sin(angle));

            // Преобразуем матрицу собственных вектором, домножая справа на матрицу вращения
            eigenVectorsMatrix = eigenVectorsMatrix.MultiplyRight(rotationMatrix);

            // Преобразуем матрицу, домножая справа на матрицу вращения, а слева на транспонированную матрицу вращения
            matrix = matrix.MultiplyRight(rotationMatrix);
            matrix = matrix.MultiplyLeft(rotationMatrix.Transposed());
        }

        /// <summary>
        /// Получить собственные значения и собственные векторы оператора
        /// </summary>
        /// <param name="precision">Задаваемая точность метода</param>
        /// <returns>Объект решения. Содержит собственные вектора и собственные числа</returns>
        public EigenSolution Solve(double precision = 0.1d)
        {
            // Выполняем итерацию, пока не достигнем требуемой точности
            do
            {
                Iterate();
            } while (CalculateNorm() > precision);

            // Упаковываем собственные значения в вектор-столбец
            Matrix eigenValues = new Matrix(dim, 1);
            for (int i = 0; i < dim; i++)
            {
                eigenValues.Set(i, 0, matrix.Get(i, i));
            }

            // Упаковываем собственные вектора в список вектор-столбцов
            List<Matrix> eigenVectors = new List<Matrix>();
            for (int i = 0; i < dim; i++)
            {
                Matrix eigenVector = new Matrix(dim, 1);
                for (int j = 0; j < dim; j++)
                {
                    eigenVector.Set(j, 0, eigenVectorsMatrix.Get(j, i));
                }
                eigenVectors.Add(eigenVector);
            }

            // Создаём объект решения
            EigenSolution solution = new EigenSolution(eigenVectors, eigenValues);

            // Для отладки можно сохранить конечную итерированную матрицу
            solution.SetLastIterationMatrix(matrix);

            return solution;
        }
    }
}
