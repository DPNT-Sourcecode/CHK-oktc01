using System;

namespace BeFaster.App.Solutions.SUM
{
    public static class SumSolution
    {
        /// <summary>
        /// Sums two parameters.
        /// Throws ArgumentOutOfRangeException if any of the params not a positive integer between 0-100.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x+y</returns>
        public static int Sum(int x, int y)
        {
            ValidateSumParameters(x, y);
            return x + y;
        }

        private static void ValidateSumParameters(int x, int y)
        {
            ValidateParameter(nameof(x), x);
            ValidateParameter(nameof(y), y);
        }

        private static void ValidateParameter(string paramName, int param)
        {
            byte min = 0;
            byte max = 100;
            string validationMessage = "Value must be a positive integer between 0-100";

            if (param < min || param > max)
                throw new ArgumentOutOfRangeException(paramName, validationMessage);
        }

    }
}
