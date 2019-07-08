using System;
using System.IO;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AO_J
{
    /// <summary>
    /// 不使用AO，但比较通用的操作
    /// </summary>
    public class General
    {
        private static General m_singleton = null;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 私有构造函数，目前不做任何事
        /// </summary>
        private General() { }

        /// <summary>
        /// 获取该类静态实例
        /// </summary>
        /// <returns></returns>
        public static General getInstance()
        {
            if (m_singleton == null)
            {
                lock (locker)
                {
                    if (m_singleton == null)
                    {
                        m_singleton = new General();
                    }
                }
            }
            return m_singleton;
        }

        /// <summary>
        /// 复制文件夹到指定位置
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="destinationPath">目标路径</param>
        public void copyDirectory(string sourcePath, string destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                string destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                    File.Copy(fsi.FullName, destName, true);
                else                                    //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    copyDirectory(fsi.FullName, destName);
                }
            }
        }

        /// <summary>
        /// 移动文件夹到指定位置
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="destinationPath">目标路径</param>
        public void moveDirectory(string sourcePath, string destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                string destName = System.IO.Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                    File.Move(fsi.FullName, destName);
                else                                    //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    moveDirectory(fsi.FullName, destName);
                }
            }
        }

        /// <summary>
        /// 为文件夹添加完全访问权限
        /// </summary>
        /// <param name="filepath">文件夹路径</param>
        public void addSecurityControl(string filepath)
        {
            //获取文件夹信息
            DirectoryInfo dir = new DirectoryInfo(filepath);
            //获得该文件夹的所有访问权限
            System.Security.AccessControl.DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
            //设定文件ACL继承
            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            //添加ereryone用户组的访问权限规则 完全控制权限
            FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            //添加Users用户组的访问权限规则 完全控制权限
            FileSystemAccessRule usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            bool isModified = false;
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
            //设置访问权限
            dir.SetAccessControl(dirSecurity);
        }

        /// <summary>
        /// 获取目录下所有指定后缀格式的文件，支持多层级目录（不支持多后缀名）
        /// </summary>
        /// <param name="srcPath">搜索目录</param>
        /// <param name="filterPattern">后缀名字符串（例如：".docx"）</param>
        /// <param name="filenameList">文件名列表，使用 ref 传入</param>
        public void getAllSpecifiedFilesFromFolder(string srcPath, string filterPattern, ref List<string> filenameList)
        {
            DirectoryInfo info = new DirectoryInfo(srcPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                if (fsi is System.IO.FileInfo && fsi.Extension.ToUpper() == filterPattern.ToUpper()) // 如果是文件，复制文件
                {
                    filenameList.Add(fsi.FullName);
                }
                else if (fsi is System.IO.DirectoryInfo) // 如果是文件夹，新建文件夹，递归
                {
                    getAllSpecifiedFilesFromFolder(fsi.FullName, filterPattern, ref filenameList);
                }
            }
        }

        /// <summary>
        /// 比较两个双精度浮点数是否相等
        /// </summary>
        /// <param name="d1">第一个双精度浮点数</param>
        /// <param name="d2">另一个双精度浮点数</param>
        /// <param name="tolerance">精度控制</param>
        /// <returns>相等返回true，否则返回false</returns>
        public bool isEqualDouble(double d1, double d2, double tolerance)
        {
            if (Math.Abs(d1 - d2) <= tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断字符串是否代表整数
        /// </summary>
        /// <param name="val">字符串</param>
        /// <returns>字符串为整数，返回true；否则返回false。</returns>
        public bool strIsInteger(string val)
        {
            if (val == "")
            {
                return false;
            }
            string pattern = @"^[+-]?\d*$";
            return Regex.IsMatch(val, pattern);
        }

        /// <summary>
        /// 判断字符串是否代表浮点型数字
        /// </summary>
        /// <param name="val">字符串</param>
        /// <returns>字符串为浮点型，返回true；否则返回false。</returns>
        public bool strIsFloat(string val)
        {
            if (val == "")
            {
                return false;
            }
            string pattern = @"^[+-]?\d*[.]\d*$";
            return Regex.IsMatch(val, pattern);
        }

        /// <summary>
        /// 判断字符串是否代表数字，包括整型和浮点型
        /// </summary>
        /// <param name="val">字符串</param>
        /// <returns>字符串为数字，返回true；否则返回false。</returns>
        public bool strIsNumeric(string val)
        {
            if (val == "")
            {
                return false;
            }
            string pattern = @"^[+-]?\d*[.]?\d*$";
            return Regex.IsMatch(val, pattern);
        }
    }
}
