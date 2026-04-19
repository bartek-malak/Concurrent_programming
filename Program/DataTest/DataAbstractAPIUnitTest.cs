using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Test
{
    [TestClass]
    public class DataAbstractAPIUnitTest
    {
        [TestMethod]
        public void ConstructorTestTestMethod()
        {
            DataAbstractAPI instance1 = DataAbstractAPI.GetDataLayer();
            DataAbstractAPI instance2 = DataAbstractAPI.GetDataLayer();
            Assert.AreSame<DataAbstractAPI>(instance1, instance2);
        }
    }
}
