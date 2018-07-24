using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.esriSystem;

namespace AO_J_TestProject
{
    [TestClass()]
    public class TestInitialize
    {
        public static string m_testDataPath = "..\\..\\..\\AO_J_TestProject\\TestData\\";
        public static string m_testResultFolder = "..\\..\\..\\AO_J_TestProject\\TestData\\TestResult\\";

        private static IAoInitialize aoInitialize = null;
        private static esriLicenseStatus licenseStatus;

        [AssemblyInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            // 初始化ArcObjects权限
            #region Licensing
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            aoInitialize = new AoInitializeClass();
            licenseStatus = aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
            if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
            {
                Console.WriteLine("Unable to check-out an ArcInfo license, error code is {0}", licenseStatus);
                return;
            }
            aoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            #endregion

            // 创建测试结果文件夹
            if (System.IO.Directory.Exists(m_testResultFolder))
            {
                System.IO.Directory.Delete(m_testResultFolder, true);
            }
            System.IO.Directory.CreateDirectory(m_testResultFolder);
        }

        [AssemblyCleanup()]
        public static void MyTestCleanup()
        {
            // 删除测试结果文件夹
            //System.IO.Directory.Delete(m_testResultFolder, true);
        }
    }
}
