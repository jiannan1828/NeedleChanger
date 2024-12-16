
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

        public int Multiplier = 4;
        public int u8OneCycleFlag = 0;


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
            double dbRstNozzleX = 0.0;

            {  //吸嘴X軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 吸嘴X軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴X軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstNozzleX = double.Parse(position) / 100.0;
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_吸嘴X軸.BackColor = Color.Red;
                    lbl_acpos_吸嘴X軸.BackColor = Color.White;
                    lbl_spd_吸嘴X軸.BackColor = Color.White;
                }
                else
                {
                    select_吸嘴X軸.BackColor = Color.Green;
                    lbl_acpos_吸嘴X軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴X軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_吸嘴X軸.Text = dbRstNozzleX.ToString("F3");
                lbl_spd_吸嘴X軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_吸嘴X軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴X軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴X軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -4;
                        int MinRAW = 51073;
                        double Maxdb = 0.0;
                        double Mindb = 500.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    
                        lbl_吸嘴X軸_RAW.Text = Convert.ToString();
                        lbl_吸嘴X軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_吸嘴X軸_Back.Text = cnback.ToString();
                    }
                
            }

            if (dbIncreaseNozzleX == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //吸嘴X軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeNozzleX = dbIncreaseNozzleX;

                //伸長量overflow保護
                //if (fChangeNozzleX >= 40.35) {
                //    fChangeNozzleX = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetNozzleX = (int)(fChangeNozzleX * 100);

                //執行移動吸嘴
                int axis = (int)WMX3軸定義.吸嘴X軸;
                int position = iTargetNozzleX;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstNozzleX;
        }  // end of public double dbapiNozzleX(double dbIncreaseNozzleX)  //NozzleX
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleY(double dbIncreaseNozzleY)  //NozzleY
        {
            double dbRstNozzleY = 0.0;

            {  //吸嘴Y軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 吸嘴Y軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstNozzleY = double.Parse(position) / 100.0;
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_吸嘴Y軸.BackColor = Color.Red;
                    lbl_acpos_吸嘴Y軸.BackColor = Color.White;
                    lbl_spd_吸嘴Y軸.BackColor = Color.White;
                }
                else
                {
                    select_吸嘴Y軸.BackColor = Color.Green;
                    lbl_acpos_吸嘴Y軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴Y軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_吸嘴Y軸.Text = dbRstNozzleY.ToString("F3");
                lbl_spd_吸嘴Y軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_吸嘴Y軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴Y軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴Y軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -1005;
                        int MinRAW = 10875;
                        double Maxdb = 0.0;
                        double Mindb = 100.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_吸嘴Y軸_RAW.Text = Convert.ToString();
                        lbl_吸嘴Y軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_吸嘴Y軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseNozzleY == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //吸嘴X軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeNozzleY = dbIncreaseNozzleY;

                //伸長量overflow保護
                //if (fChangeNozzleY >= 40.35) {
                //    fChangeNozzleY = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetNozzleY = (int)(fChangeNozzleY * 100);

                //執行移動吸嘴
                int axis = (int)WMX3軸定義.吸嘴Y軸;
                int position = iTargetNozzleY;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstNozzleY;
        }  // end of public double dbapiNozzleY(double dbIncreaseNozzleY)  //NozzleY
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleZ(double dbIncreaseNozzleZ)  //NozzleZ
        {
            double dbRstNozzleZ = 0.0;

            {  //吸嘴Z軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 吸嘴Z軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstNozzleZ = double.Parse(position) / 1000.0;
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_吸嘴Z軸.BackColor = Color.Red;
                    lbl_acpos_吸嘴Z軸.BackColor = Color.White;
                    lbl_spd_吸嘴Z軸.BackColor = Color.White;
                }
                else
                {
                    select_吸嘴Z軸.BackColor = Color.Green;
                    lbl_acpos_吸嘴Z軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴Z軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_吸嘴Z軸.Text = dbRstNozzleZ.ToString("F3");
                lbl_spd_吸嘴Z軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_吸嘴Z軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴Z軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -93;
                        int MinRAW = 41496;
                        double Maxdb = 0.0;
                        double Mindb = 40.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_吸嘴Z軸_RAW.Text = Convert.ToString();
                        lbl_吸嘴Z軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_吸嘴Z軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseNozzleZ == dbRead)
            {

            }
            else
            {  //吸嘴Z軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeNozzleZ = dbIncreaseNozzleZ;

                //伸長量overflow保護
                if (fChangeNozzleZ >= 40.35) {
                    fChangeNozzleZ = 40.35;
                }

                //計算補正至長度的數值
                int iTargetNozzleZ = (int)(fChangeNozzleZ * 1000);

                //執行伸縮吸嘴
                int axis = (int)WMX3軸定義.吸嘴Z軸;
                int position = iTargetNozzleZ;
                int speed = (int)(40.00 * 1000 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstNozzleZ;
        }  // end of public double dbapiNozzleZ(double dbIncreaseNozzleZ)  //NozzleZ
        //---------------------------------------------------------------------------------------
        public double dbapiNozzleR(double dbIncreaseDegree)  //NozzleR
        {
            double dbRstNozzleR = 0.0;

            {  //吸嘴R軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 吸嘴R軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.吸嘴R軸, ref position, ref speed);

                //計算讀取角度
                if (position != "")
                {
                    dbRstNozzleR = double.Parse(position) / 100.0;
                }

                //overflow
                while (dbRstNozzleR >= 360.0)
                {
                    dbRstNozzleR -= 360.0;
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_吸嘴R軸.BackColor = Color.Red;
                    lbl_acpos_吸嘴R軸.BackColor = Color.White;
                    lbl_spd_吸嘴R軸.BackColor = Color.White;
                }
                else
                {
                    select_吸嘴R軸.BackColor = Color.Green;
                    lbl_acpos_吸嘴R軸.BackColor = Color.Gray;
                    lbl_spd_吸嘴R軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_吸嘴R軸.Text = dbRstNozzleR.ToString("F3");
                lbl_spd_吸嘴R軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_吸嘴R軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴R軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_吸嘴R軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -11880;
                        int MinRAW = 24320;
                        double Maxdb = 0.0;
                        double Mindb = 360.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_吸嘴R軸_RAW.Text = Convert.ToString();
                        lbl_吸嘴R軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_吸嘴R軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseDegree == dbRead)
            {

            }
            else
            {  //吸嘴R軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeDegree = dbIncreaseDegree;

                //計算補正至角度的數值
                int iTargetDeg = (int)(fChangeDegree * 100);

                //執行旋轉吸嘴
                int axis = (int)WMX3軸定義.吸嘴R軸;
                int position = iTargetDeg;
                int speed = (int)(360.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstNozzleR;
        }  // end of public double dbapiNozzleR(double dbIncreaseDegree)  //NozzleR
        //---------------------------------------------------------------------------------------
        public double dbapiCarrierX(double dbIncreaseCarrierX)  //CarrierX
        {
            double dbRstCarrierX = 0.0;

            {  //載盤X軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 載盤X軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.載盤X軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstCarrierX = double.Parse(position);
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_載盤X軸.BackColor = Color.Red;
                    lbl_acpos_載盤X軸.BackColor = Color.White;
                    lbl_spd_載盤X軸.BackColor = Color.White;
                }
                else
                {
                    select_載盤X軸.BackColor = Color.Green;
                    lbl_acpos_載盤X軸.BackColor = Color.Gray;
                    lbl_spd_載盤X軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_載盤X軸.Text = dbRstCarrierX.ToString("F3");
                lbl_spd_載盤X軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_載盤X軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_載盤X軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_載盤X軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -39198;
                        int MinRAW = 1;
                        double Maxdb = 0.0;
                        double Mindb = 190.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_載盤X軸_RAW.Text = Convert.ToString();
                        lbl_載盤X軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_載盤X軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseCarrierX == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //載盤X軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeCarrierX = dbIncreaseCarrierX;

                //伸長量overflow保護
                //if (fChangeCarrierX >= 40.35) {
                //    fChangeCarrierX = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetCarrierX = (int)(fChangeCarrierX * 100);

                //執行移動載盤
                int axis = (int)WMX3軸定義.載盤X軸;
                int position = iTargetCarrierX;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstCarrierX;
        }  // end of public double dbapiCarrierX(double dbIncreaseCarrierX)  //CarrierX
        //---------------------------------------------------------------------------------------
        public double dbapiCarrierY(double dbIncreaseCarrierY)  //CarrierY
        {
            double dbRstCarrierY = 0.0;

            {  //載盤Y軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 載盤Y軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.載盤Y軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstCarrierY = double.Parse(position);
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_載盤Y軸.BackColor = Color.Red;
                    lbl_acpos_載盤Y軸.BackColor = Color.White;
                    lbl_spd_載盤Y軸.BackColor = Color.White;
                }
                else
                {
                    select_載盤Y軸.BackColor = Color.Green;
                    lbl_acpos_載盤Y軸.BackColor = Color.Gray;
                    lbl_spd_載盤Y軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_載盤Y軸.Text = dbRstCarrierY.ToString("F3");
                lbl_spd_載盤Y軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_載盤Y軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_載盤Y軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_載盤Y軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -809888;
                        int MinRAW = -50;
                        double Maxdb = 0.0;
                        double Mindb = 800.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_載盤Y軸_RAW.Text = Convert.ToString();
                        lbl_載盤Y軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_載盤Y軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseCarrierY == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //載盤Y軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeCarrierY = dbIncreaseCarrierY;

                //伸長量overflow保護
                //if (fChangeCarrierY >= 40.35) {
                //    fChangeCarrierY = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetCarrierY = (int)(fChangeCarrierY * 100);

                //執行移動載盤
                int axis = (int)WMX3軸定義.載盤Y軸;
                int position = iTargetCarrierY;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstCarrierY;
        }  // end of public double dbapiCarrierY(double dbIncreaseCarrierY)  //CarrierY
        //---------------------------------------------------------------------------------------
        public double dbapiSetZ(double dbIncreaseSetZ)  //SetZ
        {
            double dbRstSetZ = 0.0;

            {  //植針Z軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 植針Z軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.植針Z軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstSetZ = double.Parse(position);
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_植針Z軸.BackColor = Color.Red;
                    lbl_acpos_植針Z軸.BackColor = Color.White;
                    lbl_spd_植針Z軸.BackColor = Color.White;
                }
                else
                {
                    select_植針Z軸.BackColor = Color.Green;
                    lbl_acpos_植針Z軸.BackColor = Color.Gray;
                    lbl_spd_植針Z軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_植針Z軸.Text = dbRstSetZ.ToString("F3");
                lbl_spd_植針Z軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_植針Z軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_植針Z軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = -7045;
                        int MinRAW = 13;
                        double Maxdb = 0.0;
                        double Mindb = 30.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_植針Z軸_RAW.Text = Convert.ToString();
                        lbl_植針Z軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_植針Z軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseSetZ == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //植針Z軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeSetZ = dbIncreaseSetZ;

                //伸長量overflow保護
                //if (fChangeSetZ >= 40.35) {
                //    fChangeSetZ = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetSetZ = (int)(fChangeSetZ * 100);

                //執行移動植針Z軸
                int axis = (int)WMX3軸定義.植針Z軸;
                int position = iTargetSetZ;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstSetZ;
        }  // end of public double dbapiSetZ(double dbIncreaseSetZ)  //SetZ
        //---------------------------------------------------------------------------------------
        public double dbapiSetR(double dbIncreaseSetR)  //SetR
        {
            double dbRstSetR = 0.0;

            {  //植針R軸 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 植針R軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.植針R軸, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstSetR = double.Parse(position)/100.0;
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed)/100.0;
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_植針R軸.BackColor = Color.Red;
                    lbl_acpos_植針R軸.BackColor = Color.White;
                    lbl_spd_植針R軸.BackColor = Color.White;
                }
                else
                {
                    select_植針R軸.BackColor = Color.Green;
                    lbl_acpos_植針R軸.BackColor = Color.Gray;
                    lbl_spd_植針R軸.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_植針R軸.Text = dbRstSetR.ToString("F3");
                lbl_spd_植針R軸.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_植針R軸_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_植針R軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_植針R軸_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = 1000;
                        int MinRAW = 0;
                        double Maxdb = 1000.0;
                        double Mindb = 0.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_植針R軸_RAW.Text = Convert.ToString();
                        lbl_植針R軸_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_植針R軸_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseSetR == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //植針R軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeSetR = dbIncreaseSetR;

                //伸長量overflow保護
                //if (fChangeSetR >= 40.35) {
                //    fChangeSetR = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetSetR = (int)(fChangeSetR * 100);

                //執行移動植針R軸
                int axis = (int)WMX3軸定義.植針R軸;
                int position = iTargetSetR;
                int speed = (int)(dbAxisRGearRatio * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstSetR;
        }  // end of public double dbapiSetR(double dbIncreaseSetR)  //SetR
        //---------------------------------------------------------------------------------------
        public double dbapiGate(double dbIncreaseGate)  //Gate
        {
            double dbRstGate = 0.0;

            {  //工作門 讀取與顯示
                int rslt = 0;
                string position = "";
                string speed = "";

                //讀取 工作門 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.工作門, ref position, ref speed);

                //計算讀取長度
                if (position != "")
                {
                    dbRstGate = double.Parse(position);
                }

                //顯示運動速度
                double dbSpeed = 0.0;
                if (speed != "")
                {
                    dbSpeed = double.Parse(speed);
                }

                //變更顏色
                if (rslt == 1)
                {
                    select_工作門.BackColor = Color.Red;
                    lbl_acpos_工作門.BackColor = Color.White;
                    lbl_spd_工作門.BackColor = Color.White;
                }
                else
                {
                    select_工作門.BackColor = Color.Green;
                    lbl_acpos_工作門.BackColor = Color.Gray;
                    lbl_spd_工作門.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_工作門.Text = dbRstGate.ToString("F3");
                lbl_spd_工作門.Text = dbSpeed.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_工作門_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_工作門_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_工作門_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                    if (position != "")
                    {
                        int MaxRAW = 56580;
                        int MinRAW = -1271;
                        double Maxdb = 575.0;
                        double Mindb = 0.0;
                        Normal calculate = new Normal();
                        int Convert = (int)(double.Parse(position));
                        double dbGet = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);

                        lbl_工作門_RAW.Text = Convert.ToString();
                        lbl_工作門_Convert.Text = dbGet.ToString("F3");
                        int cnback = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, MaxRAW, MinRAW);
                        lbl_工作門_Back.Text = cnback.ToString();
                    }

            }

            if (dbIncreaseGate == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //工作門 變更位置
                // 取得欲變更的的浮點數
                double fChangeGate = dbIncreaseGate;

                //伸長量overflow保護
                //if (fChangeGate >= 40.35) {
                //    fChangeGate = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetGate = (int)(fChangeGate * 100);

                //執行移動工作門
                int axis = (int)WMX3軸定義.工作門;
                int position = iTargetGate;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstGate;
        }  // end of public double dbapiGate(double dbIncreaseGate)  //Gate
        //---------------------------------------------------------------------------------------
        public double dbapiIAI(double dbIncreaseGate)  //IAI
        {
            double dbRstIAI = 0.0;

            {  //Socket定位攝影機軸 讀取與顯示
                int rslt = 0;

                //讀取 Socket定位攝影機軸 資訊
                byte[] aGetGetIAI = new byte[2];
                clsServoControlWMX3.WMX3_GetInIO(ref aGetGetIAI, (int)(addr_IAI.pxeaI_GetStatusSignal2_2Bytes) / 10, 2);
                rslt += ((aGetGetIAI[(int)(addr_IAI.pxeaI_GetServoONState - addr_IAI.pxeaI_GetStatusSignal2_2Bytes) / 10] & (1 << (int)(addr_IAI.pxeaI_GetServoONState) % 10)) != 0) ? 1 : 0;

                byte[] aGetSetIAI = new byte[2];
                clsServoControlWMX3.WMX3_GetOutIO(ref aGetSetIAI, (int)(addr_IAI.pxeaI_SetControlSignal2_2Bytes) / 10, 2);
                rslt += ((aGetSetIAI[(int)(addr_IAI.pxeaI_SetDisableBrake - addr_IAI.pxeaI_SetControlSignal2_2Bytes) / 10] & (1 << (int)(addr_IAI.pxeaI_SetDisableBrake) % 10)) != 0) ? 1 : 0;

                //計算讀取長度
                int iIAIpos = clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GetPosition, 0);
                double dbIAIpos = (double)iIAIpos / 100.0;
                dbRstIAI = dbIAIpos;

                //顯示運動速度
                int iIAIspd = clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GetCurrentSpeed4Bytes, 0);
                double dbIAIspd = (double)iIAIspd / 100.0;

                //變更顏色
                if (rslt == 2)
                {
                    select_IAI.BackColor = Color.Red;
                    lbl_acpos_IAI.BackColor = Color.White;
                    lbl_spd_IAI.BackColor = Color.White;
                }
                else
                {
                    select_IAI.BackColor = Color.Green;
                    lbl_acpos_IAI.BackColor = Color.Gray;
                    lbl_spd_IAI.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_IAI.Text = dbIAIpos.ToString("F3");
                lbl_spd_IAI.Text = dbIAIspd.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_IAI_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_IAI_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_IAI_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                {
                    lbl_IAI_RAW.Text = iIAIpos.ToString();
                    lbl_IAI_Convert.Text = dbIAIpos.ToString("F3");
                    lbl_IAI_Back.Text = ((int)(dbIAIpos * 100)).ToString();
                }

            }

            if (dbIncreaseGate == dbRead)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //IAI 變更位置
                // 取得欲變更的的浮點數
                double fChangeGate = dbIncreaseGate;

                //伸長量overflow保護
                if (fChangeGate >= 30.1) {
                    fChangeGate = 30.1;
                }

                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 1);

                //執行移動工作門
                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn, 1);
                clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstIAI;
        }  // end of public double dbapiGate(double dbIncreaseGate)  //IAI
        //---------------------------------------------------------------------------------------
        public double dbapJoDell3D掃描(double dbIncreaseGate)  //JoDell3D掃描
        {
            double dbRstJoDell3D掃描 = 0.0;

            {  //JoDell植針嘴 讀取與顯示
                int rslt = 0;

                //讀取 JoDell3D掃描 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice = (int)(addr_JODELL.pxeaJ_3D掃描_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //計算讀取長度
                int iJoDell3D掃描pos = clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_GetPosition, 0);
                double dbJoDell3D掃描pos = (double)iJoDell3D掃描pos / 100.0;
                dbRstJoDell3D掃描 = dbJoDell3D掃描pos;

                //顯示運動速度
                int iJoDell3D掃描spd = clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                double dbJoDell3D掃描spd = (double)iJoDell3D掃描spd / 100;

                //變更顏色
                if (rslt == 4)
                {
                    select_JoDell3D掃描.BackColor = Color.Red;
                    lbl_acpos_JoDell3D掃描.BackColor = Color.White;
                    lbl_spd_JoDell3D掃描.BackColor = Color.White;
                }
                else
                {
                    select_JoDell3D掃描.BackColor = Color.Green;
                    lbl_acpos_JoDell3D掃描.BackColor = Color.Gray;
                    lbl_spd_JoDell3D掃描.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_JoDell3D掃描.Text = dbJoDell3D掃描pos.ToString("F3");
                lbl_spd_JoDell3D掃描.Text = dbJoDell3D掃描spd.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_JoDell3D掃描_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_JoDell3D掃描_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_JoDell3D掃描_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                {
                    lbl_JoDell3D掃描_RAW.Text = iJoDell3D掃描pos.ToString();
                    lbl_JoDell3D掃描_Convert.Text = dbJoDell3D掃描pos.ToString("F3");
                    lbl_JoDell3D掃描_Back.Text = ((int)(dbJoDell3D掃描pos * 100)).ToString();
                }

            }

            if (dbIncreaseGate == dbRead)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //IAI 變更位置
                // 取得欲變更的的浮點數
                double fChangeGate = dbIncreaseGate;

                //伸長量overflow保護
                if (fChangeGate >= 30.0) {
                    fChangeGate = 30.0;
                }


                //執行移動JoDell3D掃描
                clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, 1);
                clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstJoDell3D掃描;
        }  // end of public double dbapJoDell3D掃描(double dbIncreaseGate)  //JoDell3D掃描
        //---------------------------------------------------------------------------------------
        public double dbapJoDell吸針嘴(double dbIncreaseGate)  //JoDell吸針嘴
        {
            double dbRstJoDell吸針嘴 = 0.0;

            {  //JoDell吸針嘴 讀取與顯示
                int rslt = 0;

                //讀取 JoDell吸針嘴 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice = (int)(addr_JODELL.pxeaJ_吸針嘴_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //計算讀取長度
                int iJoDell吸針嘴pos = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GetPosition, 0);
                double dbJoDell吸針嘴pos = (double)iJoDell吸針嘴pos / 100.0;
                dbRstJoDell吸針嘴 = dbJoDell吸針嘴pos;

                //顯示運動速度
                int iJoDell吸針嘴spd = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                double dbJoDell吸針嘴spd = (double)iJoDell吸針嘴spd / 100;

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

                //顯示資訊
                lbl_acpos_JoDell吸針嘴.Text = dbJoDell吸針嘴pos.ToString("F3");
                lbl_spd_JoDell吸針嘴.Text = dbJoDell吸針嘴spd.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_JoDell吸針嘴_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_JoDell吸針嘴_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_JoDell吸針嘴_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                {
                    lbl_JoDell吸針嘴_RAW.Text = iJoDell吸針嘴pos.ToString();
                    lbl_JoDell吸針嘴_Convert.Text = dbJoDell吸針嘴pos.ToString("F3");
                    lbl_JoDell吸針嘴_Back.Text = ((int)(dbJoDell吸針嘴pos * 100)).ToString();
                }

            }

            if (dbIncreaseGate == dbRead)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //IAI 變更位置
                // 取得欲變更的的浮點數
                double fChangeGate = dbIncreaseGate;

                //伸長量overflow保護
                if (fChangeGate >= 30.0) {
                    fChangeGate = 30.0;
                }


                //執行移動JoDell吸針嘴
                clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, 1);
                clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
            }

            return dbRstJoDell吸針嘴;
        }  // end of public double dbapJoDell吸針嘴(double dbIncreaseGate)  //JoDell吸針嘴
        //---------------------------------------------------------------------------------------
        public double dbapJoDell植針嘴(double dbIncreaseGate)  //JoDell植針嘴
        {
            double dbRstJoDell植針嘴 = 0.0;

            {  //JoDell植針嘴 讀取與顯示
                int rslt = 0;

                //讀取 JoDell植針嘴 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice = (int)(addr_JODELL.pxeaJ_植針嘴_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //計算讀取長度
                int iJoDell植針嘴pos = clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_GetPosition, 0);
                double dbJoDell植針嘴pos = (double)iJoDell植針嘴pos / 100.0;
                dbRstJoDell植針嘴 = dbJoDell植針嘴pos;

                //顯示運動速度
                int iJoDell植針嘴spd = clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                double dbJoDell植針嘴spd = (double)iJoDell植針嘴spd / 100;

                //變更顏色
                if (rslt == 4)
                {
                    select_JoDell植針嘴.BackColor = Color.Red;
                    lbl_acpos_JoDell植針嘴.BackColor = Color.White;
                    lbl_spd_JoDell植針嘴.BackColor = Color.White;
                }
                else
                {
                    select_JoDell植針嘴.BackColor = Color.Green;
                    lbl_acpos_JoDell植針嘴.BackColor = Color.Gray;
                    lbl_spd_JoDell植針嘴.BackColor = Color.Gray;
                }

                //顯示資訊
                lbl_acpos_JoDell植針嘴.Text = dbJoDell植針嘴pos.ToString("F3");
                lbl_spd_JoDell植針嘴.Text = dbJoDell植針嘴spd.ToString("F3");

                //bshow_debug_RAW_Conver_Back_Value
                lbl_JoDell植針嘴_RAW.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_JoDell植針嘴_Convert.Visible = bshow_debug_RAW_Conver_Back_Value;
                lbl_JoDell植針嘴_Back.Visible = bshow_debug_RAW_Conver_Back_Value;
                if (bshow_debug_RAW_Conver_Back_Value == true)
                {
                    lbl_JoDell植針嘴_RAW.Text = iJoDell植針嘴pos.ToString();
                    lbl_JoDell植針嘴_Convert.Text = dbJoDell植針嘴pos.ToString("F3");
                    lbl_JoDell植針嘴_Back.Text = ((int)(dbJoDell植針嘴pos * 100)).ToString();
                }

            }

            if (dbIncreaseGate == dbRead)
            {
                //this.Text = "Z軸尚未回到上位";
            }
            else
            {  //IAI 變更位置
                // 取得欲變更的的浮點數
                double fChangeGate = dbIncreaseGate;

                //伸長量overflow保護
                if (fChangeGate >= 50.0) {
                    fChangeGate = 50.0;
                }


                //執行移動JoDell植針嘴
                clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_MotorOn, 1);
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

            //設定吸嘴中心座標
            txtX1.Text = "-35.84";
            txtY1.Text = "50.80";
            txtX2.Text = "-64.77";
            txtY2.Text = "49.11";
            txtCalXYoriginal(sender, e);

            this.Text = "2024/12/09 18:32";

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
                } else if (selectedRadioButton == select_IAI) {
                    wmxId_RadioGroupChanged = WMX3軸定義.IAI;
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
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.IAI) {
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
                        break;
                    case WMX3軸定義.吸嘴Y軸:
                        break;
                    case WMX3軸定義.吸嘴Z軸:
                        break;
                    case WMX3軸定義.吸嘴R軸:
                        break;
                    case WMX3軸定義.載盤X軸:
                        break;
                    case WMX3軸定義.載盤Y軸:
                        break;
                    case WMX3軸定義.植針Z軸:
                        break;
                    case WMX3軸定義.植針R軸:
                        dbapiSetR(result);
                        break;
                    case WMX3軸定義.工作門:
                        break;
                    case WMX3軸定義.IAI:
                        dbapiIAI(result);
                        break;
                    case WMX3軸定義.JoDell3D掃描:
                        dbapJoDell3D掃描(result);
                        break;
                    case WMX3軸定義.JoDell吸針嘴:
                        dbapJoDell吸針嘴(result);
                        break;
                    case WMX3軸定義.JoDell植針嘴:
                        dbapJoDell植針嘴(result);
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
            }

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
                lblHEPA.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_HEPA)           / 10)] & (1 << (int)(WMX3IO對照.pxeIO_HEPA)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl艙內燈.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_LIGHT)          / 10)] & (1 << (int)(WMX3IO對照.pxeIO_LIGHT)           % 10)) != 0) ? Color.Green : Color.Red;

                lbl右按鈕綠燈.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板右按鈕綠燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl紅燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台紅燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台紅燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl中按鈕綠燈.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板中按鈕綠燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl黃燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台黃燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台黃燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl左按鈕紅燈.BackColor = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板左按鈕紅燈) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl綠燈.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台綠燈)       / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台綠燈)        % 10)) != 0) ? Color.Green : Color.Red;
                lblBuzzer.BackColor     = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Buzzer)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Buzzer)          % 10)) != 0) ? Color.Green : Color.Red;
            }

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
            }



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




        private void btnNozzleDownPos_Click(object sender, EventArgs e)
        {
            dbapiNozzleZ(26.2);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bool isOn = false;

            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, isOn);
            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, isOn);

            u8OneCycleFlag = 0;
        }

        private void btn_AlarmRST_Click(object sender, EventArgs e)
        {
            clsServoControlWMX3.WMX3_ClearAlarm();
        }


        private void btnChgX_Click(object sender, EventArgs e)
        {
            try
            {
                // 取得txtX裡的浮點數
                double fChangeNozzleX = double.Parse(txtX.Text);

                //格式化數值
                txtX.Text = fChangeNozzleX.ToString("F3");
                fChangeNozzleX = double.Parse(txtX.Text);

                //執行伸縮吸嘴
                dbapiNozzleX(fChangeNozzleX);
            }
            catch (FormatException)
            {
                MessageBox.Show("請輸入有效的浮點數");
            }
        }

        private void btnChgY_Click(object sender, EventArgs e)
        {
            try
            {
                // 取得txtX裡的浮點數
                double fChangeNozzleY = double.Parse(txtY.Text);

                //格式化數值
                txtY.Text = fChangeNozzleY.ToString("F3");
                fChangeNozzleY = double.Parse(txtY.Text);

                //執行伸縮吸嘴
                dbapiNozzleY(fChangeNozzleY);
            }
            catch (FormatException)
            {
                MessageBox.Show("請輸入有效的浮點數");
            }
        }

        private void btnChgNozzleZ_Click(object sender, EventArgs e)
        {
            try
            {
                // 取得txtChgNozzleZ裡的浮點數
                double fChangeNozzleZ = double.Parse(txtChgNozzleZ.Text);

                //格式化數值
                txtChgNozzleZ.Text = fChangeNozzleZ.ToString("F3");
                fChangeNozzleZ = double.Parse(txtChgNozzleZ.Text);

                //執行伸縮吸嘴
                dbapiNozzleZ(fChangeNozzleZ);
            }
            catch (FormatException)
            {
                MessageBox.Show("請輸入有效的浮點數");
            }
        }

        private void btnChgNozzleDeg_Click(object sender, EventArgs e)
        {
            try {
                // 取得txtDeg裡的浮點數
                double fChangeDegree = double.Parse(txtDeg.Text);

                //格式化數值
                txtDeg.Text = fChangeDegree.ToString("F3");
                fChangeDegree = double.Parse(txtDeg.Text);

                //執行旋轉吸嘴
                dbapiNozzleR(fChangeDegree);
            } catch (FormatException) {
                MessageBox.Show("請輸入有效的浮點數");
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

                clsVibration.u32LED_Level = 50;
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

                clsVibration.u32LED_Level = 50;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }

        private void btnVibrationLED_Click(object sender, EventArgs e)
        {
            //Vibration
            clsVibration.apiEstablishTCPVibration();
            {
                clsVibration.u32LED_Level = 50;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }

        private void btnVibrationLEDOff_Click(object sender, EventArgs e)
        {
            //Vibration
            clsVibration.apiEstablishTCPVibration();
            {
                clsVibration.u32LED_Level = 0;
                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
            }
        }







        private void txtCalXYoriginal(object sender, EventArgs e)
        {
            double dbAverageXo = (double.Parse(txtX1.Text) + double.Parse(txtX2.Text)) / 2;
            double dbAverageYo = (double.Parse(txtY1.Text) + double.Parse(txtY2.Text)) / 2;

            lblXo.Text = dbAverageXo.ToString("F3");
            lblYo.Text = dbAverageYo.ToString("F3");
        }

        private void btnCatchPinXY_Click(object sender, EventArgs e)
        {
            //光源震動盤
            List<Vector3> pins;
            bool 料盤有料 = inspector1.xInsp震動盤(out pins);
            Vector3 temp = (料盤有料) ? pins.First() : new Vector3();
            label4.Text = string.Format("光源震動盤 震動盤 = {0} X = {1:F2} Y = {2:F2} θ = {3:F2}", 料盤有料, temp.X, temp.Y, temp.θ);

            //取得Pin位置 根據吸嘴中心
            double dbPinX = double.Parse(lblXo.Text) - temp.X;
            double dbPinY = double.Parse(lblYo.Text) + temp.Y;
            double dbPinZ = 26.6;
            double dbPinR = 23.00 + temp.θ;

            //設定Pin位置
            txtX.Text = dbPinX.ToString("F3");
            txtY.Text = dbPinY.ToString("F3");
            txtChgNozzleZ.Text = dbPinZ.ToString("F3");
            txtDeg.Text = dbPinR.ToString("F3");
        }

        private void btnSet1_Click(object sender, EventArgs e)
        {
            txtX1.Text = txtX.Text;
            txtY1.Text = txtY.Text;
        }

        private void btnSet2_Click(object sender, EventArgs e)
        {
            txtX2.Text = txtX.Text;
            txtY2.Text = txtY.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] pData = new byte[1];

            //Buzzer
            //pData[0] |= 0b10000000;
            //io.SetOutBytes(7, 1, pData);

            //吸嘴吸真空
            pData[0] |= 0b00000001;
            clsServoControlWMX3.WMX3_SetIO(ref pData, 6, 1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] pData = new byte[1];

            //Buzzer
            //pData[0] &= 0b01111111;
            //io.SetOutBytes(7, 1, pData);

            //吸嘴吸真空
            pData[0] &= 0b11111110;
            clsServoControlWMX3.WMX3_SetIO(ref pData, 6, 1);

            //吸嘴破真空
            pData[0] |= 0b00000100;
            clsServoControlWMX3.WMX3_SetIO(ref pData, 6, 1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] pData = new byte[1];

            //吸嘴破真空
            pData[0] &= 0b11111011;
            clsServoControlWMX3.WMX3_SetIO(ref pData, 6, 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double dbPinX = -162.43;
            double dbPinY = 29.5;

            //設定Pin位置
            txtX.Text = dbPinX.ToString("F3");
            txtY.Text = dbPinY.ToString("F3");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double dbPinX = -241.79;
            double dbPinY = 27.99;

            //設定Pin位置
            txtX.Text = dbPinX.ToString("F3");
            txtY.Text = dbPinY.ToString("F3");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            bool bNullPin = false;

            switch(u8OneCycleFlag)
            {
                case 0:
                    break;

                case 1:
                    //震動開始
                    btnVibrationInit_Click(sender, e);
                    u8OneCycleFlag = 2;
                    break;

                case 2:
                    if (dbapiNozzleZ(dbRead) <= 0.5)
                    {
                        //設定拋料位置
                        button7_Click(sender, e);

                        //移動吸嘴
                        btnChgX_Click(sender, e);
                        btnChgY_Click(sender, e);

                        //震動停止
                        btnVibrationStop_Click(sender, e);

                        u8OneCycleFlag = 3;
                    }
                    break;

                case 3:
                    if (dbapiNozzleZ(dbRead) <= 0.5)
                    {
                        //從影像拿取針位置
                        button2_Click(sender, e);

                        //光源震動盤
                        List<Vector3> pins;
                        bool 料盤有料 = inspector1.xInsp震動盤(out pins);
                        Vector3 temp = (料盤有料) ? pins.First() : new Vector3();
                        //label4.Text = string.Format("光源震動盤 震動盤 = {0} X = {1:F2} Y = {2:F2} θ = {3:F2}", 料盤有料, temp.X, temp.Y, temp.θ);
                        if (料盤有料 == false)
                        {
                            //如果沒看到針
                            bNullPin = true;

                            u8OneCycleFlag = 7;
                        }
                        else
                        {
                            //求得吸嘴中心位置
                            txtCalXYoriginal(sender, e);

                            //設定取針位置
                            btnCatchPinXY_Click(sender, e);

                            //移動吸嘴
                            btnChgX_Click(sender, e);
                            btnChgY_Click(sender, e);
                            btnChgNozzleDeg_Click(sender, e);

                            u8OneCycleFlag = 4;
                        }
                    }
                    break;

                case 4:
                    //下降Nozzle
                    btnChgNozzleZ_Click(sender, e);

                    u8OneCycleFlag = 5;
                    break;

                case 5:
                    //吸針
                    button3_Click(sender, e);

                    //收回Nozzle
                    txtChgNozzleZ.Text = "0.00";
                    btnChgNozzleZ_Click(sender, e);

                    if (dbapiNozzleZ(dbRead) <= 0.5)
                    {
                        u8OneCycleFlag = 6;
                    }
                    break;

                case 6:
                    if (dbapiNozzleZ(dbRead) <= 0.5)
                    {
                        //設定至下視覺位
                        button6_Click(sender, e);

                        //移動吸嘴
                        btnChgX_Click(sender, e);
                        btnChgY_Click(sender, e);

                        u8OneCycleFlag = 7;
                    }
                    break;

                case 7:
                    if (dbapiNozzleZ(dbRead) <= 0.5)
                    {
                        //移至拋料
                        button7_Click(sender, e);

                        //移動吸嘴
                        btnChgX_Click(sender, e);
                        btnChgY_Click(sender, e);

                        u8OneCycleFlag = 8;
                    }
                    break;

                case 8:
                    //下降Nozzle
                    txtChgNozzleZ.Text = "5.00";
                    btnChgNozzleZ_Click(sender, e);

                    u8OneCycleFlag = 9;
                    break;

                case 9:
                    //吐料
                    //破真空
                    button4_Click(sender, e);

                    u8OneCycleFlag = 10;
                    break;

                case 10:
                    //關真空
                    button5_Click(sender, e);

                    //收回Nozzle
                    txtChgNozzleZ.Text = "0.00";
                    btnChgNozzleZ_Click(sender, e);

                    if (dbapiNozzleZ(dbRead) <= 0.5)
                    {
                        u8OneCycleFlag = 11;
                    }
                    break;

                case 11:
                    if(bNullPin == true)
                    {
                        //沒針了
                        u8OneCycleFlag = 0;
                    } 
                    else
                    {
                        //還有針
                        int CycleCNT = int.Parse(txtcyclecnt.Text);
                        if (CycleCNT >= 1)
                        {
                            CycleCNT--;
                            txtcyclecnt.Text = CycleCNT.ToString();

                            u8OneCycleFlag = 1;
                        }
                        else
                        {
                            u8OneCycleFlag = 0;
                        }
                    }

                    break;
            }

            button8.Text = u8OneCycleFlag.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (u8OneCycleFlag == 0)
            {
                u8OneCycleFlag = 1;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            lblTmr.Text = timer2.Interval.ToString();
            lblSpd.Text = Multiplier.ToString();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            timer2.Interval = int.Parse(txtTmr.Text);
            Multiplier = int.Parse(txtSpd.Text);
        }










        private void label6_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Label SelectLabel = sender as System.Windows.Forms.Label;



        }



        private void label11_Click(object sender, EventArgs e)
        {
            int addr_TargetSetDevice = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
            int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Position2Bytes) / 10;

            int varJODELL_TX_P0_Position = 800;
            byte[] JODELL_TX_P0_Position = new byte[2];

            JODELL_TX_P0_Position[0] = (byte)(varJODELL_TX_P0_Position & 0xFF);
            JODELL_TX_P0_Position[1] = (byte)(varJODELL_TX_P0_Position >> 8);

            //Write TX
            clsServoControlWMX3.WMX3_SetIO(ref JODELL_TX_P0_Position, addr_TargetSetDevice + addr_TargetSetFunction, 2);
        }

        private void label10_Click(object sender, EventArgs e)
        {
            int addr_TargetSetDevice = (int)(addr_JODELL.pxeaJ_吸針嘴_Output) / 10;
            int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Position2Bytes) / 10;

            int varJODELL_TX_P0_Position = 2200;
            byte[] JODELL_TX_P0_Position = new byte[2];

            JODELL_TX_P0_Position[0] = (byte)(varJODELL_TX_P0_Position & 0xFF);
            JODELL_TX_P0_Position[1] = (byte)(varJODELL_TX_P0_Position >> 8);

            //Write TX
            clsServoControlWMX3.WMX3_SetIO(ref JODELL_TX_P0_Position, addr_TargetSetDevice + addr_TargetSetFunction, 2);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int iPos = int.Parse(textBox1.Text);

            if(iPos >= 5000)
            {
                iPos = 5000;
            }

            if(iPos <= 0)
            {
                iPos = 0;
            }

            textBox1.Text = iPos.ToString();

            {
                int addr_TargetSetDevice = (int)(addr_JODELL.pxeaJ_植針嘴_Output) / 10;
                int addr_TargetSetFunction = (int)(addr_JODELL.pxeaJ_SetAddr_P0_Position2Bytes) / 10;

                int varJODELL_TX_P0_Position = iPos;
                byte[] JODELL_TX_P0_Position = new byte[2];

                JODELL_TX_P0_Position[0] = (byte)(varJODELL_TX_P0_Position & 0xFF);
                JODELL_TX_P0_Position[1] = (byte)(varJODELL_TX_P0_Position >> 8);

                //Write TX
                clsServoControlWMX3.WMX3_SetIO(ref JODELL_TX_P0_Position, addr_TargetSetDevice + addr_TargetSetFunction, 2);
            }
        }


    }  // end of public partial class Form1 : Form

}  // end of namespace InjectorInspector
