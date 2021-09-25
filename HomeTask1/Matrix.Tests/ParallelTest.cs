using NUnit.Framework;

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
        public void SimpleMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(3, 4);
            var secondMatrix = new MatrixMultiplication.Matrix(4, 5);
            TestParallel(firstMatrix, secondMatrix);
        }

        [Test]
        public void NormMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(12, 18);
            var secondMatrix = new MatrixMultiplication.Matrix(18, 8);
            TestParallel(firstMatrix, secondMatrix);
        }
        [Test]
        public void BigMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(45, 43);
            var secondMatrix = new MatrixMultiplication.Matrix(43, 50);
            TestParallel(firstMatrix, secondMatrix);
        }
        [Test]
        public void HugeMatrix()
        {
            var firstMatrix = new MatrixMultiplication.Matrix(184, 123);
            var secondMatrix = new MatrixMultiplication.Matrix(123, 345);
            TestParallel(firstMatrix, secondMatrix);
        }
    }
}