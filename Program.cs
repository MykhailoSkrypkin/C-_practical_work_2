using System;
using VectorLibrary;

namespace VectorProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DemoConstructors();
            DemoIndexer();
            DemoArithmetic();
            DemoTrueFalse();
            DemoConversions();
            DemoComparison();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        // 1. Constructors and ToString 

        static void DemoConstructors()
        {
            Header("1. Constructors and output (ToString)");

            Vector byDefault = new Vector();
            Vector zero = new Vector(5);
            Vector fromArray = new Vector(new double[] { 0, 5, 0, 0, 7, 0 });
            Vector copy = new Vector(fromArray);

            Describe("default", byDefault);
            Describe("zero, dimension 5", zero);
            Describe("from an array", fromArray);
            Describe("copy", copy);

            copy[1] = 100;
            Console.WriteLine("After copy[1] = 100 the copy has changed, the original has not:");
            Describe("original", fromArray);
            Describe("copy", copy);
        }

        // 2. Indexer 

        static void DemoIndexer()
        {
            Header("2. Indexer");

            Vector v = new Vector(6);
            v[4] = 7;
            v[1] = 5;
            Describe("v", v);

            Console.WriteLine($"v[1] = {v[1]}, v[2] = {v[2]} (zero is not stored in memory, but it can be read)");

            v[1] = 0;
            Console.WriteLine("Writing zero removes the element from the list:");
            Describe("v after v[1] = 0", v);

            try
            {
                v[10] = 1;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Index out of range: " + ex.Message);
            }
        }

        // 3. Arithmetic operations

        static void DemoArithmetic()
        {
            Header("3. Addition, subtraction, multiplication, dot product");

            Vector a = new Vector(new double[] { 0, 5, 0, 0, 7, 0 });
            Vector b = new Vector(new double[] { 1, 0, 0, 0, 2, 0 });

            ShowOperations(a, b);

            Console.WriteLine();
            Console.WriteLine("Zeros resulting from operations are not stored in memory:");
            Describe("a - a", a - a);

            Console.WriteLine();
            Console.WriteLine("Vectors of different dimensions:");
            try
            {
                Vector c = new Vector(3);
                Vector result = a + c;
                Console.WriteLine(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void ShowOperations(Vector a, Vector b)
        {
            Describe("a", a);
            Describe("b", b);
            Describe("a + b", a + b);
            Describe("a - b", a - b);
            Describe("-a", -a);
            Describe("a * 2", a * 2);
            Describe("2.5 * a", 2.5 * a);
            Console.WriteLine($"a * b (dot product) = {a * b}");
        }

        // 4. true / false operators 

        static void DemoTrueFalse()
        {
            Header("4. true and false operators");

            Vector nonZero = new double[] { 0, 1, 0 };
            Vector zero = nonZero - nonZero;
            Vector none = null;

            if (nonZero) Console.WriteLine($"{nonZero} - non-zero vector: true");
            else Console.WriteLine($"{nonZero} - zero vector: false");

            if (zero) Console.WriteLine($"{zero} - non-zero vector: true");
            else Console.WriteLine($"{zero} - zero vector: false");

            if (none) Console.WriteLine("null - true");
            else Console.WriteLine("null - false");
        }

        // 5. Type conversions

        static void DemoConversions()
        {
            Header("5. Type conversions");

            // Implicit: double[] -> Vector
            Vector v = new double[] { 0, 3, 0, 4 };
            Describe("implicitly from an array", v);

            Vector sum = v + new double[] { 1, 0, 0, 1 };
            Describe("v + array (the array was converted to Vector automatically)", sum);

            // Explicit: Vector -> double[]
            double[] dense = (double[])v;
            Console.WriteLine("Explicitly to an array: " + string.Join("; ", dense));

            // Explicit: Vector -> double (length of the vector)
            double length = (double)v;
            Console.WriteLine($"Explicitly to a number (length of the vector): {length}");
        }

        // 6. Comparison

        static void DemoComparison()
        {
            Header("6. Comparison: == and !=");

            Vector a = new Vector(new double[] { 0, 5, 0, 0, 7, 0 });
            Vector same = new Vector(a);
            Vector otherValue = new Vector(new double[] { 0, 5, 0, 0, 8, 0 });
            Vector otherDimension = new Vector(new double[] { 0, 5, 0 });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"a == copy of a                       : {a == same}");
            Console.WriteLine($"a != copy of a                       : {a != same}");
            Console.WriteLine($"a == {otherValue} : {a == otherValue}");
            Console.WriteLine($"a != {otherValue} : {a != otherValue}");
            Console.WriteLine($"a == {otherDimension} (different dimension) : {a == otherDimension}");
        }

        // Helper methods ----------------------------------------------------

        static void Header(string title)
        {
            Console.WriteLine();
            Console.WriteLine("=== " + title + " ===");
        }

        static void Describe(string name, Vector v)
        {
            Console.WriteLine($"{name} = {v}   [dimension {v.Dimension}, elements in memory: {v.NonZeroCount}]");
        }
    }
}
