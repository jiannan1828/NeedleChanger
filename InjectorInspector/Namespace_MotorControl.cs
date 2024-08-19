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

using Namespace_MotorControl;

namespace Namespace_MotorControl
{
	public class PublicClass_MotorControl
    {
        WMX3Api wmx = new WMX3Api();
        Io io = new Io();
        CoreMotion motion = new CoreMotion();
        CoreMotionStatus CmStatus = new CoreMotionStatus();
        EngineStatus EnStatus = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch = new Stopwatch();
        AdvancedMotion advmon = new AdvancedMotion();

        public enum xeStatusWMX3
        {
            xeSW_Null = 0,
            xeSW_Init = 1,
            xeSW_StartCommu,
            xeSW_StopCommu,
            xeSW_SetMotorIndex,
            xeSW_ClsMotorIndex,
            xeSW_VelocityStart_MotorIndex,
            xeSW_VelocityStop_MotorIndex,
        }

        // 構造函數
        public int Class1()
		{

            return 8787;
		}

        public xeStatusWMX3 InitWMX3()
        {
            xeStatusWMX3 xeSW_rslt = xeStatusWMX3.xeSW_Init;

            if(true) {
                wmx.CreateDevice("C:\\Program Files\\SoftServo\\WMX3", DeviceType.DeviceTypeNormal, 10000);//建立裝置
                wmx.SetDeviceName("DLF");//設定裝置名稱

                int A = motion.Config.SetGearRatio(0, 1048576, 10000);//設置齒輪比
                int B = motion.Config.SetGearRatio(1, 1048576, 10000);//設置齒輪比

                //wmx.CloseDevice();
            }

            return xeSW_rslt;
        }

        public xeStatusWMX3 StartWMX3()
        {
            xeStatusWMX3 xeSW_rslt = xeStatusWMX3.xeSW_StartCommu;

            int ret;
            ret = wmx.StartCommunication();
            if (ret != 0) {
                string str = WMX3Api.ErrorToString(ret);
                MessageBox.Show(str);
            }

            return xeSW_rslt;
        }

        public xeStatusWMX3 StopWMX3()
        {
            xeStatusWMX3 xeSW_rslt = xeStatusWMX3.xeSW_StopCommu;

            wmx.StopCommunication();//WMX3通訊停止

            return xeSW_rslt;
        }

        public xeStatusWMX3 SetWMX3_MotorIndex(int i32Index)
        {
            xeStatusWMX3 xeSW_rslt = xeStatusWMX3.xeSW_SetMotorIndex;

            int i32MotorIndex = i32Index;
            int newStatus = 1;  //Active
            int ret = motion.AxisControl.SetServoOn(i32MotorIndex, newStatus);//啟動伺服
            if (ret != 0) {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }

            return xeSW_rslt;
        }

        public xeStatusWMX3 ClsWMX3_MotorIndex(int i32Index)
        {
            xeStatusWMX3 xeSW_rslt = xeStatusWMX3.xeSW_ClsMotorIndex;

            int i32MotorIndex = i32Index;
            int newStatus = 0;  //Disactive
            int ret = motion.AxisControl.SetServoOn(i32MotorIndex, newStatus);
            if (ret != 0) {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }

            return xeSW_rslt;
        }

        public xeStatusWMX3 VelocityWMX3_MotorIndex(int i32Index, int i32Velocity, int i32AcceIncrease, int i32AcceDecrease)
        {
            xeStatusWMX3 xeSW_rslt = xeStatusWMX3.xeSW_Null;

            int iMotorIndex   = i32Index;
            int iVelocity     = i32Velocity;
            int iAcceIncrease = i32AcceIncrease;
            int iAcceDecrease = i32AcceDecrease;

            if (0 < iVelocity) {
                xeSW_rslt = xeStatusWMX3.xeSW_VelocityStart_MotorIndex;

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
                vel.Axis = iMotorIndex;
                vel.Profile.Type = ProfileType.Trapezoidal;
                vel.Profile.Velocity = iVelocity;
                vel.Profile.Acc = iAcceIncrease;
                vel.Profile.Dec = iAcceDecrease;

                //Execute a velocity command
                if(true) {
                    int ret1 = motion.Velocity.StartVel(vel);
                }
            } else {  //iVelocity == 0
                xeSW_rslt = xeStatusWMX3.xeSW_VelocityStop_MotorIndex;

                if(true) {
                    int ret2 = motion.Velocity.Stop(iMotorIndex);
                }
            }

            return xeSW_rslt;
        }
    }
}
