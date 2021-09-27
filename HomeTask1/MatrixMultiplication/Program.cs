using System;
using System.IO;
using System.Diagnostics;

namespace MatrixMultiplication
{
    class Program
    {
        /// <summary>
        /// Считаем среденее и дисперсию в массиве данных
        /// </summary>
        /// <param name="data"></param>
        static void AvgAndDispersionCount(double[] data, out double average, out double dispersion)
        {
            average = 0;
            foreach (var item in data)
            {
                average += item;
            }
            average /= data.Length;
            double standardSquareDeviation = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                standardSquareDeviation += (data[i] - average) * (data[i] - average);
            }
            standardSquareDeviation /= data.Length;
            dispersion = Math.Sqrt(standardSquareDeviation);

        }
        static void CheckingTheEffectiveness()
        {
            var stopwatch = new Stopwatch();
            double[] resultsParallel = new double[10];
            double[] resultsNonParallel = new double[10];
            var random = new Random();
            using var sWriter = new StreamWriter("Experiment_results.txt");
            for (int j = 0; j < 10; ++j)
            {
                int numberOfStringsFirst = 100 * (j + 1);
                int numberOfColumnsFirst = 150;
                int numberOfColunmsSecond = 180;
                var firstMatrix = new Matrix(numberOfStringsFirst, numberOfColumnsFirst, true);
                var secondMatrix = new Matrix(numberOfColumnsFirst, numberOfColunmsSecond, true);
                var resultMatrix = new Matrix();
                stopwatch.Start();
                for (int i = 0; i < 10; ++i)
                {
                    resultMatrix = firstMatrix.ParallelMultiplication(secondMatrix);
                }
                stopwatch.Stop();
                resultsParallel[j] = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();
                for (int i = 0; i < 10; ++i)
                {
                    resultMatrix = firstMatrix.NonParallelMultiplication(secondMatrix);
                }
                stopwatch.Stop();
                resultsNonParallel[j] = stopwatch.Elapsed.TotalSeconds;
                AvgAndDispersionCount(resultsParallel, out double averageParallel, out double dispertionParallel);
                AvgAndDispersionCount(resultsNonParallel, out double averageNonParallel, out double dispertionNonParallel);
                sWriter.WriteLine($"Matrix [{100 * (j + 1)} * 150] * [150 * 180]");
                sWriter.WriteLine($"Parallel: Average: {averageParallel} +- {dispertionParallel}");
                sWriter.WriteLine($"NonParallel: Average: {averageNonParallel} +- {dispertionNonParallel}");
                sWriter.WriteLine();
                Console.Write(".");
            }
        }
        static void Task()
        {
            var mat1 = new Matrix(300, 50, true);
            mat1.Print("first.txt");
            var mat2 = new Matrix(50, 80, true);
            mat2.Print("second.txt");

            var matrix1 = new Matrix("first.txt");
            var matrix2 = new Matrix("second.txt");
            var matrix3 = new Matrix();
            matrix3 = matrix1.ParallelMultiplication(matrix2);
            string resultFileName = "multiplication_result.txt";
            matrix3.Print(resultFileName);
            Console.WriteLine("Matrix has been successfully written in file: " + Environment.CurrentDirectory + "\\" + resultFileName);
        }
        static void Main(string[] args)
        {
            //Task();
            //CheckingTheEffectiveness();


        }
    }
}
