using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using System.Runtime.InteropServices;

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
    }
}
