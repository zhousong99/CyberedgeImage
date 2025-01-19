using Emgu.CV.CvEnum;

namespace CyberedgeImageProcess2024
{
    public partial class ResizeDialog : Form
    {
        public Emgu.CV.CvEnum.Inter Interpolation = Emgu.CV.CvEnum.Inter.Linear;
        public int newWidth, newHeight;
        private double xScale = 0.5, yScale = 0.5;

        private int origWidth, origHeight;

        public string NewWindowTitle
        {
            get { return txtNewWindowTitle.Text; }
            set { txtNewWindowTitle.Text = PublicFunctions.GetUniqueName(value); }
        }

        public bool BoolCreateNewWindow
        {
            get { return chkCreateNewWindow.Checked; }
        }

        public double XScale
        {
            get { return xScale; }
        }

        public double YScale
        {
            get { return yScale; }
        }
        public ResizeDialog()
        {
            InitializeComponent();
            cbInterpolation.SelectedIndex = (int)Inter.Linear;
        }

        private void cbInterpolation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Interpolation = (Emgu.CV.CvEnum.Inter)cbInterpolation.SelectedIndex;
        }

        private void chkAspect_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAspect.Checked)
            {
                UpdateTextBox();
            }
        }

        private void UpdateTextBox()
        {

            txtScaleY.Text = txtScaleX.Text;
            yScale = xScale;
            newHeight = (int)Math.Round(origHeight * yScale);
            txtHeight.Text = newHeight.ToString();
        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {
            double newXScale = xScale;
            double newYScale = yScale;

            if (sender == txtScaleX && txtScaleX.Focused)
            {
                String newXText = txtScaleX.Text;
                newXScale = PublicFunctions.ConvertStringToDouble(newXText, 0);
                if (newXScale == 0) return;
                if (newXScale != xScale)
                {
                    newWidth = (int)Math.Round(newXScale * origWidth);
                    txtWidth.Text = newWidth.ToString();
                    if (chkAspect.Checked)
                    {
                        txtScaleY.Text = txtScaleX.Text;
                        newHeight = (int)Math.Round(newXScale * origHeight);
                        txtHeight.Text = newHeight.ToString();
                        newYScale = newXScale;
                    }
                }
            }
            else if (sender == txtScaleY && txtScaleY.Focused)
            {
                String newYText = txtScaleY.Text;
                newYScale = PublicFunctions.ConvertStringToDouble(newYText, 0);
                if (newYScale == 0) return;
                if (newYScale != yScale)
                {
                    newHeight = (int)Math.Round(newYScale * origHeight);
                    txtHeight.Text = newHeight.ToString();
                    if (chkAspect.Checked)
                    {
                        txtScaleX.Text = txtScaleY.Text;
                        newWidth = (int)Math.Round(newYScale * origWidth);
                        txtWidth.Text = newWidth.ToString();
                        newXScale = newYScale;
                    }
                }
            }
            else if (sender == txtWidth && txtWidth.Focused)
            {
                newWidth = (int)Math.Round(PublicFunctions.ConvertStringToDouble(txtWidth.Text, 0.0));
                if (newWidth != 0)
                {
                    newXScale = 1.0 * newWidth / origWidth;
                    txtScaleX.Text = newXScale.ToString("F2");
                    if (chkAspect.Checked)
                    {
                        txtScaleY.Text = txtScaleX.Text;
                        newHeight = (int)Math.Round(newXScale * origHeight);
                        txtHeight.Text = newHeight.ToString();
                        newYScale = newXScale;
                    }
                }
            }
            else if (sender == txtHeight && txtHeight.Focused)
            {
                newHeight = (int)Math.Round(PublicFunctions.ConvertStringToDouble(txtHeight.Text, 0.0));
                if (newHeight != 0)
                {
                    newYScale = 1.0 * newHeight / origHeight;
                    txtScaleY.Text = newYScale.ToString("F2");
                    if (chkAspect.Checked)
                    {
                        txtScaleX.Text = txtScaleY.Text;
                        newWidth = (int)Math.Round(newYScale * origWidth);
                        txtWidth.Text = newWidth.ToString();
                        newXScale = newYScale;
                    }
                }
            }

            xScale = newXScale;
            yScale = newYScale;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkCreateNewWindow_CheckedChanged(object sender, EventArgs e)
        {
            txtNewWindowTitle.Visible = chkCreateNewWindow.Checked;
            lblNewWindowTitle.Visible = chkCreateNewWindow.Checked;
        }

        private void ResizeDialog_Load(object sender, EventArgs e)
        {

        }

        public void SetSize(int width, int height)
        {
            origWidth = width;
            origHeight = height;
            newWidth = (width / 2);
            txtWidth.Text = newWidth.ToString();
            newHeight = (height / 2);
            txtHeight.Text = newHeight.ToString();
        }
    }
}
