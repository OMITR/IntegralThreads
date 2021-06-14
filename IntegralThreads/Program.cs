using System;
using System.Diagnostics;

namespace ParallelIntegral
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var integral = new Integral(
                function: x => 4 / x - 5 * Math.Pow(x, 4) + 2 * Math.Sqrt(x),
                left: 1,
                right: 2,
                eps: 0.00000001
            );

            integral.Start();
            Console.WriteLine($"Результат: {integral.Result}");

            var conResult = integral.SolveCon();
            Console.WriteLine($"Результат: {conResult}");

            Console.ReadKey();
        }
    }
}