using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GISClient;

namespace AO_J
{
    /// <summary>
    /// 与地图控件的交互，主要但不限于Add-in模式开发使用
    /// </summary>
    public class MapControlInteraction
    {
        /// <summary>
        /// 构造函数，目前不做任何事
        /// </summary>
        public MapControlInteraction() { }

        /// <summary>
        /// 根据图层名称获取图层对象
        /// </summary>
        /// <param name="name">图层名称</param>
        /// <param name="mapControl">地图控件对象</param>
        /// <returns>ILayer图层对象</returns>
        public ILayer getLayerByName(string name, IMapControlDefault mapControl)
        {
            if (mapControl == null) return null;

            for (int i = 0; i < mapControl.LayerCount; i++)
            {
                ILayer layer = mapControl.get_Layer(i);
                if (name.ToUpper() == layer.Name.ToUpper())
                    return layer;
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称获取图层对象
        /// </summary>
        /// <param name="name">图层名称</param>
        /// <param name="map">IMap地图对象</param>
        /// <param name="editor">IEditor编辑器对象</param>
        /// <returns>ILayer图层对象</returns>
        public ILayer getLayerByName(string name, IMap map, IEditor editor = null)
        {
            if (map == null) return null;

            for (int i = 0; i < map.LayerCount; i++)
            {
                ILayer layer = map.get_Layer(i);
                if (layer is IFeatureLayer)
                {
                    if (editor != null) // 如果在编辑状态，只考虑当前编辑的数据集
                    {
                        if ((layer as IDataset).Workspace != editor.EditWorkspace)
                            continue;
                    }
                    if (name.ToUpper() == layer.Name.ToUpper())
                        return layer;
                }
                else if (layer is IGroupLayer)
                {
                    ICompositeLayer pGroupLayer = layer as ICompositeLayer;
                    List<ILayer> temp_layer_list = getlayerbygroup(pGroupLayer, null);
                    for (int j = 0; j < temp_layer_list.Count; j++)
                    {
                        if (editor != null) // 如果在编辑状态，只考虑当前编辑的数据集
                        {
                            if ((temp_layer_list[j] as IDataset).Workspace != editor.EditWorkspace)
                                continue;
                        }
                        if (temp_layer_list[j].Name == name)
                        {
                            return temp_layer_list[j];
                        }
                    }
                }
            }
            return null;
        }
        
        /// 获取图层组中的所有要素图层，递归调用
        /// </summary>
        /// <param name="temp_grouplayer"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<ILayer> getlayerbygroup(ICompositeLayer temp_grouplayer, List<ILayer> result)
        {
            List<ILayer> temp_layer_list = null;
            if (result == null)
            {
                temp_layer_list = new List<ILayer>();
            }
            else
            {
                temp_layer_list = result;
            }

            for (int j = 0; j < temp_grouplayer.Count; j++)
            {
                ILayer dqlayer = temp_grouplayer.get_Layer(j);
                if (dqlayer is GroupLayer)
                {
                    ICompositeLayer pGroupLayer = dqlayer as ICompositeLayer;
                    temp_layer_list = getlayerbygroup(pGroupLayer, temp_layer_list);
                }
                else if (dqlayer is IFeatureLayer)
                {
                    temp_layer_list.Add(dqlayer);
                }
            }
            return temp_layer_list;
        }

        /// <summary>
        /// 添加 WMS 网络图层
        /// </summary>
        /// <param name="map">地图对象</param>
        /// <param name="address">网址</param>
        /// <param name="layerName">图层名称</param>
        /// <param name="layerPosition">图层添加位置</param>
        public void addWMSLayer(IMap map, string address, string layerName, int layerPosition)
        {
            IPropertySet propSet = new PropertySet();
            propSet.SetProperty("url", address);

            IWMSConnectionName wmsConName = new WMSConnectionName();
            wmsConName.ConnectionProperties = propSet;

            IWMSGroupLayer wmsMapLayer = new WMSMapLayer() as IWMSGroupLayer;
            IDataLayer dataLayer = wmsMapLayer as IDataLayer;
            dataLayer.Connect(wmsConName as IName);

            IWMSServiceDescription wmsServiceDescription = wmsMapLayer.WMSServiceDescription;
            ILayer theLayer = null;
            for (int i = 0; i < wmsServiceDescription.LayerDescriptionCount; i++)
            {
                IWMSLayerDescription wmsLayerDesc = wmsServiceDescription.get_LayerDescription(i);
                if (wmsLayerDesc.LayerDescriptionCount == 0)
                {
                    IWMSLayer wmsLayer = wmsMapLayer.CreateWMSLayer(wmsLayerDesc);
                    theLayer = wmsLayer as ILayer;
                }
                else
                {
                    IWMSGroupLayer wmsGroupLayer = wmsMapLayer.CreateWMSGroupLayers(wmsLayerDesc);
                    for (int j = 0; j < wmsGroupLayer.Count; j++)
                    {
                        ILayer layer = wmsGroupLayer.get_Layer(i);
                        layer.Visible = true;
                        wmsMapLayer.InsertLayer(layer, 0);
                    }
                    theLayer = wmsMapLayer as ILayer;
                }
            }
            theLayer.Name = wmsServiceDescription.WMSTitle;
            theLayer.Visible = true;
            map.AddLayer(theLayer);
            map.MoveLayer(theLayer, layerPosition);
        }

    }
}
