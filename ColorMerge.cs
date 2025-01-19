namespace CyberedgeImageProcess2024
{
    public partial class ColorMerge : Form
    {
        private static int maxChannels = 7;
        private String firstChannelName = string.Empty;
        private static String[] colors = { "red", "green", "blue", "gray", "cyan", "magenta", "yellow" };
        private bool autoFillDisabled = false;
        public ColorMerge()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            Close();
        }

        public ImageWindow[] GetWindows()
        {
            List<ImageWindow> windows = new List<ImageWindow>();
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbRed.Text));
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbGreen.Text));
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbBlue.Text));
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbGray.Text));
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbCyan.Text));
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbMagenta.Text));
            windows.Add(PublicFunctions.GetImageWindowWithTitle(cmbYellow.Text));
            return windows.ToArray();
        }

        private void ColorMerge_Load(object sender, EventArgs e)
        {
            List<string> titleList = PublicFunctions.GetAllTitle();
            if (titleList == null)
            {
                MessageBox.Show("No images are open.");
                Close();
                return;
            }



            titleList.Add("");
            List<String> nameList = GetInitialNames(titleList);
            string[] strings = titleList.ToArray();
            string[] names = nameList.ToArray();

            cmbRed.DataSource = strings.Clone();
            cmbRed.Text = names[0];
            cmbGreen.DataSource = strings.Clone();
            cmbGreen.Text = names[1];
            cmbBlue.DataSource = strings.Clone();
            cmbBlue.Text = names[2];
            cmbGray.DataSource = strings.Clone();
            cmbGray.Text = names[3];
            cmbCyan.DataSource = strings.Clone();
            cmbCyan.Text = names[4];
            cmbMagenta.DataSource = strings.Clone();
            cmbMagenta.Text = names[5];
            cmbYellow.DataSource = strings.Clone();
            cmbYellow.Text = names[6];
        }

        private List<string> GetInitialNames(List<String> titles)
        {
            List<String> names = new List<string>();
            names.Add("");
            for (int i = 0; i < maxChannels; i++)
                names.Add(GetName(i + 1, titles));
            return names;
        }

        private String GetName(int channel, List<String> titles)
        {
            if (autoFillDisabled)
                return string.Empty;
            String str = "C" + channel;
            String name = null;
            for (int i = titles.Count - 1; i >= 0; i--)
            {
                if (titles != null && titles[i].StartsWith(str) && (firstChannelName == string.Empty || titles[i].Contains(firstChannelName)))
                {
                    name = titles[i];
                    if (channel == 1)
                    {
                        if (name == null || name.Length < 3)
                            return string.Empty;
                        firstChannelName = name.Substring(3);
                    }
                    break;
                }
            }
            if (name == null)
            {
                for (int i = titles.Count - 1; i >= 0; i--)
                {
                    int index = titles[i].IndexOf(colors[channel - 1]);
                    if (titles != null && index != -1 && (firstChannelName == null || titles[i].Contains(firstChannelName)))
                    {
                        name = titles[i];
                        if (channel == 1 && index > 0)
                            firstChannelName = name.Substring(0, index - 1);
                        break;
                    }
                }
            }
            if (channel == 1 && name == null)
                autoFillDisabled = true;
            if (name != null)
                return name;
            else
                return string.Empty;
        }
    }
}
