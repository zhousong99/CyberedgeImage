using Emgu.CV;
using Emgu.CV.CvEnum;


namespace CyberedgeImageProcess2024
{
    /// <summary>
    /// 记录当前显示的图像是在整个图像的位置和大小，以及其他一些处理方法
    /// srcRect记录了图像在整个图像上的位置
    /// pictureBoxMat为窗口显示的图像
    /// </summary>
    public class ImageMat
    {
        private static double[] zoomLevels = {
        1/72.0, 1/48.0, 1/32.0, 1/24.0, 1/16.0, 1/12.0,
        1/8.0, 1/6.0, 1/4.0, 1/3.0, 1/2.0, 0.75, 1.0, 1.5,
        2.0, 3.0, 4.0, 6.0, 8.0, 12.0, 16.0, 24.0, 32.0 };

        /** If the mouse moves less than this in screen pixels, successive zoom operations are on the same image pixel */
        protected const int MAX_MOUSEMOVE_ZOOM = 10;
        private const int LIST_OFFSET = 100000;
        private Color showAllColor = Color.FromArgb(0, 255, 255);
        private Color defaultColor = Color.FromArgb(0, 255, 255);

        EdgeImagePlus imp;
        Mat pictureBoxMat;
        protected Rectangle srcRect;
        protected int imageWidth, imageHeight;
        protected int dstWidth, dstHeight;
        protected Form mainForm;
        protected double magnification;
        protected EdgeImagePlus clipboard;
        protected bool imageUpdated;
        private Roi currentRoi;


        protected int lastZoomSX = -9999999;
        protected int lastZoomSY = -9999999;

        protected int zoomTargetOX = -1;
        protected int zoomTargetOY;
        protected int xMouseStart;
        protected int yMouseStart;
        protected int xSrcStart;
        protected int ySrcStart;
        protected int xMouse; // current cursor offscreen x location 
        protected int yMouse; // current cursor offscreen y location
        protected int flags;
        private bool showCursorStatus = true;
        

        public int DstWidth
        { get { return dstWidth; } }

        public int DstHeight
        {
            get { return dstHeight; }
        }

        public int SrcWidth
        {
            get { return srcRect.Width; }
        }

        public int SrcHeight
        {
            get { return srcRect.Height; }
        }

        public int SrcTop
        {
            get { return srcRect.Top; }
        }

        public int SrcLeft
        {
            get { return srcRect.Left; }
        }

        public Mat PictureBoxMat
        {
            get { return pictureBoxMat; }
            set { pictureBoxMat = value; }
        }

        public ImageMat(EdgeImagePlus imp)
        {
            this.imp = imp;

            mainForm = Application.OpenForms[0];
            int width = imp.Width;
            int height = imp.Height;
            imageWidth = width;
            imageHeight = height;
            srcRect = new Rectangle(0, 0, imageWidth, imageHeight);
            SetSize(imageWidth, imageHeight);
            magnification = 1.0;
        }

        public void SetMagnification(double magnification)
        {
            SetMagnification2(magnification);
        }

        void SetMagnification2(double magnification)
        {
            if (magnification > 32.0)
                magnification = 32.0;
            if (magnification < zoomLevels[0])
                magnification = zoomLevels[0];
            this.magnification = magnification;
            imp.SetTitle(imp.Title);
        }


        public static double GetLowerZoomLevel(double currentMag)
        {
            double newMag = zoomLevels[0];
            for (int i = 0; i < zoomLevels.Length; i++)
            {
                if (zoomLevels[i] < currentMag)
                    newMag = zoomLevels[i];
                else
                    break;
            }
            return newMag;
        }

        public static double getHigherZoomLevel(double currentMag)
        {
            double newMag = 32.0;
            for (int i = zoomLevels.Length - 1; i >= 0; i--)
            {
                if (zoomLevels[i] > currentMag)
                    newMag = zoomLevels[i];
                else
                    break;
            }
            return newMag;
        }

        /// <summary>
        /// 返回图像能放大到多大，如果不能放大了
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        protected Size canEnlarge(int newWidth, int newHeight)
        {
            ImageWindow win = imp.ImageWindow;
            if (win == null) return new Size(0, 0);

            if (newWidth < PublicConst.MaxWidth && newHeight < PublicConst.MaxHeight)
                return new Size(newWidth, newHeight);
            else
                return new Size(0, 0);
        }

        public void SetSize(int width, int height)
        {
            dstWidth = width;
            dstHeight = height;
        }

        public double GetMagnification()
        {
            return magnification;
        }

        /// <summary>
        /// 放大。如果用户移动过鼠标，则以sx, sy为中心点， 否则中心点不变
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        public void zoomIn(int sx, int sy)
        {
            if (magnification >= 32) return;

            bool mouseMoved = Math.Pow(sx - lastZoomSX, 2) + Math.Pow(sy - lastZoomSY, 2) > MAX_MOUSEMOVE_ZOOM * MAX_MOUSEMOVE_ZOOM;
            lastZoomSX = sx;
            lastZoomSY = sy;
            if (mouseMoved)
            {
                zoomTargetOX = srcRect.Left + SrcWidth / 2;
                zoomTargetOY = srcRect.Top + SrcHeight / 2;
            }
            double newMag = getHigherZoomLevel(magnification);
            int newWidth = (int)(imageWidth * newMag);
            int newHeight = (int)(imageHeight * newMag);
            Size newSize = canEnlarge(newWidth, newHeight);
            if (newSize.Width != 0)
            {
                SetSize(newSize.Width, newSize.Height);
                if (newSize.Width != newWidth || newSize.Height != newHeight)
                    AdjustSourceRect(newMag, zoomTargetOX, zoomTargetOY);
                else
                    SetMagnification(newMag);
                imp.ImageWindow.Pack();
            }
            else // can't enlarge window
                AdjustSourceRect(newMag, zoomTargetOX, zoomTargetOY);

            Paint();

        }

        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        public void zoomOut(int sx, int sy)
        {
            if (magnification <= zoomLevels[0])
                return;
            bool mouseMoved = Math.Pow(sx - lastZoomSX, 2) + Math.Pow(sy - lastZoomSY, 2) > MAX_MOUSEMOVE_ZOOM * MAX_MOUSEMOVE_ZOOM;
            lastZoomSX = sx;
            lastZoomSY = sy;
            if (mouseMoved || zoomTargetOX < 0)
            {
                zoomTargetOX = srcRect.Left + SrcWidth / 2;
                zoomTargetOY = srcRect.Top + SrcHeight / 2;
            }
            double oldMag = magnification;
            double newMag = GetLowerZoomLevel(magnification);
            double srcRatio = (double)srcRect.Width / srcRect.Height;
            double imageRatio = (double)imageWidth / imageHeight;
            double initialMag = imp.ImageWindow.InitialMagnification;
            if (Math.Abs(srcRatio - imageRatio) > 0.05)
            {
                double scale = oldMag / newMag;
                int newSrcWidth = (int)Math.Round(srcRect.Width * scale);
                int newSrcHeight = (int)Math.Round(srcRect.Height * scale);
                if (newSrcWidth > imageWidth) newSrcWidth = imageWidth;
                if (newSrcHeight > imageHeight) newSrcHeight = imageHeight;
                int newSrcX = srcRect.X - (newSrcWidth - srcRect.Width) / 2;
                int newSrcY = srcRect.Y - (newSrcHeight - srcRect.Height) / 2;
                if (newSrcX + newSrcWidth > imageWidth) newSrcX = imageWidth - newSrcWidth;
                if (newSrcY + newSrcHeight > imageHeight) newSrcY = imageHeight - newSrcHeight;
                if (newSrcX < 0) newSrcX = 0;
                if (newSrcY < 0) newSrcY = 0;
                srcRect = new Rectangle(newSrcX, newSrcY, newSrcWidth, newSrcHeight);
                int newDstWidth = (int)(srcRect.Width * newMag);
                int newDstHeight = (int)(srcRect.Height * newMag);
                SetMagnification(newMag);

                if (newDstWidth < dstWidth || newDstHeight < dstHeight)
                {
                    SetSize(newDstWidth, newDstHeight);
                    imp.ImageWindow.Pack();
                }
                else
                {
                    Paint();
                }
                return;
            }
            if (imageWidth * newMag > dstWidth)
            {
                int w = (int)Math.Round(dstWidth / newMag);
                if (w * newMag < dstWidth) w++;
                int h = (int)Math.Round(dstHeight / newMag);
                if (h * newMag < dstHeight) h++;
                Rectangle r = new Rectangle(zoomTargetOX - w / 2, zoomTargetOY - h / 2, w, h);
                if (r.X < 0) r.X = 0;
                if (r.Y < 0) r.Y = 0;
                if (r.X + w > imageWidth) r.X = imageWidth - w;
                if (r.Y + h > imageHeight) r.Y = imageHeight - h;
                srcRect = r;
                SetMagnification(newMag);
            }
            else
            {
                srcRect = new Rectangle(0, 0, imageWidth, imageHeight);
                SetSize((int)(imageWidth * newMag), (int)(imageHeight * newMag));
                SetMagnification(newMag);
                imp.ImageWindow.Pack();
            }
            Paint();
        }

        /// <summary>
        /// 调节位置，以防出现错误的范围
        /// </summary>
        /// <param name="newMag">新的缩放倍数</param>
        /// <param name="x">中心点x</param>
        /// <param name="y">中心点y</param>
        void AdjustSourceRect(double newMag, int x, int y)
        {
            int w = (int)Math.Round(dstWidth / newMag);
            if (w * newMag < dstWidth) w++;
            int h = (int)Math.Round(dstHeight / newMag);
            if (h * newMag < dstHeight) h++;
            Rectangle r = new Rectangle(x - w / 2, y - h / 2, w, h);
            if (r.X < 0) r.X = 0;
            if (r.Y < 0) r.Y = 0;
            if (r.X + w > imageWidth) r.X = imageWidth - w;
            if (r.Y + h > imageHeight) r.Y = imageHeight - h;

            srcRect = r;
            SetMagnification(newMag);
        }

        public void Scroll(int sx, int sy)
        {
            int ox = sx;
            int oy = sy;

            int newx = xSrcStart + (int)((xMouseStart - ox) / magnification);
            int newy = ySrcStart + (int)((yMouseStart - oy) / magnification);
            if (newx < 0) newx = 0;
            if (newy < 0) newy = 0;
            if ((newx + srcRect.Width) > imageWidth) newx = imageWidth - srcRect.Width;
            if ((newy + srcRect.Height) > imageHeight) newy = imageHeight - srcRect.Height;
            srcRect.X = newx;
            srcRect.Y = newy;

            Paint();
        }


        protected void SetupScroll(int ox, int oy)
        {
            xMouseStart = ox;
            yMouseStart = oy;
            xSrcStart = srcRect.X;
            ySrcStart = srcRect.Y;
        }

        public void MouseMoved(MouseEventArgs e)
        {
            int sx = e.X;
            int sy = e.Y;
            int ox = OffScreenX(sx);
            int oy = OffScreenY(sy);
            flags = (int)e.Button;
            SetCursor(sx, sy, ox, oy);

            Roi roi = imp.Roi;
            int type = roi != null ? roi.Type : -1;

            ImageWindow win = imp.ImageWindow;

            if (win != null && showCursorStatus)  //显示鼠标位置和像素点值
                win.MouseMoved(ox, oy);
        }

        /// <summary>
        /// 拖动鼠标
        /// </summary>
        /// <param name="e"></param>
        public void MouseDragged(MouseEventArgs e)
        {
            Roi roi = imp.Roi;

            int x = e.X;
            int y = e.Y;
            xMouse = OffScreenX(x);
            yMouse = OffScreenY(y);
            //mousePressedX = mousePressedY = -1;
            if (PublicConst.CurrentTool == PublicConst.TOOL_HAND || PublicFunctions.IsKeyPressed(Keys.Space))
            {
                Scroll(x, y);    //图像滚动放在ImageWindow里处理了
            }
            else
            {
                    roi?.HandleMouseDrag(x, y, flags);
            }
        }

        /** Sets the cursor based on the current tool and cursor location. */
        public void SetCursor(int sx, int sy, int ox, int oy)
        {
            xMouse = ox;
            yMouse = oy;
            Roi roi = imp.Roi;
            ImageWindow win = imp.ImageWindow;

            if (win == null)
                return;
            if (PublicFunctions.IsKeyPressed(Keys.Space))
            {
                SetCursor(Cursors.Hand);
                return;
            }
            int id = PublicConst.CurrentTool;
            switch (id)
            {
                //case PublicConst.TOOL_MAGNIFIER:
                 //   SetCursor(Cursors.SizeAll);
                 //   break;
                case PublicConst.TOOL_HAND:
                    SetCursor(Cursors.Hand);
                    break;
                default:  //selection tool

                    if (roi != null && roi.State != Roi.CONSTRUCTING && roi.IsHandle(sx, sy) >= 0)
                    {
                        SetCursor(Cursors.Hand);
                    }

                    else if ((roi != null && roi.State != Roi.CONSTRUCTING && roi.Contains(ox, oy)))
                        SetCursor(Cursors.Default);
                    else
                        SetCursor(Cursors.Cross);

                    break;
            }
        }



        public void SetCursor(Cursor cursor)
        {
            if (imp == null) return;
            ImageWindow win = imp.ImageWindow;
            if (win == null) return;
            win.SetCursor(cursor);
        }



        /// <summary>
        /// 垂直滚动条进行了滚动
        /// </summary>
        /// <param name="h">滚动的距离</param>
        public void VScroll(int h)
        {
            xMouseStart = 0;
            yMouseStart = 0;
            xSrcStart = srcRect.X;
            ySrcStart = srcRect.Y;

            Scroll(xMouseStart, yMouseStart + h);
        }

        /// <summary>
        /// 水平滚动条进行了滚动
        /// </summary>
        /// <param name="w">滚动的距离</param>
        public void HScroll(int w)
        {
            xMouseStart = 0;
            yMouseStart = 0;
            xSrcStart = srcRect.X;
            ySrcStart = srcRect.Y;

            Scroll(xMouseStart + w, yMouseStart);
        }

        /// <summary>
        /// 画出图像，如果有Roi，一起画出
        /// </summary>
        public void Paint()  //这两个要合并
        {
            if (imageUpdated)
            {
                imageUpdated = false;
                imp.UpdateImage();
            }

            PaintFromImg();

        }

        /// <summary>
        /// 不更新图片，只从Img中刷新窗口图像。
        /// </summary>
        public void PaintFromImg()
        {

            pictureBoxMat = new Mat(imp.Img, srcRect).Clone();
            if (srcRect.Width != dstWidth || srcRect.Height != dstHeight)
            {
                CvInvoke.Resize(pictureBoxMat, pictureBoxMat, new Size(dstWidth, DstHeight));
            }

            Roi roi = imp.Roi;

            if (roi != null) DrawRoi(roi, pictureBoxMat);
            if (srcRect.Width < imageWidth || srcRect.Height < imageHeight)
                drawZoomIndicator();
            imp.ImageWindow.Rendering();
        }

        public void Paint(int x, int y, int w, int h)
        {
            Roi roi = imp.Roi;
            if (imageUpdated)
            {
                imageUpdated = false;
                imp.UpdateImage();
            }

            pictureBoxMat = new Mat(imp.Img, srcRect).Clone();
            if (srcRect.Width != dstWidth || srcRect.Height != dstHeight)
            {
                CvInvoke.Resize(pictureBoxMat, pictureBoxMat, new Size(dstWidth, DstHeight));
            }
            if (roi != null) DrawRoi(roi, pictureBoxMat);

            if (srcRect.Width < imageWidth || srcRect.Height < imageHeight)
                drawZoomIndicator();

            imp.ImageWindow.Rendering(x, y, w, h);
        }

        /// <summary>
        /// 画出Roi
        /// </summary>
        /// <param name="roi"></param>
        /// <param name="g"></param>
        private void DrawRoi(Roi roi, Mat g)
        {
            if (roi == currentRoi)
            {
                Color lineColor = roi.StrokeColor;
                Color fillColor = roi.FillColor;
                float lineWidth = roi.GetStrokeWidth();
                roi.StrokeColor = Color.Black;
                roi.FillColor = Color.Black;
                bool strokeSet = roi.GetStroke() != null;
                if (strokeSet)
                    roi.SetStrokeWidth(1);
                roi.Draw(g);
                roi.StrokeColor = lineColor;
                if (strokeSet)
                    roi.SetStrokeWidth(lineWidth);
                roi.FillColor = fillColor;
                currentRoi = null;
            }
            else
                roi.Draw(g);
        }


        void drawZoomIndicator()
        {
            if (!PublicConst.ZoomIndicatorVisible) return;
            int x1 = 10;
            int y1 = 10;
            double aspectRatio = (double)imageHeight / imageWidth;
            int w1 = 64;
            if (aspectRatio > 1.0)
                w1 = (int)(w1 / aspectRatio);
            int h1 = (int)(w1 * aspectRatio);
            if (w1 < 4) w1 = 4;
            if (h1 < 4) h1 = 4;
            int w2 = (int)(w1 * ((double)srcRect.Width / imageWidth));
            int h2 = (int)(h1 * ((double)srcRect.Height / imageHeight));
            if (w2 < 1) w2 = 1;
            if (h2 < 1) h2 = 1;
            int x2 = (int)(w1 * ((double)srcRect.X / imageWidth));
            int y2 = (int)(h1 * ((double)srcRect.Y / imageHeight));

            CvInvoke.Rectangle(pictureBoxMat, new Rectangle(x1, y1, w1, h1), new Emgu.CV.Structure.MCvScalar(128, 128, 255));
            CvInvoke.Rectangle(pictureBoxMat, new Rectangle(x1 + x2, y1 + y2, w2, h2), new Emgu.CV.Structure.MCvScalar(128, 128, 255));
        }

        /// <summary>
        /// 将显示屏幕上的坐标转换为图像上的坐标
        /// </summary>
        /// <param name="sx"></param>
        /// <returns></returns>
        public int OffScreenX2(int sx)
        {
            return srcRect.X + (int)Math.Round(sx / magnification);
        }

        /// <summary>
        /// 将显示屏幕上的坐标转换为图像上的坐标
        /// </summary>
        /// <param name="sy"></param>
        /// <returns></returns>
        public int OffScreenY2(int sy)
        {
            return srcRect.Y + (int)Math.Round(sy / magnification);
        }

        /// <summary>
        /// 将显示屏幕上的坐标转换为图像上的坐标,最靠近像素中心点
        /// </summary>
        /// <param name="sx"></param>
        /// <returns></returns>
        public int OffScreenX(int sx)
        {
            return srcRect.X + (int)(sx / magnification);
        }

        /// <summary>
        /// 将显示屏幕上的坐标转换为图像上的坐标，最靠近像素中心点
        /// </summary>
        /// <param name="sy"></param>
        /// <returns></returns>
        public int OffScreenY(int sy)
        {
            return srcRect.Y + (int)(sy / magnification);
        }

        /**Converts a screen x-coordinate to a floating-point offscreen x-coordinate.*/
        public double OffScreenXD(int sx)
        {
            return srcRect.X + sx / magnification;
        }

        /**Converts a screen y-coordinate to a floating-point offscreen y-coordinate.*/
        public double OffScreenYD(int sy)
        {
            return srcRect.Y + sy / magnification;

        }

        /**Converts an offscreen x-coordinate to a screen x-coordinate.*/
        public int ScreenX(int ox)
        {
            return (int)((ox - srcRect.X) * magnification);
        }

        /**Converts an offscreen y-coordinate to a screen y-coordinate.*/
        public int ScreenY(int oy)
        {
            return (int)((oy - srcRect.Y) * magnification);
        }

        /**Converts a floating-point offscreen x-coordinate to a screen x-coordinate.*/
        public int ScreenXD(double ox)
        {
            return (int)((ox - srcRect.X) * magnification);
        }

        /**Converts a floating-point offscreen x-coordinate to a screen x-coordinate.*/
        public int ScreenYD(double oy)
        {
            return (int)((oy - srcRect.Y) * magnification);
        }

        public void MousePressed(MouseEventArgs e)
        {
            showCursorStatus = true;
            int toolID = PublicConst.CurrentTool;
            ImageWindow win = imp.ImageWindow;

            int x = e.X;
            int y = e.Y;
            flags = (int)e.Button;


            int ox = OffScreenX(x);
            int oy = OffScreenY(y);
            xMouse = ox; yMouse = oy;

            if (PublicFunctions.IsKeyPressed(Keys.Space))  //按下空格键不放表示图像的拖动
            {
                SetupScroll(e.X, e.Y);
                return;
            }

            Roi roi1 = imp.Roi;
            int size1 = roi1 != null ? roi1.Size() : 0;
            RectangleF r1 = roi1 != null ? roi1.Bounds : Rectangle.Empty;

            switch (toolID)
            {
                case PublicConst.TOOL_HAND:     //图像拖动
                case PublicConst.TOOL_UNUSED:
                    SetupScroll(e.X, e.Y);
                    break;
                default:  //selection tool
                    HandleRoiMouseDown(e);
                    break;
            }

        }

        public void MouseReleased(MouseEventArgs e)
        {
            int ox = OffScreenX(e.X);
            int oy = OffScreenY(e.Y);


            Roi roi = imp.Roi;
            if (roi != null)
            {
                RectangleF r = roi.Bounds;
                int type = roi.Type;
                if ((r.Width == 0 || r.Height == 0)  && roi.State == Roi.CONSTRUCTING)
                {
                    imp.DeleteRoi();
                }
                else
                {
                    roi.HandleMouseUp(e.X, e.Y);
                }
            }
        }

        protected void HandleRoiMouseDown(MouseEventArgs e)
        {
            int sx = e.X;
            int sy = e.Y;
            int ox = OffScreenX(sx);
            int oy = OffScreenY(sy);
            Roi roi = imp.Roi;
            int tool = PublicConst.CurrentTool;

            int handle = roi != null ? roi.IsHandle(sx, sy) : -1;

            if (roi != null)
            {
                if (handle >= 0)  //鼠标点在了调整柄中
                {
                    roi.MouseDownInHandle(handle, sx, sy);
                    return;
                }

                RectangleF r = roi.Bounds;
                int type = roi.Type;
                if (type == Roi.RECTANGLE && r.Width == imp.Width && r.Height == imp.Height
                && roi.PasteMode == Roi.NOT_PASTING)
                {
                    imp.DeleteRoi();
                    return;
                }

                if (roi.Contains(ox, oy))         //鼠标点在了roi中
                {
                    if (roi.ModState == Roi.NO_MODS)
                        roi.HandleMouseDown(sx, sy);
                    else
                    {
                        imp.DeleteRoi();
                        imp.CreateNewRoi(sx, sy);
                    }
                    return;
                }
            }

            imp.CreateNewRoi(sx, sy);  //点击了工具后，第一次在图像上点击鼠标，新建roi
        }

        public void SetImageUpdated()
        {
            imageUpdated = true;
        }

        public void UpdateImage(EdgeImagePlus imp)
        {
            this.imp = imp;
            int width = imp.Width;
            int height = imp.Height;
            imageWidth = width;
            imageHeight = height;
            srcRect = new Rectangle(0, 0, imageWidth, imageHeight);
            SetSize(imageWidth, imageHeight);
            magnification = 1.0;
        }
    }
}

