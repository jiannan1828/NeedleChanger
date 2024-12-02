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

#pragma warning disable CS0168
#pragma warning disable CS0169

namespace Inspector
{
    public partial class Inspector : UserControl
    {
        internal bool DebugMode = true; //右鍵選單顯示
        public bool xEngineer = true;   //右鍵選單顯示
        public InspParameter parameter = new InspParameter();
        Insp入料區 Insp入料;
        InspTray區 InspTray;
        Insp吸嘴區 InspNozzle;
        InspSocket區 InspSocket;
        InspCCD5區 InspCCD5;
        InspCCD6區 InspCCD6;
        OPTControlerRS OPT;
        bool ExitP = false;
        ushort[] DT = new ushort[30000];
        List<Vector3> TrayItem;
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer() { Interval = 200, Enabled = true };

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
            Insp入料 = new Insp入料區(this, Win3);
            InspTray = new InspTray區(this, Win2);
            InspNozzle = new Insp吸嘴區(this, Win1);
            //InspSocket = new InspSocket區(this, Win1);
            //InspCCD5 = new InspCCD5區(this, Win4);
            //InspCCD6 = new InspCCD6區(this, Win5);
            OPT = new OPTControlerRS(4);
            OPT.Open("COM3");
            Task.Factory.StartNew(Scan);
            Application.ApplicationExit += onExit;
            timer1.Tick += onTick1;
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
            Insp入料.Show();
            InspTray.Show();
            InspNozzle.Show();
        }

        public void LoadRecipe(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<int>(LoadRecipe), Num);
            else
            {
                string dPath = string.Format("D:\\EQ\\Injector\\Recipe\\{0}.XML", Num);
                if (File.Exists(dPath))
                    parameter = DeSerializeXML<InspParameter>(File.ReadAllBytes(dPath));
                num_Pin寬Min.Value = (decimal)parameter.TrayPin寬Min;
                num_Pin寬Max.Value = (decimal)parameter.TrayPin寬Max;
                num_Pin長Min.Value = (decimal)parameter.TrayPin長Min;
                num_Pin長Max.Value = (decimal)parameter.TrayPin長Max;
                num_距離限制.Value = (decimal)parameter.Pin相鄰距離;
                num_Throshold.Value = (int)parameter.TrayThreshold;
            }
        }

        public void SaveRecipe(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<int>(LoadRecipe), Num);
            else
            {
                string dPath = string.Format("D:\\EQ\\Injector\\Recipe\\{0}.XML", Num);
                string dDir = Path.GetDirectoryName(dPath);
                try
                {
                    if (!Directory.Exists(dDir))
                        Directory.CreateDirectory(dDir);
                    File.WriteAllBytes(dPath, SerializeXML(parameter));
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
                    if (P == CCDName.入料) Insp入料.CCD.TriggerMode = "FreeRun";
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
                    if (P == CCDName.入料) Insp入料.CCD.TriggerMode = "Stop";
                    if (P == CCDName.震動盤) InspTray.CCD.TriggerMode = "Stop";
                }
                Msg = string.Join(", ", items.Select(x => x.ToString()));
            }
            WriteLog("Stop " + Msg);
        }
        /// <summary>分析入料盤，回覆有無料</summary>
        public bool xInsp入料() { return Insp入料.Insp(); }
        /// <summary>分析震動盤，回覆針位置，無料時回覆 false</summary>
        public bool xInsp震動盤(out List<Vector3> target) { return InspTray.Insp(out target); }
        /// <summary>分析吸嘴，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool xInsp吸嘴(out double targetθ) { return InspNozzle.Insp(out targetθ); }
        /// <summary>吸嘴校正初始化，會將分析資料輸出為像素位置</summary>
        public void xCarbInit吸嘴() { InspNozzle.CarbInit(); }
        /// <summary>吸嘴校正，依序傳入 0度X1Y1 / 90度X1Y1 / 90度X2Y1 / 90度X2Y2</summary>
        public void xCarb吸嘴(List<Vector3> ImageLoc, List<Vector3> axisLoc) { InspNozzle.Carb(ImageLoc, axisLoc); }
        /// <summary>分析Socket盤，，傳入目前Socket軸位置(X / Y)，回覆針孔位置</summary>
        public List<Vector3> xInspSocket(Vector3 Loc) { return InspSocket.Insp(Loc); }
        /// <summary>Socket盤校正初始化，會將分析資料輸出為像素位置</summary>
        public void xCarbInitSocket() { InspSocket.CarbInit(); }
        /// <summary>吸嘴校正，依序傳入Socket分析後第一筆資料 X1Y1 / X2Y1 / X2Y2</summary>
        public void xCarbSocket(List<Vector3> ImageLoc, List<Vector3> axisLoc) { InspSocket.Carb(ImageLoc, axisLoc); }
        /// <summary>未知作用</summary>
        public void xInspCCD5() { }
        /// <summary>未知作用</summary>
        public void xInspCCD6() { }

        internal void DisposeObj(params HObject[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
                if (arg[i] != null)
                    arg[i].Dispose();
        }

        internal HObject RemoveBorder(HObject regn, HTuple W, HTuple H)
        {
            HObject right, left, top, outRegion;
            HOperatorSet.SelectShape(regn, out right, "column2", "and", 5, W - 5);
            HOperatorSet.SelectShape(right, out left, "column1", "and", 5, W - 5);
            HOperatorSet.SelectShape(left, out top, "row1", "and", 5, H - 5);
            HOperatorSet.SelectShape(top, out outRegion, "row2", "and", 5, H - 5);
            DisposeObj(right, left, top);
            return outRegion;
        }

        private void 入料CCD設定_DoubleClick(object sender, EventArgs e)
        {
            Insp入料.CCD.SetParam();
        }

        private void TrayCCD設定_DoubleClick(object sender, EventArgs e)
        {
            InspTray.CCD.SetParam();
        }

        private void 吸嘴CCD設定_DoubleClick(object sender, EventArgs e)
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

        internal void WriteImage(HObject img, string title)
        {
            try
            {
                string FName = string.Format("D:\\Images\\{0:yyyyMMdd}\\{0:HHmmssff}{1}.JPG", DateTime.Now, title);
                string FDir = Path.GetDirectoryName(FName);
                if (!Directory.Exists(FDir))
                    Directory.CreateDirectory(FDir);
                HOperatorSet.WriteImage(img, "jpeg 70", 0, FName);
            }
            catch
            {
            }
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
        int ImgWidth = 5120, ImgHeight = 5120;
        internal HWindowControl hWin;
        HObject dImage;
        public HObject Image
        {
            get { return dImage; }
            set
            {
                if (dImage == null)
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
        public int TrayThreshold = 120;
        public double TrayPin寬Min = 4;
        public double TrayPin寬Max = 80;
        public double TrayPin長Min = 4;
        public double TrayPin長Max = 20;
        public double Pin相鄰距離 = 40;
    }
    #endregion

    #region 入料
    public class Insp入料區
    {
        HWindowHelper helper;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        bool first = true, ItemSuccess;
        Inspector owner;
        HObject binregion;

        public Insp入料區(Inspector sender, HWindowControl win)
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

        HObject GetAllPin(HObject image)
        {
            HObject rgn1, connregion, SelectedRegions, rgnFill, highpass, rgnFull, rgnDiff;
            HTuple min, range, L1, L2, phi, _Max;
            //HOperatorSet.Threshold(img, out rgn1, 0, 120);
            //HOperatorSet.Connection(rgn1, out connregion);
            //HOperatorSet.SelectShape(connregion, out SelectedRegions, "rect2_len2", "and", 0.3, 150);
            //HOperatorSet.SelectShape(SelectedRegions, out SelectedRegions, "rect2_len1", "and", 1, 150);
            //HOperatorSet.FillUp(SelectedRegions, out rgnFill);
            HOperatorSet.GetDomain(image, out rgnFull);
            HOperatorSet.MinMaxGray(rgnFull, image, 0, out min, out _Max, out range);
            HOperatorSet.Threshold(image, out rgn1, _Max.D - 30, 255);
            HOperatorSet.FillUp(rgn1, out rgnFill);
            HOperatorSet.Difference(rgnFill, rgn1, out rgnDiff);
            HOperatorSet.Connection(rgnDiff, out connregion);
            HOperatorSet.SelectShape(connregion, out SelectedRegions, "area", "and", 30, 999999);
            owner.DisposeObj(rgn1, rgnFill, rgnFull, rgnDiff, connregion);
            return SelectedRegions;
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
                if (manualInsp)
                    owner.WriteImage(temp, "入料");
                HOperatorSet.GetImageSize(temp, out W, out H);
                Region入料 = GetAllPin(temp);
                HOperatorSet.Union1(Region入料, out binregion);
                HOperatorSet.AreaCenter(binregion, out area, out row, out col);
                result = (area.Length > 0) && (area.I > 400);
                ItemSuccess = result;
                if ((!manualInsp) && result)
                    owner.WriteImage(temp, "入料");
            }
            owner.BeginInvoke(new Action(helper.AdjustView));
            return result;
        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.ClearWindow(Win.HalconWindow);
            HOperatorSet.DispImage(image, Win.HalconWindow);
            //Win.HalconWindow.SetDraw("margin");
            Win.HalconWindow.SetDraw("fill");
            if (binregion != null)
            {
                if (ItemSuccess)
                    Win.HalconWindow.SetColor("green");
                else
                    Win.HalconWindow.SetColor("red");
                HOperatorSet.DispRegion(binregion, Win.HalconWindow);
            }
        }

        public void Show()
        {
            helper.doResult();
        }


    }
    #endregion

    #region Tray
    public class InspTray區
    {
        HWindowHelper helper;
        HObject RegionTray, RegionMeas, PinArea, MeasPin;
        HTuple rowTray, colTray, rowDist, colDist, rowPin, colPin;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        bool first = true;
        Inspector owner;
        int AddL1 = 15;

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
            temp = temp.RotateImage(-90.0, "constant");
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
            menu.Items.Add("分析震動盤", null, (sender, e) =>
            {
                List<Vector3> target;
                Insp(out target);
            });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
            menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
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
            HOperatorSet.SelectShape(ConnectionArea, out SelectedRegions, "rect2_len2", "and", owner.parameter.TrayPin寬Min, owner.parameter.TrayPin寬Max);
            HOperatorSet.FillUp(SelectedRegions, out rgnFill);
            HOperatorSet.SelectShape(rgnFill, out rgnConv, "convexity", "and", 0.55, 2);
            HOperatorSet.SmallestRectangle2(rgnConv, out row, out col, out phi, out L1, out L2);
            if (L1.Length > 0)
                HOperatorSet.SelectShape(rgnFill, out rgnMax, "rect2_len1", "and", owner.parameter.TrayPin長Min, owner.parameter.TrayPin長Max);
            else
                HOperatorSet.CopyObj(rgnFill, out rgnMax, 1, 1);
            owner.DisposeObj(ConnectionArea, SelectedRegions, rgnFill, rgnConv);
            return rgnMax;
        }

        HObject RemoveNear(HObject img, HObject pin, HObject allPin)
        {
            HTuple Cnt, Num, row, col, phi, L1, L2;
            HObject outRgn;
            HOperatorSet.GenEmptyObj(out outRgn);
            HOperatorSet.SmallestRectangle2(pin, out row, out col, out phi, out L1, out L2);
            for (int i = 0; i < L2.Length; i++)
            {
                L2[i] = L2[i].D + owner.parameter.Pin相鄰距離;
                L1[i] = L1[i].D + AddL1;
            }
            if (L2.Length > 0)
            {
                HOperatorSet.GenRectangle2(out RegionMeas, row, col, phi, L1, L2);
                for (int i = 1; i <= L2.Length; i++)
                {
                    HObject Sel, Intr, Connection, PinSel;
                    HOperatorSet.SelectObj(RegionMeas, out Sel, i);
                    HOperatorSet.SelectObj(pin, out PinSel, i);
                    HOperatorSet.Intersection(allPin, Sel, out Intr);
                    HOperatorSet.Connection(Intr, out Connection);
                    HOperatorSet.CountObj(Connection, out Cnt);
                    if (Cnt == 1)
                        HOperatorSet.ConcatObj(outRgn, PinSel, out outRgn);
                }
                HOperatorSet.CountObj(outRgn, out Cnt);
            }
            else
                RegionMeas = null;

            return outRgn;
        }

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
                HObject temp, SelectedRegions, SelectPins;
                HTuple threshold, W, H, phi, L1, L2, deg;
                HOperatorSet.CopyImage(helper.Image, out temp);
                if (manualInsp && (!owner.ck_RealTray.Checked))
                    owner.WriteImage(temp, "Tray");
                HOperatorSet.GetImageSize(temp, out W, out H);
                SelectedRegions = GetPinNoOverlap(temp, out PinArea);
                SelectPins = owner.RemoveBorder(SelectedRegions, W, H);
                RegionTray = RemoveNear(temp, SelectPins, PinArea);
                MeasurePin(temp, RegionTray, W, H, out rowTray, out colTray, out phi);
                if (phi.Length > 0)
                {
                    deg = phi.TupleDeg();
                    for (int i = 0; i < rowTray.Length; i++)
                    {
                        var P = CCD.GetReal(colTray[i].D, rowTray[i].D, W, H);
                        target.Add(new Vector3() { X = P.X, Y = P.Y, θ = -deg[i].D });
                    }
                    if ((!manualInsp) && (!owner.ck_RealTray.Checked))
                        owner.WriteImage(temp, "Tray");
                }
                owner.DisposeObj(temp, SelectedRegions, SelectPins);
            }
            owner.BeginInvoke(new Action(Show));
            return target.Count > 0;
        }

        void ShowResult(HWindowControl Win, HObject image)
        {
            if (image == null)
                return;
            HOperatorSet.DispImage(image, Win.HalconWindow);
            if (RegionTray != null)
            {
                Win.HalconWindow.SetDraw("margin");
                Win.HalconWindow.SetColor("orange");
                HOperatorSet.DispRegion(RegionTray, Win.HalconWindow);
                if (owner.ck_Limit.Checked && (RegionMeas != null))
                    HOperatorSet.DispRegion(RegionMeas, Win.HalconWindow);
                Win.HalconWindow.SetColor("green");
                HOperatorSet.DispCross(Win.HalconWindow, rowTray, colTray, 200, 0);
                if (owner.ck_Measure.Checked && (rowDist != null) && (colDist != null))
                    HOperatorSet.DispCross(Win.HalconWindow, rowDist, colDist, 30, 0);
                if (owner.ck_Measure.Checked && (rowPin != null) && (colPin != null))
                {
                    Win.HalconWindow.SetColor("orange");
                    HOperatorSet.DispCross(Win.HalconWindow, rowPin, colPin, 30, 0);
                }
            }
        }

        public void Show()
        {
            helper.doResult();
        }

    }
    #endregion

    #region 吸嘴
    public class Insp吸嘴區
    {
        HWindowHelper helper;
        HObject RegionNozzle;
        ContextMenuStrip menu;
        internal SisoFGArea CCD;
        Inspector owner;
        bool first = true, InspOK = false;
        HTuple P1x, P1y, P2x, P2y;
        HObject PinArea;
        double PinDeg = 0;

        public Insp吸嘴區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new SisoFGArea("CCDNozzle", "FreeRun", "./", 2, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            temp = temp.RotateImage(90.0, "constant");
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
            menu.Items.Add("分析吸嘴", null, (sender, e) =>
            {
                double target;
                Insp(out target);
            });
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            menu.Items.Add("FullView", null, (sender, e) => { helper.AdjustView(); });
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
                HObject temp;
                HTuple W, H, Cnt;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "吸嘴");
                HOperatorSet.GetImageSize(temp, out W, out H);
                PinArea = GetPinArea(temp, W, H);
                HOperatorSet.CountObj(PinArea, out Cnt);
                if (Cnt != 0)
                {
                    PinDeg = targetθ = CheckDirection(temp, PinArea, W, H);
                    success = true;
                }
                else
                    success = false;
                owner.DisposeObj(temp);
                //CenterArea(temp, out Ellipse);
                //FindCircleArea(temp, Ellipse, out CaliperXLD, out CircleXLD);
                //FindHalf(temp, CircleXLD, out HalfArea);
            }
            owner.BeginInvoke(new Action(Show));
            InspOK = success;
            return success;
        }

        HObject GetPinArea(HObject image, HTuple W, HTuple H)
        {
            HObject fullRegion, region, RegionClosing, ConnectedRegions, SelectedCenterW;
            HObject SelectedCenterH, result;
            HTuple _Min, _Max, _Range;
            HOperatorSet.GetDomain(image, out fullRegion);
            HOperatorSet.MinMaxGray(fullRegion, image, 0, out _Min, out _Max, out _Range);
            HOperatorSet.Threshold(image, out region, _Max - 30, 255);
            HOperatorSet.ClosingRectangle1(region, out RegionClosing, 1, 20);
            HOperatorSet.Connection(RegionClosing, out ConnectedRegions);
            HOperatorSet.SelectShape(ConnectedRegions, out SelectedCenterW, "column", "and", (W / 2) - 80, (W / 2) + 80);
            HOperatorSet.SelectShape(SelectedCenterW, out SelectedCenterH, "row", "and", (H / 2) - 120, (H / 2) + 120);
            HOperatorSet.SelectShapeStd(SelectedCenterH, out result, "max_area", 70);
            owner.DisposeObj(fullRegion, region, RegionClosing, ConnectedRegions, SelectedCenterW, SelectedCenterH);
            return result;
        }

        double CheckDirection(HObject image, HObject area, HTuple W, HTuple H)
        {
            HTuple Deg = 0, row, col, phi, L1, L2, MeasureHandle;
            HTuple RowEdge, ColumnEdge, Amplitude, Distance;
            HOperatorSet.SmallestRectangle2(area, out row, out col, out phi, out L1, out L2);
            if (L1.Length == 0)
                return 0;
            HOperatorSet.GenMeasureRectangle2(row, col, phi, L1 + 5, L2 + 5, W, H, "nearest_neighbor", out MeasureHandle);
            HOperatorSet.MeasurePos(image, MeasureHandle, 1, 30, "all", "all", out RowEdge, out ColumnEdge, out Amplitude, out Distance);
            Deg = phi.TupleDeg();
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
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetLineWidth(2);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
            if (InspOK && (P1x.Length > 0))
            {
                Win.HalconWindow.SetColor("green");
                HOperatorSet.DispArrow(Win.HalconWindow, P1y, P1x, P2y, P2x, 12);
            }
            if (!InspOK && (PinArea != null))
            {
                HOperatorSet.DispRegion(PinArea, Win.HalconWindow);
                Win.HalconWindow.SetColor("red");
            }
            Win.HalconWindow.SetTposition(60, 250);
            var SSS = Win.HalconWindow.GetFont();
            //Win.HalconWindow.SetFont("default-normal-24");
            string Msg = (InspOK) ? string.Format("針角度={0:F1}", PinDeg) : "NG";
            Win.HalconWindow.WriteString(Msg);
        }

        public void Show()
        {
            helper.doResult();
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
            temp = temp.RotateImage(0.0, "constant");
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
            //menu.Items.Add("分析震動盤", null, (sender, e) =>
            //{
            //    List<Vector3> target;
            //    Insp(out target);
            //});
            menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            //menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }
        /// <summary>分析Socket盤，，傳入目前Socket軸位置(X / Y)，回覆針孔位置</summary>
        public List<Vector3> Insp(Vector3 Loc)
        {
            var result = new List<Vector3>();
            helper.inShow = true;
            if (helper.ContainImage)
            {
                HObject temp;
                HTuple W, H;
                HOperatorSet.CopyImage(helper.Image, out temp);
                owner.WriteImage(temp, "Socket");
            }
            owner.BeginInvoke(new Action(helper.AdjustView));
            return result;
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
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
        }

    }

    #endregion

    #region CCD5
    public class InspCCD5區
    {
        HWindowHelper helper;
        HObject RegionSocket;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        Inspector owner;
        bool first = true;

        public InspCCD5區(Inspector sender, HWindowControl win)
        {
            owner = sender;
            CCD = new ImageSource("CCD5", "FreeRun", "./", 5, onRecvImage, IntPtr.Zero);
            helper = new HWindowHelper(win) { ShowResult = ShowResult };
            SetMenu();
        }

        void onRecvImage(object sender, string ImageType, int width, int height, IntPtr buffer)
        {
            HImage temp = new HImage("byte", width, height, buffer);
            temp = temp.RotateImage(0.0, "constant");
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
            //menu = new ContextMenuStrip();
            //helper.hWin.ContextMenuStrip = menu;
            //menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            //menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            //menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            //var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            //items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            //items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            //items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }

        public void Insp() { }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
        }

    }

    #endregion

    #region CCD6
    public class InspCCD6區
    {
        HWindowHelper helper;
        HObject RegionSocket;
        ContextMenuStrip menu;
        internal ImageSource CCD;
        Inspector owner;
        bool first = true;

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
            temp = temp.RotateImage(0.0, "constant");
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
            //menu = new ContextMenuStrip();
            //helper.hWin.ContextMenuStrip = menu;
            //menu.Items.Add("LoadImage", null, (sender, e) => { helper.LoadImage(); });
            //menu.Items.Add("Set CCD Parameter", null, (sender, e) => { CCD.SetParam(); });
            //menu.Opening += onMenu1Opening;
        }

        void onMenu1Opening(object sender, CancelEventArgs e)
        {
            //var items = (sender as ContextMenuStrip).Items.OfType<ToolStripItem>();
            //items.First(x => x.Text == "LoadImage").Visible = owner.DebugMode;
            //items.First(x => x.Text == "分析震動盤").Visible = owner.DebugMode;
            //items.First(x => x.Text == "Set CCD Parameter").Visible = owner.xEngineer;
        }

        public void Insp() { }

        void ShowResult(HWindowControl Win, HObject image)
        {
            HOperatorSet.DispImage(image, Win.HalconWindow);
            Win.HalconWindow.SetColor("orange");
            Win.HalconWindow.SetDraw("margin");
        }

    }

    #endregion

}
