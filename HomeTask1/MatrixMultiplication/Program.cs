using System;
using System.IO;
using System.Diagnostics;

namespace MatrixMultiplication
{
    static class Program
    {
        /// <summary>
        /// Counting average and standart deviation
        /// </summary>
        /// <param name="data"> array with measured results </param>
        private static (double average, double standardSquareDeviation) AvgAndstandardSquareDeviationCount(double[] data)
        {
            var average = .0;
            foreach (var item in data)
            {
                average += item;
            }
            average /= data.Length;
            var dispersion = .0;
            for (int i = 0; i < data.Length; ++i)
            {
                dispersion += (data[i] - average) * (data[i] - average);
            }
            dispersion /= data.Length;
            var standardSquareDeviation = Math.Sqrt(dispersion);
            return (average, standardSquareDeviation);
        }

        private static void CheckingTheEffectiveness()
        {
            var stopwatch = new Stopwatch();
            using var steramWriter = new StreamWriter("Experiment_results.txt");
            steramWriter.WriteLine("(All results are in seconds)");
            for (int j = 0; j < 10; ++j)
            {
                var resultsParallel = new double[10];
                var resultsNonParallel = new double[10];
                int numberOfStringsFirst = 100 * (j + 1);
                int numberOfColumnsFirst = 300;
                int numberOfColunmsSecond = 500;
                var firstMatrix = new Matrix(numberOfStringsFirst, numberOfColumnsFirst, true);
                var secondMatrix = new Matrix(numberOfColumnsFirst, numberOfColunmsSecond, true);
                for (int i = 0; i < 10; ++i)
                {
                    stopwatch.Start();
                    firstMatrix.ParallelMultiplication(secondMatrix);
                    stopwatch.Stop();
                    resultsParallel[i] = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Reset();
                }
                for (int i = 0; i < 10; ++i)
                {
                    stopwatch.Start();
                    firstMatrix.NonParallelMultiplication(secondMatrix);
                    stopwatch.Stop();
                    resultsNonParallel[i] = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Reset();
                }
                (var averageParallel, var standardSquareDeviationParallel) = AvgAndstandardSquareDeviationCount(resultsParallel);
                (var averageNonParallel, var standardSquareDeviationNonParallel) = AvgAndstandardSquareDeviationCount(resultsNonParallel);
                steramWriter.WriteLine($"Matrix [{numberOfStringsFirst} * {numberOfColumnsFirst}] * [{numberOfColumnsFirst} * {numberOfColunmsSecond}]");
                steramWriter.WriteLine("Parallel: Average: {0:f4} +- {1:f4}", averageParallel, standardSquareDeviationParallel);
                steramWriter.WriteLine("NonParallel: Average: {0:f4} +- {1:f4}", averageNonParallel, standardSquareDeviationNonParallel);
                steramWriter.WriteLine();
                Console.Write(".");
            }
        }

        private static void Task()
        {
            try
            {
                var matrix1 = new Matrix("first.txt");
                var matrix2 = new Matrix("second.txt");
                var matrix3 = matrix1.ParallelMultiplication(matrix2);
                string resultFileName = "multiplication_result.txt";
                matrix3.Print(resultFileName);
                Console.WriteLine("Matrix has been successfully written in file: " + Environment.CurrentDirectory + "\\" + resultFileName);
            } catch (FileNotFoundException eFileNotFound)
            {
                Console.WriteLine($"File {eFileNotFound.FileName} wasn't found");
            } catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Matrixes have wrong sizes");
            }
        }

        static void Main(string[] args)
        {
            Task();
            CheckingTheEffectiveness();
        }
    }
}
