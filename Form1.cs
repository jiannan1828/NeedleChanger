
//---------------------------------------------------------------------------------------
//Default using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

//---------------------------------------------------------------------------------------
//SMB added
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection.Emit;
using System.Diagnostics;

//---------------------------------------------------------------------------------------
//Halcon
using Inspector;
using HalconDotNet;

//---------------------------------------------------------------------------------------
//Vibration
using static InjectorInspector.Vibration;

//---------------------------------------------------------------------------------------
//ServoControl
using static InjectorInspector.ServoControl;

//---------------------------------------------------------------------------------------
//JSON
using System.IO;
using WMX3ApiCLR;
using static System.Windows.Forms.AxHost;
using System.Xml.Linq;
using static InjectorInspector.Form1;
//using System.Text.Json;

//---------------------------------------------------------------------------------------
namespace InjectorInspector
{
    //---------------------------------------------------------------------------------------
    public partial class Form1 : Form
    {
        //---------------------------------------------------------------------------------------
        //Debug config
        bool bshow_debug_RAW_Conver_Back_Value = false;
        
        //---------------------------------------------------------------------------------------
        //WMX3
        ServoControl clsServoControlWMX3 = new ServoControl();

        //---------------------------------------------------------------------------------------
        //Vibration
        Vibration clsVibration = new Vibration();

        //---------------------------------------------------------------------------------------
        //Debug for implementation
        public int ErrorCode = 0;
        public int cntcallback = 0;

        //---------------------------------------------------------------------------------------
        //------------------------------ Test function with Vision ------------------------------
        //---------------------------------------------------------------------------------------
        public void apiCallBackTest()
        {
            cntcallback++;
            this.Text = cntcallback.ToString() + "  " + inspector1.InspNozzle.CCD.GrabCount.ToString();


            if (inspector1.InspectOK == true && inspector1.Inspected == true) {
                label10.Text = inspector1.PinDeg.ToString();
                inspector1.InspectOK = false;
            }

        }
        //---------------------------------------------------------------------------------------
        bool   b黑色料倉有料_tmrTakePinTick = false;
        bool   b柔震盤有料_tmrTakePinTick   = false;
        double dbPinX_tmrTakePinTick        = 0.0,
               dbPinY_tmrTakePinTick        = 0.0,
               dbPinR_tmrTakePinTick        = 0.0;
        private void btn_取得PinInfo_Click(object sender, EventArgs e)
        {
            //吸料盤校正用
            PointF pos;
            bool success = inspector1.xCarb震動盤(out pos);
            label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);

            //黑色料倉
            bool 料倉有料 = inspector1.xInsp入料();
            label3.Text = string.Format("黑色料倉 料倉有料 = {0}", 料倉有料);
            b黑色料倉有料_tmrTakePinTick = 料倉有料;

            //光源震動盤
            List<Vector3> pins;
            bool 料盤有料 = inspector1.xInsp震動盤(out pins);
            Vector3 temp = (料盤有料) ? pins.First() : new Vector3();
            label4.Text = string.Format("光源震動盤 震動盤 = {0} X = {1:F2} Y = {2:F2} θ = {3:F2}", 料盤有料, temp.X, temp.Y, temp.θ);
            b柔震盤有料_tmrTakePinTick = 料盤有料;
            dbPinX_tmrTakePinTick = temp.X;
            dbPinY_tmrTakePinTick = temp.Y;
            dbPinR_tmrTakePinTick = temp.θ;

            if (inspector1.Inspected && inspector1.InspectOK) {
                double deg = inspector1.PinDeg;
                label5.Text = string.Format("吸嘴物料分析  θ = {0:F2}", deg);
            } else
                label5.Text = "吸嘴物料分析失敗";

            int cntdebug = inspector1.RecvCount;
        }
        //---------------------------------------------------------------------------------------
        //------------------------------ Test function with Vision ------------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //------------------------ Xavier Call, Control the Servo machine -----------------------
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleX(double dbIncreaseNozzleX, double dbTargetSpeed)  //NozzleX
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 500000;
                const int    MinRAW =      0;
                const double Maxdb  =  500.0;
                const double Mindb  =    0.0;
                const double Sum    = 500000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleX = 0.0;

            {  // start of 吸嘴X軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 吸嘴X軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴X軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_吸嘴X軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴X軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴X軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_吸嘴X軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_吸嘴X軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_吸嘴X軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstNozzleX             = dbGet;
                    lbl_acpos_吸嘴X軸.Text   = dbRstNozzleX.ToString("F3");

                    //顯示運動速度
                    lbl_spd_吸嘴X軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_吸嘴X軸.BackColor    = Color.Red;
                    lbl_acpos_吸嘴X軸.BackColor = Color.White;
                    lbl_spd_吸嘴X軸.BackColor   = Color.White;
                } else {
                    select_吸嘴X軸.BackColor    = Color.Green;
                    lbl_acpos_吸嘴X軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴X軸.BackColor   = Color.Gray;
                }

            }  // end of 吸嘴X軸 讀取與顯示

            if (dbIncreaseNozzleX == dbRead) {

            } else {  //吸嘴X軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseNozzleX && dbIncreaseNozzleX<=Maxdb ) {

                } else if( dbIncreaseNozzleX<=Mindb ) {
                    dbIncreaseNozzleX = (int)Mindb;
                } else if( Maxdb<=dbIncreaseNozzleX ) {
                    dbIncreaseNozzleX = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeNozzleX = calculate.Map(dbIncreaseNozzleX, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動吸嘴
                int axis     = (int)WMX3軸定義.吸嘴X軸;
                int position = fChangeNozzleX;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleX == dbRead) {

            return dbRstNozzleX;
        }  // end of public double dbapiNozzleX(double dbIncreaseNozzleX)  //NozzleX
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleY(double dbIncreaseNozzleY, double dbTargetSpeed)  //NozzleY
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  10000;
                const int    MinRAW =      0;
                const double Maxdb  =  100.0;
                const double Mindb  =    0.0;
                const double Sum    =  10000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleY = 0.0;

            {  // start of 吸嘴Y軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 吸嘴Y軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_吸嘴Y軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴Y軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴Y軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_吸嘴Y軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_吸嘴Y軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_吸嘴Y軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstNozzleY             = dbGet;
                    lbl_acpos_吸嘴Y軸.Text   = dbRstNozzleY.ToString("F3");

                    //顯示運動速度
                    lbl_spd_吸嘴Y軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_吸嘴Y軸.BackColor    = Color.Red;
                    lbl_acpos_吸嘴Y軸.BackColor = Color.White;
                    lbl_spd_吸嘴Y軸.BackColor   = Color.White;
                } else {
                    select_吸嘴Y軸.BackColor    = Color.Green;
                    lbl_acpos_吸嘴Y軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴Y軸.BackColor   = Color.Gray;
                }

            }  // end of 吸嘴Y軸 讀取與顯示

            if (dbIncreaseNozzleY == dbRead) {

            } else {  //吸嘴X軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseNozzleY && dbIncreaseNozzleY<=Maxdb ) {

                } else if( dbIncreaseNozzleY<=Mindb ) {
                    dbIncreaseNozzleY = (int)Mindb;
                } else if( Maxdb<=dbIncreaseNozzleY ) {
                    dbIncreaseNozzleY = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeNozzleY = calculate.Map(dbIncreaseNozzleY, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動吸嘴
                int axis     = (int)WMX3軸定義.吸嘴Y軸;
                int position = fChangeNozzleY;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleY == dbRead) {

            return dbRstNozzleY;
        }  // end of public double dbapiNozzleY(double dbIncreaseNozzleY)  //NozzleY
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleZ(double dbIncreaseNozzleZ, double dbTargetSpeed)  //NozzleZ
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  41496;
                const int    MinRAW =    -93;
                const double Maxdb  =   40.0;
                const double Mindb  =    0.0;
                const double Sum    =  40000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleZ = 0.0;

            {  // start of 吸嘴Z軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 吸嘴Z軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_吸嘴Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_吸嘴Z軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_吸嘴Z軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_吸嘴Z軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstNozzleZ             = dbGet;
                    lbl_acpos_吸嘴Z軸.Text   = dbRstNozzleZ.ToString("F3");

                    //顯示運動速度
                    lbl_spd_吸嘴Z軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_吸嘴Z軸.BackColor    = Color.Red;
                    lbl_acpos_吸嘴Z軸.BackColor = Color.White;
                    lbl_spd_吸嘴Z軸.BackColor   = Color.White;
                } else {
                    select_吸嘴Z軸.BackColor    = Color.Green;
                    lbl_acpos_吸嘴Z軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴Z軸.BackColor   = Color.Gray;
                }

            }  // end of 吸嘴Z軸 讀取與顯示

            if (dbIncreaseNozzleZ == dbRead) {

            } else {  //吸嘴Z軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseNozzleZ && dbIncreaseNozzleZ<=Maxdb ) {

                } else if( dbIncreaseNozzleZ<=Mindb ) {
                    dbIncreaseNozzleZ = (int)Mindb;
                } else if ( Maxdb<=dbIncreaseNozzleZ ) {
                    dbIncreaseNozzleZ = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeNozzleZ = calculate.Map(dbIncreaseNozzleZ, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行伸縮吸嘴
                int axis     = (int)WMX3軸定義.吸嘴Z軸;
                int position = fChangeNozzleZ;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleZ == dbRead) {

            return dbRstNozzleZ;
        }  // end of public double dbapiNozzleZ(double dbIncreaseNozzleZ)  //NozzleZ
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleR(double dbIncreaseNozzleR, double dbTargetSpeed)  //NozzleR
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  24120;
                const int    MinRAW = -11880;
                const double Maxdb  =  360.0;
                const double Mindb  =    0.0;
                const double Sum    =  36000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleR = 0.0;

            {  // start of 吸嘴R軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 吸嘴R軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴R軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_吸嘴R軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴R軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_吸嘴R軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_吸嘴R軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    //overflow
                    while (dbGet >= 360.0) {
                        dbGet -= 360.0;
                    }
                    lbl_吸嘴R軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_吸嘴R軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstNozzleR             = dbGet;
                    lbl_acpos_吸嘴R軸.Text   = dbRstNozzleR.ToString("F3");

                    //顯示運動速度
                    lbl_spd_吸嘴R軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_吸嘴R軸.BackColor    = Color.Red;
                    lbl_acpos_吸嘴R軸.BackColor = Color.White;
                    lbl_spd_吸嘴R軸.BackColor   = Color.White;
                } else {
                    select_吸嘴R軸.BackColor    = Color.Green;
                    lbl_acpos_吸嘴R軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴R軸.BackColor   = Color.Gray;
                }

            }  // end of 吸嘴R軸 讀取與顯示

            if (dbIncreaseNozzleR == dbRead) {

            } else {  //吸嘴R軸 變更位置
                //伸長量overflow保護

                //if( Mindb<=dbIncreaseNozzleR && dbIncreaseNozzleR<=Maxdb ) {
                //
                //} else if( dbIncreaseNozzleR<=Mindb ) {
                //    dbIncreaseNozzleR = (int)Mindb;
                //} else if( Maxdb<= dbIncreaseNozzleR) {
                //    dbIncreaseNozzleR = (int)Maxdb;
                //}

                // 取得欲變更的的浮點數
                int fChangeNozzleR = calculate.Map(dbIncreaseNozzleR, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行旋轉吸嘴
                int axis     = (int)WMX3軸定義.吸嘴R軸;
                int position = fChangeNozzleR;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleR == dbRead) {

            return dbRstNozzleR;
        }  // end of public double dbapiNozzleR(double dbIncreaseNozzleR)  //NozzleR
        //---------------------------------------------------------------------------------------
        public double dbapiCarrierX(double dbIncreaseCarrierX, double dbTargetSpeed)  //CarrierX
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  190000;
                const int    MinRAW =      0;
                const double Maxdb  =  190.0;
                const double Mindb  =    0.0;
                const double Sum    =  190000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstCarrierX = 0.0;

            {  // start of 載盤X軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 載盤X軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.載盤X軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_載盤X軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_載盤X軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_載盤X軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_載盤X軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_載盤X軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_載盤X軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstCarrierX            = dbGet;
                    lbl_acpos_載盤X軸.Text   = dbRstCarrierX.ToString("F3");

                    //顯示運動速度
                    lbl_spd_載盤X軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_載盤X軸.BackColor    = Color.Red;
                    lbl_acpos_載盤X軸.BackColor = Color.White;
                    lbl_spd_載盤X軸.BackColor   = Color.White;
                } else {
                    select_載盤X軸.BackColor    = Color.Green;
                    lbl_acpos_載盤X軸.BackColor = Color.Gray;
                    lbl_spd_載盤X軸.BackColor   = Color.Gray;
                }

            }  // end of 載盤X軸 讀取與顯示

            if (dbIncreaseCarrierX == dbRead) {

            } else {  //載盤X軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseCarrierX && dbIncreaseCarrierX<=Maxdb ) {

                } else if( dbIncreaseCarrierX<=Mindb ) {
                    dbIncreaseCarrierX = (int)Mindb;
                } else if( Maxdb<=dbIncreaseCarrierX ) {
                    dbIncreaseCarrierX = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeCarrierX = calculate.Map(dbIncreaseCarrierX, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動載盤
                int axis     = (int)WMX3軸定義.載盤X軸;
                int position = fChangeCarrierX;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseCarrierX == dbRead) {

            return dbRstCarrierX;
        }  // end of public double dbapiCarrierX(double dbIncreaseCarrierX)  //CarrierX
        //---------------------------------------------------------------------------------------
        public double dbapiCarrierY(double dbIncreaseCarrierY, double dbTargetSpeed)  //CarrierY
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 800000;
                const int    MinRAW =      0;
                const double Maxdb  =  800.0;
                const double Mindb  =    0.0;
                const double Sum    = 800000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstCarrierY = 0.0;

            {  // start of 載盤Y軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 載盤Y軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.載盤Y軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_載盤Y軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_載盤Y軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_載盤Y軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_載盤Y軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_載盤Y軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_載盤Y軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstCarrierY            = dbGet;
                    lbl_acpos_載盤Y軸.Text   = dbRstCarrierY.ToString("F3");

                    //顯示運動速度
                    lbl_spd_載盤Y軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_載盤Y軸.BackColor    = Color.Red;
                    lbl_acpos_載盤Y軸.BackColor = Color.White;
                    lbl_spd_載盤Y軸.BackColor   = Color.White;
                } else {
                    select_載盤Y軸.BackColor    = Color.Green;
                    lbl_acpos_載盤Y軸.BackColor = Color.Gray;
                    lbl_spd_載盤Y軸.BackColor   = Color.Gray;
                }

            }  // end of 載盤Y軸 讀取與顯示

            if (dbIncreaseCarrierY == dbRead) {

            } else {  //載盤Y軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseCarrierY && dbIncreaseCarrierY<=Maxdb ) {

                } else if( dbIncreaseCarrierY<=Mindb ) {
                    dbIncreaseCarrierY = (int)Mindb;
                } else if( Maxdb<=dbIncreaseCarrierY ) {
                    dbIncreaseCarrierY = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeCarrierY = calculate.Map(dbIncreaseCarrierY, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動載盤
                int axis     = (int)WMX3軸定義.載盤Y軸;
                int position = fChangeCarrierY;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseCarrierY == dbRead) {

            return dbRstCarrierY;
        }  // end of public double dbapiCarrierY(double dbIncreaseCarrierY)  //CarrierY
        //---------------------------------------------------------------------------------------
        public double dbapiSetZ(double dbIncreaseSetZ, double dbTargetSpeed)  //SetZ
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   3300;
                const int    MinRAW =      0;
                const double Maxdb  =     33;
                const double Mindb  =    0.0;
                const double Sum    =   3300;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstSetZ = 0.0;

            {  // start of 植針Z軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 植針Z軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.植針Z軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_植針Z軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_植針Z軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_植針Z軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstSetZ                = dbGet;
                    lbl_acpos_植針Z軸.Text   = dbRstSetZ.ToString("F3");

                    //顯示運動速度
                    lbl_spd_植針Z軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_植針Z軸.BackColor    = Color.Red;
                    lbl_acpos_植針Z軸.BackColor = Color.White;
                    lbl_spd_植針Z軸.BackColor   = Color.White;
                } else {
                    select_植針Z軸.BackColor    = Color.Green;
                    lbl_acpos_植針Z軸.BackColor = Color.Gray;
                    lbl_spd_植針Z軸.BackColor   = Color.Gray;
                }

            }  // end of 植針Z軸 讀取與顯示

            if (dbIncreaseSetZ == dbRead) {

            } else {  //植針Z軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseSetZ && dbIncreaseSetZ<=Maxdb ) {

                } else if( dbIncreaseSetZ<=Mindb ) {
                    dbIncreaseSetZ = (int)Mindb;
                } else if( Maxdb<=dbIncreaseSetZ ) {
                    dbIncreaseSetZ = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeSetZ = calculate.Map(dbIncreaseSetZ, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動植針Z軸
                int axis     = (int)WMX3軸定義.植針Z軸;
                int position = fChangeSetZ;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseSetZ == dbRead) {

            return dbRstSetZ;
        }  // end of public double dbapiSetZ(double dbIncreaseSetZ)  //SetZ
        //---------------------------------------------------------------------------------------
        public double dbapiSetR(double dbIncreaseSetR, double dbTargetSpeed)  //SetR
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 360000;
                const int    MinRAW =      0;
                const double Maxdb  =  360.0;
                const double Mindb  =    0.0;
                const double Sum    = 360000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstSetR = 0.0;

            {  // start of 植針R軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 植針R軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.植針R軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_植針R軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value;
                    lbl_植針R軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_植針R軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_植針R軸_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_植針R軸_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_植針R軸_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstSetR                = dbGet;
                    lbl_acpos_植針R軸.Text   = dbRstSetR.ToString("F3");

                    //顯示運動速度
                    lbl_spd_植針R軸.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_植針R軸.BackColor    = Color.Red;
                    lbl_acpos_植針R軸.BackColor = Color.White;
                    lbl_spd_植針R軸.BackColor   = Color.White;
                } else {
                    select_植針R軸.BackColor    = Color.Green;
                    lbl_acpos_植針R軸.BackColor = Color.Gray;
                    lbl_spd_植針R軸.BackColor   = Color.Gray;
                }

            }  // end of 植針R軸 讀取與顯示

            if (dbIncreaseSetR == dbRead) {

            } else {  //植針R軸 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseSetR && dbIncreaseSetR<=Maxdb ) {

                } else if( dbIncreaseSetR<=Mindb ) {
                    dbIncreaseSetR = (int)Mindb;
                } else if( Maxdb<=dbIncreaseSetR ) {
                    dbIncreaseSetR = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeSetR = calculate.Map(dbIncreaseSetR, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動植針R軸
                int axis     = (int)WMX3軸定義.植針R軸;
                int position = fChangeSetR;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseSetR == dbRead) {

            return dbRstSetR;
        }  // end of public double dbapiSetR(double dbIncreaseSetR)  //SetR
        //---------------------------------------------------------------------------------------
        public double dbapiGate(double dbIncreaseGate, double dbTargetSpeed)  //Gate
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  58000;
                const int    MinRAW =      0;
                const double Maxdb  =  580.0;
                const double Mindb  =    0.0;
                const double Sum    =  58000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstGate = 0.0;

            {  // start of 工作門 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 工作門 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.工作門, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    lbl_工作門_RAW.Visible      = bshow_debug_RAW_Conver_Back_Value;
                    lbl_工作門_Convert.Visible  = bshow_debug_RAW_Conver_Back_Value;
                    lbl_工作門_Back.Visible     = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    lbl_工作門_RAW.Text      = Convert.ToString();

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    lbl_工作門_Convert.Text  = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_工作門_Back.Text     = cnback.ToString();


                    //顯示讀取長度
                    dbRstGate                = dbGet;
                    lbl_acpos_工作門.Text    = dbRstGate.ToString("F3");

                    //顯示運動速度
                    lbl_spd_工作門.Text      = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 1) {
                    select_工作門.BackColor     = Color.Red;
                    lbl_acpos_工作門.BackColor  = Color.White;
                    lbl_spd_工作門.BackColor    = Color.White;
                } else {
                    select_工作門.BackColor     = Color.Green;
                    lbl_acpos_工作門.BackColor  = Color.Gray;
                    lbl_spd_工作門.BackColor    = Color.Gray;
                }

            }  // end of 工作門 讀取與顯示

            if (dbIncreaseGate == dbRead) {

            } else {  //工作門 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseGate && dbIncreaseGate<=Maxdb ) {

                } else if( dbIncreaseGate<=Mindb ) {
                    dbIncreaseGate = (int)Mindb;
                } else if( Maxdb<=dbIncreaseGate ) {
                    dbIncreaseGate = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeGate = calculate.Map(dbIncreaseGate, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行移動工作門
                int axis     = (int)WMX3軸定義.工作門;
                int position = fChangeGate;
                int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseGate == dbRead) {

            return dbRstGate;
        }  // end of public double dbapiGate(double dbIncreaseGate)  //Gate
        //---------------------------------------------------------------------------------------
        public double dbapiIAI(double dbIncreaseGate)  //IAI
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   3000;
                const int    MinRAW =      0;
                const double Maxdb  =   30.0;
                const double Mindb  =    0.0;
                const double Sum    =   3000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstIAI = 0.0;

            {  // start of Socket定位攝影機軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 Socket定位攝影機軸 資訊
                byte[] aGetGetIAI = new byte[2];
                clsServoControlWMX3.WMX3_GetInIO(ref aGetGetIAI, (int)(addr_IAI.pxeaI_GetStatusSignal2_2Bytes) / 10, 2);
                rslt += ((aGetGetIAI[(int)(addr_IAI.pxeaI_GetServoONState - addr_IAI.pxeaI_GetStatusSignal2_2Bytes) / 10] & (1 << (int)(addr_IAI.pxeaI_GetServoONState) % 10)) != 0) ? 1 : 0;

                byte[] aGetSetIAI = new byte[2];
                clsServoControlWMX3.WMX3_GetOutIO(ref aGetSetIAI, (int)(addr_IAI.pxeaI_SetControlSignal2_2Bytes) / 10, 2);
                rslt += ((aGetSetIAI[(int)(addr_IAI.pxeaI_SetDisableBrake - addr_IAI.pxeaI_SetControlSignal2_2Bytes) / 10] & (1 << (int)(addr_IAI.pxeaI_SetDisableBrake) % 10)) != 0) ? 1 : 0;

                //當數值有效
                if(true) { 
                    lbl_IAI_RAW.Visible               = bshow_debug_RAW_Conver_Back_Value;
                    lbl_IAI_Convert.Visible           = bshow_debug_RAW_Conver_Back_Value;
                    lbl_IAI_Back.Visible              = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GetCurrentSpeed4Bytes, 0);
                    lbl_IAI_RAW.Text              = Convert.ToString();

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed                = Speed / dbSpdF;
                    lbl_IAI_Convert.Text          = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    lbl_IAI_Back.Text             = cnback.ToString();


                    //顯示讀取長度
                    dbRstIAI                      = dbGet;
                    lbl_acpos_IAI.Text            = dbRstIAI.ToString("F3");

                    //顯示運動速度
                    lbl_spd_IAI.Text              = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 2) {
                    select_Socket檢測.BackColor      = Color.Red;
                    lbl_acpos_IAI.BackColor          = Color.White;
                    lbl_spd_IAI.BackColor            = Color.White;
                } else {
                    select_Socket檢測.BackColor      = Color.Green;
                    lbl_acpos_IAI.BackColor          = Color.Gray;
                    lbl_spd_IAI.BackColor            = Color.Gray;
                }

            }  // end of Socket定位攝影機軸 讀取與顯示

            if (dbIncreaseGate == dbRead) {

            } else {  //IAI 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseGate && dbIncreaseGate<=Maxdb ) {

                } else if( dbIncreaseGate<=Mindb ) {
                    dbIncreaseGate = (int)Mindb;
                } else if( Maxdb<=dbIncreaseGate ) {
                    dbIncreaseGate = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                double fChangeGate = calculate.Map(dbIncreaseGate, (double)Maxdb, (double)Mindb, (double)Maxdb, (double)Mindb);

                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 1);

                //執行移動工作門
                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstIAI;
        }  // end of public double dbapiGate(double dbIncreaseGate)  //IAI
        //---------------------------------------------------------------------------------------
        public double dbapJoDell3D掃描(double dbIncreaseGate)  //JoDell3D掃描
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   3000;
                const int    MinRAW =      0;
                const double Maxdb  =   30.0;
                const double Mindb  =    0.0;
                const double Sum    =   3000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstJoDell3D掃描 = 0.0;

            {  // start of JoDell3D掃描 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 JoDell3D掃描 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_3D掃描_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //當數值有效
                if(true) { 
                    lbl_JoDell3D掃描_RAW.Visible      = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell3D掃描_Convert.Visible  = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell3D掃描_Back.Visible     = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    lbl_JoDell3D掃描_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed                = Speed / dbSpdF;
                    lbl_JoDell3D掃描_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    lbl_JoDell3D掃描_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstJoDell3D掃描             = dbGet;
                    lbl_acpos_JoDell3D掃描.Text   = dbRstJoDell3D掃描.ToString("F3");

                    //顯示運動速度
                    lbl_spd_JoDell3D掃描.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 4) {
                    select_JoDell3D掃描.BackColor    = Color.Red;
                    lbl_acpos_JoDell3D掃描.BackColor = Color.White;
                    lbl_spd_JoDell3D掃描.BackColor   = Color.White;
                } else {
                    select_JoDell3D掃描.BackColor    = Color.Green;
                    lbl_acpos_JoDell3D掃描.BackColor = Color.Gray;
                    lbl_spd_JoDell3D掃描.BackColor   = Color.Gray;
                }

            }  // end of JoDell3D掃描 讀取與顯示

            if (dbIncreaseGate == dbRead) {

            } else {  //3D掃描 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseGate && dbIncreaseGate<=Maxdb ) {

                } else if( dbIncreaseGate<=Mindb ) {
                    dbIncreaseGate = (int)Mindb;
                } else if( Maxdb<=dbIncreaseGate ) {
                    dbIncreaseGate = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                double fChangeGate = calculate.Map(dbIncreaseGate, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);

                //執行移動JoDell3D掃描
                clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstJoDell3D掃描;
        }  // end of public double dbapJoDell3D掃描(double dbIncreaseGate)  //JoDell3D掃描
        //---------------------------------------------------------------------------------------
        public double dbapJoDell吸針嘴(double dbIncreaseGate)  //JoDell吸針嘴
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   3000;
                const int    MinRAW =      0;
                const double Maxdb  =   30.0;
                const double Mindb  =    0.0;
                const double Sum    =   3000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstJoDell吸針嘴 = 0.0;

            {  // start of JoDell吸針嘴 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 JoDell吸針嘴 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_吸針嘴_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //當數值有效
                if(true) { 
                    lbl_JoDell吸針嘴_RAW.Visible      = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell吸針嘴_Convert.Visible  = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell吸針嘴_Back.Visible     = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    lbl_JoDell吸針嘴_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed                = Speed / dbSpdF;
                    lbl_JoDell吸針嘴_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    lbl_JoDell吸針嘴_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstJoDell吸針嘴             = dbGet;
                    lbl_acpos_JoDell吸針嘴.Text   = dbRstJoDell吸針嘴.ToString("F3");

                    //顯示運動速度
                    lbl_spd_JoDell吸針嘴.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 4) {
                    select_JoDell吸針嘴.BackColor    = Color.Red;
                    lbl_acpos_JoDell吸針嘴.BackColor = Color.White;
                    lbl_spd_JoDell吸針嘴.BackColor   = Color.White;
                } else {
                    select_JoDell吸針嘴.BackColor    = Color.Green;
                    lbl_acpos_JoDell吸針嘴.BackColor = Color.Gray;
                    lbl_spd_JoDell吸針嘴.BackColor   = Color.Gray;
                }

            }  // end of JoDell吸針嘴 讀取與顯示

            if (dbIncreaseGate == dbRead) {

            } else {  //吸針嘴 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseGate && dbIncreaseGate<=Maxdb ) {

                } else if( dbIncreaseGate<=Mindb ) {
                    dbIncreaseGate = (int)Mindb;
                } else if( Maxdb<=dbIncreaseGate ) {
                    dbIncreaseGate = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                double fChangeGate = calculate.Map(dbIncreaseGate, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);

                //執行移動JoDell吸針嘴
                clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstJoDell吸針嘴;
        }  // end of public double dbapJoDell吸針嘴(double dbIncreaseGate)  //JoDell吸針嘴
        //---------------------------------------------------------------------------------------
        public double dbapJoDell植針嘴(double dbIncreaseGate)  //JoDell植針嘴
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   5000;
                const int    MinRAW =      0;
                const double Maxdb  =   50.0;
                const double Mindb  =    0.0;
                const double Sum    =   5000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstJoDell植針嘴 = 0.0;

            {  // start of JoDell植針嘴 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 JoDell植針嘴 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //當數值有效
                if(true) { 
                    lbl_JoDell植針嘴_RAW.Visible      = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell植針嘴_Convert.Visible  = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell植針嘴_Back.Visible     = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    lbl_JoDell植針嘴_RAW.Text     = Convert.ToString();

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed                = Speed / dbSpdF;
                    lbl_JoDell植針嘴_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    lbl_JoDell植針嘴_Back.Text    = cnback.ToString();


                    //顯示讀取長度
                    dbRstJoDell植針嘴             = dbGet;
                    lbl_acpos_JoDell植針嘴.Text   = dbRstJoDell植針嘴.ToString("F3");

                    //顯示運動速度
                    lbl_spd_JoDell植針嘴.Text     = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 4) {
                    select_JoDell植針嘴.BackColor    = Color.Red;
                    lbl_acpos_JoDell植針嘴.BackColor = Color.White;
                    lbl_spd_JoDell植針嘴.BackColor   = Color.White;
                } else {
                    select_JoDell植針嘴.BackColor    = Color.Green;
                    lbl_acpos_JoDell植針嘴.BackColor = Color.Gray;
                    lbl_spd_JoDell植針嘴.BackColor   = Color.Gray;
                }

            }  // end of JoDell植針嘴 讀取與顯示

            if (dbIncreaseGate == dbRead) {

            } else {  //植針嘴 變更位置
                //伸長量overflow保護
                if( Mindb<=dbIncreaseGate && dbIncreaseGate<=Maxdb ) {

                } else if( dbIncreaseGate<=Mindb ) {
                    dbIncreaseGate = (int)Mindb;
                } else if( Maxdb<=dbIncreaseGate ) {
                    dbIncreaseGate = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                double fChangeGate = calculate.Map(dbIncreaseGate, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);

                //執行移動JoDell植針嘴
                clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstJoDell植針嘴;
        }  // end of public double dbapJoDell植針嘴(double dbIncreaseGate)  //JoDell植針嘴
        //---------------------------------------------------------------------------------------
        //------------------------ Xavier Call, Control the Servo machine -----------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //-------------------------------- Project Code implement -------------------------------
        //---------------------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }
        //---------------------------------------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            //init vision
            inspector1.xInit();
            //Add the callback api from snapshot api
            inspector1.on下視覺 = apiCallBackTest;

            //先跳到第2頁
            int iAimToPageIndex = 4-1;
            tabControl1.SelectedTab = tabControl1.TabPages[iAimToPageIndex - 1];
        }
        //---------------------------------------------------------------------------------------
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();

            frm_Manual.Close();
            //sw.Close();
        }
        //---------------------------------------------------------------------------------------
        public bool enGC_吸嘴X軸      = false;
        public bool enGC_吸嘴Y軸      = false;
        public bool enGC_吸嘴Z軸      = false;
        public bool enGC_吸嘴R軸      = false;

        public bool enGC_載盤X軸      = false;
        public bool enGC_載盤Y軸      = false;

        public bool enGC_植針Z軸      = false;
        public bool enGC_植針R軸      = false;

        public bool enGC_工作門       = false;

        public bool enGC_IAI          = false;

        public bool enGC_JoDell植針嘴 = false;
        public bool enGC_JoDell3D掃描 = false;
        public bool enGC_JoDell吸針嘴 = false;

        public void en_Group_Click(object sender, EventArgs e)
        {  // start of public void en_Group_Click(object sender, EventArgs e)
            if (enGC_吸嘴X軸 != en_吸嘴X軸.Checked) {
                enGC_吸嘴X軸  = en_吸嘴X軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, enGC_吸嘴X軸);
            }
            if (enGC_吸嘴Y軸 != en_吸嘴Y軸.Checked) {
                enGC_吸嘴Y軸  = en_吸嘴Y軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, enGC_吸嘴Y軸);
            }
            if (enGC_吸嘴Z軸 != en_吸嘴Z軸.Checked) {
                enGC_吸嘴Z軸  = en_吸嘴Z軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, enGC_吸嘴Z軸);
            }
            if (enGC_吸嘴R軸 != en_吸嘴R軸.Checked) {
                enGC_吸嘴R軸  = en_吸嘴R軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, enGC_吸嘴R軸);
            }

            if (enGC_載盤X軸 != en_載盤X軸.Checked) {
                enGC_載盤X軸  = en_載盤X軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, enGC_載盤X軸);
            }
            if (enGC_載盤Y軸 != en_載盤Y軸.Checked) {
                enGC_載盤Y軸  = en_載盤Y軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, enGC_載盤Y軸);
            }

            if (enGC_植針Z軸 != en_植針Z軸.Checked) {
                enGC_植針Z軸  = en_植針Z軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, enGC_植針Z軸);
            }
            if (enGC_植針R軸 != en_植針R軸.Checked) {
                enGC_植針R軸  = en_植針R軸.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, enGC_植針R軸);
            }

            if (enGC_工作門 != en_工作門.Checked){
                enGC_工作門  = en_工作門.Checked;
                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門, enGC_工作門);
            }

            if (enGC_IAI != en_IAI.Checked){
                enGC_IAI  = en_IAI.Checked;
                Thread.Sleep(10);
                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, (enGC_IAI)? 1.0:0.0);
                Thread.Sleep(10);
                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,  (enGC_IAI)? 1.0:0.0);
            }

            if (enGC_JoDell3D掃描 != en_JoDell3D掃描.Checked){
                enGC_JoDell3D掃描  = en_JoDell3D掃描.Checked;

                clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, (enGC_JoDell3D掃描) ? 1.0 : 0.0);
            }

            if (enGC_JoDell吸針嘴 != en_JoDell吸針嘴.Checked){
                enGC_JoDell吸針嘴  = en_JoDell吸針嘴.Checked;

                clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, (enGC_JoDell吸針嘴) ? 1.0 : 0.0);
            }

            if (enGC_JoDell植針嘴 != en_JoDell植針嘴.Checked){
                enGC_JoDell植針嘴  = en_JoDell植針嘴.Checked;

                clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_MotorOn, (enGC_JoDell植針嘴) ? 1.0 : 0.0);
            }

    }  // end of public void en_Group_Click(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        public WMX3軸定義 wmxId_RadioGroupChanged = WMX3軸定義.AXIS_START;
        private void RadioGroupChanged(object sender, EventArgs e)
        {  // start of private void RadioGroupChanged(object sender, EventArgs e)
            // 將 sender 轉型為 RadioButton
            System.Windows.Forms.RadioButton selectedRadioButton = sender as System.Windows.Forms.RadioButton;

            //辨識選擇之軸
            if (selectedRadioButton != null && selectedRadioButton.Checked == true) {
                if (selectedRadioButton == select_吸嘴X軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴X軸;
                } else if (selectedRadioButton == select_吸嘴Y軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴Y軸;
                } else if (selectedRadioButton == select_吸嘴Z軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴Z軸;
                } else if (selectedRadioButton == select_吸嘴R軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴R軸;
                } else if (selectedRadioButton == select_載盤X軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.載盤X軸;
                } else if (selectedRadioButton == select_載盤Y軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.載盤Y軸;
                } else if (selectedRadioButton == select_植針Z軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.植針Z軸;
                } else if (selectedRadioButton == select_植針R軸) {
                    wmxId_RadioGroupChanged = WMX3軸定義.植針R軸;
                } else if (selectedRadioButton == select_工作門) {
                    wmxId_RadioGroupChanged = WMX3軸定義.工作門;
                } else if (selectedRadioButton == select_Socket檢測) {
                    wmxId_RadioGroupChanged = WMX3軸定義.IAISocket孔檢測;
                } else if (selectedRadioButton == select_JoDell3D掃描) {
                    wmxId_RadioGroupChanged = WMX3軸定義.JoDell3D掃描;
                } else if (selectedRadioButton == select_JoDell吸針嘴) {
                    wmxId_RadioGroupChanged = WMX3軸定義.JoDell吸針嘴;
                } else if (selectedRadioButton == select_JoDell植針嘴) {
                    wmxId_RadioGroupChanged = WMX3軸定義.JoDell植針嘴;
                }
            }

            //複製選擇之軸
            if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴X軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴X軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴Y軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴Y軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴Z軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴Z軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴R軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴R軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.載盤X軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_載盤X軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.載盤Y軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_載盤Y軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.植針Z軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_植針Z軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.植針R軸) {
                txtABSpos.Text = (double.Parse(lbl_acpos_植針R軸.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.工作門) {
                txtABSpos.Text = (double.Parse(lbl_acpos_工作門.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.IAISocket孔檢測) {
                txtABSpos.Text = (double.Parse(lbl_acpos_IAI.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.JoDell3D掃描) {
                txtABSpos.Text = (double.Parse(lbl_acpos_JoDell3D掃描.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.JoDell吸針嘴) {
                txtABSpos.Text = (double.Parse(lbl_acpos_JoDell吸針嘴.Text).ToString("F3"));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.JoDell植針嘴) {
                txtABSpos.Text = (double.Parse(lbl_acpos_JoDell植針嘴.Text).ToString("F3"));
            } else {
                txtABSpos.Text = "N/A";
            }

        }  // end of private void RadioGroupChanged(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        private void lbl_SetIO_Click(object sender, EventArgs e)
        {  // start of private void lbl_SetIO_Click(object sender, EventArgs e)
            // 將 sender 轉型為 Label
            System.Windows.Forms.Label SelectLabel = sender as System.Windows.Forms.Label;

            //辨識選擇之Label
            if (SelectLabel != null) {
                       if (SelectLabel == lbl擺放蓋板   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)     / 10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)     % 10, (lbl擺放蓋板.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl吸料真空閥 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_吸料真空電磁閥) / 10, (int)(WMX3IO對照.pxeIO_吸料真空電磁閥) % 10, (lbl吸料真空閥.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl堵料吹氣缸 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣缸)     / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣缸)     % 10, (lbl堵料吹氣缸.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl接料區缸   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_接料區氣桿)     / 10, (int)(WMX3IO對照.pxeIO_接料區氣桿)     % 10, (lbl接料區缸.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl植針吹氣   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)       / 10, (int)(WMX3IO對照.pxeIO_植針吹氣)       % 10, (lbl植針吹氣.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl收料區缸   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_收料區缸)       / 10, (int)(WMX3IO對照.pxeIO_收料區缸)       % 10, (lbl收料區缸.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl堵料吹氣   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣)       / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣)       % 10, (lbl堵料吹氣.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl載盤真空閥 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_載盤真空閥)     / 10, (int)(WMX3IO對照.pxeIO_載盤真空閥)     % 10, (lbl載盤真空閥.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk真空2    ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空2)    / 10, (int)(WMX3IO對照.pxeIO_Socket真空2)    % 10, (lblsk真空2.BackColor    == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl載盤破真空 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_載盤破真空)     / 10, (int)(WMX3IO對照.pxeIO_載盤破真空)     % 10, (lbl載盤破真空.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk破真空2  ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket破真空2)  / 10, (int)(WMX3IO對照.pxeIO_Socket破真空2)  % 10, (lblsk破真空2.BackColor  == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk真空1    ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空1)    / 10, (int)(WMX3IO對照.pxeIO_Socket真空1)    % 10, (lblsk真空1.BackColor    == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl擺放座真空 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座吸真空)   / 10, (int)(WMX3IO對照.pxeIO_擺放座吸真空)   % 10, (lbl擺放座真空.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk破真空1  ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket破真空1)  / 10, (int)(WMX3IO對照.pxeIO_Socket破真空1)  % 10, (lblsk破真空1.BackColor  == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl擺放破真空 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座破真空)   / 10, (int)(WMX3IO對照.pxeIO_擺放座破真空)   % 10, (lbl擺放破真空.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl取料吸嘴吸 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)     / 10, (int)(WMX3IO對照.pxeIO_取料吸嘴吸)     % 10, (lbl取料吸嘴吸.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl下後左門鎖 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_下後左門鎖)     / 10, (int)(WMX3IO對照.pxeIO_下後左門鎖)     % 10, (lbl下後左門鎖.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl取料吸嘴破 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空) / 10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空) % 10, (lbl取料吸嘴破.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl下後右門鎖 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_下後右門鎖)     / 10, (int)(WMX3IO對照.pxeIO_下後右門鎖)     % 10, (lbl下後右門鎖.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl植針Z煞車  ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針Z煞車)      / 10, (int)(WMX3IO對照.pxeIO_植針Z煞車)      % 10, (lbl植針Z煞車.BackColor  == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblHEPA       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_HEPA)           / 10, (int)(WMX3IO對照.pxeIO_HEPA)           % 10, (lblHEPA.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl艙內燈     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT)          / 10, (int)(WMX3IO對照.pxeIO_LIGHT)          % 10, (lbl艙內燈.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl右按鈕綠燈 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈) / 10, (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈) % 10, (lbl右按鈕綠燈.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl紅燈       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)       / 10, (int)(WMX3IO對照.pxeIO_機台紅燈)       % 10, (lbl紅燈.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl中按鈕綠燈 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈) / 10, (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈) % 10, (lbl中按鈕綠燈.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl黃燈       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台黃燈)       / 10, (int)(WMX3IO對照.pxeIO_機台黃燈)       % 10, (lbl黃燈.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl左按鈕紅燈 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈) / 10, (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈) % 10, (lbl左按鈕紅燈.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl綠燈       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)       / 10, (int)(WMX3IO對照.pxeIO_機台綠燈)       % 10, (lbl綠燈.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblBuzzer     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer)         / 10, (int)(WMX3IO對照.pxeIO_Buzzer)         % 10, (lblBuzzer.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                }


            }
        }  // end of private void lbl_SetIO_Click(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        public void btn_adjust_JOG(object sender, EventArgs e)
        {  // start of public void btn_adjust_JOG(object sender, EventArgs e)
            // 將 sender 轉型為 Button
            System.Windows.Forms.Button ptrBtn = sender as System.Windows.Forms.Button;

            double result = double.Parse(txtABSpos.Text) + 0.0;

            if (ptrBtn == btn_plus_d001) {
                result += 0.001;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_d001) {
                result -= 0.001;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_d01) {
                result += 0.01;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_d01) {
                result -= 0.01;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_d1) {
                result += 0.1;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_d1) {
                result -= 0.1;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_1) {
                result += 1.0;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_1) {
                result -= 1.0;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_10) {
                result += 10.0;
                ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_10) {
                result -= 10.0;
                ptrBtn = btnABSMove;
            }

            if (ptrBtn == btnABSMove) {
                //辨識選擇之軸
                switch(wmxId_RadioGroupChanged) {
                    case WMX3軸定義.吸嘴X軸:
                        if(enGC_吸嘴X軸 == true) {
                            dbapiNozzleX(result, 250);
                        }
                        break;
                    case WMX3軸定義.吸嘴Y軸:
                        if(enGC_吸嘴Y軸 == true) {
                            dbapiNozzleY(result, 100);
                        }
                        break;
                    case WMX3軸定義.吸嘴Z軸:
                        if(enGC_吸嘴Z軸 == true) {
                            dbapiNozzleZ(result, 20);
                        }
                        break;
                    case WMX3軸定義.吸嘴R軸:
                        if(enGC_吸嘴R軸 == true) {
                            dbapiNozzleR(result, 70);
                        }
                        break;
                    case WMX3軸定義.載盤X軸:
                        if(enGC_載盤X軸 == true) {
                            dbapiCarrierX(result, 190);
                        }
                        break;
                    case WMX3軸定義.載盤Y軸:
                        if(enGC_載盤Y軸 == true) {
                            dbapiCarrierY(result, 800);
                        }
                        break;
                    case WMX3軸定義.植針Z軸:
                        if(enGC_植針Z軸 == true) {
                            dbapiSetZ(result, 33);
                        }
                        break;
                    case WMX3軸定義.植針R軸:
                        if(enGC_植針R軸 == true) {
                            dbapiSetR(result, 360);
                        }
                        break;
                    case WMX3軸定義.工作門:
                        if(enGC_工作門 == true) {
                            dbapiGate(result, 580/4);
                        }
                        break;
                    case WMX3軸定義.IAISocket孔檢測:
                        if(enGC_IAI == true) {
                            dbapiIAI(result);
                        }
                        break;
                    case WMX3軸定義.JoDell3D掃描:
                        if(enGC_JoDell3D掃描 == true) {
                            dbapJoDell3D掃描(result);
                        }
                        break;
                    case WMX3軸定義.JoDell吸針嘴:
                        if(enGC_JoDell吸針嘴 == true) {
                            dbapJoDell吸針嘴(result);
                        }
                        break;
                    case WMX3軸定義.JoDell植針嘴:
                        if(enGC_JoDell植針嘴 == true) {
                            dbapJoDell植針嘴(result);
                        }
                        break;
                }
            }

            txtABSpos.Text = result.ToString("F3");
        }  // end of public void btn_adjust_JOG(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //讀取OutputIO
        public byte[] pDataGetOutIO = new byte[4];

        //讀取InputIO
        public byte[] pDataGetInIO = new byte[8];

        private void timer1_Tick(object sender, EventArgs e)
        {  // start of private void timer1_Tick(object sender, EventArgs e)
            //WMX3通訊狀態
            int getCommuStatus = clsServoControlWMX3.WMX3_check_Commu();
            if (getCommuStatus == 1) {
                label1.Text = "連線中";
                label1.ForeColor = Color.Red;
            } else {
                label1.Text = "尚未連線";
                label1.ForeColor = Color.Black;
            }


            //region 讀取軸狀態
            double dbState = dbRead;
            {
                //軸控保護需要分別保護封裝
                inspector1.nozzleX = dbapiNozzleX(dbState, 0);
                inspector1.nozzleY = dbapiNozzleY(dbState, 0);
                dbapiNozzleZ(dbState, 0);
                dbapiNozzleR(dbState, 0);

                inspector1.移載X = dbapiCarrierX(dbState, 0);
                inspector1.移載Y = dbapiCarrierY(dbState, 0);

                dbapiSetZ(dbState, 0);
                dbapiSetR(dbState, 0);

                dbapiGate(dbState, 0);

                dbapiIAI(dbState);

                dbapJoDell3D掃描(dbState);
                dbapJoDell吸針嘴(dbState);
                dbapJoDell植針嘴(dbState);
            }  // end of double dbState = dbRead;

            //讀取 Yaskawa OutputIO
            clsServoControlWMX3.WMX3_GetOutIO(ref pDataGetOutIO, (int)WMX3IO對照.pxeIO_Addr4, 4);
            {
                lbl擺放蓋板.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸料真空閥.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_吸料真空電磁閥) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料吹氣缸.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_堵料吹氣缸)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料吹氣缸)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl接料區缸.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_接料區氣桿)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_接料區氣桿)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl植針吹氣.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_植針吹氣)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針吹氣)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl收料區缸.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_收料區缸)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_收料區缸)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料吹氣.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_堵料吹氣)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料吹氣)        % 10)) != 0) ? Color.Green : Color.Red;

                lbl載盤真空閥.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_載盤真空閥)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤真空閥)      % 10)) != 0) ? Color.Green : Color.Red;
                lblsk真空2.BackColor    = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket真空2)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket真空2)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤破真空.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_載盤破真空)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤破真空)      % 10)) != 0) ? Color.Green : Color.Red;
                lblsk破真空2.BackColor  = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket破真空2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket破真空2)   % 10)) != 0) ? Color.Green : Color.Red;
                lblsk真空1.BackColor    = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket真空1)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket真空1)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放座真空.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座吸真空)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座吸真空)    % 10)) != 0) ? Color.Green : Color.Red;
                lblsk破真空1.BackColor  = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket破真空1)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket破真空1)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放破真空.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座破真空)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座破真空)    % 10)) != 0) ? Color.Green : Color.Red;

                lbl取料吸嘴吸.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_取料吸嘴吸)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料吸嘴吸)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後左門鎖.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_下後左門鎖)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下後左門鎖)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料吸嘴破.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_取料吸嘴破真空) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後右門鎖.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_下後右門鎖)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下後右門鎖)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl植針Z煞車.BackColor  = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_植針Z煞車)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針Z煞車)       % 10)) != 0) ? Color.Green : Color.Red;
                lblHEPA.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_HEPA)           / 10)] & (1 << (int)(WMX3IO對照.pxeIO_HEPA)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl艙內燈.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_LIGHT)          / 10)] & (1 << (int)(WMX3IO對照.pxeIO_LIGHT)           % 10)) != 0) ? Color.Green : Color.Red;

                lbl右按鈕綠燈.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板右按鈕綠燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl紅燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台紅燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台紅燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl中按鈕綠燈.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板中按鈕綠燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl黃燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台黃燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台黃燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl左按鈕紅燈.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板左按鈕紅燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl綠燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台綠燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台綠燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lblBuzzer.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Buzzer)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Buzzer)          % 10)) != 0) ? Color.Green : Color.Red;
            }  // end of clsServoControlWMX3.WMX3_GetOutIO(ref pDataGetOutIO, (int)WMX3IO對照.pxeIO_Addr4, 4);

            //讀取 Yaskawa InputIO
            clsServoControlWMX3.WMX3_GetInIO(ref pDataGetInIO, (int)WMX3IO對照.pxeIO_Addr28, 8);
            {
                lbl載盤Y後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤Y軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤Y軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料Y後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料Y軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料Y軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤Y前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤Y軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤Y軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料Y前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料Y軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料Y軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料X後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料X軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料X軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料X前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料X軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料X軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;

                lbl植針Z後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_植針Z軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針Z軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl植針Z前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_植針Z軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針Z軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤X前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤X軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤X軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤X後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤X軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤X軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;

                lbl載盤空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤真空檢1)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤真空檢1)     % 10)) != 0) ? Color.Green : Color.Red;
                lblsk2空1.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket2真空檢1)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket2真空檢1)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤真空檢2)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤真空檢2)     % 10)) != 0) ? Color.Green : Color.Red;
                lblsk2空2.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket2真空檢2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket2真空檢2)  % 10)) != 0) ? Color.Green : Color.Red;
                lblsk1空1.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket1真空檢1)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket1真空檢1)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座真空檢1)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座真空檢1)   % 10)) != 0) ? Color.Green : Color.Red;
                lblsk1空2.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket1真空檢2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket1真空檢2)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座真空檢2)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座真空檢2)   % 10)) != 0) ? Color.Green : Color.Red;

                lbl吸嘴空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸嘴真空檢1)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸嘴真空檢1)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸嘴空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸嘴真空檢2)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸嘴真空檢2)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料ng盒.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料NG收料盒)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料NG收料盒)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl兩點壓1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_兩點組合壓力檢1) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_兩點組合壓力檢1) % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料盒.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_堵料收料盒)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料收料盒)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl兩點壓2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_兩點組合壓力檢2) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_兩點組合壓力檢2) % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸料盒.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸料收料盒)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸料收料盒)      % 10)) != 0) ? Color.Green : Color.Red;

                lbl復歸鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_復歸按鈕)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_復歸按鈕)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl啟動鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_啟動按鈕)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_啟動按鈕)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl停止鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_停止按鈕)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_停止按鈕)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl急停鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_緊急停止按鈕)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_緊急停止按鈕)    % 10)) != 0) ? Color.Green : Color.Red;

                lbl_擺放座開.BackColor  = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板開)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板開)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl_擺放座關.BackColor  = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板合)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板合)    % 10)) != 0) ? Color.Green : Color.Red;

                lbl上左右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩左側右門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩左側右門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上右右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩右側右門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩右側右門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上左左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩左側左門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩左側左門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上右左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩右側左門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩右側左門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上後右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩後側右門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩後側右門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl螢幕小門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_螢幕旁小門)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_螢幕旁小門)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl上後左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩後側左門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩後側左門)    % 10)) != 0) ? Color.Green : Color.Red;

                lbl下左右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架左側右門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架左側右門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架後側左門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架後側左門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下左左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架左側左門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架左側左門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架後側右門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架後側右門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下右右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架右側右門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架右側右門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下右左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架右側左門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架右側左門)  % 10)) != 0) ? Color.Green : Color.Red;
            }  // end of clsServoControlWMX3.WMX3_GetInIO(ref pDataGetInIO, (int)WMX3IO對照.pxeIO_Addr28, 8);

        }  // end of private void timer1_Tick(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //-------------------------------- Project Code implement -------------------------------
        //---------------------------------------------------------------------------------------






        //---------------------------------------------------------------------------------------
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_establish_Commu();
        }
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();
        }
        private void btn_AlarmRST_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_ClearAlarm();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            bool isOn = false;

            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門,  isOn);

            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff,            isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,             isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_MotorOn, isOn?1.0:0.0);
        }
        private void btnSetHome_Click(object sender, EventArgs e)
        {
            int rslt = 0;
            int axis = 0;
            string position = "";
            string speed = "";

            axis = (int)WMX3軸定義.吸嘴X軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1)
            {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }

            axis = (int)WMX3軸定義.吸嘴Y軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1)
            {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }

            axis = (int)WMX3軸定義.吸嘴Z軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1)
            {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }

            axis = (int)WMX3軸定義.吸嘴R軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1)
            {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }
        }



        enum 柔震 { 震散     = 0,
                    上下至中 = 1,
                    左右至中 = 2,
                    料倉     = 3,
        }; 柔震 e柔震 = 柔震.震散;
        uint[] 頻率  = { 0188, 0180, 0169, 0220 },
               相1始 = { 0000, 0297, 0000, 0000 }, 相1終 = { 0500, 0572, 0896, 0000 }, 相1力 = { 0750, 1000, 1000, 0000 },
               相2始 = { 0000, 0056, 0280, 0000 }, 相2終 = { 0500, 0070, 0902, 0000 }, 相2力 = { 0750, 1000, 1000, 0000 },
               相3始 = { 0000, 0485, 0235, 0000 }, 相3終 = { 0500, 0229, 0457, 0000 }, 相3力 = { 0750, 1000, 1000, 0000 },
               相4始 = { 0000, 0160, 0381, 0000 }, 相4終 = { 0500, 0318, 0464, 0000 }, 相4力 = { 0750, 1000, 1000, 0000 },
               倉始  = { 0000, 0000, 0000, 0010 }, 倉終  = { 0000, 0000, 0000, 0100 }, 倉力  = { 0000, 0000, 0000, 0440 };

        private void lbl柔震index(object sender, EventArgs e)
        {
            System.Windows.Forms.Label SelectLabel = sender as System.Windows.Forms.Label;
            if (SelectLabel != null) {
                       if (SelectLabel == lbl震散   ) { lbl震散.BackColor = Color.Red;   lbl上下收.BackColor = Color.Green; lbl左右收.BackColor = Color.Green; lbl料倉.BackColor = Color.Green;
                } else if (SelectLabel == lbl上下收 ) { lbl震散.BackColor = Color.Green; lbl上下收.BackColor = Color.Red;   lbl左右收.BackColor = Color.Green; lbl料倉.BackColor = Color.Green;
                } else if (SelectLabel == lbl左右收 ) { lbl震散.BackColor = Color.Green; lbl上下收.BackColor = Color.Green; lbl左右收.BackColor = Color.Red;   lbl料倉.BackColor = Color.Green;
                } else if (SelectLabel == lbl料倉   ) { lbl震散.BackColor = Color.Green; lbl上下收.BackColor = Color.Green; lbl左右收.BackColor = Color.Green; lbl料倉.BackColor = Color.Red;
                } 
            }
        }

        public void btnVibrationInit_Click(object sender, EventArgs e) {
                       if (lbl震散.BackColor   == Color.Red) { e柔震 = 柔震.震散;
                } else if (lbl上下收.BackColor == Color.Red) { e柔震 = 柔震.上下至中;
                } else if (lbl左右收.BackColor == Color.Red) { e柔震 = 柔震.左右至中;
                } else if (lbl料倉.BackColor   == Color.Red) { e柔震 = 柔震.料倉;
                }

            //Vibration
            clsVibration.apiEstablishTCPVibration();
            {
                clsVibration.u32Frequency                   = 頻率[(int)e柔震];
                clsVibration.u32VibrationSource1_StartPhase = 相1始[(int)e柔震]; clsVibration.u32VibrationSource1_StopPhase = 相1終[(int)e柔震]; clsVibration.u32VibrationSource1_Power = 相1力[(int)e柔震];
                clsVibration.u32VibrationSource2_StartPhase = 相2始[(int)e柔震]; clsVibration.u32VibrationSource2_StopPhase = 相2終[(int)e柔震]; clsVibration.u32VibrationSource2_Power = 相2力[(int)e柔震];
                clsVibration.u32VibrationSource3_StartPhase = 相3始[(int)e柔震]; clsVibration.u32VibrationSource3_StopPhase = 相3終[(int)e柔震]; clsVibration.u32VibrationSource3_Power = 相3力[(int)e柔震];
                clsVibration.u32VibrationSource4_StartPhase = 相4始[(int)e柔震]; clsVibration.u32VibrationSource4_StopPhase = 相4終[(int)e柔震]; clsVibration.u32VibrationSource4_Power = 相4力[(int)e柔震];
                clsVibration.u32BlackDepotSource_StartPhase = 倉始[(int)e柔震];  clsVibration.u32BlackDepotSource_StopPhase = 倉終[(int)e柔震];  clsVibration.u32BlackDepotSource_Power = 倉力[(int)e柔震];
                clsVibration.SetVibration(clsVibration.u32Frequency,
                                          clsVibration.u32VibrationSource1_StartPhase,
                                          clsVibration.u32VibrationSource1_StopPhase,
                                          clsVibration.u32VibrationSource2_StartPhase,
                                          clsVibration.u32VibrationSource2_StopPhase,
                                          clsVibration.u32VibrationSource3_StartPhase,
                                          clsVibration.u32VibrationSource3_StopPhase,
                                          clsVibration.u32VibrationSource4_StartPhase,
                                          clsVibration.u32VibrationSource4_StopPhase,
                                          clsVibration.u32BlackDepotSource_StartPhase,
                                          clsVibration.u32BlackDepotSource_StopPhase,
                                          clsVibration.u32VibrationSource1_Power,
                                          clsVibration.u32VibrationSource2_Power,
                                          clsVibration.u32VibrationSource3_Power,
                                          clsVibration.u32VibrationSource4_Power,
                                          clsVibration.u32BlackDepotSource_Power);

                clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }

        private void btnVibrationStop_Click(object sender, EventArgs e)
        {
            //Vibration
            clsVibration.apiEstablishTCPVibration();
            {
                uint bRunning = 0;
                clsVibration.Px1_SendCMD(xe_U15_CMD.xeUC_TestMode_FunctionOn, bRunning);
            }
        }

        private void SB_VBLED_Scroll(object sender, ScrollEventArgs e)
        {
            //Vibration LED
            clsVibration.apiEstablishTCPVibration(); {
                clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }






        enum eWarningSpeed {
            xeeWS_Disable,

            xeeWS_RedLowSpeed,
            xeeWS_RedHighSpeed,

            xeeWS_YellowLowSpeed,
            xeeWS_YellowHighSpeed,

            xeeWS_GreenLowSpeed,
            xeeWS_GreenHighSpeed,
        }; 
        eWarningSpeed eWIndicatorSpeed = eWarningSpeed.xeeWS_Disable;

        int iWarningLEDCnt        = 0;
        bool bBuzzerWarningRed    = false,
             bBuzzerWarningYellow = false,
             bBuzzerWarningGreen  = false;

        private void tmr_Buzzer_Tick(object sender, EventArgs e)
        {  // start of private void tmr_Buzzer_Tick(object sender, EventArgs e)
            int iWarningLEDSpeed = 0;

            switch (eWIndicatorSpeed) {
                case eWarningSpeed.xeeWS_Disable:
                    iWarningLEDCnt = 0;
                    break;

                case eWarningSpeed.xeeWS_RedLowSpeed:   iWarningLEDSpeed = 5;  goto lbl_WarningRED;
                case eWarningSpeed.xeeWS_RedHighSpeed:  iWarningLEDSpeed = 2;  goto lbl_WarningRED;
                    lbl_WarningRED: {
                        iWarningLEDCnt++;
                        if(iWarningLEDCnt>=iWarningLEDSpeed) {
                            iWarningLEDCnt = 0;

                            bBuzzerWarningRed = !bBuzzerWarningRed;
                        }

                        if (bBuzzerWarningRed == false) {
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)/10, (int)(WMX3IO對照.pxeIO_機台紅燈)%10, 0);
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台黃燈)/10, (int)(WMX3IO對照.pxeIO_機台黃燈)%10, 0);
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)/10, (int)(WMX3IO對照.pxeIO_機台綠燈)%10, 0);
                        } else {
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)/10, (int)(WMX3IO對照.pxeIO_機台紅燈)%10, 1);
                        }
                    } break;

                case eWarningSpeed.xeeWS_YellowLowSpeed:   iWarningLEDSpeed = 5;  goto lbl_WarningYellow;
                case eWarningSpeed.xeeWS_YellowHighSpeed:  iWarningLEDSpeed = 2;  goto lbl_WarningYellow; 
                    lbl_WarningYellow: {
                        iWarningLEDCnt++;
                        if(iWarningLEDCnt>=iWarningLEDSpeed) {
                            iWarningLEDCnt = 0;

                            bBuzzerWarningYellow = !bBuzzerWarningYellow;
                        }

                        if (bBuzzerWarningYellow == false) {
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)/10, (int)(WMX3IO對照.pxeIO_機台紅燈)%10, 0);
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台黃燈)/10, (int)(WMX3IO對照.pxeIO_機台黃燈)%10, 0);
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)/10, (int)(WMX3IO對照.pxeIO_機台綠燈)%10, 0);
                        } else {
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台黃燈)/10, (int)(WMX3IO對照.pxeIO_機台黃燈)%10, 1);
                        }
                    } break;

                case eWarningSpeed.xeeWS_GreenLowSpeed:   iWarningLEDSpeed = 5;  goto lbl_WarningGreen;
                case eWarningSpeed.xeeWS_GreenHighSpeed:  iWarningLEDSpeed = 2;  goto lbl_WarningGreen; 
                    lbl_WarningGreen: {
                        iWarningLEDCnt++;
                        if(iWarningLEDCnt>=iWarningLEDSpeed) {
                            iWarningLEDCnt = 0;

                            bBuzzerWarningGreen = !bBuzzerWarningGreen;
                        }

                        if (bBuzzerWarningGreen == false) {
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)/10, (int)(WMX3IO對照.pxeIO_機台紅燈)%10, 0);
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台黃燈)/10, (int)(WMX3IO對照.pxeIO_機台黃燈)%10, 0);
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)/10, (int)(WMX3IO對照.pxeIO_機台綠燈)%10, 0);
                        } else {
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)/10, (int)(WMX3IO對照.pxeIO_機台綠燈)%10, 1);
                        }
                    } break;
            }  // end of switch (eWIndicatorSpeed) {
        }  // end of private void tmr_Buzzer_Tick(object sender, EventArgs e)







        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            /*
                y=0.000454x−2.5071
                x=(y+2.5071)/0.000454
            */

            Normal calculate = new Normal();       
            double dbGet = calculate.Map(vcb_植針吹氣流量閥.Value, 110, -10, -10, 110);

            if (100.0 <= dbGet) {
                dbGet = 100;
            }
            if(dbGet <= 0.0) {
                dbGet = 0;
            }

            double y = (double)( dbGet/10.0 );
            double x = (y + 2.5071) / 0.000454;

            int iGetValue = (int)x;
          //lbl_植針吹氣流量閥.Text = iGetValue.ToString();
            byte[] aGetValue = BitConverter.GetBytes(iGetValue);
          //clsServoControlWMX3.WMX3_SetIO(ref aGetValue, (int)WMX3IO對照.pxeIO_Addr_AnalogOut_0, 2);
            clsServoControlWMX3.WMX3_SetIO(ref aGetValue, (int)WMX3IO對照.pxeIO_Addr_AnalogOut_1, 2);

            int iGetIn0Value = 0;
            byte[] aGetIn0Value = new byte[2];
            clsServoControlWMX3.WMX3_GetInIO(ref aGetIn0Value, (int)WMX3IO對照.pxeIO_Addr_AnalogIn_0, 2);
            iGetIn0Value = BitConverter.ToInt16(aGetIn0Value, 0);

            int iGetIn1Value = 0;
            byte[] aGetIn1Value = new byte[2];
            clsServoControlWMX3.WMX3_GetInIO(ref aGetIn1Value, (int)WMX3IO對照.pxeIO_Addr_AnalogIn_1, 2);
            iGetIn1Value = BitConverter.ToInt16(aGetIn1Value, 0);

            int iGetIn2Value = 0;
            byte[] aGetIn2Value = new byte[2];
            clsServoControlWMX3.WMX3_GetInIO(ref aGetIn2Value, (int)WMX3IO對照.pxeIO_Addr_AnalogIn_2, 2);
            iGetIn2Value = BitConverter.ToInt16(aGetIn2Value, 0);

            int iGetIn3Value = 0;
            byte[] aGetIn3Value = new byte[2];
            clsServoControlWMX3.WMX3_GetInIO(ref aGetIn3Value, (int)WMX3IO對照.pxeIO_Addr_AnalogIn_3, 2);
            iGetIn3Value = BitConverter.ToInt16(aGetIn3Value, 0);

            //this.Text = "In:"                   + " " +
            //            iGetIn0Value.ToString() + " " + 
            //            iGetIn1Value.ToString() + " " + 
            //            iGetIn2Value.ToString() + " " + 
            //            iGetIn3Value.ToString() + " " +
            //            "Out:"                  + " " +
            //            iGetValue.ToString()    + " " +
            //            "y:" + y.ToString();
            lbl_植針吹氣流量閥.Text = string.Format("{0:F1}", y);
        }







        public enum xe_tmr_sequense {
            xets_empty,
            xets_idle,
            xets_home_start,
                xets_home_StartGate_01,
                xets_home_StartGate_02,
                xets_home_CheckGate,
                xets_home_EndGate, 

                xets_home_鬆開擺放座蓋板,

                xets_home_StartZR電動缸Home_01, 
                xets_home_StartZR電動缸Home_02, 
                xets_home_CheckZR電動缸Home, 
                xets_home_EndZR電動缸Home, 

                xets_home_StartXYHome_01, 
                xets_home_StartXYHome_02, 
                xets_home_CheckXYHome, 
                xets_home_EndXYHome, 

                xets_home_StartSetZR_01,
                xets_home_StartSetZR_02,
                xets_home_CheckSetZR,
                xets_home_EndSetZR01,

                xets_home_StartCarrierXHome_01,
                xets_home_StartCarrierXHome_02,
                xets_home_CheckCarrierXHome,
                xets_home_EndCarrierXHome,

                xets_home_StartCarrierYHome_01,
                xets_home_StartCarrierYHome_02,
                xets_home_CheckCarrierYHome,
                xets_home_EndCarrierYHome,
            xets_home_end,
            xets_end,
        };
        public xe_tmr_sequense xeTmrSequense = xe_tmr_sequense.xets_empty;

        public int ihomeFinishedCNT = 0;
        public bool bhome = false;

        public const double dbNozzle安全原點X = 242;
        public const double dbNozzle安全原點Y = 28;
        public const double dbNozzle安全原點Z = 0;
        public const double dbNozzle安全原點R = 1.350;

        private void btn_home_Click(object sender, EventArgs e)
        {
            bhome = true;
        }

        private void tmr_Sequense_Tick(object sender, EventArgs e)
        {
            int getrslt = 0;
            lbl_debug.Text = clsServoControlWMX3.WMX3_check_ServoOpState((int)WMX3軸定義.工作門, ref getrslt);

            switch (xeTmrSequense) {
                case xe_tmr_sequense.xets_home_start:
                    btn_home.Text = "Start Home";

                    //Disable All
                    en_吸嘴X軸.Checked      = false;
                    en_吸嘴Y軸.Checked      = false;
                    en_吸嘴Z軸.Checked      = false;
                    en_吸嘴R軸.Checked      = false;

                    en_載盤X軸.Checked      = false;
                    en_載盤Y軸.Checked      = false;

                    en_植針Z軸.Checked      = false;
                    en_植針R軸.Checked      = false;

                    en_工作門.Checked       = false;

                    en_IAI.Checked          = false;
                    en_JoDell3D掃描.Checked = false;
                    en_JoDell吸針嘴.Checked = false;
                    en_JoDell植針嘴.Checked = false;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, false);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, false);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, false);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, false);  Thread.Sleep(10);

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, false);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, false);  Thread.Sleep(10);

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, false);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, false);  Thread.Sleep(10);

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門,  false);  Thread.Sleep(10);

                    clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 0);             Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,  0);             Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, 0);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, 0);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_MotorOn, 0);  Thread.Sleep(10);

                    xeTmrSequense = xe_tmr_sequense.xets_home_StartGate_01; 
                    break;

                case xe_tmr_sequense.xets_home_StartGate_01:
                    en_工作門.Checked = true;
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門, true);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_StartGate_02;
                    break;

                case xe_tmr_sequense.xets_home_StartGate_02:
                    dbapiGate(580, 580/4);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_CheckGate;
                    break;

                case xe_tmr_sequense.xets_home_CheckGate:
                    if (true) {
                        int rslt01 = 0;
                        int axis01 = 0;

                        axis01 = (int)WMX3軸定義.工作門;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        double iGetPos = dbapiGate(dbRead, 0); ;

                        if (rslt01==1 && 580.0*0.99 <= iGetPos) { 
                            xeTmrSequense = xe_tmr_sequense.xets_home_鬆開擺放座蓋板;
                        }
                    }
                    break;

                case xe_tmr_sequense.xets_home_鬆開擺放座蓋板:
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板) / 10, (int)(WMX3IO對照.pxeIO_擺放座蓋板) % 10, 0);
                    xeTmrSequense = xe_tmr_sequense.xets_home_EndGate;
                    break;

                case xe_tmr_sequense.xets_home_EndGate:
                case xe_tmr_sequense.xets_home_StartZR電動缸Home_01:
                    en_吸嘴Z軸.Checked = true;
                    en_吸嘴R軸.Checked = true;

                    en_IAI.Checked          = true;
                    en_JoDell3D掃描.Checked = true;
                    en_JoDell吸針嘴.Checked = true;
                    en_JoDell植針嘴.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, true);   Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, true);   Thread.Sleep(10);

                    clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 1);             Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,  1);             Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, 1);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, 1);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_MotorOn, 1);  Thread.Sleep(10);

                    xeTmrSequense = xe_tmr_sequense.xets_home_StartZR電動缸Home_02;
                    break;

                case xe_tmr_sequense.xets_home_StartZR電動缸Home_02:
                    if(true) { 
                        int rslt = 0;
                        int axis = 0;
                        string position = "";
                        string speed    = "";

                        axis = (int)WMX3軸定義.吸嘴Z軸;
                        rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);  Thread.Sleep(10);
                        if (rslt == 1) {
                            clsServoControlWMX3.WMX3_SetHomePosition(axis);                               Thread.Sleep(10);
                        }

                        axis = (int)WMX3軸定義.吸嘴R軸;
                        rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);  Thread.Sleep(10);
                        if (rslt == 1) {
                            clsServoControlWMX3.WMX3_SetHomePosition(axis);                               Thread.Sleep(10);
                        }

                        dbapiIAI(0);  Thread.Sleep(10);

                        xeTmrSequense = xe_tmr_sequense.xets_home_CheckZR電動缸Home;
                    }
                    break;

                case xe_tmr_sequense.xets_home_CheckZR電動缸Home:
                    if(true) {
                        int rslt01 = 0, rslt02 = 0;
                        int axis01 = 0, axis02 = 0;

                        axis01 = (int)WMX3軸定義.吸嘴Z軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        axis02 = (int)WMX3軸定義.吸嘴R軸;
                        rslt02 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis02);  Thread.Sleep(10);

                        if (rslt01==1 && rslt02==1) {
                            if (dbapiNozzleZ(dbRead, 0) <= 1.0) {
                                xeTmrSequense = xe_tmr_sequense.xets_home_EndZR電動缸Home;
                            }
                        }
                    }
                    break;

                case xe_tmr_sequense.xets_home_EndZR電動缸Home:
                    dbapiIAI(10);          Thread.Sleep(10);

                    dbapJoDell3D掃描(10);  Thread.Sleep(10);
                    dbapJoDell吸針嘴(10);  Thread.Sleep(10);
                    dbapJoDell植針嘴(10);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_StartXYHome_01;
                    break;

                case xe_tmr_sequense.xets_home_StartXYHome_01:
                    en_吸嘴X軸.Checked = true;
                    en_吸嘴Y軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, true);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, true);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_StartXYHome_02;
                    break;

                case xe_tmr_sequense.xets_home_StartXYHome_02:
                    if (dbapiNozzleZ(dbRead, 0) <= 1.0) {
                        dbapiNozzleX(dbNozzle安全原點X, 50);   Thread.Sleep(10);
                        dbapiNozzleY(dbNozzle安全原點Y, 10);   Thread.Sleep(10);
                        xeTmrSequense = xe_tmr_sequense.xets_home_CheckXYHome;
                    }
                    break;

                case xe_tmr_sequense.xets_home_CheckXYHome:
                    if (dbapiNozzleZ(dbRead, 0) <= 1.0) { 
                        int rslt01 = 0, rslt02 = 0;
                        int axis01 = 0, axis02 = 0;

                        axis01 = (int)WMX3軸定義.吸嘴X軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        axis02 = (int)WMX3軸定義.吸嘴Y軸;
                        rslt02 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis02);  Thread.Sleep(10);

                        if (rslt01 == 1 && rslt02 == 1) {
                            xeTmrSequense = xe_tmr_sequense.xets_home_EndXYHome;
                        }
                    }
                    break;

                case xe_tmr_sequense.xets_home_EndXYHome:
                case xe_tmr_sequense.xets_home_StartSetZR_01:
                    en_植針Z軸.Checked = true;
                    en_植針R軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, true);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, true);  Thread.Sleep(10);

                    xeTmrSequense = xe_tmr_sequense.xets_home_StartSetZR_02;
                    break;

                case xe_tmr_sequense.xets_home_StartSetZR_02:
                    dbapiSetZ(15, 33);           Thread.Sleep(10);
                    dbapiSetR(268.08, 360);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_CheckSetZR;
                    break;

                case xe_tmr_sequense.xets_home_CheckSetZR:
                    if (true) {
                        int rslt01 = 0, rslt02 = 0;
                        int axis01 = 0, axis02 = 0;

                        axis01 = (int)WMX3軸定義.植針Z軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        axis02 = (int)WMX3軸定義.植針R軸;
                        rslt02 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis02);  Thread.Sleep(10);

                        if (rslt01 == 1 && rslt02 == 1) {
                            if (dbapiSetZ(dbRead, 0) <= 16) {
                                xeTmrSequense = xe_tmr_sequense.xets_home_EndSetZR01;
                            }
                        }
                    }
                    break;

                case xe_tmr_sequense.xets_home_EndSetZR01:
                case xe_tmr_sequense.xets_home_StartCarrierXHome_01:
                    en_載盤X軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, true);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_StartCarrierXHome_02;
                    break;

                case xe_tmr_sequense.xets_home_StartCarrierXHome_02:
                    if (dbapiSetZ(dbRead, 0) <= 16) {
                        dbapiCarrierX(95, 190*0.2);
                        xeTmrSequense = xe_tmr_sequense.xets_home_CheckCarrierXHome;
                    }
                    break;

                case xe_tmr_sequense.xets_home_CheckCarrierXHome:
                    if (true) {
                        int rslt01 = 0;
                        int axis01 = 0;

                        axis01 = (int)WMX3軸定義.載盤X軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        if (rslt01 == 1) {
                            if (dbapiSetZ(dbRead, 0) <= 16) {
                                xeTmrSequense = xe_tmr_sequense.xets_home_EndCarrierXHome;
                            }
                        }
                    }
                    break;

                case xe_tmr_sequense.xets_home_EndCarrierXHome:
                case xe_tmr_sequense.xets_home_StartCarrierYHome_01:
                    en_載盤Y軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, true);  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_home_StartCarrierYHome_02;
                    break;

                case xe_tmr_sequense.xets_home_StartCarrierYHome_02:
                    if (dbapiSetZ(dbRead, 0) <= 16) {
                        dbapiCarrierY(10, 800*0.2);
                        xeTmrSequense = xe_tmr_sequense.xets_home_CheckCarrierYHome;
                    }
                    break;

                case xe_tmr_sequense.xets_home_CheckCarrierYHome:
                    if (true) {
                        int rslt01 = 0;
                        int axis01 = 0;

                        axis01 = (int)WMX3軸定義.載盤Y軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        if (rslt01 == 1) {
                            if (dbapiSetZ(dbRead, 0) <= 16) {
                                xeTmrSequense = xe_tmr_sequense.xets_home_EndCarrierYHome;
                            }
                        }
                    }
                    break;

                case xe_tmr_sequense.xets_home_EndCarrierYHome:
                case xe_tmr_sequense.xets_home_end:
                    dbapiNozzleR(dbNozzle安全原點R, 36);  Thread.Sleep(10);
                    dbapiGate(0, 580/4);                  Thread.Sleep(10);
                    xeTmrSequense = xe_tmr_sequense.xets_end;
                    break;

                default:
                case xe_tmr_sequense.xets_empty:
                case xe_tmr_sequense.xets_idle:
                case xe_tmr_sequense.xets_end:
                    btn_home.Text = "Home";

                    if(bhome == true) {
                        xeTmrSequense = xe_tmr_sequense.xets_home_start;
                        bhome = false;
                    }
                    break;
            }

        }















        public enum xe_tmr_takepin
        {
            xett_Empty,
                xett_確定執行要取針,
                    xett_關工作門,
                        xett_檢查工作門關閉,
                        xett_確定工作門關閉,

                    xett_取得柔震盤針資訊,
                        xett_柔震盤無針,
                            xett_柔震盤料倉震動,
                                xett_等待柔震盤料倉震動2秒,
                            xett_柔震盤上下震動,
                                xett_等待柔震盤上下震動2秒,
                            xett_柔震盤左右震動,
                                xett_等待柔震盤左右震動2秒,
                            xett_柔震盤散震震動,
                                xett_等待柔震盤散震震動2秒,
                            xett_柔震盤停止,
                                xett_等待柔震停止2秒,
                            xett_檢查柔震盤針資訊,
                            xett_柔震盤無針retry,

                        xett_得到針資訊,
                            xett_縮回Nozzle0到0,
                            xett_檢測NozzleZ到0,
                            xett_判斷NozzleZ到0安全位置,

                            xett_移動NozzleXYR吸料位,
                            xett_檢測NozzleXYR吸料位,
                            xett_判斷NozzleXYR吸料位為安全位置,

                            xett_下降NozzleZ,
                            xett_檢測NozzleZ吸料位,
                            xett_判斷NozzleZ吸料位安全位置,

                            xett_Nozzle吸料開始,
                            xett_Nozzle吸料等待,
                            xett_Nozzle吸料完成,

                            xett_NozzleZ縮回0,
                            xett_NozzleZ檢查是否縮回0,
                            xett_NozzleZ縮為0完成,

                            xett_移至飛拍起始位置,
                            xett_檢測是否在飛拍起始位置,
                            xett_確認在飛拍起始位置,

                            xett_NozzleX以速度250移動來觸發飛拍,
                            xett_檢測是否飛拍移動完成,
                            xett_確定飛拍移動完成,

                            xett_移至吐料位,
                            xett_檢測是否在吐料位,
                            xett_確認在吐料位,

                            /* bTakePin */                                     /* bChambered */
                            xett_NozzleZ下降至吐料高度,                        xett_NozzleXYR移至上膛位,
                            xett_檢查NozzleZ是否在吐料高度,                    xett_檢查NozzleXYR是否移至上膛位,
                            xett_確認NozzleZ在吐料高度,                        xett_確認NozzleXYR移至上膛位,

                            xett_Nozzle吸料停止,                               xett_擺放座蓋板打開,
                            xett_Nozzle吐料開始,                               xett_檢查擺放座蓋板是否打開,  
                                                                               xett_擺放座蓋板打開等待1秒,
                            xett_Nozzle吐料等待,                               xett_確認擺放座蓋板打開,
                            xett_Nozzle吐料完成,
                                                                               xett_擺放座R軸至放料位,
                            xett_NozzleZ退回安全高度0,                         xett_檢查擺放座R軸是否至放料位,
                            xett_檢查NozzleZ是否退回安全高度0,                 xett_確認擺放座R軸至放料位,
                            xett_確定NozzleZ已退回安全高度0,
                                                                               xett_擺放座Z軸至放料位,
                xett_檢測是否還需要取針,                                       xett_檢查擺放座Z軸是否至放料位,
                    xett_還需要取針,                                           xett_確認擺放座Z軸至放料位,
                        xett_重覆一開始的狀態,
                                                                               xett_NozzleZ下降至上膛位,
                    xett_不需要取針,                                           xett_檢查NozzleZ是否下降至上膛位,
                        xett_NozzleXYR移置安全位置,                            xett_確認NozzleZ下降至上膛位,
                        xett_檢查NozzleXYR是否移至安全位置,
                        xett_確認NozzleXYR在安全位置,                          xett_擺放座開真空,
                                                                               xett_擺放座開真空等待1秒,

                                                                               xett_Nozzle吸嘴關真空,
                                                                               xett_Nozzle吸嘴關真空等待1秒,

                xett_取針結束,                                                 xett_吸嘴破真空,
                                                                               xett_吸嘴破真空等待1秒,
                                                                               xett_吸嘴破真空關閉,

                                                                               xett_Nozzle回至0點保護位,
                                                                               xett_檢查Nozzle是否回至0點保護位,
                                                                               xett_確認Nozzle回至0點保護位,

                                                                               //槍管task
                                                                               xett_擺放座R軸至植針位,
                                                                               xett_檢查擺放座R軸是否至植針位,
                                                                               xett_確認擺放座R軸至植針位,

                                                                               xett_擺放座蓋板關閉,
                                                                               xett_檢查擺放座蓋板是否關閉,
                                                                               xett_確認擺放座蓋板關閉,

                                                                               xett_載盤XY移置拍照檢查位,
                                                                               xett_檢查載盤XY是否移置拍照檢查位,
                                                                               xett_確認載盤XY移置拍照檢查位,

                                                                               xett_載盤XY移置直針位,
                                                                               xett_檢查載盤XY是否移置直針位,
                                                                               xett_確認載盤XY移置直針位,

                                                                               xett_擺放座Z軸至植針位,
                                                                               xett_檢查擺放座Z軸是否至植針位,
                                                                               xett_確認擺放座Z軸至植針位,

                                                                               xett_擺放座真空關閉,

                                                                               xett_開啟流量閥1,
                                                                               xett_開啟流量閥1等待1秒,

                                                                               xett_植針吹氣電磁閥開啟,
                                                                               xett_植針吹氣電磁閥開啟等待1秒,

                                                                               xett_植針吹氣電磁閥關閉,

                                                                               xett_擺放座蓋板再次打開,
                                                                               xett_檢查擺放座蓋板是否再次打開,
                                                                               xett_擺放座蓋板再次打開等待1秒,
                                                                               xett_確認擺放座蓋板再次打開,

                                                                               xett_擺放座R軸再次至放料位,
                                                                               xett_檢查擺放座R軸是否再次至放料位,
                                                                               xett_確認擺放座R軸再次至放料位,

                                                                               xett_擺放座Z軸再次至放料位,
                                                                               xett_檢查擺放座Z軸是否再次至放料位,
                                                                               xett_確認擺放座Z軸再次至放料位,

                                                                               xett_載盤XY再次移置拍照檢查位,
                                                                               xett_檢查載盤XY是否再次移置拍照檢查位,
                                                                               xett_確認載盤XY再次移置拍照檢查位,


            xett_End,
        };

        public xe_tmr_takepin xeTmrTakePin = xe_tmr_takepin.xett_Empty;

        public int  iTakePinFinishedCNT1 = 0;



        public int  iTakePinFinishedCNT2 = 0;
        public bool bTakePin             = false;
        public bool bChambered           = false;
        public bool bPause               = false;
        public bool btmrStop             = false;
        public int  itmrStop             = 1;
        public const double db取料Nozzle中心點X = 49.94;
        public const double db取料Nozzle中心點Y = 49.875;
        public const double db取料Nozzle中心點Z = 26;
        public const double db取料Nozzle中心點R = 1.350;

        public const double db下視覺取像X_Start = 105;
        public const double db下視覺取像X_END   = 243.000;
        public const double db下視覺取像Y       = 27.05;
        public const double db下視覺取像Z       = 0;

        public const double db吐料位X           = 243.000;
        public const double db吐料位下降Z高度   = 2.000;

        public DateTime Prev_CycleTime;
        public DateTime Curr_CycleTime;
        public TimeSpan CycleTime;

        private void btn_TakePin_Click(object sender, EventArgs e)
        {
            bTakePin = true;
        }
        private void btn上膛_Click(object sender, EventArgs e)
        {
            bChambered = true;
        }
        private void btn_tmrStop_Click(object sender, EventArgs e)
        {
            btmrStop = true;
        }
        private void btn_tmrPause_Click(object sender, EventArgs e)
        {
            tmr_TakePin.Enabled = bPause;
            bPause = !bPause;
        }
        private void tmr_TakePin_Tick(object sender, EventArgs e)
        {  // start of private void tmr_TakePin_Tick(object sender, EventArgs e)
            

            if (btmrStop == true && int.Parse(txt_取料循環.Text)>=1 ) { 
                itmrStop = int.Parse(txt_取料循環.Text);
                txt_取料循環.Text = "0";
            }

            lblLog.Text = xeTmrTakePin.ToString() + ", 柔震重試:" + iTakePinFinishedCNT2;
            switch (xeTmrTakePin) {
                case xe_tmr_takepin.xett_Empty:  
                    if(bTakePin==true || bChambered==true) {
                        int 求出取料循環次數 = int.Parse(txt_取料循環.Text);
                        if(求出取料循環次數>=1) { 
                            xeTmrTakePin = xe_tmr_takepin.xett_確定執行要取針;  
                        } else {
                            xeTmrTakePin = xe_tmr_takepin.xett_取針結束;
                        }
                    }
                    if(btmrStop == true) {
                        btmrStop = false;
                        txt_取料循環.Text = (itmrStop-1).ToString();
                        itmrStop = 0;
                    }
                    break;

                    case xe_tmr_takepin.xett_確定執行要取針:                               xeTmrTakePin = xe_tmr_takepin.xett_關工作門;  break;
                    case xe_tmr_takepin.xett_關工作門:     
                        dbapiGate(580, 580/4);
                        xeTmrTakePin = xe_tmr_takepin.xett_檢查工作門關閉;
                        break;
                        case xe_tmr_takepin.xett_檢查工作門關閉:   
                            if (true) {
                                int rslt01 = 0;
                                int axis01 = 0;

                                axis01 = (int)WMX3軸定義.工作門;
                                rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                                double iGetPos = dbapiGate(dbRead, 0); ;

                                if (rslt01==1 && 580.0*0.99 <= iGetPos) {
                                    xeTmrTakePin = xe_tmr_takepin.xett_確定工作門關閉;
                                }
                            }
                            break;
                        case xe_tmr_takepin.xett_確定工作門關閉:                                  xeTmrTakePin = xe_tmr_takepin.xett_取得柔震盤針資訊;  break;

                        case xe_tmr_takepin.xett_取得柔震盤針資訊:     
                            btn_取得PinInfo_Click(sender, e); 
                            if(b柔震盤有料_tmrTakePinTick == true) { 
                                //柔震有料
                                xeTmrTakePin = xe_tmr_takepin.xett_得到針資訊;
                            } else { 
                                //柔震無料
                                xeTmrTakePin = xe_tmr_takepin.xett_柔震盤無針;

                                //設定retry次數
                                iTakePinFinishedCNT2 = 3;
                            }
                            break;
                            case xe_tmr_takepin.xett_柔震盤無針:            
                                xeTmrTakePin = xe_tmr_takepin.xett_柔震盤料倉震動;  
                                break;
                                case xe_tmr_takepin.xett_柔震盤料倉震動:
                                    lbl震散.BackColor   = Color.Green; 
                                    lbl上下收.BackColor = Color.Green;   
                                    lbl左右收.BackColor = Color.Green; 
                                    lbl料倉.BackColor   = Color.Red;
                                    btnVibrationInit_Click(sender, e);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待柔震盤料倉震動2秒;
                                    break;
                                    case xe_tmr_takepin.xett_等待柔震盤料倉震動2秒: 
                                        iTakePinFinishedCNT1++;
                                        if(iTakePinFinishedCNT1>=50) { 
                                            iTakePinFinishedCNT1 = 0;
                                            btnVibrationStop_Click(sender, e);
                                            xeTmrTakePin = xe_tmr_takepin.xett_柔震盤上下震動;
                                        }
                                        break;
                                case xe_tmr_takepin.xett_柔震盤上下震動: 
                                    lbl震散.BackColor   = Color.Green; 
                                    lbl上下收.BackColor = Color.Red;   
                                    lbl左右收.BackColor = Color.Green; 
                                    lbl料倉.BackColor   = Color.Green;
                                    btnVibrationInit_Click(sender, e);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待柔震盤上下震動2秒;
                                    break;
                                    case xe_tmr_takepin.xett_等待柔震盤上下震動2秒: 
                                        iTakePinFinishedCNT1++;
                                        if(iTakePinFinishedCNT1>=50) { 
                                            iTakePinFinishedCNT1 = 0;
                                            btnVibrationStop_Click(sender, e);
                                            xeTmrTakePin = xe_tmr_takepin.xett_柔震盤左右震動;
                                        }
                                        break;
                                case xe_tmr_takepin.xett_柔震盤左右震動:
                                    lbl震散.BackColor   = Color.Green; 
                                    lbl上下收.BackColor = Color.Green;   
                                    lbl左右收.BackColor = Color.Red; 
                                    lbl料倉.BackColor   = Color.Green;
                                    btnVibrationInit_Click(sender, e);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待柔震盤左右震動2秒;
                                    break;
                                    case xe_tmr_takepin.xett_等待柔震盤左右震動2秒:
                                        iTakePinFinishedCNT1++;
                                        if(iTakePinFinishedCNT1>=50) { 
                                            iTakePinFinishedCNT1 = 0;
                                            btnVibrationStop_Click(sender, e);
                                            xeTmrTakePin = xe_tmr_takepin.xett_柔震盤散震震動;
                                        }
                                        break;
                                case xe_tmr_takepin.xett_柔震盤散震震動:
                                    lbl震散.BackColor   = Color.Red; 
                                    lbl上下收.BackColor = Color.Green;   
                                    lbl左右收.BackColor = Color.Green; 
                                    lbl料倉.BackColor   = Color.Green;
                                    btnVibrationInit_Click(sender, e);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待柔震盤散震震動2秒;
                                    break;
                                    case xe_tmr_takepin.xett_等待柔震盤散震震動2秒:
                                        iTakePinFinishedCNT1++;
                                        if(iTakePinFinishedCNT1>=50) { 
                                            iTakePinFinishedCNT1 = 0;
                                            xeTmrTakePin = xe_tmr_takepin.xett_柔震盤停止;
                                        }
                                        break;
                                case xe_tmr_takepin.xett_柔震盤停止:
                                    btnVibrationStop_Click(sender, e);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待柔震停止2秒;
                                    break;
                                    case xe_tmr_takepin.xett_等待柔震停止2秒:
                                        iTakePinFinishedCNT1++;
                                        if(iTakePinFinishedCNT1>=50) { 
                                            iTakePinFinishedCNT1 = 0;
                                            xeTmrTakePin = xe_tmr_takepin.xett_檢查柔震盤針資訊;
                                        }
                                        break;
                                case xe_tmr_takepin.xett_檢查柔震盤針資訊:
                                    btn_取得PinInfo_Click(sender, e); 
                                    if(b柔震盤有料_tmrTakePinTick == true) { 
                                        //柔震有料
                                        xeTmrTakePin = xe_tmr_takepin.xett_得到針資訊;
                                    } else { 
                                        //柔震無料
                                        xeTmrTakePin = xe_tmr_takepin.xett_柔震盤無針retry;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_柔震盤無針retry: 
                                    iTakePinFinishedCNT2--;
                                    if(iTakePinFinishedCNT2==0) { 
                                        //設定retry次數
                                        iTakePinFinishedCNT2 = 3;

                                        xeTmrTakePin = xe_tmr_takepin.xett_取針結束;
                                    } else { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_柔震盤無針;
                                    }
                                    break;

                            case xe_tmr_takepin.xett_得到針資訊:                           xeTmrTakePin = xe_tmr_takepin.xett_縮回Nozzle0到0;  break;
                                case xe_tmr_takepin.xett_縮回Nozzle0到0: 
                                    dbapiNozzleZ(0, bTakePin?40*8:40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測NozzleZ到0;
                                    break;
                                case xe_tmr_takepin.xett_檢測NozzleZ到0: {
                                    double dbGetZ_1 = dbapiNozzleZ(dbRead, 0);
                                    if(dbGetZ_1 <= 0.1) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_判斷NozzleZ到0安全位置;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_判斷NozzleZ到0安全位置:           xeTmrTakePin = xe_tmr_takepin.xett_移動NozzleXYR吸料位;  break;

                                case xe_tmr_takepin.xett_移動NozzleXYR吸料位: {
                                    double dbTargetNozzleY = db取料Nozzle中心點Y + dbPinY_tmrTakePinTick;
                                    if(dbTargetNozzleY <= 5 && 95 <= dbTargetNozzleY) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_柔震盤料倉震動;
                                    } else { 
                                        inspector1.下視覺正向 = false;
                                        dbapiNozzleX(db取料Nozzle中心點X + dbPinX_tmrTakePinTick, bTakePin?500*4:500*2);
                                        dbapiNozzleY(db取料Nozzle中心點Y + dbPinY_tmrTakePinTick, bTakePin?100*8:100*4);    
                                        dbapiNozzleR(db取料Nozzle中心點R + dbPinR_tmrTakePinTick, bTakePin?360*8:360*4);
                                        xeTmrTakePin = xe_tmr_takepin.xett_檢測NozzleXYR吸料位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_檢測NozzleXYR吸料位: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbY = dbapiNozzleY(dbRead, 0);
                                    double dbR = dbapiNozzleR(dbRead, 0);

                                    double dbTargetX = db取料Nozzle中心點X + dbPinX_tmrTakePinTick;
                                    double dbTargetY = db取料Nozzle中心點Y + dbPinY_tmrTakePinTick;
                                    double dbTargetR = db取料Nozzle中心點R + dbPinR_tmrTakePinTick;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) //&&
                                        //( (dbTargetR*0.99<=dbR && dbR<=dbTargetR*1.01) || (dbTargetR*1.01<=dbR && dbR<=dbTargetR*0.99) ) 
                                      ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_判斷NozzleXYR吸料位為安全位置;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_判斷NozzleXYR吸料位為安全位置:    xeTmrTakePin = xe_tmr_takepin.xett_下降NozzleZ;  break;

                                case xe_tmr_takepin.xett_下降NozzleZ: 
                                    dbapiNozzleZ(db取料Nozzle中心點Z, bTakePin?40*8:40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測NozzleZ吸料位;
                                    break;
                                case xe_tmr_takepin.xett_檢測NozzleZ吸料位: {
                                    double dbZ = dbapiNozzleZ(dbRead, 0);
                                    double dbTargetZ = db取料Nozzle中心點Z;
                                    if( (dbTargetZ*0.99<= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_判斷NozzleZ吸料位安全位置;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_判斷NozzleZ吸料位安全位置: 
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料開始;
                                    break;

                                case xe_tmr_takepin.xett_Nozzle吸料開始: 
                                    if(bTakePin == true) { 
                                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10, 1);
                                    }
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料等待;
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吸料等待:                   xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料完成;  break;
                                case xe_tmr_takepin.xett_Nozzle吸料完成:                   xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ縮回0;    break;

                                case xe_tmr_takepin.xett_NozzleZ縮回0: 
                                    dbapiNozzleZ(0, bTakePin?40*8:40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ檢查是否縮回0;
                                    break;
                                case xe_tmr_takepin.xett_NozzleZ檢查是否縮回0: 
                                    double dbGetZ = dbapiNozzleZ(dbRead, 0);
                                    if(dbGetZ<=0.1) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ縮為0完成;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_NozzleZ縮為0完成:                 xeTmrTakePin = xe_tmr_takepin.xett_移至飛拍起始位置;  break;

                                case xe_tmr_takepin.xett_移至飛拍起始位置:
                                    dbapiNozzleY(db下視覺取像Y,          bTakePin?100*8:100*4);
                                    dbapiNozzleX(db下視覺取像X_Start,    bTakePin?500*4:500*2);
                                    dbapiNozzleZ(db下視覺取像Z,          bTakePin? 40*8: 40*4);
                                    dbapiNozzleR(db取料Nozzle中心點R+90, bTakePin?360*8:360*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測是否在飛拍起始位置;
                                    break;
                                case xe_tmr_takepin.xett_檢測是否在飛拍起始位置: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbY = dbapiNozzleY(dbRead, 0);
                                    double dbZ = dbapiNozzleZ(dbRead, 0);
                                    double dbR = dbapiNozzleR(dbRead, 0);

                                    double dbTargetX = db下視覺取像X_Start;
                                    double dbTargetY = db下視覺取像Y;
                                    double dbTargetZ = db下視覺取像Z;
                                    double dbTargetR = db取料Nozzle中心點R + 90;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) &&
                                        (                        dbZ <= 0.1           ) &&
                                        ( (dbTargetR*0.99<= dbR && dbR <= dbTargetR*1.01) || (dbTargetR*1.01<=dbR && dbR<=dbTargetR*0.99) ) 
                                      ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認在飛拍起始位置;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認在飛拍起始位置:                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleX以速度250移動來觸發飛拍;  break;

                                case xe_tmr_takepin.xett_NozzleX以速度250移動來觸發飛拍: 
                                    inspector1.下視覺正向 = true;
                                    dbapiNozzleX(db下視覺取像X_END, 250);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測是否飛拍移動完成;
                                    break;
                                case xe_tmr_takepin.xett_檢測是否飛拍移動完成: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbTargetX = db下視覺取像X_END;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確定飛拍移動完成;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確定飛拍移動完成:                 xeTmrTakePin = xe_tmr_takepin.xett_移至吐料位;  break;

                                case xe_tmr_takepin.xett_移至吐料位: 
                                    dbapiNozzleX(db吐料位X, bTakePin?500*4:500*2);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測是否在吐料位;
                                    break;
                                case xe_tmr_takepin.xett_檢測是否在吐料位: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbTargetX = db吐料位X;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認在吐料位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認在吐料位:                           
                                    if(bTakePin == true) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ下降至吐料高度; 
                                    } else if(bChambered == true) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleXYR移至上膛位;
                                    }
                                    break;

                                /* bTakePin */
                                case xe_tmr_takepin.xett_NozzleZ下降至吐料高度:
                                    dbapiNozzleZ(db吐料位下降Z高度, bTakePin?40*8:40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleZ是否在吐料高度;
                                    break;
                                case xe_tmr_takepin.xett_檢查NozzleZ是否在吐料高度: {
                                    double dbGetZ_2 = dbapiNozzleZ(dbRead, 0);
                                    double dbTargetZ = db吐料位下降Z高度;
                                    if( (dbTargetZ*0.99<= dbGetZ_2 && dbGetZ_2 <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleZ在吐料高度;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認NozzleZ在吐料高度:            xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料停止;  break;

                                case xe_tmr_takepin.xett_Nozzle吸料停止:
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10,     (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10,     0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吐料開始;
                                    break;

                                case xe_tmr_takepin.xett_Nozzle吐料開始: 
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吐料等待;
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吐料等待:            
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=30) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吐料完成; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吐料完成:              
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)%10, 0);
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10,     (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10,     1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ退回安全高度0;  
                                    break;

                                case xe_tmr_takepin.xett_NozzleZ退回安全高度0: 
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10,     (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10,     0);
                                    dbapiNozzleZ(0, bTakePin?40*8:40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleZ是否退回安全高度0;
                                    break;
                                case xe_tmr_takepin.xett_檢查NozzleZ是否退回安全高度0: {
                                    double dbGetZ_3 = dbapiNozzleZ(dbRead, 0);
                                    if(dbGetZ_3 <= 0.1) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確定NozzleZ已退回安全高度0;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確定NozzleZ已退回安全高度0:                xeTmrTakePin = xe_tmr_takepin.xett_檢測是否還需要取針;  break;
                    //-----------------------------------------------------------------------------------------------------------------------------------------------
                                /* bChambered */
                                case xe_tmr_takepin.xett_NozzleXYR移至上膛位:     
                                    dbapiNozzleX(495,   500*2);
                                    dbapiNozzleY(77.05, 100*4);
                                    dbapiNozzleR(91.35, 360*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleXYR是否移至上膛位;  break;
                                    break;
                                case xe_tmr_takepin.xett_檢查NozzleXYR是否移至上膛位: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbY = dbapiNozzleY(dbRead, 0);
                                    double dbR = dbapiNozzleR(dbRead, 0);

                                    double dbTargetX = 495;
                                    double dbTargetY = 77.05;
                                    double dbTargetR = 91.35;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) &&
                                        ( (dbTargetR*0.99<= dbR && dbR <= dbTargetR*1.01) || (dbTargetR*1.01<=dbR && dbR<=dbTargetR*0.99) ) 
                                        ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleXYR移至上膛位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認NozzleXYR移至上膛位:                             xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板打開;  break;

                                case xe_tmr_takepin.xett_擺放座蓋板打開:      
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)/10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座蓋板是否打開;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座蓋板是否打開:                
                                    int 擺放座蓋板打開是否為0 = pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板) % 10);
                                    if(擺放座蓋板打開是否為0 == 0) { 
                                        //已打開
                                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板打開等待1秒;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_擺放座蓋板打開等待1秒:
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=20) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座蓋板打開; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認擺放座蓋板打開:                                    xeTmrTakePin = xe_tmr_takepin.xett_擺放座R軸至放料位;  break;
                             
                                case xe_tmr_takepin.xett_擺放座R軸至放料位:  
                                    dbapiSetR(268.08, 360);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座R軸是否至放料位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座R軸是否至放料位: {
                                    double dbR = dbapiSetR(dbRead, 0);
                                    double dbTargetR = 268.08;
                                    if( (dbTargetR * 0.99 <= dbR && dbR <= dbTargetR * 1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座R軸至放料位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座R軸至放料位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸至放料位;  break;
                            
                                case xe_tmr_takepin.xett_擺放座Z軸至放料位:                      
                                    dbapiSetZ(12, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座Z軸是否至放料位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座Z軸是否至放料位: {
                                    double dbZ = dbapiSetZ(dbRead, 0);
                                    double dbTargetZ = 12;
                                    if( (dbTargetZ*0.99<= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座Z軸至放料位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座Z軸至放料位:                                  xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ下降至上膛位;  break;
                                         
                                case xe_tmr_takepin.xett_NozzleZ下降至上膛位:      
                                    dbapiNozzleZ(36.72, 40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleZ是否下降至上膛位;
                                    break;
                                case xe_tmr_takepin.xett_檢查NozzleZ是否下降至上膛位:  {
                                    double dbZ = dbapiNozzleZ(dbRead, 0);
                                    double dbTargetZ = 36.72;
                                    if( (dbTargetZ*0.99<= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleZ下降至上膛位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認NozzleZ下降至上膛位:                              xeTmrTakePin = xe_tmr_takepin.xett_擺放座開真空;  break;
                              
                                case xe_tmr_takepin.xett_擺放座開真空:    
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座吸真空)/10, (int)(WMX3IO對照.pxeIO_擺放座吸真空)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_擺放座開真空等待1秒;
                                    break;
                                case xe_tmr_takepin.xett_擺放座開真空等待1秒:                    
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=20) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸嘴關真空; 
                                    }
                                    break;

                                case xe_tmr_takepin.xett_Nozzle吸嘴關真空:              
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸嘴關真空等待1秒; 
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吸嘴關真空等待1秒: 
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=10) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_吸嘴破真空; 
                                    }
                                    break;

                                case xe_tmr_takepin.xett_吸嘴破真空:     
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_吸嘴破真空等待1秒; 
                                    break;
                                case xe_tmr_takepin.xett_吸嘴破真空等待1秒:
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=30) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_吸嘴破真空關閉; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_吸嘴破真空關閉:       
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle回至0點保護位; 
                                    break;

                                case xe_tmr_takepin.xett_Nozzle回至0點保護位:                    
                                    dbapiNozzleZ(0, 40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查Nozzle是否回至0點保護位;
                                    break;
                                case xe_tmr_takepin.xett_檢查Nozzle是否回至0點保護位: {
                                    double dbGetZ_1 = dbapiNozzleZ(dbRead, 0);
                                    if(dbGetZ_1 <= 0.1) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認Nozzle回至0點保護位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認Nozzle回至0點保護位:                              xeTmrTakePin = xe_tmr_takepin.xett_擺放座R軸至植針位;  break;

                                //槍管task
                                case xe_tmr_takepin.xett_擺放座R軸至植針位:    
                                    dbapiSetR(178.08, 360);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座R軸是否至植針位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座R軸是否至植針位: {
                                    double dbR = dbapiSetR(dbRead, 0);
                                    double dbTargetR = 178.08;
                                    if( (dbTargetR * 0.99 <= dbR && dbR <= dbTargetR * 1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座R軸至植針位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座R軸至植針位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板關閉;  break;

                                case xe_tmr_takepin.xett_擺放座蓋板關閉:           
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)/10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座蓋板是否關閉;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座蓋板是否關閉:   
                                    int 擺放座蓋板閉合是否為4 = pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板合) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板合) % 10);
                                    if (擺放座蓋板閉合是否為4 == 4) { 
                                        //已閉合
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座蓋板關閉;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認擺放座蓋板關閉:                                    xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置拍照檢查位;  break;

                                case xe_tmr_takepin.xett_載盤XY移置拍照檢查位:     
                                    dbapiCarrierX(134.511, 190*0.8);
                                    dbapiCarrierY(606.255, 800*0.8);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置拍照檢查位;
                                    break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否移置拍照檢查位: {
                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = 134.511;
                                    double dbTargetY = 606.255;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置拍照檢查位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認載盤XY移置拍照檢查位:                                xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置直針位;  break;

                                case xe_tmr_takepin.xett_載盤XY移置直針位:     
                                    dbapiCarrierX(125.666, 190*0.8);
                                    dbapiCarrierY(662.417, 800*0.8);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置直針位;
                                    break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否移置直針位: {
                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = 125.666;
                                    double dbTargetY = 662.417;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置直針位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認載盤XY移置直針位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸至植針位;  break;

                                case xe_tmr_takepin.xett_擺放座Z軸至植針位:   
                                    dbapiSetZ(21.000, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座Z軸是否至植針位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座Z軸是否至植針位: {
                                    double dbZ = dbapiSetZ(dbRead, 0);
                                    double dbTargetZ = 21.000;
                                    if( (dbTargetZ*0.99<= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座Z軸至植針位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座Z軸至植針位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座真空關閉;  break;

                                case xe_tmr_takepin.xett_擺放座真空關閉:    
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座吸真空)/10, (int)(WMX3IO對照.pxeIO_擺放座吸真空)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_開啟流量閥1;
                                    break;

                                case xe_tmr_takepin.xett_開啟流量閥1:
                                    vcb_植針吹氣流量閥.Value = 100-99;
                                    ScrollEventArgs xe = null;
                                    vScrollBar1_Scroll(sender, xe);
                                    xeTmrTakePin = xe_tmr_takepin.xett_開啟流量閥1等待1秒;
                                    break;
                                case xe_tmr_takepin.xett_開啟流量閥1等待1秒:  
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=20) { 
                                        iTakePinFinishedCNT2 = 0;

                                        vcb_植針吹氣流量閥.Value = 100-0;
                                        ScrollEventArgs xxe = null;
                                        vScrollBar1_Scroll(sender, xxe);

                                        xeTmrTakePin = xe_tmr_takepin.xett_植針吹氣電磁閥開啟; 
                                    }
                                    break;

                                case xe_tmr_takepin.xett_植針吹氣電磁閥開啟:       
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)/10, (int)(WMX3IO對照.pxeIO_植針吹氣)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_植針吹氣電磁閥開啟等待1秒; 
                                    break;
                                case xe_tmr_takepin.xett_植針吹氣電磁閥開啟等待1秒:  
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=20) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_植針吹氣電磁閥關閉; 
                                    }
                                    break;

                                case xe_tmr_takepin.xett_植針吹氣電磁閥關閉:       
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)/10, (int)(WMX3IO對照.pxeIO_植針吹氣)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板再次打開; 
                                    break;

                                case xe_tmr_takepin.xett_擺放座蓋板再次打開:      
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)/10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座蓋板是否再次打開;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座蓋板是否再次打開:                
                                    int 擺放座蓋板再次打開是否為0 = pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板) % 10);
                                    if(擺放座蓋板再次打開是否為0 == 0) { 
                                        //已打開
                                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板再次打開等待1秒;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_擺放座蓋板再次打開等待1秒:
                                    iTakePinFinishedCNT2++;
                                    if(iTakePinFinishedCNT2>=20) { 
                                        iTakePinFinishedCNT2 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座蓋板再次打開; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認擺放座蓋板再次打開:                      xeTmrTakePin = xe_tmr_takepin.xett_擺放座R軸再次至放料位;  break;

                                case xe_tmr_takepin.xett_擺放座R軸再次至放料位:   
                                    dbapiSetR(268.08, 360);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座R軸是否再次至放料位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座R軸是否再次至放料位: {
                                    double dbR = dbapiSetR(dbRead, 0);
                                    double dbTargetR = 268.08;
                                    if( (dbTargetR * 0.99 <= dbR && dbR <= dbTargetR * 1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座R軸再次至放料位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座R軸再次至放料位:                    xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸再次至放料位;  break;

                                case xe_tmr_takepin.xett_擺放座Z軸再次至放料位:      
                                    dbapiSetZ(12, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座Z軸是否再次至放料位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座Z軸是否再次至放料位:  {
                                    double dbZ = dbapiSetZ(dbRead, 0);
                                    double dbTargetZ = 12;
                                    if( (dbTargetZ*0.99<= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座Z軸再次至放料位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座Z軸再次至放料位:                    xeTmrTakePin = xe_tmr_takepin.xett_載盤XY再次移置拍照檢查位;  break;

                                case xe_tmr_takepin.xett_載盤XY再次移置拍照檢查位:      
                                    dbapiCarrierX(134.511, 190*0.8);
                                    dbapiCarrierY(606.255, 800*0.8);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否再次移置拍照檢查位;
                                    break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否再次移置拍照檢查位: {
                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = 134.511;
                                    double dbTargetY = 606.255;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY再次移置拍照檢查位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認載盤XY再次移置拍照檢查位:                  xeTmrTakePin = xe_tmr_takepin.xett_檢測是否還需要取針;  break;
                    //-----------------------------------------------------------------------------------------------------------------------------------------------
                    case xe_tmr_takepin.xett_檢測是否還需要取針: {    
                        int 求出取料循環次數 = int.Parse(txt_取料循環.Text);
                        求出取料循環次數--;
                        txt_取料循環.Text = 求出取料循環次數.ToString();
                        if(求出取料循環次數>=1) { 
                            xeTmrTakePin = xe_tmr_takepin.xett_還需要取針;  
                        } else {
                            xeTmrTakePin = xe_tmr_takepin.xett_不需要取針;
                        }
                    } break;
                        case xe_tmr_takepin.xett_還需要取針:
                            Curr_CycleTime = DateTime.Now; //20241230 4xuan added
                            CycleTime = Curr_CycleTime - Prev_CycleTime;
                            Prev_CycleTime = DateTime.Now;

                            lbl_CycleTime.Text = "循環時間 : " + CycleTime.ToString(@"ss\.fff");
                    xeTmrTakePin = xe_tmr_takepin.xett_重覆一開始的狀態;  break;

                            case xe_tmr_takepin.xett_重覆一開始的狀態:                     xeTmrTakePin = xe_tmr_takepin.xett_確定執行要取針;  break;

                        case xe_tmr_takepin.xett_不需要取針:                               xeTmrTakePin = xe_tmr_takepin.xett_NozzleXYR移置安全位置;  break;
                            case xe_tmr_takepin.xett_NozzleXYR移置安全位置: 
                                dbapiNozzleZ(0,                  40*8);  
                                dbapiNozzleX(dbNozzle安全原點X, 500*1);  
                                dbapiNozzleY(dbNozzle安全原點Y, 100*1);      
                                dbapiNozzleR(dbNozzle安全原點R, 360*8);      
                                xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleXYR是否移至安全位置;
                                break;
                            case xe_tmr_takepin.xett_檢查NozzleXYR是否移至安全位置: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbY = dbapiNozzleY(dbRead, 0);
                                    double dbZ = dbapiNozzleZ(dbRead, 0);
                                    double dbR = dbapiNozzleR(dbRead, 0);

                                    double dbTargetX = dbNozzle安全原點X;
                                    double dbTargetY = dbNozzle安全原點Y;
                                    double dbTargetZ = 0;
                                    double dbTargetR = dbNozzle安全原點R;
                                    if( (dbTargetX*0.99<= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99<= dbY && dbY <= dbTargetY*1.01) &&
                                        (                        dbZ <= 0.1           ) &&
                                        ( (dbTargetR*0.99<= dbR && dbR <= dbTargetR*1.01) || (dbTargetR*1.01<=dbR && dbR<=dbTargetR*0.99) ) 
                                      ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleXYR在安全位置;
                                    }
                            } break;
                            case xe_tmr_takepin.xett_確認NozzleXYR在安全位置:              xeTmrTakePin = xe_tmr_takepin.xett_取針結束;  break;

                    case xe_tmr_takepin.xett_取針結束:
                        

                        bTakePin   = false; 
                        bChambered = false;
                        xeTmrTakePin = xe_tmr_takepin.xett_Empty;  
                        break;
                case xe_tmr_takepin.xett_End:           
                    xeTmrTakePin = xe_tmr_takepin.xett_Empty;  
                    break;
            }
        }  // end of private void tmr_TakePin_Tick(object sender, EventArgs e)




        private void btn_Manual_Click(object sender, EventArgs e)
        {
            frm_Manual.Show();
        }















        private void button1_Click(object sender, EventArgs e)
        {
            //inspector1.xInit();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            inspector1.SaveRecipe(8);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            inspector1.LoadRecipe(8);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Vector3 pos;
            bool success = inspector1.xInspSocket(out pos);
            label6.Text = string.Format("Socket 偵測 {0} 中心偏移 = {1:F3} , {2:F3}", success, pos.X, pos.Y);
            success = inspector1.xInspSocket植針後檢查();
            label7.Text = (success) ? "植針後檢查 OK" : "植針後檢查 NG";
        }


        private void inspector1_Load(object sender, EventArgs e)
        {

        }





    }  // end of public partial class Form1 : Form

}  // end of namespace InjectorInspector
