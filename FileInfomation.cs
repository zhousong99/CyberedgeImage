using Emgu.CV;
using Emgu.CV.Util;

namespace CyberedgeImageProcess2024
{
    public class FileInfomation : ICloneable
    {

        /** 8-bit unsigned integer (0-255). */
        public const int GRAY8 = 0;

        /**	16-bit signed integer (-32768-32767). Imported signed images
            are converted to unsigned by adding 32768. */
        public const int GRAY16_SIGNED = 1;

        /** 16-bit unsigned integer (0-65535). */
        public const int GRAY16_UNSIGNED = 2;

        /**	32-bit signed integer. Imported 32-bit integer images are
            converted to floating-point. */
        public const int GRAY32_INT = 3;

        /** 32-bit floating-point. */
        public const int GRAY32_FLOAT = 4;

        /** 8-bit unsigned integer with color lookup table. */
        public const int COLOR8 = 5;

        /** 24-bit interleaved RGB. Import/export only. */
        public const int RGB = 6;

        /** 24-bit planer RGB. Import only. */
        public const int RGB_PLANAR = 7;

        /** 1-bit black and white. Import only. */
        public const int BITMAP = 8;

        /** 32-bit interleaved ARGB. Import only. */
        public const int ARGB = 9;

        /** 24-bit interleaved BGR. Import only. */
        public const int BGR = 10;

        /**	32-bit unsigned integer. Imported 32-bit integer images are
            converted to floating-point. */
        public const int GRAY32_UNSIGNED = 11;

        /** 48-bit interleaved RGB. */
        public const int RGB48 = 12;

        /** 12-bit unsigned integer (0-4095). Import only. */
        public const int GRAY12_UNSIGNED = 13;

        /** 24-bit unsigned integer. Import only. */
        public const int GRAY24_UNSIGNED = 14;

        /** 32-bit interleaved BARG (MCID). Import only. */
        public const int BARG = 15;

        /** 64-bit floating-point. Import only.*/
        public const int GRAY64_FLOAT = 16;

        /** 48-bit planar RGB. Import only. */
        public const int RGB48_PLANAR = 17;

        /** 32-bit interleaved ABGR. Import only. */
        public const int ABGR = 18;

        /** 32-bit interleaved CMYK. Import only. */
        public const int CMYK = 19;

        // File formats
        public const int UNKNOWN = 0;
        public const int RAW = 1;
        public const int TIFF = 2;
        public const int GIF_OR_JPG = 3;
        public const int FITS = 4;
        public const int BMP = 5;
        public const int DICOM = 6;
        public const int ZIP_ARCHIVE = 7;
        public const int PGM = 8;
        public const int IMAGEIO = 9;

        // Compression modes
        public const int COMPRESSION_UNKNOWN = 0;
        public const int COMPRESSION_NONE = 1;
        public const int LZW = 2;
        public const int LZW_WITH_DIFFERENCING = 3;
        public const int JPEG = 4;
        public const int PACK_BITS = 5;
        public const int ZIP = 6;

        /* File format (TIFF, GIF_OR_JPG, BMP, etc.). Used by the File/Revert command */
        public int fileFormat;

        /* File type (GRAY8, GRAY_16_UNSIGNED, RGB, etc.) */
        public int fileType;
        public String fileName;
        public String directory;
        public String url;

        public int offset = 0;  // Use getOffset() to read
        public int nImages;

        public int compression;
        public String unit;
        public int samplesPerPixel;
        public int width;
        public int height;
        public bool intelByteOrder;
        public int lutSize;
        public bool whiteIsZero;
        public byte[] reds;
        public byte[] greens;
        public byte[] blues;
        public Object pixels;
        public double pixelWidth = 1.0;
        public double pixelHeight = 1.0;
        public double pixelDepth = 1.0;

        /// <summary>
        /// 建立FileInfomation，并设置默认值
        /// </summary>
        public FileInfomation()
        {
            // assign default values
            fileFormat = UNKNOWN;
            fileType = GRAY8;
            fileName = "Untitled";
            directory = "";
            url = "";
            nImages = 1;
            compression = COMPRESSION_NONE;
            samplesPerPixel = 1;
        }

        public Object Clone()
        {
            try
            {
                FileInfomation copy = (FileInfomation)(base.MemberwiseClone());
                return copy;
            }
            catch
            { return null; }
        }
    }
}
