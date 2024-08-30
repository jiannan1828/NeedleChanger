using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.IO;

using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;

namespace InjectorInspector
{
    public partial class VisionTestFrm : Form
    {
        Task Progress;

        public VisionTestFrm()
        {
            InitializeComponent();
        }

        private void 分析吸嘴影像_Click(object sender, EventArgs e)
        {
            Inspector.Vector3 dOut;
            inspector1.xInsp吸嘴_Image(new Inspector.Vector3(), out dOut);
        }

        private void 分析Tray盤影像_Click(object sender, EventArgs e)
        {
            List<Inspector.Vector3> lst;
            inspector1.xInsp震動盤_Image(out lst);
        }

        private void 分析入料盤影像_Click(object sender, EventArgs e)
        {
            inspector1.xInsp入料_Image();
        }

        private void SetupCCDTray_Click(object sender, EventArgs e)
        {
            inspector1.CCDTray.SetParam();
        }

        private void SetupCCD入料_Click(object sender, EventArgs e)
        {
            inspector1.CCD入料.SetParam();
        }

        List<string> SelectFolderImage()
        {
            List<string> Images = new List<string>();
            using(var P = new FolderBrowserDialog())
            {
                P.RootFolder = Environment.SpecialFolder.MyComputer;
                if (P.ShowDialog() == DialogResult.OK)
                {
                    var files = Directory.EnumerateFiles(P.SelectedPath);
                    foreach(var Pf in files)
                    {
                        string Lower = Pf.ToLower();
                        if (Lower.EndsWith(".bmp"))
                            Images.Add(Pf);
                        if (Lower.EndsWith(".jpg"))
                            Images.Add(Pf);
                        if (Lower.EndsWith(".tif"))
                            Images.Add(Pf);
                        if (Lower.EndsWith(".tiff"))
                            Images.Add(Pf);
                    }
                }
            }
            return Images;
        }

        private void 批次分析入料盤_Click(object sender, EventArgs e)
        {
            var FNames = SelectFolderImage();
            if ((Progress == null) || Progress.IsCompleted)
                Progress = Task.Factory.StartNew((arg) =>
                {
                    List<string> Names = arg as List<string>;
                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Maximum = Names.Count;
                        progressBar1.Value = 0;
                        progressBar1.Show();
                    }));
                    
                    foreach(var P in Names)
                    {
                        inspector1.xInsp入料_Image(P);
                        this.Invoke(new Action(() =>
                        {
                            progressBar1.Value += 1;
                        }));
                        Thread.Sleep(50);
                    }
                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Hide();
                    }));
                }, FNames);
        }

        private void 批次分析Tray盤_Click(object sender, EventArgs e)
        {
            var FNames = SelectFolderImage();
            if ((Progress == null) || Progress.IsCompleted)
                Progress = Task.Factory.StartNew((arg) =>
                {
                    List<string> Names = arg as List<string>;
                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Maximum = Names.Count;
                        progressBar1.Value = 0;
                        progressBar1.Show();
                    }));

                    foreach (var P in Names)
                    {
                        List<Inspector.Vector3> lst;
                        inspector1.xInsp震動盤_Image(P, out lst);
                        this.Invoke(new Action(() =>
                        {
                            progressBar1.Value += 1;
                        }));
                        Thread.Sleep(50);
                    }
                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Hide();
                    }));
                }, FNames);
        }

        private void 批次分析吸嘴_Click(object sender, EventArgs e)
        {
            var FNames = SelectFolderImage();
            if ((Progress == null) || Progress.IsCompleted)
                Progress = Task.Factory.StartNew((arg) =>
                {
                    List<string> Names = arg as List<string>;
                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Maximum = Names.Count;
                        progressBar1.Value = 0;
                        progressBar1.Show();
                    }));

                    foreach (var P in Names)
                    {
                        Inspector.Vector3 dOut;
                        inspector1.xInsp吸嘴_Image(P, new Inspector.Vector3(), out dOut);
                        this.Invoke(new Action(() =>
                        {
                            progressBar1.Value += 1;
                        }));
                        Thread.Sleep(50);
                    }
                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Hide();
                    }));
                }, FNames);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (AppDomain.CurrentDomain.FriendlyName != "DefaultDomain")
                Task.Factory.StartNew(WaitInit);
        }

        void WaitInit()
        {
            while (!inspector1.isInit)
                Thread.Sleep(10);
            this.BeginInvoke(new Action(() =>
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                Text = string.Format("Initial Time = {0:F2}", inspector1.T1);
            }));
        }
    }
}
