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
using System.Data;

namespace InjectorInspector
{
    //軸的對應號碼
    public enum WMX3軸定義
    {  // start of public enum WMX3軸定義
        AXIS_START = -1,
            吸嘴X軸 = 3,  //小線碼
            吸嘴Y軸 = 7,  //YASKAWA
            吸嘴Z軸 = 1,  //VCM伸縮
            吸嘴R軸 = 0,  //VCM旋轉

            載盤X軸 = 4,  //YASKAWA
            載盤Y軸 = 2,  //大線碼

            植針Z軸 = 5,  //YASKAWA
            植針R軸 = 6,  //YASKAWA

            工作門  = 8,  //工作門
        AXIS_END = 999,
        YASKAWA  = 1048576,
        DELTA_ASDA_B2 = 1280000,
        DELTA_ASDA_B3 = 16777216,
    }  // end of public enum WMX3軸定義

    public enum WMX3IO對照
    {
                                     //out:
        pxeIO_Addr4           =  4,  //4
        pxeIO_擺放座蓋板      = 00,  //0 擺放座蓋板缸
        pxeIO_吸料真空電磁閥  = 01,  //1 稀料真空閥
        pxeIO_堵料吹氣缸      = 02,  //2 賭料吹氣感缸
        pxeIO_接料區氣桿      = 03,  //3 接料區缸
        pxeIO_植針吹氣        = 04,  //4 職真吹氣電磁閥3(未驗)
        pxeIO_收料區缸        = 05,  //5 收料區缸
        pxeIO_堵料吹氣        = 06,  //6 賭料吹氣
        pxeIO_NA_O_07         = 07,  //7 未接

        pxeIO_Addr5           =  5,  //5
        pxeIO_載盤真空閥      = 10,  //0 仔盤真空電磁閥
        pxeIO_Socket真空2     = 11,  //1 socket真空2電磁閥
        pxeIO_載盤破真空      = 12,  //2 仔盤破電磁閥
        pxeIO_Socket破真空2   = 13,  //3 socket破真空2電磁閥
        pxeIO_Socket真空1     = 14,  //4 socket真空1電磁閥
        pxeIO_擺放座吸真空    = 15,  //5 擺放做溪真空
        pxeIO_Socket破真空1   = 16,  //6 socket破空1電磁閥
        pxeIO_擺放座破真空    = 17,  //7 擺放做破真空

        pxeIO_Addr6           =  6,  //6
        pxeIO_取料吸嘴吸      = 20,  //0 取料吸嘴溪真空
        pxeIO_NA_O_21         = 21,  //1 未接
        pxeIO_取料吸嘴破真空  = 22,  //2 取料吸嘴破真空
        pxeIO_NA_O_23         = 23,  //3 未接
        pxeIO_NA_O_24         = 24,  //4 未接
        pxeIO_HEPA            = 25,  //5 hipa
        pxeIO_NA_O_26         = 26,  //6 未接
        pxeIO_LIGHT           = 27,  //7 艙內燈

        pxeIO_Addr7           =  7,  //7
        pxeIO_面板右按鈕綠燈  = 30,  //0 面板按鈕綠燈(右)
        pxeIO_機台紅燈        = 31,  //1 紅燈
        pxeIO_面板中按鈕綠燈  = 32,  //2 面板按鈕綠燈(中)
        pxeIO_機台黃燈        = 33,  //3 黃燈
        pxeIO_面板左按鈕紅燈  = 34,  //4 面板按鈕紅燈(左)
        pxeIO_機台綠燈        = 35,  //5 綠燈
        pxeIO_NA_O_36         = 36,  //6 未接
        pxeIO_Buzzer          = 37,  //7 Buzzer

                                     //In:
        pxeIO_Addr28          = 28,  //28
        pxeIO_載盤Y軸後極限   = 00,  //0 陷馬仔盤Y軸後極限
        pxeIO_取料Y軸後極限   = 01,  //1 安川取料Y軸後極限
        pxeIO_載盤Y軸前極限   = 02,  //2 陷馬仔盤Y軸前極限
        pxeIO_取料Y軸前極限   = 03,  //3 安川取料Y軸前極限
        pxeIO_取料X軸後極限   = 04,  //4 陷馬取料X後極限
        pxeIO_NA05            = 05,  //5 未接
        pxeIO_取料X軸前極限   = 06,  //6 陷馬取料X前極限
        pxeIO_NA07            = 07,  //7 未接

        pxeIO_Addr29          = 29,  //29
        pxeIO_植針Z軸後極限   = 10,  //0 安川直真z軸後極限
        pxeIO_NA11            = 11,  //1 未接
        pxeIO_植針Z軸前極限   = 12,  //2 安川直真z軸前極限
        pxeIO_NA13            = 13,  //3 未接
        pxeIO_載盤X軸前極限   = 14,  //4 安川仔盤x軸前極限
        pxeIO_NA15            = 15,  //5 未接
        pxeIO_載盤X軸後極限   = 16,  //6 安川仔盤x軸後極限
        pxeIO_NA17            = 17,  //7 未接

        pxeIO_Addr30          = 30,  //30 
        pxeIO_載盤真空檢1     = 20,  //0 在盤真空檢1
        pxeIO_Socket2真空檢1  = 21,  //1 SOCKET2真空檢之1
        pxeIO_載盤真空檢2     = 22,  //2 在盤真空檢2(有問題)
        pxeIO_Socket2真空檢2  = 23,  //3 SOCKET2真空檢之2
        pxeIO_Socket1真空檢1  = 24,  //4 SOCKET1真空檢之1
        pxeIO_擺放座真空檢1   = 25,  //5 擺放做真空檢1
        pxeIO_Socket1真空檢2  = 26,  //6 SOCKET1真空檢之2
        pxeIO_擺放座真空檢2   = 27,  //7 擺放做真空檢2

        pxeIO_Addr31          = 31,  //31
        pxeIO_吸嘴真空檢1     = 30,  //0 吸嘴真空檢之1
        pxeIO_NA31            = 31,  //1 未接
        pxeIO_吸嘴真空檢2     = 32,  //2 吸嘴真空檢之2 
        pxeIO_取料NG收料盒    = 33,  //3 取料ng收料和
        pxeIO_兩點組合壓力檢1 = 34,  //4 兩點組合壓力剪之1
        pxeIO_堵料收料盒      = 35,  //5 賭料收料額
        pxeIO_兩點組合壓力檢2 = 36,  //6 兩點組合壓力剪之2
        pxeIO_吸料收料盒      = 37,  //7 西料收料和

        pxeIO_Addr32          = 32,  //32
        pxeIO_復歸按鈕        = 40,  //0 賦歸
        pxeIO_NA41            = 41,  //1 未接
        pxeIO_啟動按鈕        = 42,  //2 啟動
        pxeIO_NA43            = 43,  //3 未接
        pxeIO_停止按鈕        = 44,  //4 停止
        pxeIO_NA45            = 45,  //5 未接
        pxeIO_緊急停止按鈕    = 46,  //6 緊急停止
        pxeIO_NA47            = 47,  //7 未接

        pxeIO_Addr33          = 33,  //33
        pxeIO_NA50            = 50,  //0 位階
        pxeIO_NA51            = 51,  //1 位階
        pxeIO_NA52            = 52,  //2 位階
        pxeIO_NA53            = 53,  //3 位階
        pxeIO_NA54            = 54,  //4 位階
        pxeIO_NA55            = 55,  //5 位階
        pxeIO_NA56            = 56,  //6 位階
        pxeIO_NA57            = 57,  //7 位階

        pxeIO_Addr34          = 34,  //34
        pxeIO_上罩左側右門    = 60,  //0 上兆左側右們
        pxeIO_上罩右側右門    = 61,  //1 上兆右側右們
        pxeIO_上罩左側左門    = 62,  //2 上兆左側左門
        pxeIO_上罩右側左門    = 63,  //pxeIO_3 上兆右側左門
        pxeIO_上罩後側右門    = 64,  //pxeIO_4 上兆後側右們
        pxeIO_螢幕旁小門      = 65,  //5 螢幕旁小門
        pxeIO_上罩後側左門    = 66,  //6 上兆後側左門
        pxeIO_NA67            = 67,  //7 未接

        pxeIO_Addr35          = 35,  //35
        pxeIO_下支架左側右門  = 70,  //0 下支架左側右門
        pxeIO_NA71            = 71,  //1 未知
        pxeIO_下支架左側左門  = 72,  //2 下支架左側左門
        pxeIO_NA73            = 73,  //3 未知
        pxeIO_下支架右側右門  = 74,  //4 下支架右側右門
        pxeIO_NA75            = 75,  //5 未知
        pxeIO_下支架右側左門  = 76,  //6 下支架右側左門
    }

    //擴展定義字串轉換
    public static class ByteArrayExtensions
    {  // start of public static class ByteArrayExtensions

        // 自定義方法來將 byte[] 轉換為二進位字串
        public static string ToBinary(this byte[] byteArray)
        {
            StringBuilder binaryString = new StringBuilder();

            foreach (byte b in byteArray)
            {
                binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));  // 將每個 byte 轉換為二進位並補齊到 8 位
            }

            return binaryString.ToString();
        }

        // 將 byte[] 轉換為十六進位字符串
        public static string ToHex(this byte[] byteArray)
        {
            StringBuilder hexString = new StringBuilder(byteArray.Length * 2);  // 預估每個字節有 2 個十六進位字符

            foreach (byte b in byteArray)
            {
                hexString.AppendFormat("{0:X2}", b);  // 將每個字節轉換為兩位十六進位數字
            }

            return hexString.ToString();
        }

    }  //end of public static class ByteArrayExtensions

    //WMX3控制
    internal class ServoControl
    {  // start of internal class ServoControl

        //WMX3軸定義
        public const double dbRead = 99999.9;

        //WMX3
        private WMX3Api wmx;
        private CoreMotion motion;
        private CoreMotionStatus CmStatus;
        private EngineStatus EnStatus;
        private Config.HomeParam AxisHomeParam;
        private Stopwatch stopWatch;
        private AdvancedMotion advmon;
        private Io io;
        public static CoreMotionAxisStatus[] cmAxis = new CoreMotionAxisStatus[8];
        public System.Windows.Forms.NumericUpDown NUD_Motor_NO;

        public ServoControl()
        {  // 建構子，初始化物件
            CreateWMX3Handle();
        }

        ~ServoControl()
        {  // 解構子，釋放非托管資源
            KillWMX3Handle();
        }



        /// <summary>
        /// ServoMotor WMX3 Control API
        /// </summary>
        /// 

        public void CreateWMX3Handle()
        {
            //清除未知記憶體
            KillWMX3Handle();

            // 僅在初始化時進行一次賦值，避免重複初始化
            if (wmx == null)
            {
                wmx = new WMX3Api();
            }

            if (motion == null)
            {
                motion = new CoreMotion();
            }

            if (CmStatus == null)
            {
                CmStatus = new CoreMotionStatus();
            }

            if (EnStatus == null)
            {
                EnStatus = new EngineStatus();
            }

            if (AxisHomeParam == null)
            {
                AxisHomeParam = new Config.HomeParam();
            }

            if (stopWatch == null)
            {
                stopWatch = new Stopwatch();
            }

            if (advmon == null)
            {
                advmon = new AdvancedMotion();
            }

            if (io == null)
            {
                io = new Io();
            }
        }

        public void KillWMX3Handle()
        {
            //清除未知記憶體
            if (wmx != null)
            {
                wmx.Dispose();  // 釋放資源
                wmx = null;     // 設為 null 以避免錯誤引用
            }

            if (motion != null)
            {
                motion.Dispose();  // 釋放資源
                motion = null;     // 設為 null 以避免錯誤引用
            }

            if (CmStatus != null)
            {
                //CmStatus.Dispose();  // 釋放資源
                CmStatus = null;     // 設為 null 以避免錯誤引用
            }

            if (EnStatus != null)
            {
                //EnStatus.Dispose();  // 釋放資源
                EnStatus = null;     // 設為 null 以避免錯誤引用
            }

            if (AxisHomeParam != null)
            {
                //AxisHomeParam.Dispose();  // 釋放資源
                AxisHomeParam = null;     // 設為 null 以避免錯誤引用
            }

            if (stopWatch != null)
            {
                //stopWatch.Dispose();  // 釋放資源
                stopWatch = null;     // 設為 null 以避免錯誤引用
            }

            if (advmon != null)
            {
                advmon.Dispose();  // 釋放資源
                advmon = null;     // 設為 null 以避免錯誤引用
            }

            if (io != null)
            {
                io.Dispose();  // 釋放資源
                io = null;     // 設為 null 以避免錯誤引用
            }
        }

        public void WMX3_Initial()
        {
            //建立WMX3 Handle
            CreateWMX3Handle();

            //建立裝置
            wmx.CreateDevice("C:\\Program Files\\SoftServo\\WMX3", DeviceType.DeviceTypeNormal, 10000);

            //設定裝置名稱
            wmx.SetDeviceName("DLF");

            //設置齒輪比
            if (wmx != null)
            {
                motion.Config.SetGearRatio((int)WMX3軸定義.吸嘴X軸, 1000, 100);  //小線碼
                motion.Config.SetGearRatio((int)WMX3軸定義.吸嘴Y軸, (int)WMX3軸定義.YASKAWA, 2000); 
              //motion.Config.SetGearRatio((int)WMX3軸定義.吸嘴Z軸, 1000, 100);  //VCM伸縮
              //motion.Config.SetGearRatio((int)WMX3軸定義.吸嘴R軸, 1000, 100);  //VCM旋轉

                motion.Config.SetGearRatio((int)WMX3軸定義.載盤X軸, (int)WMX3軸定義.YASKAWA, 2000);
              //motion.Config.SetGearRatio((int)WMX3軸定義.載盤Y軸, 1000, 100);    //大線碼

                motion.Config.SetGearRatio((int)WMX3軸定義.植針Z軸, (int)WMX3軸定義.YASKAWA, 2000);
                motion.Config.SetGearRatio((int)WMX3軸定義.植針R軸, (int)WMX3軸定義.YASKAWA, 2000);

                motion.Config.SetGearRatio((int)WMX3軸定義.工作門, (int)WMX3軸定義.DELTA_ASDA_B3, 20);
            }
            else
            {
                return;
            }

        }  //end of public void WMX3_Initial()

        public int WMX3_establish_Commu()
        {
            int rslt = 0;

            //Active WMX3
            WMX3_Initial();

            if (wmx != null)
            {
                int ret = wmx.StartCommunication();
                if (ret != 0)
                {
                    string str = WMX3Api.ErrorToString(ret);
                    MessageBox.Show(str);
                }

                rslt = 1;
            }
            else
            {
                rslt = 0;
            }

            return rslt;
        }  //end of public void WMX3_establish_Commu()

        public void WMX3_destroy_Commu()
        {

            if (wmx != null)
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
                wmx = null;
            }
            else
            {
                return;
            }

        }  //end of public void WMX3_destroy_Commu()

        public int WMX3_check_Commu()
        {
            int rslt = 0;

            if (wmx != null)
            {
                //讀取當前通訊狀態
                motion.GetStatus(ref CmStatus);

                switch (CmStatus.EngineState)
                {
                    default:
                    case EngineState.Running:
                        rslt = 0;
                        break;

                    case EngineState.Communicating:
                        rslt = 1;
                        break;
                }
            }
            else
            {
                rslt = 0;
            }

            return rslt;
        }  //end of public int WMX3_check_Commu()

        public void WMX3_ServoOnOff(int axis, bool bOn)
        {
            int newStatus = bOn?1:0;

            if (wmx != null)
            {
                 //啟動伺服
                int ret = motion.AxisControl.SetServoOn(axis, newStatus);

                if (ret != 0)
                {
                    string ers = CoreMotion.ErrorToString(ret);
                    MessageBox.Show($"{ers}");
                }
            }
            else
            {
                return;
            }

        }  //end of public void WMX3_ServoOn(int axis)

        public int WMX3_check_ServoOnOff(int axis, ref string position, ref string speed)
        {
            int rslt = 0;

            if (wmx != null)
            { 
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
                position = Profile.Substring(0, Math.Min(Profile.Length, 12));
                speed = cmAxis.ActualVelocity.ToString();
                //AcTqr0.Text = cmAxis.ActualTorque.ToString();
            } 
            else
            {
                rslt = 0;
            }

            return rslt;
        }  //end of int WMX3_check_ServoOnOff(int axis)

        public int WMX3_Pivot(int axis, int pivot, int speed, int accel, int daccel)
        {
            int rslt = 0;

            if (wmx != null)
            {
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
            }
            else
            { 
                rslt = 0;
            }

            return rslt;
        }  //end of public void WMX3_Pivot(int pivot, int speed, int accel, int daccel)

        public int WMX3_SetHomePosition(int axis)
        {
            int rslt = 0;

            if (wmx != null)
            {
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
            }
            else
            {
                rslt = 0;
            }

            return rslt;
        }  //end of public int WMX3_SetHomePosition(int axis)

        public int WMX3_GetOutIO(ref byte[] pData, int addr, int size)
        {

            if (wmx != null)
            {
                // 如果傳入的 pData 為 null 或大小小於 size，則初始化 pData
                if (pData == null || addr == 0 || size == 0 || pData.Length < size)
                {
                    return 0;  // 錯誤長度
                }

                // 讀取 OutputIO
                for (int cnt = 0; cnt < size; cnt++)
                {
                    byte[] pDataGet = new byte[1];

                    // 從指定地址讀取資料並填充到 pData
                    io.GetOutBytes(addr + cnt, 1, ref pDataGet);
                    pData[cnt] = pDataGet[0];
                }

                return 1;  // 成功返回 1
            }
            else
            {
                return 0;
            }

        }

        public int WMX3_GetInIO(ref byte[] pData, int addr, int size)
        {

            if (wmx != null)
            {
                // 如果傳入的 pData 為 null 或大小小於 size，則初始化 pData
                if (pData == null || addr == 0 || size == 0 || pData.Length < size)
                {
                    return 0;  // 錯誤長度
                }

                // 讀取 InputIO
                for (int cnt = 0; cnt < size; cnt++)
                {
                    byte[] pDataGet = new byte[1];

                    // 從指定地址讀取資料並填充到 pData
                    io.GetInBytes(addr + cnt, 1, ref pDataGet);
                    pData[cnt] = pDataGet[0];
                }

                return 1;  // 成功返回 1
            }
            else
            {
                return 0;
            }

        }

        public int WMX3_SetIO(ref byte[] pData, int addr, int size)
        {

            if (wmx != null)
            {
                // 如果傳入的 pData 為 null 或大小小於 size，則初始化 pData
                if (pData == null || addr == 0 || size == 0 || pData.Length < size)
                {
                    return 0;  // 錯誤長度
                }

                // 讀取 InputIO
                for (int cnt = 0; cnt < size; cnt++)
                {
                    byte[] pDataGet = new byte[1];

                    // 從指定地址寫入資料
                    pDataGet[0] = pData[cnt];
                    io.SetOutBytes(addr + cnt, 1, pDataGet);
                }

                return 1;  // 成功返回 1
            }
            else
            {
                return 0;
            }
        }

        public int WMX3_SetIOBit(int addrByte, int addrBit, byte bData)
        {

            if (wmx != null)
            {
                if (addrByte == 0)
                {
                    return 0;  // 錯誤位置
                }

                io.SetOutBit(addrByte, addrBit, bData);

                return 1;  // 成功返回 1
            }
            else
            {
                return 0;
            }

        }

        public void WMX3_ClearAlarm()
        {
            if (wmx != null)
            {
                motion.AxisControl.ClearAmpAlarm((int)NUD_Motor_NO.Value);
            }
            else
            {
                return;
            }
        }



    }  // end of internal class ServoControl
}
