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
using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;

namespace InjectorInspector
{
    public partial class Form1 : Form
    {
        WMX3Api wmx = new WMX3Api();
        Io io = new Io();
        CoreMotion motion = new CoreMotion();
        CoreMotionStatus CmStatus = new CoreMotionStatus();
        EngineStatus EnStatus = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch = new Stopwatch();
        AdvancedMotion advmon= new AdvancedMotion();
       


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            wmx.CreateDevice("C:\\Program Files\\SoftServo\\WMX3", DeviceType.DeviceTypeNormal, 10000);//建立裝置
            wmx.SetDeviceName("DLF");//設定裝置名稱

            int A = motion.Config.SetGearRatio(0, 1048576, 10000);//設置齒輪比
            int B = motion.Config.SetGearRatio(1, 1048576, 10000);//設置齒輪比


            //wmx.CloseDevice();
        }



        private void strCommunication_Click_1(object sender, EventArgs e)
        {
            int ret;
            ret = wmx.StartCommunication();
            if (ret != 0)
            {
                string str = WMX3Api.ErrorToString(ret);
                MessageBox.Show(str);
            }
        }
        private void stoCommunication_Click_1(object sender, EventArgs e)
        {
            wmx.StopCommunication();//WMX3通訊停止
        }

        private void svOn0_Click(object sender, EventArgs e)
        {
            int ret = motion.AxisControl.SetServoOn(0, 1);//啟動伺服
            if (ret != 0)
            {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }

        }
        private void svOn1_Click_1(object sender, EventArgs e)
        {
            int ret = motion.AxisControl.SetServoOn(1, 1);//啟動伺服
            if (ret != 0)
            {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }

        }
        private void svOff0_Click_1(object sender, EventArgs e)
        {
            int ret = motion.AxisControl.SetServoOn(0, 0);
            if (ret != 0)
            {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }
        }
    
        private void svOff1_Click(object sender, EventArgs e)
        {
            int ret = motion.AxisControl.SetServoOn(1, 0);
            if (ret != 0)
            {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }
        }
        private void Pos_Click(object sender, EventArgs e)
        {
            Config.HomeParam homeParam = new Config.HomeParam();
            motion.Config.GetHomeParam(0, ref homeParam);
            homeParam.HomeType = Config.HomeType.CurrentPos;

            motion.Home.StartHome(0);

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            motion.GetStatus(ref CmStatus);//狀態回授
            #region WMX3通訊狀態
            switch (CmStatus.EngineState)//讀取當前通訊狀態
            {
                case EngineState.Running:

                    label12.Text = "尚未連線";
                    label12.ForeColor = Color.Black;


                    break;

                case EngineState.Communicating:

                    label12.Text = "連線中";
                    label12.ForeColor = Color.Red;

                    break;
            }
            #endregion WMX3通訊狀態
            #region 讀取軸狀態

            CoreMotionAxisStatus cmAxis_0 = CmStatus.AxesStatus[0];
            CoreMotionAxisStatus cmAxis_1 = CmStatus.AxesStatus[1];
            //讀取SV ON狀態
            if (cmAxis_0.ServoOn == true)
            { svOn0.BackColor = Color.Red; }
            else
            { svOn0.BackColor = Color.Green;}
            if (cmAxis_1.ServoOn == true)
            { svOn1.BackColor = Color.Red; }
            else
            { svOn1.BackColor = Color.Green; }
            //讀取目前位置
            string P0= cmAxis_0.ActualPos.ToString();           
            AcPos0.Text = P0.Substring(0,Math.Min(P0.Length,6));
            AcSpd0.Text = cmAxis_0.ActualVelocity.ToString();
            AcTqr0.Text = cmAxis_0.ActualTorque.ToString();

            string P1 = cmAxis_1.ActualPos.ToString();
            AcPos1.Text = P1.Substring(0, Math.Min(P1.Length, 6));
            AcSpd1.Text = cmAxis_1.ActualVelocity.ToString();
            AcTqr1.Text = cmAxis_1.ActualTorque.ToString();
        }

        private void stPos1_Click_1(object sender, EventArgs e)
        {
            int ret1 = motion.AxisControl.SetAxisCommandMode(0, AxisCommandMode.Position);//位置控制設定
            Motion.PosCommand pos = new Motion.PosCommand();//POS參數設置
                pos.Axis = 0;//軸
                pos.Target = int.Parse(textBox2.Text);//指定位置
                pos.Profile.Type = ProfileType.Trapezoidal;//運動模式
                pos.Profile.Velocity = int.Parse(textBox1.Text);//速度
                pos.Profile.Acc = 100000 * 10;//加速度
                pos.Profile.Dec = 100000 * 10;//減速度
                int ret = motion.Motion.StartPos(pos);//啟動POS運轉
   

            if (ret != 0)
                {
                    string ers = CoreMotion.ErrorToString(ret);//如果無法通訊則報錯誤給使用者
                 //   textBox12.Text += "軸" + textBox3.Text + ":" + ers + "\r\n";

                }

            
        }

        private void button2_Click_2(object sender, EventArgs e)
       {
            AxisHomeParam.HomeType = Config.HomeType.CurrentPos;

            int ret = motion.Config.SetHomeParam(0, AxisHomeParam);//設置原點參數
            int ret1 = motion.Config.SetHomeParam(1, AxisHomeParam);//設置原點參數
            if (ret != 0)
           {
               string ers = CoreMotion.ErrorToString(ret);//如果無法通訊則報錯誤給使用者
             //   textBox12.Text += "軸" + textBox9.Text + "設置HOME錯誤" + ers + "\r\n";
            }
            int ret2 = motion.Home.StartHome(0);//開始回原點
            int ret3 = motion.Home.StartHome(1);//開始回原點
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Velocity.VelCommand vel=new Velocity.VelCommand();
            Torque.TrqCommand trq=new Torque.TrqCommand();

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

        private void button2_Click_1(object sender, EventArgs e)
        {
            int ret2 = motion.Velocity.Stop(0);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
          
            AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
            
            path.Axis[0] = 0;
            path.Axis[1] = 1;

            path.EnableConstProfile = 1;
            path.Profile[0].Type = ProfileType.Trapezoidal;
            path.Profile[0].Velocity = 10000;
            path.Profile[0].Acc = 10000;
            path.Profile[0].Dec = 10000;

            path.NumPoints = 6;

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
            int a1=advmon.AdvMotion.StartPathIntplPos(path);
            //int a = WmxLib_Adv.AdvMotion.StartCSplinePos(0, splineCommand, 9, splinePoint);
            //AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
            //static AdvancedMotion WmxLib_Adv = new AdvancedMotion(Wmx3Lib);
            //AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();


        }

  



        #endregion 讀取軸狀態




        // Homing.
        // Config.HomeParam homeParam = new Config.HomeParam();
        // motion.Config.GetHomeParam(0, ref homeParam);
        // homeParam.HomeType = Config.HomeType.CurrentPos;
        // Motion.Config.SetHomeParam(0, homeParam);

        //Motion.Motion.Wait(0);
    }
}

