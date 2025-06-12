using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    class MathFunction
    {
        // Математическое выражение, переводящее массив аргументов в значение функции
        private Func<double[], double> expression;

        /// <summary>
        /// Конструктор математической функции
        /// </summary>
        /// <param name="expression">Выражение функции. Принимает массив чисел, возвращает одно число</param>
        public MathFunction(Func<double[], double> expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Вычислить значение функции в точке
        /// </summary>
        /// <param name="arguments">Координаты точки</param>
        /// <returns>Значение функции в точке</returns>
        public double Evaluate(double[] arguments)
        {
            return expression.Invoke(arguments);
        }

        /// <summary>
        /// Вычислить частную производную в точке
        /// </summary>
        /// <param name="coordinates">Координаты точки</param>
        /// <param name="indexOfVariable">Индекс переменной, по которой вычисляется частная производная</param>
        /// <param name="step">Шаг, отступаемый от точки в оба направления</param>
        /// <returns>Значение частной производной в точке</returns>
        public double EvaluateDerivative(double[] coordinates, int indexOfVariable, double step = 0.01d)
        {
            double[] x2 = new double[coordinates.Length];
            double[] x1 = new double[coordinates.Length];
            Array.Copy(coordinates, x2, coordinates.Length);
            Array.Copy(coordinates, x1, coordinates.Length);
            x2[indexOfVariable] = coordinates[indexOfVariable] + step;
            x1[indexOfVariable] = coordinates[indexOfVariable] - step;

            return (Evaluate(x2) - Evaluate(x1)) / (step * 2);
        }
    }
}
