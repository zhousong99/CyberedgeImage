using FellowOakDicom;
using FellowOakDicom.Imaging;


namespace CyberedgeImageProcess2024
{
    class Dicom : EdgeImagePlus
    {
        double windowCenter, windowWidth;
        double rescaleIntercept, rescaleSlope = 1.0;
        DicomDataset dataset;

        public Dicom(string filename)
        {
            //读取dicom文件信息
            fileInfo = fileInfo ?? new FileInfomation();
            //初始化fileInfo
            fileInfo.directory = System.IO.Path.GetDirectoryName(filename);
            fileInfo.fileName = System.IO.Path.GetFileName(filename);
            dataset = DicomFile.Open(filename).Dataset;
            GetFileInfo(dataset);

            if (fileInfo != null && fileInfo.width > 0 && fileInfo.height > 0 )
            {
                EdgeImagePlus imp = CreateImp();
                bool openAsFloat = rescaleSlope != 1.0;
                
                if (fileInfo.fileType == FileInfomation.GRAY16_SIGNED)
                {
                    if (rescaleIntercept != 0.0 && (rescaleSlope == 1.0))
                    {
                        double[] coeff = new double[2];
                        coeff[0] = rescaleSlope * (-32768) + rescaleIntercept;
                        coeff[1] = rescaleSlope;
                    }
                }
                else if (rescaleIntercept != 0.0 &&
                          (rescaleSlope == 1.0  || fileInfo.fileType == FileInfomation.GRAY8))
                {
                    double[] coeff = new double[2];
                    coeff[0] = rescaleIntercept;
                    coeff[1] = rescaleSlope;
                }
                if (windowWidth > 0.0)
                {
                    double min = windowCenter - windowWidth / 2;
                    double max = windowCenter + windowWidth / 2;

                    ImageProcessor ip = imp.GetProcessor();
                    ip.SetMinAndMax(min, max);
                }

                SetProcessor(filename, imp.GetProcessor());
                SetFileInfo(fileInfo); 
                Show();
            }
        }

        /// <summary>
        /// 读出Dicom文件信息
        /// </summary>
        /// <param name="dataset"></param>
        private void GetFileInfo(DicomDataset dataset)
        {
            bool signed = false;

            fileInfo.width = 0;
            fileInfo.height = 0;
            fileInfo.offset = 0;
            fileInfo.intelByteOrder = true;
            fileInfo.fileType = FileInfomation.GRAY16_UNSIGNED;
            fileInfo.fileFormat = FileInfomation.DICOM;
            int samplesPerPixel = 1;
            int planarConfiguration = 0;
            String photoInterpretation = "";

            //读取信息
            string s;
            double d;
            int index;

            foreach (DicomItem dicomItem in dataset)
            {
                if (dicomItem.Tag == DicomTag.NumberOfFrames)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    if (d > 1.0)
                        fileInfo.nImages = (int)d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.SamplesPerPixel)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    samplesPerPixel = (int)d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.PhotometricInterpretation)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    photoInterpretation = s;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.PlanarConfiguration)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    planarConfiguration = (int)d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.Rows)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    fileInfo.height = (int)d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.Columns)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    fileInfo.width = (int)d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.PixelSpacing || dicomItem.Tag == DicomTag.ImagerPixelSpacing)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    GetSpatialScale(ref fileInfo, s);
                    continue;
                }
                if (dicomItem.Tag == DicomTag.SliceThickness)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    fileInfo.pixelDepth = (int)d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.BitsAllocated)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    if (d == 8)
                        fileInfo.fileType = FileInfomation.GRAY8;
                    else if (d == 32)
                        fileInfo.fileType = FileInfomation.GRAY32_UNSIGNED;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.PixelRepresentation)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    if (d == 1)
                    {
                        fileInfo.fileType = FileInfomation.GRAY16_UNSIGNED;
                        signed = true;
                    }
                    continue;
                }
                if (dicomItem.Tag == DicomTag.WindowCenter)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    windowCenter = d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.WindowWidth)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    index = s.IndexOf('\\');
                    if (index != -1) s = s.Substring(index + 1);
                    windowWidth = s2d(s);
                    continue;
                }
                if (dicomItem.Tag == DicomTag.RescaleIntercept)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    rescaleIntercept = d;
                    continue;
                }
                if (dicomItem.Tag == DicomTag.RescaleSlope)
                {
                    s = dataset.GetString(dicomItem.Tag);
                    d = s2d(s, 0);
                    rescaleSlope = d;
                    continue;
                }
                
                if (dicomItem.Tag == DicomTag.FloatPixelData)
                {
                    fileInfo.fileType = FileInfomation.GRAY32_FLOAT;
                }
                if (dicomItem.Tag == DicomTag.PixelData)
                {
                    fileInfo.fileType = FileInfomation.GRAY32_FLOAT;
                }
            } 
            if (fileInfo.fileType == FileInfomation.GRAY8)
            {
                if (fileInfo.reds != null && fileInfo.greens != null && fileInfo.blues != null
                && fileInfo.reds.Length == fileInfo.greens.Length
                && fileInfo.reds.Length == fileInfo.blues.Length)
                {
                    fileInfo.fileType = FileInfomation.COLOR8;
                    fileInfo.lutSize = fileInfo.reds.Length;
                }
            }

            if (fileInfo.fileType == FileInfomation.GRAY32_UNSIGNED && signed)
                fileInfo.fileType = FileInfomation.GRAY32_INT;

            if (samplesPerPixel == 3 && photoInterpretation.StartsWith("RGB"))
            {
                if (planarConfiguration == 0)
                    fileInfo.fileType = FileInfomation.RGB;
                else if (planarConfiguration == 1)
                    fileInfo.fileType = FileInfomation.RGB_PLANAR;
            }
            else if (photoInterpretation.EndsWith("1 "))
                fileInfo.whiteIsZero = true;
        }

        private void GetSpatialScale(ref FileInfomation fi, String scale)
        {
            double xscale = 0, yscale = 0;
            int i = scale.IndexOf('\\');
            if (i > 0)
            {
                yscale = s2d(scale.Substring(0, i));
                xscale = s2d(scale.Substring(i + 1));
            }
            if (xscale != 0.0 && yscale != 0.0)
            {
                fi.pixelWidth = xscale;
                fi.pixelHeight = yscale;
                fi.unit = "mm";
            }
        }

        private EdgeImagePlus CreateImp()
        {
            EdgeImagePlus imp = null;
            object pixels = null;
            DicomPixelData pixelData = DicomPixelData.Create(dataset);
            switch (fileInfo.fileType)
            {
                case FileInfomation.GRAY8:
                case FileInfomation.COLOR8:
                case FileInfomation.BITMAP:
                    pixels = pixelData.GetFrame(0).Data;
                    if (pixels == null) return null;

                    ip = new ByteProcessor(fileInfo.width, fileInfo.height, (Byte[])pixels);
                    if (fileInfo.fileType == FileInfomation.CMYK)
                        ip.Invert();
                    imp = new EdgeImagePlus(fileInfo.fileName, ip);
                    break;

                case FileInfomation.GRAY16_SIGNED:
                case FileInfomation.GRAY16_UNSIGNED:
                case FileInfomation.GRAY12_UNSIGNED:
                    pixels = pixelData.GetFrame(0).Data;
                    if (pixels == null) return null;

                    ip = new ShortProcessor(fileInfo.width, fileInfo.height, (short[])pixels, imp);
                    imp = new EdgeImagePlus(fileInfo.fileName, ip);
                    break;

                case FileInfomation.GRAY32_INT:
                case FileInfomation.GRAY32_UNSIGNED:
                case FileInfomation.GRAY32_FLOAT:
                case FileInfomation.GRAY24_UNSIGNED:
                case FileInfomation.GRAY64_FLOAT:
                    pixels = pixelData.GetFrame(0).Data;
                    if (pixels == null) return null;

                    ip = new FloatProcessor(fileInfo.width, fileInfo.height, PublicFunctions.ByteArray2Float((Byte[])pixels), imp);
                    imp = new EdgeImagePlus(fileInfo.fileName, ip);
                    break;

                case FileInfomation.RGB:
                case FileInfomation.BGR:
                case FileInfomation.ARGB:
                case FileInfomation.ABGR:
                case FileInfomation.BARG:
                case FileInfomation.RGB_PLANAR:
                case FileInfomation.CMYK:
                    pixels = PublicFunctions.ByteArray2Color(pixelData.GetFrame(0).Data);
                    if (pixels == null) return null;

                    ip = new ColorProcessor(fileInfo.width, fileInfo.height, (Color[])pixels);
                    if (fileInfo.fileType == FileInfomation.CMYK)
                        ip.Invert();
                    imp = new EdgeImagePlus(fileInfo.fileName, ip);
                    break;
            }
            return imp;
        }

        private static double s2d(string s, double d = 0)
        {
            return PublicFunctions.ConvertStringToDouble(s, d);
        }
    }
}
