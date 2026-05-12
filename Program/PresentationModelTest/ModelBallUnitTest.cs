using System;
using BusinessLogic;

namespace PresentationModel.Test
{
    [TestClass]
    public class ModelBallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            ModelBall ball = new ModelBall(0.0, 0.0, new BusinessLogicIBallFixture());
            Assert.AreEqual<double>(0.0, ball.Top);
            Assert.AreEqual<double>(0.0, ball.Top);
        }

        [TestMethod]
        public void PositionChangeNotificationTestMethod()
        {
            int notificationCounter = 0;
            ModelBall ball = new ModelBall(0, 0.0, new BusinessLogicIBallFixture());
            ball.PropertyChanged += (sender, args) => notificationCounter++;
            Assert.AreEqual(0, notificationCounter);
            ball.SetLeft(1.0);
            Assert.AreEqual<int>(1, notificationCounter);
            Assert.AreEqual<double>(1.0, ball.Left);
            Assert.AreEqual<double>(0.0, ball.Top);
            ball.SettTop(1.0);
            Assert.AreEqual(2, notificationCounter);
            Assert.AreEqual<double>(1.0, ball.Left);
            Assert.AreEqual<double>(1.0, ball.Top);
        }

        
        // Fixture

        private class BusinessLogicIBallFixture : BusinessLogic.IBall
        {
            public event EventHandler<LogicBallEventArgs> NewPositionNotification;

            // Simple implementation of IPosition for the fixture
            private record PositionImpl(double x, double y) : IPosition;

            public IPosition Position { get; private set; } = new PositionImpl(0.0, 0.0);

            public IPosition Velocity { get; set; } = new PositionImpl(0.0, 0.0);

            public double Mass { get; } = 1.0;

            public double Radius { get; } = 1.0;

            public void Dispose()
            {
                // no-op for fixture
            }
        }
    }
}
