using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Camera
{
    public partial class SisoSetFrm : Form
    {
        SisoFGArea ccd = null;
        string PreRotate = "";

        public SisoSetFrm()
        {
            InitializeComponent();
        }

        public static void SetParam(SisoFGArea sender)
        {
            using(SisoSetFrm Frm = new SisoSetFrm())
            {
                Frm.ccd = sender;
                Frm.ShowDialog();
            }
        }

        public void MVSAreaGESetFrm_Load(object sender, EventArgs e)
        {
            KeyList.Items.AddRange(SisoGrabber.AllBoard.SelectMany(x => x.Devices.Where(n => n != "")).ToArray());
            KeyList.Text = ccd.Param.Key;
            if (ccd.isConnected)
            {
                GammaBar.Minimum = (int)ccd.GammaMin;
                GammaBar.Maximum = (int)ccd.GammaMax;
                GammaBar.Value = (int)(ccd.Param.Gamma);
            }
            lb_Gamma.Text = ccd.Param.Gamma.ToString("F0");
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

        public void btn_SaveConfig_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                this.BeginInvoke(new Action<string>(ccd.SaveConfig), "");
            });
        }

        public void btn_SaveParam_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                this.BeginInvoke(new Action<string>(ccd.SaveParam), "");
            });
        }

        public void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void btn_Search_Click(object sender, EventArgs e)
        {
            SisoGrabber.ListDevice(true);
            KeyList.Items.Clear();
            KeyList.Items.AddRange(SisoGrabber.AllBoard.SelectMany(x => x.Devices.Where(n => n != "")).ToArray());
        }

        public void btn_Set_Click(object sender, EventArgs e)
        {
            if (KeyList.Text != "")
            {
                if (ccd.isConnected)
                    ccd.Close();
                ccd.Param.Key = KeyList.Text;
                //ccd.ReConnect();
            }
        }

        public void GammaBar_Scroll(object sender, ScrollEventArgs e)
        {
            ccd.Param.Gamma = GammaBar.Value;
            ccd.Gamma = ccd.Param.Gamma;
            lb_Gamma.Text = ccd.Param.Gamma.ToString("F0");
        }

        public void ExposureBar_Scroll(object sender, ScrollEventArgs e)
        {
            ccd.Param.Exposure = ExposureBar.Value;
            ccd.Exposure = ccd.Param.Exposure;
            lb_Exposure.Text = ccd.Param.Exposure.ToString("F0");
        }

        public void ed_KeyPress(object sender, KeyPressEventArgs e)
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

        public void ck_MirrorX_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                bool v = ck_MirrorX.Checked;
                ccd.Param.MirrorX = v;
                ccd.MirrorX = v;
            });
        }

        public void ck_MirrorY_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                bool v = ck_MirrorY.Checked;
                ccd.Param.MirrorY = v;
                ccd.MirrorY = v;
            });
        }

        public void ck_Binning_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                bool v = ck_Binning.Checked;
                ccd.Param.Binning = v;
                ccd.Binning = v;
            });
        }

        public void cb_Rotate_DropDown(object sender, EventArgs e)
        {
            PreRotate = cb_Rotate.Text;
        }

        public void cb_Rotate_DropDownClosed(object sender, EventArgs e)
        {
            object dt = cb_Rotate.SelectedItem;
            if ((dt != null) && (PreRotate != dt.ToString()))
            {
                double v = 0;
                if (double.TryParse(cb_Rotate.SelectedItem.ToString(), out v))
                    ccd.Param.ImageRotate = v;
            }
        }

        public void btn_LoadConfig_Click(object sender, EventArgs e)
        {
            using(var P = new OpenFileDialog())
            {
                if (P.ShowDialog() == DialogResult.OK)
                {
                    var PreMode = ccd.TriggerMode;
                    ccd.TriggerMode = "Stop";
                    ccd.LoadConfig(P.FileName);
                    ccd.TriggerMode = PreMode;
                }
            }
        }
    }

    #region SisoGrabber
    public class SisoGrabber : IDisposable
    {
        public Fg_Struct handle = null;
        public SgcBoardHandle sgcHandle = null; 
        public uint Board = 0;
        public string Name = "", applets;
        int CXPPorts = 0, CLPorts = 0, CLHSPorts = 0, GIGEPorts = 0, MaxPorts = 0;
        int sgcCount = 0;
        string[] FGParams;
        int[] Format, Width, Height;
        bool[] isUse;
        internal SgcCameraHandle[] sgcCam = new SgcCameraHandle[4];
        public string[] Devices;
        List<string> ErrMsg = new List<string>();

        public bool IsGrabbing(uint index)
        {
            if (handle == null)
                return false;
            int V = 0;
            SiSoCsRt.Fg_getParameterWithInt(handle, SiSoCsRt.FG_DMASTATUS, out V, index);
            return V != 0;
        }

        public bool isConnected(uint index)
        {
            if (handle == null)
                return false;
            int V = 0;
            SiSoCsRt.Fg_getParameterWithInt(handle, SiSoCsRt.FG_WIDTH, out V, index);
            return V != 0;
        }

        public bool inUse(uint index)
        {
            if (handle == null)
                return false;
            return isUse[index];
        }

        public void Start(uint channelIndex, dma_mem dMem, int numPics)
        {
            if (handle == null)
                return;
            var ErrCode = SiSoCsRt.Fg_setParameterWithInt(handle, SiSoCsRt.FG_TRIGGERSTATE, SiSoCsRt.TS_ACTIVE, channelIndex);
            ErrCode = SiSoCsRt.Fg_AcquireEx(handle, channelIndex, numPics, SiSoCsRt.ACQ_BLOCK, dMem);
            if (sgcCam[channelIndex] != null)
                SiSoCsRt.Sgc_startAcquisition(sgcCam[channelIndex], 1);
        }

        public void Stop(uint channelIndex, dma_mem dMem)
        {
            if (handle != null)
            {
                if (sgcCam[channelIndex] != null)
                    SiSoCsRt.Sgc_stopAcquisition(sgcCam[channelIndex], 1);
                if (dMem != null)
                    SiSoCsRt.Fg_stopAcquireEx(handle, channelIndex, dMem, 0);
                else
                    SiSoCsRt.Fg_stopAcquire(handle, channelIndex);
            }
        }

        int GetMaxPort()
        {
            int V = 0;
            SiSoCsRt.Fg_getIntSystemInformationForBoardIndex(Board, Fg_Info_Selector.INFO_NR_CXP_PORTS, FgProperty.PROP_ID_VALUE, out CXPPorts);
            V = Math.Max(V, CXPPorts);
            SiSoCsRt.Fg_getIntSystemInformationForBoardIndex(Board, Fg_Info_Selector.INFO_NR_CL_PORTS, FgProperty.PROP_ID_VALUE, out CLPorts);
            V = Math.Max(V, CLPorts);
            SiSoCsRt.Fg_getIntSystemInformationForBoardIndex(Board, Fg_Info_Selector.INFO_NR_CLHS_PORTS, FgProperty.PROP_ID_VALUE, out CLHSPorts);
            V = Math.Max(V, CLHSPorts);
            SiSoCsRt.Fg_getIntSystemInformationForBoardIndex(Board, Fg_Info_Selector.INFO_NR_GIGE_PORTS, FgProperty.PROP_ID_VALUE, out GIGEPorts);
            V = Math.Max(V, GIGEPorts);
            return V;
        }

        void GetAllFGParamName(Fg_Struct _handle)
        {
            if (_handle == null)
                return;
            MaxPorts = GetMaxPort();
            Format = new int[MaxPorts];
            Width = new int[MaxPorts];
            Height = new int[MaxPorts];
            isUse = new bool[MaxPorts];
            SiSoCsRt.Fg_getIntSystemInformationForFgHandle(_handle, Fg_Info_Selector.INFO_BOARDSTATUS, FgProperty.PROP_ID_VALUE, out int VV);
            int ParamCount = SiSoCsRt.Fg_getNrOfParameter(_handle);
            FGParams = new string[ParamCount];
            for (int i = 0; i < ParamCount; i++)
                FGParams[i] = SiSoCsRt.Fg_getParameterName(_handle, i);
        }

        void SGCError(int error)
        {
            if (error != 0)
                ErrMsg.Add(SiSoCsRt.Sgc_getErrorDescription(error));
        }

        void CheckCameraConnected(uint boardIndex)
        {
            if (SiSoCsRt.Fg_findApplet(boardIndex, out applets) == 0)
            {
                var tempHandle = SiSoCsRt.Fg_InitEx(applets, boardIndex, MeInitFlags.FG_INIT_FLAG_SLAVE);
                GetAllFGParamName(tempHandle);
                Array.Resize(ref Devices, MaxPorts);
                for (uint i = 0; i < MaxPorts; i++)
                {
                    Devices[i] = "";
                    SiSoCsRt.Fg_getParameterWithInt(tempHandle, SiSoCsRt.FG_FORMAT, out Format[i], i);
                    SiSoCsRt.Fg_getParameterWithInt(tempHandle, SiSoCsRt.FG_WIDTH, out Width[i], i);
                    SiSoCsRt.Fg_getParameterWithInt(tempHandle, SiSoCsRt.FG_HEIGHT, out Height[i], i);
                    if (Width[i] > 0)
                        Devices[i] = Name + ":" + i.ToString();
                }
                int sgcErr = 0;
                var sgcBoard = SiSoCsRt.Sgc_initBoard(tempHandle, 0, out sgcErr);
                if (sgcErr == 0)
                {
                    sgcErr = SiSoCsRt.Sgc_scanPorts(sgcBoard, 0xF, 2000, SiSoCsRt.LINK_SPEED_NONE);
                    sgcCount = SiSoCsRt.Sgc_getCameraCount(sgcBoard);
                }
                
                SiSoCsRt.Sgc_freeBoard(sgcBoard);
                SiSoCsRt.Fg_FreeGrabber(tempHandle);
            }
        }

        public void LoadConfig(string FName)
        {
            if (handle != null)
            {
                SiSoCsRt.Fg_loadConfig(handle, FName);
            }
        }

        public void SaveConfig(string FName)
        {
            if (handle != null)
                SiSoCsRt.Fg_saveConfig(handle, FName);
        }

        public SisoGrabber(uint boardIndex)
        {
            Board = boardIndex;
            string name, serial;
            SiSoCsRt.Fg_getStringSystemInformationForBoardIndex(boardIndex, Fg_Info_Selector.INFO_BOARDNAME, FgProperty.PROP_ID_VALUE, out name);
            SiSoCsRt.Fg_getStringSystemInformationForBoardIndex(boardIndex, Fg_Info_Selector.INFO_BOARDSERIALNO, FgProperty.PROP_ID_VALUE, out serial);
            int index = name.LastIndexOf('/');
            if (index > 0)
                name = name.Remove(0, index + 1);
            Name = (boardIndex + 1).ToString() + "-" + name;
            CheckCameraConnected(boardIndex);

        }

        public void Close()
        {
            if (handle != null)
            {
                for (uint i = 0; i < Devices.Length; i++)
                    if (Devices[i] != "")
                        SiSoCsRt.Fg_stopAcquire(handle, i);
                if (sgcHandle != null)
                    SiSoCsRt.Sgc_freeBoard(sgcHandle);
                SiSoCsRt.Fg_FreeGrabber(handle);
            }
        }

        public bool TimeOut(double baseTim, double range) { return (NowTime - baseTim) > range; }

        public double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        static Task listAll;
        public static List<SisoGrabber> AllBoard = new List<SisoGrabber>();
        static object dLockDevice = new object();
        public static SisoGrabber SelectedBoard(string target)
        { return AllBoard.FirstOrDefault(x => target.StartsWith(x.Name)); }
        public static bool OpenCamera(string target, string configName, out SisoGrabber FG, out uint channelIndex)
        {
            bool success = false;
            channelIndex = 0;
            lock (dLockDevice)
            {
                FG = SelectedBoard(target);
                int index = target.LastIndexOf(':');
                string sindex = target.Remove(0, index + 1);
                int sgcErr = 0;
                if (FG.handle == null)
                {
                    FG.handle = SiSoCsRt.Fg_InitEx(FG.applets, FG.Board, MeInitFlags.FG_INIT_FLAG_DEFAULT);
                    if (FG.handle != null)
                    {
                        if ((configName != "") && File.Exists(configName))
                            SiSoCsRt.Fg_loadConfig(FG.handle, configName);
                        FG.sgcHandle = SiSoCsRt.Sgc_initBoard(FG.handle, 0, out sgcErr);
                    }
                }
                if ((FG != null) && uint.TryParse(sindex, out channelIndex) && (!FG.isUse[channelIndex]))
                    if (FG.isConnected(channelIndex))
                    {
                        success = FG.isUse[channelIndex] = true;
                        if (FG.sgcHandle != null)
                        {
                            sgcErr = SiSoCsRt.Sgc_scanPorts(FG.sgcHandle, 0xF, 2000, SiSoCsRt.LINK_SPEED_NONE);
                            var cnt = SiSoCsRt.Sgc_getCameraCount(FG.sgcHandle);
                            //FG.sgcCam[channelIndex] = SiSoCsRt.Sgc_getCameraByIndex(FG.sgcHandle, channelIndex, out sgcErr);
                            FG.sgcCam[channelIndex] = SiSoCsRt.Sgc_getCamera(FG.sgcHandle, channelIndex, out sgcErr);
                            sgcErr = SiSoCsRt.Sgc_connectCamera(FG.sgcCam[channelIndex]);
                        }
                    }
            }
            return success;
        }
        public static void CloseCamera(SisoGrabber FG, uint channelIndex, dma_mem dMem)
        {
            if ((FG == null) || (FG.handle == null) || (!FG.isConnected(channelIndex)))
                return;
            if (FG.isUse[channelIndex])
            {
                if (dMem == null)
                    SiSoCsRt.Fg_stopAcquire(FG.handle, channelIndex);
                else
                    SiSoCsRt.Fg_stopAcquireEx(FG.handle, channelIndex, dMem, 0);
            }
            lock (dLockDevice)
            {
                FG.isUse[channelIndex] = false;
                if (FG.sgcCam[channelIndex] != null)
                {
                    SiSoCsRt.Sgc_disconnectCamera(FG.sgcCam[channelIndex]);
                    FG.sgcCam[channelIndex] = null;
                }
            }
        }
        static int GetBoardCount()
        {
            byte[] Buf = new byte[256];
            uint BufLen = (uint)Buf.Length;
            int Count = 0;
            if (SiSoCsRt.Fg_getSystemInformation(null, Fg_Info_Selector.INFO_NR_OF_BOARDS, FgProperty.PROP_ID_VALUE, 0, Buf, ref BufLen) == SiSoCsRt.FG_OK)
                Count = int.Parse(Encoding.ASCII.GetString(Buf, 0, (int)BufLen));
            return Count;
        }
        static List<SisoGrabber> GetBoards()
        {
            var result = new List<SisoGrabber>();
            int count = GetBoardCount();
            for (uint i = 0; i < count; i++)
                result.Add(new SisoGrabber(i));
            return result;
        }
        public static void ListDevice(bool wait)
        {
            if ((listAll == null) || listAll.IsCompleted)
                listAll = Task.Factory.StartNew(() =>
                {
                    lock (dLockDevice)
                        if (AllBoard.Count == 0)
                            AllBoard = GetBoards();
                });
            if (wait)
                listAll.Wait();
        }

        #region IDisposable實作
        public bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~SisoGrabber() { Dispose(true); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            Close();
            disposed = true;
        }
        #endregion

    }
    #endregion

    #region SisoFGArea
    public class SisoFGArea
    {
        string defaultmode = "Stop", dConfigForder = "", CCDName = "CCD1";
        SisoGrabber dev;
        uint CamIndex = 0;
        Task tryConnect = null;
        int dBufferCount = 8;
        bool ConnectSet = false, ProgramExit = false, AutoReconnect = false;
        public ParamDefine Param = new ParamDefine();
        public bool IsGrabbing
        {
            get { return (dev != null) && dev.IsGrabbing(CamIndex); }
        }
        public bool isConnected
        {
            get { return (dev != null) && dev.isConnected(CamIndex); }
        }
        /// <summary>影像實際大小</summary>
        public int Width = 640;
        /// <summary>影像實際大小</summary>
        public int Height = 480;
        object dLockImage = new object();
        IntPtr PaintHwnd = IntPtr.Zero;
        public int ErrCount = 0, GrabCount = 0, onRecvCount = 0;
        int OutputFormat = 0;
        /// <summary> ImageWidth, ImageHeight, ImageBufferPtr </summary>
        Action<object, string, int, int, IntPtr> OnRecv = null;
        internal double ExposureMin, ExposureMax, GammaMin, GammaMax, SpeedRateMin, SpeedRateMax;
        public List<string> Sources = new List<string>();
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer() { Interval = 100, Enabled = true };
        Rectangle rectControl;
        dma_mem dMem;
        ulong PayLoad = 0;
        /// <summary>Grabber Callback</summary>
        void onImageGrabbed()
        {
            if (ProgramExit)
                return;
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
        public SisoFGArea(string ccdName, string DefaultMode, string ConfigForder, int BufferCount, Action<object, string, int, int, IntPtr> onImage, IntPtr paintHwnd)
        {
            PaintHwnd = paintHwnd;
            if (PaintHwnd != IntPtr.Zero)
            {
                rectControl = Control.FromHandle(PaintHwnd).DisplayRectangle;

            }
            CCDName = ccdName;
            defaultmode = DefaultMode;
            dBufferCount = BufferCount;
            dConfigForder = Path.GetFullPath(ConfigForder);
            if (!dConfigForder.EndsWith("\\"))
                dConfigForder += "\\";
            OnRecv = onImage;
            SisoGrabber.ListDevice(false);
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
            while ((SisoGrabber.AllBoard == null) || (SisoGrabber.AllBoard.Count == 0))
                Thread.Sleep(1000);
            SisoGrabber Temp = null;
            string configName = string.Format("C:\\{0}.mcf", CCDName);
            while (true)
            {
                if (Param.Key != "")
                {
                    Temp = SisoGrabber.SelectedBoard(Param.Key);
                    if ((Temp != null) && SisoGrabber.OpenCamera(Param.Key, configName, out Temp, out CamIndex))
                        break;
                }
                Thread.Sleep(5000);
                if (SisoGrabber.AllBoard.Count == 0)
                    SisoGrabber.ListDevice(true);
            }
            dev = Temp;
            ConnectSet = true;
            while((isConnected) && (dev.inUse(CamIndex)))
                Thread.Sleep(1000);
            dev = null;
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
            if (isConnected && (dMem != null))
            {
                while (true)
                {
                    long NowBlock = 0;
                    NowBlock = SiSoCsRt.Fg_getStatusEx(dev.handle, SiSoCsRt.NUMBER_OF_BLOCKED_IMAGES, 0, CamIndex, dMem);
                    if (NowBlock < 1)
                        break;
                    //long PicNo = SiSoCsRt.Fg_getImageEx(dev.handle, SiSoCsRt.SEL_NEXT_IMAGE, 0, CamIndex, 5, dMem);
                    long PicNo = SiSoCsRt.Fg_getImageEx(dev.handle, SiSoCsRt.SEL_NEXT_IMAGE, 0, CamIndex, 5, dMem);
                    if (PicNo > 0)
                    {
                        var Img = SiSoCsRt.Fg_getImagePtrEx(dev.handle, PicNo, CamIndex, dMem);
                        if (OnRecv != null)
                        {
                            OnRecv(this, OutputFormat.ToString(), Width, Height, Img.asPtr());
                            GrabCount = (GrabCount + 1) % 1000;
                        }
                        onRecvCount = (onRecvCount + 1) % 1000;
                        SiSoCsRt.Fg_setStatusEx(dev.handle, SiSoCsRt.FG_UNBLOCK, PicNo, CamIndex, dMem);
                    }
                    else
                        ErrCount = (ErrCount + 1) % 1000;
                }

            }
            if (ConnectSet)
            {
                ConnectSet = false;
                getAllParam();
                setDefaultParam();
            }
        }

        //void DrawToControl(Tuple<IntPtr, MV_FRAME_OUT_INFO_EX> Temp)
        //{
        //    cImg.UpdateImageInfo(Temp.Item1, Temp.Item2.nFrameLen, Temp.Item2.nHeight, Temp.Item2.nWidth, Temp.Item2.enPixelType);
        //    dev.DisplayOneFrame(ref cDisplay);
        //}

        void setDefaultParam()
        {
            //getCLC_Paramter();
            //_onReceive = onImageGrabbed;
            //GC.KeepAlive(_onReceive);   //必須!
            //_onExcept = onException;
            //GC.KeepAlive(_onExcept);
            //dev.RegisterImageCallBackEx(_onReceive, IntPtr.Zero);
            //dev.RegisterExceptionCallBack(_onExcept, IntPtr.Zero);
            //dev.SetIntValue("GevHeartbeatTimeout", 3000);
            //dev.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            //dev.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            LoadConfig(dConfigForder + CCDName + ".mcf");
            Exposure = Param.Exposure;
            GammaEnable = true;
            Gamma = Param.Gamma;
            MirrorX = Param.MirrorX;
            MirrorY = Param.MirrorY;
            Binning = Param.Binning;
            SpeedRate = Param.SpeedRate;
            string VV = TriggerMode;
            TriggerMode = defaultmode;
        }

        void getAllParam()
        {
            SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_WIDTH, out Width, CamIndex);
            SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_HEIGHT, out Height, CamIndex);
            SiSoCsRt.Fg_getParameterPropertyWithTypeEx(dev.handle, SiSoCsRt.FG_PROCESSING_GAMMA, FgProperty.PROP_ID_MAX, out GammaMax, CamIndex);
            SiSoCsRt.Fg_getParameterPropertyWithTypeEx(dev.handle, SiSoCsRt.FG_PROCESSING_GAMMA, FgProperty.PROP_ID_MIN, out GammaMin, CamIndex);
            SiSoCsRt.Fg_getParameterPropertyWithTypeEx(dev.handle, SiSoCsRt.FG_TRIGGER_FRAMESPERSECOND, FgProperty.PROP_ID_MAX, out SpeedRateMax, CamIndex);
            SiSoCsRt.Fg_getParameterPropertyWithTypeEx(dev.handle, SiSoCsRt.FG_TRIGGER_FRAMESPERSECOND, FgProperty.PROP_ID_MIN, out SpeedRateMin, CamIndex);

            //if (dev.GetFloatValue("ExposureTime", ref cFloat) == 0)

            //{
            //    ExposureMin = cFloat.Min;
            //    ExposureMax = cFloat.Max;
            //}
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
                int V = 0;
                int src = 0;
                SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_AREATRIGGERMODE, out V, CamIndex);
                SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_TRIGGERINSRC, out src, CamIndex);
                if (V == SiSoCsRt.ATM_GENERATOR)
                    return "FreeRun";
                if (V == SiSoCsRt.ATM_SOFTWARE)
                    return "Software";
                if ((src >= SiSoCsRt.TRGINSRC_FRONT_GPI_0) && (src <= SiSoCsRt.TRGINSRC_FRONT_GPI_3))
                    return string.Format("FRONT_GPI_{0}", src - SiSoCsRt.TRGINSRC_FRONT_GPI_0);
                if ((src >= SiSoCsRt.TRGINSRC_GPI_0) && (src <= SiSoCsRt.TRGINSRC_GPI_7))
                    return string.Format("GPI_{0}", src - SiSoCsRt.TRGINSRC_GPI_0);
                return "DisConnect";
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
                        SiSoCsRt.Fg_setParameterWithInt(dev.handle, SiSoCsRt.FG_AREATRIGGERMODE, SiSoCsRt.ATM_GENERATOR, CamIndex);
                        Start();
                        break;
                    case "Software":
                        SiSoCsRt.Fg_setParameterWithInt(dev.handle, SiSoCsRt.FG_AREATRIGGERMODE, SiSoCsRt.ATM_SOFTWARE, CamIndex);
                        Start();
                        break;
                    default:
                        int V = 0;
                        if (value.StartsWith("GPI_") && int.TryParse(value.Remove(0, 4), out V))
                        {
                            V += SiSoCsRt.TRGINSRC_GPI_0;
                            SiSoCsRt.Fg_setParameterWithInt(dev.handle, SiSoCsRt.FG_AREATRIGGERMODE, SiSoCsRt.ATM_EXTERNAL, CamIndex);
                            SiSoCsRt.Fg_setParameterWithInt(dev.handle, SiSoCsRt.FG_TRIGGERINSRC, V, CamIndex);
                            Start();
                        }
                        if (value.StartsWith("FRONT_GPI_") && int.TryParse(value.Remove(0, 10), out V))
                        {
                            V += SiSoCsRt.TRGINSRC_FRONT_GPI_0;
                            SiSoCsRt.Fg_setParameterWithInt(dev.handle, SiSoCsRt.FG_AREATRIGGERMODE, SiSoCsRt.ATM_EXTERNAL, CamIndex);
                            SiSoCsRt.Fg_setParameterWithInt(dev.handle, SiSoCsRt.FG_TRIGGERINSRC, V, CamIndex);
                            Start();
                        }
                        break;
                }
            }
        }

        int bytesperPixel(int format)
        {
            int bytes = 1;
            switch (format)
            {
                case SiSoCsRt.FG_GRAY: bytes = 1; break;
                case SiSoCsRt.FG_GRAY16: bytes = 2; break;
                case SiSoCsRt.FG_COL24: bytes = 3; break;
                case SiSoCsRt.FG_COL32: bytes = 4; break;
                case SiSoCsRt.FG_COL30: bytes = 5; break;
                case SiSoCsRt.FG_COL48: bytes = 6; break;
            }
            return bytes;
        }
        /// <summary>啟動Grabber</summary>
        void Start()
        {
            if ((!isConnected) || IsGrabbing)
                return;
            SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_WIDTH, out Width, CamIndex);
            SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_HEIGHT, out Height, CamIndex);
            OutputFormat = 0;
            SiSoCsRt.Fg_getParameterWithInt(dev.handle, SiSoCsRt.FG_FORMAT, out OutputFormat, CamIndex);
            ulong dPayLoad = (ulong)(Width * Height * bytesperPixel(OutputFormat) * dBufferCount);
            if ((PayLoad < dPayLoad) || (dMem == null))
            {
                if (dMem != null)
                    SiSoCsRt.Fg_FreeMemEx(dev.handle, dMem);
                dMem = SiSoCsRt.Fg_AllocMemEx(dev.handle, dPayLoad, dBufferCount);
                PayLoad = dPayLoad;
            }
            GrabCount = 0;
            onRecvCount = 0;
            dev.Start(CamIndex, dMem, SiSoCsRt.GRAB_INFINITE);
        }
        /// <summary>停止Grabber</summary>
        void Stop()
        {
            if (!isConnected)
                return;
            dev.Stop(CamIndex, dMem);
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
            //SiSoCsRt.Fg_setParameterWithUInt(dev.handle, SiSoCsRt.FG_SENDSOFTWARETRIGGER, 1, CamIndex);
            SiSoCsRt.Fg_sendSoftwareTrigger(dev.handle, CamIndex);
        }

        void onExit(object sender, EventArgs e)
        {
            ProgramExit = true;
            Close();
        }
        /// <summary>關閉Camera</summary>
        public void Close()
        {
            SisoGrabber.CloseCamera(dev, CamIndex, dMem);
            if (dMem != null)
            {
                try
                {
                    SiSoCsRt.Fg_FreeMemEx(dev.handle, dMem);
                }
                catch { }
                dMem = null;
            }
        }

        public void SetParam()
        {
            SisoSetFrm.SetParam(this);
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
                //if (isConnected && (dev.GetFloatValue("ExposureTime", ref cFloat) == 0))
                //    return cFloat.CurValue;
                //else
                    return Param.Exposure;
            }
            set
            {
                //無法設定
                //if (isConnected)
                //{
                    //double V = Math.Min(Math.Max(value, ExposureMin), ExposureMax);
                    //dev.SetFloatValue("ExposureTime", (float)V);
                //}
            }
        }
        /// <summary>臨時變更GammaEnable</summary>
        public bool GammaEnable
        {
            get
            {
                //bool En = false;
                //return (isConnected && (dev.GetBoolValue("GammaEnable", ref En) == 0)) ? En : false;
                return true;
            }
            set
            {
                //if (isConnected)
                //{
                    //dev.SetBoolValue("GammaEnable", value);
                //}
            }
        }
        /// <summary>臨時變更Gamma</summary>
        public double Gamma
        {
            get
            {
                double V = 0;
                if (isConnected && (SiSoCsRt.Fg_getParameterWithDouble(dev.handle, SiSoCsRt.FG_PROCESSING_GAMMA, out V, CamIndex) == 0))
                    return V;
                else
                    return Param.Gamma;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, GammaMin), GammaMax);
                    SiSoCsRt.Fg_setParameterWithDouble(dev.handle, SiSoCsRt.FG_PROCESSING_GAMMA, V, CamIndex);
                }
            }
        }
        /// <summary>臨時變更FrameRate</summary>
        public double SpeedRate
        {
            get
            {
                double V = 0;
                if (isConnected && (SiSoCsRt.Fg_getParameterWithDouble(dev.handle, SiSoCsRt.FG_TRIGGER_FRAMESPERSECOND, out V, CamIndex) == 0))
                    return V;
                else
                    return Param.Gamma;
            }
            set
            {
                if (isConnected)
                {
                    double V = Math.Min(Math.Max(value, SpeedRateMin), SpeedRateMax);
                    SiSoCsRt.Fg_setParameterWithDouble(dev.handle, SiSoCsRt.FG_TRIGGER_FRAMESPERSECOND, V, CamIndex);
                }
            }
        }
        /// <summary>臨時變更MirrorX</summary>
        public bool MirrorX
        {
            get
            {
                //bool En = false;
                return false;
                //return (isConnected && (dev.GetBoolValue("ReverseX", ref En) == 0)) ? En : Param.MirrorX;
            }
            set
            {
                //if (isConnected)
                //    dev.SetBoolValue("ReverseX", value);
            }
        }
        /// <summary>臨時變更MirrorY</summary>
        public bool MirrorY
        {
            get
            {
                bool En = false;
                return false;
                //return (isConnected && (dev.GetBoolValue("ReverseY", ref En) == 0)) ? En : Param.MirrorY;
            }
            set
            {
                //if (isConnected)
                //    dev.SetBoolValue("ReverseY", value);
            }
        }
        /// <summary>停止時臨時變更Binning</summary>
        public bool Binning
        {
            get
            {
                //if (!isConnected)
                //    return false;
                return true;
                //dev.GetEnumValue("BinningHorizontal", ref cEnu);
                //return cEnu.CurValue != 1;
            }
            set
            {
                //if (isConnected)
                //    dev.SetEnumValue("BinningHorizontal", value ? (uint)2 : 1);
            }
        }
        /// <summary>載入設定檔</summary>
        public void LoadConfig(string CfgFileName)
        {
            string FName = (CfgFileName == "") ? dConfigForder + CCDName + ".mcf" : CfgFileName;
            if ((FName != "") && File.Exists(FName) && (dev != null))
            {
                dev.LoadConfig(FName);
                getAllParam();
            }
        }
        /// <summary>儲存設定檔, 不設定名稱時為使用者Dialog自行設定</summary>
        public void SaveConfig(string CfgFileName)
        {
            if (dev != null)
            {
                string FName = (CfgFileName == "") ? dConfigForder + CCDName + ".mcf" : CfgFileName;
                dev.SaveConfig(FName);
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
