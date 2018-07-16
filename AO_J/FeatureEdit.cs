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
using System.Collections.Generic;
using System.Runtime.InteropServices;

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


        #region Workspace, FeatureClass...
        /// <summary>
        /// 根据数据库类型获取对应的IWorkspaceFactory对象
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns>对应类型的IWorkspaceFactory对象</returns>
        public IWorkspaceFactory getWorkspaceFactory(DatabaseType databaseType)
        {
            IWorkspaceFactory pWorkspaceFactory = null;
            Type FactoryType = null;
            switch (databaseType)
            {
                case DatabaseType.mdb:
                    FactoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.AccessWorkspaceFactory");
                    break;
                case DatabaseType.gdb:
                    FactoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    break;
                case DatabaseType.shapefile:
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
        /// 创建数据库
        /// </summary>
        /// <param name="databaseFullName">数据库完整路径名称</param>
        /// <returns>数据空间IWorkspace对象</returns>
        public IWorkspace createDatabase(string databaseFullName)
        {
            string extension = System.IO.Path.GetExtension(databaseFullName);
            DatabaseType dt = DatabaseType.unknown;
            switch (extension.ToLower())
            {
                case ".gdb":
                    dt = DatabaseType.gdb;
                    break;
                case ".mdb":
                    dt = DatabaseType.mdb;
                    break;
            }

            IWorkspaceFactory workspaceFactory = getWorkspaceFactory(dt);
            IWorkspace workspace = null;
            if (dt == DatabaseType.gdb)
            {
                if (System.IO.Directory.Exists(databaseFullName))
                {
                    workspace = workspaceFactory.OpenFromFile(databaseFullName, 0);
                    return workspace;
                }
            }
            else if (dt == DatabaseType.mdb)
            {
                if (File.Exists(databaseFullName))
                {
                    workspace = workspaceFactory.OpenFromFile(databaseFullName, 0);
                    return workspace;
                }
            }

            string databaseName = System.IO.Path.GetFileNameWithoutExtension(databaseFullName);
            string databaseDirectory = System.IO.Path.GetDirectoryName(databaseFullName);
            if (databaseName == "") return null;
            IWorkspaceName workspaceName = workspaceFactory.Create(databaseDirectory, databaseName, null, 0);
            ESRI.ArcGIS.esriSystem.IName name = workspaceName as ESRI.ArcGIS.esriSystem.IName;
            workspace = (IWorkspace)name.Open();

            Marshal.ReleaseComObject(workspaceFactory);
            return workspace;
        }

        /// <summary>
        /// 根据数据库类型，直接得到IFeatureWorkspace对象
        /// </summary>
        /// <param name="databaseFullName">文件路径</param>
        /// <returns>要素工作空间IFeatureWorkspace对象</returns>
        public IFeatureWorkspace getFeatureWorkspaceFromFile(string databaseFullName)
        {
            string extension = System.IO.Path.GetExtension(databaseFullName);
            DatabaseType dt = DatabaseType.unknown;
            switch (extension.ToLower())
            {
                case ".gdb":
                    dt = DatabaseType.gdb;
                    break;
                case ".mdb":
                    dt = DatabaseType.mdb;
                    break;
                case ".shp":
                    dt = DatabaseType.shapefile;
                    break;
            }

            IWorkspaceFactory workspaceFactory = getWorkspaceFactory(dt);
            IFeatureWorkspace featureWorkspace = null;

            if (dt == DatabaseType.shapefile)
            {
                featureWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(databaseFullName), 0) as IFeatureWorkspace;
            }
            else
            {
                featureWorkspace = workspaceFactory.OpenFromFile(databaseFullName, 0) as IFeatureWorkspace;
            }
            Marshal.ReleaseComObject(workspaceFactory);
            return featureWorkspace;
        }

        /// <summary>
        /// 根据文件名、后缀、要素类名称，直接得到IfeatureClass对象
        /// </summary>
        /// <param name="databaseFullName">数据库文件路径</param>
        /// <param name="featureClassName">要素类名称</param>
        /// <returns>要素类对象</returns>
        public IFeatureClass getFeatureClassFromFile(string databaseFullName, string featureClassName)
        {
            IFeatureWorkspace featureWorkspace = getFeatureWorkspaceFromFile(databaseFullName);

            IFeatureClass featureClass = null;
            if (featureWorkspace != null)
            {
                featureClass = featureWorkspace.OpenFeatureClass(featureClassName);
            }
            Marshal.ReleaseComObject(featureWorkspace);
            return featureClass;
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        /// <param name="workspace">工作空间</param>
        /// <param name="featureDataset">要素数据集</param>
        /// <param name="featureClassName">要素类名称</param>
        /// <param name="geometryType">要素类几何类型</param>
        /// <param name="spatialrefrence">空间参考</param>
        /// <returns>要素类对象</returns>
        public IFeatureClass createFeatureClass(ESRI.ArcGIS.Geodatabase.IWorkspace workspace,
            ESRI.ArcGIS.Geodatabase.IFeatureDataset featureDataset, System.String featureClassName, esriGeometryType geometryType, ISpatialReference spatialrefrence)
        {
            IWorkspace2 workspace2 = workspace as IWorkspace2;
            if (workspace2 == null) return null;

            ESRI.ArcGIS.esriSystem.UID CLSID = null;
            ESRI.ArcGIS.esriSystem.UID CLSEXT = null;
            System.String strConfigKeyword = null;
            if (featureClassName == "") return null; // name was not passed in 

            ESRI.ArcGIS.Geodatabase.IFeatureClass featureClass;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace featureWorkspace = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)workspace; // Explicit Cast

            if (workspace2.get_NameExists(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass, featureClassName)) //feature class with that name already exists 
            {
                featureClass = featureWorkspace.OpenFeatureClass(featureClassName);
                return featureClass;
            }

            // assign the class id value if not assigned
            if (CLSID == null)
            {
                CLSID = new ESRI.ArcGIS.esriSystem.UIDClass();
                CLSID.Value = "esriGeoDatabase.Feature";
            }

            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();
            ESRI.ArcGIS.Geodatabase.IFields fields = null;
            // if a fields collection is not passed in then supply our own

            if (fields == null)
            {
                // create the fields using the required fields method
                fields = objectClassDescription.RequiredFields;
                ESRI.ArcGIS.Geodatabase.IFieldsEdit fieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)fields; // Explicit Cast
                fields = (ESRI.ArcGIS.Geodatabase.IFields)fieldsEdit; // Explicit Cast
            }

            System.String strShapeField = "";

            // locate the shape field
            for (int j = 0; j < fields.FieldCount; j++)
            {
                if (fields.get_Field(j).Type == ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry)
                {
                    strShapeField = fields.get_Field(j).Name;
                }
            }

            IField field = fields.get_Field(fields.FindField(strShapeField));
            IGeometryDef geometryDef = field.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = geometryType;
            if (spatialrefrence == null) // default spatial reference is WGS_1984
            {
                ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
                spatialrefrence = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            }
            geometryDefEdit.SpatialReference_2 = spatialrefrence;

            // Use IFieldChecker to create a validated fields collection.
            ESRI.ArcGIS.Geodatabase.IFieldChecker fieldChecker = new ESRI.ArcGIS.Geodatabase.FieldCheckerClass();
            ESRI.ArcGIS.Geodatabase.IEnumFieldError enumFieldError = null;
            ESRI.ArcGIS.Geodatabase.IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (ESRI.ArcGIS.Geodatabase.IWorkspace)workspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            // finally create and return the feature class
            if (featureDataset == null)// if no feature dataset passed in, create at the workspace level
            {
                featureClass = featureWorkspace.CreateFeatureClass(featureClassName, validatedFields, CLSID, CLSEXT, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, strShapeField, strConfigKeyword);
            }
            else
            {
                featureClass = featureDataset.CreateFeatureClass(featureClassName, validatedFields, CLSID, CLSEXT, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, strShapeField, strConfigKeyword);
            }
            return featureClass;
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="workspace">工作空间</param>
        /// <param name="tableName">数据表名称</param>
        /// <returns>表格对象</returns>
        public ITable createTable(IWorkspace workspace, string tableName)
        {
            IWorkspace2 workspace2 = workspace as IWorkspace2;
            if (workspace2 == null) return null;

            ITable table = null;
            IDatasetName dn = null;
            IEnumDatasetName edn = workspace.get_DatasetNames(esriDatasetType.esriDTAny);
            bool isExist = false;
            while ((dn = edn.Next()) != null)
            {
                if (dn.Name == tableName)
                {
                    isExist = true;
                    break;
                }
            }
            if (isExist)
            {
                table = (workspace as IFeatureWorkspace).OpenTable(tableName);
            }
            else
            {
                // create the behavior clasid for the featureclass
                ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace; // Explicit Cast

                uid.Value = "esriGeoDatabase.Object";
                IObjectClassDescription objectClassDescription = new ObjectClassDescriptionClass();

                IFields fields = objectClassDescription.RequiredFields;
                // Use IFieldChecker to create a validated fields collection.
                IFieldChecker fieldChecker = new FieldCheckerClass();
                IEnumFieldError enumFieldError = null;
                IFields validatedFields = null;
                fieldChecker.ValidateWorkspace = (IWorkspace)workspace;
                fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

                // create and return the table
                table = featureWorkspace.CreateTable(tableName, validatedFields, uid, null, "");
            }
            return table;
        }

        /// <summary>
        /// 添加新字段
        /// </summary>
        /// <param name="featureClass">要素类</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="FieldType">字段类型</param>
        /// <param name="fieldLength">字段长度</param>
        public void addField(IFeatureClass featureClass, string fieldName, esriFieldType FieldType, int fieldLength)
        {
            //若存在，则不需添加
            if (featureClass.Fields.FindField(fieldName) > -1) return;
            IField pField = new FieldClass();
            IFieldEdit pFieldEdit = pField as IFieldEdit;
            pFieldEdit.AliasName_2 = fieldName;
            pFieldEdit.Name_2 = fieldName;
            pFieldEdit.Type_2 = FieldType;
            pFieldEdit.Length_2 = fieldLength;

            IClass pClass = featureClass as IClass;
            pClass.AddField(pField);
        }
        # endregion Workspace, FeatureClass...

        #region Feature value
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
        #endregion Feature value

        /// <summary>
        /// 导出图层中要素到单独的shp文件
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="queryFilter">要素筛选器</param>
        /// <param name="selectionSet">要素选择集</param>
        /// <param name="outName">输出shp文件路径</param>
        public void exportFeaturesToShp(IFeatureLayer featureLayer, IQueryFilter queryFilter, ISelectionSet selectionSet, string outName)
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
            exportOper.ExportFeatureClass(datasetName, queryFilter, selectionSet, null, outFeatClassName, 0);
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

        /// <summary>
        /// 比较两个几何图形是否相等。
        /// 当两个几何图形所有点都相等时，认为这两个几何图形相等。
        /// </summary>
        /// <param name="geo1">第一个几何图形对象</param>
        /// <param name="geo2">另一个几何图形对象</param>
        /// <returns>相等返回true，否则返回false</returns>
        public bool equalGeometry(IGeometry geo1, IGeometry geo2)
        {
            bool isEqual = true;
            if (geo1.Dimension != geo2.Dimension ||
                geo1.GeometryType != geo2.GeometryType ||
                geo1.IsEmpty != geo2.IsEmpty ||
                geo1.SpatialReference != geo2.SpatialReference)
            {
                isEqual = false;
            }

            if (geo1.GeometryType == esriGeometryType.esriGeometryPoint && 
                geo2.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                if ((geo1 as IPoint).Compare(geo2 as IPoint) != 0)
                {
                    isEqual = false;
                }
            }
            else
            {
                IPointCollection expectedPC = geo1 as IPointCollection;
                IPointCollection acturalPC = geo2 as IPointCollection;
                if (expectedPC.PointCount == acturalPC.PointCount)
                {
                    for (int i = 0; i < expectedPC.PointCount; i++)
                    {
                        if (expectedPC.Point[i].Compare(acturalPC.Point[i]) != 0)
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }
            }
            

            return isEqual;
        }

        /// <summary>
        /// 判断两个要素是否相等。
        /// 当两个要素的几何图形相等，且除OID以外所有字段值都相等时，认为两个要素相等。
        /// </summary>
        /// <param name="f1">第一个要素对象</param>
        /// <param name="f2">另一个要素对象</param>
        /// <param name="ignoreFields">忽略的字段列表</param>
        /// <returns>相等返回true，否则返回false</returns>
        public bool equalFeature(IFeature f1, IFeature f2, List<string> ignoreFields = null)
        {
            bool res = true;

            if (f1.FeatureType != f2.FeatureType) { return false; }
            if (!equalGeometry(f1.Shape, f2.Shape)) { return false; }

            if (f1.Fields.FieldCount != f2.Fields.FieldCount)
            {
                return false;
            }
            else
            {
                // i start from 1 to skip OID field
                for (int i = 1; i < f1.Fields.FieldCount; i++)
                {
                    if (ignoreFields != null && ignoreFields.Contains(f1.Fields.Field[i].Name) == false)
                    {
                        object val1 = f1.get_Value(i);
                        object val2 = f2.get_Value(i);
                        if (val1.ToString() != val2.ToString())
                        {
                            return false;
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// FeatureClass格式转换
        /// </summary>
        /// <param name="sourceWorkspace">原始工作空间</param>
        /// <param name="targetWorkspace">目标工作空间</param>
        /// <param name="nameOfSourceFeatureClass">原始FeatureClass名称</param>
        /// <param name="nameOfTargetFeatureClass">目标FeatureClass名称</param>
        /// <param name="pQueryFilter"></param>
        public static void convertFeatureClass(IWorkspace sourceWorkspace, IWorkspace targetWorkspace,
            string nameOfSourceFeatureClass, string nameOfTargetFeatureClass, IQueryFilter pQueryFilter)
        {
            //create source workspace name
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

            //create source dataset name
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureClass;

            //create target workspace name
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;

            //create target dataset name
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureClass;

            //Open input Featureclass to get field definitions.
            ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();

            //Validate the field names because you are converting between different workspace types.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields targetFeatureClassFields;
            IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
            IEnumFieldError enumFieldError;

            // Most importantly set the input and validate workspaces!
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;
            fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);

            // Loop through the output fields to find the geomerty field
            IField geometryField;
            for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
            {
                if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    geometryField = targetFeatureClassFields.get_Field(i);
                    // Get the geometry field's geometry defenition
                    IGeometryDef geometryDef = geometryField.GeometryDef;

                    //Give the geometry definition a spatial index grid count and grid size
                    IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;

                    targetFCGeoDefEdit.GridCount_2 = 1;
                    targetFCGeoDefEdit.set_GridSize(0, 0); //Allow ArcGIS to determine a valid grid size for the data loaded
                    targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;

                    // Load the feature class
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                    //IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, null, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);
                    IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, pQueryFilter, null, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);

                    break;
                }
            }
        }
    }
}
