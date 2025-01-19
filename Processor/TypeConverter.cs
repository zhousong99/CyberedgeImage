namespace CyberedgeImageProcess2024
{
    internal class TypeConverter
    {
        private const int BYTE = 0, SHORT = 1, FLOAT = 2, RGB = 3;
        private ImageProcessor ip;
        private int type;
        Boolean doScaling = true;
        int width, height;

        public TypeConverter(ImageProcessor ip, Boolean doScaling)
        {
            this.ip = ip;
            this.doScaling = doScaling;
            if (ip is ByteProcessor)
                type = BYTE;

            else if (ip is ShortProcessor)

                type = SHORT;

            else if (ip is FloatProcessor)

                type = FLOAT;

            else
                type = RGB;
            width = ip.Width;
            height = ip.Height;
        }

        

        /** Converts a ColorProcessor to a FloatProcessor. 
            The pixels are converted to grayscale using the formula
            g=r/3+g/3+b/3. Call ColorProcessor.setRGBWeights() 
            to do weighted conversions. */
        FloatProcessor ConvertRGBToFloat(EdgeImagePlus imp)
        {
            int[] pixels = (int[])ip.GetPixels();
            double[] w = ColorProcessor.GetWeightingFactors();
            if (((ColorProcessor)ip).GetRGBWeights() != null)
                w = ((ColorProcessor)ip).GetRGBWeights();
            double rw = w[0], gw = w[1], bw = w[2];
            float[] pixels32 = new float[width * height];
            int c, r, g, b;
            for (int i = 0; i < width * height; i++)
            {
                c = pixels[i];
                r = (c & 0xff0000) >> 16;
                g = (c & 0xff00) >> 8;
                b = c & 0xff;
                pixels32[i] = (float)(r * rw + g * gw + b * bw);
            }
            return new FloatProcessor(width, height, pixels32, imp);
        }

        

        /** Converts processor to a FloatProcessor. */
        public ImageProcessor ConvertToFloat(float[] ctable, EdgeImagePlus imp)
        {
            switch (type)
            {
                case BYTE:
                    return ConvertByteToFloat(ctable, imp);
                case SHORT:
                    return ConvertShortToFloat(ctable, imp);
                case FLOAT:
                    return ip;
                case RGB:
                    return ConvertRGBToFloat(imp);
                default:
                    return null;
            }
        }

        /** Converts a ByteProcessor to a FloatProcessor. Applies a
         * calibration function if the 'cTable' is not null.
         * @see ImageProcessor.setCalibrationTable
         */
        FloatProcessor ConvertByteToFloat(float[] cTable, EdgeImagePlus imp)
        {
            int n = width * height;
            byte[] pixels8 = (byte[])ip.GetPixels();
            float[] pixels32 = new float[n];
            if (cTable != null && cTable.Length == 256)
            {
                for (int i = 0; i < n; i++)
                    pixels32[i] = cTable[pixels8[i] & 255];
            }
            else
            {
                for (int i = 0; i < n; i++)
                    pixels32[i] = pixels8[i] & 255;
            }

            return new FloatProcessor(width, height, pixels32, imp);
        }

        /** Converts a ShortProcessor to a FloatProcessor. Applies a
            calibration function if the calibration table is not null.
            @see ImageProcessor.setCalibrationTable
         */
        FloatProcessor ConvertShortToFloat(float[] cTable, EdgeImagePlus imp)
        {
            short[] pixels16 = (short[])ip.GetPixels();
            float[] pixels32 = new float[width * height];
            if (cTable != null && cTable.Length == 65536)
                for (int i = 0; i < width * height; i++)
                    pixels32[i] = cTable[pixels16[i] & 0xffff];
            else
                for (int i = 0; i < width * height; i++)
                    pixels32[i] = pixels16[i] & 0xffff;

            return new FloatProcessor(width, height, pixels32, imp);
        }

        /** Converts processor to a ColorProcessor. */
        public ImageProcessor ConvertToRGB(EdgeImagePlus imp)
        {
            if (type == RGB)
                return ip;
            else
            {
                ImageProcessor ip2 = ip.ConvertToByte(doScaling);
                return new ColorProcessor(ip2.CreateImage(), imp);
            }
        }
    }
}
