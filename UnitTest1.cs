using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using VectorLibrary;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static Vector V(params double[] values)
        {
            return new Vector(values);
        }

        private static bool AsBool(Vector v)
        {
            if (v) return true;
            return false;
        }

        [TestMethod]
        public void DefaultConstructor_CreatesZeroVectorOfDimensionOne()
        {
            Vector v = new Vector();

            Assert.AreEqual(1, v.Dimension);
            Assert.AreEqual(0, v.NonZeroCount);
            Assert.AreEqual(0.0, v[0]);
        }

        [TestMethod]
        public void DimensionConstructor_CreatesZeroVectorOfGivenDimension()
        {
            Vector v = new Vector(5);

            Assert.AreEqual(5, v.Dimension);
            Assert.AreEqual(0, v.NonZeroCount);
        }

        [TestMethod]
        public void DimensionConstructor_NonPositiveDimension_Throws()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Vector(0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Vector(-3));
        }

        [TestMethod]
        public void ArrayConstructor_StoresOnlyNonZeroElements()
        {
            Vector v = V(0, 5, 0, 0, 7, 0);

            Assert.AreEqual(6, v.Dimension);
            Assert.AreEqual(2, v.NonZeroCount);
        }

        [TestMethod]
        public void ArrayConstructor_NullOrEmptyArray_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Vector((double[])null));
            Assert.ThrowsException<ArgumentException>(() => new Vector(new double[0]));
        }

        [TestMethod]
        public void CopyConstructor_CreatesIndependentCopy()
        {
            Vector original = V(0, 5, 0, 0, 7, 0);
            Vector copy = new Vector(original);

            Assert.AreEqual(original, copy);

            copy[1] = 100;

            Assert.AreEqual(5.0, original[1]);
            Assert.AreEqual(100.0, copy[1]);
        }

        [TestMethod]
        public void ToString_PrintsZeroCoordinatesToo()
        {
            Assert.AreEqual("(0; 5; 0; 0; 7; 0)", V(0, 5, 0, 0, 7, 0).ToString());
            Assert.AreEqual("(0; 0; 0)", new Vector(3).ToString());
        }

        [TestMethod]
        public void Input_ReadsVectorFromConsole()
        {
            TextReader oldIn = Console.In;
            TextWriter oldOut = Console.Out;
            try
            {
                // Розмірність 6, два ненульові елементи: [1] = 5 та [4] = 7.
                Console.SetIn(new StringReader("6\n2\n1\n5\n4\n7\n"));
                Console.SetOut(new StringWriter());

                Vector v = new Vector();
                v.Input();

                Assert.AreEqual(6, v.Dimension);
                Assert.AreEqual(2, v.NonZeroCount);
                Assert.AreEqual("(0; 5; 0; 0; 7; 0)", v.ToString());
            }
            finally
            {
                Console.SetIn(oldIn);
                Console.SetOut(oldOut);
            }
        }

        [TestMethod]
        public void Indexer_Get_ReturnsStoredValueOrZero()
        {
            Vector v = V(0, 5, 0, 0, 7, 0);

            Assert.AreEqual(5.0, v[1]);
            Assert.AreEqual(7.0, v[4]);
            Assert.AreEqual(0.0, v[0]);
            Assert.AreEqual(0.0, v[3]);
        }

        [TestMethod]
        public void Indexer_SetNonZero_InsertsNodesInOrder()
        {
            Vector v = new Vector(6);

            v[4] = 7;
            v[1] = 5;
            v[2] = 3;
            v[4] = 9;   

            Assert.AreEqual(3, v.NonZeroCount);
            Assert.AreEqual("(0; 5; 3; 0; 9; 0)", v.ToString());
        }

        [TestMethod]
        public void Indexer_SetZero_RemovesNodeFromMemory()
        {
            Vector v = V(0, 5, 0, 0, 7, 0);

            v[1] = 0;
            v[0] = 0;   

            Assert.AreEqual(1, v.NonZeroCount);
            Assert.AreEqual("(0; 0; 0; 0; 7; 0)", v.ToString());
        }

        [TestMethod]
        public void Indexer_OutOfRange_Throws()
        {
            Vector v = new Vector(4);

            Assert.ThrowsException<IndexOutOfRangeException>(() => { _ = v[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { _ = v[4]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { v[10] = 1; });
        }


        [TestMethod]
        public void Addition_ReturnsSumOfVectors()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);
            Vector b = V(1, 0, 0, 0, 2, 0);

            Vector sum = a + b;

            Assert.AreEqual(V(1, 5, 0, 0, 9, 0), sum);
            Assert.AreEqual(3, sum.NonZeroCount);
        }

        [TestMethod]
        public void Addition_ResultingZeros_AreNotStored()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);

            Vector zero = a + (-a);
            Vector partial = V(1, 2, 3) + V(-1, 2, -3);

            Assert.AreEqual(0, zero.NonZeroCount);
            Assert.AreEqual(6, zero.Dimension);
            Assert.AreEqual(1, partial.NonZeroCount);
            Assert.AreEqual("(0; 4; 0)", partial.ToString());
        }

        [TestMethod]
        public void Subtraction_ReturnsDifferenceOfVectors()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);
            Vector b = V(1, 0, 0, 0, 2, 0);

            Assert.AreEqual(V(-1, 5, 0, 0, 5, 0), a - b);
            Assert.AreEqual(0, (a - a).NonZeroCount);
        }

        [TestMethod]
        public void Operations_DifferentDimensions_Throw()
        {
            Vector a = V(1, 2, 3);
            Vector b = V(1, 2);

            Assert.ThrowsException<ArgumentException>(() => { _ = a + b; });
            Assert.ThrowsException<ArgumentException>(() => { _ = a - b; });
            Assert.ThrowsException<ArgumentException>(() => { _ = a * b; });
        }

        [TestMethod]
        public void MultiplicationByNumber_WorksInBothOrders()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);

            Assert.AreEqual(V(0, 10, 0, 0, 14, 0), a * 2);
            Assert.AreEqual(V(0, 10, 0, 0, 14, 0), 2 * a);

            Vector zero = a * 0;
            Assert.AreEqual(0, zero.NonZeroCount);
            Assert.AreEqual(6, zero.Dimension);
        }

        [TestMethod]
        public void DotProduct_ReturnsScalarProduct()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);
            Vector b = V(1, 0, 0, 0, 2, 0);

            Assert.AreEqual(14.0, a * b, 1e-9);        
            Assert.AreEqual(74.0, a * a, 1e-9);        
            Assert.AreEqual(0.0, a * V(1, 0, 2, 3, 0, 4), 1e-9);   
        }

        [TestMethod]
        public void UnaryMinus_ReturnsOppositeVector()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);

            Assert.AreEqual(V(0, -5, 0, 0, -7, 0), -a);
        }

        [TestMethod]
        public void TrueFalse_NonZeroVectorIsTrue_ZeroVectorIsFalse()
        {
            Assert.IsTrue(AsBool(V(0, 5, 0)));
            Assert.IsFalse(AsBool(new Vector(3)));
            Assert.IsFalse(AsBool(null));
        }

        [TestMethod]
        public void ImplicitConversion_ArrayToVector()
        {
            Vector v = new double[] { 0, 1, 0 };
            Vector sum = V(1, 1, 1) + new double[] { 0, 2, 0 };

            Assert.AreEqual(3, v.Dimension);
            Assert.AreEqual(1, v.NonZeroCount);
            Assert.AreEqual(V(1, 3, 1), sum);
        }

        [TestMethod]
        public void ExplicitConversion_VectorToArray()
        {
            double[] dense = (double[])V(0, 5, 0, 0, 7, 0);

            CollectionAssert.AreEqual(new double[] { 0, 5, 0, 0, 7, 0 }, dense);
        }

        [TestMethod]
        public void ExplicitConversion_VectorToDouble_ReturnsLength()
        {
            Assert.AreEqual(5.0, (double)V(3, 0, 4), 1e-9);
            Assert.AreEqual(5.0, V(3, 4).Norm, 1e-9);
        }

        [TestMethod]
        public void EqualityOperators_CompareDimensionAndCoordinates()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);

            Assert.IsTrue(a == V(0, 5, 0, 0, 7, 0));
            Assert.IsFalse(a != V(0, 5, 0, 0, 7, 0));
            Assert.IsTrue(a != V(0, 5, 0, 0, 8, 0));        
            Assert.IsTrue(V(1, 0) != V(1, 0, 0));           
            Assert.IsTrue(V(0, 0, 0) == new Vector(3));     
        }

        [TestMethod]
        public void EqualityOperators_HandleNull()
        {
            Vector a = V(1, 2, 3);
            Vector nothing = null;

            Assert.IsTrue(nothing == null);
            Assert.IsFalse(a == null);
            Assert.IsTrue(a != null);
            Assert.IsFalse(nothing == a);
        }

        [TestMethod]
        public void EqualsAndGetHashCode_AreConsistentWithEquality()
        {
            Vector a = V(0, 5, 0, 0, 7, 0);
            Vector b = V(0, 5, 0, 0, 7, 0);

            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a.Equals("not a vector"));
        }
    }
}
