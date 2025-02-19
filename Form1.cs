﻿
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
        //State Enum
        public const double dbRead           = 9916777216.99;
        public const double dbCheckArrived   = 9916777254.87;
        public const double dbAxisMoveOk     = 9916777294.78;
        public const double dbAxisMoveNg     = 9916777209.87;
        public const double dbAimToNext      = 9916777299.77;
        public const double dbSpecific       = 9916777277.18;

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
            //Vision Callback Function test
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
        //---------------------------------------------------------------------------------------
        void apiParaWriteIndex(string filename, int index, double dbValue)
        {
            // 在 Form 類中創建 apiJsonParameterHandle 的實例
            apiJsonParameterHandle handJson = new apiJsonParameterHandle();

            // 初始化 apiJsonParameterHandle，並指定檔案名稱
            handJson.InitialJsonFile(filename);

            // 使用 apiJsonParameterHandle 中的 JsonNeedleContentList 資料
            BindingList<JsonParameterContent> jsonContentList = new BindingList<JsonParameterContent>(handJson.JsonNeedleContentList);

            // 根據索引讀取資料
            jsonContentList[index].dbPosition = dbValue;
        }
        //---------------------------------------------------------------------------------------
        double apiParaReadIndex(string filename, int index) {
            double rslt = 0.0;

            // 在 Form 類中創建 apiJsonParameterHandle 的實例
            apiJsonParameterHandle handJson = new apiJsonParameterHandle();

            // 初始化 apiJsonParameterHandle，並指定檔案名稱
            handJson.InitialJsonFile(filename);

            // 使用 apiJsonParameterHandle 中的 JsonNeedleContentList 資料
            BindingList<JsonParameterContent> jsonContentList = new BindingList<JsonParameterContent>(handJson.JsonNeedleContentList);

            // 根據索引讀取資料
            rslt = jsonContentList[index].dbPosition;

            return rslt;
        }
        //---------------------------------------------------------------------------------------
        string apiParaReadStr(string filename, int index) {
            string rslt;

            // 在 Form 類中創建 apiJsonParameterHandle 的實例
            apiJsonParameterHandle handJson = new apiJsonParameterHandle();

            // 初始化 apiJsonParameterHandle，並指定檔案名稱
            handJson.InitialJsonFile(filename);

            // 使用 apiJsonParameterHandle 中的 JsonNeedleContentList 資料
            BindingList<JsonParameterContent> jsonContentList = new BindingList<JsonParameterContent>(handJson.JsonNeedleContentList);

            // 根據索引讀取資料
            rslt = jsonContentList[index].strNote;

            return rslt;
        }
        //---------------------------------------------------------------------------------------
        void apiReadNeedleInfo(string filename, int Index, ref double dbX, ref double dbY) {
            JSON temp = new JSON();

            try {
                temp = JsonConvert.DeserializeObject<JSON>(File.ReadAllText(filename));
            } catch (Exception ex) {
                MessageBox.Show($"讀取 Json 檔時發生錯誤: {ex.Message}");
            }

            dbX = temp.Needles[Index].X;
            dbY = temp.Needles[Index].Y;
        }
        //---------------------------------------------------------------------------------------
        public void button2_Click(object sender, EventArgs e)
        {
            //Save Vision Recipe
            inspector1.SaveRecipe(8);
        }
        //---------------------------------------------------------------------------------------
        public void button5_Click(object sender, EventArgs e)
        {
            //Read Vision Recipe
            inspector1.LoadRecipe(8);
        }
        //---------------------------------------------------------------------------------------
        bool   b有看到校正孔         = false;
        double dbCameraCalibrationX = 0.0;
        double dbCameraCalibrationY = 0.0;
        public void btn_Socket孔檢查_Click(object sender, EventArgs e)
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
        public void btn_植針嘴檢查_Click(object sender, EventArgs e)
        {
            //植針嘴有無堵料, 無:ok, 有:ng
            Inspector.Vector3 pos2;
            bool success2 = inspector1.xInsp夾爪(out pos2);   //夾爪針孔偵測 回傳:OK/NG 及找到孔的位置
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
        int    iPC = 0, iRC = 0;
        public void btn_取得目標座標_Click(object sender, EventArgs e)
        {
            if (bRemove == true) {
                //找下一個要抽針的ID
                bChambered = false;
                bTakePin = false;
                iPC = 0;
                if (iRC == 0) {
                    iRC = find_RemoveNeedles();
                }

                try {
                    iHoleIndex = RemoveNeedles[0].Index;  // 嘗試訪問索引 0 的元素
                } catch (Exception ex) {
                    // 捕捉其他類型的異常
                    Console.WriteLine("發生錯誤：" + ex.Message);
                    iHoleIndex = -1;
                }

                //取得目前抽針ID的位置
                if (iHoleIndex == -1) {
                    //沒拿到
                    bRemove = false;
                } else if (iHoleIndex >= 0) {
                    //有拿到
                    double dbX = 0.0, dbY = 0.0;

                    find_Needle_Position(PerspectiveTransformMatrix, iHoleIndex, ref dbX, ref dbY);
                    FocusedNeedle = RemoveNeedles[0];
                    show_grp_NeedleInfo(grp_NeedleInfo);
                    pic_Needles.Refresh();

                    txt_HoldIndex.Text = iHoleIndex.ToString();

                    dbPinHolePositionX = dbX;
                    dbPinHolePositionY = dbY;

                    label14.Text = dbX.ToString();
                    label15.Text = dbY.ToString();

                    //刪除目前的抽針ID
                    RemoveNeedles.RemoveAt(0);
                    iRC = RemoveNeedles.Count();
                }
            } else 
            
            if (bChambered == true) {
                //找下一個要植針的ID
                bRemove = false;
                bTakePin = false;
                iRC = 0;
                if (iPC == 0) {
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
                if (iHoleIndex == -1) {
                    //沒拿到
                    bChambered = false;
                } else if (iHoleIndex >= 0) {
                    //有拿到
                    double dbX = 0.0, dbY = 0.0;

                    find_Needle_Position(PerspectiveTransformMatrix, iHoleIndex, ref dbX, ref dbY);
                    FocusedNeedle = PlaceNeedles[0];
                    show_grp_NeedleInfo(grp_NeedleInfo);
                    pic_Needles.Refresh();

                    txt_HoldIndex.Text = iHoleIndex.ToString();

                    dbPinHolePositionX = dbX;
                    dbPinHolePositionY = dbY;

                    label14.Text = dbX.ToString();
                    label15.Text = dbY.ToString();

                    //刪除目前的植針ID
                    PlaceNeedles.RemoveAt(0);
                    iPC = PlaceNeedles.Count();
                }
            } else 
            
            if (bTakePin == true) {

            }
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
        public double dbTargetState = 0.0;
        public double dbapiStateStatus(double dbState)
        {
            switch (dbState) {
                default:
                    return 0.0;

                case dbRead: 
                    switch (dbTargetState) {
                        default:
                            return 0.0;

                        case dbAimToNext:
                            dbTargetState = 0.0;
                            return dbAimToNext;

                        case dbSpecific:
                            dbTargetState = 0.0;
                            return dbSpecific;
                    }
                    break;

                case dbAimToNext:
                case dbSpecific:
                    dbTargetState = dbState;
                    break;
            }

            return dbTargetState;
        }  // end of public double dbapiDelayCNT01(double dbDelayCNT) 
        //---------------------------------------------------------------------------------------
        public double dbTargetDelayCNT01 = 0.0;
        public double dbapiDelayCNT01(double dbDelayCNT) 
        {
            if(dbTargetDelayCNT01>0) { 
                dbTargetDelayCNT01--;
            }

            switch(dbDelayCNT) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    if(dbTargetDelayCNT01==0) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {
                    dbTargetDelayCNT01 = dbDelayCNT;
                } break;
            }

            return dbTargetDelayCNT01;
        }  // end of public double dbapiDelayCNT01(double dbDelayCNT) 
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionNozzleX = 0.0;
        public double dbapiNozzleX(double dbIncreaseNozzleX, double dbTargetSpeed)  //NozzleX
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 500000;
                const int    MinRAW =      0;
                const double Maxdb  =  500.0;
                const double Mindb  =    0.0;
                const double Sum    = 500000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleX     = 0.0;

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

            //Function Classification
            switch(dbIncreaseNozzleX) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionNozzleX * 1.01 > dbTargetPositionNozzleX * 0.99) { 
                        dbMin = dbTargetPositionNozzleX * 0.99 - 0.1;
                        dbMax = dbTargetPositionNozzleX * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionNozzleX * 1.01 - 0.1;
                        dbMax = dbTargetPositionNozzleX * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstNozzleX &&
                                 dbRstNozzleX <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //吸嘴X軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseNozzleX && dbIncreaseNozzleX<=Maxdb ) {

                    } else if( dbIncreaseNozzleX<=Mindb ) {
                        dbIncreaseNozzleX = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseNozzleX ) {
                        dbIncreaseNozzleX = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeNozzleX      = calculate.Map(dbIncreaseNozzleX, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionNozzleX = dbIncreaseNozzleX;

                    //執行移動吸嘴
                    int axis     = (int)WMX3軸定義.吸嘴X軸;
                    int position = fChangeNozzleX;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstNozzleX;
        }  // end of public double dbapiNozzleX(double dbIncreaseNozzleX)  //NozzleX
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionNozzleY = 0.0;
        public double dbapiNozzleY(double dbIncreaseNozzleY, double dbTargetSpeed)  //NozzleY
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  10000;
                const int    MinRAW =      0;
                const double Maxdb  =  100.0;
                const double Mindb  =    0.0;
                const double Sum    =  10000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleY     = 0.0;

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

            //Function Classification
            switch(dbIncreaseNozzleY) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionNozzleY * 1.01 > dbTargetPositionNozzleY * 0.99) { 
                        dbMin = dbTargetPositionNozzleY * 0.99 - 0.1;
                        dbMax = dbTargetPositionNozzleY * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionNozzleY * 1.01 - 0.1;
                        dbMax = dbTargetPositionNozzleY * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstNozzleY &&
                                 dbRstNozzleY <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //吸嘴Y軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseNozzleY && dbIncreaseNozzleY<=Maxdb ) {

                    } else if( dbIncreaseNozzleY<=Mindb ) {
                        dbIncreaseNozzleY = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseNozzleY ) {
                        dbIncreaseNozzleY = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeNozzleY      = calculate.Map(dbIncreaseNozzleY, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionNozzleY = dbIncreaseNozzleY;

                    //執行移動吸嘴
                    int axis     = (int)WMX3軸定義.吸嘴Y軸;
                    int position = fChangeNozzleY;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstNozzleY;
        }  // end of public double dbapiNozzleY(double dbIncreaseNozzleY)  //NozzleY
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionNozzleZ = 0.0;
        public double dbapiNozzleZ(double dbIncreaseNozzleZ, double dbTargetSpeed)  //NozzleZ
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  41496;
                const int    MinRAW =    -93;
                const double Maxdb  =   40.0;
                const double Mindb  =    0.0;
                const double Sum    =  40000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleZ     = 0.0;
            
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

            //Function Classification
            switch(dbIncreaseNozzleZ) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionNozzleZ * 1.01 > dbTargetPositionNozzleZ * 0.99) { 
                        dbMin = dbTargetPositionNozzleZ * 0.99 - 0.1;
                        dbMax = dbTargetPositionNozzleZ * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionNozzleZ * 1.01 - 0.1;
                        dbMax = dbTargetPositionNozzleZ * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstNozzleZ &&
                                 dbRstNozzleZ <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //吸嘴Z軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseNozzleZ && dbIncreaseNozzleZ<=Maxdb ) {

                    } else if( dbIncreaseNozzleZ<=Mindb ) {
                        dbIncreaseNozzleZ = (int)Mindb;
                    } else if ( Maxdb<=dbIncreaseNozzleZ ) {
                        dbIncreaseNozzleZ = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeNozzleZ      = calculate.Map(dbIncreaseNozzleZ, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionNozzleZ = dbIncreaseNozzleZ;

                    //執行伸縮吸嘴
                    int axis     = (int)WMX3軸定義.吸嘴Z軸;
                    int position = fChangeNozzleZ;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstNozzleZ;
        }  // end of public double dbapiNozzleZ(double dbIncreaseNozzleZ)  //NozzleZ
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionNozzleR = 0.0;
        public double dbapiNozzleR(double dbIncreaseNozzleR, double dbTargetSpeed)  //NozzleR
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  24120;
                const int    MinRAW = -11880;
                const double Maxdb  =  360.0;
                const double Mindb  =    0.0;
                const double Sum    =  36000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstNozzleR     = 0.0;

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
                    while (dbGet >= 360.0) { dbGet -= 360.0; }  //overflow
                    while (dbGet <    0.0) { dbGet += 360.0; }  //overflow
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

            //Function Classification
            switch(dbIncreaseNozzleR) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionNozzleR*1.01 > dbTargetPositionNozzleR*0.99) { 
                        dbMin = dbTargetPositionNozzleR*0.99 - 0.1;
                        dbMax = dbTargetPositionNozzleR*1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionNozzleR*1.01 - 0.1;
                        dbMax = dbTargetPositionNozzleR*0.99 + 0.1;
                    }
                    if( dbMin <= dbRstNozzleR && 
                                 dbRstNozzleR <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //吸嘴R軸 變更位置
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
                    dbTargetPositionNozzleR = dbIncreaseNozzleR;
                    while (dbTargetPositionNozzleR >= 360.0) { dbTargetPositionNozzleR -= 360.0; }  //overflow
                    while (dbTargetPositionNozzleR <    0.0) { dbTargetPositionNozzleR += 360.0; }  //overflow

                    //執行旋轉吸嘴
                    int axis     = (int)WMX3軸定義.吸嘴R軸;
                    int position = fChangeNozzleR;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed * 2;
                    int daccel   = speed * 2;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstNozzleR;
        }  // end of public double dbapiNozzleR(double dbIncreaseNozzleR)  //NozzleR
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionCarrierX = 0.0;
        public double dbapiCarrierX(double dbIncreaseCarrierX, double dbTargetSpeed)  //CarrierX
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 190000;
                const int    MinRAW =      0;
                const double Maxdb  =  190.0;
                const double Mindb  =    0.0;
                const double Sum    = 190000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstCarrierX    = 0.0;

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

            //Function Classification
            switch(dbIncreaseCarrierX) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionCarrierX * 1.01 > dbTargetPositionCarrierX * 0.99) { 
                        dbMin = dbTargetPositionCarrierX * 0.99 - 0.1;
                        dbMax = dbTargetPositionCarrierX * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionCarrierX * 1.01 - 0.1;
                        dbMax = dbTargetPositionCarrierX * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstCarrierX &&
                                 dbRstCarrierX <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //載盤X軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseCarrierX && dbIncreaseCarrierX<=Maxdb ) {

                    } else if( dbIncreaseCarrierX<=Mindb ) {
                        dbIncreaseCarrierX = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseCarrierX ) {
                        dbIncreaseCarrierX = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeCarrierX = calculate.Map(dbIncreaseCarrierX, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionCarrierX = dbIncreaseCarrierX;

                    //執行移動載盤
                    int axis     = (int)WMX3軸定義.載盤X軸;
                    int position = fChangeCarrierX;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstCarrierX;
        }  // end of public double dbapiCarrierX(double dbIncreaseCarrierX)  //CarrierX
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionCarrierY = 0.0;
        public double dbapiCarrierY(double dbIncreaseCarrierY, double dbTargetSpeed)  //CarrierY
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 800000;
                const int    MinRAW =      0;
                const double Maxdb  =  800.0;
                const double Mindb  =    0.0;
                const double Sum    = 800000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstCarrierY    = 0.0;

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

            //Function Classification
            switch(dbIncreaseCarrierY) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionCarrierY * 1.01 > dbTargetPositionCarrierY * 0.99) { 
                        dbMin = dbTargetPositionCarrierY * 0.99 - 0.1;
                        dbMax = dbTargetPositionCarrierY * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionCarrierY * 1.01 - 0.1;
                        dbMax = dbTargetPositionCarrierY * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstCarrierY &&
                                 dbRstCarrierY <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //載盤Y軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseCarrierY && dbIncreaseCarrierY<=Maxdb ) {

                    } else if( dbIncreaseCarrierY<=Mindb ) {
                        dbIncreaseCarrierY = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseCarrierY ) {
                        dbIncreaseCarrierY = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeCarrierY      = calculate.Map(dbIncreaseCarrierY, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionCarrierY = dbIncreaseCarrierY;

                    //執行移動載盤
                    int axis     = (int)WMX3軸定義.載盤Y軸;
                    int position = fChangeCarrierY;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstCarrierY;
        }  // end of public double dbapiCarrierY(double dbIncreaseCarrierY)  //CarrierY
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionSetZ = 0.0;
        public double dbapiSetZ(double dbIncreaseSetZ, double dbTargetSpeed)  //SetZ
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   3300;
                const int    MinRAW =      0;
                const double Maxdb  =     33;
                const double Mindb  =    0.0;
                const double Sum    =   3300;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstSetZ        = 0.0;

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

            //Function Classification
            switch(dbIncreaseSetZ) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionSetZ * 1.01 > dbTargetPositionSetZ * 0.99) { 
                        dbMin = dbTargetPositionSetZ * 0.99 - 0.1;
                        dbMax = dbTargetPositionSetZ * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionSetZ * 1.01 - 0.1;
                        dbMax = dbTargetPositionSetZ * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstSetZ &&
                                 dbRstSetZ <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //植針Z軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseSetZ && dbIncreaseSetZ<=Maxdb ) {

                    } else if( dbIncreaseSetZ<=Mindb ) {
                        dbIncreaseSetZ = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseSetZ ) {
                        dbIncreaseSetZ = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeSetZ      = calculate.Map(dbIncreaseSetZ, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionSetZ = dbIncreaseSetZ;

                    //執行移動植針Z軸
                    int axis     = (int)WMX3軸定義.植針Z軸;
                    int position = fChangeSetZ;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstSetZ;
        }  // end of public double dbapiSetZ(double dbIncreaseSetZ)  //SetZ
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionSetR = 0.0;
        public double dbapiSetR(double dbIncreaseSetR, double dbTargetSpeed)  //SetR
        {
            Normal calculate = new Normal();
                const int    MaxRAW = 360000;
                const int    MinRAW =      0;
                const double Maxdb  =  360.0;
                const double Mindb  =    0.0;
                const double Sum    = 360000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstSetR        = 0.0;

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

            //Function Classification
            switch(dbIncreaseSetR) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionSetR * 1.01 > dbTargetPositionSetR * 0.99) { 
                        dbMin = dbTargetPositionSetR * 0.99 - 0.1;
                        dbMax = dbTargetPositionSetR * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionSetR * 1.01 - 0.1;
                        dbMax = dbTargetPositionSetR * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstSetR &&
                                 dbRstSetR <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //植針R軸 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseSetR && dbIncreaseSetR<=Maxdb ) {

                    } else if( dbIncreaseSetR<=Mindb ) {
                        dbIncreaseSetR = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseSetR ) {
                        dbIncreaseSetR = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeSetR      = calculate.Map(dbIncreaseSetR, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionSetR = dbIncreaseSetR;

                    //執行移動植針R軸
                    int axis     = (int)WMX3軸定義.植針R軸;
                    int position = fChangeSetR;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstSetR;
        }  // end of public double dbapiSetR(double dbIncreaseSetR)  //SetR
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionGate = 0.0;
        public double dbapiGate(double dbIncreaseGate, double dbTargetSpeed)  //Gate
        {
            Normal calculate = new Normal();
                const int    MaxRAW =  58000;
                const int    MinRAW =      0;
                const double Maxdb  =  580.0;
                const double Mindb  =    0.0;
                const double Sum    =  58000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstGate        = 0.0;

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

            //Function Classification
            switch(dbIncreaseGate) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionGate * 1.01 > dbTargetPositionGate * 0.99) { 
                        dbMin = dbTargetPositionGate * 0.99 - 0.1;
                        dbMax = dbTargetPositionGate * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionGate * 1.01 - 0.1;
                        dbMax = dbTargetPositionGate * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstGate &&
                                 dbRstGate <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //工作門 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseGate && dbIncreaseGate<=Maxdb ) {

                    } else if( dbIncreaseGate<=Mindb ) {
                        dbIncreaseGate = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseGate ) {
                        dbIncreaseGate = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    int fChangeGate = calculate.Map(dbIncreaseGate, Maxdb, Mindb, MaxRAW, MinRAW);
                    dbTargetPositionGate = dbIncreaseGate;

                    //執行移動工作門
                    int axis     = (int)WMX3軸定義.工作門;
                    int position = fChangeGate;
                    int speed    = (int)(dbTargetSpeed * (MaxRAW/ Maxdb));
                    int accel    = speed;
                    int daccel   = speed;
                    clsServoControlWMX3.WMX3_Pivot(axis, position, speed, accel, daccel);
                } break;
            }

            return dbRstGate;
        }  // end of public double dbapiGate(double dbIncreaseGate)  //Gate
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionIAI = 0.0;
        public double dbapiIAI(double dbIncreaseIAI)  //IAI
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   3000;
                const int    MinRAW =      0;
                const double Maxdb  =   30.0;
                const double Mindb  =    0.0;
                const double Sum    =   3000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstIAI         = 0.0;

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

            //Function Classification
            switch(dbIncreaseIAI) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionIAI * 1.01 > dbTargetPositionIAI * 0.99) { 
                        dbMin = dbTargetPositionIAI * 0.99 - 0.1;
                        dbMax = dbTargetPositionIAI * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionIAI * 1.01 - 0.1;
                        dbMax = dbTargetPositionIAI * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstIAI &&
                                 dbRstIAI <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //IAI 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseIAI && dbIncreaseIAI<=Maxdb ) {

                    } else if( dbIncreaseIAI<=Mindb ) {
                        dbIncreaseIAI = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseIAI ) {
                        dbIncreaseIAI = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    double fChangeGate = calculate.Map(dbIncreaseIAI, (double)Maxdb, (double)Mindb, (double)Maxdb, (double)Mindb);
                    dbTargetPositionIAI = dbIncreaseIAI;

                    clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 1);

                    //執行移動工作門
                    clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GoToPosition, fChangeGate);
                } break;
            }

            return dbRstIAI;
        }  // end of public double dbapiIAI(double dbIncreaseIAI)  //IAI
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionJoDell3D掃描 = 0.0;
        public double dbapiJoDell3D掃描(double dbIncreaseJoDell3D)  //JoDell3D掃描
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

            //Function Classification
            switch(dbIncreaseJoDell3D) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionJoDell3D掃描 * 1.01 > dbTargetPositionJoDell3D掃描 * 0.99) { 
                        dbMin = dbTargetPositionJoDell3D掃描 * 0.99 - 0.1;
                        dbMax = dbTargetPositionJoDell3D掃描 * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionJoDell3D掃描 * 1.01 - 0.1;
                        dbMax = dbTargetPositionJoDell3D掃描 * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstJoDell3D掃描 &&
                                 dbRstJoDell3D掃描 <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //3D掃描 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseJoDell3D && dbIncreaseJoDell3D<=Maxdb ) {

                    } else if( dbIncreaseJoDell3D<=Mindb ) {
                        dbIncreaseJoDell3D = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseJoDell3D ) {
                        dbIncreaseJoDell3D = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    double fChangeGate = calculate.Map(dbIncreaseJoDell3D, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);
                    dbTargetPositionJoDell3D掃描 = dbIncreaseJoDell3D;

                    //執行移動JoDell3D掃描
                    clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
                } break;
            }

            return dbRstJoDell3D掃描;
        }  // end of public double dbapiJoDell3D掃描(double dbIncreaseJoDell3D)  //JoDell3D掃描
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionJoDell吸針嘴 = 0.0;
        public double dbapiJoDell吸針嘴(double dbIncreaseJoDell吸針嘴)  //JoDell吸針嘴
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

            //Function Classification
            switch(dbIncreaseJoDell吸針嘴) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionJoDell吸針嘴 * 1.01 > dbTargetPositionJoDell吸針嘴 * 0.99) { 
                        dbMin = dbTargetPositionJoDell吸針嘴 * 0.99 - 0.1;
                        dbMax = dbTargetPositionJoDell吸針嘴 * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionJoDell吸針嘴 * 1.01 - 0.1;
                        dbMax = dbTargetPositionJoDell吸針嘴 * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstJoDell吸針嘴 &&
                                 dbRstJoDell吸針嘴 <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //3D掃描 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseJoDell吸針嘴 && dbIncreaseJoDell吸針嘴<=Maxdb ) {

                    } else if( dbIncreaseJoDell吸針嘴<=Mindb ) {
                        dbIncreaseJoDell吸針嘴 = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseJoDell吸針嘴 ) {
                        dbIncreaseJoDell吸針嘴 = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    double fChangeGate = calculate.Map(dbIncreaseJoDell吸針嘴, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);
                    dbTargetPositionJoDell吸針嘴 = dbIncreaseJoDell吸針嘴;

                    //執行移動JoDell吸針嘴
                    clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
                } break;
            }

            return dbRstJoDell吸針嘴;
        }  // end of public double dbapiJoDell吸針嘴(double dbIncreaseJoDell吸針嘴)  //JoDell吸針嘴
        //---------------------------------------------------------------------------------------
        public double dbTargetPositionJoDell植針嘴 = 0.0;
        public double dbapiJoDell植針嘴(double dbIncreaseJoDell植針嘴)  //JoDell植針嘴
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

            //Function Classification
            switch(dbIncreaseJoDell植針嘴) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionJoDell植針嘴 * 1.01 > dbTargetPositionJoDell植針嘴 * 0.99) { 
                        dbMin = dbTargetPositionJoDell植針嘴 * 0.99 - 0.1;
                        dbMax = dbTargetPositionJoDell植針嘴 * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionJoDell植針嘴 * 1.01 - 0.1;
                        dbMax = dbTargetPositionJoDell植針嘴 * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstJoDell植針嘴 &&
                                 dbRstJoDell植針嘴 <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //植針嘴 變更位置
                    //伸長量overflow保護
                    if( Mindb<=dbIncreaseJoDell植針嘴 && dbIncreaseJoDell植針嘴<=Maxdb ) {

                    } else if( dbIncreaseJoDell植針嘴<=Mindb ) {
                        dbIncreaseJoDell植針嘴 = (int)Mindb;
                    } else if( Maxdb<=dbIncreaseJoDell植針嘴 ) {
                        dbIncreaseJoDell植針嘴 = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    double fChangeGate = calculate.Map(dbTargetPositionJoDell植針嘴, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);
                    dbTargetPositionJoDell植針嘴 = dbTargetPositionJoDell植針嘴;

                    //執行移動JoDell植針嘴
                    clsServoControlWMX3.WMX3_JoDell植針嘴(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
                } break;
            }

            return dbRstJoDell植針嘴;
        }  // end of public double dbapiJoDell植針嘴(double dbIncreaseJoDell植針嘴)  //JoDell植針嘴
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
            switch(e.KeyCode) {
                case Keys.Enter:
                    i計時300ms = 0;

                    if (BarcodeBuffer.Count > 0) {
                        btn_OpenFile_Click(sender, e);
                        BarcodeBuffer.Clear();
                    }
                    break;

                default:
                    // 判斷輸入字為: 0~9 或 'a'~'z' 或 'A'~'Z'
                    if (Char.IsLetter((char)e.KeyCode) || Char.IsDigit((char)e.KeyCode) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)) {
                        BarcodeBuffer.Add((char)e.KeyCode);  // 將有效的字符添加到緩衝區
                        i計時300ms = i計時300ms_Define;
                    }
                    break;
            }
        }

        public void tmrBarCodeScanner_Tick(object sender, EventArgs e)
        {
            if (i計時300ms > 0) {
                i計時300ms--;
            }

            if (i計時300ms == 0) {
                i計時300ms = 0;

                if (BarcodeBuffer.Count > 0) {
                    BarcodeBuffer.Clear();
                }
            }
        }
        //---------------------------------------------------------------------------------------
        double dbapiCallbackSendParameter(string GetPara)
        {
            double rsult = 0.0;

                   if (GetPara == "NeedleCircleParameter") {  //Socket針孔 真圓相似度
                rsult = apiParaReadIndex("SaveParameterJason.json", 18);
            } else if (GetPara == "NeedleHeadLength") {  //針頭長
                rsult = apiParaReadIndex("SaveParameterJason.json", 19);
            } else if (GetPara == "NeedleHeadWidth") {  //針頭寬
                rsult = apiParaReadIndex("SaveParameterJason.json", 20);
            } else if (GetPara == "NeedleTailLength") {  //針尾長
                rsult = apiParaReadIndex("SaveParameterJason.json", 21);
            } else if (GetPara == "NeedleTailWidth") {  //針尾寬
                rsult = apiParaReadIndex("SaveParameterJason.json", 22);
            } else if (GetPara == "NeedleLengthMax") {  //針長Max
                rsult = apiParaReadIndex("SaveParameterJason.json", 23);
            } else if (GetPara == "NeedleLengthMin") {  //針長Min
                rsult = apiParaReadIndex("SaveParameterJason.json", 24);
            } else if (GetPara == "NeedleWidthMax") {  //針寬Max
                rsult = apiParaReadIndex("SaveParameterJason.json", 25);
            } else if (GetPara == "NeedleWidthMin") {  //針寬Min
                rsult = apiParaReadIndex("SaveParameterJason.json", 26);
            } else if (GetPara == "NeedleThreshold") {  //閥值
                rsult = apiParaReadIndex("SaveParameterJason.json", 27);
            }

            return rsult;
        }
        //---------------------------------------------------------------------------------------
        public void Form1_Load(object sender, EventArgs e)
        {
            //Add the callback api from snapshot api
            inspector1.on下視覺 = apiCallBackTest;
            inspector1.getParam = dbapiCallbackSendParameter;

            //init vision
            inspector1.xInit();

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
        ParameterForm fmParameterFormHandle;
        public void btn_參數_Click(object sender, EventArgs e)
        {
            ParameterForm fmParameterForm = new ParameterForm();
            fmParameterForm.Show();

            fmParameterFormHandle = fmParameterForm;

            btn_參數.Enabled = false;
        }
        //---------------------------------------------------------------------------------------
        public void btn_植針_Click(object sender, EventArgs e)
        {
            PlaceForm fmPlaceForm = new PlaceForm();
            fmPlaceForm.Show();

            btn_植針.Enabled = false;
        }
        //---------------------------------------------------------------------------------------
        private void btn_取針_Click(object sender, EventArgs e)
        {
            RemoveForm fmRemoveForm = new RemoveForm();
            fmRemoveForm.Show();

            btn_取針.Enabled = false;
        }
        //---------------------------------------------------------------------------------------
        private void btn_置換_Click(object sender, EventArgs e)
        {
            ReplaceForm fmReplaceForm = new ReplaceForm();
            fmReplaceForm.Show();

            btn_置換.Enabled = false;
        }
        //---------------------------------------------------------------------------------------
        private void btn_拋料_Click(object sender, EventArgs e)
        {
            TakePinForm fmTakePinForm = new TakePinForm();
            fmTakePinForm.Show();

            btn_拋料.Enabled = false;
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
            if (rslt == 1) {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }

            axis = (int)WMX3軸定義.吸嘴Y軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1) {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }

            axis = (int)WMX3軸定義.吸嘴Z軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1) {
                clsServoControlWMX3.WMX3_SetHomePosition(axis);
            }

            axis = (int)WMX3軸定義.吸嘴R軸;
            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1) {
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
                } else if (SelectLabel == lbl_NA_25       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_NA_O_07)          / 10, (int)(WMX3IO對照.pxeIO_NA_O_07)          % 10, (lbl_NA_25.BackColor       == Color.Red) ? (byte)1 : (byte)0);

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
                } else if (SelectLabel == lbl_NA_31       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_NA_O_36)          / 10, (int)(WMX3IO對照.pxeIO_NA_O_36)          % 10, (lbl_NA_31.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                } else if (SelectLabel == lblBuzzer       ) { clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Buzzer)           / 10, (int)(WMX3IO對照.pxeIO_Buzzer)           % 10, (lblBuzzer.BackColor       == Color.Red) ? (byte)1 : (byte)0);
                }
            }  // end of if (SelectLabel != null) {
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
                    case WMX3軸定義.JoDell3D掃描:    if(enGC_JoDell3D掃描  == true) { dbapiJoDell3D掃描(result);       } break;
                    case WMX3軸定義.JoDell吸針嘴:    if(enGC_JoDell吸針嘴  == true) { dbapiJoDell吸針嘴(result);       } break;
                    case WMX3軸定義.JoDell植針嘴:    if(enGC_JoDell植針嘴  == true) { dbapiJoDell植針嘴(result);       } break;
                }
            }

            txtABSpos.Text = result.ToString("F3");
        }  // end of public void btn_adjust_JOG(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        private void btn_plus_minus_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button ptrBtn = sender as System.Windows.Forms.Button;

            double result = double.Parse(txtABSpos.Text);
            double dbdiff = double.Parse(edit_diff_value.Text);

            if (ptrBtn == btn_plus ) {
                txtABSpos.Text = (result + dbdiff).ToString("F3");
            } else if (ptrBtn == btn_minus) {
                txtABSpos.Text = (result - dbdiff).ToString("F3");
            }
            edit_diff_value.Text = 0.0.ToString("F3");
        }
        //---------------------------------------------------------------------------------------
        //讀取OutputIO
        public byte[] pDataGetOutIO = new byte[4];

        //讀取InputIO
        public byte[] pDataGetInIO = new byte[8];

        public void tmr_ReadWMX3_Tick(object sender, EventArgs e)
        {  // start of public void tmr_ReadWMX3_Tick(object sender, EventArgs e)
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

                dbapiJoDell3D掃描(dbState);
                dbapiJoDell吸針嘴(dbState);
                dbapiJoDell植針嘴(dbState);
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
                lbl_NA_25.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_NA_O_07)          / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA_O_07)           % 10)) != 0) ? Color.Green : Color.Red;

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

                lbl右按鈕綠燈.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板右按鈕綠燈)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl紅燈.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台紅燈)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台紅燈)          % 10)) != 0) ? Color.Green : Color.Red;
                lbl中按鈕綠燈.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板中按鈕綠燈)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl黃燈.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台黃燈)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台黃燈)          % 10)) != 0) ? Color.Green : Color.Red;
                lbl左按鈕紅燈.BackColor   = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_面板左按鈕紅燈)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl綠燈.BackColor         = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_機台綠燈)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_機台綠燈)          % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_31.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_NA_O_36)          / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA_O_36)           % 10)) != 0) ? Color.Green : Color.Red;
                lblBuzzer.BackColor       = ((pDataGetOutIO[((int)(WMX3IO對照.pxeIO_Buzzer)           / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Buzzer)            % 10)) != 0) ? Color.Green : Color.Red;
            }  // end of clsServoControlWMX3.WMX3_GetOutIO(ref pDataGetOutIO, (int)WMX3IO對照.pxeIO_Addr4, 4);

            //讀取 Yaskawa InputIO
            clsServoControlWMX3.WMX3_GetInIO(ref pDataGetInIO, (int)WMX3IO對照.pxeIO_Addr28, 8);
            {
                lbl載盤Y後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤Y軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤Y軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料Y後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料Y軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料Y軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤Y前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤Y軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤Y軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料Y前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料Y軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料Y軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料X後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料X軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料X軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_01.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA05)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA05)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料X前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料X軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料X軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_02.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA07)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA07)            % 10)) != 0) ? Color.Green : Color.Red;         

                lbl植針Z後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_植針Z軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針Z軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_03.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA11)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA11)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl植針Z前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_植針Z軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_植針Z軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_04.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA13)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA13)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤X前.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤X軸前極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤X軸前極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_05.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA15)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA15)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤X後.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤X軸後極限)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤X軸後極限)   % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_06.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA17)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA17)            % 10)) != 0) ? Color.Green : Color.Red;

                lbl載盤空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤真空檢1)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤真空檢1)     % 10)) != 0) ? Color.Green : Color.Red;
                lblsk2空1.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket2真空檢1)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket2真空檢1)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl載盤空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_載盤真空檢2)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_載盤真空檢2)     % 10)) != 0) ? Color.Green : Color.Red;
                lblsk2空2.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket2真空檢2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket2真空檢2)  % 10)) != 0) ? Color.Green : Color.Red;
                lblsk1空1.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket1真空檢1)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket1真空檢1)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座真空檢1)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座真空檢1)   % 10)) != 0) ? Color.Green : Color.Red;
                lblsk1空2.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_Socket1真空檢2)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_Socket1真空檢2)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl擺放空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座真空檢2)   / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座真空檢2)   % 10)) != 0) ? Color.Green : Color.Red;

                lbl吸嘴空1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸嘴真空檢1)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸嘴真空檢1)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_07.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA31)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA31)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸嘴空2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸嘴真空檢2)     / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸嘴真空檢2)     % 10)) != 0) ? Color.Green : Color.Red;
                lbl取料ng盒.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_取料NG收料盒)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_取料NG收料盒)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl兩點壓1.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_兩點組合壓力檢1) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_兩點組合壓力檢1) % 10)) != 0) ? Color.Green : Color.Red;
                lbl堵料盒.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_堵料收料盒)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_堵料收料盒)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl兩點壓2.BackColor    = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_兩點組合壓力檢2) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_兩點組合壓力檢2) % 10)) != 0) ? Color.Green : Color.Red;
                lbl吸料盒.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_吸料收料盒)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_吸料收料盒)      % 10)) != 0) ? Color.Green : Color.Red;            

                lbl復歸鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_復歸按鈕)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_復歸按鈕)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_08.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA41)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA41)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl啟動鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_啟動按鈕)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_啟動按鈕)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_09.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA43)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA43)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl停止鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_停止按鈕)        / 10)] & (1 << (int)(WMX3IO對照.pxeIO_停止按鈕)        % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_10.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA45)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA45)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl急停鈕.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_緊急停止按鈕)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_緊急停止按鈕)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_11.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA47)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA47)            % 10)) != 0) ? Color.Green : Color.Red;

                lbl_擺放座開.BackColor  = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板開)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板開)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_13.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA51)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA51)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl_擺放座關.BackColor  = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板合)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板合)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_15.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA53)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA53)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_16.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA54)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA54)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_17.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA55)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA55)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_18.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA56)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA56)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_19.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA57)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA57)            % 10)) != 0) ? Color.Green : Color.Red;

                lbl上左右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩左側右門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩左側右門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上右右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩右側右門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩右側右門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上左左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩左側左門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩左側左門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上右左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩右側左門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩右側左門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl上後右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩後側右門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩後側右門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl螢幕小門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_螢幕旁小門)      / 10)] & (1 << (int)(WMX3IO對照.pxeIO_螢幕旁小門)      % 10)) != 0) ? Color.Green : Color.Red;
                lbl上後左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_上罩後側左門)    / 10)] & (1 << (int)(WMX3IO對照.pxeIO_上罩後側左門)    % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_20.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA67)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA67)            % 10)) != 0) ? Color.Green : Color.Red;

                lbl下左右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架左側右門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架左側右門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架後側左門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架後側左門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下左左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架左側左門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架左側左門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下後右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架後側右門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架後側右門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl下右右門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架右側右門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架右側右門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_23.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA75)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA75)            % 10)) != 0) ? Color.Green : Color.Red;
                lbl下右左門.BackColor   = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_下支架右側左門)  / 10)] & (1 << (int)(WMX3IO對照.pxeIO_下支架右側左門)  % 10)) != 0) ? Color.Green : Color.Red;
                lbl_NA_24.BackColor     = ((pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA76)            / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA76)            % 10)) != 0) ? Color.Green : Color.Red;
            }  // end of clsServoControlWMX3.WMX3_GetInIO(ref pDataGetInIO, (int)WMX3IO對照.pxeIO_Addr28, 8);

        }  // end of public void tmr_ReadWMX3_Tick(object sender, EventArgs e)
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
            clsVibration.apiEstablishTCPVibration(); {
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
        public enum xe_tmr_home {
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
        public xe_tmr_home xeTmrHome = xe_tmr_home.xets_empty;

        public int ihomeFinishedCNT = 0;
        public bool bhome    = false;
        public bool bGotHome = false;

        public const double dbNozzle安全原點X = 242;
        public const double dbNozzle安全原點Y = 28;
        public const double dbNozzle安全原點Z = 0;
        public const double dbNozzle安全原點R = 1.350;

        //---------------------------------------------------------------------------------------
        public void btn_home_Click(object sender, EventArgs e)
        {
            bhome    = true;
        }
        //---------------------------------------------------------------------------------------
        public void tmr_Home_Tick(object sender, EventArgs e)
        {
            int getrslt = 0;
            lbl_debug.Text = clsServoControlWMX3.WMX3_check_ServoOpState((int)WMX3軸定義.工作門, ref getrslt);

            switch (xeTmrHome) {
                case xe_tmr_home.xets_home_start:
                    btn_home.Text = "Start Home";

                    //Workaround for prevent Z Collide
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, false);  Thread.Sleep(200);
                    dbapiJoDell3D掃描(10);                                                Thread.Sleep(10);
                    dbapiJoDell吸針嘴(5);                                                 Thread.Sleep(10);
                    dbapiJoDell植針嘴(10);                                                Thread.Sleep(10);
                    dbapiSetZ(15, 33);                                                    Thread.Sleep(200);

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

                    xeTmrHome = xe_tmr_home.xets_home_StartGate_01; 
                    break;

                case xe_tmr_home.xets_home_StartGate_01:
                    en_工作門.Checked = true;
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門, true);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_StartGate_02;
                    break;

                case xe_tmr_home.xets_home_StartGate_02:
                    dbapiGate(580, 580/4);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_CheckGate;
                    break;

                case xe_tmr_home.xets_home_CheckGate:
                    if(dbapiGate(dbCheckArrived, 0) == dbAxisMoveOk) {
                        xeTmrHome = xe_tmr_home.xets_home_鬆開擺放座蓋板;
                    }
                    break;

                case xe_tmr_home.xets_home_鬆開擺放座蓋板:
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板) / 10, (int)(WMX3IO對照.pxeIO_擺放座蓋板) % 10, 0);
                    xeTmrHome = xe_tmr_home.xets_home_EndGate;
                    break;

                case xe_tmr_home.xets_home_EndGate:
                case xe_tmr_home.xets_home_StartZR電動缸Home_01:
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

                    xeTmrHome = xe_tmr_home.xets_home_StartZR電動缸Home_02;
                    break;

                case xe_tmr_home.xets_home_StartZR電動缸Home_02:
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

                        xeTmrHome = xe_tmr_home.xets_home_CheckZR電動缸Home;
                    }
                    break;

                case xe_tmr_home.xets_home_CheckZR電動缸Home:
                    if(true) {
                        int rslt01 = 0, rslt02 = 0;
                        int axis01 = 0, axis02 = 0;

                        axis01 = (int)WMX3軸定義.吸嘴Z軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        axis02 = (int)WMX3軸定義.吸嘴R軸;
                        rslt02 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis02);  Thread.Sleep(10);

                        if (rslt01==1 && rslt02==1) {
                            if (dbapiNozzleZ(dbRead, 0) <= 1.0) {
                                xeTmrHome = xe_tmr_home.xets_home_EndZR電動缸Home;
                            }
                        }
                    }
                    break;

                case xe_tmr_home.xets_home_EndZR電動缸Home:
                    dbapiIAI(10);          Thread.Sleep(10);

                    dbapiJoDell3D掃描(10);  Thread.Sleep(10);
                    dbapiJoDell吸針嘴( 5);  Thread.Sleep(10);
                    dbapiJoDell植針嘴(10);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_StartXYHome_01;
                    break;

                case xe_tmr_home.xets_home_StartXYHome_01:
                    en_吸嘴X軸.Checked = true;
                    en_吸嘴Y軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, true);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, true);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_StartXYHome_02;
                    break;

                case xe_tmr_home.xets_home_StartXYHome_02:
                    if (dbapiNozzleZ(dbRead, 0) <= 1.0) {
                        dbapiNozzleX(dbNozzle安全原點X, 50);   Thread.Sleep(10);
                        dbapiNozzleY(dbNozzle安全原點Y, 10);   Thread.Sleep(10);
                        xeTmrHome = xe_tmr_home.xets_home_CheckXYHome;
                    }
                    break;

                case xe_tmr_home.xets_home_CheckXYHome:
                    if (dbapiNozzleZ(dbRead, 0) <= 1.0) { 
                        int rslt01 = 0, rslt02 = 0;
                        int axis01 = 0, axis02 = 0;

                        axis01 = (int)WMX3軸定義.吸嘴X軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        axis02 = (int)WMX3軸定義.吸嘴Y軸;
                        rslt02 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis02);  Thread.Sleep(10);

                        if (rslt01 == 1 && rslt02 == 1) {
                            xeTmrHome = xe_tmr_home.xets_home_EndXYHome;
                        }
                    }
                    break;

                case xe_tmr_home.xets_home_EndXYHome:
                case xe_tmr_home.xets_home_StartSetZR_01:
                    en_植針Z軸.Checked = true;
                    en_植針R軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, true);  Thread.Sleep(10);
                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, true);  Thread.Sleep(10);

                    xeTmrHome = xe_tmr_home.xets_home_StartSetZR_02;
                    break;

                case xe_tmr_home.xets_home_StartSetZR_02:
                    dbapiSetZ(15, 33);           Thread.Sleep(10);
                    dbapiSetR(268.08, 360);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_CheckSetZR;
                    break;

                case xe_tmr_home.xets_home_CheckSetZR:
                    if (true) {
                        int rslt01 = 0, rslt02 = 0;
                        int axis01 = 0, axis02 = 0;

                        axis01 = (int)WMX3軸定義.植針Z軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        axis02 = (int)WMX3軸定義.植針R軸;
                        rslt02 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis02);  Thread.Sleep(10);

                        if (rslt01 == 1 && rslt02 == 1) {
                            if (dbapiSetZ(dbRead, 0) <= 16) {
                                xeTmrHome = xe_tmr_home.xets_home_EndSetZR01;
                            }
                        }
                    }
                    break;

                case xe_tmr_home.xets_home_EndSetZR01:
                case xe_tmr_home.xets_home_StartCarrierXHome_01:
                    en_載盤X軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, true);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_StartCarrierXHome_02;
                    break;

                case xe_tmr_home.xets_home_StartCarrierXHome_02:
                    if (dbapiSetZ(dbRead, 0) <= 16) {
                        dbapiCarrierX(95, 190*0.2);
                        xeTmrHome = xe_tmr_home.xets_home_CheckCarrierXHome;
                    }
                    break;

                case xe_tmr_home.xets_home_CheckCarrierXHome:
                    if (true) {
                        int rslt01 = 0;
                        int axis01 = 0;

                        axis01 = (int)WMX3軸定義.載盤X軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        if (rslt01 == 1) {
                            if (dbapiSetZ(dbRead, 0) <= 16) {
                                xeTmrHome = xe_tmr_home.xets_home_EndCarrierXHome;
                            }
                        }
                    }
                    break;

                case xe_tmr_home.xets_home_EndCarrierXHome:
                case xe_tmr_home.xets_home_StartCarrierYHome_01:
                    en_載盤Y軸.Checked = true;

                    clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, true);  Thread.Sleep(10);
                    xeTmrHome = xe_tmr_home.xets_home_StartCarrierYHome_02;
                    break;

                case xe_tmr_home.xets_home_StartCarrierYHome_02:
                    if (dbapiSetZ(dbRead, 0) <= 16) {
                        dbapiCarrierY(10, 800*0.2);
                        xeTmrHome = xe_tmr_home.xets_home_CheckCarrierYHome;
                    }
                    break;

                case xe_tmr_home.xets_home_CheckCarrierYHome:
                    if (true) {
                        int rslt01 = 0;
                        int axis01 = 0;

                        axis01 = (int)WMX3軸定義.載盤Y軸;
                        rslt01 = clsServoControlWMX3.WMX3_check_ServoMovingState(axis01);  Thread.Sleep(10);

                        if (rslt01 == 1) {
                            if (dbapiSetZ(dbRead, 0) <= 16) {
                                xeTmrHome = xe_tmr_home.xets_home_EndCarrierYHome;
                            }
                        }
                    }
                    break;

                case xe_tmr_home.xets_home_EndCarrierYHome:
                case xe_tmr_home.xets_home_end:
                    dbapiNozzleR(dbNozzle安全原點R, 36);  Thread.Sleep(10);
                    dbapiGate(0, 580/4);                  Thread.Sleep(10);

                    bGotHome = true;

                    xeTmrHome = xe_tmr_home.xets_end;
                    break;

                default:
                case xe_tmr_home.xets_empty:
                case xe_tmr_home.xets_idle:
                case xe_tmr_home.xets_end:
                    btn_home.Text = "Home";

                    if(bhome == true) {
                        bhome    = false;
                        bGotHome = false;
                        xeTmrHome = xe_tmr_home.xets_home_start;
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

                                                                               /* bChambered */ 
                                                                               xett_植針成功,
                                                                                   xett_擺放座蓋板再次打開,
                                                                                   xett_檢查擺放座蓋板是否再次打開,
                                                                                   xett_擺放座蓋板再次打開等待1秒,
                                                                                   xett_確認擺放座蓋板再次打開,

                                                                                   xett_擺放座R軸再次至放料位,
                                                                                   xett_檢查擺放座R軸是否再次至放料位,
                                                                                   xett_確認擺放座R軸再次至放料位,

                                                                               xett_植針失敗,
                                                                                   xett_NozzleZ回保護位,
                                                                                   xett_等待NozzleZ回保護位,
                                                                                   xett_確認NozzleZ回保護位,
                                                                                   xett_設定NozzleXY回家,

                                                                                   xett_SetZ回保護放料位,
                                                                                   xett_等待SetZ回保護放料位,
                                                                                   xett_確認SetZ回保護放料位,

                                                                                   xett_載盤XY移置堵料檢查位,
                                                                                   xett_等待載盤XY移置堵料檢查位,
                                                                                   xett_確認載盤XY移置堵料檢查位,

                                                                                   xett_檢查堵料相機移至檢查堵料孔高度,
                                                                                   xett_等待檢查堵料相機移至檢查堵料孔高度,
                                                                                   xett_確認檢查堵料相機移至檢查堵料孔高度,

                                                                                   xett_擺放座蓋板打開供檢查,
                                                                                   xett_等待擺放座蓋板打開供檢查,
                                                                                   xett_確認擺放座蓋板打開供檢查,

                                                                                   xett_SetR移至檢查堵料孔檢查位,
                                                                                   xett_等待SetR移至檢查堵料孔檢查位,
                                                                                   xett_確認SetR移至檢查堵料孔檢查位,

                                                                                   xett_SetZ移至檢查堵料孔高度,
                                                                                   xett_等待SetZ移至檢查堵料孔高度,
                                                                                   xett_確認SetZ移至檢查堵料孔高度,

                                                                                   xett_取得視覺判斷堵料孔狀態,

                                                                                   xett_判斷未堵料,

                                                                                   xett_判斷堵料,
                                                                                       xett_SetR移至植針位,
                                                                                       xett_等待SetR移至植針位,
                                                                                       xett_確認SetR移至植針位,

                                                                                       xett_SetZ再次回保護放料位,
                                                                                       xett_等待SetZ再次回保護放料位,
                                                                                       xett_確認SetZ再次回保護放料位,

                                                                                       xett_載盤Y移置堵料收料位,
                                                                                       xett_等待載盤Y移置堵料收料位,
                                                                                       xett_確認載盤Y移置堵料收料位,

                                                                                       xett_SetZ移置堵料收料位,
                                                                                       xett_等待SetZ移置堵料收料位,
                                                                                       xett_確認SetZ移置堵料收料位,

                                                                                       xett_堵料吹氣桿電磁閥打開,      //堵料吹氣缸->進去
                                                                                       xett_等待堵料吹氣桿電磁閥打開,  //堵料吹氣缸->進去
                                                                                       xett_確認堵料吹氣桿電磁閥打開,  //堵料吹氣缸->進去

                                                                                       xett_堵料吹氣電磁閥打開,
                                                                                       xett_等待堵料吹氣電磁閥打開,
                                                                                       xett_堵料吹氣電磁閥關閉,
                                                                                       xett_等待堵料吹氣電磁閥關閉,

                                                                                       xett_堵料吹氣桿電磁閥關閉,      //堵料吹氣缸->出去
                                                                                       xett_等待堵料吹氣桿電磁閥關閉,  //堵料吹氣缸->出去
                                                                                       xett_確認堵料吹氣桿電磁閥關閉,  //堵料吹氣缸->出去

                                                                                       xett_重檢堵孔,

                /* bTakePin */ /*+*/ /* bChambered */ /*+*/ /*  bRemove  */
                xett_檢測是否還需要取針,
                    xett_還需要取針,
                        xett_重覆一開始的狀態,

                    xett_不需要取針,
                        xett_NozzleXYR移置安全位置,
                        xett_檢查NozzleXYR是否移至安全位置,
                        xett_確認NozzleXYR在安全位置,

                xett_取針結束,

                xett_回Home保護,

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

        public const double db取料Nozzle中心點X = 49.93;
        public const double db取料Nozzle中心點Y = 49.81;
        public const double db取料Nozzle中心點Z = 26;
        public const double db取料Nozzle中心點R = 1.34+0.7;

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
        }
        //---------------------------------------------------------------------------------------
        public void btn抽針_Click(object sender, EventArgs e)
        {
            bRemove = true;
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

                            if(求出取料循環次數>=1) {
                                if (bTakePin == true) { 
                        
                                } else if(bChambered == true || bRemove == true) {
                                    //讀DXF
                                    btn_取得目標座標_Click(sender, e);
                                    iPC = 0;
                                    iRC = 0;
                                    if(bChambered == true || bRemove == true) {
                                        //讀DXF確定有資料
                                    } else {  //if(bChambered == false && bRemove == false) {
                                        //讀DXF確定沒資料
                                        xeTmrTakePin = xe_tmr_takepin.xett_取針結束;
                                    }
                                }
                            }

                    }  // end of if(bTakePin==true || bChambered==true || bRemove==true) {
                    break;

                    case xe_tmr_takepin.xett_確定執行要取針:                               xeTmrTakePin = xe_tmr_takepin.xett_關工作門;  break;
                    case xe_tmr_takepin.xett_關工作門:     
                        dbapiGate(580, 580/4);
                        xeTmrTakePin = xe_tmr_takepin.xett_檢查工作門關閉;
                        break;
                        case xe_tmr_takepin.xett_檢查工作門關閉:   
                            if(dbapiGate(dbCheckArrived, 0) == dbAxisMoveOk) {
                                xeTmrTakePin = xe_tmr_takepin.xett_確定工作門關閉;
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
                                case xe_tmr_takepin.xett_檢測NozzleXYR吸料位: 
                                    if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_判斷NozzleXYR吸料位為安全位置;
                                    }
                                    break;
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
                                    dbapiNozzleX(db下視覺取像X_Start,    bTakePin?500*4:500*2);
                                    dbapiNozzleY(db下視覺取像Y,          bTakePin?100*8:100*4);
                                    dbapiNozzleZ(db下視覺取像Z,          bTakePin? 40*8: 40*4);
                                    dbapiNozzleR(db取料Nozzle中心點R+90, bTakePin?360*8:360*4);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測是否在飛拍起始位置;
                                    break;
                                case xe_tmr_takepin.xett_檢測是否在飛拍起始位置: 
                                    if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認在飛拍起始位置;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認在飛拍起始位置:                        xeTmrTakePin = xe_tmr_takepin.xett_NozzleX以速度250移動來觸發飛拍;  break;

                                case xe_tmr_takepin.xett_NozzleX以速度250移動來觸發飛拍: 
                                    inspector1.下視覺正向 = true;
                                    dbapiNozzleX(db下視覺取像X_END, 250);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢測是否飛拍移動完成;
                                    break;
                                case xe_tmr_takepin.xett_檢測是否飛拍移動完成: 
                                    if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確定飛拍移動完成;
                                    }
                                    break;
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
                                case xe_tmr_takepin.xett_檢測是否在吐料位: 
                                    if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認在吐料位;
                                    }
                                    break;
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
                                case xe_tmr_takepin.xett_檢查NozzleXYR是否移至上膛位: 
                                    if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleXYR移至上膛位;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認NozzleXYR移至上膛位:                             xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板打開;  break;

                                case xe_tmr_takepin.xett_擺放座蓋板打開:      
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)/10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座蓋板是否打開;
                                    break;
                                case xe_tmr_takepin.xett_檢查擺放座蓋板是否打開: {               
                                    int 擺放座蓋板打開是否為0 = pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板) % 10);
                                    if(擺放座蓋板打開是否為0 == 0) { 
                                        //已打開
                                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板打開等待1秒;
                                    }
                                } break;
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
                                    btn_取得目標座標_Click(sender, e);
                                    if(bChambered == false) { 
                                        //沒針種
                                        //要回home跟保護位
                                        xeTmrTakePin = xe_tmr_takepin.xett_回Home保護;
                                    } else {
                                        //有針種
                                        xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置拍照檢查位;
                                    }
                                } break;

                                case xe_tmr_takepin.xett_載盤XY移置拍照檢查位: { 
                                    double dbTargetX = dbPinHolePositionX;
                                    double dbTargetY = dbPinHolePositionY;

                                    dbapiCarrierX(dbTargetX, 190*0.8);
                                    dbapiCarrierY(dbTargetY, 800*0.8);

                                    double dbSocketCamera; {
                                        dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                                        dbapiIAI(dbSocketCamera);
                                    }

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
                                    if(iTakePinFinishedCNT1>=20) { 
                                        iTakePinFinishedCNT1 = 0;
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置拍照檢查位;
                                    }
                                    break;
                                case xe_tmr_takepin.xett_確認載盤XY移置拍照檢查位:                                xeTmrTakePin = xe_tmr_takepin.xett_載盤移植直針孔相機補正位;  break;

                                case xe_tmr_takepin.xett_載盤移植直針孔相機補正位: {  
                                    btn_Socket孔檢查_Click(sender, e);

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
                                    double SetPinOffsetX, SetPinOffsetY; {
                                        SetPinOffsetX = apiParaReadIndex("SaveParameterJason.json", 13);
                                        SetPinOffsetY = apiParaReadIndex("SaveParameterJason.json", 14);
                                    }

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX + SetPinOffsetX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY + SetPinOffsetY;

                                    dbapiCarrierX(dbTargetX, 190*0.8);
                                    dbapiCarrierY(dbTargetY, 800*0.8);

                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置直針位;
                                } break;
                                case xe_tmr_takepin.xett_檢查載盤XY是否移置直針位: {
                                    double SetPinOffsetX, SetPinOffsetY; {
                                        SetPinOffsetX = apiParaReadIndex("SaveParameterJason.json", 13);
                                        SetPinOffsetY = apiParaReadIndex("SaveParameterJason.json", 14);
                                    }

                                    double dbX = dbapiCarrierX(dbRead, 0);
                                    double dbY = dbapiCarrierY(dbRead, 0);

                                    double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX + SetPinOffsetX;
                                    double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY + SetPinOffsetY;
                                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置直針位;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認載盤XY移置直針位:                                  xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸至植針位;  break;

                                case xe_tmr_takepin.xett_擺放座Z軸至植針位: {   
                                    double SetPlacePinZHight; {
                                        SetPlacePinZHight = apiParaReadIndex("SaveParameterJason.json", 11);
                                        dbapiSetZ(SetPlacePinZHight, 33);
                                    }

                                    xeTmrTakePin = xe_tmr_takepin.xett_檢查擺放座Z軸是否至植針位;
                                } break;
                                case xe_tmr_takepin.xett_檢查擺放座Z軸是否至植針位: {
                                    double dbZ = dbapiSetZ(dbRead, 0);
                                    double SetPlacePinZHight; {
                                        SetPlacePinZHight = apiParaReadIndex("SaveParameterJason.json", 11);
                                    }
                                    if( (SetPlacePinZHight * 0.99 <= dbZ && dbZ <= SetPlacePinZHight * 1.01) ) { 
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
                                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸再次至放料位;
                                    }
                                    break;


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

                                case xe_tmr_takepin.xett_檢查有無植針成功: {
                                    bool success = false;

                                    double dbSetNeedleStatus; {
                                        dbSetNeedleStatus = apiParaReadIndex("SaveParameterJason.json", 36);
                                    }
                                    switch(dbSetNeedleStatus) { 
                                        case 0: //強制判斷植針ng
                                            success = false;
                                            break;

                                        case 1: //強制判斷植針ok
                                            success = true;
                                            break;

                                        case 2: //依照視覺判斷
                                            btn_Socket孔檢查_Click(sender, e); {
                                                //取得校正攝影機校正參數
                                                success = inspector1.xInspSocket植針後檢查();
                                                label7.Text  = (success) ? "植針後檢查 OK" : "植針後檢查 NG";

                                                rtb_Status_AppendMessage(rtb_Status, $"植針 {(success ? "OK":"NG")}");
                                            }
                                            break;
                                    }  // end of switch(dbSetPinStatus) { 

                                    if(bResume == true) {
                                        bResume = false;
                                        
                                        if(success == true) { 
                                            //植針ok
                                            xeTmrTakePin = xe_tmr_takepin.xett_植針成功;
                                        } else { 
                                            //植針ng
                                            xeTmrTakePin = xe_tmr_takepin.xett_植針失敗;
                                        }
                                    }
                                } break;
                    //-----------------------------------------------------------------------------------------------------------------------------------------------
                                case xe_tmr_takepin.xett_植針成功:                      xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板再次打開;  break;

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
                                case xe_tmr_takepin.xett_確認擺放座R軸再次至放料位:                    xeTmrTakePin = xe_tmr_takepin.xett_檢測是否還需要取針;  break;
                //-----------------------------------------------------------------------------------------------------------------------------------------------
                                case xe_tmr_takepin.xett_植針失敗:                                     xeTmrTakePin = xe_tmr_takepin.xett_NozzleZ回保護位;                     break;

                                case xe_tmr_takepin.xett_NozzleZ回保護位:                              
                                    dbapiNozzleZ(0, 40*8);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待NozzleZ回保護位;                 
                                    break;
                                case xe_tmr_takepin.xett_等待NozzleZ回保護位:                          
                                    if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleZ回保護位;  
                                    }         
                                    break;
                                case xe_tmr_takepin.xett_確認NozzleZ回保護位:  
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_設定NozzleXY回家;  
                                    }                     
                                    break;
                                case xe_tmr_takepin.xett_設定NozzleXY回家:   
                                    dbapiNozzleX(dbNozzle安全原點X, 500*1);  
                                    dbapiNozzleY(dbNozzle安全原點Y, 100*1);      
                                    dbapiNozzleR(dbNozzle安全原點R, 360*8); 
                                    xeTmrTakePin = xe_tmr_takepin.xett_SetZ回保護放料位;                    
                                    break;

                                case xe_tmr_takepin.xett_SetZ回保護放料位:      
                                    dbapiSetZ(12, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待SetZ回保護放料位;                
                                    break;
                                case xe_tmr_takepin.xett_等待SetZ回保護放料位:   
                                    if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認SetZ回保護放料位;  
                                    }                
                                    break;
                                case xe_tmr_takepin.xett_確認SetZ回保護放料位:   
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_載盤XY移置堵料檢查位;  
                                    }               
                                    break;

                                case xe_tmr_takepin.xett_載盤XY移置堵料檢查位: {
                                    double CheckCarryX, CheckCarryY; {
                                        CheckCarryX = apiParaReadIndex("SaveParameterJason.json", 28);
                                        CheckCarryY = apiParaReadIndex("SaveParameterJason.json", 29);
                                    }
                                    dbapiCarrierX(CheckCarryX, 190*0.8);
                                    dbapiCarrierY(CheckCarryY, 800*0.8);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待載盤XY移置堵料檢查位;            
                                } break;
                                case xe_tmr_takepin.xett_等待載盤XY移置堵料檢查位:  
                                    if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤XY移置堵料檢查位;
                                    }          
                                    break;
                                case xe_tmr_takepin.xett_確認載盤XY移置堵料檢查位:   
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_檢查堵料相機移至檢查堵料孔高度;  
                                    }     
                                    break;

                                case xe_tmr_takepin.xett_檢查堵料相機移至檢查堵料孔高度: {
                                    double CheckCameraZ; {
                                        CheckCameraZ = apiParaReadIndex("SaveParameterJason.json", 32);
                                    }
                                    dbapiJoDell植針嘴(CheckCameraZ);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待檢查堵料相機移至檢查堵料孔高度;  
                                } break;
                                case xe_tmr_takepin.xett_等待檢查堵料相機移至檢查堵料孔高度: 
                                    if( (dbapiJoDell植針嘴(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認檢查堵料相機移至檢查堵料孔高度;  
                                    }  
                                    break;
                                case xe_tmr_takepin.xett_確認檢查堵料相機移至檢查堵料孔高度:
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座蓋板打開供檢查;  
                                    }              
                                    break;

                                case xe_tmr_takepin.xett_擺放座蓋板打開供檢查:    
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_擺放座蓋板)/10, (int)(WMX3IO對照.pxeIO_擺放座蓋板)%10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待擺放座蓋板打開供檢查;            
                                    break;
                                case xe_tmr_takepin.xett_等待擺放座蓋板打開供檢查: { 
                                    int 擺放座蓋板打開是否為0 = pDataGetOutIO[((int)(WMX3IO對照.pxeIO_擺放座蓋板) / 10)] & (1 << (int)(WMX3IO對照.pxeIO_擺放座蓋板) % 10);
                                    if(擺放座蓋板打開是否為0 == 0) { 
                                        //已打開
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認擺放座蓋板打開供檢查;            
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認擺放座蓋板打開供檢查:                     
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_SetR移至檢查堵料孔檢查位;  
                                    }            
                                    break;

                                case xe_tmr_takepin.xett_SetR移至檢查堵料孔檢查位: {
                                    double CheckSetR; {
                                        CheckSetR = apiParaReadIndex("SaveParameterJason.json", 30);
                                    }
                                    dbapiSetR(CheckSetR, 360);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待SetR移至檢查堵料孔檢查位;        
                                } break;
                                case xe_tmr_takepin.xett_等待SetR移至檢查堵料孔檢查位: 
                                    if( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認SetR移至檢查堵料孔檢查位;
                                    }        
                                    break;
                                case xe_tmr_takepin.xett_確認SetR移至檢查堵料孔檢查位: 
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_SetZ移至檢查堵料孔高度;  
                                    }           
                                    break;

                                case xe_tmr_takepin.xett_SetZ移至檢查堵料孔高度: {                       
                                    double CheckSetZ; {
                                        CheckSetZ = apiParaReadIndex("SaveParameterJason.json", 31);
                                    }
                                    dbapiSetZ(CheckSetZ, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待SetZ移至檢查堵料孔高度;          
                                } break;
                                case xe_tmr_takepin.xett_等待SetZ移至檢查堵料孔高度: {
                                    if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認SetZ移至檢查堵料孔高度;
                                    }       
                                } break;
                                case xe_tmr_takepin.xett_確認SetZ移至檢查堵料孔高度: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_取得視覺判斷堵料孔狀態;  
                                    }           
                                } break;

                                case xe_tmr_takepin.xett_取得視覺判斷堵料孔狀態: {
                                    bool success = false;

                                    double dbSetPinStatus; {
                                        dbSetPinStatus = apiParaReadIndex("SaveParameterJason.json", 33);
                                    }
                                    switch(dbSetPinStatus) { 
                                        case 0: //強制判斷堵孔
                                            success = false;
                                            break;

                                        case 1: //強制判斷未堵孔
                                            success = true;
                                            break;

                                        case 2: { //依照視覺判斷
                                            success = true;
                                        } break;
                                    }  // end of switch(dbSetPinStatus) { 

                                    if(bResume == true) {
                                        bResume = false;
                                        
                                        if(success == true) { 
                                            xeTmrTakePin = xe_tmr_takepin.xett_判斷未堵料;
                                        } else { 
                                            xeTmrTakePin = xe_tmr_takepin.xett_判斷堵料;
                                        }
                                    }
                                } break;

                                case xe_tmr_takepin.xett_判斷未堵料:                                   
                                    xeTmrTakePin = xe_tmr_takepin.xett_判斷堵料;                            
                                    break;

                                case xe_tmr_takepin.xett_判斷堵料:                                     xeTmrTakePin = xe_tmr_takepin.xett_SetR移至植針位;                      break;
                                case xe_tmr_takepin.xett_SetR移至植針位: {
                                    dbapiSetR(178.08, 360);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待SetR移至植針位;                  
                                } break;
                                case xe_tmr_takepin.xett_等待SetR移至植針位: {
                                    if( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認SetR移至植針位;
                                    }                 
                                } break;
                                case xe_tmr_takepin.xett_確認SetR移至植針位: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_SetZ再次回保護放料位;  
                                    }             
                                } break;

                                case xe_tmr_takepin.xett_SetZ再次回保護放料位: {
                                    dbapiSetZ(12, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待SetZ再次回保護放料位;            
                                } break;
                                case xe_tmr_takepin.xett_等待SetZ再次回保護放料位: {
                                    if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認SetZ再次回保護放料位;
                                    }            
                                } break;
                                case xe_tmr_takepin.xett_確認SetZ再次回保護放料位: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_載盤Y移置堵料收料位;  
                                    }                 
                                } break;

                                case xe_tmr_takepin.xett_載盤Y移置堵料收料位: {
                                    double MakeClearCarryY; {
                                        MakeClearCarryY = apiParaReadIndex("SaveParameterJason.json", 34);
                                    }
                                    dbapiCarrierY(MakeClearCarryY, 800*0.8);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待載盤Y移置堵料收料位;             
                                } break;
                                case xe_tmr_takepin.xett_等待載盤Y移置堵料收料位: {
                                    if( (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認載盤Y移置堵料收料位;
                                    }             
                                } break;
                                case xe_tmr_takepin.xett_確認載盤Y移置堵料收料位: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_SetZ移置堵料收料位;  
                                    }   
                                } break;

                                case xe_tmr_takepin.xett_SetZ移置堵料收料位: {
                                    double MakeClearSetZ; {
                                        MakeClearSetZ = apiParaReadIndex("SaveParameterJason.json", 35);
                                    }
                                    dbapiSetZ(MakeClearSetZ, 33);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待SetZ移置堵料收料位;
                                } break;
                                case xe_tmr_takepin.xett_等待SetZ移置堵料收料位: {
                                    if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認SetZ移置堵料收料位;
                                    }   
                                } break;
                                case xe_tmr_takepin.xett_確認SetZ移置堵料收料位: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_堵料吹氣桿電磁閥打開;  
                                    }   
                                } break;

                                case xe_tmr_takepin.xett_堵料吹氣桿電磁閥打開: {  //堵料吹氣缸->進去      
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣缸)       / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣缸)       % 10, 1);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待堵料吹氣桿電磁閥打開;            
                                } break;  
                                case xe_tmr_takepin.xett_等待堵料吹氣桿電磁閥打開: {  //堵料吹氣缸->進去
                                    int 堵料吹氣桿插入        = pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA54)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA54)         % 10);

                                    if(堵料吹氣桿插入 != 0) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認堵料吹氣桿電磁閥打開;
                                    }
                                } break;  
                                case xe_tmr_takepin.xett_確認堵料吹氣桿電磁閥打開: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_堵料吹氣電磁閥打開;  
                                    }  
                                } break;  //堵料吹氣缸->進去  

                                case xe_tmr_takepin.xett_堵料吹氣電磁閥打開: {
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣)         / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣)         % 10, 1);
                                    dbapiDelayCNT01(10);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待堵料吹氣電磁閥打開; 
                                } break;
                                case xe_tmr_takepin.xett_等待堵料吹氣電磁閥打開: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_堵料吹氣電磁閥關閉;  
                                    }  
                                } break;
                                case xe_tmr_takepin.xett_堵料吹氣電磁閥關閉: {
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣)         / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣)         % 10, 0);
                                    dbapiDelayCNT01(10);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待堵料吹氣電磁閥關閉;
                                } break;
                                case xe_tmr_takepin.xett_等待堵料吹氣電磁閥關閉: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_堵料吹氣桿電磁閥關閉;  
                                    }  
                                } break;

                                case xe_tmr_takepin.xett_堵料吹氣桿電磁閥關閉: {  //堵料吹氣缸->出去 
                                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_堵料吹氣缸)       / 10, (int)(WMX3IO對照.pxeIO_堵料吹氣缸)       % 10, 0);
                                    xeTmrTakePin = xe_tmr_takepin.xett_等待堵料吹氣桿電磁閥關閉;
                                } break; 
                                case xe_tmr_takepin.xett_等待堵料吹氣桿電磁閥關閉: {  //堵料吹氣缸->出去 
                                    int 堵料吹氣桿退出        = pDataGetInIO[((int)(WMX3IO對照.pxeIO_NA56)         / 10)] & (1 << (int)(WMX3IO對照.pxeIO_NA56)         % 10);
                                    if(堵料吹氣桿退出 != 0) { 
                                        dbapiDelayCNT01(10);
                                        xeTmrTakePin = xe_tmr_takepin.xett_確認堵料吹氣桿電磁閥關閉;
                                    }
                                } break;
                                case xe_tmr_takepin.xett_確認堵料吹氣桿電磁閥關閉: {
                                    if( (dbapiDelayCNT01(dbCheckArrived) == dbAxisMoveOk) ) { 
                                        xeTmrTakePin = xe_tmr_takepin.xett_重檢堵孔;  
                                    } 
                                } break;  //堵料吹氣缸->出去 

                                case xe_tmr_takepin.xett_重檢堵孔:                                     xeTmrTakePin = xe_tmr_takepin.xett_SetZ回保護放料位;                    break;
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
                        double dbSocketCamera; {
                            dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                            dbapiIAI(dbSocketCamera);
                        }

                        xeTmrTakePin = xe_tmr_takepin.xett_擺放座Z軸縮回;
                    } break;
                    case xe_tmr_takepin.xett_擺放座Z軸縮回: {
                        dbapiSetZ(12, 33);
                        xeTmrTakePin = xe_tmr_takepin.xett_3D掃描電動缸縮回;
                    } break;
                    case xe_tmr_takepin.xett_3D掃描電動缸縮回: {
                         dbapiJoDell3D掃描(10);
                        xeTmrTakePin = xe_tmr_takepin.xett_吸針嘴電動缸縮回;
                    } break;
                    case xe_tmr_takepin.xett_吸針嘴電動缸縮回: {
                        dbapiJoDell吸針嘴(10);
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
                        double SetPinOffsetX, SetPinOffsetY; {
                            SetPinOffsetX = apiParaReadIndex("SaveParameterJason.json", 15);
                            SetPinOffsetY = apiParaReadIndex("SaveParameterJason.json", 16);
                        }

                        btn_取得目標座標_Click(sender, e);
                        if(bRemove == false) {
                            //沒針抽
                            //要回home跟保護位
                            xeTmrTakePin = xe_tmr_takepin.xett_回Home保護;
                        } else {
                            //有針抽
                            double dbTargetX = dbPinHolePositionX + SetPinOffsetX;
                            double dbTargetY = dbPinHolePositionY + SetPinOffsetY;

                            dbapiCarrierX(dbTargetX, 190 * 0.8);
                            dbapiCarrierY(dbTargetY, 800 * 0.8);

                            double dbSocketCamera; {
                                dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                                dbapiIAI(dbSocketCamera);
                            }

                            xeTmrTakePin = xe_tmr_takepin.xett_檢查載盤XY是否移置抽料位;
                        }
                    } break;
                    case xe_tmr_takepin.xett_檢查載盤XY是否移置抽料位: {
                        double SetPinOffsetX, SetPinOffsetY; {
                            SetPinOffsetX = apiParaReadIndex("SaveParameterJason.json", 15);
                            SetPinOffsetY = apiParaReadIndex("SaveParameterJason.json", 16);
                        }

                        double dbX = dbapiCarrierX(dbRead, 0);
                        double dbY = dbapiCarrierY(dbRead, 0);

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
                        double RemovePinZHight; {
                            RemovePinZHight = apiParaReadIndex("SaveParameterJason.json", 12);
                            dbapiJoDell吸針嘴(RemovePinZHight);
                        }

                        xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸是否至抽料位;
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸是否至抽料位: {
                        double dbZ = dbapiJoDell吸針嘴(dbRead);

                        double RemovePinZHight; {
                            RemovePinZHight = apiParaReadIndex("SaveParameterJason.json", 12);
                        }

                        if( (RemovePinZHight * 0.99 <= dbZ && dbZ <= RemovePinZHight * 1.01) ) { 
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
                        dbapiJoDell吸針嘴(10);
                        xeTmrTakePin = xe_tmr_takepin.xett_抽料Z軸是否回0; 
                    } break;
                    case xe_tmr_takepin.xett_抽料Z軸是否回0: {
                        double dbZ = dbapiJoDell吸針嘴(dbRead);
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

                        if(bTakePin == true) { 
                            if(求出取料循環次數>=1) {  
                                xeTmrTakePin = xe_tmr_takepin.xett_還需要取針;
                            } else {
                                bTakePin = false;
                                btmrStop = false;
                                xeTmrTakePin = xe_tmr_takepin.xett_不需要取針;
                            }
                        } else { 
                            if(求出取料循環次數>=1 && btmrStop==false) {  
                                if(iPC == 0 && iRC == 0) {
                                    btmrStop = false;
                                    xeTmrTakePin = xe_tmr_takepin.xett_回Home保護;
                                } else {
                                    xeTmrTakePin = xe_tmr_takepin.xett_還需要取針;
                                }
                            } else {
                                btmrStop = false;
                                xeTmrTakePin = xe_tmr_takepin.xett_不需要取針;
                            }
                        }  // end of if(bTakePin == true) { 

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
                            case xe_tmr_takepin.xett_檢查NozzleXYR是否移至安全位置: 
                                if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    xeTmrTakePin = xe_tmr_takepin.xett_確認NozzleXYR在安全位置;
                                }
                                break;
                            case xe_tmr_takepin.xett_確認NozzleXYR在安全位置:              xeTmrTakePin = xe_tmr_takepin.xett_取針結束;  break;

                    case xe_tmr_takepin.xett_取針結束:
                        bTakePin   = false; 
                        bChambered = false;
                        bRemove    = false;
                        xeTmrTakePin = xe_tmr_takepin.xett_Empty;  
                        break;

                    case xe_tmr_takepin.xett_回Home保護:
                        bTakePin   = false; 
                        bChambered = false;
                        bRemove    = false;

                        bhome      = true;

                        xeTmrTakePin = xe_tmr_takepin.xett_Empty; 
                        break;

                case xe_tmr_takepin.xett_End:           
                    xeTmrTakePin = xe_tmr_takepin.xett_Empty;  
                    break;
            }
        }  // end of public void tmr_TakePin_Tick(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public enum xe_tmr_2pCalibration {
            xet2C_empty,
            xet2C_idle,
            xet2C_2pCalibration_start,

                xet2C_StopAllTask,
                    xet2C_StopAllTask_ok,

                xet2C_ClearAllTaskStatus,
                    xet2C_ClearAllTaskStatus_ok,

                xet2C_SystemHome,
                    xet2C_SystemHome_ok,

                xet2C_Load_Calibration_Json,
                    xet2C_Load_Calibration_Json_ok,
                    xet2C_Load_Calibration_Json_ng,

                xet2C_Socket_Camera_Home,
                xet2C_Socket_Camera_Home_Wait,
                xet2C_Socket_Camera_Home_Done,
                xet2C_Socket_Camera_To_CapturePosition,
                    xet2C_Socket_Camera_To_CapturePosition_ok,

                xet2C_關工作門,
                xet2C_檢查工作門關閉,
                xet2C_確定工作門關閉,
                xet2C_載盤真空閥啟用,
                xet2C_Socket1真空閥啟用,
                xet2C_Socket2真空閥啟用,

                xet2C_開始進行校正參數調整, 
                    xet2C_移動至_校正第1點,
                        xet2C_等待移動至_校正第1點,
                        xet2C_確定移動至_校正第1點,
                    xet2C_取得補正_校正第1點,
                        xet2C_移動補正_校正第1點,
                            xet2C_等待移動補正_校正第1點,
                            xet2C_確定移動補正_校正第1點,

                    xet2C_移動至_校正第2點,
                        xet2C_等待移動至_校正第2點,
                        xet2C_確定移動至_校正第2點,
                    xet2C_取得補正_校正第2點,
                        xet2C_移動補正_校正第2點,
                            xet2C_等待移動補正_校正第2點,
                            xet2C_確定移動補正_校正第2點,

                    xet2C_儲存校正參數,               
                xet2C_完成進行校正參數調整,   
                
            xet2C_2pCalibration_end,
            xet2C_end,
        };
        public xe_tmr_2pCalibration xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_empty;

        public bool bStartCalibration = false;
        public bool bForceToLoadCalibrationJson = false;
        public uint u32Delaycnt = 0;

        //---------------------------------------------------------------------------------------
        private void btn_兩點校正_Click(object sender, EventArgs e)
        {
            bStartCalibration = true;
        }
        //---------------------------------------------------------------------------------------
        private void tmr_2p_Calibration_Tick(object sender, EventArgs e)
        {
            lbl_2pCalibraLog.Text = xetmr2pCalibration.ToString();

            if(cB_AlwaysResume.Checked == true) {
                bResume = true;
            }

            switch (xetmr2pCalibration) {  // start of switch(xetmr2pCalibration) {
                case xe_tmr_2pCalibration.xet2C_empty:
                    if(bStartCalibration == true) {
                        bStartCalibration = false;

                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_2pCalibration_start;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_2pCalibration_start:                                  
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_StopAllTask;
                    break;

                case xe_tmr_2pCalibration.xet2C_StopAllTask:
                    //Home Sequense
                    tmr_Home.Enabled = false;
                        xeTmrHome        = xe_tmr_home.xets_empty;
                        ihomeFinishedCNT = 0;
                        bhome            = false;
                        bGotHome         = false;
                    tmr_Home.Enabled = true;

                    //TakePin
                    tmr_TakePin.Enabled  = false;
                        xeTmrTakePin         = xe_tmr_takepin.xett_Empty;
                        iTakePinFinishedCNT1 = 0;
                        iTakePinFinishedCNT2 = 0;
                        bTakePin             = false;
                        bChambered           = false;
                        bRemove              = false;
                        bPause               = false;
                        btmrStop             = false;
                    tmr_TakePin.Enabled  = true;

                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_StopAllTask_ok;
                    break;
                case xe_tmr_2pCalibration.xet2C_StopAllTask_ok:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_ClearAllTaskStatus;
                    break;

                case xe_tmr_2pCalibration.xet2C_ClearAllTaskStatus:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_ClearAllTaskStatus_ok;
                    break;
                case xe_tmr_2pCalibration.xet2C_ClearAllTaskStatus_ok:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_SystemHome;
                    break;

                case xe_tmr_2pCalibration.xet2C_SystemHome:
                    bhome = true;
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_SystemHome_ok;
                    break;
                case xe_tmr_2pCalibration.xet2C_SystemHome_ok:
                    if(bGotHome == true) { 
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Load_Calibration_Json;
                    }
                    break;

                case xe_tmr_2pCalibration.xet2C_Load_Calibration_Json: {
                    if(bForceToLoadCalibrationJson == false) {
                        bForceToLoadCalibrationJson = true;

                        if (OpenFile())  {
                            tsmi_SaveFile.Enabled = true;
                            btn_SaveFile.Enabled  = true;

                            show_grp_BarcodeInfo(grp_BarcodeInfo);

                            find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                            pic_Needles.Refresh();
                        }

                        int igetCount = get_NeedleCount();
                        if(igetCount == 2) { 
                            bForceToLoadCalibrationJson = false;
                            xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Load_Calibration_Json_ok;
                        } else { 
                            bForceToLoadCalibrationJson = false;
                            xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Load_Calibration_Json_ng;
                        }
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_Load_Calibration_Json_ok:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket_Camera_Home;
                    break;
                case xe_tmr_2pCalibration.xet2C_Load_Calibration_Json_ng:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_empty;
                    break;

                case xe_tmr_2pCalibration.xet2C_Socket_Camera_Home:
                    dbapiIAI(0);
                    u32Delaycnt = 0;
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket_Camera_Home_Wait;
                    break;
                case xe_tmr_2pCalibration.xet2C_Socket_Camera_Home_Wait:
                    u32Delaycnt++;
                    if(u32Delaycnt>=20) {
                        u32Delaycnt = 0;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket_Camera_Home_Done;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_Socket_Camera_Home_Done:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket_Camera_To_CapturePosition;
                    break;
                case xe_tmr_2pCalibration.xet2C_Socket_Camera_To_CapturePosition: {   
                    double dbSocketCamera; {
                        dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                        dbapiIAI(dbSocketCamera);
                    }

                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket_Camera_To_CapturePosition_ok;
                } break;
                case xe_tmr_2pCalibration.xet2C_Socket_Camera_To_CapturePosition_ok: {
                    double dbSocketCamera; {
                        dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                    }

                    double dbIAIHeight = dbapiIAI(dbRead);

                    if(dbSocketCamera * 0.99<= dbIAIHeight && dbIAIHeight <= dbSocketCamera * 1.01) { 
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_關工作門;
                    }
                } break;

                case xe_tmr_2pCalibration.xet2C_關工作門:     
                    dbapiGate(580, 580/4);
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_檢查工作門關閉;
                    break;
                case xe_tmr_2pCalibration.xet2C_檢查工作門關閉:
                    if(dbapiGate(dbCheckArrived, 0) == dbAxisMoveOk) {
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_確定工作門關閉;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_確定工作門關閉:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_載盤真空閥啟用;
                    break;
                case xe_tmr_2pCalibration.xet2C_載盤真空閥啟用:
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_載盤真空閥) / 10, (int)(WMX3IO對照.pxeIO_載盤真空閥) % 10, 1);
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket1真空閥啟用;
                    break;
                case xe_tmr_2pCalibration.xet2C_Socket1真空閥啟用:
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空1) / 10, (int)(WMX3IO對照.pxeIO_Socket真空1) % 10, 1);
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_Socket2真空閥啟用;
                    break;
                case xe_tmr_2pCalibration.xet2C_Socket2真空閥啟用:
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + (int)(WMX3IO對照.pxeIO_Socket真空2) / 10, (int)(WMX3IO對照.pxeIO_Socket真空2) % 10, 1);
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_開始進行校正參數調整;
                    break;

                case xe_tmr_2pCalibration.xet2C_開始進行校正參數調整:
                    if (bResume==true) {
                        bResume = false;
                        btn_參數_Click(sender, e);

                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_移動至_校正第1點;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_移動至_校正第1點: {   
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlAx;
                    double dbTargetY = rlAy;
                    dbapiCarrierX(dbTargetX, 190*0.1);
                    dbapiCarrierY(dbTargetY, 800*0.1);

                    if(bResume==true) {
                        bResume = false;

                        u32Delaycnt = 0;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_等待移動至_校正第1點;
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_等待移動至_校正第1點: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlAx;
                    double dbTargetY = rlAy;

                    double dbX = dbapiCarrierX(dbRead, 0);
                    double dbY = dbapiCarrierY(dbRead, 0);
                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                        u32Delaycnt++;
                        if(u32Delaycnt>=10) { 
                            u32Delaycnt = 0;

                            if(bResume==true) { 
                                bResume = false;
                                xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_確定移動至_校正第1點;
                            }
                        }
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_確定移動至_校正第1點:
                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_取得補正_校正第1點;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_取得補正_校正第1點: {
                     btn_socket相機兩點定位_Click(sender, e);

                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_移動補正_校正第1點;
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_移動補正_校正第1點: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlAx - dbCameraCalibrationX;
                    double dbTargetY = rlAy + dbCameraCalibrationY;

                    dbapiCarrierX(dbTargetX, 190*0.8);
                    dbapiCarrierY(dbTargetY, 800*0.8);

                    fmParameterFormHandle.dataGridView1.Rows[0].Cells[1].Value = dbTargetX;
                    fmParameterFormHandle.dataGridView1.Rows[1].Cells[1].Value = dbTargetY;

                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_等待移動補正_校正第1點;
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_等待移動補正_校正第1點: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlAx + dbCameraCalibrationX;
                    double dbTargetY = rlAy + dbCameraCalibrationY;

                    double dbX = dbapiCarrierX(dbRead, 0);
                    double dbY = dbapiCarrierY(dbRead, 0);
                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                        u32Delaycnt++;
                        if(u32Delaycnt>=10) { 
                            u32Delaycnt = 0;

                            if(bResume==true) { 
                                bResume = false;
                                xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_確定移動補正_校正第1點;
                            }
                        }
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_確定移動補正_校正第1點:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_移動至_校正第2點;
                    break;

                case xe_tmr_2pCalibration.xet2C_移動至_校正第2點: {  
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlBx;
                    double dbTargetY = rlBy;
                    dbapiCarrierX(dbTargetX, 190*0.1);
                    dbapiCarrierY(dbTargetY, 800*0.1);

                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_等待移動至_校正第2點;
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_等待移動至_校正第2點: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlBx;
                    double dbTargetY = rlBy;

                    double dbX = dbapiCarrierX(dbRead, 0);
                    double dbY = dbapiCarrierY(dbRead, 0);
                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                        u32Delaycnt++;
                        if(u32Delaycnt>=10) { 
                            u32Delaycnt = 0;

                            if(bResume==true) { 
                                bResume = false;
                                xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_確定移動至_校正第2點;
                            }
                        }
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_確定移動至_校正第2點:
                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_取得補正_校正第2點;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_取得補正_校正第2點:
                    btn_socket相機兩點定位_Click(sender, e);

                    if (bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_移動補正_校正第2點;
                    }
                    break;
                case xe_tmr_2pCalibration.xet2C_移動補正_校正第2點: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlBx - dbCameraCalibrationX;
                    double dbTargetY = rlBy + dbCameraCalibrationY;

                    dbapiCarrierX(dbTargetX, 190*0.8);
                    dbapiCarrierY(dbTargetY, 800*0.8);

                    fmParameterFormHandle.dataGridView1.Rows[2].Cells[1].Value = dbTargetX;
                    fmParameterFormHandle.dataGridView1.Rows[3].Cells[1].Value = dbTargetY;

                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_等待移動補正_校正第2點;
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_等待移動補正_校正第2點: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    double dbTargetX = rlBx + dbCameraCalibrationX;
                    double dbTargetY = rlBy + dbCameraCalibrationY;

                    double dbX = dbapiCarrierX(dbRead, 0);
                    double dbY = dbapiCarrierY(dbRead, 0);
                    if( (dbTargetX*0.99 <= dbX && dbX <= dbTargetX*1.01) &&
                        (dbTargetY*0.99 <= dbY && dbY <= dbTargetY*1.01) ) { 

                        u32Delaycnt++;
                        if(u32Delaycnt>=10) { 
                            u32Delaycnt = 0;

                            if(bResume==true) { 
                                bResume = false;
                                xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_確定移動補正_校正第2點;
                            }
                        }
                    }
                } break;
                case xe_tmr_2pCalibration.xet2C_確定移動補正_校正第2點:
                    if(bResume==true) { 
                        bResume = false;
                        xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_儲存校正參數;
                    }
                    break;

                case xe_tmr_2pCalibration.xet2C_儲存校正參數:
                    fmParameterFormHandle.btn_Save_Click(sender, e);
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_完成進行校正參數調整;
                    break;
                case xe_tmr_2pCalibration.xet2C_完成進行校正參數調整: {
                    //Get Real Pxy
                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                    //Get Ideal Pxy
                    double idlpAx = 0, idlpAy = 0, idlpBx = 0, idlpBy = 0;

                    string Cal2pFileName = apiParaReadStr("SaveParameterJason.json", 8);
                    int PointLeft  = (int)apiParaReadIndex("SaveParameterJason.json", 9);
                    int PointRight = (int)apiParaReadIndex("SaveParameterJason.json", 10);

                    apiReadNeedleInfo(Cal2pFileName, PointLeft,  ref idlpAx, ref idlpAy);
                    apiReadNeedleInfo(Cal2pFileName, PointRight, ref idlpBx, ref idlpBy);

                    //Calculate Cal 2p
                    {
                        Normal calculate = new Normal();

                        // 定義 PointA, PointB 的數據
                        Normal.Point idealA = new Normal.Point(idlpAx, idlpAy);
                        Normal.Point idealB = new Normal.Point(idlpBx, idlpBy);
                        Normal.Point realA = new Normal.Point(rlAx, rlAy);
                        Normal.Point realB = new Normal.Point(rlBx, rlBy);

                        // 宣告 PointForward 和 PointBackward 變數
                        Normal.Point idealAForward = new Normal.Point();
                        Normal.Point idealABackward = new Normal.Point();
                        Normal.Point realAForward = new Normal.Point();
                        Normal.Point realABackward = new Normal.Point();

                        // 呼叫計算並傳遞相應的點作為參數
                        CalculateAndPrintPlotData(idealA, idealB, out idealAForward, out idealABackward);
                        CalculateAndPrintPlotData(realA,  realB,  out realAForward,  out realABackward);

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
                } xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_2pCalibration_end;
                break;

                default:
                case xe_tmr_2pCalibration.xet2C_2pCalibration_end: 
                case xe_tmr_2pCalibration.xet2C_idle: 
                case xe_tmr_2pCalibration.xet2C_end:
                    xetmr2pCalibration = xe_tmr_2pCalibration.xet2C_empty;
                    break;
            }  // end of switch(xetmr2pCalibration) {
        }
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
        private void tsmi_CloseFile_Click(object sender, EventArgs e)
        {
            Viewer.CloseFile();

            clear_grp_NeedleInfo(grp_NeedleInfo);
            pic_Needles.Refresh();
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

                if (circle.Display == false) // 隱藏
                {
                    fillBrush = new SolidBrush(HiddenNeedlesColor);
                }
                else if (circle.Disable == true) // 禁用
                {
                    fillBrush = new SolidBrush(EnableNeedlesColor);
                }
                else if (circle.Reserve1 == true) // 保留1
                {
                    fillBrush = new SolidBrush(Reserve1NeedlesColor);
                }
                else if (circle.Place == true) // 植針圓
                {
                    fillBrush= new SolidBrush(PlaceNeedlesColor);
                }
                else if (circle.Remove == true) // 取針圓
                {
                    fillBrush = new SolidBrush(RemoveNeedlesColor);
                }
                else if (circle.Replace == true) // 換針圓
                {
                    fillBrush = new SolidBrush(ReplaceNeedlesColor);
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
                    case Keys.Control:

                        PrevMousePos = e.Location;

                        if (HighlightedNeedle != null)
                        {
                            FocusedNeedle = HighlightedNeedle;

                            SelectedNeedles.Add(HighlightedNeedle);

                            clear_grp_NeedleInfo(grp_NeedleInfo);
                        }
                        else
                        {
                            FocusedNeedle = null;

                            clear_grp_NeedleInfo(grp_NeedleInfo);
                        }

                        break;

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

                            SelectedNeedles.Add(HighlightedNeedle);

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
                        Json.Needles[circle.Index].Place   = true;
                        Json.Needles[circle.Index].Remove  = false;
                        Json.Needles[circle.Index].Replace = false;
                        break;

                    case "取針":
                        Json.Needles[circle.Index].Place   = false;
                        Json.Needles[circle.Index].Remove  = true;
                        Json.Needles[circle.Index].Replace = false;
                        break;

                    case "置換":
                        Json.Needles[circle.Index].Place   = false;
                        Json.Needles[circle.Index].Remove  = false;
                        Json.Needles[circle.Index].Replace = true;
                        break;

                    case "顯示":
                        Json.Needles[circle.Index].Display = true;
                        break;

                    case "禁用":
                        Json.Needles[circle.Index].Disable = true;
                        break;

                    case "保留":
                        Json.Needles[circle.Index].Reserve1 = true;
                        break;

                    case "清除":
                        Json.Needles[circle.Index].Place    = false;
                        Json.Needles[circle.Index].Remove   = false;
                        Json.Needles[circle.Index].Replace  = false;
                        Json.Needles[circle.Index].Display  = true;
                        Json.Needles[circle.Index].Disable   = false;
                        Json.Needles[circle.Index].Reserve1 = false;

                        show_grp_NeedleInfo(grp_NeedleInfo);

                        break;
                }
            }
        }
        //---------------------------------------------------------------------------------------
        public void grp_NeedleInfo_ChildControlChanged(object sender, EventArgs e)
        {
            if (SelectedNeedles.Count() != 0)
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
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Place = rad_Place.Checked;
                                }
                            break;

                            case "rad_Remove":
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Remove = rad_Remove.Checked;
                                }
                            break;

                            case "rad_Replace":
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Replace = rad_Replace.Checked;
                                }
                                break;
                        }

                        break;

                    case CheckBox checkBox:

                        switch (checkBox.Name)
                        {
                            case "chk_Display":
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Display = chk_Display.Checked;
                                }
                                break;

                            case "chk_Disable":
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Disable = chk_Disable.Checked;

                                    chk_Reserve1.Checked = false;
                                    Json.Needles[SelectedNeedle.Index].Reserve1 = false;
                                }
                                
                                break;

                            case "chk_Reserve1":
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Reserve1 = chk_Reserve1.Checked;

                                    chk_Disable.Checked = false;
                                    Json.Needles[SelectedNeedle.Index].Disable = false;
                                }
                               
                                break;
                        }
                        break;

                    case Button button:
                        switch (button.Name)
                        {
                            case "btn_Reset":
                                foreach (var SelectedNeedle in SelectedNeedles)
                                {
                                    Json.Needles[SelectedNeedle.Index].Place = false;
                                    Json.Needles[SelectedNeedle.Index].Remove = false;
                                    Json.Needles[SelectedNeedle.Index].Replace = false;
                                    Json.Needles[SelectedNeedle.Index].Display = true;
                                    Json.Needles[SelectedNeedle.Index].Disable = false;
                                    Json.Needles[SelectedNeedle.Index].Reserve1 = false;
                                }

                                show_grp_NeedleInfo(grp_NeedleInfo);

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
        public void chk_Disable_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_Disable.Checked)
            {
                chk_Disable.BackColor = Color.Red;
            }
            else
            {
                chk_Disable.BackColor = SystemColors.Control;
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

            dbCameraCalibrationX = pos.X;
            dbCameraCalibrationY = pos.Y;
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
