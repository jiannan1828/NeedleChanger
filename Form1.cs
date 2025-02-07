
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
//4p Transform
using static InjectorInspector.Normal;

//---------------------------------------------------------------------------------------
//wmx3
using WMX3ApiCLR;
using static System.Windows.Forms.AxHost;
using System.Xml.Linq;

//---------------------------------------------------------------------------------------
//vision
using static InjectorInspector.Form1;

//---------------------------------------------------------------------------------------
//JSON
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

//---------------------------------------------------------------------------------------
//小佛
using static InjectorInspector.Viewer;
using static System.Runtime.CompilerServices.RuntimeHelpers;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;  //會自動長出 要手動刪掉
using System.Runtime.InteropServices;

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
        public bool dbNozzleDegInverse = false;
        public void apiCallBackTest()
        {
            cntcallback++;
            this.Text = cntcallback.ToString() + "  " + inspector1.InspNozzle.CCD.GrabCount.ToString();

            dbNozzleDegInverse = false;
            if (inspector1.InspectOK == true && inspector1.Inspected == true) {
                label10.Text = inspector1.PinDeg.ToString();
                if(inspector1.PinDeg < 0) {
                    dbNozzleDegInverse = true;
                }
                inspector1.InspectOK = false;
            }

        }
        public void button2_Click(object sender, EventArgs e)
        {
            inspector1.SaveRecipe(8);
        }
        //---------------------------------------------------------------------------------------
        public void button5_Click(object sender, EventArgs e)
        {
            inspector1.LoadRecipe(8);
        }
        //---------------------------------------------------------------------------------------
        bool   b有看到校正孔         = false;
        double dbCameraCalibrationX = 0.0;
        double dbCameraCalibrationY = 0.0;
        public void button3_Click(object sender, EventArgs e)
        {
            //植針孔位置校正攝影機取像
            Inspector.Vector3 pos;
            bool success = inspector1.xInspSocket(out pos);
            b有看到校正孔 = success;
            dbCameraCalibrationX = pos.X;
            dbCameraCalibrationY = pos.Y;
            label6.Text = string.Format("Socket 偵測 {0} 中心偏移 = {1:F3} , {2:F3}", success, pos.X, pos.Y);

            //取得校正攝影機校正參數
            success      = inspector1.xInspSocket植針後檢查();
            label7.Text  = (success) ? "植針後檢查 OK" : "植針後檢查 NG";
        }
        //---------------------------------------------------------------------------------------
        public void tB_PointAB_Calculate(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox CalculateDegreePoint = sender as System.Windows.Forms.TextBox;

            const double PI = 3.14159265358979323846;
            double Ax, Ay, Bx, By;

            // 從文字框中取得點的坐標
            Ax = Double.Parse(tB_Ax.Text);
            Ay = Double.Parse(tB_Ay.Text);
            Bx = Double.Parse(tB_Bx.Text);
            By = Double.Parse(tB_By.Text);

            // 計算斜率
            double rise = By - Ay;   // 垂直變化
            double run  = Bx - Ax;   // 水平變化

            // 計算中心
            double Cx = (Ax+Bx)/2,
                   Cy = (Ay+By)/2;

            // 確保 run 不為 0，避免除以零的錯誤
            if (run == 0) {
                if (rise > 0) {
                    lbl_計算角度.Text = "夾角為 90 度(垂直向上)";
                } else if (rise < 0) {
                    lbl_計算角度.Text = "夾角為 270 度(垂直向下)";
                } else {
                    lbl_計算角度.Text = "兩點相同，無法計算夾角";
                }
            } else {
                // 使用 Math.Atan2 計算角度，這樣可以處理所有象限的情況
                double angle_radians = Math.Atan2(rise, run);

                // 將角度從弧度轉換為度數
                double angle_degrees = angle_radians * (180 / PI);

                // 確保角度在-180度~180度內
                while (180 <= angle_degrees) {
                    angle_degrees = angle_degrees - 360;
                }
                while(angle_degrees <= -180) {
                    angle_degrees = angle_degrees + 360;
                }

                // 顯示夾角
                lbl_計算角度.Text = string.Format("Cx:{1}, Cy:{2}, 夾角: {0:F2} 度", angle_degrees, Cx, Cy);
            }
        }
        //---------------------------------------------------------------------------------------
        public void btn_ToPointAB(object sender, EventArgs e)
        {
            System.Windows.Forms.Button SetToPoint = sender as System.Windows.Forms.Button;

            //吸料盤校正用
            PointF pos = new PointF(0, 0);  // 使用正確的初始化方式
            bool success = false;
            if (inspector1.btn_二孔校正.Checked == false) {
                success = inspector1.xCarb震動盤(out pos);
                pos.X = (float)inspector1.nozzleX - pos.X;
                pos.Y = (float)inspector1.nozzleY - pos.Y;
                label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);
            }

            if(SetToPoint == btn_ToPointA) {
                tB_Ax.Text = pos.X.ToString();
                tB_Ay.Text = pos.Y.ToString();
            } else if(SetToPoint == btn_ToPointB) {
                tB_Bx.Text = pos.X.ToString();
                tB_By.Text = pos.Y.ToString();
            } else if(SetToPoint == btn_SwitchPointAB) {
                double dbX = 0.0, dbY = 0.0;

                dbX = double.Parse(tB_Ax.Text);
                dbY = double.Parse(tB_Ay.Text);

                tB_Ax.Text = tB_Bx.Text;
                tB_Ay.Text = tB_By.Text;

                tB_Bx.Text = dbX.ToString();
                tB_By.Text = dbY.ToString();    
            }
        }
        //---------------------------------------------------------------------------------------
        double dbPinHolePositionX = 0.0;
        double dbPinHolePositionY = 0.0;
        int    iHoleIndex         = 0;
        int iPC = 0;
        public void button7_Click(object sender, EventArgs e)
        {
            //找下一個要植針的ID
            if(iPC == 0) {
                iPC = find_PlaceNeedles();
            }

            try {
                iHoleIndex = PlaceNeedles[0].Index;  // 嘗試訪問索引 0 的元素
            } catch (Exception ex) {
                // 捕捉其他類型的異常
                Console.WriteLine("發生錯誤：" + ex.Message);
                iHoleIndex = -1;
            }

            //取得目前植針ID的位置
            if(iHoleIndex == -1) { 
                //沒拿到
            } else if (iHoleIndex >= 0) {
                //沒拿到
                double dbX = 0.0, dbY = 0.0;

                find_PlaceNeedle_Position(PerspectiveTransformMatrix, iHoleIndex, ref dbX, ref dbY);
                FocusedNeedle = PlaceNeedles[0];
                show_grp_NeedleInfo(grp_NeedleInfo);
                pic_Needles.Refresh();

                txt_HoldIndex.Text = iHoleIndex.ToString();

                dbPinHolePositionX = dbX;
                dbPinHolePositionY = dbY;

                label14.Text = dbX.ToString();
                label15.Text = dbY.ToString();
            }

            //刪除目前的植針ID
            PlaceNeedles.RemoveAt(0);
            iPC = PlaceNeedles.Count();
        }
        //---------------------------------------------------------------------------------------
        bool bResume = false;
        public void btn_Resume_Click(object sender, EventArgs e)
        {
            bResume = true;
        }
        //---------------------------------------------------------------------------------------
        bool   b黑色料倉有料_tmrTakePinTick = false;
        bool   b柔震盤有料_tmrTakePinTick   = false;
        double dbPinX_tmrTakePinTick        = 0.0,
               dbPinY_tmrTakePinTick        = 0.0,
               dbPinR_tmrTakePinTick        = 0.0;
        public void btn_取得PinInfo_Click(object sender, EventArgs e)
        {
            //吸料盤校正用
            PointF pos;
            double deg1;
            bool success = false;
            if (inspector1.btn_二孔校正.Checked)
            {
                success = inspector1.xCarb震動盤二孔(out pos, out deg1);
                pos.X = (float)inspector1.nozzleX - pos.X;
                pos.Y = (float)inspector1.nozzleY - pos.Y;
                label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}, deg= {3:F2}", success, pos.X, pos.Y, deg1);
            }
            else
            {
                success = inspector1.xCarb震動盤(out pos);
                //pos.X = (float)inspector1.nozzleX - pos.X;
                //pos.Y = (float)inspector1.nozzleY - pos.Y;
                label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);
            }
            //bool success = inspector1.xCarb震動盤(out pos);
            //label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);
            //bool success = inspector1.xCarb震動盤二孔(out pos, out deg1);
            //pos.X = (float)inspector1.nozzleX - pos.X;
            //pos.Y = (float)inspector1.nozzleY - pos.Y;
            //label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}, deg= {3:F2}", success, pos.X, pos.Y, deg1);

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

            if(cB_料盤有料.Checked == true) {
                b柔震盤有料_tmrTakePinTick = true;
            }
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
        public GlobalKeyboardHook gkh;
        public List<char> BarcodeBuffer = new List<char>(100); // 初始容量為 100
        public static bool isFormActive = false;
        public Form1()
        {
            InitializeComponent();

            Initialize_grp_NeedleInfo_ChildControlChanged_Listener(grp_NeedleInfo);
            Initialize_cms_pic_Needles_ItemClicked_Listener(cms_pic_Needles);

            Initialize_grp_BarcodeInfo_ChildControlChanged_Listener(grp_BarcodeInfo);
            gkh = new GlobalKeyboardHook();
            gkh.KeyUp += Gkh_KeyUp;
        }
        //---------------------------------------------------------------------------------------
        public void Form1_Activated(object sender, EventArgs e)
        {
            isFormActive = true;
        }
        //---------------------------------------------------------------------------------------
        public void Form1_Deactivate(object sender, EventArgs e)
        {
            isFormActive = false;
        }
        //---------------------------------------------------------------------------------------
        public static int i計時300ms = 0;
        const int i計時300ms_Define = 30;
        public void Gkh_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Enter:
                    i計時300ms = 0;

                    if (BarcodeBuffer.Count > 0)
                    {
                        btn_OpenFile_Click(sender, e);
                        BarcodeBuffer.Clear();
                    }
                    break;

                default:
                    // 判斷輸入字為: 0~9 或 'a'~'z' 或 'A'~'Z'
                    if (Char.IsLetter((char)e.KeyCode) || Char.IsDigit((char)e.KeyCode) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
                    {
                        BarcodeBuffer.Add((char)e.KeyCode);  // 將有效的字符添加到緩衝區
                        i計時300ms = i計時300ms_Define;
                    }
                    break;
            }
        }

        public void tmrBarCodeScanner_Tick(object sender, EventArgs e)
        {
            if (i計時300ms > 0)
            {
                i計時300ms--;

            }

            if (i計時300ms == 0)
            {
                i計時300ms = 0;

                if (BarcodeBuffer.Count > 0)
                {
                    BarcodeBuffer.Clear();
                }
            }
        }
        //---------------------------------------------------------------------------------------
        public void Form1_Load(object sender, EventArgs e)
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
        public void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();
            
            //sw.Close();
        }
        //---------------------------------------------------------------------------------------
        public void btn_manual_Click(object sender, EventArgs e)
        {
            TestForm fmTestForm = new TestForm();
            fmTestForm.Show();

            btn_manual.Enabled = false;
        }
        //---------------------------------------------------------------------------------------
        public void btn_Connect_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_establish_Commu();
        }
        //---------------------------------------------------------------------------------------
        public void btn_Disconnect_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();
        }
        //---------------------------------------------------------------------------------------
        public void btn_AlarmRST_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_ClearAlarm();
        }
        //---------------------------------------------------------------------------------------
        public void btnStop_Click(object sender, EventArgs e)
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
        //---------------------------------------------------------------------------------------
        public void btnSetHome_Click(object sender, EventArgs e)
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
        public void RadioGroupChanged(object sender, EventArgs e)
        {  // start of public void RadioGroupChanged(object sender, EventArgs e)
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

        }  // end of public void RadioGroupChanged(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        public void lbl_SetIO_Click(object sender, EventArgs e)
        {  // start of public void lbl_SetIO_Click(object sender, EventArgs e)
            // 將 sender 轉型為 Label
            System.Windows.Forms.Label SelectLabel = sender as System.Windows.Forms.Label;

            //辨識選擇之Label
            if (SelectLabel != null) {
                       if (SelectLabel == lbl擺放蓋板     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)       / 10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)       % 10, (lbl擺放蓋板.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl吸料真空閥   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   / 10, (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   % 10, (lbl吸料真空閥.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl堵料吹氣缸   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣缸)       / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣缸)       % 10, (lbl堵料吹氣缸.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl接料區缸     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_接料區氣桿)       / 10, (int)(WMX3IO對照.pxeIO_接料區氣桿)       % 10, (lbl接料區缸.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl植針吹氣     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)         / 10, (int)(WMX3IO對照.pxeIO_植針吹氣)         % 10, (lbl植針吹氣.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl收料區缸     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_收料區缸)         / 10, (int)(WMX3IO對照.pxeIO_收料區缸)         % 10, (lbl收料區缸.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl堵料吹氣     ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣)         / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣)         % 10, (lbl堵料吹氣.BackColor     == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl載盤真空閥   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_載盤真空閥)       / 10, (int)(WMX3IO對照.pxeIO_載盤真空閥)       % 10, (lbl載盤真空閥.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk真空2      ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空2)      / 10, (int)(WMX3IO對照.pxeIO_Socket真空2)      % 10, (lblsk真空2.BackColor      == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl載盤破真空   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_載盤破真空)       / 10, (int)(WMX3IO對照.pxeIO_載盤破真空)       % 10, (lbl載盤破真空.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk破真空2    ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket破真空2)    / 10, (int)(WMX3IO對照.pxeIO_Socket破真空2)    % 10, (lblsk破真空2.BackColor    == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk真空1      ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空1)      / 10, (int)(WMX3IO對照.pxeIO_Socket真空1)      % 10, (lblsk真空1.BackColor      == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl擺放座真空   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座吸真空)     / 10, (int)(WMX3IO對照.pxeIO_擺放座吸真空)     % 10, (lbl擺放座真空.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblsk破真空1    ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket破真空1)    / 10, (int)(WMX3IO對照.pxeIO_Socket破真空1)    % 10, (lblsk破真空1.BackColor    == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl擺放破真空   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座破真空)     / 10, (int)(WMX3IO對照.pxeIO_擺放座破真空)     % 10, (lbl擺放破真空.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl取料吸嘴吸   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)       / 10, (int)(WMX3IO對照.pxeIO_取料吸嘴吸)       % 10, (lbl取料吸嘴吸.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl下後左門鎖   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_下後左門鎖)       / 10, (int)(WMX3IO對照.pxeIO_下後左門鎖)       % 10, (lbl下後左門鎖.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl取料吸嘴破舊 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空舊) / 10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空舊) % 10, (lbl取料吸嘴破舊.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl下後右門鎖   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_下後右門鎖)       / 10, (int)(WMX3IO對照.pxeIO_下後右門鎖)       % 10, (lbl下後右門鎖.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl植針Z煞車    ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針Z煞車)        / 10, (int)(WMX3IO對照.pxeIO_植針Z煞車)        % 10, (lbl植針Z煞車.BackColor    == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblHEPA         ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_HEPA)             / 10, (int)(WMX3IO對照.pxeIO_HEPA)             % 10, (lblHEPA.BackColor         == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl取料吸嘴破新 ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新) / 10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新) % 10, (lbl取料吸嘴破新.BackColor == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl艙內燈       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT)            / 10, (int)(WMX3IO對照.pxeIO_LIGHT)            % 10, (lbl艙內燈.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl右按鈕綠燈   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)   / 10, (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)   % 10, (lbl右按鈕綠燈.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl紅燈         ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)         / 10, (int)(WMX3IO對照.pxeIO_機台紅燈)         % 10, (lbl紅燈.BackColor         == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl中按鈕綠燈   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)   / 10, (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)   % 10, (lbl中按鈕綠燈.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl黃燈         ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台黃燈)         / 10, (int)(WMX3IO對照.pxeIO_機台黃燈)         % 10, (lbl黃燈.BackColor         == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl左按鈕紅燈   ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)   / 10, (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)   % 10, (lbl左按鈕紅燈.BackColor   == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lbl綠燈         ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)         / 10, (int)(WMX3IO對照.pxeIO_機台綠燈)         % 10, (lbl綠燈.BackColor         == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblBuzzer       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer)           / 10, (int)(WMX3IO對照.pxeIO_Buzzer)           % 10, (lblBuzzer.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                }


            }
        }  // end of public void lbl_SetIO_Click(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        public void btn_adjust_JOG(object sender, EventArgs e)
        {  // start of public void btn_adjust_JOG(object sender, EventArgs e)
            // 將 sender 轉型為 Button
            System.Windows.Forms.Button ptrBtn = sender as System.Windows.Forms.Button;

            double result = double.Parse(txtABSpos.Text) + 0.0;

                   if (ptrBtn == btn_plus_d001  ) { result += 0.001; ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_d001 ) { result -= 0.001; ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_d01   ) { result += 0.01;  ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_d01  ) { result -= 0.01;  ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_d1    ) { result += 0.1;   ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_d1   ) { result -= 0.1;   ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_1     ) { result += 1.0;   ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_1    ) { result -= 1.0;   ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_plus_10    ) { result += 10.0;  ptrBtn = btnABSMove;
            } else if (ptrBtn == btn_minus_10   ) { result -= 10.0;  ptrBtn = btnABSMove;
            }

            if (ptrBtn == btnABSMove) {
                //辨識選擇之軸
                switch(wmxId_RadioGroupChanged) {
                    case WMX3軸定義.吸嘴X軸:         if(enGC_吸嘴X軸       == true) { dbapiNozzleX(    result, 250);   } break;
                    case WMX3軸定義.吸嘴Y軸:         if(enGC_吸嘴Y軸       == true) { dbapiNozzleY(    result, 100);   } break;
                    case WMX3軸定義.吸嘴Z軸:         if(enGC_吸嘴Z軸       == true) { dbapiNozzleZ(    result,  20);   } break;
                    case WMX3軸定義.吸嘴R軸:         if(enGC_吸嘴R軸       == true) { dbapiNozzleR(    result,  70);   } break;
                    case WMX3軸定義.載盤X軸:         if(enGC_載盤X軸       == true) { dbapiCarrierX(   result, 190);   } break;
                    case WMX3軸定義.載盤Y軸:         if(enGC_載盤Y軸       == true) { dbapiCarrierY(   result, 800);   } break;
                    case WMX3軸定義.植針Z軸:         if(enGC_植針Z軸       == true) { dbapiSetZ(       result, 33);    } break;
                    case WMX3軸定義.植針R軸:         if(enGC_植針R軸       == true) { dbapiSetR(       result, 360);   } break;
                    case WMX3軸定義.工作門:          if(enGC_工作門        == true) { dbapiGate(       result, 580/4); } break;
                    case WMX3軸定義.IAISocket孔檢測: if(enGC_IAI           == true) { dbapiIAI(        result);        } break;
                    case WMX3軸定義.JoDell3D掃描:    if(enGC_JoDell3D掃描  == true) { dbapJoDell3D掃描(result);        } break;
                    case WMX3軸定義.JoDell吸針嘴:    if(enGC_JoDell吸針嘴  == true) { dbapJoDell吸針嘴(result);        } break;
                    case WMX3軸定義.JoDell植針嘴:    if(enGC_JoDell植針嘴  == true) { dbapJoDell植針嘴(result);        } break;
                }
            }

            txtABSpos.Text = result.ToString("F3");
        }  // end of public void btn_adjust_JOG(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //讀取OutputIO
        public byte[] pDataGetOutIO = new byte[4];

        //讀取InputIO
        public byte[] pDataGetInIO = new byte[8];

        public void timer1_Tick(object sender, EventArgs e)
        {  // start of public void timer1_Tick(object sender, EventArgs e)
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
                lbl擺放蓋板.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸料真空閥.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料吹氣缸.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_堵料吹氣缸)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料吹氣缸)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl接料區缸.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_接料區氣桿)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_接料區氣桿)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl植針吹氣.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_植針吹氣)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針吹氣)          % 10)) != 0) ? Color.Green : Color.Red;
                lbl收料區缸.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_收料區缸)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_收料區缸)          % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料吹氣.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_堵料吹氣)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料吹氣)          % 10)) != 0) ? Color.Green : Color.Red;

                lbl載盤真空閥.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_載盤真空閥)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤真空閥)        % 10)) != 0) ? Color.Green : Color.Red;
                lblsk真空2.BackColor      = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket真空2)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket真空2)       % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤破真空.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_載盤破真空)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤破真空)        % 10)) != 0) ? Color.Green : Color.Red;
                lblsk破真空2.BackColor    = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket破真空2)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket破真空2)     % 10)) != 0) ? Color.Green : Color.Red;
                lblsk真空1.BackColor      = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket真空1)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket真空1)       % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放座真空.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座吸真空)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座吸真空)      % 10)) != 0) ? Color.Green : Color.Red;
                lblsk破真空1.BackColor    = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Socket破真空1)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket破真空1)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放破真空.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座破真空)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座破真空)      % 10)) != 0) ? Color.Green : Color.Red;

                lbl取料吸嘴吸.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_取料吸嘴吸)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料吸嘴吸)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後左門鎖.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_下後左門鎖)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下後左門鎖)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料吸嘴破舊.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_取料吸嘴破真空舊) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料吸嘴破真空舊)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後右門鎖.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_下後右門鎖)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下後右門鎖)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl植針Z煞車.BackColor    = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_植針Z煞車)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針Z煞車)         % 10)) != 0) ? Color.Green : Color.Red;
                lblHEPA.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_HEPA)             / 10)] & (1 << (int)(WMX3IO對照.pxeIO_HEPA)              % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料吸嘴破新.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_取料吸嘴破真空新) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl艙內燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_LIGHT)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_LIGHT)             % 10)) != 0) ? Color.Green : Color.Red;

                lbl右按鈕綠燈.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板右按鈕綠燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl紅燈.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台紅燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台紅燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl中按鈕綠燈.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板中按鈕綠燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl黃燈.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台黃燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台黃燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl左按鈕紅燈.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板左按鈕紅燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl綠燈.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台綠燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台綠燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lblBuzzer.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Buzzer)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Buzzer)          % 10)) != 0) ? Color.Green : Color.Red;
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
#if (false)
                if(lbl載盤空2.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 1);
                }
                if (lbl載盤空2.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 0);
                }

                if(lbl載盤空1.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer) / 10, (int)(WMX3IO對照.pxeIO_Buzzer) % 10, 0);
                }
                if (lbl載盤空1.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer) / 10, (int)(WMX3IO對照.pxeIO_Buzzer) % 10, 1);
                }
#endif
                lblsk2空2.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket2真空檢2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket2真空檢2)  % 10)) != 0) ? Color.Green : Color.Red;
                lblsk1空1.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket1真空檢1)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket1真空檢1)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座真空檢1)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座真空檢1)   % 10)) != 0) ? Color.Green : Color.Red;
                lblsk1空2.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket1真空檢2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket1真空檢2)  % 10)) != 0) ? Color.Green : Color.Red;
#if (false)
                if(lblsk2空2.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 1);
                }
                if (lblsk2空2.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 0);
                }

                if(lblsk2空1.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer) / 10, (int)(WMX3IO對照.pxeIO_Buzzer) % 10, 0);
                }
                if (lblsk2空1.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer) / 10, (int)(WMX3IO對照.pxeIO_Buzzer) % 10, 1);
                }
#endif
#if (false)
                if(lblsk1空2.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 1);
                }
                if (lblsk1空2.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 0);
                }

                if(lblsk1空1.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer) / 10, (int)(WMX3IO對照.pxeIO_Buzzer) % 10, 0);
                }
                if (lblsk1空1.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer) / 10, (int)(WMX3IO對照.pxeIO_Buzzer) % 10, 1);
                }
#endif
                lbl擺放空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座真空檢2)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座真空檢2)   % 10)) != 0) ? Color.Green : Color.Red;
#if (false)
                if(lbl擺放空2.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 1);
                }
                if (lbl擺放空2.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 0);
                }

                if(lbl擺放空1.BackColor == Color.Red) { 
                    //關
                    //Vibration LED
                    SB_VBLED.Value = 5;
                    clsVibration.apiEstablishTCPVibration(); {
                        clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                        clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
                        lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value;
                    }
                }
                if (lbl擺放空1.BackColor == Color.Green) {
                    //開
                    //Vibration LED
                    SB_VBLED.Value = 50;
                    clsVibration.apiEstablishTCPVibration(); {
                        clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                        clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
                        lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value;
                    }
                }
#endif
                lbl吸嘴空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸嘴真空檢1)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸嘴真空檢1)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸嘴空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸嘴真空檢2)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸嘴真空檢2)     % 10)) != 0) ? Color.Green : Color.Red;
#if (false)
                if(lbl吸嘴空2.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 1);
                }
                if (lbl吸嘴空2.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_LIGHT) / 10, (int)(WMX3IO對照.pxeIO_LIGHT) % 10, 0);
                }

                if(lbl吸嘴空1.BackColor == Color.Red) { 
                    //關
                    //Vibration LED
                    SB_VBLED.Value = 5;
                    clsVibration.apiEstablishTCPVibration(); {
                        clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                        clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
                        lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value;
                    }
                }
                if (lbl吸嘴空1.BackColor == Color.Green) {
                    //開
                    //Vibration LED
                    SB_VBLED.Value = 50;
                    clsVibration.apiEstablishTCPVibration(); {
                        clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                        clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
                        lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value;
                    }
                }
#endif
                lbl取料ng盒.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料NG收料盒)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料NG收料盒)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl兩點壓1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_兩點組合壓力檢1) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_兩點組合壓力檢1) % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料盒.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_堵料收料盒)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料收料盒)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl兩點壓2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_兩點組合壓力檢2) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_兩點組合壓力檢2) % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸料盒.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸料收料盒)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸料收料盒)      % 10)) != 0) ? Color.Green : Color.Red;
#if (false)
                //右
                if(lbl兩點壓2.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)       / 10, (int)(WMX3IO對照.pxeIO_機台紅燈)       % 10, 0);
                }
                if (lbl兩點壓2.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台紅燈)       / 10, (int)(WMX3IO對照.pxeIO_機台紅燈)       % 10, 1);
                }

                //左
                if(lbl兩點壓1.BackColor == Color.Red) {
                    //關
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)       / 10, (int)(WMX3IO對照.pxeIO_機台綠燈)       % 10, 0);
                }
                if (lbl兩點壓1.BackColor == Color.Green) {
                    //開
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_機台綠燈)       / 10, (int)(WMX3IO對照.pxeIO_機台綠燈)       % 10, 1);
                }
#endif

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

        }  // end of public void timer1_Tick(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //-------------------------------- Project Code implement -------------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //---------------------------------- Vibration implement --------------------------------
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
        public void lbl柔震index(object sender, EventArgs e)
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
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
        public void btnVibrationStop_Click(object sender, EventArgs e)
        {
            //Vibration
            clsVibration.apiEstablishTCPVibration();
            {
                uint bRunning = 0;
                clsVibration.Px1_SendCMD(xe_U15_CMD.xeUC_TestMode_FunctionOn, bRunning);
            }
        }
        //---------------------------------------------------------------------------------------
        public void SB_VBLED_Scroll(object sender, ScrollEventArgs e)
        {
            //Vibration LED
            clsVibration.apiEstablishTCPVibration(); {
                clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
                lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value;
            }
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------- Vibration implement --------------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //----------------------------- Warning Indicator implement -----------------------------
        //---------------------------------------------------------------------------------------
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

        //---------------------------------------------------------------------------------------
        public void tmr_Buzzer_Tick(object sender, EventArgs e)
        {  // start of public void tmr_Buzzer_Tick(object sender, EventArgs e)
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
        }  // end of public void tmr_Buzzer_Tick(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //----------------------------- Warning Indicator implement -----------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //----------------------------- Flow Valve Control implement ----------------------------
        //---------------------------------------------------------------------------------------
        public void vcb流量閥_Scroll(object sender, ScrollEventArgs e)
        {
            System.Windows.Forms.VScrollBar vcb流量閥 = sender as System.Windows.Forms.VScrollBar;

            /*
                y=0.000454x−2.5071
                x=(y+2.5071)/0.000454
            */

            Normal calculate = new Normal();
            double dbGet = 0.0;

            if(vcb流量閥 == vcb_吸嘴破真空流量閥) {
                dbGet = calculate.Map(vcb_吸嘴破真空流量閥.Value, 110, -10, -10, 110)/10;

                if (10.0 <= dbGet) {
                    dbGet = 10;
                }
                if(dbGet <= 0.0) {
                    dbGet = 0;
                }
            } else if(vcb流量閥 == vcb_植針吹氣流量閥) {
                dbGet = calculate.Map(vcb_植針吹氣流量閥.Value,  110, -10, -10, 110);

                if (100.0 <= dbGet) {
                    dbGet = 100;
                }
                if(dbGet <= 0.0) {
                    dbGet = 0;
                }
            }

            double y = (double)( dbGet/10.0 );
            double x = (y + 2.5071) / 0.000454;

            int iGetValue = (int)x;
            byte[] aGetValue = BitConverter.GetBytes(iGetValue);

            if(vcb流量閥 == vcb_吸嘴破真空流量閥) {
                clsServoControlWMX3.WMX3_SetIO(ref aGetValue, (int)WMX3IO對照.pxeIO_Addr_AnalogOut_0, 2);
            } else if(vcb流量閥 == vcb_植針吹氣流量閥) {
                clsServoControlWMX3.WMX3_SetIO(ref aGetValue, (int)WMX3IO對照.pxeIO_Addr_AnalogOut_1, 2);
            }

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

            if(vcb流量閥 == vcb_吸嘴破真空流量閥) {
                lbl_吸嘴破真空流量閥.Text = string.Format("{0:F1}", y);
            } else if(vcb流量閥 == vcb_植針吹氣流量閥) {
                lbl_植針吹氣流量閥.Text = string.Format("{0:F1}", y);
            }

        }
        //---------------------------------------------------------------------------------------
        //----------------------------- Flow Valve Control implement ----------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //-------------------------------- State Machine implement ------------------------------
        //---------------------------------------------------------------------------------------
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

        //---------------------------------------------------------------------------------------
        public void btn_home_Click(object sender, EventArgs e)
        {
            bhome = true;
        }
        //---------------------------------------------------------------------------------------
        public void tmr_Sequense_Tick(object sender, EventArgs e)
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
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public enum xe_tmr_takepin {
            xett_Empty,
                xett_確定執行要取針,
                    xett_關工作門,
                        xett_檢查工作門關閉,
                        xett_確定工作門關閉,

                    xett_載盤真空閥啟用,

                    /* bTakePin */                                                                                                  /*  bRemove  */
                    xett_Socket1真空閥啟用,                                                                                         xett_Socket1真空閥關掉,
                    xett_Socket2真空閥啟用,                                                                                         xett_Socket2真空閥關掉,
                                                                                                                                    
                    xett_取得柔震盤針資訊,                                                                                          xett_NozzleZ縮回0保護,   
                        xett_柔震盤無針,                                                                                            xett_NozzleXY回家,
                            xett_柔震盤料倉震動,                                                                                    xett_Socket相機移至拍照位22,  
                                xett_等待柔震盤料倉震動2秒,                                                                         xett_擺放座Z軸縮回, 
                            xett_柔震盤上下震動,                                                                                    xett_3D掃描電動缸縮回,
                                xett_等待柔震盤上下震動2秒,                                                                         xett_吸針嘴電動缸縮回,
                            xett_柔震盤左右震動,                                                                                    xett_吸針接料盒就位,
                                xett_等待柔震盤左右震動2秒,                                                                         xett_Nozzle電磁閥關閉,                                                                  
                            xett_柔震盤散震震動,                                                                                    xett_植針座電磁閥關閉,
                                xett_等待柔震盤散震震動2秒,                                                                         xett_以上11項,
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

                            xett_Nozzle吐料開始,                               xett_擺放座蓋板打開,
                            xett_Nozzle吸料停止,                               xett_檢查擺放座蓋板是否打開,  
                                                                               xett_擺放座蓋板打開等待1秒,
                            xett_Nozzle吐料等待,                               xett_確認擺放座蓋板打開,
                            xett_Nozzle吐料完成,
                                                                               xett_擺放座R軸至放料位,
                            xett_NozzleZ退回安全高度0,                         xett_檢查擺放座R軸是否至放料位,
                            xett_檢查NozzleZ是否退回安全高度0,                 xett_確認擺放座R軸至放料位,
                            xett_確定NozzleZ已退回安全高度0,
                                                                               xett_擺放座Z軸至放料位,
                                                                               xett_檢查擺放座Z軸是否至放料位,
                                                                               xett_確認擺放座Z軸至放料位,

                                                                               xett_NozzleZ下降至上膛位,
                                                                               xett_檢查NozzleZ是否下降至上膛位,
                                                                               xett_確認NozzleZ下降至上膛位,

                                                                               xett_擺放座開真空,
                                                                               xett_擺放座開真空等待1秒,

                                                                               xett_吸嘴破真空,
                                                                               xett_Nozzle吸嘴關真空,
                                                                               xett_Nozzle吸嘴關真空等待1秒,
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

                                                                               xett_取得植針目標座標,

                                                                               //載盤
                                                                               xett_載盤XY移置拍照檢查位,
                                                                               xett_檢查載盤XY是否移置拍照檢查位,
                                                                               xett_等待載盤XY移置拍照檢查位,
                                                                               xett_確認載盤XY移置拍照檢查位,

                                                                               xett_載盤移植直針孔相機補正位,
                                                                               xett_檢查載盤移植直針孔相機補正位,
                                                                               xett_確認載盤移植直針孔相機補正位,

                                                                               /* bChambered */                                     /*  bRemove  */
                                                                               xett_載盤XY移置直針位,                               xett_載盤XY移置抽料位,
                                                                               xett_檢查載盤XY是否移置直針位,                       xett_檢查載盤XY是否移置抽料位,
                                                                               xett_確認載盤XY移置直針位,                           xett_確認載盤XY移置抽料位,

                                                                               xett_擺放座Z軸至植針位,                              xett_抽料Z軸至抽料位,
                                                                               xett_檢查擺放座Z軸是否至植針位,                      xett_抽料Z軸是否至抽料位,
                                                                               xett_確認擺放座Z軸至植針位,                          xett_抽料Z軸確認至抽料位,

                                                                               xett_擺放座真空關閉,                                 xett_抽料電磁閥開啟,
                                                                                                                                    xett_抽料電磁閥開啟等待1秒,
                                                                               xett_植針吹氣電磁閥開啟,                             xett_抽料電磁閥關閉,
                                                                               xett_植針吹氣電磁閥開啟等待1秒,
                                                                                                                                    xett_抽料Z軸回0,
                                                                               xett_開啟流量閥1,                                    xett_抽料Z軸是否回0,
                                                                               xett_開啟流量閥1等待1秒,                             xett_抽料Z軸確認回0,

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

                                                                               /* bChambered */ /*+*/ /*  bRemove  */
                                                                               xett_載盤XY再次移置拍照檢查位,
                                                                               xett_檢查載盤XY是否再次移置拍照檢查位,
                                                                               xett_等待載盤XY再次移置拍照檢查位,
                                                                               xett_確認載盤XY再次移置拍照檢查位,

                                                                               /* bChambered */                                     /*  bRemove  */
                                                                               xett_檢查有無植針成功,                               xett_檢查有無抽針成功,

                /* bTakePin */ /*+*/ /* bChambered */ /*+*/ /*  bRemove  */
                xett_檢測是否還需要取針,
                    xett_還需要取針,
                        xett_重覆一開始的狀態,

                    xett_不需要取針,
                        xett_NozzleXYR移置安全位置,
                        xett_檢查NozzleXYR是否移至安全位置,
                        xett_確認NozzleXYR在安全位置,

                xett_取針結束,

            xett_End,
        };
        public xe_tmr_takepin xeTmrTakePin = xe_tmr_takepin.xett_Empty;

        public int  iTakePinFinishedCNT1 = 0;
        public int  iTakePinFinishedCNT2 = 0;
        public bool bTakePin             = false;
        public bool bChambered           = false;
        public bool bRemove              = false;
        public bool bPause               = false;
        public bool btmrStop             = false;
        public int  itmrStop             = 1;
        public const double db取料Nozzle中心點X = 49.93;
        public const double db取料Nozzle中心點Y = 49.81;
        public const double db取料Nozzle中心點Z = 26;
        public const double db取料Nozzle中心點R = 1.34;

        public const double db下視覺取像X_Start = 105;
        public const double db下視覺取像X_END   = 243.000;
        public const double db下視覺取像Y       = 27.05;
        public const double db下視覺取像Z       = 0;

        public const double db吐料位X          = 243.000;
        public const double db吐料位下降Z高度   = 2.000;

        public DateTime Prev_CycleTime;
        public DateTime Curr_CycleTime;
        public TimeSpan CycleTime;

        MX PerspectiveTransformMatrix = new MX();

        //---------------------------------------------------------------------------------------
        public void btn_TakePin_Click(object sender, EventArgs e)
        {
            bTakePin = true;
        }
        //---------------------------------------------------------------------------------------
        public void btn上膛_Click(object sender, EventArgs e)
        {
            bChambered = true;

            {
                Normal calculate = new Normal();

                // 定義 PointA, PointB 的數據
                Normal.Point idealA = new Normal.Point(387.62823, 93.82427);
                Normal.Point idealB = new Normal.Point(419.62823, 107.82427);
                Normal.Point realA = new Normal.Point(145.556, 616.323);
                Normal.Point realB = new Normal.Point(113.584, 602.195);

                // 宣告 PointForward 和 PointBackward 變數
                Normal.Point idealAForward = new Normal.Point();
                Normal.Point idealABackward = new Normal.Point();
                Normal.Point realAForward = new Normal.Point();
                Normal.Point realABackward = new Normal.Point();

                // 呼叫計算並傳遞相應的點作為參數
                CalculateAndPrintPlotData(idealA, idealB, out idealAForward, out idealABackward);
                CalculateAndPrintPlotData(realA, realB, out realAForward, out realABackward);

                // 計算PerspectiveTransform
                double[,] idealCoords = { { idealA.X,         idealA.Y },
                                          { idealAForward.X,  idealAForward.Y },
                                          { idealB.X,         idealB.Y },
                                          { idealABackward.X, idealABackward.Y } };

                double[,] realCoords  = { { realA.X,         realA.Y },
                                          { realABackward.X, realABackward.Y },
                                          { realB.X,         realB.Y },
                                          { realAForward.X,  realAForward.Y } };

                ComputePerspectiveTransform(idealCoords, realCoords, PerspectiveTransformMatrix);

                //// 求得映射轉換座標
                //double X_In = idealA.X,
                //       Y_In = idealA.Y;
                //Normal.Point pMapping = MapToCoords(PerspectiveTransformMatrix, X_In, Y_In);
            }
        }
        //---------------------------------------------------------------------------------------
        public void btn抽針_Click(object sender, EventArgs e)
        {
            bRemove = true;

            {
                Normal calculate = new Normal();

                // 定義 PointA, PointB 的數據
                Normal.Point idealA = new Normal.Point(387.62823, 93.82427);
                Normal.Point idealB = new Normal.Point(419.62823, 107.82427);
                Normal.Point realA = new Normal.Point(145.556, 616.323);
                Normal.Point realB = new Normal.Point(113.584, 602.195);

                // 宣告 PointForward 和 PointBackward 變數
                Normal.Point idealAForward = new Normal.Point();
                Normal.Point idealABackward = new Normal.Point();
                Normal.Point realAForward = new Normal.Point();
                Normal.Point realABackward = new Normal.Point();

                // 呼叫計算並傳遞相應的點作為參數
                CalculateAndPrintPlotData(idealA, idealB, out idealAForward, out idealABackward);
                CalculateAndPrintPlotData(realA, realB, out realAForward, out realABackward);

                // 計算PerspectiveTransform
                double[,] idealCoords = { { idealA.X,         idealA.Y },
                                          { idealAForward.X,  idealAForward.Y },
                                          { idealB.X,         idealB.Y },
                                          { idealABackward.X, idealABackward.Y } };

                double[,] realCoords  = { { realA.X,         realA.Y },
                                          { realABackward.X, realABackward.Y },
                                          { realB.X,         realB.Y },
                                          { realAForward.X,  realAForward.Y } };

                ComputePerspectiveTransform(idealCoords, realCoords, PerspectiveTransformMatrix);

                //// 求得映射轉換座標
                //double X_In = idealA.X,
                //       Y_In = idealA.Y;
                //Normal.Point pMapping = MapToCoords(PerspectiveTransformMatrix, X_In, Y_In);
            }
        }
        //---------------------------------------------------------------------------------------
        public void btn_tmrStop_Click(object sender, EventArgs e)
        {
            btmrStop = true;
        }
        //---------------------------------------------------------------------------------------
        public void btn_tmrPause_Click(object sender, EventArgs e)
        {
            tmr_TakePin.Enabled = bPause;
            bPause = !bPause;
        }
        //---------------------------------------------------------------------------------------
        public void btn_tmrClear_Click(object sender, EventArgs e)
        {
            bTakePin = false;
            bChambered = false;
            xeTmrTakePin = xe_tmr_takepin.xett_Empty;
        }
        //---------------------------------------------------------------------------------------
        public void tmr_TakePin_Tick(object sender, EventArgs e)
        {  // start of public void tmr_TakePin_Tick(object sender, EventArgs e)

            lblLog.Text = xeTmrTakePin.ToString() + ", 柔震重試:" + iTakePinFinishedCNT2;

            if(cB_AlwaysResume.Checked == true) {
                bResume = true;
            }

            switch (xeTmrTakePin) {
                case xe_tmr_takepin.xett_Empty:  
                    if(bTakePin==true || bChambered==true || bRemove==true) {
                        int 求出取料循環次數 = int.Parse(txt_取料循環.Text);
                        if(求出取料循環次數>=1) { 
                            xeTmrTakePin = xe_tmr_takepin.xett_確定執行要取針;
                        } else {
                            xeTmrTakePin = xe_tmr_takepin.xett_取針結束;
                        }
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
                        case xe_tmr_takepin.xett_確定工作門關閉:                                  xeTmrTakePin = xe_tmr_takepin.xett_載盤真空閥啟用;  break;

                        case xe_tmr_takepin.xett_載盤真空閥啟用:
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_載盤真空閥) / 10, (int)(WMX3IO對照.pxeIO_載盤真空閥) % 10, 1);
                            if(bRemove==true) {
                                xeTmrTakePin = xe_tmr_takepin.xett_Socket1真空閥關掉;
                            } else {
                                xeTmrTakePin = xe_tmr_takepin.xett_Socket1真空閥啟用;
                            }
                            break;

                        case xe_tmr_takepin.xett_Socket1真空閥啟用:
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空1) / 10, (int)(WMX3IO對照.pxeIO_Socket真空1) % 10, 1);
                            xeTmrTakePin = xe_tmr_takepin.xett_Socket2真空閥啟用;
                            break;

                        case xe_tmr_takepin.xett_Socket2真空閥啟用:
                            clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空2) / 10, (int)(WMX3IO對照.pxeIO_Socket真空2) % 10, 1);
                            xeTmrTakePin = xe_tmr_takepin.xett_取得柔震盤針資訊;
                            break;

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

                                        if(bResume == true) {
                                            bResume = false;
                                            xeTmrTakePin = xe_tmr_takepin.xett_判斷NozzleZ吸料位安全位置;
                                        }
                                    }
                                } break;
                                case xe_tmr_takepin.xett_判斷NozzleZ吸料位安全位置: 
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料開始;
                                    break;

                                case xe_tmr_takepin.xett_Nozzle吸料開始: {
                                    byte b吸嘴吸 = 0;
                                    if(bTakePin == true || bChambered == true) { 
                                        b吸嘴吸 = 1;
                                    }
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10, b吸嘴吸);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料等待;
                                } break;
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
                                case xe_tmr_takepin.xett_確定飛拍移動完成: {
                        
                                    double dbTargetNozzleR = 0.0;
                                    if(dbNozzleDegInverse) { 
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90 + 180;
                                    } else { 
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90;
                                    }
                                    dbapiNozzleR(dbTargetNozzleR, 360*4);

                                    xeTmrTakePin = xe_tmr_takepin.xett_移至吐料位;  
                                } break;

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
                                case xe_tmr_takepin.xett_確認NozzleZ在吐料高度:            xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吐料開始;  break;

                                case xe_tmr_takepin.xett_Nozzle吐料開始: {

                                    vcb_吸嘴破真空流量閥.Value = 100-100;
                                    ScrollEventArgs xe = null;
                                    vcb流量閥_Scroll(vcb_吸嘴破真空流量閥, xe);

                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10,       (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10,       0);

                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸料停止;
                                } break;
                                case xe_tmr_takepin.xett_Nozzle吸料停止:
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吐料等待;
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吐料等待:            
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=60) { 
                                        iTakePinFinishedCNT1 = 0;

                                        vcb_吸嘴破真空流量閥.Value = 100-0;
                                        ScrollEventArgs xe = null;
                                        vcb流量閥_Scroll(vcb_吸嘴破真空流量閥, xe);

                                        xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吐料完成; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吐料完成:
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)%10, 0);
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
                                case xe_tmr_takepin.xett_NozzleXYR移至上膛位: {    
                                    double dbTargetNozzleR = 0.0;
                                    if(dbNozzleDegInverse) { 
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90 + 180;
                                    } else { 
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90;
                                    }

                                    dbapiNozzleX(495,             500*2);
                                    dbapiNozzleY(77.05,           100*4);
                                    dbapiNozzleR(dbTargetNozzleR, 360*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleXYR是否移至上膛位;  break;
                                } break;
                                case xe_tmr_takepin.xett_檢查NozzleXYR是否移至上膛位: {
                                    double dbX = dbapiNozzleX(dbRead, 0);
                                    double dbY = dbapiNozzleY(dbRead, 0);
                                    double dbR = dbapiNozzleR(dbRead, 0);

                                    double dbTargetNozzleR = 0.0;
                                    if(dbNozzleDegInverse) { 
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90 + 180;
                                    } else { 
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90;
                                    }

                                    double dbTargetX = 495;
                                    double dbTargetY = 77.05;
                                    double dbTargetR = dbTargetNozzleR;
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
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=20) { 
                                        iTakePinFinishedCNT1 = 0;
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
                                    dbapiNozzleZ(36.65, 40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查NozzleZ是否下降至上膛位;
                                    break;
                                case xe_tmr_takepin.xett_檢查NozzleZ是否下降至上膛位:  {
                                    double dbZ = dbapiNozzleZ(dbRead, 0);
                                    double dbTargetZ = 36.65;
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
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=20) { 
                                        iTakePinFinishedCNT1 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_吸嘴破真空; 
                                    }
                                    break;

                                case xe_tmr_takepin.xett_吸嘴破真空: {

                                    vcb_吸嘴破真空流量閥.Value = 100-100;
                                    ScrollEventArgs xe = null;
                                    vcb流量閥_Scroll(vcb_吸嘴破真空流量閥, xe);

                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10,       (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10,       0);

                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸嘴關真空; 
                                } break;
                                case xe_tmr_takepin.xett_Nozzle吸嘴關真空:              
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle吸嘴關真空等待1秒; 
                                    break;
                                case xe_tmr_takepin.xett_Nozzle吸嘴關真空等待1秒: 
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=10) { 
                                        iTakePinFinishedCNT1 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_吸嘴破真空等待1秒; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_吸嘴破真空等待1秒:
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=60) { 
                                        iTakePinFinishedCNT1 = 0;

                                        vcb_吸嘴破真空流量閥.Value = 100-0;
                                        ScrollEventArgs xe = null;
                                        vcb流量閥_Scroll(vcb_吸嘴破真空流量閥, xe);

                                        xeTmrTakePin = xe_tmr_takepin.xett_吸嘴破真空關閉; 
                                    }
                                    break;
                                case xe_tmr_takepin.xett_吸嘴破真空關閉:       
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_Nozzle回至0點保護位; 
                                    break;

                                case xe_tmr_takepin.xett_Nozzle回至0點保護位:                    
                                    dbapiNozzleZ(0, 40*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查Nozzle是否回至0點保護位;
                                    break;
                                case xe_tmr_takepin.xett_檢查Nozzle是否回至0點保護位: {
                                    double dbGetZ_1 = dbapiNozzleZ(dbRead, 0);
                                    if(dbGetZ_1 <= 0.1) { 

                                        if(bResume == true) {
                                            bResume = false;
                                            xeTmrTakePin = xe_tmr_takepin.xett_確認Nozzle回至0點保護位;
                                        }
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

                                    if(bResume == true) {
                                        bResume = false;
                                        擺放座蓋板閉合是否為4 = 4;
                                    }

                                    if (擺放座蓋板閉合是否為4 == 4) { 
                                        //已閉合
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座蓋板關閉;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認擺放座蓋板關閉:                                     xeTmrTakePin = xe_tmr_takepin.xett_取得植針目標座標;  break;

                                case xe_tmr_takepin.xett_取得植針目標座標: {
                                    button7_Click(sender, e);
                                    xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置拍照檢查位;
                                } break;

                                case xe_tmr_takepin.xett_載盤XY移置拍照檢查位: { 
                                    double dbTargetX = dbPinHolePositionX;
                                    double dbTargetY = dbPinHolePositionY;

                                    dbapiCarrierX(dbTargetX, 190*0.8);
                                    dbapiCarrierY(dbTargetY, 800*0.8);

                                    dbapiIAI(22.0);

                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置拍照檢查位;
                                } break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否移置拍照檢查位: {
                                    double dbreadX = dbapiCarrierX(dbRead, 0);
                                    double dbreadY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = dbPinHolePositionX;
                                    double dbTargetY = dbPinHolePositionY;
                                    if( (dbTargetX*0.99 <= dbreadX && dbreadX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99 <= dbreadY && dbreadY <= dbTargetY*1.01) ) { 

                                        if(bResume == true) {
                                            bResume = false;
                                            xeTmrTakePin = xe_tmr_takepin.xett_等待載盤XY移置拍照檢查位;
                                        }
                                    }
                                } break;
                                case xe_tmr_takepin.xett_等待載盤XY移置拍照檢查位:
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=10) { 
                                        iTakePinFinishedCNT1 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置拍照檢查位;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認載盤XY移置拍照檢查位:                                xeTmrTakePin = xe_tmr_takepin.xett_載盤移植直針孔相機補正位;  break;

                                case xe_tmr_takepin.xett_載盤移植直針孔相機補正位: {  
                                    button3_Click(sender, e);

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY;

                                    dbapiCarrierX(dbTargetX, 190*0.8);
                                    dbapiCarrierY(dbTargetY, 800*0.8);

                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤移植直針孔相機補正位;  
                                } break;
                                case xe_tmr_takepin.xett_檢查載盤移植直針孔相機補正位: {    
                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY;
                                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                                        if(bResume == true) {
                                            bResume = false;
                                            xeTmrTakePin = xe_tmr_takepin.xett_確認載盤移植直針孔相機補正位;
                                        }
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認載盤移植直針孔相機補正位:                            xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置直針位;  break;

                                case xe_tmr_takepin.xett_載盤XY移置直針位: {  
                                    const double SetPinOffsetX =  2.255;
                                    const double SetPinOffsetY = 54.462;

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX + SetPinOffsetX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY + SetPinOffsetY;

                                    dbapiCarrierX(dbTargetX, 190*0.8);
                                    dbapiCarrierY(dbTargetY, 800*0.8);

                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置直針位;
                                } break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否移置直針位: {
                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    const double SetPinOffsetX =  2.255;
                                    const double SetPinOffsetY = 54.462;

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX + SetPinOffsetX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY + SetPinOffsetY;
                                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置直針位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認載盤XY移置直針位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸至植針位;  break;

                                case xe_tmr_takepin.xett_擺放座Z軸至植針位:   
                                    dbapiSetZ(26, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座Z軸是否至植針位;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座Z軸是否至植針位: {
                                    double dbZ = dbapiSetZ(dbRead, 0);
                                    double dbTargetZ = 26;
                                    if( (dbTargetZ*0.99 <= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座Z軸至植針位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座Z軸至植針位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座真空關閉;  break;

                                case xe_tmr_takepin.xett_擺放座真空關閉:    
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座吸真空)/10, (int)(WMX3IO對照.pxeIO_擺放座吸真空)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_植針吹氣電磁閥開啟;
                                    break;

                                case xe_tmr_takepin.xett_植針吹氣電磁閥開啟:       
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)/10, (int)(WMX3IO對照.pxeIO_植針吹氣)%10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_植針吹氣電磁閥開啟等待1秒; 
                                    break;
                                case xe_tmr_takepin.xett_植針吹氣電磁閥開啟等待1秒:  
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=20) { 
                                        iTakePinFinishedCNT1 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_開啟流量閥1; 
                                    }
                                    break;

                                case xe_tmr_takepin.xett_開啟流量閥1: {

                                    vcb_植針吹氣流量閥.Value = 100-99;
                                    ScrollEventArgs xe = null;
                                    vcb流量閥_Scroll(vcb_植針吹氣流量閥, xe);

                                    xeTmrTakePin = xe_tmr_takepin.xett_開啟流量閥1等待1秒;
                                } break;
                                case xe_tmr_takepin.xett_開啟流量閥1等待1秒: { 
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=60) { 
                                        iTakePinFinishedCNT1 = 0;

                                        vcb_植針吹氣流量閥.Value = 100-0;
                                        ScrollEventArgs xe = null;
                                        vcb流量閥_Scroll(vcb_植針吹氣流量閥, xe);

                                        xeTmrTakePin = xe_tmr_takepin.xett_植針吹氣電磁閥關閉; 
                                    }
                                } break;

                                case xe_tmr_takepin.xett_植針吹氣電磁閥關閉:       
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)/10, (int)(WMX3IO對照.pxeIO_植針吹氣)%10, 0);

                                    if(bResume == true) {
                                        bResume = false;
                                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板再次打開;
                                    }

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
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=20) { 
                                        iTakePinFinishedCNT1 = 0;
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

                                case xe_tmr_takepin.xett_載盤XY再次移置拍照檢查位: {
                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY;

                                    dbapiCarrierX(dbTargetX, 190*0.8);
                                    dbapiCarrierY(dbTargetY, 800*0.8);

                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否再次移置拍照檢查位;
                                } break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否再次移置拍照檢查位: {
                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY;
                                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                                        if(bResume == true) {
                                            bResume = false;
                                            xeTmrTakePin = xe_tmr_takepin.xett_等待載盤XY再次移置拍照檢查位;
                                        }
                                    }
                                } break;
                                case xe_tmr_takepin.xett_等待載盤XY再次移置拍照檢查位:
                                    iTakePinFinishedCNT1++;
                                    if(iTakePinFinishedCNT1>=10) { 
                                        iTakePinFinishedCNT1 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY再次移置拍照檢查位;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認載盤XY再次移置拍照檢查位:                  
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查有無植針成功;  
                                    break;

                                case xe_tmr_takepin.xett_檢查有無植針成功:

                                    button3_Click(sender, e); {
                                        //取得校正攝影機校正參數
                                        bool success = inspector1.xInspSocket植針後檢查();
                                        label7.Text  = (success) ? "植針後檢查 OK" : "植針後檢查 NG";

                                        rtb_Status_AppendMessage(rtb_Status, $"植針 {(success ? "OK":"NG")}");
                                    }

                                    if(bResume == true) {
                                        bResume = false;
                                        xeTmrTakePin = xe_tmr_takepin.xett_檢測是否還需要取針;
                                    }
                                    break;
                    //-----------------------------------------------------------------------------------------------------------------------------------------------
                    /*  bRemove  */
                    case xe_tmr_takepin.xett_Socket1真空閥關掉: {
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空1) / 10, (int)(WMX3IO對照.pxeIO_Socket真空1) % 10, 0);
                        xeTmrTakePin = xe_tmr_takepin.xett_Socket2真空閥關掉;
                    } break;
                    case xe_tmr_takepin.xett_Socket2真空閥關掉: {
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空2) / 10, (int)(WMX3IO對照.pxeIO_Socket真空2) % 10, 0);
                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ縮回0保護;
                    } break;
                    case xe_tmr_takepin.xett_NozzleZ縮回0保護: {
                        dbapiNozzleZ(0,                  40*8); 
                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleXY回家;
                    } break;
                    case xe_tmr_takepin.xett_NozzleXY回家: {
                        double dbGetZ_1 = dbapiNozzleZ(dbRead, 0);
                        if(dbGetZ_1 <= 0.1) { 
                            dbapiNozzleX(dbNozzle安全原點X, 500*1);  
                            dbapiNozzleY(dbNozzle安全原點Y, 100*1);      
                            dbapiNozzleR(dbNozzle安全原點R, 360*8);  
                            xeTmrTakePin = xe_tmr_takepin.xett_Socket相機移至拍照位22;
                        }
                    } break;
                    case xe_tmr_takepin.xett_Socket相機移至拍照位22: {
                        dbapiIAI(22.0);
                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸縮回;
                    } break;
                    case xe_tmr_takepin.xett_擺放座Z軸縮回: {
                        dbapiSetZ(12, 33);
                        xeTmrTakePin = xe_tmr_takepin.xett_3D掃描電動缸縮回;
                    } break;
                    case xe_tmr_takepin.xett_3D掃描電動缸縮回: {
                         dbapJoDell3D掃描(10);
                        xeTmrTakePin = xe_tmr_takepin.xett_吸針嘴電動缸縮回;
                    } break;
                    case xe_tmr_takepin.xett_吸針嘴電動缸縮回: {
                        dbapJoDell吸針嘴(10);
                        xeTmrTakePin = xe_tmr_takepin.xett_吸針接料盒就位;
                    } break;
                    case xe_tmr_takepin.xett_吸針接料盒就位: {
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_接料區氣桿)       / 10, (int)(WMX3IO對照.pxeIO_接料區氣桿)       % 10, 1);
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_收料區缸)         / 10, (int)(WMX3IO對照.pxeIO_收料區缸)         % 10, 0);
                        xeTmrTakePin = xe_tmr_takepin.xett_Nozzle電磁閥關閉;
                    } break;
                    case xe_tmr_takepin.xett_Nozzle電磁閥關閉: {
                        vcb_吸嘴破真空流量閥.Value = 100-0;
                        ScrollEventArgs xe = null;
                        vcb流量閥_Scroll(vcb_吸嘴破真空流量閥, xe);

                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴吸)/10,       (int)(WMX3IO對照.pxeIO_取料吸嘴吸)%10,       0);
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)/10, (int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)%10, 0);
                        xeTmrTakePin = xe_tmr_takepin.xett_植針座電磁閥關閉;
                    } break;         
                    case xe_tmr_takepin.xett_植針座電磁閥關閉: {
                        vcb_植針吹氣流量閥.Value = 100-0;
                        ScrollEventArgs xe = null;
                        vcb流量閥_Scroll(vcb_植針吹氣流量閥, xe);

                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)/10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)%10, 0);
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座吸真空)/10, (int)(WMX3IO對照.pxeIO_擺放座吸真空)%10, 0);
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_植針吹氣)/10, (int)(WMX3IO對照.pxeIO_植針吹氣)%10, 0);
                        xeTmrTakePin = xe_tmr_takepin.xett_以上11項;
                    } break;
                    case xe_tmr_takepin.xett_以上11項: {
                        xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置抽料位;
                    } break;
                    case xe_tmr_takepin.xett_載盤XY移置抽料位: {
                        const double SetPinOffsetX =  2.796;
                        const double SetPinOffsetY =-49.011;

                        button7_Click(sender, e);

                        double dbTargetX = dbPinHolePositionX + SetPinOffsetX;
                        double dbTargetY = dbPinHolePositionY + SetPinOffsetY;

                        dbapiCarrierX(dbTargetX, 190*0.8);
                        dbapiCarrierY(dbTargetY, 800*0.8);

                        dbapiIAI(22.0);

                        xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置抽料位;
                    } break;
                    case xe_tmr_takepin.xett_檢查載盤XY是否移置抽料位: {
                        double dbX = dbapiCarrierX(dbRead, 0);
                        double dbY = dbapiCarrierY(dbRead, 0);

                        const double SetPinOffsetX =  2.796;
                        const double SetPinOffsetY =-49.011;

                        double dbTargetX = dbPinHolePositionX + SetPinOffsetX;
                        double dbTargetY = dbPinHolePositionY + SetPinOffsetY;
                        if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                            (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                            xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置抽料位;
                        }
                    } break;
                    case xe_tmr_takepin.xett_確認載盤XY移置抽料位: {
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸至抽料位;
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸至抽料位: {
                        dbapJoDell吸針嘴(14.8);
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸是否至抽料位;
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸是否至抽料位: {
                        double dbZ = dbapJoDell吸針嘴(dbRead);
                        double dbTargetZ = 14.8;
                        if( (dbTargetZ*0.99 <= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                            xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸確認至抽料位;
                        }
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸確認至抽料位: {
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料電磁閥開啟;
                    } break;
                    case xe_tmr_takepin.xett_抽料電磁閥開啟: {
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   / 10, (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   % 10, 1);
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料電磁閥開啟等待1秒;
                    } break;
                    case xe_tmr_takepin.xett_抽料電磁閥開啟等待1秒: {
                        iTakePinFinishedCNT1++;
                        if(iTakePinFinishedCNT1>=20) { 
                            iTakePinFinishedCNT1 = 0;
                            xeTmrTakePin = xe_tmr_takepin.xett_抽料電磁閥關閉; 
                        }
                    } break;
                    case xe_tmr_takepin.xett_抽料電磁閥關閉: {
                        clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   / 10, (int)(WMX3IO對照.pxeIO_吸料真空電磁閥)   % 10, 0);
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸回0; 
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸回0: {
                        dbapJoDell吸針嘴(10);
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸是否回0; 
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸是否回0: {
                        double dbZ = dbapJoDell吸針嘴(dbRead);
                        double dbTargetZ = 10;
                        if( (dbTargetZ*0.99 <= dbZ && dbZ <= dbTargetZ*1.01) ) { 
                            xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸確認回0;
                        }
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸確認回0: {
                        xeTmrTakePin = xe_tmr_takepin.xett_檢查有無抽針成功;
                    } break;
                    case xe_tmr_takepin.xett_檢查有無抽針成功: {
                        xeTmrTakePin = xe_tmr_takepin.xett_檢測是否還需要取針;
                    } break;
                    //-----------------------------------------------------------------------------------------------------------------------------------------------                                    
                    case xe_tmr_takepin.xett_檢測是否還需要取針: {    
                        int 求出取料循環次數 = int.Parse(txt_取料循環.Text);
                        求出取料循環次數--;
                        txt_取料循環.Text = 求出取料循環次數.ToString();
                        if(求出取料循環次數>=1 && btmrStop==false) { 
                            xeTmrTakePin = xe_tmr_takepin.xett_還需要取針;  
                        } else {
                            btmrStop = false;
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
                        bRemove    = false;
                        xeTmrTakePin = xe_tmr_takepin.xett_Empty;  
                        break;
                case xe_tmr_takepin.xett_End:           
                    xeTmrTakePin = xe_tmr_takepin.xett_Empty;  
                    break;
            }
        }  // end of public void tmr_TakePin_Tick(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //-------------------------------- State Machine implement ------------------------------
        //---------------------------------------------------------------------------------------


        #region 和尚小佛
        //---------------------------------------------------------------------------------------
        //---------------------------------------- 和尚小佛 --------------------------------------
        //---------------------------------------------------------------------------------------
        public void tsmi_OpenFile_Click(object sender, EventArgs e)
        { 
            if (OpenFile())
            {
                tsmi_SaveFile.Enabled = true;
                btn_SaveFile.Enabled = true;

                show_grp_BarcodeInfo(grp_BarcodeInfo);

                find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                pic_Needles.Refresh();
            }
        }
        //---------------------------------------------------------------------------------------
        public void tsmi_SaveFile_Click(object sender, EventArgs e)
        {
            Viewer.SaveFile();
        }
        //---------------------------------------------------------------------------------------
        public void pic_Needles_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(ZoomFactor, -ZoomFactor);
            e.Graphics.TranslateTransform(Offset.X / ZoomFactor, Offset.Y / -ZoomFactor); // 拖曳圖片轉換座標

            #region 畫出所有圓
            foreach (var circle in Json.Needles)
            {
                Brush fillBrush;

                RectangleF rectangleF = new RectangleF(
                    (float)(circle.X * ScaleFactor - circle.Diameter / 2 * ScaleFactor),
                    (float)(circle.Y * ScaleFactor - circle.Diameter / 2 * ScaleFactor),
                    (float)(2 * circle.Diameter / 2 * ScaleFactor),
                    (float)(2 * circle.Diameter / 2 * ScaleFactor)
                );

                if (circle.Display == false) // 隱藏圓
                {
                    fillBrush = new SolidBrush(HiddenNeedlesColor);
                }
                else if (circle.Place == true) // 植針圓
                {
                    fillBrush= new SolidBrush(PlaceNeedlesColor);
                }
                else // 預設圓
                {
                    fillBrush = new SolidBrush(DefaltNeedleColor);
                }

                if (circle == FocusedNeedle) // 點擊圓
                {
                    fillBrush = new SolidBrush(FocusedNeedleColor);
                }
                else if (circle == HighlightedNeedle) // 觸擊圓
                {
                    fillBrush = new SolidBrush(HiddenNeedlesColor);
                    //rectangleF = new RectangleF(
                    //    (float)((circle.X * ScaleFactor - circle.Diameter / 2 * ScaleFactor) - (circle.Diameter / 2 * ScaleFactor * 0.5)),
                    //    (float)((circle.Y * ScaleFactor - circle.Diameter / 2 * ScaleFactor) - (circle.Diameter / 2 * ScaleFactor * 0.5)),
                    //    (float)(2 * circle.Diameter / 2 * ScaleFactor * 1.5),
                    //    (float)(2 * circle.Diameter / 2 * ScaleFactor * 1.5)
                    //);
                }

                e.Graphics.FillEllipse(fillBrush, rectangleF);

            }
            #endregion

            #region 畫拖曳框
            if (IsDrag)
            {
                // 設置半透明框的顏色 (Alpha 值為 128，表示半透明)
                Color DragBoxColor = Color.FromArgb(128, 0, 0, 255);
                Brush DragBoxBrush = new SolidBrush(DragBoxColor);

                RectangleF DragBox = new RectangleF(
                    Drag_Boundary.minX,
                    Drag_Boundary.minY, 
                    Drag_Boundary.width,
                    Drag_Boundary.height
                );

                e.Graphics.FillRectangle(DragBoxBrush, DragBox);
            }
            #endregion

            #region 畫框選中的圓

            foreach (var circle in SelectedNeedles)
            {
                Brush fillBrush;

                fillBrush = new SolidBrush(SelectedNeedlesColor);
                
                RectangleF rectangleF = new RectangleF(
                    (float)(circle.X * ScaleFactor - circle.Diameter / 2 * ScaleFactor),
                    (float)(circle.Y * ScaleFactor - circle.Diameter / 2 * ScaleFactor),
                    (float)(2 * circle.Diameter / 2 * ScaleFactor),
                    (float)(2 * circle.Diameter / 2 * ScaleFactor)
                );

                e.Graphics.FillEllipse(fillBrush, rectangleF);
            }
            #endregion
        }
        //---------------------------------------------------------------------------------------
        public void pic_Needles_MouseMove(object sender, MouseEventArgs e)
        {
            RealMousePos.X = (e.X - Offset.X) / ZoomFactor ;
            RealMousePos.Y = -(e.Y - Offset.Y) / ZoomFactor ;

            lbl_RealMousePos.Text = "真實座標 : " + RealMousePos.ToString();  
            lbl_PicMousePos.Text = "繪圖座標 : " + e.Location.ToString();
            lbl_Offset.Text = "Offset : " + Offset.ToString();
            lbl_ZoomFactor.Text = "縮放比例 : " + ZoomFactor.ToString();

            // 左鍵移動顯示位置
            if (e.Button == MouseButtons.Left)
            {
                switch (Control.ModifierKeys)
                {
                    case Keys.Shift:
                        Drag_End.X = (e.X - Offset.X) / ZoomFactor;
                        Drag_End.Y = -(e.Y - Offset.Y) / ZoomFactor;
                        find_Drag_Boundary();

                        break;
                    default:
                        // 計算滑鼠移動的差值
                        Offset.X += e.X - PrevMousePos.X;
                        Offset.Y += e.Y - PrevMousePos.Y;

                        PrevMousePos = e.Location; // 拖曳當中隨時紀錄當下滑鼠在 PictureBox 上的位置, 不以左鍵點擊當下的位置
                        break;
                }
            }


            foreach (var circle in Viewer.Json.Needles)
            {
                // 计算鼠标位置与圆心的距离
                Mouse2CircleDistance = Math.Sqrt(
                    Math.Pow((e.X - Offset.X) / ScaleFactor / ZoomFactor - circle.X, 2) +
                    Math.Pow((e.Y - Offset.Y) / ScaleFactor / -ZoomFactor - circle.Y, 2)
                );

                if (Mouse2CircleDistance <= circle.Diameter / 2)
                {
                    IsMouseinCircle = true;

                    HighlightedNeedle = circle; // 記錄高亮的圓

                    break;
                }
                else
                {
                    IsMouseinCircle = false;
                    HighlightedNeedle = null;
                }
            }

            if (IsMouseinCircle) {
                ttp_NeedleInfo.SetToolTip(
                    pic_Needles,
                    "流水號 : " + HighlightedNeedle.Index.ToString() + "\n" +
                    "名稱 : " + (HighlightedNeedle.Name ?? "無") + "\n" +  // 如果為 null, 顯示 "無"
                    "Id : " + (HighlightedNeedle.Id ?? "無") + "\n" +
                    "座標X : " + HighlightedNeedle.X.ToString("F3") + "\n" +
                    "座標Y : " + HighlightedNeedle.Y.ToString("F3") + "\n" +
                    "直徑 : " + HighlightedNeedle.Diameter.ToString("F3") + "\n" 
                );
            }
            else
            {
                ttp_NeedleInfo.SetToolTip(pic_Needles, string.Empty);  // 清除提示
            }

            pic_Needles.Refresh();
        }
        //---------------------------------------------------------------------------------------
        public void pic_Needles_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (Control.ModifierKeys)
                {
                    case Keys.Shift:

                        clear_grp_NeedleInfo(grp_NeedleInfo);

                        if (!IsDrag)
                        {
                            Drag_Start.X = (e.X - Offset.X) / ZoomFactor;
                            Drag_Start.Y = -(e.Y - Offset.Y) / ZoomFactor;
                            IsDrag = true;
                        }

                        break;

                    default:
                        SelectedNeedles.Clear(); // 清空拖曳框選擇到的圓

                        PrevMousePos = e.Location;

                        if (HighlightedNeedle != null)
                        {
                            FocusedNeedle = HighlightedNeedle;

                            show_grp_NeedleInfo(grp_NeedleInfo);
                        }
                        else
                        {
                            FocusedNeedle = null;

                            clear_grp_NeedleInfo(grp_NeedleInfo);
                        }
                        break;
                }
            }
        }
        //---------------------------------------------------------------------------------------
        public void pic_Needles_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (Control.ModifierKeys)
                {
                    case Keys.Shift:
                        if (IsDrag)
                        {
                            find_Selected_Needles();
                            pic_Needles.Refresh();
                        }

                        break;
                }

                IsDrag = false; // 這裡不能寫在 case 裡面, 如果拖曳中途先把 Shift 放掉, 就無法清除 Flag
            }
        }
        //---------------------------------------------------------------------------------------
        public void pic_Needles_MouseWheel(object sender, MouseEventArgs e)
        {

            // 滑鼠在 PictureBox 上的位置對應的真實座標（縮放前）
            RealMousePosBeforeZoom.X = (e.X - Offset.X) / ZoomFactor;
            RealMousePosBeforeZoom.Y = (e.Y - Offset.Y) / -ZoomFactor;

            if (e.Delta > 0)
            {
                ZoomFactor *= 1.1f; // 滾輪向上，放大
            }
            else if (e.Delta < 0)
            {
                if (ZoomFactor > 1) // 最小就 1 倍
                {
                    ZoomFactor /= 1.1f; // 滾輪向下，縮小
                }
            }

            // 滑鼠在 PictureBox 上的位置對應的真實座標（縮放後）
            RealMousePosAfterZoom.X = (e.X - Offset.X) / ZoomFactor;
            RealMousePosAfterZoom.Y = (e.Y - Offset.Y) / -ZoomFactor;

            // 根據縮放前後的真實座標差異調整偏移量
            Offset.X += (RealMousePosAfterZoom.X - RealMousePosBeforeZoom.X) * ZoomFactor;
            Offset.Y += (RealMousePosAfterZoom.Y - RealMousePosBeforeZoom.Y) * -ZoomFactor;

            pic_Needles.Refresh();
        }
        //---------------------------------------------------------------------------------------
        public void cms_pic_Needles_Opened(object sender, EventArgs e)
        {
            if (SelectedNeedles.Count != 0)
            {
                tsmi_Place.Enabled    = true;
                tsmi_Remove.Enabled   = true;
                tsmi_Replace.Enabled  = true;
                tsmi_Display.Enabled  = true;
                tsmi_Enable.Enabled   = true;
                tsmi_Reset.Enabled    = true;
                tsmi_Reserve1.Enabled = true;
            }
            else
            {
                tsmi_Place.Enabled    = false;
                tsmi_Remove.Enabled   = false;
                tsmi_Replace.Enabled  = false;
                tsmi_Display.Enabled  = false;
                tsmi_Enable.Enabled   = false;
                tsmi_Reset.Enabled    = false;
                tsmi_Reserve1.Enabled = false;
            }
        }
        //---------------------------------------------------------------------------------------
        public void cms_pic_Needles_ItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            foreach (var circle in SelectedNeedles)
            {
                switch (item.Text)
                {
                    case "植針":
                        Json.Needles[circle.Index].Place = true;
                        break;

                    case "取針":
                        Json.Needles[circle.Index].Remove = true;
                        break;

                    case "置換":
                        Json.Needles[circle.Index].Replace = true;
                        break;

                    case "顯示":
                        Json.Needles[circle.Index].Display = true;
                        break;

                    case "啟用":
                        Json.Needles[circle.Index].Enable = true;
                        break;

                    case "保留":
                        Json.Needles[circle.Index].Reserve1 = true;
                        break;

                    case "清除":
                        Json.Needles[circle.Index].Place    = false;
                        Json.Needles[circle.Index].Remove   = false;
                        Json.Needles[circle.Index].Replace  = false;
                        Json.Needles[circle.Index].Display  = true;
                        Json.Needles[circle.Index].Enable   = false;
                        Json.Needles[circle.Index].Reserve1 = false;
                        break;
                }
            }
        }
        //---------------------------------------------------------------------------------------
        public void grp_NeedleInfo_ChildControlChanged(object sender, EventArgs e)
        {
            if (FocusedNeedle != null)
            {
                switch (sender)
                {
                    case TextBox textBox:

                        switch (textBox.Name)
                        {
                            case "txt_Name":
                                Json.Needles[FocusedNeedle.Index].Name = txt_Name.Text;
                                break;

                            case "txt_Id":
                                Json.Needles[FocusedNeedle.Index].Id = txt_Id.Text;
                                break;
                        }
                        break;

                    case RadioButton radioButton:

                        switch (radioButton.Name)
                        {
                            case "rad_Place":
                                Json.Needles[FocusedNeedle.Index].Place = rad_Place.Checked;
                                //dgv_Needles.Rows[FocusedNeedle.Index].Cells["Place"].Value = rad_Place.Checked;
                                break;

                            case "rad_Remove":
                                Json.Needles[FocusedNeedle.Index].Remove = rad_Remove.Checked;
                                break;

                            case "rad_Replace":
                                Json.Needles[FocusedNeedle.Index].Replace = rad_Replace.Checked;
                                break;
                        }

                        break;

                    case CheckBox checkBox:

                        switch (checkBox.Name)
                        {
                            case "chk_Display":
                                Json.Needles[FocusedNeedle.Index].Display = chk_Display.Checked;
                                break;

                            case "chk_Enable":
                                Json.Needles[FocusedNeedle.Index].Enable = chk_Enable.Checked;
                                break;

                            case "chk_Reserve1":
                                Json.Needles[FocusedNeedle.Index].Reserve1 = chk_Reserve1.Checked;
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }
        }
        //---------------------------------------------------------------------------------------
        public void grp_NeedleInfo_Search(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (sender is TextBox textbox)
                {
                    Viewer.search_grp_NeedleInfo(textbox.Name, textbox.Text);
                    Viewer.show_grp_NeedleInfo(grp_NeedleInfo);
                    pic_Needles.Refresh();
                }
            }
        }
        //---------------------------------------------------------------------------------------
        public void chk_Display_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_Display.Checked)
            {
                chk_Display.BackColor = Color.Red;
            }
            else
            {
                chk_Display.BackColor = SystemColors.Control;
            }
        }
        //---------------------------------------------------------------------------------------
        public void chk_Enable_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_Enable.Checked)
            {
                chk_Enable.BackColor = Color.Red;
            }
            else
            {
                chk_Enable.BackColor = SystemColors.Control;
            }
        }
        //---------------------------------------------------------------------------------------
        public void chk_Reserve1_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_Reserve1.Checked)
            {
                chk_Reserve1.BackColor = Color.Red;
            }
            else
            {
                chk_Reserve1.BackColor = SystemColors.Control;
            }
        }
        //-------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void grp_BarcodeInfo_ChildControlChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            switch (textBox.Name)
            {
                case "txt_Barcode":
                    Json.Barcode.Barcode = txt_Barcode.Text;
                    break;

                case "txt_短編號":
                    Json.Barcode.短編號 = txt_短編號.Text;
                    break;

                case "txt_客戶":
                    Json.Barcode.客戶 = txt_客戶.Text;
                    break;

                case "txt_型號":
                    Json.Barcode.型號 = txt_型號.Text;
                    break;

                case "txt_板全號":
                    Json.Barcode.板全號 = txt_板全號.Text;
                    break;

                case "txt_儲位":
                    Json.Barcode.儲位 = txt_儲位.Text;
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        public void btn_OpenFile_Click(object sender, EventArgs e)
        {
            tsmi_SaveFile.Enabled = true;
            btn_SaveFile.Enabled = true;

            strFileName = new string(BarcodeBuffer.ToArray()).Trim(); 
            try
            {
                Json = JsonConvert.DeserializeObject<JSON>(File.ReadAllText(@"028\" + strFileName + ".json"));
                show_grp_BarcodeInfo(grp_BarcodeInfo);

                //MessageBox.Show($"檔案 {@"028\" + txt_Barcode.Text + ".json"} 成功讀取！");
                rtb_Status_AppendMessage(rtb_Status, $"檔案 {@"028\" + strFileName + ".json"} 成功讀取！");

                find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                pic_Needles.Refresh();
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"讀取 Json 檔時發生錯誤: {ex.Message}");
            }
        }
        //---------------------------------------------------------------------------------------
        public void btn_SaveFile_Click(object sender, EventArgs e)
        {
            // 使用 Newtonsoft.Json 進行物件序列化，並設定格式化輸出（會縮排顯示）
            string json = JsonConvert.SerializeObject(Json, Newtonsoft.Json.Formatting.Indented);
            // 使用 StreamWriter 儲存 Json 到選定的檔案
            strFileName = txt_Barcode.Text + ".json";

            using (StreamWriter writer = new StreamWriter(@"028\" + strFileName))
            {
                writer.Write(json);
            }

            MessageBox.Show("檔案儲存成功！");
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------- 和尚小佛 --------------------------------------
        //---------------------------------------------------------------------------------------
        #endregion


        #region 暫時或實驗中
        //---------------------------------------------------------------------------------------
        //-------------------------------------- 暫時或實驗中 ------------------------------------
        //---------------------------------------------------------------------------------------
        public void button1_Click(object sender, EventArgs e)
        {
            //inspector1.xInit();
        }
        //---------------------------------------------------------------------------------------
        private void btn_socket相機兩點定位_Click(object sender, EventArgs e)
        {
            Vector3 pos;
            bool success = inspector1.xInspSocket校正孔(out pos);
            label16.Text = string.Format("Socket校正孔 = {0}, X = {1:F3} , Y = {2:F3}", success, pos.X, pos.Y);
        }
        //---------------------------------------------------------------------------------------
        //-------------------------------------- 暫時或實驗中 ------------------------------------
        //---------------------------------------------------------------------------------------
        #endregion


    }  // end of public partial class Form1 : Form
    //---------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------
    public class GlobalKeyboardHook
    {
        // 鍵盤掛勾的委派
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public HookProc hookProc;

        // 鍵盤掛勾句柄
        public IntPtr hookID = IntPtr.Zero;

        // 鍵盤事件
        public event EventHandler<KeyEventArgs> KeyUp;

        // 掛勾類型
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYUP = 0x0101;

        //---------------------------------------------------------------------------------------
        public GlobalKeyboardHook()
        {
            hookProc = HookCallback;
            hookID = SetHook(hookProc);
        }
        //---------------------------------------------------------------------------------------
        ~GlobalKeyboardHook()
        {
            UnhookWindowsHookEx(hookID);
        }
        //---------------------------------------------------------------------------------------
        public IntPtr SetHook(HookProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        //---------------------------------------------------------------------------------------
        public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // 如果 Form1 在當前活動狀態，才觸發事件
                if (isFormActive) {
                    KeyUp?.Invoke(this, new KeyEventArgs((Keys)vkCode));
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
        //---------------------------------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
    //---------------------------------------------------------------------------------------
}  // end of namespace InjectorInspector
