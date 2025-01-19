using Newtonsoft.Json;


namespace CyberedgeImageProcess2024
{
    public class CustomizeMenuItem
    {
        public string MenuItemCaption { get; set; }
        public string CommandLine { get; set; }
    }



    public class Yolov8OnnxConfigStruct
    {
        public string ModelName { get; set; }
        public string OnnxFile { get; set; }
    }


    public partial class CyberedgeMain : Form
    {
        private const int MAX_OPEN_RECENT_ITEMS = 15;

        /// <summary>
        /// 根据Json文件，配置常用工具菜单
        /// </summary>
        private void ManageCommonToolsMenu()
        {
            string FileName = "CommonToolsMenu.txt";
            #region 读取配置文件
            if (!File.Exists(FileName))
            {
                string strJson = "[{\"MenuItemCaption\":\"记事本\",\"CommandLine\":\"notepad.exe\"}";
                strJson += ",{\"MenuItemCaption\":\"画图\",\"CommandLine\":\"mspaint.exe\"}"; ;
                strJson += ",{\"MenuItemCaption\":\"计算器\",\"CommandLine\":\"calc.exe\"}]"; ;
                using (File.Create(FileName)) { };
                File.WriteAllText(FileName, strJson);
            }

            string FileContent = File.ReadAllText(FileName);
            #endregion

            #region 配置菜单
            IList<CustomizeMenuItem> listMenuItem = new List<CustomizeMenuItem>();
            listMenuItem = JsonConvert.DeserializeObject<IList<CustomizeMenuItem>>(FileContent);
            foreach (CustomizeMenuItem menuInfo in listMenuItem)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(menuInfo.MenuItemCaption);
                menuItem.Tag = menuInfo.CommandLine;
                menuItem.Click += CustomizeMenuItem_Click;
                miCommonTools.DropDownItems.Add(menuItem);
            }
            #endregion
        }

        private void CustomizeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            PublicFunctions.RunExec(menuItem.Tag.ToString());
        }

        /** 增加文件路径到文件/最近打开文件子菜单最高处 */
        public void AddOpenRecentItem(String path)
        {
            if (!File.Exists(path)) { return; }
            int count = miOpenRecent.DropDownItems.Count;
            for (int i = 0; i < count;)
            {
                if (miOpenRecent.DropDownItems[i].Text.Equals(path))
                {
                    miOpenRecent.DropDownItems.RemoveAt(i);
                    count--;
                }
                else
                    i++;
            }
            if (count == MAX_OPEN_RECENT_ITEMS)
                miOpenRecent.DropDownItems.RemoveAt(MAX_OPEN_RECENT_ITEMS - 1);
            ToolStripMenuItem menuItem = new ToolStripMenuItem(path);
            menuItem.Click += RecentItemMenuItem_Click;
            miOpenRecent.DropDownItems.Insert(0, menuItem);
        }


        /// <summary>
        /// 保存最近打开图像文件名
        /// </summary>
        private void SaveRecentItem()
        {
            string fileName = "RecentItem.txt";
            int count = Math.Min(miOpenRecent.DropDownItems.Count, 10);   //最多保留10个
            if (count > 0)
            {
                if (!File.Exists(fileName))
                {
                    using (File.Create(fileName)) { };
                }
                string[] recentItems = new string[count];
                for (int i = 0; i < count; i++)
                {
                    recentItems[i] = miOpenRecent.DropDownItems[i].Text;
                }
                File.WriteAllLines(fileName, recentItems);
            }
        }

        private void RecentItemMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            Opener opener = new Opener();
            opener.Open(menuItem.Text);
            AddOpenRecentItem(menuItem.Text);
        }

        /// <summary>
        /// 加载最近打开的图像名到菜单
        /// </summary>
        private void LoadRecentItem()
        {
            string fileName = "RecentItem.txt";
            if (File.Exists(fileName))
            {
                string[] recentItems = File.ReadAllLines(fileName);

                for (int i = 0; i < recentItems.Length; i++)
                {
                    if (string.IsNullOrEmpty(recentItems[i])) continue;
                    if (!File.Exists(recentItems[i])) continue;
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(recentItems[i]);
                    menuItem.Click += RecentItemMenuItem_Click;
                    miOpenRecent.DropDownItems.Add(menuItem);
                }
            }
        }



    }
}
