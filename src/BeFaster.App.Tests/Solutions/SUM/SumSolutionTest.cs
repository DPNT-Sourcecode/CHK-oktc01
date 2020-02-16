using System;
using BeFaster.App.Solutions.SUM;
using NUnit.Framework;

namespace BeFaster.App.Tests.Solutions.SUM
{
    [TestFixture]
    public class SumSolutionTest
    {
        [TestCase(1, 1, ExpectedResult = 2)]
        public int ComputeSum(int x, int y)
        {
            return SumSolution.Sum(x, y);
        }

        [TestCase(-1, 1)]
        [TestCase(101, 1)]
        [TestCase(1, -1)]
        [TestCase(1, 101)]
        public void ShouldValidateParameters(int x, int y)
        {
            Assert.Throws<ArgumentOutOfRangeException>(delegate { SumSolution.Sum(x, y); });
        }
    }
}
