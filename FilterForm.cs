using Emgu.CV;

namespace CyberedgeImageProcess2024
{
    public partial class FilterForm : Form
    {
        private EdgeImagePlus imp;
        private Mat pictureMatSnapshot;
        private Roi roi;
        public bool OnPreview = false;
        private string command;
        private Mat image;

        public int KSize
        {
            get
            {
                return (int)this.numericSize.Value;
            }
        }

        public float floatValue1
        {
            get { return (float)PublicFunctions.ConvertStringToDouble(txt1.Text, 0.0); }
        }

        public float floatValue2
        {
            get { return (float)PublicFunctions.ConvertStringToDouble(txt2.Text, 0.0); }
        }
        public FilterForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SetImagePlus(EdgeImagePlus imp)
        {
            this.imp = imp;
        }

        public void SetCommand(string command)
        {
            this.command = command;
        }

        public void SetIncement(int value)
        {
            this.numericSize.Increment = value;
        }

        public void SetMin(int value)
        {
            this.numericSize.Minimum = value;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 简单恢复为原始的图像
        /// </summary>
        public void Reset()
        {
            imp.ImageProcessor.Reset();
            imp.ImageProcessor.CreateImage();
            if (pictureMatSnapshot != null)
            {
                imp.GetImageMat().PictureBoxMat = pictureMatSnapshot.Clone();
                imp.ImageWindow.Rendering();
            }
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            pictureMatSnapshot = imp.GetImageMat().PictureBoxMat.Clone();
            roi = imp.Roi;
            image = imp.ImageProcessor.CreateImage();
        }

        public void Set(String title, String[] prompts, int kSize = 3, float f1 = 0, float f2 = 0)
        {
            Label[] labels = new Label[prompts.Length];
            this.Text = title;
            this.label1.Text = prompts[0];
            this.numericSize.Value = kSize;

            if (prompts[1] != string.Empty)
            {
                this.label2.Text = prompts[1];
                this.txt1.Text = f1.ToString();
            }
            else
            {
                this.label2.Visible = false;
                this.txt1.Visible = false;
            }
            if (prompts[2] != string.Empty)
            {
                this.label3.Text = prompts[2];
                this.txt2.Text = f2.ToString();
            }
            else
            {
                this.label3.Visible = false;
                this.txt2.Visible = false;
            }

        }

        private void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            OnPreview = chkPreview.Checked;

            numericSize_ValueChanged(sender, e);
        }

        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            Reset();
            if (OnPreview)
            {
                imp.PreviewFunction(command, KSize, null, floatValue1, floatValue2);
            }
            else
            {
                //Reset();
            }
        }

        private void numericSize_Leave(object sender, EventArgs e)
        {

        }
    }
}
