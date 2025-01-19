using Emgu.CV;


namespace CyberedgeImageProcess2024
{
    /// <summary>
    /// 类型转换，其他未实现的参考imageJ的ImageConverter，StackConverter, TypeConverter
    /// </summary>
    static class Converter
    {
        private static bool doScaling = true;
        /** Converts the EdgeImagePlus to the specified image type. The string
argument corresponds to one of the labels in the Image/Type submenu
("8-bit", "16-bit", "32-bit", "8-bit Color", "RGB Color", "RGB Stack", "HSB Stack", "Lab Stack" or "HSB (32-bit)"). */
        public static void Convert(EdgeImagePlus imp, string item)
        {
            Roi roi = imp.Roi;
            imp.DeleteRoi();
            bool saveChanges = imp.changes;
            imp.changes = true;

            try
            {
                {
                    if (item == "8-bit")
                        ConvertToGray8(imp);
                }
            }
            catch
            {
                imp.changes = saveChanges;
                return;
            }
            if (roi != null)
                imp.SetRoi(roi);

            imp.RepaintWindow();
        }

        /** Converts this EdgeImagePlus to 8-bit grayscale. */
        public static void ConvertToGray8(EdgeImagePlus imp)
        {
            ImageProcessor ip = imp.GetProcessor();
           
            imp.SetProcessor(null, ConvertRGBToByte(ip, doScaling));
            ImageProcessor ip2 = imp.GetProcessor();
        }
 

        /** Converts a ColorProcessor to a ByteProcessor. 
	The pixels are converted to grayscale using the formula
	g=r/3+g/3+b/3. Call ColorProcessor.setRGBWeights() 
	to do weighted conversions. */
        public static ByteProcessor ConvertRGBToByte(ImageProcessor ip, bool doScaling)
        {
            if (ip is ByteProcessor) return (ByteProcessor)ip;

            Color[] pixelsColor = (Color[])ip.GetPixels();   //取出彩色像素
            double[] w = ColorProcessor.GetWeightingFactors();
            if (((ColorProcessor)ip).GetRGBWeights() != null)
                w = ((ColorProcessor)ip).GetRGBWeights();
            double rw = w[0], gw = w[1], bw = w[2];
            byte[] pixels8 = new byte[ip.Width * ip.Height];   //建立灰度像素
            Color c;
            int r, g, b;
            for (int i = 0; i < ip.Width * ip.Height; i++)
            {
                c = pixelsColor[i];
                r = c.R;
                g = c.G;
                b = c.B;
                pixels8[i] = (byte)(r * rw + g * gw + b * bw + 0.5);
            }
            return new ByteProcessor(ip.Width, ip.Height, pixels8, null);
        }

        public static ImageProcessor ConvertToRGB(ImageProcessor ip, bool doScaling)
        {
            if (ip is ColorProcessor)
                return ip;
            else
            {
                ImageProcessor ip2 = ip.ConvertToByte(doScaling);
                return new ColorProcessor(ip2.CreateImage());
            }
        }
    }
}