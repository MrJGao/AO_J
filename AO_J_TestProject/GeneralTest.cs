using AO_J;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
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
        private static string m_testDataPath = "..\\..\\..\\AO_J_TestProject\\TestData\\";
        private static string m_testResultPath = "..\\..\\..\\AO_J_TestProject\\TestData\\TestResult\\";

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
        ///General 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void GeneralConstructorTest()
        {
            General target = new General();
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }

        /// <summary>
        ///CopyDirectory 的测试
        ///</summary>
        [TestMethod()]
        public void CopyDirectoryTest()
        {
            string sourcePath = m_testDataPath + "directory";
            string destinationPath = m_testResultPath;
            General.CopyDirectory(sourcePath, destinationPath);
            Assert.IsTrue(Directory.Exists(destinationPath));
        }

        /// <summary>
        ///addSecurityControl 的测试
        ///</summary>
        [TestMethod()]
        public void addSecurityControlTest()
        {
            string filepath = string.Empty; // TODO: 初始化为适当的值
            General.addSecurityControl(filepath);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        // 清空路径下所有文件和文件夹
        private static void deletefileOrDic(System.IO.DirectoryInfo path)
        {
            try
            {
                foreach (System.IO.DirectoryInfo d in path.GetDirectories())
                {
                    d.Delete(true);
                }
                foreach (System.IO.FileInfo f in path.GetFiles())
                {
                    f.Delete();
                }
            }
            catch (Exception ex)
            {
            }
        }  
    }
}
