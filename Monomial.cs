using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    /// <summary>
    /// Вспомогательный класс: одночлен
    /// </summary>
    class Monomial
    {
        // Размерность
        private int dim;

        // Степени переменных
        private Dictionary<int, int> variablesPowers;

        // Числовой коэффициент
        private double coefficient;

        /// <summary>
        /// Конструктор одночлена
        /// </summary>
        /// <param name="variablesPowers">Степени переменных</param>
        /// <param name="coefficient">Числовой коэффициент</param>
        /// <exception cref="ArgumentException">variablesPowers ключи должны быть неотрицательными</exception>
        public Monomial(Dictionary<int, int> variablesPowers, double coefficient)
        {
            this.variablesPowers = variablesPowers;
            this.coefficient = coefficient;

            if (!CheckPowersValidity())
            {
                throw new ArgumentException("variablesPowers ключи должны быть неотрицательными");
            }

            RecalculateFields();
        }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="another">Одночлен для копирования</param>
        public Monomial(Monomial another) : this(new Dictionary<int, int>(another.GetVariablesPowers()), another.GetCoefficient()) { }

        /// <summary>
        /// Пересчитать размерность
        /// </summary>
        private void RecalculateFields()
        {
            foreach (int key in variablesPowers.Keys)
            {
                if (key > dim)
                {
                    dim = key;
                }
            }
        }

        /// <summary>
        /// Проверить, что нет отрицательных индексов переменных
        /// </summary>
        /// <returns>false, если найден отрицательный индекс; иначе true</returns>
        private bool CheckPowersValidity()
        {
            foreach (KeyValuePair<int, int> element in variablesPowers)
            {
                if (element.Key < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Размерность</returns>
        public int GetDim()
        {
            return dim;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Коэффициент</returns>
        public double GetCoefficient()
        {
            return coefficient;
        }

        /// <summary>
        /// Сеттер
        /// </summary>
        /// <param name="coefficient">Коэффициент</param>
        public void SetCoefficient(double coefficient)
        {
            this.coefficient = coefficient;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Степени переменных</returns>
        public Dictionary<int, int> GetVariablesPowers()
        {
            return variablesPowers;
        }

        /// <summary>
        /// Расчитать степень одночлена
        /// </summary>
        /// <returns>Целое число, степень одночлена</returns>
        public int CalculatePower()
        {
            int res = 0;

            foreach (int power in variablesPowers.Values)
            {
                res += power;
            }

            return res;
        }

        /// <summary>
        /// Проверить одночлены на однородность
        /// </summary>
        /// <param name="another">Одночлен для сравнения</param>
        /// <returns>true, если одночлены однородные; иначе false</returns>
        public bool IsHomogeneousTo(Monomial another)
        {
            if (variablesPowers.Count != another.GetVariablesPowers().Count)
            {
                return false;
            }

            foreach (int key in variablesPowers.Keys)
            {
                if (!another.GetVariablesPowers().ContainsKey(key))
                {
                    return false;
                }

                if (another.GetVariablesPowers()[key] != GetVariablesPowers()[key])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Оператор сложения одночленов
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Сумма одночленов</returns>
        /// <exception cref="ArgumentException">Можно складывать только однородные одночлены</exception>
        public static Monomial? operator +(Monomial left, Monomial right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            if (!left.IsHomogeneousTo(right))
            {
                throw new ArgumentException("Можно складывать только однородные одночлены");
            }

            Monomial result = new Monomial(left);
            result.SetCoefficient(left.GetCoefficient() + right.GetCoefficient());
            return result;
        }

        /// <summary>
        /// Оператор разности одночленов
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Разность одночленов</returns>
        /// <exception cref="ArgumentException">Можно вычитать только однородные одночлены</exception>
        public static Monomial? operator -(Monomial left, Monomial right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            if (!left.IsHomogeneousTo(right))
            {
                throw new ArgumentException("Можно вычитать только однородные одночлены");
            }

            Monomial result = new Monomial(left);
            result.SetCoefficient(left.GetCoefficient() - right.GetCoefficient());
            return result;
        }

        /// <summary>
        /// Обратить знак одночлена
        /// </summary>
        /// <param name="monomial">Операнд</param>
        /// <returns>Одночлен с противоположным знаком</returns>
        public static Monomial? operator -(Monomial monomial)
        {
            if (monomial == null)
            {
                return null;
            }

            Monomial result = new Monomial(monomial);
            result.SetCoefficient(-monomial.GetCoefficient());
            return result;
        }

        /// <summary>
        /// Оператор умножения одночленов
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Произведение одночленов</returns>
        public static Monomial? operator *(Monomial left, Monomial right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            Monomial result = new Monomial(left);
            int dim = left.GetDim();
            if (right.GetDim() > dim)
            {
                dim = right.GetDim();
            }

            for (int i = 0; i <= dim; i++)
            {
                int power1 = left.GetVariablesPowers().ContainsKey(i) ? left.GetVariablesPowers()[i] : 0;
                int power2 = right.GetVariablesPowers().ContainsKey(i) ? right.GetVariablesPowers()[i] : 0;
                int power = power1 + power2;
                if (power == 0)
                {
                    continue;
                }

                result.variablesPowers[i] = power;
            }

            result.SetCoefficient(left.GetCoefficient() * right.GetCoefficient());

            result.RecalculateFields();

            return result;
        }

        /// <summary>
        /// Оператор умножения одночлена на число справа
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Произведение одночлена на число</returns>
        public static Monomial? operator *(Monomial left, double right)
        {
            if (left == null)
            {
                return null;
            }

            Monomial result = new Monomial(left);
            result.SetCoefficient(left.GetCoefficient() * right);

            return result;
        }

        /// <summary>
        /// Оператор умножения одночлена на число слева
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Произведение одночлена на число</returns>
        public static Monomial? operator *(double left, Monomial right)
        {
            if (right == null)
            {
                return null;
            }

            Monomial result = new Monomial(right);
            result.SetCoefficient(right.GetCoefficient() * left);

            return result;
        }

        /// <summary>
        /// Вычислить значение одночлена в точке
        /// </summary>
        /// <param name="coordinates">Координаты точки</param>
        /// <returns>Значение одночлена в точке</returns>
        /// <exception cref="ArgumentException">Количество передаваемых координат должно быть не меньше, чем в многочлене</exception>
        public double Evaluate(double[] coordinates)
        {
            if (coordinates == null || coordinates.Length - 1 < dim)
            {
                throw new ArgumentException("Количество передаваемых координат должно быть не меньше, чем в многочлене");
            }

            double result = coefficient;

            for (int i = 0; i <= dim; i++)
            {
                int power = variablesPowers.ContainsKey(i) ? variablesPowers[i] : 0;
                for (int j = 0; j < power; j++)
                {
                    result *= coordinates[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Строковое представление одночлена
        /// </summary>
        /// <returns>Строка</returns>
        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(coefficient);
            sb.Append("; ");

            for (int i = 0; i <= dim; i++)
            {
                if (!variablesPowers.ContainsKey(i))
                {
                    sb.Append(0);
                    sb.Append(", ");
                    continue;
                }

                sb.Append(variablesPowers[i]);
                sb.Append(", ");
            }

            return sb.ToString().Substring(0, sb.Length - 2);
        }

        /// <summary>
        /// Функция сравнения одночленов
        /// </summary>
        /// <param name="obj">Сравниваемый одночлен</param>
        /// <returns>true, если одночлены равны; иначе false</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            Monomial another = (Monomial)obj;

            if (!IsHomogeneousTo(another))
            {
                return false;
            }

            return coefficient == another.coefficient;
        }
    }
}
