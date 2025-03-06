using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssau_slae
{
    internal class SLAE
    {
        /// <summary>
        /// Расширенная матрица СЛАУ.
        /// Матрица коэффициентов, к которой справа приписали столбец правой части.
        /// </summary>
        private Matrix expandedMatrix;

        /// <summary>
        /// Конструктор СЛАУ
        /// </summary>
        /// <param name="coefficientMatrix">Матрица коэффициентов</param>
        /// <param name="rightHandMatrix">Матрица правых частей</param>
        /// <exception cref="ArgumentNullException">Матрицы не должны быть null</exception>
        /// <exception cref="ArgumentException">Поддерживается решение только квадратных матриц.
        /// Количество строк матрциы коэффициентов должно быть равно количеству строк матрицы правой части</exception>
        public SLAE(Matrix coefficientMatrix, Matrix rightHandMatrix)
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

            expandedMatrix = BuildExpandedMatrix(coefficientMatrix, rightHandMatrix);
        }

        /// <summary>
        /// Конструктор СЛАУ
        /// </summary>
        /// <param name="coefficientMatrix">Двумерный массив коэффициентов</param>
        /// <param name="rightHandMatrix">Двумерный массив-столбец правой части</param>
        public SLAE(double[,] coefficientMatrix, double[,] rightHandMatrix)
            : this(new Matrix(coefficientMatrix), new Matrix(rightHandMatrix)) { }

        /// <summary>
        /// Построить расширенную матрицу
        /// </summary>
        /// <param name="coefficientMatrix">Матрица коэффициентов</param>
        /// <param name="rightHandMatrix">Матрица правой части</param>
        /// <returns>Расширенная матрица СЛАУ</returns>
        public static Matrix BuildExpandedMatrix(Matrix coefficientMatrix, Matrix rightHandMatrix)
        {
            Matrix res = new Matrix(coefficientMatrix.GetRowsCount(), coefficientMatrix.GetColumnsCount() + 1);
            for (int i = 0; i < coefficientMatrix.GetRowsCount(); i++)
            {
                for (int j = 0; j < coefficientMatrix.GetColumnsCount(); j++)
                {
                    res.Set(i, j, coefficientMatrix.Get(i, j));
                }
            }

            for (int i = 0; i < coefficientMatrix.GetRowsCount(); i++)
            {
                res.Set(i, coefficientMatrix.GetColumnsCount(), rightHandMatrix.Get(i, 0));
            }

            return res;
        }

        /// <summary>
        /// Выделить матрицу правой части из текущей расширенной матрицы
        /// </summary>
        /// <returns>Матрица правой части</returns>
        private Matrix ExtractRightHandMatrix()
        {
            Matrix res = new Matrix(expandedMatrix.GetRowsCount(), 1);
            for (int i = 0; i < expandedMatrix.GetRowsCount(); i++)
            {
                res.Set(i, 0, expandedMatrix.Get(i, expandedMatrix.GetColumnsCount() - 1));
            }

            return res;
        }

        /// <summary>
        /// Сделать единичку на заданной позиции в расширенной матрице.
        /// Сначала пытаемся поделить заданную строку. Если в этом месте 0, пытаемся прибавлять строки.
        /// При движении вниз можно использовать только строки ниже. При движении вверх аналогично
        /// </summary>
        /// <param name="row">Номер строки</param>
        /// <param name="column">Номер столбца</param>
        /// <param name="down">Направление. true - вниз, false - вверх</param>
        private void MakeOneAtPositionWithRows(int row, int column, bool down = true)
        {
            // Сначала пытаемся поделить строку
            if (expandedMatrix.Get(row, column) != 0f)
            {
                expandedMatrix.DivideRow(row, expandedMatrix.Get(row, column));
                return;
            }

            // Номер строки с ненулевым элементов в указанном столбце
            int notNullElementRowNumber = -1;

            // Пытаемся найти строку, в которой есть ненулевой элемент в нужном столбце
            if (down)
            {
                for (int i = row + 1; i < expandedMatrix.GetRowsCount(); i++)
                {
                    if (expandedMatrix.Get(i, column) != 0)
                    {
                        notNullElementRowNumber = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = row - 1; i >= 0; i--)
                {
                    if (expandedMatrix.Get(i, column) != 0)
                    {
                        notNullElementRowNumber = i;
                        break;
                    }
                }
            }

            // Если не нашли, выходим из метода
            if (notNullElementRowNumber == -1)
            {
                return;
            }

            // Если нашли, делаем единичку сложением строк
            expandedMatrix.SumRow(row, notNullElementRowNumber, 1d / expandedMatrix.Get(notNullElementRowNumber, column));
        }

        /// <summary>
        /// Делаем матрицу коэффициентов нижне-треугольной, преобразовывая при этом расширенную матрицу.
        /// Этот метод для одного указанного столбца
        /// </summary>
        /// <param name="column">Номер столбца</param>
        private void TriangularizeColumnDown(int column)
        {
            // Делаем единичку на диагонали
            MakeOneAtPositionWithRows(column, column);

            // Если не получилось, выходим из метода
            if (expandedMatrix.Get(column, column) == 0)
            {
                return;
            }

            // Если получилось, вычитаем из нижних строк нужное количество текущей строки
            for (int i = column + 1; i < expandedMatrix.GetRowsCount(); i++)
            {
                expandedMatrix.SumRow(i, column, -1d * expandedMatrix.Get(i, column));
            }
        }

        /// <summary>
        /// Делаем матрицу коэффициентов диагональной (после нижне-треугольной), преобразовывая при этом расширенную матрицу.
        /// Этот метод для одного указанного столбца
        /// </summary>
        /// <param name="column">Номер столбца</param>
        private void TriangularizeColumnUp(int column)
        {
            // Делаем единичку на диагонали
            MakeOneAtPositionWithRows(column, column, false);

            // Если не получилось, выходим из метода
            if (expandedMatrix.Get(column, column) == 0)
            {
                return;
            }

            // Если получилось, вычитаем из верхних строк нужное количество текущей строки
            for (int i = column - 1; i >= 0; i--)
            {
                expandedMatrix.SumRow(i, column, -1d * expandedMatrix.Get(i, column));
            }
        }

        /// <summary>
        /// Делаем матрицу коэффициентов нижне-треугольной, преобразовывая при этом расширенную матрицу
        /// </summary>
        private void TriangularizeDown()
        {
            for (int i = 0; i < expandedMatrix.GetColumnsCount() - 1; i++)
            {
                TriangularizeColumnDown(i);
            }
        }

        /// <summary>
        /// Делаем матрицу коэффициентов диагональной (после нижне-треугольной), преобразовывая при этом расширенную матрицу
        /// </summary>
        private void TriangularizeUp()
        {
            for (int i = expandedMatrix.GetColumnsCount() - 2; i >= 0; i--)
            {
                TriangularizeColumnUp(i);
            }
        }

        /// <summary>
        /// Делаем матрицу коэффициентов диагональной (после нижне-треугольной), преобразовывая при этом расширенную матрицу
        /// </summary>
        private void Triangularize()
        {
            TriangularizeDown();
            TriangularizeUp();
        }

        /// <summary>
        /// Проверка на несовместное уравнение. Например, 0 = 1
        /// </summary>
        /// <returns>true, если не найдено несовместное уравнение; иначе false</returns>
        private bool ContainsInconsistentEquation()
        {
            for (int i = 0; i < expandedMatrix.GetRowsCount(); i++)
            {
                bool zeroRow = true;
                for (int j = 0; j < expandedMatrix.GetColumnsCount() - 1; j++)
                {
                    if (expandedMatrix.Get(i, j) != 0)
                    {
                        zeroRow = false;
                        break;
                    }
                }

                if (!zeroRow)
                {
                    continue;
                }

                if (expandedMatrix.Get(i, expandedMatrix.GetColumnsCount() - 1) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Решение методом Гаусса.
        /// Есть проверка на несовместное уравнение (например 0 = 1). Но не проверяются столбцы.
        /// Также нет проверки, если решений бесконечно много
        /// </summary>
        /// <returns>Объект решения СЛАУ</returns>
        public SLAESolution SolveGauss()
        {
            // Приводим матрицу коэффициентов к диагональному виду
            Triangularize();

            // Если есть несовместное уравнение, возвращаем "нет решений"
            if (ContainsInconsistentEquation())
            {
                return new SLAESolution(SLAESolution.SolutionType.NO_SOLUTIONS);
            }

            // Иначе делаем вид, что решение единственно, и возвращаем его
            SLAESolution slaeSolution = new SLAESolution(SLAESolution.SolutionType.UNIQUE_SOLUTION);
            slaeSolution.SetUniqueSolution(ExtractRightHandMatrix());

            return slaeSolution;
        }
    }
}