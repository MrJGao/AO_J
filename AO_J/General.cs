using System;
using System.IO;
using System.Security.AccessControl;
using System.Collections.Generic;

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
        public void CopyDirectory(String sourcePath, String destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                    File.Copy(fsi.FullName, destName, true);
                else                                    //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
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
    }
}
