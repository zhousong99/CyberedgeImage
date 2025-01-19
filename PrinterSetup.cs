namespace CyberedgeImageProcess2024
{
    public partial class PrinterSetup : Form
    {
        public PrinterSetup()
        {
            InitializeComponent();
        }
        public PrinterSetup(EdgeImagePlus imp) : this()
        {
            if (imp != null)
            {
                if (imp.Roi != null)
                {
                    this.chkPrintSelection.Enabled = true;
                }
            }
        }

        private void PrinterSetup_Load(object sender, EventArgs e)
        {

        }

        public int Scaling
        {
            get { return (int)this.numScaling.Value; }
            set { this.numScaling.Value = value; }
        }

        public bool DrawBorder
        {
            get { return this.chkDrawBorder.Checked; }
            set { chkDrawBorder.Checked = value; }
        }

        public bool Center
        {
            get { return this.chkCenter.Checked; }
            set { this.chkCenter.Checked = value; }
        }

        public bool PrintTitle
        {
            get { return this.chkPrintTitle.Checked; }
            set { this.chkPrintTitle.Checked = value; }
        }

        public bool PrintSelection
        {
            get { return this.chkPrintSelection.Checked; }
            set { this.chkPrintSelection.Checked = value; }
        }

        public bool Rotate
        {
            get { return this.chkRotate.Checked; }
            set { this.chkRotate.Checked = value; }
        }

        public bool ActualSize
        {
            get { return this.chkActualSize.Checked; }
            set { this.chkActualSize.Checked = value; }
        }
    }
}
