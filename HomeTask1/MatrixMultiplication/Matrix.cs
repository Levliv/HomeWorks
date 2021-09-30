using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MatrixMultiplication
{
    /// <summary>
    /// Матрицы типа int размера n * m
    /// </summary>
    public class Matrix
    {
        private int[,] dataIn2DArray;
        public int Rows { get; set; }
        public int Columns { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Matrix()
        {
            Rows = 0;
            Columns = 0;
            dataIn2DArray = new int[Rows, Columns];
        }

        /// <summary>
        /// Matrix generator
        /// </summary>
        /// <param name="isRandom">Do you want the elements to be generated randomly?</param>
        public Matrix(int numberOfStrings, int numberOfColumns, bool isRandom = false)
        {
            Rows = numberOfStrings;
            Columns = numberOfColumns;
            dataIn2DArray = new int[Rows, Columns];
            if (isRandom)
            {
                var random = new Random();
                for (int i = 0; i < Rows; ++i)
                {
                    for (int j = 0; j < Columns; ++j)
                    {
                        dataIn2DArray[i, j] = random.Next(10);
                    }
                }
            }

        }

        /// <summary>
        /// Reading matrix from file
        /// </summary>
        /// <param name="path">File address</param>
        public Matrix(string path)
        {
            using var streanRider = new StreamReader(path);
            var stringOfMartixSize = streanRider.ReadLine();
            var strings1 = stringOfMartixSize.Split(' ');
            Rows = int.Parse(strings1[0]);
            Columns = int.Parse(strings1[1]);
            dataIn2DArray = new int[Rows, Columns];
            for (int i = 0; i < Rows; ++i)
            {
                string lineFromFile = streanRider.ReadLine();
                string[] strings2 = lineFromFile.Split(' ');
                for (int j = 0; j < Columns; ++j)
                {
                    dataIn2DArray[i, j] = int.Parse(strings2[j]);
                }
            }
        }

        /// <summary>
        /// Getter for [i, j] element from matrix
        /// </summary>
        /// <param name="stringIndex"> Row number </param>
        /// <param name="columnIndex"> Column number </param>
        /// <returns> Returns element in the i-th row and j-th column </returns>
        public int GetElement(int stringIndex, int columnIndex)
        {
            if (stringIndex < 0 || stringIndex >= Rows || columnIndex < 0 || columnIndex >= Columns)
            {
                throw new ArgumentOutOfRangeException();
            }
            return dataIn2DArray[stringIndex, columnIndex];
        }

        /// <summary>
        /// Setter for [i, j] element from matrix
        /// </summary>
        /// <param name="stringIndex"> Row number </param>
        /// <param name="columnIndex"> Column number </param>
        /// <param name="value"> value you wanna place at that position </param>
        public void SetElement(int stringIndex, int columnIndex, int value)
        {
            if (stringIndex < 0 || stringIndex >= Rows || columnIndex < 0 || columnIndex >= Columns)
            {
                throw new ArgumentOutOfRangeException();
            }
            dataIn2DArray[stringIndex, columnIndex] = value;
        }

        /// <summary>
        /// Printing into file
        /// </summary>
        /// <param name="path"> Path to the file you wanna see the matrix in</param>
        public void Print(string path)
        {
            using var sWriter = new StreamWriter(path);
            sWriter.WriteLine(Rows + " " + Columns);
            for (int i = 0; i < Rows; ++i)
            {
                var str = string.Empty;
                for (int j = 0; j < Columns; ++j)
                {
                    str += dataIn2DArray[i, j] + " ";
                }
                sWriter.WriteLine(str);
            }
            sWriter.WriteLine();
            sWriter.Close();
        }

        /// <summary>
        /// Showing matrix on the screen
        /// </summary>
        public void Print()
        {
            for (int i = 0; i < Rows; ++i)
            {
                var str = string.Empty;
                for (int j = 0; j < Columns; ++j)
                {
                    str += dataIn2DArray[i, j] + " ";
                }
                Console.WriteLine(str);
            }
        }

        /// <summary>
        /// Parallel matrix multiplication
        /// </summary>
        /// <param name="rhsMatrix"> The right matrix that will be multiplied </param>
        /// <returns> Matrix that equals the result of multiplication this 2 matrixes </returns>
        public Matrix ParallelMultiplication(Matrix rhsMatrix)
        {
            if (Columns != rhsMatrix.Rows)
            {
                throw new ArgumentOutOfRangeException("Sizes are not appropriate");
            }
            var resultMatrix = new Matrix(Rows, rhsMatrix.Columns);
            var numberOfThreads = Environment.ProcessorCount;
            var threads = new Thread[numberOfThreads];
            int chunkSize = this.Rows / numberOfThreads + 1;
            for (int i = 0; i < numberOfThreads; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (int t = chunkSize * localI; t < chunkSize * (localI + 1) && t < this.Rows; ++t)
                    {
                        for (int j = 0; j < rhsMatrix.Columns; ++j)
                        {
                            var sum = 0;
                            for (int k = 0; k < rhsMatrix.Rows; ++k)
                            {
                                sum += this.GetElement(t, k) * rhsMatrix.GetElement(k, j);
                            }
                            resultMatrix.SetElement(t, j, sum);
                        }
                    }
                });
            }
            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            return resultMatrix;
        }

        /// <summary>
        /// NON parallel matrix multiplication
        /// </summary>
        /// <param name="rhsMatrix"> The right matrix that will be multiplied </param>
        /// <returns> Matrix that equals the result of multiplication this 2 matrixes </returns>
        public Matrix NonParallelMultiplication(Matrix rhsMatrix)
        {
            if (Columns != rhsMatrix.Rows)
            {
                throw new ArgumentOutOfRangeException("Sizes are not appropriate");
            }
            Matrix resultMatrix = new Matrix(Rows, rhsMatrix.Columns);
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < rhsMatrix.Columns; ++j)
                {
                    var sum = 0;
                    for (int k = 0; k < rhsMatrix.Rows; ++k)
                    {
                        sum += GetElement(i, k) * rhsMatrix.GetElement(k, j);
                    }
                    resultMatrix.SetElement(i, j, sum);
                }
            }
            return resultMatrix;
        }
    }
}
