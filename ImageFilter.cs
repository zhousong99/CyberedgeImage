namespace CyberedgeImageProcess2024
{
    public class ImageFilter
    {
        private String arg;
        private EdgeImagePlus imp;
        private Boolean canceled;
        private FilterForm filterDialog;
        public int DialogState = 0; // 0:取消， 1非预览确定， 2预览确定

        public int KSize
        {
            get { return filterDialog.KSize; }
        }

        public float floatValue1
        {
            get { return filterDialog.floatValue1; }
        }

        public float floatValue2
        {
            get { return filterDialog.floatValue2; }
        }
        public ImageFilter(EdgeImagePlus imp, String arg)
        {
            this.imp = imp;
            arg = arg.ToLower();
            this.arg = arg;
            filterDialog = new FilterForm();
            filterDialog.SetImagePlus(imp);   //为了共享imp等信息
            filterDialog.SetCommand(arg);
            switch (arg)
            {
                case "gaussianblur":
                    filterDialog.SetIncement(2);
                    filterDialog.SetMin(3);
                    GetValue("高斯模糊", new string[] { "内核大小: ", "sigmaX", "sigmaY" }, 3, 0, 0);
                    break;
                case "medianblur":
                    filterDialog.SetIncement(2);
                    filterDialog.SetMin(3);
                    GetValue("中值滤波", new string[] { "内核大小: ", "", "" }, 3, 0, 0);
                    break;
                case "blur":
                    GetValue("均值滤波", new string[] { "内核大小: ", "", "" }, 3, 0, 0);
                    break;
                case "unsharpmask":
                    GetValue("锐化", new string[] { "内核大小: ", "权重(0.1-0.9)", "" }, 5, 0.6f, 0);
                    break;
                case "bilateral":
                    GetValue("双边滤波", new string[] { "核直径: ", "颜色方差", "空间方差" }, 15, 20, 50);
                    break;

            }

        }



        /// <summary>
        /// 想在对话框中获取数值
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <param name="digits"></param>
        private void GetValue(String title, String[] prompts, int kSize = 3, float f1 = 0, float f2 = 0)
        {
            filterDialog.Set(title, prompts, kSize, f1, f2);
            filterDialog.ShowDialog();
            SetState();
        }



        private void SetState()
        {
            if (filterDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                DialogState = 0;    //取消
            }
            else
            {
                if (filterDialog.OnPreview)
                {
                    DialogState = 2;   //确认，带预览

                }
                else
                {
                    DialogState = 1;  //确认，无预览

                }
            }
        }



    }

}
