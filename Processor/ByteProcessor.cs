using Emgu.CV;


namespace CyberedgeImageProcess2024
{
    public class ByteProcessor : ImageProcessor
    {
        const int ERODE = 10, DILATE = 11;
        protected byte[] pixels;
        protected byte[] snapshotPixels;
        private int bgColor = 255; //white
        private bool bgColorSet;
        private int min = 0, max = 255;
        private int binaryCount, binaryBackground;


        public ByteProcessor(Mat img)
        {
            width = img.Width;
            height = img.Height;
            image = img.Clone();
            pixels = new Byte[width * height];
            unsafe
            {
                byte* dataPtr = (byte*)img.DataPointer;
                int chnls = img.NumberOfChannels;
                int step = img.Step;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * step + x;
                        byte value = dataPtr[index];

                        pixels[y * width + x] = value;
                    }
                }
            }
            ResetRoi();
        }

        public ByteProcessor(Mat img, EdgeImagePlus imp) : this(img)
        {
            this.imp = imp;
        }
        public ByteProcessor(int width, int height, EdgeImagePlus imp = null) : this(width, height, new byte[width * height], imp)
        {
            ;
        }

        public ByteProcessor(int width, int height, byte[] pixels, EdgeImagePlus imp = null)
        {
            this.imp = imp;
            if (pixels != null && width * height != pixels.Length)
                throw new Exception(WRONG_LENGTH);
            this.width = width;
            this.height = height;
            ResetRoi();
            this.pixels = pixels;
            CreateImage();
        }


        /// <summary>
        /// CreateImage本身是用来生成一个单通道的灰度图image，同时返回一个三通道的图，可以给imp的img
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
                        Byte color = pixels[y * width + x];
                        int index = y * step + x;
                        dataPtr[index] = color;
                    }
                }
            }
            Mat resultMat = new Mat();
            CvInvoke.CvtColor(image, resultMat, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);  //显示时只显示3通道，返回时帮忙处理掉
            ApplyLUT(ref resultMat);
            return resultMat;
        }

        override public ImageProcessor Crop()
        {
            ImageProcessor ip2 = CreateProcessor(roiWidth, roiHeight);
            byte[] pixels2 = (byte[])ip2.GetPixels();
            for (int ys = roiY; ys < roiY + roiHeight; ys++)
            {
                int offset1 = (ys - roiY) * roiWidth;
                int offset2 = ys * width + roiX;
                for (int xs = 0; xs < roiWidth; xs++)
                    pixels2[offset1++] = pixels[offset2++];
            }
            return ip2;
        }

        public override ImageProcessor CreateProcessor(int width, int height)
        {
            ImageProcessor ip2;
            ip2 = new ByteProcessor(width, height, new byte[width * height], imp);
            ip2.SetInterpolationMethod(interpolationMethod);
            return ip2;
        }


        override public void SetMinAndMax(double min, double max)
        {
            if (max < min)
                return;
            this.min = (int)Math.Round(min);
            this.max = (int)Math.Round(max);
            if (rLUT1 == null)
            {
                rLUT1 = new byte[256]; gLUT1 = new byte[256]; bLUT1 = new byte[256];
                rLUT2 = new byte[256]; gLUT2 = new byte[256]; bLUT2 = new byte[256];
            }
            if (rLUT2 == null)
                return;
            int index;
            for (int i = 0; i < 256; i++)
            {
                if (i < min)
                {
                    rLUT2[i] = rLUT1[0];
                    gLUT2[i] = gLUT1[0];
                    bLUT2[i] = bLUT1[0];
                }
                else if (i > max)
                {
                    rLUT2[i] = rLUT1[255];
                    gLUT2[i] = gLUT1[255];
                    bLUT2[i] = bLUT1[255];
                }
                else
                {
                    index = i - this.min;
                    index = (int)(256.0 * index / (max - min));
                    if (index < 0)
                        index = 0;
                    if (index > 255)
                        index = 255;
                    rLUT2[i] = rLUT1[index];
                    gLUT2[i] = gLUT1[index];
                    bLUT2[i] = bLUT1[index];
                }
            }
            minThreshold = NO_THRESHOLD;
            UseLUT = true;
        }


        override public void ResetMinAndMax()
        {
            SetMinAndMax(0, 255);
        }

        /// <summary>
        /// 返回图像的一个副本
        /// </summary>
        /// <returns></returns>
        override public ImageProcessor Duplicate()
        {
            ImageProcessor ip2 = CreateProcessor(width, height);
            byte[] pixels2 = (byte[])ip2.GetPixels();
            System.Array.Copy(pixels, 0, pixels2, 0, width * height);
            return ip2;
        }


        override public Object GetPixels()
        {
            return (Object)pixels;
        }


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
            {
                mask = this.Mask;
                if (mask == null || mask.Width != roiWidth || mask.Height != roiHeight)
                    return;
            }
            byte[] mpixels = (byte[])mask.GetPixels();
            for (int y = roiY, my = 0; y < (roiY + roiHeight); y++, my++)
            {
                int i = y * width + roiX;
                int mi = my * roiWidth;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    if (mpixels[mi++] != 0)
                        pixels[i] = fgColor.G;
                    i++;
                }
            }
        }

        /// <summary>
        /// 用snapshot来重设图像
        /// </summary>
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

        override public void SetBackgroundValue(double value)
        {
            bgColor = (int)value;
            if (bgColor < 0) bgColor = 0;
            if (bgColor > 255) bgColor = 255;
            bgColorSet = true;
        }

        override public void Filter(int type)
        {
            int p1, p2, p3, p4, p5, p6, p7, p8, p9;
            byte[] pixels2 = (byte[])GetPixelsCopy();
            if (width == 1)
            {
                FilterEdge(type, pixels2, roiHeight, roiX, roiY, 0, 1);
                return;
            }
            int offset, sum1, sum2 = 0, sum = 0;
            int[] values = new int[10];
            if (type == MEDIAN_FILTER) values = new int[10];
            int rowOffset = width;
            int count;
            int binaryForeground = 255 - binaryBackground;
            for (int y = yMin; y <= yMax; y++)
            {
                offset = xMin + y * width;
                p2 = pixels2[offset - rowOffset - 1] & 0xff;
                p3 = pixels2[offset - rowOffset] & 0xff;
                p5 = pixels2[offset - 1] & 0xff;
                p6 = pixels2[offset] & 0xff;
                p8 = pixels2[offset + rowOffset - 1] & 0xff;
                p9 = pixels2[offset + rowOffset] & 0xff;

                for (int x = xMin; x <= xMax; x++)
                {
                    p1 = p2; p2 = p3;
                    p3 = pixels2[offset - rowOffset + 1] & 0xff;
                    p4 = p5; p5 = p6;
                    p6 = pixels2[offset + 1] & 0xff;
                    p7 = p8; p8 = p9;
                    p9 = pixels2[offset + rowOffset + 1] & 0xff;

                    switch (type)
                    {
                        case BLUR_MORE:
                            sum = (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + 4) / 9;
                            break;
                        case FIND_EDGES: // 3x3 Sobel filter
                            sum1 = p1 + 2 * p2 + p3 - p7 - 2 * p8 - p9;
                            sum2 = p1 + 2 * p4 + p7 - p3 - 2 * p6 - p9;
                            sum = (int)Math.Sqrt(sum1 * sum1 + sum2 * sum2);
                            if (sum > 255) sum = 255;
                            break;
                        case MEDIAN_FILTER:
                            values[1] = p1; values[2] = p2; values[3] = p3; values[4] = p4; values[5] = p5;
                            values[6] = p6; values[7] = p7; values[8] = p8; values[9] = p9;
                            sum = FindMedian(values);
                            break;
                        case MIN:
                            sum = p5;
                            if (p1 < sum) sum = p1;
                            if (p2 < sum) sum = p2;
                            if (p3 < sum) sum = p3;
                            if (p4 < sum) sum = p4;
                            if (p6 < sum) sum = p6;
                            if (p7 < sum) sum = p7;
                            if (p8 < sum) sum = p8;
                            if (p9 < sum) sum = p9;
                            break;
                        case MAX:
                            sum = p5;
                            if (p1 > sum) sum = p1;
                            if (p2 > sum) sum = p2;
                            if (p3 > sum) sum = p3;
                            if (p4 > sum) sum = p4;
                            if (p6 > sum) sum = p6;
                            if (p7 > sum) sum = p7;
                            if (p8 > sum) sum = p8;
                            if (p9 > sum) sum = p9;
                            break;
                        case ERODE:
                            if (p5 == binaryBackground)
                                sum = binaryBackground;
                            else
                            {
                                count = 0;
                                if (p1 == binaryBackground) count++;
                                if (p2 == binaryBackground) count++;
                                if (p3 == binaryBackground) count++;
                                if (p4 == binaryBackground) count++;
                                if (p6 == binaryBackground) count++;
                                if (p7 == binaryBackground) count++;
                                if (p8 == binaryBackground) count++;
                                if (p9 == binaryBackground) count++;
                                if (count >= binaryCount)
                                    sum = binaryBackground;
                                else
                                    sum = binaryForeground;
                            }
                            break;
                        case DILATE:
                            if (p5 == binaryForeground)
                                sum = binaryForeground;
                            else
                            {
                                count = 0;
                                if (p1 == binaryForeground) count++;
                                if (p2 == binaryForeground) count++;
                                if (p3 == binaryForeground) count++;
                                if (p4 == binaryForeground) count++;
                                if (p6 == binaryForeground) count++;
                                if (p7 == binaryForeground) count++;
                                if (p8 == binaryForeground) count++;
                                if (p9 == binaryForeground) count++;
                                if (count >= binaryCount)
                                    sum = binaryForeground;
                                else
                                    sum = binaryBackground;
                            }
                            break;
                    }

                    pixels[offset++] = (byte)sum;
                }
            }
            if (xMin == 1) FilterEdge(type, pixels2, roiHeight, roiX, roiY, 0, 1);
            if (yMin == 1) FilterEdge(type, pixels2, roiWidth, roiX, roiY, 1, 0);
            if (xMax == width - 2) FilterEdge(type, pixels2, roiHeight, width - 1, roiY, 0, 1);
            if (yMax == height - 2) FilterEdge(type, pixels2, roiWidth, roiX, height - 1, 1, 0);
        }

        /// <summary>
        /// 返回pixel data的副本.当snapshot不为空并snapshotCopyMode为真时，返回snapshot
        /// </summary>
        /// <returns></returns>
        public Object GetPixelsCopy()
        {
            if (snapshotPixels != null && snapshotCopyMode)
            {
                snapshotCopyMode = false;
                return snapshotPixels;
            }
            else
            {
                byte[] pixels2 = new byte[width * height];
                System.Array.Copy(pixels, 0, pixels2, 0, width * height);
                return pixels2;
            }
        }

        void FilterEdge(int type, byte[] pixels2, int n, int x, int y, int xinc, int yinc)
        {
            int p1, p2, p3, p4, p5, p6, p7, p8, p9;
            int sum = 0, sum1, sum2;
            int count;
            int binaryForeground = 255 - binaryBackground;
            int bg = binaryBackground;
            int fg = binaryForeground;

            for (int i = 0; i < n; i++)
            {
                
                {
                    p1 = GetEdgePixel(pixels2, x - 1, y - 1); p2 = GetEdgePixel(pixels2, x, y - 1); p3 = GetEdgePixel(pixels2, x + 1, y - 1);
                    p4 = GetEdgePixel(pixels2, x - 1, y); p5 = GetEdgePixel(pixels2, x, y); p6 = GetEdgePixel(pixels2, x + 1, y);
                    p7 = GetEdgePixel(pixels2, x - 1, y + 1); p8 = GetEdgePixel(pixels2, x, y + 1); p9 = GetEdgePixel(pixels2, x + 1, y + 1);
                }
                switch (type)
                {
                    case BLUR_MORE:
                        sum = (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + 4) / 9;
                        break;
                    case FIND_EDGES: // 3x3 Sobel filter
                        sum1 = p1 + 2 * p2 + p3 - p7 - 2 * p8 - p9;
                        sum2 = p1 + 2 * p4 + p7 - p3 - 2 * p6 - p9;
                        sum = (int)Math.Sqrt(sum1 * sum1 + sum2 * sum2);
                        if (sum > 255) sum = 255;
                        break;
                    case MIN:
                        sum = p5;
                        if (p1 < sum) sum = p1;
                        if (p2 < sum) sum = p2;
                        if (p3 < sum) sum = p3;
                        if (p4 < sum) sum = p4;
                        if (p6 < sum) sum = p6;
                        if (p7 < sum) sum = p7;
                        if (p8 < sum) sum = p8;
                        if (p9 < sum) sum = p9;
                        break;
                    case MAX:
                        sum = p5;
                        if (p1 > sum) sum = p1;
                        if (p2 > sum) sum = p2;
                        if (p3 > sum) sum = p3;
                        if (p4 > sum) sum = p4;
                        if (p6 > sum) sum = p6;
                        if (p7 > sum) sum = p7;
                        if (p8 > sum) sum = p8;
                        if (p9 > sum) sum = p9;
                        break;
                    case ERODE:
                        if (p5 == binaryBackground)
                            sum = binaryBackground;
                        else
                        {
                            count = 0;
                            if (p1 == binaryBackground) count++;
                            if (p2 == binaryBackground) count++;
                            if (p3 == binaryBackground) count++;
                            if (p4 == binaryBackground) count++;
                            if (p6 == binaryBackground) count++;
                            if (p7 == binaryBackground) count++;
                            if (p8 == binaryBackground) count++;
                            if (p9 == binaryBackground) count++;
                            if (count >= binaryCount)
                                sum = binaryBackground;
                            else
                                sum = binaryForeground;
                        }
                        break;
                    case DILATE:
                        if (p5 == binaryForeground)
                            sum = binaryForeground;
                        else
                        {
                            count = 0;
                            if (p1 == binaryForeground) count++;
                            if (p2 == binaryForeground) count++;
                            if (p3 == binaryForeground) count++;
                            if (p4 == binaryForeground) count++;
                            if (p6 == binaryForeground) count++;
                            if (p7 == binaryForeground) count++;
                            if (p8 == binaryForeground) count++;
                            if (p9 == binaryForeground) count++;
                            if (count >= binaryCount)
                                sum = binaryForeground;
                            else
                                sum = binaryBackground;
                        }
                        break;
                }
                pixels[x + y * width] = (byte)sum;
                x += xinc; y += yinc;
            }
        }

        private int GetEdgePixel(byte[] pixels2, int x, int y)
        {
            if (x <= 0) x = 0;
            if (x >= width) x = width - 1;
            if (y <= 0) y = 0;
            if (y >= height) y = height - 1;
            return pixels2[x + y * width] & 255;
        }

        private int FindMedian(int[] values)
        {
            //Finds the 5th largest of 9 values
            for (int i = 1; i <= 4; i++)
            {
                int max = 0;
                int mj = 1;
                for (int j = 1; j <= 9; j++)
                    if (values[j] > max)
                    {
                        max = values[j];
                        mj = j;
                    }
                values[mj] = 0;
            }
            int max2 = 0;
            for (int j = 1; j <= 9; j++)
                if (values[j] > max2)
                    max2 = values[j];
            return max2;
        }

        override public void Convolve3x3(int[] kernel)
        {
            int v1, v2, v3;    //input pixel values around the current pixel
            int v4, v5, v6;
            int v7, v8, v9;
            int scale = 0;
            int k1 = kernel[0], k2 = kernel[1], k3 = kernel[2],
            k4 = kernel[3], k5 = kernel[4], k6 = kernel[5],
            k7 = kernel[6], k8 = kernel[7], k9 = kernel[8];
            for (int i = 0; i < kernel.Length; i++)
                scale += kernel[i];
            if (scale == 0) scale = 1;
            byte[] pixels2 = (byte[])GetPixelsCopy();
            int xEnd = roiX + roiWidth;
            int yEnd = roiY + roiHeight;
            for (int y = roiY; y < yEnd; y++)
            {
                int p = roiX + y * width;            //points to current pixel
                int p6 = p - (roiX > 0 ? 1 : 0);      //will point to v6, currently lower
                int p3 = p6 - (y > 0 ? width : 0);    //will point to v3, currently lower
                int p9 = p6 + (y < height - 1 ? width : 0); // ...  to v9, currently lower
                v2 = pixels2[p3] & 0xff;
                v5 = pixels2[p6] & 0xff;
                v8 = pixels2[p9] & 0xff;
                if (roiX > 0) { p3++; p6++; p9++; }
                v3 = pixels2[p3] & 0xff;
                v6 = pixels2[p6] & 0xff;
                v9 = pixels2[p9] & 0xff;
                for (int x = roiX; x < xEnd; x++, p++)
                {
                    if (x < width - 1) { p3++; p6++; p9++; }
                    v1 = v2; v2 = v3;
                    v3 = pixels2[p3] & 0xff;
                    v4 = v5; v5 = v6;
                    v6 = pixels2[p6] & 0xff;
                    v7 = v8; v8 = v9;
                    v9 = pixels2[p9] & 0xff;
                    int sum = k1 * v1 + k2 * v2 + k3 * v3
                            + k4 * v4 + k5 * v5 + k6 * v6
                            + k7 * v7 + k8 * v8 + k9 * v9;
                    sum = (sum + scale / 2) / scale;   //add scale/2 to round
                    if (sum > 255) sum = 255;
                    if (sum < 0) sum = 0;
                    pixels[p] = (byte)sum;
                }
            }
        }

        override public void ApplyTable(int[] lut)
        {
            int lineStart, lineEnd;
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                lineStart = y * width + roiX;
                lineEnd = lineStart + roiWidth;
                for (int i = lineEnd; --i >= lineStart;)
                    pixels[i] = (byte)lut[pixels[i] & 0xff];
            }
        }

        /**Make a snapshot of the current image.*/
        override public void Snapshot()
        {
            snapshotWidth = width;
            snapshotHeight = height;
            if (snapshotPixels == null || (snapshotPixels != null && snapshotPixels.Length != pixels.Length))
                snapshotPixels = new byte[width * height];
            System.Array.Copy(pixels, 0, snapshotPixels, 0, width * height);
        }

        override public void SetSnapshotPixels(Object pixels)
        {
            snapshotPixels = (byte[])pixels;
            snapshotWidth = width;
            snapshotHeight = height;
        }

        override public Object GetSnapshotPixels()
        {
            return snapshotPixels;
        }

        override public void SetPixels(Object pixels)
        {
            if (pixels != null && this.pixels != null && (((byte[])pixels).Length != this.pixels.Length))
                throw new Exception("");
            this.pixels = (byte[])pixels;
            ResetPixels(pixels);
            if (pixels == null) snapshotPixels = null;
            image = null;
        }

        /** Swaps the pixel and snapshot (undo) arrays. */
        override public void SwapPixelArrays()
        {
            if (snapshotPixels == null) return;
            byte pixel;
            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];
                pixels[i] = snapshotPixels[i];
                snapshotPixels[i] = pixel;
            }
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
        /** Draws a pixel in the current foreground color. */
        override public void DrawPixel(int x, int y)
        {
            if (x >= clipXMin && x <= clipXMax && y >= clipYMin && y <= clipYMax)
                pixels[y * width + x] = fgColor.G;
        }

        override public float GetPixelValue(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (cTable == null)
                    return pixels[y * width + x] & 0xff;
                else
                    return cTable[pixels[y * width + x] & 0xff];
            }
            else
                return float.NaN;
        }

        override public Color Get(int x, int y)
        {
            byte c = pixels[y * width + x];
            return Color.FromArgb(c, c, c);
        }

        override public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                byte c = pixels[y * width + x];
                return Color.FromArgb(c, c, c);
            }
            else
                return Color.Black;
        }

        override public void Set(int x, int y, Color value)
        {
            pixels[y * width + x] = value.G;
        }

        /** Returns the background fill value. */
        override public Color GetBackgroundValue()
        {
            return Color.FromArgb(bgColor, bgColor, bgColor);
        }

        /** Uses bilinear interpolation to find the pixel value at real coordinates (x,y). */
        private double GetInterpolatedPixel(double x, double y, byte[] pixels)
        {
            int xbase = (int)x;
            int ybase = (int)y;
            double xFraction = x - xbase;
            double yFraction = y - ybase;
            int offset = ybase * width + xbase;
            int lowerLeft = pixels[offset] & 255;
            int lowerRight = pixels[offset + 1] & 255;
            int upperRight = pixels[offset + width + 1] & 255;
            int upperLeft = pixels[offset + width] & 255;
            double upperAverage = upperLeft + xFraction * (upperRight - upperLeft);
            double lowerAverage = lowerLeft + xFraction * (lowerRight - lowerLeft);
            return lowerAverage + yFraction * (upperAverage - lowerAverage);
        }

        /** Uses the current interpolation method (BILINEAR or BICUBIC) 
	to calculate the pixel value at real coordinates (x,y). */
        override public double GetInterpolatedPixel(double x, double y)
        {
            return 0;
        }
        

        /** Stores the specified value at (x,y). Does
	nothing if (x,y) is outside the image boundary.
	Values outside the range 0-255 are clamped. */
        override public void PutPixel(int x, int y, Color value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {

                pixels[y * width + x] = value.G;
            }
        }

        /** Sets the default fill/draw value, where 0<=value<=255. */
        override public void SetValue(Color value)
        {
            fgColor = value;

            fillValueSet = true;
        }

        override public ImageProcessor Resize(int dstWidth, int dstHeight)
        {
            ByteProcessor ip2 = new ByteProcessor(dstWidth, dstHeight, imp);
            Mat mat = image.Clone();
            CvInvoke.Resize(mat, mat, new Size(dstWidth, dstHeight), 0, 0, (Emgu.CV.CvEnum.Inter)interpolationMethod);
            ip2.CopyImageToPixels(mat);
            return ip2;
        }


        /** Copies the image contained in 'ip' to (xloc, yloc) using one of
    the transfer modes defined in the Blitter interface. */
        public override void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {
            {
                ip = ip.ConvertToByte(true);
                new ByteBlitter(this).CopyBits(ip, xloc, yloc, mode);
            }
        }

        public override void CopyImageToPixels(Point point, Size size)
        {
            if (width * height != pixels.Length)    //新建， 否则就是覆盖
            {
                pixels = null;
                pixels = new Byte[width * height];
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

        protected override Mat EqualizeHist(Mat mat)
        {
            Mat dst = new Mat();
            CvInvoke.EqualizeHist(mat, dst);
            return dst;
        }

        protected override Mat CLAHE(Mat mat)
        {
            Mat dst = new Mat();
            CvInvoke.CLAHE(mat, 40, new Size(8, 8), dst);
            return dst;
        }

        override public int[] GetHistogram()
        {

            int[] histogram = new int[256];
            for (int y = roiY; y < (roiY + roiHeight); y++)
            {
                int i = y * width + roiX;
                for (int x = roiX; x < (roiX + roiWidth); x++)
                {
                    int v = pixels[i++] & 0xff;
                    histogram[v]++;
                }
            }
            return histogram;
        }
    }
}
