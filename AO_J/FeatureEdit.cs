using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using System.IO;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.DataSourcesFile;

namespace AO_J
{
    /// <summary>
    /// 与要素查询、创建、删除等有关的操作
    /// </summary>
    public class FeatureEdit
    {
        private static FeatureEdit m_singleton = null;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 私有构造函数，目前不做任何事
        /// </summary>
        private FeatureEdit()
        {

        }

        /// <summary>
        /// 获取该类静态实例
        /// </summary>
        /// <returns></returns>
        public static FeatureEdit getInstance()
        {
            if (m_singleton == null)
            {
                lock (locker)
                {
                    if (m_singleton == null)
                    {
                        m_singleton = new FeatureEdit();
                    }
                }
            }
            return m_singleton;
        }

        /// <summary>
        /// 根据后缀名获取对应的IWorkspaceFactory对象
        /// </summary>
        /// <param name="extension">数据集文件后缀名</param>
        /// <returns></returns>
        public IWorkspaceFactory getWorkspaceFactory(string extension)
        {
            extension = extension.ToUpper();

            IWorkspaceFactory pWorkspaceFactory = null;
            Type FactoryType = null;
            switch (extension)
            {
                case "MDB":
                    FactoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.AccessWorkspaceFactory");
                    break;
                case "GDB":
                    FactoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    break;
                case "SHP":
                    FactoryType = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory");
                    break;
                default:
                    FactoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    break;
            }
            pWorkspaceFactory = Activator.CreateInstance(FactoryType) as IWorkspaceFactory;

            return pWorkspaceFactory;
        }

        /// <summary>
        /// 根据文件名和后缀，直接得到IFeatureWorkspace对象
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="extention">后缀</param>
        /// <returns></returns>
        public IFeatureWorkspace getFeatureWorkspaceFromFile(string filename, string extention)
        {
            IWorkspaceFactory workspaceFactory = getWorkspaceFactory(extention);
            IFeatureWorkspace featureWorkspace = null;

            if (extention.ToUpper() == "SHP")
            {
                featureWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(filename), 0) as IFeatureWorkspace;
            }
            else
            {
                featureWorkspace = workspaceFactory.OpenFromFile(filename, 0) as IFeatureWorkspace;
            }
            return featureWorkspace;
        }

        /// <summary>
        /// 根据文件名、后缀、要素类名称，直接得到IfeatureClass对象
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="featureClassName">要素类名称</param>
        /// <param name="extention">后缀</param>
        /// <returns>要素类对象</returns>
        public IFeatureClass getFeatureClassFromFile(string filename, string featureClassName, string extention)
        {
            IFeatureWorkspace featureWorkspace = getFeatureWorkspaceFromFile(filename, extention);

            IFeatureClass featureClass = null;
            if (featureWorkspace != null)
            {
                featureClass = featureWorkspace.OpenFeatureClass(featureClassName);
            }

            return featureClass;
        }

        /// <summary>
        /// 得到要素的属性值
        /// </summary>
        /// <param name="feature">要素</param>
        /// <param name="fieldName">属性字段名</param>
        /// <returns>要素属性值</returns>
        public object getFeatureValue(IFeature feature, string fieldName)
        {
            int index = feature.Fields.FindField(fieldName);
            if (index == -1) return false;

            object val = feature.get_Value(index);

            return val;
        }

        /// <summary>
        /// 设置要素值，适用于少量要素单个操作，批量操作请使用feature buffer
        /// </summary>
        /// <param name="feature">要素</param>
        /// <param name="fieldName">属性字段名称</param>
        /// <param name="value">输入值</param>
        /// <param name="save">是否立即存储更改，同时操作多个属性字段值，在操作最后统一存储会更快</param>
        /// <returns></returns>
        /// <returns>成功，返回true。否则返回false</returns>
        public bool setFeatureValue(IFeature feature, string fieldName, object value, bool save)
        {
            int index = feature.Fields.FindField(fieldName);
            if (index == -1) return false;

            try
            {
                feature.set_Value(index, value);
            }
            catch (System.Exception)
            {
                return false;
            }
            if (save == true) { feature.Store(); }

            return true;
        }

        /// <summary>
        /// 设置feature buffer的字段值，配合feature cursor进行批量操作
        /// </summary>
        /// <param name="feaBuf">IFeatureBuffer对象</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">输入值</param>
        /// <returns></returns>
        public bool setFeatureBufferValue(IFeatureBuffer feaBuf, string fieldName, object value)
        {
            int index = feaBuf.Fields.FindField(fieldName);
            if (index == -1)
            {
                return false;
            }
            try
            {
                feaBuf.set_Value(index, value);
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 导出图层中选定要素到单独的shp文件
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="selectionSet">要素选择集</param>
        /// <param name="outName">输出shp文件路径</param>
        public void exportSelectedFeatureToShp(IFeatureLayer featureLayer, ISelectionSet selectionSet, string outName)
        {
            if (featureLayer == null) return;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(outName))) return;

            // 裁剪要素
            IDataset dataset = featureLayer as IDataset;
            IFeatureClassName infeatClassName = dataset.FullName as IFeatureClassName;
            IDatasetName datasetName = infeatClassName as IDatasetName;

            // 输出要素类
            IFeatureClassName outFeatClassName = new FeatureClassName() as IFeatureClassName;
            outFeatClassName.FeatureType = esriFeatureType.esriFTSimple;
            outFeatClassName.ShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryAny;
            outFeatClassName.ShapeFieldName = "Shape";

            // 输出文件
            IDatasetName outDatasetName = outFeatClassName as IDatasetName;
            outDatasetName.Name = System.IO.Path.GetFileNameWithoutExtension(outName);
            IWorkspaceName workspaceName = new WorkspaceName() as IWorkspaceName;
            workspaceName.PathName = System.IO.Path.GetDirectoryName(outName);
            workspaceName.WorkspaceFactoryProgID = "esriDataSourcesGDB.ShapefileWorkspaceFactory";
            outDatasetName.WorkspaceName = workspaceName;

            // 导出
            IExportOperation exportOper = new ExportOperation();
            exportOper.ExportFeatureClass(datasetName, null, selectionSet, null, outFeatClassName, 0);
        }

        /// <summary>
        /// 判断两个点对象是否相等
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <param name="tolerance">精度控制</param>
        /// <returns>相等返回true，否则返回false</returns>
        public bool equalPoints(ESRI.ArcGIS.Geometry.IPoint point1, ESRI.ArcGIS.Geometry.IPoint point2, double tolerance)
        {
            if ((Math.Abs(point1.X - point2.X) <= tolerance) &&
                (Math.Abs(point1.Y - point2.Y) <= tolerance))
            {
                if (!double.IsNaN(point1.Z) && !double.IsNaN(point2.Z))
                {
                    if (Math.Abs(point1.Z - point2.Z) <= tolerance)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }
    }
}
