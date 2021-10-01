using NUnit.Framework;
using System;

namespace Matrix.Tests
{
    public class Tests
    {
        /// <summary>
        /// Main method for testing parallel multiplication
        /// </summary>
        static void TestParallel(MatrixMultiplication.Matrix firstMatrix,
                                 MatrixMultiplication.Matrix secondMatrix)
        {
            var matrix3 = firstMatrix.ParallelMultiplication(secondMatrix);
            var matrix4 = firstMatrix.NonParallelMultiplication(secondMatrix);
            for (int i = 0; i < matrix3.Rows; ++i)
            {
                for (int j = 0; j < matrix3.Columns; ++j)
                {
                    Assert.AreEqual(matrix3[i, j], matrix4[i, j], $"Error in element [{i}, {j}]");
                }
            }
        }

        [Test]
        public void CorectnessOfMatrixMultiplicationAlgorithmTested()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(2, 2);
            var secondMatrix = new MatrixMultiplication.Matrix(2, 3);
            var rightMatrix = new MatrixMultiplication.Matrix(2, 3);
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j) 
                {
                    firstMatrix[i, j] = i + j + 1;
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    secondMatrix[i, j] = (i + 1) * (j + 1);
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Console.Write(firstMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Console.Write(secondMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            rightMatrix[0, 0] =  5;
            rightMatrix[0, 1] =  10;
            rightMatrix[0, 2] =  15;
            rightMatrix[1, 0] =  8;
            rightMatrix[1, 1] =  16;
            rightMatrix[1, 2] =  30;
            var resultMatrix = firstMatrix.ParallelMultiplication(secondMatrix);
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Console.WriteLine(resultMatrix[i, j]);
                    Assert.AreEqual(resultMatrix[i, j], rightMatrix[i, j]);
                }
            }
        }

        [Test]
        public void NotCorrectSizesNonParallel()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(3, 3);
            var secondMatrix = new MatrixMultiplication.Matrix(4, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => firstMatrix.NonParallelMultiplication(secondMatrix));
        }

        [Test]
        public void NotCorrectSizesParallel()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(3, 3);
            var secondMatrix = new MatrixMultiplication.Matrix(4, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => firstMatrix.ParallelMultiplication(secondMatrix));
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