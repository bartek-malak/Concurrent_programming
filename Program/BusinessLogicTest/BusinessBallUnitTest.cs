using System;
using System.Collections.Generic;
using System.Text;
using BusinessLogic;
using Data;

namespace BusinessLogic.Test
{
    [TestClass]
    public class BusinessBallUnitTest
    {
        [TestMethod]
        public void TestPositionEventPropagation()
        {
            DataBallFixture dataBall = new DataBallFixture();
            Ball logicBall = new Ball(dataBall);
            int callCount = 0;
            IPosition? lastPos = null;

            logicBall.NewPositionNotification += (sender, args) =>
            {
                callCount++;
                lastPos = args.Position;
            };

            dataBall.SimulateMove(15.5, 20.0);

            Assert.AreEqual(1, callCount);
            Assert.AreEqual(15.5, lastPos.x);
            Assert.AreEqual(20.0, lastPos.y);
        }



        // Fixture
        private class DataBallFixture : Data.IBall
        {
            public event EventHandler<BallEventArgs>? NewPositionNotification;

            // Implement IBall members required by the interface
            private VectorFixture _position = new VectorFixture(0, 0);
            public IVector Position => _position;

            public IVector Velocity { get; set; } = new VectorFixture(0, 0);

            public double Mass { get; } = 1.0;

            public double Radius { get; } = 1.0;

            public void Dispose()
            {
                // No unmanaged resources to release in the test fixture
            }

            public void SimulateMove(double x, double y)
            {
                BallEventArgs args = new BallEventArgs(new VectorFixture(x, y));
                NewPositionNotification?.Invoke(this, args);

                // Update position so consumers reading Position see the new value
                _position = new VectorFixture(x, y);
            }
        }

        private class VectorFixture : IVector
        {
            public double x { get; init; }
            public double y { get; init; }
            public VectorFixture(double X, double Y) {
                x = X;
                y = Y; 
            }
        }
    }
}
