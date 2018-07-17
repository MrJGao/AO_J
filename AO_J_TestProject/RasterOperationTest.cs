using AO_J;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace AO_J_TestProject
{
    
    
    /// <summary>
    ///这是 RasterOperationTest 的测试类，旨在
    ///包含所有 RasterOperationTest 单元测试
    ///</summary>
    [TestClass()]
    public class RasterOperationTest
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
        ///getInstance 的测试
        ///</summary>
        [TestMethod()]
        public void getInstanceTest()
        {
            RasterOperation actual = RasterOperation.getInstance();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///clipRaster 的测试
        ///</summary>
        [TestMethod()]
        public void clipRasterTest()
        {
            RasterOperation target = RasterOperation.getInstance();
            string imgFullName = System.IO.Path.Combine(TestInitialize.m_testDataPath, "raster", "landcover.img");

            IWorkspaceFactory workspaceFactory = Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory")) as IWorkspaceFactory;
            IWorkspace workspace = workspaceFactory.OpenFromFile(System.IO.Path.Combine(TestInitialize.m_testDataPath, "shapefiles"), 0);
            IFeatureClass featureClass = (workspace as IFeatureWorkspace).OpenFeatureClass("clipTest.shp");
            IFeatureCursor cursor = featureClass.Search(null, true);
            IFeature feature = cursor.NextFeature();
            IPolygon clipPolygon = feature.ShapeCopy as IPolygon;
            bool selectInside = true;
            string outImgFullName = System.IO.Path.Combine(TestInitialize.m_testResultFolder, "clipRes.img");
            string outImgFormat = "IMAGINE Image";

            bool actual = target.clipRaster(imgFullName, clipPolygon, selectInside, outImgFullName, outImgFormat);
            
            IWorkspaceFactoryLockControl wflockControl = workspaceFactory as IWorkspaceFactoryLockControl;
            if (wflockControl.SchemaLockingEnabled)
            {
                wflockControl.DisableSchemaLocking();
            }
            Assert.AreEqual(true, actual);

        }

        
    }
}
