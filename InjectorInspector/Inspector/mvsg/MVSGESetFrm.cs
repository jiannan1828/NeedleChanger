using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Camera
{
    public partial class MVSAreaGESetFrm : Form
    {
        MVSAreaGE ccd = null;
        string PreRotate = "";

        public MVSAreaGESetFrm()
        {
            InitializeComponent();
        }

        public static void SetParam(MVSAreaGE sender)
        {
            using(MVSAreaGESetFrm Frm = new MVSAreaGESetFrm())
            {
                Frm.ccd = sender;
                Frm.ShowDialog();
            }
        }

        private void MVSAreaGESetFrm_Load(object sender, EventArgs e)
        {
            KeyList.Items.AddRange(MVSAreaGE.AllDevice.Select(x => x.chManufacturerName + ":" + x.chSerialNumber).ToArray());
            KeyList.Text = ccd.Param.Key;
            GammaBar.Value = (int)(ccd.Param.Gamma * 10);
            lb_Gamma.Text = ccd.Param.Gamma.ToString("F1");
            ExposureBar.Value = (int)ccd.Param.Exposure;
            lb_Exposure.Text = ccd.Param.Exposure.ToString("F0");
            ed_Speed.Text = ccd.Param.SpeedRate.ToString("F1");
            cb_Rotate.Text = ccd.Param.ImageRotate.ToString("F0");
            ck_MirrorX.Checked = ccd.Param.MirrorX;
            ck_MirrorY.Checked = ccd.Param.MirrorY;
            ck_Binning.Enabled = !ccd.IsGrabbing;
            ck_Binning.Checked = ccd.Param.Binning;
            ed_ScaleX.Text = ccd.Param.ScaleX.ToString("F7");
            ed_ScaleY.Text = ccd.Param.ScaleY.ToString("F7");
            ed_Dxx.Text = ccd.Param.vDx_X.ToString("F7");
            ed_Dxy.Text = ccd.Param.vDx_Y.ToString("F7");
            ed_Dyx.Text = ccd.Param.vDy_X.ToString("F7");
            ed_Dyy.Text = ccd.Param.vDy_Y.ToString("F7");
        }

        private void btn_SaveConfig_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                this.BeginInvoke(new Action<string>(ccd.SaveConfig), "");
            });
        }

        private void btn_SaveParam_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                this.BeginInvoke(new Action<string>(ccd.SaveParam), "");
            });
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            MVSAreaGE.ListDevice(true);
            KeyList.Items.Clear();
            KeyList.Items.AddRange(MVSAreaGE.AllDevice.Select(x => x.chManufacturerName + ":" + x.chSerialNumber).ToArray());
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            if (KeyList.Text != "")
            {
                if (ccd.isConnected)
                    ccd.Close();
                ccd.Param.Key = KeyList.Text;
                //ccd.ReConnect();
            }
        }

        private void GammaBar_Scroll(object sender, ScrollEventArgs e)
        {
            ccd.Param.Gamma = GammaBar.Value / 10.0;
            ccd.Gamma = ccd.Param.Gamma;
            lb_Gamma.Text = ccd.Param.Gamma.ToString("F1");
        }

        private void ExposureBar_Scroll(object sender, ScrollEventArgs e)
        {
            ccd.Param.Exposure = ExposureBar.Value;
            ccd.Exposure = ccd.Param.Exposure;
            lb_Exposure.Text = ccd.Param.Exposure.ToString("F0");
        }

        private void ed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r') return;
            double v = 0;
            TextBox g = (TextBox)sender;
            if (!double.TryParse(g.Text, out v))
                return;
            switch (g.Name)
            {
                case "ed_Speed":
                    ccd.Param.SpeedRate = v;
                    ccd.SpeedRate = v;
                    break;
                case "ed_Rotate":
                    ccd.Param.ImageRotate = v;
                    break;
                case "ed_ScaleX":
                    ccd.Param.ScaleX = v;
                    break;
                case "ed_ScaleY":
                    ccd.Param.ScaleY = v;
                    break;
                case "ed_Dxx":
                    ccd.Param.vDx_X = v;
                    break;
                case "ed_Dxy":
                    ccd.Param.vDx_Y = v;
                    break;
                case "ed_Dyx":
                    ccd.Param.vDy_X = v;
                    break;
                case "ed_Dyy":
                    ccd.Param.vDy_Y = v;
                    break;
            }
        }

        private void ck_MirrorX_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                bool v = ck_MirrorX.Checked;
                ccd.Param.MirrorX = v;
                ccd.MirrorX = v;
            });
        }

        private void ck_MirrorY_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                bool v = ck_MirrorY.Checked;
                ccd.Param.MirrorY = v;
                ccd.MirrorY = v;
            });
        }

        private void ck_Binning_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                bool v = ck_Binning.Checked;
                ccd.Param.Binning = v;
                ccd.Binning = v;
            });
        }

        private void cb_Rotate_DropDown(object sender, EventArgs e)
        {
            PreRotate = cb_Rotate.Text;
        }

        private void cb_Rotate_DropDownClosed(object sender, EventArgs e)
        {
            object dt = cb_Rotate.SelectedItem;
            if ((dt != null) && (PreRotate != dt.ToString()))
            {
                double v = 0;
                if (double.TryParse(cb_Rotate.SelectedItem.ToString(), out v))
                    ccd.Param.ImageRotate = v;
            }
        }
    }

    #region MVSAreaGE For MVS 3.4.3
    /// <summary>MVSAreaGE For MVS 3.4.3</summary>
    public class MVSAreaGE
    {
        #region 共用參數
        static Task listAll;
        public static List<CGigECameraInfo> AllDevice;
        static bool isListDevice = false;
        static object dLockDevice = new object();
        public static void ListDevice(bool wait)
        {
            if ((listAll == null) || listAll.IsCompleted)
                listAll = Task.Factory.StartNew(() =>
                {
                    List<CCameraInfo> devs = new List<CCameraInfo>();
                    lock (dLockDevice)
                    {
                        int Ret = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref devs);
                        if (Ret == 0)
                        {
                            AllDevice = devs.Select(x => (CGigECameraInfo)x).ToList();
                            isListDevice = true;
                        }
                    }
                });
            if (wait)
                listAll.Wait();
        }
        #endregion

        public AutoResetEvent isGrabbed = new AutoResetEvent(false);
        CCamera dev = null;
        string defaultmode = "Stop", dConfigForder = "", CCDName = "CCD1";
        Task tryConnect = null;
        int dBufferCount = 8;
        cbOutputExdelegate _onReceive;
        cbExceptiondelegate _onExcept;
        CFloatValue cFloat = new CFloatValue();
        CIntValue cInt = new CIntValue();
        CEnumValue cEnu = new CEnumValue();
        CEnumEntry cEntry = new CEnumEntry();
        CImage cImg = new CImage();
        CDisplayFrameInfo cDisplay = new CDisplayFrameInfo();
        bool ConnectSet = false, ProgramExit = false, AutoReconnect = false, _isGrab = false, _isOpen = false;
        public ParamDefine Param = new ParamDefine();
        public bool IsGrabbing
        {
            get { return (dev != null) && _isGrab; }
        }
        public bool isConnected
        {
            get { return (dev != null) && dev.IsDeviceConnected(); }
        }
        /// <summary>影像實際大小</summary>
        public int Width = 640;
        /// <summary>影像實際大小</summary>
        public int Height = 480;
        public MvGvspPixelType ImageType  = MvGvspPixelType.PixelType_Gvsp_Mono8;
        List<Tuple<IntPtr, MV_FRAME_OUT_INFO_EX>> Buf = new List<Tuple<IntPtr, MV_FRAME_OUT_INFO_EX>>();
        object dLockImage = new object();
        IntPtr PaintHwnd = IntPtr.Zero;
        public int ErrCount = 0, GrabCount = 0, onRecvCount = 0;
        /// <summary> ImageWidth, ImageHeight, ImageBufferPtr </summary>
        Action<object, string, int, int, IntPtr> OnRecv = null;
        double ExposureMin, ExposureMax, GammaMin, GammaMax, SpeedRateMin, SpeedRateMax;
        public List<string> Sources = new List<string>();
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer() { Interval = 100, Enabled = true };
        Rectangle rectControl;
        /// <summary>Grabber Callback</summary>
        void onImageGrabbed(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            if (ProgramExit)
                return;
            GrabCount = (GrabCount + 1) % 1000;
            MV_FRAME_OUT_INFO_EX g = pFrameInfo;
            if ((g.nWidth > 0) && (g.nHeight > 0) && (pData != IntPtr.Zero) && (Buf.Count < 500))
            {
                Width = g.nWidth;
                Height = g.nHeight;
                ImageType = g.enPixelType;
                lock (dLockImage)
                    Buf.Add(new Tuple<IntPtr, MV_FRAME_OUT_INFO_EX>(pData, g));
            }
            else
                ErrCount = (ErrCount + 1) % 1000;
        }

        void onException(uint nMsgType, IntPtr pUser)
        {
            //if (nMsgType == 32769)
            Close();
        }
        /// <summary>外部使用，影像位置轉實際位置</summary>
        public PointF GetReal(double X, double Y, int W, int H, bool Dxy = true)
        {
            PointF ret = new PointF(0, 0);
            double X1, Y1;

            X1 = (X - (W / 2.0));
            Y1 = (Y - (H / 2.0));
            double Tx = Param.vDy_Y * X1 + Param.vDy_X * Y1;
            double Ty = Param.vDx_X * Y1 + Param.vDx_Y * X1;
            if (Dxy)
                ret = new PointF((float)(Tx * Param.ScaleX), (float)(Ty * Param.ScaleY));
            else
                ret = new PointF((float)(X1 * Param.ScaleX), (float)(Y1 * Param.ScaleY));
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ccdName">CCD設定檔名稱</param>
        /// <param name="DefaultMode">CCD連線後預設取像模式</param>
        /// <param name="ConfigForder">CCD設定檔預設目錄</param>
        /// <param name="BufferCount"></param>
        /// <param name="onImage">取圖Callback(sender, imageFormat, Width, Height, ImageBuffer) 由內部Timer呼叫</param>
        /// <param name="paintHwnd">取圖後顯示在哪個視窗，Intptr.Zero為不顯示</param>
        public MVSAreaGE(string ccdName, string DefaultMode, string ConfigForder, int BufferCount, Action<object, string, int, int, IntPtr> onImage, IntPtr paintHwnd)
        {
            PaintHwnd = paintHwnd;
            if (PaintHwnd != IntPtr.Zero)
            {
                rectControl = Control.FromHandle(PaintHwnd).DisplayRectangle;
                cDisplay.WindowHandle = PaintHwnd;
                cDisplay.Image = cImg;
            }
            CCDName = ccdName;
            defaultmode = DefaultMode;
            dBufferCount = BufferCount;
            dConfigForder = Path.GetFullPath(ConfigForder);
            if (!dConfigForder.EndsWith("\\"))
                dConfigForder += "\\";
            OnRecv = onImage;
            ListDevice(false);
            LoadParam();
            ReConnect();
            timer1.Tick += timerTick;
            Application.ApplicationExit += onExit;
        }
        /// <summary>嘗試開啟CCD, 並將狀態變更為預設狀態</summary>
        public void ReConnect()
        {
            if ((tryConnect == null) || tryConnect.IsCompleted)
                tryConnect = Task.Factory.StartNew(doConnect);
        }

        void doConnect()
        {
            _isOpen = false;
            while (!isListDevice)
                Thread.Sleep(500);
            CCamera Temp = new CCamera();
            while (true)
            {
                if (Param.Key != "")
                {
                    CCameraInfo target = (CCameraInfo)AllDevice.FirstOrDefault(x => (x.chManufacturerName + ":" + x.chSerialNumber) == Param.Key);
                    if (target != null)
                        if (CSystem.IsDeviceAccessible(ref target, MV_ACCESS_MODE.MV_ACCESS_CONTROL))
                            if (Temp.CreateHandle(ref target) == 0)
                            {
                                if (Temp.OpenDevice() == 0)
                                    break;
                                else
                                    Temp.DestroyHandle();
                            }
                }
                Thread.Sleep(5000);
                if (AllDevice.Count == 0)
                    ListDevice(true);
            }
            dev = Temp;
            _isOpen = true;
            ConnectSet = true;
            while(isConnected)
                Thread.Sleep(1000);
            Thread.Sleep(2000);
            AutoReconnect = true;
        }

        void timerTick(object sender, EventArgs e)
        {
            if (ProgramExit)
                return;
            if (AutoReconnect)
            {
                AutoReconnect = false;
                ReConnect();
            }
            while (isConnected && (Buf.Count > 0))
            {
                var Temp = Buf[0];
                lock (dLockImage)
                    Buf.RemoveAt(0);
                if (Temp.Item1 != IntPtr.Zero)
                {
                    onRecvCount = (onRecvCount + 1) % 1000;
                    if (OnRecv != null)
                        OnRecv(this, ImageType.ToString(), Width, Height, Temp.Item1);
                    isGrabbed.Set();
                    if ((Buf.Count == 0) && (PaintHwnd != IntPtr.Zero))
                        DrawToControl(Temp);
                }
                else
                    ErrCount = (ErrCount + 1) % 1000;
            }
            if (ConnectSet)
            {
                ConnectSet = false;
                getAllParam();
                setDefaultParam();
            }
        }

        void DrawToControl(Tuple<IntPtr, MV_FRAME_OUT_INFO_EX> Temp)
        {
            cImg.UpdateImageInfo(Temp.Item1, Temp.Item2.nFrameLen, Temp.Item2.nHeight, Temp.Item2.nWidth, Temp.Item2.enPixelType);
            dev.DisplayOneFrame(ref cDisplay);
        }

        void setDefaultParam()
        {
            getCLC_Paramter();
            _onReceive = onImageGrabbed;
            GC.KeepAlive(_onReceive);   //必須!
            _onExcept = onException;
            GC.KeepAlive(_onExcept);
            dev.RegisterImageCallBackEx(_onReceive, IntPtr.Zero);
            dev.RegisterExceptionCallBack(_onExcept, IntPtr.Zero);
            dev.SetIntValue("GevHeartbeatTimeout", 3000);
            dev.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            dev.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            LoadConfig(dConfigForder + CCDName + ".cfg");
            Exposure = Param.Exposure;
            GammaEnable = true;
            Gamma = Param.Gamma;
            MirrorX = Param.MirrorX;
            MirrorY = Param.MirrorY;
            Binning = Param.Binning;
            SpeedRate = Param.SpeedRate;
            TriggerMode = defaultmode;
        }

        List<string> GetEnumList(string key)
        {
            List<string> rst = new List<string>();
            dev.GetEnumValue(key, ref cEnu);
            for (int i = 0; i < cEnu.SupportedNum; i++)
            {
                cEntry.Value = cEnu.SupportValue[i];
                dev.GetEnumEntrySymbolic(key, ref cEntry);
                Sources.Add(cEntry.Symbolic);
            }
            return rst;
        }

        void getCLC_Paramter()
        {
            Sources = GetEnumList("TriggerSource");
            //dev.GetEnumValue("TriggerSource", ref cEnu);
            //Sources.Clear();
            //for(int i = 0; i < cEnu.SupportedNum; i++)
            //{
            //    cEntry.Value = cEnu.SupportValue[i];
            //    dev.GetEnumEntrySymbolic("TriggerSource", ref cEntry);
            //    Sources.Add(cEntry.Symbolic);
            //}
        }

        void RemoveAllEventList(object obj, string eventName)
        {
            Type objTyp = obj.GetType();
            var fields = objTyp.GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
            EventInfo infos = objTyp.GetEvent(eventName);
            var values = (Delegate)fields.GetValue(obj);
            if ((values != null) && (infos != null))
                foreach (var P in values.GetInvocationList())
                    infos.RemoveEventHandler(obj, P);
        }

        void getAllParam()
        {
            if (dev.GetIntValue("Width", ref cInt) == 0) Width = (int)cInt.CurValue;
            if (dev.GetIntValue("Height", ref cInt) == 0) Height = (int)cInt.CurValue;
            if (dev.GetFloatValue("ExposureTime", ref cFloat) == 0)
            {
                ExposureMin = cFloat.Min;
                ExposureMax = cFloat.Max;
            }
            if (dev.GetFloatValue("Gamma", ref cFloat) == 0)
            {
                GammaMin = cFloat.Min;
                GammaMax = cFloat.Max;
            }
            if (dev.GetFloatValue("AcquisitionFrameRate", ref cFloat) == 0)
            {
                SpeedRateMin = cFloat.Min;
                SpeedRateMax = cFloat.Max;
            }
        }
        /// <summary>變更取圖模式, Stop / FreeRun / Software / Line0 / Line1 / Line2 / Line3 / Line4 / Line5</summary>
        public string TriggerMode
        {
            get
            {
                if (!isConnected)
                    return "DisConnect";
                if (!IsGrabbing)
                    return "Stop";
                dev.GetEnumValue("TriggerMode", ref cEnu);
                if (cEnu.CurValue == (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF)
                    return "FreeRun";
                dev.GetEnumValue("TriggerSource", ref cEnu);
                cEntry.Value = cEnu.CurValue;
                dev.GetEnumEntrySymbolic("TriggerSource", ref cEntry);
                return cEntry.Symbolic;
            }
            set
            {
                if ((dev == null) || (!isConnected))
                    return;
                switch (value)
                {
                    case "Stop":
                        Stop();
                        break;
                    case "FreeRun":
                        dev.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                        Start();
                        break;
                    default:
                        dev.SetEnumValueByString("TriggerSource", value);
                        dev.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                        Start();
                        break;
                }
            }
        }
        /// <summary>啟動Grabber</summary>
        void Start()
        {
            if ((!isConnected) || IsGrabbing)
                return;
            _isGrab = (dev.StartGrabbing() == 0);
            GrabCount = 0;
            onRecvCount = 0;
        }
        /// <summary>停止Grabber</summary>
        void Stop()
        {
            if (!isConnected)
                return;
            dev.StopGrabbing();
            _isGrab = false;
        }
        /// <summary>變更為Software模式並取單張圖</summary>
        public void SoftTrigger()
        {
            if (!isConnected)
                return;
            if (TriggerMode != "Software")
                TriggerMode = "Software";
            if (!IsGrabbing)
                Start();
            dev.SetCommandValue("TriggerSoftware");
        }

        void onExit(object sender, EventArgs e)
        {
            ProgramExit = true;
            Close();
        }
        /// <summary>關閉Camera</summary>
        public void Close()
        {
            if ((dev == null) || (!_isOpen))
                return;
            Stop();
            lock (dLockImage)
                Buf.Clear();
            _isOpen = false;
            dev.CloseDevice();
            dev.DestroyHandle();
        }

        public void SetParam()
        {
            MVSAreaGESetFrm.SetParam(this);
        }
        /// <summary>載入指定參數或ConfigForder內固定參數</summary>
        public void LoadParam(string fullFileName = "")
        {
            string FName = (fullFileName == "") ? dConfigForder + CCDName + ".XML" : fullFileName;
            if (File.Exists(FName))
                using (MemoryStream fs = new MemoryStream(File.ReadAllBytes(FName)))
                {
                    XmlSerializer XML = new XmlSerializer(typeof(ParamDefine));
                    Param = (ParamDefine)XML.Deserialize(fs);
                }
        }
        /// <summary>儲存指定參數或ConfigForder內固定參數</summary>
        public void SaveParam(string fullFileName = "")
        {
            string FName = (fullFileName == "") ? dConfigForder + CCDName + ".XML" : fullFileName;
            XmlSerializer XML = new XmlSerializer(typeof(ParamDefine));
            using (MemoryStream fs = new MemoryStream())
            {
                XML.Serialize(fs, Param);
                File.WriteAllBytes(FName, fs.ToArray());
            }
        }
        /// <summary>臨時變更曝光時間</summary>
        public double Exposure
        {
            get
            {
                if (isConnected && (dev.GetFloatValue("ExposureTime", ref cFloat) == 0))
                    return cFloat.CurValue;
                else
                    return Param.Exposure;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, ExposureMin), ExposureMax);
                    dev.SetFloatValue("ExposureTime", (float)V);
                }
            }
        }
        /// <summary>臨時變更GammaEnable</summary>
        public bool GammaEnable
        {
            get
            {
                bool En = false;
                return (isConnected && (dev.GetBoolValue("GammaEnable", ref En) == 0)) ? En : false;
            }
            set
            {
                if (isConnected)
                {
                    dev.SetBoolValue("GammaEnable", value);
                }
            }
        }
        /// <summary>臨時變更Gamma</summary>
        public double Gamma
        {
            get
            {
                if (isConnected && (dev.GetFloatValue("Gamma", ref cFloat) == 0))
                    return cFloat.CurValue;
                else
                    return Param.Gamma;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, GammaMin), GammaMax);
                    dev.SetFloatValue("Gamma", (float)V);
                }
            }
        }
        /// <summary>臨時變更FrameRate</summary>
        public double SpeedRate
        {
            get
            {
                if (isConnected && (dev.GetFloatValue("AcquisitionFrameRate", ref cFloat) == 0))
                    return cFloat.CurValue;
                else
                    return Param.SpeedRate;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, SpeedRateMin), SpeedRateMax);
                    if (V >= 1)
                        dev.SetFloatValue("AcquisitionFrameRate", (float)V);
                    dev.SetBoolValue("AcquisitionFrameRateEnable", V >= 1);
                }
            }
        }
        /// <summary>臨時變更MirrorX</summary>
        public bool MirrorX
        {
            get
            {
                bool En = false;
                return (isConnected && (dev.GetBoolValue("ReverseX", ref En) == 0)) ? En : Param.MirrorX;
            }
            set
            {
                if (isConnected)
                    dev.SetBoolValue("ReverseX", value);
            }
        }
        /// <summary>臨時變更MirrorY</summary>
        public bool MirrorY
        {
            get
            {
                bool En = false;
                return (isConnected && (dev.GetBoolValue("ReverseY", ref En) == 0)) ? En : Param.MirrorY;
            }
            set
            {
                if (isConnected)
                    dev.SetBoolValue("ReverseY", value);
            }
        }
        /// <summary>停止時臨時變更Binning</summary>
        public bool Binning
        {
            get
            {
                if (!isConnected)
                    return false;
                dev.GetEnumValue("BinningHorizontal", ref cEnu);
                return cEnu.CurValue != 1;
            }
            set
            {
                if (isConnected)
                    dev.SetEnumValue("BinningHorizontal", value ? (uint)2 : 1);
            }
        }
        /// <summary>載入設定檔</summary>
        public void LoadConfig(string CfgFileName)
        {
            string FName = (CfgFileName == "") ? dConfigForder + CCDName + ".cfg" : CfgFileName;
            if ((FName != "") && File.Exists(FName) && (dev != null) && _isOpen)
                dev.FeatureLoad(FName);
        }
        /// <summary>儲存設定檔, 不設定名稱時為使用者Dialog自行設定</summary>
        public void SaveConfig(string CfgFileName)
        {
            if ((dev != null) && _isOpen)
            {
                string FName = (CfgFileName == "") ? dConfigForder + CCDName + ".cfg" : CfgFileName;
                dev.FeatureSave(FName);
            }
        }

        #region ParamDefine
        public class ParamDefine
        {
            public string Key = "";
            public double Gamma = 1;
            public double Exposure = 5000;
            public double SpeedRate = 0;
            public bool MirrorX = false;
            public bool MirrorY = false;
            public bool Binning = false;
            /// <summary>外部使用參數，影像旋轉角度</summary>
            public double ImageRotate = 0;
            /// <summary>外部使用參數，影像轉實際距離</summary>
            public double ScaleX = 0.001;
            /// <summary>外部使用參數，影像轉實際距離</summary>
            public double ScaleY = 0.001;
            /// <summary>外部使用參數，影像垂直度轉換</summary>
            public double vDx_X = 1;
            /// <summary>外部使用參數，影像垂直度轉換</summary>
            public double vDx_Y = 0;
            /// <summary>外部使用參數，影像垂直度轉換</summary>
            public double vDy_X = 0;
            /// <summary>外部使用參數，影像垂直度轉換</summary>
            public double vDy_Y = 1;
            public ParamDefine Clone()
            {
                return (ParamDefine)this.MemberwiseClone();
            }
        }
        #endregion
    }
    #endregion

}
