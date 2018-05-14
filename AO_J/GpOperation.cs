using System;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;

namespace AO_J
{
    /// <summary>
    /// 需要使用到AO里GeoProcessing进行操作的功能
    /// </summary>
    public class GpOperation
    {
        private static GpOperation m_singleton = null;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 私有构造函数，目前不做任何事
        /// </summary>
        private GpOperation() { }

        /// <summary>
        /// 获取该类静态实例
        /// </summary>
        /// <returns></returns>
        public static GpOperation getInstance()
        {
            if (m_singleton == null)
            {
                lock (locker)
                {
                    if (m_singleton == null)
                    {
                        m_singleton = new GpOperation();
                    }
                }
            }
            return m_singleton;
        }

        /// <summary>
        /// 裁剪矢量图层
        /// </summary>
        /// <param name="clipFilename">裁剪图层文件</param>
        /// <param name="inputFilename">被裁剪的图层文件</param>
        /// <param name="outputFilename">输出文件路径</param>
        public void clipLayers(string clipFilename, string inputFilename, string outputFilename)
        {
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            gp.OverwriteOutput = true;
            gp.AddOutputsToMap = false;

            ESRI.ArcGIS.AnalysisTools.Clip clip = new ESRI.ArcGIS.AnalysisTools.Clip();
            clip.in_features = inputFilename;
            clip.clip_features = clipFilename;
            clip.out_feature_class = outputFilename;

            try
            {
                IGeoProcessorResult results = (IGeoProcessorResult)gp.Execute(clip, null);
                if (results.Status != esriJobStatus.esriJobSucceeded)
                    throw new Exception("裁剪图层(clipLayers)遇到错误");
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(this.GetType().Name + "出错，错误信息：" + e.Message );
            }
        }


    }
}
