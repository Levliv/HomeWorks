using System;
using System.IO;
using System.Threading;

namespace MatrixMultiplication
{

    /// <summary>
    /// Матрицы типа int размера n * m
    /// </summary>
    public class Matrix
    {
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
            using var streamReader = new StreamReader(path);
            var stringOfMartixSize = streamReader.ReadLine();
            var strings1 = stringOfMartixSize.Split(' ');
            Rows = int.Parse(strings1[0]);
            Columns = int.Parse(strings1[1]);
            dataIn2DArray = new int[Rows, Columns];
            for (int i = 0; i < Rows; ++i)
            {
                string lineFromFile = streamReader.ReadLine();
                string[] strings2 = lineFromFile.Split(' ');
                for (int j = 0; j < Columns; ++j)
                {
                    dataIn2DArray[i, j] = int.Parse(strings2[j]);
                }
            }
        }

        private int[,] dataIn2DArray;

        /// <summary>
        /// Number of rows in the matrix
        /// </summary>
        public int Rows { get; }
        
        /// <summary>
        /// Number of columns in the matrix 
        /// </summary>
        public int Columns { get; }

        delegate void T(string str);

        /// <summary>
        /// Indexer for class matrix
        /// </summary>
        /// <param name="i"> number of row </param>
        /// <param name="j"> number of column </param>
        /// <returns> element on i, j position </returns>
        public int this[int i, int j]
        {
            get => dataIn2DArray[i, j];
            set { dataIn2DArray[i, j] = value; }
        }
        
        /// <summary>
        /// Printing into file
        /// </summary>
        /// <param name="path"> Path to the file you wanna see the matrix in</param>
        public void Print(string path = "")
        {
            T t;
            if (path == "")
            {
                t = Console.WriteLine;
            }
            else
            {
                using var streamWriter = new StreamWriter(path);
                streamWriter.WriteLine(Rows + " " + Columns);
                t = streamWriter.WriteLine;
            }
            for (int i = 0; i < Rows; ++i)
            {
                var str = string.Empty;
                for (int j = 0; j < Columns; ++j)
                {
                    str += dataIn2DArray[i, j] + " ";
                }
                t(str);
            }
            if (path == "")
            {
                Console.WriteLine("The result has been shown on the screen");
            }
            else
            {
                Console.WriteLine("Matrix has been successfully written in file: " + Environment.CurrentDirectory + "\\" + path);
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
                                sum += this[t, k] * rhsMatrix[k, j];
                            }
                            resultMatrix[t, j] = sum;
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
            var resultMatrix = new Matrix(Rows, rhsMatrix.Columns);
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < rhsMatrix.Columns; ++j)
                {
                    var sum = 0;
                    for (int k = 0; k < rhsMatrix.Rows; ++k)
                    {
                        sum += this[i, k] * rhsMatrix[k, j];
                    }
                    resultMatrix[i, j] = sum;
                }
            }
            return resultMatrix;
        }
    }
}
