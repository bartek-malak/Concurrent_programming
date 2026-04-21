using BusinessLogic;
using PresentationModel;

namespace PresentationModel.Test
{
    [TestClass]
    public class PresentationModelUnitTest
    {
        [TestMethod]
        public void DisposeTestMethod()
        {
            UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            ModelImplementation? newInstance = null;
            using (newInstance = new(underneathLayerFixture))
            {
                newInstance.CheckObjectDisposed(x => Assert.IsFalse(x));
                newInstance.CheckUnderneathLayerAPI(x => Assert.AreSame(underneathLayerFixture, x));
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
                Assert.IsFalse(underneathLayerFixture.Disposed);
            }
            newInstance.CheckObjectDisposed(x => Assert.IsTrue(x));
            newInstance.CheckUnderneathLayerAPI(x => Assert.AreSame(underneathLayerFixture, x));
            Assert.IsTrue(underneathLayerFixture.Disposed);
            Assert.Throws<ObjectDisposedException>(() => newInstance.Dispose());
        }

        [TestMethod]
        public void StartTestMethod()
        {
            UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            using (ModelImplementation newInstance = new(underneathLayerFixture))
            {
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
                IDisposable subscription = newInstance.Subscribe(x => { });
                newInstance.CheckBallChangedEvent(x => Assert.IsFalse(x));
                newInstance.Start(10);
                Assert.AreEqual<int>(10, underneathLayerFixture.NumberOfBalls);
                subscription.Dispose();
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
            }
        }


        //Fixture

        private class UnderneathLayerFixture : BusinessLogicAbstractAPI
        {

            internal bool Disposed = false;
            internal int NumberOfBalls = 0;

            public override void Dispose()
            {
                Disposed = true;
            }

            public override Dimensions GetDimensions()
            {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IPosition, BusinessLogic.IBall> upperLayerHandler)
            {
                NumberOfBalls = numberOfBalls;
                Assert.IsNotNull(upperLayerHandler);
            }
        }
    }
}
