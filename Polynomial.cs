using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    /// <summary>
    /// Вспомогательный класс многочленов
    /// </summary>
    class Polynomial
    {
        // Размерность
        private int dim;

        // Степень
        private int power;

        // Список одночленов
        private List<Monomial> monomials;

        // Свободный член
        private double freeNumber;

        /// <summary>
        /// Конструктор многочлена
        /// </summary>
        /// <param name="monomials">Одночлены</param>
        /// <param name="freeNumber">Свободный член</param>
        public Polynomial(List<Monomial> monomials, double freeNumber = 0d)
        {
            this.monomials = monomials;

            RecalculateFields();

            this.freeNumber = freeNumber;
        }

        /// <summary>
        /// Пересчитать степень и размерность
        /// </summary>
        private void RecalculateFields()
        {
            foreach (Monomial mononom in monomials)
            {
                if (mononom.CalculatePower() > power)
                {
                    power = mononom.CalculatePower();
                }

                if (mononom.GetDim() > dim)
                {
                    dim = mononom.GetDim();
                }
            }
        }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="another">Многочлен для копирования</param>
        public Polynomial(Polynomial another) : this(new List<Monomial>(another.monomials), another.freeNumber) { }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Одночлены</returns>
        public List<Monomial> GetMonomials()
        {
            return monomials;
        }

        /// <summary>
        /// Геттер
        /// </summary>
        /// <returns>Свободный член</returns>
        public double GetFreeNumber()
        {
            return freeNumber;
        }

        /// <summary>
        /// Оператор сложения многочленов
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Сумма многочленов</returns>
        public static Polynomial? operator +(Polynomial left, Polynomial right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            Polynomial result = new Polynomial(left);

            foreach (Monomial mononom in right.monomials)
            {
                result.monomials.Add(mononom);
            }

            result.EvaluateHomogeneousMonomials();
            result.RecalculateFields();
            result.freeNumber += right.freeNumber;

            return result;
        }

        /// <summary>
        /// Оператор вычитания многочленов
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Разность многочленов</returns>
        public static Polynomial? operator -(Polynomial left, Polynomial right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            Polynomial result = new Polynomial(left);

            foreach (Monomial mononom in right.monomials)
            {
                result.monomials.Add(-mononom);
            }

            result.EvaluateHomogeneousMonomials();
            result.RecalculateFields();
            result.freeNumber -= right.freeNumber;

            return result;
        }

        /// <summary>
        /// Обращение знаков многочлена
        /// </summary>
        /// <param name="polynomial">Операнд</param>
        /// <returns>Многочлен с противоположными знаками</returns>
        public static Polynomial? operator -(Polynomial polynomial)
        {
            if (polynomial == null)
            {
                return null;
            }

            List<Monomial> mononoms = new List<Monomial>();
            for (int i = 0; i < polynomial.monomials.Count; i++)
            {
                mononoms.Add(-polynomial.monomials[i]);
            }

            return new Polynomial(mononoms, -polynomial.freeNumber);
        }

        /// <summary>
        /// Привести подобные слагаемые
        /// </summary>
        private void EvaluateHomogeneousMonomials()
        {
            List<int> removalIndexes = new List<int>();

            for (int i = 0; i < monomials.Count; i++)
            {
                if (removalIndexes.Contains(i))
                {
                    continue;
                }

                for (int j = i + 1; j < monomials.Count; j++)
                {
                    if (monomials[j].IsHomogeneousTo(monomials[i]))
                    {
                        monomials[i] += monomials[j];
                        removalIndexes.Add(j);
                    }
                }

                if (monomials[i].GetCoefficient() == 0d)
                {
                    removalIndexes.Add(i);
                }
            }

            removalIndexes.Sort();

            if (removalIndexes.Count == 0)
            {
                return;
            }

            for (int i = removalIndexes.Last(); i >= 0; i--)
            {
                if (!removalIndexes.Contains(i))
                {
                    continue;
                }

                monomials.RemoveAt(i);
            }
        }

        /// <summary>
        /// Произведение многочленов
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Произведение многочленов</returns>
        public static Polynomial? operator *(Polynomial left, Polynomial right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            Polynomial result = new Polynomial(new List<Monomial>(), left.freeNumber * right.freeNumber);

            for (int i = 0; i < left.monomials.Count; i++)
            {
                for (int j = 0; j < right.monomials.Count; j++)
                {
                    result.monomials.Add(left.monomials[i] * right.monomials[j]);
                }
            }

            for (int i = 0; i < left.monomials.Count; i++)
            {
                result.monomials.Add(left.monomials[i] * right.freeNumber);
            }

            for (int i = 0; i < right.monomials.Count; i++)
            {
                result.monomials.Add(right.monomials[i] * left.freeNumber);
            }

            result.EvaluateHomogeneousMonomials();
            result.RecalculateFields();

            return result;
        }

        /// <summary>
        /// Вычислить значение многочлена в точке
        /// </summary>
        /// <param name="coordinates">Координаты точки</param>
        /// <returns>Значение многочлена в точке</returns>
        /// <exception cref="ArgumentException">Количество передаваемых координат должно быть не меньше, чем в многочлене</exception>
        public double Evaluate(double[] coordinates)
        {
            if (coordinates == null || coordinates.Length - 1 < dim)
            {
                throw new ArgumentException("Количество передаваемых координат должно быть не меньше, чем в многочлене");
            }

            double result = freeNumber;

            for (int i = 0; i < monomials.Count; i++)
            {
                result += monomials[i].Evaluate(coordinates);
            }

            return result;
        }

        /// <summary>
        /// Преобразование многочлена в функцию
        /// </summary>
        /// <returns>Функция-многочлен</returns>
        public MathFunction ConvertToFunction()
        {
            return new MathFunction((double[] args) =>
            {
                return Evaluate(args);
            });
        }

        /// <summary>
        /// Строковое представление многочлена
        /// </summary>
        /// <returns>Строка</returns>
        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i <= monomials.Count; i++)
            {
                sb.Append("Monomial ");
                sb.Append(i);
                sb.Append(" : ");
                sb.Append(monomials[i - 1].ToString());
                sb.Append("\n");
            }

            sb.Append("free number : ");
            sb.Append(freeNumber);

            return sb.ToString();
        }

        /// <summary>
        /// Функция сравнения многочленов
        /// </summary>
        /// <param name="obj">Многочлен для сравнения</param>
        /// <returns>true, если многочлены равны; иначе false</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            Polynomial another = (Polynomial)obj;

            Polynomial difference = (this - another);

            return difference.GetMonomials().Count == 0 && difference.GetFreeNumber() == 0d;
        }
    }
}
