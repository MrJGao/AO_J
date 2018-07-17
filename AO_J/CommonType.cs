using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO_J
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        unknown,
        gdb,
        mdb,
        shapefile
    }

    public enum ImgFormat
    {
        Unknown,
        Imagine, //	"IMAGINE Image"
        TIFF, // "TIFF"
        GRID, // "GRID"
        JPEG, // "JPG"
        JP2000, // "JP2"
        BMP, // "BMP"
        PNG, // "PNG"
        GIF, // "GIF"
        PCI_Raster, // "PIX"
        X11_Pixmap, // "XPM"
        PCRaster, // "MAP"
        Memory_Raster, // "MEM"
        HDF4, // "HDF4"
        BIL, // "BIL"
        BIP, // "BIP"
        BSQ, // "BSQ"
        Idrisi_Raster, // "RST"
        ENVI_Raster, // "ENVI"
        Geodatabase_Raster // "GDB"
    }
}
