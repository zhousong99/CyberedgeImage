namespace CyberedgeImageProcess2024
{
    public class ImageMath
    {
        private String arg;
        private EdgeImagePlus imp;
        private Boolean canceled;
        private double lower = -1.0, upper = -1.0;
        private String macro2;
        private MathDialog mathDialog;

        private const double defaultAddValue = 25;
        private const double defaultMulValue = 1.25;
        private const double defaultMinValue = 0;
        private const double defaultMaxValue = 255;
        private const String defaultAndValue = "11110000";
        private const double defaultGammaValue = 0.5;

        private double addValue = defaultAddValue;
        private double mulValue = defaultMulValue;
        private double minValue = defaultMinValue;
        private double maxValue = defaultMaxValue;
        private String andValue = defaultAndValue;
        private double defaultValue;

        public int DialogState = 0; // 0:取消， 1非预览确定， 2预览确定

        public ImageMath(EdgeImagePlus imp, String arg)
        {
            this.imp = imp;
            arg = arg.ToLower();
            this.arg = arg;
            mathDialog = new MathDialog();
            mathDialog.SetImageMath(imp);   //为了共享imp等信息
            mathDialog.SetCommand(arg);
            switch (arg)
            {
                case "add":
                    GetValue("加", "值: ", addValue, 0);
                    break;
                case "subtract":
                    GetValue("减", "值: ", addValue, 0);
                    break;
                case "multiply":
                    GetValue("乘", "值: ", mulValue, 0);
                    break;
                case "divide":
                    GetValue("除", "值: ", mulValue, 0);
                    break;
                case "set":
                    GetValue("设置", "值: ", addValue, 0);
                    break;
                case "minimum":
                    GetValue("最小", "值: ", minValue, 0);
                    break;
                case "maximum":
                    GetValue("最大", "值: ", maxValue, 0);
                    break;
                case "gamma":
                    GetValue("伽马", "值0-5: ", defaultGammaValue, 0);
                    break;
                case "and":
                    GetValue("与", "二进制值: ", andValue, 0);
                    break;
                case "or":
                    GetValue("或", "二进制值: ", andValue, 0);
                    break;
                case "xor":
                    GetValue("异或", "二进制值: ", andValue, 0);
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
        private void GetValue(String title, String prompt, double defaultValue, int digits)
        {
            this.defaultValue = defaultValue;
            mathDialog.Set(title, prompt, defaultValue, digits);
            mathDialog.ShowDialog();
            SetState();
        }

        private void GetValue(String title, String prompt, string defaultValue, int digits)
        {
            mathDialog.Set(title, prompt, defaultValue, digits);
            mathDialog.ShowDialog();
            SetState();
        }

        private void SetState()
        {
            if (mathDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                DialogState = 0;    //取消
            }
            else
            {
                if (mathDialog.OnPreview)
                {
                    DialogState = 2;   //确认，带预览

                }
                else
                {
                    DialogState = 1;  //确认，无预览

                }
            }
        }


        public double Value()
        {
            return mathDialog.GetValue();
        }

        public int BinaryValue()
        {
            return mathDialog.GetBinaryValue();
        }
    }
}
