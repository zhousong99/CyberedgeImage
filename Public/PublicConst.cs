using System.Runtime.InteropServices;

namespace CyberedgeImageProcess2024
{
    static class PublicConst
    {
        public struct Yolov8Config
        {
            public string ClassifyOnnx;
            public string DetectOnnx;
            public string SegmentOnnx;
        }



        public const string Yolov8ConfigFilename = "Yolov8Config.txt";

        //要发信息数据结构，作为SendMessage函数的LParam参数
        public struct SENDDATASTRUCT
        {
            public IntPtr dwData;       //附加一些个人自定义标志信息,自己喜欢
            public int DataLength;      //信息的长度
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;       //要发送的信息
        }



        //打开的文件类型
        public const int UNKNOWN = 0;
        public const int TIFF = 1;
        public const int DICOM = 2;
        public const int FITS = 3;
        public const int PGM = 4;
        public const int JPEG = 5;
        public const int GIF = 6;
        public const int LUT = 7;
        public const int BMP = 8;
        public const int ZIP = 9;
        public const int JAVA_OR_TEXT = 10;
        public const int ROI = 11;
        public const int TEXT = 12;
        public const int PNG = 13;
        public const int TIFF_AND_DICOM = 14;
        public const int CUSTOM = 15;
        public const int AVI = 16;
        public const int OJJ = 17;
        public const int TABLE = 18;
        public const int RAW = 19;

        //工具栏上的工具
        public const int TOOL_RECTANGLE = 0;
        public const int TOOL_OVAL = 1;
        public const int TOOL_POLYGON = 2;
        public const int TOOL_FREEROI = 3;
        public const int TOOL_LINE = 4;
        public const int TOOL_POLYLINE = 5;
        public const int TOOL_FREELINE = 6;
        public const int TOOL_POINT = 7, TOOL_CROSSHAIR = 7;
        public const int TOOL_WAND = 8;
        public const int TOOL_TEXT = 9;
        public const int TOOL_UNUSED = 10;
        public const int TOOL_MAGNIFIER = 11;
        public const int TOOL_HAND = 12;
        public const int TOOL_DROPPER = 13;
        public const int TOOL_ANGLE = 14;
        //矩形有三种
        public const int RECT_ROI = 0, ROUNDED_RECT_ROI = 1, ROTATED_RECT_ROI = 2;  //不再用圆角矩形
                                                                                    //圆形有三种
        public const int OVAL_ROI = 0, ELLIPSE_ROI = 1, BRUSH_ROI = 2;


        //全局变量
        public static int MaxWidth, MaxHeight;
        public static bool ZoomIndicatorVisible = true;      //是否显示图像左上角的缩放指示器
        public static double RectToolOptions_DefaultStrokeWidth = 0.0;
        public static Color RectToolOptions_DefaultStrokeColor;
        public static int CurrentTool = TOOL_UNUSED;
        public static string CurrentToolName = string.Empty;
        public static int Rect_TYPE = RECT_ROI;
        public static int OVAL_Type = OVAL_ROI;
        public static int ArcSize = 20;    //圆角大小
        public static bool MultiPointMode = false;
        public static Color ForegRoundColor = Color.Cyan;


        //自定义消息
        public const int USER = 0x500;
        public const int ADD_OPEN_RECENT_ITEM = USER + 100;        //添加最近打开菜单
        public const int SHOW_STATUS = USER + 110;                 //在工具栏显示鼠标或其他状态


        public static Color BackgroundColor = Color.Black;
        public static Color ForegroundColor = Color.White;
    }
}
