using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UserStartUp
{
    public partial class Form1 : Form
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();

        void UpdateCom(App app) {

            for (int i = 0; i < app.sub.Count; i++)
            {
                ToolStripMenuItem temp = new ToolStripMenuItem();

                temp.Name = app.sub[i].name+"-"+ app.sub[i].id;
                temp.Size = new Size(180, 22);
                temp.Text = app.sub[i].name;
                temp.Tag = app.sub[i].id.ToString();
                if(!app.sub[i].path.Equals(String.Empty)) temp.Click += new EventHandler(OpenProgram);
                app.sub[i].toolStripMenuItem = temp;

                if (app.sub[i].level == 0)
                {
                    contextMenuStrip1.Items.Add(temp);
                }
                else {
                    app.sub[i].up.toolStripMenuItem.DropDownItems.Add(temp);
                }

                if (app.sub[i].sub.Count > 0)
                    UpdateCom(app.sub[i]);
            }
        }
        public Form1()
        {
            InitializeComponent();

            UpdateCom(LoadData());

            GC.Collect();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Application.Restart();
        }

        private void OpenProgram(object sender, EventArgs e) {

            int id = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());

            Process.Start(dict[id]);
        }

        private App LoadData() {

            App root;

            string[] text = File.ReadAllLines(Application.StartupPath + "\\info.txt");

            App nowUpApp = new App();
            root = nowUpApp;

            for (int i = 0; i < text.Length; i++)
            {

                if (String.IsNullOrWhiteSpace(text[i]) || text[i][0].Equals('#'))
                    continue;

                string line = text[i];
                string[] split = line.Split(',');

                string name = split[0];
                string path = "";
                for (int n = 1; n < split.Length; n++)
                {
                    path += split[n];
                    if (n != split.Length - 1) path += ",";
                }

                int nowLevel = 0;
                int charIndex = 0;
                while (line[charIndex++].Equals('\t'))
                {
                    nowLevel++;
                }
                App ap = new App();
                ap.name = name;
                ap.path = path.Replace("\t", "");
                ap.level = nowLevel;
                ap.id = dict.Count;

                if (nowUpApp.level != nowLevel - 1)
                {
                    if (nowLevel == nowUpApp.level)
                    {
                        nowUpApp = nowUpApp.up;
                    }
                    else if (nowLevel < nowUpApp.level)
                    {
                        while (nowUpApp.level != nowLevel - 1)
                        {
                            nowUpApp = nowUpApp.up;
                        }
                    }
                    else {
                        nowUpApp = nowUpApp.sub[nowUpApp.sub.Count - 1];
                        ////格式出错
                        //Application.Exit();
                    }
                }

                ap.up = nowUpApp;
                nowUpApp.sub.Add(ap);
                dict.Add(ap.id, ap.path);
            }

            return root;
        }

    }

    public class App{
        public int level = -1;
        public string name;
        public string path;
        public List<App> sub = new List<App>();
        public App up;

        public int id;
        public ToolStripMenuItem toolStripMenuItem;
    }
}
