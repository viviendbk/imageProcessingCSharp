using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace projet_info
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int test = QRCode.ConvertCharToInt('A');
            Assert.AreEqual(test, 10);
        }
        public void TestMethod2()
        {
            byte[] a = { 1, 1, 1, 1, 1, 1, 1, 1 };
            byte[] test = QRCode.IntToBit(255, 8);
            Assert.AreEqual(test, a);
        }
    }
}
