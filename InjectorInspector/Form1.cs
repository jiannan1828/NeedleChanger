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

//WMX3
using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;

//JSON
using System.IO;
using System.Text.Json;

//TCP Server
//Vibration
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

namespace InjectorInspector
{
    public partial class Form1 : Form {

        //WMX3
        WMX3Api wmx = new WMX3Api();
        Io io = new Io();
        CoreMotion motion              = new CoreMotion();
        CoreMotionStatus CmStatus      = new CoreMotionStatus();
        EngineStatus EnStatus          = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch            = new Stopwatch();
        AdvancedMotion advmon          = new AdvancedMotion();






        /// <summary>
        /// ServoMotor WMX3 Control API
        /// </summary>
        /// 
        public void WMX3_Initial()
        {
            //建立裝置
            wmx.CreateDevice("C:\\Program Files\\SoftServo\\WMX3", DeviceType.DeviceTypeNormal, 10000);  

            //設定裝置名稱
            wmx.SetDeviceName("DLF");

            //設置齒輪比
            int A = motion.Config.SetGearRatio(0, 1048576, 10000);  
            int B = motion.Config.SetGearRatio(1, 1048576, 10000);

        }  //end of public void WMX3_Initial()

        public int WMX3_establish_Commu()
        {
            int rslt = 0;

            int ret = wmx.StartCommunication();
            if (ret != 0) {
                string str = WMX3Api.ErrorToString(ret);
                MessageBox.Show(str);
            }

            return rslt;
        }  //end of public void WMX3_establish_Commu()

        public void WMX3_destroy_Commu()
        {
            wmx.StopCommunication();
        }  //end of public void WMX3_destroy_Commu()

        public int WMX3_check_Commu()
        {
            int rslt = 0;

            //讀取當前通訊狀態
            motion.GetStatus(ref CmStatus);

            switch (CmStatus.EngineState) {
                case EngineState.Running:
                    rslt = 0;
                    break;

                case EngineState.Communicating:
                    rslt = 1;
                    break;
            }

            return rslt;
        }  //end of public int WMX3_check_Commu()

        public void WMX3_ServoOnOff(int axis, int bOn)
        {
            int newStatus = bOn;

            //啟動伺服
            int ret = motion.AxisControl.SetServoOn(axis, newStatus);

            if (ret != 0) {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }
        }  //end of public void WMX3_ServoOn(int axis)

        public int WMX3_check_ServoOnOff(int axis, ref string position, ref string speed)
        {
            int rslt = 0;

            //讀取SV ON狀態
            CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[axis];
            if (cmAxis.ServoOn == true) {
                rslt = 1;
            } else {
                rslt = 0;
            }

            //讀取目前位置
            string Profile = cmAxis.ActualPos.ToString();

            //strtok info
            position = Profile.Substring(0, Math.Min(Profile.Length, 6));
            speed    = cmAxis.ActualVelocity.ToString();
            //AcTqr0.Text = cmAxis.ActualTorque.ToString();

            return rslt;
        }  //end of int WMX3_check_ServoOnOff(int axis)

        public int WMX3_Pivot(int axis, int pivot, int speed, int accel, int daccel)
        {
            int rslt = 0;

            //位置控制設定
            int ret1 = motion.AxisControl.SetAxisCommandMode(0, AxisCommandMode.Position);

            //POS參數設置
            Motion.PosCommand pos = new Motion.PosCommand();

            pos.Profile.Type     = ProfileType.Trapezoidal;  //運動模式
            pos.Axis             = axis;    //軸
            pos.Target           = pivot;   //指定位置
            pos.Profile.Velocity = speed;   //速度
            pos.Profile.Acc      = accel;   //加速度
            pos.Profile.Dec      = daccel;  //減速度

            //啟動POS運轉
            rslt = motion.Motion.StartPos(pos);  

            if (rslt != 0) {
                string ers = CoreMotion.ErrorToString(rslt);  //如果無法通訊則報錯誤給使用者
                //   textBox12.Text += "軸" + textBox3.Text + ":" + ers + "\r\n";
            }

            return rslt;
        }  //end of public void WMX3_Pivot(int pivot, int speed, int accel, int daccel)

        public int WMX3_SetHomePosition(int axis)
        {
            int rslt = 0;

            //讀取當前座標
            AxisHomeParam.HomeType = Config.HomeType.CurrentPos;

            //設置原點參數
            rslt = motion.Config.SetHomeParam(axis, AxisHomeParam);

            if (rslt != 0) {
                string ers = CoreMotion.ErrorToString(rslt);//如果無法通訊則報錯誤給使用者
                //   textBox12.Text += "軸" + textBox9.Text + "設置HOME錯誤" + ers + "\r\n";
            }

            //開始回原點
            rslt = motion.Home.StartHome(axis); 

            return rslt;
        }  //end of public int WMX3_SetHomePosition(int axis)






        /// <summary>
        /// Test function
        /// </summary>
        /// 
        private void button5_Click(object sender, EventArgs e)
        {


            //Initial WMX3 Profile
                var NewMorotContext = new JsonWMX3Handle();
                NewMorotContext.InitialJsonFile("WMX3MotorProfile.json");

                JsonMotorContent newTestMotorContent = new JsonMotorContent {
                    strLabel = "TracePlate_U",
                    u32Index = 17,
                    u32numerator      = 1048576,
                    u32denominator    = 12000,
                    u32Velocity       = 1000,
                    u32Acceleration   = 520,
                    u32Deacceleration = 320,
                    bServoOn          = true,
                };
                NewMorotContext.AddJsonContent(newTestMotorContent);

                uint max_Velocity = 1500;
                uint min_Velocity = 800;
                label7.Text = NewMorotContext.ReadRangeDataFetcherFromJsonFile(max_Velocity, min_Velocity);

                label5.Text = NewMorotContext.ReadIndexFromJsonFile(38);


            //Initial Trajectory Profile
                var NewStepProfileContext = new JsonTrajectoryCurveHandle();
                NewStepProfileContext.InitialJsonFile("TrajectoryCurveProfile.json");

                JsonTrajectoryCurveContent newTrajectoryCurveContent = new JsonTrajectoryCurveContent {
                    strPositionLabel = "移至取料盤",
                    u32Index = 0,

                    u32RemainStep = 3,

                    strMotorAxis_01_Label = "X_Axis",
                        MotorAxis_01_Index = 3,
                        iMotorAxis_01_Position = 1357,
                        iMotorAxis_01_Speed = 2000,
                        iMotorAxis_01_Accel = 3000,
                        iMotorAxis_01_Daccel = 4000,
                        bMotorAxis_01_Enable = true,
                    strMotorAxis_02_Label = "Y_Axis",
                        MotorAxis_02_Index = 7,
                        iMotorAxis_02_Position = 2468,
                        iMotorAxis_02_Speed = 3000,
                        iMotorAxis_02_Accel = 4000,
                        iMotorAxis_02_Daccel = 5000,
                        bMotorAxis_02_Enable = true,
                    strMotorAxis_03_Label = "Z_Axis",
                        MotorAxis_03_Index = 11,
                        iMotorAxis_03_Position = 3579,
                        iMotorAxis_03_Speed = 4000,
                        iMotorAxis_03_Accel = 5000,
                        iMotorAxis_03_Daccel = 6000,
                        bMotorAxis_03_Enable = true,
                    strMotorAxis_04_Label = "U_Axis",
                        MotorAxis_04_Index = 13,
                        iMotorAxis_04_Position = 4680,
                        iMotorAxis_04_Speed = 5000,
                        iMotorAxis_04_Accel = 6000,
                        iMotorAxis_04_Daccel = 7000,
                        bMotorAxis_04_Enable = true,
                    strMotorAxis_05_Label = "V_Axis",
                        MotorAxis_05_Index = 17,
                        iMotorAxis_05_Position = 2580,
                        iMotorAxis_05_Speed = 6000,
                        iMotorAxis_05_Accel = 7000,
                        iMotorAxis_05_Daccel = 8000,
                        bMotorAxis_05_Enable = true,
                };
                NewStepProfileContext.AddJsonContent(newTrajectoryCurveContent);

                JsonTrajectoryCurveContent newTrajectoryCurveContent2 = new JsonTrajectoryCurveContent {
                    strPositionLabel = "移至取料盤",
                    u32Index = 1,

                    u32RemainStep = 2,

                    strMotorAxis_01_Label = "X_Axis",
                        MotorAxis_01_Index = 3,
                        iMotorAxis_01_Position = 1355,
                        iMotorAxis_01_Speed = 2000,
                        iMotorAxis_01_Accel = 3000,
                        iMotorAxis_01_Daccel = 4000,
                        bMotorAxis_01_Enable = true,
                    strMotorAxis_02_Label = "Y_Axis",
                        MotorAxis_02_Index = 7,
                        iMotorAxis_02_Position = 2458,
                        iMotorAxis_02_Speed = 3000,
                        iMotorAxis_02_Accel = 4000,
                        iMotorAxis_02_Daccel = 5000,
                        bMotorAxis_02_Enable = true,
                    strMotorAxis_03_Label = "Z_Axis",
                        MotorAxis_03_Index = 11,
                        iMotorAxis_03_Position = 3559,
                        iMotorAxis_03_Speed = 4000,
                        iMotorAxis_03_Accel = 5000,
                        iMotorAxis_03_Daccel = 6000,
                        bMotorAxis_03_Enable = true,
                    strMotorAxis_04_Label = "U_Axis",
                        MotorAxis_04_Index = 13,
                        iMotorAxis_04_Position = 4650,
                        iMotorAxis_04_Speed = 5000,
                        iMotorAxis_04_Accel = 6000,
                        iMotorAxis_04_Daccel = 7000,
                        bMotorAxis_04_Enable = true,
                    strMotorAxis_05_Label = "V_Axis",
                        MotorAxis_05_Index = 17,
                        iMotorAxis_05_Position = 2550,
                        iMotorAxis_05_Speed = 6000,
                        iMotorAxis_05_Accel = 7000,
                        iMotorAxis_05_Daccel = 8000,
                        bMotorAxis_05_Enable = true,
                };
                NewStepProfileContext.AddJsonContent(newTrajectoryCurveContent2);

                label14.Text = NewStepProfileContext.ReadIndexFromJsonFile(0);


            //Initial Needle Profile
            var newNeedleContext = new JsonNeedleHandle();
                newNeedleContext.InitialJsonFile("NeedlePlacementCoordinates.json");

                JsonNeedleContent newTestContent = new JsonNeedleContent {
                    strLabel       = "GGshimida",
                    u32Index       = 87,
                    dblXCoordinate = 1.357,
                    dblYCoordinate = 2.468,
                    bReplace = false,
                    bVisible = true
                };
                newNeedleContext.AddJsonContent(newTestContent);

                JsonNeedleContent newTestContent2 = new JsonNeedleContent {
                    strLabel = "KKis87",
                    u32Index = 7878,
                    dblXCoordinate = 0.2468,
                    dblYCoordinate = 1.3579,
                    bReplace = false,
                    bVisible = true
                };
                newNeedleContext.AddJsonContent(newTestContent2);

                newNeedleContext.RemoveJsonContentByIndex(18);

                double max_X = 4.82;
                double min_X = 1.58;
                double max_Y = 5.972;
                double min_Y = 2.53;
                label8.Text = newNeedleContext.ReadRangeDataFetcherFromJsonFile(max_X, min_X, max_Y, min_Y);

                this.Text = newNeedleContext.ReadIndexFromJsonFile(7);

        }






        /// <summary>
        /// Project Code implement
        /// </summary>
        /// 
        public Form1()
        {
            //C# project code component initialize
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WMX3_Initial();

            //先跳到第2頁
            int iAimToPageIndex = 3;
            tabControl1.SelectedTab = tabControl1.TabPages[iAimToPageIndex - 1];

            this.Text = "2024/09/04 14:04";
        }

        private void strCommunication_Click_1(object sender, EventArgs e)
        {
            WMX3_establish_Commu();
        }
        private void stoCommunication_Click_1(object sender, EventArgs e)
        {
            WMX3_destroy_Commu();
        }
        private void svOn0_Click(object sender, EventArgs e)
        {
            int axis = 0;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void svOn1_Click_1(object sender, EventArgs e)
        {
            int axis = 1;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void svOff0_Click_1(object sender, EventArgs e)
        {
            int axis = 0;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void svOff1_Click(object sender, EventArgs e)
        {
            int axis = 1;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }

        private void Home0_Click(object sender, EventArgs e)
        {
            int axis;

            axis = 0;
            WMX3_SetHomePosition(axis);

            axis = 1;
            WMX3_SetHomePosition(axis);
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //WMX3通訊狀態
            int getCommuStatus = WMX3_check_Commu();
            if (getCommuStatus == 1) {
                label12.Text = "連線中";
                label12.ForeColor = Color.Red;
            } else {
                label12.Text = "尚未連線";
                label12.ForeColor = Color.Black;
            }

            //region 讀取軸狀態
            int rslt = 0;
            int axis = 0;
            string position = "";
            string speed    = "";

            axis = 0;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if(rslt == 1) {
                svOn0.BackColor = Color.Red;
            } else {
                svOn0.BackColor = Color.Green;
            }
            AcPos0.Text = position;
            AcSpd0.Text = speed;

            axis = 1;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1) {
                svOn1.BackColor = Color.Red;
            } else {
                svOn1.BackColor = Color.Green;
            }
            AcPos1.Text = position;
            AcSpd1.Text = speed;
        }

        private void stPos1_Click_1(object sender, EventArgs e)
        {
            //單點動

            int axis;

            int position = int.Parse(textBox2.Text);
            int speed    = int.Parse(textBox1.Text);
            int accel    = 100000 * 10;
            int daccel   = 100000 * 10;

            axis = 0;
            WMX3_Pivot(axis, position, speed, accel, daccel);

            axis = 1;
            WMX3_Pivot(axis, position * 10, speed * 10, accel * 10, daccel * 10);
        }

        private void 緊急停止_Click(object sender, EventArgs e)
        {
            int isOn = 0;
            int axis = 0;

            axis = 0;
            WMX3_ServoOnOff(axis, isOn);

            axis = 1;
            WMX3_ServoOnOff(axis, isOn);
        }









 






        /// <summary>
        /// Reserve function
        /// </summary>
        /// 
        private void button3_Click_1(object sender, EventArgs e)
        {
            //路徑動
            AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
            
            path.Axis[0] = 0;
            path.Axis[1] = 1;

            path.EnableConstProfile  = 1;

            path.Profile[0].Type = ProfileType.Trapezoidal;
            path.NumPoints = 6;
            
            path.Profile[0].Velocity = 10000;
            path.Profile[0].Acc      = 10000;
            path.Profile[0].Dec      = 10000;

            path.Type[0] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0,0] = 0;
            path.Target[1,0] = 0;

            path.Type[1] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 1] = 0;
            path.Target[1, 1] = 5000;

            path.Type[2] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 2] = 5000;
            path.Target[1, 2] = 15000;

            path.Type[3] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 3] = 25000;
            path.Target[1, 3] = 15000;

            path.Type[4] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 4] = 30000;
            path.Target[1, 4] = 5000;

            path.Type[5] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 5] = 30000;
            path.Target[1, 5] = 0;

            // int ret = motion.Motion.StartPos(path);
            int a1 = advmon.AdvMotion.StartPathIntplPos(path);

            //int a = WmxLib_Adv.AdvMotion.StartCSplinePos(0, splineCommand, 9, splinePoint);
            //AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
            //static AdvancedMotion WmxLib_Adv = new AdvancedMotion(Wmx3Lib);
            //AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
        }





        /// <summary>
        /// TCP Server
        /// Vibration
        /// </summary>
        /// 
        public enum xe_U15_CMD {
            xeUC_TestMode_Parameters                = 32,
            xeUC_TestMode_FunctionOn                = 33,
            xeUC_SendParametersToRecipeNo           = 34,
            xeUC_SendLightOnToRecipeNo              = 35,
            xeUC_RunRecipeNo                        = 36,
            xeUC_Reserve_SendDepotsToRecipeNo       = 37,
            xeUC_ReadMachineStatus                  = 38,
            xeUC_ReadParametersOfRecipeNo           = 39,
            xeUC_TestMode_LightLevel                = 40,
            xeUC_Reserve_TestMode_ParametersOfDepot = 41,
            xeUC_NullCMD_AutoReadStatus             = 42,
            xeUC_Reserve_SetControlMode             = 43,
            xeUC_Reserve_GetControlMode             = 44,
            xeUC_RunMultiRecipeNo                   = 45,
            xeUC_SetRecipeGroupNo                   = 46,
            xeUC_GetRecipeGroupNo                   = 47,
            xeUC_RunRecipeGroupNo                   = 48,
            xeUC_Reserve_GetDIOStatus               = 49,
            xeUC_ReadMachineInfo                    = 50,
            xeUC_Reserve01                          = 51,
            xeUC_Reserve02                          = 52,
            xeUC_Reserve03                          = 53,
            xeUC_KillRecipeNo                       = 54,
            xeUC_KillRecipeGroupNo                  = 55,
            xeUC_Reserve_UpperLightOn               = 56,

            xeUC_MachineInfo_START                  = 100,
            xeUC_MachineFirmwareType                = 101,
            xeUC_MachineFirmwareVersion             = 102,
            xeUC_MachineType                        = 103,
            xeUC_MachineSerialNumber                = 104,
            xeUC_MachineInfo_END                    = 105,
        }

        //Command Header
        public const string strCMDU15Header = "AB,1,";

        public uint u32Frequency = 350; 
            public uint u32VibrationSource1_StartPhase = 315;
            public uint u32VibrationSource1_StopPhase  = 461;
            public uint u32VibrationSource2_StartPhase = 276;
            public uint u32VibrationSource2_StopPhase  = 426;
            public uint u32VibrationSource3_StartPhase = 228;
            public uint u32VibrationSource3_StopPhase  = 374;
            public uint u32VibrationSource4_StartPhase = 224;
            public uint u32VibrationSource4_StopPhase  = 381;
            public uint u32BlackDepotSource_StartPhase = 291;
            public uint u32BlackDepotSource_StopPhase  = 446;
            public uint u32VibrationSource1_Power = 307;
            public uint u32VibrationSource2_Power = 213;
            public uint u32VibrationSource3_Power = 220;
            public uint u32VibrationSource4_Power = 213;
            public uint u32BlackDepotSource_Power = 324;

        public uint u32LED_Level = 10;


        public void button6_Click(object sender, EventArgs e) {
            // 設定伺服器端的 IP 和 Port
            IPAddress ip = IPAddress.Parse("192.168.2.124");
            int port = 6032;

            // 建立 TcpListener 來監聽指定的 IP 和 Port
            TcpListener server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine("伺服器啟動中，等待客戶端連接...");

            // 接受來自客戶端的連接
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("客戶端已連接！");

            // 延遲0.5秒（500毫秒）
            Thread.Sleep(500); 
            
            //Vibration
            if(true) {  //Test Mode
                // 儲存變量
                uint u32SaveFrequency = 0;
                    uint u32SaveVibrationSource1_StartPhase = 0;
                    uint u32SaveVibrationSource1_StopPhase  = 0;
                    uint u32SaveVibrationSource2_StartPhase = 0;
                    uint u32SaveVibrationSource2_StopPhase  = 0;
                    uint u32SaveVibrationSource3_StartPhase = 0;
                    uint u32SaveVibrationSource3_StopPhase  = 0;
                    uint u32SaveVibrationSource4_StartPhase = 0;
                    uint u32SaveVibrationSource4_StopPhase  = 0;
                    uint u32SaveBlackDepotSource_StartPhase = 0;
                    uint u32SaveBlackDepotSource_StopPhase  = 0;
                    uint u32SaveVibrationSource1_Power = 0;
                    uint u32SaveVibrationSource2_Power = 0;
                    uint u32SaveVibrationSource3_Power = 0;
                    uint u32SaveVibrationSource4_Power = 0;
                    uint u32SaveBlackDepotSource_Power = 0;

                // 檢查變量是否變更
                if (u32SaveFrequency != u32Frequency ||
                    u32SaveVibrationSource1_StartPhase != u32VibrationSource1_StartPhase ||
                    u32SaveVibrationSource1_StopPhase  != u32VibrationSource1_StopPhase  ||
                    u32SaveVibrationSource2_StartPhase != u32VibrationSource2_StartPhase ||
                    u32SaveVibrationSource2_StopPhase  != u32VibrationSource2_StopPhase  ||
                    u32SaveVibrationSource3_StartPhase != u32VibrationSource3_StartPhase ||
                    u32SaveVibrationSource3_StopPhase  != u32VibrationSource3_StopPhase  ||
                    u32SaveVibrationSource4_StartPhase != u32VibrationSource4_StartPhase ||
                    u32SaveVibrationSource4_StopPhase  != u32VibrationSource4_StopPhase  ||
                    u32SaveBlackDepotSource_StartPhase != u32BlackDepotSource_StartPhase ||
                    u32SaveBlackDepotSource_StopPhase  != u32BlackDepotSource_StopPhase  ||
                    u32SaveVibrationSource1_Power != u32VibrationSource1_Power ||
                    u32SaveVibrationSource2_Power != u32VibrationSource2_Power ||
                    u32SaveVibrationSource3_Power != u32VibrationSource3_Power ||
                    u32SaveVibrationSource4_Power != u32VibrationSource4_Power ||
                    u32SaveBlackDepotSource_Power != u32BlackDepotSource_Power) {

                    // 確認變量數值
                    if(u32Frequency >= 1500) u32Frequency = 1500;
                        if(u32VibrationSource1_StartPhase >= 1000) u32VibrationSource1_StartPhase = 1000;
                        if(u32VibrationSource1_StopPhase  >= 1000) u32VibrationSource1_StopPhase  = 1000;
                        if(u32VibrationSource2_StartPhase >= 1000) u32VibrationSource2_StartPhase = 1000;
                        if(u32VibrationSource2_StopPhase  >= 1000) u32VibrationSource2_StopPhase  = 1000;
                        if(u32VibrationSource3_StartPhase >= 1000) u32VibrationSource3_StartPhase = 1000;
                        if(u32VibrationSource3_StopPhase  >= 1000) u32VibrationSource3_StopPhase  = 1000;
                        if(u32VibrationSource4_StartPhase >= 1000) u32VibrationSource4_StartPhase = 1000;
                        if(u32VibrationSource4_StopPhase  >= 1000) u32VibrationSource4_StopPhase  = 1000;
                        if(u32BlackDepotSource_StartPhase >= 1000) u32BlackDepotSource_StartPhase = 1000;
                        if(u32BlackDepotSource_StopPhase  >= 1000) u32BlackDepotSource_StopPhase  = 1000;
                        if(u32VibrationSource1_Power  >= 1000) u32VibrationSource1_Power  = 1000;
                        if(u32VibrationSource2_Power  >= 1000) u32VibrationSource2_Power  = 1000;
                        if(u32VibrationSource3_Power  >= 1000) u32VibrationSource3_Power  = 1000;
                        if(u32VibrationSource4_Power  >= 1000) u32VibrationSource4_Power  = 1000;
                        if(u32BlackDepotSource_Power  >= 1000) u32BlackDepotSource_Power  = 1000;
                            
                    // 更新保存的變量
                    u32SaveFrequency = u32Frequency;
                        u32SaveVibrationSource1_StartPhase = u32VibrationSource1_StartPhase;
                        u32SaveVibrationSource1_StopPhase  = u32VibrationSource1_StopPhase;
                        u32SaveVibrationSource2_StartPhase = u32VibrationSource2_StartPhase;
                        u32SaveVibrationSource2_StopPhase  = u32VibrationSource2_StopPhase;
                        u32SaveVibrationSource3_StartPhase = u32VibrationSource3_StartPhase;
                        u32SaveVibrationSource3_StopPhase  = u32VibrationSource3_StopPhase;
                        u32SaveVibrationSource4_StartPhase = u32VibrationSource4_StartPhase;
                        u32SaveVibrationSource4_StopPhase  = u32VibrationSource4_StopPhase;
                        u32SaveBlackDepotSource_StartPhase = u32BlackDepotSource_StartPhase;
                        u32SaveBlackDepotSource_StopPhase  = u32BlackDepotSource_StopPhase;
                        u32SaveVibrationSource1_Power = u32VibrationSource1_Power;
                        u32SaveVibrationSource2_Power = u32VibrationSource2_Power;
                        u32SaveVibrationSource3_Power = u32VibrationSource3_Power;
                        u32SaveVibrationSource4_Power = u32VibrationSource4_Power;
                        u32SaveBlackDepotSource_Power = u32BlackDepotSource_Power;

                    // 發送命令
                    Console.WriteLine("發送命令");
                    Px16_SendTestCMD(client, xe_U15_CMD.xeUC_TestMode_Parameters, u32SaveFrequency,
                                                                                  u32SaveVibrationSource1_StartPhase,
                                                                                  u32SaveVibrationSource1_StopPhase,
                                                                                  u32SaveVibrationSource2_StartPhase,
                                                                                  u32SaveVibrationSource2_StopPhase,
                                                                                  u32SaveVibrationSource3_StartPhase,
                                                                                  u32SaveVibrationSource3_StopPhase,
                                                                                  u32SaveVibrationSource4_StartPhase,
                                                                                  u32SaveVibrationSource4_StopPhase,
                                                                                  u32SaveBlackDepotSource_StartPhase,
                                                                                  u32SaveBlackDepotSource_StopPhase,
                                                                                  u32SaveVibrationSource1_Power,
                                                                                  u32SaveVibrationSource2_Power,
                                                                                  u32SaveVibrationSource3_Power,
                                                                                  u32SaveVibrationSource4_Power,
                                                                                  u32SaveBlackDepotSource_Power);
                }

            }  //end of if (true) {  //Test Mode
            
            //Vibration
            if (true) { //Test LED Level
                // 保存 LED 等級變數
                uint u32SaveLED_Level = 0;

                // 檢查 LED 等級是否改變
                if (u32SaveLED_Level != u32LED_Level) {
                    
                    // 確認 LED 等級數值
                    if(u32LED_Level >= 50) u32LED_Level = 50;
                    
                    // 更新 LED 等級變數
                    u32SaveLED_Level = u32LED_Level;

                    // 更新 LED 等級
                    Console.WriteLine("更新 LED 等級");
                    Px1_SendCMD(client, xe_U15_CMD.xeUC_TestMode_LightLevel, u32SaveLED_Level);
                }
            }  //end of if (true) { //Test LED Level

            // 處理客戶端連接的任務可交由不同的線程執行
            if(false) {
                Thread thread = new Thread(HandleClient);
                thread.Start(client);
            }
        }

        public void HandleClient(object obj) {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try {
                // 持續處理客戶端的資料傳輸
                while (this.IsDisposed == false) {
                    // 讀取來自客戶端的資料
                    if ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0) {
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("收到: " + message);

                        // 伺服器主動回應訊息
                        byte[] response = Encoding.ASCII.GetBytes("伺服器收到: " + message);
                        stream.Write(response, 0, response.Length);
                    }

                    // 伺服器主動發送訊息給客戶端
                    string serverMessage = "這是伺服器主動發送的訊息";
                    byte[] serverResponse = Encoding.ASCII.GetBytes(serverMessage);
                    stream.Write(serverResponse, 0, serverResponse.Length);
                    Console.WriteLine("伺服器發送: " + serverMessage);

                    // 可以加一些延遲來避免過於頻繁的訊息發送
                    Thread.Sleep(1000);
                }
            } catch (Exception e) {
                Console.WriteLine("發生錯誤: " + e.Message);
            } finally {
                // 關閉連接
                client.Close();
                Console.WriteLine("客戶端已斷開連接。");
            }
        }

        public void Px1_SendCMD(object obj, xe_U15_CMD Px1_CMD, uint u32Px1) {
            switch (Px1_CMD) {
                case xe_U15_CMD.xeUC_TestMode_FunctionOn:
                case xe_U15_CMD.xeUC_RunRecipeNo:
                case xe_U15_CMD.xeUC_ReadParametersOfRecipeNo:
                case xe_U15_CMD.xeUC_TestMode_LightLevel:
                case xe_U15_CMD.xeUC_GetRecipeGroupNo:
                case xe_U15_CMD.xeUC_RunRecipeGroupNo:
                case xe_U15_CMD.xeUC_KillRecipeNo:
                case xe_U15_CMD.xeUC_KillRecipeGroupNo: {
                        const uint u32ParaCount = 1;
                        // 組合命令字串
                        string CMD_U15 = $"{strCMDU15Header}{(uint)Px1_CMD},{(uint)u32ParaCount},{(uint)u32Px1},\r\n\0";

                        // 發送TCP數據
                        SendTCPData(obj, CMD_U15);
                    } break;

                default:
                    break;
            }
        }

        public void Px16_SendTestCMD(object obj, xe_U15_CMD Px16_CMD, uint u32Px1_FRQ,
                                                                      uint u32Px2_M1S,
                                                                      uint u32Px3_M1E,
                                                                      uint u32Px4_M2S,
                                                                      uint u32Px5_M2E,
                                                                      uint u32Px6_M3S,
                                                                      uint u32Px7_M3E,
                                                                      uint u32Px8_M4S,
                                                                      uint u32Px9_M4E,
                                                                      uint u32Px10_MDS,
                                                                      uint u32Px11_MDE,
                                                                      uint u32Px12_M1P,
                                                                      uint u32Px13_M2P,
                                                                      uint u32Px14_M3P,
                                                                      uint u32Px15_M4P,
                                                                      uint u32Px16_MDP) {
            switch (Px16_CMD) {
                case xe_U15_CMD.xeUC_TestMode_Parameters: {
                        const uint u32ParaCount = 16;

                        // 構建指令字串
                        string CMD_U15 = $"{strCMDU15Header}{(uint)Px16_CMD},{(uint)u32ParaCount},{(uint)u32Px1_FRQ},{(uint)u32Px2_M1S},{(uint)u32Px3_M1E},{(uint)u32Px4_M2S},{(uint)u32Px5_M2E},{(uint)u32Px6_M3S},{(uint)u32Px7_M3E},{(uint)u32Px8_M4S},{(uint)u32Px9_M4E},{(uint)u32Px10_MDS},{(uint)u32Px11_MDE},{(uint)u32Px12_M1P},{(uint)u32Px13_M2P},{(uint)u32Px14_M3P},{(uint)u32Px15_M4P},{(uint)u32Px16_MDP},\r\n\0";

                        // 發送TCP數據
                        SendTCPData(obj, CMD_U15);
                } break;

                default:
                    break;
            }
        }

        public void SendTCPData(object obj, string data) {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            if (client != null && stream != null) {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(data);
                stream.Write(bytesToSend, 0, bytesToSend.Length);
            }
        }





        /// <summary>
        /// Uunknow
        /// </summary>
        /// 
        private void Pos_Click(object sender, EventArgs e)
        {
            Config.HomeParam homeParam = new Config.HomeParam();
            motion.Config.GetHomeParam(0, ref homeParam);
            homeParam.HomeType = Config.HomeType.CurrentPos;

            motion.Home.StartHome(0);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Velocity.VelCommand vel = new Velocity.VelCommand();
            Torque.TrqCommand trq = new Torque.TrqCommand();

            //Set axis to velocity command mode
            int ret = motion.AxisControl.SetAxisCommandMode(0, AxisCommandMode.Velocity);
            //  err = wmxlib_cm.axisControl->SetAxisCommandMode(0, AxisCommandMode::Velocity);

            // if (err != ErrorCode::None)
            // {
            //     wmxlib_cm.ErrorToString(err, errString, sizeof(errString));
            //     printf("Failed to set axis command mode to velocity. Error=%d (%s)\n", err, errString);
            //     goto exit;
            // }

            //Set velocity command parameters
            vel.Axis = 0;
            vel.Profile.Type = ProfileType.Trapezoidal;
            vel.Profile.Velocity = 100000;
            vel.Profile.Acc = 10000;
            vel.Profile.Dec = 10000;
            //Execute a velocity command
            int ret1 = motion.Velocity.StartVel(vel);
        }
        private void button2_Click_2(object sender, EventArgs e)
        {
            int ret2 = motion.Velocity.Stop(0);
        }

        // Homing.
        // Config.HomeParam homeParam = new Config.HomeParam();
        // motion.Config.GetHomeParam(0, ref homeParam);
        // homeParam.HomeType = Config.HomeType.CurrentPos;
        // Motion.Config.SetHomeParam(0, homeParam);

        //Motion.Motion.Wait(0);
    }  //end of public partial class Form1 : Form {






    /// <summary>
    /// JSON function
    /// </summary>


    //Profile of Needle 
    public class JsonNeedleContent {
        public string strLabel { get; set; }
        public uint u32Index { get; set; }
        public double dblXCoordinate { get; set; }
        public double dblYCoordinate { get; set; }
        public bool bReplace { get; set; }
        public bool bVisible { get; set; }
    }  //end of public void RoundCoordinates(int decimalPlaces) {

    //Handle of Needle 
    public class JsonNeedleHandle { 
        public List<JsonNeedleContent> JsonNeedleContentList { get; set; }
        public string FilePath { get; set; }
        public string strFileName { get; set; }

        public void InitialJsonFile(string strNameFile) {
            // 初始化列表和文件路徑
            strFileName = strNameFile; // Set the file name
            JsonNeedleContentList = new List<JsonNeedleContent>();
            GenerateFilePath();

            // 如果文件存在，讀取現有數據
            if (File.Exists(FilePath)) {
                try {
                    var jsonString = File.ReadAllText(FilePath);
                    JsonNeedleContentList = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString) ?? new List<JsonNeedleContent>();
                    Console.WriteLine("成功初始化 JSON 文件。");
                } catch (Exception ex) {
                    Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
                }
            } else {
                Console.WriteLine("文件不存在，初始化為空列表。");
            }
        }  //end of public void InitialJsonFile(string strNameFile) {
        public string GenerateFilePath() {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            return FilePath;
        }  //end of public string GenerateFilePath() {
        public void AddJsonContent(JsonNeedleContent newContent) {
            if (newContent == null) {
                throw new ArgumentNullException(nameof(newContent), "新內容不能為空");
            }

            JsonNeedleContentList.Add(newContent);
            WriteDataToJsonFile();
            Console.WriteLine("成功添加新的 JSON 內容並更新文件。");
        }  //end of public void AddJsonContent(JsonNeedleContent newContent) {
        public void WriteDataToJsonFile() {
            try {
                // 先排序並去重
                SortAndRemoveDuplicates();

                // 序列化列表為 JSON 字串
                string jsonString = JsonSerializer.Serialize(JsonNeedleContentList, new JsonSerializerOptions { WriteIndented = true });

                // 輸出序列化結果到控制台，幫助調試
                Console.WriteLine("更新後的 JSON 文件內容:\n" + jsonString);

                // 寫入 JSON 字串到文件
                File.WriteAllText(FilePath, jsonString);

                Console.WriteLine("成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 處理錯誤，例如記錄錯誤信息
                Console.WriteLine("寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile() {
        public void SortAndRemoveDuplicates() {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any()) {
                Console.WriteLine("列表為空，無需排序和去重。");
                return;
            }

            // 使用 LINQ 去重並排序
            JsonNeedleContentList = JsonNeedleContentList
                .GroupBy(p => p.u32Index)
                .Select(g => g.First())    // 取每個組的第一個項目
                .OrderBy(p => p.u32Index)  // 按 u32Index 排序
                .ToList();

            Console.WriteLine("排序並去重完成。");
        }  //end of public void SortAndRemoveDuplicates() {
        public void RemoveJsonContentByIndex(uint u32Index) {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any()) {
                Console.WriteLine("列表為空，無需刪除內容。");
                return;
            }

            // 使用 RemoveAll 方法來刪除所有符合條件的項目
            int removedCount = JsonNeedleContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0) {
                // 如果有項目被刪除，則寫入更新後的列表到文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功刪除 {removedCount} 個 u32Index 為 {u32Index} 的項目。");
            } else {
                // 如果未找到匹配的項目，顯示提示
                Console.WriteLine($"未找到 u32Index 為 {u32Index} 的項目。");
            }
        }  //end of public void RemoveJsonContentByIndex(uint u32Index) {
        public void WriteDataToJsonFile_Test() {
            // 使用正確的類型來創建對象
            List<JsonNeedleContent> TestReadWriteJson = new List<JsonNeedleContent>
            {
                new JsonNeedleContent { strLabel = "PinA", u32Index =  0, dblXCoordinate = 1.234, dblYCoordinate = 5.678, bReplace = true, bVisible = true },
                new JsonNeedleContent { strLabel = "SSKR", u32Index =  3, dblXCoordinate = 2.345, dblYCoordinate = 6.789, bReplace = true, bVisible = true },
                new JsonNeedleContent { strLabel = "FUCK", u32Index = 18, dblXCoordinate = 3.456, dblYCoordinate = 7.890, bReplace = true, bVisible = true },
            };

            // 序列化對象為 JSON 字串
            string jsonString = JsonSerializer.Serialize(TestReadWriteJson, new JsonSerializerOptions { WriteIndented = true });

            // 確保目錄存在，如果不存在則創建
            string filePath = GenerateFilePath();

            try {
                // 寫入 JSON 字串到文件
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine("測試數據成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("測試數據寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile_Test() {
        public string ReadIndexFromJsonFile(uint u32Index) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonNeedleContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString);

                    // 查找 u32_Index
                    JsonNeedleContent GotNeedle = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32Index);

                    if (GotNeedle != null) {
                        // 找到，輸出信息
                        rslt = $"{GotNeedle.strLabel} {GotNeedle.u32Index} {GotNeedle.dblXCoordinate} {GotNeedle.dblYCoordinate}";
                        Console.WriteLine("查詢結果: " + rslt);
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadIndexFromJsonFile(uint u32Index) {
        public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonNeedleContent> Needle = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString);

                    // 根據X和Y範圍過濾
                    var filteredNeedle = Needle?.Where(p => p.dblXCoordinate >= min_X && p.dblXCoordinate <= max_X &&
                                                            p.dblYCoordinate >= min_Y && p.dblYCoordinate <= max_Y);

                    if (filteredNeedle != null && filteredNeedle.Any()) {
                        // 找到滿足條件的
                        rslt = string.Join("", filteredNeedle.Select(p => $"{p.strLabel} (u32Index: {p.u32Index}, X: {p.dblXCoordinate}, Y: {p.dblYCoordinate}) \r\n"));
                        Console.WriteLine("篩選結果:\n" + rslt);
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
    }  //end of public class JsonNeedleHandle { 


    //Profile of WMX3 
    public class JsonMotorContent {
        public string strLabel { get; set; }
        public uint u32Index { get; set; }
        public uint u32numerator { get; set; }
        public uint u32denominator { get; set; }
        public uint u32Velocity { get; set; }
        public uint u32Acceleration { get; set; }
        public uint u32Deacceleration { get; set; }
        public bool bServoOn { get; set; }
    }  //end of public class MotorContent {

    //Handle of WMX3
    public class JsonWMX3Handle {
        public List<JsonMotorContent> JsonMotorContentList { get; set; }
        public string FilePath { get; set; }
        public string strFileName { get; set; }

        public void InitialJsonFile(string strNameFile) {
            // 初始化列表和文件路徑
            strFileName = strNameFile; // Set the file name
            JsonMotorContentList = new List<JsonMotorContent>();
            GenerateFilePath();

            // 如果文件存在，讀取現有數據
            if (File.Exists(FilePath)) {
                try {
                    var jsonString = File.ReadAllText(FilePath);
                    JsonMotorContentList = JsonSerializer.Deserialize<List<JsonMotorContent>>(jsonString) ?? new List<JsonMotorContent>();
                    Console.WriteLine("成功初始化 JSON 文件。");
                } catch (Exception ex) {
                    Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
                }
            } else {
                Console.WriteLine("文件不存在，初始化為空列表。");
            }
        }  //end of public void InitialJsonFile(string strNameFile) {
        public string GenerateFilePath() {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            return FilePath;
        }  //end of public string GenerateFilePath() {
        public void AddJsonContent(JsonMotorContent newContent) {
            if (newContent == null) {
                throw new ArgumentNullException(nameof(newContent), "新內容不能為空");
            }

            JsonMotorContentList.Add(newContent);
            WriteDataToJsonFile();
            Console.WriteLine("成功添加新的 JSON 內容並更新文件。");
        }  //end of public void AddJsonContent(JsonMotorContent newContent) {
        public void WriteDataToJsonFile() {
            try {
                // 先排序並去重
                SortAndRemoveDuplicates();

                // 序列化列表為 JSON 字串
                string jsonString = JsonSerializer.Serialize(JsonMotorContentList, new JsonSerializerOptions { WriteIndented = true });

                // 輸出序列化結果到控制台，幫助調試
                Console.WriteLine("更新後的 JSON 文件內容:\n" + jsonString);

                // 寫入 JSON 字串到文件
                File.WriteAllText(FilePath, jsonString);

                Console.WriteLine("成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 處理錯誤，例如記錄錯誤信息
                Console.WriteLine("寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile() {
        public void SortAndRemoveDuplicates() {
            if (JsonMotorContentList == null || !JsonMotorContentList.Any()) {
                Console.WriteLine("列表為空，無需排序和去重。");
                return;
            }

            // 使用 LINQ 去重並排序
            JsonMotorContentList = JsonMotorContentList
                .GroupBy(p => p.u32Index)
                .Select(g => g.First())    // 取每個組的第一個項目
                .OrderBy(p => p.u32Index)  // 按 u32Index 排序
                .ToList();

            Console.WriteLine("排序並去重完成。");
        }  //end of public void SortAndRemoveDuplicates() {
        public void RemoveJsonContentByIndex(uint u32Index) {
            if (JsonMotorContentList == null || !JsonMotorContentList.Any()) {
                Console.WriteLine("列表為空，無需刪除內容。");
                return;
            }

            // 使用 RemoveAll 方法來刪除所有符合條件的項目
            int removedCount = JsonMotorContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0) {
                // 如果有項目被刪除，則寫入更新後的列表到文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功刪除 {removedCount} 個 u32Index 為 {u32Index} 的項目。");
            } else {
                // 如果未找到匹配的項目，顯示提示
                Console.WriteLine($"未找到 u32Index 為 {u32Index} 的項目。");
            }
        }  //end of public void RemoveJsonContentByIndex(uint u32Index) {
        public string ReadIndexFromJsonFile(uint u32Index) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonMotorContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonMotorContent>>(jsonString);

                    // 查找 u32_Index
                    JsonMotorContent GotMotor = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32Index);

                    if (GotMotor != null) {
                        // 找到，輸出信息
                        rslt = $"{GotMotor.strLabel} {GotMotor.u32Index} {GotMotor.u32numerator} {GotMotor.u32denominator} {GotMotor.u32Velocity} {GotMotor.u32Acceleration} {GotMotor.u32Deacceleration} {GotMotor.bServoOn}";
                        Console.WriteLine("查詢結果: " + rslt);
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadIndexFromJsonFile(uint u32Index) {
        public string ReadRangeDataFetcherFromJsonFile(uint max_Velocity, uint min_Velocity) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonMotorContent> Motor = JsonSerializer.Deserialize<List<JsonMotorContent>>(jsonString);

                    // 根據X和Y範圍過濾
                    var filteredMotor = Motor?.Where(p => p.u32Velocity >= min_Velocity && p.u32Velocity <= max_Velocity);

                    if (filteredMotor != null && filteredMotor.Any()) {
                        // 找到滿足條件的
                        rslt = string.Join("", filteredMotor.Select(p => $"{p.strLabel} (u32Index: {p.u32Index}, V: {p.u32Velocity}) \r\n"));
                        Console.WriteLine("篩選結果:\n" + rslt);
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
    }  //end of public class JsonWMX3Handle {


    //Profile of Trajectory Curve 
    public class JsonTrajectoryCurveContent {
        public string strPositionLabel { get; set; }
        public uint u32Index { get; set; }

        public uint u32RemainStep { get; set; }

        //Motor Axis 01
        public string strMotorAxis_01_Label { get; set; }
        public int MotorAxis_01_Index { get; set; }
        public int iMotorAxis_01_Position { get; set; }
        public int iMotorAxis_01_Speed { get; set; }
        public int iMotorAxis_01_Accel { get; set; }
        public int iMotorAxis_01_Daccel { get; set; }
        public bool bMotorAxis_01_Enable { get; set; }

        //Motor Axis 02
        public string strMotorAxis_02_Label { get; set; }
        public int MotorAxis_02_Index { get; set; }
        public int iMotorAxis_02_Position { get; set; }
        public int iMotorAxis_02_Speed { get; set; }
        public int iMotorAxis_02_Accel { get; set; }
        public int iMotorAxis_02_Daccel { get; set; }
        public bool bMotorAxis_02_Enable { get; set; }

        //Motor Axis 03
        public string strMotorAxis_03_Label { get; set; }
        public int MotorAxis_03_Index { get; set; }
        public int iMotorAxis_03_Position { get; set; }
        public int iMotorAxis_03_Speed { get; set; }
        public int iMotorAxis_03_Accel { get; set; }
        public int iMotorAxis_03_Daccel { get; set; }
        public bool bMotorAxis_03_Enable { get; set; }

        //Motor Axis 04
        public string strMotorAxis_04_Label { get; set; }
        public int MotorAxis_04_Index { get; set; }
        public int iMotorAxis_04_Position { get; set; }
        public int iMotorAxis_04_Speed { get; set; }
        public int iMotorAxis_04_Accel { get; set; }
        public int iMotorAxis_04_Daccel { get; set; }
        public bool bMotorAxis_04_Enable { get; set; }

        //Motor Axis 05
        public string strMotorAxis_05_Label { get; set; }
        public int MotorAxis_05_Index { get; set; }
        public int iMotorAxis_05_Position { get; set; }
        public int iMotorAxis_05_Speed { get; set; }
        public int iMotorAxis_05_Accel { get; set; }
        public int iMotorAxis_05_Daccel { get; set; }
        public bool bMotorAxis_05_Enable { get; set; }

        //Motor Axis 06
        public string strMotorAxis_06_Label { get; set; }
        public int MotorAxis_06_Index { get; set; }
        public int iMotorAxis_06_Position { get; set; }
        public int iMotorAxis_06_Speed { get; set; }
        public int iMotorAxis_06_Accel { get; set; }
        public int iMotorAxis_06_Daccel { get; set; }
        public bool bMotorAxis_06_Enable { get; set; }

        //Motor Axis 07
        public string strMotorAxis_07_Label { get; set; }
        public int MotorAxis_07_Index { get; set; }
        public int iMotorAxis_07_Position { get; set; }
        public int iMotorAxis_07_Speed { get; set; }
        public int iMotorAxis_07_Accel { get; set; }
        public int iMotorAxis_07_Daccel { get; set; }
        public bool bMotorAxis_07_Enable { get; set; }

        //Motor Axis 08
        public string strMotorAxis_08_Label { get; set; }
        public int MotorAxis_08_Index { get; set; }
        public int iMotorAxis_08_Position { get; set; }
        public int iMotorAxis_08_Speed { get; set; }
        public int iMotorAxis_08_Accel { get; set; }
        public int iMotorAxis_08_Daccel { get; set; }
        public bool bMotorAxis_08_Enable { get; set; }

        //Motor Axis 09
        public string strMotorAxis_09_Label { get; set; }
        public int MotorAxis_09_Index { get; set; }
        public int iMotorAxis_09_Position { get; set; }
        public int iMotorAxis_09_Speed { get; set; }
        public int iMotorAxis_09_Accel { get; set; }
        public int iMotorAxis_09_Daccel { get; set; }
        public bool bMotorAxis_09_Enable { get; set; }

        //Motor Axis 10
        public string strMotorAxis_10_Label { get; set; }
        public int MotorAxis_10_Index { get; set; }
        public int iMotorAxis_10_Position { get; set; }
        public int iMotorAxis_10_Speed { get; set; }
        public int iMotorAxis_10_Accel { get; set; }
        public int iMotorAxis_10_Daccel { get; set; }
        public bool bMotorAxis_10_Enable { get; set; }

        //Motor Axis 11
        public string strMotorAxis_11_Label { get; set; }
        public int MotorAxis_11_Index { get; set; }
        public int iMotorAxis_11_Position { get; set; }
        public int iMotorAxis_11_Speed { get; set; }
        public int iMotorAxis_11_Accel { get; set; }
        public int iMotorAxis_11_Daccel { get; set; }
        public bool bMotorAxis_11_Enable { get; set; }

        //Motor Axis 12
        public string strMotorAxis_12_Label { get; set; }
        public int MotorAxis_12_Index { get; set; }
        public int iMotorAxis_12_Position { get; set; }
        public int iMotorAxis_12_Speed { get; set; }
        public int iMotorAxis_12_Accel { get; set; }
        public int iMotorAxis_12_Daccel { get; set; }
        public bool bMotorAxis_12_Enable { get; set; }
    }  //end of public class JsonTrajectoryCurveContent {

    //Handle of Trajectory Curve
    public class JsonTrajectoryCurveHandle {
        public List<JsonTrajectoryCurveContent> JsonTrajectoryCurveContentList { get; set; }
        public string FilePath { get; set; }
        public string strFileName { get; set; }

        public void InitialJsonFile(string strNameFile) {
            // 初始化列表和文件路徑
            strFileName = strNameFile; // Set the file name
            JsonTrajectoryCurveContentList = new List<JsonTrajectoryCurveContent>();
            GenerateFilePath();

            // 如果文件存在，讀取現有數據
            if (File.Exists(FilePath)) {
                try {
                    var jsonString = File.ReadAllText(FilePath);
                    JsonTrajectoryCurveContentList = JsonSerializer.Deserialize<List<JsonTrajectoryCurveContent>>(jsonString) ?? new List<JsonTrajectoryCurveContent>();
                    Console.WriteLine("成功初始化 JSON 文件。");
                } catch (Exception ex) {
                    Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
                }
            } else {
                Console.WriteLine("文件不存在，初始化為空列表。");
            }
        }  //end of public void InitialJsonFile(string strNameFile) {
        public string GenerateFilePath() {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            return FilePath;
        }  //end of public string GenerateFilePath() {
        public void AddJsonContent(JsonTrajectoryCurveContent newContent) {
            if (newContent == null) {
                throw new ArgumentNullException(nameof(newContent), "新內容不能為空");
            }

            JsonTrajectoryCurveContentList.Add(newContent);
            WriteDataToJsonFile();
            Console.WriteLine("成功添加新的 JSON 內容並更新文件。");
        }  //end of public void AddJsonContent(JsonTrajectoryCurveContent newContent) {
        public void WriteDataToJsonFile() {
            try {
                // 先排序並去重
                SortAndRemoveDuplicates();

                // 序列化列表為 JSON 字串
                string jsonString = JsonSerializer.Serialize(JsonTrajectoryCurveContentList, new JsonSerializerOptions { WriteIndented = true });

                // 輸出序列化結果到控制台，幫助調試
                Console.WriteLine("更新後的 JSON 文件內容:\n" + jsonString);

                // 寫入 JSON 字串到文件
                File.WriteAllText(FilePath, jsonString);

                Console.WriteLine("成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 處理錯誤，例如記錄錯誤信息
                Console.WriteLine("寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile() {
        public void SortAndRemoveDuplicates() {
            if (JsonTrajectoryCurveContentList == null || !JsonTrajectoryCurveContentList.Any()) {
                Console.WriteLine("列表為空，無需排序和去重。");
                return;
            }

            // 使用 LINQ 去重並排序
            JsonTrajectoryCurveContentList = JsonTrajectoryCurveContentList
                .GroupBy(p => p.u32Index)
                .Select(g => g.First())    // 取每個組的第一個項目
                .OrderBy(p => p.u32Index)  // 按 u32Index 排序
                .ToList();

            Console.WriteLine("排序並去重完成。");
        }  //end of public void SortAndRemoveDuplicates() {
        public void RemoveJsonContentByIndex(uint u32Index) {
            if (JsonTrajectoryCurveContentList == null || !JsonTrajectoryCurveContentList.Any()) {
                Console.WriteLine("列表為空，無需刪除內容。");
                return;
            }

            // 使用 RemoveAll 方法來刪除所有符合條件的項目
            int removedCount = JsonTrajectoryCurveContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0) {
                // 如果有項目被刪除，則寫入更新後的列表到文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功刪除 {removedCount} 個 u32Index 為 {u32Index} 的項目。");
            } else {
                // 如果未找到匹配的項目，顯示提示
                Console.WriteLine($"未找到 u32Index 為 {u32Index} 的項目。");
            }
        }  //end of public void RemoveJsonContentByIndex(uint u32Index) {
        public string ReadIndexFromJsonFile(uint u32Index) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonTrajectoryCurveContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonTrajectoryCurveContent>>(jsonString);

                    // 查找 u32_Index
                    JsonTrajectoryCurveContent GotStepProfile = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32Index);

                    if (GotStepProfile != null) {
                        // 找到，輸出信息
                        rslt = $"{GotStepProfile.strPositionLabel} {GotStepProfile.u32Index} {GotStepProfile.u32RemainStep} {GotStepProfile.strMotorAxis_01_Label}";
                        Console.WriteLine("查詢結果: " + rslt);
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadIndexFromJsonFile(uint u32Index) {
    }  //end of public class JsonTrajectoryCurveHandle {

}

