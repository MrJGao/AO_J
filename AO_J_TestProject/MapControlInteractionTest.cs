using AO_J;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;

namespace AO_J_TestProject
{
    
    
    /// <summary>
    ///这是 MapControlInteractionTest 的测试类，旨在
    ///包含所有 MapControlInteractionTest 单元测试
    ///</summary>
    [TestClass()]
    public class MapControlInteractionTest
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
        ///MapControlInteraction 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void MapControlInteractionConstructorTest()
        {
            MapControlInteraction target = new MapControlInteraction();
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }

        /// <summary>
        ///addWMSLayer 的测试
        ///</summary>
        [TestMethod()]
        public void addWMSLayerTest()
        {
            MapControlInteraction target = new MapControlInteraction(); // TODO: 初始化为适当的值
            IMap map = null; // TODO: 初始化为适当的值
            string address = string.Empty; // TODO: 初始化为适当的值
            string layerName = string.Empty; // TODO: 初始化为适当的值
            int layerPosition = 0; // TODO: 初始化为适当的值
            target.addWMSLayer(map, address, layerName, layerPosition);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///getLayerByName 的测试
        ///</summary>
        [TestMethod()]
        public void getLayerByNameTest()
        {
            MapControlInteraction target = new MapControlInteraction(); // TODO: 初始化为适当的值
            string name = string.Empty; // TODO: 初始化为适当的值
            IMap map = null; // TODO: 初始化为适当的值
            IEditor editor = null; // TODO: 初始化为适当的值
            ILayer expected = null; // TODO: 初始化为适当的值
            ILayer actual;
            actual = target.getLayerByName(name, map, editor);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///getLayerByName 的测试
        ///</summary>
        [TestMethod()]
        public void getLayerByNameTest1()
        {
            MapControlInteraction target = new MapControlInteraction(); // TODO: 初始化为适当的值
            string name = string.Empty; // TODO: 初始化为适当的值
            IMapControlDefault mapControl = null; // TODO: 初始化为适当的值
            ILayer expected = null; // TODO: 初始化为适当的值
            ILayer actual;
            actual = target.getLayerByName(name, mapControl);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///getlayerbygroup 的测试
        ///</summary>
        [TestMethod()]
        public void getlayerbygroupTest()
        {
            MapControlInteraction target = new MapControlInteraction(); // TODO: 初始化为适当的值
            ICompositeLayer temp_grouplayer = null; // TODO: 初始化为适当的值
            List<ILayer> result = null; // TODO: 初始化为适当的值
            List<ILayer> expected = null; // TODO: 初始化为适当的值
            List<ILayer> actual;
            actual = target.getlayerbygroup(temp_grouplayer, result);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
