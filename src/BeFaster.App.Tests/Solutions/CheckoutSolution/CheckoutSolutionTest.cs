using NUnit.Framework;

namespace BeFaster.App.Tests.Solutions.CheckoutSolution
{
    using CheckoutSolution = App.Solutions.CHK.CheckoutSolution;

    [TestFixture]
    public static class CheckoutSolutionTest
    {

        [TestCase(null, ExpectedResult = -1)]
        [TestCase("", ExpectedResult = 0)]
        [TestCase(" ", ExpectedResult = -1)]
        public static int ComputePriceShouldValidateInput(string param)
        {
            return CheckoutSolution.ComputePrice(param);
        }

        [TestCase("ABCD", ExpectedResult = 115)]
        [TestCase("AAAAD", ExpectedResult = 195)]
        [TestCase("BBD", ExpectedResult = 60)]
        [TestCase("BBBD", ExpectedResult = 90)]
        [TestCase("a", ExpectedResult = -1)]
        [TestCase("A", ExpectedResult = 50)]
        [TestCase("B", ExpectedResult = 30)]
        [TestCase("b", ExpectedResult = -1)]
        [TestCase("C", ExpectedResult = 20)]
        [TestCase("c", ExpectedResult = -1)]
        [TestCase("d", ExpectedResult = -1)]
        [TestCase("D", ExpectedResult = 15)]
        [TestCase("AAb", ExpectedResult = -1)]
        [TestCase("k", ExpectedResult = -1)]
        [TestCase("", ExpectedResult = 0)]
        [TestCase("ABCa", ExpectedResult = -1)]
        public static int ComputePriceReturnsValidResult(string param)
        {
            return CheckoutSolution.ComputePrice(param);
        }

        [TestCase("EE", ExpectedResult = 80)]
        [TestCase("EEB", ExpectedResult = 80)]
        [TestCase("EEEEBB", ExpectedResult = 160)]
        [TestCase("BEBEEE", ExpectedResult = 160)]
        [TestCase("ABCDEABCDE", ExpectedResult = 280)]
        [TestCase("ABCDE", ExpectedResult = 155)]
        [TestCase("ABCDECBAABCABBAAAEEAA", ExpectedResult = 665)]
        [TestCase("FFF", ExpectedResult = 20)]
        [TestCase("FFFF", ExpectedResult = 30)]
        [TestCase("FFFFF", ExpectedResult = 30)]
        [TestCase("FFFFFF", ExpectedResult = 40)]
        public static int ShouldApplyFreeProduct(string param)
        {
            return CheckoutSolution.ComputePrice(param);
        }

    }
}
