using Emgu.CV;

namespace CyberedgeImageProcess2024
{
    public class FloatProcessor : ImageProcessor
    {
        private float min, max, snapshotMin, snapshotMax;
        private float[] pixels;
        protected byte[] pixels8;
        private float[] snapshotPixels = null;
        private float fillColor = float.MaxValue;
        private bool fixedScale = false;
        private float bgValue;
        protected int rx, ry, rw, rh;
        protected int pixelCount;
        protected double pw, ph;


        public FloatProcessor(int width, int height, float[] pixels, EdgeImagePlus imp)
        {
            this.imp = imp;
            if (pixels != null && width * height != pixels.Length)
                throw new ArgumentException(WRONG_LENGTH);
            this.width = width;
            this.height = height;
            this.pixels = pixels;
            ResetRoi();
        }

        /** Creates a blank FloatProcessor using the default grayscale LUT that
            displays zero as black. Call invertLut() to display zero as white. */
        public FloatProcessor(int width, int height, EdgeImagePlus imp) : this(width, height, new float[width * height], imp)
        {

        }

        /** Creates a FloatProcessor from an int array using the default grayscale LUT. */
        public FloatProcessor(int width, int height, int[] pixels, EdgeImagePlus imp) : this(width, height, imp)
        {

            for (int i = 0; i < pixels.Length; i++)
                this.pixels[i] = (float)(pixels[i]);
        }

        /** Creates a FloatProcessor from a double array using the default grayscale LUT. */
        public FloatProcessor(int width, int height, double[] pixels, EdgeImagePlus imp) : this(width, height, imp)
        {

            for (int i = 0; i < pixels.Length; i++)
                this.pixels[i] = (float)pixels[i];
        }

        /** Creates a FloatProcessor from a 2D float array using the default LUT. */
        public FloatProcessor(float[][] array, EdgeImagePlus imp)
        {
            this.imp = imp;
            width = array.Length;
            height = array[0].Length;
            pixels = new float[width * height];
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[i++] = array[x][y];
                }
            }
            ResetRoi();
        }

        /** Creates a FloatProcessor from a 2D int array. */
        public FloatProcessor(int[][] array, EdgeImagePlus imp)
        {
            this.imp = imp;
            width = array.Length;
            height = array[0].Length;
            pixels = new float[width * height];
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[i++] = (float)array[x][y];
                }
            }
            ResetRoi();
        }

        /**
        Calculates the minimum and maximum pixel value for the entire image. 
        Returns without doing anything if fixedScale has been set true as a result
        of calling SetMinAndMax(). In this case, GetMin() and GetMax() return the
        fixed min and max defined by SetMinAndMax(), rather than the calculated min
        and max.
        @see #GetMin()
        @see #GetMin()
        */
        public void FindMinAndMax()
        {
            if (fixedScale)
                return;
            float min = float.NaN;
            float max = float.NaN;
            int len = width * height;
            int i = 0;
            for (; i < len; i++)
                if (!float.IsNaN(pixels[i]))
                    break;
            if (i < len)
            {
                min = pixels[i];
                max = pixels[i];
            }
            for (; i < len; i++)
            {
                float value = pixels[i];
                if (value < min)
                    min = value;
                else if (value > max)
                    max = value;
            }
            this.min = min;
            this.max = max;
            minMaxSet = true;
        }

        /** Sets the min and max variables that control how real
         * pixel values are mapped to 0-255 screen values. Use
         * ResetMinAndMax() to enable auto-scaling;
        */
        override public void SetMinAndMax(double minimum, double maximum)
        {
            if (minimum == 0.0 && maximum == 0.0)
            {
                ResetMinAndMax();
                return;
            }
            min = (float)minimum;
            max = (float)maximum;
            fixedScale = true;
            minMaxSet = true;
            ResetThreshold();
        }

        /** Recalculates the min and max values used to Scale pixel
            values to 0-255 for display. This ensures that this 
            FloatProcessor is set up to correctly display the image. */
        public override void ResetMinAndMax()
        {
            fixedScale = false;
            FindMinAndMax();
            ResetThreshold();
        }

        /** Returns the smallest displayed pixel value. */
        public override double GetMin()
        {
            if (!minMaxSet) FindMinAndMax();
            return min;
        }

        /** Returns the largest displayed pixel value. */
        public override double GetMax()
        {
            if (!minMaxSet) FindMinAndMax();
            return max;
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
                int size = width * height;
                double value;
                if (lutUpdateMode == BLACK_AND_WHITE_LUT)
                {
                    for (int i = 0; i < size; i++)
                    {
                        value = pixels[i];
                        if (value >= minThreshold && value <= maxThreshold)
                            pixels8[i] = (byte)255;
                        else
                            pixels8[i] = (byte)0;
                    }
                }
                else
                { // threshold red
                    for (int i = 0; i < size; i++)
                    {
                        value = pixels[i];
                        if (value >= minThreshold && value <= maxThreshold)
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
            ApplyLUT(ref resultMat);
            return resultMat;
        }

        // creates 8-bit image by linearly scaling from float to 8-bits
        private byte[] Create8BitImage(bool thresholding)
        {
            int size = width * height;
            if (pixels8 == null)
                pixels8 = new byte[size];
            double value;
            int ivalue;
            double min2 = GetMin();
            double max2 = GetMax();
            double Scale = 255.0 / (max2 - min2);
            int maxValue = thresholding ? 254 : 255;
            for (int i = 0; i < size; i++)
            {
                value = pixels[i] - min2;
                if (value < 0.0) value = 0.0;
                ivalue = (int)(value * Scale + 0.5);
                if (ivalue > maxValue) ivalue = maxValue;
                pixels8[i] = (byte)ivalue;
            }
            return pixels8;
        }


        /** Returns a new, blank FloatProcessor with the specified width and height. */
        override public ImageProcessor CreateProcessor(int width, int height)
        {
            ImageProcessor ip2 = new FloatProcessor(width, height, new float[width * height], imp);
            ip2.SetMinAndMax(GetMin(), GetMax());
            ip2.SetInterpolationMethod(interpolationMethod);
            return ip2;
        }

        override public void Snapshot()
        {
            snapshotWidth = width;
            snapshotHeight = height;
            snapshotMin = (float)GetMin();
            snapshotMax = (float)GetMax();
            if (snapshotPixels == null || (snapshotPixels != null && snapshotPixels.Length != pixels.Length))
                snapshotPixels = new float[width * height];
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

        /** Swaps the pixel and Snapshot (undo) arrays. */
        override public void SwapPixelArrays()
        {
            if (snapshotPixels == null) return;
            float pixel;
            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];
                pixels[i] = snapshotPixels[i];
                snapshotPixels[i] = pixel;
            }
        }

        override public void SetSnapshotPixels(Object pixels)
        {
            snapshotPixels = (float[])pixels;
            snapshotWidth = width;
            snapshotHeight = height;
        }

        override public Object GetSnapshotPixels()
        {
            return snapshotPixels;
        }

        /** Returns a pixel value that must be converted using
            float.intBitsTofloat(). */
        override public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return FloatToColor(pixels[y * width + x]);
            else
                return FloatToColor(0);
        }

        public sealed override Color Get(int x, int y)
        {
            return FloatToColor(pixels[y * width + x]);
        }

        override public void Set(int x, int y, Color value)
        {
            pixels[y * width + x] = ColorToFloat(value);
        }

        public Color Get(int index)
        {
            return FloatToColor(pixels[index]);
        }

        public void Set(int index, int value)
        {
            pixels[index] = IntBitsTofloat(value);
        }

        public float Getf(int x, int y)
        {
            return pixels[y * width + x];
        }

        public void Setf(int x, int y, float value)
        {
            pixels[y * width + x] = value;
        }

        public float Getf(int index)
        {
            return pixels[index];
        }

        public void Setf(int index, float value)
        {
            pixels[index] = value;
        }

        /** Returns the value of the pixel at (x,y) in a
            one element int array. iArray is an optiona
            preallocated array. */
        public int[] GetPixel(int x, int y, int[] iArray)
        {
            if (iArray == null) iArray = new int[1];
            iArray[0] = (int)GetPixelValue(x, y);
            return iArray;
        }

        /** Sets a pixel in the image using a one element int array. */
        public void PutPixel(int x, int y, int[] iArray)
        {
            PutPixelValue(x, y, iArray[0]);
        }

        /** Uses the current interpolation method (BILINEAR or BICUBIC) 
            to calculate the pixel value at real coordinates (x,y). */
        public override double GetInterpolatedPixel(double x, double y)
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

        public int GetPixelInterpolated(double x, double y)
        {
            if (interpolationMethod == BILINEAR)
            {
                if (x < 0.0 || y < 0.0 || x >= width - 1 || y >= height - 1)
                    return 0;
                else
                    return FloatToIntBits((float)GetInterpolatedPixel(x, y, pixels));
            }
            else if (interpolationMethod == BICUBIC)
                return FloatToIntBits((float)GetBicubicInterpolatedPixel(x, y, this));
            else
                return (int)ColorToIntBits(GetPixel((int)(x + 0.5), (int)(y + 0.5)));
        }

        /** Stores the specified value at (x,y). The value is expected to be a
            float that has been converted to an int using float.FloatToIntBits(). */
        override public void PutPixel(int x, int y, Color value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                pixels[y * width + x] = ColorToFloat(value);
        }

        /** Stores the specified real value at (x,y). */
        public void PutPixelValue(int x, int y, double value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                pixels[y * width + x] = (float)value;
        }

        /** Returns the value of the pixel at (x,y) as a float. */
        override public float GetPixelValue(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return pixels[y * width + x];
            else
                return float.NaN;
        }

        /** Draws a pixel in the current foreground color. */
        override public void DrawPixel(int x, int y)
        {
            if (x >= clipXMin && x <= clipXMax && y >= clipYMin && y <= clipYMax)
                PutPixel(x, y, FloatToColor(fillColor));
        }

        /** Returns a reference to the float array containing
            this image's pixel data. */
        override public Object GetPixels()
        {
            return (Object)pixels;
        }

        /** Returns a copy of the pixel data. Or returns a reference to the
            Snapshot buffer if it is not null and 'snapshotCopyMode' is true.
            @see ImageProcessor#Snapshot
            @see ImageProcessor#setSnapshotCopyMode
        */
        public Object GetPixelsCopy()
        {
            if (snapshotCopyMode && snapshotPixels != null)
            {
                snapshotCopyMode = false;
                return snapshotPixels;
            }
            else
            {
                float[] pixels2 = new float[width * height];
                System.Array.Copy(pixels, 0, pixels2, 0, width * height);
                return pixels2;
            }
        }

        override public void SetPixels(Object pixels)
        {
            this.pixels = (float[])pixels;
            ResetPixels(pixels);
            if (pixels == null) snapshotPixels = null;
            if (pixels == null) pixels8 = null;
        }

        /** Copies the image contained in 'ip' to (xloc, yloc) using one of
            the transfer modes defined in the Blitter interface. */
        override public void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {
            ip = ip.ConvertToFloat();
            new FloatBlitter(this).CopyBits(ip, xloc, yloc, mode);
        }

        override public void ApplyTable(int[] lut) { }

        private new void Process(int op, double value)
        {
            float c, v1, v2;
            c = (float)value;
            float min2 = 0f, max2 = 0f;
            if (op == INVERT)
            { min2 = (float)GetMin(); max2 = (float)GetMax(); }
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    v1 = pixels[i];
                    switch (op)
                    {
                        case INVERT:
                            v2 = max2 - (v1 - min2);
                            break;
                        case FILL:
                            v2 = fillColor;
                            break;
                        case SET:
                            v2 = c;
                            break;
                        case ADD:
                            v2 = v1 + c;
                            break;
                        case MULT:
                            v2 = v1 * c;
                            break;
                        case GAMMA:
                            if (v1 <= 0f)
                                v2 = 0f;
                            else
                                v2 = (float)Math.Exp(c * Math.Log(v1));
                            break;
                        case LOG:
                            v2 = (float)Math.Log(v1);
                            break;
                        case EXP:
                            v2 = (float)Math.Exp(v1);
                            break;
                        case SQR:
                            v2 = v1 * v1;
                            break;
                        case SQRT:
                            if (v1 <= 0f)
                                v2 = 0f;
                            else
                                v2 = (float)Math.Sqrt(v1);
                            break;
                        case ABS:
                            v2 = (float)Math.Abs(v1);
                            break;
                        case MINIMUM:
                            if (v1 < value)
                                v2 = (float)value;
                            else
                                v2 = v1;
                            break;
                        case MAXIMUM:
                            if (v1 > value)
                                v2 = (float)value;
                            else
                                v2 = v1;
                            break;
                        default:
                            v2 = v1;
                            break;
                    }
                    pixels[i++] = v2;
                }
            }
        }

        
        public new void Add(int value) { Process(ADD, value); }
        public void Add(double value) { Process(ADD, value); }
        public void Set(double value) { Process(SET, value); }
        public new void Multiply(double value) { Process(MULT, value); }
        public new void And(int value) { }
        public new void Or(int value) { }
        public new void Xor(int value) { }
        public new void Gamma(double value) { Process(GAMMA, value); }
        public void Log() { Process(LOG, 0.0); }
        public void Exp() { Process(EXP, 0.0); }
        public void Sqr() { Process(SQR, 0.0); }
        public void Sqrt() { Process(SQRT, 0.0); }
        public void Abs() { Process(ABS, 0.0); }
        public new void Min(double value) { Process(MINIMUM, value); }
        public new void Max(double value) { Process(MAXIMUM, value); }
        

        /** Fills the current rectangular ROI. */
        override public void Fill() { Process(FILL, 0.0); }

        /** Fills pixels that are within roi and part of the mask.
            Does nothing if the mask is not the same as the the ROI. */
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
                        pixels[i] = fillColor;
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
         *	contributed by Glynne Casteel. */
        void Filter3x3(int type, int[] kernel)
        {
            float v1, v2, v3;           //input pixel values around the current pixel
            float v4, v5, v6;
            float v7, v8, v9;
            float k1 = 0f, k2 = 0f, k3 = 0f;    //kernel values (used for CONVOLVE only)
            float k4 = 0f, k5 = 0f, k6 = 0f;
            float k7 = 0f, k8 = 0f, k9 = 0f;
            float Scale = 0f;
            if (type == CONVOLVE)
            {
                k1 = kernel[0]; k2 = kernel[1]; k3 = kernel[2];
                k4 = kernel[3]; k5 = kernel[4]; k6 = kernel[5];
                k7 = kernel[6]; k8 = kernel[7]; k9 = kernel[8];
                for (int i = 0; i < kernel.Length; i++)
                    Scale += kernel[i];
                if (Scale == 0) Scale = 1f;
                Scale = 1f / Scale; //multiplication factor (multiply is faster than divide)
            }

            float[] pixels2 = (float[])GetPixelsCopy();
            int xEnd = roiX + roiWidth;
            int yEnd = roiY + roiHeight;
            for (int y = roiY; y < yEnd; y++)
            {
                int p = roiX + y * width;           //points to current pixel
                int p6 = p - (roiX > 0 ? 1 : 0);        //will point to v6, currently lower
                int p3 = p6 - (y > 0 ? width : 0);  //will point to v3, currently lower
                int p9 = p6 + (y < height - 1 ? width : 0); // ...	to v9, currently lower
                v2 = pixels2[p3];
                v5 = pixels2[p6];
                v8 = pixels2[p9];
                if (roiX > 0) { p3++; p6++; p9++; }
                v3 = pixels2[p3];
                v6 = pixels2[p6];
                v9 = pixels2[p9];

                switch (type)
                {
                    case BLUR_MORE:
                        for (int x = roiX; x < xEnd; x++, p++)
                        {
                            if (x < width - 1) { p3++; p6++; p9++; }
                            v1 = v2; v2 = v3;
                            v3 = pixels2[p3];
                            v4 = v5; v5 = v6;
                            v6 = pixels2[p6];
                            v7 = v8; v8 = v9;
                            v9 = pixels2[p9];
                            pixels[p] = (v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9) * 0.11111111f; //0.111... = 1/9
                        }
                        break;
                    case FIND_EDGES:
                        for (int x = roiX; x < xEnd; x++, p++)
                        {
                            if (x < width - 1) { p3++; p6++; p9++; }
                            v1 = v2; v2 = v3;
                            v3 = pixels2[p3];
                            v4 = v5; v5 = v6;
                            v6 = pixels2[p6];
                            v7 = v8; v8 = v9;
                            v9 = pixels2[p9];
                            float sum1 = v1 + 2 * v2 + v3 - v7 - 2 * v8 - v9;
                            float sum2 = v1 + 2 * v4 + v7 - v3 - 2 * v6 - v9;
                            pixels[p] = (float)Math.Sqrt(sum1 * sum1 + sum2 * sum2);
                        }
                        break;
                    case CONVOLVE:
                        for (int x = roiX; x < xEnd; x++, p++)
                        {
                            if (x < width - 1) { p3++; p6++; p9++; }
                            v1 = v2; v2 = v3;
                            v3 = pixels2[p3];
                            v4 = v5; v5 = v6;
                            v6 = pixels2[p6];
                            v7 = v8; v8 = v9;
                            v9 = pixels2[p9];
                            float sum = k1 * v1 + k2 * v2 + k3 * v3
                                      + k4 * v4 + k5 * v5 + k6 * v6
                                      + k7 * v7 + k8 * v8 + k9 * v9;
                            sum *= Scale;
                            pixels[p] = sum;
                        }
                        break;
                }
            }
        }

 
        override public ImageProcessor Crop()
        {
            ImageProcessor ip2 = CreateProcessor(roiWidth, roiHeight);
            float[] pixels2 = (float[])ip2.GetPixels();
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
            float[] pixels2 = (float[])ip2.GetPixels();
            System.Array.Copy(pixels, 0, pixels2, 0, width * height);
            return ip2;
        }



        /** Uses bilinear interpolation to find the pixel value at real coordinates (x,y). */
        private double GetInterpolatedPixel(double x, double y, float[] pixels)
        {
            int xbase = (int)x;
            int ybase = (int)y;
            double xFraction = x - xbase;
            double yFraction = y - ybase;
            int offset = ybase * width + xbase;
            double lowerLeft = pixels[offset];
            double lowerRight = pixels[offset + 1];
            double upperRight = pixels[offset + width + 1];
            double upperLeft = pixels[offset + width];
            double upperAverage;
            if (Double.IsNaN(upperLeft) && xFraction >= 0.5)
                upperAverage = upperRight;
            else if (Double.IsNaN(upperRight) && xFraction < 0.5)
                upperAverage = upperLeft;
            else
                upperAverage = upperLeft + xFraction * (upperRight - upperLeft);
            double lowerAverage;
            if (Double.IsNaN(lowerLeft) && xFraction >= 0.5)
                lowerAverage = lowerRight;
            else if (Double.IsNaN(lowerRight) && xFraction < 0.5)
                lowerAverage = lowerLeft;
            else
                lowerAverage = lowerLeft + xFraction * (lowerRight - lowerLeft);
            if (Double.IsNaN(lowerAverage) && yFraction >= 0.5)
                return upperAverage;
            else if (Double.IsNaN(upperAverage) && yFraction < 0.5)
                return lowerAverage;
            else
                return lowerAverage + yFraction * (upperAverage - lowerAverage);
        }


        /** Creates a new FloatProcessor containing a scaled copy of this image or selection. */
        override public ImageProcessor Resize(int dstWidth, int dstHeight)
        {
            if (roiWidth == dstWidth && roiHeight == dstHeight)
                return Crop();
            FloatProcessor ip2 = new FloatProcessor(dstWidth, dstHeight, imp);
            Mat mat = image.Clone();
            CvInvoke.Resize(mat, mat, new Size(dstWidth, dstHeight), 0, 0, (Emgu.CV.CvEnum.Inter)interpolationMethod);
            ip2.CopyImageToPixels(mat);
            return ip2;
        }


        /** This method is from Chapter 16 of "Digital Image Processing:
            An Algorithmic Introduction Using Java" by Burger and Burge
            (http://www.imagingbook.com/). */
        public double GetBicubicInterpolatedPixel(double x0, double y0, FloatProcessor ip2)
        {
            int u0 = (int)Math.Floor(x0);   //use floor to handle negative coordinates too
            int v0 = (int)Math.Floor(y0);
            if (u0 <= 0 || u0 >= width - 2 || v0 <= 0 || v0 >= height - 2)
                return ColorToFloat(ip2.GetBilinearInterpolatedPixel(x0, y0));
            double q = 0;
            for (int j = 0; j <= 3; j++)
            {
                int v = v0 - 1 + j;
                double p = 0;
                for (int i = 0; i <= 3; i++)
                {
                    int u = u0 - 1 + i;
                    p = p + ip2.Getf(u, v) * Cubic(x0 - u);
                }
                q = q + p * Cubic(y0 - v);
            }
            return q;
        }



        /** Sets the background Fill/draw color. */
        override public void SetBackgroundColor(Color color)
        {
            int bestIndex = 128;
            double value = GetMin() + (GetMax() - GetMin()) * (bestIndex / 255.0);
            SetBackgroundValue(value);
        }

        /** Sets the default Fill/draw value. */
        override public void SetValue(Color value)
        {
            fillColor = ColorToFloat(value);
            fillValueSet = true;
        }



        override public void SetBackgroundValue(double value)
        {
            bgValue = (float)value;
        }

        override public Color GetBackgroundValue()
        {
            return FloatToColor(bgValue);
        }


        private int FloatToIntBits(float value)
        {
            int intValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            return intValue;
        }

        private Color FloatToColor(float value)
        {
            return IntBits2Color(FloatToIntBits(value));
        }

        public static Color IntBits2Color(int color)
        {
            int r = 0xFF & color;
            int g = 0xFF00 & color;
            g >>= 8;
            int b = 0xFF0000 & color;
            b >>= 16;
            int a = (int)(0xFF000000 & color);
            a >>= 24;
            return Color.FromArgb(a, r, g, b);
        }
        public static float IntBitsTofloat(int value)
        {
            byte[] buf = BitConverter.GetBytes(value);

            return BitConverter.ToSingle(buf, 0);
        }

        private int ColorToIntBits(Color color)
        {
                return (int)(((int)color.A << 24) | ((int)color.B << 16) | (ushort)(((ushort)color.G << 8) | color.R));
        }

        private float ColorToFloat(Color color)
        {
            return IntBitsTofloat(ColorToIntBits(color));
        }

        public override void CopyImageToPixels(Point point, Size size)
        {
            if (width * height != pixels.Length)    //新建， 否则就是覆盖
            {
                pixels = null;
                pixels = new float[width * height];
            }

            unsafe
            {
                byte* dataPtr = (byte*)image.DataPointer;
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

        override public int[] GetHistogram() { return null; }
    }
}
