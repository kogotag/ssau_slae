using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    /// <summary>
    /// Класс для хранения собственных чисел и собственных векторов
    /// </summary>
    class EigenSolution
    {
        // Собственные векторы
        private List<Matrix> eigenVectors;

        // Собственные значения
        private Matrix eigenValues;

        // Матрица из последней итерации метода вращений Якоби
        private Matrix lastIterationMatrix;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="eigenVectors">Собственные векторы</param>
        /// <param name="eigenValues">Собственные значения</param>
        public EigenSolution(List<Matrix> eigenVectors, Matrix eigenValues)
        {
            this.eigenVectors = eigenVectors;
            this.eigenValues = eigenValues;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Собственные векторы</returns>
        public List<Matrix> GetEigenVectors()
        {
            return eigenVectors;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Собственные значения</returns>
        public Matrix GetEigenValues()
        {
            return eigenValues;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Матрица из последней итерации метода вращений Якоби</returns>
        public Matrix GetLastIterationMatrix()
        {
            return lastIterationMatrix;
        }

        /// <summary>
        /// Сеттер
        /// </summary>
        /// <param name="lastIterationMatrix">Матрица из последней итерации метода вращений Якоби</param>
        public void SetLastIterationMatrix(Matrix lastIterationMatrix)
        {
            this.lastIterationMatrix = lastIterationMatrix;
        }
    }
}
