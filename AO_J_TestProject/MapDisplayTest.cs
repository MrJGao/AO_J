using AO_J;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

namespace AO_J_TestProject
{
    
    
    /// <summary>
    ///这是 MapDisplayTest 的测试类，旨在
    ///包含所有 MapDisplayTest 单元测试
    ///</summary>
    [TestClass()]
    public class MapDisplayTest
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
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///MapDisplay 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void MapDisplayConstructorTest()
        {
            MapDisplay target = new MapDisplay();
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }

        /// <summary>
        ///ConvertPixelsToMapUnits 的测试
        ///</summary>
        [TestMethod()]
        public void ConvertPixelsToMapUnitsTest()
        {
            IActiveView pActiveView = null; // TODO: 初始化为适当的值
            double pixelUnits = 0F; // TODO: 初始化为适当的值
            double expected = 0F; // TODO: 初始化为适当的值
            double actual;
            actual = MapDisplay.ConvertPixelsToMapUnits(pActiveView, pixelUnits);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///FlashGeometry 的测试
        ///</summary>
        [TestMethod()]
        public void FlashGeometryTest()
        {
            MapDisplay target = new MapDisplay(); // TODO: 初始化为适当的值
            IGeometry geometry = null; // TODO: 初始化为适当的值
            IRgbColor color = null; // TODO: 初始化为适当的值
            IDisplay display = null; // TODO: 初始化为适当的值
            int delay = 0; // TODO: 初始化为适当的值
            target.FlashGeometry(geometry, color, display, delay);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }
    }
}
