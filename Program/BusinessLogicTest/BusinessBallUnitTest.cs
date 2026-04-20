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

            public void SimulateMove(double x, double y)
            {
                BallEventArgs args = new BallEventArgs(new VectorFixture(x, y));
                NewPositionNotification?.Invoke(this, args);
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
