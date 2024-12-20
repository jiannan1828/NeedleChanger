
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
//using System.Text.Json;

//---------------------------------------------------------------------------------------
namespace InjectorInspector
{
    //---------------------------------------------------------------------------------------
    public partial class Form1 : Form
    {
        //---------------------------------------------------------------------------------------
        //Debug config
        bool bshow_debug_RAW_Conver_Back_Value = true;
        
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
        }
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
        //------------------------------ Test function with Vision ------------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //------------------------ Xavier Call, Control the Servo machine -----------------------
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleX(double dbIncreaseNozzleX)  //NozzleX
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleX == dbRead) {

            return dbRstNozzleX;
        }  // end of public double dbapiNozzleX(double dbIncreaseNozzleX)  //NozzleX
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleY(double dbIncreaseNozzleY)  //NozzleY
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleY == dbRead) {

            return dbRstNozzleY;
        }  // end of public double dbapiNozzleY(double dbIncreaseNozzleY)  //NozzleY
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleZ(double dbIncreaseNozzleZ)  //NozzleZ
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleZ == dbRead) {

            return dbRstNozzleZ;
        }  // end of public double dbapiNozzleZ(double dbIncreaseNozzleZ)  //NozzleZ
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleR(double dbIncreaseNozzleR)  //NozzleR
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
                int rslt        = 0;
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
                if( Mindb<=dbIncreaseNozzleR && dbIncreaseNozzleR<=Maxdb ) {

                } else if( dbIncreaseNozzleR<=Mindb ) {
                    dbIncreaseNozzleR = (int)Mindb;
                } else if( Maxdb<= dbIncreaseNozzleR) {
                    dbIncreaseNozzleR = (int)Maxdb;
                }

                // 取得欲變更的的浮點數
                int fChangeNozzleR = calculate.Map(dbIncreaseNozzleR, Maxdb, Mindb, MaxRAW, MinRAW);

                //執行旋轉吸嘴
                int axis     = (int)WMX3軸定義.吸嘴R軸;
                int position = fChangeNozzleR;
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseNozzleR == dbRead) {

            return dbRstNozzleR;
        }  // end of public double dbapiNozzleR(double dbIncreaseNozzleR)  //NozzleR
        //---------------------------------------------------------------------------------------
        public double dbapiCarrierX(double dbIncreaseCarrierX)  //CarrierX
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  19000;
                const int    MinRAW =      0;
                const double Maxdb  =  190.0;
                const double Mindb  =    0.0;
                const double Sum    =  19000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstCarrierX = 0.0;

            {  // start of 載盤X軸 讀取與顯示
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseCarrierX == dbRead) {

            return dbRstCarrierX;
        }  // end of public double dbapiCarrierX(double dbIncreaseCarrierX)  //CarrierX
        //---------------------------------------------------------------------------------------
        public double dbapiCarrierY(double dbIncreaseCarrierY)  //CarrierY
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseCarrierY == dbRead) {

            return dbRstCarrierY;
        }  // end of public double dbapiCarrierY(double dbIncreaseCarrierY)  //CarrierY
        //---------------------------------------------------------------------------------------
        public double dbapiSetZ(double dbIncreaseSetZ)  //SetZ
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseSetZ == dbRead) {

            return dbRstSetZ;
        }  // end of public double dbapiSetZ(double dbIncreaseSetZ)  //SetZ
        //---------------------------------------------------------------------------------------
        public double dbapiSetR(double dbIncreaseSetR)  //SetR
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
                int accel    = speed * 2;
                int daccel   = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);

            }  // end of if (dbIncreaseSetR == dbRead) {

            return dbRstSetR;
        }  // end of public double dbapiSetR(double dbIncreaseSetR)  //SetR
        //---------------------------------------------------------------------------------------
        public double dbapiGate(double dbIncreaseGate)  //Gate
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
                int rslt        = 0;
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
                int speed    = (int)((Maxdb/10) *dbSpdF);
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

                } else if( dbIncreaseGate<= Mindb) {
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

                } else if( dbIncreaseGate<= Mindb) {
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
            const int MaxRAW = 3000;
            const int MinRAW = 0;
            const double Maxdb = 30.0;
            const double Mindb = 0.0;
            const double Sum = 3000;
            const double dbSpdF = Sum / Maxdb;

            double dbRstJoDell吸針嘴 = 0.0;

            {  // start of JoDell吸針嘴 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 JoDell吸針嘴 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice = (int)(addr_JODELL.pxeaJ_吸針嘴_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++)
                {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //當數值有效
                if (true)
                {
                    lbl_JoDell吸針嘴_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell吸針嘴_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                    lbl_JoDell吸針嘴_Back.Visible = bshow_debug_RAW_Conver_Back_Value;


                    //得到原始數值
                    int Convert = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    lbl_JoDell吸針嘴_RAW.Text = Convert.ToString();

                    //得到轉換數值
                    double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed = Speed / dbSpdF;
                    lbl_JoDell吸針嘴_Convert.Text = dbGet.ToString("F3");

                    //轉回原始數值
                    int cnback = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    lbl_JoDell吸針嘴_Back.Text = cnback.ToString();


                    //顯示讀取長度
                    dbRstJoDell吸針嘴 = dbGet;
                    lbl_acpos_JoDell吸針嘴.Text = dbRstJoDell吸針嘴.ToString("F3");

                    //顯示運動速度
                    lbl_spd_JoDell吸針嘴.Text = dbSpeed.ToString("F3");
                }

                //變更顏色
                if (rslt == 4)
                {
                    select_JoDell吸針嘴.BackColor = Color.Red;
                    lbl_acpos_JoDell吸針嘴.BackColor = Color.White;
                    lbl_spd_JoDell吸針嘴.BackColor = Color.White;
                }
                else
                {
                    select_JoDell吸針嘴.BackColor = Color.Green;
                    lbl_acpos_JoDell吸針嘴.BackColor = Color.Gray;
                    lbl_spd_JoDell吸針嘴.BackColor = Color.Gray;
                }

            }  // end of JoDell吸針嘴 讀取與顯示

            if (dbIncreaseGate == dbRead)
            {

            }
            else
            {  //吸針嘴 變更位置
                //伸長量overflow保護
                if (Mindb <= dbIncreaseGate && dbIncreaseGate <= Maxdb)
                {

                }
                else if (dbIncreaseGate <= Mindb)
                {
                    dbIncreaseGate = (int)Mindb;
                }
                else if (Maxdb <= dbIncreaseGate)
                {
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

                } else if( dbIncreaseGate<= Mindb) {
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
            int iAimToPageIndex = 4;
            tabControl1.SelectedTab = tabControl1.TabPages[iAimToPageIndex - 1];
            tabControl1.TabPages[0].Text = "Image";
            tabControl1.TabPages[2].Text = "Jog";
        }
        //---------------------------------------------------------------------------------------
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();
            
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

                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, (enGC_IAI)? 1.0:0.0);
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

            if (ptrBtn == btn_plus_1) {
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
                            dbapiNozzleX(result);
                        }
                        break;
                    case WMX3軸定義.吸嘴Y軸:
                        if(enGC_吸嘴Y軸 == true) {
                            dbapiNozzleY(result);
                        }
                        break;
                    case WMX3軸定義.吸嘴Z軸:
                        if(enGC_吸嘴Z軸 == true) {
                            dbapiNozzleZ(result);
                        }
                        break;
                    case WMX3軸定義.吸嘴R軸:
                        if(enGC_吸嘴R軸 == true) {
                            dbapiNozzleR(result);
                        }
                        break;
                    case WMX3軸定義.載盤X軸:
                        if(enGC_載盤X軸 == true) {
                            dbapiCarrierX(result);
                        }
                        break;
                    case WMX3軸定義.載盤Y軸:
                        if(enGC_載盤Y軸 == true) {
                            dbapiCarrierY(result);
                        }
                        break;
                    case WMX3軸定義.植針Z軸:
                        if(enGC_植針Z軸 == true) {
                            dbapiSetZ(result);
                        }
                        break;
                    case WMX3軸定義.植針R軸:
                        if(enGC_植針R軸 == true) {
                            dbapiSetR(result);
                        }
                        break;
                    case WMX3軸定義.工作門:
                        if(enGC_工作門 == true) {
                            dbapiGate(result);
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
                dbapiNozzleX(dbState);
                dbapiNozzleY(dbState);
                dbapiNozzleZ(dbState);
                dbapiNozzleR(dbState);

                dbapiCarrierX(dbState);
                dbapiCarrierY(dbState);

                dbapiSetZ(dbState);
                dbapiSetR(dbState);

                dbapiGate(dbState);

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
















        /// <summary>
        /// Reserve function
        /// </summary>
        /// 
        private void button1_Click(object sender, EventArgs e)
        {
            //inspector1.xInit();
        }






        



        public void btnVibrationInit_Click(object sender, EventArgs e) {
            //Vibration
            clsVibration.apiEstablishTCPVibration();
            {
                clsVibration.u32Frequency = 500;
                clsVibration.u32VibrationSource1_StartPhase =  600; clsVibration.u32VibrationSource1_StopPhase = 1000; clsVibration.u32VibrationSource1_Power = 500;
                clsVibration.u32VibrationSource2_StartPhase = 1000; clsVibration.u32VibrationSource2_StopPhase =  600; clsVibration.u32VibrationSource2_Power = 500;
                clsVibration.u32VibrationSource3_StartPhase =  600; clsVibration.u32VibrationSource3_StopPhase = 1000; clsVibration.u32VibrationSource3_Power = 500;
                clsVibration.u32VibrationSource4_StartPhase = 1000; clsVibration.u32VibrationSource4_StopPhase =  600; clsVibration.u32VibrationSource4_Power = 500;
                clsVibration.u32BlackDepotSource_StartPhase =  400; clsVibration.u32BlackDepotSource_StopPhase =  600; clsVibration.u32BlackDepotSource_Power = 200;
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

        private void btnVibrationLED_Click(object sender, EventArgs e)
        {
            //Vibration LED
            clsVibration.apiEstablishTCPVibration();
            {
                clsVibration.u32LED_Level = 50;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }

        private void btnVibrationLEDOff_Click(object sender, EventArgs e)
        {
            //Vibration LED
            clsVibration.apiEstablishTCPVibration();
            {
                clsVibration.u32LED_Level = 0;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

            /*
                y=0.000454x−2.5071
                x=(y+2.5071)/0.000454
            */

            
            if (100.0<=vScrollBar1.Value) {
                vScrollBar1.Value = 100;
            }
            if(vScrollBar1.Value <= 0.0) {
                vScrollBar1.Value = 0;
            }

            double y = (double)( (double)(vScrollBar1.Value) / (double)(10.0) );
            double x = (y + 2.5071) / 0.000454;

            int iGetValue = (int)x;
            textBox1.Text = iGetValue.ToString();
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

            this.Text = "In:"                   + " " +
                        iGetIn0Value.ToString() + " " + 
                        iGetIn1Value.ToString() + " " + 
                        iGetIn2Value.ToString() + " " + 
                        iGetIn3Value.ToString() + " " +
                        "Out:"                  + " " +
                        iGetValue.ToString()    + " " +
                        "y:" + y.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            /*
            if (int.TryParse(textBox1.Text, out int iGetValue))
            {
                // 設定 vScrollBar1 的值
                vScrollBar1.Value = iGetValue;

                // 手動觸發 Scroll 事件處理器
                ScrollEventArgs scrollEventArgs = new ScrollEventArgs(
                    ScrollEventType.EndScroll,  // 假設您是結束滾動
                    vScrollBar1.Value,          // 設置的值
                    vScrollBar1.Value           // 現在的值
                );
                vScrollBar1_Scroll(sender, scrollEventArgs);
            }
            */
        }
    }  // end of public partial class Form1 : Form

}  // end of namespace InjectorInspector
