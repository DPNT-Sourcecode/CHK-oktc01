using System;
using BeFaster.App.Solutions.HLO;
using NUnit.Framework;

namespace BeFaster.App.Tests.Solutions.HLO
{
    [TestFixture]
    public class HelloSolutionTest
    {

        [TestCase("FriendName")]
        public void ResponseShouldContainParam(string param)
        {
            Assert.True(HelloSolution.Hello(param).IndexOf(param,StringComparison.CurrentCultureIgnoreCase) > -1);
        }

        [TestCase("FriendName")]
        public void ResponseShouldEndWithExclamation(string param)
        {
            Assert.True(HelloSolution.Hello(param).EndsWith("!"));
        }
    }
}
