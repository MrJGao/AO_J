using AO_J;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AO_J_TestProject
{
    /// <summary>
    ///这是 GeneralTest 的测试类，旨在
    ///包含所有 GeneralTest 单元测试
    ///</summary>
    [TestClass()]
    public class GeneralTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
         
        // 编写测试时，还可使用以下特性:
        
        // 使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }
        
        // 使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
        
        // 使用 TestInitialize 在运行每个测试前先运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        
        // 使用 TestCleanup 在运行完每个测试后运行代码
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        #endregion

        /// <summary>
        ///getInstance 的测试
        ///</summary>
        [TestMethod()]
        public void getGeneralInstanceTest()
        {
            General g = General.getInstance();
            Assert.IsNotNull(g);
        }

        /// <summary>
        ///strIsInteger 的测试
        ///</summary>
        [TestMethod()]
        public void strIsIntegerTest()
        {
            General g = General.getInstance();
            Assert.IsTrue(g.strIsInteger("123"));
            Assert.IsTrue(g.strIsInteger("+10"));
            Assert.IsTrue(g.strIsInteger("-10"));
            Assert.IsTrue(g.strIsInteger("001"));
            Assert.IsTrue(g.strIsInteger("000"));

            Assert.IsFalse(g.strIsInteger(""));
            Assert.IsFalse(g.strIsInteger("1.2"));
            Assert.IsFalse(g.strIsInteger("abc"));
            Assert.IsFalse(g.strIsInteger("abc12"));
            Assert.IsFalse(g.strIsInteger("12abc"));
            Assert.IsFalse(g.strIsInteger("1a2b3c"));
        }

        /// <summary>
        ///strIsFloat 的测试
        ///</summary>
        [TestMethod()]
        public void strIsFloatTest()
        {
            General g = General.getInstance();
            Assert.IsTrue(g.strIsFloat("10.1"));
            Assert.IsTrue(g.strIsFloat("+0.4"));
            Assert.IsTrue(g.strIsFloat("-0.4"));
            Assert.IsTrue(g.strIsFloat("00.440"));
            Assert.IsTrue(g.strIsFloat("0.22"));

            Assert.IsFalse(g.strIsFloat(""));
            Assert.IsFalse(g.strIsFloat("121"));
            Assert.IsFalse(g.strIsFloat("abc"));
            Assert.IsFalse(g.strIsFloat("abc1.123"));
            Assert.IsFalse(g.strIsFloat("abc.11"));
        }

        /// <summary>
        ///strIsNumeric 的测试
        ///</summary>
        [TestMethod()]
        public void strIsNumericTest()
        {
            General g = General.getInstance();
            Assert.IsTrue(g.strIsNumeric("10.1"));
            Assert.IsTrue(g.strIsNumeric("+0.4"));
            Assert.IsTrue(g.strIsNumeric("-0.4"));
            Assert.IsTrue(g.strIsNumeric("00.440"));
            Assert.IsTrue(g.strIsNumeric("0.22"));

            Assert.IsTrue(g.strIsNumeric("123"));
            Assert.IsTrue(g.strIsNumeric("+10"));
            Assert.IsTrue(g.strIsNumeric("-10"));
            Assert.IsTrue(g.strIsNumeric("001"));
            Assert.IsTrue(g.strIsNumeric("000"));

            Assert.IsFalse(g.strIsNumeric(""));
            Assert.IsFalse(g.strIsNumeric("abc"));
            Assert.IsFalse(g.strIsNumeric("abc1.123"));
            Assert.IsFalse(g.strIsNumeric("abc.11"));

        }
    }
}
