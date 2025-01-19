using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;


namespace CyberedgeImageProcess2024
{

    static class PublicFunctions
    {
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            IntPtr hWnd,        // 信息发住的窗口的句柄
            int Msg,            // 消息ID
            int wParam,         // 参数1
            ref PublicConst.SENDDATASTRUCT lParam // 参数2   [MarshalAs(UnmanagedType.LPTStr)]StringBuilder lParam
        );

        /// <summary>
        /// 发送消息给窗口
        /// </summary>
        /// <param name="form">窗口</param>
        /// <param name="messageID">消息号</param>
        /// <param name="message">消息</param>
        public static void SendMessageToForm(Form form, int messageID, string message)
        {
            byte[] myInfo = System.Text.Encoding.Default.GetBytes(message);
            int len = myInfo.Length;
            PublicConst.SENDDATASTRUCT myData;
            myData.dwData = (IntPtr)100;
            myData.lpData = message;
            myData.DataLength = len + 1;
            SendMessage(form.Handle, messageID, 100, ref myData);
        }


        /// <summary>
        /// 打开外部可执行文件
        /// </summary>
        /// <param name="ExePath">可执行文件名称</param>
        /// <param name="Param">命令行参数</param>
        static public void RunExec(string ExePath, string Param = null)
        {
            if (!string.IsNullOrEmpty(Param))
            {
                ExePath = ExePath + " " + Param.Trim();
            }

            try
            {
                Process p = Process.Start(ExePath);
            }
            catch
            {
                MessageBox.Show("无法运行该程序， 请检查路径、程序名称和参数。");
            }

        }

        /// <summary>
        /// 根据文件的特征码来识别文件类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public int GetFileType(String path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.ASCII, true))
                {

                    byte[] buf = new byte[132];
                    sr.BaseStream.Read(buf, 0, 132);
                    //sr.Read(buf, 0, 132);
                    int b0 = buf[0] & 255;
                    int b1 = buf[1] & 255;
                    int b2 = buf[2] & 255;
                    int b3 = buf[3] & 255;

                    // Combined TIFF and DICOM created by GE Senographe scanners
                    if (buf[128] == 68 && buf[129] == 73 && buf[130] == 67 && buf[131] == 77
                    && ((b0 == 73 && b1 == 73) || (b0 == 77 && b1 == 77)))
                        return PublicConst.TIFF_AND_DICOM;

                    // Big-endian TIFF ("MM")
                    if (path.EndsWith(".lsm"))
                        return PublicConst.UNKNOWN; // The LSM	Reader plugin opens these files
                    if (b0 == 73 && b1 == 73 && b2 == 42 && b3 == 0 && !(path.EndsWith(".flex")))
                        return PublicConst.TIFF;

                    // Little-endian TIFF ("II")
                    if (b0 == 77 && b1 == 77 && b2 == 0 && b3 == 42)
                        return PublicConst.TIFF;

                    // JPEG
                    if (b0 == 255 && b1 == 216 && b2 == 255)
                        return PublicConst.JPEG;

                    // GIF ("GIF8")
                    if (b0 == 71 && b1 == 73 && b2 == 70 && b3 == 56)
                        return PublicConst.GIF;

                    path = path.ToLower();

                    // DICOM ("DICM" at offset 128)
                    if (buf[128] == 68 && buf[129] == 73 && buf[130] == 67 && buf[131] == 77 || path.EndsWith(".dcm"))
                    {
                        return PublicConst.DICOM;
                    }

                    // ACR/NEMA with first tag = 00002,00xx or 00008,00xx
                    if ((b0 == 8 || b0 == 2) && b1 == 0 && b3 == 0 && !path.EndsWith(".spe") && !path.Equals("fid"))
                        return PublicConst.DICOM;

                    // PGM ("P1", "P4", "P2", "P5", "P3" or "P6")
                    if (b0 == 80 && (b1 == 49 || b1 == 52 || b1 == 50 || b1 == 53 || b1 == 51 || b1 == 54) && (b2 == 10 || b2 == 13 || b2 == 32 || b2 == 9))
                        return PublicConst.PGM;

                    // Lookup table
                    if (path.EndsWith(".lut"))
                        return PublicConst.LUT;

                    // PNG
                    if (b0 == 137 && b1 == 80 && b2 == 78 && b3 == 71)
                        return PublicConst.PNG;

                    // ZIP containing a TIFF
                    if (path.EndsWith(".zip"))
                        return PublicConst.ZIP;

                    // FITS ("SIMP")
                    if ((b0 == 83 && b1 == 73 && b2 == 77 && b3 == 80) || path.EndsWith(".fts.gz") || path.EndsWith(".fits.gz"))
                        return PublicConst.FITS;

                    // Java source file, text file or macro
                    if (path.EndsWith(".java") || path.EndsWith(".txt") || path.EndsWith(".ijm") || path.EndsWith(".js")
                        || path.EndsWith(".bsh") || path.EndsWith(".py") || path.EndsWith(".html"))
                        return PublicConst.JAVA_OR_TEXT;

                    // ImageJ, NIH Image, Scion Image for Windows ROI
                    if (b0 == 73 && b1 == 111) // "Iout"
                        return PublicConst.ROI;

                    // ObjectJ project
                    if ((b0 == 'o' && b1 == 'j' && b2 == 'j' && b3 == 0) || path.EndsWith(".ojj"))
                        return PublicConst.OJJ;

                    // Results table (tab-delimited or comma-separated tabular text)
                    if (path.EndsWith(".xls") || path.EndsWith(".csv") || path.EndsWith(".tsv"))
                        return PublicConst.TABLE;

                    // AVI
                    if (path.EndsWith(".avi"))
                        return PublicConst.AVI;

                    // Text file
                    bool isText = true;
                    for (int i = 0; i < 10; i++)
                    {
                        int c = buf[i] & 255;
                        if ((c < 32 && c != 9 && c != 10 && c != 13) || c > 126)
                        {
                            isText = false;
                            break;
                        }
                    }
                    if (isText)
                        return PublicConst.TEXT;

                    // BMP ("BM")
                    if ((b0 == 66 && b1 == 77) || path.EndsWith(".dib"))
                        return PublicConst.BMP;

                    // RAW
                    if (path.EndsWith(".raw"))
                        return PublicConst.RAW;

                    return PublicConst.UNKNOWN;

                }

            }

        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }


        // 声明用于 DPI 相关 API 的常量和函数
        private const int DPI_AWARENESS_CONTEXT_UNAWARE = -1;
        private const int DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2;
        private const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3;
        private const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);




        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;

        // 获取宽度缩放百分比
        public static float ScaleX
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                float ScaleX = (float)GetDeviceCaps(hdc, DESKTOPHORZRES) / (float)GetDeviceCaps(hdc, HORZRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return ScaleX;
            }
        }
        // 获取高度缩放百分比
        public static float ScaleY
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                float ScaleY = (float)(float)GetDeviceCaps(hdc, DESKTOPVERTRES) / (float)GetDeviceCaps(hdc, VERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return ScaleY;
            }
        }

        public static float GetScreenScale()
        {
            return Math.Max(ScaleX, ScaleY);

        }

        /// <summary>
        /// 检查文件是不是已经被打开
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool IsDuplicateName(string name)
        {
            if (name == null)
                return false;

            Form mainform = Application.OpenForms[0];
            int winCount = mainform.MdiChildren.Length;
            for (int i = 0; i < winCount; i++)
            {
                if (mainform.MdiChildren[i] is ImageWindow)  //如果图像窗口标题名字已经被用
                {
                    ImageWindow imageWindow = mainform.MdiChildren[i] as ImageWindow;
                    string title = imageWindow.Imp.Title;
                    if (name.Equals(title))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取所有打开的窗口Title
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllTitle()
        {
            List<string> titleList = new List<string>();
            Form mainform = Application.OpenForms[0];
            int winCount = mainform.MdiChildren.Length;
            for (int i = 0; i < winCount; i++)
            {
                if (mainform.MdiChildren[i] is ImageWindow)  //如果图像窗口标题名字已经被用
                {
                    ImageWindow imageWindow = mainform.MdiChildren[i] as ImageWindow;
                    string title = imageWindow.Imp.Title;
                    titleList.Add(title);
                }
            }
            return titleList;
        }

        /** Returns a unique name by adding, before the extension,  -1, -2, etc. as needed. */
        public static String GetUniqueName(String name)
        {
            return GetUniqueName(null, name);
        }

        public static string GetUniqueName(EdgeImagePlus imp, string name)
        {
            String name2 = name;
            String extension = "";
            int len = name2.Length;
            int lastDot = name2.LastIndexOf(".");
            if (lastDot != -1 && len - lastDot < 6 && lastDot != len - 1)
            {
                extension = name2.Substring(lastDot);
                name2 = name2.Substring(0, lastDot);
            }
            int lastDash = name2.LastIndexOf("-");
            len = name2.Length;
            //if (imp != null && imp.getProp("UniqueName") == null)
            lastDash = -1;
            if (lastDash != -1 && len - lastDash < 4 && lastDash < len - 1 && Char.IsDigit(name2.Substring(lastDash + 1, 1), 0) && name2.Substring(lastDash + 1, 1) != "0")
                name2 = name2.Substring(0, lastDash);
            for (int i = 1; i <= 99; i++)
            {
                String name3 = name2 + "-" + i + extension;
                if (!IsDuplicateName(name3))
                    return name3;
            }
            return name;
        }

        public static void CheckForDuplicateName(EdgeImagePlus imp)
        {
                string name = imp.Title;
                if (IsDuplicateName(name))
                    imp.Title = GetUniqueName(name);
        }

        /// <summary>
        /// 返回对话框中文字的放大倍数
        /// </summary>
        /// <returns></returns>
        public static double GetGuiScale()
        {
            return 1;
        }

        [DllImport("user32.dll")]
        static extern short GetKeyState(Keys nVirtKey);

        public static bool IsKeyPressed(Keys testKey)
        {
            bool keyPressed = false;
            short result = GetKeyState(testKey);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled on.
                    keyPressed = false;
                    break;

                case 1:
                    // Not pressed, but toggled on
                    keyPressed = false;
                    break;

                default:
                    // Pressed (and may be toggled on)
                    keyPressed = true;
                    break;
            }

            return keyPressed;
        }

        public static Emgu.CV.Structure.MCvScalar Color2Scalar(Color color)
        {
            return new MCvScalar(color.B, color.G, color.R);
        }


        public static int[] FloatArray2Int(float[] floatArray)
        {
            // Create an integer array to store the converted values
            int[] intArray = new int[floatArray.Length];

            // Convert float array to integer array
            for (int i = 0; i < floatArray.Length; i++)
            {
                intArray[i] = (int)floatArray[i];
            }

            return intArray;
        }

        public static float[] ByteArray2Float(Byte[] byteArray)
        {
            // Create an integer array to store the converted values
            float[] floatArray = new float[byteArray.Length];

            // Convert byte array to float array
            for (int i = 0; i < floatArray.Length; i++)
            {
                floatArray[i] = (float)byteArray[i];
            }

            return floatArray;
        }

        public static Color[] ByteArray2Color(Byte[] byteArray)
        {
            // Create an integer array to store the converted values
            Color[] colorArray = new Color[byteArray.Length / 3];

            // Convert float array to integer array
            for (int i = 0; i < byteArray.Length / 3; i++)
            {
                Color color = Color.FromArgb(byteArray[i * 3], byteArray[i * 3 + 1], byteArray[i * 3 + 2]);
                colorArray[i] = color;
            }

            return colorArray;
        }

         public static GraphicsPath ConvertRegionToGraphicsPath(Region region)
        {
            GraphicsPath path = new GraphicsPath();

            using (Matrix matrix = new Matrix())
            {
                foreach (RectangleF rect in region.GetRegionScans(matrix))
                {
                    path.AddRectangle(rect);
                }
            }
            return path;
        }

       
        public static System.Drawing.Point[] Combin2ArrayToPointArray(int[] xArray, int[] yArray, int nPoints)
        {
            System.Drawing.Point[] p = new System.Drawing.Point[nPoints];
            for (int i = 0; i < nPoints; i++)
            {
                p[i].X = xArray[i];
                p[i].Y = yArray[i];
            }
            return p;

        }


        public static void ShowStatus(string message)
        {
            Form MainForm = Application.OpenForms[0];
            PublicFunctions.SendMessageToForm(MainForm, PublicConst.SHOW_STATUS, message);
        }

        public static void AddRecentMenuItem(string filename)
        {
            Form MainForm = Application.OpenForms[0];
            PublicFunctions.SendMessageToForm(MainForm, PublicConst.ADD_OPEN_RECENT_ITEM, filename);
        }


        /** Pad 'n' with leading zeros to the specified number of digits. */
        public static String Pad(int n, int digits)
        {
            String str = "" + n;
            while (str.Length < digits)
                str = "0" + str;
            return str;
        }

        

        public static double ConvertStringToDouble(string str, double defaultValue)
        {
            try
            {
                return Convert.ToDouble(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static ImageWindow GetImageWindowWithTitle(string title)
        {
            Form mainform = Application.OpenForms[0];

            int winCount = mainform.MdiChildren.Length;
            for (int i = 0; i < winCount; i++)
            {
                if (mainform.MdiChildren[i] is ImageWindow)  //如果图像窗口标题名字已经被用
                {
                    ImageWindow imageWindow = mainform.MdiChildren[i] as ImageWindow;
                    if (imageWindow.Imp.Title == title)
                    {
                        return imageWindow;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前使用的图像
        /// </summary>
        /// <returns></returns>
        public static EdgeImagePlus CurrentImagePlus()
        {
            //获取当前图像窗口
            Form mainform = Application.OpenForms[0];
            Form form = mainform.ActiveMdiChild;
            if (!(form is ImageWindow))
            {
                MessageBox.Show("请先选择一个图像窗口");
                return null;
            }

            return (form as ImageWindow).Imp;   //当前窗口中的图像
        }

        /// <summary>
        /// 将颜色转换为整形值
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int Color2Int(Color color)
        {
            return (int)(((int)color.B << 16) | (ushort)(((ushort)color.G << 8) | color.R));
        }

        /// <summary>
        /// 将整形值还原为颜色。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Int2Color(int color)
        {
            int r = 0xFF & color;
            int g = 0xFF00 & color;
            g >>= 8;
            int b = 0xFF0000 & color;
            b >>= 16;
            return Color.FromArgb(r, g, b);
        }

        static double Sqr(double x)
        {
            return x * x;
        }

        
    }
}
