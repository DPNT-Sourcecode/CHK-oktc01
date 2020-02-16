using BeFaster.App.Solutions.TST;
using NUnit.Framework;

namespace BeFaster.App.Tests.Solutions.TST
{
    public class OneTest {
    
        [Test]
        public void RunApply() {
            Assert.AreEqual(One.apply(), 1);
        }
    }
}
