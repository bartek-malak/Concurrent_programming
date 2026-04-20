
namespace BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicAbstractAPIUnitTest
    {
        [TestMethod]
        public void BusinessLogicConstructorTestMethod()
        {
            BusinessLogicAbstractAPI instance1 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            BusinessLogicAbstractAPI instance2 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            Assert.AreSame(instance1, instance2);
            instance1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => instance2.Dispose());
        }
    }
}
