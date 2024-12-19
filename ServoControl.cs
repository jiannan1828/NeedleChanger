
//---------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//---------------------------------------------------------------------------------------
//WMX3
using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using static WMX3ApiCLR.Config;

//---------------------------------------------------------------------------------------
namespace InjectorInspector
{

    //---------------------------------------------------------------------------------------
    //軸的對應號碼
    public enum WMX3軸定義
    {  // start of public enum WMX3軸定義
        AXIS_START          =   -1,
            吸嘴X軸         =    3,  //小線碼
            吸嘴Y軸         =    7,  //YASKAWA
            吸嘴Z軸         =    1,  //VCM伸縮
            吸嘴R軸         =    0,  //VCM旋轉

            載盤X軸         =    4,  //YASKAWA
            載盤Y軸         =    2,  //大線碼

            植針Z軸         =    5,  //YASKAWA
            植針R軸         =    6,  //YASKAWA

            工作門          =    8,  //工作門

            IAISocket孔檢測 = 1000,  //Socket孔
            JoDell3D掃描    = 1001,
            JoDell吸針嘴    = 1002,
            JoDell植針嘴    = 1003,
        AXIS_END,

        YASKAWA             = 1048576,
        DELTA_ASDA_B2       = 1280000,
        DELTA_ASDA_B3       = 16777216,
    }  // end of public enum WMX3軸定義

    //---------------------------------------------------------------------------------------
    public enum WMX3IO對照
    {  // start of public enum WMX3IO對照
        //out:
        pxeIO_Addr_Out_START      =  4,
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
            pxeIO_下後左門鎖      = 21,  //1 下後左門鎖
            pxeIO_取料吸嘴破真空  = 22,  //2 取料吸嘴破真空
            pxeIO_下後右門鎖      = 23,  //3 下後右門鎖
            pxeIO_植針Z煞車       = 24,  //4 植針Z煞車
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
        pxeIO_Addr_Out_END,

        //In:
        pxeIO_Addr_In_START       = 28,
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
            pxeIO_上罩右側左門    = 63,  //3 上兆右側左門
            pxeIO_上罩後側右門    = 64,  //4 上兆後側右們
            pxeIO_螢幕旁小門      = 65,  //5 螢幕旁小門
            pxeIO_上罩後側左門    = 66,  //6 上兆後側左門
            pxeIO_NA67            = 67,  //7 未接

            pxeIO_Addr35          = 35,  //35
            pxeIO_下支架左側右門  = 70,  //0 下支架左側右門
            pxeIO_下支架後側左門  = 71,  //1 下支架後側左門
            pxeIO_下支架左側左門  = 72,  //2 下支架左側左門
            pxeIO_下支架後側右門  = 73,  //3 下支架後側右門
            pxeIO_下支架右側右門  = 74,  //4 下支架右側右門
            pxeIO_NA75            = 75,  //5 未知
            pxeIO_下支架右側左門  = 76,  //6 下支架右側左門
        pxeIO_Addr_In_END,
    }  // end of public enum WMX3IO對照

    //---------------------------------------------------------------------------------------
    public enum addr_IAI
    {  // start of public enum addr_IAI
        //Socket定位
        //IAI Set IO addr
        pxeaI_SetAddrSTART                      = 2320,
            pxeaI_SetTargetPosition4Bytes       = 2320,
            pxeaI_SetPositionBand4Bytes         = 2360,
            pxeaI_SetSpeed4Bytes                = 2400,
            pxeaI_SetZoneBoundaryPlus4Bytes     = 2440,
            pxeaI_SetZoneBoundaryMinus4Bytes    = 2480,
            pxeaI_SetAcceleration2Bytes         = 2520,
            pxeaI_SetDeceleration2Bytes         = 2540,
            pxeaI_SetPushCurrentLimit2Bytes     = 2560,
            pxeaI_SetLoadCurrentThreshole2Bytes = 2580,
            pxeaI_SetControlSignal1_2Bytes      = 2600,
                pxeaI_SetReserveByte1Bit0       = 2600,
                pxeaI_SetPUSH                   = 2601,
                pxeaI_SetDIR                    = 2602,
                pxeaI_SetIncrease               = 2603,
                pxeaI_Set_ServoGain_GSL0        = 2604,
                pxeaI_Set_ServoGain_GSL1        = 2605,
                pxeaI_SetMOD0                   = 2606,
                pxeaI_SetMOD1                   = 2607,
                pxeaI_SetASO0                   = 2610,
                pxeaI_SetASO1                   = 2611,
                pxeaI_SetSMOD                   = 2612,
                pxeaI_SetNTC0                   = 2613,
                pxeaI_SetNTC1                   = 2614,
                pxeaI_SetReserveByte2Bit5       = 2615,
                pxeaI_SetReserveByte2Bit6       = 2616,
                pxeaI_SetReserveByte2Bit7       = 2617,
            pxeaI_SetControlSignal2_2Bytes      = 2620,
                pxeaI_SetDSTR_Start             = 2620,
                pxeaI_SetHOME                   = 2621,
                pxeaI_SetSTP_Pause              = 2622,
                pxeaI_SetResetAlarm             = 2623,
                pxeaI_SetMotorON                = 2624,
                pxeaI_SetJISL                   = 2625,
                pxeaI_SetJVEL                   = 2626,
                pxeaI_SetJOGminus               = 2627,
                pxeaI_SetJOGplus                = 2630,
                pxeaI_SetReserveByte4Bit1       = 2631,
                pxeaI_SetReserveByte4Bit2       = 2632,
                pxeaI_SetReserveByte4Bit3       = 2633,
                pxeaI_SetReserveByte4Bit4       = 2634,
                pxeaI_SetReserveByte4Bit5       = 2635,
                pxeaI_SetRMOD                   = 2636,
                pxeaI_SetDisableBrake           = 2637,
        pxeaI_SetAddrEND,

        //IAI Get IO addr
        pxeaI_GetAddrSTART                      = 0980,
            pxeaI_GetCurrentPosition4Bytes      = 0980,
            pxeaI_GetCommandCurrent4Bytes       = 1020,
            pxeaI_GetCurrentSpeed4Bytes         = 1060,
            pxeaI_GetAlarmCode2Bytes            = 1100,
            pxeaI_GetStatusSignal1_2Bytes       = 1260,
            pxeaI_GetStatusSignal2_2Bytes       = 1280,
                pxeaI_GetPEND_PositionEnd       = 1280,
                pxeaI_GetHEND_HomeEnd           = 1281,
                pxeaI_GetMOVEState              = 1282,
                pxeaI_GetAlarmState             = 1283,
                pxeaI_GetServoONState           = 1284,
                pxeaI_GetPSFL                   = 1285,
                pxeaI_GetPUSH                   = 1286,
                pxeaI_GetGHMS_SearchHomeState   = 1287,
                pxeaI_GetRMODS                  = 1290,
                pxeaI_GetTRQS_forPCON           = 1291,
                pxeaI_GetLOAD_forPCON           = 1292,
                pxeaI_GetPZONE                  = 1293,
                pxeaI_GetZONE1                  = 1294,
                pxeaI_GetZONE2                  = 1295,
                pxeaI_GetPWR_MotorReadyState    = 1296,
                pxeaI_GetEMGS_EmergencyStop     = 1297,
        pxeaI_GetAddrEND,

        pxeaI_BrakeOff,
        pxeaI_MotorOn,
        pxeaI_SetHome,
        pxeaI_GoToPosition,
        pxeaI_GetPosition,
    }  // end of public enum addr_IAI

    //---------------------------------------------------------------------------------------
    public enum addr_JODELL
    {  // start of public enum addr_JODELL
        pxeaJ_DeviceSTART,
            pxeaJ_Device01_Output =  160,  //3D掃描
            pxeaJ_Device01_Input  =  440,  //3D掃描
            pxeaJ_3D掃描_Output   =  160,
            pxeaJ_3D掃描_Input    =  440, 

            pxeaJ_Device02_Output =  880,  //吸針嘴
            pxeaJ_Device02_Input  =  620,  //吸針嘴
            pxeaJ_吸針嘴_Output   =  880, 
            pxeaJ_吸針嘴_Input    =  620, 

            pxeaJ_Device03_Output = 1600,  //植針嘴
            pxeaJ_Device03_Input  =  800,  //植針嘴
            pxeaJ_植針嘴_Output   = 1600,  
            pxeaJ_植針嘴_Input    =  800,  
        pxeaJ_DeviceEND,

        pxeaJ_SetAddr_START                          = 000,
            pxeaJ_SetAddr_Restart2Bytes              = 000,
            pxeaJ_SetAddr_RecipeSave2Bytes           = 020,
            pxeaJ_SetAddr_Default2Bytes              = 040,
            pxeaJ_SetAddr_CmdSource2Bytes            = 060,
            pxeaJ_SetAddr_ControlMode2Bytes          = 080,
            pxeaJ_SetAddr_EnableCmd2Bytes            = 100,
            pxeaJ_SetAddr_FaultClearCmd              = 120,
            pxeaJ_SetAddr_ActCmd2Bytes               = 140,
            pxeaJ_SetAddr_RelActCmd                  = 160,
            pxeaJ_SetAddr_P0_Position2Bytes          = 180,
            pxeaJ_SetAddr_P0_Speed2Bytes             = 200,
            pxeaJ_SetAddr_P0_Torque2Bytes            = 220,
            pxeaJ_SetAddr_P1_Position2Bytes          = 240,
            pxeaJ_SetAddr_P1_Speed2Bytes             = 260,
            pxeaJ_SetAddr_P1_Torque2Bytes            = 280,
            pxeaJ_SetAddr_P2_Position2Bytes          = 300,
            pxeaJ_SetAddr_P2_Speed2Bytes             = 320,
            pxeaJ_SetAddr_P2_Torque2Bytes            = 340,
            pxeaJ_SetAddr_P3_Position2Bytes          = 460,
            pxeaJ_SetAddr_P3_Speed2Bytes             = 380,
            pxeaJ_SetAddr_P3_Torque2Bytes            = 400,
            pxeaJ_SetAddr_P4_Position2Bytes          = 420,
            pxeaJ_SetAddr_P4_Speed2Bytes             = 440,
            pxeaJ_SetAddr_P4_Torque2Bytes            = 460,
            pxeaJ_SetAddr_REL_Position2Bytes         = 480,
            pxeaJ_SetAddr_OnePointMovePosition2Bytes = 500,
            pxeaJ_SetAddr_SlowPos_Precent2Bytes      = 520,
            pxeaJ_SetAddr_SlowSpd_Percent2Bytes      = 540,
            pxeaJ_SetAddr_SlowDir2Bytes              = 560,
            pxeaJ_SetAddr_OutputForce2Bytes          = 580,
            pxeaJ_SetAddr_BrakeForce2Bytes           = 600,
            pxeaJ_SetAddr_IOInOut2Bytes              = 620,
            pxeaJ_SetAddr_Reserve01_2Bytes           = 640,
            pxeaJ_SetAddr_Reserve02_2Bytes           = 660,
            pxeaJ_SetAddr_Reserve03_2Bytes           = 680,
            pxeaJ_SetAddr_Reserve04_2Bytes           = 700,
        pxeaJ_SetAddr_END,

        pxeaJ_GetAddr_START                          = 000,
            pxeaJ_GetAddr_RunStatus2Bytes            = 000,
            pxeaJ_GetAddr_ActStatus2Bytes            = 020,
            pxeaJ_GetAddr_Position2Bytes             = 040,
            pxeaJ_GetAddr_Speed2Bytes                = 060,
            pxeaJ_GetAddr_Torque2Bytes               = 080,
            pxeaJ_GetAddr_DeviceErrorCode2Bytes      = 100,
            pxeaJ_GetAddr_MotorErrorCode2Bytes       = 120,
            pxeaJ_GetAddr_InputsMonitor2Bytes        = 140,
            pxeaJ_GetAddr_OutputMonitor2Bytes        = 160,
        pxeaJ_GetAddr_END,

        pxeaI_MotorOn,
        pxeaI_SetHome,
        pxeaI_GoToPosition,
        pxeaI_GetPosition,
    }  // end of public enum addr_JODELL

    //---------------------------------------------------------------------------------------
    //擴展定義字串轉換
    public static class ByteArrayExtensions
    {  // start of public static class ByteArrayExtensions

        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
    }  //end of public static class ByteArrayExtensions

    //---------------------------------------------------------------------------------------
    //WMX3控制
    internal class ServoControl
    {  // start of internal class ServoControl

        //---------------------------------------------------------------------------------------
        //WMX3軸定義
        public const double dbRead = 99999.9;
        public const double dbAxisRGearRatio = 36000;

        //---------------------------------------------------------------------------------------
        //WMX3
        private WMX3Api          wmx;
        private CoreMotion       motion;
        private CoreMotionStatus CmStatus;
        private EngineStatus     EnStatus;
        private Config.HomeParam AxisHomeParam;
        private Stopwatch        stopWatch;
        private AdvancedMotion   advmon;
        private Io               io;
        public static CoreMotionAxisStatus[] cmAxis = new CoreMotionAxisStatus[8];
        public System.Windows.Forms.NumericUpDown NUD_Motor_NO;

        //---------------------------------------------------------------------------------------
        public ServoControl()
        {  // 建構子，初始化物件
            CreateWMX3Handle();
        }
        //---------------------------------------------------------------------------------------
        ~ServoControl()
        {  // 解構子，釋放非托管資源
            KillWMX3Handle();
        }
        //---------------------------------------------------------------------------------------

        /// <summary>
        /// ServoMotor WMX3 Control API
        /// </summary>
        /// 

        public void CreateWMX3Handle()
        {
            //清除未知記憶體
            KillWMX3Handle();

            // 僅在初始化時進行一次賦值，避免重複初始化
            if (wmx == null)                      wmx = new WMX3Api();
            if (motion == null)                motion = new CoreMotion();
            if (CmStatus == null)            CmStatus = new CoreMotionStatus();
            if (EnStatus == null)            EnStatus = new EngineStatus();
            if (AxisHomeParam == null)  AxisHomeParam = new Config.HomeParam();
            if (stopWatch == null)          stopWatch = new Stopwatch();
            if (advmon == null)                advmon = new AdvancedMotion();
            if (io == null)                        io = new Io();
        }
        //---------------------------------------------------------------------------------------
        public void KillWMX3Handle()
        {
            //清除未知記憶體
            if (wmx != null)           { wmx.Dispose();              wmx = null; }
            if (motion != null)        { motion.Dispose();        motion = null; }
            if (CmStatus != null)      {                        CmStatus = null; }
            if (EnStatus != null)      {                        EnStatus = null; }
            if (AxisHomeParam != null) {                   AxisHomeParam = null; }
            if (stopWatch != null)     {                       stopWatch = null; }
            if (advmon != null)        { advmon.Dispose();        advmon = null; }
            if (io != null)            { io.Dispose();                io = null; }
        }
        //---------------------------------------------------------------------------------------
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
              //motion.Config.SetGearRatio(           (int)WMX3軸定義.吸嘴X軸, 1000, 100);  //小線碼                        //ok    -27796    22204    = 50000
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.吸嘴X軸, true);

                motion.Config.SetGearRatio(           (int)WMX3軸定義.吸嘴Y軸, (int)WMX3軸定義.YASKAWA, 2000);              //ok      -636     9364    = 10000
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.吸嘴Y軸, true);

              //motion.Config.SetGearRatio(           (int)WMX3軸定義.吸嘴Z軸, 1000, 100);  //VCM伸縮                       //ok    = 40
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.吸嘴Z軸, false);

              //motion.Config.SetGearRatio(           (int)WMX3軸定義.吸嘴R軸, 1000, 100);  //VCM旋轉                       //ok
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.吸嘴R軸, false);

                motion.Config.SetGearRatio(           (int)WMX3軸定義.載盤X軸, (int)WMX3軸定義.YASKAWA, 1000);              //ok     14863    -3137    = 18000
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.載盤X軸, true);

              //motion.Config.SetGearRatio(           (int)WMX3軸定義.載盤Y軸, 1000, 100);    //大線碼                      //ok   -149705   650295    = 800000
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.載盤Y軸, true);

                motion.Config.SetGearRatio(           (int)WMX3軸定義.植針Z軸, (int)WMX3軸定義.YASKAWA, 1000);              //ok     -5500    -2500    = 3000
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.植針Z軸, true);

                motion.Config.SetGearRatio(           (int)WMX3軸定義.植針R軸, (int)WMX3軸定義.YASKAWA, dbAxisRGearRatio);  //ok
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.植針R軸, true);

                motion.Config.SetGearRatio(           (int)WMX3軸定義.工作門, (int)WMX3軸定義.DELTA_ASDA_B3, 2000);         //ok      -344    57763    = 58107
                motion.Config.SetAbsoluteEncoderMode( (int)WMX3軸定義.工作門, true);

                SystemParam spErr = new SystemParam();
                AxisParam apErr = new AxisParam();
                motion.Config.ImportAndSetAll("D:\\CodeNeedleChanger\\NeedleChanger\\bin\\Debug", ref spErr, ref apErr);
            }
            else
            {
                return;
            }

        }  //end of public void WMX3_Initial()
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
        public int WMX3_IAI(addr_IAI aIJob, double dbInData)
        {  // start of public void WMX3_IAI(ref addr_IAI aIJob, ref double dbInData)
            int rslt = 0;

            /*
            Byte 0 + 30 Bit4 SON(馬達ON):1    { 看Bit4: SV馬達ON訊號}
            Byte 0 + 30 Bit1 HOME
            Byte 0 1 / 2     目標位置
            Byte 0 4 / 6     定位訊號寬度
            Byte 0 8 / 10    運轉速度
            Byte 0 + 20      運轉加速
            Byte 0 + 22      運轉減速
            Byte 0 + 30 Bit0 DSTR(啟動) { 開始跑時，Bit0 PEND定位訊號為0；完成時，此位為1}
            如果 Bit 0 + 30 Bit3 ALM(故障訊號為1)  就要清除Bit 3 RES(故障復歸填1)
            */

            //故障復歸
            //讀取 IAI 資訊
            byte[] aGetIAIalarm = new byte[2];
            int rstAlarm = 0;
            WMX3_GetInIO(ref aGetIAIalarm, (int)(addr_IAI.pxeaI_GetStatusSignal2_2Bytes) / 10, 2);
            rstAlarm = ((aGetIAIalarm[(int)(addr_IAI.pxeaI_GetAlarmState - addr_IAI.pxeaI_GetStatusSignal2_2Bytes) / 10] & (1 << (int)(addr_IAI.pxeaI_GetAlarmState) % 10)) != 0) ? 1 : 0;
            if(rstAlarm == 1) {
                Thread.Sleep(1);
                WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetResetAlarm) / 10, (int)(addr_IAI.pxeaI_SetResetAlarm) % 10, (byte)1);

                Thread.Sleep(1);
                WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetResetAlarm) / 10, (int)(addr_IAI.pxeaI_SetResetAlarm) % 10, (byte)0);
            }

            //定位訊號寬度
            int iIAIPositionBand = 1;
            byte[] aIAIPositionBand = new byte[4];
            aIAIPositionBand = BitConverter.GetBytes(iIAIPositionBand);
            WMX3_SetIO(ref aIAIPositionBand, (int)(addr_IAI.pxeaI_SetPositionBand4Bytes) / 10, 4);

            //運轉速度
            int iIAISpeed = 3000;
            iIAISpeed = (iIAISpeed >= 4000) ? 4000 : iIAISpeed;
            byte[] aIAISpeed = new byte[4];
            aIAISpeed = BitConverter.GetBytes(iIAISpeed);
            WMX3_SetIO(ref aIAISpeed, (int)(addr_IAI.pxeaI_SetSpeed4Bytes) / 10, 4);

            //運轉加減速
            int iIAIAcceleration = 20;
            iIAIAcceleration = (iIAIAcceleration >= 20) ? 20 : iIAIAcceleration;
            byte[] aIAIAcceleration = new byte[2];
            aIAIAcceleration = BitConverter.GetBytes(iIAIAcceleration);
            WMX3_SetIO(ref aIAIAcceleration, (int)(addr_IAI.pxeaI_SetAcceleration2Bytes) / 10, 2);  //運轉加速
            WMX3_SetIO(ref aIAIAcceleration, (int)(addr_IAI.pxeaI_SetDeceleration2Bytes) / 10, 2);  //運轉減速

            switch (aIJob)
            {
                case addr_IAI.pxeaI_BrakeOff:
                    WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetDisableBrake) / 10, (int)(addr_IAI.pxeaI_SetDisableBrake) % 10, (dbInData>0)? (byte)1: (byte)0 );
                    break;

                case addr_IAI.pxeaI_MotorOn:
                    WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetMotorON) / 10, (int)(addr_IAI.pxeaI_SetMotorON) % 10, (dbInData > 0) ? (byte)1 : (byte)0);
                    break;

                case addr_IAI.pxeaI_SetHome:  lbl_Home:
                    Thread.Sleep(1);
                    WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetHOME) / 10, (int)(addr_IAI.pxeaI_SetHOME) % 10, (byte)1);

                    Thread.Sleep(1);
                    WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetHOME) / 10, (int)(addr_IAI.pxeaI_SetHOME) % 10, (byte)0);
                    break;

                case addr_IAI.pxeaI_GoToPosition:
                    //目標位置
                    int iIAITargetPosition = (int)(dbInData * 100);
                    iIAITargetPosition = (iIAITargetPosition >= 3010) ? 3010 : (iIAITargetPosition <= -10) ? -10 : iIAITargetPosition;
                    byte[] aIAITargetPosition = new byte[4];
                    aIAITargetPosition = BitConverter.GetBytes(iIAITargetPosition);
                    WMX3_SetIO(ref aIAITargetPosition, (int)(addr_IAI.pxeaI_SetTargetPosition4Bytes) / 10, 4);

                    //如果設定位置為0
                    if (dbInData <= 0) {
                        goto lbl_Home;
                    } else { 
                        Thread.Sleep(1);
                        WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetDSTR_Start) / 10, (int)(addr_IAI.pxeaI_SetDSTR_Start) % 10, (byte)1);

                        Thread.Sleep(1);
                        WMX3_SetIOBit((int)(addr_IAI.pxeaI_SetDSTR_Start) / 10, (int)(addr_IAI.pxeaI_SetDSTR_Start) % 10, (byte)0);
                    }
                    break;

                case addr_IAI.pxeaI_GetPosition:
                    byte[] IAIpos = new byte[4];
                    WMX3_GetInIO(ref IAIpos, ((int)(addr_IAI.pxeaI_GetCurrentPosition4Bytes) / 10), 4);
                    rslt = BitConverter.ToInt32(IAIpos, 0);
                    break;

                case addr_IAI.pxeaI_GetCurrentSpeed4Bytes:
                    byte[] IAIspd = new byte[4];
                    WMX3_GetInIO(ref IAIspd, ((int)(addr_IAI.pxeaI_GetCurrentSpeed4Bytes) / 10), 4);
                    rslt = BitConverter.ToInt32(IAIspd, 0);
                    break;

                default:
                    break;
            }

            return rslt;
        }  // end of public void WMX3_IAI(ref addr_IAI aIJob, ref double dbInData)
        //---------------------------------------------------------------------------------------
        public int WMX3_JoDell3D掃描(addr_JODELL aIJob, double dbInData)
        {  // start of public int WMX3_JoDell3D掃描(addr_JODELL aIJob, double dbInData)
            int rslt = 0;

            //故障復歸
            //讀取 JoDell3D掃描 資訊
            //byte[] aGetIAIalarm = new byte[2];
            //int rstAlarm = 0;
            //WMX3_GetInIO(ref aGetIAIalarm, (int)(addr_JODELL.pxeaI_GetStatusSignal2_2Bytes) / 10, 2);
            //rstAlarm = ((aGetIAIalarm[(int)(addr_JODELL.pxeaI_GetAlarmState - addr_JODELL.pxeaI_GetStatusSignal2_2Bytes) / 10] & (1 << (int)(addr_JODELL.pxeaI_GetAlarmState) % 10)) != 0) ? 1 : 0;
            //if(rstAlarm == 1) {
            //    Thread.Sleep(1);
            //    WMX3_SetIOBit((int)(addr_JODELL.pxeaI_SetResetAlarm) / 10, (int)(addr_JODELL.pxeaI_SetResetAlarm) % 10, (byte)1);
            //
            //    Thread.Sleep(1);
            //    WMX3_SetIOBit((int)(addr_JODELL.pxeaI_SetResetAlarm) / 10, (int)(addr_JODELL.pxeaI_SetResetAlarm) % 10, (byte)0);
            //}

            //運轉速度
            {
                int iJoDell3D掃描Speed     = 3000;
                iJoDell3D掃描Speed         = (iJoDell3D掃描Speed >= 4000) ? 4000 : iJoDell3D掃描Speed;

                byte[] aJoDell3D掃描Speed  = new byte[2];
                aJoDell3D掃描Speed         = BitConverter.GetBytes(iJoDell3D掃描Speed);

                int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Speed2Bytes) / 10;
                WMX3_SetIO(ref aJoDell3D掃描Speed, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }

            //運轉扭力
            {
                int iJoDell3D掃描Torque    = 25;
                iJoDell3D掃描Torque        = (iJoDell3D掃描Torque >= 25) ? 25 : iJoDell3D掃描Torque;

                byte[] aJoDell3D掃描Torque = new byte[2];
                aJoDell3D掃描Torque        = BitConverter.GetBytes(iJoDell3D掃描Torque);

                int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Torque2Bytes) / 10;
                WMX3_SetIO(ref aJoDell3D掃描Torque, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }

            switch (aIJob)
            {
                case addr_JODELL.pxeaI_MotorOn: {
                    int iJoDell3D掃描Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                    byte[] aJoDell3D掃描Enable = new byte[2];
                    aJoDell3D掃描Enable        = BitConverter.GetBytes(iJoDell3D掃描Enable);

                    int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                    int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                    WMX3_SetIO(ref aJoDell3D掃描Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                } break;

                case addr_JODELL.pxeaI_SetHome:  lbl_Home:
                    Thread.Sleep(1);
                    dbInData = 0;
                    {
                        int iJoDell3D掃描Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                        byte[] aJoDell3D掃描Enable = new byte[2];
                        aJoDell3D掃描Enable = BitConverter.GetBytes(iJoDell3D掃描Enable);

                        int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                        int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell3D掃描Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }

                    Thread.Sleep(1);
                    dbInData = 1;
                    {
                        int iJoDell3D掃描Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                        byte[] aJoDell3D掃描Enable = new byte[2];
                        aJoDell3D掃描Enable        = BitConverter.GetBytes(iJoDell3D掃描Enable);

                        int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                        int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell3D掃描Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }
                    break;

                case addr_JODELL.pxeaI_GoToPosition: {
                    //執行移動JoDell3D掃描
                    {
                        int iJoDell3D掃描TargetActCmd      = 0;

                        byte[] aJoDell3D掃描TargetActCmd   = new byte[2];
                               aJoDell3D掃描TargetActCmd   = BitConverter.GetBytes(iJoDell3D掃描TargetActCmd);

                        int addr_TargetSetDevice           = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                        int addr_TargetSetFunction         = (int)(addr_JODELL.pxeaJ_SetAddr_ActCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell3D掃描TargetActCmd, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }

                    //目標位置
                    {
                        int iJoDell3D掃描TargetPosition0    = (int)(dbInData * 100);
                            iJoDell3D掃描TargetPosition0    = (iJoDell3D掃描TargetPosition0 >= 3000) ? 3000 : (iJoDell3D掃描TargetPosition0 <= 0) ? 0 : iJoDell3D掃描TargetPosition0;

                        byte[] aJoDell3D掃描TargetPosition0 = new byte[2];
                               aJoDell3D掃描TargetPosition0 = BitConverter.GetBytes(iJoDell3D掃描TargetPosition0);

                        int addr_TargetSetDevice            = (int)(addr_JODELL.pxeaJ_3D掃描_Output) / 10;
                        int addr_TargetSetFunction          = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Position2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell3D掃描TargetPosition0, addr_TargetSetDevice + addr_TargetSetFunction, 2);

                        //如果設定位置為0
                        if (dbInData >= 30.0) {
                            goto lbl_Home;
                        }
                    }
                } break;

                case addr_JODELL.pxeaI_GetPosition: {
                    byte[] JODELL_RX           = new byte[18];

                    int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Input) / 10;
                    int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                    WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                    int[] varJODELL_RX         = new int[JODELL_RX.Length / 2];
                    for (int i = 0; i < varJODELL_RX.Length; i++) {
                        varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                    }

                    rslt = varJODELL_RX[(int)(addr_JODELL.pxeaJ_GetAddr_Position2Bytes) / 10 / 2];
                } break;

                case addr_JODELL.pxeaJ_GetAddr_Speed2Bytes: {
                    byte[] JODELL_RX           = new byte[18];

                    int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Input) / 10;
                    int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                    WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                    int[] varJODELL_RX         = new int[JODELL_RX.Length / 2];
                    for (int i = 0; i < varJODELL_RX.Length; i++) {
                        varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                    }

                    rslt = varJODELL_RX[(int)(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes) / 10 / 2];
                } break;

                default:
                    break;
            }

            return rslt;
        }  // end of public int WMX3_JoDell3D掃描(addr_JODELL aIJob, double dbInData)
        //---------------------------------------------------------------------------------------
        public int WMX3_JoDell吸針嘴(addr_JODELL aIJob, double dbInData)
        {  // start of public int WMX3_JoDell吸針嘴(addr_JODELL aIJob, double dbInData)
            int rslt = 0;

            //故障復歸
            //讀取 JoDell吸針嘴 資訊
            //byte[] aGetIAIalarm = new byte[2];
            //int rstAlarm = 0;
            //WMX3_GetInIO(ref aGetIAIalarm, (int)(addr_JODELL.pxeaI_GetStatusSignal2_2Bytes) / 10, 2);
            //rstAlarm = ((aGetIAIalarm[(int)(addr_JODELL.pxeaI_GetAlarmState - addr_JODELL.pxeaI_GetStatusSignal2_2Bytes) / 10] & (1 << (int)(addr_JODELL.pxeaI_GetAlarmState) % 10)) != 0) ? 1 : 0;
            //if(rstAlarm == 1) {
            //    Thread.Sleep(1);
            //    WMX3_SetIOBit((int)(addr_JODELL.pxeaI_SetResetAlarm) / 10, (int)(addr_JODELL.pxeaI_SetResetAlarm) % 10, (byte)1);
            //
            //    Thread.Sleep(1);
            //    WMX3_SetIOBit((int)(addr_JODELL.pxeaI_SetResetAlarm) / 10, (int)(addr_JODELL.pxeaI_SetResetAlarm) % 10, (byte)0);
            //}

            //運轉速度
            {
                int iJoDell吸針嘴Speed     = 3000;
                iJoDell吸針嘴Speed         = (iJoDell吸針嘴Speed >= 4000) ? 4000 : iJoDell吸針嘴Speed;

                byte[] aJoDell吸針嘴Speed  = new byte[2];
                aJoDell吸針嘴Speed         = BitConverter.GetBytes(iJoDell吸針嘴Speed);

                int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Speed2Bytes) / 10;
                WMX3_SetIO(ref aJoDell吸針嘴Speed, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }

            //運轉扭力
            {
                int iJoDell吸針嘴Torque    = 25;
                iJoDell吸針嘴Torque        = (iJoDell吸針嘴Torque >= 25) ? 25 : iJoDell吸針嘴Torque;

                byte[] aJoDell吸針嘴Torque = new byte[2];
                aJoDell吸針嘴Torque        = BitConverter.GetBytes(iJoDell吸針嘴Torque);

                int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Torque2Bytes) / 10;
                WMX3_SetIO(ref aJoDell吸針嘴Torque, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }

            switch (aIJob)
            {
                case addr_JODELL.pxeaI_MotorOn: {
                    int iJoDell吸針嘴Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                    byte[] aJoDell吸針嘴Enable = new byte[2];
                    aJoDell吸針嘴Enable        = BitConverter.GetBytes(iJoDell吸針嘴Enable);

                    int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                    int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                    WMX3_SetIO(ref aJoDell吸針嘴Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                } break;

                case addr_JODELL.pxeaI_SetHome:  lbl_Home:
                    Thread.Sleep(1);
                    dbInData = 0;
                    {
                        int iJoDell吸針嘴Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                        byte[] aJoDell吸針嘴Enable = new byte[2];
                        aJoDell吸針嘴Enable        = BitConverter.GetBytes(iJoDell吸針嘴Enable);

                        int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                        int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell吸針嘴Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }

                    Thread.Sleep(1);
                    dbInData = 1;
                    {
                        int iJoDell吸針嘴Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                        byte[] aJoDell吸針嘴Enable = new byte[2];
                        aJoDell吸針嘴Enable = BitConverter.GetBytes(iJoDell吸針嘴Enable);

                        int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                        int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell吸針嘴Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }
                    break;

                case addr_JODELL.pxeaI_GoToPosition: {
                    //執行移動JoDell吸針嘴
                    {
                        int iJoDell吸針嘴TargetActCmd      = 0;

                        byte[] aJoDell吸針嘴TargetActCmd   = new byte[2];
                               aJoDell吸針嘴TargetActCmd   = BitConverter.GetBytes(iJoDell吸針嘴TargetActCmd);

                        int addr_TargetSetDevice           = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                        int addr_TargetSetFunction         = (int)(addr_JODELL.pxeaJ_SetAddr_ActCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell吸針嘴TargetActCmd, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }

                    //目標位置
                    {
                        int iJoDell吸針嘴TargetPosition0    = (int)(dbInData * 100);
                            iJoDell吸針嘴TargetPosition0    = (iJoDell吸針嘴TargetPosition0 >= 3000) ? 3000 : (iJoDell吸針嘴TargetPosition0 <= 0) ? 0 : iJoDell吸針嘴TargetPosition0;

                        byte[] aJoDell吸針嘴TargetPosition0 = new byte[2];
                               aJoDell吸針嘴TargetPosition0 = BitConverter.GetBytes(iJoDell吸針嘴TargetPosition0);

                        int addr_TargetSetDevice            = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
                        int addr_TargetSetFunction          = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Position2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell吸針嘴TargetPosition0, addr_TargetSetDevice + addr_TargetSetFunction, 2);

                        //如果設定位置為0
                        if (dbInData >= 30.0) {
                            goto lbl_Home;
                        }
                    }
                } break;

                case addr_JODELL.pxeaI_GetPosition: {
                    byte[] JODELL_RX           = new byte[18];

                    int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Input) / 10;
                    int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                    WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                    int[] varJODELL_RX         = new int[JODELL_RX.Length / 2];
                    for (int i = 0; i < varJODELL_RX.Length; i++) {
                        varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                    }

                    rslt = varJODELL_RX[(int)(addr_JODELL.pxeaJ_GetAddr_Position2Bytes) / 10 / 2];
                } break;

                case addr_JODELL.pxeaJ_GetAddr_Speed2Bytes: {
                    byte[] JODELL_RX           = new byte[18];

                    int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Input) / 10;
                    int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                    WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                    int[] varJODELL_RX         = new int[JODELL_RX.Length / 2];
                    for (int i = 0; i < varJODELL_RX.Length; i++) {
                        varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                    }

                    rslt = varJODELL_RX[(int)(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes) / 10 / 2];
                } break;

                default:
                    break;
            }

            return rslt;
        }  // end of public int WMX3_JoDell吸針嘴(addr_JODELL aIJob, double dbInData)
        //---------------------------------------------------------------------------------------
        public int WMX3_JoDell植針嘴(addr_JODELL aIJob, double dbInData)
        {  // start of public int WMX3_JoDell植針嘴(addr_JODELL aIJob, double dbInData)
            int rslt = 0;

            //故障復歸
            //讀取 JoDell植針嘴 資訊
            //byte[] aGetIAIalarm = new byte[2];
            //int rstAlarm = 0;
            //WMX3_GetInIO(ref aGetIAIalarm, (int)(addr_JODELL.pxeaI_GetStatusSignal2_2Bytes) / 10, 2);
            //rstAlarm = ((aGetIAIalarm[(int)(addr_JODELL.pxeaI_GetAlarmState - addr_JODELL.pxeaI_GetStatusSignal2_2Bytes) / 10] & (1 << (int)(addr_JODELL.pxeaI_GetAlarmState) % 10)) != 0) ? 1 : 0;
            //if(rstAlarm == 1) {
            //    Thread.Sleep(1);
            //    WMX3_SetIOBit((int)(addr_JODELL.pxeaI_SetResetAlarm) / 10, (int)(addr_JODELL.pxeaI_SetResetAlarm) % 10, (byte)1);
            //
            //    Thread.Sleep(1);
            //    WMX3_SetIOBit((int)(addr_JODELL.pxeaI_SetResetAlarm) / 10, (int)(addr_JODELL.pxeaI_SetResetAlarm) % 10, (byte)0);
            //}

            //運轉速度
            {
                int iJoDell植針嘴Speed     = 3000;
                iJoDell植針嘴Speed         = (iJoDell植針嘴Speed >= 4000) ? 4000 : iJoDell植針嘴Speed;

                byte[] aJoDell植針嘴Speed  = new byte[2];
                aJoDell植針嘴Speed         = BitConverter.GetBytes(iJoDell植針嘴Speed);

                int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Speed2Bytes) / 10;
                WMX3_SetIO(ref aJoDell植針嘴Speed, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }

            //運轉扭力
            {
                int iJoDell植針嘴Torque    = 25;
                iJoDell植針嘴Torque        = (iJoDell植針嘴Torque >= 25) ? 25 : iJoDell植針嘴Torque;

                byte[] aJoDell植針嘴Torque = new byte[2];
                aJoDell植針嘴Torque        = BitConverter.GetBytes(iJoDell植針嘴Torque);

                int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Torque2Bytes) / 10;
                WMX3_SetIO(ref aJoDell植針嘴Torque, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }

            switch (aIJob)
            {
                case addr_JODELL.pxeaI_MotorOn: {
                    int iJoDell植針嘴Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                    byte[] aJoDell植針嘴Enable = new byte[2];
                    aJoDell植針嘴Enable        = BitConverter.GetBytes(iJoDell植針嘴Enable);

                    int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                    int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                    WMX3_SetIO(ref aJoDell植針嘴Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                } break;

                case addr_JODELL.pxeaI_SetHome:  lbl_Home:
                    Thread.Sleep(1);
                    dbInData = 0;
                    {
                        int iJoDell植針嘴Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                        byte[] aJoDell植針嘴Enable = new byte[2];
                        aJoDell植針嘴Enable        = BitConverter.GetBytes(iJoDell植針嘴Enable);

                        int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                        int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell植針嘴Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }

                    Thread.Sleep(1);
                    dbInData = 1;
                    {
                        int iJoDell植針嘴Enable    = (dbInData > 0) ? (byte)1 : (byte)0;

                        byte[] aJoDell植針嘴Enable = new byte[2];
                        aJoDell植針嘴Enable = BitConverter.GetBytes(iJoDell植針嘴Enable);

                        int addr_TargetSetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                        int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_EnableCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell植針嘴Enable, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }
                    break;

                case addr_JODELL.pxeaI_GoToPosition: {
                    //執行移動JoDell植針嘴
                    {
                        int iJoDell植針嘴TargetActCmd      = 0;

                        byte[] aJoDell植針嘴TargetActCmd   = new byte[2];
                               aJoDell植針嘴TargetActCmd   = BitConverter.GetBytes(iJoDell植針嘴TargetActCmd);

                        int addr_TargetSetDevice           = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                        int addr_TargetSetFunction         = (int)(addr_JODELL.pxeaJ_SetAddr_ActCmd2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell植針嘴TargetActCmd, addr_TargetSetDevice + addr_TargetSetFunction, 2);
                    }

                    //目標位置
                    {
                        int iJoDell植針嘴TargetPosition0    = (int)(dbInData * 100);
                            iJoDell植針嘴TargetPosition0    = (iJoDell植針嘴TargetPosition0 >= 5000) ? 5000 : (iJoDell植針嘴TargetPosition0 <= 0) ? 0 : iJoDell植針嘴TargetPosition0;

                        byte[] aJoDell植針嘴TargetPosition0 = new byte[2];
                               aJoDell植針嘴TargetPosition0 = BitConverter.GetBytes(iJoDell植針嘴TargetPosition0);

                        int addr_TargetSetDevice            = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                        int addr_TargetSetFunction          = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Position2Bytes) / 10;
                        WMX3_SetIO(ref aJoDell植針嘴TargetPosition0, addr_TargetSetDevice + addr_TargetSetFunction, 2);

                        //如果設定位置為0
                        if (dbInData >= 50.0) {
                            goto lbl_Home;
                        }
                    }
                } break;

                case addr_JODELL.pxeaI_GetPosition: {
                    byte[] JODELL_RX           = new byte[18];

                    int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Input) / 10;
                    int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                    WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                    int[] varJODELL_RX         = new int[JODELL_RX.Length / 2];
                    for (int i = 0; i < varJODELL_RX.Length; i++) {
                        varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                    }

                    rslt = varJODELL_RX[(int)(addr_JODELL.pxeaJ_GetAddr_Position2Bytes) / 10 / 2];
                } break;

                case addr_JODELL.pxeaJ_GetAddr_Speed2Bytes: {
                    byte[] JODELL_RX           = new byte[18];

                    int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Input) / 10;
                    int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                    WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                    int[] varJODELL_RX         = new int[JODELL_RX.Length / 2];
                    for (int i = 0; i < varJODELL_RX.Length; i++) {
                        varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                    }

                    rslt = varJODELL_RX[(int)(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes) / 10 / 2];
                } break;

                default:
                    break;
            }

            return rslt;
        }  // end of public int WMX3_JoDell植針嘴(addr_JODELL aIJob, double dbInData)
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
    }  // end of internal class ServoControl
    //---------------------------------------------------------------------------------------
}
