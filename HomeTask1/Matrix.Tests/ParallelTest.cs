using NUnit.Framework;
using System;

namespace Matrix.Tests
{
    public class Tests
    {
        /// <summary>
        /// Метод для тестирования корректности параллельного умножения матриц
        /// </summary>
        /// <param name="numberOfStrings1"></param>
        /// <param name="numberOfColumns1"></param>
        /// <param name="numberOfColumns2"></param>
        static void TestParallel(MatrixMultiplication.Matrix firstMatrix,
                                 MatrixMultiplication.Matrix secondMatrix)
        {
            var matrix3 = new MatrixMultiplication.Matrix(firstMatrix.Strings, firstMatrix.Columns);
            var matrix4 = new MatrixMultiplication.Matrix(secondMatrix.Strings, secondMatrix.Columns);
            matrix3 = firstMatrix.ParallelMultiplication(secondMatrix);
            matrix4 = firstMatrix.NonParallelMultiplication(secondMatrix);
            for (int i = 0; i < matrix3.Strings; ++i)
            {
                for (int j = 0; j < matrix3.Columns; ++j)
                {
                    Assert.AreEqual(matrix3.GetElement(i, j), matrix4.GetElement(i, j), $"Error in element [{i}, {j}]");
                }
            }
        }

        [Test]
        public void ItDoesWork()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(2, 2);
            var secondMatrix = new MatrixMultiplication.Matrix(2, 3);
            var resultMatrix = new MatrixMultiplication.Matrix(2, 3);
            var rightMatrix = new MatrixMultiplication.Matrix(2, 3);
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j) {
                    firstMatrix.SetElement(i, j, i + j + 1);
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    secondMatrix.SetElement(i, j, (i + 1) * (j + 1));
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Console.Write(firstMatrix.GetElement(i, j) + " ");
                }
                Console.WriteLine();
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Console.Write(secondMatrix.GetElement(i, j) + " ");
                }
                Console.WriteLine();
            }
            rightMatrix.SetElement(0, 0, 5);
            rightMatrix.SetElement(0, 1, 10);
            rightMatrix.SetElement(0, 2, 15);
            rightMatrix.SetElement(1, 0, 8);
            rightMatrix.SetElement(1, 1, 16);
            rightMatrix.SetElement(1, 2, 30);
            resultMatrix = firstMatrix.ParallelMultiplication(secondMatrix);
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Console.WriteLine(resultMatrix.GetElement(i, j));
                    Assert.AreEqual(resultMatrix.GetElement(i, j), rightMatrix.GetElement(i, j));
                }
            }
        }

        [Test]
        public void NotCorrectSizesNonParallel()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(3, 3);
            var secondMatrix = new MatrixMultiplication.Matrix(4, 5);
            Assert.Throws<Exception>(() => firstMatrix.NonParallelMultiplication(secondMatrix));
        }

        [Test]
        public void NotCorrectSizesParallel()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(3, 3);
            var secondMatrix = new MatrixMultiplication.Matrix(4, 5);
            Assert.Throws<Exception>(() => firstMatrix.ParallelMultiplication(secondMatrix));
        }

        [Test]
        public void SimpleMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(3, 4, true);
            var secondMatrix = new MatrixMultiplication.Matrix(4, 5, true);
            TestParallel(firstMatrix, secondMatrix);
        }

        [Test]
        public void NormMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(12, 18, true);
            var secondMatrix = new MatrixMultiplication.Matrix(18, 8, true);
            TestParallel(firstMatrix, secondMatrix);
        }
        [Test]
        public void BigMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(43, 43, true);
            var secondMatrix = new MatrixMultiplication.Matrix(43, 50, true);
            TestParallel(firstMatrix, secondMatrix);
        }
        [Test]
        public void HugeMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(111, 123, true);
            var secondMatrix = new MatrixMultiplication.Matrix(123, 345, true);
            TestParallel(firstMatrix, secondMatrix);
        }
    }
}