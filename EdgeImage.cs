using Emgu.CV;

namespace CyberedgeImageProcess2024
{
    /// <summary>
    /// 图像管理器，管理一个图像文件， 一般里面包含一张图片， 也可能包含多张， 如dicom。
    /// 显示的图像在变量img中
    /// </summary>
    public partial class EdgeImagePlus : ICloneable
    {
        /** 8-bit grayscale (unsigned)*/
        public const int GRAY8 = 0;

        /** 16-bit grayscale (unsigned) */
        public const int GRAY16 = 1;

        /** 32-bit floating-point grayscale */
        public const int GRAY32 = 2;

        /** 8-bit indexed color */
        public const int COLOR_256 = 3;

        /** 32-bit RGB color */
        public const int COLOR_RGB = 4;

        /** Title of image used by Flatten command */
        public const String flattenTitle = "flatten~canvas";

        protected const int OPENED = 0;
        protected const int CLOSED = 1;
        protected const int UPDATED = 2;
        protected const int SAVED = 3;

        //图像是否被修改
        public bool changes = false;
        protected Mat img;   //用于显示的图像
        protected ImageProcessor ip;//图像的处理器，不同类型的图像可能有不同的处理器，如彩色和灰度图处理器不一样
        protected ImageWindow win;//图像显示的窗口
        protected Roi roi;//图像的Roi
        protected int currentSlice; // 多组合图当前是第几张图， 如tiff，dicom
        protected bool compositeImage;   //是否为合成图像
        protected bool locked;
        protected int nChannels = 1;
        protected int nSlices = 1;
        protected int nFrames = 1;
        protected bool dimensionsSet;
        private string title;
        protected FileInfomation fileInfo;
        private int imageType = 0;
        private bool typeSet;
        private static int currentID = -1;
        private int id;

        private bool errorLoadingImage;
        private static int default16bitDisplayRange;
        private double defaultMin, defaultMax;
        private int width;
        private int height;
        private string directory;
        private string extension;

        private Object snapshotPixels;      // 临时记录像素信息，可以用于Undo等操作
        private EdgeImagePlus clipboard = null;


        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public Mat Img
        {
            get { return img; }
            set { img = value; }
        }


        public ImageWindow ImageWindow
        {
            get { return win; }
        }

        public ImageProcessor ImageProcessor
        {
            get { return ip; }
        }

        public string Title
        {
            get { return title; }
            set { SetTitle(value); }
        }

        public Emgu.CV.CvEnum.DepthType Type
        {
            get { return img == null ? Emgu.CV.CvEnum.DepthType.Default : img.Depth; }
        }

        public int NumberOfChannels
        {
            get { return img.NumberOfChannels; }
        }

        public string Directory    //图像目录
        {
            get { return directory; }
            set { directory = value; }
        }

        public string Extension   //图像扩展名
        {
            get { return extension; }
            set { extension = value; }
        }

        public ImageMat GetImageMat()
        {
            if (win == null) return null;
            return win.GetCanvas();
        }

        public Roi Roi
        {
            get { return roi; }
            set { roi = value; }
        }

        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// 构造一个EdgeImagePlus
        /// </summary>
        public EdgeImagePlus()
        {
            title = "null";
            setID();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">用于显示的标题</param>
        /// <param name="image"></param>
        public EdgeImagePlus(String title, Mat image)
        {
            this.title = title;
            if (image != null)
                SetImage(image);
            setID();
        }

        /// <summary>
        /// 用ImageProcessor建立EdgeImagePlus
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ip"></param>
        public EdgeImagePlus(String title, ImageProcessor ip)
        {
            SetProcessor(title, ip);
            setID();
        }

        public void Show()
        {
            Show("");
        }

        /// <summary>
        /// 打开一个图像窗口来显示图像
        /// </summary>
        /// <param name="statusMessage"></param>
        public void Show(String statusMessage)
        {
            if (isVisible())  return;
            
            win = null;
            img = GetImage();
            if ((this != null) && (img.Cols >= 0) && (img.Rows >= 0))
            {
                win = new ImageWindow(this);
                roi?.SetImage(this);
                
                if (imageType == GRAY16 && default16bitDisplayRange != 0)
                {
                    ResetDisplayRange();
                    UpdateAndDraw();
                }
            }
        }

        /// <summary>
        /// 用指定的image来替换图像
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(Mat image)
        {
            roi = null;
            errorLoadingImage = false;

            int newWidth = image.Cols;
            int newHeight = image.Rows;
            bool dimensionsChanged = newWidth != width || newHeight != height;
            width = newWidth;
            height = newHeight;

            int type = COLOR_RGB;
            if (image != null && image.NumberOfChannels >= 3)
            {
                ip = new ColorProcessor(image, this);
                type = COLOR_RGB;
            }
            if (ip == null && image != null)  //如果ip为空，就是上一个if不成立，则为灰度图像。其他类型的暂时没有处理
            {
                ip = new ByteProcessor(image, this);
                type = GRAY8;
            }
            SetType(type);
            this.img = ip.CreateImage();

            if (win != null)
            {
                if (dimensionsChanged)
                    UpdateImage();
                else
                    RepaintWindow();
            }
        }

        public void SetWindow(ImageWindow win)
        {
            this.win = win;
        }

        public int GetSizeInBytes()
        {
            return Width * Height * img.NumberOfChannels;
        }

        /// <summary>
        /// 设置图像窗口的标题
        /// </summary>
        /// <param name="title">原始title</param>
        public void SetTitle(String title)
        {
            if (title == null) return;

            if (win != null)
            {
                string scale = "";
                double magnification = win.GetCanvas().GetMagnification();
                if (magnification != 1.0)
                {
                    double percent = magnification * 100.0;
                    scale = " (" + percent.ToString("F1") + "%)";
                }
                win.SetTitle(title + scale);
            }
            this.title = title;
        }


        /// <summary>
        /// 调用Draw方法来画出图像，并刷新图像窗口来显示
        /// </summary>
        public void RepaintWindow()
        {
            if (win != null)
            {
                Draw();
                win.Repaint();
            }
        }

        /// <summary>
        /// 调用 UpdateAndDraw方法来更新像素信息并画出图
        /// </summary>
        public void UpdateAndRepaintWindow()
        {
            if (win != null)
            {
                UpdateAndDraw();
                win.Repaint();
            }
        }

        /// <summary>
        /// 从ImageProcessor中的像素数据中更新图像，并显示
        /// 如果没有图像窗口关联到本图像，则不做任何事
        /// </summary>
        public void UpdateAndDraw()
        {
            if (win == null)
            {
                img = null;
                return;
            }

            win?.GetCanvas().SetImageUpdated();
            Draw();
        }

        /// <summary>
        /// 画出图像，如果有Roi， 一起画出
        /// </summary>
        public void Draw()
        {
            win?.GetCanvas().Paint();
        }

        /// <summary>
        /// 画图像，并画出矩形区域中的Roi
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Draw(int x, int y, int width, int height)
        {
            if (win != null)
            {
                ImageMat im = win.GetCanvas();
                double mag = im.GetMagnification();
                x = im.ScreenX(x);
                y = im.ScreenY(y);
                width = (int)(width * mag);
                height = (int)(height * mag);
                im.Paint(x, y, width, height);
            }
        }

        /// <summary>
        /// 当ImageProcessor生成了一幅新图像后，ImageMat.paint() 调用本方法
        /// </summary>
        public void UpdateImage()
        {
            if (win == null)
            {
                img = null;
                return;
            }
            if (ip != null)
                img = ip.CreateImage();
        }


        private void setID()
        {
            id = --currentID;
        }

        /// <summary>
        /// 返回本图像是不是在一个窗口显示出来了
        /// </summary>
        /// <returns></returns>
        public bool isVisible()
        {
            if (win == null) return false;

            return win != null && win.Visible;
        }

        /// <summary>
        /// 返回本EdgeImagePlus的浅拷贝
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            try
            {
                EdgeImagePlus copy = (EdgeImagePlus)(base.MemberwiseClone());
                copy.win = null;
                return copy;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 保存本图像
        /// </summary>
        public void Save()
        {
            if (directory == null || title != fileInfo.fileName)
            {
                extension = "jpeg";
                SaveAs(title);
            }
            {
                img.Save(directory + "\\" + title);
            }
        }

        public void SaveAs(string title = "")
        {
            string path;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Bitmap|*.bmp;*.DIB|JPEG|*.jpg;*.jpeg;*.jpe|便携式网络图片|*.png|便携式图像格式|*.PBM;*.PGM;*.PPM|Sun rasters|*.SR;*.RAS|TIFF|*.tiff;*.tif|OpenEXR HDR|*.exr|JPEG 2000|*.jp2|GIF|*.gif";
                saveFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(title);
                switch (extension.ToLower())
                {
                    case ".bmp":
                    case ".dib":
                        saveFileDialog.FilterIndex = 1;
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".jpe":
                        saveFileDialog.FilterIndex = 2;
                        break;
                    case ".png":
                        saveFileDialog.FilterIndex = 3;
                        break;
                    case ".pbm":
                    case ".pgm":
                    case ".ppm":
                        saveFileDialog.FilterIndex = 4;
                        break;
                    case ".sr":
                    case ".asr":
                        saveFileDialog.FilterIndex = 5;
                        break;
                    case ".tiff":
                    case ".tif":
                        saveFileDialog.FilterIndex = 6;
                        break;
                    case ".exr":
                        saveFileDialog.FilterIndex = 7;
                        break;
                    case ".jp2":
                        saveFileDialog.FilterIndex = 8;
                        break;
                    case ".gif":
                        saveFileDialog.FilterIndex = 9;
                        break;
                    default:  //如为其他格式图等，暂时保存为png图
                        saveFileDialog.FilterIndex = 3;
                        break;

                }
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }

            }
            Title = System.IO.Path.GetFileName(path);
            directory = System.IO.Path.GetDirectoryName(path);
            extension = System.IO.Path.GetExtension(path);
            fileInfo.fileName = Title;

            Save();
            PublicFunctions.AddRecentMenuItem(path);
        }

        /// <summary>
        /// 用于显示鼠标位置和像素点信息，
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseMoved(int x, int y)
        {
            Roi roi2 = Roi;
            if ((roi2 == null || roi2.State == Roi.NORMAL))
                PublicFunctions.ShowStatus(GetLocationAsString(x, y) + GetValueAsString(x, y));
        }

        /// <summary>
        /// 建立一个新的Roi，类型由在工具栏上选择了哪类工具决定
        /// </summary>
        /// <param name="sx">Roi起始坐标</param>
        /// <param name="sy"></param>
        public void CreateNewRoi(int sx, int sy)
        {
            Roi previousRoi = roi;
            DeleteRoi();   //also saves the roi as <code>Roi.previousRoi</code> if non-null
            Roi prevRoi = Roi.PreviousRoi;
            if (prevRoi != null)
                prevRoi.SetImage(previousRoi == null ? null : this); //with 'this' it will be recalled in case of ESC
            switch (PublicConst.CurrentTool)
            {
                case PublicConst.TOOL_RECTANGLE:
                        roi = new Roi(sx, sy, this, PublicConst.ArcSize);
                    break;
                case PublicConst.TOOL_OVAL:
                    break;
                case PublicConst.TOOL_POLYGON:
                case PublicConst.TOOL_POLYLINE:
                case PublicConst.TOOL_ANGLE:
                    break;
                case PublicConst.TOOL_FREEROI:
                case PublicConst.TOOL_FREELINE:
                    break;
                case PublicConst.TOOL_LINE:
                    break;
                case PublicConst.TOOL_TEXT:
                    break;
                case PublicConst.TOOL_POINT:
                    
                    break;
            }
        }


        /// <summary>
        /// 删除Roi，重画图像。 
        /// </summary>
        public void DeleteRoi()
        {
            if (roi == null)
                return;
            SaveRoi();
            roi = null;
            if (ip != null)
            {
                ip.ResetRoi();
            }
            Draw();
        }


        public void SaveRoi()
        {
            Roi roi2 = roi;
            if (roi2 != null)
            {
                roi2.EndPaste();
                RectangleF r = roi2.Bounds;
                if ((r.Width > 0 || r.Height > 0))
                {
                    Roi.SetPreviousRoi(roi2);
                }
            }
        }


        /// <summary>
        /// 返回正在处理的图像，Mat格式
        /// </summary>
        /// <returns></returns>
        public Mat GetImage()
        {
            if (img == null && ip != null)
                img = ip.CreateImage();
            return img;
        }


        /// <summary>
        ///返回掩膜，大小和Roi一致，非0为掩膜部分。如果Roi大小是矩形，则返回null
        /// </summary>
        /// <returns></returns>
        public ImageProcessor GetMask()
        {
            if (roi == null)
            {
                ip?.ResetRoi();
                return null;
            }
            ImageProcessor mask = roi.GetMask();
            if (mask == null)
                return null;
            if (ip != null && roi != null)
            {
                ip.Mask = mask;
                ip.SetRoi(roi.Bounds);
            }
            return mask;
        }



        /// <summary>
        /// 返回当前使用的ip，设置ip的线宽为当前使用的线宽，如果使用的光学定标，也对ip实施
        /// </summary>
        /// <returns></returns>
        public ImageProcessor GetProcessor()
        {
            if (ip == null)
                return null;
            if (roi != null && roi.IsArea())
                ip.SetRoi(roi.Bounds);
            else
                ip.ResetRoi();
            
            return ip;
        }

        /// <summary>
        /// 返回图像由几个图像组成
        /// </summary>
        /// <returns></returns>
        public int getStackSize()
        {
            return 1;
        }

        /// <summary>
        /// 需要参考PlugInFilterRunner
        /// 进行各种图像处理
        /// </summary>
        /// <param name="command"></param>
        public void RunFunction(string command)
        {
            int dialogState = 1; // 0:取消， 1非预览确定， 2预览确定
            double value;
            if (ip == null) return;

            command = command.ToLower();

            if (roi != null) roi.EndPaste();
            if (roi != null && roi.IsArea())
            {
                ip.SetRoi(roi);  //设置好后面的操作的范围，如ip中的xMin,xMax
            }
            else
            {
                ip.ResetRoi();
            }

            ip.Snapshot();    //将ip中的pixels[]复制到snapshotPixels[]做备份
            snapshotPixels = ip.GetSnapshotPixels();  //为了Undo和Preview做准备？


            ImageFilter imageFilter;

            switch (command)
            {
                case "sharpen":
                    ip.SetSnapshotCopyMode(true);
                    ip.Sharpen();
                    ip.SetSnapshotCopyMode(false);
                    break;
                case "smooth":
                    ip.SetSnapshotCopyMode(true);
                    ip.Filter(ImageProcessor.BLUR_MORE);
                    ip.SetSnapshotCopyMode(false);
                    break;
                case "findedges":
                    ip.SetSnapshotCopyMode(true);
                    ip.Filter(ImageProcessor.FIND_EDGES);
                    ip.SetSnapshotCopyMode(false);
                    break;
                case "invert":
                    ip.Invert();
                    break;
                case "fliph":
                    ip.FlipHorizontal();
                    break;
                case "imagemathlog":
                    ip.log();
                    break;
                case "imagemathexp":
                    ip.exp();
                    break;
                case "imagemathsqr":
                    ip.sqr();
                    break;
                case "imagemathsqrt":
                    ip.sqrt();
                    break;
                case "imagemathadd":
                    dialogState = ImageMathFunction(this, "Add", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Add((int)(value));
                    }
                    break;
                case "imagemathsubtracts":
                    dialogState = ImageMathFunction(this, "Subtract", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Subtract((int)(value));
                    }
                    break;
                case "imagemathmultiply":
                    dialogState = ImageMathFunction(this, "Multiply", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Multiply(value);
                    }
                    break;
                case "imagemathdivide":
                    dialogState = ImageMathFunction(this, "Divide", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Divide(value);
                    }
                    break;
                case "imagemathset":
                    dialogState = ImageMathFunction(this, "Set", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Divide(value);
                    }
                    break;
                case "imagemathmin":
                    dialogState = ImageMathFunction(this, "Minimum", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Min(value);
                    }
                    break;
                case "imagemathmax":
                    dialogState = ImageMathFunction(this, "Maximum", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Max(value);
                    }
                    break;
                case "imagemathgamma":
                    dialogState = ImageMathFunction(this, "Gamma", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Gamma(value);
                    }
                    break;
                case "imagemathand":
                    dialogState = ImageMathFunction(this, "And", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.And((int)value);
                    }
                    break;
                case "imagemathor":
                    dialogState = ImageMathFunction(this, "Or", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Or((int)value);
                    }
                    break;
                case "imagemathxor":
                    dialogState = ImageMathFunction(this, "Xor", out value);
                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Xor((int)value);
                    }
                    break;
                case "resize":
                    //snapshotPixels = null;
                    dialogState = Resizer();
                    UpdateAndRepaintWindow();
                    return;


                case "unevenlightcompensate":
                    snapshotPixels = null;
                    dialogState = 1;

                    ip.UnevenLightCompensate();
                    break;
                case "adjust":
                    snapshotPixels = null;
                    dialogState = 1;
                    ip.Adjust();
                    break;
                case "gaussianblur":
                    imageFilter = new ImageFilter(this, command);
                    dialogState = imageFilter.DialogState;

                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.GaussianBlur(imageFilter.KSize, imageFilter.floatValue1, imageFilter.floatValue2);
                    }
                    break;
                case "medianblur":
                    imageFilter = new ImageFilter(this, command);
                    dialogState = imageFilter.DialogState;

                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.MedianBlur(imageFilter.KSize);
                    }
                    break;
                case "blur":
                    imageFilter = new ImageFilter(this, command);
                    dialogState = imageFilter.DialogState;

                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Blur(imageFilter.KSize);
                    }
                    break;
                case "bilateral":
                    imageFilter = new ImageFilter(this, command);
                    dialogState = imageFilter.DialogState;

                    if (dialogState == 1)  //如果没有预览，需要做操作.因为如果是预览状态，则预览已经做过了操作
                    {
                        ip.Bilateral(imageFilter.KSize, imageFilter.floatValue1, imageFilter.floatValue2);
                    }
                    break;

            }
            changes = true;

            if (dialogState > 0)
            {
                if (snapshotPixels != null)   //preview结束的时候用？
                {
                    ip.SetSnapshotPixels(snapshotPixels);  //将记录下来的旧的snapshotPixels[]复制到ip的snapshotPixels[]中
                }

                ip.Reset(ip.Mask);  //从ip的snapshotPixels[]中取出像素，对ROI区域进行修改，将roi矩形区域内的，不是roi的内容恢复
            }
            else
            {
                ip.Reset();    //恢复原始图像
            }
            UpdateAndRepaintWindow();
        }

        /// <summary>
        /// preview时临时处理图像
        /// </summary>
        /// <param name="command"></param>
        public void PreviewFunction(string command, double value, object o1 = null, double v1 = 0, double v2 = 0, double v3 = 0)
        {
            switch (command.ToLower())
            {
                case "add":
                    ip.Add((int)(value));
                    break;
                case "subtract":
                    ip.Subtract((int)(value));
                    break;
                case "multiply":
                    ip.Multiply(value);
                    break;
                case "divide":
                    ip.Divide(value);
                    break;
                case "set":
                    ip.SetImage((int)value);
                    break;
                case "minimum":
                    ip.Min((int)value);
                    break;
                case "maximum":
                    ip.Max((int)value);
                    break;
                case "gamma":
                    value = Math.Max(0, value);
                    value = Math.Min(5, value);
                    ip.Gamma(value);
                    break;
                case "and":
                    ip.And((int)value);
                    break;
                case "or":
                    ip.Or((int)value);
                    break;
                case "xor":
                    ip.Xor((int)value);
                    break;
                case "filter2d":
                    ip.Filter2D((ConvolutionKernelF)o1, (float)value);
                    break;
                case "gaussianblur":
                    ip.GaussianBlur((int)value, (float)v1, (float)v2);
                    break;
                case "medianblur":
                    ip.MedianBlur((int)value);
                    break;
                case "blur":
                    ip.Blur((int)value);
                    break;
                case "bilateral":
                    ip.Bilateral((int)value, (float)v1, (float)v2);
                    break;
                case "unsharpmask":
                    ip.UnSharpMask((int)value, (float)v1);
                    break;
            }


            if (snapshotPixels != null)   //preview结束的时候用？
            {
                ip.SetSnapshotPixels(snapshotPixels);  //将记录下来的旧的snapshotPixels[]复制到ip的snapshotPixels[]中
            }

            ip.Reset(ip.Mask);  //从ip的snapshotPixels[]中取出像素，对ROI区域进行修改，将roi矩形区域内的，不是roi的内容恢复
            UpdateAndRepaintWindow();

        }

        private int ImageMathFunction(EdgeImagePlus imp, String arg, out double value)
        {
            ImageMath imageMath = new ImageMath(imp, arg);
            if (arg.ToLower() == "and" || arg.ToLower() == "or" || arg.ToLower() == "xor")
            {
                value = imageMath.BinaryValue();
            }
            else
            {
                value = imageMath.Value();
            }
            return imageMath.DialogState;
        }

        private int Resizer()
        {
            ImageProcessor ip = ImageProcessor;
            if (roi != null && !roi.IsArea())
                ip.ResetRoi();
            ResizeDialog resizeDialog = new ResizeDialog();
            resizeDialog.SetSize(Width, Height);
            resizeDialog.NewWindowTitle = Title;
            resizeDialog.ShowDialog();
            if (resizeDialog.DialogResult != DialogResult.OK)
            {
                return 0;
            }
            bool newWindow = resizeDialog.BoolCreateNewWindow;
            int newWidth = resizeDialog.newWidth;
            int newHeight = resizeDialog.newHeight;
            double xScale = resizeDialog.XScale;
            double yScale = resizeDialog.YScale;
            string newTitle = resizeDialog.NewWindowTitle;
            int interpolationMethod = (int)resizeDialog.Interpolation;


            if ((ip.Width > 1 && ip.Height > 1) || newWindow)
                ip.SetInterpolationMethod(interpolationMethod);
            else
                ip.SetInterpolationMethod(0);
            
            Scale(newWidth, newHeight, xScale, yScale, newWindow, newTitle);


            return resizeDialog.DialogResult == DialogResult.OK ? 1 : 0;
        }



        private void Scale(int newWidth, int newHeight, double xScale, double yScale, bool newWindow, string title)
        {
            if (newWindow)
            {
                Rectangle r = ip.GetRoi();
                EdgeImagePlus imp2 = CreateImagePlus();
                imp2.SetProcessor(title, ip.Resize(newWidth, newHeight));
                

                imp2.Show();
                TrimProcessor();
                imp2.TrimProcessor();
                imp2.changes = true;
            }
            else
            {
                Size newSize = new Size(newWidth, newHeight);
                ImageProcessor imageProcessor = this.ip.Resize(newWidth, newHeight);
                SetProcessor(imageProcessor);


                this.Show();
            }
        }



        /// <summary>
        /// 能用mask参数的功能从这里走，不能的从RunFunction走.下面是没有预览功能的
        /// </summary>
        /// <param name="command"></param>
        public void RunEmguFunction(string command)
        {
            Mat emguMask = null;
            if (ip == null) return;

            command = command.ToLower();

            if (roi != null) roi.EndPaste();
            if (roi != null && roi.IsArea())
            {
                ip.SetRoi(roi);
                emguMask = roi.GetEmguMask();
            }
            else
            {
                ip.ResetRoi();
            }
            ip.Snapshot();    //将ip中的pixels[]复制到snapshotPixels[]做备份
            snapshotPixels = ip.GetSnapshotPixels();  //为了Undo和Preview做准备？

            switch (command)
            {
                case "erode":
                    ip.Erode();
                    break;
                case "dilate":
                    ip.Dilate();
                    break;
                case "open":
                    ip.Open();
                    break;
                case "close":
                    ip.Close();
                    break;
                case "fliph":
                    ip.FlipHorizontal();
                    break;
                case "flipv":
                    ip.FlipVertically();
                    break;
                case "tophat":
                    ip.TopHat();
                    break;
                case "blackhat":
                    ip.BlackHat();
                    break;
                case "gradient":
                    ip.Gradient();
                    break;
                case "equalizehist":
                    ip.EqualizeHist();
                    break;
                case "clahe":
                    ip.CLAHE();
                    break;

            }
            ip.CopyImageToPixels();  //将emgu处理的内容（Mat类型，存放在image变量中）复制到pixels[]中
                                     //UpdateAndRepaintWindow();
            changes = true;
            if (snapshotPixels != null)   //preview结束的时候用？
            {
                ip.SetSnapshotPixels(snapshotPixels);  //将记录下来的旧的snapshotPixels[]复制到ip的snapshotPixels[]中
            }

            ip.Reset(ip.Mask);  //从ip的snapshotPixels[]中取出像素，对ROI区域进行修改，将roi矩形区域内的，不是roi的内容恢复
            UpdateAndRepaintWindow();
        }

        /// <summary>
        /// 返回一个不含图像的EdgeImagePlus
        /// </summary>
        /// <returns></returns>
        public EdgeImagePlus CreateImagePlus()
        {
            EdgeImagePlus imp2 = new EdgeImagePlus();
            imp2.SetType(imageType);

            return imp2;
        }


        /// <summary>
        /// 为图像赋值一个Roi并显示。 如果图像上现在由大小为0的Roi则删除
        /// </summary>
        /// <param name="newRoi"></param>
        public void SetRoi(Roi newRoi)
        {
            SetRoi(newRoi, true);
        }

        /// <summary>
        /// 将newRoi添加到图像上。如果updateDisplay为真，则显示Roi
        /// </summary>
        /// <param name="newRoi"></param>
        /// <param name="updateDisplay"></param>
        public void SetRoi(Roi newRoi, bool updateDisplay)
        {
            if (newRoi == null)
            {
                DeleteRoi();
                return;
            }

            RectangleF bounds = newRoi.Bounds;
            if (newRoi.IsVisible())
            {

                if (newRoi == null)
                {
                    DeleteRoi();
                    return;
                }
                EdgeImagePlus imp = newRoi.GetImage();
                if (imp != null && imp.ID != ID)
                    newRoi = (Roi)newRoi.Clone();
                newRoi.SetImage(null);
            }
            if (bounds.Width == 0 && bounds.Height == 0 )
            {
                DeleteRoi();
                return;
            }
            roi = newRoi;
            if (ip != null)
            {
                ip.SetMask(null);
                if (roi.IsArea())
                    ip.SetRoi(bounds);
                else
                    ip.ResetRoi();
            }
            roi.SetImage(this);

            if (updateDisplay)
                Draw();

        }

        /// <summary>
        /// 建立一个选择区域（矩形）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetRoi(int x, int y, int width, int height)
        {
            SetRoi(new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// 建立一个选择区域（矩形）
        /// </summary>
        /// <param name="r"></param>
        public void SetRoi(Rectangle r)
        {
            SetRoi(new Roi(r.X, r.Y, r.Width, r.Height));
        }

        /// <summary>
        /// 用指定的ImageProcessor来替换原来的ImageProcessor，并刷新显示。对于stacks（多图），ImageProcessor要和其他图上的类型一致，并且大小一致。
        /// </summary>
        /// <param name="ip"></param>
        public void SetProcessor(ImageProcessor ip)
        {
            SetProcessor(null, ip);
        }

        /// <summary>
        /// 用指定的ImageProcessor来替换原来的ImageProcessor，并刷新显示。对于stacks（多图），ImageProcessor要和其他图上的类型一致，并且大小一致。
        /// title为null表示不修改title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ip"></param>
        public void SetProcessor(String title, ImageProcessor ip)
        {
            if (ip == null || ip.GetPixels() == null)
                throw new Exception("ip null or ip.getPixels() null");

            ip.imp = this;
            SetProcessor2(title, ip);
        }

        void SetProcessor2(String title, ImageProcessor ip)
        {
            if (title != null) SetTitle(title);
            if (ip == null)
                return;
            this.ip = ip;

            bool dimensionsChanged = width > 0 && height > 0 && (width != ip.Width || height != ip.Height);

            img = null;
            img = ip.CreateImage();
            if (dimensionsChanged) roi = null;
            int type;
            if (ip is ByteProcessor)
                type = GRAY8;
            else if (ip is ColorProcessor)
                type = COLOR_RGB;
            else if (ip is ShortProcessor)
                type = GRAY16;
            else
                type = GRAY32;

            if (width == 0)
                imageType = type;
            else
                SetType(type);
            width = ip.Width;
            height = ip.Height;
            if (win != null)
            {
                if (dimensionsChanged)
                    win.UpdateImage(this);
                else
                    RepaintWindow();
                Draw();
            }

        }




        /// <summary>
        /// 返回图像每像素占用的位数
        /// </summary>
        /// <returns></returns>
        public int GetBitDepth()
        {
            ImageProcessor ip2 = ip;
            if (ip2 == null)
            {
                int bitDepth = 0;
                switch (imageType)
                {
                    case GRAY8: bitDepth = typeSet ? 8 : 0; break;
                    case COLOR_256: bitDepth = 8; break;
                    case GRAY16: bitDepth = 16; break;
                    case GRAY32: bitDepth = 32; break;
                    case COLOR_RGB: bitDepth = 24; break;
                }
                return bitDepth;
            }
            if (ip2 is ByteProcessor)
                return 8;

            else if (ip2 is ShortProcessor)
                return 16;

            else if (ip2 is ColorProcessor)
                return 24;

            else if (ip2 is FloatProcessor)
                return 32;
            return 0;
        }

        private int[] pvalue = new int[4];

        /// <summary>
        /// 返回坐标处的颜色信息，对于灰度图，pvalue[3]中是灰度值。
        /// 对于彩色图，pvalue的[0],[1],[2]对应r, g, b
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int[] GetPixel(int x, int y)
        {
            pvalue[0] = pvalue[1] = pvalue[2] = pvalue[3] = 0;
            switch (imageType)
            {
                case GRAY8:
                    Color index = Color.Black;
                    if (ip != null)
                        index = ip.GetPixel(x, y);
                    
                    pvalue[3] = index.G;
                    break;
                case COLOR_RGB:
                    if (ip != null && img.NumberOfChannels == 1)
                    {
                        pvalue[0] = ip.GetPixel(x, y).G;
                        return pvalue;
                    }
                    Color c = Color.Black;
                    if (imageType == COLOR_RGB && ip != null)
                        c = ip.GetPixel(x, y);
                    
                    int r = c.R;
                    int g = c.G;
                    int b = c.B;
                    pvalue[0] = r;
                    pvalue[1] = g;
                    pvalue[2] = b;
                    break;
                case GRAY16:
                case GRAY32:
                    Color pix = ip.GetPixel(x, y);
                    if (ip != null) pvalue[0] = PublicFunctions.Color2Int(pix);
                    break;
            }
            return pvalue;
        }


        protected void SetType(int type)
        {
            if ((type < 0) || (type > COLOR_RGB))
                return;

            imageType = type;
            typeSet = true;
        }


        /// <summary>
        /// 保存FileInfomation，以后可以getOriginalFileInfo()取出
        /// </summary>
        /// <param name="fi"></param>
        public void SetFileInfo(FileInfomation fi)
        {
            fileInfo = fi;
        }

        /// <summary>
        /// 释放部分空间
        /// </summary>
        public void TrimProcessor()
        {
            ImageProcessor ip2 = ip;
            if (!locked && ip2 != null)
            {
                Roi roi2 = Roi;
                if (roi2 != null)
                    roi2.EndPaste();
                ip2.SetSnapshotPixels(null);
            }
        }

        /// <summary>
        /// 执行伪彩色
        /// </summary>
        /// <param name="lutName">lut方案名称</param>
        public void RunLUT(string lutName)
        {
            if (ImageProcessor is ColorProcessor)
            {
                MessageBox.Show("请先将图像转换为灰色图像");
                return;
            }

            ImageProcessor.ShowLut(lutName);
            UpdateAndDraw();
        }

        /// <summary>
        /// 复制当前选中的范围到剪切板
        /// </summary>
        /// <param name="cut">是否剪切，暂不考虑</param>
        public void Copy(Boolean cut)
        {
            Roi roi = Roi;
            if (cut && roi == null)
            {
                MessageBox.Show("这个命令需要先选中一个区域");
                return;
            }

            ImageProcessor ip = GetProcessor();
            ImageProcessor ip2;
            ip2 = ip.Crop();
            clipboard = new EdgeImagePlus("Clipboard", ip2);
            if (roi != null)
                clipboard.SetRoi((Roi)roi.Clone());
            Clipboard.SetImage(ip2.CreateImage().ToBitmap());
        }


 
        /// <summary>
        /// 当一个图像文件含有多张图像时返回当前处理的图像，目前不处理多张图像的问题
        /// </summary>
        /// <returns></returns>
        public int GetCurrentSlice()
        {
            return 1;
        }

       /// <summary>
       /// 关闭图像窗口
       /// </summary>
        public void Close()
        {
                win?.Close();
        }


        public void SetDisplayRange(double min, double max)
        {
            if (ip != null)
                ip.SetMinAndMax(min, max);
        }
        

        public void ResetDisplayRange()
        {
            if (defaultMin != 0.0 || defaultMax != 0.0)
                SetDisplayRange(defaultMin, defaultMax);
            else if (imageType == GRAY16 && default16bitDisplayRange >= 8 && default16bitDisplayRange <= 16)
                ip.SetMinAndMax(0, Math.Pow(2, default16bitDisplayRange) - 1);
            else
                ip.ResetMinAndMax();
        }




        /// <summary>
        /// 返回一个坐标信息的字符串
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string GetLocationAsString(int x, int y)
        {

            string xx = "", yy = "";

            String s = " x=" + x.ToString("f0") + xx + ", y=" + y.ToString("f0") + yy;

            return s;
        }

        /// <summary>
        /// 返回坐标处的颜色信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private String GetValueAsString(int x, int y)
        {

            int[] v = GetPixel(x, y);
            int type = imageType;
            switch (type)
            {
                case GRAY8:
                    return (", 颜色=" + v[3]);
                case GRAY16:
                case COLOR_256:

                    double cValue = v[0]; 
                    if (cValue == v[0])
                        return (", 颜色=" + v[0]);
                    else
                        return (", 颜色=" + (cValue).ToString("f2") + " (" + v[0] + ")");
                case GRAY32:
                    double value = FloatProcessor.IntBitsTofloat(v[0]);
                    String s = (int)value == value ? value.ToString("f0") + ".0" : value.ToString("f4");
                    return (", value=" + s);
                case COLOR_RGB:
                    if (ip != null && img.NumberOfChannels == 1)
                        return (", 颜色=" + v[0]);
                    else
                    {
                        String hex = Color.FromArgb(v[0], v[1], v[2]).ToString();
                        return (", 颜色=" + PublicFunctions.Pad(v[0], 3) + "," + PublicFunctions.Pad(v[1], 3) + "," + PublicFunctions.Pad(v[2], 3));
                    }
                default: return ("");
            }
        }

    }
}
