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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //inspector1.xInit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inspector1.xInit();
            inspector1.on下視覺 = apiCallBackTest;
        }



        //PCCP Xavier Tsai, added for testing <START>
        public static WMX3Api Wmx3Lib = new WMX3Api();
        public static EngineStatus EnStatus = new EngineStatus();
        public static CoreMotionStatus CmStatus = new CoreMotionStatus();
        public static CoreMotion Wmx3Lib_cm = new CoreMotion();//Wmx3Lib);
        public static AdvancedMotion WmxLib_Adv = new AdvancedMotion();// Wmx3Lib);
        public static CoreMotionAxisStatus[] cmAxis = new CoreMotionAxisStatus[8];
        public System.Windows.Forms.NumericUpDown NUD_Motor_NO;

        //WMX3
        WMX3Api wmx = new WMX3Api();
        //Io io = new Io();
        CoreMotion motion = new CoreMotion();
        //CoreMotionStatus CmStatus = new CoreMotionStatus();
        //EngineStatus EnStatus = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch = new Stopwatch();
        AdvancedMotion advmon = new AdvancedMotion();

        int ErrorCode;

        public int cntcallback = 0;

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
            int A = motion.Config.SetGearRatio(0, 1048576, 10000);
            int B = motion.Config.SetGearRatio(1, 1048576, 10000);

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
            speed = cmAxis.ActualVelocity.ToString();
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

            pos.Profile.Type = ProfileType.Trapezoidal;  //運動模式
            pos.Axis = axis;    //軸
            pos.Target = pivot;   //指定位置
            pos.Profile.Velocity = speed;   //速度
            pos.Profile.Acc = accel;   //加速度
            pos.Profile.Dec = daccel;  //減速度

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

            //讀取當前座標
            AxisHomeParam.HomeType = Config.HomeType.CurrentPos;

            //設置原點參數
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

        private void btn_DeviceCreate_Click(object sender, EventArgs e)
        {
            Wmx3Lib.CreateDevice("C:\\Program Files\\SoftServo\\WMX3\\", DeviceType.DeviceTypeNormal, 100000);
            Wmx3Lib.SetDeviceName("MotorControl");
            label1.Text = "創建無異常";
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            int ret = Wmx3Lib.StartCommunication();//WMX3通訊啟動
            if (ret != 0)
            {

                string str = WMX3Api.ErrorToString(ret); //如果無法通訊則報錯誤給使用者
                label1.Text = "裝置連線失敗-" + str;
            }
            else
            {
                label1.Text = "裝置已連線";
                //Main_Timer.Enabled = true;

                //Thread TR_M_Status_Read = new Thread(M_Status_Read);
                //TR_M_Status_Read.IsBackground = true;
                //TR_M_Status_Read.Start();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Stop Communication.
            Wmx3Lib.StopCommunication(10000);
            //Quit device.
            Wmx3Lib.CloseDevice();
            Wmx3Lib_cm.Dispose();
            Wmx3Lib.Dispose();

            

            //sw.Close();
        }

        private void btn_AlarmRST_Click(object sender, EventArgs e)
        {
            Wmx3Lib_cm.AxisControl.ClearAmpAlarm((int)NUD_Motor_NO.Value);
        }

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
            label2.Text = string.Format("分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);

            //黑色料倉
            bool 料倉有料 = inspector1.xInsp入料();
            label3.Text = string.Format("料倉有料 = {0}", 料倉有料);

            //光源震動盤
            List<Vector3> pins;
            bool 料盤有料 = inspector1.xInsp震動盤(out pins);
            Vector3 temp = (料盤有料) ? pins.First() : new Vector3();
            label4.Text = string.Format("震動盤 = {0} X = {1:F2} Y = {2:F2} θ = {3:F2}", 料盤有料, temp.X, temp.Y, temp.θ);

            if (inspector1.Inspected && inspector1.InspectOK)
            {
                double deg = inspector1.PinDeg;
                label5.Text = string.Format("吸嘴分析  θ = {0:F2}", deg);
            }
            else
                label5.Text = "吸嘴分析失敗";

            int cntdebug = inspector1.RecvCount;


        }
        //PCCP Xavier Tsai, added for testing <END>



    }
}
