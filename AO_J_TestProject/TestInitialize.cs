using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AO_J_TestProject
{
    [TestClass()]
    public class TestInitialize
    {
        private static string m_testResultFolder = "..\\..\\..\\AO_J_TestProject\\TestData\\TestResult\\";

        [AssemblyInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            // 创建测试结果文件夹
            System.IO.Directory.CreateDirectory(m_testResultFolder);
        }

        [AssemblyCleanup()]
        public static void MyTestCleanup()
        {
            // 删除测试结果文件夹
            System.IO.Directory.Delete(m_testResultFolder, true);
        }
    }
}
