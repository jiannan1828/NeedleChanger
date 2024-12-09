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
//using System.Text.Json;

//---------------------------------------------------------------------------------------
namespace InjectorInspector
{
    public partial class Form1 : Form
    {
        //---------------------------------------------------------------------------------------
        //WMX3
        ServoControl clsServoControlWMX3 = new ServoControl();

        //---------------------------------------------------------------------------------------
        //Vibration
        Vibration clsVibration = new Vibration();

        //---------------------------------------------------------------------------------------
        //Debug
        public int ErrorCode = 0;
        public int cntcallback = 0;

        public int Multiplier = 4;
        public int u8OneCycleFlag = 0;

        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Test function with Vision
        /// </summary>
        /// 
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
        /// <summary>
        /// Xavier Call, Control the Servo machine
        /// </summary>
        /// 
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
                lbl_acpos_吸嘴X軸.Text = dbRstNozzleX.ToString("F2");
                lbl_spd_吸嘴X軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseNozzleX == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //吸嘴X軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeNozzleX = dbIncreaseNozzleX;

                //伸長量overflow保護
                //if (fChangeNozzleX >= 40.35)
                //{
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
                lbl_acpos_吸嘴Y軸.Text = dbRstNozzleY.ToString("F2");
                lbl_spd_吸嘴Y軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseNozzleY == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //吸嘴X軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeNozzleY = dbIncreaseNozzleY;

                //伸長量overflow保護
                //if (fChangeNozzleY >= 40.35)
                //{
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
                lbl_acpos_吸嘴Z軸.Text = dbRstNozzleZ.ToString("F2");
                lbl_spd_吸嘴Z軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseNozzleZ == dbRead)
            {

            }
            else
            {  //吸嘴Z軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeNozzleZ = dbIncreaseNozzleZ;

                //伸長量overflow保護
                if (fChangeNozzleZ >= 40.35)
                {
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
                lbl_acpos_吸嘴R軸.Text = dbRstNozzleR.ToString("F2");
                lbl_spd_吸嘴R軸.Text = dbSpeed.ToString("F2");

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
                lbl_acpos_載盤X軸.Text = dbRstCarrierX.ToString("F2");
                lbl_spd_載盤X軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseCarrierX == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //載盤X軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeCarrierX = dbIncreaseCarrierX;

                //伸長量overflow保護
                //if (fChangeCarrierX >= 40.35)
                //{
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
                lbl_acpos_載盤Y軸.Text = dbRstCarrierY.ToString("F2");
                lbl_spd_載盤Y軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseCarrierY == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //載盤Y軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeCarrierY = dbIncreaseCarrierY;

                //伸長量overflow保護
                //if (fChangeCarrierY >= 40.35)
                //{
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
                lbl_acpos_植針Z軸.Text = dbRstSetZ.ToString("F2");
                lbl_spd_植針Z軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseSetZ == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //植針Z軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeSetZ = dbIncreaseSetZ;

                //伸長量overflow保護
                //if (fChangeSetZ >= 40.35)
                //{
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
                    dbRstSetR = double.Parse(position);
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
                lbl_acpos_植針R軸.Text = dbRstSetR.ToString("F2");
                lbl_spd_植針R軸.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseSetR == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //植針R軸 變更位置
                // 取得欲變更的的浮點數
                double fChangeSetR = dbIncreaseSetR;

                //伸長量overflow保護
                //if (fChangeSetR >= 40.35)
                //{
                //    fChangeSetR = 40.35;
                //}

                //計算補正至長度的數值
                int iTargetSetR = (int)(fChangeSetR * 100);

                //執行移動植針R軸
                int axis = (int)WMX3軸定義.植針R軸;
                int position = iTargetSetR;
                int speed = (int)(50.00 * 100 * Multiplier);
                int accel = speed * 2;
                int daccel = speed * 2;
                clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
            }

            return dbRstSetR;
        }  // end of public double dbapiSetR(double dbIncreaseSetR)  //SetR


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
                lbl_acpos_工作門.Text = dbRstGate.ToString("F2");
                lbl_spd_工作門.Text = dbSpeed.ToString("F2");

            }

            if (dbIncreaseGate == dbRead || dbapiNozzleZ(dbRead) >= 0.5)
            {
                this.Text = "Z軸尚未回到上位";
            }
            else
            {  //工作門 變更位置
                // 取得欲變更的的浮點數
                double fChangeGate = dbIncreaseGate;

                //伸長量overflow保護
                //if (fChangeGate >= 40.35)
                //{
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
        /// <summary>
        /// Project Code implement
        /// </summary>
        /// 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //init vision
            inspector1.xInit();
            //Add the callback api from snapshot api
            inspector1.on下視覺 = apiCallBackTest;

            //先跳到第2頁
            int iAimToPageIndex = 3;
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();
            
            //sw.Close();
        }

        public bool En吸嘴X軸 = false;
        public bool En吸嘴Y軸 = false;
        public bool En吸嘴Z軸 = false;
        public bool En吸嘴R軸 = false;

        public bool En載盤X軸 = false;
        public bool En載盤Y軸 = false;

        public bool En植針Z軸 = false;
        public bool En植針R軸 = false;

        public bool En工作門 = false;

        public void en_Group_Click(object sender, EventArgs e)
        {  // start of public void en_Group_Click(object sender, EventArgs e)
            if (En吸嘴X軸 != en_吸嘴X軸.Checked)
            {
                En吸嘴X軸 = en_吸嘴X軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, En吸嘴X軸);
            }

            if (En吸嘴Y軸 != en_吸嘴Y軸.Checked)
            {
                En吸嘴Y軸 = en_吸嘴Y軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, En吸嘴Y軸);
            }

            if (En吸嘴Z軸 != en_吸嘴Z軸.Checked)
            {
                En吸嘴Z軸 = en_吸嘴Z軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, En吸嘴Z軸);
            }

            if (En吸嘴R軸 != en_吸嘴R軸.Checked)
            {
                En吸嘴R軸 = en_吸嘴R軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, En吸嘴R軸);
            }


            if (En載盤X軸 != en_載盤X軸.Checked)
            {
                En載盤X軸 = en_載盤X軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, En載盤X軸);
            }

            if (En載盤Y軸 != en_載盤Y軸.Checked)
            {
                En載盤Y軸 = en_載盤Y軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, En載盤Y軸);
            }


            if (En植針Z軸 != en_植針Z軸.Checked)
            {
                En植針Z軸 = en_植針Z軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, En植針Z軸);
            }

            if (En植針R軸 != en_植針R軸.Checked)
            {
                En植針R軸 = en_植針R軸.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, En植針R軸);
            }


            if (En工作門 != en_工作門.Checked)
            {
                En工作門 = en_工作門.Checked;

                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門, En工作門);
            }
        }  // end of public void en_Group_Click(object sender, EventArgs e)


        public WMX3軸定義 wmxId_RadioGroupChanged = WMX3軸定義.AXIS_START;
        private void RadioGroupChanged(object sender, EventArgs e)
        {  // start of private void RadioGroupChanged(object sender, EventArgs e)
            // 將 sender 轉型為 RadioButton
            System.Windows.Forms.RadioButton selectedRadioButton = sender as System.Windows.Forms.RadioButton;

            //辨識選擇之軸
            if (selectedRadioButton != null && selectedRadioButton.Checked == true)
            {
                if (selectedRadioButton == select_吸嘴X軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴X軸;
                }
                else if (selectedRadioButton == select_吸嘴Y軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴Y軸;
                }
                else if (selectedRadioButton == select_吸嘴Z軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴Z軸;
                }
                else if (selectedRadioButton == select_吸嘴R軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.吸嘴R軸;
                }
                else if (selectedRadioButton == select_載盤X軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.載盤X軸;
                }
                else if (selectedRadioButton == select_載盤Y軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.載盤Y軸;
                }
                else if (selectedRadioButton == select_植針Z軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.植針Z軸;
                }
                else if (selectedRadioButton == select_植針R軸)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.植針R軸;
                }
                else if (selectedRadioButton == select_工作門)
                {
                    wmxId_RadioGroupChanged = WMX3軸定義.工作門;
                }
            }

            //複製選擇之軸
            if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴X軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴X軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴Y軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴Y軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴Z軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴Z軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴R軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴R軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.載盤X軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_載盤X軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.載盤Y軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_載盤Y軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.植針Z軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_植針Z軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.植針R軸)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_植針R軸.Text).ToString("F2"));
            }
            else if (wmxId_RadioGroupChanged == WMX3軸定義.工作門)
            {
                txtABSpos.Text = (double.Parse(lbl_acpos_工作門.Text).ToString("F2"));
            }
            else
            {
                txtABSpos.Text = "N/A";
            }

        }  // end of private void RadioGroupChanged(object sender, EventArgs e)

        public void btn_adjust_JOG(object sender, EventArgs e)
        {  // start of public void btn_adjust_JOG(object sender, EventArgs e)
            // 將 sender 轉型為 Button
            System.Windows.Forms.Button ptrBtn = sender as System.Windows.Forms.Button;

            if (ptrBtn == btnABSMove)
            {
                // 先進行計算，再格式化數字
                double result = double.Parse(lbl_acpos_工作門.Text) + 0.0;
                txtABSpos.Text = result.ToString("F2");
            }
            else

            if (ptrBtn == btn_plus_1)
            {
                double result = double.Parse(lbl_acpos_工作門.Text) + 1.0;
                txtABSpos.Text = result.ToString("F2");
            }
            else if (ptrBtn == btn_minus_1)
            {
                double result = double.Parse(lbl_acpos_工作門.Text) - 1.0;
                txtABSpos.Text = result.ToString("F2");
            }
            else

            if (ptrBtn == btn_plus_10)
            {
                double result = double.Parse(lbl_acpos_工作門.Text) + 10.0;
                txtABSpos.Text = result.ToString("F2");
            }
            else if (ptrBtn == btn_minus_10)
            {
                double result = double.Parse(lbl_acpos_工作門.Text) - 10.0;
                txtABSpos.Text = result.ToString("F2");
            }

        }  // end of public void btn_adjust_JOG(object sender, EventArgs e)







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

        private void timer1_Tick(object sender, EventArgs e)
        {
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
            dbapiNozzleX(dbRead);
            dbapiNozzleY(dbRead);
            dbapiNozzleZ(dbRead);
            dbapiNozzleR(dbRead);

            dbapiCarrierX(dbRead);
            dbapiCarrierY(dbRead);

            dbapiSetZ(dbRead);
            dbapiSetR(dbRead);

            dbapiGate(dbRead);


            //讀取InputIO
            byte[] pDataGetIO = new byte[8];
            clsServoControlWMX3.WMX3_GetIO(ref pDataGetIO, 28, 8);

            // 使用 StringBuilder 來構建文本，減少字串拼接的開銷
            var sb = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    byte[] data = new byte[1] { pDataGetIO[i] };
                    sb.AppendFormat("{0}:{1} ", data.ToHex(), data.ToBinary());
                }
            // 設定 Text 屬性
            this.Text = sb.ToString();

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
                txtX.Text = fChangeNozzleX.ToString("F2");
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
                txtY.Text = fChangeNozzleY.ToString("F2");
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
                txtChgNozzleZ.Text = fChangeNozzleZ.ToString("F2");
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
                txtDeg.Text = fChangeDegree.ToString("F2");
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

            lblXo.Text = dbAverageXo.ToString("F2");
            lblYo.Text = dbAverageYo.ToString("F2");
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
            txtX.Text = dbPinX.ToString("F2");
            txtY.Text = dbPinY.ToString("F2");
            txtChgNozzleZ.Text = dbPinZ.ToString("F2");
            txtDeg.Text = dbPinR.ToString("F2");
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
            txtX.Text = dbPinX.ToString("F2");
            txtY.Text = dbPinY.ToString("F2");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double dbPinX = -241.79;
            double dbPinY = 27.99;

            //設定Pin位置
            txtX.Text = dbPinX.ToString("F2");
            txtY.Text = dbPinY.ToString("F2");
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





    }  // end of public partial class Form1 : Form

}  // end of namespace InjectorInspector
