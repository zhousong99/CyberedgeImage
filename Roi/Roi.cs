using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace CyberedgeImageProcess2024
{
    public class Roi : Object, ICloneable
    {
        public const int CONSTRUCTING = 0, MOVING = 1, RESIZING = 2, NORMAL = 3, MOVING_HANDLE = 4; // States
        public const int RECTANGLE = 0; // Types
        public const int HANDLE_SIZE = 5;  // replaced by getHandleSize()
        public const int NOT_PASTING = -1;
        public const int FERET_ARRAYSIZE = 16; // Size of array with Feret values
        public const int FERET_ARRAY_POINTOFFSET = 8; // Where point coordinates start in Feret array
        //private const string NAMES_KEY = "group.names";

        public const int NO_MODS = 0, ADD_TO_ROI = 1, SUBTRACT_FROM_ROI = 2; // modification states

        protected int startX, startY, x, y, width, height;
        protected double startXD, startYD;
        protected RectangleF bounds = RectangleF.Empty;
        protected int activeHandle;
        protected int state;
        public int modState = NO_MODS;
        //int cornerDiameter;             //for rounded rectangle
        protected int previousSX, previousSY;     //remember for aborting moving with esc and constrain

        protected static Color ROIColor = Color.Yellow;
        public static Pen onePixelWide = new Pen(Color.Yellow, 1.0F);

        //protected static int pasteMode = Blitter.COPY;
        protected static int lineWidth = 1;
        protected static Color defaultFillColor;
        private static Vector2 listeners = new Vector2();
        //private static LUT glasbeyLut;
        //private static int defaultGroup; // zero is no specific group
        //private static Color groupColor;
        private static double defaultStrokeWidth = 1;
        //private static String groupNamesString = Prefs.get(NAMES_KEY, null);
        //private static String[] groupNames;
       // private static bool groupNamesChanged;

        /** Get using getPreviousRoi() and set using setPreviousRoi() */
        public static Roi previousRoi;

        protected int type;
        protected int xMax, yMax;
        protected EdgeImagePlus imp;
        private int imageID;
        protected ImageMat im;
        protected int oldX, oldY, oldWidth, oldHeight;  //remembers previous clip rect
        protected int clipX, clipY, clipWidth, clipHeight;
        protected EdgeImagePlus clipboard;
        protected bool constrain;    // to be square or limit to horizontal/vertical motion
        protected bool center;
        protected bool aspect;
        protected bool updateFullWindow;
        protected double mag = 1.0;
        protected double asp_bk;        //saves aspect ratio if resizing takes roi very small
        protected ImageProcessor cachedMask;
        protected Color handleColor = Color.White;
        protected Color strokeColor;
        protected Color fillColor;
        protected Pen stroke;
        protected bool nonScalable;
        protected bool wideLine;
        protected bool ignoreClipRect;
        protected double flattenScale = 1.0;
        protected static Color defaultColor;



        private string name;
        private bool subPixel;
        private bool isCursor;
        private double xcenter = Double.NaN;
        private double ycenter;
        private bool listenersNotified;
        private bool antiAlias = true;
        private bool usingDefaultStroke;
        private static int defaultHandleSize;
        private int handleSize = -1;
        //private bool scaleStrokeWidth; 

        /// <summary>
        /// 画笔颜色
        /// </summary>
        public Color StrokeColor
        {
            get { return strokeColor; }
            set { strokeColor = value; }
        }

        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public Pen Stroke
        {
            get
            {
                if (usingDefaultStroke)
                    return null;
                else
                    return stroke;
            }
            set
            {
                stroke = value;
                if (stroke != null)
                    usingDefaultStroke = false;
            }
        }


        public int Type
        {
            get { return type; }
        }

        public int State
        {
            get { return state; }
        }

        public static Roi PreviousRoi
        {
            get { return GetPreviousRoi(); }
            set { SetPreviousRoi(value); }
        }

        public int StartX
        {
            get { return startX; }
        }

        public int StartY
        {
            get { return startY; }
        }

        virtual public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int ModState
        {
            get { return modState; }
            set { modState = value; }
        }

        public int PasteMode
        {
            get { return GetPasteMode(); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }





        /// <summary>
        /// 建立一个矩形Roi
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Roi(int x, int y, int width, int height) : this(x, y, width, height, 0)
        {
        }

        /// <summary>
        /// 用浮点类型坐标建立一个矩形Roi
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Roi(double x, double y, double width, double height) : this(x, y, width, height, 0)
        {
        }

        /** Creates a new rounded rectangular ROI. */
        public Roi(int x, int y, int width, int height, int cornerDiameter)
        {
            SetImage(null);
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            if (width > xMax) width = xMax;
            if (height > yMax) height = yMax;
            //this.cornerDiameter = cornerDiameter;
            this.x = x;
            this.y = y;
            startX = x; startY = y;
            oldX = x; oldY = y; oldWidth = 0; oldHeight = 0;
            this.width = width;
            this.height = height;
            oldWidth = width;
            oldHeight = height;
            clipX = x;
            clipY = y;
            clipWidth = width;
            clipHeight = height;
            state = NORMAL;
            type = RECTANGLE;
            if (im != null)
            {
                MessageBox.Show("Roi(int x, int y, int width, int height, int cornerDiameter)");
            }
            double defaultWidth = DefaultStrokeWidth();
            if (defaultWidth > 0)
            {
                stroke = new Pen(defaultColor, (float)defaultWidth);
                usingDefaultStroke = true;
            }
            fillColor = defaultFillColor;
            /*this.group = defaultGroup; //initialize with current group and associated color
            if (defaultGroup > 0)
            {
                this.strokeColor = groupColor;
            }*/
        }

        /** Creates a rounded rectangular ROI using double arguments. */
        public Roi(double x, double y, double width, double height, int cornerDiameter) : this((int)x, (int)y, (int)Math.Ceiling(width), (int)Math.Ceiling(height), cornerDiameter)
        {
            ;
            bounds = new RectangleF((float)x, (float)y, (float)width, (float)height);
            subPixel = true;
        }

        /// <summary>
        /// 建立一个矩形Roi
        /// </summary>
        /// <param name="r"></param>
        public Roi(Rectangle r) : this(r.X, r.Y, r.Width, r.Height)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="imp"></param>
        public Roi(int sx, int sy, EdgeImagePlus imp) : this(sx, sy, imp, 0)
        {
        }

        /// <summary>
        /// 用户在屏幕sx，sy处建立一个Roi
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="imp"></param>
        /// <param name="cornerDiameter">不为0则是圆角矩形（暂时不用）</param>
        public Roi(int sx, int sy, EdgeImagePlus imp, int cornerDiameter)
        {
            SetImage(imp);
            int ox = sx, oy = sy;
            if (im != null)
            {
                ox = im.OffScreenX2(sx);
                oy = im.OffScreenY2(sy);
            }
            SetLocation(ox, oy);
            //this.cornerDiameter = cornerDiameter;
            width = 0;
            height = 0;
            state = CONSTRUCTING;
            type = RECTANGLE;
            if (cornerDiameter > 0)
            {
                double swidth = PublicConst.RectToolOptions_DefaultStrokeWidth;
                if (swidth > 0.0)
                    SetStrokeWidth(swidth);
                Color scolor = PublicConst.RectToolOptions_DefaultStrokeColor;
                if (scolor != Color.Empty)
                {
                    strokeColor = scolor;
                }
            }
            double defaultWidth = DefaultStrokeWidth();
            if (defaultWidth > 0)
            {
                stroke = new Pen(defaultColor, (float)defaultWidth);
                usingDefaultStroke = true;
            }
            fillColor = defaultFillColor;
            //this.group = defaultGroup;
            //if (defaultGroup > 0)
            //{
            //    this.strokeColor = groupColor;
            //}
        }

        /// <summary>
        /// 设置Roi所在的EdgeImagePlus图像，设置null取消关联
        /// </summary>
        /// <param name="imp"></param>
        public void SetImage(EdgeImagePlus imp)
        {
            this.imp = imp;
            cachedMask = null;
            if (imp == null)
            {
                im = null;
                clipboard = null;
                xMax = yMax = int.MaxValue;
            }
            else
            {
                im = imp.GetImageMat();
                xMax = imp.Width;
                yMax = imp.Height;
            }
        }


        /// <summary>
        /// 返回Roi所在的EdgeImagePlus，或null
        /// </summary>
        /// <returns></returns>
        public EdgeImagePlus GetImage()
        {
            return imp;
        }

        /// <summary>
        /// 设置Roi在图像上的坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        virtual public void SetLocation(int x, int y)
        {
            this.x = x;
            this.y = y;
            startX = x; startY = y;
            oldX = x; oldY = y; oldWidth = 0; oldHeight = 0;
            if (bounds != RectangleF.Empty)
            {
                if (!IsInteger(bounds.X) || !IsInteger(bounds.Y))
                {
                    cachedMask = null;
                    width = (int)Math.Ceiling(bounds.Width);
                    height = (int)Math.Ceiling(bounds.Height);
                }
                bounds.X = x;
                bounds.Y = y;
            }
        }





        /// <summary>
        /// Returns 'true' if this ROI uses for drawing the convention for line and point ROIs, where the coordinates are with respect to the pixel center.
        ///Returns false for area rois, which have coordinates with respect to the upper left corners of the pixels
        /// </summary>
        /// <returns></returns>
        /*protected bool UseLineSubpixelConvention()
        {
            return IsLineOrPoint();
        }

        /// <summary>
        /// 返回是不是选中的线段或点类型
        /// </summary>
        /// <returns></returns>
        protected bool IsLineOrPoint()
        {
            return IsLine() || type == POINT;
        }

        /// <summary>
        /// 返回选中的是不是线段类型
        /// </summary>
        /// <returns></returns>
        public bool IsLine()
        {
            return type >= LINE && type <= FREELINE;
        }*/

        /// <summary>
        /// 判断一个数是不是整数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsInteger(double x)
        {
            return x == (int)x;
        }


        /// <summary>
        /// 设置画Roi的笔的粗细， 如果设为0，则不管缩放倍数，都是用1
        /// </summary>
        /// <param name="strokeWidth"></param>
        virtual public void SetStrokeWidth(float strokeWidth)
        {
            if (strokeWidth < 0f)
                strokeWidth = 0f;
            if (strokeWidth == 0f && usingDefaultStroke)
                return;
            if (strokeWidth > 0f)
            {
                //scaleStrokeWidth = true;
                usingDefaultStroke = false;
            }
            
            if (strokeWidth == 0f)
                this.stroke = null;
            else if (wideLine)
            {
                this.stroke = new Pen(strokeColor, strokeWidth);
                this.stroke.StartCap = LineCap.Flat;
                this.stroke.EndCap = LineCap.Flat;
                this.stroke.LineJoin = LineJoin.Bevel;
            }
            else
                this.stroke = new Pen(strokeColor, strokeWidth);
            if (strokeWidth > 1f)
            {
                fillColor = Color.Empty;
            }
            //if (notify)
            //	notifyListeners(RoiListener.MODIFIED);
        }

        public void SetStrokeWidth(double strokeWidth)
        {
            SetStrokeWidth((float)strokeWidth);

        }


        /// <summary>
        /// 返回线的宽度
        /// </summary>
        /// <returns></returns>
        public float GetStrokeWidth()
        {
            return (stroke != null && !usingDefaultStroke) ? stroke.Width : 1f;
        }



        /// <summary>
        /// 将图像上的坐标x转换为屏幕坐标
        /// </summary>
        /// <param name="ox"></param>
        /// <returns></returns>
        protected int ScreenXD(double ox)
        {
            if (im == null) return (int)ox;

            return im.ScreenXD(ox);
        }

        /// <summary>
        /// 将图像上的坐标y转换为屏幕坐标
        /// </summary>
        /// <param name="oy"></param>
        /// <returns></returns>
        protected int ScreenYD(double oy)
        {
            if (im == null) return (int)oy;

            return im.ScreenYD(oy);
        }

        protected int ScreenX(int ox) { return ScreenXD(ox); }
        protected int ScreenY(int oy) { return ScreenYD(oy); }

        /// <summary>
        /// 将屏幕坐标sx转换为图像上的坐标
        /// </summary>
        /// <param name="sx"></param>
        /// <returns></returns>
        protected int OffScreenX(int sx)
        {
            if (im == null) return sx;
            return im.OffScreenX2(sx);
        }

        /// <summary>
        /// 将屏幕坐标sy转换为图像上的坐标
        /// </summary>
        /// <param name="sy"></param>
        /// <returns></returns>
        protected int OffScreenY(int sy)
        {
            if (im == null) return sy;
            return im.OffScreenY2(sy);
        }

        /// <summary>
        /// 将屏幕坐标sx转换为图像上的坐标
        /// </summary>
        /// <param name="sx"></param>
        /// <returns></returns>
        public double OffScreenXD(int sx)
        {
            if (im == null) return sx;
            double offScreenValue = im.OffScreenXD(sx);

            return offScreenValue;
        }

        /// <summary>
        /// 将屏幕坐标sy转换为图像上的坐标
        /// </summary>
        /// <param name="sy"></param>
        /// <returns></returns>
        public double OffScreenYD(int sy)
        {
            if (im == null) return sy;
            double offScreenValue = im.OffScreenYD(sy);

            return offScreenValue;
        }

        /// <summary>
        /// 当坐标在调整柄内或接近调整柄时，返回调整柄的编号，否则返回-1
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <returns></returns>
        virtual public int IsHandle(int sx, int sy)
        {
            if (clipboard != null || im == null) return -1;
            double mag = im.GetMagnification();
            int margin = 5;
            int size = GetHandleSize() + margin;
            int halfSize = size / 2;
            double x = GetXBase();
            double y = GetYBase();
            double width = GetFloatWidth();
            double height = GetFloatHeight();
            int sx1 = ScreenXD(x) - halfSize;
            int sy1 = ScreenYD(y) - halfSize;
            int sx3 = ScreenXD(x + width) - halfSize;
            int sy3 = ScreenYD(y + height) - halfSize;
            int sx2 = sx1 + (sx3 - sx1) / 2;
            int sy2 = sy1 + (sy3 - sy1) / 2;
            if (sx >= sx1 && sx <= sx1 + size && sy >= sy1 && sy <= sy1 + size) return 0;
            if (sx >= sx2 && sx <= sx2 + size && sy >= sy1 && sy <= sy1 + size) return 1;
            if (sx >= sx3 && sx <= sx3 + size && sy >= sy1 && sy <= sy1 + size) return 2;
            if (sx >= sx3 && sx <= sx3 + size && sy >= sy2 && sy <= sy2 + size) return 3;
            if (sx >= sx3 && sx <= sx3 + size && sy >= sy3 && sy <= sy3 + size) return 4;
            if (sx >= sx2 && sx <= sx2 + size && sy >= sy3 && sy <= sy3 + size) return 5;
            if (sx >= sx1 && sx <= sx1 + size && sy >= sy3 && sy <= sy3 + size) return 6;
            if (sx >= sx1 && sx <= sx1 + size && sy >= sy2 && sy <= sy2 + size) return 7;
            return -1;
        }

        /// <summary>
        /// 返回当前调整柄大小
        /// </summary>
        /// <returns></returns>
        public int GetHandleSize()
        {
            if (handleSize >= 0)
                return handleSize;
            else
                return GetDefaultHandleSize();
        }

        /// <summary>
        /// 返回默认的调整柄的大小
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultHandleSize()
        {
            if (defaultHandleSize > 0)
                return defaultHandleSize;
            double defaultWidth = DefaultStrokeWidth();
            int size = 7;
            if (defaultWidth > 1.5) size = 9;
            if (defaultWidth >= 3) size = 11;
            if (defaultWidth >= 4) size = 13;
            if (defaultWidth >= 5) size = 15;
            if (defaultWidth >= 11) size = (int)defaultWidth;
            defaultHandleSize = size;
            return defaultHandleSize;
        }



        public void HandleMouseDrag(int sx, int sy, int flags)
        {
            if (im == null) return;
            constrain = PublicFunctions.IsKeyPressed(Keys.Shift);   //是否按下了shift键，表示强制控制Roi长宽一致，画出正方形和圆等
            center = PublicFunctions.IsKeyPressed(Keys.Control);    //按下ctrl表示起始点为中心点，拖动鼠标时边界往四周扩大
            aspect = PublicFunctions.IsKeyPressed(Keys.Alt);
            switch (state)
            {
                case CONSTRUCTING:    //在构建Roi时，拖动鼠标，区域改变
                    Grow(sx, sy);
                    break;
                case MOVING:         //移动Roi位置
                    Move(sx, sy);
                    break;
                case MOVING_HANDLE:   //移动调整柄
                    MoveHandle(sx, sy);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 用于构建Roi时，当鼠标在移动时，根据鼠标位置来调整Roi的大小
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        virtual protected void Grow(int sx, int sy)
        {
            if (clipboard != null) return;
            int xNew = im.OffScreenX2(sx);
            int yNew = im.OffScreenY2(sy);
            if (type == RECTANGLE)
            {
                if (xNew < 0) xNew = 0;
                if (yNew < 0) yNew = 0;
            }
            if (constrain)  //控制纵横比，画出正方或正圆
            {
                // constrain selection to be square
                if (!center)
                {
                    GrowConstrained(xNew, yNew);
                    return;
                }
                int dx, dy, d;
                dx = xNew - x;
                dy = yNew - y;
                if (dx < dy)
                    d = dx;
                else
                    d = dy;
                xNew = x + d;
                yNew = y + d;
            }
            if (center)   //鼠标第一次点击的初始位置在中心
            {
                width = Math.Abs(xNew - startX) * 2;
                height = Math.Abs(yNew - startY) * 2;
                x = startX - width / 2;
                y = startY - height / 2;
            }
            else
            {
                width = Math.Abs(xNew - startX);
                height = Math.Abs(yNew - startY);
                x = (xNew >= startX) ? startX : startX - width;
                y = (yNew >= startY) ? startY : startY - height;
                if (type == RECTANGLE)
                {
                    if ((x + width) > xMax) width = xMax - x;
                    if ((y + height) > yMax) height = yMax - y;
                }
            }
            UpdateClipRect();  //计算出外框大小
            imp.Draw(clipX, clipY, clipWidth, clipHeight);
            oldX = x;
            oldY = y;
            oldWidth = width;
            oldHeight = height;
            bounds = RectangleF.Empty;
        }



        /// <summary>
        /// 发现当前Roi和前一次Roi的矩形区域的合并（大区域）
        /// </summary>
        protected void UpdateClipRect()
        {
            clipX = (x <= oldX) ? x : oldX;
            clipY = (y <= oldY) ? y : oldY;
            clipWidth = ((x + width >= oldX + oldWidth) ? x + width : oldX + oldWidth) - clipX + 1;
            clipHeight = ((y + height >= oldY + oldHeight) ? y + height : oldY + oldHeight) - clipY + 1;
            int handleSize = GetHandleSize();
            double mag = im != null ? im.GetMagnification() : 1;
            int m = mag < 1.0 ? (int)(handleSize / mag) : handleSize;
            m += ClipRectMargin();   //0，不用了
            double strokeWidth = GetStrokeWidth();
            if (strokeWidth == 0.0)
                strokeWidth = DefaultStrokeWidth();
            m = (int)(m + strokeWidth * 2);
            clipX -= m; clipY -= m;
            clipWidth += m * 2; clipHeight += m * 2;
        }

        virtual protected int ClipRectMargin()
        {
            return 0;
        }

        private void GrowConstrained(int xNew, int yNew)
        {
            int dx = xNew - startX;
            int dy = yNew - startY;
            width = height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy));
            if (type == RECTANGLE)
            {
                x = (xNew >= startX) ? startX : startX - width;
                y = (yNew >= startY) ? startY : startY - height;
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if ((x + width) > xMax) width = xMax - x;
                if ((y + height) > yMax) height = yMax - y;
            }
            else
            {
                x = startX + dx / 2 - width / 2;
                y = startY + dy / 2 - height / 2;
            }
            UpdateClipRect();
            imp.Draw(clipX, clipY, clipWidth, clipHeight);
            oldX = x;
            oldY = y;
            oldWidth = width;
            oldHeight = height;
        }

        /// <summary>
        /// 画出Roi的外框，以及调整柄
        /// </summary>
        /// <param name="g"></param>
        virtual public void Draw(Mat g)
        {
            Color color = strokeColor != Color.Empty ? strokeColor : ROIColor;
            if (fillColor != Color.Empty) color = fillColor;
            MCvScalar scalar = PublicFunctions.Color2Scalar(color);
            int thickness = (int)GetStrokeWidth();
            /*if (Interpreter.isBatchMode() && imp != null && imp.getOverlay() != null && strokeColor == null && fillColor == null)
				return;*/

            mag = GetMagnification();
            int sw = (int)(width * mag);
            int sh = (int)(height * mag);
            int sx1 = ScreenX(x);
            int sy1 = ScreenY(y);
            if (SubPixelResolution() && bounds != RectangleF.Empty)
            {
                sw = (int)(bounds.Width * mag);
                sh = (int)(bounds.Height * mag);
                sx1 = ScreenXD(bounds.X);
                sy1 = ScreenYD(bounds.Y);
            }
            Rectangle rectangle = new Rectangle(sx1, sy1, sw, sh);
            int sx2 = sx1 + sw / 2;
            int sy2 = sy1 + sh / 2;
            int sx3 = sx1 + sw;
            int sy3 = sy1 + sh;


            if (fillColor != Color.Empty)
            {
                CvInvoke.Rectangle(g, rectangle, PublicFunctions.Color2Scalar(Color.Cyan), thickness);

            }
            else
            {
                CvInvoke.Rectangle(g, rectangle, scalar, thickness);
            }


            if (clipboard == null)
            {
                DrawHandle(g, sx1, sy1);
                DrawHandle(g, sx2, sy1);
                DrawHandle(g, sx3, sy1);
                DrawHandle(g, sx3, sy2);
                DrawHandle(g, sx3, sy3);
                DrawHandle(g, sx2, sy3);
                DrawHandle(g, sx1, sy3);
                DrawHandle(g, sx1, sy2);
            }
            DrawPreviousRoi(g);
            if (state != NORMAL)
                ShowStatus();
            if (updateFullWindow)
            { updateFullWindow = false; imp.Draw(); }
        }

        protected double GetMagnification()
        {
            return im != null ? im.GetMagnification() : 1.0;
        }

        protected void DrawHandle(Mat g, int x, int y)
        {
            int threshold1 = 7500;
            int threshold2 = 1500;
            double size = (this.width * this.height) * this.mag * this.mag;

            {
                if (state == CONSTRUCTING && !(type == RECTANGLE))
                    size = threshold1 + 1;
            }
            int width = 7;
            int x0 = x, y0 = y;
            if (size > threshold1)
            {
                x -= 3;
                y -= 3;
            }
            else if (size > threshold2)
            {
                x -= 2;
                y -= 2;
                width = 5;
            }
            else
            {
                x--; y--;
                width = 3;
            }
            int inc = GetHandleSize() - 7;
            width += inc;
            x -= inc / 2;
            y -= inc / 2;
            MCvScalar color = PublicFunctions.Color2Scalar(Color.Black);
            if (width < 3)
            {
                CvInvoke.Rectangle(g, new Rectangle(x0, y0, 1, 1), color, -1);
                return;
            }
            CvInvoke.Rectangle(g, new Rectangle(x++, y++, width, width), color, -1);
            color = PublicFunctions.Color2Scalar(handleColor);
            width -= 2;
            CvInvoke.Rectangle(g, new Rectangle(x, y, width, width), color, -1);
        }

        protected void DrawPreviousRoi(Mat g)
        {
            if (previousRoi != null && previousRoi != this && previousRoi.modState != NO_MODS)
            {
                //if (type != POINT && previousRoi.Type == POINT && previousRoi.modState != SUBTRACT_FROM_ROI)
                //    return;
                previousRoi.SetImage(imp);
                previousRoi.Draw(g);
            }
        }



        public double GetXBase()
        {
            if (bounds != RectangleF.Empty)
                return bounds.X;
            else
                return x;
        }

        public double GetYBase()
        {
            if (bounds != RectangleF.Empty)
                return bounds.Y;
            else
                return y;
        }

        public double GetFloatWidth()
        {
            if (bounds != RectangleF.Empty)
                return bounds.Width;
            else
                return width;
        }

        public double GetFloatHeight()
        {
            if (bounds != RectangleF.Empty)
                return bounds.Height;
            else
                return height;
        }


        private static double DefaultStrokeWidth()
        {
            double defaultWidth = defaultStrokeWidth;
            double guiScale = PublicFunctions.GetGuiScale();
            if (defaultWidth <= 1 && guiScale > 1.0)
            {
                defaultWidth = guiScale;
                if (defaultWidth < 1.5) defaultWidth = 1.5;
            }
            return defaultWidth;
        }

        virtual public void MouseMoved(MouseEventArgs e)
        {
        }

        /// <summary>
        /// 返回选中的如Roi的点数
        /// </summary>
        /// <returns></returns>
        virtual public int Size()
        {
            return GetFloatPolygon().npoints;
            //return 4; 
        }

        /// <summary>
        /// 返回选中的如Roil的边界矩形
        /// </summary>
        public RectangleF Bounds
        {
            get { return new Rectangle(x, y, width, height); }
            set { SetBounds(value); }
        }



        /// <summary>
        /// 保存Roi为PreviousRoi，这样以后可以恢复
        /// </summary>
        /// <param name="roi"></param>
        public static void SetPreviousRoi(Roi roi)
        {
            if (roi != null)
            {
                previousRoi = (Roi)roi.Clone();
                previousRoi.SetImage(null);
            }
            else
                previousRoi = null;
        }

        /// <summary>
        /// 返回SetPreviousRoi保存的Roi
        /// </summary>
        /// <returns></returns>
        public static Roi GetPreviousRoi()
        {
            return previousRoi;
        }

        /// <summary>
        /// 返回选中的如Roi的形状情况，返回为 FloatPolygon 类型
        /// </summary>
        /// <returns></returns>
        virtual public FloatPolygon GetFloatPolygon()
        {

            if (SubPixelResolution() && bounds != RectangleF.Empty)
            {
                float[] xpoints = new float[4];
                float[] ypoints = new float[4];
                xpoints[0] = (float)bounds.X;
                ypoints[0] = (float)bounds.Y;
                xpoints[1] = (float)(bounds.X + bounds.Width);
                ypoints[1] = (float)bounds.Y;
                xpoints[2] = (float)(bounds.X + bounds.Width);
                ypoints[2] = (float)(bounds.Y + bounds.Height);
                xpoints[3] = (float)bounds.X;
                ypoints[3] = (float)(bounds.Y + bounds.Height);
                return new FloatPolygon(xpoints, ypoints);
            }
            else
            {


                float[] xpoints = new float[4];
                float[] ypoints = new float[4];
                xpoints[0] = x;
                ypoints[0] = y;
                xpoints[1] = x + width;
                ypoints[1] = y;
                xpoints[2] = x + width;
                ypoints[2] = y + height;
                xpoints[3] = x;
                ypoints[3] = y + height;

                return new FloatPolygon(xpoints, ypoints, 4);
            }
        }




        /** Returns 'true' if this is an area selection. */
        public bool IsArea()
        {
            return true;
        }

        /** Returns true if this is a slection that supports sub-pixel resolution. */
        virtual public bool SubPixelResolution()
        {
            return subPixel;
        }

        virtual public void MouseDownInHandle(int handle, int sx, int sy)
        {
            state = MOVING_HANDLE;
            previousSX = sx;
            previousSY = sy;
            activeHandle = handle;
        }

        virtual public void HandleMouseDown(int sx, int sy)
        {
            if (state == NORMAL && im != null)
            {
                state = MOVING;
                previousSX = sx;
                previousSY = sy;
                startX = OffScreenX(sx);
                startY = OffScreenY(sy);
                startXD = OffScreenXD(sx);
                startYD = OffScreenYD(sy);
            }
        }

        virtual public void HandleMouseUp(int screenX, int screenY)
        {
            state = NORMAL;
            if (imp == null) return;
            imp.Draw(clipX - 5, clipY - 5, clipWidth + 10, clipHeight + 10);


            ModifyRoi();
        }

        protected void ModifyRoi()
        {
            if (previousRoi == null || previousRoi.modState == NO_MODS || imp == null)
                return;

            Roi originalRoi = (Roi)previousRoi.Clone();
            Roi previous = (Roi)previousRoi.Clone();
            previous.modState = NO_MODS;
            ShapeRoi s1 = null;
            ShapeRoi s2 = null;
            if (previousRoi is ShapeRoi)
                s1 = (ShapeRoi)previousRoi;
            else
                s1 = new ShapeRoi(previousRoi);
            if (this is ShapeRoi)
                s2 = (ShapeRoi)this;
            else
                s2 = new ShapeRoi(this);
            if (previousRoi.modState == ADD_TO_ROI)
                s1.or(s2);
            else
                s1.not(s2);
            previousRoi.modState = NO_MODS;
            Roi roi2 = s1.trySimplify();
            if (roi2 == null) return;
            if (roi2 != null)
                roi2.CopyAttributes(previousRoi);
            imp.Roi = roi2;

            SetPreviousRoi(previous);
        }


        private int GetPasteMode()
        {
            return NOT_PASTING;
        }

        /// <summary>
        /// 返回坐标点是否在Roi内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        virtual public bool Contains(int x, int y)
        {
            Rectangle r = new Rectangle(this.x, this.y, width, height);
            return r.Contains(x, y);
        }


        /// <summary>
        /// 返回用于画Roi的画笔
        /// </summary>
        /// <returns></returns>
        public Pen GetStroke()
        {
            if (usingDefaultStroke)
                return null;
            else
                return stroke;
        }


        /// <summary>
        /// 矩形Roi没有mask，直接用矩形Roi做范围
        /// </summary>
        /// <returns></returns>
        virtual public ImageProcessor GetMask()
        {
            return null;
        }

        virtual public Rectangle GetEmguRectangleMask()
        {
            Rectangle resultRectangle = new Rectangle();
            resultRectangle.X = (int)Bounds.X;
            resultRectangle.Y = (int)Bounds.Y;
            resultRectangle.Width = (int)Bounds.Width;
            resultRectangle.Height = (int)Bounds.Height;
            return resultRectangle;
        }

        virtual public Mat GetEmguMask()
        {
            Mat mask = new Mat(imp.Img.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            CvInvoke.Rectangle(mask, GetEmguRectangleMask(), PublicFunctions.Color2Scalar(Color.White), -1);
            return mask;
        }


        public void SetBounds(RectangleF b)
        {
            if (!(type == RECTANGLE || b == RectangleF.Empty))
                return;
            this.x = (int)b.X;
            this.y = (int)b.Y;
            this.width = (int)Math.Ceiling(b.Width);
            this.height = (int)Math.Ceiling(b.Height);
            Bounds = new RectangleF(b.X, b.Y, b.Width, b.Height);
            cachedMask = null;
        }

        virtual public void MoveHandle(int sx, int sy)
        {
            double asp;
            if (clipboard != null) return;
            int ox = im.OffScreenX2(sx);
            int oy = im.OffScreenY2(sy);
            if (ox < 0) ox = 0; if (oy < 0) oy = 0;
            if (ox > xMax) ox = xMax; if (oy > yMax) oy = yMax;
            int x1 = x, y1 = y, x2 = x1 + width, y2 = y + height, xc = x + width / 2, yc = y + height / 2;
            if (width > 7 && height > 7)
            {
                asp = (double)width / (double)height;
                asp_bk = asp;
            }
            else
                asp = asp_bk;

            switch (activeHandle)
            {
                case 0:
                    x = ox; y = oy;
                    break;
                case 1:
                    y = oy;
                    break;
                case 2:
                    x2 = ox; y = oy;
                    break;
                case 3:
                    x2 = ox;
                    break;
                case 4:
                    x2 = ox; y2 = oy;
                    break;
                case 5:
                    y2 = oy;
                    break;
                case 6:
                    x = ox; y2 = oy;
                    break;
                case 7:
                    x = ox;
                    break;
            }
            if (x < x2)
                width = x2 - x;
            else
            { width = 1; x = x2; }
            if (y < y2)
                height = y2 - y;
            else
            { height = 1; y = y2; }

            if (center)
            {
                switch (activeHandle)
                {
                    case 0:
                        width = (xc - x) * 2;
                        height = (yc - y) * 2;
                        break;
                    case 1:
                        height = (yc - y) * 2;
                        break;
                    case 2:
                        width = (x2 - xc) * 2;
                        x = x2 - width;
                        height = (yc - y) * 2;
                        break;
                    case 3:
                        width = (x2 - xc) * 2;
                        x = x2 - width;
                        break;
                    case 4:
                        width = (x2 - xc) * 2;
                        x = x2 - width;
                        height = (y2 - yc) * 2;
                        y = y2 - height;
                        break;
                    case 5:
                        height = (y2 - yc) * 2;
                        y = y2 - height;
                        break;
                    case 6:
                        width = (xc - x) * 2;
                        height = (y2 - yc) * 2;
                        y = y2 - height;
                        break;
                    case 7:
                        width = (xc - x) * 2;
                        break;
                }
                if (x >= x2)
                {
                    width = 1;
                    x = x2 = xc;
                }
                if (y >= y2)
                {
                    height = 1;
                    y = y2 = yc;
                }
                bounds = RectangleF.Empty;
            }

            if (constrain)
            {
                if (activeHandle == 1 || activeHandle == 5)
                    width = height;
                else
                    height = width;

                if (x >= x2)
                {
                    width = 1;
                    x = x2 = xc;
                }
                if (y >= y2)
                {
                    height = 1;
                    y = y2 = yc;
                }
                switch (activeHandle)
                {
                    case 0:
                        x = x2 - width;
                        y = y2 - height;
                        break;
                    case 1:
                        x = xc - width / 2;
                        y = y2 - height;
                        break;
                    case 2:
                        y = y2 - height;
                        break;
                    case 3:
                        y = yc - height / 2;
                        break;
                    case 5:
                        x = xc - width / 2;
                        break;
                    case 6:
                        x = x2 - width;
                        break;
                    case 7:
                        y = yc - height / 2;
                        x = x2 - width;
                        break;
                }
                if (center)
                {
                    x = xc - width / 2;
                    y = yc - height / 2;
                }
            }

            if (aspect && !constrain)
            {
                if (activeHandle == 1 || activeHandle == 5) width = (int)Math.Round((double)height * asp);
                else height = (int)Math.Round((double)width / asp);

                switch (activeHandle)
                {
                    case 0:
                        x = x2 - width;
                        y = y2 - height;
                        break;
                    case 1:
                        x = xc - width / 2;
                        y = y2 - height;
                        break;
                    case 2:
                        y = y2 - height;
                        break;
                    case 3:
                        y = yc - height / 2;
                        break;
                    case 5:
                        x = xc - width / 2;
                        break;
                    case 6:
                        x = x2 - width;
                        break;
                    case 7:
                        y = yc - height / 2;
                        x = x2 - width;
                        break;
                }
                if (center)
                {
                    x = xc - width / 2;
                    y = yc - height / 2;
                }

                // 当roi非常小时，尝试保持纵横比：
                if (width < 8)
                {
                    if (width < 1) width = 1;
                    height = (int)Math.Round((double)width / asp_bk);
                }
                if (height < 8)
                {
                    if (height < 1) height = 1;
                    width = (int)Math.Round((double)height * asp_bk);
                }
            }

            UpdateClipRect();
            imp.Draw(clipX, clipY, clipWidth, clipHeight);
            oldX = x; oldY = y;
            oldWidth = width; oldHeight = height;
            bounds = RectangleF.Empty;
            subPixel = false;
        }

        /// <summary>
        /// ROI移动
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        virtual protected void Move(int sx, int sy)
        {
            if (constrain)
            {  
                int dx2 = sx - previousSX;
                int dy2 = sy - previousSY;
                if (Math.Abs(dx2) > Math.Abs(dy2))
                    dy2 = 0;
                else
                    dx2 = 0;
                sx = previousSX + dx2;
                sy = previousSY + dy2;
            }
            int xNew = im.OffScreenX(sx);
            int yNew = im.OffScreenY(sy);
            int dx = xNew - startX;
            int dy = yNew - startY;
            if (dx == 0 && dy == 0)
                return;
            x += dx;
            y += dy;
            if (bounds != RectangleF.Empty)
                SetLocation((int)(bounds.X + dx), (int)(bounds.Y + dy));

            bool isImageRoi = false;
            if (clipboard == null && type == RECTANGLE && !isImageRoi)
            {
                if (x < 0) x = 0; if (y < 0) y = 0;
                if ((x + width) > xMax) x = xMax - width;
                if ((y + height) > yMax) y = yMax - height;
            }
            startX = xNew;
            startY = yNew;
            UpdateClipRect();
            imp.Draw(clipX, clipY, clipWidth, clipHeight);
            oldX = x;
            oldY = y;
            oldWidth = width;
            oldHeight = height;
        }

        virtual public void ShowStatus()
        {
            if (imp == null)
                return;
            String value;
            if (state != CONSTRUCTING && (type == RECTANGLE ) && width <= 25 && height <= 25)
            {
                ImageProcessor ip = imp.GetProcessor();
                double v = ip.GetPixelValue(this.x, this.y);
                value = ", value=" + v.ToString("f0");//IJ.d2s(v, digits);
            }
            else
                value = "";

            String size;
            size = ", w=" + width + ", h=" + height;
            PublicFunctions.ShowStatus(imp.GetLocationAsString(this.x, this.y) + size + value);
        }

        virtual public object Clone()
        {
            try
            {
                Roi copy = (Roi)(base.MemberwiseClone());
                copy.SetImage(null);
                if (!usingDefaultStroke)
                    copy.Stroke = this.stroke;
                if (fillColor != Color.Empty)
                {
                    copy.FillColor = fillColor;
                }
                copy.imageID = this.imageID;
                if (bounds != RectangleF.Empty)
                {
                    copy.bounds.X = this.bounds.X;
                    copy.bounds.Y = this.bounds.Y;
                    copy.bounds.Width = this.bounds.Width;
                    copy.bounds.Height = this.bounds.Height;
                }
                return copy;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 复制一个ROI的属性到本ROI
        /// </summary>
        /// <param name="roi2"></param>
        virtual public void CopyAttributes(Roi roi2)
        {
            this.StrokeColor = roi2.strokeColor;
            this.FillColor = roi2.fillColor;
            this.SetStrokeWidth(roi2.GetStrokeWidth());
            this.name = roi2.Name;
        }


        /** Returns true if this ROI is currently displayed on an image. */
        public bool IsVisible()
        {
            return im != null;
        }

        void UpdatePaste()
        {
            if (clipboard != null)
            {
                imp.GetMask();
                ImageProcessor ip = imp.GetProcessor();
                ip.Reset();
                int xoffset = 0, yoffset = 0;
                Roi croi = clipboard != null ? clipboard.Roi : null;
                if (croi != null)
                {
                    RectangleF r = croi.Bounds;
                    if (r.X < 0) xoffset = (int)-r.X;
                    if (r.Y < 0) yoffset = (int)-r.Y;
                }
                ip.CopyBits(clipboard.GetProcessor(), x + xoffset, y + yoffset, PasteMode);
                if (type != RECTANGLE)
                    ip.Reset(ip.GetMask());
                if (im != null)
                    im.SetImageUpdated();
            }
        }

        public void EndPaste()
        {
            if (clipboard != null)
            {
                UpdatePaste();
                clipboard = null;
            }
        }


        /// <summary>
        /// 画出外框线
        /// </summary>
        /// <param name="ip"></param>
        public void DrawPixels(ImageProcessor ip)
        {
            EndPaste();
            int saveWidth = ip.GetLineWidth();
            if (GetStrokeWidth() > 1f)
                ip.SetLineWidth((int)Math.Round(GetStrokeWidth()));

            if (ip.GetLineWidth() == 1)
                ip.DrawRect(x, y, width + 1, height + 1);
            else
                ip.DrawRect(x, y, width, height);

            ip.SetLineWidth(saveWidth);
            if (GetStrokeWidth() > 1)
                updateFullWindow = true;
        }


        /// <summary>
        /// 返回ROI名称，可以为null
        /// </summary>
        /// <returns></returns>
        public String GetName()
        {
            return name;
        }

        /** Sets the name of this ROI. */
        public void SetName(String name)
        {
            this.name = name;
        }

        public int GetPosition()
        {
            return 0;
        }
    }
}
