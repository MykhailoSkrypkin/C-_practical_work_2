using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorLibrary
{
    public interface IVector
    {
        public int Dimension { get; }
        public int NonZeroCount { get; }
        public double Norm { get; }

        // Конструктори
        public Vector();
        public Vector(int dimension);
        public Vector(double[] values);
        public Vector(Vector other);

        // Введення та виведення
        public void Input();
        public override string ToString();

        // Індексатор
        public double this[int index] { get; set; }

        // Арифметичні операції
        public static Vector operator +(Vector a, Vector b);
        public static Vector operator -(Vector a, Vector b);
        public static Vector operator -(Vector v);
        public static Vector operator *(Vector v, double k);
        public static Vector operator *(double k, Vector v);
        public static double operator *(Vector a, Vector b);

        // Операції true / false
        public static bool operator true(Vector v);
        public static bool operator false(Vector v);

        // Приведення типів
        public static implicit operator Vector(double[] values);
        public static explicit operator double[](Vector v);
        public static explicit operator double(Vector v);

        // Порівняння
        public static bool operator ==(Vector a, Vector b);
        public static bool operator !=(Vector a, Vector b);
        public bool Equals(Vector other);
        public override bool Equals(object obj);
        public override int GetHashCode();
    }
}
