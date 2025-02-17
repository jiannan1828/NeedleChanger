using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;
using Camera;
using System.Reflection;
using System.IO;
using xNet;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.MonthCalendar;

#pragma warning disable CS0168
#pragma warning disable CS0169

namespace Inspector
{
    public partial class Inspector : UserControl
    {
        internal bool DebugMode = true; //右鍵選單顯示
        public bool xEngineer = true;   //右鍵選單顯示
        public InspParameter parameter = new InspParameter();
        Insp料倉區 Insp料倉;
        InspTray區 InspTray;
        public Insp吸嘴區 InspNozzle;
        InspSocket區 InspSocket;
        InspCCD5區 Insp夾爪CCD;
        InspCCD6區 Insp吸針孔;
        Exlite OPT;
        bool ExitP = false;
        ushort[] DT = new ushort[30000];
        List<Vector3> TrayItem;
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer() { Interval = 200, Enabled = true };
        /// <summary> 針目前角度 </summary>
        public double PinDeg = 0;
        /// <summary> 吸嘴取像張數 </summary>
        public int RecvCount = 0;
        /// <summary> 吸嘴已分析 </summary>
        public bool Inspected = false;
        /// <summary> 吸嘴分析結果成功 </summary>
        public bool InspectOK = false;
        public Action on下視覺;
        public Func<string, double> getParam;

        public int lights = 120;

        public double nozzleX, nozzleY;
        public double 移載X, 移載Y;
        public int 缺料警告數量 = 10;
        public bool 下視覺正向 = true;

        int GetDWord(int Index)
        {
            uint nV = DT[Index + 1] * 65536U + DT[Index];
            return (int)nV;
        }
        void SetDWord(int Index, int Value)
        {
            uint nV = (uint)Value;
            DT[Index] = (ushort)(nV % 65536);
            DT[Index + 1] = (ushort)(nV / 65536);
        }

        public bool TimeOut(double baseTim, double range) { return (NowTime - baseTim) > range; }
        public double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        public bool ActionIdle(Task _Task) { return (_Task == null) || _Task.IsCompleted || _Task.IsFaulted || _Task.IsCanceled; }
        /// <summary>Class轉為XML Byte[]</summary>
        public byte[] SerializeXML<T>(T Obj)
        {
            XmlSerializer XML = new XmlSerializer(Obj.GetType());
            MemoryStream fs = new MemoryStream();
            XML.Serialize(fs, Obj);
            return fs.ToArray();
        }
        /// <summary>Byte[]轉為Class</summary>
        public T DeSerializeXML<T>(byte[] Buf)
        {
            using (MemoryStream fs = new MemoryStream(Buf))
            {
                XmlSerializer XML = new XmlSerializer(typeof(T));
                return (T)XML.Deserialize(fs);
            }
        }

        public Inspector()
        {
            AppDomain.CurrentDomain.AssemblyResolve += onResolve;   //在 InitializeComponent 之前
            InitializeComponent();
        }
        /// <summary>程式庫不再目前目錄，嘗試解析載入</summary>
        Assembly onResolve(object sender, ResolveEventArgs args)
        {
            string FName = "";
            //if (args.Name.StartsWith("ThridLibray"))
            //{
            //    string MVROOT = Environment.GetEnvironmentVariable("MV_GENICAM_64");
            //    if (MVROOT != null)
            //    {
            //        var n1 = MVROOT.Split(new string[] { "MV Viewer" }, StringSplitOptions.None);
            //        FName = n1.First() + @"MV Viewer\Development\DOTNET DLL\IMV\DOTNET_4.0\x64\ThridLibray.dll";
            //    }
            //}
            if (args.Name.StartsWith("halcondotnetxl"))
            {
                string HALCONROOT = Environment.GetEnvironmentVariable("HALCONROOT");
                if (HALCONROOT != null)
                    FName = HALCONROOT + @"\bin\dotnet35\halcondotnetxl.dll";
            }
            if (args.Name.StartsWith("MvCamCtrl.Net"))
            {
                string MVROOT = Environment.GetEnvironmentVariable("MVCAM_COMMON_RUNENV");
                if (MVROOT != null)
                    FName = MVROOT + @"\DotNet\AnyCpu\MvCamCtrl.Net.dll";
            }
            return ((FName != "") && File.Exists(FName)) ? Assembly.LoadFrom(FName) : null;
        }

        /// <summary>初始化，執行一次</summary>
        public void xInit()
        {
            WriteLog("Init");
            Insp料倉 = new Insp料倉區(this, Win3);
            InspTray = new InspTray區(this, Win2);
            InspNozzle = new Insp吸嘴區(this, Win1);
            InspSocket = new InspSocket區(this, Win5);
            Insp夾爪CCD = new InspCCD5區(this, Win6);
            Insp吸針孔 = new InspCCD6區(this, Win4);
            OPT = new Exlite(2);
            OPT.Open("COM1", 1);
            OPT.Lights[0] = 120;
            Task.Factory.StartNew(Scan);
            System.Windows.Forms.Application.ApplicationExit += onExit;
            timer1.Tick += onTick1;
            LoadRecipe(0);
        }

        void onExit(object sender, EventArgs e)
        {
            ExitP = true;
        }

        void Scan()
        {
            byte[] Buf = new byte[100];
            byte ECode = 0;
            bool PreConnected = false;
            while (!ExitP)
            {
                Thread.Sleep(2);
            }
        }

        void onTick1(object sender, EventArgs e)
        {
            Insp料倉.Show();
            InspTray.Show();
            InspNozzle.Show();
            InspSocket.Show();
            Insp夾爪CCD.Show();
            Insp吸針孔.Show();
            OPT.Lights[0] = lights;
        }

        public void LoadRecipe(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<int>(LoadRecipe), Num);
            else
            {
                string dPath = string.Format("{0}\\Recipe\\{1}.XML", System.Windows.Forms.Application.StartupPath, Num);
                if (File.Exists(dPath))
                    parameter = DeSerializeXML<InspParameter>(File.ReadAllBytes(dPath));

                Read吸嘴參數();
                ReadTray參數();

                num_Pin寬Min.Value = (decimal)parameter.Pin寬度Min;
                num_Pin寬Max.Value = (decimal)parameter.Pin寬度Max;
                num_Pin長Min.Value = (decimal)parameter.Pin長度Min;
                num_Pin長Max.Value = (decimal)parameter.Pin長度Max;
                num_Throshold.Value = (int)parameter.TrayThreshold;
                ed_針頭長.Text = parameter.針頭長度.ToString("F2");
                ed_針頭寬.Text = parameter.針頭寬度.ToString("F2");
                ed_針尾長.Text = parameter.針尾長度.ToString("F2");
                ed_針尾寬.Text = parameter.針尾寬度.ToString("F2");
                string dPath2 = string.Format("{0}\\Recipe\\{1}.model", System.Windows.Forms.Application.StartupPath, Num);
                //if (File.Exists(dPath2))
                //    HOperatorSet.ReadShapeModel(dPath2, out InspNozzle.model);
            }
        }

        internal void Read吸嘴參數()
        {
            double V = getParam("NeedleHeadLength");  //針頭長
            if (Math.Abs(V) > 0.01) { 
                parameter.針頭長度 = V;
            }

            V = getParam("NeedleHeadWidth");  //針頭寬
            if (Math.Abs(V) > 0.01) { 
                parameter.針頭寬度 = V;
            }

            V = getParam("NeedleTailLength");  //針尾長
            if (Math.Abs(V) > 0.01) { 
                parameter.針尾長度 = V;
            }

            V = getParam("NeedleTailWidth");  //針尾寬
            if (Math.Abs(V) > 0.01) { 
                parameter.針尾寬度 = V;
            }
        }

        internal void ReadTray參數()
        {
            double V = getParam("NeedleLengthMax");  //針長Max
            if (Math.Abs(V) > 0.01) { 
                parameter.Pin長度Max = V;
            }

            V = getParam("NeedleLengthMin");  //針長Min
            if (Math.Abs(V) > 0.01) { 
                parameter.Pin長度Min = V;
            }

            V = getParam("NeedleWidthMax");  //針寬Max
            if (Math.Abs(V) > 0.01) { 
                parameter.Pin寬度Max = V;
            }

            V = getParam("NeedleWidthMin");  //針寬Min
            if (Math.Abs(V) > 0.01) { 
                parameter.Pin寬度Min = V;
            }

            V = getParam("NeedleThreshold");  //閥值
            if (Math.Abs(V) > 0.01) { 
                parameter.TrayThreshold = (int)V;
            }
        }

        public void SaveRecipe(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<int>(LoadRecipe), Num);
            else
            {
                string dPath = string.Format("{0}\\Recipe\\{1}.XML", System.Windows.Forms.Application.StartupPath, Num);
                string dPath2 = string.Format("{0}\\Recipe\\{1}.model", System.Windows.Forms.Application.StartupPath, Num);
                string dDir = Path.GetDirectoryName(dPath);
                try
                {
                    if (!Directory.Exists(dDir))
                        Directory.CreateDirectory(dDir);
                    File.WriteAllBytes(dPath, SerializeXML(parameter));
                    //if (InspNozzle.model != null)
                    //    HOperatorSet.WriteShapeModel(InspNozzle.model, dPath2);
                }
                catch { }
            }
        }

        /// <summary>設定持續取像</summary>
        public void xFreeRun(params CCDName[] items)
        {
            string Msg = "";
            if (items.Contains(CCDName.ALL))
            {
                xFreeRun(CCDName.入料, CCDName.震動盤, CCDName.吸嘴, CCDName.Socket, CCDName.CCD5, CCDName.CCD6);
                Msg = "入料, 震動盤, 吸嘴, Socket, CCD5, CCD6";
            }
            else
            {
                foreach (var P in items)
                {
                    if (P == CCDName.入料) Insp料倉.CCD.TriggerMode = "FreeRun";
                    if (P == CCDName.震動盤) InspTray.CCD.TriggerMode = "FreeRun";
                }
                Msg = string.Join(", ", items.Select(x => x.ToString()));
            }
            WriteLog("FreeRun " + Msg);
        }
        /// <summary>設定停止取像</summary>
        public void xStop(params CCDName[] items)
        {
            string Msg = "";
            if (items.Contains(CCDName.ALL))
            {
                xStop(CCDName.入料, CCDName.震動盤, CCDName.吸嘴, CCDName.Socket, CCDName.CCD5, CCDName.CCD6);
                Msg = "入料, 震動盤, 吸嘴, Socket, CCD5, CCD6";
            }
            else
            {
                foreach (var P in items)
                {
                    if (P == CCDName.入料) Insp料倉.CCD.TriggerMode = "Stop";
                    if (P == CCDName.震動盤) InspTray.CCD.TriggerMode = "Stop";
                }
                Msg = string.Join(", ", items.Select(x => x.ToString()));
            }
            WriteLog("Stop " + Msg);
        }
        /// <summary>分析入料盤，回覆有無料</summary>
        public bool xInsp入料() { return Insp料倉.Insp(); }
        /// <summary>分析震動盤，回覆針位置，無料時回覆 false</summary>
        public bool xInsp震動盤(out List<Vector3> target) { return InspTray.Insp(out target); }

        public bool xCarb震動盤(out PointF Pos) { return InspTray.Carb(out Pos); }

        public bool xCarb震動盤二孔(out PointF Pos, out double deg) { return InspTray.Carb2Hole(out Pos, out deg); }
        /// <summary>分析吸嘴，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool xInsp吸嘴(out double targetθ) 
        {
            return InspNozzle.Insp(out targetθ);
        }
        /// <summary>吸嘴校正初始化，會將分析資料輸出為像素位置</summary>
        public void xCarbInit吸嘴() { InspNozzle.CarbInit(); }
        /// <summary>吸嘴校正，依序傳入 0度X1Y1 / 90度X1Y1 / 90度X2Y1 / 90度X2Y2</summary>
        public void xCarb吸嘴(List<Vector3> ImageLoc, List<Vector3> axisLoc) { InspNozzle.Carb(ImageLoc, axisLoc); }
        /// <summary>分析Socket盤，，傳入目前Socket軸位置(X / Y)，回覆針孔位置</summary>
        public bool xInspSocket(out Vector3 Loc) { return InspSocket.Insp(out Loc); }

        public bool xInspSocket校正孔(out Vector3 Loc) { return InspSocket.校正孔尋找(out Loc); }

        public bool xInspSocket植針後檢查() { return InspSocket.植針後Check(); }
        /// <summary>Socket盤校正初始化，會將分析資料輸出為像素位置</summary>
        public void xCarbInitSocket() { InspSocket.CarbInit(); }
        /// <summary>吸嘴校正，依序傳入Socket分析後第一筆資料 X1Y1 / X2Y1 / X2Y2</summary>
        public void xCarbSocket(List<Vector3> ImageLoc, List<Vector3> axisLoc) { InspSocket.Carb(ImageLoc, axisLoc); }
        /// <summary>未知作用</summary>
        public bool xInsp夾爪(out Vector3 pos) { return Insp夾爪CCD.Insp(out pos); }
        /// <summary>未知作用</summary>
        public bool xInsp吸針孔(out Vector3 pos) { return Insp吸針孔.Insp(out pos); }

        internal void DisposeObj(params HObject[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
                if (arg[i] != null)
                    arg[i].Dispose();
        }

        internal HObject RemoveBorder(HObject regn, HTuple W, HTuple H)
        {
            HObject right, left, top, outRegion;
            HOperatorSet.SelectShape(regn, out right, "column2", "and", 5, W - 100);    //右
            HOperatorSet.SelectShape(right, out left, "column1", "and", 100, W - 5);    //左

            HOperatorSet.SelectShape(left, out top, "row1", "and", 100, H - 5);         //上
            HOperatorSet.SelectShape(top, out outRegion, "row2", "and", 5, H - 200);    //下
            DisposeObj(right, left, top);
            return outRegion;
        }

        public void 入料CCD設定_DoubleClick(object sender, EventArgs e)
        {
            Insp料倉.CCD.SetParam();
        }

        public void TrayCCD設定_DoubleClick(object sender, EventArgs e)
        {
            InspTray.CCD.SetParam();
        }

        public void 吸嘴CCD設定_DoubleClick(object sender, EventArgs e)
        {
            InspNozzle.CCD.SetParam();
        }

        internal void WriteLog(string Msg)
        {
            try
            {
                DateTime now = DateTime.Now;
                string FName = string.Format("D:\\Images\\{0:yyyyMMdd}\\Inspect.Log", now);
                string FDir = Path.GetDirectoryName(FName);
                if (!Directory.Exists(FDir))
                    Directory.CreateDirectory(FDir);
                File.AppendAllText(FName, string.Format("{0:HH:mm:ss.f}\t {1}\r\n", now, Msg));
            }
            catch
            {
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            InspNozzle.Teach();
        }

        private void ed_針頭長_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (((TextBox)sender).Name)
            {
                case "ed_針頭長":
                    double.TryParse(((TextBox)sender).Text, out parameter.針頭長度);
                    break;
                case "ed_針頭寬":
                    double.TryParse(((TextBox)sender).Text, out parameter.針頭寬度);
                    break;
                case "ed_針尾長":
                    double.TryParse(((TextBox)sender).Text, out parameter.針尾長度);
                    break;
                case "ed_針尾寬":
                    double.TryParse(((TextBox)sender).Text, out parameter.針尾寬度);
                    break;
            }
        }

        internal void WriteImage(HObject img, string subDirectory, string title)
        {
            try
            {
                string FolderDirection;
                if(PinDeg >= 0) {
                    FolderDirection = "正向";
                } else {
                    FolderDirection = "反向";
                }

                string FName = string.Format("D:\\Images\\{0:yyyyMMdd}\\{1}\\{4}\\{0:HHmmssff}-{2},{3}.JPG", DateTime.Now, subDirectory, title, PinDeg, FolderDirection);
                string FDir = Path.GetDirectoryName(FName);
                if (!Directory.Exists(FDir))
                    Directory.CreateDirectory(FDir);
                HOperatorSet.WriteImage(img, "jpeg 70", 0, FName);
            }
            catch(Exception ex)
            {
            }
        }

        public void num_Pin長Min_ValueChanged(object sender, EventArgs e)
        {
            if (sender == num_Pin寬Min)
                parameter.Pin寬度Min = (double)num_Pin寬Min.Value;
            if (sender == num_Pin寬Max)
                parameter.Pin寬度Max = (double)num_Pin寬Max.Value;
            if (sender == num_Pin長Min)
                parameter.Pin長度Min = (double)num_Pin長Min.Value;
            if (sender == num_Pin長Max)
                parameter.Pin長度Max = (double)num_Pin長Max.Value;
            if (sender == num_Throshold)
                parameter.TrayThreshold = (int)num_Throshold.Value;
        }
    }

    public enum CCDName : int { ALL, 入料, 震動盤, 吸嘴, Socket, CCD5, CCD6 }

    #region Vector3
    public struct Vector3
    {
        public double X;
        public double Y;
        public double θ;
        public static implicit operator PointF(Vector3 a) => new PointF((float)a.X, (float)a.Y);
        public static implicit operator Vector3(PointF a) => new Vector3() { X = a.X, Y = a.Y, θ = 0 };
    }
    #endregion

    #region HWindowHelper
    public class HWindowHelper
    {
        RectangleF view = new Rectangle(0, 0, 5120, 5120), FullView;
        RectangleF tempView = new RectangleF(0, 0, 5120, 5120);
        PointF tempP1 = new PointF(0, 0), tempP2 = new PointF(0, 0);
        internal bool inShow = false;
        float scale = 1.0f;
        public int ImgWidth = 5120, ImgHeight = 5120;
        internal HWindowControl hWin;
        HObject dImage;
        public HObject Image
        {
            get { return dImage; }
            set
            {
                if (dImage != null)
                    dImage.Dispose();
                dImage = value;
            }
        }

        public Action<HWindowControl, HObject> ShowResult;

        public HWindowHelper(HWindowControl owner)
        {
            hWin = owner;
            hWin.HMouseWheel += hWin1_MouseWheel;
            hWin.MouseDown += hWin1_MouseDown;
            hWin.MouseMove += hWin1_MouseMove;
            HOperatorSet.GenEmptyObj(out dImage);
        }

        public bool ContainImage
        {
            get
            {
                if (Image == null)
                    return false;
                HTuple W, H;
                HOperatorSet.GetImageSize(Image, out W, out H);
                return W.Length > 0;
            }
        }

        void hWin1_MouseWheel(object sender, HMouseEventArgs e)
        {
            float cX = (view.Left + view.Right) / 2;
            float cY = (view.Top + view.Bottom) / 2;
            scale = Math.Max(0.002f, scale * ((e.Delta > 0) ? 1.25f : 0.8f));
            scale = Math.Min(1.5f, scale);
            float dW = FullView.Width * scale;
            float dH = FullView.Height * scale;
            //float dW = view.Width * ((e.Delta > 0) ? 1.25f : 0.8f) / 2;
            //float dH = view.Height * ((e.Delta > 0) ? 1.25f : 0.8f) / 2;
            float X1 = cX - dW;
            float Y1 = cY - dH;
            float X2 = Math.Max(X1 + 5, cX + dW);
            float Y2 = Math.Max(Y1 + 5, cY + dH);
            view = RectangleF.FromLTRB(X1, Y1, X2, Y2);
            doResult();
        }

        public void doResult()
        {
            hWin.SuspendLayout();
            hWin.HalconWindow.ClearWindow();
            float dBottom = Math.Max(view.Top + 1, view.Bottom - 1);
            float dRight = Math.Max(view.Right + 1, view.Right - 1);
            hWin.HalconWindow.SetPart(view.Top, view.Left, dBottom, dRight);
            if (ContainImage)
                ShowResult?.Invoke(hWin, Image);
            hWin.ResumeLayout();
            inShow = false;
        }

        void hWin1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float X = tempView.Width / hWin.Width * e.X + tempView.Left;
                float Y = tempView.Height / hWin.Height * e.Y + tempView.Top;
                tempP2 = new PointF(X, Y);
                if (!inShow)
                {
                    inShow = true;
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(50);
                        RectangleF temp = tempView;
                        temp.X = temp.X + tempP1.X - tempP2.X;
                        temp.Y = temp.Y + tempP1.Y - tempP2.Y;
                        view = temp;
                        hWin.BeginInvoke(new Action(doResult));
                    });
                }
            }
        }

        void hWin1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tempView = view;
                float X = tempView.Width / hWin.Width * e.X + tempView.Left;
                float Y = tempView.Height / hWin.Height * e.Y + tempView.Top;
                tempP1 = new PointF(X, Y);
            }
        }

        public void AdjustView()
        {
            HTuple ImW, ImH;
            HOperatorSet.GetImageSize(Image, out ImW, out ImH);
            ImgWidth = ImW;
            ImgHeight = ImH;
            double dW = hWin.Width / 1.0 / ImgWidth;
            double dH = hWin.Height / 1.0 / ImgHeight;
            double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, Temp = 0;
            if (dW < dH)
            {
                X1 = 0;
                X2 = ImgWidth - 1;
                Temp = hWin.Height / dW;
                Y1 = ImgHeight / 2.0 - (Temp / 2);
                Y2 = Y1 + Temp;
            }
            else
            {
                Y1 = 0;
                Y2 = ImgHeight - 1;
                Temp = hWin.Width / dH;
                X1 = ImgWidth / 2.0 - (Temp / 2);
                X2 = X1 + Temp;
            }
            scale = 1.0f;
            view = RectangleF.FromLTRB((float)X1, (float)Y1, (float)X2, (float)Y2);
            FullView = view;
            doResult();
        }

        public void SetImageSize(HObject img)
        {
            HTuple width, height;
            try
            {
                HOperatorSet.GetImageSize(img, out width, out height);
                ImgWidth = width;
                ImgHeight = height;
            }
            catch (Exception ex)
            { }
        }

        public void LoadImage()
        {
            using (var P = new OpenFileDialog())
            {
                P.Filter = "Images | *.bmp;*.jpg;*.tiff;*.tif";
                inShow = true;
                if (P.ShowDialog() == DialogResult.OK)
                {
                    HOperatorSet.ReadImage(out dImage, P.FileName);
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(250);
                        AdjustView();
                    });
                }
                else
                    inShow = false;
            }
        }

    }

    #endregion

    #region InspParameter
    public class InspParameter
    {
        public int TrayThreshold = 180;
        public double Pin寬度Min = 1;
        public double Pin寬度Max = 80;
        public double Pin長度Min = 1;
        public double Pin長度Max = 70;
        public double Pin相鄰距離 = 40;
        public bool 下視覺特徵為針頭 = false;
        public double 針頭長度 = 0.5;
        public double 針頭寬度 = 0.11;
        public double 針尾長度 = 0.5;
        public double 針尾寬度 = 0.09;
    }
    #endregion

    #region 料倉
    public class Insp料倉區
    {
        HWindowHelper helper;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        bool first = true, ItemSuccess;
        Inspector owner;
        HObject binregion, CheckRegion;

        public Insp料倉區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new ImageSource("CCD入料", "FreeRun", "./", 5, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            helper.Image = temp;
            helper.SetImageSize(temp);
            if (first)
            {
                first = false;

                helper.AdjustView();
            }
        }

        void SetMenu()
        {
            menu = new ContextMenuStrip();
            helper.hWin.ContextMenuStrip = menu;
            menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            menu.Items.Add("分析入料盤", null, (sender, e) => { Insp(); });
            menu.Items.Add("Save Image", null, (sender, e) => { owner.WriteImage(helper.Image, "料倉", "料倉"); });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            items.First(x => x.Text == "分析入料盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }

        HObject BlockArea(HObject image)
        {
            HObject thRegion, connRegion, fillRegion, unionRegion, openRegion, erRegion;
            HOperatorSet.Threshold(image, out thRegion, 0, 60);
            HOperatorSet.Connection(thRegion, out connRegion);
            HOperatorSet.FillUp(connRegion, out fillRegion);
            HOperatorSet.Union1(fillRegion, out unionRegion);
            HOperatorSet.OpeningRectangle1(unionRegion, out openRegion, 1500, 600);
            HOperatorSet.ErosionRectangle1(openRegion, out erRegion, 500, 1050);
            owner.DisposeObj(thRegion, connRegion, fillRegion, unionRegion, openRegion);
            return erRegion;
        }

        HObject GetAllPin(HObject image, HObject checkarea)
        {
            HObject lightRegion, pinArea, conn, LimW, LimH, uniArea;
            HOperatorSet.Threshold(image, out lightRegion, 30, 255);
            HOperatorSet.Intersection(lightRegion, checkarea, out pinArea);
            double w = owner.parameter.Pin寬度Min / CCD.Param.ScaleX / 2.0;
            double h = owner.parameter.Pin長度Min / CCD.Param.ScaleX / 2.0;
            HOperatorSet.Connection(pinArea, out conn);
            HOperatorSet.SelectShape(conn, out LimW, "rect2_len2", "and", w, 9999999);
            HOperatorSet.SelectShape(LimW, out LimH, "rect2_len1", "and", h, 1000);
            HOperatorSet.Union1(LimH, out uniArea);
            owner.DisposeObj(lightRegion, pinArea, conn, LimW, LimH);
            return uniArea;
        }

        bool CheckArea(HObject region)
        {
            HTuple row, col, area;
            HOperatorSet.AreaCenter(binregion, out area, out row, out col);
            double w = (owner.parameter.Pin寬度Max + owner.parameter.Pin寬度Min) * 0.4;
            double h = (owner.parameter.Pin長度Max + owner.parameter.Pin長度Min) * 0.4;
            double One = w * h / CCD.Param.ScaleX / CCD.Param.ScaleX;
            double V = (area.Length > 0) ? area.D / One : 0;
            var result = V > owner.缺料警告數量;
            return result;
        }

        /// <summary>分析入料盤，回覆有無料</summary>
        public bool Insp(bool manualInsp = true)
        {
            HObject Region入料 = null;
            bool result = false;
            if (helper.ContainImage)
            {
                HObject temp;
                HTuple threshold, W, H, row, col, area;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "料倉", "料倉");
                HOperatorSet.GetImageSize(temp, out W, out H);
                CheckRegion = BlockArea(temp);
                binregion = GetAllPin(temp, CheckRegion);
                result = CheckArea(binregion);
                ItemSuccess = result;
                SaveResult(temp, result);
                owner.DisposeObj(temp, Region入料);
            }
            owner.BeginInvoke(new Action(helper.AdjustView));
            return result;
        }

        void SaveResult(HObject img, bool success)
        {
            var imgColor = Compose3(img);
            HObject mix1, mix2 = null;
            HOperatorSet.PaintRegion(CheckRegion, imgColor, out mix1, new HTuple(255, 165, 0), "margin");   //orange
            if (success)
            {
                HOperatorSet.PaintRegion(binregion, mix1, out mix2, new HTuple(0, 255, 0), "fill"); //green
                owner.WriteImage(mix2, "料倉", "料倉Find");
            }
            else
            {
                if ((binregion != null) && (binregion.CountObj() == 1))
                {
                    HOperatorSet.PaintRegion(binregion, mix1, out mix2, new HTuple(255, 0, 0), "fill"); //red
                    owner.WriteImage(mix2, "料倉", "料倉Find");
                }
                else
                    owner.WriteImage(mix1, "料倉", "料倉Find");
            }

            owner.DisposeObj(mix1, mix2);
        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            if (CheckRegion != null)
            {
                Win.HalconWindow.SetDraw("margin");
                Win.HalconWindow.SetColor("orange");
                HOperatorSet.DispRegion(CheckRegion, Win.HalconWindow);
                if (binregion != null)
                {
                    Win.HalconWindow.SetDraw("fill");
                    if (ItemSuccess)
                        Win.HalconWindow.SetColor("green");
                    else
                        Win.HalconWindow.SetColor("red");
                    HOperatorSet.DispRegion(binregion, Win.HalconWindow);
                }
            }
        }

        public void Show()
        {
            helper.doResult();
        }

        HObject Compose3(HObject img)
        {
            HObject img2, img3, imgColor;
            HOperatorSet.CopyImage(img, out img2);
            HOperatorSet.CopyImage(img, out img3);
            HOperatorSet.Compose3(img, img2, img3, out imgColor);
            owner.DisposeObj(img2, img3);
            return imgColor;
        }

    }
    #endregion

    #region Tray
    public class InspTray區
    {
        HWindowHelper helper;
        HObject RegionTray, RegionMeas, PinArea, MeasPin, CarbArea, WhiteArea;
        HTuple rowTray, colTray, rowDist, colDist, rowPin, colPin;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        bool first = true;
        double LimitL1, LimitL2;
        Inspector owner;
        int AddL1 = 15;
        PointF CarbPos, CarbH;
        PointF 校正點一 = new PointF(), 校正點二 = new PointF(), 吸嘴中心 = new PointF();
        HTuple CarbDeg = 0.0;

        public InspTray區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new ImageSource("CCDTray", "FreeRun", "./", 5, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            HImage temp1 = temp.RotateImage(90.0, "constant");
            HImage temp2 = temp1.RotateImage(-0.72, "constant");
            owner.DisposeObj(temp, temp1);
            helper.Image = temp2;
            helper.SetImageSize(temp2);
            if (first)
            {
                first = false;
                helper.AdjustView();
            }
        }

        void SetMenu()
        {
            menu = new ContextMenuStrip();
            helper.hWin.ContextMenuStrip = menu;
            menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            menu.Items.Add("分析震動盤", null, (sender, e) =>
            {
                List<Vector3> target;
                Insp(out target);
            });
            menu.Items.Add("分析校正一", null, (sender, e) =>
            {
                Carb(out 校正點一);
                校正點一.X = (float)owner.nozzleX - 校正點一.X;
                校正點一.Y = (float)owner.nozzleY - 校正點一.Y;
                HOperatorSet.AngleLx(校正點一.Y, 校正點一.X, 校正點二.Y, 校正點二.X, out CarbDeg);
                CarbDeg = CarbDeg.TupleDeg();
            });
            menu.Items.Add("分析校正二", null, (sender, e) =>
            {
                Carb(out 校正點二);
                校正點二.X = (float)owner.nozzleX - 校正點二.X;
                校正點二.Y = (float)owner.nozzleY - 校正點二.Y;
                HOperatorSet.AngleLx(校正點一.Y, 校正點一.X, 校正點二.Y, 校正點二.X, out CarbDeg);
                CarbDeg = CarbDeg.TupleDeg();
            });
            menu.Items.Add("校正二孔", null, (sender, e) =>
            {
                double nDeg = 0;
                PointF dCenter;
                Carb2Hole(out dCenter, out nDeg);
                吸嘴中心.X = (float)owner.nozzleX - dCenter.X;
                吸嘴中心.Y = (float)owner.nozzleY - dCenter.Y;
                //HOperatorSet.AngleLx(校正點一.Y, 校正點一.X, 校正點二.Y, 校正點二.X, out CarbDeg);
                CarbDeg = nDeg;
            });
            menu.Items.Add("Save Image", null, (sender, e) => { owner.WriteImage(helper.Image, "料盤", "料盤"); });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Items.Add("Save Recipe Default", null, (sender, e) => { owner.SaveRecipe(0); });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }

        HObject GetWhiteArea(HObject image)
        {
            HObject ThRegion, TransRegion, eroRegion;
            HOperatorSet.Threshold(image, out ThRegion, 128, 255);
            HOperatorSet.ShapeTrans(ThRegion, out TransRegion, "convex");
            HOperatorSet.ErosionRectangle1(TransRegion, out eroRegion, 800, 550);
            owner.DisposeObj(ThRegion, TransRegion);
            return eroRegion;
        }

        HObject GetAllPin(HObject img, HObject whiteArea, double WMin, double WMax, double HMin, double HMax)
        {
            HObject redImage, meanImg, dynRegion, cloRegion, conn, SelInner, LimH, LimW;
            HOperatorSet.ReduceDomain(img, whiteArea, out redImage);
            HOperatorSet.MeanImage(redImage, out meanImg, 9, 9);
            HOperatorSet.DynThreshold(redImage, meanImg, out dynRegion, 5, "dark");
            HOperatorSet.ClosingCircle(dynRegion, out cloRegion, WMin);
            HOperatorSet.Connection(cloRegion, out conn);
            HOperatorSet.SelectShape(conn, out SelInner, "inner_radius", "and", WMin, 99999);
            HOperatorSet.SelectShape(SelInner, out LimH, "rect2_len1", "and", HMin, 99999);
            HOperatorSet.SelectShape(LimH, out LimW, "rect2_len2", "and", WMin, 99999);
            owner.DisposeObj(redImage, meanImg, dynRegion, cloRegion, conn, SelInner, LimH);
            return LimW;
        }

        HObject GetNormalPin(HObject selectRegion, double WMin, double WMax, double HMin, double HMax)
        {
            HObject LimH, LimW;
            HOperatorSet.SelectShape(selectRegion, out LimH, "rect2_len1", "and", HMin, HMax);
            HOperatorSet.SelectShape(LimH, out LimW, "rect2_len2", "and", WMin, WMax);
            owner.DisposeObj(LimH);
            return LimW;
        }

        HObject RemoveNear(HObject normalpin, HObject allPin, HObject whiteArea)
        {
            HTuple Cnt, Num, area, row, col, phi, L1, L2, row1, col1;
            HTuple rowStart, colStart, rowEnd, colEnd;
            HObject outRgn, fAllPin, SelPin;
            HOperatorSet.SmallestRectangle1(whiteArea, out rowStart, out colStart, out rowEnd, out colEnd);
            HOperatorSet.Union1(allPin, out fAllPin);
            HOperatorSet.GenEmptyObj(out outRgn);
            HOperatorSet.CountObj(normalpin, out Cnt);
            LimitL1 = Math.Max(5.5 / 2, owner.parameter.Pin長度Max / 2) / CCD.Param.ScaleX;
            LimitL2 = 4.0 / 2 / CCD.Param.ScaleX;
            HTuple outSel = new HTuple();
            if (Cnt > 0)
            {
                HOperatorSet.SmallestRectangle2(normalpin, out row, out col, out phi, out L1, out L2);
                for (int i = 0; i < Cnt; i++)
                {
                    HObject tempPin = null, intrArea = null, rc, rc2, SelTemp = null;
                    HOperatorSet.SelectObj(normalpin, out SelPin, i + 1);
                    HOperatorSet.GenRectangle2(out rc, row[i], col[i], phi[i], LimitL1, LimitL2);
                    HOperatorSet.SelectShape(rc, out rc2,
                        new HTuple("row1", "row2", "column1", "column2"),
                        "and",
                        new HTuple(rowStart.D + 5, rowStart.D + 5, colStart.D + 5, colStart.D + 5),
                        new HTuple(rowEnd.D - 5, rowEnd.D - 5, colEnd.D - 5, colEnd.D - 5));
                    if (rc2.CountObj() > 0)
                    {
                        HOperatorSet.Difference(fAllPin, SelPin, out tempPin);
                        HOperatorSet.Intersection(tempPin, rc2, out intrArea);
                        HOperatorSet.SelectShape(intrArea, out SelTemp, "area", "and", 2, 999999);
                        HOperatorSet.AreaCenter(SelTemp, out area, out row1, out col1);
                        if (area.Length == 0)
                            outSel.Append(i + 1);
                    }
                    owner.DisposeObj(tempPin, intrArea, rc, rc2, SelTemp);
                }
                if (outSel.Length > 0)
                    HOperatorSet.SelectObj(normalpin, out outRgn, outSel);
            }
            HOperatorSet.SmallestRectangle2(outRgn, out rowPin, out colPin, out phi, out L1, out L2);
            for(int i = 0; i < outSel.Length; i++)
            {
                L1[i] = LimitL1;
                L2[i] = LimitL2;
            }
            RegionTray = null;
            if (outSel.Length > 0)
                HOperatorSet.GenRectangle2(out RegionTray, rowPin, colPin, phi, L1, L2);
            return outRgn;
        }


        HObject GetPinNoOverlap(HObject img, out HObject DarkArea)
        {
            HObject rgn1, ConnectionArea, SelectedRegions, rgnFill, rgnConv, rgnMax;
            HTuple row, col, L1, L2, phi, _Max, _area;
            HOperatorSet.Threshold(img, out DarkArea, 0, owner.parameter.TrayThreshold);
            HOperatorSet.Connection(DarkArea, out ConnectionArea);
            HOperatorSet.AreaCenter(ConnectionArea, out _area, out row, out col);
            HOperatorSet.SelectShape(ConnectionArea, out ConnectionArea, "area", "and", 80, 999999);
            HOperatorSet.Union1(ConnectionArea, out DarkArea);
            HOperatorSet.SelectShape(ConnectionArea, out SelectedRegions, "rect2_len2", "and", owner.parameter.Pin寬度Min, owner.parameter.Pin寬度Max);
            HOperatorSet.FillUp(SelectedRegions, out rgnFill);
            HOperatorSet.SelectShape(rgnFill, out rgnConv, "convexity", "and", 0.55, 2);
            HOperatorSet.SmallestRectangle2(rgnConv, out row, out col, out phi, out L1, out L2);
            if (L1.Length > 0)
                HOperatorSet.SelectShape(rgnFill, out rgnMax, "rect2_len1", "and", owner.parameter.Pin長度Min, owner.parameter.Pin長度Max);
            else
                HOperatorSet.CopyObj(rgnFill, out rgnMax, 1, 1);
            owner.DisposeObj(ConnectionArea, SelectedRegions, rgnFill, rgnConv);
            return rgnMax;
        }

        //HObject RemoveNear(HObject img, HObject pin, HObject allPin)
        //{
        //    HTuple Cnt, Num, row, col, phi, L1, L2;
        //    HObject outRgn;
        //    HOperatorSet.GenEmptyObj(out outRgn);
        //    HOperatorSet.SmallestRectangle2(pin, out row, out col, out phi, out L1, out L2);
        //    for (int i = 0; i < L2.Length; i++)
        //    {
        //        L2[i] = L2[i].D + owner.parameter.Pin相鄰距離;
        //        L1[i] = L1[i].D + AddL1;
        //    }
        //    if (L2.Length > 0)
        //    {
        //        HOperatorSet.GenRectangle2(out RegionMeas, row, col, phi, L1, L2);
        //        for (int i = 1; i <= L2.Length; i++)
        //        {
        //            HObject Sel, Intr, Connection, PinSel;
        //            HOperatorSet.SelectObj(RegionMeas, out Sel, i);
        //            HOperatorSet.SelectObj(pin, out PinSel, i);
        //            HOperatorSet.Intersection(allPin, Sel, out Intr);
        //            HOperatorSet.Connection(Intr, out Connection);
        //            HOperatorSet.CountObj(Connection, out Cnt);
        //            if (Cnt == 1)
        //                HOperatorSet.ConcatObj(outRgn, PinSel, out outRgn);
        //        }
        //        HOperatorSet.CountObj(outRgn, out Cnt);
        //    }
        //    else
        //        RegionMeas = null;

        //    return outRgn;
        //}

        HTuple CheckDirection(HTuple row, HTuple col, HTuple phi, HTuple dist, ref HTuple outRow, ref HTuple outCol)
        {
            HTuple deg = phi.TupleDeg();
            if (row.Length > 2)
            {
                if (dist.DArr.First() > dist.DArr.Last())
                {
                    deg = deg.D + 180;
                    deg = (Math.Abs(deg.D) < Math.Abs(deg.D - 360)) ? deg.D : deg.D - 360;
                    outRow.Append(row.DArr.Last());
                    outCol.Append(col.DArr.Last());
                }
                else
                {
                    outRow.Append(row.DArr.First());
                    outCol.Append(col.DArr.First());
                }
            }
            else
                if (row.Length > 0)
            {
                outRow.Append(row.DArr.First());
                outCol.Append(col.DArr.First());
            }
            return deg.TupleRad();
        }

        void MeasurePin(HObject image, HObject pin, HTuple W, HTuple H, out HTuple row, out HTuple col, out HTuple phi)
        {
            HObject outPin, SelPin;
            HTuple L1, L2, amp, dist;
            row = col = phi = new HTuple();
            if (pin == null)
                return;
            HOperatorSet.SmallestRectangle2(pin, out row, out col, out phi, out L1, out L2);
            if (L1.Length > 0)
            {
                HTuple r = new HTuple(), c = new HTuple(), rTemp, cTemp, index = new HTuple();
                HTuple rDir = new HTuple(), cDir = new HTuple();
                for (int i = 0; i < L1.Length; i++)
                {
                    HTuple measure;
                    HOperatorSet.GenMeasureRectangle2(row[i], col[i], phi[i], L1[i] + AddL1, L2[i] + 8, W, H, "nearest_neighbor", out measure);
                    HOperatorSet.MeasurePos(image, measure, 1.0, 15, "all", "all", out rTemp, out cTemp, out amp, out dist);
                    if (rTemp.Length > 0)
                    {
                        r.Append(rTemp);
                        c.Append(cTemp);
                    }
                    phi[i] = CheckDirection(rTemp, cTemp, phi[i], dist, ref rDir, ref cDir);
                    HOperatorSet.CloseMeasure(measure);
                }
                rowDist = r;
                colDist = c;
                rowPin = rDir;
                colPin = cDir;
            }
        }
        /// <summary>分析震動盤，回覆針位置，無料時回覆 false</summary>
        public bool Insp(out List<Vector3> target, bool manualInsp = true)
        {
            target = new List<Vector3>();
            if (helper.ContainImage)
            {
                owner.ReadTray參數();
                HObject temp, SelectedRegions, SelectPins;
                HTuple threshold, W, H, phi, L1, L2, deg;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "料盤", "料盤");
                HOperatorSet.GetImageSize(temp, out W, out H);

                WhiteArea = GetWhiteArea(temp);
                double WMin = owner.parameter.Pin寬度Min / CCD.Param.ScaleX / 2.0;
                double WMax = owner.parameter.Pin寬度Max / CCD.Param.ScaleX / 2.0;
                double HMin = owner.parameter.Pin長度Min / CCD.Param.ScaleX / 2.0;
                double HMax = owner.parameter.Pin長度Max / CCD.Param.ScaleX / 2.0;
                var AllPin = GetAllPin(temp, WhiteArea, WMin, WMax, HMin, HMax);
                var NormalPin = GetNormalPin(AllPin, WMin, WMax, HMin, HMax);
                PinArea = RemoveNear(NormalPin, AllPin, WhiteArea);
                SaveResult(temp, WhiteArea, PinArea, RegionTray);
                owner.DisposeObj(AllPin, NormalPin);

                //SelectedRegions = GetPinNoOverlap(temp, out PinArea);
                //SelectPins = owner.RemoveBorder(SelectedRegions, W, H);
                //RegionTray = RemoveNear(temp, SelectPins, PinArea);
                MeasurePin(temp, RegionTray, W, H, out rowTray, out colTray, out phi);
                if (phi.Length > 0)
                {
                    deg = phi.TupleDeg();
                    for (int i = 0; i < rowTray.Length; i++)
                    {
                        var P = CCD.GetReal(colTray[i].D, rowTray[i].D, W, H);
                        target.Add(new Vector3() { X = P.X, Y = P.Y, θ = -deg[i].D });
                    }
                    //owner.WriteImage(temp, "料盤", "料盤");
                }
                owner.DisposeObj(temp, AllPin, NormalPin);
            }
            owner.BeginInvoke(new Action(Show));
            return target.Count > 0;
        }

        void SaveResult(HObject img, HObject white, HObject pin, HObject blocked)
        {
            var imgColor = Compose3(img);
            HObject mix1, mix2, mix3 = null;
            HOperatorSet.PaintRegion(white, imgColor, out mix1, new HTuple(255, 165, 0), "margin");   //orange
            HOperatorSet.PaintRegion(pin, mix1, out mix2, new HTuple(0, 255, 0), "fill"); //green
            if (blocked != null)
            {
                HOperatorSet.PaintRegion(blocked, mix2, out mix3, new HTuple(255, 165, 0), "margin");
                owner.WriteImage(mix3, "料盤", "料盤Find");
            }
            else
                owner.WriteImage(mix2, "料盤", "料盤Find");
            //orange
            //HOperatorSet.PaintRegion(CheckRegion, imgColor, out mix1, new HTuple(255, 165, 0), "margin");   //orange
            //if (success)
            //    HOperatorSet.PaintRegion(binregion, mix1, out mix2, new HTuple(0, 255, 0), "fill"); //green
            //else
            //    HOperatorSet.PaintRegion(binregion, mix1, out mix2, new HTuple(255, 0, 0), "fill"); //red

            owner.DisposeObj(mix1, mix2, mix3);
        }

        public bool Carb(out PointF Pos)
        {
            bool success = false;
            Pos = new PointF(0, 0);
            if (helper.ContainImage)
            {
                HObject temp, region, connArea, Sel1, Sel2;
                HTuple usedT, row, col, rad, W, H;
                HOperatorSet.CopyImage(helper.Image, out temp);
                HOperatorSet.GetImageSize(temp, out W, out H);
                HOperatorSet.BinaryThreshold(temp, out region, "max_separability", "light", out usedT);
                HOperatorSet.Connection(region, out connArea);
                HOperatorSet.SelectShape(connArea, out Sel1, "circularity", "and", 0.9, 1);
                HOperatorSet.SelectShape(Sel1, out Sel2, "outer_radius", "and", 45, 65);
                HOperatorSet.SelectShapeStd(Sel2, out CarbArea, "max_area", 70);
                HOperatorSet.SmallestCircle(CarbArea, out row, out col, out rad);
                if (row.Length == 1)
                {
                    CarbH = new PointF((float)col.D, (float)row.D);
                    CarbPos = Pos = CCD.GetReal(col.D, row.D, W, H);
                    success = true;
                }
            }
            return success;
        }

        public bool Carb2Hole(out PointF Center, out double Deg)
        {
            bool success = false;
            Center = new PointF(0, 0);
            Deg = 0;
            if (helper.ContainImage)
            {
                HObject temp, region, connArea, Sel1, Sel2, dSort;
                HTuple usedT, row, col, rad, W, H;
                HOperatorSet.CopyImage(helper.Image, out temp);
                HOperatorSet.GetImageSize(temp, out W, out H);
                HOperatorSet.BinaryThreshold(temp, out region, "max_separability", "light", out usedT);
                HOperatorSet.Connection(region, out connArea);
                HOperatorSet.SelectShape(connArea, out Sel1, "circularity", "and", 0.9, 1);
                HOperatorSet.SelectShape(Sel1, out Sel2, "outer_radius", "and", 45, 65);
                HOperatorSet.SortRegion(Sel2, out dSort, "first_point", "true", "column");
                HOperatorSet.SmallestCircle(dSort, out row, out col, out rad);
                if (row.Length == 2)
                {
                    var Carb1 = new PointF((float)col.DArr[0], (float)row.DArr[0]);
                    var Carb2 = new PointF((float)col.DArr[1], (float)row.DArr[1]);
                    double dx = (Carb1.X - Carb2.X) / 10.0 * 14.5;
                    double dy = (Carb1.Y - Carb2.Y) / 10.0 * 14.5;
                    double tx = Carb1.X + dx;
                    double ty = Carb1.Y + dy;
                    CarbH = new PointF((float)tx, (float)ty);
                    CarbPos = Center = CCD.GetReal(tx, ty, W, H);
                    HTuple ndeg;
                    HOperatorSet.AngleLx(Carb1.Y, Carb1.X, Carb2.Y, Carb2.X, out ndeg);
                    Deg = ndeg.TupleDeg().D;
                    //CarbH = new PointF((float)col.D, (float)row.D);
                    //CarbPos = Pos = CCD.GetReal(col.D, row.D, W, H);
                    success = true;
                }
            }
            return success;
        }
        void ShowResult(HWindowControl Win, HObject image)
        {
            if (image == null)
                return;
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            if (WhiteArea != null)
            {
                Win.HalconWindow.SetDraw("margin");
                Win.HalconWindow.SetColor("orange");
                HOperatorSet.DispRegion(WhiteArea, Win.HalconWindow);
            }
            if (PinArea != null)
            {
                Win.HalconWindow.SetDraw("margin");
                Win.HalconWindow.SetColor("green");
                HOperatorSet.DispRegion(PinArea, Win.HalconWindow);
                Win.HalconWindow.SetColor("orange");
                if (RegionTray != null)
                    HOperatorSet.DispRegion(RegionTray, Win.HalconWindow);
            }
            HTuple W, H;
            if (image != null)
            {
                Win.HalconWindow.SetColor("green");
                HOperatorSet.GetImageSize(image, out W, out H);
                var LStyle = Win.HalconWindow.GetLineStyle();
                Win.HalconWindow.SetLineStyle(7);
                Win.HalconWindow.DispLine(H.I / 2.0, 0, H.I / 2, W.I - 1);
                Win.HalconWindow.DispLine(0, W.I / 2.0, H.I - 1, W.I / 2.0);
                Win.HalconWindow.SetLineStyle(LStyle);
            }

            if (owner.ck_NuCarb.Checked)
                HOperatorSet.DispCross(Win.HalconWindow, CarbH.Y, CarbH.X, 60, 0);

            return;

            if (RegionTray != null)
            {
                Win.HalconWindow.SetDraw("margin");
                Win.HalconWindow.SetColor("orange");
                HOperatorSet.DispRegion(RegionTray, Win.HalconWindow);
                    HOperatorSet.DispRegion(RegionMeas, Win.HalconWindow);
                Win.HalconWindow.SetColor("green");
                HOperatorSet.DispCross(Win.HalconWindow, rowTray, colTray, 200, 0);
                if (rowTray.Length > 0)
                    HOperatorSet.DispCircle(Win.HalconWindow, rowTray.DArr[0], colTray.DArr[0], 120);
                if (owner.ck_Measure.Checked && (rowDist != null) && (colDist != null))
                    HOperatorSet.DispCross(Win.HalconWindow, rowDist, colDist, 30, 0);
                if (owner.ck_Measure.Checked && (rowPin != null) && (colPin != null))
                {
                    Win.HalconWindow.SetColor("orange");
                    HOperatorSet.DispCross(Win.HalconWindow, rowPin, colPin, 30, 0);
                }
            }
            if (owner.ck_NuCarb.Checked && (CarbArea != null))
            {
                Win.HalconWindow.SetColor("orange");
                HOperatorSet.DispCross(Win.HalconWindow, CarbH.Y, CarbH.X, 60, 0);
                Win.HalconWindow.SetTposition(60, 250);
                string Msg = string.Format("X={0:F3} Y= {1:F3}, Deg= {2:F2}", CarbPos.X, CarbPos.Y, CarbDeg.D);
                Win.HalconWindow.WriteString(Msg);
                Win.HalconWindow.SetColor("green");
                Win.HalconWindow.DispLine(0, helper.ImgWidth / 2.0, helper.ImgHeight - 1, helper.ImgWidth / 2.0);
                Win.HalconWindow.DispLine(helper.ImgHeight / 2.0, 0, helper.ImgHeight / 2.0, helper.ImgWidth - 1);
            }
        }

        public void Show()
        {
            helper.doResult();
        }

        HObject Compose3(HObject img)
        {
            HObject img2, img3, imgColor;
            HOperatorSet.CopyImage(img, out img2);
            HOperatorSet.CopyImage(img, out img3);
            HOperatorSet.Compose3(img, img2, img3, out imgColor);
            owner.DisposeObj(img2, img3);
            return imgColor;
        }

    }
    #endregion

    #region 吸嘴
    public class Insp吸嘴區
    {
        HWindowHelper helper;
        HObject RegionNozzle;
        ContextMenuStrip menu;
        public SisoFGArea CCD;
        Inspector owner;
        bool first = true, InspOK = false;
        HTuple P1x, P1y, P2x, P2y;
        HObject PinArea, dTop, dBot;
        public HTuple model;
        /// <summary> 針目前角度 </summary>
        public double PinDeg = 0;
        public int RecvCount = 0;
        /// <summary> 已分析 </summary>
        public bool Inspected = false;
        /// <summary> 分析結果成功 </summary>
        public bool InspectOK = false;
        
        public Insp吸嘴區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new SisoFGArea("CCDNozzle", "FRONT_GPI_0", "./", 9, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            helper.Image = temp;
            helper.SetImageSize(temp);
            if (first)
            {
                first = false;
                helper.AdjustView();
            }
            owner.RecvCount = RecvCount = (RecvCount + 1) % 1000;
            if (CCD.TriggerMode.Contains("GPI"))
            {
                double Temp = 0;
                Insp(out Temp);
            }
            if (owner.on下視覺 != null)
                owner.on下視覺();
        }

        void SetMenu()
        {
            menu = new ContextMenuStrip();
            helper.hWin.ContextMenuStrip = menu;
            menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            menu.Items.Add("分析吸嘴", null, (sender, e) =>
            {
                double target;
                Insp(out target);
            });
            menu.Items.Add("Save Image", null, (sender, e) => { owner.WriteImage(helper.Image, "吸嘴", "吸嘴"); });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Items.Add("Stop CCD", null, (sender, e) => { CCD.TriggerMode = "Stop"; });
            menu.Items.Add("Start CCD", null, (sender, e) => { CCD.TriggerMode = "FRONT_GPI_0"; });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            items.First(x => x.Text == "分析吸嘴").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }
        /// <summary>分析吸嘴，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool Insp(out double targetθ)
        {
            bool success = false;
            targetθ = 0;
            if (helper.ContainImage)
            {
                owner.Read吸嘴參數();
                if ((owner.parameter.針頭長度 < 0.01) || (owner.parameter.針頭寬度 < 0.01) || (owner.parameter.針尾長度 < 0.01) || (owner.parameter.針尾寬度 < 0.01))
                {
                    owner.parameter.針頭長度 = owner.parameter.針尾長度 = 0.5;
                    owner.parameter.針頭寬度 = 0.11;
                    owner.parameter.針尾寬度 = 0.09;
                }
                HObject temp;
                HTuple W, H, Cnt, darea, dAngle, dScore;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "吸嘴", "吸嘴");
                HOperatorSet.GetImageSize(temp, out W, out H);
                double WMin = owner.parameter.Pin寬度Min / CCD.Param.ScaleX / 2.0;
                double WMax = owner.parameter.Pin寬度Max / CCD.Param.ScaleX / 2.0;
                double HMin = owner.parameter.Pin長度Min / CCD.Param.ScaleX / 2.0;
                double HMax = owner.parameter.Pin長度Max / CCD.Param.ScaleX / 2.0;
                PinArea = GetNozzleArea(temp, 70, WMin, WMax, HMin, HMax);
                RegionNozzle = FilterPin(PinArea, WMin, WMax, HMin, HMax);
                if ((RegionNozzle.CountObj() == 1))
                    try
                    {
                        HObject reduces, rcTop,rcBot;
                        HTuple row1, row2, col1, col2, phi1, phi2, row, col, L1, L2, L3, L4, TRow, TCol, BRow, BCol;
                        HOperatorSet.ReduceDomain(temp, RegionNozzle, out reduces);
                        HOperatorSet.AreaCenter(RegionNozzle, out darea, out P1y, out P1x);
                        HOperatorSet.SmallestRectangle1(RegionNozzle, out row1, out col1, out row2, out col2);

                        double LenMin = Math.Min(owner.parameter.針尾長度, owner.parameter.針頭長度) * 0.75;
                        double dLen = LenMin / CCD.Param.ScaleX;
                        HOperatorSet.GenRectangle1(out rcTop, row1, col1, row1.D + dLen, col2);
                        HOperatorSet.Intersection(RegionNozzle, rcTop, out dTop);
                        HOperatorSet.SmallestRectangle2(dTop, out TRow, out TCol, out phi1, out L1, out L2);
                        HOperatorSet.GenRectangle1(out rcBot, row2.D - dLen, col1, row2.D, col2);
                        HOperatorSet.Intersection(RegionNozzle, rcBot, out dBot);
                        HOperatorSet.SmallestRectangle2(dBot, out BRow, out BCol, out phi1, out L3, out L4);
                        owner.WriteLog(string.Format("Top = {0:F2} , Bot = {1:F2} , Len = {2:F2}", L2.D * CCD.Param.ScaleX * 2, L4.D * CCD.Param.ScaleX * 2, (row2.D - row1.D) * CCD.Param.ScaleX));
                        bool same1 = L2.D > L4.D;
                        int PinS = (int)(owner.parameter.針頭寬度 * 100);
                        int PinE = (int)(owner.parameter.針尾寬度 * 100);
                        bool same2 = PinS > PinE;
                        if (PinS != PinE)
                        {
                            P2x = (same1 == same2) ? TCol : BCol;
                            P2y = (same1 == same2) ? TRow : BRow;
                        }


                        //if ((model != null) && (model.Length > 0))
                        //    HOperatorSet.FindShapeModel(reduces,model, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 0, 0.9, out P2y, out P2x, out dAngle, out dScore);
                        success = (P1x.Length == 1) && (P2y != null) && (P2y.Length == 1);
                        if (success)
                        {
                            HOperatorSet.AngleLx(P1y, P1x, P2y, P2x, out dAngle);
                            var dAngle1 = dAngle.TupleDeg();
                            //if (!owner.parameter.下視覺特徵為針頭)
                            //    dAngle1 = dAngle1.D + 180;
                            owner.PinDeg = PinDeg = targetθ = dAngle1;
                        }
                        owner.DisposeObj(reduces);
                    }
                    catch (Exception ex) { }

                //PinArea = GetPinArea(temp, W, H);
                //HOperatorSet.CountObj(PinArea, out Cnt);
                //if (Cnt == 1)
                //{
                //    owner.PinDeg = PinDeg = targetθ = CheckDirection(temp, PinArea, W, H);
                //    success = true;
                //}
                //else
                //    success = false;
                SaveResult(temp, success);
                owner.DisposeObj(temp);
                owner.InspectOK = InspectOK = success;
                owner.Inspected = Inspected = true;
            }
            owner.BeginInvoke(new Action(Show));
            InspOK = success;
            return success;
        }

        HObject GetNozzleArea(HObject image, double threshold, double WMin, double WMax, double HMin, double HMax)
        {
            HObject Border, region, RegionUnion, RegionClosing, ConnectedRegions, LimH, LimW, NozzleArea;
            HOperatorSet.ThresholdSubPix(image, out Border, threshold);
            HOperatorSet.GenRegionContourXld(Border, out region, "filled");
            HOperatorSet.Union1(region, out RegionUnion);
            HOperatorSet.ClosingRectangle1(RegionUnion, out RegionClosing, 5, (int)WMax);
            HOperatorSet.Connection(RegionClosing, out ConnectedRegions);
            HOperatorSet.SelectShape(ConnectedRegions, out LimH, "rect2_len1", "and", HMin, 99999);
            HOperatorSet.SelectShape(LimH,out LimW, "rect2_len2", "and", WMin, 99999);
            HOperatorSet.ClosingCircle(LimW, out NozzleArea, WMax);
            //var Border = image.ThresholdSubPix(threshold);
            //var region = Border.GenRegionContourXld("filled");
            //var RegionUnion = region.Union1();
            //var RegionClosing = RegionUnion.ClosingRectangle1(5, (int)WMax);
            //var ConnectedRegions = RegionClosing.Connection();
            //var LimH = ConnectedRegions.SelectShape("rect2_len1", "and", HMin, 99999);
            //var LimW = LimH.SelectShape("rect2_len2", "and", WMin, 99999);
            //var NozzleArea = LimW.ClosingCircle(WMax);
            owner.DisposeObj(Border, region, RegionUnion, RegionClosing, ConnectedRegions, LimH, LimW);
            return NozzleArea;
        }

        HObject FilterPin(HObject pin, double WMin, double WMax, double HMin, double HMax)
        {
            HObject union, LimH, LimW;
            HOperatorSet.Union1(pin, out union);
            HOperatorSet.SelectShape(union,out LimH, "rect2_len1", "and", HMin, HMax);
            HOperatorSet.SelectShape(LimH,out LimW, "rect2_len2", "and", WMin, WMax);
            //var union = pin.Union1();
            //var LimH = union.SelectShape("rect2_len1", "and", HMin, HMax);
            //var LimW = LimH.SelectShape("rect2_len2", "and", WMin, WMax);
            owner.DisposeObj(union, LimH);
            return LimW;
        }


        void SaveResult(HObject img, bool success)
        {
            string dir = (owner.下視覺正向) ? "吸嘴正向" : "吸嘴反向";
            if (success)
            {
                var imgColor = Compose3(img);
                HObject mix1, mix2, mix3;
                HOperatorSet.PaintRegion(PinArea, imgColor, out mix1, new HTuple(0, 255, 0), "margin");   //green
                owner.WriteImage(mix1, dir, "吸嘴-OK");
                owner.DisposeObj(mix1, imgColor);
            }
            else
            {
                owner.WriteImage(img, dir, "吸嘴-NG");
            }
        }

        HObject GetPinArea(HObject image, HTuple W, HTuple H)
        {
            HObject fullRegion, region, RegionClosing, ConnectedRegions, SelectedCenterW;
            HObject SelectedCenterH, result, regionRC2, redImage;
            HTuple _Min, _Max, _Range, row, col, phi, L1, L2, usedT;
            double wMin, wMax, hMin, hMax;
            wMin = owner.parameter.Pin寬度Min / CCD.Param.ScaleX / 2;
            wMax = owner.parameter.Pin寬度Max / CCD.Param.ScaleX / 2;
            hMin = owner.parameter.Pin長度Min / CCD.Param.ScaleX / 2;
            hMax = owner.parameter.Pin長度Max / CCD.Param.ScaleX / 2;
            HOperatorSet.Threshold(image, out region, 100, 255);
            HOperatorSet.ClosingRectangle1(region, out RegionClosing, 12, wMax);
            HOperatorSet.Connection(RegionClosing, out ConnectedRegions);
            HOperatorSet.SelectShape(ConnectedRegions, out SelectedCenterH, "rect2_len1", "and", hMin, hMax);
            HOperatorSet.SelectShape(SelectedCenterH, out SelectedCenterW, "rect2_len2", "and", wMin, wMax);
            //HOperatorSet.GetDomain(image, out fullRegion);
            //HOperatorSet.MinMaxGray(fullRegion, image, 0, out _Min, out _Max, out _Range);
            //HOperatorSet.SmallestRectangle2(region, out row, out col, out phi, out L1, out L2);
            //HOperatorSet.GenRectangle2(out regionRC2, row, col, phi, L1.D * 1.8, L2);
            //HOperatorSet.ReduceDomain(image, regionRC2, out redImage);
            //HOperatorSet.Threshold(redImage, out region, 70, 255);
            //HOperatorSet.BinaryThreshold(redImage, out region, "max_separability", "light", out usedT);
            //HOperatorSet.ClosingRectangle1(region, out RegionClosing, 1, 20);
            //HOperatorSet.Connection(RegionClosing, out ConnectedRegions);
            //HOperatorSet.SelectShape(ConnectedRegions, out SelectedCenterW, "column", "and", (W / 2) - 800, (W / 2) + 800);
            //HOperatorSet.SelectShape(SelectedCenterW, out SelectedCenterH, "row", "and", (H / 2) - 500, (H / 2) + 500);
            //HOperatorSet.SelectShape(SelectedCenterH, out SelectedCenterH, "height", "and", 600, 99999);
            //HOperatorSet.SelectShapeStd(SelectedCenterH, out result, "max_area", 70);
            owner.DisposeObj(region, RegionClosing, ConnectedRegions, SelectedCenterH);
            return SelectedCenterW;
        }

        double CheckDirection(HObject image, HObject area, HTuple W, HTuple H)
        {
            HTuple Deg = 0, row, col, phi, L1, L2, MeasureHandle;
            HTuple RowEdge, ColumnEdge, Amplitude, Distance;
            HOperatorSet.SmallestRectangle2(area, out row, out col, out phi, out L1, out L2);
            if (L1.Length == 0)
                return 0;
            HOperatorSet.GenMeasureRectangle2(row, col, phi, L1 + 5, L2 + 5, W, H, "nearest_neighbor", out MeasureHandle);
            HOperatorSet.MeasurePos(image, MeasureHandle, 1, 20, "all", "all", out RowEdge, out ColumnEdge, out Amplitude, out Distance);
            Deg = phi.TupleDeg();
            if (ColumnEdge.Length == 0)
                return 0;
            P1x = ColumnEdge.DArr.FirstOrDefault();
            P1y = RowEdge.DArr.FirstOrDefault();
            P2x = ColumnEdge.DArr.LastOrDefault();
            P2y = RowEdge.DArr.LastOrDefault();
            if ((Distance.Length > 1) && (Distance[0] > Distance[Distance.Length - 1]))
            {
                Deg = Deg.D + 180;
                P1x = ColumnEdge.DArr.LastOrDefault();
                P1y = RowEdge.DArr.LastOrDefault();
                P2x = ColumnEdge.DArr.FirstOrDefault();
                P2y = RowEdge.DArr.FirstOrDefault();
            }
            if (Math.Abs(Deg.D) > Math.Abs(Deg.D - 360))
                Deg = Deg.D - 360;
            HOperatorSet.CloseMeasure(MeasureHandle);
            return Deg.D;
        }

        void CreateCapCircle(HObject image, HObject cirRegion, out HObject outCaliperXLD, out HObject outCircleXLD, HTuple direction, HTuple Len1, HTuple Len2, HTuple Sigma, HTuple Threshold, HTuple num_Measure, HTuple select, HTuple MinScore, HTuple numInstance)
        {
            HTuple MetrologyHandle, row, col, rad, index;
            HOperatorSet.CreateMetrologyModel(out MetrologyHandle);
            HOperatorSet.InnerCircle(cirRegion, out row, out col, out rad);
            HOperatorSet.AddMetrologyObjectGeneric(MetrologyHandle, "circle",
                new HTuple(row, col, rad), Len1, Len2, Sigma, Threshold, null, null, out index);
            HOperatorSet.SetMetrologyObjectParam(MetrologyHandle, "all", "measure_transition", direction);
            HOperatorSet.SetMetrologyObjectParam(MetrologyHandle, "all", "num_measures", num_Measure);
            HOperatorSet.SetMetrologyObjectParam(MetrologyHandle, "all", "num_instances", numInstance);
            HOperatorSet.SetMetrologyObjectParam(MetrologyHandle, "all", "measure_interpolation", "bicubic");
            HOperatorSet.SetMetrologyObjectParam(MetrologyHandle, "all", "measure_select", select);
            HOperatorSet.SetMetrologyObjectParam(MetrologyHandle, "all", "min_score", MinScore);
            HOperatorSet.GetMetrologyObjectMeasures(out outCaliperXLD, MetrologyHandle, "all", "all", out row, out col);
            HOperatorSet.ApplyMetrologyModel(image, MetrologyHandle);
            HOperatorSet.GetMetrologyObjectResultContour(out outCircleXLD, MetrologyHandle, "all", "all", 1.5);
            HOperatorSet.ClearMetrologyModel(MetrologyHandle);
        }

        void FindCircleArea(HObject image, HObject cirRegion, out HObject outCaliperXLD, out HObject outCircleXLD)
        {
            CreateCapCircle(image, cirRegion, out outCaliperXLD, out outCircleXLD, "positive",
                90, 5, 1, 30, 20, "all", 0.6, 1);
        }
        /// <summary>吸嘴校正初始化，會將分析資料輸出為像素位置</summary>
        public void CarbInit()
        {

        }
        /// <summary>吸嘴校正，依序傳入 0度X1Y1 / 90度X1Y1 / 90度X2Y1 / 90度X2Y2</summary>
        public void Carb(List<Vector3> ImageLoc, List<Vector3> axisLoc)
        {

        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetLineWidth(2);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
            if (InspOK && (P1x != null) && (P1x.Length > 0))
            {
                Win.HalconWindow.SetColor("green");
                HOperatorSet.DispArrow(Win.HalconWindow, P1y, P1x, P2y, P2x, 12);
            }
            if (PinArea != null)
            {
                HTuple row, col, phi, L1, L2;
                Win.HalconWindow.SetColor("green");
                HOperatorSet.DispRegion(PinArea, Win.HalconWindow);
                if (dTop != null)
                {
                    Win.HalconWindow.SetColor("orange");
                    HOperatorSet.DispRegion(dTop, Win.HalconWindow);
                    HOperatorSet.SmallestRectangle2(dTop, out row, out col, out phi, out L1, out L2);
                    Win.HalconWindow.SetTposition((int)row.D, (int)col.D + 80);
                    Win.HalconWindow.WriteString(string.Format("寬度 = {0:F2}", L2.D * CCD.Param.ScaleX * 2));
                }
                if (dBot != null)
                {
                    Win.HalconWindow.SetColor("orange");
                    HOperatorSet.DispRegion(dBot, Win.HalconWindow);
                    HOperatorSet.SmallestRectangle2(dBot, out row, out col, out phi, out L1, out L2);
                    Win.HalconWindow.SetTposition((int)row.D, (int)col.D + 80);
                    Win.HalconWindow.WriteString(string.Format("寬度 = {0:F2}", L2.D * CCD.Param.ScaleX * 2));
                }
            }
            if ((P1x != null) && (P2x != null) && (P1x.Length == 1) && (P2x.Length == 1))
            {
                Win.HalconWindow.DispArrow(P1y, P1x, P2y, P2x, 5);
                //Win.HalconWindow.SetFont("Arial-64");
                Win.HalconWindow.SetTposition((int)P1y.D, (int)P1x.D + 50);
                string Msg = (InspOK) ? string.Format("針角度={0:F1}", PinDeg) : "NG";
                Win.HalconWindow.WriteString(Msg);
                //Win.HalconWindow.WriteString(string.Format("θ = {0:F2}", PinDeg));
            }

            //Win.HalconWindow.SetTposition(60, 250);
            //var SSS = Win.HalconWindow.GetFont();
            ////Win.HalconWindow.SetFont("default-normal-24");
            //string Msg = (InspOK) ? string.Format("針角度={0:F1}", PinDeg) : "NG";
            //Win.HalconWindow.WriteString(Msg);
        }

        public void Show()
        {
            helper.doResult();
        }

        HObject Compose3(HObject img)
        {
            HObject img2, img3, imgColor;
            HOperatorSet.CopyImage(img, out img2);
            HOperatorSet.CopyImage(img, out img3);
            HOperatorSet.Compose3(img, img2, img3, out imgColor);
            owner.DisposeObj(img2, img3);
            return imgColor;
        }

        public void Teach()
        {
            double targetθ;
            Insp(out targetθ);
            if (RegionNozzle.CountObj() > 0)
            {
                using (var P = new 吸嘴Teach())
                {
                    P.ShowDialog(helper.Image, RegionNozzle, ref model, ref owner.parameter.下視覺特徵為針頭);
                }
            }
            else
                MessageBox.Show("無法判斷針軸區域，請調整取料 Pin長/寬");
        }

    }

    #endregion

    #region Socket
    public class InspSocket區
    {
        HWindowHelper helper;
        HObject RegionSocket;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        Inspector owner;
        bool first = true;
        HTuple pX, pY, CarbX, CarbY;
        int targetIndex = -1;
        Vector3 hole;
        public Vector3 CarbP;

        public InspSocket區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new ImageSource("CCDSocket", "FreeRun", "./", 5, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            HImage temp1 = temp.RotateImage(-90.5, "bilinear");
            owner.DisposeObj(temp);
            helper.Image = temp1;
            helper.SetImageSize(temp1);
            if (first)
            {
                first = false;
                helper.AdjustView();
            }
        }

        void SetMenu()
        {
            menu = new ContextMenuStrip();
            helper.hWin.ContextMenuStrip = menu;
            menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            menu.Items.Add("分析Socket", null, (sender, e) =>
            {
                Vector3 n;
                var sel = Insp(out n);
            });
            menu.Items.Add("校正孔尋找", null, (sender, e) =>
            {
                Vector3 n;
                var sel = 校正孔尋找(out n);
            });
            menu.Items.Add("Save Image", null, (sender, e) => { owner.WriteImage(helper.Image, "Socket", "Socket"); });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            //items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }
        /// <summary>分析Socket盤，，傳入目前Socket軸位置(X / Y)，回覆針孔位置</summary>
        public bool Insp(out Vector3 Loc)
        {
            var result = new Vector3();
            helper.inShow = true;
            targetIndex = -1;
            if (helper.ContainImage)
            {
                double NeedleCircleParameter = owner.getParam("NeedleCircleParameter");
                HObject temp, binArea, connArea, SelArea, OutArea, MaxArea, CirArea;
                HTuple W, H, usedThr, row, col, Radius, Dist;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "Socket", "Socket");
                HOperatorSet.GetImageSize(temp, out W, out H);
                HOperatorSet.BinaryThreshold(temp, out binArea, "max_separability", "light", out usedThr);
                HOperatorSet.Connection(binArea, out connArea);
                HOperatorSet.SelectShape(connArea, out SelArea, "circularity", "and", NeedleCircleParameter, 1);
                HOperatorSet.SelectShape(SelArea, out OutArea, "outer_radius", "and", 20, 180);
                //HOperatorSet.SelectShapeStd(OutArea, out MaxArea, "max_area", 70);
                HOperatorSet.SmallestCircle(OutArea, out row, out col, out Radius);
                if (row.Length > 0)
                {
                    HOperatorSet.SelectShape(OutArea, out CirArea, "row", "and", H / 2.0 - 50, H / 2.0 + 50);
                    HOperatorSet.SelectShape(CirArea, out CirArea, "column", "and", W / 2.0 - 50, W / 2.0 + 50);
                    HOperatorSet.AreaCenter(OutArea, out usedThr, out row, out col);
                    //HOperatorSet.SelectShape(OutArea, out CirArea, "outer_radius", "and", Radius.D - 2, Radius.D + 2);
                HOperatorSet.SmallestCircle(CirArea, out row, out col, out Radius);
                double smallDist = 999999;
                
                for(int i = 0; i < row.Length; i++)
                {
                    HOperatorSet.DistancePp(row[i], col[i], H.D / 2.0, W.D / 2.0, out Dist);
                    if (Dist.D < smallDist)
                    {
                        smallDist = Dist.D;
                        result.X = pX = col[i].D;
                        result.Y = pY = row[i].D;
                        targetIndex = i;
                    }
                }
                if (targetIndex >= 0)
                {
                    result = CCD.GetReal(result.X, result.Y, W, H);
                    result.X = -result.X;
                    hole = result;
                }
                }
            }
            Loc = result;
            owner.BeginInvoke(new Action(helper.AdjustView));
            return ((targetIndex >= 0) && (Math.Abs( result.X) < 0.2) && (Math.Abs(result.Y) < 0.2)) ;
        }

        public bool 校正孔尋找(out Vector3 Loc)
        {
            var result = new Vector3();
            helper.inShow = true;
            bool success = false;
            if (helper.ContainImage)
            {
                HObject temp, binArea, connArea, LimW, LimH, LimC;
                HTuple W, H, usedThr, row, col, Radius, Dist;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "Socket", "Socket");
                HOperatorSet.GetImageSize(temp, out W, out H);
                HOperatorSet.BinaryThreshold(temp, out binArea, "max_separability", "light", out usedThr);
                HOperatorSet.Connection(binArea, out connArea);
                double Cx = W.D / 2;
                double Cy = H.D / 2;
                HOperatorSet.SelectShape(connArea, out LimC, "outer_radius", "and", 5, 999999);
                
                HOperatorSet.SelectShape(LimC, out LimW, "row", "and", Cy - 200, Cy + 200);
                HOperatorSet.AreaCenter(LimC, out Dist, out CarbY, out CarbX);
                HOperatorSet.SelectShape(LimW, out LimH, "column", "and", Cx - 200, Cx + 200);
                HOperatorSet.AreaCenter(LimH, out Dist, out CarbY, out CarbX);
                if (CarbY.Length == 1)
                {
                    success = true;
                    var P = CCD.GetReal(CarbX.D, CarbY.D, W, H);
                    result = new Vector3() { X = P.X, Y = P.Y };
                    CarbP = result;
                }
                owner.DisposeObj(temp, binArea, connArea, LimC, LimW, LimH);
            }
            Loc = result;
            owner.BeginInvoke(new Action(helper.AdjustView));
            return success;
        }

        public bool 植針後Check()
        {
            Vector3 pos;

            var success = !Insp(out pos);
            if (success) {
                //找不到孔, 已植針
                success = true;
            } else { 
                //找到孔, 做孔位置檢查
                if( (Math.Abs(pos.X)>0.1) || (Math.Abs(pos.Y)>0.1) ) {
                    //目標位置有針, 孔位置偏離中心0.1mm
                    success = true;
                } else { 
                    //目標位置無針, 正中心有一個孔
                    success = false;
                }
            }

            return success;
        }
        /// <summary>Socket盤校正初始化，會將分析資料輸出為像素位置</summary>
        public void CarbInit()
        {

        }
        /// <summary>吸嘴校正，依序傳入Socket分析後第一筆資料 X1Y1 / X2Y1 / X2Y2</summary>
        public void Carb(List<Vector3> ImageLoc, List<Vector3> axisLoc)
        {

        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetColor("orange");
            if ((targetIndex >= 0) && (pX != null) && (pY != null))
            {
                if (owner.ck_Socket孔.Checked)
                {
                    Win.HalconWindow.DispCross(pY, pX, 300, 0);
                    Win.HalconWindow.SetTposition((int)(pY.D + 300), 300);
                    Win.HalconWindow.WriteString(string.Format("X = {0:F3} , Y = {1:F3}", hole.X, hole.Y));
                }
            }
            if ((CarbX != null) && (CarbY != null) && (CarbX.Length == 1))
            {
                if (owner.ck_Socket校正孔.Checked)
                {
                    Win.HalconWindow.SetColor("orange");
                    Win.HalconWindow.DispCross(CarbY, CarbX, 300, 0);
                    Win.HalconWindow.SetTposition((int)(CarbY.D + 300), 300);
                    Win.HalconWindow.WriteString(string.Format("X = {0:F3} , Y = {1:F3}", CarbP.X, CarbP.Y));
                }
            }


            HTuple W, H;
            if (image != null)
            {
                Win.HalconWindow.SetColor("green");
                HOperatorSet.GetImageSize(image, out W, out H);
                var LStyle = Win.HalconWindow.GetLineStyle();
                Win.HalconWindow.SetLineStyle(7);
                Win.HalconWindow.DispLine(H.I / 2.0, 0, H.I / 2, W.I - 1);
                Win.HalconWindow.DispLine(0, W.I / 2.0, H.I - 1, W.I / 2.0);
                Win.HalconWindow.SetLineStyle(LStyle);
            }
        }
        public void Show()
        {
            helper.doResult();
        }
    }

    #endregion

    #region 夾爪CCD
    public class InspCCD5區
    {
        HWindowHelper helper;
        HObject RegionSocket;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        Inspector owner;
        bool first = true;
        HTuple pX, pY;
        int targetIndex = -1;
        Vector3 hole;

        public InspCCD5區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new ImageSource("CCD5", "FreeRun", "./", 8, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            HImage temp1 = temp.RotateImage(-90.0, "constant");
            owner.DisposeObj(temp);
            helper.Image = temp1;
            helper.SetImageSize(temp1);
            if (first)
            {
                first = false;
                helper.AdjustView();
            }
        }

        void SetMenu()
        {
            menu = new ContextMenuStrip();
            helper.hWin.ContextMenuStrip = menu;
            menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            menu.Items.Add("分析夾爪針孔校正", null, (sender, e) =>
            {
                Vector3 n;
                var sel = Insp(out n);
            });
            menu.Items.Add("Save Image", null, (sender, e) => { owner.WriteImage(helper.Image, "夾爪", "夾爪"); });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            //items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }

        public bool Insp(out Vector3 Loc)
        {
            var result = new Vector3();
            helper.inShow = true;
            targetIndex = -1;
            if (helper.ContainImage)
            {
                HObject temp, binArea, connArea, SelArea, OutArea, MaxArea, CirArea;
                HTuple W, H, usedThr, row, col, Radius, Dist, area1, area2;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "針孔校正", "針孔校正");
                HOperatorSet.GetImageSize(temp, out W, out H);
                HOperatorSet.BinaryThreshold(temp, out binArea, "max_separability", "light", out usedThr);
                HOperatorSet.Connection(binArea, out connArea);
                HOperatorSet.SelectShape(connArea, out SelArea, "circularity", "and", 0.6, 1);
                HOperatorSet.SelectShape(SelArea, out OutArea, "outer_radius", "and", 20, 200);
                if (OutArea.CountObj() == 1)
                {
                    HOperatorSet.ShapeTrans(OutArea, out CirArea, "outer_circle");
                    HOperatorSet.AreaCenter(OutArea, out area1, out row, out col);
                    HOperatorSet.AreaCenter(CirArea, out area2, out row, out col);
                    double percent = area1.D / area2.D;
                    if (percent > 0.75)
                    {
                        result.X = pX = col.D;
                        result.Y = pY = row.D;
                        result = CCD.GetReal(result.X, result.Y, W, H);
                        result.X = -result.X;
                        hole = result;
                        targetIndex = 0;
                    }
                }
            }
            Loc = result;
            owner.BeginInvoke(new Action(helper.AdjustView));
            return targetIndex >= 0;
        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
            if ((targetIndex >= 0) && (pX != null) && (pY != null))
            {
                Win.HalconWindow.DispCross(pY, pX, 300, 0);
                Win.HalconWindow.SetTposition((int)(pY.D + 300), 300);
                Win.HalconWindow.WriteString(string.Format("X = {0:F3} , Y = {1:F3}", hole.X, hole.Y));
            }
            HTuple W, H;
            if (image != null)
            {
                Win.HalconWindow.SetColor("green");
                HOperatorSet.GetImageSize(image, out W, out H);
                var LStyle = Win.HalconWindow.GetLineStyle();
                Win.HalconWindow.SetLineStyle(7);
                Win.HalconWindow.DispLine(H.I / 2.0, 0, H.I / 2, W.I - 1);
                Win.HalconWindow.DispLine(0, W.I / 2.0, H.I - 1, W.I / 2.0);
                Win.HalconWindow.SetLineStyle(LStyle);
            }
        }
        public void Show()
        {
            helper.doResult();
        }
    }

    #endregion

    #region 吸針孔校正
    public class InspCCD6區
    {
        HWindowHelper helper;
        HObject RegionSocket;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        Inspector owner;
        bool first = true;
        HTuple pX, pY;
        int targetIndex = -1;
        Vector3 hole;

        public InspCCD6區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new ImageSource("CCD6", "FreeRun", "./", 5, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            HImage temp1 = temp.RotateImage(-90.4, "bilinear");
            owner.DisposeObj(temp);
            helper.Image = temp1;
            helper.SetImageSize(temp1);
            if (first)
            {
                first = false;
                helper.AdjustView();
            }
        }

        void SetMenu()
        {
            menu = new ContextMenuStrip();
            helper.hWin.ContextMenuStrip = menu;
            menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            menu.Items.Add("分析針孔校正", null, (sender, e) =>
            {
                Vector3 n;
                var sel = Insp(out n);
            });
            menu.Items.Add("Save Image", null, (sender, e) => { owner.WriteImage(helper.Image, "針孔校正", "針孔校正"); });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            //items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }

        public bool Insp(out Vector3 Loc) 
        {
            var result = new Vector3();
            helper.inShow = true;
            targetIndex = -1;
            if (helper.ContainImage)
            {
                HObject temp, binArea, connArea, SelArea, OutArea, MaxArea, CirArea;
                HTuple W, H, usedThr, row, col, Radius, Dist;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "針孔校正", "針孔校正");
                HOperatorSet.GetImageSize(temp, out W, out H);
                HOperatorSet.BinaryThreshold(temp, out binArea, "max_separability", "light", out usedThr);
                HOperatorSet.Connection(binArea, out connArea);
                HOperatorSet.SelectShape(connArea, out SelArea, "circularity", "and", 0.8, 1);
                HOperatorSet.SelectShape(SelArea, out OutArea, "outer_radius", "and", 35, 85);
                if (OutArea.CountObj() == 1)
                {
                    HOperatorSet.SmallestCircle(OutArea, out row, out col, out Radius);
                    result.X = pX = col.D;
                    result.Y = pY = row.D;
                    result = CCD.GetReal(result.X, result.Y, W, H);
                    result.X = -result.X;
                    hole = result;
                    targetIndex = 0;
                }
            }
            Loc = result;
            owner.BeginInvoke(new Action(helper.AdjustView));
            return targetIndex >= 0;
        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
            if ((targetIndex >= 0) && (pX != null) && (pY != null))
            {
                Win.HalconWindow.DispCross(pY, pX, 300, 0);
                Win.HalconWindow.SetTposition((int)(pY.D + 300), 300);
                Win.HalconWindow.WriteString(string.Format("X = {0:F3} , Y = {1:F3}", hole.X, hole.Y));
            }
            HTuple W, H;
            if (image != null)
            {
                Win.HalconWindow.SetColor("green");
                HOperatorSet.GetImageSize(image, out W, out H);
                var LStyle = Win.HalconWindow.GetLineStyle();
                Win.HalconWindow.SetLineStyle(7);
                Win.HalconWindow.DispLine(H.I / 2.0, 0, H.I / 2, W.I - 1);
                Win.HalconWindow.DispLine(0, W.I / 2.0, H.I - 1, W.I / 2.0);
                Win.HalconWindow.SetLineStyle(LStyle);
            }
        }
        public void Show()
        {
            helper.doResult();
        }
    }

    #endregion

}
