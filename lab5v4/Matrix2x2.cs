using System;

namespace lab5v4
{
    public class Matrix2x2
    {
        private int[,] _matrix = new int[2, 2];

        public Matrix2x2() { }

        public Matrix2x2(int m00, int m01, int m10, int m11)
        {
            _matrix[0, 0] = m00;
            _matrix[0, 1] = m01;
            _matrix[1, 0] = m10;
            _matrix[1, 1] = m11;
        }

        public int this[int row, int col]
        {
            get
            {
                ValidateIndices(row, col);
                return _matrix[row, col];
            }
            set
            {
                ValidateIndices(row, col);
                _matrix[row, col] = value;
            }
        }

        private void ValidateIndices(int row, int col)
        {
            if (row < 0 || row > 1 || col < 0 || col > 1)
            {
                throw new IndexOutOfRangeException("Індекси матриці 2x2 мають бути 0 або 1.");
            }
        }

        public static Matrix2x2 operator +(Matrix2x2 a, Matrix2x2 b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException("Матриці не могут бути null.");

            return new Matrix2x2(
                a[0, 0] + b[0, 0], a[0, 1] + b[0, 1],
                a[1, 0] + b[1, 0], a[1, 1] + b[1, 1]
            );
        }

        public static Matrix2x2 operator *(Matrix2x2 a, int scalar)
        {
            if (a is null)
                throw new ArgumentNullException("Матриця не може бути null.");

            return new Matrix2x2(
                a[0, 0] * scalar, a[0, 1] * scalar,
                a[1, 0] * scalar, a[1, 1] * scalar
            );
        }

        public static Matrix2x2 operator *(int scalar, Matrix2x2 a)
        {
            return a * scalar;
        }

        public static bool operator ==(Matrix2x2? a, Matrix2x2? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            return a.Equals(b);
        }

        public static bool operator !=(Matrix2x2? a, Matrix2x2? b)
        {
            return !(a == b);
        }

        public Matrix2x2 Transpose()
        {
            return new Matrix2x2(
                _matrix[0, 0], _matrix[1, 0],
                _matrix[0, 1], _matrix[1, 1]
            );
        }

        public override bool Equals(object? obj)
        {
            if (obj is Matrix2x2 other)
            {
                return _matrix[0, 0] == other[0, 0] &&
                       _matrix[0, 1] == other[0, 1] &&
                       _matrix[1, 0] == other[1, 0] &&
                       _matrix[1, 1] == other[1, 1];
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_matrix[0, 0], _matrix[0, 1], _matrix[1, 0], _matrix[1, 1]);
        }

        public override string ToString()
        {
            return $"[{_matrix[0, 0]}, {_matrix[0, 1]}]\n[{_matrix[1, 0]}, {_matrix[1, 1]}]";
        }
    }
}