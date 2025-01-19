using Emgu.CV;



namespace CyberedgeImageProcess2024
{
    public partial class ImageWindow : Form
    {
       protected EdgeImagePlus imp;
        protected Form mainForm;
        protected ImageMat im;
        private double initialMagnification = 1;
        protected bool closed;

        private int screenHeight, screenWidth;
        private bool mouseDown = false;

        //当用户按下Escape键或关闭窗口，则running设置为false
        public bool running;

        //如果用户在窗口中点击鼠标、按下Escape键或关闭窗口，则running2设置为false
        public bool running2;

        public EdgeImagePlus Imp { get { return imp; } }

        public double InitialMagnification
        {
            get { return initialMagnification; }
        }
        public ImageWindow()
        {
            InitializeComponent();

            this.SizeChanged -= ImageWindow_SizeChanged;
            this.PictureBox.MouseDown -= PictureBox_MouseDown;
            this.PictureBox.MouseMove -= PictureBox_MouseMove;
            this.PictureBox.MouseUp -= PictureBox_MouseUp;


            PictureBox.ContextMenuStrip = null;
            mainForm = Application.OpenForms[0];
            this.MdiParent = mainForm;
            this.DoubleBuffered = true;
            this.PictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(PictureBox_MouseWheel);
        }

        public ImageWindow(EdgeImagePlus image) : this(image, null)
        {
        }

        public ImageWindow(EdgeImagePlus image, ImageMat im) : this()
        {
            imp = image;
            if (im == null)
            {
                im = new ImageMat(imp);
            }
            this.im = im;
            imp.SetWindow(this);   //建立双向链接
            SetLocationAndSize(false);

            im.Paint();   //把imp中选取的图像区域赋值到picturebox.image上。
            ImageWindowShow();

            this.PictureBox.MouseDown += PictureBox_MouseDown;
            this.PictureBox.MouseMove += PictureBox_MouseMove;
            this.PictureBox.MouseUp += PictureBox_MouseUp;
        }

        /// <summary>
        /// 设置初始的窗口大小，据此设置初始的缩放倍数
        /// </summary>
        /// <param name="updating"></param>
        private void SetLocationAndSize(bool updating)
        {
            if (imp == null)
                return;
            int width = imp.Width;
            int height = imp.Height;

            double screenScalingFactor = PublicFunctions.GetScreenScale();
            screenHeight = (int)(PublicConst.MaxHeight / screenScalingFactor);
            screenWidth = (int)(PublicConst.MaxWidth / screenScalingFactor);

            //找到合适的窗口大小
            double mag = 1;
            while (width * mag > screenWidth || height * mag >= screenHeight)
            {
                double mag2 = ImageMat.GetLowerZoomLevel(mag);
                if (mag2 == mag) break;
                mag = mag2;
            }


                initialMagnification = mag;
                im.SetSize((int)(width * mag), (int)(height * mag));
           
            im.SetMagnification(mag);

            this.SizeChanged -= ImageWindow_SizeChanged;
            this.Width -= this.PictureBox.Width - im.DstWidth;
            this.Height -= this.PictureBox.Height - im.DstHeight;
            this.SizeChanged += ImageWindow_SizeChanged;
        }



        public void SetTitle(string title)
        {
            Text = title;
        }


        public void ImageWindowShow()
        {
            DrawInfo();
            Show();
        }


        public void DrawInfo()
        {
            if (imp == null)
                return;
            lblSubTitle.Text = createSubtitle();
        }


        /// <summary>
        /// 得到本图像窗口中图像的大小，返回字符串
        /// </summary>
        /// <param name="imp"></param>
        /// <returns></returns>
        public string getImageSize(EdgeImagePlus imp)
        {
            if (imp == null)
                return null;
            double size = imp.GetSizeInBytes() / 1024.0;
            String s2 = null, s3 = null;
            if (size < 1024.0)
            { s2 = size.ToString("F0"); s3 = "K"; }
            else if (size < 10000.0)
            { s2 = (size / 1024.0).ToString("F1"); s3 = "MB"; }
            else if (size < 1048576.0)
            { s2 = Math.Round(size / 1024.0).ToString("F0"); s3 = "MB"; }
            else
            { s2 = (size / 1048576.0).ToString("F1"); s3 = "GB"; }
            if (s2.EndsWith(".0")) s2 = s2.Substring(0, s2.Length - 2);
            return s2 + s3;
        }

        //改变大小只改变缩放倍数， 不改变im.srcRectangle位置
        private void ImageWindow_SizeChanged(object sender, EventArgs e)
        {
            if (im == null) return;
            double wScale = 1.0 * this.PictureBox.Width / im.SrcWidth;
            double hScale = 1.0 * this.PictureBox.Height / im.SrcHeight;
            double mag = Math.Min(wScale, hScale);
            im.SetSize((int)(im.SrcWidth * mag), (int)(im.SrcHeight * mag));

            im.SetMagnification(mag);
            this.SizeChanged -= ImageWindow_SizeChanged;
            this.Width -= this.PictureBox.Width - im.DstWidth;
            this.Height -= this.PictureBox.Height - im.DstHeight;
            this.SizeChanged += ImageWindow_SizeChanged;
        }

        public String createSubtitle()
        {
            String s = "";
            if (imp == null)
                return s;

            Emgu.CV.CvEnum.DepthType type = imp.Type;
            int channels = imp.NumberOfChannels;


            s += imp.Width.ToString() + "x" + imp.Height.ToString() + " pixels; ";


            if (imp.ImageProcessor is ColorProcessor)
            {
                s += "RGB";
            }
            else if (imp.ImageProcessor is ByteProcessor)
            {
                s += "8-bit";
            }
            else if (imp.ImageProcessor is ShortProcessor)
            {
                s += "16-bit";
            }
            else if (imp.ImageProcessor is FloatProcessor)
            {
                s += "32-bit";
            }
            return s + "; " + getImageSize(imp);
        }


        public ImageMat GetCanvas()
        {
            return im;
        }

        public void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0) { im.zoomIn(e.X, e.Y); }
            else im.zoomOut(e.X, e.Y);
        }

        private void ImageWindow_Load(object sender, EventArgs e)
        {

        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            im.MousePressed(e);
            mouseDown = true;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (PublicFunctions.IsKeyPressed(Keys.Space))   //按下空格键表示移动图像
            {
                ToolHand_MouseMove(e);
                return;
            }

            if (mouseDown)
            {
                im.MouseDragged(e);
                return;
            }

            im.MouseMoved(e);
        }

        /// <summary>
        /// 用于显示鼠标位置和像素点信息，暂时不用
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseMoved(int x, int y)
        {
            imp.MouseMoved(x, y);
        }

        /// <summary>
        /// 拖动鼠标，移动图像
        /// </summary>
        /// <param name="e"></param>
        private void ToolHand_MouseMove(MouseEventArgs e)
        {
            if (imp.Img.Height == im.SrcHeight || imp.Img.Width == im.SrcWidth) return;

            if (mouseDown)
            {
                im.Scroll(e.X, e.Y);
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            im.MouseReleased(e);
        }

        /// <summary>
        /// 根据im.DstWidth, im.DstHeight来调整窗口的大小，从而调整pictureBox的大小
        /// </summary>
        public void Pack()
        {
            this.SizeChanged -= ImageWindow_SizeChanged;
            this.Width -= this.PictureBox.Width - im.DstWidth;
            this.Height -= this.PictureBox.Height - im.DstHeight;
            this.SizeChanged += ImageWindow_SizeChanged;
        }


        /// <summary>
        /// 将imp中的一部分或全部，以及标注和roi等绘制到PictureBox
        /// </summary>
        public void Rendering()
        {
            if (PictureBox.Image != null)
            {
                PictureBox.Image.Dispose();
            }
            PictureBox.Image = BitmapExtension.ToBitmap(im.PictureBoxMat);
        }

        /// <summary>
        /// 更新图像上的某个矩形区域
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void Rendering(int x, int y, int w, int h)
        {

            Graphics g = Graphics.FromImage(PictureBox.Image);

            int offX = 0, offY = 0, offW = 0, offH = 0;
            if (x < 0) offX = -x;
            if (y < 0) offY = -y;
            int b = im.PictureBoxMat.Width - (x + w);
            if (b < 0) offW = -b;

            b = im.PictureBoxMat.Height - (y + h);
            if (b < 0) offH = -b;

            x = x + offX < 0 ? 0 : x + offX;
            y = y + offY < 0 ? 0 : y + offY;
            w = w - offW < im.PictureBoxMat.Width ? w - offW : im.PictureBoxMat.Width;
            h = h - offH < im.PictureBoxMat.Height ? h - offH : im.PictureBoxMat.Height;


            Mat mat = new Mat(im.PictureBoxMat, new Rectangle(x, y, w, h));
            g.DrawImage(BitmapExtension.ToBitmap(mat), new Rectangle(x, y, w, h));


            PictureBox.Image = BitmapExtension.ToBitmap(im.PictureBoxMat);

        }



        private void PictureBox_Click(object sender, EventArgs e)
        {

        }

        public void SetCursor(Cursor cursor)
        {
            this.PictureBox.Cursor = cursor;
        }

        private void ImageWindow_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (imp != null && ((e.KeyChar >= 32 && e.KeyChar <= 255) || e.KeyChar == '\b' || e.KeyChar == '\n'))
            {
                Roi roi = imp.Roi;

            }
        }

        private void lblSubTitle_Click(object sender, EventArgs e)
        {

        }

        public void Repaint()
        {
            DrawInfo();
            PictureBox.Refresh();
        }

        public void UpdateImage(EdgeImagePlus imp)
        {
            if (imp != this.imp)
                throw new Exception("imp!=this.imp");
            this.imp = imp;
            im.UpdateImage(imp);
            SetLocationAndSize(true);

            Pack();
            Repaint();

        }

    }
}
