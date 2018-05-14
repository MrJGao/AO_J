using AO_J;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Runtime.InteropServices;
using System.IO;
using ESRI.ArcGIS.Geometry;

namespace AO_J_TestProject
{
    /// <summary>
    ///这是 FeatureEditTest 的测试类，旨在
    ///包含所有 FeatureEditTest 单元测试
    ///</summary>
    [TestClass()]
    public class FeatureEditTest
    {
        private static IWorkspaceFactory m_workspaceFactory = null;
        private static IFeatureWorkspace m_featureWorkspace = null;

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            // 打开测试shp数据
            Type FactoryType = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory");
            m_workspaceFactory = Activator.CreateInstance(FactoryType) as IWorkspaceFactory;
            string shpPath = System.IO.Path.GetFullPath(TestInitialize.m_testDataPath + "shapefiles\\airports.shp");
            m_featureWorkspace = m_workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(shpPath), 0) as IFeatureWorkspace;
        }
        
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Marshal.ReleaseComObject(m_featureWorkspace);
            Marshal.ReleaseComObject(m_workspaceFactory);
        }
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        
        //使用 TestCleanup 在运行完每个测试后运行代码
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        #endregion

        /// <summary>
        ///exportSelectedFeatureToShp 的测试
        ///</summary>
        [TestMethod()]
        public void exportSelectedFeatureToShpTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();
            IFeatureLayer featureLayer = null; // TODO: 初始化为适当的值
            ISelectionSet selectionSet = null; // TODO: 初始化为适当的值
            string outName = string.Empty; // TODO: 初始化为适当的值
            target.exportSelectedFeatureToShp(featureLayer, selectionSet, outName);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///getFeatureClassFromFile 的测试
        ///</summary>
        [TestMethod()]
        public void getFeatureClassFromFileTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();
            string filename = TestInitialize.m_testDataPath + "shapefiles\\airports.shp";
            string featureClassName = "airports";
            string extention = "shp";
            IFeatureClass actual;
            actual = target.getFeatureClassFromFile(filename, featureClassName, extention);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///getFeatureValue 的测试
        ///</summary>
        [TestMethod()]
        public void getFeatureValueTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();
            
            IFeatureClass featureClass = m_featureWorkspace.OpenFeatureClass("airports");
            IFeature feature = featureClass.GetFeature(0);
            string fieldName = "NAME";
            object expected = "NOATAK";
            object actual = target.getFeatureValue(feature, fieldName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///getFeatureWorkspaceFromFile 的测试
        ///</summary>
        [TestMethod()]
        public void getFeatureWorkspaceFromFileTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();
            string filename = TestInitialize.m_testDataPath + "shapefiles\\airports.shp";
            string extention = "shp";
            IFeatureWorkspace actual;
            actual = target.getFeatureWorkspaceFromFile(filename, extention);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///getWorkspaceFactory 的测试
        ///</summary>
        [TestMethod()]
        public void getWorkspaceFactoryTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();

            // get shapefile workspace factory
            string extension = "shp";
            IWorkspaceFactory actual = target.getWorkspaceFactory(extension);
            Assert.IsNotNull(actual);

            // get mdb workspace factory
            extension = "mdb";
            actual = target.getWorkspaceFactory(extension);
            Assert.IsNotNull(actual);

            // get gdb workspace factory
            extension = "gdb";
            actual = target.getWorkspaceFactory(extension);
            Assert.IsNotNull(actual);

            // get mdb workspace factory
            extension = "mdb";
            actual = target.getWorkspaceFactory(extension);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///setFeatureBufferValue 的测试
        ///</summary>
        [TestMethod()]
        public void setFeatureBufferValueTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();// TODO: 初始化为适当的值
            IFeatureBuffer feaBuf = null; // TODO: 初始化为适当的值
            string fieldName = string.Empty; // TODO: 初始化为适当的值
            object value = null; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.setFeatureBufferValue(feaBuf, fieldName, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///setFeatureValue 的测试
        ///</summary>
        [TestMethod()]
        public void setFeatureValueTest()
        {
            FeatureEdit target = FeatureEdit.getInstance();

            IFeatureClass featureClass = m_featureWorkspace.OpenFeatureClass("airports");
            IFeature feature = featureClass.GetFeature(0);
            string fieldName = string.Empty;
            object value = null;
            bool save = false;
            bool expected = false;
            bool actual;
            actual = target.setFeatureValue(feature, fieldName, value, save);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///equalPoints 的测试
        ///</summary>
        [TestMethod()]
        public void equalPointsTest()
        {
            FeatureEdit fe = FeatureEdit.getInstance();

            // equal
            IPoint point1 = new PointClass() { X = 2.356, Y = 3.34 };
            IPoint point2 = new PointClass() { X = 2.356, Y = 3.34 };
            double tolerance = 0.0001;
            bool expected = true; 
            bool actual = fe.equalPoints(point1, point2, tolerance);
            Assert.AreEqual(expected, actual);

            // not equal
            IPoint point3 = new PointClass() { X = 2.345, Y = 1.13 };
            IPoint point4 = new PointClass() { X = 1.234, Y = 2.45 };
            expected = false;
            actual = fe.equalPoints(point3, point4, tolerance);
            Assert.AreEqual(expected, actual);

            // use equal Z value
            IPoint point5 = new PointClass() { X = 2.123, Y = 1.323, Z = 134 };
            IPoint point6 = new PointClass() { X = 2.123, Y = 1.323, Z = 134 };
            expected = true;
            actual = fe.equalPoints(point5, point6, tolerance);
            Assert.AreEqual(expected, actual);

            // use not equal Z value
            IPoint point7 = new PointClass() { X = 2.123, Y = 1.323, Z = 134 };
            IPoint point8 = new PointClass() { X = 2.123, Y = 1.323, Z = 136 };
            expected = false;
            actual = fe.equalPoints(point7, point8, tolerance);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///getInstance 的测试
        ///</summary>
        [TestMethod()]
        public void getFeatureEditInstanceTest()
        {
            FeatureEdit fe = FeatureEdit.getInstance();
            Assert.IsNotNull(fe);
        }

        /// <summary>
        ///equalGeometry 的测试
        ///</summary>
        [TestMethod()]
        public void equalGeometryTest()
        {
            FeatureEdit fe = FeatureEdit.getInstance();
            
            // 测试两个相同的几何对象
            IGeometry geo1 = new PolylineClass();
            IPointCollection pc1 = geo1 as IPointCollection;
            pc1.AddPoint(new PointClass() { X = 1.2, Y = 2.3 });
            pc1.AddPoint(new PointClass() { X = 1.4, Y = 2.5 });
            pc1.AddPoint(new PointClass() { X = 1.1, Y = 2.8 });

            IGeometry geo2 = new PolylineClass();
            IPointCollection pc2 = geo2 as IPointCollection;
            pc2.AddPoint(new PointClass() { X = 1.2, Y = 2.3 });
            pc2.AddPoint(new PointClass() { X = 1.4, Y = 2.5 });
            pc2.AddPoint(new PointClass() { X = 1.1, Y = 2.8 });

            bool actual = fe.equalGeometry(geo1, geo2);
            Assert.IsTrue(actual);
            
            // 测试两个不同的几何对象
            IGeometry geo3 = new PolylineClass();
            IPointCollection pc3 = geo3 as IPointCollection;
            pc3.AddPoint(new PointClass() { X = 1.1, Y = 2.8 });
            pc3.AddPoint(new PointClass() { X = 1.4, Y = 2.5 });
            pc3.AddPoint(new PointClass() { X = 1.1, Y = 2.8 });

            actual = fe.equalGeometry(geo1, geo3);
            Assert.IsFalse(actual);
        }


        /// <summary>
        ///equalFeature 的测试
        ///</summary>
        [TestMethod()]
        public void equalFeatureTest()
        {
            FeatureEdit fe = FeatureEdit.getInstance();

            IFeatureClass featureClass = m_featureWorkspace.OpenFeatureClass("airports");
            IFeature f1 = featureClass.GetFeature(0);
            IFeature f2 = featureClass.CreateFeature();
            f2.Shape = f1.ShapeCopy;
            for (int i = 1; i < f1.Fields.FieldCount; i++)
            {
                f2.set_Value(i, f1.get_Value(i));
            }
            Assert.IsTrue(fe.equalFeature(f1, f2));
            // 最后删除掉这个新建的测试要素
            f2.Delete();

        }
    }
}
