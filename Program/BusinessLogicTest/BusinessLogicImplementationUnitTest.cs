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
        public void ThreeBallsCollisionTest()
        {
            // Fixture sets three balls where first two collide and third is away
            DataLayerThreeCollisionFixture fixture = new();
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(3, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(3, received.Count);

                // initial velocities for first two towards each other
                Assert.IsTrue(received[0].Velocity.x > 0);
                Assert.IsTrue(received[1].Velocity.x < 0);

                // third ball should be stationary
                Assert.AreEqual(0, received[2].Velocity.x);
                Assert.AreEqual(0, received[2].Velocity.y);

                // Trigger movement on one data ball to cause collision handling
                fixture.RaisePositionNotificationForBall(0);

                // After collision first two should have reversed directions
                Assert.IsTrue(received[0].Velocity.x < 0, "First ball did not reverse X velocity");
                Assert.IsTrue(received[1].Velocity.x > 0, "Second ball did not reverse X velocity");

                // third ball should remain unchanged
                Assert.AreEqual(0, received[2].Velocity.x);
                Assert.AreEqual(0, received[2].Velocity.y);
            }
        }

        [TestMethod]
        public void NoCollisionWhenSeparatingTest()
        {
            // Fixture sets two balls positioned overlapping but moving apart
            DataLayerSeparatingFixture fixture = new();
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(2, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(2, received.Count);

                double v0x_before = received[0].Velocity.x;
                double v1x_before = received[1].Velocity.x;

                // Trigger movement on one data ball
                fixture.RaisePositionNotificationForBall(0);

                // Velocities should remain unchanged because balls are separating (dotProduct > 0)
                Assert.AreEqual(v0x_before, received[0].Velocity.x);
                Assert.AreEqual(v1x_before, received[1].Velocity.x);
            }
        }

        private class DataLayerWallFixture : DataAbstractAPI
        {
            private DataBallWall[] balls = new DataBallWall[1];
            private readonly int boardWidth = 200;
            private readonly int boardHeight = 200;

            public DataLayerWallFixture(bool leftSide = false, bool topSide = false, bool bottomSide = false)
            {
                if (leftSide)
                {
                    // place ball near left edge moving left
                    balls[0] = new DataBallWall() { Position = new DataVectorWall(1, 50), Velocity = new DataVectorWall(-5, 0), Mass = 1.0, Radius = 5 };
                }
                else if (topSide)
                {
                    balls[0] = new DataBallWall() { Position = new DataVectorWall(50, 1), Velocity = new DataVectorWall(0, -5), Mass = 1.0, Radius = 5 };
                }
                else if (bottomSide)
                {
                    balls[0] = new DataBallWall() { Position = new DataVectorWall(50, 195), Velocity = new DataVectorWall(0, 5), Mass = 1.0, Radius = 5 };
                }
                else
                {
                    // default: right side
                    balls[0] = new DataBallWall() { Position = new DataVectorWall(195, 50), Velocity = new DataVectorWall(5, 0), Mass = 1.0, Radius = 5 };
                }
            }

            public override int Width => boardWidth;
            public override int Height => boardHeight;
            public override double BallRadius => 5.0;

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                for (int i = 0; i < balls.Length; i++)
                {
                    upperLayerHandler(balls[i].Position, balls[i]);
                }
            }

            public void RaisePositionNotificationForBall(int index)
            {
                balls[index].RaiseNewPosition(new BallEventArgs(balls[index].Position));
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls() => balls;

            private record DataVectorWall : Data.IVector
            {
                public DataVectorWall() { }
                public DataVectorWall(double x, double y) { this.x = x; this.y = y; }
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallWall : Data.IBall
            {
                public DataBallWall() { }
                public Data.IVector Position { get; init; }
                public Data.IVector Velocity { get; set; }
                public double Mass { get; init; }
                public double Radius { get; init; }
                public event EventHandler<BallEventArgs>? NewPositionNotification;
                public void Dispose() { }

                public void RaiseNewPosition(BallEventArgs e)
                {
                    NewPositionNotification?.Invoke(this, e);
                }
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

        [TestMethod]
        public void RightWallCollisionTest()
        {
            var fixture = new DataLayerWallFixture();
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(1, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(1, received.Count);

                // initial velocity towards right wall
                Assert.IsTrue(received[0].Velocity.x > 0);

                // trigger position notification which should invoke CheckWallCollisions
                fixture.RaisePositionNotificationForBall(0);

                // velocity should have been inverted on X
                Assert.IsTrue(received[0].Velocity.x < 0, "Ball did not reverse X velocity at right wall");
            }
        }

        [TestMethod]
        public void LeftWallCollisionTest()
        {
            var fixture = new DataLayerWallFixture(leftSide: true);
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(1, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(1, received.Count);

                // initial velocity towards left wall
                Assert.IsTrue(received[0].Velocity.x < 0);

                fixture.RaisePositionNotificationForBall(0);

                Assert.IsTrue(received[0].Velocity.x > 0, "Ball did not reverse X velocity at left wall");
            }
        }

        [TestMethod]
        public void BottomWallCollisionTest()
        {
            var fixture = new DataLayerWallFixture(bottomSide: true);
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(1, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(1, received.Count);

                // initial velocity towards bottom wall
                Assert.IsTrue(received[0].Velocity.y > 0);

                fixture.RaisePositionNotificationForBall(0);

                Assert.IsTrue(received[0].Velocity.y < 0, "Ball did not reverse Y velocity at bottom wall");
            }
        }

        [TestMethod]
        public void TopWallCollisionTest()
        {
            var fixture = new DataLayerWallFixture(topSide: true);
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(1, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(1, received.Count);

                // initial velocity towards top wall
                Assert.IsTrue(received[0].Velocity.y < 0);

                fixture.RaisePositionNotificationForBall(0);

                Assert.IsTrue(received[0].Velocity.y > 0, "Ball did not reverse Y velocity at top wall");
            }
        }

        [TestMethod]
        public void CollisionChangesDirectionTest()
        {
            // Fixture sets two balls headed towards each other
            DataLayerCollisionFixture fixture = new();
            using (BusinessLogicImplementation logic = new(fixture))
            {
                var received = new System.Collections.Generic.List<BusinessLogic.IBall>();
                logic.Start(2, (pos, ball) => received.Add(ball));

                Assert.AreEqual<int>(2, received.Count);

                // initial velocities should be towards each other on X axis
                double v0x_before = received[0].Velocity.x;
                double v1x_before = received[1].Velocity.x;
                Assert.IsTrue(v0x_before > 0);
                Assert.IsTrue(v1x_before < 0);

                // Trigger movement on one data ball to cause collision handling
                fixture.RaisePositionNotificationForBall(0);

                // After collision they should have reversed directions
                double v0x_after = received[0].Velocity.x;
                double v1x_after = received[1].Velocity.x;

                Assert.IsTrue(v0x_after < 0, "First ball did not reverse X velocity");
                Assert.IsTrue(v1x_after > 0, "Second ball did not reverse X velocity");
            }
        }




        // Fixture

        private class DataLayerConstructorFixture : DataAbstractAPI
        {
            public override int Width { get { throw new NotImplementedException(); } }

            public override int Height { get { throw new NotImplementedException(); } }

            public override double BallRadius { get { throw new NotImplementedException(); } }

            public override void Dispose()
            { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls()
                => System.Array.Empty<Data.IBall>();
        }

        private class DataLayerDisposeFixture : DataAbstractAPI
        {
            internal bool Disposed = false;

            public override int Width { get { throw new NotImplementedException(); } }

            public override int Height { get { throw new NotImplementedException(); } }

            public override double BallRadius { get { throw new NotImplementedException(); } }

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls()
                => System.Array.Empty<Data.IBall>();
        }

        private class DataLayerStartFixture : DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;

            public override int Width { get { return 500; } }
            public override int Height { get { return 400; } }
            public override double BallRadius { get { return 15.0; } }

            public override void Dispose()
            { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls()
                => System.Array.Empty<Data.IBall>();

            private record DataVectorFixture : Data.IVector
            {
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallFixture : Data.IBall
            {
                public DataBallFixture()
                {
                    Position = new DataVectorFixture();
                    Velocity = new DataVectorFixture();
                    Mass = 1.0;
                    Radius = 1.0;
                }

                public Data.IVector Position { get; }
                public Data.IVector Velocity { get; set; }
                public double Mass { get; }
                public double Radius { get; }

                public event EventHandler<BallEventArgs>? NewPositionNotification = null;

                public void Dispose() { }
            }
        }

        private class DataLayerCollisionFixture : DataAbstractAPI
        {
            private DataBallCollision[] balls = new DataBallCollision[2];
            public DataLayerCollisionFixture()
            {
                balls[0] = new DataBallCollision() { Position = new DataVectorCollision(100, 50), Velocity = new DataVectorCollision(5, 0), Mass = 1.0, Radius = 5 };
                balls[1] = new DataBallCollision() { Position = new DataVectorCollision(110, 50), Velocity = new DataVectorCollision(-5, 0), Mass = 1.0, Radius = 5 };
            }

            public override int Width => 200;
            public override int Height => 200;
            public override double BallRadius => 5.0;

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                for (int i = 0; i < balls.Length; i++)
                {
                    upperLayerHandler(balls[i].Position, balls[i]);
                }
            }

            public void RaisePositionNotificationForBall(int index)
            {
                balls[index].RaiseNewPosition(new BallEventArgs(balls[index].Position));
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls() => balls;

            private record DataVectorCollision : Data.IVector
            {
                public DataVectorCollision() { }
                public DataVectorCollision(double x, double y) { this.x = x; this.y = y; }
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallCollision : Data.IBall
            {
                public DataBallCollision() { }
                public Data.IVector Position { get; init; }
                public Data.IVector Velocity { get; set; }
                public double Mass { get; init; }
                public double Radius { get; init; }
                public event EventHandler<BallEventArgs>? NewPositionNotification;
                public void Dispose() { }

                public void RaiseNewPosition(BallEventArgs e)
                {
                    NewPositionNotification?.Invoke(this, e);
                }
            }
        }

        private class DataLayerThreeCollisionFixture : DataAbstractAPI
        {
            private DataBallCollision[] balls = new DataBallCollision[3];
            public DataLayerThreeCollisionFixture()
            {
                balls[0] = new DataBallCollision() { Position = new DataVectorCollision(90, 50), Velocity = new DataVectorCollision(5, 0), Mass = 1.0, Radius = 5 };
                balls[1] = new DataBallCollision() { Position = new DataVectorCollision(100, 50), Velocity = new DataVectorCollision(-5, 0), Mass = 1.0, Radius = 5 };
                balls[2] = new DataBallCollision() { Position = new DataVectorCollision(150, 50), Velocity = new DataVectorCollision(0, 0), Mass = 1.0, Radius = 5 };
            }

            public override int Width => 300;
            public override int Height => 200;
            public override double BallRadius => 5.0;

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                for (int i = 0; i < balls.Length; i++)
                {
                    upperLayerHandler(balls[i].Position, balls[i]);
                }
            }

            public void RaisePositionNotificationForBall(int index)
            {
                balls[index].RaiseNewPosition(new BallEventArgs(balls[index].Position));
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls() => balls;

            private record DataVectorCollision : Data.IVector
            {
                public DataVectorCollision() { }
                public DataVectorCollision(double x, double y) { this.x = x; this.y = y; }
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallCollision : Data.IBall
            {
                public DataBallCollision() { }
                public Data.IVector Position { get; init; }
                public Data.IVector Velocity { get; set; }
                public double Mass { get; init; }
                public double Radius { get; init; }
                public event EventHandler<BallEventArgs>? NewPositionNotification;
                public void Dispose() { }

                public void RaiseNewPosition(BallEventArgs e)
                {
                    NewPositionNotification?.Invoke(this, e);
                }
            }
        }

        private class DataLayerSeparatingFixture : DataAbstractAPI
        {
            private DataBallCollision[] balls = new DataBallCollision[2];
            public DataLayerSeparatingFixture()
            {
                // Overlapping positions but velocities are away from each other
                balls[0] = new DataBallCollision() { Position = new DataVectorCollision(100, 50), Velocity = new DataVectorCollision(5, 0), Mass = 1.0, Radius = 10 };
                balls[1] = new DataBallCollision() { Position = new DataVectorCollision(105, 50), Velocity = new DataVectorCollision(10, 0), Mass = 1.0, Radius = 10 };
            }

            public override int Width => 300;
            public override int Height => 200;
            public override double BallRadius => 5.0;

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                for (int i = 0; i < balls.Length; i++)
                {
                    upperLayerHandler(balls[i].Position, balls[i]);
                }
            }

            public void RaisePositionNotificationForBall(int index)
            {
                balls[index].RaiseNewPosition(new BallEventArgs(balls[index].Position));
            }

            public override System.Collections.Generic.IEnumerable<Data.IBall> GetBalls() => balls;

            private record DataVectorCollision : Data.IVector
            {
                public DataVectorCollision() { }
                public DataVectorCollision(double x, double y) { this.x = x; this.y = y; }
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallCollision : Data.IBall
            {
                public DataBallCollision() { }
                public Data.IVector Position { get; init; }
                public Data.IVector Velocity { get; set; }
                public double Mass { get; init; }
                public double Radius { get; init; }
                public event EventHandler<BallEventArgs>? NewPositionNotification;
                public void Dispose() { }

                public void RaiseNewPosition(BallEventArgs e)
                {
                    NewPositionNotification?.Invoke(this, e);
                }
            }
        }
    }
}
