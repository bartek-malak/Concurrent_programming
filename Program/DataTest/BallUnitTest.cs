using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;

namespace DataTest
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            var testinVector = new Vector(0.0, 0.0);
            double radius = 1.0;
            // Provide required velocity, mass and logger parameters and discard the instance to avoid unused-assignment warning
            _ = new Ball(testinVector, radius, new Vector(0.0, 0.0), 1.0, new Logger());
        }
    }
}
