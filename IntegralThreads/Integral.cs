using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ParallelIntegral
{
    public class Integral
    {
        private readonly Func<double, double> _function;

        private readonly double _left;
        private readonly double _right;
        private readonly double _eps;

        private double _result;
        public double Result => _result;

        public Integral(Func<double, double> function, double left, double right, double eps)
        {
            _function = function;
            _left = left;
            _right = right;
            _eps = eps;
        }

        public void Start()
        {
            var length = _right - _left;
            var lengthForThread = length / Environment.ProcessorCount;
            var threads = new Thread[Environment.ProcessorCount];

            double[] results = new double[Environment.ProcessorCount];

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                int local = i;
                var left = _left + lengthForThread * local;
                var right = _left + lengthForThread * (local + 1);

                var thread = new Thread(() => Solve(left, right, out results[local]));
                thread.Start();
                threads[i] = thread;
            }

            var watchParallel = Stopwatch.StartNew();

            foreach (var thread in threads)
                thread.Join();

            Console.WriteLine($"Время многопоточного вычисления: {watchParallel.ElapsedMilliseconds} мс");

            _result = results.Sum();
        }

        public double SolveCon()
        {
            var watchCon = Stopwatch.StartNew();
            double res = 0;

            for (double left = _left; left < _right; left += _eps)
            {
                double right = left + _eps;
                double center = left + (right - left) / 2;

                res += _function(center) * _eps;
            }

            Console.WriteLine($"Время однопоточного вычисления: {watchCon.ElapsedMilliseconds} мс");

            return res;
        }

        private void Solve(double left, double right, out double result)
        {
            var length = right - left;
            var half = length / 2;
            var center = left + half;

            if (length < _eps)
            {
                result = _function(center) * length;
            }
            else
            {
                Solve(left, center, out var r1);
                Solve(center, right, out var r2);

                result = r1 + r2;
            }
        }
    }
}