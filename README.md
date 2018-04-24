# AO_J
经常使用到的ArcObjects操作集合

## 开发环境
* Visual Studio 2010
* ArcObjects 10.x
* .NET Framework 4.0

## 代码组织
按照代码功能适用性进行区分，主要分为5个模块：
* 要素编辑
* GeoProcessing功能
* 地图组件相关交互
* 地图显示
* 通用功能

### 要素编辑
FeatureEdit类。包含要素查询、创建、删除等有关功能函数。

### GeoProcessing功能
GpOperation类。封装需要使用ArcObjects开发库中的GeoProcessing、GeoProcessor等接口的功能函数，多用于地理空间分析。

### 地图组件相关交互
MapControlInteraction类。封装与ArcObjects开发库中地图组件交互的功能函数，例如MapControl、TocControl、Editor等组件。也可以与ArcMap软件组件进行交互，可灵活应用于ArcMap Add-in开发模式。

### 地图显示
MapDisplay类。处理与地图显示相关的功能函数，例如要素闪烁、地图坐标与像素坐标转换等功能。

### 通用功能
General类。封装与ArcObjects不直接相关，但在开发中却经常用到的功能函数，主要是C#语言和Windows系统提供的一些常用功能。
