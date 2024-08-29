using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.IO;
using VMControls.Winform.Release;
using VM.PlatformSDKCS;
using VM.Core;
using Camera;
using VMControls.WPF;

//#pragma warning disable CS0168
//#pragma warning disable CS0169

namespace Inspector
{
    public partial class Inspector : UserControl
    {
        //internal bool DebugMode = true;
        //public bool xEngineer = true;
        public InspParameter parameter = new InspParameter();
        public bool isInit = false;
        internal bool ExitP = false;
        public MVSAreaGE CCDTray, CCD入料, CCD吸嘴, CCDSocket;
        Insp入料區 Insp入料;
        InspTray區 InspTray;
        Insp吸嘴區 InspNozzle;
        InspSocket區 InspSocket;
        InspCCD5區 InspCCD5;
        InspCCD6區 InspCCD6;
        //EventWaitHandle Wa;
        //ManualResetEvent ME;
        //AutoResetEvent AE;
        //Thread TR;
        internal GlobalVariableModuleCs.GlobalVariableModuleTool GlobalVar;
        internal VmSolution solution;
        int Count入料 = 0;
        object dLock = new object();
        internal bool ReqGC = false;
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = true };
        double GCTime = 0;

        public Inspector()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += onResolve;
            AppDomain.CurrentDomain.AssemblyResolve += onResolve;   //在 InitializeComponent 之前
            InitializeComponent();
            Application.ApplicationExit += onExit;
            if (AppDomain.CurrentDomain.FriendlyName != "DefaultDomain")
                Init();
        }

        /// <summary>程式庫不再目前目錄，嘗試解析載入</summary>
        static Assembly onResolve(object sender, ResolveEventArgs args)
        {
            string FName = "";
            string asmName = new AssemblyName(args.Name).Name;

            if (asmName.StartsWith("ThridLibray") || asmName.StartsWith("CLID"))
            {
                string MVRoot = Environment.GetEnvironmentVariable("MV_GENICAM_64");
                if (MVRoot != null)
                {
                    string[] nforder = MVRoot.Split(new string[] { "MV Viewer" }, StringSplitOptions.None);
                    FName = nforder.First() + @"MV Viewer\Development\DOTNET DLL\IMV\DOTNET_4.0\x64\" + asmName + ".DLL";
                }
            }

            if (asmName.StartsWith("MvCamCtrl.Net"))
            {
                string MVRoot = Environment.GetEnvironmentVariable("MVCAM_COMMON_RUNENV");
                if (MVRoot != null)
                {
                    FName = MVRoot + @"\DotNet\AnyCpu\" + asmName + ".DLL";
                }
            }

            if (asmName.StartsWith("halcondotnetxl"))
            {
                string HALCONROOT = Environment.GetEnvironmentVariable("HALCONROOT");
                if (HALCONROOT != null)
                    FName = HALCONROOT + @"\bin\dotnet35\halcondotnetxl.dll";
            }
            if ((FName != "") && File.Exists(FName))
                return Assembly.LoadFrom(FName);
            else
                return null;
        }

        void onExit(object sender, EventArgs e)
        {
            CCD入料.Close();
            ExitP = true;
            VmSolution.Instance?.Dispose();
        }

        internal double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        internal bool TimeOut(double BaseTime, double range)
        { return (NowTime - BaseTime) >= range; }

        void onImage入料(object sender, string imageType, int width, int height, IntPtr pData)
        {
            ImageBaseData Img = new ImageBaseData(pData, (uint)(width * height), width, height, VMPixelFormat.VM_PIXEL_MONO_08);
            Insp入料.source.SetImageData(Img);
            Insp入料.source.ModuParams.PixelFormat = ImageSourceModuleCs.ImageSourceParam.PixelFormatEnum.MONO8;
            Insp入料.win.ImageSource = Img;
            Count入料 = (Count入料 + 1) % 1000;
            Img.Dispose();
            ReqGC = true;
        }

        void onImageTray(object sender, string imageType, int width, int height, IntPtr pData)
        {
            ImageBaseData Img = new ImageBaseData(pData, (uint)(width * height), width, height, VMPixelFormat.VM_PIXEL_MONO_08);
            Img.Dispose();
            ReqGC = true;
        }

        void onImageNozzle(object sender, string imageType, int width, int height, IntPtr pData)
        {
            ImageBaseData Img = new ImageBaseData(pData, (uint)(width * height), width, height, VMPixelFormat.VM_PIXEL_MONO_08);
            Img.Dispose();
            ReqGC = true;
        }

        void onImageSocket(object sender, string imageType, int width, int height, IntPtr pData)
        {
            ImageBaseData Img = new ImageBaseData(pData, (uint)(width * height), width, height, VMPixelFormat.VM_PIXEL_MONO_08);
            Img.Dispose();
            ReqGC = true;
        }

        void onTick(object sender, EventArgs e)
        {
            if (ReqGC && TimeOut(GCTime, 1))
            {
                GCTime = NowTime;
                ReqGC = false;
                GC.Collect();
            }
        }

        public double T1 = 0;
        Task doInit;
        /// <summary>初始化，執行一次</summary>
        public void Init()
        {
            timer1.Tick -= onTick;
            timer1.Tick += onTick;
            T1 = NowTime;
            if ((!isInit) && (doInit == null))
                doInit = Task.Factory.StartNew(() =>
                {
                    while (!this.IsHandleCreated)
                        Thread.Sleep(10);
                    solution = VmSolution.Instance;
                    VmSolution.Load(Application.StartupPath + @"\TestItem.sol");
                    GlobalVar = solution.Modules.OfType<GlobalVariableModuleCs.GlobalVariableModuleTool>().FirstOrDefault();
                    this.Invoke(new Action(InitInsp));
                    isInit = true;
                    T1 = NowTime - T1;
                });
        }

        void InitInsp()
        {
            Insp入料 = new Insp入料區(this, Win3);
            InspTray = new InspTray區(this, Win6);
            InspNozzle = new Insp吸嘴區(this, Win2);
            InspSocket = new InspSocket區(this, Win1);
            InspCCD5 = new InspCCD5區(this, Win4);
            InspCCD6 = new InspCCD6區(this, Win5);
            CCD入料 = new MVSAreaGE("CCD入料", "Stop", "./", 8, onImage入料, IntPtr.Zero);
            CCDTray = new MVSAreaGE("CCDTray", "Stop", "./", 8, onImageTray, IntPtr.Zero);
            CCD吸嘴 = new MVSAreaGE("CCD吸嘴", "Stop", "./", 8, onImageNozzle, IntPtr.Zero);
            CCDSocket = new MVSAreaGE("CCDSocket", "Stop", "./", 8, onImageSocket, IntPtr.Zero);
        }

        /// <summary>設定持續取像</summary>
        public void xFreeRun(params CCDName[] items)
        {
            if (items.Contains(CCDName.ALL))
                xFreeRun(CCDName.入料, CCDName.震動盤, CCDName.吸嘴, CCDName.Socket, CCDName.CCD5, CCDName.CCD6);
            else
                foreach(var P in items)
                {
                    if (P == CCDName.入料) CCD入料.TriggerMode = "FreeRun";
                    if (P == CCDName.震動盤) CCDTray.TriggerMode = "FreeRun";
                    if (P == CCDName.吸嘴) CCD吸嘴.TriggerMode = "FreeRun";
                    if (P == CCDName.Socket) CCDSocket.TriggerMode = "FreeRun";
                }
        }
        /// <summary>設定停止取像</summary>
        public void xStop(params CCDName[] items)
        {
            if (items.Contains(CCDName.ALL))
                xStop(CCDName.入料, CCDName.震動盤, CCDName.吸嘴, CCDName.Socket, CCDName.CCD5, CCDName.CCD6);
            else
                foreach (var P in items)
                {
                    if (P == CCDName.入料) CCD入料.TriggerMode = "Stop";
                    if (P == CCDName.震動盤) CCDTray.TriggerMode = "Stop";
                    if (P == CCDName.吸嘴) CCD吸嘴.TriggerMode = "Stop";
                    if (P == CCDName.Socket) CCDSocket.TriggerMode = "Stop";
                }
        }
        /// <summary>分析入料盤，回覆有無料</summary>
        public bool xInsp入料()
        {
            if (CCD入料.TriggerMode == "FreeRun")
                return Insp入料.Insp();
            else
            {
                CCD入料.SoftTrigger();
                CCD入料.isGrabbed.WaitOne(1500);
                return Insp入料.Insp();
            }
        }
        /// <summary>分析入料盤影像，回覆有無料</summary>
        public bool xInsp入料_Image()
        {
            CCD入料.TriggerMode = "Stop";
            return Insp入料.SelectImageIn() ? Insp入料.Insp() : false;
        }
        /// <summary>分析入料盤影像，回覆有無料</summary>
        public bool xInsp入料_Image(string FName)
        {
            CCD入料.TriggerMode = "Stop";
            SetImage(FName, Insp入料.win, Insp入料.source);
            return Insp入料.Insp();
        }
        /// <summary>分析震動盤，回覆針位置，無料時回覆 false</summary>
        public bool xInsp震動盤(out List<Vector3> target)
        {
            return InspTray.Insp(out target);
        }
        /// <summary>分析震動盤影像，回覆針位置，無料時回覆 false</summary>
        public bool xInsp震動盤_Image(out List<Vector3> target)
        {
            CCDTray.TriggerMode = "Stop";
            target = new List<Vector3>();
            return InspTray.SelectImageIn() ? InspTray.Insp(out target) : false;
        }
        /// <summary>分析震動盤影像，回覆針位置，無料時回覆 false</summary>
        public bool xInsp震動盤_Image(string FName, out List<Vector3> target)
        {
            CCDTray.TriggerMode = "Stop";
            SetImage(FName, InspTray.win, InspTray.source);
            return InspTray.Insp(out target);
        }
        /// <summary>分析吸嘴，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool xInsp吸嘴(Vector3 Loc, out Vector3 target)
        {
            return InspNozzle.Insp(Loc, out target);
        }
        /// <summary>分析吸嘴影像，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool xInsp吸嘴_Image(Vector3 Loc, out Vector3 target)
        {
            CCD吸嘴.TriggerMode = "Stop";
            target = new Vector3();
            return InspNozzle.SelectImageIn() ? InspNozzle.Insp(Loc, out target) : false;
        }
        /// <summary>分析吸嘴影像，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool xInsp吸嘴_Image(string FName, Vector3 Loc, out Vector3 target)
        {
            CCD吸嘴.TriggerMode = "Stop";
            SetImage(FName, InspNozzle.win, InspNozzle.source);
            return InspNozzle.Insp(Loc, out target);
        }
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

        internal void SaveRender(object arg)
        {
            VmRenderControl Win = (VmRenderControl)((arg as object[])[0]);
            string Title = (string)((arg as object[])[1]);
            string FName = string.Format("D:\\Images\\{0:yyyyMMdd}\\{0:HHmmss.f}{1}.jpg", DateTime.Now, Title);
            try
            {
                string dir = Path.GetDirectoryName(FName);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                Win.SaveRenderedImage(FName, 50);
            }
            catch (Exception ex)
            { }
        }

        internal string SelectImage()
        {
            using (var P = new OpenFileDialog())
            {
                P.Filter = "ImageFile | *.bmp;*.jpg;*.tiff;*.tif";
                return (P.ShowDialog() == DialogResult.OK) ? P.FileName : "";
            }
        }

        public void SetImage(string FName, VmRenderControl window, ImageSourceModuleCs.ImageSourceModuleTool source)
        {
            if (FName == "")
                return;
            using (Bitmap bmp = new Bitmap(FName))
            {
                var image = new ImageBaseData(bmp);
                source.SetImageData(image);
                if (image.Pixelformat == (int)VMPixelFormat.VM_PIXEL_MONO_08)
                    source.ModuParams.PixelFormat = ImageSourceModuleCs.ImageSourceParam.PixelFormatEnum.MONO8;
                else
                    source.ModuParams.PixelFormat = ImageSourceModuleCs.ImageSourceParam.PixelFormatEnum.RGB24;
                window.ImageSource = image;
            }
            ReqGC = true;
        }

        internal void SetWindowDefault(params VmRenderControl[] window)
        {
            foreach(var P in window)
            {
                P.ChangeTopBarVisibility(false);
                P.CoordinateInfoVisible = false;
                P.SetBackground("#000000");
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
        public static implicit operator System.Drawing.PointF(Vector3 a) => new System.Drawing.PointF((float)a.X, (float)a.Y);
        public static implicit operator Vector3(System.Drawing.PointF a) => new Vector3() { X = a.X, Y = a.Y, θ = 0 };
    }
    #endregion

    #region InspParameter
    public class InspParameter
    {
        /// <summary>Pin延伸分析區域</summary>
        public double PinRange = 3;
        public double TrayPinMin = 7;
        public double TrayPinMax = 15;
        public double NozzleMax = 32;
        public double TrayPinArea = 240;
        public double GoodPercent = 90;
    }
    #endregion

    #region 入料
    public class Insp入料區
    {
        Inspector owner;
        internal VmProcedure QueryItem;
        public VmRenderControl win;
        public ImageSourceModuleCs.ImageSourceModuleTool source;
        IMVSBlobFindModuCs.IMVSBlobFindModuTool Blob1;

        public Insp入料區(Inspector sender, VmRenderControl window)
        {
            owner = sender;
            win = window;
            QueryItem = (VmProcedure)owner.solution["QueryItem"];
            source = (ImageSourceModuleCs.ImageSourceModuleTool)QueryItem.Modules["source"];
            source.ModuParams.ImageSourceType = ImageSourceModuleCs.ImageSourceParam.ImageSourceTypeEnum.SDK;
            source.ModuParams.OutMono8 = true;
            Blob1 = (IMVSBlobFindModuCs.IMVSBlobFindModuTool)QueryItem.Modules["Blob1"];
            owner.SetWindowDefault(win);
            //win.ChangeTopBarVisibility(false);
            //win.CoordinateInfoVisible = false;
            //win.SetBackground("#000000");
        }

        public bool SelectImageIn()
        {
            string FName = owner.SelectImage();
            if (FName != "")
                owner.SetImage(FName, win, source);
            return FName != "";
        }

        /// <summary>分析入料盤，回覆有無料</summary>
        public bool Insp()
        {
            double baseTime = owner.NowTime;
            QueryItem.Run();
            var RC = Blob1.ModuResult.MinBoudingRect;
            double InspTime = owner.NowTime - baseTime;
            owner.BeginInvoke(new Action<VmRenderControl, List<RectBox>, double>(ShowResult), win, RC, InspTime);
            //ShowResult(win, RC);
            return RC.Count > 0;
        }

        string GetColorStr(Color color)
        { return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B); }

        void ShowResult(VmRenderControl Win, List<RectBox> Box, double inspTime)
        {
            string scolor = GetColorStr(Color.Orange);
            
            foreach (var P in Box)
            {
                RectangleEx ex = new RectangleEx(new System.Windows.Point(P.CenterPoint.X, P.CenterPoint.Y),
                    P.BoxWidth, P.BoxHeight, P.Angle, 0, 0, 1, scolor);
                win.DrawShape(ex);
            }
            string str = string.Format("分析時間:{0:F2}", inspTime);
            TextEx ET = new TextEx(str, new System.Windows.Point(200, 200),
                16, 1, GetColorStr(Color.Orange));
            win.DrawShape(ET);
            Task.Factory.StartNew(owner.SaveRender, new object[] { win, "入料" });
        }
    }
    #endregion

    #region Tray
    public class InspTray區
    {
        Inspector owner;
        VmProcedure FindItem;
        public VmRenderControl win;
        public ImageSourceModuleCs.ImageSourceModuleTool source;
        IMVSGroupCs.IMVSGroupTool group1;
        //DataSetModuleCs.DataSetModuleTool Datas;

        public InspTray區(Inspector sender, VmRenderControl window)
        {
            owner = sender;
            win = window;
            FindItem = (VmProcedure)owner.solution["FindItem"];
            source = (ImageSourceModuleCs.ImageSourceModuleTool)FindItem.Modules["source"];
            source.ModuParams.ImageSourceType = ImageSourceModuleCs.ImageSourceParam.ImageSourceTypeEnum.SDK;
            source.ModuParams.OutMono8 = true;
            group1 = (IMVSGroupCs.IMVSGroupTool)FindItem.Modules["group1"];
            //Datas = (DataSetModuleCs.DataSetModuleTool)group1.Modules["DataItem"];
            owner.SetWindowDefault(win);
            //win.ChangeTopBarVisibility(false);
            //win.CoordinateInfoVisible = false;
            //win.SetBackground("#000000");
        }

        public bool SelectImageIn()
        {
            string FName = owner.SelectImage();
            if (FName != "")
                owner.SetImage(FName, win, source);
            return FName != "";
        }

        /// <summary>分析震動盤，回覆針位置，無料時回覆 false</summary>
        public bool Insp(out List<Vector3> target)
        {
            double baseTime = owner.NowTime;
            target = new List<Vector3>();
            FindItem.Run();
            var StartX = group1.ModuResult.GetOutputFloat("StartX").pFloatVal;
            var StartY = group1.ModuResult.GetOutputFloat("StartY").pFloatVal;
            var EndX = group1.ModuResult.GetOutputFloat("EndX").pFloatVal;
            var EndY = group1.ModuResult.GetOutputFloat("EndY").pFloatVal;
            var Angle = group1.ModuResult.GetOutputFloat("Angle").pFloatVal;
            var TargetW = group1.ModuResult.GetOutputFloat("targetW").pFloatVal;
            var TargetH = group1.ModuResult.GetOutputFloat("targetH").pFloatVal;
            for (int i = 0; i < StartX.Length; i++)
            {
                if (StartX[i] > 1)
                    target.Add(new Vector3()
                    {
                        X = StartX[i],
                        Y = StartY[i],
                        θ = Angle[i]
                    });
            }
            double InspTime = owner.NowTime - baseTime;
            owner.BeginInvoke(new Action<VmRenderControl, float[], float[], float[], float[], float[], float[], float[], double>(ShowResult), win, StartX, StartY, EndX, EndY, TargetW, TargetH, Angle, InspTime);
            //ShowResult(win, StartX, StartY, EndX, EndY, TargetW, TargetH, Angle);
            return target.Count > 0;
        }

        string GetColorStr(Color color)
        { return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B); }

        void ShowResult(VmRenderControl Win, float[] StartX, float[] StartY, float[] EndX, float[] EndY, float[] targetW, float[] targetH, float[] Angle, double inspTime)
        {
            string scolor = GetColorStr(Color.Orange);
            float Deg = 0;
            List<System.Windows.Point> dStart = StartX.Zip(StartY, (a, b) => new System.Windows.Point(a, b)).ToList();
            List<System.Windows.Point> dEnd = EndX.Zip(EndY, (a, b) => new System.Windows.Point(a, b)).ToList();

            for (int i = 0; i < StartX.Length; i++)
            {
                if (StartX[i] < 1)
                    continue;
                LineEx ex = new LineEx(
                    dStart[i],
                    dEnd[i],
                    1, scolor, 1);
                Deg = Angle[i];
                win.DrawShape(ex);
            }
            scolor = GetColorStr(Color.Green);
            for (int i = 0; i < targetW.Length; i++)
            {
                RectangleEx ex = new RectangleEx(
                    dStart[i], targetW[i] * 2.0f, targetH[i], Angle[i], 0, 0, 1,
                    scolor, null, 1);
                win.DrawShape(ex);
            }
            string str = string.Format("分析時間:{0:F2}", inspTime);
            TextEx ET = new TextEx(str, new System.Windows.Point(200, 200),
                16, 1, GetColorStr(Color.Orange));
            win.DrawShape(ET);
            Task.Factory.StartNew(owner.SaveRender, new object[] { win, "Tray" });
        }

    }
    #endregion

    #region 吸嘴
    public class Insp吸嘴區
    {
        Inspector owner;
        VmProcedure InspItem;
        public VmRenderControl win;
        public ImageSourceModuleCs.ImageSourceModuleTool source;
        IMVSGroupCs.IMVSGroupTool 粗定位, 吸嘴方向;

        public Insp吸嘴區(Inspector sender, VmRenderControl window)
        {
            owner = sender;
            win = window;
            InspItem = (VmProcedure)owner.solution["InspItem"];
            source = (ImageSourceModuleCs.ImageSourceModuleTool)InspItem.Modules["source"];
            source.ModuParams.ImageSourceType = ImageSourceModuleCs.ImageSourceParam.ImageSourceTypeEnum.SDK;
            source.ModuParams.OutMono8 = true;
            粗定位 = (IMVSGroupCs.IMVSGroupTool)InspItem.Modules["粗定位"];
            吸嘴方向 = (IMVSGroupCs.IMVSGroupTool)InspItem.Modules["吸嘴方向"];
            owner.SetWindowDefault(win);
            //win.ChangeTopBarVisibility(false);
            //win.CoordinateInfoVisible = false;
            //win.SetBackground("#000000");
        }

        public bool SelectImageIn()
        {
            string FName = owner.SelectImage();
            if (FName != "")
                owner.SetImage(FName, win, source);
            return FName != "";
        }

        /// <summary>分析吸嘴，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool Insp(Vector3 Loc, out Vector3 target)
        {
            double baseTime = owner.NowTime;
            owner.GlobalVar.SetGlobalVar("PinRange", owner.parameter.PinRange.ToString("F1"));
            MatchOutline outLine = null;
            target = new Vector3();
            InspItem.Run();
            var 圓心X = 粗定位.ModuResult.GetOutputFloat("X").pFloatVal;
            var 圓心Y = 粗定位.ModuResult.GetOutputFloat("Y").pFloatVal;
            var 圓半徑 = 粗定位.ModuResult.GetOutputFloat("半徑").pFloatVal;
            Circle Cir = (圓心X == null) ? null : new Circle(new VM.PlatformSDKCS.PointF(圓心X[0], 圓心Y[0]), 圓半徑[0]);
            var Box = 吸嘴方向.ModuResult.GetOutputBoxArray("Box").FirstOrDefault();
            bool success = CheckSuccess(out outLine);
            double InspTime = owner.NowTime - baseTime;
            owner.BeginInvoke(new Action<VmRenderControl, bool, Circle, RectBox, MatchOutline, double>(ShowResult), win, success, Cir, Box, outLine, InspTime);
            //ShowResult(win, success, Cir, Box, outLine, InspTime);
            return success;
        }

        bool CheckSuccess(out MatchOutline outLine)
        {
            var AllInsp = InspItem.Modules.OfType<IMVSContourMatchModuCs.IMVSContourMatchModuTool>().ToList();
            bool success = false;
            outLine = null;
            var dEmpty = AllInsp.FirstOrDefault(n => n.Name == "Empty");
            if (dEmpty.ModuResult.MatchNum > 0)
            {
                outLine = dEmpty.ModuResult.MatchOutline;
                return false;
            }
            else
            {
                AllInsp.Remove(dEmpty);
                foreach(var P in AllInsp)
                {
                    success = P.ModuResult.MatchNum > 0;
                    if (success)
                    {
                        outLine = P.ModuResult.MatchOutline;
                        break;
                    }
                }
                return success;
            }
        }
        /// <summary>吸嘴校正初始化，會將分析資料輸出為像素位置</summary>
        public void CarbInit()
        {

        }
        /// <summary>吸嘴校正，依序傳入 0度X1Y1 / 90度X1Y1 / 90度X2Y1 / 90度X2Y2</summary>
        public void Carb(List<Vector3> ImageLoc, List<Vector3> axisLoc)
        {

        }

        string GetColorStr(Color color)
        { return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B); }

        void ShowResult(VmRenderControl Win, bool success, Circle 圓心, RectBox Box, MatchOutline Match, double inspTime)
        {
            if (圓心 != null)
            {
                string scolor = (success) ? GetColorStr(Color.Orange) : GetColorStr(Color.Red);
                CircleEx ex = new CircleEx(new System.Windows.Point(圓心.CenterPoint.X, 圓心.CenterPoint.Y),
                    圓心.Radius, 1, scolor, null, 3);
                win.DrawShape(ex);
            }
            if (Box != null)
            {
                string scolor = (success) ? GetColorStr(Color.Green) : GetColorStr(Color.Red);
                RectangleEx ex = new RectangleEx(new System.Windows.Point(Box.CenterPoint.X, Box.CenterPoint.Y),
                    Box.BoxWidth, Box.BoxHeight, Box.Angle, 0, 0, 1, scolor, null, 3);
                win.DrawShape(ex);
            }
            if (Match != null)
            {
                string scolor = (success) ? GetColorStr(Color.Green) : GetColorStr(Color.Red);
                PointSetEx ex = new PointSetEx();
                ex.PointList = Match.MatchOutlinePoints.Select(n => new System.Windows.Point(n.MatchOutlineX, n.MatchOutlineY)).ToList();
                ex.PointNum = Match.MatchOutlinePoints.Count;
                win.DrawShape(ex);
            }
            string str = string.Format("分析時間:{0:F2}", inspTime);
            TextEx ET = new TextEx(str, new System.Windows.Point(200, 200),
                16, 1, GetColorStr(Color.Orange));
            win.DrawShape(ET);

            Task.Factory.StartNew(owner.SaveRender, new object[] { win, "吸嘴" });
        }

    }

    #endregion

    #region Socket
    public class InspSocket區
    {
        Inspector owner;
        VmProcedure TestSocket;
        VmRenderControl win;

        public InspSocket區(Inspector sender, VmRenderControl window)
        {
            owner = sender;
            win = window;
            TestSocket = (VmProcedure)owner.solution["TestSocket"];
            owner.SetWindowDefault(win);
            //win.ChangeTopBarVisibility(false);
            //win.CoordinateInfoVisible = false;
            //win.SetBackground("#000000");
        }
        /// <summary>分析Socket盤，，傳入目前Socket軸位置(X / Y)，回覆針孔位置</summary>
        public List<Vector3> Insp(Vector3 Loc)
        {
            return new List<Vector3>();
        }
        /// <summary>Socket盤校正初始化，會將分析資料輸出為像素位置</summary>
        public void CarbInit()
        {

        }
        /// <summary>吸嘴校正，依序傳入Socket分析後第一筆資料 X1Y1 / X2Y1 / X2Y2</summary>
        public void Carb(List<Vector3> ImageLoc, List<Vector3> axisLoc)
        {

        }

        void ShowResult(VmRenderControl Win, ImageBaseData image)
        {
        }

    }

    #endregion

    #region CCD5
    public class InspCCD5區
    {
        Inspector owner;
        //VmProcedure TestSocket;
        VmRenderControl win;

        public InspCCD5區(Inspector sender, VmRenderControl window)
        {
            owner = sender;
            win = window;
            owner.SetWindowDefault(win);
            //win.ChangeTopBarVisibility(false);
            //win.CoordinateInfoVisible = false;
            //win.SetBackground("#000000");
        }

        public void Insp() { }

        void ShowResult(VmRenderControl Win, ImageBaseData image)
        {
        }

    }

    #endregion

    #region CCD6
    public class InspCCD6區
    {
        Inspector owner;
        //VmProcedure TestSocket;
        VmRenderControl win;

        public InspCCD6區(Inspector sender, VmRenderControl window)
        {
            owner = sender;
            win = window;
            owner.SetWindowDefault(win);
            //win.ChangeTopBarVisibility(false);
            //win.CoordinateInfoVisible = false;
            //win.SetBackground("#000000");
        }

        public void Insp() { }

        void ShowResult(VmRenderControl Win, ImageBaseData image)
        {
        }

    }

    #endregion

}
