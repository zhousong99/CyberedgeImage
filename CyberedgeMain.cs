using System.Diagnostics;
using System.Diagnostics;

namespace CyberedgeImageProcess2024
{

    public partial class CyberedgeMain : Form
    {
        PageSetupDialog fPageSetupDialog;
        OpenFileDialog openFileDialog;
        public CyberedgeMain()
        {
            InitDialogSetup();
            InitializeComponent();
            ManageCommonToolsMenu();
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            using (frmAboutUs AboutUs = new frmAboutUs())
            {
                AboutUs.ShowDialog();
            }
        }

        private void miClose_Click(object sender, EventArgs e)
        {
            Form form = this.ActiveMdiChild;
            if (form == null) return;
            if (form is ImageWindow)
            {
                form.Close();
            }
        }

        private void miOpen_Click(object sender, EventArgs e)
        {
            Opener opener = new Opener();
            opener.Open(openFileDialog);
            AddOpenRecentItem(openFileDialog.FileName);
        }

        private void InitDialogSetup()
        {
            fPageSetupDialog = new PageSetupDialog();
            fPageSetupDialog.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            fPageSetupDialog.PageSettings = new System.Drawing.Printing.PageSettings();
            fPageSetupDialog.EnableMetric = false;


            openFileDialog = new OpenFileDialog();
        }

        private void miCloseAll_Click(object sender, EventArgs e)
        {
            foreach (Form form in this.MdiChildren)
            {
                if (form is ImageWindow)
                {
                    form.Close();
                }
            }
        }

        private void miQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void miPageSetup_Click(object sender, EventArgs e)
        {
            fPageSetupDialog.ShowDialog();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;

            Printer printer = new Printer(imp);
            printer.PageSetup();
        }

        /// <summary>
        /// 获取当前使用的图像
        /// </summary>
        /// <returns></returns>
        private EdgeImagePlus CurrentImagePlus()
        {
            //获取当前图像窗口
            Form form = this.ActiveMdiChild;
            if (form == null || !(form is ImageWindow))
            {
                MessageBox.Show("请先选择一个图像窗口");
                return null;
            }

            return (form as ImageWindow).Imp;   //当前窗口中的图像
        }

        private void miPrint_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;

            Printer printer = new Printer(imp);
            printer.SetPageFormat(fPageSetupDialog.PageSettings);
            printer.Print();
        }

        private void CyberedgeMain_Load(object sender, EventArgs e)
        {
            PublicConst.MaxWidth = this.ClientSize.Width - 20;
            PublicConst.MaxHeight = this.ClientSize.Height - 80;
        }

        private void miCopy_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;

            imp.Copy(false);
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            ResetAllButton();
            btnRectangle.Checked = true;
            PublicConst.CurrentTool = PublicConst.TOOL_RECTANGLE;
            PublicConst.Rect_TYPE = PublicConst.RECT_ROI;
            SetCursor();
        }

        public void SetCursor()
        {
            int id = PublicConst.CurrentTool;
            Cursor cursor = Cursors.Default;
            switch (id)
            {
                case PublicConst.TOOL_MAGNIFIER:
                    cursor = Cursors.SizeAll;
                    break;
                case PublicConst.TOOL_HAND:
                    cursor = Cursors.Hand;
                    break;
                case PublicConst.TOOL_UNUSED:
                    cursor = Cursors.Default;
                    break;
                default:  //selection tool
                    cursor = Cursors.Cross;
                    break;
            }

            //循环设置每个窗口
            foreach (Form form in this.MdiChildren)
            {
                if (form is ImageWindow)
                {
                    (form as ImageWindow).SetCursor(cursor);
                }
            }
        }

        private void btnScroll_Click(object sender, EventArgs e)
        {
            bool btnChecked = btnScroll.Checked;
            ResetAllButton();
            btnScroll.Checked = !btnChecked;
            PublicConst.CurrentTool = btnChecked ? PublicConst.TOOL_UNUSED : PublicConst.TOOL_HAND;
            SetCursor();
        }

        private void ResetAllButton()
        {
            for (int i = 0; i < ToolStrip.Items.Count; i++)
            {
                if (ToolStrip.Items[i] is ToolStripButton)
                {
                    ToolStripButton button = ToolStrip.Items[i] as ToolStripButton;
                    button.Checked = false;
                    button.CheckState = CheckState.Unchecked;
                }
            }
        }

        private void miZoom_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("Resize");
        }

        private void miGray_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null || imp.ImageProcessor is ByteProcessor) return;
            Converter.Convert(imp, "8-bit");
        }

        private void miDuplicate_Click(object sender, EventArgs e)
        {

        }

        private void miNegative_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            //imp.RunEmguFunction("Invert");
            imp.RunFunction("Invert");
        }

        private void fireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Fire");
        }

        private void iceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Ice");
        }

        private void rGB322ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("RGB332");
        }

        private void redGreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("RedGreen");
        }

        private void 红RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Red");
        }

        private void 绿GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Green");
        }

        private void 蓝BToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Blue");
        }

        private void 青CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Cyan");
        }

        private void 品红ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Magenta");
        }

        private void 黄YToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunLUT("Yellow");
        }

        private void 色彩合成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;

            imp.ColorMerge();
        }

        private void 三色分离ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;

            imp.SplitRGB(imp);
        }

        private void 卷积CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;

            imp.RunFunction("Filter2D");
        }

        private void 高斯模糊GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("GaussianBlur");
        }

        private void 中值MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("MedianBlur");
        }

        private void 均值EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("Blur");
        }

        private void 锐化UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("UnsharpMask");
        }

        private void 双边IToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("Bilateral");
        }

        private void miSmooth_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("Smooth");
        }

        private void miSharpen_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("Sharpen");
        }

        private void miFindEdges_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("FindEdges");
        }

        private void 直方图均衡HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("EqualizeHist");
        }

        private void 对比度增强EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("Adjust");
        }

        private void cLAHEcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("CLAHE");
        }

        private void 腐蚀EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("Erode");
        }

        private void 膨胀DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("Dilate");
        }

        private void 开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("Open");
        }

        private void 关CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("Close");
        }

        private void 高帽TToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("TopHat");
        }

        private void 黑帽BToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("BlackHat");
        }

        private void 梯度GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunEmguFunction("Gradient");
        }

        private void 层叠CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void 平铺TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void 背景扣除BToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("UnevenLightCompensate");
        }

        private void 图像运算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageCalculator");
        }

        private void 加AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathAdd");
        }

        private void 减SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathSubtracts");
        }

        private void 乘MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathMultiply");
        }

        private void 除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathDivide");
        }

        private void 与NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathAnd");
        }

        private void 或OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathOr");
        }

        private void 异或XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathXor");
        }

        private void 最小IToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathMin");
        }

        private void 最大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathMax");
        }

        private void 对数LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathLog");
        }

        private void 乘幂PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathExp");
        }

        private void 平方QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathSqr");
        }

        private void 平方根RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathSqrt");
        }

        private void 伽马GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathGamma");
        }

        private void 设值EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeImagePlus imp = CurrentImagePlus();
            if (imp == null) return;
            imp.RunFunction("ImageMathSet");
        }

        private void CyberedgeMain_Shown(object sender, EventArgs e)
        {
            LoadRecentItem();
        }

        private void CyberedgeMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveRecentItem();
        }

        ///重写窗体的消息处理函数DefWndProc，从中加入自己定义消息　MYMESSAGE　的检测的处理入口
        protected override void DefWndProc(ref Message m)
        {
            PublicConst.SENDDATASTRUCT myData = new PublicConst.SENDDATASTRUCT();//这是创建自定义信息的结构
            Type mytype = myData.GetType();

            switch (m.Msg)
            {
                //接收自定义消息MYMESSAGE，并显示其参数
                case PublicConst.ADD_OPEN_RECENT_ITEM:
                    myData = (PublicConst.SENDDATASTRUCT)m.GetLParam(mytype);//这里获取的就是作为LParam参数发送来的信息的结构
                    string fileName = myData.lpData; //收到的自定义信息
                    AddOpenRecentItem(fileName);
                    break;
                case PublicConst.SHOW_STATUS:      //在工具栏显示信息
                    myData = (PublicConst.SENDDATASTRUCT)m.GetLParam(mytype);//这里获取的就是作为LParam参数发送来的信息的结构
                    lblStatus.Text = myData.lpData;
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void miHelpView_Click(object sender, EventArgs e)
        {
            

            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "help.pdf";
            myProcess.Start();
        }
    }

}
