using ic4;
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
    public partial class IMSSetFrm : Form
    {
        ImageSource ccd = null;
        string PreRotate = "";

        public IMSSetFrm()
        {
            InitializeComponent();
        }

        public static void SetParam(ImageSource sender)
        {
            using(IMSSetFrm Frm = new IMSSetFrm())
            {
                Frm.ccd = sender;
                Frm.ShowDialog();
            }
        }

        private void MVSAreaGESetFrm_Load(object sender, EventArgs e)
        {
            KeyList.Items.AddRange(ImageSource.AllDevice.Select(x => x.ModelName + ":" + x.Serial).ToArray());
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
                Thread.Sleep(100);
                this.BeginInvoke(new Action<string>(ccd.SaveParam), "");
            });
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            ccd.ListDevice(true);
            KeyList.Items.Clear();
            KeyList.Items.AddRange(ImageSource.AllDevice.Select(x => x.ModelName + ":" + x.Serial).ToArray());
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

    #region ImageSource CCD Control For ICdotnet4
    /// <summary>ImageSource CCD Control</summary>
    public class ImageSource
    {
        #region 共用參數
        static Task listAll;
        public static DeviceInfo[] AllDevice;
        static bool isListDevice = false;
        static object dLockDevice = new object();
        public void ListDevice(bool wait)
        {
            if (!Library.IsInitialized)
                Library.Init();
            if ((listAll == null) || listAll.IsCompleted)
                listAll = Task.Factory.StartNew(() =>
                {
                    lock (dLockDevice)
                    {
                        try
                        {
                            AllDevice = DeviceEnum.Devices.ToArray();
                        }
                        catch { }
                        isListDevice = true;
                    }
                });
            if (wait)
                listAll.Wait();
        }
        #endregion

        Grabber grabber;
        QueueSink sink;
        List<ImageBuffer> Buf;
        PixelFormat pixelFormat;
        string defaultmode = "Stop", dConfigForder = "", CCDName = "CCD1";
        Task tryConnect = null;
        int dBufferCount = 8;
        bool ConnectSet = false, ProgramExit = false, AutoReconnect = false, _isOpen = false;
        public ParamDefine Param = new ParamDefine();
        public bool IsGrabbing
        {
            get
            {
                if ((grabber == null) || (!grabber.IsAcquisitionActive))
                    return false;
                else
                    return true;
            }
        }
        public bool isConnected
        {
            get { return (grabber != null) && grabber.IsDeviceOpen; }
        }
        /// <summary>影像實際大小</summary>
        public int Width = 640;
        /// <summary>影像實際大小</summary>
        public int Height = 480;
        //public MvGvspPixelType ImageType  = MvGvspPixelType.PixelType_Gvsp_Mono8;
        //List<Tuple<IntPtr, MV_FRAME_OUT_INFO_EX>> Buf = new List<Tuple<IntPtr, MV_FRAME_OUT_INFO_EX>>();
        object dLockImage = new object();
        IntPtr PaintHwnd = IntPtr.Zero;
        public int ErrCount = 0, GrabCount = 0, onRecvCount = 0;
        /// <summary> ImageWidth, ImageHeight, ImageBufferPtr </summary>
        Action<object, string, int, int, IntPtr> OnRecv = null;
        double ExposureMin, ExposureMax, GammaMin, GammaMax, SpeedRateMin, SpeedRateMax;
        double PreShow = 0;
        public List<string> Sources = new List<string>();
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer() { Interval = 100, Enabled = true };
        Rectangle rectControl;
        /// <summary>Grabber Callback</summary>
        void onImageGrabbed(object sender, QueueSinkEventArgs arg)
        {
            ImageBuffer buffer;
            if (ProgramExit)
                return;
            if (arg.Sink.TryPopOutputBuffer(out buffer))
            {
                GrabCount = (GrabCount + 1) % 1000;
                Width = buffer.ImageType.Width;
                Height = buffer.ImageType.Height;
                pixelFormat = buffer.ImageType.PixelFormat;
                lock (dLockImage)
                    Buf.Add(buffer);
            }
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
        public ImageSource(string ccdName, string DefaultMode, string ConfigForder, int BufferCount, Action<object, string, int, int, IntPtr> onImage, IntPtr paintHwnd)
        {
            CCDName = ccdName;
            PaintHwnd = paintHwnd;
            if (PaintHwnd != IntPtr.Zero)
                rectControl = Control.FromHandle(PaintHwnd).DisplayRectangle;
            defaultmode = DefaultMode;
            dBufferCount = BufferCount;
            dConfigForder = Path.GetFullPath(ConfigForder);
            if (!dConfigForder.EndsWith("\\"))
                dConfigForder += "\\";
            OnRecv = onImage;
            ListDevice(false);
            Buf = new List<ImageBuffer>();
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
            DeviceInfo Temp = null;
            Grabber gTemp = null;
            while (!ProgramExit)
            {
                if (Param.Key != "")
                {
                    Temp = AllDevice.FirstOrDefault(x => (x.ModelName + ":" + x.Serial) == Param.Key);
                    if (Temp != null)
                        try
                        {
                            gTemp = new Grabber(Temp);
                            break;
                        }
                        catch (Exception ex)
                        {

                        }
                }
                Thread.Sleep(5000);
                if ((!ProgramExit) && (AllDevice.Length == 0))
                    ListDevice(true);
            }
            grabber = gTemp;
            if (sink == null)
            {
                sink = new QueueSink();
                sink.FramesQueued += onImageGrabbed;
            }
            
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
                if ((Temp != null) && (Temp.Ptr != IntPtr.Zero))
                {
                    onRecvCount = (onRecvCount + 1) % 1000;
                    if (OnRecv != null)
                        OnRecv(this, Temp.ImageType.PixelFormat.ToString(), Width, Height, Temp.Ptr);
                    if ((Buf.Count == 0) && (PaintHwnd != IntPtr.Zero))
                        DrawToControl(Temp);
                    grabber.DevicePropertyMap.ConnectChunkData(Temp);
                    Temp.Dispose();
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

        double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        void DrawToControl(ImageBuffer p)
        {
            if ((NowTime - PreShow) < 0.1)
                return;
            PreShow = NowTime;
            using (Bitmap dt = p.CreateBitmapCopy())
            {
                Graphics g = Graphics.FromHwnd(PaintHwnd);
                Rectangle rs = new Rectangle(0, 0, dt.Width, dt.Height);
                g.ResetTransform();
                switch ((int)-Param.ImageRotate)
                {
                    case -270:
                    case 90:
                        g.TranslateTransform(rectControl.Width, 0);
                        break;
                    case -180:
                    case 180:
                        g.TranslateTransform(rectControl.Width, rectControl.Height);
                        break;
                    case -90:
                    case 270:
                        g.TranslateTransform(0, rectControl.Height);
                        break;
                    default:
                        break;
                }
                g.RotateTransform((float)-Param.ImageRotate);
                if (((int)Math.Abs(Param.ImageRotate) % 180) == 0)
                    g.ScaleTransform(rectControl.Width / 1.0f / dt.Width, rectControl.Height / 1.0f / dt.Height);
                else
                    g.ScaleTransform(rectControl.Height / 1.0f / dt.Width, rectControl.Width / 1.0f / dt.Height);
                g.DrawImage(dt, 0, 0);
            }
        }

        void setDefaultParam()
        {
            //_onReceive = onImageGrabbed;
            //dev.SetIntValue("GevHeartbeatTimeout", 3000);
            //dev.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            //dev.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            //LoadConfig(dConfigForder + CCDName + ".cfg");
            var boolProp = (PropEnumeration)grabber.DevicePropertyMap.All.FirstOrDefault(x => x.Name == "ExposureAuto");
            if (boolProp != null)
                boolProp.Value = "Off";
            Exposure = Param.Exposure;
            GammaEnable = true;
            Gamma = Param.Gamma;
            MirrorX = Param.MirrorX;
            MirrorY = Param.MirrorY;
            Binning = Param.Binning;
            SpeedRate = Param.SpeedRate;
            TriggerMode = defaultmode;
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
            Width = (int)grabber.DevicePropertyMap.GetValueLong(PropId.Width);
            Height = (int)grabber.DevicePropertyMap.GetValueLong(PropId.Height);
            var dProp = (PropFloat)grabber.DevicePropertyMap.All.FirstOrDefault(x => x.Name == "ExposureTime");
            if (dProp != null)
            {
                ExposureMin = dProp.Minimum;
                ExposureMax = dProp.Maximum;
            }
            dProp = (PropFloat)grabber.DevicePropertyMap.All.FirstOrDefault(x => x.Name == "Gamma");
            if (dProp != null)
            {
                GammaMin = dProp.Minimum;
                GammaMax = dProp.Maximum;
            }
            dProp = (PropFloat)grabber.DevicePropertyMap.All.FirstOrDefault(x => x.Name == "AcquisitionFrameRate");

            if (dProp != null)
            {
                SpeedRateMin = dProp.Minimum;
                SpeedRateMax = dProp.Maximum;
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
                if (grabber.DevicePropertyMap.GetValueString("TriggerMode") == "Off")
                    return "FreeRun";
                return grabber.DevicePropertyMap.GetValueString("TriggerSource");
            }
            set
            {
                if ((grabber == null) || (!isConnected))
                    return;
                PropEnumeration tMode, tSec;
                switch (value)
                {
                    case "Stop":
                        Stop();
                        break;
                    case "FreeRun":
                        grabber.DevicePropertyMap.SetValue(PropId.TriggerMode, "Off");
                        Start();
                        break;
                    default:
                        Stop();
                        tMode = grabber.DevicePropertyMap.FindEnumeration("TriggerMode");
                        tSec = grabber.DevicePropertyMap.FindEnumeration("TriggerSource");
                        grabber.DevicePropertyMap.SetValue(PropId.TriggerMode, "On");
                        grabber.DevicePropertyMap.SetValue(PropId.TriggerSource, value);
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

            grabber.StreamSetup(sink, StreamSetupOption.AcquisitionStart);
            if (!grabber.IsAcquisitionActive)
                grabber.AcquisitionStart();
            //grabber.DevicePropertyMap.ExecuteCommand(PropId.AcquisitionStart);
            GrabCount = 0;
            onRecvCount = 0;
        }
        /// <summary>停止Grabber</summary>
        void Stop()
        {
            if (!isConnected)
                return;
            grabber.StreamStop();
            grabber.AcquisitionStop();
        }
        /// <summary>變更為Software模式並取單張圖</summary>
        public void SoftTrigger()
        {
            if (!isConnected)
                return;
            if (TriggerMode != "Software")
                TriggerMode = "Software";
            //if (!IsGrabbing)
            //    Start();
            grabber.DevicePropertyMap.ExecuteCommand(PropId.TriggerSoftware);
            //grabber.DevicePropertyMap.SetValue("TriggerSoftware", "Software");
        }

        void onExit(object sender, EventArgs e)
        {
            ProgramExit = true;
            Thread.Sleep(50);
            Close();
        }
        /// <summary>關閉Camera</summary>
        public void Close()
        {
            if ((grabber == null) || (!grabber.IsDeviceOpen))
                return;
            Stop();
            //lock (dLockImage)
            //    Buf.Clear();
            _isOpen = false;
            grabber.DeviceClose();
        }

        public void SetParam()
        {
            IMSSetFrm.SetParam(this);
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
                if (!isConnected)
                    return Param.Exposure;
                return  (float)grabber.DevicePropertyMap.GetValueDouble("ExposureTime");
            }
            set
            {
                if (isConnected)
                {
                    var dProp = (PropFloat)grabber.DevicePropertyMap.All.FirstOrDefault(x => x.Name == "ExposureTime");
                    double V = Math.Min(Math.Max(value, dProp.Minimum), dProp.Maximum);
                    grabber.DevicePropertyMap.SetValue(PropId.ExposureTime, V);
                }
            }
        }
        /// <summary>臨時變更GammaEnable</summary>
        public bool GammaEnable
        {
            get
            {
                if (!isConnected)
                    return false;
                PropBoolean dEn;
                if (grabber.DevicePropertyMap.TryFindBoolean("GammaEnable", out dEn))
                    return dEn.Value;
                else
                    return true;
            }
            set
            {
                if (!isConnected)
                    return;
                PropBoolean dEn;
                if (grabber.DevicePropertyMap.TryFindBoolean("GammaEnable", out dEn))
                    dEn.Value = value;
            }
        }
        /// <summary>臨時變更Gamma</summary>
        public double Gamma
        {
            get
            {
                PropFloat dProp;
                if (isConnected && grabber.DevicePropertyMap.TryFindFloat("Gamma", out dProp))
                    return dProp.Value;
                else
                    return Param.Gamma;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, GammaMin), GammaMax);
                    PropFloat dProp;
                    if (isConnected && grabber.DevicePropertyMap.TryFindFloat("Gamma", out dProp))
                        dProp.Value = V;
                }
            }
        }
        /// <summary>臨時變更FrameRate</summary>
        public double SpeedRate
        {
            get
            {
                PropFloat dProp;
                if (isConnected && grabber.DevicePropertyMap.TryFindFloat("AcquisitionFrameRate", out dProp))
                    return dProp.Value;
                else
                    return Param.SpeedRate;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, SpeedRateMin), SpeedRateMax);
                    if (value < SpeedRateMin)
                        V = SpeedRateMax;
                    PropFloat dProp;
                    if (isConnected && grabber.DevicePropertyMap.TryFindFloat("AcquisitionFrameRate", out dProp))
                        dProp.Value = V;
                }
            }
        }
        /// <summary>臨時變更MirrorX</summary>
        public bool MirrorX
        {
            get
            {
                PropBoolean dProp;
                if (isConnected && grabber.DevicePropertyMap.TryFindBoolean("ReverseX", out dProp))
                    return dProp.Value;
                else
                    return Param.MirrorX;
            }
            set
            {
                if (isConnected)
                {
                    var dProp = (PropBoolean)grabber.DevicePropertyMap.All.FirstOrDefault(x => x.Name == "ReverseX");
                    if (isConnected && (dProp != null))
                        dProp.Value = value;
                }
            }
        }
        /// <summary>臨時變更MirrorY</summary>
        public bool MirrorY
        {
            get
            {
                PropBoolean dProp;
                if (isConnected && grabber.DevicePropertyMap.TryFindBoolean("ReverseY", out dProp))
                    return dProp.Value;
                else
                    return Param.MirrorY;
            }
            set
            {
                if (isConnected)
                {
                    PropBoolean dProp;
                    if (isConnected && grabber.DevicePropertyMap.TryFindBoolean("ReverseY", out dProp))
                        dProp.Value = value;
                }
            }
        }
        /// <summary>停止時臨時變更Binning</summary>
        public bool Binning
        {
            get
            {
                PropInteger dPropH, dPropV;
                grabber.DevicePropertyMap.TryFindInteger("BinningHorizontal", out dPropH);
                grabber.DevicePropertyMap.TryFindInteger("BinningVertical", out dPropV);
                if (isConnected && (dPropH != null) && (dPropV != null))
                    return dPropH.Value == 1;
                else
                    return Param.Binning;
            }
            set
            {
                PropInteger dPropH, dPropV;
                grabber.DevicePropertyMap.TryFindInteger("BinningHorizontal", out dPropH);
                grabber.DevicePropertyMap.TryFindInteger("BinningVertical", out dPropV);
                if (isConnected && (dPropH != null) && (dPropV != null))
                {
                    dPropH.Value = (value) ? 2 : 1;
                    dPropV.Value = (value) ? 2 : 1;
                }
            }
        }
        /// <summary>載入設定檔</summary>
        public void LoadConfig(string CfgFileName)
        {
            string FName = (CfgFileName == "") ? dConfigForder + CCDName + ".xml" : CfgFileName;
            if ((FName != "") && File.Exists(FName) && (grabber != null) && _isOpen)
            {
                grabber.DeviceClose();
                grabber.DeviceOpenFromState(FName);
            }
        }
        /// <summary>儲存設定檔, 不設定名稱時為使用者Dialog自行設定</summary>
        public void SaveConfig(string CfgFileName)
        {
            if ((grabber != null) && _isOpen)
            {
                string FName = (CfgFileName == "") ? dConfigForder + CCDName + ".xml" : CfgFileName;
                grabber.DeviceSaveState(FName);
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
