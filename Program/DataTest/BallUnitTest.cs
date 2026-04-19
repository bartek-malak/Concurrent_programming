using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testinVector = new Vector(0.0, 0.0);
            double radius = 1.0;
            Ball newInstance = new(testinVector, radius);
        }
    }
}
