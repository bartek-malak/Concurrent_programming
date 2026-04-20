using Data;

namespace BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixture()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataLayerDisposeFixture dataLayerFixture = new DataLayerDisposeFixture();
            BusinessLogicImplementation newInstance = new(dataLayerFixture);
            Assert.IsFalse(dataLayerFixture.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.Throws<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.Throws<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
            Assert.IsTrue(dataLayerFixture.Disposed);
        }

        [TestMethod]
        public void StartTestMethod()
        {
            DataLayerStartFixture dataLayerFixture = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixture))
            {
                int called = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); });
                Assert.AreEqual<int>(1, called);
                Assert.IsTrue(dataLayerFixture.StartCalled);
                Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixture.NumberOfBallseCreated);
            }
        }

        [TestMethod]
        public void GetDimensionsTest()
        {
            DataLayerStartFixture dataLayerFixture = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixture))
            {
                Assert.AreEqual(dataLayerFixture.Width, newInstance.GetDimensions().CanvasWidth);
                Assert.AreEqual(dataLayerFixture.Height, newInstance.GetDimensions().CanvasHeight);
                Assert.AreEqual(dataLayerFixture.BallRadius, newInstance.GetDimensions().BallRadius);
            }
        }




        // Fixture

        private class DataLayerConstructorFixture : Data.DataAbstractAPI
        {
            public override int Width => throw new NotImplementedException();

            public override int Height => throw new NotImplementedException();

            public override double BallRadius => throw new NotImplementedException();

            public override void Dispose()
            { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerDisposeFixture : Data.DataAbstractAPI
        {
            internal bool Disposed = false;

            public override int Width => throw new NotImplementedException();

            public override int Height => throw new NotImplementedException();

            public override double BallRadius => throw new NotImplementedException();

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerStartFixture : Data.DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;

            public override int Width => 500;
            public override int Height => 400;
            public override double BallRadius => 15.0;

            public override void Dispose()
            { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
            }

            private record DataVectorFixture : Data.IVector
            {
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallFixture : Data.IBall
            {
                public event EventHandler<BallEventArgs>? NewPositionNotification = null;
            }
        }
    }
}
