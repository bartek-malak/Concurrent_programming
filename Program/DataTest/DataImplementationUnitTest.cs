using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Test
{
    [TestClass]
    public class DataImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                IEnumerable<IBall>? ballsList = null;
                newInstance.CheckBallsList(x => ballsList = x);
                Assert.IsNotNull(ballsList);
                int numberOfBalls = 0;
                newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
                Assert.AreEqual<int>(0, numberOfBalls);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataImplementation newInstance = new DataImplementation();
            bool newInstanceDisposed = false;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            IEnumerable<IBall>? ballsList = null;
            newInstance.CheckBallsList(x => ballsList = x);
            Assert.IsNotNull(ballsList);
            newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(0, x));
            Assert.Throws<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.Throws<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
        }

        [TestMethod]
        public void StartTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfCallbackInvoked = 0;
                int numberOfBalls2Create = 10;
                int width = newInstance.Width;
                int height = newInstance.Height;
                double radius = newInstance.BallRadius;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) =>
                  {
                      numberOfCallbackInvoked++;
                      Assert.IsGreaterThanOrEqualTo(radius, startingPosition.x);
                      Assert.IsLessThanOrEqualTo(width - radius, startingPosition.x);
                      Assert.IsGreaterThanOrEqualTo(radius, startingPosition.y);
                      Assert.IsLessThanOrEqualTo(height - radius, startingPosition.y);
                      Assert.IsNotNull(ball);
                  });
                Assert.AreEqual<int>(numberOfBalls2Create, numberOfCallbackInvoked);
                newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(10, x));
            }
        }

        [TestMethod]
        public void Start_DoesNotGenerateBallsOutsideArea()
        {
            using (DataImplementation instance = new DataImplementation())
            {
                var positions = new System.Collections.Generic.List<(double x, double y)>();
                int createCount = 1000;
                int width = instance.Width;
                int height = instance.Height;
                double radius = instance.BallRadius;

                instance.Start(createCount, (startingPosition, ball) =>
                {
                    positions.Add((startingPosition.x, startingPosition.y));
                });

                Assert.AreEqual<int>(createCount, positions.Count);

                foreach (var p in positions)
                {
                    Assert.IsGreaterThanOrEqualTo(radius, p.x);
                    Assert.IsLessThanOrEqualTo(width - radius, p.x);
                    Assert.IsGreaterThanOrEqualTo(radius, p.y);
                    Assert.IsLessThanOrEqualTo(height - radius, p.y);
                }
            }
        }
    }
}
