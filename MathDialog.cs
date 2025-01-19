using Emgu.CV;

namespace CyberedgeImageProcess2024
{
    public partial class MathDialog : Form
    {

        private EdgeImagePlus imp;
        private Mat pictureMatSnapshot;
        private Roi roi;
        public bool OnPreview = false;
        private string command;
        private double defaultValue;
        private string defalutStringValue;
        public MathDialog()
        {
            InitializeComponent();
        }

        private void MathDialog_Load(object sender, EventArgs e)
        {
            pictureMatSnapshot = imp.GetImageMat().PictureBoxMat.Clone();
            roi = imp.Roi;
        }

        public void Set(String title, String prompt, double defaultValue, int digits)
        {
            this.Text = title;
            this.lblPrompt.Text = prompt;
            this.defaultValue = defaultValue;
            this.txtValue.Text = defaultValue.ToString();
        }

        public void Set(String title, String prompt, string defaultValue, int digits)
        {
            this.Text = title;
            this.lblPrompt.Text = prompt;
            defalutStringValue = defaultValue;
            this.txtValue.Text = defaultValue;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPreview.Checked)
            {
                OnPreview = true;
                imp.PreviewFunction(command, GetValue());
            }
            else
            {
                OnPreview = false;
                imp.ImageProcessor.Reset();
                Reset();  //简单的恢复显示图
            }
        }

        public void SetImageMath(EdgeImagePlus imp)
        {
            this.imp = imp;
        }

        private void MathDialog_Shown(object sender, EventArgs e)
        {

        }

        public void SetCommand(string command)
        {
            this.command = command;
        }

        public string GetTxtValue()
        {
            return txtValue.Text;
        }

        /// <summary>
        /// 简单恢复为原始的图像
        /// </summary>
        public void Reset()
        {
            if (pictureMatSnapshot != null)
            {
                imp.GetImageMat().PictureBoxMat = pictureMatSnapshot.Clone();
                imp.ImageWindow.Rendering();
            }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (OnPreview)
            {
                imp.ImageProcessor.Reset();
                if (command == "and" || command == "or" || command == "xor")
                {
                    imp.PreviewFunction(command, GetBinaryValue());
                }
                else
                {
                    imp.PreviewFunction(command, GetValue());
                }
            }
            else
            {
                Reset();
            }
        }

        public double GetValue()
        {
            try
            {
                return double.Parse(GetTxtValue());
            }
            catch
            {
                MessageBox.Show("必须输入数值");
                return defaultValue;
            }
        }

        public int GetBinaryValue()
        {
            try
            {
                return Convert.ToInt32(GetTxtValue(), 2);
            }
            catch
            {
                MessageBox.Show("必须输入二进制数据");
                return Convert.ToInt32("11110000", 2); ;
            }
        }
    }
}
