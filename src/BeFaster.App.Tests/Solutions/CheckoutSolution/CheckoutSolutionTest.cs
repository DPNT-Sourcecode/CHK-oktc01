using NUnit.Framework;

namespace BeFaster.App.Tests.Solutions.CheckoutSolution
{
    using CheckoutSolution = App.Solutions.CHK.CheckoutSolution;

    [TestFixture]
    public static class CheckoutSolutionTest
    {

        [TestCase(null, ExpectedResult = -1)]
        [TestCase("", ExpectedResult = -1)]
        [TestCase(" ", ExpectedResult = -1)]
        public static int ComputePriceShouldValidateInput(string param)
        {
            return CheckoutSolution.ComputePrice(param);
        }

        [TestCase("a,b,c,d", ExpectedResult = 115)]
        [TestCase("a,a,a,a,d", ExpectedResult = 195)]
        [TestCase("b,b,d", ExpectedResult = 60)]
        [TestCase("B,B,b,d", ExpectedResult = 90)]
        [TestCase("a", ExpectedResult = 50)]
        [TestCase("A", ExpectedResult = 50)]
        [TestCase("B", ExpectedResult = 30)]
        [TestCase("b", ExpectedResult = 30)]
        [TestCase("C", ExpectedResult = 20)]
        [TestCase("c", ExpectedResult = 20)]
        [TestCase("d", ExpectedResult = 15)]
        [TestCase("D", ExpectedResult = 15)]
        [TestCase("alsdkfjdsa", ExpectedResult = -1)]
        public static int ComputePriceReturnsValidResult(string param)
        {
            return CheckoutSolution.ComputePrice(param);
        }

    }
}
