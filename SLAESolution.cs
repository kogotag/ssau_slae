using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    class SLAESolution
    {
        /// <summary>
        /// Тип решения.
        /// </summary>
        public enum SolutionType
        {
            NO_SOLUTIONS, // Нет решений
            UNIQUE_SOLUTION, // Единственное решение
            INFINITY_SOLUTIONS // Бесконечно много решений. Не поддерживается в текущей версии
        }

        private Matrix? uniqueSolution; // Единственное решение
        private SolutionType typeOfSolution; // Тип решения

        /// <summary>
        /// Конструктор решения СЛАУ
        /// </summary>
        /// <param name="typeOfSolution">Тип решения</param>
        public SLAESolution(SolutionType typeOfSolution)
        {
            this.typeOfSolution = typeOfSolution;
        }

        /// <returns>Тип решения</returns>
        public SolutionType GetSolutionType()
        {
            return typeOfSolution;
        }

        /// <summary>
        /// Задать единственное решение
        /// </summary>
        /// <param name="solution">Единственное решение</param>
        public void SetUniqueSolution(Matrix solution)
        {
            uniqueSolution = solution;
        }

        /// <returns>Единственное решение</returns>
        public Matrix? GetUniqueSolution()
        {
            return uniqueSolution;
        }

        /// <summary>
        /// Строковое представление решения СЛАУ
        /// </summary>
        /// <returns>Строка</returns>
        /// <exception cref="Exception">Не было задано единственное решение</exception>
        public override string? ToString()
        {
            if (typeOfSolution == SolutionType.NO_SOLUTIONS)
            {
                return "Система не имеет решений";
            }

            if (typeOfSolution == SolutionType.UNIQUE_SOLUTION)
            {
                if (uniqueSolution == null)
                {
                    throw new Exception("Expected not null solution");
                }

                return "Система имеет единственное решение:\n" + uniqueSolution.ToString();
            }

            return "Для бесконечно большого количества решений вывод ещё не реализован";
        }

        /// <summary>
        /// Сравнение решений СЛАУ
        /// </summary>
        /// <param name="obj">Другое решение</param>
        /// <returns>true, если решения одного типа и совпадают; иначе false</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            SLAESolution another = (SLAESolution)obj;

            if (typeOfSolution != another.typeOfSolution)
            {
                return false;
            }

            if (typeOfSolution == SolutionType.UNIQUE_SOLUTION
                && (uniqueSolution == null || another.uniqueSolution == null || !uniqueSolution.Equals(another.uniqueSolution)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Сравнение решений СЛАУ с заданной точностью
        /// </summary>
        /// <param name="obj">Другое решение</param>
        /// <param name="precision">Точность сравнения</param>
        /// <returns>true, если решения одного типа, и различаются не больше, чем на заданное значение точности</returns>
        public bool EqualsPrecision(object? obj, double precision = 0.001)
        {
            if (obj == null)
            {
                return false;
            }

            SLAESolution another = (SLAESolution)obj;

            if (typeOfSolution != another.typeOfSolution)
            {
                return false;
            }

            if (typeOfSolution == SolutionType.UNIQUE_SOLUTION
                && (uniqueSolution == null || another.uniqueSolution == null || !uniqueSolution.EqualsPrecision(another.uniqueSolution, precision)))
            {
                return false;
            }

            return true;
        }
    }
}