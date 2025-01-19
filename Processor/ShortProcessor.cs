using Emgu.CV;


namespace CyberedgeImageProcess2024
{
    public class ShortProcessor : ImageProcessor
    {
        protected int min, max, snapshotMin, snapshotMax; //wsr
        protected short[] pixels;
        protected byte[] pixels8;
        private short[] snapshotPixels;
        protected bool fixedScale;
        private int bgValue;


        /** Creates a new ShortProcessor using the specified pixel array and ColorModel.
    Set 'cm' to null to use the default grayscale LUT. */
        public ShortProcessor(int width, int height, short[] pixels, EdgeImagePlus imp)
        {
            this.imp = imp;
            if (pixels != null && width * height != pixels.Length)
                throw new ArgumentException(WRONG_LENGTH);
            Init(width, height, pixels);
        }

        /** Creates a blank ShortProcessor using the default grayscale LUT that
            displays zero as black. Call invertLut() to display zero as white. */
        public ShortProcessor(int width, int height, EdgeImagePlus imp) : this(width, height, new short[width * height], imp)
        {
        }


        void Init(int width, int height, short[] pixels)
        {
            this.width = width;
            this.height = height;
            this.pixels = pixels;
            ResetRoi();
        }

        /**
        * @deprecated
        * 16 bit images are normally unsigned but signed images can be simulated by
        * subtracting 32768 and using a calibration function to restore the original values.
        */
        public ShortProcessor(int width, int height, short[] pixels, EdgeImagePlus imp, bool unsigned) : this(width, height, pixels, imp)
        {
        }

        /** Obsolete. 16 bit images are normally unsigned but signed images can be used by
            subtracting 32768 and using a calibration function to restore the original values. */
        public ShortProcessor(int width, int height, EdgeImagePlus imp, bool unsigned) : this(width, height, imp)
        {
        }


        public void FindMinAndMax()
        {
            if (fixedScale || pixels == null)
                return;
            int size = width * height;
            int value;
            int min = pixels[0] & 0xffff;
            int max = pixels[0] & 0xffff;
            for (int i = 1; i < size; i++)
            {
                value = pixels[i] & 0xffff;
                if (value < min)
                    min = value;
                else if (value > max)
                    max = value;
            }
            this.min = min;
            this.max = max;
            minMaxSet = true;
        }

        /** Create an 8-bit AWT image by scaling pixels in the range min-max to 0-255. */
        override public Mat CreateImage()
        {
            if (!minMaxSet)
                FindMinAndMax();
            bool firstTime = pixels8 == null;
            bool thresholding = minThreshold != NO_THRESHOLD && lutUpdateMode < NO_LUT_UPDATE;

            if (firstTime || !lutAnimation)
                Create8BitImage(thresholding && lutUpdateMode == RED_LUT);

            if (thresholding)
            {
                int t1 = (int)minThreshold;
                int t2 = (int)maxThreshold;
                int size = width * height;
                int value;
                if (lutUpdateMode == BLACK_AND_WHITE_LUT)
                {
                    for (int i = 0; i < size; i++)
                    {
                        value = (pixels[i] & 0xffff);
                        if (value >= t1 && value <= t2)
                            pixels8[i] = (byte)255;
                        else
                            pixels8[i] = (byte)0;
                    }
                }
                else
                { // threshold red
                    for (int i = 0; i < size; i++)
                    {
                        value = (pixels[i] & 0xffff);
                        if (value >= t1 && value <= t2)
                            pixels8[i] = (byte)255;
                    }
                }
            }

            if (pixels8.Length != width * height)
            {
                throw new Exception("无法从pixels生成Mat");
            }
            if (image == null)
            {
                image = new Mat(new Size(width, height), Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            }
            unsafe
            {
                byte* dataPtr = (byte*)image.DataPointer;
                int chnls = image.NumberOfChannels;
                int step = image.Step;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Byte color = pixels8[y * width + x];
                        int index = y * step + x;
                        dataPtr[index] = color;
                    }
                }
            }
            Mat resultMat = new Mat();
            CvInvoke.CvtColor(image, resultMat, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);  //显示时只显示3通道，返回时帮忙处理掉
            lutAnimation = false;
            return resultMat;
        }

        // create 8-bit image by linearly scaling from 16-bits to 8-bits
        private byte[] Create8BitImage(bool thresholding)
        {
            int size = width * height;
            if (pixels8 == null)
                pixels8 = new byte[size];
            int value;
            int min2 = (int)GetMin(), max2 = (int)GetMax();
            int maxValue = 255;
            double scale = 256.0 / (max2 - min2 + 1);
            if (thresholding)
            {
                maxValue = 254;
                scale = 255.0 / (max2 - min2 + 1);
            }
            for (int i = 0; i < size; i++)
            {
                value = (pixels[i] & 0xffff) - min2;
                if (value < 0) value = 0;
                value = (int)(value * scale + 0.5);
                if (value > maxValue) value = maxValue;
                pixels8[i] = (byte)value;
            }
            return pixels8;
        }

        

        /** Returns a new, blank ShortProcessor with the specified width and height. */
        override public ImageProcessor CreateProcessor(int width, int height)
        {
            ImageProcessor ip2 = new ShortProcessor(width, height, new short[width * height], imp);
            ip2.SetMinAndMax(GetMin(), GetMax());
            ip2.SetInterpolationMethod(interpolationMethod);
            return ip2;
        }

        override public void Snapshot()
        {
            snapshotWidth = width;
            snapshotHeight = height;
            snapshotMin = (int)GetMin();
            snapshotMax = (int)GetMax();
            if (snapshotPixels == null || (snapshotPixels != null && snapshotPixels.Length != pixels.Length))
                snapshotPixels = new short[width * height];
            System.Array.Copy(pixels, 0, snapshotPixels, 0, width * height);
        }

        override public void Reset()
        {
            if (snapshotPixels == null)
                return;
            min = snapshotMin;
            max = snapshotMax;
            minMaxSet = true;
            System.Array.Copy(snapshotPixels, 0, pixels, 0, width * height);
        }

        override public void Reset(ImageProcessor mask)
        {
            if (mask == null || snapshotPixels == null)
                return;
            if (mask.Width != roiWidth || mask.Height != roiHeight)
                throw new ArgumentException(maskSizeError(mask));
            byte[] mpixels = (byte[])mask.GetPixels();
            for (int y = roiY, my = 0; y < (roiY + roiHeight); y++, my++)
            {
                int i = y * width + roiX;
                int mi = my * roiWidth;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    if (mpixels[mi++] == 0)
                        pixels[i] = snapshotPixels[i];
                    i++;
                }
            }
        }

        /** Swaps the pixel and snapshot (undo) arrays. */
        override public void SwapPixelArrays()
        {
            if (snapshotPixels == null) return;
            short pixel;
            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];
                pixels[i] = snapshotPixels[i];
                snapshotPixels[i] = pixel;
            }
        }

        override public void SetSnapshotPixels(Object pixels)
        {
            snapshotPixels = (short[])pixels;
            snapshotWidth = width;
            snapshotHeight = height;
        }

        override public Object GetSnapshotPixels()
        {
            return snapshotPixels;
        }

        /** Returns the smallest displayed pixel value. */
        override public double GetMin()
        {
            if (!minMaxSet) FindMinAndMax();
            return min;
        }

        /** Returns the largest displayed pixel value. */
        override public double GetMax()
        {
            if (!minMaxSet) FindMinAndMax();
            return max;
        }

        /**
        Sets the min and max variables that control how real
        pixel values are mapped to 0-255 screen values. With
        signed 16-bit images, use IJ.SetMinAndMax(imp,min,max).
        @see #ResetMinAndMax
        @see ij.plugin.frame.ContrastAdjuster 
        @see ij.IJ#SetMinAndMax(ij.ImagePlus,double,double)
        */
        override public void SetMinAndMax(double minimum, double maximum)
        {
            if (minimum == 0.0 && maximum == 0.0)
            {
                ResetMinAndMax();
                return;
            }
            min = (int)Math.Round(minimum);
            max = (int)Math.Round(maximum);
            fixedScale = true;
            minMaxSet = true;
            ResetThreshold();
        }

        /** Recalculates the min and max values used to scale pixel
            values to 0-255 for display. This ensures that this 
            ShortProcessor is set up to correctly display the image. */
        override public void ResetMinAndMax()
        {
            fixedScale = false;
            FindMinAndMax();
            ResetThreshold();
        }

        override public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return PublicFunctions.Int2Color(pixels[y * width + x] & 0xffff);
            else
                return Color.Black;
        }

        override public Color Get(int x, int y)
        { //wsr final
            return PublicFunctions.Int2Color(pixels[y * width + x] & 0xffff);
        }

        override public void Set(int x, int y, Color value)
        {
            pixels[y * width + x] = (short)PublicFunctions.Color2Int(value);
        }

        

        /** Uses the current interpolation method (BILINEAR or BICUBIC)
            to calculate the pixel value at real coordinates (x,y). */
        override public double GetInterpolatedPixel(double x, double y)
        {
            if (interpolationMethod == BICUBIC)
                return GetBicubicInterpolatedPixel(x, y, this);
            else
            {
                if (x < 0.0) x = 0.0;
                if (x >= width - 1.0) x = width - 1.001;
                if (y < 0.0) y = 0.0;
                if (y >= height - 1.0) y = height - 1.001;
                return GetInterpolatedPixel(x, y, pixels);
            }
        }

       

        /** Stores the specified value at (x,y). Does
            nothing if (x,y) is outside the image boundary.
            Values outside the range 0-65535 are clipped.
        */
        public void PutPixel(int x, int y, int value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (value > 65535) value = 65535;
                if (value < 0) value = 0;
                pixels[y * width + x] = (short)value;
            }
        }

        override public void PutPixel(int x, int y, Color color)
        {
            PutPixel(x, y, PublicFunctions.Color2Int(color));
        }

        /** Stores the specified real value at (x,y). Does nothing
            if (x,y) is outside the image boundary. Values outside 
            the range 0-65535 (-32768-32767 for signed images)
            are clipped. Support for signed values requires a calibration
            table, which is set up automatically with PlugInFilters.
        */
        public void PutPixelValue(int x, int y, double value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (cTable != null && cTable[0] == -32768f) // signed image
                    value += 32768.0;
                if (value > 65535.0)
                    value = 65535.0;
                else if (value < 0.0)
                    value = 0.0;
                pixels[y * width + x] = (short)(value + 0.5);
            }
        }

        /** Draws a pixel in the current foreground color. */
        override public void DrawPixel(int x, int y)
        {
            if (x >= clipXMin && x <= clipXMax && y >= clipYMin && y <= clipYMax)
                PutPixel(x, y, fgColor);
        }

        /** Returns the value of the pixel at (x,y) as a float. For signed
            images, returns a signed value if a calibration table has
            been set using setCalibrationTable() (this is done automatically 
            in PlugInFilters). */
        override public float GetPixelValue(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (cTable == null)
                    return pixels[y * width + x] & 0xffff;
                else
                    return cTable[pixels[y * width + x] & 0xffff];
            }
            else
                return float.NaN;
        }

        /**	Returns a reference to the short array containing this image's
            pixel data. To avoid sign extension, the pixel values must be
            accessed using a mask (e.g. int i = pixels[j]&0xffff). */
        override public Object GetPixels()
        {
            return (Object)pixels;
        }

        /** Returns a copy of the pixel data. Or returns a reference to the
            snapshot buffer if it is not null and 'snapshotCopyMode' is true.
            @see ImageProcessor#snapshot
            @see ImageProcessor#setSnapshotCopyMode
        */
        public Object GetPixelsCopy()
        {
            if (snapshotPixels != null && snapshotCopyMode)
            {
                snapshotCopyMode = false;
                return snapshotPixels;
            }
            else
            {
                short[] pixels2 = new short[width * height];
                System.Array.Copy(pixels, 0, pixels2, 0, width * height);
                return pixels2;
            }
        }

        override public void SetPixels(Object pixels)
        {
            this.pixels = (short[])pixels;
            ResetPixels(pixels);
            if (pixels == null) snapshotPixels = null;
            if (pixels == null) pixels8 = null;
            //Raster = null;
        }

        

        /** Copies the image contained in 'ip' to (xloc, yloc) using one of
            the transfer modes defined in the Blitter interface. */
        override public void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {

                ip = ip.ConvertToShort(false);
                new ShortBlitter(this).CopyBits(ip, xloc, yloc, mode);
            
        }

        /** Transforms the pixel data using a 65536 entry lookup table. */
        override public void ApplyTable(int[] lut)
        {
            if (lut.Length != 65536)
                throw new ArgumentException("lut.Length!=65536");
            int lineStart, lineEnd, v;
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                lineStart = y * width + roiX;
                lineEnd = lineStart + roiWidth;
                for (int i = lineEnd; --i >= lineStart;)
                {
                    v = lut[pixels[i] & 0xffff];
                    pixels[i] = (short)v;
                }
            }
            FindMinAndMax();
        }

        protected new void Process(int op, double value)
        { 
            int v1, v2;
            double range = GetMax() - GetMin();

            int offset = IsSigned16Bit() ? 32768 : 0;
            int min2 = (int)GetMin() - offset;
            int max2 = (int)GetMax() - offset;
            int fgColor2 = PublicFunctions.Color2Int(fgColor) - offset;
            int intValue = (int)value;

            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    v1 = (pixels[i] & 0xffff) - offset;
                    switch (op)
                    {
                        case INVERT:
                            v2 = max2 - (v1 - min2);
                            break;
                        case FILL:
                            v2 = fgColor2;
                            break;
                        case SET:
                            v2 = intValue;
                            break;
                        case ADD:
                            v2 = v1 + intValue;
                            break;
                        case MULT:
                            v2 = (int)Math.Round(v1 * value);
                            break;
                        case AND:
                            v2 = v1 & intValue;
                            break;
                        case OR:
                            v2 = v1 | intValue;
                            break;
                        case XOR:
                            v2 = v1 ^ intValue;
                            break;
                        case GAMMA:
                            if (range <= 0.0 || v1 == min2)
                                v2 = v1;
                            else
                                v2 = (int)(Math.Exp(value * Math.Log((v1 - min2) / range)) * range + min2);
                            break;
                        case LOG:
                            if (v1 <= 0)
                                v2 = 0;
                            else
                                v2 = (int)(Math.Log(v1) * (max2 / Math.Log(max2)));
                            break;
                        case EXP:
                            v2 = (int)(Math.Exp(v1 * (Math.Log(max2) / max2)));
                            break;
                        case SQR:
                            double d1 = v1;
                            v2 = (int)(d1 * d1);
                            break;
                        case SQRT:
                            v2 = (int)Math.Sqrt(v1);
                            break;
                        case ABS:
                            v2 = (int)Math.Abs(v1);
                            break;
                        case MINIMUM:
                            if (v1 < value)
                                v2 = intValue;
                            else
                                v2 = v1;
                            break;
                        case MAXIMUM:
                            if (v1 > value)
                                v2 = intValue;
                            else
                                v2 = v1;
                            break;
                        default:
                            v2 = v1;
                            break;
                    }
                    v2 += offset;
                    if (v2 < 0)
                        v2 = 0;
                    if (v2 > 65535)
                        v2 = 65535;
                    pixels[i++] = (short)v2;
                }
            }
        }

        

        public new void Add(int value) { Process(ADD, value); }
        public void Add(double value) { Process(ADD, value); }
        public void Set(double value) { Process(SET, value); }
        public new void Multiply(double value) { Process(MULT, value); }
        public new void And(int value) { Process(AND, value); }
        public new void Or(int value) { Process(OR, value); }
        public new void Xor(int value) { Process(XOR, value); }
        public new void Gamma(double value) { Process(GAMMA, value); }
        public void Log() { Process(LOG, 0.0); }
        public void Exp() { Process(EXP, 0.0); }
        public void Sqr() { Process(SQR, 0.0); }
        public void Sqrt() { Process(SQRT, 0.0); }
        public void Abs() { Process(ABS, 0.0); }
        public new void Min(double value) { Process(MINIMUM, value); }
        public new void Max(double value) { Process(MAXIMUM, value); }

        /** Fills the current rectangular ROI. */
        override public void Fill()
        {
            Process(FILL, 0.0);
        }

        /** Fills pixels that are within roi and part of the mask.
            Does nothing if the mask is not the same as the ROI. */
        override public void Fill(ImageProcessor mask)
        {
            if (mask == null)
            { Fill(); return; }
            int roiWidth = this.roiWidth, roiHeight = this.roiHeight;
            int roiX = this.roiX, roiY = this.roiY;
            if (mask.Width != roiWidth || mask.Height != roiHeight)
                return;
            byte[] mpixels = (byte[])mask.GetPixels();
            for (int y = roiY, my = 0; y < (roiY + roiHeight); y++, my++)
            {
                int i = y * width + roiX;
                int mi = my * roiWidth;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    if (mpixels[mi++] != 0)
                        pixels[i] = (short)PublicFunctions.Color2Int(fgColor);
                    i++;
                }
            }
        }

        /** Does 3x3 convolution. */
        override public void Convolve3x3(int[] kernel)
        {
            Filter3x3(CONVOLVE, kernel);
        }

        /** Filters using a 3x3 neighborhood. */
        override public void Filter(int type)
        {
            Filter3x3(type, null);
        }

        /** 3x3 filter operations, code partly based on 3x3 convolution code
         *  contributed by Glynne Casteel. */
        void Filter3x3(int type, int[] kernel)
        {
            int v1, v2, v3;           //input pixel values around the current pixel
            int v4, v5, v6;
            int v7, v8, v9;
            int k1 = 0, k2 = 0, k3 = 0;  //kernel values (used for CONVOLVE only)
            int k4 = 0, k5 = 0, k6 = 0;
            int k7 = 0, k8 = 0, k9 = 0;
            int scale = 0;
            if (type == CONVOLVE)
            {
                k1 = kernel[0]; k2 = kernel[1]; k3 = kernel[2];
                k4 = kernel[3]; k5 = kernel[4]; k6 = kernel[5];
                k7 = kernel[6]; k8 = kernel[7]; k9 = kernel[8];
                for (int i = 0; i < kernel.Length; i++)
                    scale += kernel[i];
                if (scale == 0) scale = 1;
            }

            short[] pixels2 = (short[])GetPixelsCopy();
            int xEnd = roiX + roiWidth;
            int yEnd = roiY + roiHeight;
            for (int y = roiY; y < yEnd; y++)
            {
                int p = roiX + y * width;            //points to current pixel
                int p6 = p - (roiX > 0 ? 1 : 0);      //will point to v6, currently lower
                int p3 = p6 - (y > 0 ? width : 0);    //will point to v3, currently lower
                int p9 = p6 + (y < height - 1 ? width : 0); // ...  to v9, currently lower
                v2 = pixels2[p3] & 0xffff;
                v5 = pixels2[p6] & 0xffff;
                v8 = pixels2[p9] & 0xffff;
                if (roiX > 0) { p3++; p6++; p9++; }
                v3 = pixels2[p3] & 0xffff;
                v6 = pixels2[p6] & 0xffff;
                v9 = pixels2[p9] & 0xffff;

                switch (type)
                {
                    case BLUR_MORE:
                        for (int x = roiX; x < xEnd; x++, p++)
                        {
                            if (x < width - 1) { p3++; p6++; p9++; }
                            v1 = v2; v2 = v3;
                            v3 = pixels2[p3] & 0xffff;
                            v4 = v5; v5 = v6;
                            v6 = pixels2[p6] & 0xffff;
                            v7 = v8; v8 = v9;
                            v9 = pixels2[p9] & 0xffff;
                            pixels[p] = (short)((v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9 + 4) / 9);
                        }
                        break;
                    case FIND_EDGES:
                        for (int x = roiX; x < xEnd; x++, p++)
                        {
                            if (x < width - 1) { p3++; p6++; p9++; }
                            v1 = v2; v2 = v3;
                            v3 = pixels2[p3] & 0xffff;
                            v4 = v5; v5 = v6;
                            v6 = pixels2[p6] & 0xffff;
                            v7 = v8; v8 = v9;
                            v9 = pixels2[p9] & 0xffff;
                            double sum1 = v1 + 2 * v2 + v3 - v7 - 2 * v8 - v9;
                            double sum2 = v1 + 2 * v4 + v7 - v3 - 2 * v6 - v9;
                            double result = Math.Sqrt(sum1 * sum1 + sum2 * sum2);
                            if (result > 65535.0) result = 65535.0;
                            pixels[p] = (short)result;
                        }
                        break;
                    case CONVOLVE:
                        for (int x = roiX; x < xEnd; x++, p++)
                        {
                            if (x < width - 1) { p3++; p6++; p9++; }
                            v1 = v2; v2 = v3;
                            v3 = pixels2[p3] & 0xffff;
                            v4 = v5; v5 = v6;
                            v6 = pixels2[p6] & 0xffff;
                            v7 = v8; v8 = v9;
                            v9 = pixels2[p9] & 0xffff;
                            int sum = k1 * v1 + k2 * v2 + k3 * v3
                                    + k4 * v4 + k5 * v5 + k6 * v6
                                    + k7 * v7 + k8 * v8 + k9 * v9;
                            sum = (sum + scale / 2) / scale;   //scale/2 for rounding
                            if (sum > 65535) sum = 65535;
                            if (sum < 0) sum = 0;
                            pixels[p] = (short)sum;
                        }
                        break;
                }
            }
        }

        

        

        /** Uses bilinear interpolation to find the pixel value at real coordinates (x,y). */
        private double GetInterpolatedPixel(double x, double y, short[] pixels)
        {
            int xbase = (int)x;
            int ybase = (int)y;
            double xFraction = x - xbase;
            double yFraction = y - ybase;
            int offset = ybase * width + xbase;
            int lowerLeft = pixels[offset] & 0xffff;
            int lowerRight = pixels[offset + 1] & 0xffff;
            int upperRight = pixels[offset + width + 1] & 0xffff;
            int upperLeft = pixels[offset + width] & 0xffff;
            double upperAverage = upperLeft + xFraction * (upperRight - upperLeft);
            double lowerAverage = lowerLeft + xFraction * (lowerRight - lowerLeft);
            return lowerAverage + yFraction * (upperAverage - lowerAverage);
        }

        /** Creates a new ShortProcessor containing a scaled copy of this image or selection. */
        override public ImageProcessor Resize(int dstWidth, int dstHeight)
        {
            ShortProcessor ip2 = new ShortProcessor(dstWidth, dstHeight, imp);
            Mat mat = image.Clone();
            CvInvoke.Resize(mat, mat, new Size(dstWidth, dstHeight), 0, 0, (Emgu.CV.CvEnum.Inter)interpolationMethod);
            ip2.CopyImageToPixels(mat);
            return ip2;
            
        }

        override public ImageProcessor Crop()
        {
            ImageProcessor ip2 = CreateProcessor(roiWidth, roiHeight);
            short[] pixels2 = (short[])ip2.GetPixels();
            for (int ys = roiY; ys < roiY + roiHeight; ys++)
            {
                int offset1 = (ys - roiY) * roiWidth;
                int offset2 = ys * width + roiX;
                for (int xs = 0; xs < roiWidth; xs++)
                    pixels2[offset1++] = pixels[offset2++];
            }
            return ip2;
        }

        /** Returns a duplicate of this image. */
        override public ImageProcessor Duplicate()
        {
            ImageProcessor ip2 = CreateProcessor(width, height);
            short[] pixels2 = (short[])ip2.GetPixels();
            System.Array.Copy(pixels, 0, pixels2, 0, width * height);
            return ip2;
        }

        

        /** Sets the background fill/draw color. */
        override public void SetBackgroundColor(Color color)
        {
            //int bestIndex = GetBestIndex(color);
            int bestIndex = 128;
            int value = (int)(GetMin() + (GetMax() - GetMin()) * (bestIndex / 255.0));
            SetBackgroundValue(value);
        }

        /** Sets the default fill/draw value, where 0<=value<=65535). */
        public void SetValue(double value)
        {
            fgColor = PublicFunctions.Int2Color((int)value);
            if (value < 0) fgColor = PublicFunctions.Int2Color(0);
            if (value > 65535) fgColor = PublicFunctions.Int2Color(65535);
        }

        override public void SetValue(Color color)
        {
            SetValue(PublicFunctions.Color2Int(color));
        }

        /** Returns the foreground fill/draw value. */
        public double GetForegroundValue()
        {
            return PublicFunctions.Color2Int(fgColor);
        }

        override public void SetBackgroundValue(double value)
        {
            bgValue = (int)value;
            if (bgValue < 0) bgValue = 0;
            if (bgValue > 65535) bgValue = 65535;
        }

        override public Color GetBackgroundValue()
        {
            return PublicFunctions.Int2Color(bgValue);
        }

        /** Returns 65,536 bin histogram of the current ROI, which
            can be non-rectangular. */
        override public int[] GetHistogram()
        {
            if (mask != null)
                return GetHistogram(mask);
            int roiX = this.roiX, roiY = this.roiY;
            int roiWidth = this.roiWidth, roiHeight = this.roiHeight;
            int[] histogram = new int[65536];
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                    histogram[pixels[i++] & 0xffff]++;
            }
            return histogram;
        }

        private int[] GetHistogram(ImageProcessor mask)
        {
            if (mask.Width != this.roiWidth || mask.Height != this.roiHeight)
                throw new ArgumentException(maskSizeError(mask));
            int roiX = this.roiX, roiY = this.roiY;
            int roiWidth = this.roiWidth, roiHeight = this.roiHeight;
            byte[] mpixels = (byte[])mask.GetPixels();
            int[] histogram = new int[65536];
            for (int y = roiY, my = 0; y < (roiY + roiHeight); y++, my++)
            {
                int i = y * width + roiX;
                int mi = my * roiWidth;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    if (mpixels[mi++] != 0)
                        histogram[pixels[i] & 0xffff]++;
                    i++;
                }
            }
            return histogram;
        }

        /** Creates a histogram of length maxof(max+1,256). For small 
            images or selections, computations using these histograms 
            are faster compared to 65536 element histograms. */
        private int[] GetHistogram2()
        {
            if (this.mask != null)
                return GetHistogram2(mask);
            int roiX = this.roiX, roiY = this.roiY;
            int roiWidth = this.roiWidth, roiHeight = this.roiHeight;
            int max = 0;
            int value;
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int index = y * width + roiX;
                for (int i = 0; i < roiWidth; i++)
                {
                    value = pixels[index++] & 0xffff;
                    if (value > max)
                        max = value;
                }
            }
            int size = max + 1;
            if (size < 256) size = 256;
            int[] histogram = new int[size];
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int index = y * width + roiX;
                for (int i = 0; i < roiWidth; i++)
                    histogram[pixels[index++] & 0xffff]++;
            }
            return histogram;
        }

        private int[] GetHistogram2(ImageProcessor mask)
        {
            if (mask.Width != this.roiWidth || mask.Height != this.roiHeight)
                throw new ArgumentException(maskSizeError(mask));
            int roiX = this.roiX, roiY = this.roiY;
            int roiWidth = this.roiWidth, roiHeight = this.roiHeight;
            byte[] mpixels = (byte[])mask.GetPixels();
            int max = 0;
            int value;
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int index = y * width + roiX;
                for (int i = 0; i < roiWidth; i++)
                {
                    value = pixels[index++] & 0xffff;
                    if (value > max)
                        max = value;
                }
            }
            int size = max + 1;
            if (size < 256) size = 256;
            int[] histogram = new int[size];
            for (int y = roiY, my = 0; y < (roiY + roiHeight); y++, my++)
            {
                int index = y * width + roiX;
                int mi = my * roiWidth;
                for (int i = 0; i < roiWidth; i++)
                {
                    if (mpixels[mi++] != 0)
                        histogram[pixels[index] & 0xffff]++;
                    index++;
                }
            }
            return histogram;
        }

        public void SetLutAnimation(bool lutAnimation)
        {
            this.lutAnimation = false;
        }

        

        public void Threshold(int level)
        {
            for (int i = 0; i < width * height; i++)
            {
                if ((pixels[i] & 0xffff) <= level)
                    pixels[i] = 0;
                else
                    pixels[i] = (short)255;
            }
            FindMinAndMax();
        }

        

        /** Returns 'true' if this is a signed 16-bit image. */
        public bool IsSigned16Bit()
        {
            return cTable != null && cTable[0] == -32768f && cTable[1] == -32767f;
        }

        /** Returns a binary mask, or null if a threshold is not set. */
        public ByteProcessor CreateMask()
        {
            if (GetMinThreshold() == NO_THRESHOLD)
                return null;
            int minThreshold = (int)GetMinThreshold();
            int maxThreshold = (int)GetMaxThreshold();
            ByteProcessor mask = new ByteProcessor(width, height);
            byte[] mpixels = (byte[])mask.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                int value = pixels[i] & 0xffff;
                if (value >= minThreshold && value <= maxThreshold)
                    mpixels[i] = (byte)255;
            }
            return mask;
        }

        protected override Mat CLAHE(Mat mat)
        {
            Mat dst = new Mat();
            CvInvoke.CLAHE(mat, 40, new Size(8, 8), dst);
            return dst;
        }

        protected override Mat EqualizeHist(Mat mat)
        {
            Mat dst = new Mat();
            CvInvoke.EqualizeHist(mat, dst);
            return dst;
        }

        public override void CopyImageToPixels(Point point, Size size)
        {
            if (width * height != pixels.Length)    //新建， 否则就是覆盖
            {
                pixels = null;
                pixels = new short[width * height];
            }

            unsafe
            {
                short* dataPtr = (short*)image.DataPointer;
                int step = image.Step;
                for (int y = point.Y; y < point.Y + size.Height; y++)
                {
                    for (int x = point.X; x < point.X + size.Width; x++)
                    {
                        int index = y * step + x;
                        pixels[y * width + x] = dataPtr[index];
                    }
                }
            }
        }

        /** Not implemented. */
        //public void MedianFilter() { }
        /** Not implemented. */
        //public void Erode() { }
        /** Not implemented. */
        //public void Dilate() { }



    }
}
