using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//PCCP Xavier Tsai, added for testing <START>
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection.Emit;
using System.Diagnostics;

//WMX3
using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;

//JSON
using System.IO;
//using System.Text.Json;

//TCP Server
//Vibration
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using Inspector;
using System.Security.Cryptography;
//PCCP Xavier Tsai, added for testing <END>

namespace InjectorInspector
{
    public partial class Form1 : Form
    {
        //PCCP Xavier Tsai, added for testing <START>
        //WMX3
        WMX3Api wmx = new WMX3Api();
        CoreMotion motion              = new CoreMotion();
        CoreMotionStatus CmStatus      = new CoreMotionStatus();
        EngineStatus EnStatus          = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch            = new Stopwatch();
        AdvancedMotion advmon          = new AdvancedMotion();
      //Io io = new Io();
        public static CoreMotionAxisStatus[] cmAxis = new CoreMotionAxisStatus[8];
        public System.Windows.Forms.NumericUpDown NUD_Motor_NO;
        
        //Debug
        public int ErrorCode = 0;
        public int cntcallback = 0;

        //Create TCP Vibration Connection
        public bool isEstablishTCP = false;

        //軸的對應號碼
        public const int 吸嘴X軸 = 3;
        public const int 吸嘴Y軸 = 7;
        public const int 吸嘴Z軸 = 1;
        public const int 吸嘴R軸 = 0;
        //PCCP Xavier Tsai, added for testing <END>




        //PCCP Xavier Tsai, added for testing <START>
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
            int axis;

            //axis = 吸嘴R軸;
            //int A = motion.Config.SetGearRatio(axis, 1048576, 10000);  

            //axis = 吸嘴Z軸;
            //int B = motion.Config.SetGearRatio(axis, 1048576, 10000);

            axis = 吸嘴Y軸;
            int G = motion.Config.SetGearRatio(axis, 1048576, 2000);
        }  //end of public void WMX3_Initial()

        public int WMX3_establish_Commu()
        {
            int rslt = 0;

            int ret = wmx.StartCommunication();
            if (ret != 0)
            {
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

            switch (CmStatus.EngineState)
            {
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

            if (ret != 0)
            {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }
        }  //end of public void WMX3_ServoOn(int axis)

        public int WMX3_check_ServoOnOff(int axis, ref string position, ref string speed)
        {
            int rslt = 0;

            //讀取SV ON狀態
            CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[axis];
            if (cmAxis.ServoOn == true)
            {
                rslt = 1;
            }
            else
            {
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

            if (rslt != 0)
            {
                string ers = CoreMotion.ErrorToString(rslt);  //如果無法通訊則報錯誤給使用者
                //   textBox12.Text += "軸" + textBox3.Text + ":" + ers + "\r\n";
            }

            return rslt;
        }  //end of public void WMX3_Pivot(int pivot, int speed, int accel, int daccel)

        public int WMX3_SetHomePosition(int axis)
        {
            int rslt = 0;

            //設定為讀取內部home
            AxisHomeParam.HomeType = Config.HomeType.ZPulse;

            //尋找內部home
            rslt = motion.Config.SetHomeParam(axis, AxisHomeParam);

            if (rslt != 0)
            {
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
        public void apiCallBackTest()
        {
            cntcallback++;
            this.Text = cntcallback.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //吸料盤校正用
            PointF pos;
            bool success = inspector1.xCarb震動盤(out pos);
            label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);

            //黑色料倉
            bool 料倉有料 = inspector1.xInsp入料();
            label3.Text = string.Format("黑色料倉 料倉有料 = {0}", 料倉有料);

            //光源震動盤
            List<Vector3> pins;
            bool 料盤有料 = inspector1.xInsp震動盤(out pins);
            Vector3 temp = (料盤有料) ? pins.First() : new Vector3();
            label4.Text = string.Format("光源震動盤 震動盤 = {0} X = {1:F2} Y = {2:F2} θ = {3:F2}", 料盤有料, temp.X, temp.Y, temp.θ);

            if (inspector1.Inspected && inspector1.InspectOK)
            {
                double deg = inspector1.PinDeg;
                label5.Text = string.Format("吸嘴物料分析  θ = {0:F2}", deg);
            }
            else
                label5.Text = "吸嘴物料分析失敗";

            int cntdebug = inspector1.RecvCount;


        }






        /// <summary>
        /// Project Code implement
        /// </summary>
        /// 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //init vision
            inspector1.xInit();
            //Add the callback api from snapshot api
            inspector1.on下視覺 = apiCallBackTest;
            
            //Active WMX3
            WMX3_Initial();

            //先跳到第2頁
            int iAimToPageIndex = 2;
            tabControl1.SelectedTab = tabControl1.TabPages[iAimToPageIndex - 1];

            this.Text = "2024/09/04 14:04";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            WMX3_destroy_Commu();
            
            // Stop Communication.
            wmx.StopCommunication(10000);
            
            //Quit device.
            wmx.CloseDevice();
            motion.Dispose();
            wmx.Dispose();

            //sw.Close();
        }

        private void btn_On_吸嘴R軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴R軸;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_Off_吸嘴R軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴R軸;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_On_吸嘴Z軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴Z軸;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_Off_吸嘴Z軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴Z軸;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_On_吸嘴Y軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴Y軸;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_Off_吸嘴Y軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴Y軸;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_On_吸嘴X軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴X軸;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void btn_Off_吸嘴X軸_Click(object sender, EventArgs e)
        {
            int axis = 吸嘴X軸;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            WMX3_establish_Commu();
        }
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            WMX3_destroy_Commu();
        }

        private void btnSetHome_Click(object sender, EventArgs e)
        {
            int axis;

            axis = 吸嘴R軸;
            WMX3_SetHomePosition(axis);

            axis = 吸嘴Z軸;
            WMX3_SetHomePosition(axis);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //WMX3通訊狀態
            int getCommuStatus = WMX3_check_Commu();
            if (getCommuStatus == 1) {
                label1.Text = "連線中";
                label1.ForeColor = Color.Red;
            } else {
                label1.Text = "尚未連線";
                label1.ForeColor = Color.Black;
            }

            //region 讀取軸狀態
            int rslt = 0;
            int axis = 0;
            string position = "";
            string speed    = "";

            axis = 吸嘴R軸;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if(rslt == 1) {
                btn_On_吸嘴R軸.BackColor = Color.Red;
            } else {
                btn_On_吸嘴R軸.BackColor = Color.Green;
            }
            AcPos0.Text = position;
            AcSpd0.Text = speed;

            axis = 吸嘴Z軸;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1) {
                btn_On_吸嘴Z軸.BackColor = Color.Red;
            } else {
                btn_On_吸嘴Z軸.BackColor = Color.Green;
            }
            AcPos1.Text = position;
            AcSpd1.Text = speed;

            axis = 吸嘴X軸;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1)
            {
                btn_On_吸嘴X軸.BackColor = Color.Red;
            }
            else
            {
                btn_On_吸嘴X軸.BackColor = Color.Green;
            }
            AcPos7.Text = position;
            AcSpd7.Text = speed;

            axis = 吸嘴Y軸;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1)
            {
                btn_On_吸嘴Y軸.BackColor = Color.Red;
            }
            else
            {
                btn_On_吸嘴Y軸.BackColor = Color.Green;
            }
            AcPos3.Text = position;
            AcSpd3.Text = speed;
        }

        private void btnPosition01_Click(object sender, EventArgs e)
        {
            //單點動

            int axis;
            int position;
            int speed;
            int accel;
            int daccel;

            axis     = 吸嘴R軸;
            position = 27500;
            speed    = 10000;
            accel    = 10000;
            daccel   = 10000;
            WMX3_Pivot(axis, position, speed, accel, daccel);

            axis     = 吸嘴Z軸;
            position = 41370-1000;
            speed    = 10000;
            accel    = 10000;
            daccel   = 10000;
            WMX3_Pivot(axis, position, speed, accel, daccel);
        }

        private void btnNozzleDownPos_Click(object sender, EventArgs e)
        {
            //單點動
            int axis;
            int position;
            int speed;
            int accel;
            int daccel;

            axis     = 吸嘴Z軸;
            position = 26300;
            speed    = 10000;
            accel    = 10000;
            daccel   = 10000;
            WMX3_Pivot(axis, position, speed, accel, daccel);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            int isOn = 0;
            int axis = 0;

            axis = 0;
            WMX3_ServoOnOff(axis, isOn);

            axis = 1;
            WMX3_ServoOnOff(axis, isOn);
        }

        private void btn_AlarmRST_Click(object sender, EventArgs e)
        {
            motion.AxisControl.ClearAmpAlarm((int)NUD_Motor_NO.Value);
        }














        /// <summary>
        /// Reserve function
        /// </summary>
        /// 
        private void button1_Click(object sender, EventArgs e)
        {
            //inspector1.xInit();
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

        // 設定伺服器端的 IP 和 Port
        public IPAddress ip   = IPAddress.Parse("192.168.2.124");
        public int       port = 6032;
        TcpListener server;
        TcpClient   client;

        //Command Header
        public const string strCMDU15Header = "AB,1,";

        public uint u32Frequency = 0; 
            public uint u32VibrationSource1_StartPhase = 0;
            public uint u32VibrationSource1_StopPhase  = 0;
            public uint u32VibrationSource2_StartPhase = 0;
            public uint u32VibrationSource2_StopPhase  = 0;
            public uint u32VibrationSource3_StartPhase = 0;
            public uint u32VibrationSource3_StopPhase  = 0;
            public uint u32VibrationSource4_StartPhase = 0;
            public uint u32VibrationSource4_StopPhase  = 0;
            public uint u32BlackDepotSource_StartPhase = 0;
            public uint u32BlackDepotSource_StopPhase  = 0;
            public uint u32VibrationSource1_Power = 0;
            public uint u32VibrationSource2_Power = 0;
            public uint u32VibrationSource3_Power = 0;
            public uint u32VibrationSource4_Power = 0;
            public uint u32BlackDepotSource_Power = 0;

        public uint u32LED_Level = 0;


        public void apiEstablishTCPVibration() {
            if (isEstablishTCP == false) {
                isEstablishTCP = true;

                try {
                    // 建立 TcpListener 來監聽指定的 IP 和 Port
                    server = new TcpListener(ip, port);
                    server.Start();
                    Console.WriteLine("伺服器啟動中，等待客戶端連接...");

                    // 接受來自客戶端的連接
                    client = server.AcceptTcpClient();
                    Console.WriteLine("客戶端已連接！");
                } catch (SocketException ex) {
                    Console.WriteLine("SocketException: " + ex.Message);
                }

                // 延遲0.5秒（500毫秒）
                Thread.Sleep(500);

                if (true) {  //Test Mode
                    SetVibration(u32Frequency, u32VibrationSource1_StartPhase,
                                               u32VibrationSource1_StopPhase,
                                               u32VibrationSource2_StartPhase,
                                               u32VibrationSource2_StopPhase,
                                               u32VibrationSource3_StartPhase,
                                               u32VibrationSource3_StopPhase,
                                               u32VibrationSource4_StartPhase,
                                               u32VibrationSource4_StopPhase,
                                               u32BlackDepotSource_StartPhase,
                                               u32BlackDepotSource_StopPhase,
                                               u32VibrationSource1_Power,
                                               u32VibrationSource2_Power,
                                               u32VibrationSource3_Power,
                                               u32VibrationSource4_Power,
                                               u32BlackDepotSource_Power);
                }  //end of if (true) {  //Test Mode

                //Vibration
                if (true) { //Test LED Level
                    SetVibrationLED(u32LED_Level);
                }  //end of if (true) { //Test LED Level

                // 處理客戶端連接的任務可交由不同的線程執行
                if (false) {
                    Thread thread = new Thread(HandleClient);
                    thread.Start(client);
                }
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

        public void SetVibration(uint u32Px1_FRQ, uint u32Px2_M1S,
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

            // 確認變量數值
            if (u32Px1_FRQ  > 1500) u32Px1_FRQ  = 1500;
            if (u32Px2_M1S  > 1000) u32Px2_M1S  = 1000;
            if (u32Px3_M1E  > 1000) u32Px3_M1E  = 1000;
            if (u32Px4_M2S  > 1000) u32Px4_M2S  = 1000;
            if (u32Px5_M2E  > 1000) u32Px5_M2E  = 1000;
            if (u32Px6_M3S  > 1000) u32Px6_M3S  = 1000;
            if (u32Px7_M3E  > 1000) u32Px7_M3E  = 1000;
            if (u32Px8_M4S  > 1000) u32Px8_M4S  = 1000;
            if (u32Px9_M4E  > 1000) u32Px9_M4E  = 1000;
            if (u32Px10_MDS > 1000) u32Px10_MDS = 1000;
            if (u32Px11_MDE > 1000) u32Px11_MDE = 1000;
            if (u32Px12_M1P > 1000) u32Px12_M1P = 1000;
            if (u32Px13_M2P > 1000) u32Px13_M2P = 1000;
            if (u32Px14_M3P > 1000) u32Px14_M3P = 1000;
            if (u32Px15_M4P > 1000) u32Px15_M4P = 1000;
            if (u32Px16_MDP > 1000) u32Px16_MDP = 1000;

            // 儲存變量
            uint u32SaveFrequency                   = 0;
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
            uint u32SaveVibrationSource1_Power      = 0;
            uint u32SaveVibrationSource2_Power      = 0;
            uint u32SaveVibrationSource3_Power      = 0;
            uint u32SaveVibrationSource4_Power      = 0;
            uint u32SaveBlackDepotSource_Power      = 0;

            // 檢查變量是否變更
            if ( u32SaveFrequency                   != u32Px1_FRQ  ||
                 u32SaveVibrationSource1_StartPhase != u32Px2_M1S  ||
                 u32SaveVibrationSource1_StopPhase  != u32Px3_M1E  ||
                 u32SaveVibrationSource2_StartPhase != u32Px4_M2S  ||
                 u32SaveVibrationSource2_StopPhase  != u32Px5_M2E  ||
                 u32SaveVibrationSource3_StartPhase != u32Px6_M3S  ||
                 u32SaveVibrationSource3_StopPhase  != u32Px7_M3E  ||
                 u32SaveVibrationSource4_StartPhase != u32Px8_M4S  ||
                 u32SaveVibrationSource4_StopPhase  != u32Px9_M4E  ||
                 u32SaveBlackDepotSource_StartPhase != u32Px10_MDS ||
                 u32SaveBlackDepotSource_StopPhase  != u32Px11_MDE ||
                 u32SaveVibrationSource1_Power      != u32Px12_M1P ||
                 u32SaveVibrationSource2_Power      != u32Px13_M2P ||
                 u32SaveVibrationSource3_Power      != u32Px14_M3P ||
                 u32SaveVibrationSource4_Power      != u32Px15_M4P ||
                 u32SaveBlackDepotSource_Power      != u32Px16_MDP ) {

                // 更新保存的變量
                u32SaveFrequency                   = u32Px1_FRQ;
                u32SaveVibrationSource1_StartPhase = u32Px2_M1S;
                u32SaveVibrationSource1_StopPhase  = u32Px3_M1E;
                u32SaveVibrationSource2_StartPhase = u32Px4_M2S;
                u32SaveVibrationSource2_StopPhase  = u32Px5_M2E;
                u32SaveVibrationSource3_StartPhase = u32Px6_M3S;
                u32SaveVibrationSource3_StopPhase  = u32Px7_M3E;
                u32SaveVibrationSource4_StartPhase = u32Px8_M4S;
                u32SaveVibrationSource4_StopPhase  = u32Px9_M4E;
                u32SaveBlackDepotSource_StartPhase = u32Px10_MDS;
                u32SaveBlackDepotSource_StopPhase  = u32Px11_MDE;
                u32SaveVibrationSource1_Power      = u32Px12_M1P;
                u32SaveVibrationSource2_Power      = u32Px13_M2P;
                u32SaveVibrationSource3_Power      = u32Px14_M3P;
                u32SaveVibrationSource4_Power      = u32Px15_M4P;
                u32SaveBlackDepotSource_Power      = u32Px16_MDP;

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
        }

        public void SetVibrationLED(uint u32LEDLevel) {
            Console.WriteLine("更新 LED 等級");
            Px1_SendCMD(client, xe_U15_CMD.xeUC_TestMode_LightLevel, u32LEDLevel);
        }

        public void SendTCPData(object obj, string data) {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            if (client != null && stream != null) {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(data);
                stream.Write(bytesToSend, 0, bytesToSend.Length);
            }
        }

        public void btnVibrationInit_Click(object sender, EventArgs e) {
            //Vibration
            apiEstablishTCPVibration();
        }

        private void btnVibrationStop_Click(object sender, EventArgs e)
        {
            if (isEstablishTCP == true) {
                uint flagTestOn = 0;
                Px1_SendCMD(client, xe_U15_CMD.xeUC_TestMode_FunctionOn, flagTestOn);
            }
        }

        private void btnVibrationLED_Click(object sender, EventArgs e)
        {
            //Vibration
            apiEstablishTCPVibration();

            u32LED_Level = 50;
            SetVibrationLED(u32LED_Level);
        }

        private void btnVibrationLEDOff_Click(object sender, EventArgs e)
        {
            //Vibration
            apiEstablishTCPVibration();

            uint u32SaveLED_Level = 0;
            SetVibrationLED(u32SaveLED_Level);
        }

        private void btnChgDeg_Click(object sender, EventArgs e)
        {
            try {
                // 取得txtDeg裡的浮點數
                float fdegree = float.Parse(txtDeg.Text);

                // 使用 degree 變數進行後續操作
                int iChgDeg = (int)(fdegree * 100);

                //取得當前吸嘴角度
                int ideg = int.Parse(AcPos0.Text);

                //計算補正至90度的數值
                int iTargetDeg = ideg + iChgDeg;

                //執行旋轉吸嘴
                int axis = 0;
                int position = iTargetDeg;
                int speed = 10000;
                int accel = 10000;
                int daccel = 10000;
                WMX3_Pivot(axis, position, speed, accel, daccel);

            } catch (FormatException) {
                MessageBox.Show("請輸入有效的浮點數");
            }
        }

        private void btnChgX_Click(object sender, EventArgs e)
        {
            try
            {
                // 取得txtX裡的浮點數
                float ftxtX = float.Parse(txtX.Text);

                // 使用 degree 變數進行後續操作
                int itxtX = (int)(ftxtX * 1000);

                //取得當前吸嘴吸料盤校正用X
                int rslt = 0;
                int axis = 0;
                string position = "";
                string speed = "";

                axis = 吸嘴X軸;
                rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
                if (rslt == 1)
                {
                    btn_On_吸嘴X軸.BackColor = Color.Red;
                }
                else
                {
                    btn_On_吸嘴X軸.BackColor = Color.Green;
                }
                AcPos7.Text = position;
                AcSpd7.Text = speed;

                //計算補正至X的數值
                //int iTargetX = ideg + iChgDeg;

                //執行旋轉吸嘴
                //int axis = 0;
                //int position = iTargetDeg;
                //int speed = 10000;
                //int accel = 10000;
                //int daccel = 10000;
                //WMX3_Pivot(axis, position, speed, accel, daccel);

            }
            catch (FormatException)
            {
                MessageBox.Show("請輸入有效的浮點數");
            }
        }

        private void btnChgY_Click(object sender, EventArgs e)
        {

        }






        //PCCP Xavier Tsai, added for testing <END>



    }
}
