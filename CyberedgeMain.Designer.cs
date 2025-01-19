
namespace CyberedgeImageProcess2024
{
    partial class CyberedgeMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CyberedgeMain));
            menuMain = new MenuStrip();
            miFile = new ToolStripMenuItem();
            miOpen = new ToolStripMenuItem();
            miOpenRecent = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            miClose = new ToolStripMenuItem();
            miCloseAll = new ToolStripMenuItem();
            miSave = new ToolStripMenuItem();
            miSaveAs = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            miPageSetup = new ToolStripMenuItem();
            toolStripMenuItem15 = new ToolStripMenuItem();
            miPrint = new ToolStripMenuItem();
            miQuit = new ToolStripMenuItem();
            miEdit = new ToolStripMenuItem();
            miCopy = new ToolStripMenuItem();
            miImage = new ToolStripMenuItem();
            miZoom = new ToolStripMenuItem();
            miGray = new ToolStripMenuItem();
            miNegative = new ToolStripMenuItem();
            miLUT = new ToolStripMenuItem();
            fireToolStripMenuItem = new ToolStripMenuItem();
            iceToolStripMenuItem = new ToolStripMenuItem();
            rGB322ToolStripMenuItem = new ToolStripMenuItem();
            redGreenToolStripMenuItem = new ToolStripMenuItem();
            红RToolStripMenuItem = new ToolStripMenuItem();
            绿GToolStripMenuItem = new ToolStripMenuItem();
            蓝BToolStripMenuItem = new ToolStripMenuItem();
            青CToolStripMenuItem = new ToolStripMenuItem();
            品红ToolStripMenuItem = new ToolStripMenuItem();
            黄YToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem12 = new ToolStripSeparator();
            色彩合成ToolStripMenuItem = new ToolStripMenuItem();
            三色分离ToolStripMenuItem = new ToolStripMenuItem();
            miProcess = new ToolStripMenuItem();
            滤波FToolStripMenuItem = new ToolStripMenuItem();
            卷积CToolStripMenuItem = new ToolStripMenuItem();
            高斯模糊GToolStripMenuItem = new ToolStripMenuItem();
            中值MToolStripMenuItem = new ToolStripMenuItem();
            均值EToolStripMenuItem = new ToolStripMenuItem();
            锐化UToolStripMenuItem = new ToolStripMenuItem();
            双边IToolStripMenuItem = new ToolStripMenuItem();
            miSmooth = new ToolStripMenuItem();
            miSharpen = new ToolStripMenuItem();
            miFindEdges = new ToolStripMenuItem();
            处理PToolStripMenuItem = new ToolStripMenuItem();
            直方图均衡HToolStripMenuItem = new ToolStripMenuItem();
            对比度增强EToolStripMenuItem = new ToolStripMenuItem();
            cLAHEcToolStripMenuItem = new ToolStripMenuItem();
            形态学OToolStripMenuItem = new ToolStripMenuItem();
            腐蚀EToolStripMenuItem = new ToolStripMenuItem();
            膨胀DToolStripMenuItem = new ToolStripMenuItem();
            开OToolStripMenuItem = new ToolStripMenuItem();
            关CToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem13 = new ToolStripSeparator();
            高帽TToolStripMenuItem1 = new ToolStripMenuItem();
            黑帽BToolStripMenuItem1 = new ToolStripMenuItem();
            梯度GToolStripMenuItem = new ToolStripMenuItem();
            运算MToolStripMenuItem = new ToolStripMenuItem();
            加AToolStripMenuItem = new ToolStripMenuItem();
            减SToolStripMenuItem = new ToolStripMenuItem();
            乘MToolStripMenuItem = new ToolStripMenuItem();
            除DToolStripMenuItem = new ToolStripMenuItem();
            与NToolStripMenuItem = new ToolStripMenuItem();
            或OToolStripMenuItem = new ToolStripMenuItem();
            异或XToolStripMenuItem = new ToolStripMenuItem();
            最小IToolStripMenuItem = new ToolStripMenuItem();
            最大ToolStripMenuItem = new ToolStripMenuItem();
            伽马GToolStripMenuItem = new ToolStripMenuItem();
            设值EToolStripMenuItem = new ToolStripMenuItem();
            对数LToolStripMenuItem = new ToolStripMenuItem();
            乘幂PToolStripMenuItem = new ToolStripMenuItem();
            平方QToolStripMenuItem = new ToolStripMenuItem();
            平方根RToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem6 = new ToolStripSeparator();
            图像运算ToolStripMenuItem = new ToolStripMenuItem();
            背景扣除BToolStripMenuItem = new ToolStripMenuItem();
            miTool = new ToolStripMenuItem();
            miCommonTools = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripSeparator();
            miWindows = new ToolStripMenuItem();
            层叠CToolStripMenuItem = new ToolStripMenuItem();
            平铺TToolStripMenuItem = new ToolStripMenuItem();
            miHelp = new ToolStripMenuItem();
            miHelpView = new ToolStripMenuItem();
            miAbout = new ToolStripMenuItem();
            ToolStrip = new ToolStrip();
            btnRectangle = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            btnScroll = new ToolStripButton();
            lblStatus = new ToolStripLabel();
            progressBar = new ToolStripProgressBar();
            menuMain.SuspendLayout();
            ToolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuMain
            // 
            menuMain.ImageScalingSize = new Size(20, 20);
            menuMain.Items.AddRange(new ToolStripItem[] { miFile, miEdit, miImage, miProcess, miTool, miWindows, miHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Padding = new Padding(7, 3, 0, 3);
            menuMain.Size = new Size(1195, 30);
            menuMain.TabIndex = 0;
            menuMain.Text = "menuStrip1";
            // 
            // miFile
            // 
            miFile.DropDownItems.AddRange(new ToolStripItem[] { miOpen, miOpenRecent, toolStripMenuItem1, miClose, miCloseAll, miSave, miSaveAs, toolStripMenuItem2, miPageSetup, toolStripMenuItem15, miPrint, miQuit });
            miFile.Name = "miFile";
            miFile.Size = new Size(71, 24);
            miFile.Text = "文件(&F)";
            // 
            // miOpen
            // 
            miOpen.Name = "miOpen";
            miOpen.ShortcutKeys = Keys.Control | Keys.O;
            miOpen.Size = new Size(275, 26);
            miOpen.Text = "打开(&O)";
            miOpen.Click += miOpen_Click;
            // 
            // miOpenRecent
            // 
            miOpenRecent.Name = "miOpenRecent";
            miOpenRecent.Size = new Size(275, 26);
            miOpenRecent.Text = "最近打开(&E)";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(272, 6);
            // 
            // miClose
            // 
            miClose.Name = "miClose";
            miClose.ShortcutKeys = Keys.Control | Keys.W;
            miClose.Size = new Size(275, 26);
            miClose.Text = "关闭(&C)";
            miClose.Click += miClose_Click;
            // 
            // miCloseAll
            // 
            miCloseAll.Name = "miCloseAll";
            miCloseAll.ShortcutKeys = Keys.Control | Keys.Shift | Keys.W;
            miCloseAll.Size = new Size(275, 26);
            miCloseAll.Text = "关闭所有(&L)";
            miCloseAll.Click += miCloseAll_Click;
            // 
            // miSave
            // 
            miSave.Name = "miSave";
            miSave.ShortcutKeys = Keys.Control | Keys.S;
            miSave.Size = new Size(275, 26);
            miSave.Text = "保存(&S)";
            // 
            // miSaveAs
            // 
            miSaveAs.Name = "miSaveAs";
            miSaveAs.Size = new Size(275, 26);
            miSaveAs.Text = "另存为(&A)...";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(272, 6);
            // 
            // miPageSetup
            // 
            miPageSetup.Name = "miPageSetup";
            miPageSetup.Size = new Size(275, 26);
            miPageSetup.Text = "页面设置(&U)...";
            miPageSetup.Click += miPageSetup_Click;
            // 
            // toolStripMenuItem15
            // 
            toolStripMenuItem15.Name = "toolStripMenuItem15";
            toolStripMenuItem15.Size = new Size(275, 26);
            toolStripMenuItem15.Text = "打印设置(&T)...";
            toolStripMenuItem15.Click += toolStripMenuItem15_Click;
            // 
            // miPrint
            // 
            miPrint.Name = "miPrint";
            miPrint.ShortcutKeys = Keys.Control | Keys.P;
            miPrint.Size = new Size(275, 26);
            miPrint.Text = "打印(&P)...";
            miPrint.Click += miPrint_Click;
            // 
            // miQuit
            // 
            miQuit.Name = "miQuit";
            miQuit.ShortcutKeys = Keys.Alt | Keys.F4;
            miQuit.Size = new Size(275, 26);
            miQuit.Text = "退出(&X)";
            miQuit.Click += miQuit_Click;
            // 
            // miEdit
            // 
            miEdit.DropDownItems.AddRange(new ToolStripItem[] { miCopy });
            miEdit.Name = "miEdit";
            miEdit.ShortcutKeys = Keys.Control | Keys.C;
            miEdit.Size = new Size(71, 24);
            miEdit.Text = "编辑(&E)";
            // 
            // miCopy
            // 
            miCopy.Name = "miCopy";
            miCopy.ShortcutKeys = Keys.Control | Keys.C;
            miCopy.Size = new Size(198, 26);
            miCopy.Text = "复制(&C)";
            miCopy.Click += miCopy_Click;
            // 
            // miImage
            // 
            miImage.DropDownItems.AddRange(new ToolStripItem[] { miZoom, miGray, miNegative, miLUT, 色彩合成ToolStripMenuItem, 三色分离ToolStripMenuItem });
            miImage.Name = "miImage";
            miImage.Size = new Size(67, 24);
            miImage.Text = "图像(&I)";
            // 
            // miZoom
            // 
            miZoom.Name = "miZoom";
            miZoom.Size = new Size(189, 26);
            miZoom.Text = "缩放(&Z)...";
            miZoom.Click += miZoom_Click;
            // 
            // miGray
            // 
            miGray.Name = "miGray";
            miGray.Size = new Size(189, 26);
            miGray.Text = "灰度图(&G)";
            miGray.Click += miGray_Click;
            // 
            // miNegative
            // 
            miNegative.Name = "miNegative";
            miNegative.Size = new Size(189, 26);
            miNegative.Text = "负片(&N)";
            miNegative.Click += miNegative_Click;
            // 
            // miLUT
            // 
            miLUT.DropDownItems.AddRange(new ToolStripItem[] { fireToolStripMenuItem, iceToolStripMenuItem, rGB322ToolStripMenuItem, redGreenToolStripMenuItem, 红RToolStripMenuItem, 绿GToolStripMenuItem, 蓝BToolStripMenuItem, 青CToolStripMenuItem, 品红ToolStripMenuItem, 黄YToolStripMenuItem, toolStripMenuItem12 });
            miLUT.Name = "miLUT";
            miLUT.Size = new Size(189, 26);
            miLUT.Text = "伪彩(&L)...";
            // 
            // fireToolStripMenuItem
            // 
            fireToolStripMenuItem.Name = "fireToolStripMenuItem";
            fireToolStripMenuItem.Size = new Size(149, 26);
            fireToolStripMenuItem.Text = "火(&F)";
            fireToolStripMenuItem.Click += fireToolStripMenuItem_Click;
            // 
            // iceToolStripMenuItem
            // 
            iceToolStripMenuItem.Name = "iceToolStripMenuItem";
            iceToolStripMenuItem.Size = new Size(149, 26);
            iceToolStripMenuItem.Text = "冰(&I)";
            iceToolStripMenuItem.Click += iceToolStripMenuItem_Click;
            // 
            // rGB322ToolStripMenuItem
            // 
            rGB322ToolStripMenuItem.Name = "rGB322ToolStripMenuItem";
            rGB322ToolStripMenuItem.Size = new Size(149, 26);
            rGB322ToolStripMenuItem.Text = "RGB&332";
            rGB322ToolStripMenuItem.Click += rGB322ToolStripMenuItem_Click;
            // 
            // redGreenToolStripMenuItem
            // 
            redGreenToolStripMenuItem.Name = "redGreenToolStripMenuItem";
            redGreenToolStripMenuItem.Size = new Size(149, 26);
            redGreenToolStripMenuItem.Text = "红绿(&G)";
            redGreenToolStripMenuItem.Click += redGreenToolStripMenuItem_Click;
            // 
            // 红RToolStripMenuItem
            // 
            红RToolStripMenuItem.Name = "红RToolStripMenuItem";
            红RToolStripMenuItem.Size = new Size(149, 26);
            红RToolStripMenuItem.Text = "红(&R)";
            红RToolStripMenuItem.Click += 红RToolStripMenuItem_Click;
            // 
            // 绿GToolStripMenuItem
            // 
            绿GToolStripMenuItem.Name = "绿GToolStripMenuItem";
            绿GToolStripMenuItem.Size = new Size(149, 26);
            绿GToolStripMenuItem.Text = "绿(&E)";
            绿GToolStripMenuItem.Click += 绿GToolStripMenuItem_Click;
            // 
            // 蓝BToolStripMenuItem
            // 
            蓝BToolStripMenuItem.Name = "蓝BToolStripMenuItem";
            蓝BToolStripMenuItem.Size = new Size(149, 26);
            蓝BToolStripMenuItem.Text = "蓝(&B)";
            蓝BToolStripMenuItem.Click += 蓝BToolStripMenuItem_Click;
            // 
            // 青CToolStripMenuItem
            // 
            青CToolStripMenuItem.Name = "青CToolStripMenuItem";
            青CToolStripMenuItem.Size = new Size(149, 26);
            青CToolStripMenuItem.Text = "青(&C)";
            青CToolStripMenuItem.Click += 青CToolStripMenuItem_Click;
            // 
            // 品红ToolStripMenuItem
            // 
            品红ToolStripMenuItem.Name = "品红ToolStripMenuItem";
            品红ToolStripMenuItem.Size = new Size(149, 26);
            品红ToolStripMenuItem.Text = "品红(&M)";
            品红ToolStripMenuItem.Click += 品红ToolStripMenuItem_Click;
            // 
            // 黄YToolStripMenuItem
            // 
            黄YToolStripMenuItem.Name = "黄YToolStripMenuItem";
            黄YToolStripMenuItem.Size = new Size(149, 26);
            黄YToolStripMenuItem.Text = "黄(&Y)";
            黄YToolStripMenuItem.Click += 黄YToolStripMenuItem_Click;
            // 
            // toolStripMenuItem12
            // 
            toolStripMenuItem12.Name = "toolStripMenuItem12";
            toolStripMenuItem12.Size = new Size(146, 6);
            // 
            // 色彩合成ToolStripMenuItem
            // 
            色彩合成ToolStripMenuItem.Name = "色彩合成ToolStripMenuItem";
            色彩合成ToolStripMenuItem.Size = new Size(189, 26);
            色彩合成ToolStripMenuItem.Text = "色彩合成(&M)...";
            色彩合成ToolStripMenuItem.Click += 色彩合成ToolStripMenuItem_Click;
            // 
            // 三色分离ToolStripMenuItem
            // 
            三色分离ToolStripMenuItem.Name = "三色分离ToolStripMenuItem";
            三色分离ToolStripMenuItem.Size = new Size(189, 26);
            三色分离ToolStripMenuItem.Text = "三色分离(&S)";
            三色分离ToolStripMenuItem.Click += 三色分离ToolStripMenuItem_Click;
            // 
            // miProcess
            // 
            miProcess.DropDownItems.AddRange(new ToolStripItem[] { 滤波FToolStripMenuItem, miSmooth, miSharpen, miFindEdges, 处理PToolStripMenuItem, 形态学OToolStripMenuItem, 运算MToolStripMenuItem, toolStripMenuItem6, 图像运算ToolStripMenuItem, 背景扣除BToolStripMenuItem });
            miProcess.Name = "miProcess";
            miProcess.Size = new Size(72, 24);
            miProcess.Text = "处理(&P)";
            // 
            // 滤波FToolStripMenuItem
            // 
            滤波FToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 卷积CToolStripMenuItem, 高斯模糊GToolStripMenuItem, 中值MToolStripMenuItem, 均值EToolStripMenuItem, 锐化UToolStripMenuItem, 双边IToolStripMenuItem });
            滤波FToolStripMenuItem.Name = "滤波FToolStripMenuItem";
            滤波FToolStripMenuItem.Size = new Size(185, 26);
            滤波FToolStripMenuItem.Text = "滤波(&F)";
            // 
            // 卷积CToolStripMenuItem
            // 
            卷积CToolStripMenuItem.Name = "卷积CToolStripMenuItem";
            卷积CToolStripMenuItem.Size = new Size(185, 26);
            卷积CToolStripMenuItem.Text = "卷积(&C)...";
            卷积CToolStripMenuItem.Click += 卷积CToolStripMenuItem_Click;
            // 
            // 高斯模糊GToolStripMenuItem
            // 
            高斯模糊GToolStripMenuItem.Name = "高斯模糊GToolStripMenuItem";
            高斯模糊GToolStripMenuItem.Size = new Size(185, 26);
            高斯模糊GToolStripMenuItem.Text = "高斯模糊(&G)...";
            高斯模糊GToolStripMenuItem.Click += 高斯模糊GToolStripMenuItem_Click;
            // 
            // 中值MToolStripMenuItem
            // 
            中值MToolStripMenuItem.Name = "中值MToolStripMenuItem";
            中值MToolStripMenuItem.Size = new Size(185, 26);
            中值MToolStripMenuItem.Text = "中值(&M)...";
            中值MToolStripMenuItem.Click += 中值MToolStripMenuItem_Click;
            // 
            // 均值EToolStripMenuItem
            // 
            均值EToolStripMenuItem.Name = "均值EToolStripMenuItem";
            均值EToolStripMenuItem.Size = new Size(185, 26);
            均值EToolStripMenuItem.Text = "均值(&E)...";
            均值EToolStripMenuItem.Click += 均值EToolStripMenuItem_Click;
            // 
            // 锐化UToolStripMenuItem
            // 
            锐化UToolStripMenuItem.Name = "锐化UToolStripMenuItem";
            锐化UToolStripMenuItem.Size = new Size(185, 26);
            锐化UToolStripMenuItem.Text = "锐化(&U)...";
            锐化UToolStripMenuItem.Click += 锐化UToolStripMenuItem_Click;
            // 
            // 双边IToolStripMenuItem
            // 
            双边IToolStripMenuItem.Name = "双边IToolStripMenuItem";
            双边IToolStripMenuItem.Size = new Size(185, 26);
            双边IToolStripMenuItem.Text = "双边(&I)...";
            双边IToolStripMenuItem.Click += 双边IToolStripMenuItem_Click;
            // 
            // miSmooth
            // 
            miSmooth.Name = "miSmooth";
            miSmooth.Size = new Size(185, 26);
            miSmooth.Text = "平滑(&S)";
            miSmooth.Click += miSmooth_Click;
            // 
            // miSharpen
            // 
            miSharpen.Name = "miSharpen";
            miSharpen.Size = new Size(185, 26);
            miSharpen.Text = "锐化(&P)";
            miSharpen.Click += miSharpen_Click;
            // 
            // miFindEdges
            // 
            miFindEdges.Name = "miFindEdges";
            miFindEdges.Size = new Size(185, 26);
            miFindEdges.Text = "边缘(&E)";
            miFindEdges.Click += miFindEdges_Click;
            // 
            // 处理PToolStripMenuItem
            // 
            处理PToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 直方图均衡HToolStripMenuItem, 对比度增强EToolStripMenuItem, cLAHEcToolStripMenuItem });
            处理PToolStripMenuItem.Name = "处理PToolStripMenuItem";
            处理PToolStripMenuItem.Size = new Size(185, 26);
            处理PToolStripMenuItem.Text = "增强(&P)";
            // 
            // 直方图均衡HToolStripMenuItem
            // 
            直方图均衡HToolStripMenuItem.Name = "直方图均衡HToolStripMenuItem";
            直方图均衡HToolStripMenuItem.Size = new Size(215, 26);
            直方图均衡HToolStripMenuItem.Text = "直方图均衡(&H)";
            直方图均衡HToolStripMenuItem.Click += 直方图均衡HToolStripMenuItem_Click;
            // 
            // 对比度增强EToolStripMenuItem
            // 
            对比度增强EToolStripMenuItem.Name = "对比度增强EToolStripMenuItem";
            对比度增强EToolStripMenuItem.Size = new Size(215, 26);
            对比度增强EToolStripMenuItem.Text = "对比度自动增强(&E)";
            对比度增强EToolStripMenuItem.Click += 对比度增强EToolStripMenuItem_Click;
            // 
            // cLAHEcToolStripMenuItem
            // 
            cLAHEcToolStripMenuItem.Name = "cLAHEcToolStripMenuItem";
            cLAHEcToolStripMenuItem.Size = new Size(215, 26);
            cLAHEcToolStripMenuItem.Text = "CLAHE(&C)";
            cLAHEcToolStripMenuItem.Click += cLAHEcToolStripMenuItem_Click;
            // 
            // 形态学OToolStripMenuItem
            // 
            形态学OToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 腐蚀EToolStripMenuItem, 膨胀DToolStripMenuItem, 开OToolStripMenuItem, 关CToolStripMenuItem, toolStripMenuItem13, 高帽TToolStripMenuItem1, 黑帽BToolStripMenuItem1, 梯度GToolStripMenuItem });
            形态学OToolStripMenuItem.Name = "形态学OToolStripMenuItem";
            形态学OToolStripMenuItem.Size = new Size(185, 26);
            形态学OToolStripMenuItem.Text = "形态学(&O)";
            // 
            // 腐蚀EToolStripMenuItem
            // 
            腐蚀EToolStripMenuItem.Name = "腐蚀EToolStripMenuItem";
            腐蚀EToolStripMenuItem.Size = new Size(143, 26);
            腐蚀EToolStripMenuItem.Text = "腐蚀(&E)";
            腐蚀EToolStripMenuItem.Click += 腐蚀EToolStripMenuItem_Click;
            // 
            // 膨胀DToolStripMenuItem
            // 
            膨胀DToolStripMenuItem.Name = "膨胀DToolStripMenuItem";
            膨胀DToolStripMenuItem.Size = new Size(143, 26);
            膨胀DToolStripMenuItem.Text = "膨胀(&D)";
            膨胀DToolStripMenuItem.Click += 膨胀DToolStripMenuItem_Click;
            // 
            // 开OToolStripMenuItem
            // 
            开OToolStripMenuItem.Name = "开OToolStripMenuItem";
            开OToolStripMenuItem.Size = new Size(143, 26);
            开OToolStripMenuItem.Text = "开(&O)";
            开OToolStripMenuItem.Click += 开OToolStripMenuItem_Click;
            // 
            // 关CToolStripMenuItem
            // 
            关CToolStripMenuItem.Name = "关CToolStripMenuItem";
            关CToolStripMenuItem.Size = new Size(143, 26);
            关CToolStripMenuItem.Text = "关(&C)";
            关CToolStripMenuItem.Click += 关CToolStripMenuItem_Click;
            // 
            // toolStripMenuItem13
            // 
            toolStripMenuItem13.Name = "toolStripMenuItem13";
            toolStripMenuItem13.Size = new Size(140, 6);
            // 
            // 高帽TToolStripMenuItem1
            // 
            高帽TToolStripMenuItem1.Name = "高帽TToolStripMenuItem1";
            高帽TToolStripMenuItem1.Size = new Size(143, 26);
            高帽TToolStripMenuItem1.Text = "高帽(&T)";
            高帽TToolStripMenuItem1.Click += 高帽TToolStripMenuItem1_Click;
            // 
            // 黑帽BToolStripMenuItem1
            // 
            黑帽BToolStripMenuItem1.Name = "黑帽BToolStripMenuItem1";
            黑帽BToolStripMenuItem1.Size = new Size(143, 26);
            黑帽BToolStripMenuItem1.Text = "黑帽(&B)";
            黑帽BToolStripMenuItem1.Click += 黑帽BToolStripMenuItem1_Click;
            // 
            // 梯度GToolStripMenuItem
            // 
            梯度GToolStripMenuItem.Name = "梯度GToolStripMenuItem";
            梯度GToolStripMenuItem.Size = new Size(143, 26);
            梯度GToolStripMenuItem.Text = "梯度(&G)";
            梯度GToolStripMenuItem.Click += 梯度GToolStripMenuItem_Click;
            // 
            // 运算MToolStripMenuItem
            // 
            运算MToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 加AToolStripMenuItem, 减SToolStripMenuItem, 乘MToolStripMenuItem, 除DToolStripMenuItem, 与NToolStripMenuItem, 或OToolStripMenuItem, 异或XToolStripMenuItem, 最小IToolStripMenuItem, 最大ToolStripMenuItem, 伽马GToolStripMenuItem, 设值EToolStripMenuItem, 对数LToolStripMenuItem, 乘幂PToolStripMenuItem, 平方QToolStripMenuItem, 平方根RToolStripMenuItem });
            运算MToolStripMenuItem.Name = "运算MToolStripMenuItem";
            运算MToolStripMenuItem.Size = new Size(185, 26);
            运算MToolStripMenuItem.Text = "运算(&M)";
            // 
            // 加AToolStripMenuItem
            // 
            加AToolStripMenuItem.Name = "加AToolStripMenuItem";
            加AToolStripMenuItem.Size = new Size(157, 26);
            加AToolStripMenuItem.Text = "加(&A)...";
            加AToolStripMenuItem.Click += 加AToolStripMenuItem_Click;
            // 
            // 减SToolStripMenuItem
            // 
            减SToolStripMenuItem.Name = "减SToolStripMenuItem";
            减SToolStripMenuItem.Size = new Size(157, 26);
            减SToolStripMenuItem.Text = "减(&S)...";
            减SToolStripMenuItem.Click += 减SToolStripMenuItem_Click;
            // 
            // 乘MToolStripMenuItem
            // 
            乘MToolStripMenuItem.Name = "乘MToolStripMenuItem";
            乘MToolStripMenuItem.Size = new Size(157, 26);
            乘MToolStripMenuItem.Text = "乘(&M)...";
            乘MToolStripMenuItem.Click += 乘MToolStripMenuItem_Click;
            // 
            // 除DToolStripMenuItem
            // 
            除DToolStripMenuItem.Name = "除DToolStripMenuItem";
            除DToolStripMenuItem.Size = new Size(157, 26);
            除DToolStripMenuItem.Text = "除(&D)...";
            除DToolStripMenuItem.Click += 除DToolStripMenuItem_Click;
            // 
            // 与NToolStripMenuItem
            // 
            与NToolStripMenuItem.Name = "与NToolStripMenuItem";
            与NToolStripMenuItem.Size = new Size(157, 26);
            与NToolStripMenuItem.Text = "与(N)...";
            与NToolStripMenuItem.Click += 与NToolStripMenuItem_Click;
            // 
            // 或OToolStripMenuItem
            // 
            或OToolStripMenuItem.Name = "或OToolStripMenuItem";
            或OToolStripMenuItem.Size = new Size(157, 26);
            或OToolStripMenuItem.Text = "或(&O)...";
            或OToolStripMenuItem.Click += 或OToolStripMenuItem_Click;
            // 
            // 异或XToolStripMenuItem
            // 
            异或XToolStripMenuItem.Name = "异或XToolStripMenuItem";
            异或XToolStripMenuItem.Size = new Size(157, 26);
            异或XToolStripMenuItem.Text = "异或(&X)...";
            异或XToolStripMenuItem.Click += 异或XToolStripMenuItem_Click;
            // 
            // 最小IToolStripMenuItem
            // 
            最小IToolStripMenuItem.Name = "最小IToolStripMenuItem";
            最小IToolStripMenuItem.Size = new Size(157, 26);
            最小IToolStripMenuItem.Text = "最小(&I)...";
            最小IToolStripMenuItem.Click += 最小IToolStripMenuItem_Click;
            // 
            // 最大ToolStripMenuItem
            // 
            最大ToolStripMenuItem.Name = "最大ToolStripMenuItem";
            最大ToolStripMenuItem.Size = new Size(157, 26);
            最大ToolStripMenuItem.Text = "最大(&U)...";
            最大ToolStripMenuItem.Click += 最大ToolStripMenuItem_Click;
            // 
            // 伽马GToolStripMenuItem
            // 
            伽马GToolStripMenuItem.Name = "伽马GToolStripMenuItem";
            伽马GToolStripMenuItem.Size = new Size(157, 26);
            伽马GToolStripMenuItem.Text = "伽马(&G)...";
            伽马GToolStripMenuItem.Click += 伽马GToolStripMenuItem_Click;
            // 
            // 设值EToolStripMenuItem
            // 
            设值EToolStripMenuItem.Name = "设值EToolStripMenuItem";
            设值EToolStripMenuItem.Size = new Size(157, 26);
            设值EToolStripMenuItem.Text = "设值(&E)..";
            设值EToolStripMenuItem.Click += 设值EToolStripMenuItem_Click;
            // 
            // 对数LToolStripMenuItem
            // 
            对数LToolStripMenuItem.Name = "对数LToolStripMenuItem";
            对数LToolStripMenuItem.Size = new Size(157, 26);
            对数LToolStripMenuItem.Text = "对数(&L)";
            对数LToolStripMenuItem.Click += 对数LToolStripMenuItem_Click;
            // 
            // 乘幂PToolStripMenuItem
            // 
            乘幂PToolStripMenuItem.Name = "乘幂PToolStripMenuItem";
            乘幂PToolStripMenuItem.Size = new Size(157, 26);
            乘幂PToolStripMenuItem.Text = "乘幂(&P)";
            乘幂PToolStripMenuItem.Click += 乘幂PToolStripMenuItem_Click;
            // 
            // 平方QToolStripMenuItem
            // 
            平方QToolStripMenuItem.Name = "平方QToolStripMenuItem";
            平方QToolStripMenuItem.Size = new Size(157, 26);
            平方QToolStripMenuItem.Text = "平方(&Q)";
            平方QToolStripMenuItem.Click += 平方QToolStripMenuItem_Click;
            // 
            // 平方根RToolStripMenuItem
            // 
            平方根RToolStripMenuItem.Name = "平方根RToolStripMenuItem";
            平方根RToolStripMenuItem.Size = new Size(157, 26);
            平方根RToolStripMenuItem.Text = "平方根(&R)";
            平方根RToolStripMenuItem.Click += 平方根RToolStripMenuItem_Click;
            // 
            // toolStripMenuItem6
            // 
            toolStripMenuItem6.Name = "toolStripMenuItem6";
            toolStripMenuItem6.Size = new Size(182, 6);
            // 
            // 图像运算ToolStripMenuItem
            // 
            图像运算ToolStripMenuItem.Name = "图像运算ToolStripMenuItem";
            图像运算ToolStripMenuItem.Size = new Size(185, 26);
            图像运算ToolStripMenuItem.Text = "图像运算(&C)...";
            图像运算ToolStripMenuItem.Click += 图像运算ToolStripMenuItem_Click;
            // 
            // 背景扣除BToolStripMenuItem
            // 
            背景扣除BToolStripMenuItem.Name = "背景扣除BToolStripMenuItem";
            背景扣除BToolStripMenuItem.Size = new Size(185, 26);
            背景扣除BToolStripMenuItem.Text = "光照不均匀(&L)";
            背景扣除BToolStripMenuItem.Click += 背景扣除BToolStripMenuItem_Click;
            // 
            // miTool
            // 
            miTool.DropDownItems.AddRange(new ToolStripItem[] { miCommonTools, toolStripMenuItem4 });
            miTool.Name = "miTool";
            miTool.Size = new Size(72, 24);
            miTool.Text = "工具(&T)";
            // 
            // miCommonTools
            // 
            miCommonTools.Name = "miCommonTools";
            miCommonTools.Size = new Size(173, 26);
            miCommonTools.Text = "常用工具(&G)";
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(170, 6);
            // 
            // miWindows
            // 
            miWindows.DropDownItems.AddRange(new ToolStripItem[] { 层叠CToolStripMenuItem, 平铺TToolStripMenuItem });
            miWindows.Name = "miWindows";
            miWindows.Size = new Size(78, 24);
            miWindows.Text = "窗口(&W)";
            // 
            // 层叠CToolStripMenuItem
            // 
            层叠CToolStripMenuItem.Name = "层叠CToolStripMenuItem";
            层叠CToolStripMenuItem.Size = new Size(142, 26);
            层叠CToolStripMenuItem.Text = "层叠(&C)";
            层叠CToolStripMenuItem.Click += 层叠CToolStripMenuItem_Click;
            // 
            // 平铺TToolStripMenuItem
            // 
            平铺TToolStripMenuItem.Name = "平铺TToolStripMenuItem";
            平铺TToolStripMenuItem.Size = new Size(142, 26);
            平铺TToolStripMenuItem.Text = "平铺(&T)";
            平铺TToolStripMenuItem.Click += 平铺TToolStripMenuItem_Click;
            // 
            // miHelp
            // 
            miHelp.DropDownItems.AddRange(new ToolStripItem[] { miHelpView, miAbout });
            miHelp.Name = "miHelp";
            miHelp.Size = new Size(75, 24);
            miHelp.Text = "帮助(&H)";
            // 
            // miHelpView
            // 
            miHelpView.Name = "miHelpView";
            miHelpView.ShortcutKeys = Keys.F1;
            miHelpView.Size = new Size(224, 26);
            miHelpView.Text = "查看帮助(&H)...";
            miHelpView.Visible = false;
            miHelpView.Click += miHelpView_Click;
            // 
            // miAbout
            // 
            miAbout.Name = "miAbout";
            miAbout.Size = new Size(224, 26);
            miAbout.Text = "关于(&A)...";
            miAbout.Click += miAbout_Click;
            // 
            // ToolStrip
            // 
            ToolStrip.ImageScalingSize = new Size(20, 20);
            ToolStrip.Items.AddRange(new ToolStripItem[] { btnRectangle, toolStripSeparator2, btnScroll, lblStatus, progressBar });
            ToolStrip.Location = new Point(0, 30);
            ToolStrip.Name = "ToolStrip";
            ToolStrip.Size = new Size(1195, 27);
            ToolStrip.TabIndex = 3;
            ToolStrip.Text = "toolStrip1";
            // 
            // btnRectangle
            // 
            btnRectangle.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnRectangle.Image = (Image)resources.GetObject("btnRectangle.Image");
            btnRectangle.ImageTransparentColor = Color.Magenta;
            btnRectangle.Name = "btnRectangle";
            btnRectangle.Size = new Size(29, 24);
            btnRectangle.Text = "矩形";
            btnRectangle.ToolTipText = "矩形";
            btnRectangle.Click += btnRectangle_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 27);
            // 
            // btnScroll
            // 
            btnScroll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnScroll.Image = (Image)resources.GetObject("btnScroll.Image");
            btnScroll.ImageTransparentColor = Color.White;
            btnScroll.Name = "btnScroll";
            btnScroll.Size = new Size(29, 24);
            btnScroll.Text = "移动";
            btnScroll.Click += btnScroll_Click;
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(17, 24);
            lblStatus.Text = "  ";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(141, 24);
            progressBar.Visible = false;
            // 
            // CyberedgeMain
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1195, 707);
            Controls.Add(ToolStrip);
            Controls.Add(menuMain);
            DoubleBuffered = true;
            IsMdiContainer = true;
            MainMenuStrip = menuMain;
            Margin = new Padding(3, 4, 3, 4);
            Name = "CyberedgeMain";
            Text = "Cyberedge图像处理";
            WindowState = FormWindowState.Maximized;
            FormClosing += CyberedgeMain_FormClosing;
            Load += CyberedgeMain_Load;
            Shown += CyberedgeMain_Shown;
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            ToolStrip.ResumeLayout(false);
            ToolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripMenuItem miOpen;
        private System.Windows.Forms.ToolStripMenuItem miOpenRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miClose;
        private System.Windows.Forms.ToolStripMenuItem miCloseAll;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem miPageSetup;
        private System.Windows.Forms.ToolStripMenuItem miPrint;
        private System.Windows.Forms.ToolStripMenuItem miQuit;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miImage;
        private System.Windows.Forms.ToolStripMenuItem miZoom;
        private System.Windows.Forms.ToolStripMenuItem miGray;
        private System.Windows.Forms.ToolStripMenuItem miNegative;
        private System.Windows.Forms.ToolStripMenuItem miLUT;
        private System.Windows.Forms.ToolStripMenuItem miProcess;
        private System.Windows.Forms.ToolStripMenuItem miTool;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem miWindows;
        private System.Windows.Forms.ToolStripMenuItem 层叠CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平铺TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem miHelpView;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripMenuItem 色彩合成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 三色分离ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 滤波FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miSmooth;
        private System.Windows.Forms.ToolStripMenuItem miSharpen;
        private System.Windows.Forms.ToolStripMenuItem miFindEdges;
        private System.Windows.Forms.ToolStripMenuItem 形态学OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 运算MToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem 图像运算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 背景扣除BToolStripMenuItem;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripButton btnRectangle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnScroll;
        private System.Windows.Forms.ToolStripLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem miCommonTools;
        private System.Windows.Forms.ToolStripMenuItem 加AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 减SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 乘MToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 除DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 与NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 或OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 异或XToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 最小IToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 最大ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 伽马GToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设值EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 对数LToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 乘幂PToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平方QToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平方根RToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 腐蚀EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 膨胀DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关CToolStripMenuItem;
        private ToolStripMenuItem fireToolStripMenuItem;
        private ToolStripMenuItem iceToolStripMenuItem;
        private ToolStripMenuItem rGB322ToolStripMenuItem;
        private ToolStripMenuItem redGreenToolStripMenuItem;
        private ToolStripMenuItem 红RToolStripMenuItem;
        private ToolStripMenuItem 绿GToolStripMenuItem;
        private ToolStripMenuItem 蓝BToolStripMenuItem;
        private ToolStripMenuItem 青CToolStripMenuItem;
        private ToolStripMenuItem 品红ToolStripMenuItem;
        private ToolStripMenuItem 黄YToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem12;
        private ToolStripMenuItem 卷积CToolStripMenuItem;
        private ToolStripMenuItem 高斯模糊GToolStripMenuItem;
        private ToolStripMenuItem 中值MToolStripMenuItem;
        private ToolStripMenuItem 均值EToolStripMenuItem;
        private ToolStripMenuItem 锐化UToolStripMenuItem;
        private ToolStripMenuItem 双边IToolStripMenuItem;
        private ToolStripMenuItem 处理PToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem13;
        private ToolStripMenuItem 高帽TToolStripMenuItem1;
        private ToolStripMenuItem 黑帽BToolStripMenuItem1;
        private ToolStripMenuItem 梯度GToolStripMenuItem;
        private ToolStripMenuItem 直方图均衡HToolStripMenuItem;
        private ToolStripMenuItem 对比度增强EToolStripMenuItem;
        private ToolStripMenuItem cLAHEcToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripProgressBar progressBar;
    }
}

