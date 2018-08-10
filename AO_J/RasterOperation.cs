using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.GeoAnalyst;

namespace AO_J
{
    /// <summary>
    /// 栅格相关处理操作
    /// </summary>
    public class RasterOperation
    {
        private static RasterOperation m_singleton = null;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 私有构造函数，目前不做任何事
        /// </summary>
        private RasterOperation()
        {
            // 为了避免栅格文件的中文路径问题，加上下面这句，但不清楚原因
            ESRI.ArcGIS.Controls.IMapControlDefault mapControl = new ESRI.ArcGIS.Controls.MapControlClass();
        }

        /// <summary>
        /// 获取该类静态实例
        /// </summary>
        /// <returns></returns>
        public static RasterOperation getInstance()
        {
            if (m_singleton == null)
            {
                lock (locker)
                {
                    if (m_singleton == null)
                    {
                        m_singleton = new RasterOperation();
                    }
                }
            }
            return m_singleton;
        }
        
        /// <summary>
        /// 影像裁剪
        /// </summary>
        /// <param name="imgFullName">影像文件路径</param>
        /// <param name="clipPolygon">裁剪多边形</param>
        /// <param name="selectInside">是否裁剪内部</param>
        /// <param name="outImgFullName">输出结果影像路径</param>
        /// <param name="outImgFormat">输出影像格式</param>
        /// <returns>是否执行成功</returns>
        public bool clipRaster(string imgFullName, IPolygon clipPolygon, bool selectInside, string outImgFullName, string outImgFormat)
        {
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
            IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(imgFullName), 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(imgFullName));
            IExtractionOp2 extractionOp = new RasterExtractionOpClass();
            IGeoDataset clipRes = extractionOp.Polygon(rasterDataset as IGeoDataset, clipPolygon, selectInside);
            if (clipRes == null) return false;

            IWorkspace outWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(outImgFullName), 0);
            ISaveAs2 saveAs = clipRes as ISaveAs2;
            saveAs.SaveAs(outImgFullName, outWorkspace, outImgFormat);

            Marshal.ReleaseComObject(workspaceFactory);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(extractionOp);
            Marshal.ReleaseComObject(clipRes);
            Marshal.ReleaseComObject(outWorkspace);
            Marshal.ReleaseComObject(saveAs);
            System.GC.Collect();

            return true;
        }

        /// <summary>
        /// 根据影像格式枚举，返回对应的格式字符串
        /// </summary>
        /// <param name="format">影像格式枚举</param>
        /// <returns>影像格式字符串</returns>
        public string getImgFormatStr(ImgFormat format)
        {
            string formatStr = "";
            switch (format)
            {
                case ImgFormat.Unknown:
                    formatStr = "";
                    break;
                case ImgFormat.Imagine:
                    formatStr = "IMAGINE Image";
                    break;
                case ImgFormat.TIFF:
                    formatStr = "TIFF";
                    break;
                case ImgFormat.GRID:
                    formatStr = "GRID";
                    break;
                case ImgFormat.JPEG:
                    formatStr = "JPG";
                    break;
                case ImgFormat.JP2000:
                    formatStr = "JP2";
                    break;
                case ImgFormat.BMP:
                    formatStr = "BMP";
                    break;
                case ImgFormat.PNG:
                    formatStr = "PNG";
                    break;
                case ImgFormat.GIF:
                    formatStr = "GIF";
                    break;
                case ImgFormat.PCI_Raster:
                    formatStr = "PIX";
                    break;
                case ImgFormat.X11_Pixmap:
                    formatStr = "XPM";
                    break;
                case ImgFormat.PCRaster:
                    formatStr = "MAP";
                    break;
                case ImgFormat.Memory_Raster:
                    formatStr = "MEM";
                    break;
                case ImgFormat.HDF4:
                    formatStr = "HDF4";
                    break;
                case ImgFormat.BIL:
                    formatStr = "BIL";
                    break;
                case ImgFormat.BIP:
                    formatStr = "BIP";
                    break;
                case ImgFormat.BSQ:
                    formatStr = "BSQ";
                    break;
                case ImgFormat.Idrisi_Raster:
                    formatStr = "RST";
                    break;
                case ImgFormat.ENVI_Raster:
                    formatStr = "ENVI";
                    break;
                case ImgFormat.Geodatabase_Raster:
                    formatStr = "GDB";
                    break;
            }
            return formatStr;
        }

        /// <summary>
        /// 根据属性表达式提取栅格
        /// </summary>
        /// <param name="imgFullName">影像文件路径</param>
        /// <param name="expression">表达式</param>
        /// <returns>地理数据集</returns>
        public IGeoDataset extractRasterByAttribute(string imgFullName, string expression)
        {
            // 打开影像
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
            IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(imgFullName), 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(imgFullName));

            // 构建RasterDescriptor
            IRasterDescriptor rasterDescriptor = new RasterDescriptorClass();
            IRaster raster = rasterDataset.CreateDefaultRaster();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = expression;
            rasterDescriptor.Create(raster, queryFilter, "");

            // 提取影像
            IExtractionOp2 extractionOp = new RasterExtractionOpClass();
            IGeoDataset extractRes = extractionOp.Attribute(rasterDescriptor);

            Marshal.ReleaseComObject(workspaceFactory);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(extractionOp);
            Marshal.ReleaseComObject(extractRes);
            System.GC.Collect();

            return extractRes;
        }

        /// <summary>
        /// 根据属性表达式提取栅格
        /// </summary>
        /// <param name="imgFullName">影像文件路径</param>
        /// <param name="expression">表达式</param>
        /// <param name="outImgFullName">输出影像文件路径</param>
        /// <param name="imgFormat">影像格式</param>
        /// <returns>是否执行成功</returns>
        public bool extractRasterByAttribute(string imgFullName, string expression, string outImgFullName, ImgFormat imgFormat)
        {
            // 打开影像
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
            IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(imgFullName), 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(imgFullName));

            // 构建RasterDescriptor
            IRasterDescriptor rasterDescriptor = new RasterDescriptorClass();
            IRaster raster = rasterDataset.CreateDefaultRaster();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = expression;
            rasterDescriptor.Create(raster, queryFilter, "");

            // 提取影像
            IExtractionOp2 extractionOp = new RasterExtractionOpClass();
            IGeoDataset extractRes = extractionOp.Attribute(rasterDescriptor);
            if (extractRes == null) return false;

            IWorkspace outWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(outImgFullName), 0);
            ISaveAs2 saveAs = extractRes as ISaveAs2;
            saveAs.SaveAs(outImgFullName, outWorkspace, this.getImgFormatStr(imgFormat));

            Marshal.ReleaseComObject(workspaceFactory);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(extractionOp);
            Marshal.ReleaseComObject(extractRes);
            Marshal.ReleaseComObject(outWorkspace);
            Marshal.ReleaseComObject(saveAs);
            System.GC.Collect();
            
            return true;
        }

        /// <summary>
        /// 提取栅格影像
        /// </summary>
        /// <param name="imgFullName">影像文件路径</param>
        /// <param name="expression">表达式</param>
        /// <param name="outDb">输出矢量数据库路径</param>
        /// <param name="outFeatureClassName">输出要素类名称（若为空，则自动赋值为影像名称）</param>
        /// <returns>是否执行成功</returns>
        public bool extractRasterAttributeToVector(string imgFullName, string expression, string outDb, string outFeatureClassName = "")
        {
            // 打开影像
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
            IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(imgFullName), 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(imgFullName));

            // 构建RasterDescriptor
            IRasterDescriptor rasterDescriptor = new RasterDescriptorClass();
            IRaster raster = rasterDataset.CreateDefaultRaster();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = expression;
            rasterDescriptor.Create(raster, queryFilter, "");

            // 提取影像
            IExtractionOp2 extractionOp = new RasterExtractionOpClass();
            IGeoDataset extractRes = extractionOp.Attribute(rasterDescriptor);
            if (extractRes == null) return false;

            // 转换为矢量
            IRaster extractResRaster = extractRes as IRaster;
            IRasterDomainExtractor rasterpolygon = new RasterDomainExtractor();
            IPolygon polygon = rasterpolygon.ExtractDomain(extractResRaster, false);

            // 输出矢量
            IWorkspace workspace = FeatureEdit.getInstance().createDatabase(outDb);
            outFeatureClassName = outFeatureClassName == "" ? System.IO.Path.GetFileNameWithoutExtension(imgFullName) : outFeatureClassName;
            IFeatureClass featureClass = FeatureEdit.getInstance().createFeatureClass(workspace, null, outFeatureClassName, esriGeometryType.esriGeometryPolygon, 
                (rasterDataset as IGeoDataset).SpatialReference);
            IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;
            workspaceEdit.StartEditing(false);
            IFeature newFeature = featureClass.CreateFeature();
            newFeature.Shape = polygon;
            newFeature.Store();
            workspaceEdit.StopEditing(true);

            Marshal.ReleaseComObject(workspaceFactory);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(extractionOp);
            Marshal.ReleaseComObject(extractRes);
            Marshal.ReleaseComObject(workspace);
            Marshal.ReleaseComObject(featureClass);
            Marshal.ReleaseComObject(newFeature);
            Marshal.ReleaseComObject(polygon);
            System.GC.Collect();

            return true;
        }

        /// <summary>
        /// 获取影像对应波段的统计对象
        /// </summary>
        /// <param name="imgFullName">影像文件路径</param>
        /// <param name="bandNum">波段序号，默认为0</param>
        /// <returns>该波段的统计对象</returns>
        public IRasterStatistics getRasterBandStatistics(string imgFullName, int bandNum = 0)
        {
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
            IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(imgFullName), 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(imgFullName));
            IRaster raster = (rasterDataset as IRasterDataset2).CreateFullRaster();
            IRasterBandCollection rbc = (IRasterBandCollection)raster;

            if ((rbc.Count - 1) < bandNum) { return null; }// 请求的波段序号超出范围
            
            IRasterBand rb = rbc.Item(bandNum);
            bool tmpBool;
            rb.HasStatistics(out tmpBool);
            if (!tmpBool) { rb.ComputeStatsAndHist(); }

            IRasterStatistics rs = rb.Statistics;
            return rs;
        }
    }
}
