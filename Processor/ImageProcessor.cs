using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing.Drawing2D;



namespace CyberedgeImageProcess2024
{
    public abstract class ImageProcessor : ICloneable
    {
        /** Value of pixels included in masks. */
        public const uint BLACK = 0xFF000000;

        /** Value returned by GetMinThreshold() when thresholding is not enabled. */
        public const double NO_THRESHOLD = -808080.0;

        /** Left justify text. */
        public const int LEFT_JUSTIFY = 0;
        /** Center justify text. */
        public const int CENTER_JUSTIFY = 1;
        /** Right justify text. */
        public const int RIGHT_JUSTIFY = 2;

        /** Isodata thresholding method */
        public const int ISODATA = 0;

        /** Modified isodata method used in Image/Adjust/Threshold tool */
        public const int ISODATA2 = 1;

        /** Composite image projection modes. */
        public const int UPDATE_RED = 1, UPDATE_GREEN = 2, UPDATE_BLUE = 3, Set_FIRST_CHANNEL = 4,
            SUM_PROJECTION = 5, MAX_PROJECTION = 6, MIN_PROJECTION = 7, INVERT_PROJECTION = 8;

        /** Interpolation methods */
        public const int NEAREST_NEIGHBOR = 0, NONE = 0, BILINEAR = 1, BICUBIC = 2;
        public const int BLUR_MORE = 0, FIND_EDGES = 1, MEDIAN_FILTER = 2, MIN = 3, MAX = 4, CONVOLVE = 5;
        public const int RED_LUT = 0, BLACK_AND_WHITE_LUT = 1, NO_LUT_UPDATE = 3, OVER_UNDER_LUT = 2;
        protected const int INVERT = 0, FILL = 1, ADD = 2, MULT = 3, AND = 4, OR = 5,
            XOR = 6, GAMMA = 7, LOG = 8, MINIMUM = 9, MAXIMUM = 10, SQR = 11, SQRT = 12, EXP = 13, ABS = 14, SET = 15;
        protected const string WRONG_LENGTH = "width*height!=pixels.Length";

        protected Mat image;
        protected Mat maskedImage;
        public EdgeImagePlus imp;
        protected Color fgColor = Color.Blue;
        protected int lineWidth = 1;
        protected int cx, cy; //current drawing coordinates
        protected Font font = Application.OpenForms[0].Font;
        protected Font fontMetrics;
        protected bool antialiasedText;
        protected bool boldFont;
        private static String[] interpolationMethods;
        // Over/Under tresholding colors
        private static int overRed, overGreen = 255, overBlue;
        private static int underRed, underGreen, underBlue = 255;
        private static bool useBicubic;
        private int sliceNumber;
        //private Overlay overlay;
        private bool noReset;

        ProgressBar progressBar;
        protected int width, snapshotWidth;
        protected int height, snapshotHeight;
        protected int roiX, roiY, roiWidth, roiHeight;
        protected int xMin, xMax, yMin, yMax;
        protected bool snapshotCopyMode;
        protected ImageProcessor mask;

        protected byte[] rLUT1, gLUT1, bLUT1; // base LUT
        protected byte[] rLUT2, gLUT2, bLUT2; // LUT as modified by SetMinAndMax and SetThreshold
        protected int interpolationMethod = NONE;
        protected double minThreshold = NO_THRESHOLD, maxThreshold = NO_THRESHOLD;
        protected int histogramSize = 256;
        protected double histogramMin, histogramMax;
        protected float[] cTable;
        protected bool lutAnimation;
        protected Color drawingColor = Color.Black;
        protected int clipXMin, clipXMax, clipYMin, clipYMax; // clip rect used by drawTo, drawLine, drawDot and drawPixel
        protected int justification = LEFT_JUSTIFY;
        protected int lutUpdateMode;
        protected Bitmap fmImage;
        protected Graphics fmGraphics;

        protected bool interpolate;  // replaced by interpolationMethod
        protected bool minMaxSet;
        protected static double seed = Double.NaN;
        protected static Random rnd;
        protected bool fillValueSet;

        protected Emgu.CV.CvEnum.FontFace fontFace = Emgu.CV.CvEnum.FontFace.HersheyComplex;      //文字标注的字体
        protected double fontScale = 1;                 //文字标注的字的放大倍数
        protected int thickness = 3;                    //文字标注中字的粗细
        protected int baseLine = 12;                    //文字的高

        public Matrix<Byte> LUT = new Matrix<Byte>(1, 256, 3);
        public bool UseLUT = false;
        protected byte[] reds = new byte[256];
        protected byte[] greens = new byte[256];
        protected byte[] blues = new byte[256];
        public abstract Mat CreateImage();
        public abstract ImageProcessor Crop();  //Returns a new processor containing an image that corresponds to the current ROI.
        private ImageProcessor dotMask;

        /** Fills pixels that are within the ROI bounding rectangle and part of
*	the mask (i.e. pixels that have a value=BLACK in the mask array).
*	Use ip.GetMask() to acquire the mask.
*	Throws and IllegalArgumentException if the mask is null or
*	the size of the mask is not the same as the size of the ROI.
*	@see #setColor(Color)
*	@see #setValue(double)
*	@see #getMask
*	@see #fill(Roi)
*/
        public abstract void Fill(ImageProcessor mask);

        /** Returns a reference to this image's pixel array. The
	array type (byte[], short[], float[] or int[]) varies
	depending on the image type. */
        public abstract Object GetPixels();

        public abstract void SetMinAndMax(double min, double max);

        /** Returns a duplicate of this image. */
        public abstract ImageProcessor Duplicate();

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public ImageProcessor Mask
        {
            get { return mask; }
            set { mask = value; }
        }



        /** Defines a rectangular region of interest and Sets the mask
	to null if this ROI is not the same size as the previous one.
	@see ImageProcessor#resetRoi
*/
        public void SetRoi(RectangleF roi)
        {
            if (roi == null)
                ResetRoi();
            else
                SetRoi((int)roi.X, (int)roi.Y, (int)roi.Width, (int)roi.Height);
        }

        /** Defines a rectangular region of interest and Sets the mask to
			null if this ROI is not the same size as the previous one.
			@see ImageProcessor#resetRoi
		*/
        public void SetRoi(int x, int y, int rwidth, int rheight)
        {
            if (x < 0 || y < 0 || x + rwidth > width || y + rheight > height)
            {
                //find intersection of roi and this image
                Rectangle r1 = new Rectangle(x, y, rwidth, rheight);
                Rectangle r2 = Rectangle.Intersect(r1, new Rectangle(0, 0, width, height));
                if (r2.Width <= 0 || r2.Height <= 0)
                {
                    roiX = 0; roiY = 0; roiWidth = 0; roiHeight = 0;
                    xMin = 0; xMax = 0; yMin = 0; yMax = 0;
                    mask = null;
                    return;
                }
                if (mask != null && mask.Width == rwidth && mask.Height == rheight)
                {
                    Rectangle r3 = new Rectangle(0, 0, r2.Width, r2.Height);
                    if (x < 0) r3.X = -x;
                    if (y < 0) r3.Y = -y;
                    mask.SetRoi(r3);
                    if (mask != null)
                        mask = mask.Crop();
                }
                roiX = r2.X; roiY = r2.Y; roiWidth = r2.Width; roiHeight = r2.Height;
            }
            else
            {
                roiX = x; roiY = y; roiWidth = rwidth; roiHeight = rheight;
            }
            if (mask != null && (mask.Width != roiWidth || mask.Height != roiHeight))
                mask = null;

            //setup limits for 3x3 filters
            xMin = Math.Max(roiX, 1);
            xMax = Math.Min(roiX + roiWidth - 1, width - 2);
            yMin = Math.Max(roiY, 1);
            yMax = Math.Min(roiY + roiHeight - 1, height - 2);
        }


        public void SetRoi(Roi roi)
        {
            if (roi == null)
                ResetRoi();
            else
            {

                    mask = roi.GetMask();
                    SetRoi(roi.Bounds);
                
            }
        }



        /// <summary>
        /// 设置整个图为Roi和clip范围
        /// </summary>
        public void ResetRoi()
        {
            roiX = 0; roiY = 0; roiWidth = width; roiHeight = height;
            xMin = 1; xMax = width - 2; yMin = 1; yMax = height - 2;
            mask = null;
            clipXMin = 0; clipXMax = width - 1; clipYMin = 0; clipYMax = height - 1;
        }

        /// <summary>
        /// 返回当前ROi范围的一个矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle GetRoi()
        {
            return new Rectangle(roiX, roiY, roiWidth, roiHeight);
        }

        /** Sets the line width used by lineTo() and drawDot(). */
        public void SetLineWidth(int width)
        {
            lineWidth = width;
            if (lineWidth < 1) lineWidth = 1;
        }

        /** Returns the current line width. */
        public int GetLineWidth()
        {
            return lineWidth;
        }


        protected void showProgress(double percentDone)
        {

        }
        

        protected bool inversionTested = false;
        protected bool invertedLut;

        



        /** Sets the background fill/draw color. */
        virtual public void SetBackgroundColor(Color color)
        {
        }

        /** Sets the default fill/draw value. */
        public void SetColor(Color value)
        {
            SetValue(value);
        }



        /** Sets the default fill/draw value. */
        public abstract void SetValue(Color value);



        /** Sets the background fill value used by the rotate() and scale() methods. */
        public abstract void SetBackgroundValue(double value);

        
        public void SetGlobalForegroundColor()
        {
            Color value = PublicConst.ForegroundColor;
            SetValue(value);
        }

        public void SetGlobalBackgroundColor()
        {
            Color value = PublicConst.BackgroundColor;
            SetValue(value);
        }


        /** Returns the smallest displayed pixel value. */
        public abstract double GetMin();

        /** Returns the largest displayed pixel value. */
        public abstract double GetMax();


        /** For short and float images, recalculates the min and max
			image values needed to correctly display the image. For
			ByteProcessors, resets the LUT. */
        virtual public void ResetMinAndMax() { }

        

        /** Returns the lower threshold level. Returns NO_THRESHOLD
			if thresholding is not enabled. */
        public double GetMinThreshold()
        {
            return minThreshold;
        }

        /** Returns the upper threshold level. */
        public double GetMaxThreshold()
        {
            return maxThreshold;
        }

        /** Returns the LUT update mode, which can be RED_LUT, BLACK_AND_WHITE_LUT,
			OVER_UNDER_LUT or NO_LUT_UPDATE. */
        public int GetLutUpdateMode()
        {
            return lutUpdateMode;
        }

        

        /** Defines a rectangular region of interest and Sets the mask
			to null if this ROI is not the same size as the previous one.
			@see ImageProcessor#resetRoi
*/
        public void SetRoi(Rectangle roi)
        {
            if (roi == null)
                ResetRoi();
            else
                SetRoi(roi.X, roi.Y, roi.Width, roi.Height);
        }




        /** Defines a byte mask that limits processing to an
			irregular ROI. Background pixels in the mask have
			a value of zero. */
        public void SetMask(ImageProcessor mask)
        {
            this.mask = mask;
        }

        /** For images with irregular ROIs, returns a mask, otherwise,
			returns null. Pixels outside the mask have a value of zero. */
        public ImageProcessor GetMask()
        {
            return mask;
        }


        public void Process(int op, double value)
        {
            double SCALE = 255.0 / Math.Log(255.0);
            int v;

            int[] lut = new int[256];
            for (int i = 0; i < 256; i++)
            {
                switch (op)
                {
                    case INVERT:
                        v = 255 - i;
                        break;
                    case FILL:
                        v = fgColor.G;
                        break;
                    case SET:
                        v = (int)value;
                        break;
                    case ADD:
                        v = i + (int)value;
                        break;
                    case MULT:
                        v = (int)Math.Round(i * value);
                        break;
                    case AND:
                        v = i & (int)value;
                        break;
                    case OR:
                        v = i | (int)value;
                        break;
                    case XOR:
                        v = i ^ (int)value;
                        break;
                    case GAMMA:
                        v = (int)(Math.Exp(Math.Log(i / 255.0) * value) * 255.0);
                        break;
                    case LOG:
                        if (i == 0)
                            v = 0;
                        else
                            v = (int)(Math.Log(i) * SCALE);
                        break;
                    case EXP:
                        v = (int)(Math.Exp(i / SCALE));
                        break;
                    case SQR:
                        v = i * i;
                        break;
                    case SQRT:
                        v = (int)Math.Sqrt(i);
                        break;
                    case MINIMUM:
                        if (i < value)
                            v = (int)value;
                        else
                            v = i;
                        break;
                    case MAXIMUM:
                        if (i > value)
                            v = (int)value;
                        else
                            v = i;
                        break;
                    default:
                        v = i;
                        break;
                }
                if (v < 0)
                    v = 0;
                if (v > 255)
                    v = 255;
                lut[i] = v;
            }
            ApplyTable(lut);
        }

        

        /** Returns the pixel values down the column starting at (x,y). */
        public void GetColumn(int x, int y, Color[] data, int Length)
        {
            for (int i = 0; i < Length; i++)
                data[i] = GetPixel(x, y++);
        }

        

        /** Inserts the pixels contained in 'data' into a
			column starting at (x,y). */
        public void PutColumn(int x, int y, Color[] data, int Length)
        {
            for (int i = 0; i < Length; i++)
                PutPixel(x, y++, data[i]);
        }

        
        public void MoveTo(int x, int y)
        {
            cx = x;
            cy = y;
        }



        /** Draws a line from the current drawing location to (x2,y2). */
        public void LineTo(int x2, int y2)
        {
            int xMin = clipXMin - lineWidth / 2 - 1;  //need not draw dots outside of this rect
            int xMax = clipXMax + lineWidth / 2 + 1;
            int yMin = clipYMin - lineWidth / 2 - 1;
            int yMax = clipYMax + lineWidth / 2 + 1;
            int dx = x2 - cx;
            int dy = y2 - cy;
            int absdx = dx >= 0 ? dx : -dx;
            int absdy = dy >= 0 ? dy : -dy;
            int n = absdy > absdx ? absdy : absdx;
            double xinc = dx != 0 ? (double)dx / n : 0; //single point (dx=dy=n=0): avoid division by zero
            double yinc = dy != 0 ? (double)dy / n : 0;
            double x = cx;
            double y = cy;
            cx = x2; cy = y2;       //keep end point as starting for the next lineTo
            int i1 = 0;
            if (dx > 0)
                i1 = Math.Max(i1, (int)((xMin - x) / xinc));
            else if (dx < 0)
                i1 = Math.Max(i1, (int)((xMax - x) / xinc));
            else if (x < xMin || x > xMax)
                return; // vertical line outside y range
            if (dy > 0)
                i1 = Math.Max(i1, (int)((yMin - y) / yinc));
            else if (dy < 0)
                i1 = Math.Max(i1, (int)((yMax - y) / yinc));
            else if (y < yMin || y > yMax)
                return; // horizontal line outside y range
            int i2 = n;
            if (dx > 0)
                i2 = Math.Min(i2, (int)((xMax - x) / xinc));
            else if (dx < 0)
                i2 = Math.Min(i2, (int)((xMin - x) / xinc));
            if (dy > 0)
                i2 = Math.Min(i2, (int)((yMax - y) / yinc));
            else if (dy < 0)
                i2 = Math.Min(i2, (int)((yMin - y) / yinc));
            x += i1 * xinc;
            y += i1 * yinc;
            for (int i = i1; i <= i2; i++)
            {
                if (lineWidth == 1)
                    DrawPixel((int)Math.Round(x), (int)Math.Round(y));
                else if (lineWidth == 2)
                    DrawDot2((int)Math.Round(x), (int)Math.Round(y));
                else
                    DrawDot((int)Math.Round(x), (int)Math.Round(y));
                x += xinc;
                y += yinc;
            }
        }

        /** Draws a line from (x1,y1) to (x2,y2). */
        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            MoveTo(x1, y1);
            LineTo(x2, y2);
        }

        

        /** Draws a rectangle. */
        public void DrawRect(int x, int y, int width, int height)
        {
            if (width < 1 || height < 1)
                return;
            if (lineWidth == 1)
            {
                MoveTo(x, y);
                LineTo(x + width - 1, y);
                LineTo(x + width - 1, y + height - 1);
                LineTo(x, y + height - 1);
                LineTo(x, y);
            }
            else
            {
                MoveTo(x, y);
                LineTo(x + width, y);
                LineTo(x + width, y + height);
                LineTo(x, y + height);
                LineTo(x, y);
            }
        }

       

        /** @deprecated */
        public void DrawDot2(int x, int y)
        {
            DrawPixel(x, y);
            DrawPixel(x - 1, y);
            DrawPixel(x, y - 1);
            DrawPixel(x - 1, y - 1);
        }

        /** Draws a dot using the current line width and fill/draw value. */
        public void DrawDot(int xcenter, int ycenter)
        {
            double r = lineWidth / 2.0;
            int xmin = (int)(xcenter - r + 0.5), ymin = (int)(ycenter - r + 0.5);
            int xmax = xmin + lineWidth, ymax = ymin + lineWidth;

            if (xmin < clipXMin || ymin < clipYMin || xmax > clipXMax || ymax > clipYMax)
            {
                // draw edge dot
                double r2 = r * r;
                r -= 0.5;
                double xoffset = xmin + r, yoffset = ymin + r;
                double xx, yy;
                for (int y = ymin; y < ymax; y++)
                {
                    for (int x = xmin; x < xmax; x++)
                    {
                        xx = x - xoffset; yy = y - yoffset;
                        if (xx * xx + yy * yy <= r2)
                            DrawPixel(x, y);
                    }
                }
            }
            else
            {

                SetRoi(xmin, ymin, lineWidth, lineWidth);
                Fill(dotMask);
                roiX = 0; roiY = 0; roiWidth = width; roiHeight = height;
                xMin = 1; xMax = width - 2; yMin = 1; yMax = height - 2;
                mask = null;
            }
        }
       

        /** Sharpens the image or ROI using a 3x3 convolution kernel. */
        public void Sharpen()
        {
            if (width > 1)
            {

                int[] kernel = {-1, -1, -1,
                        -1, 12, -1,
                        -1, -1, -1};
                Convolve3x3(kernel);
            }
        }

        /** Finds edges in the image or ROI using a Sobel operator. */
        virtual public void FindEdges()
        {

            if (width > 1)
                Filter(FIND_EDGES);

        }


        /** Flips the image or ROI horizontally. */
        public void FlipHorizontal()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            CvInvoke.Flip(mat, mat, FlipType.Horizontal);


        }

        public void FlipVertically()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            CvInvoke.Flip(mat, mat, FlipType.Vertical);
        }


        virtual public void Fill()
        {
            Process(FILL, 0.0);
        }


        public abstract Color GetPixel(int x, int y);

        /** This is a faster version of GetPixel() that does not do bounds checking. */
        public abstract Color Get(int x, int y);


        /** This is a faster version of putPixel() that does not clip
			out of range values and does not do bounds checking. */
        public abstract void Set(int x, int y, Color value);



        /** Sets a pixel in the image using an int array of samples.
			RGB pixels have three samples, all others have one. */
        virtual public void PutPixel(int x, int y, Color[] iArray)
        {
            PutPixel(x, y, iArray[0]);
        }



        /** This method is from Chapter 16 of "Digital Image Processing:
			An Algorithmic Introduction Using Java" by Burger and Burge
			(http://www.imagingbook.com/). */
        public double GetBicubicInterpolatedPixel(double x0, double y0, ImageProcessor ip2)
        {
            int u0 = (int)Math.Floor(x0);   //use floor to handle negative coordinates too
            int v0 = (int)Math.Floor(y0);
            if (u0 <= 0 || u0 >= width - 2 || v0 <= 0 || v0 >= height - 2)
                return PublicFunctions.Color2Int(ip2.GetBilinearInterpolatedPixel(x0, y0));
            double q = 0;
            for (int j = 0; j <= 3; j++)
            {
                int v = v0 - 1 + j;
                double p = 0;
                for (int i = 0; i <= 3; i++)
                {
                    int u = u0 - 1 + i;
                    p = p + PublicFunctions.Color2Int(ip2.Get(u, v)) * Cubic(x0 - u);
                }
                q = q + p * Cubic(y0 - v);
            }
            return q;
        }

        public Color GetBilinearInterpolatedPixel(double x, double y)
        {
            if (x >= -1 && x < width && y >= -1 && y < height)
            {
                int method = interpolationMethod;
                interpolationMethod = BILINEAR;
                Color value = PublicFunctions.Int2Color((int)GetInterpolatedPixel(x, y));
                interpolationMethod = method;
                return value;
            }
            else
                return GetBackgroundValue();
        }

        private const double a = 0.5; // Catmull-Rom interpolation
        public double Cubic(double x)
        {
            if (x < 0.0) x = -x;
            double z = 0.0;
            if (x < 1.0)
                z = x * x * (x * (-a + 2.0) + (a - 3.0)) + 1.0;
            else if (x < 2.0)
                z = -a * x * x * x + 5.0 * a * x * x - 8.0 * a * x + 4.0 * a;
            return z;
        }

        public abstract void PutPixel(int x, int y, Color value);

        public abstract float GetPixelValue(int x, int y);

        /** Stores the specified value at (x,y). */
        //public abstract void putPixelValue(int x, int y, double value);

        /** Sets the pixel at (x,y) to the current fill/draw value. */
        public abstract void DrawPixel(int x, int y);

        /** Sets a new pixel array for the image. The Length of the array must be equal to width*height.
			Use SetSnapshotPixels(null) to clear the snapshot buffer. */
        public abstract void SetPixels(Object pixels);

        /** Copies the image contained in 'ip' to (xloc, yloc) using one of
			the transfer modes defined in the Blitter interface. */
        public abstract void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode);

        /** Transforms the image or ROI using a lookup table. The
			Length of the table must be 256 for byte images and
			65536 for short images. RGB and float images are not
			supported. */
        public abstract void ApplyTable(int[] lut);

        /** Inverts the image or ROI. */
        public void Invert() { Process(INVERT, 0.0); }

        /** Adds 'value' to each pixel in the image or ROI. */
        public void Add(int value) { Process(ADD, value); }

        /** Adds 'value' to each pixel in the image or ROI. */
        //public void add(double value) { process(ADD, value); }

        /** Subtracts 'value' from each pixel in the image or ROI. */
        public void Subtract(int value)
        {
            Add(-value);
        }

        /** Multiplies each pixel in the image or ROI by 'value'. */
        public void Multiply(double value) { Process(MULT, value); }

        public void Divide(double value)
        {
            if (value != 0)
            {
                Process(MULT, 1 / value);

            }
        }

        /** Assigns 'value' to each pixel in the image or ROI. */
        public void SetImage(int value) { Process(SET, value); }

        /** Binary AND of each pixel in the image or ROI with 'value'. */
        public void And(int value) { Process(AND, value); }

        /** Binary OR of each pixel in the image or ROI with 'value'. */
        public void Or(int value) { Process(OR, value); }

        /** Binary exclusive OR of each pixel in the image or ROI with 'value'. */
        public void Xor(int value) { Process(XOR, value); }

        /** Performs gamma correction of the image or ROI. */
        public void Gamma(double value) { Process(GAMMA, value); }

        /** Does a natural logarithmic (base e) transform of the image or ROI. */
        public void log() { Process(LOG, 0.0); }

        /** Performs a exponential transform on the image or ROI. */
        public void exp() { Process(EXP, 0.0); }

        /** Performs a square transform on the image or ROI. */
        public void sqr() { Process(SQR, 0.0); }

        /** Performs a square root transform on the image or ROI. */
        public void sqrt() { Process(SQRT, 0.0); }

        /** If this is a 32-bit or signed 16-bit image, performs an
			absolute value transform, otherwise does nothing. */
        //public void abs() { }

        /** Pixels less than 'value' are Set to 'value'. */
        public void Min(double value) { Process(MINIMUM, value); }

        /** Pixels greater than 'value' are Set to 'value'. */
        public void Max(double value) { Process(MAXIMUM, value); }


        /** Returns a new, blank processor with the specified width and height. */
        public abstract ImageProcessor CreateProcessor(int width, int height);

        public abstract void Snapshot();

        /** Restores the pixel data from the snapshot (undo) buffer. */
        public abstract void Reset();

        /** Swaps the pixel and snapshot (undo) buffers. */
        public abstract void SwapPixelArrays();

        /** Restores pixels from the snapshot buffer that are
			within the rectangular roi but not part of the mask. */
        public abstract void Reset(ImageProcessor mask);

        /** Sets a new pixel array for the snapshot (undo) buffer. */
        public abstract void SetSnapshotPixels(Object pixels);

        /** Returns a reference to the snapshot (undo) buffer, or null. */
        public abstract Object GetSnapshotPixels();

        /** Convolves the image or ROI with the specified
			3x3 integer convolution kernel. */
        virtual public void Convolve3x3(ConvolutionKernelF kernel)
        {
            if (maskedImage != null)
            {
                CvInvoke.Filter2D(maskedImage, maskedImage, kernel, new Point(1, 1));
            }
            else
            {
                CvInvoke.Filter2D(image, image, kernel, new Point(1, 1));
            }
        }

        public abstract void Convolve3x3(int[] kernel);

        public abstract ImageProcessor Resize(int dstWidth, int dstHeight);


        public abstract int[] GetHistogram();



        /** Erodes the image or ROI using a 3x3 maximum filter. Requires 8-bit or RGB image. */
        //public abstract void erode();
        public void Erode()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            CvInvoke.Erode(mat, mat, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        }

        public void Filter2D(ConvolutionKernelF kernel, float delta)
        {
            Mat mat = new Mat();
            Rectangle roi = (imp.Roi == null ? new Rectangle(0, 0, Width, Height) : imp.Roi.GetEmguRectangleMask());

            if (imp.Roi != null)
            {
                mat = new Mat(image, roi);
            }
            else
            {
                mat = image;
            }

            CvInvoke.Filter2D(mat, mat, kernel, new Point(-1, -1), delta);
            CopyImageToPixels(roi.Location, roi.Size);
        }

        /// <summary>
        /// 将image中的内容复制到Pixels中
        /// </summary>
        public void CopyImageToPixels()
        {
            CopyImageToPixels(new Point(0, 0), new Size(width, height));

        }
        public void CopyImageToPixels(Mat image)  //同时改变大小
        {
            this.image = image.Clone();
            width = image.Cols;
            height = image.Rows;
            CopyImageToPixels();
        }

        public abstract void CopyImageToPixels(Point point, Size size);

        /** Dilates the image or ROI using a 3x3 minimum filter. Requires 8-bit or RGB image. */
        //public abstract void dilate();
        public void Dilate()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            CvInvoke.Dilate(mat, mat, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        }

        public void Open()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            CvInvoke.Erode(mat, mat, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            CvInvoke.Dilate(mat, mat, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        }

        public void Close()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            CvInvoke.Dilate(mat, mat, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            CvInvoke.Erode(mat, mat, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        }

        public void TopHat()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(mat, mat, MorphOp.Tophat, kernel, new Point(-1, -1), 1, BorderType.Reflect101, new MCvScalar());

        }

        public void BlackHat()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(mat, mat, MorphOp.Blackhat, kernel, new Point(-1, -1), 1, BorderType.Reflect101, new MCvScalar());

        }

        public void Gradient()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(mat, mat, MorphOp.Gradient, kernel, new Point(-1, -1), 1, BorderType.Reflect101, new MCvScalar());

        }

        public void EqualizeHist()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }

            EqualizeHist(mat).CopyTo(mat);
        }

        protected abstract Mat EqualizeHist(Mat mat);

        public void CLAHE()
        {
            Mat mat = new Mat();
            if (imp.Roi != null)
            {
                mat = new Mat(image, imp.Roi.GetEmguRectangleMask());
            }
            else
            {
                mat = image;
            }

            CLAHE(mat).CopyTo(mat); ;


        }

        protected abstract Mat CLAHE(Mat mat);



        protected void ResetPixels(Object pixels)
        {
            if (pixels == null)
            {
                if (this.image != null)
                {
                    //image.flush();
                    image = null;
                }
            }
        }

        /** Returns an 8-bit version of this image as a ByteProcessor. */
        public ImageProcessor ConvertToByte(bool doScaling)
        {
            return Converter.ConvertRGBToByte(this, doScaling);

        }

        /** Returns a 16-bit version of this image as a ShortProcessor. */
        public ImageProcessor ConvertToShort(bool doScaling)
        {
            return this;
        }

        /** Returns a 32-bit float version of this image as a FloatProcessor.
			For byte and short images, converts using a calibration function
			if a calibration table has been Set using SetCalibrationTable(). */
        public ImageProcessor ConvertToFloat()
        {
            TypeConverter tc = new TypeConverter(this, false);
            return tc.ConvertToFloat(cTable, imp);
        }

        /** Returns an RGB version of this image as a ColorProcessor. */
        public ImageProcessor ConvertToRGB()
        {
            return Converter.ConvertToRGB(this, true);
        }

        /** Returns an 8-bit version of this image as a ByteProcessor. 16-bit and 32-bit
		 * pixel data are scaled from min-max to 0-255.
*/
        public ByteProcessor ConvertToByteProcessor()
        {
            return ConvertToByteProcessor(true);
        }

        /** Returns an 8-bit version of this image as a ByteProcessor. 16-bit and 32-bit
		 * pixel data are scaled from min-max to 0-255 if 'scale' is true.
*/
        public ByteProcessor ConvertToByteProcessor(bool scale)
        {
            ByteProcessor bp;
            if (this is ByteProcessor)
                bp = (ByteProcessor)this.Duplicate();
            else
                bp = (ByteProcessor)this.ConvertToByte(scale);
            return bp;
        }

        /** Returns a 16-bit version of this image as a ShortProcessor. 32-bit
		 * pixel data are scaled from min-max to 0-255.
		*/
        public ShortProcessor ConvertToShortProcessor()
        {
            return ConvertToShortProcessor(true);
        }

        /** Returns a 16-bit version of this image as a ShortProcessor. 32-bit
		 * pixel data are scaled from min-max to 0-255 if 'scale' is true.
*/
        public ShortProcessor ConvertToShortProcessor(bool scale)
        {
            ShortProcessor sp;
            if (this is ShortProcessor)
                sp = (ShortProcessor)this.Duplicate();
            else
                sp = (ShortProcessor)this.ConvertToShort(scale);
            return sp;
        }

        /** Returns a 32-bit float version of this image as a FloatProcessor.
			For byte and short images, converts using a calibration function
			if a calibration table has been Set using SetCalibrationTable(). */
        public FloatProcessor ConvertToFloatProcessor()
        {
            FloatProcessor fp;
            if (this is FloatProcessor)
                fp = (FloatProcessor)this.Duplicate();
            else
                fp = (FloatProcessor)this.ConvertToFloat();
            return fp;
        }

        /** Returns an RGB version of this image as a ColorProcessor. */
        public ColorProcessor ConvertToColorProcessor()
        {
            ColorProcessor cp;
            if (this is ColorProcessor)
                cp = (ColorProcessor)this.Duplicate();
            else
                cp = (ColorProcessor)this.ConvertToRGB();
            return cp;
        }

        
        protected String maskSizeError(ImageProcessor mask)
        {
            return "Mask size (" + mask.Width + "x" + mask.Height + ") != ROI size (" +
                roiWidth + "x" + roiHeight + ")";
        }

        

        /**	The GetPixelsCopy() method returns a reference to the
			snapshot buffer if it is not null and 'snapshotCopyMode' is true.
			@see ImageProcessor#getPixelsCopy
			@see ImageProcessor#snapshot
*/
        public void SetSnapshotCopyMode(bool b)
        {
            snapshotCopyMode = b;
        }

        



        /** Use this method to Set the interpolation method (NONE,
			 BILINEAR or BICUBIC) used by scale(), resize() and rotate(). */
        public void SetInterpolationMethod(int method)
        {
            if (method < NONE || method > BICUBIC)
                throw new Exception("Invalid interpolation method");
            interpolationMethod = method;
        }

        

        /** A 3x3 filter operation, where the argument (BLUR_MORE,  FIND_EDGES,
	 MEDIAN_FILTER, MIN or MAX) determines the filter type. */
        public abstract void Filter(int type);

        

        /** Returns the background fill value. */
        public abstract Color GetBackgroundValue();

        /** Uses the current interpolation method (bilinear or bicubic)
		to find the pixel value at real coordinates (x,y). */
        public abstract double GetInterpolatedPixel(double x, double y);





        

        public void GaussianBlur(int kSize, float sigmaX, float sigmaY)
        {
            ImageFilter("GaussianBlur", kSize, sigmaX, sigmaY);
        }

        public void MedianBlur(int kSize)
        {
            ImageFilter("MedianBlur", kSize);
        }

        public void Blur(int kSize)
        {
            ImageFilter("Blur", kSize);
        }

        public void Bilateral(int kSize, float sigmaColor, float sigmaSpace)
        {
            ImageFilter("Bilateral", kSize, sigmaColor, sigmaSpace);
        }

        public void UnSharpMask(int radins, float weight)
        {
            ImageFilter("UnSharpMask", radins, weight);
        }

        public void ImageFilter(string command, int kSize, float floatV1 = 0, float floatV2 = 0)
        {
            Mat mat = new Mat();
            Rectangle roi = (imp.Roi == null ? new Rectangle(0, 0, Width, Height) : imp.Roi.GetEmguRectangleMask());

            if (imp.Roi != null)
            {
                mat = new Mat(image, roi);
            }
            else
            {
                mat = image;
            }
            switch (command.ToLower())
            {
                case "gaussianblur":
                    CvInvoke.GaussianBlur(mat, mat, new Size(kSize, kSize), floatV1, floatV2);
                    break;
                case "medianblur":
                    CvInvoke.MedianBlur(mat, mat, kSize);
                    break;
                case "blur":
                    CvInvoke.Blur(mat, mat, new Size(kSize, kSize), new Point(-1, -1));
                    break;
                case "bilateral":
                    Mat mat1 = mat.Clone();
                    CvInvoke.BilateralFilter(mat, mat1, kSize, floatV1, floatV2);
                    mat1.CopyTo(mat);
                    break;
                case "unsharpmask":
                    Mat mat2 = mat.Clone();
                    UnsharpMask(ref mat2, kSize, floatV1);
                    mat2.CopyTo(mat);
                    break;

            }


            CopyImageToPixels(roi.Location, roi.Size);
        }

        protected void UnsharpMask(ref Mat mat, float sigma, float weight)
        {
            Mat mat1 = mat.Clone();
            CvInvoke.GaussianBlur(mat, mat1, new Size(51, 51), sigma, sigma);
            mat1 = mat - mat1 * weight;
            mat = (mat + mat1);
        }

        public void UnevenLightCompensate()  //光照不均匀
        {
            double average = CvInvoke.Mean(image).V0;
            int blockSize = Width / 10;
            int rows_new = (int)Math.Ceiling((double)image.Rows / (double)blockSize);
            int cols_new = (int)Math.Ceiling((double)image.Cols / (double)blockSize);
            Emgu.CV.Matrix<Byte> blockImage = new Emgu.CV.Matrix<Byte>(rows_new, cols_new);


            for (int i = 0; i < rows_new; i++)
            {
                for (int j = 0; j < cols_new; j++)
                {
                    int rowmin = i * blockSize;
                    int rowmax = (i + 1) * blockSize;
                    if (rowmax > image.Rows) rowmax = image.Rows;
                    int colmin = j * blockSize;
                    int colmax = (j + 1) * blockSize;
                    if (colmax > image.Cols) colmax = image.Cols;
                    Rectangle rect = new Rectangle();
                    rect.X = colmin;
                    rect.Y = rowmin;
                    rect.Width = colmax - colmin;
                    rect.Height = rowmax - rowmin;
                    Mat imageROI = new Mat(image, rect);

                    double temaver = CvInvoke.Mean(imageROI).V0; ;
                    blockImage.Data[i, j] = (byte)temaver;
                }
            }
            blockImage = blockImage - average;
            CvInvoke.Resize(blockImage, blockImage, new Size(image.Width, image.Height), 0, 0, Inter.Cubic);
            if (image.NumberOfChannels == 1)
            {
                image = image - blockImage.Mat;
            }
            if (image.NumberOfChannels == 3)
            {
                Mat[] mats = image.Split();
                mats[0] = mats[0] - blockImage.Mat;
                mats[1] = mats[1] - blockImage.Mat;
                mats[2] = mats[2] - blockImage.Mat;
                VectorOfMat vectorOfMat = new VectorOfMat(mats);
                CvInvoke.Merge(vectorOfMat, image);
            }


            CopyImageToPixels();
        }

        public void Adjust()
        {
            Image<Gray, Byte> tmpImage = new Image<Gray, Byte>(image);
            int left, right, middle;
            GetAdjustmentPosition(tmpImage, out left, out middle, out right, 1);

            LevelsAdjustment(left, right, middle, tmpImage);   //对比度拉伸
        }

        private void LevelsAdjustment(int left, int right, int middle, Image<Gray, byte> grayImage)
        {
            int[] grayArray = new int[256];        //存放灰度的变换表
            System.Array.Clear(grayArray, 0, 255);

            //生成变换表
            for (int i = 0; i < middle; i++)   //左边
            {
                if (middle > left)
                {
                    grayArray[i] = (int)((i - left) * 1.0 / (middle - left) * 127);
                    grayArray[i] = Math.Min(Math.Max(grayArray[i], 0), 127);
                }
                else
                {
                    grayArray[i] = i;
                }
            }

            for (int i = middle; i <= 255; i++)    //右边
            {
                if (right > middle)
                {
                    grayArray[i] = (int)((i - middle) * 1.0 / (right - middle) * 127 + 128);
                    grayArray[i] = Math.Min(Math.Max(grayArray[i], 127), 255);
                }
                else
                {
                    grayArray[i] = i;
                }
            }

            ApplyTable(grayArray);

        }

        private void GetAdjustmentPosition(Image<Gray, byte> image,
                                            out int outLeft,
                                            out int outMiddle,
                                            out int outRight,
                                            int type = 0)    //等于1表示左侧不调整
        {

            Image<Gray, Byte> tmpImage = image;
            DenseHistogram dHist = GetHistogram(tmpImage);
            float[] histArray = new float[256];
            histArray = dHist.GetBinValues();

            //找到直方图左右有数据的位置
            int left = 0, right = 255;
            int size = tmpImage.Width * tmpImage.Height;
            int count = 0;
            for (int i = 0; i < 256; i++)
            {
                count += (int)histArray[i];
                if (count * 1.0f / size >= 0.01)
                {
                    left = i;
                    break;
                }
            }
            left = type == 0 ? left : 0;

            count = 0;
            for (int i = 255; i >= 0; i--)
            {
                count += (int)histArray[i];
                if (count * 1.0f / size >= 0.01)
                {
                    right = i;
                    break;
                }
            }




            //找到最高点，一般在右边
            outLeft = left;
            outRight = right;

            float fCount = 0;
            for (int i = (left + right) / 2; i <= right; i++)
            {
                if (histArray[i] > fCount)
                {
                    fCount = histArray[i];
                    outRight = i;
                }
            }

            //找到左边的最高点
            fCount = 0;
            for (int i = left; i <= (left + right) / 2; i++)
            {
                if (histArray[i] > fCount)
                {
                    fCount = histArray[i];
                    outLeft = i;
                }
            }

            //找到中间最低点
            fCount = 0;
            outMiddle = (outLeft + outRight) / 2;
            fCount = size;
            for (int i = outLeft; i <= outRight; i++)
            {
                if (histArray[i] < fCount)
                {
                    fCount = histArray[i];
                    outMiddle = i;
                }
            }

            if (outMiddle - outLeft < 10 || outRight - outMiddle < 10)
            {
                outMiddle = (outLeft + outRight) / 2;
            }

        }

        private DenseHistogram GetHistogram(Image<Gray, byte> gray)
        {
            DenseHistogram dHist;
            dHist = new DenseHistogram(256, new RangeF(0f, 255f));
            Image<Gray, byte>[] hists = gray.Split();
            dHist.Calculate<byte>(new Image<Gray, byte>[] { hists[0] }, false, null);
            return dHist;
        }

        

        /** Disables thresholding. */
        public void ResetThreshold()
        {
            minThreshold = NO_THRESHOLD;

            rLUT1 = null;
            rLUT2 = null;
            inversionTested = false;

        }

        public void ShowLut(string lutName)
        {
            int nColors = 0;

            reds = new byte[256];
            greens = new byte[256];
            blues = new byte[256];
            switch (lutName.ToLower())
            {
                case ("fire"):
                    nColors = Fire(ref reds, ref greens, ref blues);
                    break;
                case ("ice"):
                    nColors = Ice(ref reds, ref greens, ref blues);
                    break;
                case ("rgb332"):
                    nColors = RGB332(ref reds, ref greens, ref blues);
                    break;
                case ("redgreen"):
                    nColors = RedGreen(ref reds, ref greens, ref blues);
                    break;
                case ("red"):
                    nColors = PrimaryColor(4, ref reds, ref greens, ref blues);
                    break;
                case ("green"):
                    nColors = PrimaryColor(2, ref reds, ref greens, ref blues);
                    break;
                case ("blue"):
                    nColors = PrimaryColor(1, ref reds, ref greens, ref blues);
                    break;
                case ("cyan"):
                    nColors = PrimaryColor(3, ref reds, ref greens, ref blues);
                    break;
                case ("magenta"):
                    nColors = PrimaryColor(5, ref reds, ref greens, ref blues);
                    break;
                case ("yellow"):
                    nColors = PrimaryColor(6, ref reds, ref greens, ref blues);
                    break;
                default:
                    //nColors = OpenLut(lutName);
                    break;
            }
            if (nColors == 0) return;
            if (nColors > 0 && nColors < 256)
                Interpolate(ref reds, ref greens, ref blues, nColors);

            UseLUT = true;
        }

        protected void ApplyLUT(ref Mat resultMat)
        {
            if (!UseLUT) return;
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            if (rLUT2 != null)   //分割
            {
                rLUT2.CopyTo(reds, 0);
                gLUT2.CopyTo(greens, 0);
                bLUT2.CopyTo(blues, 0);
            }


            Mat[] RGB = resultMat.Split();

            unsafe
            {
                byte* blueDataPtr = (byte*)RGB[0].DataPointer;
                byte* greenDataPtr = (byte*)RGB[1].DataPointer;
                byte* redDataPtr = (byte*)RGB[2].DataPointer;
                int step = image.Step;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * step + x;
                        redDataPtr[index] = reds[redDataPtr[index]];
                        greenDataPtr[index] = greens[greenDataPtr[index]];
                        blueDataPtr[index] = blues[blueDataPtr[index]];
                    }
                }
            }


            VectorOfMat vChannels = new VectorOfMat();
            vChannels.Push(RGB[0]);
            vChannels.Push(RGB[1]);
            vChannels.Push(RGB[2]);
            CvInvoke.Merge(vChannels, resultMat);
        }


        private void Interpolate(ref byte[] reds, ref byte[] greens, ref byte[] blues, int nColors)
        {
            byte[] r = new byte[nColors];
            byte[] g = new byte[nColors];
            byte[] b = new byte[nColors];
            Array.Copy(reds, r, nColors);
            Array.Copy(greens, g, nColors);
            Array.Copy(blues, b, nColors);

            double scale = nColors / 256.0;
            int i1, i2;
            double fraction;
            for (int i = 0; i < 256; i++)
            {
                i1 = (int)(i * scale);
                i2 = i1 + 1;
                if (i2 == nColors) i2 = nColors - 1;
                fraction = i * scale - i1;
                reds[i] = (byte)((1.0 - fraction) * (r[i1] & 255) + fraction * (r[i2] & 255));
                greens[i] = (byte)((1.0 - fraction) * (g[i1] & 255) + fraction * (g[i2] & 255));
                blues[i] = (byte)((1.0 - fraction) * (b[i1] & 255) + fraction * (b[i2] & 255));
            }
        }


        private int Fire(ref byte[] reds, ref byte[] greens, ref byte[] blues)
        {
            int[] r = { 0, 0, 1, 25, 49, 73, 98, 122, 146, 162, 173, 184, 195, 207, 217, 229, 240, 252, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            int[] g = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 35, 57, 79, 101, 117, 133, 147, 161, 175, 190, 205, 219, 234, 248, 255, 255, 255, 255 };
            int[] b = { 0, 61, 96, 130, 165, 192, 220, 227, 210, 181, 151, 122, 93, 64, 35, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 35, 98, 160, 223, 255 };
            for (int i = 0; i < r.Length; i++)
            {
                reds[i] = (byte)r[i];
                greens[i] = (byte)g[i];
                blues[i] = (byte)b[i];
            }
            return r.Length;
        }

        private int PrimaryColor(int color, ref byte[] reds, ref byte[] greens, ref byte[] blues)
        {
            for (int i = 0; i < 256; i++)
            {
                if ((color & 4) != 0)
                    reds[i] = (byte)i;
                if ((color & 2) != 0)
                    greens[i] = (byte)i;
                if ((color & 1) != 0)
                    blues[i] = (byte)i;
            }
            return 256;
        }

        private int Ice(ref byte[] reds, ref byte[] greens, ref byte[] blues)
        {
            int[] r = { 0, 0, 0, 0, 0, 0, 19, 29, 50, 48, 79, 112, 134, 158, 186, 201, 217, 229, 242, 250, 250, 250, 250, 251, 250, 250, 250, 250, 251, 251, 243, 230 };
            int[] g = { 156, 165, 176, 184, 190, 196, 193, 184, 171, 162, 146, 125, 107, 93, 81, 87, 92, 97, 95, 93, 93, 90, 85, 69, 64, 54, 47, 35, 19, 0, 4, 0 };
            int[] b = { 140, 147, 158, 166, 170, 176, 209, 220, 234, 225, 236, 246, 250, 251, 250, 250, 245, 230, 230, 222, 202, 180, 163, 142, 123, 114, 106, 94, 84, 64, 26, 27 };
            for (int i = 0; i < r.Length; i++)
            {
                reds[i] = (byte)r[i];
                greens[i] = (byte)g[i];
                blues[i] = (byte)b[i];
            }
            return r.Length;
        }


        private int RGB332(ref byte[] reds, ref byte[] greens, ref byte[] blues)
        {
            for (int i = 0; i < 256; i++)
            {
                reds[i] = (byte)(i & 0xe0);
                greens[i] = (byte)((i << 3) & 0xe0);
                blues[i] = (byte)((i << 6) & 0xc0);
            }
            return 256;
        }

        private int RedGreen(ref byte[] reds, ref byte[] greens, ref byte[] blues)
        {
            for (int i = 0; i < 128; i++)
            {
                reds[i] = (byte)(i * 2);
                greens[i] = (byte)0;
                blues[i] = (byte)0;
            }
            for (int i = 128; i < 256; i++)
            {
                reds[i] = (byte)0;
                greens[i] = (byte)(i * 2);
                blues[i] = (byte)0;
            }
            return 256;
        }

        /** Returns a shallow copy of this ImageProcess. */
        public object Clone()
        {
            try
            {
                ImageProcessor copy = (ImageProcessor)(base.MemberwiseClone());
                return copy;
            }
            catch
            {
                return null;
            }
        }
    }
}

