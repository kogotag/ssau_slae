using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    internal class Matrix
    {
        // Внутреннее поле элементов матрицы
        private double[,]? _matrix;

        /// <summary>
        /// Конструктор матрицы по заданным размерам
        /// </summary>
        /// <param name="rows">Количество строк</param>
        /// <param name="columns">Количество столбцов</param>
        public Matrix(int rows, int columns)
        {
            _matrix = new double[rows, columns];
        }

        /// <summary>
        /// Конструктор матрицы по двумерному массиву
        /// </summary>
        /// <param name="matrix">Двумерный массив</param>
        public Matrix(double[,] matrix)
        {
            _matrix = matrix;
        }

        /// <summary>
        /// Конструктор копирования матрицы
        /// </summary>
        /// <param name="matrixOther">Матрица для копирования</param>
        public Matrix(Matrix matrixOther)
        {
            double[,] matrix = new double[matrixOther.GetRowsCount(), matrixOther.GetColumnsCount()];

            for (int i = 0; i < matrixOther.GetRowsCount(); i++)
            {
                for (int j = 0; j < matrixOther.GetColumnsCount(); j++)
                {
                    matrix[i, j] = matrixOther.Get(i, j);
                }
            }
        }

        /// <returns>Количество строк матрицы</returns>
        public int GetRowsCount()
        {
            if (_matrix == null)
            {
                return 0;
            }

            return _matrix.GetLength(0);
        }

        /// <returns>Количество столбцов матрицы</returns>
        public int GetColumnsCount()
        {
            if (_matrix == null)
            {
                return 0;
            }

            return _matrix.GetLength(1);
        }

        /// <summary>
        /// Получить указанный элемент матрицы
        /// </summary>
        /// <param name="row">Номер строки элемента</param>
        /// <param name="column">Номер столбца элемента</param>
        /// <returns>Элемент матрицы</returns>
        public double Get(int row, int column)
        {
            if (_matrix == null)
            {
                return 0;
            }

            return _matrix[row, column];
        }

        /// <summary>
        /// Изменить элемент матрицы
        /// </summary>
        /// <param name="row">Номер строки элемента</param>
        /// <param name="column">Номер столбца элемента</param>
        /// <param name="value">Новое значение элемента</param>
        public void Set(int row, int column, double value)
        {
            if (_matrix == null)
            {
                return;
            }

            _matrix[row, column] = value;
        }

        /// <summary>
        /// Прибавить к одной строке матрицы другую
        /// </summary>
        /// <param name="modifiedRow">Номер строки, к которой прибавляют</param>
        /// <param name="addedRow">Номер прибавляемой строки</param>
        /// <param name="coefficient">Коэффициент, умножающийся на прибавляемую строку перед сложением</param>
        public void SumRow(int modifiedRow, int addedRow, double coefficient)
        {
            if (_matrix == null)
            {
                return;
            }

            for (int i = 0; i < _matrix.GetLength(1); i++)
            {
                _matrix[modifiedRow, i] = _matrix[modifiedRow, i] + coefficient * _matrix[addedRow, i];
            }
        }

        /// <summary>
        /// Прибавить к одному столбцу другой
        /// </summary>
        /// <param name="modifiedColumn">Номер столбца, к которому прибавляют</param>
        /// <param name="addedColumn">Номер прибавляемого столбца</param>
        /// <param name="coefficient">Коэффициент, умножающийся на прибавляемый стобец перед сложением</param>
        public void SumColumns(int modifiedColumn, int addedColumn, double coefficient)
        {
            if (_matrix == null)
            {
                return;
            }

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                _matrix[i, modifiedColumn] = _matrix[i, modifiedColumn] + coefficient * _matrix[i, addedColumn];
            }
        }

        /// <summary>
        /// Поменять строки местами
        /// </summary>
        /// <param name="row1">Первая строка</param>
        /// <param name="row2">Вторая строка</param>
        public void SwapRows(int row1, int row2)
        {
            if (_matrix == null || row1 == row2)
            {
                return;
            }

            double[] temp = new double[_matrix.GetLength(1)];

            for (int i = 0; i < _matrix.GetLength(1); i++)
            {
                temp[i] = _matrix[row1, i];
                _matrix[row1, i] = _matrix[row2, i];
            }

            for (int i = 0; i < _matrix.GetLength(1); i++)
            {
                _matrix[row2, i] = temp[i];
            }
        }

        /// <summary>
        /// Поменять столбцы местами
        /// </summary>
        /// <param name="column1">Первый столбец</param>
        /// <param name="column2">Второй столбец</param>
        public void SwapColumns(int column1, int column2)
        {
            if (_matrix == null || column1 == column2)
            {
                return;
            }

            double[] temp = new double[_matrix.GetLength(0)];

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                temp[i] = _matrix[i, column1];
                _matrix[i, column1] = _matrix[i, column2];
            }

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                _matrix[i, column2] = temp[i];
            }
        }

        /// <summary>
        /// Умножить строку на число
        /// </summary>
        /// <param name="row">Номер умножаемой строки</param>
        /// <param name="coefficient">Множитель</param>
        public void MultiplyRow(int row, double coefficient)
        {
            if (_matrix == null)
            {
                return;
            }

            for (int i = 0; i < _matrix.GetLength(1); i++)
            {
                _matrix[row, i] = _matrix[row, i] * coefficient;
            }
        }

        /// <summary>
        /// Умножить столбец на число
        /// </summary>
        /// <param name="column">Номер умножаемого столбца</param>
        /// <param name="coefficient">Множитель</param>
        public void MultiplyColumn(int column, double coefficient)
        {
            if (_matrix == null)
            {
                return;
            }

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                _matrix[column, i] = _matrix[column, i] * coefficient;
            }
        }

        /// <summary>
        /// Поделить строку на число
        /// </summary>
        /// <param name="row">Номер делимой строки</param>
        /// <param name="coefficient">Делитель</param>
        public void DivideRow(int row, double coefficient)
        {
            if (_matrix == null)
            {
                return;
            }

            for (int i = 0; i < _matrix.GetLength(1); i++)
            {
                _matrix[row, i] = _matrix[row, i] / coefficient;
            }
        }

        /// <summary>
        /// Поделить столбец на число
        /// </summary>
        /// <param name="column">Номер делимого столбца</param>
        /// <param name="coefficient">Делитель</param>
        public void DivideColumn(int column, double coefficient)
        {
            if (_matrix == null)
            {
                return;
            }

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                _matrix[column, i] = _matrix[column, i] / coefficient;
            }
        }

        /// <summary>
        /// Умножение на матрицу слева
        /// </summary>
        /// <param name="multiplier">Матрица-множитель</param>
        /// <returns>Произведение матриц</returns>
        /// <exception cref="ArgumentException">Количество столбцов левой матрицы должно быть равно количеству строк правой</exception>
        public Matrix MultiplyLeft(Matrix multiplier)
        {
            if (multiplier.GetColumnsCount() != GetRowsCount())
            {
                throw new ArgumentException("left matrix columns amount must equal to right matrix rows amount");
            }

            Matrix result = new Matrix(multiplier.GetRowsCount(), GetColumnsCount());

            for (int i = 0; i < multiplier.GetRowsCount(); i++)
            {
                for (int j = 0; j < GetColumnsCount(); j++)
                {
                    for (int r = 0; r < multiplier.GetColumnsCount(); r++)
                    {
                        result.Set(i, j, result.Get(i, j) + multiplier.Get(i, r) * Get(r, j));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Умножение на матрицу справа
        /// </summary>
        /// <param name="multiplier">Матрица-множитель</param>
        /// <returns>Произведение матриц</returns>
        /// <exception cref="ArgumentException">Количество столбцов левой матрицы должно быть равно количеству строк правой</exception>
        public Matrix MultiplyRight(Matrix multiplier)
        {
            if (GetColumnsCount() != multiplier.GetRowsCount())
            {
                throw new ArgumentException("left matrix columns amount must equal to right matrix rows amount");
            }

            Matrix result = new Matrix(GetRowsCount(), multiplier.GetColumnsCount());

            for (int i = 0; i < GetRowsCount(); i++)
            {
                for (int j = 0; j < multiplier.GetColumnsCount(); j++)
                {
                    for (int r = 0; r < GetColumnsCount(); r++)
                    {
                        result.Set(i, j, result.Get(i, j) + Get(i, r) * multiplier.Get(r, j));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Транспонирование матрицы
        /// </summary>
        /// <param name="matrix">Матрица, которую нужно транспонировать</param>
        /// <returns>Транспонированная матрица</returns>
        public Matrix Transposed()
        {
            Matrix transposedMatrix = new Matrix(GetColumnsCount(), GetRowsCount());
            for (int i = 0; i < GetRowsCount(); i++)
            {
                for (int j = 0; j < GetColumnsCount(); j++)
                {
                    transposedMatrix.Set(j, i, Get(i, j));
                }
            }

            return transposedMatrix;
        }

        /// <summary>
        /// Оператор сложения матриц
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Сумма матриц</returns>
        /// <exception cref="ArgumentException">Матрицы должны быть одного размера</exception>
        public static Matrix? operator +(Matrix left, Matrix right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            if (left.GetRowsCount() != right.GetRowsCount() || left.GetColumnsCount() != right.GetColumnsCount())
            {
                throw new ArgumentException("Можно складывать только матрицы с одинаковыми размерами");
            }

            Matrix res = new Matrix(left);

            for (int i = 0; i < left.GetRowsCount(); i++)
            {
                for (int j = 0; j < left.GetColumnsCount(); j++)
                {
                    res.Set(i, j, res.Get(i, j) + right.Get(i, j));
                }
            }

            return res;
        }

        /// <summary>
        /// Оператор вычитания матриц
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Разность матриц</returns>
        /// <exception cref="ArgumentException">Матрицы должны быть одного размера</exception>
        public static Matrix? operator -(Matrix left, Matrix right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            if (left.GetRowsCount() != right.GetRowsCount() || left.GetColumnsCount() != right.GetColumnsCount())
            {
                throw new ArgumentException("Можно вычитать только матрицы с одинаковыми размерами");
            }

            Matrix res = new Matrix(left);

            for (int i = 0; i < left.GetRowsCount(); i++)
            {
                for (int j = 0; j < left.GetColumnsCount(); j++)
                {
                    res.Set(i, j, res.Get(i, j) - right.Get(i, j));
                }
            }

            return res;
        }

        /// <summary>
        /// Унарный оператор вычитания. Обращает знак всех элементов матрицы
        /// </summary>
        /// <param name="matrix">Матрица</param>
        /// <returns>Изменённая матрица</returns>
        public static Matrix? operator -(Matrix matrix)
        {
            if (matrix == null)
            {
                return null;
            }

            Matrix res = new Matrix(matrix);

            for (int i = 0; i < matrix.GetRowsCount(); i++)
            {
                for (int j = 0; j < matrix.GetColumnsCount(); j++)
                {
                    res.Set(i, j, -res.Get(i, j));
                }
            }

            return res;
        }

        /// <summary>
        /// Норма вектора
        /// </summary>
        /// <returns>Норма вектора</returns>
        /// <exception cref="ArgumentException">Матрица должна быть вектором-столбцом</exception>
        public double VectorNorm()
        {
            if (_matrix == null)
            {
                return 0;
            }

            if (_matrix.GetLength(1) != 1)
            {
                throw new ArgumentException("Норма введена только для векторов-столбцов");
            }

            double sum = 0;

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                sum += _matrix[i, 0] * _matrix[i, 0];
            }

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Строковое представление матрицы
        /// </summary>
        /// <returns>Строка</returns>
        public override string? ToString()
        {
            string res = "";

            if (_matrix == null)
            {
                return res;
            }

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    res += _matrix[i, j].ToString();
                    if (j == _matrix.GetLength(1) - 1)
                    {
                        continue;
                    }
                    res += " ";
                }
                if (i == _matrix.GetLength(0) - 1)
                {
                    continue;
                }
                res += "\n";
            }

            return res;
        }

        /// <summary>
        /// Сравнение матриц
        /// </summary>
        /// <param name="obj">Другая матрица</param>
        /// <returns>true, если матрицы равны; иначе false</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            Matrix another = (Matrix)obj;

            if (GetRowsCount() != another.GetRowsCount())
            {
                return false;
            }

            if (GetColumnsCount() != another.GetColumnsCount())
            {
                return false;
            }

            for (int i = 0; i < GetRowsCount(); i++)
            {
                for (int j = 0; j < GetColumnsCount(); j++)
                {
                    if (Get(i, j) != another.Get(i, j))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Сравнение матриц с заданной точностью
        /// </summary>
        /// <param name="obj">Другая матрица</param>
        /// <param name="precision">Точность</param>
        /// <returns>true, если каждая пара соответствующих элементов матриц отличается друг от друга не больше чем на заданное значение точности. Иначе false</returns>
        public bool EqualsPrecision(object? obj, double precision = 0.001)
        {
            if (obj == null)
            {
                return false;
            }

            Matrix another = (Matrix)obj;

            if (GetRowsCount() != another.GetRowsCount())
            {
                return false;
            }

            if (GetColumnsCount() != another.GetColumnsCount())
            {
                return false;
            }

            for (int i = 0; i < GetRowsCount(); i++)
            {
                for (int j = 0; j < GetColumnsCount(); j++)
                {
                    if (Math.Abs(Get(i, j) - another.Get(i, j)) > precision)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Единичная матрица
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <returns>Единичная матрица</returns>
        public static Matrix Identity(int dim)
        {
            double[,] elements = new double[dim, dim];

            for (int i = 0; i < dim; i++)
            {
                elements[i, i] = 1;
            }

            return new Matrix(elements);
        }
    }
}
