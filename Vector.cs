using System;
using System.Text;

namespace VectorLibrary
{
    public class Vector : IVector
    {
        private const double Epsilon = 1e-9;
        private Node head;

        #region Properties

        public int Dimension { get; private set; }

        public int NonZeroCount
        {
            get
            {
                int count = 0;
                for (Node p = head; p != null; p = p.Next) count++;
                return count;
            }
        }

        public double Norm
        {
            get { return Math.Sqrt(this * this); }
        }

        #endregion

        #region Constructors

        public Vector() : this(1)
        {
        }

        public Vector(int dimension)
        {
            if (dimension < 1)
                throw new ArgumentOutOfRangeException(nameof(dimension), "The dimension must be at least 1.");

            Dimension = dimension;
            head = null;
        }

        public Vector(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length < 1)
                throw new ArgumentException("The array cannot be empty.", nameof(values));

            Dimension = values.Length;
            Node tail = null;
            for (int i = 0; i < values.Length; i++)
                Append(this, ref tail, i, values[i]);
        }

        public Vector(Vector other)
        {
            if (ReferenceEquals(other, null))
                throw new ArgumentNullException(nameof(other));

            Dimension = other.Dimension;
            Node tail = null;
            for (Node p = other.head; p != null; p = p.Next)
                Append(this, ref tail, p.Index, p.Value);
        }

        #endregion

        #region Input / Output

        public void Input()
        {
            int dimension = ReadInt("Dimension of the vector: ", 1, int.MaxValue);
            int count = ReadInt("Number of non-zero elements: ", 0, dimension);

            Dimension = dimension;
            head = null;

            for (int i = 0; i < count; i++)
            {
                int index = ReadInt($"  Index {i + 1}-th element (0..{dimension - 1}): ", 0, dimension - 1);
                double value = ReadDouble($"  Value of element [{index}]: ");
                this[index] = value;
            }
        }

        /// <summary>String representation of the vector: zeros are displayed, even if they are not stored in memory.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");
            Node current = head;

            for (int i = 0; i < Dimension; i++)
            {
                double value = 0;
                if (current != null && current.Index == i)
                {
                    value = current.Value;
                    current = current.Next;
                }

                sb.Append(value);
                if (i < Dimension - 1) sb.Append("; ");
            }

            return sb.Append(")").ToString();
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
                    return result;
                Console.WriteLine("Invalid value, please try again.");
            }
        }

        private static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double result))
                    return result;
                Console.WriteLine("Invalid value, please try again.");
            }
        }

        #endregion

        #region Indexators

        public double this[int index]
        {
            get
            {
                CheckIndex(index);
                for (Node p = head; p != null && p.Index <= index; p = p.Next)
                {
                    if (p.Index == index) return p.Value;
                }
                return 0;
            }
            set
            {
                CheckIndex(index);
                SetValue(index, value);
            }
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= Dimension)
                throw new IndexOutOfRangeException($"Index must be in the range 0..{Dimension - 1}.");
        }

        private void SetValue(int index, double value)
        {
            bool isZero = IsNearZero(value);
            Node prev = null;
            Node current = head;

            while (current != null && current.Index < index)
            {
                prev = current;
                current = current.Next;
            }

            if (current != null && current.Index == index)
            {
                if (isZero)
                {
                    if (prev == null) head = current.Next;
                    else prev.Next = current.Next;
                }
                else
                {
                    current.Value = value;
                }
            }
            else if (!isZero)
            {
                Node node = new Node(index, value, current);
                if (prev == null) head = node;
                else prev.Next = node;
            }
        }

        #endregion

        #region Arithmetic Operations

        public static Vector operator +(Vector a, Vector b)
        {
            return Combine(a, b, 1);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return Combine(a, b, -1);
        }

        public static Vector operator -(Vector v)
        {
            return v * -1.0;
        }

        public static Vector operator *(Vector v, double k)
        {
            if (ReferenceEquals(v, null))
                throw new ArgumentNullException(nameof(v));

            Vector result = new Vector(v.Dimension);
            Node tail = null;
            for (Node p = v.head; p != null; p = p.Next)
                Append(result, ref tail, p.Index, p.Value * k);
            return result;
        }

        public static Vector operator *(double k, Vector v)
        {
            return v * k;
        }

        public static double operator *(Vector a, Vector b)
        {
            CheckDimensions(a, b);

            double sum = 0;
            Node p = a.head;
            Node q = b.head;

            while (p != null && q != null)
            {
                if (p.Index < q.Index) p = p.Next;
                else if (q.Index < p.Index) q = q.Next;
                else
                {
                    sum += p.Value * q.Value;
                    p = p.Next;
                    q = q.Next;
                }
            }

            return sum;
        }

        private static Vector Combine(Vector a, Vector b, int sign)
        {
            CheckDimensions(a, b);

            Vector result = new Vector(a.Dimension);
            Node tail = null;
            Node p = a.head;
            Node q = b.head;

            while (p != null || q != null)
            {
                if (q == null || (p != null && p.Index < q.Index))
                {
                    Append(result, ref tail, p.Index, p.Value);
                    p = p.Next;
                }
                else if (p == null || q.Index < p.Index)
                {
                    Append(result, ref tail, q.Index, sign * q.Value);
                    q = q.Next;
                }
                else
                {
                    Append(result, ref tail, p.Index, p.Value + sign * q.Value);
                    p = p.Next;
                    q = q.Next;
                }
            }

            return result;
        }

        private static void CheckDimensions(Vector a, Vector b)
        {
            if (ReferenceEquals(a, null)) throw new ArgumentNullException(nameof(a));
            if (ReferenceEquals(b, null)) throw new ArgumentNullException(nameof(b));
            if (a.Dimension != b.Dimension)
                throw new ArgumentException("Dimensions of the vectors do not match.");
        }

        private static void Append(Vector target, ref Node tail, int index, double value)
        {
            if (IsNearZero(value)) return;

            Node node = new Node(index, value);
            if (tail == null) target.head = node;
            else tail.Next = node;
            tail = node;
        }

        private static bool IsNearZero(double value)
        {
            return Math.Abs(value) <= Epsilon;
        }

        #endregion

        #region Operations true / false

        public static bool operator true(Vector v)
        {
            return !ReferenceEquals(v, null) && v.head != null;
        }

        public static bool operator false(Vector v)
        {
            return ReferenceEquals(v, null) || v.head == null;
        }

        #endregion

        #region Type conversions

        public static implicit operator Vector(double[] values)
        {
            return new Vector(values);
        }

        public static explicit operator double[](Vector v)
        {
            if (ReferenceEquals(v, null))
                throw new ArgumentNullException(nameof(v));

            double[] result = new double[v.Dimension];
            for (Node p = v.head; p != null; p = p.Next)
                result[p.Index] = p.Value;
            return result;
        }

        public static explicit operator double(Vector v)
        {
            if (ReferenceEquals(v, null))
                throw new ArgumentNullException(nameof(v));

            return v.Norm;
        }

        #endregion

        #region Comparison and hashing

        public static bool operator ==(Vector a, Vector b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            if (a.Dimension != b.Dimension) return false;

            Node p = a.head;
            Node q = b.head;

            while (p != null && q != null)
            {
                if (p.Index != q.Index || Math.Abs(p.Value - q.Value) > Epsilon)
                    return false;
                p = p.Next;
                q = q.Next;
            }

            return p == null && q == null;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public bool Equals(Vector other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vector);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17 * 31 + Dimension;
                for (Node p = head; p != null; p = p.Next)
                    hash = hash * 31 + p.Index;
                return hash;
            }
        }

        #endregion
    }
}