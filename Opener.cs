using Emgu.CV;

namespace CyberedgeImageProcess2024
{
    public class Opener
    {
        private static string defaultDirectory = null;
        private int fileType;
        private bool error;

        public Opener() { }

        public void Open(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "图像文件|*.bmp;*.DIB;*.jpg;*.jpeg;*.jpe;*.png;*.PBM;*.PGM;*.PPM;*.SR;*.RAS;*.tiff;*.tif;*.exr;*.jp2;*.gif;*.dcm";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Open(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 打开图像，并创建窗口显示图像（由imp.Show实现）
        /// </summary>
        /// <param name="path"></param>
        public void Open(String path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("找不到该文件，无法打开！");
            }

            EdgeImagePlus imp = OpenImage(path);
            
            if (imp == null)  return;
            if (imp != null)
            {
                PublicFunctions.CheckForDuplicateName(imp);  //查看是否有重名（一般是已经打开了一次）
                imp.Show();
            }
        }


        public EdgeImagePlus OpenImage(String path)
        {
            if (path == null || path.Equals(""))
                path = GetPath();
            if (path == null) return null;
            EdgeImagePlus imp = null;

            imp = OpenImage(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path));
            imp.Title = System.IO.Path.GetFileName(path);
            imp.Directory = System.IO.Path.GetDirectoryName(path);
            imp.Extension = System.IO.Path.GetExtension(path);
            FileInfomation fi = new FileInfomation() {
                fileFormat = PublicFunctions.GetFileType(path),
                fileName = System.IO.Path.GetFileName(path),
                directory = imp.Directory };
            imp.SetFileInfo(fi);

            return imp;
        }

        /// <summary>
        /// 打开图像文件
        /// </summary>
        /// <param name="directory">路径</param>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public EdgeImagePlus OpenImage(string directory, String name)
        {
            Emgu.CV.Mat mat = null;

            string path = directory + '\\' + name;
            fileType = PublicFunctions.GetFileType(path);
            if (fileType == PublicConst.DICOM)
            {
                return (EdgeImagePlus)(new Dicom(path));
            }
            else
            {
                mat = new Emgu.CV.Mat(path, Emgu.CV.CvEnum.ImreadModes.Unchanged);
                if (mat.NumberOfChannels == 4)
                {
                    CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgra2Bgr);
                }
                return new EdgeImagePlus(path, mat);
            }
        }

        private static string GetPath()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
