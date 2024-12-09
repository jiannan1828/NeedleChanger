using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//WMX3
using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace InjectorInspector
{
    internal class ServoControl
    {
        //軸的對應號碼
        public const int 吸嘴X軸 = 3;
        public const int 吸嘴Y軸 = 7;
        public const int 吸嘴Z軸 = 1;
        public const int 吸嘴R軸 = 0;


        //PCCP Xavier Tsai, added for testing <START>
        //WMX3
        WMX3Api wmx = new WMX3Api();
        CoreMotion motion = new CoreMotion();
        CoreMotionStatus CmStatus = new CoreMotionStatus();
        EngineStatus EnStatus = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch = new Stopwatch();
        AdvancedMotion advmon = new AdvancedMotion();
        Io io = new Io();
        public static CoreMotionAxisStatus[] cmAxis = new CoreMotionAxisStatus[8];
        public System.Windows.Forms.NumericUpDown NUD_Motor_NO;



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

            axis = 吸嘴X軸;
            int D = motion.Config.SetGearRatio(axis, 1000, 100);

            axis = 吸嘴Y軸;
            int H = motion.Config.SetGearRatio(axis, 1048576, 2000);
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
            if (false)
            {
                wmx.StopCommunication();
            }
            else
            {
                wmx.StopCommunication(10000);
            }

            //Quit device.
            wmx.CloseDevice();
            motion.Dispose();
            wmx.Dispose();

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

            switch (axis)
            {
                case 0:
                case 1:
                    motion.Config.GetHomeParam(axis, ref AxisHomeParam);//讀取原點模式

                    //設定為讀取內部home
                    AxisHomeParam.HomeType = Config.HomeType.ZPulse;

                    //尋找內部home
                    rslt = motion.Config.SetHomeParam(axis, AxisHomeParam);//設置原點參數

                    if (rslt != 0)
                    {
                        string ers = CoreMotion.ErrorToString(rslt);//如果無法通訊則報錯誤給使用者
                    }
                    break;
            }

            //開始回原點
            rslt = motion.Home.StartHome(axis);

            return rslt;
        }  //end of public int WMX3_SetHomePosition(int axis)

    }
}
