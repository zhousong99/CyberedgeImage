using Emgu.CV;
using Emgu.CV.Util;

namespace CyberedgeImageProcess2024
{
    public class ColorProcessor : ImageProcessor
    {
        protected Color[] pixels;
        protected Color[] snapshotPixels = null;
        private Color bgColor = Color.White; //white
        protected int min = 0, max = 255;
        private bool caSnapshot;

        private static double rWeight = 1d / 3d, gWeight = 1d / 3d, bWeight = 1d / 3d;
        private double[] weights;


        /// <summary>
        /// 用Mat建立一个处理器，将图像放入pixels[]中
        /// </summary>
        /// <param name="img"></param>
        public ColorProcessor(Mat img)
        {
            image = img.Clone();
            width = img.Width;
            height = img.Height;
            pixels = new Color[width * height];
            unsafe
            {
                byte* dataPtr = (byte*)img.DataPointer;
                int chnls = img.NumberOfChannels;
                int step = img.Step;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte b = 0, g = 0, r = 0, a = 0xff;

                        for (int c = 0; c < chnls; c++)
                        {
                            int index = y * step + x * chnls + c;
                            byte value = dataPtr[index];

                            switch (c)
                            {
                                case (0):
                                    b = value;
                                    break;
                                case (1):
                                    g = value;
                                    break;
                                case (2):
                                    r = value;
                                    break;
                                case (3):
                                    a = value;
                                    break;
                            }
                        }
                        Color color = Color.FromArgb(a, r, g, b);
                        pixels[y * width + x] = color;
                    }
                }
            }

            fgColor = Color.Black;
            ResetRoi();
        }

        public ColorProcessor(Mat img, EdgeImagePlus imp) : this(img)
        {
            this.imp = imp;
        }

        /**Creates a blank ColorProcessor of the specified dimensions. */
        public ColorProcessor(int width, int height, EdgeImagePlus imp) : this(width, height, new Color[width * height], imp)
        {
            ;
        }

        /**Creates a ColorProcessor from a pixel array. */
        public ColorProcessor(int width, int height, Color[] pixels, EdgeImagePlus imp = null)
        {
            this.imp = imp;
            if (pixels != null && width * height != pixels.Length)
                throw new Exception(WRONG_LENGTH);
            this.width = width;
            this.height = height;
            fgColor = Color.Black; //black
            ResetRoi();
            this.pixels = pixels;
            this.image = CreateImage();
        }



        /// <summary>
        /// 从pixels[]中取出颜色，生成MAT。根据LUT生成伪彩色？？
        /// </summary>
        /// <returns></returns>
        override public Mat CreateImage()
        {
            if (pixels.Length != width * height)
            {
                throw new Exception("无法从pixels生成Mat");
            }

            if (image == null)
            {
                image = new Mat(new Size(width, height), Emgu.CV.CvEnum.DepthType.Cv8U, 3);
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
                        Color color = pixels[y * width + x];
                        for (int c = 0; c < chnls; c++)
                        {
                            int index = y * step + x * chnls + c;
                            switch (c)
                            {
                                case (0):
                                    dataPtr[index] = color.B;
                                    break;
                                case (1):
                                    dataPtr[index] = color.G;
                                    break;
                                case (2):
                                    dataPtr[index] = color.R;
                                    break;
                                case (3):
                                    dataPtr[index] = color.A;
                                    break;
                            }
                        }
                    }
                }
            }
            return image;
        }

        /** Fills pixels that are within roi and part of the mask.
	Does nothing if the mask is not the same as the the ROI. */
        override public void Fill(ImageProcessor mask)
        {
            if (mask == null)
            {
                Fill();
                return;
            }
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
                        pixels[i] = fgColor;
                    i++;
                }
            }
        }

        /** Fills the current rectangular ROI. */
        override public void Fill()
        {
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                    pixels[i++] = fgColor;
            }
        }

        override public ImageProcessor Crop()
        {
            Color[] pixels2 = new Color[roiWidth * roiHeight];
            for (int ys = roiY; ys < roiY + roiHeight; ys++)
            {
                int offset1 = (ys - roiY) * roiWidth;
                int offset2 = ys * width + roiX;
                for (int xs = 0; xs < roiWidth; xs++)
                    pixels2[offset1++] = pixels[offset2++];
            }
            ColorProcessor cp2 = new ColorProcessor(roiWidth, roiHeight, pixels2, imp);
            return cp2;
        }

        /** Returns a new, blank ColorProcessor with the specified width and height. */
        public override ImageProcessor CreateProcessor(int width, int height)
        {
            ImageProcessor ip2 = new ColorProcessor(width, height, imp);
            ip2.SetInterpolationMethod(interpolationMethod);
            return ip2;
        }

        /** Returns a duplicate of this image. */
        override public ImageProcessor Duplicate()
        {
            Color[] pixels2 = new Color[width * height];
            System.Array.Copy(pixels, 0, pixels2, 0, width * height);
            return new ColorProcessor(width, height, pixels2, imp);
        }

        /** Uses a table look-up to map the pixels in this image from min-max to 0-255. */
        override public void SetMinAndMax(double min, double max)
        {
            SetMinAndMax(min, max, 7);
        }

        public void SetMinAndMax(double min, double max, int channels)
        {
            if (max < min)
                return;
            this.min = (int)min;
            this.max = (int)max;
            int v;
            int[] lut = new int[256];
            for (int i = 0; i < 256; i++)
            {
                v = i - this.min;
                v = (int)(256.0 * v / (max - min));
                if (v < 0)
                    v = 0;
                if (v > 255)
                    v = 255;
                lut[i] = v;
            }
            Reset();
            if (channels == 7)
                ApplyTable(lut);
            else
                ApplyTable(lut, channels);
        }

        override public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return pixels[y * width + x];
            else
                return Color.Black;
        }

        /**	Returns a reference to the int array containing
		this image's pixel data. */
        override public Object GetPixels()
        {
            return (Object)pixels;
        }


        override public void Reset()
        {
            if (snapshotPixels != null)
                System.Array.Copy(snapshotPixels, 0, pixels, 0, width * height);
        }


        override public void Reset(ImageProcessor mask)
        {
            if (mask == null || snapshotPixels == null)
                return;
            if (mask.Width != roiWidth || mask.Height != roiHeight)
                throw new Exception(maskSizeError(mask));
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

        override public void Snapshot()
        {
            snapshotWidth = width;
            snapshotHeight = height;
            if (snapshotPixels == null || (snapshotPixels != null && snapshotPixels.Length != pixels.Length))
                snapshotPixels = new Color[width * height];
            System.Array.Copy(pixels, 0, snapshotPixels, 0, width * height);
            caSnapshot = false;
        }

        override public void Convolve3x3(int[] kernel)
        {
            int k1 = kernel[0], k2 = kernel[1], k3 = kernel[2],
                k4 = kernel[3], k5 = kernel[4], k6 = kernel[5],
                k7 = kernel[6], k8 = kernel[7], k9 = kernel[8];
            Color p1, p2, p3, p4, p5, p6, p7, p8, p9;

            int scale = k1 + k2 + k3 + k4 + k5 + k6 + k7 + k8 + k9;
            if (scale == 0) scale = 1;
            int inc = roiHeight / 25;
            if (inc < 1) inc = 1;

            Color[] pixels2 = (Color[])GetPixelsCopy();
            System.Array.Copy(pixels, pixels2, pixels.Length);
            int offset;
            int rsum = 0, gsum = 0, bsum = 0;
            int rowOffset = width;
            for (int y = yMin; y <= yMax; y++)
            {
                offset = xMin + y * width;
                p1 = Color.Black;
                p2 = pixels2[offset - rowOffset - 1];
                p3 = pixels2[offset - rowOffset];
                p4 = Color.Black;
                p5 = pixels2[offset - 1];
                p6 = pixels2[offset];
                p7 = Color.Black;
                p8 = pixels2[offset + rowOffset - 1];
                p9 = pixels2[offset + rowOffset];

                for (int x = xMin; x <= xMax; x++)
                {
                    p1 = p2; p2 = p3;
                    p3 = pixels2[offset - rowOffset + 1];
                    p4 = p5; p5 = p6;
                    p6 = pixels2[offset + 1];
                    p7 = p8; p8 = p9;
                    p9 = pixels2[offset + rowOffset + 1];

                    rsum = k1 * p1.R
                         + k2 * p2.R
                         + k3 * p3.R
                         + k4 * p4.R
                         + k5 * p5.R
                         + k6 * p6.R
                         + k7 * p7.R
                         + k8 * p8.R
                         + k9 * p9.R;
                    rsum /= scale;
                    if (rsum > 255) rsum = 255;
                    if (rsum < 0) rsum = 0;

                    gsum = k1 * p1.G
                         + k2 * p2.G
                         + k3 * p3.G
                         + k4 * p4.G
                         + k5 * p5.G
                         + k6 * p6.G
                         + k7 * p7.G
                         + k8 * p8.G
                         + k9 * p9.G;
                    gsum /= scale;
                    if (gsum > 255) gsum = 255;
                    if (gsum < 0) gsum = 0;

                    bsum = k1 * p1.B
                         + k2 * p2.B
                         + k3 * p3.B
                         + k4 * p4.B
                         + k5 * p5.B
                         + k6 * p6.B
                         + k7 * p7.B
                         + k8 * p8.B
                         + k9 * p9.B;
                    bsum /= scale;
                    if (bsum > 255) bsum = 255;
                    if (bsum < 0) bsum = 0;

                    pixels[offset++] = Color.FromArgb(255, rsum, gsum, bsum);
                }
                if (y % inc == 0)
                    showProgress((double)(y - roiY) / roiHeight);
            }
            showProgress(1.0);
        }

        /** 3x3 convolution contributed by Glynne Casteel. */
        override public void Convolve3x3(ConvolutionKernelF kernel)
        {
            int[] k = new int[9];
            k[0] = (int)kernel[0, 0];
            k[1] = (int)kernel[0, 1];
            k[2] = (int)kernel[0, 2];
            k[3] = (int)kernel[1, 0];
            k[4] = (int)kernel[1, 1];
            k[5] = (int)kernel[1, 2];
            k[6] = (int)kernel[2, 0];
            k[7] = (int)kernel[2, 1];
            k[8] = (int)kernel[2, 2];
            Convolve3x3(k);
        }

        /// <summary>
        /// 利用Convolve3x3函数，支持BLUR_MORE,  FIND_EDGES, MEDIAN_FILTER, MIN or MAX等功能
        /// </summary>
        /// <param name="type"></param>
        override public void Filter(int type)
        {
            if (type == FIND_EDGES)
                FilterRGB(RGB_FIND_EDGES, 0, 0);
            else if (type == MEDIAN_FILTER)
                FilterRGB(RGB_MEDIAN, 0, 0);
            else if (type == MIN)
                FilterRGB(RGB_MIN, 0, 0);
            else if (type == MAX)
                FilterRGB(RGB_MAX, 0, 0);
            else
                BlurMore();
        }

        public const int RGB_NOISE = 0, RGB_MEDIAN = 1, RGB_FIND_EDGES = 2,
    RGB_ERODE = 3, RGB_DILATE = 4, RGB_THRESHOLD = 5, RGB_ROTATE = 6,
    RGB_SCALE = 7, RGB_RESIZE = 8, RGB_TRANSLATE = 9, RGB_MIN = 10, RGB_MAX = 11;

        /** Performs the specified filter on the red, green and blue planes of this image. */
        public void FilterRGB(int type, double arg)
        {
            FilterRGB(type, arg, 0.0);
        }

        ImageProcessor FilterRGB(int type, double arg, double arg2)
        {
            byte[] R = new byte[width * height];
            byte[] G = new byte[width * height];
            byte[] B = new byte[width * height];
            GetRGB(R, G, B);
            Rectangle roi = new Rectangle(roiX, roiY, roiWidth, roiHeight);

            ByteProcessor r = new ByteProcessor(width, height, R, null);
            r.SetRoi(roi);
            ByteProcessor g = new ByteProcessor(width, height, G, null);
            g.SetRoi(roi);
            ByteProcessor b = new ByteProcessor(width, height, B, null);
            b.SetRoi(roi);
            r.SetBackgroundValue(bgColor.R);
            g.SetBackgroundValue(bgColor.G);
            b.SetBackgroundValue(bgColor.B);
            r.SetInterpolationMethod(interpolationMethod);
            g.SetInterpolationMethod(interpolationMethod);
            b.SetInterpolationMethod(interpolationMethod);

            switch (type)
            {

                case RGB_FIND_EDGES:
                    r.FindEdges(); showProgress(0.40);
                    g.FindEdges(); showProgress(0.65);
                    b.FindEdges(); showProgress(0.90);
                    break;
            }

            R = (byte[])r.GetPixels();
            G = (byte[])g.GetPixels();
            B = (byte[])b.GetPixels();

            SetRGB(R, G, B);
            showProgress(1.0);
            return null;
        }

        /** BLUR MORE: 3x3 unweighted smoothing is implemented directly, does not convert the image to three ByteProcessors. */
        private void BlurMore()
        {
            Color p1 = Color.White, p2, p3, p4 = Color.White, p5, p6, p7 = Color.White, p8, p9;

            Color[] prevRow = new Color[width];
            Color[] thisRow = new Color[width];
            Color[] nextRow = new Color[width];
            System.Array.Copy(pixels, Math.Max(roiY - 1, 0) * width, thisRow, 0, width);
            System.Array.Copy(pixels, roiY * width, nextRow, 0, width);
            for (int y = roiY; y < roiY + roiHeight; y++)
            {
                Color[] tmp = prevRow;
                prevRow = thisRow;
                thisRow = nextRow;
                nextRow = tmp;
                if (y < height - 1)
                {
                    System.Array.Copy(pixels, (y + 1) * width, nextRow, 0, width);
                }
                else
                {
                    nextRow = thisRow;
                }
                int offset = roiX + y * width;

                p2 = prevRow[roiX == 0 ? roiX : roiX - 1];
                p3 = prevRow[roiX];
                p5 = thisRow[roiX == 0 ? roiX : roiX - 1];
                p6 = thisRow[roiX];
                p8 = nextRow[roiX == 0 ? roiX : roiX - 1];
                p9 = nextRow[roiX];

                for (int x = roiX; x < roiX + roiWidth; x++)
                {
                    p1 = p2; p2 = p3;
                    p4 = p5; p5 = p6;
                    p7 = p8; p8 = p9;
                    if (x < width - 1)
                    {
                        p3 = prevRow[x + 1];
                        p6 = thisRow[x + 1];
                        p9 = nextRow[x + 1];
                    }
                    int rsum = p1.R + p2.R + p3.R + p4.R + p5.R + p6.R + p7.R + p8.R + p9.R;
                    int gsum = p1.G + p2.G + p3.G + p4.G + p5.G + p6.G + p7.G + p8.G + p9.G;
                    int bsum = p1.B + p2.B + p3.B + p4.B + p5.B + p6.B + p7.B + p8.B + p9.B;
                    pixels[offset++] = Color.FromArgb(255, (int)((rsum + 4) / 9), (int)((gsum + 4) / 9), (int)((bsum + 4) / 9));
                }
                if (roiHeight * roiWidth > 1000000 && (y & 0xff) == 0)
                    showProgress((double)(y - roiY) / roiHeight);
            }
            showProgress(1.0);
        }

        override public void FindEdges()
        {
            FilterRGB(RGB_FIND_EDGES, 0.0);
        }


        public Object GetPixelsCopy()
        {
            if (snapshotPixels != null && snapshotCopyMode)
            {
                snapshotCopyMode = false;
                return snapshotPixels;
            }
            else
            {
                Color[] pixels2 = new Color[width * height];
                System.Array.Copy(pixels, 0, pixels2, 0, width * height);
                return pixels2;
            }
        }

        /** Sets the current pixels from 3 byte arrays (reg, green, blue). */
        public void SetRGB(byte[] R, byte[] G, byte[] B)
        {
            for (int i = 0; i < width * height; i++)
                pixels[i] = Color.FromArgb(R[i], G[i], B[i]);
        }

        /** Returns the red, green and blue planes as 3 byte arrays. */
        public void GetRGB(byte[] R, byte[] G, byte[] B)
        {
            Color c;
            int r, g, b;
            for (int i = 0; i < width * height; i++)
            {
                c = pixels[i];
                r = c.R;
                g = c.G;
                b = c.B;
                R[i] = (byte)r;
                G[i] = (byte)g;
                B[i] = (byte)b;
            }
        }


        override public void SetBackgroundValue(double value)
        {
            throw new Exception("11111");
        }

        override public void SetSnapshotPixels(Object pixels)
        {
            if (caSnapshot && pixels == null)
                return;
            snapshotPixels = (Color[])pixels;
            snapshotWidth = width;
            snapshotHeight = height;
            caSnapshot = false;
        }

        /** Returns a reference to the snapshot pixel array. Used by the ContrastAdjuster. */
        override public Object GetSnapshotPixels()
        {
            return snapshotPixels;
        }


        override public void SetPixels(Object pixels)
        {
            this.pixels = (Color[])pixels;
            ResetPixels(pixels);
            if (pixels == null)
                snapshotPixels = null;

            image = null;
            caSnapshot = false;
        }

        override public void ApplyTable(int[] lut)
        {
            Color c;
            int r, g, b;
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    c = pixels[i];
                    r = lut[c.R];
                    g = lut[c.G];
                    b = lut[c.B];
                    pixels[i] = Color.FromArgb(255, r, g, b);
                    i++;
                }
            }
        }

        public void ApplyTable(int[] lut, int channels)
        {
            Color c;
            int r = 0, g = 0, b = 0;
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    c = pixels[i];
                    if (channels == 4)
                    {
                        r = lut[c.R];
                        g = c.G;
                        b = c.B;
                    }
                    else if (channels == 2)
                    {
                        r = c.R;
                        g = lut[c.G];
                        b = c.B;
                    }
                    else if (channels == 1)
                    {
                        r = c.R;
                        g = c.G;
                        b = lut[c.B];
                    }
                    else if ((channels & 6) == 6)
                    {
                        r = lut[c.R];
                        g = lut[c.G];
                        b = c.B;
                    }
                    else if ((channels & 5) == 5)
                    {
                        r = lut[c.R];
                        g = c.G;
                        b = c.B;
                    }
                    else if ((channels & 3) == 3)
                    {
                        r = c.R;
                        g = lut[c.G];
                        b = lut[c.B];
                    }
                    pixels[i] = Color.FromArgb(255, r, g, b);
                    i++;
                }
            }
        }

        /** Swaps the pixel and snapshot (undo) arrays. */
        override public void SwapPixelArrays()
        {
            if (snapshotPixels == null)
                return;
            Color pixel;
            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];
                pixels[i] = snapshotPixels[i];
                snapshotPixels[i] = pixel;
            }
            caSnapshot = false;
        }

        /** Returns the smallest displayed pixel value. */
        override public double GetMin()
        {
            return min;
        }


        /** Returns the largest displayed pixel value. */
        override public double GetMax()
        {
            return max;
        }

        /** Draws a pixel in the current foreground color. */
        override public void DrawPixel(int x, int y)
        {
            if (x >= clipXMin && x <= clipXMax && y >= clipYMin && y <= clipYMax)
                pixels[y * width + x] = fgColor;
        }


        override public float GetPixelValue(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                Color c = pixels[y * width + x];
                int r = c.R;
                int g = c.G;
                int b = c.B;
                if (weights != null)
                    return (float)(r * weights[0] + g * weights[1] + b * weights[2]);
                else
                    return (float)(r * rWeight + g * gWeight + b * bWeight);
            }
            else
                return float.NaN;
        }

        override public Color Get(int x, int y)
        {
            return pixels[y * width + x];
        }

        override public void Set(int x, int y, Color value)
        {
            pixels[y * width + x] = value;
        }

        /** Returns the background fill value. */
        override public Color GetBackgroundValue()
        {
            return bgColor;
        }

        /** Stores the specified value at (x,y). */
        override public void PutPixel(int x, int y, Color value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                pixels[y * width + x] = value;
            }
        }

        /** Sets the default fill/draw value, where <code>value</code> is interpreted as an RGB int. */
        override public void SetValue(Color value)
        {
            fgColor = value;
            fillValueSet = true;
        }

        /** Sets a pixel in the image using a 3 element (R, G and B)
	int array of samples. */
        override public void PutPixel(int x, int y, Color[] iArray)
        {

            pixels[y * width + x] = iArray[0];
        }

        /** Calls getPixelValue(x,y). */
        override public double GetInterpolatedPixel(double x, double y)
        {
            return 0;
        }

        override public ImageProcessor Resize(int dstWidth, int dstHeight)
        {
            ColorProcessor ip2 = new ColorProcessor(dstWidth, dstHeight, imp);
            Mat mat = image.Clone();
            CvInvoke.Resize(mat, mat, new Size(dstWidth, dstHeight), 0, 0, (Emgu.CV.CvEnum.Inter)interpolationMethod);
            ip2.CopyImageToPixels(mat);
            return ip2;
        }

        /** Returns the three weighting factors used by getPixelValue(),  返回彩色图转灰度图时各颜色的比重
	getHistogram() and convertToByte() to do color conversions.
	@see #setWeightingFactors
	@see #getRGBWeights
*/
        public static double[] GetWeightingFactors()
        {
            double[] weights = new double[3];
            weights[0] = rWeight;
            weights[1] = gWeight;
            weights[2] = bWeight;
            return weights;
        }


        /** Returns the values set by setRGBWeights(), or null if setRGBWeights() has not been called. */
        public double[] GetRGBWeights()
        {
            return weights;
        }

        /** Copies the image contained in 'ip' to (xloc, yloc) using one of
    the transfer modes defined in the Blitter interface. */
        public override void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {
            ip = ip.ConvertToRGB();
            new ColorBlitter(this).CopyBits(ip, xloc, yloc, mode);
        }

        public override void CopyImageToPixels(Point point, Size size)
        {
            if (width * height != pixels.Length)    //新建， 否则就是覆盖
            {
                pixels = null;
                pixels = new Color[width * height];
            }
            unsafe
            {

                byte* dataPtr = (byte*)image.DataPointer;
                int chnls = image.NumberOfChannels;
                int step = image.Step;
                for (int y = point.Y; y < point.Y + size.Height; y++)
                {
                    for (int x = point.X; x < point.X + size.Width; x++)
                    {
                        byte b = 0, g = 0, r = 0, a = 0xff;
                        for (int c = 0; c < chnls; c++)
                        {
                            int index = y * step + x * chnls + c;
                            switch (c)
                            {
                                case (0):
                                    b = dataPtr[index];
                                    break;
                                case (1):
                                    g = dataPtr[index];
                                    break;
                                case (2):
                                    r = dataPtr[index];
                                    break;
                                case (3):
                                    a = dataPtr[index];
                                    break;
                            }
                        }
                        pixels[y * width + x] = Color.FromArgb(a, r, g, b);
                    }
                }
            }
        }

        protected override Mat EqualizeHist(Mat mat)
        {
            Mat[] rgb = mat.Split();
            if (rgb.Length < 3) return mat;

            VectorOfMat vectorOfMat = new VectorOfMat();
            foreach (Mat c in rgb)
            {
                Mat dst = new Mat();
                CvInvoke.EqualizeHist(c, dst);
                vectorOfMat.Push(dst);
            }

            Mat result = new Mat();
            CvInvoke.Merge(vectorOfMat, result);
            return result;
        }

        protected override Mat CLAHE(Mat mat)
        {
            Mat[] rgb = mat.Split();
            if (rgb.Length < 3) return mat;

            VectorOfMat vectorOfMat = new VectorOfMat();
            foreach (Mat c in rgb)
            {
                Mat dst = new Mat();
                CvInvoke.CLAHE(c, 40, new Size(8, 8), dst);
                vectorOfMat.Push(dst);
            }

            Mat result = new Mat();
            CvInvoke.Merge(vectorOfMat, result);
            return result;
        }

        override public int[] GetHistogram()
        {
            double rw = rWeight, gw = gWeight, bw = bWeight;
            if (weights != null)
            { rw = weights[0]; gw = weights[1]; bw = weights[2]; }
            int r, g, b, v;
            Color c;
            int[] histogram = new int[256];
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    c = pixels[i++];
                    r = c.R;
                    g = c.G;
                    b = c.B;
                    v = (int)(r * rw + g * gw + b * bw + 0.5);
                    histogram[v]++;
                }
            }
            return histogram;
        }
    }
}
