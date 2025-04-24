
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
//3D 掃描器
using ReceiveSurface;

//---------------------------------------------------------------------------------------
//Log
using System.Collections.Concurrent;

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
        enum eDownVisionRsult {
            eDVR_Null,
            eDVR_Get_1Pin_ok_Normal,
            eDVR_Get_1Pin_ok_Inverse,
            eDVR_Get_0Pin_ng,
            eDVR_Get_1Pin_ng,
            eDVR_Get_2Pin_ng,
            eDVR_NG,
        };
        eDownVisionRsult eDVR_Rsult = eDownVisionRsult.eDVR_Null;
        public void apiCallBackTest()
        {
            //確定是否要執行飛拍中斷事件
            bool bEnableTriggerISR = false;
            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                    if(eNeedleType == xeXavier_NeedleType.pT6Place) { 
                        bEnableTriggerISR = true;
                    }
                    break;

                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;

                case xeXavier_Indicator.xeXI_狀態_停止:
                case xeXavier_Indicator.xeXI_狀態_急停:
                case xeXavier_Indicator.xeXI_事件_暫停:
                case xeXavier_Indicator.xeXI_事件_異常: 
                default:
                    break;
            }

            //Vision Callback Function test
            cntcallback++;
            this.Text = cntcallback.ToString() + "  " + inspector1.InspNozzle.CCD.GrabCount.ToString();

            eDVR_Rsult = eDownVisionRsult.eDVR_Null;
                if (inspector1.InspectOK == true && inspector1.Inspected == true) {
                    UIHelper.SetControlProperty(label10, () => label10.Text = inspector1.PinDeg.ToString());

                    xeXavier_T2_Job getJob = Xavier_T2_delayCase(xeXavier_T2_proc.pt2GET, 0, xeXavier_T2_Job.tp2Empty);
                    if(getJob >= xeXavier_T2_Job.tp2Insert_吸嘴軸組XY移動至飛拍準備位) { 
                        this.Text = getJob.ToString() + "飛拍起始狀態正確";

                        const int torence_deg = 5;
                        if(90.0-torence_deg <= Math.Abs(inspector1.PinDeg) &&
                                               Math.Abs(inspector1.PinDeg) <= 90.0+torence_deg) { 
                            if(inspector1.PinDeg < 0) {
                                eDVR_Rsult = eDownVisionRsult.eDVR_Get_1Pin_ok_Inverse;
                            } else {
                                eDVR_Rsult = eDownVisionRsult.eDVR_Get_1Pin_ok_Normal;
                            }

                            //飛拍成功
                            if(bEnableTriggerISR == true) { 
                                Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR01);
                            }
                        } else { 
                            eDVR_Rsult = eDownVisionRsult.eDVR_Get_1Pin_ng;

                            //飛拍失敗
                            if(bEnableTriggerISR == true) { 
                                Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);
                            }
                        }
                    } else { 
                        this.Text = getJob.ToString() + "飛拍起始狀態錯誤";
                    }  // end of if(getJob >= xeXavier_T2_Job.tp2Insert_吸嘴軸組XY移動至飛拍準備位) { 

                } else {  //if(inspector1.InspectOK == false) {
                    if(inspector1.下視覺正向 == true) { 
                        switch(inspector1.PinCount) {
                            case 0:   eDVR_Rsult = eDownVisionRsult.eDVR_Get_0Pin_ng;  break;
                            case 1:   eDVR_Rsult = eDownVisionRsult.eDVR_Get_1Pin_ng;  break;
                            case 2:   eDVR_Rsult = eDownVisionRsult.eDVR_Get_2Pin_ng;  break;
                            default:  eDVR_Rsult = eDownVisionRsult.eDVR_NG;           break;
                        }

                        if(cB_料盤有料.Checked == true) {
                            //永遠跳過

                            eDVR_Rsult = eDownVisionRsult.eDVR_Get_1Pin_ok_Normal;

                            //飛拍成功
                            if(bEnableTriggerISR == true) { 
                                Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR01);
                            }
                        } else { 
                            //飛拍失敗
                            if(bEnableTriggerISR == true) { 
                                Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);
                            }
                        }
                    }
                }  // end of if (inspector1.InspectOK == true && inspector1.Inspected == true) {
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
            UIHelper.SetControlProperty(label6, () => label6.Text = string.Format("Socket 偵測 {0} 中心偏移 = {1:F3} , {2:F3}", success, pos.X, pos.Y));

            //取得校正攝影機校正參數
            success      = inspector1.xInspSocket植針後檢查();
            UIHelper.SetControlProperty(label7, () => label7.Text = (success) ? "植針後檢查 OK" : "植針後檢查 NG");
        }
        //---------------------------------------------------------------------------------------
        public void btn_植針嘴檢查_Click(object sender, EventArgs e)
        {
            //植針嘴有無堵料, 無:ok, 有:ng
            Inspector.Vector3 pos2;
            bool success2 = inspector1.xInsp夾爪(out pos2);   //夾爪針孔偵測 回傳:OK/NG 及找到孔的位置
            UIHelper.SetControlProperty(label21, () => label21.Text = success2.ToString());
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
                    UIHelper.SetControlProperty(lbl_計算角度,     () => lbl_計算角度.Text = "夾角為 90 度(垂直向上)");
                } else if (rise < 0) {
                    UIHelper.SetControlProperty(lbl_計算角度,     () => lbl_計算角度.Text = "夾角為 270 度(垂直向下)");
                } else {
                    UIHelper.SetControlProperty(lbl_計算角度,     () => lbl_計算角度.Text = "兩點相同，無法計算夾角");
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
                UIHelper.SetControlProperty(lbl_計算角度,     () => lbl_計算角度.Text = string.Format("Cx:{1}, Cy:{2}, 夾角: {0:F2} 度", angle_degrees, Cx, Cy));
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
                UIHelper.SetControlProperty(label2, () => label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y));
            }

            if(SetToPoint == btn_ToPointA) {
                UIHelper.SetControlProperty(tB_Ax, () => tB_Ax.Text = pos.X.ToString());
                UIHelper.SetControlProperty(tB_Ay, () => tB_Ay.Text = pos.Y.ToString());
            } else if(SetToPoint == btn_ToPointB) {
                UIHelper.SetControlProperty(tB_Bx, () => tB_Bx.Text = pos.X.ToString());
                UIHelper.SetControlProperty(tB_By, () => tB_By.Text = pos.Y.ToString());
            } else if(SetToPoint == btn_SwitchPointAB) {
                double dbX = 0.0, dbY = 0.0;

                dbX = double.Parse(tB_Ax.Text);
                dbY = double.Parse(tB_Ay.Text);

                UIHelper.SetControlProperty(tB_Ax, () => tB_Ax.Text = tB_Bx.Text);
                UIHelper.SetControlProperty(tB_Ay, () => tB_Ay.Text = tB_By.Text);

                UIHelper.SetControlProperty(tB_Bx, () => tB_Bx.Text = dbX.ToString());
                UIHelper.SetControlProperty(tB_By, () => tB_By.Text = dbY.ToString());
            }
        }
        //---------------------------------------------------------------------------------------
        //bool bResume = false;
        public void btn_Resume_Click(object sender, EventArgs e)
        {
            //bResume = true;
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
                UIHelper.SetControlProperty(label2, () => label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}, deg= {3:F2}", success, pos.X, pos.Y, deg1));
            }
            else
            {
                success = inspector1.xCarb震動盤(out pos);
                //pos.X = (float)inspector1.nozzleX - pos.X;
                //pos.Y = (float)inspector1.nozzleY - pos.Y;
                UIHelper.SetControlProperty(label2, () => label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y));
            }
            //bool success = inspector1.xCarb震動盤(out pos);
            //label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}", success, pos.X, pos.Y);
            //bool success = inspector1.xCarb震動盤二孔(out pos, out deg1);
            //pos.X = (float)inspector1.nozzleX - pos.X;
            //pos.Y = (float)inspector1.nozzleY - pos.Y;
            //label2.Text = string.Format("吸料盤校正用 分析結果 = {0} X = {1:F2} Y = {2:F2}, deg= {3:F2}", success, pos.X, pos.Y, deg1);

            //黑色料倉
            bool 料倉有料 = inspector1.xInsp入料();
            UIHelper.SetControlProperty(label3, () => label3.Text = string.Format("黑色料倉 料倉有料 = {0}", 料倉有料));
            b黑色料倉有料_tmrTakePinTick = 料倉有料;

            //光源震動盤
            List<Vector3> pins;
            bool 料盤有料 = inspector1.xInsp震動盤(out pins);
            Vector3 temp = (料盤有料) ? pins.First() : new Vector3();
            UIHelper.SetControlProperty(label4, () => label4.Text = string.Format("光源震動盤 震動盤 = {0} X = {1:F2} Y = {2:F2} θ = {3:F2}", 料盤有料, temp.X, temp.Y, temp.θ));
            b柔震盤有料_tmrTakePinTick = 料盤有料;
            dbPinX_tmrTakePinTick = temp.X;
            dbPinY_tmrTakePinTick = temp.Y;
            dbPinR_tmrTakePinTick = temp.θ;

            if (inspector1.Inspected && inspector1.InspectOK) {
                double deg = inspector1.PinDeg;
                UIHelper.SetControlProperty(label5, () => label5.Text = string.Format("吸嘴物料分析  θ = {0:F2}", deg));
            } else { 
                UIHelper.SetControlProperty(label5, () => label5.Text = "吸嘴物料分析失敗");
            }


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

        //---------------------------------------------------------------------------------------
        //State Enum       
        public const bool HIGH = true;
        public const bool LOW  = false;
        public const bool ON   = true;
        public const bool OFF  = false;

        //Servo EtherCAT
        public double dbInsertSpeedNozzleX  = 3000;  //(500.0) * 0.1;
        public double dbInsertSpeedNozzleY  = 3000;  //(100.0) * 0.1;
        public double dbInsertSpeedNozzleZ  = 6000;  //( 40.0) * 0.1;
        public double dbInsertSpeedNozzleR  = 3000;  //(360.0) * 0.1;
        public double dbInsertSpeedCarrierX = 2000;  //(190.0) * 0.1;
        public double dbInsertSpeedCarrierY = 2000;  //(800.0) * 0.1;
        public double dbInsertSpeedSetZ     = 3000;  //( 33.0) * 0.1;
        public double dbInsertSpeedSetR     = 3000;  //(360.0) * 0.1;
        public double dbInsertSpeedGate     = (580.0) * 0.1;

        //---------------------------------------------------------------------------------------

        // 設定YASKAWA GPIO OUT
        public void digitalWrite(int pin, bool state) {
            switch(pin) {
                case (int)WMX3IO對照.pxeIO_擺放座蓋板: 
                case (int)WMX3IO對照.pxeIO_吸料真空電磁閥: 
                case (int)WMX3IO對照.pxeIO_堵料吹氣缸:    
                case (int)WMX3IO對照.pxeIO_接料區氣桿:    
                case (int)WMX3IO對照.pxeIO_植針吹氣:      
                case (int)WMX3IO對照.pxeIO_收料區缸:      
                case (int)WMX3IO對照.pxeIO_堵料吹氣:      
                case (int)WMX3IO對照.pxeIO_NA_O_07:       

                case (int)WMX3IO對照.pxeIO_載盤真空閥:    
                case (int)WMX3IO對照.pxeIO_Socket真空2:   
                case (int)WMX3IO對照.pxeIO_載盤破真空:    
                case (int)WMX3IO對照.pxeIO_Socket破真空2: 
                case (int)WMX3IO對照.pxeIO_Socket真空1:   
                case (int)WMX3IO對照.pxeIO_擺放座吸真空:   
                case (int)WMX3IO對照.pxeIO_Socket破真空1:  
                case (int)WMX3IO對照.pxeIO_擺放座破真空:   

                case (int)WMX3IO對照.pxeIO_取料吸嘴吸:     
                case (int)WMX3IO對照.pxeIO_下後左門鎖:     
                case (int)WMX3IO對照.pxeIO_取料吸嘴破真空舊:
                case (int)WMX3IO對照.pxeIO_下後右門鎖:      
                case (int)WMX3IO對照.pxeIO_植針Z煞車:       
                case (int)WMX3IO對照.pxeIO_HEPA:            
                case (int)WMX3IO對照.pxeIO_取料吸嘴破真空新: 
                case (int)WMX3IO對照.pxeIO_LIGHT:            

                case (int)WMX3IO對照.pxeIO_面板右按鈕綠燈:   
                case (int)WMX3IO對照.pxeIO_機台紅燈:         
                case (int)WMX3IO對照.pxeIO_面板中按鈕綠燈:   
                case (int)WMX3IO對照.pxeIO_機台黃燈:         
                case (int)WMX3IO對照.pxeIO_面板左按鈕紅燈:   
                case (int)WMX3IO對照.pxeIO_機台綠燈:         
                case (int)WMX3IO對照.pxeIO_NA_O_36:          
                case (int)WMX3IO對照.pxeIO_Buzzer:        
                    clsServoControlWMX3.WMX3_SetIOBit((int)WMX3IO對照.pxeIO_Addr4 + pin/10, pin%10, (state == HIGH)?(byte)1:(byte)0);
                    break;

                default:
                    //Error
                    break;
            }
        }

        // 讀取YASKAWA GPIO OUT
        public bool digitalRead(int pin) {
            bool brsult = false;

            switch(pin) {
                case (int)WMX3IO對照.pxeIO_擺放座蓋板: 
                case (int)WMX3IO對照.pxeIO_吸料真空電磁閥: 
                case (int)WMX3IO對照.pxeIO_堵料吹氣缸:    
                case (int)WMX3IO對照.pxeIO_接料區氣桿:    
                case (int)WMX3IO對照.pxeIO_植針吹氣:      
                case (int)WMX3IO對照.pxeIO_收料區缸:      
                case (int)WMX3IO對照.pxeIO_堵料吹氣:      
                case (int)WMX3IO對照.pxeIO_NA_O_07:       

                case (int)WMX3IO對照.pxeIO_載盤真空閥:    
                case (int)WMX3IO對照.pxeIO_Socket真空2:   
                case (int)WMX3IO對照.pxeIO_載盤破真空:    
                case (int)WMX3IO對照.pxeIO_Socket破真空2: 
                case (int)WMX3IO對照.pxeIO_Socket真空1:   
                case (int)WMX3IO對照.pxeIO_擺放座吸真空:   
                case (int)WMX3IO對照.pxeIO_Socket破真空1:  
                case (int)WMX3IO對照.pxeIO_擺放座破真空:   

                case (int)WMX3IO對照.pxeIO_取料吸嘴吸:     
                case (int)WMX3IO對照.pxeIO_下後左門鎖:     
                case (int)WMX3IO對照.pxeIO_取料吸嘴破真空舊:
                case (int)WMX3IO對照.pxeIO_下後右門鎖:      
                case (int)WMX3IO對照.pxeIO_植針Z煞車:       
                case (int)WMX3IO對照.pxeIO_HEPA:            
                case (int)WMX3IO對照.pxeIO_取料吸嘴破真空新: 
                case (int)WMX3IO對照.pxeIO_LIGHT:            

                case (int)WMX3IO對照.pxeIO_面板右按鈕綠燈:   
                case (int)WMX3IO對照.pxeIO_機台紅燈:         
                case (int)WMX3IO對照.pxeIO_面板中按鈕綠燈:   
                case (int)WMX3IO對照.pxeIO_機台黃燈:         
                case (int)WMX3IO對照.pxeIO_面板左按鈕紅燈:   
                case (int)WMX3IO對照.pxeIO_機台綠燈:         
                case (int)WMX3IO對照.pxeIO_NA_O_36:          
                case (int)WMX3IO對照.pxeIO_Buzzer: {       
                    //讀取 Yaskawa OutputIO
                    byte[] pDataGetOutIO = new byte[4];
                    clsServoControlWMX3.WMX3_GetOutIO(ref pDataGetOutIO, (int)WMX3IO對照.pxeIO_Addr4, 4);
                    brsult = ((pDataGetOutIO[(pin / 10)] & (1 << pin % 10)) != 0) ? HIGH : LOW;
                } break;

                default:
                    //Error
                    break;
            }

            return brsult;
        }

        // 切換YASKAWA GPIO OUT
        public void digitalToggle(int pin) {
            switch(pin) {
                case (int)WMX3IO對照.pxeIO_擺放座蓋板: 
                case (int)WMX3IO對照.pxeIO_吸料真空電磁閥: 
                case (int)WMX3IO對照.pxeIO_堵料吹氣缸:    
                case (int)WMX3IO對照.pxeIO_接料區氣桿:    
                case (int)WMX3IO對照.pxeIO_植針吹氣:      
                case (int)WMX3IO對照.pxeIO_收料區缸:      
                case (int)WMX3IO對照.pxeIO_堵料吹氣:      
                case (int)WMX3IO對照.pxeIO_NA_O_07:       

                case (int)WMX3IO對照.pxeIO_載盤真空閥:    
                case (int)WMX3IO對照.pxeIO_Socket真空2:   
                case (int)WMX3IO對照.pxeIO_載盤破真空:    
                case (int)WMX3IO對照.pxeIO_Socket破真空2: 
                case (int)WMX3IO對照.pxeIO_Socket真空1:   
                case (int)WMX3IO對照.pxeIO_擺放座吸真空:   
                case (int)WMX3IO對照.pxeIO_Socket破真空1:  
                case (int)WMX3IO對照.pxeIO_擺放座破真空:   

                case (int)WMX3IO對照.pxeIO_取料吸嘴吸:     
                case (int)WMX3IO對照.pxeIO_下後左門鎖:     
                case (int)WMX3IO對照.pxeIO_取料吸嘴破真空舊:
                case (int)WMX3IO對照.pxeIO_下後右門鎖:      
                case (int)WMX3IO對照.pxeIO_植針Z煞車:       
                case (int)WMX3IO對照.pxeIO_HEPA:            
                case (int)WMX3IO對照.pxeIO_取料吸嘴破真空新: 
                case (int)WMX3IO對照.pxeIO_LIGHT:            

                case (int)WMX3IO對照.pxeIO_面板右按鈕綠燈:   
                case (int)WMX3IO對照.pxeIO_機台紅燈:         
                case (int)WMX3IO對照.pxeIO_面板中按鈕綠燈:   
                case (int)WMX3IO對照.pxeIO_機台黃燈:         
                case (int)WMX3IO對照.pxeIO_面板左按鈕紅燈:   
                case (int)WMX3IO對照.pxeIO_機台綠燈:         
                case (int)WMX3IO對照.pxeIO_NA_O_36:          
                case (int)WMX3IO對照.pxeIO_Buzzer: {       
                    digitalWrite(pin, !digitalRead(pin));
                } break;

                default:
                    //Error
                    break;
            }
        }

        // 讀取YASKAWA GPIO IN
        public bool indicateRead(int pin) {
            bool brsult = false;

            switch(pin) {
                case (int)WMX3IO對照.pxeIO_載盤Y軸後極限:
                case (int)WMX3IO對照.pxeIO_取料Y軸後極限:
                case (int)WMX3IO對照.pxeIO_載盤Y軸前極限:
                case (int)WMX3IO對照.pxeIO_取料Y軸前極限:
                case (int)WMX3IO對照.pxeIO_取料X軸後極限:
                case (int)WMX3IO對照.pxeIO_NA05:
                case (int)WMX3IO對照.pxeIO_取料X軸前極限:
                case (int)WMX3IO對照.pxeIO_NA07:

                case (int)WMX3IO對照.pxeIO_植針Z軸後極限:
                case (int)WMX3IO對照.pxeIO_NA11:
                case (int)WMX3IO對照.pxeIO_植針Z軸前極限:
                case (int)WMX3IO對照.pxeIO_NA13:
                case (int)WMX3IO對照.pxeIO_載盤X軸前極限:
                case (int)WMX3IO對照.pxeIO_NA15:
                case (int)WMX3IO對照.pxeIO_載盤X軸後極限:
                case (int)WMX3IO對照.pxeIO_NA17:

                case (int)WMX3IO對照.pxeIO_載盤真空檢1:
                case (int)WMX3IO對照.pxeIO_Socket2真空檢1:
                case (int)WMX3IO對照.pxeIO_載盤真空檢2:
                case (int)WMX3IO對照.pxeIO_Socket2真空檢2:
                case (int)WMX3IO對照.pxeIO_Socket1真空檢1:
                case (int)WMX3IO對照.pxeIO_擺放座真空檢1:
                case (int)WMX3IO對照.pxeIO_Socket1真空檢2:
                case (int)WMX3IO對照.pxeIO_擺放座真空檢2:

                case (int)WMX3IO對照.pxeIO_吸嘴真空檢1:
                case (int)WMX3IO對照.pxeIO_NA31:
                case (int)WMX3IO對照.pxeIO_吸嘴真空檢2:
                case (int)WMX3IO對照.pxeIO_取料NG收料盒:
                case (int)WMX3IO對照.pxeIO_兩點組合壓力檢1:
                case (int)WMX3IO對照.pxeIO_堵料收料盒:
                case (int)WMX3IO對照.pxeIO_兩點組合壓力檢2:
                case (int)WMX3IO對照.pxeIO_吸料收料盒:

                case (int)WMX3IO對照.pxeIO_復歸按鈕:
                case (int)WMX3IO對照.pxeIO_NA41:
                case (int)WMX3IO對照.pxeIO_啟動按鈕:
                case (int)WMX3IO對照.pxeIO_NA43:
                case (int)WMX3IO對照.pxeIO_停止按鈕:
                case (int)WMX3IO對照.pxeIO_NA45:
                case (int)WMX3IO對照.pxeIO_緊急停止按鈕:
                case (int)WMX3IO對照.pxeIO_NA47:

                case (int)WMX3IO對照.pxeIO_擺放座蓋板開:
                case (int)WMX3IO對照.pxeIO_NA51:
                case (int)WMX3IO對照.pxeIO_擺放座蓋板合:
                case (int)WMX3IO對照.pxeIO_NA53:
                case (int)WMX3IO對照.pxeIO_堵料吹氣桿進:
                case (int)WMX3IO對照.pxeIO_NA55:
                case (int)WMX3IO對照.pxeIO_堵料吹氣桿出:
                case (int)WMX3IO對照.pxeIO_NA57:

                case (int)WMX3IO對照.pxeIO_上罩左側右門:
                case (int)WMX3IO對照.pxeIO_上罩右側右門:
                case (int)WMX3IO對照.pxeIO_上罩左側左門:
                case (int)WMX3IO對照.pxeIO_上罩右側左門:
                case (int)WMX3IO對照.pxeIO_上罩後側右門:
                case (int)WMX3IO對照.pxeIO_螢幕旁小門:
                case (int)WMX3IO對照.pxeIO_上罩後側左門:
                case (int)WMX3IO對照.pxeIO_NA67:

                case (int)WMX3IO對照.pxeIO_下支架左側右門:
                case (int)WMX3IO對照.pxeIO_下支架後側左門:
                case (int)WMX3IO對照.pxeIO_下支架左側左門:
                case (int)WMX3IO對照.pxeIO_下支架後側右門:
                case (int)WMX3IO對照.pxeIO_下支架右側右門:
                case (int)WMX3IO對照.pxeIO_NA75:
                case (int)WMX3IO對照.pxeIO_下支架右側左門:
                case (int)WMX3IO對照.pxeIO_NA76: { 
                    //讀取 Yaskawa InputIO
                    byte[] pDataGetInIO = new byte[8];
                    clsServoControlWMX3.WMX3_GetInIO(ref pDataGetInIO, (int)WMX3IO對照.pxeIO_Addr28, 8);
                    brsult = ((pDataGetInIO[(pin / 10)] & (1 << pin % 10)) != 0) ? HIGH : LOW;
                } break;

                default:
                    //Error
                    break;
            }

            return brsult;
        }

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
        public const double dbNozzleX_Home位  = 242;
        public double dbapiNozzleX_defaultSpeed(double dbIncreaseNozzleX)  //NozzleX
        {
            double dbDefaultSpeed = (500.0) * 0.1;
            return dbapiNozzleX(dbIncreaseNozzleX, dbDefaultSpeed);
        }
        public double dbapiNozzleX_InsertSpeed(double dbIncreaseNozzleX)  //NozzleX
        {
            return dbapiNozzleX(dbIncreaseNozzleX, dbInsertSpeedNozzleX);
        }
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
                    UIHelper.SetControlProperty(lbl_吸嘴X軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴X軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴X軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_吸嘴X軸_RAW,     () => lbl_吸嘴X軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_吸嘴X軸_Convert,     () => lbl_吸嘴X軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_吸嘴X軸_Back,     () => lbl_吸嘴X軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstNozzleX             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴X軸,     () => lbl_acpos_吸嘴X軸.Text   = dbRstNozzleX.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_吸嘴X軸,     () => lbl_spd_吸嘴X軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_吸嘴X軸, () => select_吸嘴X軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴X軸,     () => lbl_acpos_吸嘴X軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴X軸,     () => lbl_spd_吸嘴X軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_吸嘴X軸, () => select_吸嘴X軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴X軸,     () => lbl_acpos_吸嘴X軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴X軸,     () => lbl_spd_吸嘴X軸.BackColor   = Color.Gray);
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

                    if(dbIncreaseNozzleX > dbTargetPositionNozzleX) { 
                        inspector1.下視覺正向 = true;
                    } else 
                    if(dbIncreaseNozzleX < dbTargetPositionNozzleX) { 
                         inspector1.下視覺正向 = false;
                    }
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
        public const double dbNozzleY_Home位  = 28;
        public double dbapiNozzleY_defaultSpeed(double dbIncreaseNozzleY)  //NozzleY
        {
            double dbDefaultSpeed = (100.0) * 0.1;
            return dbapiNozzleY(dbIncreaseNozzleY, dbDefaultSpeed);
        }
        public double dbapiNozzleY_InsertSpeed(double dbIncreaseNozzleY)  //NozzleY
        {
            return dbapiNozzleY(dbIncreaseNozzleY, dbInsertSpeedNozzleY);
        }
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
                    UIHelper.SetControlProperty(lbl_吸嘴Y軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴Y軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴Y軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_吸嘴Y軸_RAW,    () => lbl_吸嘴Y軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_吸嘴Y軸_Convert,    () => lbl_吸嘴Y軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_吸嘴Y軸_Back,    () => lbl_吸嘴Y軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstNozzleY             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴Y軸,    () => lbl_acpos_吸嘴Y軸.Text   = dbRstNozzleY.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_吸嘴Y軸,    () => lbl_spd_吸嘴Y軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_吸嘴Y軸, () => select_吸嘴Y軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴Y軸,    () => lbl_acpos_吸嘴Y軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴Y軸,    () => lbl_spd_吸嘴Y軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_吸嘴Y軸, () => select_吸嘴Y軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴Y軸,    () => lbl_acpos_吸嘴Y軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴Y軸,    () => lbl_spd_吸嘴Y軸.BackColor   = Color.Gray);
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
        public const double dbNozzleZ_Home位  = 0;
        public double dbapiNozzleZ_defaultSpeed(double dbIncreaseNozzleZ)  //NozzleZ
        {
            double dbDefaultSpeed = (40.0) * 0.1;
            return dbapiNozzleZ(dbIncreaseNozzleZ, dbDefaultSpeed);
        }
        public double dbapiNozzleZ_InsertSpeed(double dbIncreaseNozzleZ)  //NozzleZ
        {
            return dbapiNozzleZ(dbIncreaseNozzleZ, dbInsertSpeedNozzleZ);
        }
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
                    UIHelper.SetControlProperty(lbl_吸嘴Z軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴Z軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴Z軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_吸嘴Z軸_RAW,    () => lbl_吸嘴Z軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_吸嘴Z軸_Convert,    () => lbl_吸嘴Z軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_吸嘴Z軸_Back,    () => lbl_吸嘴Z軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstNozzleZ             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴Z軸,    () => lbl_acpos_吸嘴Z軸.Text   = dbRstNozzleZ.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_吸嘴Z軸,    () => lbl_spd_吸嘴Z軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_吸嘴Z軸, () => select_吸嘴Z軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴Z軸,    () => lbl_acpos_吸嘴Z軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴Z軸,    () => lbl_spd_吸嘴Z軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_吸嘴Z軸, () => select_吸嘴Z軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴Z軸,    () => lbl_acpos_吸嘴Z軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴Z軸,    () => lbl_spd_吸嘴Z軸.BackColor   = Color.Gray);
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
        public const double dbNozzleR_Home位  = 0.0;
        public const double dbNozzleR_0補正值 = 1.350;
        public double dbapiNozzleR_defaultSpeed(double dbIncreaseNozzleR)  //NozzleR
        {
            double dbDefaultSpeed = (360.0) * 0.1;
            return dbapiNozzleR(dbIncreaseNozzleR, dbDefaultSpeed);
        }
        public double dbapiNozzleR_InsertSpeed(double dbIncreaseNozzleR)  //NozzleR
        {
            return dbapiNozzleR(dbIncreaseNozzleR, dbInsertSpeedNozzleR);
        }
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
                    UIHelper.SetControlProperty(lbl_吸嘴R軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴R軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_吸嘴R軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_吸嘴R軸_RAW,    () => lbl_吸嘴R軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    while (dbGet >= 360.0) { dbGet -= 360.0; }  //overflow
                    while (dbGet <    0.0) { dbGet += 360.0; }  //overflow
                    UIHelper.SetControlProperty(lbl_吸嘴R軸_Convert,    () => lbl_吸嘴R軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_吸嘴R軸_Back,    () => lbl_吸嘴R軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstNozzleR             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴R軸,    () => lbl_acpos_吸嘴R軸.Text   = dbRstNozzleR.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_吸嘴R軸,    () => lbl_spd_吸嘴R軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_吸嘴R軸, () => select_吸嘴R軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴R軸,    () => lbl_acpos_吸嘴R軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴R軸,    () => lbl_spd_吸嘴R軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_吸嘴R軸, () => select_吸嘴R軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_吸嘴R軸,    () => lbl_acpos_吸嘴R軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_吸嘴R軸,    () => lbl_spd_吸嘴R軸.BackColor   = Color.Gray);
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
                    dbIncreaseNozzleR += dbNozzleR_0補正值;

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
        public const double dbCarrierX_Home位  = 95.0;
        public double dbapiCarrierX_defaultSpeed(double dbIncreaseCarrierX)  //CarrierX
        {
            double dbDefaultSpeed = (190.0) * 0.1;
            return dbapiCarrierX(dbIncreaseCarrierX, dbDefaultSpeed);
        }
        public double dbapiCarrierX_InsertSpeed(double dbIncreaseCarrierX)  //CarrierX
        {
            return dbapiCarrierX(dbIncreaseCarrierX, dbInsertSpeedCarrierX);
        }
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
                    UIHelper.SetControlProperty(lbl_載盤X軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_載盤X軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_載盤X軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_載盤X軸_RAW,    () => lbl_載盤X軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_載盤X軸_Convert,    () => lbl_載盤X軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_載盤X軸_Back,    () => lbl_載盤X軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstCarrierX            = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_載盤X軸,    () => lbl_acpos_載盤X軸.Text   = dbRstCarrierX.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_載盤X軸,    () => lbl_spd_載盤X軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_載盤X軸, () => select_載盤X軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_載盤X軸,    () => lbl_acpos_載盤X軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_載盤X軸,    () => lbl_spd_載盤X軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_載盤X軸, () => select_載盤X軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_載盤X軸,    () => lbl_acpos_載盤X軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_載盤X軸,    () => lbl_spd_載盤X軸.BackColor   = Color.Gray);
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
        public const double dbCarrierY_Home位  = 10.0;
        public double dbapiCarrierY_defaultSpeed(double dbIncreaseCarrierY)  //CarrierY
        {
            double dbDefaultSpeed = (800.0) * 0.1;
            return dbapiCarrierY(dbIncreaseCarrierY, dbDefaultSpeed);
        }
        public double dbapiCarrierY_InsertSpeed(double dbIncreaseCarrierY)  //CarrierY
        {
            return dbapiCarrierY(dbIncreaseCarrierY, dbInsertSpeedCarrierY);
        }
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
                    UIHelper.SetControlProperty(lbl_載盤Y軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_載盤Y軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_載盤Y軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_載盤Y軸_RAW,    () => lbl_載盤Y軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_載盤Y軸_Convert,    () => lbl_載盤Y軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_載盤Y軸_Back,    () => lbl_載盤Y軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstCarrierY            = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_載盤Y軸,    () => lbl_acpos_載盤Y軸.Text   = dbRstCarrierY.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_載盤Y軸,    () => lbl_spd_載盤Y軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_載盤Y軸, () => select_載盤Y軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_載盤Y軸,    () => lbl_acpos_載盤Y軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_載盤Y軸,    () => lbl_spd_載盤Y軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_載盤Y軸, () => select_載盤Y軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_載盤Y軸,    () => lbl_acpos_載盤Y軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_載盤Y軸,    () => lbl_spd_載盤Y軸.BackColor   = Color.Gray);
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
        public const double dbSetZ_Home位  = 15.0;
        public const double dbSetZ_放料位  = 12.0;
        public double dbapiSetZ_defaultSpeed(double dbIncreaseSetZ)  //SetZ
        {
            double dbDefaultSpeed = (33.0) * 0.1;
            return dbapiSetZ(dbIncreaseSetZ, dbDefaultSpeed);
        }
        public double dbapiSetZ_InsertSpeed(double dbIncreaseSetZ)  //SetZ
        {
            return dbapiSetZ(dbIncreaseSetZ, dbInsertSpeedSetZ);
        }
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

            double dbUpperLimit = dbIncreaseSetZ;
            if(dbUpperLimit < 10.0) {
                dbUpperLimit = 10.0;

                MessageBox.Show("錯誤:植針位置小於10, 太高了");
            }

            {  // start of 植針Z軸 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 植針Z軸 資訊
                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff((int)WMX3軸定義.植針Z軸, ref position, ref speed);

                //當數值有效
                if( (position != "") && (speed != "") ) { 
                    UIHelper.SetControlProperty(lbl_植針Z軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_植針Z軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_植針Z軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_植針Z軸_RAW,    () => lbl_植針Z軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_植針Z軸_Convert,    () => lbl_植針Z軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_植針Z軸_Back,    () => lbl_植針Z軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstSetZ                = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_植針Z軸,    () => lbl_acpos_植針Z軸.Text   = dbRstSetZ.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_植針Z軸,    () => lbl_spd_植針Z軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_植針Z軸, () => select_植針Z軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_植針Z軸,    () => lbl_acpos_植針Z軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_植針Z軸,    () => lbl_spd_植針Z軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_植針Z軸, () => select_植針Z軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_植針Z軸,    () => lbl_acpos_植針Z軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_植針Z軸,    () => lbl_spd_植針Z軸.BackColor   = Color.Gray);
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
        public double dbapiSetR_defaultSpeed(double dbIncreaseSetR)  //SetR
        {
            double dbDefaultSpeed = (360.0) * 0.1;
            return dbapiSetR(dbIncreaseSetR, dbDefaultSpeed);
        }
        public double dbapiSetR_InsertSpeed(double dbIncreaseSetR)  //SetR
        {
            return dbapiSetR(dbIncreaseSetR, dbInsertSpeedSetR);
        }
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
                    UIHelper.SetControlProperty(lbl_植針R軸_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_植針R軸_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_植針R軸_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_植針R軸_RAW,    () => lbl_植針R軸_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_植針R軸_Convert,    () => lbl_植針R軸_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_植針R軸_Back,    () => lbl_植針R軸_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstSetR                = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_植針R軸,    () => lbl_acpos_植針R軸.Text   = dbRstSetR.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_植針R軸,    () => lbl_spd_植針R軸.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_植針R軸, () => select_植針R軸.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_植針R軸,    () => lbl_acpos_植針R軸.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_植針R軸,    () => lbl_spd_植針R軸.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_植針R軸, () => select_植針R軸.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_植針R軸,    () => lbl_acpos_植針R軸.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_植針R軸,    () => lbl_spd_植針R軸.BackColor   = Color.Gray);
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
        public const double dbGate_開門    = 0.0;
        public const double dbGate_關門    = 580.0;
        public double dbapiGate_defaultSpeed(double dbIncreaseGate)  //Gate
        {
            double dbDefaultSpeed = (580.0) * 0.1;
            return dbapiGate(dbIncreaseGate, dbDefaultSpeed);
        }
        public double dbapiGate_InsertSpeed(double dbIncreaseGate)  //Gate
        {
            return dbapiGate(dbIncreaseGate, dbInsertSpeedGate);
        }
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
                    UIHelper.SetControlProperty(lbl_工作門_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_工作門_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_工作門_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert              = (int)(double.Parse(position));
                    int Speed                = (int)double.Parse(speed);
                    UIHelper.SetControlProperty(lbl_工作門_RAW,    () => lbl_工作門_RAW.Text      = Convert.ToString());

                    //得到轉換數值
                    double dbGet             = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed           = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_工作門_Convert,    () => lbl_工作門_Convert.Text  = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback               = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_工作門_Back,    () => lbl_工作門_Back.Text     = cnback.ToString());


                    //顯示讀取長度
                    dbRstGate                = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_工作門,    () => lbl_acpos_工作門.Text    = dbRstGate.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_工作門,    () => lbl_spd_工作門.Text      = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 1) {
                    UIHelper.SetControlProperty(select_工作門, () => select_工作門.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_工作門,    () => lbl_acpos_工作門.BackColor  = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_工作門,    () => lbl_spd_工作門.BackColor    = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_工作門, () => select_工作門.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_工作門,    () => lbl_acpos_工作門.BackColor  = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_工作門,    () => lbl_spd_工作門.BackColor    = Color.Gray);
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
        public const double dbIAI_Home位  = 0.0;
        public const double dbIAI_預備位  = 10.0;
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
                    UIHelper.SetControlProperty(lbl_IAI_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_IAI_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_IAI_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_GetCurrentSpeed4Bytes, 0);
                    UIHelper.SetControlProperty(lbl_IAI_RAW,    () => lbl_IAI_RAW.Text              = Convert.ToString());

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Maxdb, Mindb);
                    double dbSpeed                = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_IAI_Convert,    () => lbl_IAI_Convert.Text          = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Maxdb, (int)Mindb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_IAI_Back,    () => lbl_IAI_Back.Text             = cnback.ToString());


                    //顯示讀取長度
                    dbRstIAI                      = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_IAI,    () => lbl_acpos_IAI.Text            = dbRstIAI.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_IAI,    () => lbl_spd_IAI.Text              = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 2) {
                    UIHelper.SetControlProperty(select_Socket檢測, () => select_Socket檢測.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_IAI,    () => lbl_acpos_IAI.BackColor          = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_IAI,    () => lbl_spd_IAI.BackColor            = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_Socket檢測, () => select_Socket檢測.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_IAI,    () => lbl_acpos_IAI.BackColor          = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_IAI,    () => lbl_spd_IAI.BackColor            = Color.Gray);
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
        public const double dbJoDell3D掃描_Home位  = 10.0;
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
                    UIHelper.SetControlProperty(lbl_JoDell3D掃描_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_JoDell3D掃描_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_JoDell3D掃描_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    UIHelper.SetControlProperty(lbl_JoDell3D掃描_RAW,    () => lbl_JoDell3D掃描_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed                = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_JoDell3D掃描_Convert,    () => lbl_JoDell3D掃描_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_JoDell3D掃描_Back,    () => lbl_JoDell3D掃描_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstJoDell3D掃描             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_JoDell3D掃描,    () => lbl_acpos_JoDell3D掃描.Text   = dbRstJoDell3D掃描.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_JoDell3D掃描,    () => lbl_spd_JoDell3D掃描.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 4) {
                    UIHelper.SetControlProperty(select_JoDell3D掃描, () => select_JoDell3D掃描.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_JoDell3D掃描,    () => lbl_acpos_JoDell3D掃描.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_JoDell3D掃描,    () => lbl_spd_JoDell3D掃描.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_JoDell3D掃描, () => select_JoDell3D掃描.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_JoDell3D掃描,    () => lbl_acpos_JoDell3D掃描.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_JoDell3D掃描,    () => lbl_spd_JoDell3D掃描.BackColor   = Color.Gray);
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
        public const double dbJoDell吸針嘴_Home位  = 5.0;
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
                    UIHelper.SetControlProperty(lbl_JoDell吸針嘴_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_JoDell吸針嘴_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_JoDell吸針嘴_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert                   = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed                     = clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    UIHelper.SetControlProperty(lbl_JoDell吸針嘴_RAW,    () => lbl_JoDell吸針嘴_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet                  = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed                = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_JoDell吸針嘴_Convert,    () => lbl_JoDell吸針嘴_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback                    = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_JoDell吸針嘴_Back,    () => lbl_JoDell吸針嘴_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstJoDell吸針嘴             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_JoDell吸針嘴,    () => lbl_acpos_JoDell吸針嘴.Text   = dbRstJoDell吸針嘴.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_JoDell吸針嘴,    () => lbl_spd_JoDell吸針嘴.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 4) {
                    UIHelper.SetControlProperty(select_JoDell吸針嘴, () => select_JoDell吸針嘴.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_JoDell吸針嘴,    () => lbl_acpos_JoDell吸針嘴.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_JoDell吸針嘴,    () => lbl_spd_JoDell吸針嘴.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_JoDell吸針嘴, () => select_JoDell吸針嘴.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_JoDell吸針嘴,    () => lbl_acpos_JoDell吸針嘴.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_JoDell吸針嘴,    () => lbl_spd_JoDell吸針嘴.BackColor   = Color.Gray);
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
        public double dbTargetPositionJoDell植針嘴相機 =  0.0;
        public const double dbJoDell植針嘴相機_Home位  = 10.0;
        public double dbapiJoDell植針嘴相機(double dbIncreaseJoDell植針嘴相機)  //JoDell植針嘴相機
        {
            Normal calculate = new Normal();
                const int    MaxRAW =   5000;
                const int    MinRAW =      0;
                const double Maxdb  =   50.0;
                const double Mindb  =    0.0;
                const double Sum    =   5000;
                const double dbSpdF =  Sum / Maxdb;

            double dbRstJoDell植針嘴相機 = 0.0;

            {  // start of JoDell植針嘴相機 讀取與顯示
                int    rslt     = 0;
                string position = "";
                string speed    = "";

                //讀取 JoDell植針嘴相機 資訊
                byte[] JODELL_RX = new byte[18];
                int addr_TargetGetDevice   = (int)(addr_JODELL.pxeaJ_植針嘴相機_Input) / 10;
                int addr_TargetGetFunction = (int)(addr_JODELL.pxeaJ_GetAddr_START) / 10;
                clsServoControlWMX3.WMX3_GetInIO(ref JODELL_RX, addr_TargetGetDevice + addr_TargetGetFunction, JODELL_RX.Length);

                int[] varJODELL_RX = new int[JODELL_RX.Length / 2];
                for (int i = 0; i < varJODELL_RX.Length; i++) {
                    varJODELL_RX[i] = BitConverter.ToInt16(JODELL_RX, i * 2);
                }
                rslt = varJODELL_RX[0];

                //當數值有效
                if(true) {
                    UIHelper.SetControlProperty(lbl_JoDell植針嘴相機_RAW,     () => lbl_植針Z軸_RAW.Visible     = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_JoDell植針嘴相機_Convert, () => lbl_植針Z軸_Convert.Visible = bshow_debug_RAW_Conver_Back_Value);
                    UIHelper.SetControlProperty(lbl_JoDell植針嘴相機_Back,    () => lbl_植針Z軸_Back.Visible    = bshow_debug_RAW_Conver_Back_Value);


                    //得到原始數值
                    int Convert                       = clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaI_GetPosition, 0);
                    int Speed                         = clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaJ_GetAddr_Speed2Bytes, 0);
                    UIHelper.SetControlProperty(lbl_JoDell植針嘴相機_RAW,    () => lbl_JoDell植針嘴相機_RAW.Text     = Convert.ToString());

                    //得到轉換數值
                    double dbGet                      = calculate.Map(Convert, MaxRAW, MinRAW, Mindb, Maxdb);
                    double dbSpeed                    = Speed / dbSpdF;
                    UIHelper.SetControlProperty(lbl_JoDell植針嘴相機_Convert,    () => lbl_JoDell植針嘴相機_Convert.Text = dbGet.ToString("F3"));

                    //轉回原始數值
                    int cnback                        = (int)calculate.Map((int)dbGet, (int)Mindb, (int)Maxdb, (double)MaxRAW, (double)MinRAW);
                    UIHelper.SetControlProperty(lbl_JoDell植針嘴相機_Back,    () => lbl_JoDell植針嘴相機_Back.Text    = cnback.ToString());


                    //顯示讀取長度
                    dbRstJoDell植針嘴相機             = dbGet;
                    UIHelper.SetControlProperty(lbl_acpos_JoDell植針嘴相機,    () => lbl_acpos_JoDell植針嘴相機.Text   = dbRstJoDell植針嘴相機.ToString("F3"));

                    //顯示運動速度
                    UIHelper.SetControlProperty(lbl_spd_JoDell植針嘴相機,    () => lbl_spd_JoDell植針嘴相機.Text     = dbSpeed.ToString("F3"));
                }

                //變更顏色
                if (rslt == 4) {
                    UIHelper.SetControlProperty(select_JoDell植針嘴相機, () => select_JoDell植針嘴相機.BackColor = Color.Red);
                    UIHelper.SetControlProperty(lbl_acpos_JoDell植針嘴相機,    () => lbl_acpos_JoDell植針嘴相機.BackColor = Color.White);
                    UIHelper.SetControlProperty(lbl_spd_JoDell植針嘴相機,    () => lbl_spd_JoDell植針嘴相機.BackColor   = Color.White);
                } else {
                    UIHelper.SetControlProperty(select_JoDell植針嘴相機, () => select_JoDell植針嘴相機.BackColor = Color.Green);
                    UIHelper.SetControlProperty(lbl_acpos_JoDell植針嘴相機,    () => lbl_acpos_JoDell植針嘴相機.BackColor = Color.Gray);
                    UIHelper.SetControlProperty(lbl_spd_JoDell植針嘴相機,    () => lbl_spd_JoDell植針嘴相機.BackColor   = Color.Gray);
                }

            }  // end of JoDell植針嘴相機 讀取與顯示

            //Function Classification
            switch (dbIncreaseJoDell植針嘴相機) {
                case dbRead:
                    break;

                case dbCheckArrived: {
                    double dbMin = 0.0;
                    double dbMax = 0.0;
                    if(dbTargetPositionJoDell植針嘴相機 * 1.01 > dbTargetPositionJoDell植針嘴相機 * 0.99) { 
                        dbMin = dbTargetPositionJoDell植針嘴相機 * 0.99 - 0.1;
                        dbMax = dbTargetPositionJoDell植針嘴相機 * 1.01 + 0.1;
                    } else { 
                        dbMin = dbTargetPositionJoDell植針嘴相機 * 1.01 - 0.1;
                        dbMax = dbTargetPositionJoDell植針嘴相機 * 0.99 + 0.1;
                    }
                    if( dbMin <= dbRstJoDell植針嘴相機 &&
                                 dbRstJoDell植針嘴相機 <= dbMax) { 
                        return dbAxisMoveOk;
                    } else {
                        return dbAxisMoveNg;
                    }
                } break;

                default: {  //植針嘴 變更位置
                    //伸長量overflow保護
                    if( Mindb<= dbIncreaseJoDell植針嘴相機 && dbIncreaseJoDell植針嘴相機 <= Maxdb ) {

                    } else if(dbIncreaseJoDell植針嘴相機 <= Mindb ) {
                            dbIncreaseJoDell植針嘴相機 = (int)Mindb;
                    } else if( Maxdb<= dbIncreaseJoDell植針嘴相機) {
                            dbIncreaseJoDell植針嘴相機 = (int)Maxdb;
                    }

                    // 取得欲變更的的浮點數
                    double fChangeGate = calculate.Map(dbIncreaseJoDell植針嘴相機, (double)Mindb, (double)Maxdb, (double)Maxdb, (double)Mindb);
                        dbTargetPositionJoDell植針嘴相機 = dbIncreaseJoDell植針嘴相機;

                    //執行移動JoDell植針嘴
                    clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaI_GoToPosition, fChangeGate);
                } break;
            }

            return dbRstJoDell植針嘴相機;
        }  // end of public double dbapiJoDell植針嘴相機(double dbIncreaseJoDell植針嘴)  //JoDell植針嘴相機
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
                        UIHelper.RunOnUIThread(this, () => { btn_OpenFile_Click(sender, e); });
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
            } else if (GetPara == "SetNozzleCircularity") {  //堵嘴 植針孔閥值
                rsult = apiParaReadIndex("SaveParameterJason.json", 37);
            } else if (GetPara == "ObjectWidthToleranceCondition") {      //視覺:物件寬度誤差限制
                rsult = apiParaReadIndex("SaveParameterJason.json", 47);
            } else if (GetPara == "ObjectLengthToleranceCondition") {     //視覺:物件長度誤差限制
                rsult = apiParaReadIndex("SaveParameterJason.json", 48);
            } else if (GetPara == "ObjectLength2DetectionCondition") {    //視覺:物件長度偵測Len2
                rsult = apiParaReadIndex("SaveParameterJason.json", 49);
            } else if (GetPara == "ObjectLengthAmpDetectionCondition") {  //視覺:物件長度偵測Amp
                rsult = apiParaReadIndex("SaveParameterJason.json", 50);
            } else if (GetPara == "SocketAreaMin") {  //視覺:Socket面積限制小
                rsult = apiParaReadIndex("SaveParameterJason.json", 51);
            } else if (GetPara == "SocketAreaMax") {  //視覺:Socket面積限制大
                rsult = apiParaReadIndex("SaveParameterJason.json", 52);
            } else if (GetPara == "SocketDist") {  //視覺:Socket距離限制
                rsult = apiParaReadIndex("SaveParameterJason.json", 53);
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

            //InitSpeed Num Block
            SpeedNozzleX.Value  = (int)dbInsertSpeedNozzleX;
            SpeedNozzleY.Value  = (int)dbInsertSpeedNozzleY;
            SpeedNozzleZ.Value  = (int)dbInsertSpeedNozzleZ;
            SpeedNozzleR.Value  = (int)dbInsertSpeedNozzleR;
            SpeedCarriorX.Value = (int)dbInsertSpeedCarrierX;
            SpeedCarriorY.Value = (int)dbInsertSpeedCarrierY;
            SpeedSetZ.Value     = (int)dbInsertSpeedSetZ;
            SpeedSetR.Value     = (int)dbInsertSpeedSetR;

            //Light Thread
            {
                Thread thread_LightTask   = new Thread(new ThreadStart(DoWork_Light));
                Thread thread_NozzleTask  = new Thread(new ThreadStart(DoWork_Nozzle));
                Thread thread_SetTask     = new Thread(new ThreadStart(DoWork_Set));
                Thread thread_電動缸Task  = new Thread(new ThreadStart(DoWork_電動缸));
                Thread thread_CarriorTask = new Thread(new ThreadStart(DoWork_Carrior));
                Thread thread_FileTask    = new Thread(new ThreadStart(DoWork_File));

                thread_LightTask.Start();
                thread_NozzleTask.Start();
                thread_SetTask.Start();
                thread_電動缸Task.Start();
                thread_CarriorTask.Start();
                thread_FileTask.Start();
            }
        }
        //---------------------------------------------------------------------------------------
        void DoWork_Light() {
            while(true) {
                Xavier_TASK1();  //面板按鈕以及指示燈

                Console.WriteLine("DoWork_Light thread\r\n");
                Thread.Sleep(1);
            }
        }
        void DoWork_Nozzle() {
            while(true) {
                Xavier_TASK2();  //吸嘴軸組

                Console.WriteLine("DoWork_Nozzle thread\r\n");
                Thread.Sleep(1);
            }
        }
        void DoWork_Set() {
            while(true) {
                Xavier_TASK3();  //植針軸組

                Console.WriteLine("DoWork_Set thread\r\n");
                Thread.Sleep(1);
            }
        }
        void DoWork_電動缸() {
            while(true) {
                Xavier_TASK4();  //電動缸組_含抽針

                Console.WriteLine("DoWork_電動缸 thread\r\n");
                Thread.Sleep(1);
            }
        }
        void DoWork_Carrior()
        {
            while (true)
            {
                Xavier_TASK5();  //載盤組

                Console.WriteLine("DoWork_Carrior thread\r\n");
                Thread.Sleep(1);
            }
        }
        void DoWork_File()
        {
            while (true)
            {
                Xavier_TASK6();  //IO檢查_工作門_檔案組

                Console.WriteLine("DoWork_File thread\r\n");
                Thread.Sleep(1);
            }
        }
        //---------------------------------------------------------------------------------------
        public void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clsServoControlWMX3.WMX3_destroy_Commu();
            XavierLogger.XavierLogger_Shutdown();
            //sw.Close();
        }
        //---------------------------------------------------------------------------------------
        public void btn_manual_Click(object sender, EventArgs e)
        {
            TestForm fmTestForm = new TestForm();
            fmTestForm.Show();

            UIHelper.SetControlProperty(btn_manual, () => btn_manual.Enabled = false);
        }
        //---------------------------------------------------------------------------------------
        ParameterForm fmParameterFormHandle;
        public void btn_參數_Click(object sender, EventArgs e)
        {
            ParameterForm fmParameterForm = new ParameterForm();
            fmParameterForm.Show();

            fmParameterFormHandle = fmParameterForm;

            UIHelper.SetControlProperty(btn_參數, () => btn_參數.Enabled = false);
        }
        //---------------------------------------------------------------------------------------
        public void btn_植針_Click(object sender, EventArgs e)
        {
            PlaceForm fmPlaceForm = new PlaceForm();
            fmPlaceForm.Show();

            UIHelper.SetControlProperty(btn_植針, () => btn_植針.Enabled = false);
        }
        //---------------------------------------------------------------------------------------
        private void btn_取針_Click(object sender, EventArgs e)
        {
            RemoveForm fmRemoveForm = new RemoveForm();
            fmRemoveForm.Show();

            UIHelper.SetControlProperty(btn_取針, () => btn_取針.Enabled = false);
        }
        //---------------------------------------------------------------------------------------
        private void btn_置換_Click(object sender, EventArgs e)
        {
            ReplaceForm fmReplaceForm = new ReplaceForm();
            fmReplaceForm.Show();

            UIHelper.SetControlProperty(btn_置換, () => btn_置換.Enabled = false);
        }
        //---------------------------------------------------------------------------------------
        private void btn_拋料_Click(object sender, EventArgs e)
        {
            TakePinForm fmTakePinForm = new TakePinForm();
            fmTakePinForm.Show();

            UIHelper.SetControlProperty(btn_拋料, () => btn_拋料.Enabled = false);
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
            int i;
            for(i=0; i<20; i++) { 
                clsServoControlWMX3.WMX3_ClearAlarm(0);
                clsServoControlWMX3.WMX3_ClearAlarm(1);
            }

                clsServoControlWMX3.WMX3_ClearAlarm(2);
                clsServoControlWMX3.WMX3_ClearAlarm(3);
                clsServoControlWMX3.WMX3_ClearAlarm(4);
                clsServoControlWMX3.WMX3_ClearAlarm(5);
                clsServoControlWMX3.WMX3_ClearAlarm(6);
                clsServoControlWMX3.WMX3_ClearAlarm(7);
                clsServoControlWMX3.WMX3_ClearAlarm(8);
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

            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff,                isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,                 isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn,     isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn,     isOn?1.0:0.0);
            clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaI_MotorOn, isOn?1.0:0.0);
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
        public bool enGC_吸嘴X軸          = false;
        public bool enGC_吸嘴Y軸          = false;
        public bool enGC_吸嘴Z軸          = false;
        public bool enGC_吸嘴R軸          = false;

        public bool enGC_載盤X軸          = false;
        public bool enGC_載盤Y軸          = false;

        public bool enGC_植針Z軸          = false;
        public bool enGC_植針R軸          = false;

        public bool enGC_工作門           = false;

        public bool enGC_IAI              = false;

        public bool enGC_JoDell植針嘴相機 = false;
        public bool enGC_JoDell3D掃描     = false;
        public bool enGC_JoDell吸針嘴     = false;

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

            if (enGC_JoDell植針嘴相機 != en_JoDell植針嘴相機.Checked){
                enGC_JoDell植針嘴相機  = en_JoDell植針嘴相機.Checked;

                clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaI_MotorOn, (enGC_JoDell植針嘴相機) ? 1.0 : 0.0);
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
                } else if (selectedRadioButton == select_JoDell植針嘴相機) {
                    wmxId_RadioGroupChanged = WMX3軸定義.JoDell植針嘴相機;
                }
            }

            //複製選擇之軸
                   if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴X軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴X軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴Y軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴Y軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴Z軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴Z軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.吸嘴R軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_吸嘴R軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.載盤X軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_載盤X軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.載盤Y軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_載盤Y軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.植針Z軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_植針Z軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.植針R軸) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_植針R軸.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.工作門) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_工作門.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.IAISocket孔檢測) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_IAI.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.JoDell3D掃描) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_JoDell3D掃描.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.JoDell吸針嘴) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_JoDell吸針嘴.Text).ToString("F3")));
            } else if (wmxId_RadioGroupChanged == WMX3軸定義.JoDell植針嘴相機) {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = (double.Parse(lbl_acpos_JoDell植針嘴相機.Text).ToString("F3")));
            } else {
                UIHelper.SetControlProperty(txtABSpos,    () => txtABSpos.Text = "N/A");
            }

        }  // end of public void RadioGroupChanged(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        public void lbl_SetIO_Click(object sender, EventArgs e)
        {  // start of public void lbl_SetIO_Click(object sender, EventArgs e)
            // 將 sender 轉型為 Label
            System.Windows.Forms.Label SelectLabel = sender as System.Windows.Forms.Label;

            //辨識選擇之Label
            if (SelectLabel != null) {
                       if (SelectLabel == lbl擺放蓋板     ) { digitalToggle((int)WMX3IO對照.pxeIO_擺放座蓋板);
                } else if (SelectLabel == lbl吸料真空閥   ) { digitalToggle((int)WMX3IO對照.pxeIO_吸料真空電磁閥);
                } else if (SelectLabel == lbl堵料吹氣缸   ) { digitalToggle((int)WMX3IO對照.pxeIO_堵料吹氣缸);
                } else if (SelectLabel == lbl接料區缸     ) { digitalToggle((int)WMX3IO對照.pxeIO_接料區氣桿);
                } else if (SelectLabel == lbl植針吹氣     ) { digitalToggle((int)WMX3IO對照.pxeIO_植針吹氣);
                } else if (SelectLabel == lbl收料區缸     ) { digitalToggle((int)WMX3IO對照.pxeIO_收料區缸);
                } else if (SelectLabel == lbl堵料吹氣     ) { digitalToggle((int)WMX3IO對照.pxeIO_堵料吹氣);
                } else if (SelectLabel == lbl_NA_25       ) { digitalToggle((int)WMX3IO對照.pxeIO_NA_O_07);
                                                      
                } else if (SelectLabel == lbl載盤真空閥   ) { digitalToggle((int)WMX3IO對照.pxeIO_載盤真空閥);
                } else if (SelectLabel == lblsk真空2      ) { digitalToggle((int)WMX3IO對照.pxeIO_Socket真空2);
                } else if (SelectLabel == lbl載盤破真空   ) { digitalToggle((int)WMX3IO對照.pxeIO_載盤破真空);
                } else if (SelectLabel == lblsk破真空2    ) { digitalToggle((int)WMX3IO對照.pxeIO_Socket破真空2);
                } else if (SelectLabel == lblsk真空1      ) { digitalToggle((int)WMX3IO對照.pxeIO_Socket真空1);
                } else if (SelectLabel == lbl擺放座真空   ) { digitalToggle((int)WMX3IO對照.pxeIO_擺放座吸真空);
                } else if (SelectLabel == lblsk破真空1    ) { digitalToggle((int)WMX3IO對照.pxeIO_Socket破真空1);
                } else if (SelectLabel == lbl擺放破真空   ) { digitalToggle((int)WMX3IO對照.pxeIO_擺放座破真空);
                                                         
                } else if (SelectLabel == lbl取料吸嘴吸   ) { digitalToggle((int)WMX3IO對照.pxeIO_取料吸嘴吸);
                } else if (SelectLabel == lbl下後左門鎖   ) { digitalToggle((int)WMX3IO對照.pxeIO_下後左門鎖);
                } else if (SelectLabel == lbl取料吸嘴破舊 ) { digitalToggle((int)WMX3IO對照.pxeIO_取料吸嘴破真空舊);
                } else if (SelectLabel == lbl下後右門鎖   ) { digitalToggle((int)WMX3IO對照.pxeIO_下後右門鎖);
                } else if (SelectLabel == lbl植針Z煞車    ) { digitalToggle((int)WMX3IO對照.pxeIO_植針Z煞車);
                } else if (SelectLabel == lblHEPA         ) { digitalToggle((int)WMX3IO對照.pxeIO_HEPA);
                } else if (SelectLabel == lbl取料吸嘴破新 ) { digitalToggle((int)WMX3IO對照.pxeIO_取料吸嘴破真空新);
                } else if (SelectLabel == lbl艙內燈       ) { digitalToggle((int)WMX3IO對照.pxeIO_LIGHT);
                                                            
                } else if (SelectLabel == lbl右按鈕綠燈   ) { digitalToggle((int)WMX3IO對照.pxeIO_面板右按鈕綠燈);
                } else if (SelectLabel == lbl紅燈         ) { digitalToggle((int)WMX3IO對照.pxeIO_機台紅燈);
                } else if (SelectLabel == lbl中按鈕綠燈   ) { digitalToggle((int)WMX3IO對照.pxeIO_面板中按鈕綠燈);
                } else if (SelectLabel == lbl黃燈         ) { digitalToggle((int)WMX3IO對照.pxeIO_機台黃燈);
                } else if (SelectLabel == lbl左按鈕紅燈   ) { digitalToggle((int)WMX3IO對照.pxeIO_面板左按鈕紅燈);
                } else if (SelectLabel == lbl綠燈         ) { digitalToggle((int)WMX3IO對照.pxeIO_機台綠燈);
                } else if (SelectLabel == lbl_NA_31       ) { digitalToggle((int)WMX3IO對照.pxeIO_NA_O_36);
                } else if (SelectLabel == lblBuzzer       ) { digitalToggle((int)WMX3IO對照.pxeIO_Buzzer);
                }
            }  // end of if (SelectLabel != null) {
        }  // end of public void lbl_SetIO_Click(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        private void Speed_ValueChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.NumericUpDown vluChg = sender as System.Windows.Forms.NumericUpDown;

                   if(vluChg == SpeedNozzleX) {
                dbInsertSpeedNozzleX = (double)SpeedNozzleX.Value;
            } else if(vluChg == SpeedNozzleY) {
                dbInsertSpeedNozzleY = (double)SpeedNozzleY.Value;
            } else if(vluChg == SpeedNozzleZ) {
                dbInsertSpeedNozzleZ = (double)SpeedNozzleZ.Value;
            } else if(vluChg == SpeedNozzleR) {
                dbInsertSpeedNozzleR = (double)SpeedNozzleR.Value;
            } else if(vluChg == SpeedCarriorX) {
                dbInsertSpeedCarrierX = (double)SpeedCarriorX.Value;
            } else if(vluChg == SpeedCarriorY) {
                dbInsertSpeedCarrierY = (double)SpeedCarriorY.Value;
            } else if(vluChg == SpeedSetZ) {
                dbInsertSpeedSetZ = (double)SpeedSetZ.Value;
            } else if(vluChg == SpeedSetR) {
                dbInsertSpeedSetR = (double)SpeedSetR.Value;
            }
        }
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
                    case WMX3軸定義.吸嘴X軸:          if(enGC_吸嘴X軸          == true) { dbapiNozzleX(    result, 250);       } break;
                    case WMX3軸定義.吸嘴Y軸:          if(enGC_吸嘴Y軸          == true) { dbapiNozzleY(    result, 100);       } break;
                    case WMX3軸定義.吸嘴Z軸:          if(enGC_吸嘴Z軸          == true) { dbapiNozzleZ(    result,  20);       } break;
                    case WMX3軸定義.吸嘴R軸:          if(enGC_吸嘴R軸          == true) { dbapiNozzleR(    result,  70);       } break;
                    case WMX3軸定義.載盤X軸:          if(enGC_載盤X軸          == true) { dbapiCarrierX(   result, 190);       } break;
                    case WMX3軸定義.載盤Y軸:          if(enGC_載盤Y軸          == true) { dbapiCarrierY(   result, 800);       } break;
                    case WMX3軸定義.植針Z軸:          if(enGC_植針Z軸          == true) { dbapiSetZ(       result, 33);        } break;
                    case WMX3軸定義.植針R軸:          if(enGC_植針R軸          == true) { dbapiSetR(       result, 360);       } break;
                    case WMX3軸定義.工作門:           if(enGC_工作門           == true) { dbapiGate(       result, 580/4);     } break;
                    case WMX3軸定義.IAISocket孔檢測:  if(enGC_IAI              == true) { dbapiIAI(        result);            } break;
                    case WMX3軸定義.JoDell3D掃描:     if(enGC_JoDell3D掃描     == true) { dbapiJoDell3D掃描(result);           } break;
                    case WMX3軸定義.JoDell吸針嘴:     if(enGC_JoDell吸針嘴     == true) { dbapiJoDell吸針嘴(result);           } break;
                    case WMX3軸定義.JoDell植針嘴相機: if(enGC_JoDell植針嘴相機 == true) { dbapiJoDell植針嘴相機(result);       } break;
                }
            }

            UIHelper.SetControlProperty(txtABSpos, () => txtABSpos.Text = result.ToString("F3"));
        }  // end of public void btn_adjust_JOG(object sender, EventArgs e)
        //---------------------------------------------------------------------------------------
        private void btn_plus_minus_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button ptrBtn = sender as System.Windows.Forms.Button;

            double result = double.Parse(txtABSpos.Text);
            double dbdiff = double.Parse(edit_diff_value.Text);

            if (ptrBtn == btn_plus ) {
                UIHelper.SetControlProperty(txtABSpos, () => txtABSpos.Text = (result + dbdiff).ToString("F3"));
            } else if (ptrBtn == btn_minus) {
                UIHelper.SetControlProperty(txtABSpos, () => txtABSpos.Text = (result - dbdiff).ToString("F3"));
            }
            UIHelper.SetControlProperty(edit_diff_value, () => edit_diff_value.Text = 0.0.ToString("F3"));
        }
        //---------------------------------------------------------------------------------------
        int getCommuStatus = 0;
        public void tmr_ReadWMX3_Tick(object sender, EventArgs e)
        {  // start of public void tmr_ReadWMX3_Tick(object sender, EventArgs e)
            //WMX3通訊狀態
            getCommuStatus = clsServoControlWMX3.WMX3_check_Commu();
            if (getCommuStatus == 1) {
                UIHelper.SetControlProperty(label1, () => label1.Text = "連線中");
                UIHelper.SetControlProperty(label1, () => label1.ForeColor = Color.Red);
            } else {
                UIHelper.SetControlProperty(label1, () => label1.Text = "尚未連線");
                UIHelper.SetControlProperty(label1, () => label1.ForeColor = Color.Black);
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
                dbapiJoDell植針嘴相機(dbState);
            }  // end of double dbState = dbRead;

            //讀取 Yaskawa OutputIO                                                                                                   
            {
                UIHelper.SetIndicator(lbl擺放蓋板,     digitalRead((int)(WMX3IO對照.pxeIO_擺放座蓋板))       == HIGH);
                UIHelper.SetIndicator(lbl吸料真空閥,   digitalRead((int)(WMX3IO對照.pxeIO_吸料真空電磁閥))   == HIGH);
                UIHelper.SetIndicator(lbl堵料吹氣缸,   digitalRead((int)(WMX3IO對照.pxeIO_堵料吹氣缸))       == HIGH);
                UIHelper.SetIndicator(lbl接料區缸,     digitalRead((int)(WMX3IO對照.pxeIO_接料區氣桿))       == HIGH);
                UIHelper.SetIndicator(lbl植針吹氣,     digitalRead((int)(WMX3IO對照.pxeIO_植針吹氣))         == HIGH);
                UIHelper.SetIndicator(lbl收料區缸,     digitalRead((int)(WMX3IO對照.pxeIO_收料區缸))         == HIGH);
                UIHelper.SetIndicator(lbl堵料吹氣,     digitalRead((int)(WMX3IO對照.pxeIO_堵料吹氣))         == HIGH);
                UIHelper.SetIndicator(lbl_NA_25,       digitalRead((int)(WMX3IO對照.pxeIO_NA_O_07))          == HIGH);

                UIHelper.SetIndicator(lbl載盤真空閥,   digitalRead((int)(WMX3IO對照.pxeIO_載盤真空閥))       == HIGH);
                UIHelper.SetIndicator(lblsk真空2,      digitalRead((int)(WMX3IO對照.pxeIO_Socket真空2))      == HIGH);
                UIHelper.SetIndicator(lbl載盤破真空,   digitalRead((int)(WMX3IO對照.pxeIO_載盤破真空))       == HIGH);
                UIHelper.SetIndicator(lblsk破真空2,    digitalRead((int)(WMX3IO對照.pxeIO_Socket破真空2))    == HIGH);
                UIHelper.SetIndicator(lblsk真空1,      digitalRead((int)(WMX3IO對照.pxeIO_Socket真空1))      == HIGH);
                UIHelper.SetIndicator(lbl擺放座真空,   digitalRead((int)(WMX3IO對照.pxeIO_擺放座吸真空))     == HIGH);
                UIHelper.SetIndicator(lblsk破真空1,    digitalRead((int)(WMX3IO對照.pxeIO_Socket破真空1))    == HIGH);
                UIHelper.SetIndicator(lbl擺放破真空,   digitalRead((int)(WMX3IO對照.pxeIO_擺放座破真空))     == HIGH);

                UIHelper.SetIndicator(lbl取料吸嘴吸,   digitalRead((int)(WMX3IO對照.pxeIO_取料吸嘴吸))       == HIGH);
                UIHelper.SetIndicator(lbl下後左門鎖,   digitalRead((int)(WMX3IO對照.pxeIO_下後左門鎖))       == HIGH);
                UIHelper.SetIndicator(lbl取料吸嘴破舊, digitalRead((int)(WMX3IO對照.pxeIO_取料吸嘴破真空舊)) == HIGH);
                UIHelper.SetIndicator(lbl下後右門鎖,   digitalRead((int)(WMX3IO對照.pxeIO_下後右門鎖))       == HIGH);
                UIHelper.SetIndicator(lbl植針Z煞車,    digitalRead((int)(WMX3IO對照.pxeIO_植針Z煞車))        == HIGH);
                UIHelper.SetIndicator(lblHEPA,         digitalRead((int)(WMX3IO對照.pxeIO_HEPA))             == HIGH);
                UIHelper.SetIndicator(lbl取料吸嘴破新, digitalRead((int)(WMX3IO對照.pxeIO_取料吸嘴破真空新)) == HIGH);
                UIHelper.SetIndicator(lbl艙內燈,       digitalRead((int)(WMX3IO對照.pxeIO_LIGHT))            == HIGH);

                UIHelper.SetIndicator(lbl右按鈕綠燈,   digitalRead((int)(WMX3IO對照.pxeIO_面板右按鈕綠燈))   == HIGH);
                UIHelper.SetIndicator(lbl紅燈,         digitalRead((int)(WMX3IO對照.pxeIO_機台紅燈))         == HIGH);
                UIHelper.SetIndicator(lbl中按鈕綠燈,   digitalRead((int)(WMX3IO對照.pxeIO_面板中按鈕綠燈))   == HIGH);
                UIHelper.SetIndicator(lbl黃燈,         digitalRead((int)(WMX3IO對照.pxeIO_機台黃燈))         == HIGH);
                UIHelper.SetIndicator(lbl左按鈕紅燈,   digitalRead((int)(WMX3IO對照.pxeIO_面板左按鈕紅燈))   == HIGH);
                UIHelper.SetIndicator(lbl綠燈,         digitalRead((int)(WMX3IO對照.pxeIO_機台綠燈))         == HIGH);
                UIHelper.SetIndicator(lbl_NA_31,       digitalRead((int)(WMX3IO對照.pxeIO_NA_O_36))          == HIGH);
                UIHelper.SetIndicator(lblBuzzer,       digitalRead((int)(WMX3IO對照.pxeIO_Buzzer))           == HIGH);
            }  // end of //讀取 Yaskawa OutputIO  

            //讀取 Yaskawa InputIO
            {
                UIHelper.SetIndicator(lbl載盤Y後,      indicateRead((int)WMX3IO對照.pxeIO_載盤Y軸後極限)     == HIGH);
                UIHelper.SetIndicator(lbl取料Y後,      indicateRead((int)WMX3IO對照.pxeIO_取料Y軸後極限)     == HIGH);
                UIHelper.SetIndicator(lbl載盤Y前,      indicateRead((int)WMX3IO對照.pxeIO_載盤Y軸前極限)     == HIGH);
                UIHelper.SetIndicator(lbl取料Y前,      indicateRead((int)WMX3IO對照.pxeIO_取料Y軸前極限)     == HIGH);
                UIHelper.SetIndicator(lbl取料X後,      indicateRead((int)WMX3IO對照.pxeIO_取料X軸後極限)     == HIGH);
                UIHelper.SetIndicator(lbl_NA_01,       indicateRead((int)WMX3IO對照.pxeIO_NA05)              == HIGH);
                UIHelper.SetIndicator(lbl取料X前,      indicateRead((int)WMX3IO對照.pxeIO_取料X軸前極限)     == HIGH);
                UIHelper.SetIndicator(lbl_NA_02,       indicateRead((int)WMX3IO對照.pxeIO_NA07)              == HIGH);

                UIHelper.SetIndicator(lbl植針Z後,      indicateRead((int)WMX3IO對照.pxeIO_植針Z軸後極限)     == HIGH);
                UIHelper.SetIndicator(lbl_NA_03,       indicateRead((int)WMX3IO對照.pxeIO_NA11)              == HIGH);
                UIHelper.SetIndicator(lbl植針Z前,      indicateRead((int)WMX3IO對照.pxeIO_植針Z軸前極限)     == HIGH);
                UIHelper.SetIndicator(lbl_NA_04,       indicateRead((int)WMX3IO對照.pxeIO_NA13)              == HIGH);
                UIHelper.SetIndicator(lbl載盤X前,      indicateRead((int)WMX3IO對照.pxeIO_載盤X軸前極限)     == HIGH);
                UIHelper.SetIndicator(lbl_NA_05,       indicateRead((int)WMX3IO對照.pxeIO_NA15)              == HIGH);
                UIHelper.SetIndicator(lbl載盤X後,      indicateRead((int)WMX3IO對照.pxeIO_載盤X軸後極限)     == HIGH);
                UIHelper.SetIndicator(lbl_NA_06,       indicateRead((int)WMX3IO對照.pxeIO_NA17)              == HIGH);

                UIHelper.SetIndicator(lbl載盤空1,      indicateRead((int)WMX3IO對照.pxeIO_載盤真空檢1)       == HIGH);
                UIHelper.SetIndicator(lblsk2空1,       indicateRead((int)WMX3IO對照.pxeIO_Socket2真空檢1)    == HIGH);
                UIHelper.SetIndicator(lbl載盤空2,      indicateRead((int)WMX3IO對照.pxeIO_載盤真空檢2)       == HIGH);
                UIHelper.SetIndicator(lblsk2空2,       indicateRead((int)WMX3IO對照.pxeIO_Socket2真空檢2)    == HIGH);
                UIHelper.SetIndicator(lblsk1空1,       indicateRead((int)WMX3IO對照.pxeIO_Socket1真空檢1)    == HIGH);
                UIHelper.SetIndicator(lbl擺放空1,      indicateRead((int)WMX3IO對照.pxeIO_擺放座真空檢1)     == HIGH);
                UIHelper.SetIndicator(lblsk1空2,       indicateRead((int)WMX3IO對照.pxeIO_Socket1真空檢2)    == HIGH);
                UIHelper.SetIndicator(lbl擺放空2,      indicateRead((int)WMX3IO對照.pxeIO_擺放座真空檢2)     == HIGH);

                UIHelper.SetIndicator(lbl吸嘴空1,      indicateRead((int)WMX3IO對照.pxeIO_吸嘴真空檢1)       == HIGH);
                UIHelper.SetIndicator(lbl_NA_07,       indicateRead((int)WMX3IO對照.pxeIO_NA31)              == HIGH);
                UIHelper.SetIndicator(lbl吸嘴空2,      indicateRead((int)WMX3IO對照.pxeIO_吸嘴真空檢2)       == HIGH);
                UIHelper.SetIndicator(lbl取料ng盒,     indicateRead((int)WMX3IO對照.pxeIO_取料NG收料盒)      == HIGH);
                UIHelper.SetIndicator(lbl兩點壓1,      indicateRead((int)WMX3IO對照.pxeIO_兩點組合壓力檢1)   == HIGH);
                UIHelper.SetIndicator(lbl堵料盒,       indicateRead((int)WMX3IO對照.pxeIO_堵料收料盒)        == HIGH);
                UIHelper.SetIndicator(lbl兩點壓2,      indicateRead((int)WMX3IO對照.pxeIO_兩點組合壓力檢2)   == HIGH);
                UIHelper.SetIndicator(lbl吸料盒,       indicateRead((int)WMX3IO對照.pxeIO_吸料收料盒)        == HIGH);

                UIHelper.SetIndicator(lbl復歸鈕,       indicateRead((int)WMX3IO對照.pxeIO_復歸按鈕)          == HIGH);
                UIHelper.SetIndicator(lbl_NA_08,       indicateRead((int)WMX3IO對照.pxeIO_NA41)              == HIGH);
                UIHelper.SetIndicator(lbl啟動鈕,       indicateRead((int)WMX3IO對照.pxeIO_啟動按鈕)          == HIGH);
                UIHelper.SetIndicator(lbl_NA_09,       indicateRead((int)WMX3IO對照.pxeIO_NA43)              == HIGH);
                UIHelper.SetIndicator(lbl停止鈕,       indicateRead((int)WMX3IO對照.pxeIO_停止按鈕)          == HIGH);
                UIHelper.SetIndicator(lbl_NA_10,       indicateRead((int)WMX3IO對照.pxeIO_NA45)              == HIGH);
                UIHelper.SetIndicator(lbl急停鈕,       indicateRead((int)WMX3IO對照.pxeIO_緊急停止按鈕)      == HIGH);
                UIHelper.SetIndicator(lbl_NA_11,       indicateRead((int)WMX3IO對照.pxeIO_NA47)              == HIGH);

                UIHelper.SetIndicator(lbl_擺放座開,    indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開)      == HIGH);
                UIHelper.SetIndicator(lbl_NA_13,       indicateRead((int)WMX3IO對照.pxeIO_NA51)              == HIGH);
                UIHelper.SetIndicator(lbl_擺放座關,    indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板合)      == HIGH);
                UIHelper.SetIndicator(lbl_NA_15,       indicateRead((int)WMX3IO對照.pxeIO_NA53)              == HIGH);
                UIHelper.SetIndicator(lbl_堵料桿進,    indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿進)      == HIGH);
                UIHelper.SetIndicator(lbl_NA_17,       indicateRead((int)WMX3IO對照.pxeIO_NA55)              == HIGH);
                UIHelper.SetIndicator(lbl_堵料桿出,    indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出)      == HIGH);
                UIHelper.SetIndicator(lbl_NA_19,       indicateRead((int)WMX3IO對照.pxeIO_NA57)              == HIGH);

                UIHelper.SetIndicator(lbl上左右門,     indicateRead((int)WMX3IO對照.pxeIO_上罩左側右門)      == HIGH);
                UIHelper.SetIndicator(lbl上右右門,     indicateRead((int)WMX3IO對照.pxeIO_上罩右側右門)      == HIGH);
                UIHelper.SetIndicator(lbl上左左門,     indicateRead((int)WMX3IO對照.pxeIO_上罩左側左門)      == HIGH);
                UIHelper.SetIndicator(lbl上右左門,     indicateRead((int)WMX3IO對照.pxeIO_上罩右側左門)      == HIGH);
                UIHelper.SetIndicator(lbl上後右門,     indicateRead((int)WMX3IO對照.pxeIO_上罩後側右門)      == HIGH);
                UIHelper.SetIndicator(lbl螢幕小門,     indicateRead((int)WMX3IO對照.pxeIO_螢幕旁小門)        == HIGH);
                UIHelper.SetIndicator(lbl上後左門,     indicateRead((int)WMX3IO對照.pxeIO_上罩後側左門)      == HIGH);
                UIHelper.SetIndicator(lbl_NA_20,       indicateRead((int)WMX3IO對照.pxeIO_NA67)              == HIGH);

                UIHelper.SetIndicator(lbl下左右門,     indicateRead((int)WMX3IO對照.pxeIO_下支架左側右門)    == HIGH);
                UIHelper.SetIndicator(lbl下後左門,     indicateRead((int)WMX3IO對照.pxeIO_下支架後側左門)    == HIGH);
                UIHelper.SetIndicator(lbl下左左門,     indicateRead((int)WMX3IO對照.pxeIO_下支架左側左門)    == HIGH);
                UIHelper.SetIndicator(lbl下後右門,     indicateRead((int)WMX3IO對照.pxeIO_下支架後側右門)    == HIGH);
                UIHelper.SetIndicator(lbl下右右門,     indicateRead((int)WMX3IO對照.pxeIO_下支架右側右門)    == HIGH);
                UIHelper.SetIndicator(lbl_NA_23,       indicateRead((int)WMX3IO對照.pxeIO_NA75)              == HIGH);
                UIHelper.SetIndicator(lbl下右左門,     indicateRead((int)WMX3IO對照.pxeIO_下支架右側左門)    == HIGH);
                UIHelper.SetIndicator(lbl_NA_24,       indicateRead((int)WMX3IO對照.pxeIO_NA76)              == HIGH);
            }  // end of //讀取 Yaskawa InputIO

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
                UIHelper.SetControlProperty(lblVBLED, () => lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value);
            }
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------- Vibration implement --------------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //----------------------------- Flow Valve Control implement ----------------------------
        //---------------------------------------------------------------------------------------
        public void dbapi_FlowValve_植針吹氣(int iValue) { 
            if(iValue>=100) {
                iValue = 100;
            }
            if(iValue<=0) {
                iValue = 0;
            }
            UIHelper.SetControlProperty(vcb_植針吹氣流量閥, () => vcb_植針吹氣流量閥.Value = 100 - iValue);
            vcb流量閥_Scroll(vcb_植針吹氣流量閥, null);
        }
        public void dbapi_FlowValve_吸嘴破真空(int iValue) {
            if(iValue>=100) {
                iValue = 100;
            }
            if(iValue<=0) {
                iValue = 0;
            }
            UIHelper.SetControlProperty(vcb_吸嘴破真空流量閥, () => vcb_吸嘴破真空流量閥.Value = 100 - iValue);
            vcb流量閥_Scroll(vcb_吸嘴破真空流量閥, null);
        }
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
                UIHelper.SetControlProperty(lbl_吸嘴破真空流量閥, () => lbl_吸嘴破真空流量閥.Text = string.Format("{0:F1}", y));
            } else if(vcb流量閥 == vcb_植針吹氣流量閥) {
                UIHelper.SetControlProperty(lbl_植針吹氣流量閥, () => lbl_植針吹氣流量閥.Text = string.Format("{0:F1}", y));
            }

        }
        //---------------------------------------------------------------------------------------
        //----------------------------- Flow Valve Control implement ----------------------------
        //---------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------
        //-------------------------------- State Machine implement ------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void btn_home_Click(object sender, EventArgs e)
        {
            apiIndicator(xeXavier_Indicator.xeXI_事件_復歸);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void btn上膛_Click(object sender, EventArgs e)
        {
            apiIndicator(xeXavier_Indicator.xeXI_狀態_運行);
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
            if (OpenFile(this))
            {
                tsmi_SaveFile.Enabled = true;
                UIHelper.SetControlProperty(btn_SaveFile, () => btn_SaveFile.Enabled = true);

                show_grp_BarcodeInfo(grp_BarcodeInfo);

                find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                // 檢查是否需要在主執行緒上執行
                if (pic_Needles.InvokeRequired) {
                    // 如果是其他執行緒，使用 Invoke 方法
                    pic_Needles.Invoke(new Action( 
                                                        () => {
                                                            pic_Needles.Refresh();
                                                        }
                                                    )
                                        );
                } else {
                    // 如果是在主執行緒，直接執行
                    pic_Needles.Refresh();
                }
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

            // 檢查是否需要在主執行緒上執行
            if (pic_Needles.InvokeRequired) {
                // 如果是其他執行緒，使用 Invoke 方法
                pic_Needles.Invoke(new Action( 
                                                    () => {
                                                        pic_Needles.Refresh();
                                                    }
                                                )
                                    );
            } else {
                // 如果是在主執行緒，直接執行
                pic_Needles.Refresh();
            }
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

            UIHelper.SetControlProperty(lbl_RealMousePos, () => lbl_RealMousePos.Text = "真實座標 : " + RealMousePos.ToString());
            UIHelper.SetControlProperty(lbl_PicMousePos, () => lbl_PicMousePos.Text = "繪圖座標 : " + e.Location.ToString());
            UIHelper.SetControlProperty(lbl_Offset, () => lbl_Offset.Text = "Offset : " + Offset.ToString());
            UIHelper.SetControlProperty(lbl_ZoomFactor, () => lbl_ZoomFactor.Text = "縮放比例 : " + ZoomFactor.ToString());

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

            // 檢查是否需要在主執行緒上執行
            if (pic_Needles.InvokeRequired) {
                // 如果是其他執行緒，使用 Invoke 方法
                pic_Needles.Invoke(new Action( 
                                                    () => {
                                                        pic_Needles.Refresh();
                                                    }
                                                )
                                    );
            } else {
                // 如果是在主執行緒，直接執行
                pic_Needles.Refresh();
            }
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
                        if (IsDrag) {
                            find_Selected_Needles();

                            // 檢查是否需要在主執行緒上執行
                            if (pic_Needles.InvokeRequired) {
                                // 如果是其他執行緒，使用 Invoke 方法
                                pic_Needles.Invoke(new Action( 
                                                                    () => {
                                                                        pic_Needles.Refresh();
                                                                    }
                                                                )
                                                    );
                            } else {
                                // 如果是在主執行緒，直接執行
                                pic_Needles.Refresh();
                            }
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

            // 檢查是否需要在主執行緒上執行
            if (pic_Needles.InvokeRequired) {
                // 如果是其他執行緒，使用 Invoke 方法
                pic_Needles.Invoke(new Action( 
                                                    () => {
                                                        pic_Needles.Refresh();
                                                    }
                                                )
                                    );
            } else {
                // 如果是在主執行緒，直接執行
                pic_Needles.Refresh();
            }
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
                if (sender is TextBox textbox) {
                    Viewer.search_grp_NeedleInfo(textbox.Name, textbox.Text);
                    Viewer.show_grp_NeedleInfo(grp_NeedleInfo);

                    // 檢查是否需要在主執行緒上執行
                    if (pic_Needles.InvokeRequired) {
                        // 如果是其他執行緒，使用 Invoke 方法
                        pic_Needles.Invoke(new Action( 
                                                            () => {
                                                                pic_Needles.Refresh();
                                                            }
                                                        )
                                            );
                    } else {
                        // 如果是在主執行緒，直接執行
                        pic_Needles.Refresh();
                    }
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
            UIHelper.SetControlProperty(btn_SaveFile, () => btn_SaveFile.Enabled = true);

            strFileName = new string(BarcodeBuffer.ToArray()).Trim(); 
            try
            {
                Json = JsonConvert.DeserializeObject<JSON>(File.ReadAllText(@"028\" + strFileName + ".json"));
                show_grp_BarcodeInfo(grp_BarcodeInfo);

                //MessageBox.Show($"檔案 {@"028\" + txt_Barcode.Text + ".json"} 成功讀取！");
                rtb_Status_AppendMessage(rtb_Status, $"檔案 {@"028\" + strFileName + ".json"} 成功讀取！");

                find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                // 檢查是否需要在主執行緒上執行
                if (pic_Needles.InvokeRequired) {
                    // 如果是其他執行緒，使用 Invoke 方法
                    pic_Needles.Invoke(new Action( 
                                                        () => {
                                                            pic_Needles.Refresh();
                                                        }
                                                    )
                                        );
                } else {
                    // 如果是在主執行緒，直接執行
                    pic_Needles.Refresh();
                }
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


        //---------------------------------------------------------------------------------------
        //---------------------------------------- Invoke ---------------------------------------
        //---------------------------------------------------------------------------------------
        void SetEn工作門_Checked(bool value) {
            if (en_工作門.InvokeRequired) {
                en_工作門.Invoke(
                    new Action(
                        () => {
                            en_工作門.Checked = value;
                        }
                    )
                );
            } else {
                en_工作門.Checked = value;
            }
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------- Invoke ---------------------------------------
        //---------------------------------------------------------------------------------------


        #region 暫時或實驗中
        //---------------------------------------------------------------------------------------
        //------------------------------------- 暫時或實驗中 ------------------------------------
        //---------------------------------------------------------------------------------------
        public void button1_Click(object sender, EventArgs e)
        {
            //inspector1.xInit();
        }
        //---------------------------------------------------------------------------------------
        public void btn_socket相機兩點定位_Click(object sender, EventArgs e)
        {
            Vector3 pos;
            bool success = inspector1.xInspSocket校正孔(out pos);
            UIHelper.SetControlProperty(label16, () => label16.Text = string.Format("Socket校正孔 = {0}, X = {1:F3} , Y = {2:F3}", success, pos.X, pos.Y));

            dbCameraCalibrationX = pos.X;
            dbCameraCalibrationY = pos.Y;
        }
        //---------------------------------------------------------------------------------------
        public enum xeXavier_RunType {
            xeXRT_無,
            xeXRT_植針,
            xeXRT_抽針,
            xeXRT_取針丟棄,
        }
        xeXavier_RunType xeXavierRunType = xeXavier_RunType.xeXRT_無;

        public enum xeXavier_Indicator {
            xeXI_讀_狀態,
                xeXI_狀態_運行,
                xeXI_狀態_停止,
                xeXI_狀態_急停,

            xeXI_讀_事件,
            xeXI_事件_空,
                xeXI_事件_復歸,
                xeXI_事件_暫停,
                xeXI_事件_異常,
        }
        xeXavier_Indicator xeXI_Status   = xeXavier_Indicator.xeXI_狀態_停止;
        xeXavier_Indicator xeXI_Event    = xeXavier_Indicator.xeXI_事件_空;
        bool apiIndicator_InternalBTN    = false;
        xeXavier_Indicator xeXI_SaveRslt = xeXavier_Indicator.xeXI_狀態_停止;

        public xeXavier_Indicator apiGetMachineAction() {
            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if (rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                    break;
                case xeXavier_Indicator.xeXI_狀態_停止:
                    break;
                case xeXavier_Indicator.xeXI_狀態_急停:
                    break;

                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;
                case xeXavier_Indicator.xeXI_事件_暫停: break;
                case xeXavier_Indicator.xeXI_事件_異常: break;

                default:
                    return xeXavier_Indicator.xeXI_事件_異常;
            }

            return rslt_event;
        }
        public xeXavier_Indicator apiIndicator(xeXavier_Indicator eEventID) {
            xeXavier_Indicator result = xeXavier_Indicator.xeXI_狀態_停止;

            if (getCommuStatus == 1) {
                //Communication success
            } else {
                //Communication fail
                apiIndicator_InternalBTN = true; {
                    xeXI_Status = xeXavier_Indicator.xeXI_狀態_停止;
                    xeXI_Event  = xeXavier_Indicator.xeXI_事件_空;
                } apiIndicator_InternalBTN = false;

                return result;
            }

            //Check Real Button on Machine
            if(apiIndicator_InternalBTN == false) { 
                bool bBtnEmergencyStop = !indicateRead((int)WMX3IO對照.pxeIO_緊急停止按鈕);
                if(bBtnEmergencyStop == true) {
                    //Emergency Stop
                    apiIndicator_InternalBTN = true; {
                        apiIndicator(xeXavier_Indicator.xeXI_狀態_急停);
                    } apiIndicator_InternalBTN = false;
                } else {
                    //Not Emergency Status

                    //Event
                    bool bBtnPause = false;
                    bool bBtnError = false;
                    if(bBtnError == true) {
                        apiIndicator_InternalBTN = true; {
                            apiIndicator(xeXavier_Indicator.xeXI_事件_異常);
                        } apiIndicator_InternalBTN = false;
                    } else
                    if(bBtnPause == true) {
                        apiIndicator_InternalBTN = true; {
                            apiIndicator(xeXavier_Indicator.xeXI_事件_暫停);
                        } apiIndicator_InternalBTN = false;
                    } else { 
                        bool bBtnStop = indicateRead((int)WMX3IO對照.pxeIO_停止按鈕);
                        if(bBtnStop == true) {
                            apiIndicator_InternalBTN = true; {
                                apiIndicator(xeXavier_Indicator.xeXI_狀態_停止);
                            } apiIndicator_InternalBTN = false;
                        } else
                        if( xeXI_Event  == xeXavier_Indicator.xeXI_事件_空   && 
                            xeXI_Status == xeXavier_Indicator.xeXI_狀態_停止 ) {
                            //Normal Status
                            bool bBtnHome  = indicateRead((int)WMX3IO對照.pxeIO_復歸按鈕);
                            bool bBtnStart = indicateRead((int)WMX3IO對照.pxeIO_啟動按鈕);

                            if(bBtnHome == true) {
                                apiIndicator_InternalBTN = true; {
                                    apiIndicator(xeXavier_Indicator.xeXI_事件_復歸);
                                } apiIndicator_InternalBTN = false;
                            } else 
                            if(bBtnStart == true) {
                                apiIndicator_InternalBTN = true; {
                                    apiIndicator(xeXavier_Indicator.xeXI_狀態_運行);
                                } apiIndicator_InternalBTN = false;
                            }
                        }
                    }
                }

                apiIndicator_InternalBTN = true; {
                    xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
                    if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                        rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
                    }
                    if(xeXI_SaveRslt != rslt_event) {
                        xeXI_SaveRslt = rslt_event;
                        switch (xeXI_SaveRslt) {
                            case xeXavier_Indicator.xeXI_狀態_運行: 
                                digitalWrite((int)WMX3IO對照.pxeIO_面板左按鈕紅燈, LOW);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板中按鈕綠燈, HIGH);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板右按鈕綠燈, LOW);

                                //艙內燈關閉
                                digitalWrite((int)WMX3IO對照.pxeIO_LIGHT, HIGH);

                                eWIndicatorSpeed = eWarningSpeed.xeeWS_狀態_運行;
                                break;
                            case xeXavier_Indicator.xeXI_狀態_停止:
                                digitalWrite((int)WMX3IO對照.pxeIO_面板左按鈕紅燈, HIGH);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板中按鈕綠燈, LOW);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板右按鈕綠燈, LOW);

                                //艙內燈打開
                                digitalWrite((int)WMX3IO對照.pxeIO_LIGHT, LOW);

                                //停止buzzer叫聲
                                digitalWrite((int)WMX3IO對照.pxeIO_Buzzer, LOW);

                                eWIndicatorSpeed = eWarningSpeed.xeeWS_狀態_停止;
                                break;
                            case xeXavier_Indicator.xeXI_狀態_急停:    
                                digitalWrite((int)WMX3IO對照.pxeIO_面板左按鈕紅燈, LOW);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板中按鈕綠燈, LOW);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板右按鈕綠燈, LOW);

                                //艙內燈打開
                                digitalWrite((int)WMX3IO對照.pxeIO_LIGHT, LOW);

                                eWIndicatorSpeed = eWarningSpeed.xeeWS_狀態_急停;
                                break;

                            case xeXavier_Indicator.xeXI_事件_復歸:    
                                digitalWrite((int)WMX3IO對照.pxeIO_面板左按鈕紅燈, LOW);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板中按鈕綠燈, LOW);
                                digitalWrite((int)WMX3IO對照.pxeIO_面板右按鈕綠燈, HIGH);

                                //艙內燈打開
                                digitalWrite((int)WMX3IO對照.pxeIO_LIGHT, LOW);

                                eWIndicatorSpeed = eWarningSpeed.xeeWS_事件_復歸;
                                break;
                            case xeXavier_Indicator.xeXI_事件_暫停:    break;
                            case xeXavier_Indicator.xeXI_事件_異常:    break;
                        }
                    }
                } apiIndicator_InternalBTN = false;
            }  // end of if(apiIndicator_InternalBTN == false) { 

            switch (eEventID) {
                case xeXavier_Indicator.xeXI_讀_狀態:
                    result = xeXI_Status;
                    break;
                case xeXavier_Indicator.xeXI_狀態_運行: xeXI_Status = xeXavier_Indicator.xeXI_狀態_運行;                                                 break;
                case xeXavier_Indicator.xeXI_狀態_停止: xeXI_Status = xeXavier_Indicator.xeXI_狀態_停止; xeXI_Event = xeXavier_Indicator.xeXI_事件_空;   break;
                case xeXavier_Indicator.xeXI_狀態_急停: xeXI_Status = xeXavier_Indicator.xeXI_狀態_急停; xeXI_Event = xeXavier_Indicator.xeXI_事件_空;   break;

                case xeXavier_Indicator.xeXI_讀_事件:
                    result = xeXI_Event;
                    break;
                case xeXavier_Indicator.xeXI_事件_空:   xeXI_Event = xeXavier_Indicator.xeXI_事件_空;                                                    break;
                case xeXavier_Indicator.xeXI_事件_復歸: xeXI_Event = xeXavier_Indicator.xeXI_事件_復歸; xeXI_Status = xeXavier_Indicator.xeXI_狀態_停止; break;
                case xeXavier_Indicator.xeXI_事件_暫停: xeXI_Event = xeXavier_Indicator.xeXI_事件_暫停;                                                  break;
                case xeXavier_Indicator.xeXI_事件_異常: xeXI_Event = xeXavier_Indicator.xeXI_事件_異常; xeXI_Status = xeXavier_Indicator.xeXI_狀態_停止; break;
            }

            return result;
        }
        //---------------------------------------------------------------------------------------
        //------------------------------------- 暫時或實驗中 ------------------------------------
        //---------------------------------------------------------------------------------------
        #endregion

        #region XavierTaskFlowEngine
        //---------------------------------------------------------------------------------------
        //-------------------------------- Xavier TaskFlow Engine -------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public enum xeXavier_FlowTaskISR {
            xeXFTI_tp1_ISR = 0x01,
            xeXFTI_tp2_ISR = 0x02,
            xeXFTI_tp3_ISR = 0x04,
            xeXFTI_tp4_ISR = 0x08,
            xeXFTI_tp5_ISR = 0x10,
            xeXFTI_tp6_ISR = 0x20,
        }

        public enum xeXavier_FlowTask_ISR_ID {
            xeFTII_ISR01 = 0,
            xeFTII_ISR02,
        }

        public struct B8Bits {
            public byte TIFByte;

            public struct BitFields {
                public bool bit0;
                public bool bit1;
                public bool bit2;
                public bool bit3;
                public bool bit4;
                public bool bit5;
                public bool bit6;
                public bool bit7;
            }

            public BitFields bits {
                get {
                    return new BitFields {
                        bit0 = (TIFByte & 0x01) != 0,
                        bit1 = (TIFByte & 0x02) != 0,
                        bit2 = (TIFByte & 0x04) != 0,
                        bit3 = (TIFByte & 0x08) != 0,
                        bit4 = (TIFByte & 0x10) != 0,
                        bit5 = (TIFByte & 0x20) != 0,
                        bit6 = (TIFByte & 0x40) != 0,
                        bit7 = (TIFByte & 0x80) != 0
                    };
                } // end of get
                set {
                    TIFByte = (byte)(
                        (value.bit0 ? 0x01 : 0) |
                        (value.bit1 ? 0x02 : 0) |
                        (value.bit2 ? 0x04 : 0) |
                        (value.bit3 ? 0x08 : 0) |
                        (value.bit4 ? 0x10 : 0) |
                        (value.bit5 ? 0x20 : 0) |
                        (value.bit6 ? 0x40 : 0) |
                        (value.bit7 ? 0x80 : 0)
                    );
                }  // end of set
            }  // end of public BitFields bits
        }  // end of public struct BitFields

        B8Bits TaskISRFlag = new B8Bits();
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //Home Flag
            public bool btp2Home_告知植針軸組可以進行復歸動作                  = false;
            public bool btp2Home_告知吸嘴軸組已回home完畢                      = false;
            public bool btp3Home_告知吸嘴軸組_植針軸組無干涉                   = false;
            public bool btp3Home_告知載盤組_植針軸組無干涉                     = false;
            public bool btp3Home_告知植針軸組已回home完畢                      = false;
            public bool btp4Home_告知載盤組_電動缸無干涉                       = false;
            public bool btp4Home_告知電動缸組已回home完畢                      = false;
            public bool btp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作   = false;
            public bool btp5Home_告知載盤組已回home完畢                        = false;

            public bool btp6Home_告知工作門已關閉                              = false;

            public bool btp6Home_告知系統回home完畢                            = false;

        //植針動作 Flag
            public bool btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照      = false;
            public bool btp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標   = false;
          //public bool btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照_再次 = false;
          //public bool btp2Insert_ISR_告知取得吸針嘴組R軸                     = false;
            public bool btp3Insert_告知系統賭料排除異常_告知系統中止           = false;
            public bool btp2Insert_告知植針軸組可以進行放料作業                = false;
            public bool btp3Insert_告知載盤組_植針軸組無干涉                   = false;
            public bool btp3Insert_告知吸嘴軸組可以放物料                      = false;
            public bool btp3Insert_告知吸嘴軸組_植針軸放料完成                 = false;
            public bool btp3Insert_告知載盤組_植針軸植針完畢                   = false;
            public bool btp3Insert_告知載盤組進行補光                          = false;
            public bool btp3Insert_告知完成植針嘴堵料拍照                      = false;
            public bool btp3Insert_告知植針軸組判斷堵料                        = false;
            public bool btp3Insert_告知植針軸組堵料吹氣完畢                    = false;
            public bool btp3Insert_告知植針軸組判斷未堵料                      = false;
            public bool btp4Insert_告知載盤組_電動缸無干涉                     = false;
            public bool btp4Insert_告知Socket孔檢測相機已至拍照位              = false;
            public bool btp4Insert_告知堵料檢查植針嘴相機已至拍照位            = false;
            public bool btp4Insert_柔震盤物料異常_告知系統中止                 = false;
            public bool btp4Insert_告知吸嘴軸組柔震盤物料座標                  = false;
            public bool btp5Insert_告知檔案組已完成兩點校正                    = false;
            public bool btp5Insert_告知植針軸組載盤組已移至植針位              = false;
            public bool btp5Insert_告知系統植針成功_To_Tp6                     = false;
            public bool btp5Insert_告知系統植針成功_To_Tp3                     = false;
            public bool btp5Insert_告知系統植針失敗                            = false;                             
            public bool btp5Insert_植針異常停止_告知系統停止                   = false;
            public bool btp5Insert_告知載盤組已至補光位                        = false;
            public bool btp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位       = false;
            public bool btp6Insert_告知載盤組已拿到兩點校正資料                = false;
            public bool btp6Insert_告知系統無目標植針資料_To_Tp3               = false;               
            public bool btp6Insert_告知系統無目標植針資料_To_Tp5               = false;               
            public bool btp6Insert_告知系統無目標植針資料_To_Tp4               = false;               
            public bool btp6Insert_告知系統無目標植針資料_To_Tp2               = false;               
            public bool btp6Insert_告知系統已拿到目標植針資料_To_Tp3           = false;
            public bool btp6Insert_告知系統已拿到目標植針資料_To_Tp5           = false;
            public bool btp6Insert_告知系統已拿到目標植針資料_To_Tp4           = false;
            public bool btp6Insert_告知系統已拿到目標植針資料_To_Tp2           = false;                            
            public bool btp6Insert_清除告知系統已拿到目標植針資料              = false;

        //DeadLock Task3 Task5
            volatile public int iTask3_CNT = 0;
            volatile public int iTask5_CNT = 0;

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void CleanAllBoolFlag() { 
            //Home Flag
                 btp2Home_告知植針軸組可以進行復歸動作                  = false;
                 btp2Home_告知吸嘴軸組已回home完畢                      = false;
                 btp3Home_告知吸嘴軸組_植針軸組無干涉                   = false;
                 btp3Home_告知載盤組_植針軸組無干涉                     = false;
                 btp3Home_告知植針軸組已回home完畢                      = false;
                 btp4Home_告知載盤組_電動缸無干涉                       = false;
                 btp4Home_告知電動缸組已回home完畢                      = false;
                 btp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作   = false;
                 btp5Home_告知載盤組已回home完畢                        = false;

                 btp6Home_告知工作門已關閉                              = false;

               //btp6Home_告知系統回home完畢                            = false;

            //植針動作 Flag
                 btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照      = false;
                 btp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標   = false;
               //btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照_再次 = false;
               //btp2Insert_ISR_告知取得吸針嘴組R軸                     = false;
                 btp3Insert_告知系統賭料排除異常_告知系統中止           = false;
                 btp2Insert_告知植針軸組可以進行放料作業                = false;
                 btp3Insert_告知載盤組_植針軸組無干涉                   = false;
                 btp3Insert_告知吸嘴軸組可以放物料                      = false;
                 btp3Insert_告知吸嘴軸組_植針軸放料完成                 = false;
                 btp3Insert_告知載盤組_植針軸植針完畢                   = false;
                 btp3Insert_告知載盤組進行補光                          = false;
                 btp3Insert_告知完成植針嘴堵料拍照                      = false;
                 btp3Insert_告知植針軸組判斷堵料                        = false;
                 btp3Insert_告知植針軸組堵料吹氣完畢                    = false;
                 btp3Insert_告知植針軸組判斷未堵料                      = false;
                 btp4Insert_告知載盤組_電動缸無干涉                     = false;
                 btp4Insert_告知Socket孔檢測相機已至拍照位              = false;
                 btp4Insert_告知堵料檢查植針嘴相機已至拍照位            = false;
                 btp4Insert_柔震盤物料異常_告知系統中止                 = false;
                 btp4Insert_告知吸嘴軸組柔震盤物料座標                  = false;
                 btp5Insert_告知檔案組已完成兩點校正                    = false;
                 btp5Insert_告知植針軸組載盤組已移至植針位              = false;
                 btp5Insert_告知系統植針成功_To_Tp6                     = false;
                 btp5Insert_告知系統植針成功_To_Tp3                     = false;
                 btp5Insert_告知系統植針失敗                            = false;                             
                 btp5Insert_植針異常停止_告知系統停止                   = false;
                 btp5Insert_告知載盤組已至補光位                        = false;
                 btp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位       = false;
                 btp6Insert_告知載盤組已拿到兩點校正資料                = false;
                 btp6Insert_告知系統無目標植針資料_To_Tp3               = false;               
                 btp6Insert_告知系統無目標植針資料_To_Tp5               = false;               
                 btp6Insert_告知系統無目標植針資料_To_Tp4               = false;               
                 btp6Insert_告知系統無目標植針資料_To_Tp2               = false;               
                 btp6Insert_告知系統已拿到目標植針資料_To_Tp3           = false;
                 btp6Insert_告知系統已拿到目標植針資料_To_Tp5           = false;
                 btp6Insert_告知系統已拿到目標植針資料_To_Tp4           = false;
                 btp6Insert_告知系統已拿到目標植針資料_To_Tp2           = false;                            
                 btp6Insert_清除告知系統已拿到目標植針資料              = false;
        }
        //---------------------------------------------------------------------------------------

        #region XavierTaskFlowEngine
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public uint u32HomeDelayCNT   = 5;
        public uint u32InsertDelayCNT = 3;
        public uint u32ISRDelayCNT    = 1;

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task_Eng_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task_Info, () => lbldbg_Task_Info.Text = message);

            XavierLogger.Log("Eng", message);
        }
        //---------------------------------------------------------------------------------------
        public void Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR CallTask, xeXavier_FlowTask_ISR_ID isrID) {
        
            var tempBits = TaskISRFlag.bits;
                switch(CallTask) {
                    case xeXavier_FlowTaskISR.xeXFTI_tp1_ISR:
                        if(tempBits.bit0 == false) {  //force to tp1_ISR_START
                            tempBits.bit0 = true;
                            switch(isrID) {
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR01:  Task1CallJob(xeXavier_T1_Job.tp1_ISR01_START);  break;
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR02:  Task1CallJob(xeXavier_T1_Job.tp1_ISR02_START);  break;
                            }
                            Xavier_Task_Eng_Debugprintf("Set xeXFTI_tp1_ISR");
                        }
                        break;
                    
                    case xeXavier_FlowTaskISR.xeXFTI_tp2_ISR:
                        if(tempBits.bit1 == false) {  //force to tp2_ISR_START
                            tempBits.bit1 = true;
                            switch(isrID) {
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR01:  Task2CallJob(xeXavier_T2_Job.tp2_ISR01_START);  break;
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR02:  Task2CallJob(xeXavier_T2_Job.tp2_ISR02_START);  break;
                            }
                            Xavier_Task_Eng_Debugprintf("Set xeXFTI_tp2_ISR");
                        }
                        break;
                    
                    case xeXavier_FlowTaskISR.xeXFTI_tp3_ISR:
                        if(tempBits.bit2 == false) {  //force to tp3_ISR_START
                            tempBits.bit2 = true;
                            switch(isrID) {
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR01:  Task3CallJob(xeXavier_T3_Job.tp3_ISR01_START);  break;
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR02:  Task3CallJob(xeXavier_T3_Job.tp3_ISR02_START);  break;
                            }
                            Xavier_Task_Eng_Debugprintf("Set xeXFTI_tp3_ISR");
                        }
                        break;
                    
                    case xeXavier_FlowTaskISR.xeXFTI_tp4_ISR:
                        if(tempBits.bit3 == false) {  //force to tp4_ISR_START
                            tempBits.bit3 = true;
                            switch(isrID) {
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR01:  Task4CallJob(xeXavier_T4_Job.tp4_ISR01_START);  break;
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR02:  Task4CallJob(xeXavier_T4_Job.tp4_ISR02_START);  break;
                            }
                            Xavier_Task_Eng_Debugprintf("Set xeXFTI_tp4_ISR");
                        }
                        break;
                    
                    case xeXavier_FlowTaskISR.xeXFTI_tp5_ISR:
                        if(tempBits.bit4 == false) {  //force to tp5_ISR_START
                            tempBits.bit4 = true;
                            switch(isrID) {
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR01:  Task5CallJob(xeXavier_T5_Job.tp5_ISR01_START);  break;
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR02:  Task5CallJob(xeXavier_T5_Job.tp5_ISR02_START);  break;
                            }
                            Xavier_Task_Eng_Debugprintf("Set xeXFTI_tp5_ISR");
                        }
                        break;

                    case xeXavier_FlowTaskISR.xeXFTI_tp6_ISR:
                        if(tempBits.bit5 == false) {  //force to tp6_ISR_START
                            tempBits.bit5 = true;
                            switch(isrID) {
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR01:  Task6CallJob(xeXavier_T6_Job.tp6_ISR01_START);  break;
                                case xeXavier_FlowTask_ISR_ID.xeFTII_ISR02:  Task6CallJob(xeXavier_T6_Job.tp6_ISR02_START);  break;
                            }
                            Xavier_Task_Eng_Debugprintf("Set xeXFTI_tp6_ISR");
                        }
                        break;

                    default:
                        tempBits.bit0 = false;
                        tempBits.bit1 = false;
                        tempBits.bit2 = false;
                        tempBits.bit3 = false;
                        tempBits.bit4 = false;
                        tempBits.bit5 = false;
                        Xavier_Task_Eng_Debugprintf("Set Other ISR, will clear");
                        break;
                }
            TaskISRFlag.bits = tempBits;
            
                Xavier_Task_Eng_Debugprintf(string.Format(":{0:X2}\r\n", TaskISRFlag.TIFByte));
        }
        //---------------------------------------------------------------------------------------
        public void Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR CallTask) {

            var tempBits = TaskISRFlag.bits;
                switch(CallTask) {
                    case xeXavier_FlowTaskISR.xeXFTI_tp1_ISR:
                        if(tempBits.bit0 == true) {  //force to tp1_ISR_START
                            tempBits.bit0 = false;
                            Xavier_Task_Eng_Debugprintf("Res ISR");
                        }
                        break;
                
                    case xeXavier_FlowTaskISR.xeXFTI_tp2_ISR:
                        if(tempBits.bit1 == true) {  //force to tp2_ISR_START
                            tempBits.bit1 = false;
                            Xavier_Task_Eng_Debugprintf("Res ISR");
                        }
                        break;
                
                    case xeXavier_FlowTaskISR.xeXFTI_tp3_ISR:
                        if(tempBits.bit2 == true) {  //force to tp3_ISR_START
                            tempBits.bit2 = false;
                            Xavier_Task_Eng_Debugprintf("Res ISR");
                        }
                        break;
                    
                    case xeXavier_FlowTaskISR.xeXFTI_tp4_ISR:
                        if(tempBits.bit3 == true) {  //force to tp4_ISR_START
                            tempBits.bit3 = false;
                            Xavier_Task_Eng_Debugprintf("Res ISR");
                        }
                        break;

                    case xeXavier_FlowTaskISR.xeXFTI_tp5_ISR:
                        if(tempBits.bit4 == true) {  //force to tp5_ISR_START
                            tempBits.bit4 = false;
                            Xavier_Task_Eng_Debugprintf("Res ISR");
                        }
                        break;                

                    case xeXavier_FlowTaskISR.xeXFTI_tp6_ISR:
                        if(tempBits.bit5 == true) {  //force to tp6_ISR_START
                            tempBits.bit5 = false;
                            Xavier_Task_Eng_Debugprintf("Res ISR");
                        }
                        break;

                    default:
                        tempBits.bit0 = false;
                        tempBits.bit1 = false;
                        tempBits.bit2 = false;
                        tempBits.bit3 = false;
                        tempBits.bit4 = false;
                        tempBits.bit5 = false;
                        Xavier_Task_Eng_Debugprintf("Res Other ISR, will clear");
                        break;
                }
            TaskISRFlag.bits = tempBits;
        
                Xavier_Task_Eng_Debugprintf(string.Format(":{0:X2}\r\n", TaskISRFlag.TIFByte));
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        #endregion

        #region XavierTaskFlowEngine_T1
        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T1 -------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Global Variables----------
        public static uint Xavier_T1_dC_decdelayCNT  = 0;
        public static xeXavier_T1_Job Xavier_T1_dC_GetInJob     = 0;
        public static xeXavier_T1_Job Xavier_Task1_p_ret        = 0;
        public static xeXavier_T1_Job Xavier_Task1_ISR_JT_retmp = xeXavier_T1_Job.tp1_ISR01_START;
        public static xeXavier_T1_Job Xavier_Task1_ISR_CT_retmp = xeXavier_T1_Job.tp1_ISR01_START;

        // ----------Enumerations----------
        public enum xeXavier_T1_proc {
            pt1SET = 1,
            pt1GET,
            pt1Interrupt,
            pt1ResISR,
            pt1deExcute,
        }

        public enum xeXavier_T1_Job {
            tp1Empty = 0,
            tp1Init,
            
            tp1_ISR01_START,
            tp1_ISR01_STEP1,
            tp1_ISR01_STEP2,
            tp1_ISR01_END,

            tp1_ISR02_START,
            tp1_ISR02_STEP1,
            tp1_ISR02_STEP2,
            tp1_ISR02_END,
            
            tp1Idle,
            tp1START,
            tp1STEP1,
            tp1STEP2,
            tp1STEP3,
            tp1STEP4,
            tp1STEP5,
            tp1STEP6,
            tp1STEP7,
        }

        // --------- Local Variables ----------
        enum eWarningSpeed {
            xeeWS_Disable,

            xeeWS_RedConstant,
            xeeWS_RedLowSpeed,
            xeeWS_RedHighSpeed,

            xeeWS_YellowConstant,
            xeeWS_YellowLowSpeed,
            xeeWS_YellowHighSpeed,

            xeeWS_GreenConstant,
            xeeWS_GreenLowSpeed,
            xeeWS_GreenHighSpeed,

            xeeWS_事件_復歸 = xeeWS_GreenLowSpeed,   //綠閃
            xeeWS_事件_暫停 = xeeWS_YellowLowSpeed,  //黃閃
            xeeWS_事件_異常 = xeeWS_RedLowSpeed,     //紅閃

            xeeWS_狀態_運行 = xeeWS_GreenConstant,   //綠
            xeeWS_狀態_停止 = xeeWS_YellowConstant,  //黃
            xeeWS_狀態_急停 = xeeWS_RedConstant,     //紅
        }; 
        eWarningSpeed eWIndicatorSpeed = eWarningSpeed.xeeWS_狀態_停止;

        int iWarningLEDCnt  = 0;
        bool bWarningRed    = false,
             bWarningYellow = false,
             bWarningGreen  = false;

        // ----------Methods----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_TASK1() {  //面板按鈕以及指示燈
            xeXavier_T1_Job priTASK = 0;
            Xavier_T1_delayCase(xeXavier_T1_proc.pt1deExcute, (uint)xeXavier_T1_Job.tp1Empty, xeXavier_T1_Job.tp1Empty);
            priTASK = Xavier_Task1_proc(xeXavier_T1_proc.pt1GET, 0);

            switch (priTASK) {
                case xeXavier_T1_Job.tp1Empty:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1Init);
                    Xavier_Task1_Debugprintf("tp1Empty\r\n");
                    break;

                case xeXavier_T1_Job.tp1Init:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1START);
                    Xavier_Task1_Debugprintf("tp1Init\r\n");
                    break;

                //======ISR Job======
                case xeXavier_T1_Job.tp1_ISR01_START:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1_ISR01_STEP1);
                    Xavier_Task1_Debugprintf("tp1_ISR01_START\r\n");
                    break;

                case xeXavier_T1_Job.tp1_ISR01_STEP1:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1_ISR01_STEP2);
                    Xavier_Task1_Debugprintf("tp1_ISR01_STEP1\r\n");
                    break;

                case xeXavier_T1_Job.tp1_ISR01_STEP2:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1_ISR01_END);
                    Xavier_Task1_Debugprintf("tp1_ISR01_STEP2\r\n");
                    break;

                case xeXavier_T1_Job.tp1_ISR01_END:
                    //Xavier_Task1_ISR_CNTTmp(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT);
                    //Xavier_Task1_ISR_JobTmp(xeXavier_T1_proc.pt1SET, xeXavier_T1_Job.tp1STEP2);

                    Task1ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp1_ISR);
                    Xavier_Task1_Debugprintf("tp1_ISR01_end\r\n");
                    break;
                //======ISR Job======
                case xeXavier_T1_Job.tp1_ISR02_START:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1_ISR02_STEP1);
                    Xavier_Task1_Debugprintf("tp1_ISR02_START\r\n");
                    break;

                case xeXavier_T1_Job.tp1_ISR02_STEP1:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1_ISR02_STEP2);
                    Xavier_Task1_Debugprintf("tp1_ISR02_STEP1\r\n");
                    break;

                case xeXavier_T1_Job.tp1_ISR02_STEP2:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1_ISR02_END);
                    Xavier_Task1_Debugprintf("tp1_ISR02_STEP2\r\n");
                    break;

                case xeXavier_T1_Job.tp1_ISR02_END:
                    //Xavier_Task1_ISR_CNTTmp(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT);
                    //Xavier_Task1_ISR_JobTmp(xeXavier_T1_proc.pt1SET, xeXavier_T1_Job.tp1STEP5);
                    
                    Task1ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp1_ISR);
                    Xavier_Task1_Debugprintf("tp1_ISR02_end\r\n");
                    break;
                //======ISR Job======
                
                case xeXavier_T1_Job.tp1Idle:  //reserve
                    break;

                case xeXavier_T1_Job.tp1START: {
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP1);
                    Xavier_Task1_Debugprintf("tp1START\r\n");
                } break;

                case xeXavier_T1_Job.tp1STEP1:
                    apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);

                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP2);
                    Xavier_Task1_Debugprintf("tp1STEP1\r\n");
                    break;

                case xeXavier_T1_Job.tp1STEP2: {

                    int iWarningLEDSpeed = 0;
                    switch (eWIndicatorSpeed) {
                        case eWarningSpeed.xeeWS_Disable:
                            iWarningLEDCnt = 0;
                            digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                            break;

                        case eWarningSpeed.xeeWS_RedConstant:
                            digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, HIGH);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                            break;
                        case eWarningSpeed.xeeWS_YellowConstant:
                            digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, HIGH);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                            break;
                        case eWarningSpeed.xeeWS_GreenConstant:
                            digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                            digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, HIGH);
                            break;

                        case eWarningSpeed.xeeWS_RedLowSpeed:   iWarningLEDSpeed = 3;  goto lbl_WarningRED;
                        case eWarningSpeed.xeeWS_RedHighSpeed:  iWarningLEDSpeed = 1;  goto lbl_WarningRED;
                            lbl_WarningRED: {
                                iWarningLEDCnt++;
                                if(iWarningLEDCnt>=iWarningLEDSpeed) {
                                    iWarningLEDCnt = 0;

                                    bWarningRed = !bWarningRed;
                                }

                                if (bWarningRed == false) {
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                                } else {
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, HIGH);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                                }
                            } break;

                        case eWarningSpeed.xeeWS_YellowLowSpeed:   iWarningLEDSpeed = 3;  goto lbl_WarningYellow;
                        case eWarningSpeed.xeeWS_YellowHighSpeed:  iWarningLEDSpeed = 1;  goto lbl_WarningYellow; 
                            lbl_WarningYellow: {
                                iWarningLEDCnt++;
                                if(iWarningLEDCnt>=iWarningLEDSpeed) {
                                    iWarningLEDCnt = 0;

                                    bWarningYellow = !bWarningYellow;
                                }

                                if (bWarningYellow == false) {
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                                } else {
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, HIGH);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                                }
                            } break;

                        case eWarningSpeed.xeeWS_GreenLowSpeed:   iWarningLEDSpeed = 3;  goto lbl_WarningGreen;
                        case eWarningSpeed.xeeWS_GreenHighSpeed:  iWarningLEDSpeed = 1;  goto lbl_WarningGreen; 
                            lbl_WarningGreen: {
                                iWarningLEDCnt++;
                                if(iWarningLEDCnt>=iWarningLEDSpeed) {
                                    iWarningLEDCnt = 0;

                                    bWarningGreen = !bWarningGreen;
                                }

                                if (bWarningGreen == false) {
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, LOW);
                                } else {
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台紅燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台黃燈, LOW);
                                    digitalWrite((int)WMX3IO對照.pxeIO_機台綠燈, HIGH);
                                }
                            } break;
                    }  // end of switch (eWIndicatorSpeed) {

                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP3);
                    Xavier_Task1_Debugprintf("tp1STEP2\r\n");
                } break;

                case xeXavier_T1_Job.tp1STEP3:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP4);
                    Xavier_Task1_Debugprintf("tp1STEP3\r\n");
                    break;

                case xeXavier_T1_Job.tp1STEP4:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP5);
                    Xavier_Task1_Debugprintf("tp1STEP4\r\n");
                    break;

                case xeXavier_T1_Job.tp1STEP5:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP6);
                    Xavier_Task1_Debugprintf("tp1STEP5\r\n");
                    break;

                case xeXavier_T1_Job.tp1STEP6:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP7);
                    Xavier_Task1_Debugprintf("tp1STEP6\r\n");
                    break;

                case xeXavier_T1_Job.tp1STEP7:
                    Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, u32ISRDelayCNT, xeXavier_T1_Job.tp1STEP1);
                    Xavier_Task1_Debugprintf("tp1STEP1\r\n");
                    break;

                default:
                    break;
            }

            Xavier_Task1_proc(xeXavier_T1_proc.pt1SET, xeXavier_T1_Job.tp1Idle);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_T1_delayCase(xeXavier_T1_proc deJob, uint delayCNT, xeXavier_T1_Job excuteJob) {
            switch (deJob) {
                case xeXavier_T1_proc.pt1SET:
                    Xavier_T1_dC_decdelayCNT = delayCNT + 2;
                    Xavier_T1_dC_GetInJob = excuteJob;
                    break;

                case xeXavier_T1_proc.pt1Interrupt:
                    if (Xavier_T1_dC_GetInJob != excuteJob) {
                        Xavier_Task1_ISR_CNTTmp(xeXavier_T1_proc.pt1SET, (xeXavier_T1_Job)Xavier_T1_dC_decdelayCNT);
                        Xavier_Task1_ISR_JobTmp(xeXavier_T1_proc.pt1SET, Xavier_T1_dC_GetInJob);

                        Xavier_T1_dC_GetInJob = excuteJob;
                        Xavier_T1_dC_decdelayCNT = 2;  // equal to excute pt1deExcute to get Xavier_Task1_proc(pt1SET,GetInJob);
                    }
                    break;

                case xeXavier_T1_proc.pt1ResISR:
                    Xavier_T1_dC_decdelayCNT = (uint)Xavier_Task1_ISR_CNTTmp(xeXavier_T1_proc.pt1GET, Xavier_T1_dC_GetInJob) + 2;
                    Xavier_T1_dC_GetInJob    =       Xavier_Task1_ISR_JobTmp(xeXavier_T1_proc.pt1GET, Xavier_T1_dC_GetInJob);

                    Xavier_Task1_ISR_CNTTmp(xeXavier_T1_proc.pt1SET, (xeXavier_T1_Job)2);
                    Xavier_Task1_ISR_JobTmp(xeXavier_T1_proc.pt1SET, xeXavier_T1_Job.tp1Empty);
                    break;

                case xeXavier_T1_proc.pt1deExcute:
                    if (Xavier_T1_dC_decdelayCNT > 0) {
                        Xavier_T1_dC_decdelayCNT--;
                    }

                    if (Xavier_T1_dC_decdelayCNT == 1) {
                        Xavier_Task1_proc(xeXavier_T1_proc.pt1SET, Xavier_T1_dC_GetInJob);
                    }
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T1_Job Xavier_Task1_proc(xeXavier_T1_proc rtFun, xeXavier_T1_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T1_proc.pt1SET:
                    Xavier_Task1_p_ret = ptValue;
                    break;

                case xeXavier_T1_proc.pt1GET:
                    break;
            }

            return Xavier_Task1_p_ret;
        }
        //---------------------------------------------------------------------------------------
        public void Task1CallJob(xeXavier_T1_Job excuteJob) {
            Xavier_T1_delayCase(xeXavier_T1_proc.pt1Interrupt, 0, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task1CallJobWithDelay(xeXavier_T1_Job excuteJob, uint delayCNT) {
            Xavier_T1_delayCase(xeXavier_T1_proc.pt1SET, delayCNT, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task1ResumeJob() {
            Xavier_T1_delayCase(xeXavier_T1_proc.pt1ResISR, 0, 0);
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T1_Job Xavier_Task1_ISR_JobTmp(xeXavier_T1_proc rtFun, xeXavier_T1_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T1_proc.pt1SET:
                    Xavier_Task1_ISR_JT_retmp = ptValue;
                    break;

                case xeXavier_T1_proc.pt1GET:
                    break;
            }

            return Xavier_Task1_ISR_JT_retmp;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T1_Job Xavier_Task1_ISR_CNTTmp(xeXavier_T1_proc rtFun, xeXavier_T1_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T1_proc.pt1SET:
                    Xavier_Task1_ISR_CT_retmp = ptValue;
                    break;

                case xeXavier_T1_proc.pt1GET:
                    break;
            }

            return Xavier_Task1_ISR_CT_retmp;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Debug Method----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task1_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task1_Info, () => lbldbg_Task1_Info.Text = message);

            XavierLogger.Log("Task1", message);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T1 -------------------------------
        //---------------------------------------------------------------------------------------
        #endregion



        #region XavierTaskFlowEngine_T2
        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T2 -------------------------------
        //---------------------------------------------------------------------------------------

        // ---------Private Variables----------
        public double dbVelocityNozzleX, 
                      dbVelocityNozzleY, 
                      dbVelocityNozzleZ, 
                      dbVelocityNozzleR;

        public const double db取料Nozzle中心點X = 49.93;
        public const double db取料Nozzle中心點Y = 49.81;
        public const double db取料Nozzle中心點Z = 26;
        public const double db取料Nozzle中心點R = 1.34+0.7;
        public const double db吐料位下降Z高度   = 2.000;

        public const double db下視覺取像X_Start = 105;
        public const double db下視覺取像X_END   = 243.000;
        public const double db下視覺取像Y       = 27.05;
        public const double db下視覺取像Z       = 0;

        // ----------Global Variables----------
        public static uint Xavier_T2_dC_decdelayCNT  = 0;
        public static xeXavier_T2_Job Xavier_T2_dC_GetInJob     = 0;
        public static xeXavier_T2_Job Xavier_Task2_p_ret        = 0;
        public static xeXavier_T2_Job Xavier_Task2_ISR_JT_retmp = xeXavier_T2_Job.tp2_ISR01_START;
        public static xeXavier_T2_Job Xavier_Task2_ISR_CT_retmp = xeXavier_T2_Job.tp2_ISR01_START;

        // ----------Enumerations----------
        public enum xeXavier_T2_proc {
            pt2SET = 1,
            pt2GET,
            pt2Interrupt,
            pt2ResISR,
            pt2deExcute,
        }

        public enum xeXavier_T2_Job {
            tp2Empty = 0,
            tp2Init,
            
            tp2_ISR01_START,
                tp2Insert_ISR_飛拍成功_01,
                tp2Insert_ISR_告知取得吸針嘴組R軸,
            tp2_ISR01_END,

            tp2_ISR02_START,
                tp2Insert_ISR_飛拍失敗_02,
                tp2Insert_ISR_吸嘴Z縮回0保護,
                tp2Insert_ISR_吸嘴軸組XYR移動至吐料位,
                tp2Insert_ISR_吸嘴軸組Z下降至吐料位,
                tp2Insert_ISR_吸嘴軸組吐料前準備作業,
                tp2Insert_ISR_吸嘴軸組吐料作業,
                tp2Insert_ISR_吸嘴Z縮回0,
                tp2Insert_ISR_吸嘴XYR回home保護位,
                tp2Insert_ISR_吸嘴軸組吐料完畢,
                tp2Insert_ISR_跳回_至_tp2Insert_取針前動作準備,
            tp2_ISR02_END,
            
            tp2Idle,
            tp2START,  //判斷動作種類

            //吸嘴軸組
            tp2HomeSTART,
                tp2Home_確認吸嘴軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉,
                tp2Home_吸嘴Z縮回0,
                tp2Home_告知植針軸組可以進行復歸動作,
                tp2Home_吸嘴XYR回home_從_tp3Home_告知吸嘴軸組_植針軸組無干涉,
                tp2Home_吸嘴Z回home,
                tp2Home_告知吸嘴軸組已回home完畢,

            tp2TakeAndDiscardSTART,

            tp2InsertSTART,
                tp2Insert_取針前動作準備,
                tp2Insert_確認進行取針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料,
                tp2Insert_有植針資料,                                                    tp2Insert_無植針資料,
                tp2Insert_確認吸嘴軸不在柔震上方遮住相機,
                tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照,
                tp2Insert_取得柔震物料座標_從_tp4Insert_告知吸嘴軸組柔震盤物料座標,
                tp2Insert_吸嘴軸組XYR移動至物料座標,
                tp2Insert_吸嘴軸組Z下降前準備作業,
                tp2Insert_吸嘴軸組Z下降至取料位,
              //tp2Insert_吸嘴軸組Z下降完畢,
              //tp2Insert_吸嘴軸組取料作業,
                tp2Insert_吸嘴軸組ZR上升至安全位,
                tp2Insert_吸嘴軸組XY移動至飛拍準備位,
                tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標,
                tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照_再次,
                tp2Insert_吸嘴軸組X觸發移動飛拍,
                tp2Insert_進行植針軸組放料位檢查,
                tp2Insert_判斷值針軸組是否可以放置物料_從_tp3Insert_告知吸嘴軸組可以放物料,
                tp2Insert_無法放置物料,                                   tp2Insert_可以放置物料,
                tp2Insert_移動到植針軸組前等待,                           tp2Insert_吸嘴軸組R至放料位_從_tp2Insert_ISR_告知取得吸針嘴組R軸,
                tp2Insert_跳回_至_tp2Insert_進行植針軸組放料位檢查,       tp2Insert_移至植針軸組上方放料位,
                                                                          tp2Insert_吸嘴軸組Z下降至放料前準備作業,
                                                                          tp2Insert_吸嘴軸組Z下降至放料位,
                                                                          tp2Insert_吸嘴軸組Z下降放料完畢,
                                                                          tp2Insert_吸嘴軸組放料作業,
                                                                          tp2Insert_告知植針軸組可以進行放料作業,
                                                                          tp2Insert_吸嘴軸組放料完成_從_tp3Insert_告知吸嘴軸組_植針軸放料完成,
                                                                          tp2Insert_吸嘴Z縮回0,
                                                                        //tp2Insert_吸嘴XYR回home保護位,
                                                                        //tp2Insert_跳回_至_tp2Insert_取針前動作準備,
                                                                          tp2Insert_吸嘴軸動作完成,

            tp2RemoveSTART,
        }

        // ----------Methods----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_TASK2() {  //吸嘴軸組
            xeXavier_T2_Job priTASK = 0;
            Xavier_T2_delayCase(xeXavier_T2_proc.pt2deExcute, (uint)xeXavier_T2_Job.tp2Empty, xeXavier_T2_Job.tp2Empty);
            priTASK = Xavier_Task2_proc(xeXavier_T2_proc.pt2GET, 0);

            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;

                case xeXavier_Indicator.xeXI_狀態_停止:
                case xeXavier_Indicator.xeXI_狀態_急停:
                case xeXavier_Indicator.xeXI_事件_暫停:
                case xeXavier_Indicator.xeXI_事件_異常: 
                default:
                    priTASK = xeXavier_T2_Job.tp2START;
                    break;
            }

            switch (priTASK) {
                case xeXavier_T2_Job.tp2Empty:
                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Init);
                    Xavier_Task2_Debugprintf("tp2Empty\r\n");
                    break;

                case xeXavier_T2_Job.tp2Init:
                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2START);
                    Xavier_Task2_Debugprintf("tp2Init\r\n");
                    break;

                //======ISR Job======
                case xeXavier_T2_Job.tp2_ISR01_START:
                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_飛拍成功_01);
                    Xavier_Task2_Debugprintf("tp2_ISR01_START\r\n");
                    break;

                    case xeXavier_T2_Job.tp2Insert_ISR_飛拍成功_01:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_告知取得吸針嘴組R軸);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_飛拍成功_01\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_告知取得吸針嘴組R軸:
                        {
                            //btp2Insert_ISR_告知取得吸針嘴組R軸 = true;

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2_ISR01_END);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_告知取得吸針嘴組R軸\r\n");
                        break;

                case xeXavier_T2_Job.tp2_ISR01_END:
                    //Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT);
                    //Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc.pt2SET, xeXavier_T2_Job.tp2STEP2);

                    Task2ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR);
                    Xavier_Task2_Debugprintf("tp2_ISR01_end\r\n");
                    break;
                //======ISR Job======
                case xeXavier_T2_Job.tp2_ISR02_START:
                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_飛拍失敗_02);
                    Xavier_Task2_Debugprintf("tp2_ISR02_START\r\n");
                    break;

                    case xeXavier_T2_Job.tp2Insert_ISR_飛拍失敗_02:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴Z縮回0保護);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_飛拍失敗_02\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴Z縮回0保護:
                        {
                            dbapiNozzleZ_InsertSpeed(dbNozzleZ_Home位);
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組XYR移動至吐料位);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴Z縮回0保護);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴Z縮回0保護\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組XYR移動至吐料位:
                        {
                            dbapiNozzleX_InsertSpeed(dbNozzleX_Home位);
                            dbapiNozzleY_InsertSpeed(dbNozzleY_Home位);
                            dbapiNozzleR_InsertSpeed(dbNozzleR_Home位);
                            if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組Z下降至吐料位);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組XYR移動至吐料位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴軸組XYR移動至吐料位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組Z下降至吐料位:
                        {
                            dbapiNozzleZ_InsertSpeed(db吐料位下降Z高度);
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組吐料前準備作業);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組Z下降至吐料位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴軸組Z下降至吐料位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組吐料前準備作業:
                        {
                            //吸嘴吸真空關閉
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴吸, LOW);

                            //流量閥開啟
                            dbapi_FlowValve_吸嘴破真空(100);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組吐料作業);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴軸組吐料前準備作業\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組吐料作業:
                        {
                            //吸嘴破真空開啟
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, HIGH);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴Z縮回0);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴軸組吐料作業\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴Z縮回0:
                        {
                            //流量閥關閉
                            dbapi_FlowValve_吸嘴破真空(0);

                            //吸嘴破真空關閉
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, LOW);

                            dbapiNozzleZ_InsertSpeed(dbNozzleZ_Home位);
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴XYR回home保護位);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴Z縮回0);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴Z縮回0\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴XYR回home保護位:
                        {
                            dbapiNozzleX_InsertSpeed(dbNozzleX_Home位);
                            dbapiNozzleY_InsertSpeed(dbNozzleY_Home位);
                            dbapiNozzleR_InsertSpeed(dbNozzleR_Home位);
                            if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組吐料完畢);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_吸嘴XYR回home保護位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴XYR回home保護位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_吸嘴軸組吐料完畢:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_跳回_至_tp2Insert_取針前動作準備);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_吸嘴軸組吐料完畢\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_ISR_跳回_至_tp2Insert_取針前動作準備:
                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2_ISR02_END);
                        Xavier_Task2_Debugprintf("tp2Insert_ISR_跳回_至_tp2Insert_取針前動作準備\r\n");
                        break;

                case xeXavier_T2_Job.tp2_ISR02_END:
                    if(eNeedleType == xeXavier_NeedleType.pT6Place) { 
                        //檔案為植針檔案
                        Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc.pt2SET, (xeXavier_T2_Job)u32ISRDelayCNT);
                        Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc.pt2SET, xeXavier_T2_Job.tp2Insert_取針前動作準備);
                    } else { 
                        //檔案為取針檔案
                        Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc.pt2SET, (xeXavier_T2_Job)u32ISRDelayCNT);
                        Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc.pt2SET, xeXavier_T2_Job.tp2RemoveSTART);
                    }

                    Task2ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR);
                    Xavier_Task2_Debugprintf("tp2_ISR02_end\r\n");
                    break;
                //======ISR Job======
                
                case xeXavier_T2_Job.tp2Idle:  //reserve
                    break;

                case xeXavier_T2_Job.tp2START:  //判斷動作種類
                    { 
                        xeXavier_Indicator rslt = apiGetMachineAction();
                        switch(rslt) {
                            case xeXavier_Indicator.xeXI_狀態_運行: 
                                if(btp6Home_告知系統回home完畢 == true) {
                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2InsertSTART);
                                } else {
                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2HomeSTART);
                                }
                                break;
                            case xeXavier_Indicator.xeXI_狀態_停止:
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2START);
                                break;
                            case xeXavier_Indicator.xeXI_狀態_急停:
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2START);
                                break;

                            case xeXavier_Indicator.xeXI_事件_復歸:
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2HomeSTART);
                                break;
                            case xeXavier_Indicator.xeXI_事件_暫停:    break;
                            case xeXavier_Indicator.xeXI_事件_異常:    break;

                            default:
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2START);
                                break;
                        }
                    }
                    Xavier_Task2_Debugprintf("tp2START\r\n");
                    break;

                case xeXavier_T2_Job.tp2HomeSTART:
                    {
                        dbVelocityNozzleX = apiParaReadIndex("SaveParameterJason.json", 39) / 10;
                        dbVelocityNozzleY = apiParaReadIndex("SaveParameterJason.json", 40) / 10;
                        dbVelocityNozzleZ = apiParaReadIndex("SaveParameterJason.json", 41);
                        dbVelocityNozzleR = apiParaReadIndex("SaveParameterJason.json", 42);

                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_確認吸嘴軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                    }
                    Xavier_Task2_Debugprintf("tp2HomeSTART\r\n");
                    break;
                    case xeXavier_T2_Job.tp2Home_確認吸嘴軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉:
                        {
                            if(btp6Home_告知工作門已關閉 == true) { 
                                UIHelper.SetControlProperty(en_吸嘴Z軸,     () => en_吸嘴Z軸.Checked = true);
                                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, true);

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_吸嘴Z縮回0);                        
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_確認吸嘴軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Home_確認吸嘴軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Home_吸嘴Z縮回0:
                        {
                            dbapiNozzleZ_defaultSpeed(dbNozzleZ_Home位);
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                UIHelper.SetControlProperty(en_吸嘴R軸,     () => en_吸嘴R軸.Checked = false);
                                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, false); 

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_告知植針軸組可以進行復歸動作);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_吸嘴Z縮回0);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Home_吸嘴Z縮回0\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Home_告知植針軸組可以進行復歸動作:
                        {
                            btp2Home_告知植針軸組可以進行復歸動作 = true;

                            UIHelper.SetControlProperty(en_吸嘴X軸,     () => en_吸嘴X軸.Checked = true);
                            UIHelper.SetControlProperty(en_吸嘴Y軸,     () => en_吸嘴Y軸.Checked = true);
                            UIHelper.SetControlProperty(en_吸嘴R軸,     () => en_吸嘴R軸.Checked = true);
                            UIHelper.SetControlProperty(en_吸嘴Z軸,     () => en_吸嘴Z軸.Checked = false);
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴X軸, true); 
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Y軸, true); 
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴R軸, true);  
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, false); 

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_吸嘴XYR回home_從_tp3Home_告知吸嘴軸組_植針軸組無干涉);
                        }
                        Xavier_Task2_Debugprintf("tp2Home_告知植針軸組可以進行復歸動作\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Home_吸嘴XYR回home_從_tp3Home_告知吸嘴軸組_植針軸組無干涉:
                        {
                            if(btp3Home_告知吸嘴軸組_植針軸組無干涉 == true) {
                                dbapiNozzleX_defaultSpeed(dbNozzleX_Home位);  
                                dbapiNozzleY_defaultSpeed(dbNozzleY_Home位);       

                                if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 

                                    //NozzleZ to Home
                                    {
                                        { 
                                            int rslt = 0;
                                            int axis = 0;
                                            string position = "";
                                            string speed    = "";

                                            axis = (int)WMX3軸定義.吸嘴R軸;
                                            rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
                                            if (rslt == 1) {
                                                clsServoControlWMX3.WMX3_SetHomePosition(axis);                                 
                                            }
                                        }
                                        dbapiNozzleR_defaultSpeed(dbNozzleR_Home位);

                                        UIHelper.SetControlProperty(en_吸嘴Z軸,     () => en_吸嘴Z軸.Checked = true);
                                        clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.吸嘴Z軸, true);
                                    }

                                    //排料前準備
                                    {
                                        //吸嘴吸真空關閉
                                        digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴吸, LOW);

                                        //流量閥開啟
                                        dbapi_FlowValve_吸嘴破真空(100);
                                    }

                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_吸嘴Z回home);
                                } else { 
                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_吸嘴XYR回home_從_tp3Home_告知吸嘴軸組_植針軸組無干涉);
                                }
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_吸嘴XYR回home_從_tp3Home_告知吸嘴軸組_植針軸組無干涉);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Home_吸嘴XYR回home_從_tp3Home_告知吸嘴軸組_植針軸組無干涉\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Home_吸嘴Z回home:
                        {
                            //NozzleZ Home 作業
                            {
                                int rslt = 0;
                                int axis = 0;
                                string position = "";
                                string speed    = "";

                                axis = (int)WMX3軸定義.吸嘴Z軸;
                                rslt = clsServoControlWMX3.WMX3_check_ServoOnOff(axis, ref position, ref speed);
                                if (rslt == 1) {
                                    clsServoControlWMX3.WMX3_SetHomePosition(axis);                                 
                                }
                            }

                            //吸嘴破真空開啟
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, HIGH);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_告知吸嘴軸組已回home完畢);
                        }
                        Xavier_Task2_Debugprintf("tp2Home_吸嘴Z回home\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Home_告知吸嘴軸組已回home完畢:
                        {
                            btp2Home_告知吸嘴軸組已回home完畢 = true;

                            if(btp6Home_告知系統回home完畢 == true) { 
                                //流量閥關閉
                                dbapi_FlowValve_吸嘴破真空(0);

                                //吸嘴破真空關閉
                                digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, LOW);

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2START);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32HomeDelayCNT, xeXavier_T2_Job.tp2Home_告知吸嘴軸組已回home完畢);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Home_告知吸嘴軸組已回home完畢\r\n");
                        break;

                    case xeXavier_T2_Job.tp2TakeAndDiscardSTART:
                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2TakeAndDiscardSTART);
                        Xavier_Task2_Debugprintf("tp2TakeAndDiscardSTART\r\n");
                        break;

                case xeXavier_T2_Job.tp2InsertSTART:
                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_取針前動作準備);
                    Xavier_Task2_Debugprintf("tp2InsertSTART\r\n");
                    break;
                    case xeXavier_T2_Job.tp2Insert_取針前動作準備:
                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_確認進行取針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                        Xavier_Task2_Debugprintf("tp2Insert_取針前動作準備\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_確認進行取針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料:
                        {
                            if(true) { 
                                //不管資料 直接取針
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_有植針資料);
                            } else { 
                                //改由Task6觸發通知
                                if(btp6Insert_告知系統已拿到目標植針資料_To_Tp2 == true) { 
                                    btp6Insert_告知系統已拿到目標植針資料_To_Tp2 = false;

                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_有植針資料);
                                } else if(btp6Insert_告知系統無目標植針資料_To_Tp2 == true) {
                                    btp6Insert_告知系統無目標植針資料_To_Tp2 = false;

                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_無植針資料);
                                } else { 
                                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_確認進行取針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                                }
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_確認進行取針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_有植針資料:
                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_確認吸嘴軸不在柔震上方遮住相機);
                        Xavier_Task2_Debugprintf("tp2Insert_有植針資料\r\n");
                        break;                                      
                    case xeXavier_T2_Job.tp2Insert_無植針資料:
                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸動作完成);
                        Xavier_Task2_Debugprintf("tp2Insert_無植針資料\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_確認吸嘴軸不在柔震上方遮住相機:
                        if(dbapiNozzleX(dbRead, 0) >= 120.0) { 
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照);
                        } else { 
                            dbapiNozzleZ_InsertSpeed(dbNozzleZ_Home位);
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                dbapiNozzleX_InsertSpeed(dbNozzleX_Home位);
                                dbapiNozzleY_InsertSpeed(dbNozzleY_Home位);
                            }
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_確認吸嘴軸不在柔震上方遮住相機);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_確認吸嘴軸不在柔震上方遮住相機\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照:
                        {
                            btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照 = true;

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_取得柔震物料座標_從_tp4Insert_告知吸嘴軸組柔震盤物料座標);
                        }    
                        Xavier_Task2_Debugprintf("tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_取得柔震物料座標_從_tp4Insert_告知吸嘴軸組柔震盤物料座標:
                        {
                            if(btp4Insert_告知吸嘴軸組柔震盤物料座標 == true) { 
                                btp4Insert_告知吸嘴軸組柔震盤物料座標 = false;

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組XYR移動至物料座標);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_取得柔震物料座標_從_tp4Insert_告知吸嘴軸組柔震盤物料座標);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_取得柔震物料座標_從_tp4Insert_告知吸嘴軸組柔震盤物料座標\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組XYR移動至物料座標:
                        {
                            dbapiNozzleX_InsertSpeed(db取料Nozzle中心點X + dbPinX_tmrTakePinTick);
                            dbapiNozzleY_InsertSpeed(db取料Nozzle中心點Y + dbPinY_tmrTakePinTick);
                            dbapiNozzleR_InsertSpeed(db取料Nozzle中心點R + dbPinR_tmrTakePinTick);
                            if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降前準備作業);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組XYR移動至物料座標);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組XYR移動至物料座標\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降前準備作業:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴吸, HIGH);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至取料位);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組Z下降前準備作業\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至取料位:
                        {
                            dbapiNozzleZ_InsertSpeed(db取料Nozzle中心點Z);
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組ZR上升至安全位);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至取料位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組Z下降至取料位\r\n");
                        break;
                    #if(false)
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降完畢:
                        {
                            if( (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, 1, xeXavier_T2_Job.tp2Insert_吸嘴軸組取料作業);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降完畢);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組Z下降完畢\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組取料作業:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, 1, xeXavier_T2_Job.tp2Insert_吸嘴軸組ZR上升至安全位);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組取料作業\r\n");
                        break;
                    #endif
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組ZR上升至安全位:
                        {
                            dbapiNozzleR_InsertSpeed(db取料Nozzle中心點R + 90);

                            dbapiNozzleZ_InsertSpeed(dbNozzleZ_Home位);
                            if(dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組XY移動至飛拍準備位);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組ZR上升至安全位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組ZR上升至安全位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組XY移動至飛拍準備位:
                        {
                            dbapiNozzleR_InsertSpeed(db取料Nozzle中心點R + 90);

                          //dbapiNozzleX_InsertSpeed(db下視覺取像X_Start);
                            dbapiNozzleY_InsertSpeed(db下視覺取像Y      );
                            dbapiNozzleZ_InsertSpeed(db下視覺取像Z      );

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組XY移動至飛拍準備位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標:
                        {
                            btp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標 = true;

                            dbapiNozzleR_InsertSpeed(db取料Nozzle中心點R + 90);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照_再次);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照_再次:
                        {
                            btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照 = true;

                            dbapiNozzleR_InsertSpeed(db取料Nozzle中心點R + 90);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組X觸發移動飛拍);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照_再次\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組X觸發移動飛拍:
                        {
                            dbapiNozzleR_InsertSpeed(db取料Nozzle中心點R + 90);

                            if( //(dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                  (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                  (dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                  (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                              //dbapiNozzleX_InsertSpeed(db下視覺取像X_END);
                                dbapiNozzleX_InsertSpeed(495);

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_進行植針軸組放料位檢查);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組X觸發移動飛拍);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組X觸發移動飛拍\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_進行植針軸組放料位檢查:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_判斷值針軸組是否可以放置物料_從_tp3Insert_告知吸嘴軸組可以放物料);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_進行植針軸組放料位檢查\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_判斷值針軸組是否可以放置物料_從_tp3Insert_告知吸嘴軸組可以放物料:
                        {
                            if(btp3Insert_告知吸嘴軸組可以放物料 == true) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_可以放置物料);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_無法放置物料);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_判斷值針軸組是否可以放置物料_從_tp3Insert_告知吸嘴軸組可以放物料\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_無法放置物料:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_移動到植針軸組前等待);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_無法放置物料\r\n");
                        break;                                    
                    case xeXavier_T2_Job.tp2Insert_移動到植針軸組前等待:
                        {
                            dbapiNozzleX_InsertSpeed(495);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_跳回_至_tp2Insert_進行植針軸組放料位檢查);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_移動到植針軸組前等待\r\n");
                        break;                             
                    case xeXavier_T2_Job.tp2Insert_跳回_至_tp2Insert_進行植針軸組放料位檢查:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_進行植針軸組放料位檢查);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_跳回_至_tp2Insert_進行植針軸組放料位檢查\r\n");
                        break;         
                    case xeXavier_T2_Job.tp2Insert_可以放置物料:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組R至放料位_從_tp2Insert_ISR_告知取得吸針嘴組R軸);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_可以放置物料\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組R至放料位_從_tp2Insert_ISR_告知取得吸針嘴組R軸:
                        {
                            if(eDVR_Rsult != eDownVisionRsult.eDVR_Null) { 
                                double dbTargetNozzleR = 0.0;
                                switch(eDVR_Rsult) {
                                    case eDownVisionRsult.eDVR_Get_1Pin_ok_Normal: {
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90;   
                                        
                                        dbapiNozzleX_InsertSpeed(495);
                                        dbapiNozzleY_InsertSpeed(77.05);
                                        dbapiNozzleR_InsertSpeed(dbTargetNozzleR);

                                        eDVR_Rsult = eDownVisionRsult.eDVR_Null;

                                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_移至植針軸組上方放料位);
                                    } break;

                                    case eDownVisionRsult.eDVR_Get_1Pin_ok_Inverse: {
                                        dbTargetNozzleR = db取料Nozzle中心點R + 90 + 180;  

                                        dbapiNozzleX_InsertSpeed(495);
                                        dbapiNozzleY_InsertSpeed(77.05);
                                        dbapiNozzleR_InsertSpeed(dbTargetNozzleR);

                                        eDVR_Rsult = eDownVisionRsult.eDVR_Null;

                                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_移至植針軸組上方放料位);
                                    } break;

                                    case eDownVisionRsult.eDVR_Null: 
                                    case eDownVisionRsult.eDVR_Get_0Pin_ng:
                                    case eDownVisionRsult.eDVR_Get_1Pin_ng: 
                                    case eDownVisionRsult.eDVR_Get_2Pin_ng:  
                                    case eDownVisionRsult.eDVR_NG:
                                        //錯誤
                                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32ISRDelayCNT, xeXavier_T2_Job.tp2Insert_ISR_飛拍失敗_02);
                                        break;
                                }
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組R至放料位_從_tp2Insert_ISR_告知取得吸針嘴組R軸);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組R至放料位_從_tp2Insert_ISR_告知取得吸針嘴組R軸\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_移至植針軸組上方放料位:
                        {
                            if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至放料前準備作業);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_移至植針軸組上方放料位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_移至植針軸組上方放料位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至放料前準備作業:
                        {
                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至放料位);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組Z下降至放料前準備作業\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至放料位:
                        {
                            double dbNozzleZ下降至放料位; {
                                dbNozzleZ下降至放料位 = apiParaReadIndex("SaveParameterJason.json", 38);
                            }
                            dbapiNozzleZ_InsertSpeed(dbNozzleZ下降至放料位);
                            if(dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降放料完畢);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降至放料位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組Z下降至放料位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組Z下降放料完畢:
                        {
                            //吸嘴吸真空關閉
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴吸, LOW);

                            //流量閥開啟
                            dbapi_FlowValve_吸嘴破真空(100);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組放料作業);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組Z下降放料完畢\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組放料作業:
                        {
                            //吸嘴破真空開啟
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, HIGH);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT+40/*改了40會踩到狗屎*/, xeXavier_T2_Job.tp2Insert_告知植針軸組可以進行放料作業);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組放料作業\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_告知植針軸組可以進行放料作業:
                        {
                            btp2Insert_告知植針軸組可以進行放料作業 = true;

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組放料完成_從_tp3Insert_告知吸嘴軸組_植針軸放料完成);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_告知植針軸組可以進行放料作業\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸組放料完成_從_tp3Insert_告知吸嘴軸組_植針軸放料完成:
                        {
                            if(btp3Insert_告知吸嘴軸組_植針軸放料完成 == true) { 
                                btp3Insert_告知吸嘴軸組_植針軸放料完成 = false;

                                btp3Insert_告知吸嘴軸組可以放物料 = false;

                                //流量閥關閉
                                dbapi_FlowValve_吸嘴破真空(0);

                                //吸嘴破真空關閉
                                digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, LOW);

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT+20, xeXavier_T2_Job.tp2Insert_吸嘴Z縮回0);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸組放料完成_從_tp3Insert_告知吸嘴軸組_植針軸放料完成);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸組放料完成_從_tp3Insert_告知吸嘴軸組_植針軸放料完成\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_吸嘴Z縮回0:
                        {
                            dbapiNozzleZ_InsertSpeed(dbNozzleZ_Home位);
                            if(dbapiNozzleZ(dbCheckArrived, 0) == dbAxisMoveOk) { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_取針前動作準備);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴Z縮回0);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴Z縮回0\r\n");
                        break;
                    #if(false)
                    case xeXavier_T2_Job.tp2Insert_吸嘴XYR回home保護位:
                        {
                            dbapiNozzleX_InsertSpeed(dbNozzleX_Home位);
                            dbapiNozzleY_InsertSpeed(dbNozzleY_Home位);
                            if( (dbapiNozzleX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiNozzleY(dbCheckArrived, 0) == dbAxisMoveOk) ) {

                                //吸嘴吸真空關閉
                                digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴吸, LOW);

                                //流量閥開啟
                                dbapi_FlowValve_吸嘴破真空(100);

                                //吸嘴破真空開啟
                                digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, HIGH);

                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_跳回_至_tp2Insert_取針前動作準備);
                            } else { 
                                Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴XYR回home保護位);
                            }
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴XYR回home保護位\r\n");
                        break;
                    case xeXavier_T2_Job.tp2Insert_跳回_至_tp2Insert_取針前動作準備:
                        {
                            //流量閥關閉
                            dbapi_FlowValve_吸嘴破真空(0);

                            //吸嘴破真空關閉
                            digitalWrite((int)WMX3IO對照.pxeIO_取料吸嘴破真空新, LOW);

                            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_取針前動作準備);
                        }
                        Xavier_Task2_Debugprintf("tp2Insert_跳回_至_tp2Insert_取針前動作準備\r\n");
                        break;
                    #endif
                    case xeXavier_T2_Job.tp2Insert_吸嘴軸動作完成:
                        Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2Insert_吸嘴軸動作完成);
                        Xavier_Task2_Debugprintf("tp2Insert_吸嘴軸動作完成\r\n");
                        break;

                case xeXavier_T2_Job.tp2RemoveSTART:
                    Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, u32InsertDelayCNT, xeXavier_T2_Job.tp2RemoveSTART);
                    Xavier_Task2_Debugprintf("tp2RemoveSTART\r\n");
                    break;

                default:
                    break;
            }

            Xavier_Task2_proc(xeXavier_T2_proc.pt2SET, xeXavier_T2_Job.tp2Idle);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public xeXavier_T2_Job Xavier_T2_delayCase(xeXavier_T2_proc deJob, uint delayCNT, xeXavier_T2_Job excuteJob) {
            switch (deJob) {
                case xeXavier_T2_proc.pt2SET:
                    Xavier_T2_dC_decdelayCNT = delayCNT + 2;
                    Xavier_T2_dC_GetInJob = excuteJob;
                    break;

                case xeXavier_T2_proc.pt2Interrupt:
                    if (Xavier_T2_dC_GetInJob != excuteJob) {
                        Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc.pt2SET, (xeXavier_T2_Job)Xavier_T2_dC_decdelayCNT);
                        Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc.pt2SET, Xavier_T2_dC_GetInJob);

                        Xavier_T2_dC_GetInJob = excuteJob;
                        Xavier_T2_dC_decdelayCNT = 2;  // equal to excute pt2deExcute to get Xavier_Task2_proc(pt2SET,GetInJob);
                    }
                    break;

                case xeXavier_T2_proc.pt2ResISR:
                    Xavier_T2_dC_decdelayCNT = (uint)Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc.pt2GET, Xavier_T2_dC_GetInJob) + 2;
                    Xavier_T2_dC_GetInJob    =       Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc.pt2GET, Xavier_T2_dC_GetInJob);

                    Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc.pt2SET, (xeXavier_T2_Job)2);
                    Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc.pt2SET, xeXavier_T2_Job.tp2Empty);
                    break;

                case xeXavier_T2_proc.pt2deExcute:
                    if (Xavier_T2_dC_decdelayCNT > 0) {
                        Xavier_T2_dC_decdelayCNT--;
                    }

                    if (Xavier_T2_dC_decdelayCNT == 1) {
                        Xavier_Task2_proc(xeXavier_T2_proc.pt2SET, Xavier_T2_dC_GetInJob);
                    }
                    break;

                case xeXavier_T2_proc.pt2GET:
                    break;
            }

            return Xavier_T2_dC_GetInJob;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T2_Job Xavier_Task2_proc(xeXavier_T2_proc rtFun, xeXavier_T2_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T2_proc.pt2SET:
                    Xavier_Task2_p_ret = ptValue;
                    break;

                case xeXavier_T2_proc.pt2GET:
                    break;
            }

            return Xavier_Task2_p_ret;
        }
        //---------------------------------------------------------------------------------------
        public void Task2CallJob(xeXavier_T2_Job excuteJob) {
            Xavier_T2_delayCase(xeXavier_T2_proc.pt2Interrupt, 0, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task2CallJobWithDelay(xeXavier_T2_Job excuteJob, uint delayCNT) {
            Xavier_T2_delayCase(xeXavier_T2_proc.pt2SET, delayCNT, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task2ResumeJob() {
            Xavier_T2_delayCase(xeXavier_T2_proc.pt2ResISR, 0, 0);
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T2_Job Xavier_Task2_ISR_JobTmp(xeXavier_T2_proc rtFun, xeXavier_T2_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T2_proc.pt2SET:
                    Xavier_Task2_ISR_JT_retmp = ptValue;
                    break;

                case xeXavier_T2_proc.pt2GET:
                    break;
            }

            return Xavier_Task2_ISR_JT_retmp;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T2_Job Xavier_Task2_ISR_CNTTmp(xeXavier_T2_proc rtFun, xeXavier_T2_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T2_proc.pt2SET:
                    Xavier_Task2_ISR_CT_retmp = ptValue;
                    break;

                case xeXavier_T2_proc.pt2GET:
                    break;
            }

            return Xavier_Task2_ISR_CT_retmp;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Debug Method----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task2_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task2_Info, () => lbldbg_Task2_Info.Text = message);

            XavierLogger.Log("Task2", message);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T2 -------------------------------
        //---------------------------------------------------------------------------------------
        #endregion



        #region XavierTaskFlowEngine_T3
        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T3 -------------------------------
        //---------------------------------------------------------------------------------------

        // ---------Private Variables----------
        public int i堵料排除retry次數 = 0;
        public bool b植針嘴堵料       = false;

        // ----------Global Variables----------
        public static uint Xavier_T3_dC_decdelayCNT  = 0;
        public static xeXavier_T3_Job Xavier_T3_dC_GetInJob     = 0;
        public static xeXavier_T3_Job Xavier_Task3_p_ret        = 0;
        public static xeXavier_T3_Job Xavier_Task3_ISR_JT_retmp = xeXavier_T3_Job.tp3_ISR01_START;
        public static xeXavier_T3_Job Xavier_Task3_ISR_CT_retmp = xeXavier_T3_Job.tp3_ISR01_START;

        // ----------Enumerations----------
        public enum xeXavier_T3_proc {
            pt3SET = 1,
            pt3GET,
            pt3Interrupt,
            pt3ResISR,
            pt3deExcute,
        }

        public enum xeXavier_T3_Job {
            tp3Empty = 0,
            tp3Init,
            
            tp3_ISR01_START,
            tp3_ISR01_STEP1,
            tp3_ISR01_STEP2,
            tp3_ISR01_END,

            tp3_ISR02_START,
            tp3_ISR02_釋放蓋板與吹氣桿,
            tp3_ISR02_BACKHOME,
            tp3_ISR02_END,
            
            tp3Idle,
            tp3START,  //判斷動作種類

            //植針軸組
            tp3HomeSTART,
                tp3Home_如果植針軸組Z過高_則降低至植針軸組Z原點位,
                tp3Home_告知吸嘴軸組_植針軸組無干涉,
                tp3Home_確認植針軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉,
                tp3Home_確認植針軸組可以進行復歸動作_從_tp2Home_告知植針軸組可以進行復歸動作,
                tp3Home_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位,
                tp3Home_告知載盤組_植針軸組無干涉,
                tp3Home_植針嘴R回放料位,
                tp3Home_告知植針軸組已回home完畢,

            tp3TakeAndDiscardSTART,

            tp3InsertSTART,
                tp3Insert_歸位準備,
                tp3Insert_確認進行植針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料,
                tp3Insert_無植針資料,                                   tp3Insert_有植針資料,
                                                                        tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位,
                                                                        tp3Insert_告知載盤組_植針軸組無干涉,
                                                                        tp3Insert_植針嘴R回放料位,
                                                                        tp3Insert_判斷植針軸是否可以放料,
                                                                        tp3Insert_告知吸嘴軸組可以放物料,
                                                                        tp3Insert_確認植針軸組可以進行放料作業_從_tp2Insert_告知植針軸組可以進行放料作業,
                                                                        tp3Insert_植針軸放料前置作業,
                                                                        tp3Insert_植針軸放料作業,
                                                                        tp3Insert_告知吸嘴軸組_植針軸放料完成,
                                                                        tp3Insert_植針軸放料完畢,
                                                                        tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位,
                                                                        tp3Insert_植針軸組ZR至植針位,
                                                                        tp3Insert_擺放座蓋板關,
                                                                        tp3Insert_植針吹氣前置作業,
                                                                        tp3Insert_植針吹氣作業,
                                                                        tp3Insert_植針吹氣完畢,
                                                                        tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位_再次,
                                                                        tp3Insert_告知載盤組_植針軸植針完畢,
                                                                        tp3Insert_確認植針結果_從_tp5Insert_告知系統植針成功_或_tp5Insert_告知系統植針失敗,
                    tp3Insert_得知植針成功,                             tp3Insert_得知植針失敗,
                                                                        tp3Insert_重設堵料排除retry次數,
                                                                        tp3Insert_檢查堵料排除retry次數,
                    tp3Insert_賭料排除retry次數等於0,                   tp3Insert_賭料排除retry次數大於0,
                    tp3Insert_告知系統賭料排除異常_告知系統中止,        tp3Insert_告知載盤組進行補光,
                                                                        tp3Insert_確認植針軸組可進行堵料檢查_從_tp5Insert_告知載盤組已至補光位,
                                                                        tp3Insert_植針軸組ZR至堵孔檢查位,
                                                                        tp3Insert_確認植針軸組可進行堵料檢查_從_tp4Insert_告知堵料檢查植針嘴相機已至拍照位,
                                                                        tp3Insert_進行植針嘴堵料拍照,
                                                                        tp3Insert_植針軸組ZR回至放料位,
                                                                        tp3Insert_告知完成植針嘴堵料拍照,
                    tp3Insert_告知植針軸組判斷未堵料,                   tp3Insert_告知植針軸組判斷堵料,
                                                                        tp3Insert_進行堵料排除程序,
                                                                        tp3Insert_確認堵料排除程序前置作業_從_tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位,
                                                                        tp3Insert_植針軸組ZR至植針位供排除堵料,
                                                                        tp3Insert_植針軸組堵料吹氣桿縮入,
                                                                        tp3Insert_植針軸組堵料吹氣,
                                                                        tp3Insert_植針軸組堵料吹氣桿伸出,
                                                                        tp3Insert_植針軸組ZR至放料位,
                                                                        tp3Insert_植針軸組堵料吹氣完畢,
                                                                        tp3Insert_告知植針軸組堵料吹氣完畢,
                                                                        tp3Insert_跳回_至_tp3Insert_檢查堵料排除retry次數,
                                                                        tp3Insert_植針軸動作完成,
            tp3InsertEND,

            tp3RemoveSTART,
        }

        // ----------Methods----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_TASK3() {  //植針軸組
            xeXavier_T3_Job priTASK = 0;
            Xavier_T3_delayCase(xeXavier_T3_proc.pt3deExcute, (uint)xeXavier_T3_Job.tp3Empty, xeXavier_T3_Job.tp3Empty);
            priTASK = Xavier_Task3_proc(xeXavier_T3_proc.pt3GET, 0);

            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;

                case xeXavier_Indicator.xeXI_狀態_停止:
                case xeXavier_Indicator.xeXI_狀態_急停:
                case xeXavier_Indicator.xeXI_事件_暫停:
                case xeXavier_Indicator.xeXI_事件_異常: 
                default:
                    priTASK = xeXavier_T3_Job.tp3START;
                    break;
            }

            if( (xeXavier_T3_Job.tp3InsertSTART <= priTASK) &&
                (priTASK <= xeXavier_T3_Job.tp3InsertEND) ) {
                iTask3_CNT = (int)priTASK;
            }

            switch (priTASK) {
                case xeXavier_T3_Job.tp3Empty:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Init);
                    Xavier_Task3_Debugprintf("tp3Empty\r\n");
                    break;

                case xeXavier_T3_Job.tp3Init:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3START);
                    Xavier_Task3_Debugprintf("tp3Init\r\n");
                    break;

                //======ISR Job======
                case xeXavier_T3_Job.tp3_ISR01_START:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR01_STEP1);
                    Xavier_Task3_Debugprintf("tp3_ISR01_START\r\n");
                    break;

                case xeXavier_T3_Job.tp3_ISR01_STEP1:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR01_STEP2);
                    Xavier_Task3_Debugprintf("tp3_ISR01_STEP1\r\n");
                    break;

                case xeXavier_T3_Job.tp3_ISR01_STEP2:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR01_END);
                    Xavier_Task3_Debugprintf("tp3_ISR01_STEP2\r\n");
                    break;

                case xeXavier_T3_Job.tp3_ISR01_END:
                    //Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT);
                    //Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc.pt3SET, xeXavier_T3_Job.tp3STEP2);

                    Task3ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp3_ISR);
                    Xavier_Task3_Debugprintf("tp3_ISR01_end\r\n");
                    break;
                //======ISR Job======
                case xeXavier_T3_Job.tp3_ISR02_START:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR02_釋放蓋板與吹氣桿);
                    Xavier_Task3_Debugprintf("tp3_ISR02_START\r\n");
                    break;

                case xeXavier_T3_Job.tp3_ISR02_釋放蓋板與吹氣桿:
                    {
                        digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                        bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                        digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                        bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                        if( b堵料吹氣桿退出 == true &&
                            b擺放座蓋板打開 == true ) {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR02_BACKHOME);
                        } else {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR02_釋放蓋板與吹氣桿);
                        }
                    }
                    Xavier_Task3_Debugprintf("tp3_ISR02_釋放蓋板與吹氣桿\r\n");
                    break;

                case xeXavier_T3_Job.tp3_ISR02_BACKHOME:
                    {
                        double dbSetR放料位; {
                            dbSetR放料位 = apiParaReadIndex("SaveParameterJason.json", 44);
                        }
                        dbapiSetR_defaultSpeed(dbSetR放料位);
                        dbapiSetZ_defaultSpeed(dbSetZ_Home位);
                        if ( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) &&
                             (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR02_END);
                        } else {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3_ISR02_BACKHOME);
                        }
                    }
                    Xavier_Task3_Debugprintf("tp3_ISR02_BACKHOME\r\n");
                    break;

                case xeXavier_T3_Job.tp3_ISR02_END:
                    if(eNeedleType == xeXavier_NeedleType.pT6Place) { 
                        //檔案為植針檔案
                        Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc.pt3SET, (xeXavier_T3_Job)u32ISRDelayCNT);
                        Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc.pt3SET, xeXavier_T3_Job.tp3Insert_歸位準備);
                    } else { 
                        //檔案為取針檔案
                        Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc.pt3SET, (xeXavier_T3_Job)u32ISRDelayCNT);
                        Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc.pt3SET, xeXavier_T3_Job.tp3RemoveSTART);
                    }

                    Task3ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp3_ISR);
                    Xavier_Task3_Debugprintf("tp3_ISR02_end\r\n");
                    break;
                //======ISR Job======
                
                case xeXavier_T3_Job.tp3Idle:  //reserve
                    break;

                case xeXavier_T3_Job.tp3START:  //判斷動作種類
                    { 
                        xeXavier_Indicator rslt = apiGetMachineAction();
                        switch(rslt) {
                            case xeXavier_Indicator.xeXI_狀態_運行: 
                                if(btp6Home_告知系統回home完畢 == true) {
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3InsertSTART);
                                } else {
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3HomeSTART);
                                }
                                break;
                            case xeXavier_Indicator.xeXI_狀態_停止:
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3START);
                                break;
                            case xeXavier_Indicator.xeXI_狀態_急停:
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3START);
                                break;

                            case xeXavier_Indicator.xeXI_事件_復歸:
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3HomeSTART);
                                break;
                            case xeXavier_Indicator.xeXI_事件_暫停:    break;
                            case xeXavier_Indicator.xeXI_事件_異常:    break;

                            default:
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32ISRDelayCNT, xeXavier_T3_Job.tp3START);
                                break;
                        }
                    }
                    Xavier_Task3_Debugprintf("tp3START\r\n");
                    break;

                case xeXavier_T3_Job.tp3HomeSTART:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_如果植針軸組Z過高_則降低至植針軸組Z原點位);
                    Xavier_Task3_Debugprintf("tp3HomeSTART\r\n");
                    break;
                    case xeXavier_T3_Job.tp3Home_如果植針軸組Z過高_則降低至植針軸組Z原點位:
                        { 
                            double SetZ_position = dbapiSetZ(dbRead, 0);
                            if(SetZ_position < 15.0) { 
                                UIHelper.SetControlProperty(en_植針Z軸,     () => en_植針Z軸.Checked = true);
                                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, true);

                                dbapiSetZ_defaultSpeed(dbSetZ_Home位);
                                if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) {
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_告知吸嘴軸組_植針軸組無干涉); 
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_如果植針軸組Z過高_則降低至植針軸組Z原點位);
                                }
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_告知吸嘴軸組_植針軸組無干涉);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Home_如果植針軸組Z過高_則降低至植針軸組Z原點位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_告知吸嘴軸組_植針軸組無干涉:
                        {
                            btp3Home_告知吸嘴軸組_植針軸組無干涉 = true;
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_確認植針軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                        }
                        Xavier_Task3_Debugprintf("tp3Home_告知吸嘴軸組_植針軸組無干涉\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_確認植針軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉:
                        {
                            if(btp6Home_告知工作門已關閉 == true) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_確認植針軸組可以進行復歸動作_從_tp2Home_告知植針軸組可以進行復歸動作);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_確認植針軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Home_確認植針軸組可以進行復歸動作_從_tp6Home_告知工作門已關閉\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_確認植針軸組可以進行復歸動作_從_tp2Home_告知植針軸組可以進行復歸動作:
                        {
                            if(btp2Home_告知植針軸組可以進行復歸動作 == true) { 
                                UIHelper.SetControlProperty(en_植針Z軸,     () => en_植針Z軸.Checked = true);
                                clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針Z軸, true);

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_確認植針軸組可以進行復歸動作_從_tp2Home_告知植針軸組可以進行復歸動作);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Home_確認植針軸組可以進行復歸動作_從_tp2Home_告知植針軸組可以進行復歸動作\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開

                            dbapiSetZ_defaultSpeed(dbSetZ_Home位);    
                            if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_告知載盤組_植針軸組無干涉);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Home_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_告知載盤組_植針軸組無干涉:
                        {
                            btp3Home_告知載盤組_植針軸組無干涉 = true;

                            UIHelper.SetControlProperty(en_植針R軸,     () => en_植針R軸.Checked = true);
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.植針R軸, true);

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_植針嘴R回放料位);
                        }
                        Xavier_Task3_Debugprintf("tp3Home_告知載盤組_植針軸組無干涉\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_植針嘴R回放料位:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                            bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                            if( b堵料吹氣桿退出 == true &&
                                b擺放座蓋板打開 == true ) {

                                double dbSetR放料位; {
                                    dbSetR放料位 = apiParaReadIndex("SaveParameterJason.json", 44);
                                }
                                dbapiSetR_defaultSpeed(dbSetR放料位);

                                if ( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_告知植針軸組已回home完畢);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_植針嘴R回放料位);
                                }  
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_植針嘴R回放料位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Home_植針嘴R回放料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Home_告知植針軸組已回home完畢:
                        {
                            btp3Home_告知植針軸組已回home完畢 = true;

                            if(btp6Home_告知系統回home完畢 == true) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3START);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32HomeDelayCNT, xeXavier_T3_Job.tp3Home_告知植針軸組已回home完畢);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Home_告知植針軸組已回home完畢\r\n");
                        break;

                case xeXavier_T3_Job.tp3TakeAndDiscardSTART:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3TakeAndDiscardSTART);
                    Xavier_Task3_Debugprintf("tp3TakeAndDiscardSTART\r\n");
                    break;

                case xeXavier_T3_Job.tp3InsertSTART:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_歸位準備);
                    Xavier_Task3_Debugprintf("tp3InsertSTART\r\n");
                    break;
                    case xeXavier_T3_Job.tp3Insert_歸位準備:
                        Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認進行植針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                        Xavier_Task3_Debugprintf("tp3Insert_歸位準備\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認進行植針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料:
                        {
                            if(btp6Insert_告知系統已拿到目標植針資料_To_Tp3 == true) { 
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_有植針資料);                        
                            } else if(btp6Insert_告知系統無目標植針資料_To_Tp3 == true) { 
                                btp6Insert_告知系統無目標植針資料_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_無植針資料);  
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認進行植針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);   
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認進行植針動作_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_無植針資料:
                        {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸動作完成);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_無植針資料\r\n");
                        break;                                   
                    case xeXavier_T3_Job.tp3Insert_有植針資料:
                        {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_有植針資料\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                            bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                            dbapiSetZ_InsertSpeed(dbSetZ_放料位);
                            if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (b堵料吹氣桿退出 == true)  &&
                                (b擺放座蓋板打開 == true) ) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知載盤組_植針軸組無干涉);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知載盤組_植針軸組無干涉:
                        {
                            btp3Insert_告知載盤組_植針軸組無干涉 = true;

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針嘴R回放料位);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知載盤組_植針軸組無干涉\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針嘴R回放料位:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                            bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                            if( (b堵料吹氣桿退出 == true) && 
                                (b擺放座蓋板打開 == true) ) { 
                                double dbSetR放料位; {
                                    dbSetR放料位 = apiParaReadIndex("SaveParameterJason.json", 44);
                                }
                                dbapiSetR_InsertSpeed(dbSetR放料位);
                            }

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_判斷植針軸是否可以放料);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針嘴R回放料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_判斷植針軸是否可以放料:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                            bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                            bool bSetZ放料位 = false;
                            double dbposSetZ = dbapiSetZ(dbRead, 0);
                            if( (dbSetZ_放料位 * 0.99 <= dbposSetZ &&
                                                         dbposSetZ <= dbSetZ_放料位 * 1.01) ) { 
                                bSetZ放料位 = true;
                            }

                            double dbSetR放料位; {
                                dbSetR放料位 = apiParaReadIndex("SaveParameterJason.json", 44);
                            }
                            bool bSetR放料位 = false;
                            if ( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                bSetR放料位 = true;
                            }

                            if( (b堵料吹氣桿退出 == true) &&
                                (b擺放座蓋板打開 == true) &&
                                (bSetZ放料位     == true) &&
                                (bSetR放料位     == true) ) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知吸嘴軸組可以放物料);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_判斷植針軸是否可以放料\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知吸嘴軸組可以放物料:
                        {
                            btp3Insert_告知吸嘴軸組可以放物料 = true;
                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座吸真空, HIGH);

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT+5, xeXavier_T3_Job.tp3Insert_確認植針軸組可以進行放料作業_從_tp2Insert_告知植針軸組可以進行放料作業);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知吸嘴軸組可以放物料\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認植針軸組可以進行放料作業_從_tp2Insert_告知植針軸組可以進行放料作業:
                        {
                            if(btp2Insert_告知植針軸組可以進行放料作業 == true) { 
                                btp2Insert_告知植針軸組可以進行放料作業 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸放料前置作業);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸組可以進行放料作業_從_tp2Insert_告知植針軸組可以進行放料作業);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認植針軸組可以進行放料作業_從_tp2Insert_告知植針軸組可以進行放料作業\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸放料前置作業:
                        {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸放料作業);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸放料前置作業\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸放料作業:
                        {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知吸嘴軸組_植針軸放料完成);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸放料作業\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知吸嘴軸組_植針軸放料完成:
                        {
                            btp3Insert_告知吸嘴軸組_植針軸放料完成 = true;

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸放料完畢);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知吸嘴軸組_植針軸放料完成\r\n");
                        break; 
                    case xeXavier_T3_Job.tp3Insert_植針軸放料完畢:
                        {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸放料完畢\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位:
                        {
                            Xavier_Task3_Debugprintf("tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位(1)\r\n");
                            while(iTask5_CNT!=(int)xeXavier_T5_Job.tp5Insert_告知植針軸組載盤組已移至植針位) {
                                xeXavier_Indicator rslt = apiGetMachineAction();
                                if(rslt== xeXavier_Indicator.xeXI_狀態_停止) {
                                    break;
                                }
                            }

                            if(btp5Insert_告知植針軸組載盤組已移至植針位 == true) { 
                                btp5Insert_告知植針軸組載盤組已移至植針位 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至植針位);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組ZR至植針位:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                            bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                            if( (b堵料吹氣桿退出 == true) && 
                                (b擺放座蓋板打開 == true) ) { 

                                double SetPlacePinZHight; {
                                    SetPlacePinZHight = apiParaReadIndex("SaveParameterJason.json", 11);
                                }
                                dbapiSetZ_InsertSpeed(SetPlacePinZHight);

                                double dbSetR植針位; {
                                    dbSetR植針位 = apiParaReadIndex("SaveParameterJason.json", 43);
                                }
                                dbapiSetR_InsertSpeed(dbSetR植針位);

                                if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT+20, xeXavier_T3_Job.tp3Insert_擺放座蓋板關);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至植針位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組ZR至植針位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_擺放座蓋板關:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, HIGH);  //擺放座蓋板->閉合
                            bool b擺放座蓋板閉合 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板合);

                            if(b擺放座蓋板閉合 == true) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針吹氣前置作業);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_擺放座蓋板關);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_擺放座蓋板關\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針吹氣前置作業:
                        {
                            //擺放座真空關閉:    
                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座吸真空, LOW);

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針吹氣作業);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針吹氣前置作業\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針吹氣作業:
                        {
                            //植針吹氣電磁閥開啟:       
                            digitalWrite((int)WMX3IO對照.pxeIO_植針吹氣, HIGH);
            
                            //開啟流量閥
                            dbapi_FlowValve_植針吹氣(100);

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT+50, xeXavier_T3_Job.tp3Insert_植針吹氣完畢);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針吹氣作業\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針吹氣完畢:
                        {
                            //關閉流量閥
                            dbapi_FlowValve_植針吹氣(0);

                            //植針吹氣電磁閥關閉:       
                            digitalWrite((int)WMX3IO對照.pxeIO_植針吹氣, LOW);

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位_再次);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針吹氣完畢\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位_再次:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                            bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                            digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                            bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                            dbapiSetZ_InsertSpeed(dbSetZ_放料位);
                            if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (b堵料吹氣桿退出 == true)  &&
                                (b擺放座蓋板打開 == true) ) { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知載盤組_植針軸植針完畢);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位_再次);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_堵料吹氣桿出_擺放座蓋板開_並植針嘴Z回放料位_再次\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知載盤組_植針軸植針完畢:
                        {
                            btp3Insert_告知載盤組_植針軸植針完畢 = true;

                            Xavier_Task3_Debugprintf("tp3Insert_告知載盤組_植針軸植針完畢(2)\r\n");
                            while(iTask5_CNT!=(int)xeXavier_T5_Job.tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢) {
                                xeXavier_Indicator rslt = apiGetMachineAction();
                                if(rslt== xeXavier_Indicator.xeXI_狀態_停止) {
                                    break;
                                }
                            }

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針結果_從_tp5Insert_告知系統植針成功_或_tp5Insert_告知系統植針失敗);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知載盤組_植針軸植針完畢\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認植針結果_從_tp5Insert_告知系統植針成功_或_tp5Insert_告知系統植針失敗:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else if(btp5Insert_告知系統植針失敗 == true) { 
                                btp5Insert_告知系統植針失敗 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針失敗);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針結果_從_tp5Insert_告知系統植針成功_或_tp5Insert_告知系統植針失敗);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認植針結果_從_tp5Insert_告知系統植針成功_或_tp5Insert_告知系統植針失敗\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_得知植針成功:
                        {
                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_歸位準備);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_得知植針成功\r\n");
                        break;                             
                    case xeXavier_T3_Job.tp3Insert_得知植針失敗:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_重設堵料排除retry次數);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_得知植針失敗\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_重設堵料排除retry次數:
                        {
                            i堵料排除retry次數 = 952730678;
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_檢查堵料排除retry次數);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_重設堵料排除retry次數\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_檢查堵料排除retry次數:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                if(i堵料排除retry次數 > 0) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_賭料排除retry次數大於0);
                                } else 
                                if(i堵料排除retry次數 == 0) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_賭料排除retry次數等於0);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_檢查堵料排除retry次數\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_賭料排除retry次數等於0:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知系統賭料排除異常_告知系統中止);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_賭料排除retry次數等於0\r\n");
                        break;                   
                    case xeXavier_T3_Job.tp3Insert_告知系統賭料排除異常_告知系統中止:
                        { 
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                btp3Insert_告知系統賭料排除異常_告知系統中止 = true;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知系統賭料排除異常_告知系統中止);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知系統賭料排除異常_告知系統中止\r\n");
                        break;        
                    case xeXavier_T3_Job.tp3Insert_賭料排除retry次數大於0:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知載盤組進行補光);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_賭料排除retry次數大於0\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知載盤組進行補光:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                btp3Insert_告知載盤組進行補光 = true;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸組可進行堵料檢查_從_tp5Insert_告知載盤組已至補光位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知載盤組進行補光\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認植針軸組可進行堵料檢查_從_tp5Insert_告知載盤組已至補光位:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                if(btp5Insert_告知載盤組已至補光位 == true) {
                                    btp5Insert_告知載盤組已至補光位 = false;

                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至堵孔檢查位);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸組可進行堵料檢查_從_tp5Insert_告知載盤組已至補光位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認植針軸組可進行堵料檢查_從_tp5Insert_告知載盤組已至補光位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組ZR至堵孔檢查位:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                                bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                                digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                                bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                                //堵孔檢查高度
                                bool bCheckSetZpos = false;
                                double CheckSetZ; {
                                    CheckSetZ = apiParaReadIndex("SaveParameterJason.json", 31);
                                }
                                dbapiSetZ_InsertSpeed(CheckSetZ);
                                if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    bCheckSetZpos = true;
                                }  

                                bool bCheckSetRpos = false;
                                if( (b堵料吹氣桿退出 == true) &&
                                    (b擺放座蓋板打開 == true) ) { 
                                    double CheckSetR; {
                                        CheckSetR = apiParaReadIndex("SaveParameterJason.json", 30);
                                    }
                                    dbapiSetR_InsertSpeed(CheckSetR);

                                    if( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        bCheckSetRpos = true;
                                    }  
                                }   

                                if( (bCheckSetZpos == true) && 
                                    (bCheckSetRpos == true) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸組可進行堵料檢查_從_tp4Insert_告知堵料檢查植針嘴相機已至拍照位);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至堵孔檢查位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組ZR至堵孔檢查位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認植針軸組可進行堵料檢查_從_tp4Insert_告知堵料檢查植針嘴相機已至拍照位:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                if(btp4Insert_告知堵料檢查植針嘴相機已至拍照位 == true) { 
                                    //不要清除
                                    //btp4Insert_告知堵料檢查植針嘴相機已至拍照位 = false;

                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_進行植針嘴堵料拍照);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認植針軸組可進行堵料檢查_從_tp4Insert_告知堵料檢查植針嘴相機已至拍照位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認植針軸組可進行堵料檢查_從_tp4Insert_告知堵料檢查植針嘴相機已至拍照位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_進行植針嘴堵料拍照:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                //這邊拍照檢查
                                bool success = false;
                                    double dbSetPinStatus; {
                                        dbSetPinStatus = apiParaReadIndex("SaveParameterJason.json", 33);
                                    }
                                    switch(dbSetPinStatus) { 
                                        //強制判斷堵孔
                                        case 0:  success = false;  break;

                                        //強制判斷未堵孔
                                        case 1:  success = true;   break;

                                        //依照視覺判斷
                                        case 2: { 
                                            //btn_植針嘴檢查_Click(sender, e);

                                            //植針嘴有無堵料, 無:ok, 有:ng
                                            Inspector.Vector3 pos2;
                                            success = inspector1.xInsp夾爪(out pos2);   //夾爪針孔偵測 回傳:OK/NG 及找到孔的位置
                                        } break;
                                    }  // end of switch(dbSetPinStatus) { 
                                if(success == true) { 
                                    //未堵料
                                    b植針嘴堵料 = false;
                                } else { 
                                    //堵料
                                    b植針嘴堵料 = true;
                                }

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR回至放料位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_進行植針嘴堵料拍照\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組ZR回至放料位:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                                bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                                digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                                bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                                bool bSetZ放料位 = false;
                                dbapiSetZ_InsertSpeed(dbSetZ_放料位);
                                if(dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) { 
                                    bSetZ放料位 = true;
                                }

                                double dbSetR放料位; {
                                    dbSetR放料位 = apiParaReadIndex("SaveParameterJason.json", 44);
                                }
                                bool bSetR放料位 = false;
                                if ( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    bSetR放料位 = true;
                                }

                                if( (b堵料吹氣桿退出 == true) &&
                                    (b擺放座蓋板打開 == true) &&
                                    (bSetZ放料位     == true) &&
                                    (bSetR放料位     == true) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知完成植針嘴堵料拍照);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR回至放料位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組ZR回至放料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知完成植針嘴堵料拍照:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                if(b植針嘴堵料 == true) { 
                                    //堵料
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知植針軸組判斷堵料);
                                } else {
                                    //沒堵料
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知植針軸組判斷未堵料);
                                }

                                btp3Insert_告知完成植針嘴堵料拍照 = true;  //應該不需要
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知完成植針嘴堵料拍照\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知植針軸組判斷未堵料:
                        {
                            btp3Insert_告知植針軸組判斷未堵料 = true;

                            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_歸位準備);
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知植針軸組判斷未堵料\r\n");
                        break;                   
                    case xeXavier_T3_Job.tp3Insert_告知植針軸組判斷堵料:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                btp3Insert_告知植針軸組判斷堵料 = true;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_進行堵料排除程序);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知植針軸組判斷堵料\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_進行堵料排除程序:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認堵料排除程序前置作業_從_tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_進行堵料排除程序\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_確認堵料排除程序前置作業_從_tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                if(btp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位 == true) { 
                                    btp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位 = false;

                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至植針位供排除堵料);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_確認堵料排除程序前置作業_從_tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_確認堵料排除程序前置作業_從_tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組ZR至植針位供排除堵料:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                                bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                                digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                                bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                                bool bMakeClearSetZ = false;
                                double MakeClearSetZ; {
                                    MakeClearSetZ = apiParaReadIndex("SaveParameterJason.json", 35);
                                }
                                dbapiSetZ_InsertSpeed(MakeClearSetZ);
                                if( (dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    bMakeClearSetZ = true;
                                }   

                                bool bMakeClearSetR = false;
                                if( (b堵料吹氣桿退出 == true) &&
                                    (b擺放座蓋板打開 == true) ) { 
                                    double dbSetR植針位; {
                                        dbSetR植針位 = apiParaReadIndex("SaveParameterJason.json", 43);
                                    }
                                    dbapiSetR_InsertSpeed(dbSetR植針位);
                                    if( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        bMakeClearSetR = true;
                                    }    
                                }   

                                if( (bMakeClearSetZ == true) &&
                                    (bMakeClearSetR == true) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣桿縮入);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至植針位供排除堵料);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組ZR至植針位供排除堵料\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣桿縮入:
                        { 
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, HIGH);  //堵料吹氣缸->進去
                                bool b堵料吹氣桿插入 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿進);

                                if(b堵料吹氣桿插入 == true) { 
                                    //堵料吹氣電磁閥打開
                                    digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣, HIGH);

                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣桿縮入);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組堵料吹氣桿縮入\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                //堵料吹氣等待作業
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣桿伸出);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組堵料吹氣\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣桿伸出:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                //堵料吹氣電磁閥關閉
                                 digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣, LOW);

                                digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                                bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                                digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                                bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                                if( (b堵料吹氣桿退出 == true) &&
                                    (b擺放座蓋板打開 == true) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至放料位);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣桿伸出);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組堵料吹氣桿伸出\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組ZR至放料位:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸, LOW);  //堵料吹氣缸->出去 
                                bool b堵料吹氣桿退出 = indicateRead((int)WMX3IO對照.pxeIO_堵料吹氣桿出);

                                digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板, LOW);  //擺放座蓋板->開
                                bool b擺放座蓋板打開 = indicateRead((int)WMX3IO對照.pxeIO_擺放座蓋板開);

                                bool bSetZ放料位 = false;
                                dbapiSetZ_InsertSpeed(dbSetZ_放料位);
                                if(dbapiSetZ(dbCheckArrived, 0) == dbAxisMoveOk) { 
                                    bSetZ放料位 = true;
                                }

                                double dbSetR放料位; {
                                    dbSetR放料位 = apiParaReadIndex("SaveParameterJason.json", 44);
                                }
                                bool bSetR放料位 = false;
                                if ( (dbapiSetR(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    bSetR放料位 = true;
                                }

                                if( (b堵料吹氣桿退出 == true) &&
                                    (b擺放座蓋板打開 == true) &&
                                    (bSetZ放料位     == true) &&
                                    (bSetR放料位     == true) ) { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣完畢);
                                } else { 
                                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸組ZR至放料位);
                                }
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組ZR至放料位\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸組堵料吹氣完畢:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_告知植針軸組堵料吹氣完畢);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸組堵料吹氣完畢\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_告知植針軸組堵料吹氣完畢:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                btp3Insert_告知植針軸組堵料吹氣完畢 = true;
                                i堵料排除retry次數--;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_跳回_至_tp3Insert_檢查堵料排除retry次數);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_告知植針軸組堵料吹氣完畢\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_跳回_至_tp3Insert_檢查堵料排除retry次數:
                        {
                            if(btp5Insert_告知系統植針成功_To_Tp3 == true) { 
                                btp5Insert_告知系統植針成功_To_Tp3 = false;

                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_得知植針成功);
                            } else { 
                                Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_檢查堵料排除retry次數);
                            }
                        }
                        Xavier_Task3_Debugprintf("tp3Insert_跳回_至_tp3Insert_檢查堵料排除retry次數\r\n");
                        break;
                    case xeXavier_T3_Job.tp3Insert_植針軸動作完成:
                        Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3Insert_植針軸動作完成);
                        Xavier_Task3_Debugprintf("tp3Insert_植針軸動作完成\r\n");
                        break;

                case xeXavier_T3_Job.tp3RemoveSTART:
                    Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, u32InsertDelayCNT, xeXavier_T3_Job.tp3RemoveSTART);
                    Xavier_Task3_Debugprintf("tp3RemoveSTART\r\n");
                    break;

                default:
                    break;
            }

            Xavier_Task3_proc(xeXavier_T3_proc.pt3SET, xeXavier_T3_Job.tp3Idle);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_T3_delayCase(xeXavier_T3_proc deJob, uint delayCNT, xeXavier_T3_Job excuteJob) {
            switch (deJob) {
                case xeXavier_T3_proc.pt3SET:
                    Xavier_T3_dC_decdelayCNT = delayCNT + 2;
                    Xavier_T3_dC_GetInJob = excuteJob;
                    break;

                case xeXavier_T3_proc.pt3Interrupt:
                    if (Xavier_T3_dC_GetInJob != excuteJob) {
                        Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc.pt3SET, (xeXavier_T3_Job)Xavier_T3_dC_decdelayCNT);
                        Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc.pt3SET, Xavier_T3_dC_GetInJob);

                        Xavier_T3_dC_GetInJob = excuteJob;
                        Xavier_T3_dC_decdelayCNT = 2;  // equal to excute pt3deExcute to get Xavier_Task3_proc(pt3SET,GetInJob);
                    }
                    break;

                case xeXavier_T3_proc.pt3ResISR:
                    Xavier_T3_dC_decdelayCNT = (uint)Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc.pt3GET, Xavier_T3_dC_GetInJob) + 2;
                    Xavier_T3_dC_GetInJob    =       Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc.pt3GET, Xavier_T3_dC_GetInJob);

                    Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc.pt3SET, (xeXavier_T3_Job)2);
                    Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc.pt3SET, xeXavier_T3_Job.tp3Empty);
                    break;

                case xeXavier_T3_proc.pt3deExcute:
                    if (Xavier_T3_dC_decdelayCNT > 0) {
                        Xavier_T3_dC_decdelayCNT--;
                    }

                    if (Xavier_T3_dC_decdelayCNT == 1) {
                        Xavier_Task3_proc(xeXavier_T3_proc.pt3SET, Xavier_T3_dC_GetInJob);
                    }
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T3_Job Xavier_Task3_proc(xeXavier_T3_proc rtFun, xeXavier_T3_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T3_proc.pt3SET:
                    Xavier_Task3_p_ret = ptValue;
                    break;

                case xeXavier_T3_proc.pt3GET:
                    break;
            }

            return Xavier_Task3_p_ret;
        }
        //---------------------------------------------------------------------------------------
        public void Task3CallJob(xeXavier_T3_Job excuteJob) {
            Xavier_T3_delayCase(xeXavier_T3_proc.pt3Interrupt, 0, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task3CallJobWithDelay(xeXavier_T3_Job excuteJob, uint delayCNT) {
            Xavier_T3_delayCase(xeXavier_T3_proc.pt3SET, delayCNT, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task3ResumeJob() {
            Xavier_T3_delayCase(xeXavier_T3_proc.pt3ResISR, 0, 0);
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T3_Job Xavier_Task3_ISR_JobTmp(xeXavier_T3_proc rtFun, xeXavier_T3_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T3_proc.pt3SET:
                    Xavier_Task3_ISR_JT_retmp = ptValue;
                    break;

                case xeXavier_T3_proc.pt3GET:
                    break;
            }

            return Xavier_Task3_ISR_JT_retmp;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T3_Job Xavier_Task3_ISR_CNTTmp(xeXavier_T3_proc rtFun, xeXavier_T3_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T3_proc.pt3SET:
                    Xavier_Task3_ISR_CT_retmp = ptValue;
                    break;

                case xeXavier_T3_proc.pt3GET:
                    break;
            }

            return Xavier_Task3_ISR_CT_retmp;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Debug Method----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task3_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task3_Info, () => lbldbg_Task3_Info.Text = message);

            XavierLogger.Log("Task3", message);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T3 -------------------------------
        //---------------------------------------------------------------------------------------
        #endregion



        #region XavierTaskFlowEngine_T4
        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T4 -------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Global Variables----------
        public static uint Xavier_T4_dC_decdelayCNT  = 0;
        public static xeXavier_T4_Job Xavier_T4_dC_GetInJob     = 0;
        public static xeXavier_T4_Job Xavier_Task4_p_ret        = 0;
        public static xeXavier_T4_Job Xavier_Task4_ISR_JT_retmp = xeXavier_T4_Job.tp4_ISR01_START;
        public static xeXavier_T4_Job Xavier_Task4_ISR_CT_retmp = xeXavier_T4_Job.tp4_ISR01_START;

        // ----------Enumerations----------
        public enum xeXavier_T4_proc {
            pt4SET = 1,
            pt4GET,
            pt4Interrupt,
            pt4ResISR,
            pt4deExcute,
        }

        public enum xeXavier_T4_Job {
            tp4Empty = 0,
            tp4Init,
            
            tp4_ISR01_START,
            tp4_ISR01_STEP1,
            tp4_ISR01_STEP2,
            tp4_ISR01_END,

            tp4_ISR02_START,
            tp4_ISR02_STEP1,
            tp4_ISR02_STEP2,
            tp4_ISR02_END,
            
            tp4Idle,
            tp4START,  //判斷動作種類

            //電動缸組_含抽針
            tp4HomeSTART,
                tp4Home_確認電動缸組可以進行復歸動作_從_tp6Home_告知工作門已關閉,
                tp4Home_電動缸組_抽針嘴_3D掃描_回安全位,
                tp4Home_告知載盤組_電動缸無干涉,
                tp4Home_電動缸組_IAI相機_植針相機_回home,
                tp4Home_確認電動缸組_抽針嘴_3D掃描_可以進行復歸動作_從_tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作,
                tp4Home_電動缸組_抽針嘴_3D掃描_回home,
                tp4Home_告知電動缸組已回home完畢,

            tp4TakeAndDiscardSTART,

            tp4InsertSTART,
                tp4Insert_電動缸組_抽針嘴_3D掃描_回安全位,
                tp4Insert_告知載盤組_電動缸無干涉,
                tp4Insert_Socket孔檢測相機移至拍照位,
                tp4Insert_告知Socket孔檢測相機已至拍照位,
                tp4Insert_堵料檢查植針嘴相機移至拍照位,
                tp4Insert_告知堵料檢查植針嘴相機已至拍照位,
                tp4Insert_柔震盤檢測機制開始,
                tp4Insert_進行柔震盤物料確認_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料,
                    tp4Insert_有植針資料,                            tp4Insert_無植針資料,
                    tp4Insert_進行柔震盤物料拍照前作業,
                    tp4Insert_進行柔震盤物料確認_從_tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照,
                    tp4Insert_柔震盤物料檢測retry次數重設,
                    tp4Insert_柔震盤物料檢測retry次數,
                    tp4Insert_柔震盤物料檢測retry次數大於0,                                                tp4Insert_柔震盤物料檢測retry次數等於0,
                    tp4Insert_檢查柔震是否有物料,                                                          tp4Insert_柔震盤物料異常_告知系統中止,
                    tp4Insert_柔震盤有物料,                          tp4Insert_柔震盤無物料,
                                                                     tp4Insert_柔震盤啟動震動,
                                                                     tp4Insert_柔震盤停止震動,
                                                                     tp4Insert_跳回_至_tp4Insert_柔震盤物料檢測retry次數,
                    tp4Insert_告知吸嘴軸組柔震盤物料座標,
                    tp4Insert_清除tp4Insert_告知吸嘴軸組柔震盤有物料_從_tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標,
                    tp4Insert_跳回_至_tp4Insert_進行柔震盤物料拍照前作業,
                tp4Insert_柔震盤檢測機制完成,

            tp4RemoveSTART,
        }

        // ---------Private Variables----------
        public int i柔震盤物料檢測retry次數 = 0;
        public int i柔震TypeStep = 0;

        // ----------Methods----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_TASK4() {  //電動缸組_含抽針
            xeXavier_T4_Job priTASK = 0;
            Xavier_T4_delayCase(xeXavier_T4_proc.pt4deExcute, (uint)xeXavier_T4_Job.tp4Empty, xeXavier_T4_Job.tp4Empty);
            priTASK = Xavier_Task4_proc(xeXavier_T4_proc.pt4GET, 0);

            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;

                case xeXavier_Indicator.xeXI_狀態_停止:
                case xeXavier_Indicator.xeXI_狀態_急停:
                case xeXavier_Indicator.xeXI_事件_暫停:
                case xeXavier_Indicator.xeXI_事件_異常: 
                default:
                    priTASK = xeXavier_T4_Job.tp4START;
                    break;
            }

            switch (priTASK) {
                case xeXavier_T4_Job.tp4Empty:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Init);
                    Xavier_Task4_Debugprintf("tp4Empty\r\n");
                    break;

                case xeXavier_T4_Job.tp4Init:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4START);
                    Xavier_Task4_Debugprintf("tp4Init\r\n");
                    break;

                //======ISR Job======
                case xeXavier_T4_Job.tp4_ISR01_START:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4_ISR01_STEP1);
                    Xavier_Task4_Debugprintf("tp4_ISR01_START\r\n");
                    break;

                case xeXavier_T4_Job.tp4_ISR01_STEP1:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4_ISR01_STEP2);
                    Xavier_Task4_Debugprintf("tp4_ISR01_STEP1\r\n");
                    break;

                case xeXavier_T4_Job.tp4_ISR01_STEP2:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4_ISR01_END);
                    Xavier_Task4_Debugprintf("tp4_ISR01_STEP2\r\n");
                    break;

                case xeXavier_T4_Job.tp4_ISR01_END:
                    //Xavier_Task4_ISR_CNTTmp(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT);
                    //Xavier_Task4_ISR_JobTmp(xeXavier_T4_proc.pt4SET, xeXavier_T4_Job.tp4STEP2);

                    Task4ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp4_ISR);
                    Xavier_Task4_Debugprintf("tp4_ISR01_end\r\n");
                    break;
                //======ISR Job======
                case xeXavier_T4_Job.tp4_ISR02_START:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4_ISR02_STEP1);
                    Xavier_Task4_Debugprintf("tp4_ISR02_START\r\n");
                    break;

                case xeXavier_T4_Job.tp4_ISR02_STEP1:
                    { 
                        //載盤真空閥啟用
                        digitalWrite((int)WMX3IO對照.pxeIO_載盤真空閥, HIGH);

                        //Socket1真空閥關掉
                        digitalWrite((int)WMX3IO對照.pxeIO_Socket真空1, LOW);

                        //Socket2真空閥關掉
                        digitalWrite((int)WMX3IO對照.pxeIO_Socket真空2, LOW);

                        //Socket相機移至拍照位22
                        double dbSocketCamera; {
                            dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                            dbapiIAI(dbSocketCamera);
                        }

                        //3D掃描電動缸縮回
                        dbapiJoDell3D掃描(dbJoDell3D掃描_Home位);

                        //吸針嘴電動缸縮回
                        dbapiJoDell吸針嘴(dbJoDell吸針嘴_Home位);

                        //吸針接料盒就位
                        digitalWrite((int)WMX3IO對照.pxeIO_接料區氣桿, HIGH);
                        digitalWrite((int)WMX3IO對照.pxeIO_收料區缸,   LOW);

                        //Nozzle電磁閥關閉
                        dbapi_FlowValve_吸嘴破真空(0);

                        //植針座電磁閥關閉
                        dbapi_FlowValve_植針吹氣(0);

                        digitalWrite((int)WMX3IO對照.pxeIO_堵料吹氣缸,   LOW);  //堵料吹氣缸->出去 
                        digitalWrite((int)WMX3IO對照.pxeIO_擺放座蓋板,   LOW);  //擺放座蓋板->開
                        digitalWrite((int)WMX3IO對照.pxeIO_擺放座吸真空, LOW);
                        digitalWrite((int)WMX3IO對照.pxeIO_植針吹氣,     LOW);

                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4_ISR02_STEP2);
                    }
                    Xavier_Task4_Debugprintf("tp4_ISR02_STEP1\r\n");
                    break;

                case xeXavier_T4_Job.tp4_ISR02_STEP2:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4_ISR02_END);
                    Xavier_Task4_Debugprintf("tp4_ISR02_STEP2\r\n");
                    break;

                case xeXavier_T4_Job.tp4_ISR02_END:
                    if(eNeedleType == xeXavier_NeedleType.pT6Place) {
                        //檔案為植針檔案

                    } else {
                        //檔案為取針檔案
                        Xavier_Task4_ISR_CNTTmp(xeXavier_T4_proc.pt4SET, (xeXavier_T4_Job)u32ISRDelayCNT);
                        Xavier_Task4_ISR_JobTmp(xeXavier_T4_proc.pt4SET, xeXavier_T4_Job.tp4RemoveSTART);
                    }

                    Task4ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp4_ISR);
                    Xavier_Task4_Debugprintf("tp4_ISR02_end\r\n");
                    break;
                //======ISR Job======
                
                case xeXavier_T4_Job.tp4Idle:  //reserve
                    break;

                case xeXavier_T4_Job.tp4START:  //判斷動作種類
                    { 
                        xeXavier_Indicator rslt = apiGetMachineAction();
                        switch(rslt) {
                            case xeXavier_Indicator.xeXI_狀態_運行: 
                                if(btp6Home_告知系統回home完畢 == true) {
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4InsertSTART);
                                } else {
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4HomeSTART);
                                }
                                break;
                            case xeXavier_Indicator.xeXI_狀態_停止:
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4START);
                                break;
                            case xeXavier_Indicator.xeXI_狀態_急停:
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4START);
                                break;

                            case xeXavier_Indicator.xeXI_事件_復歸:
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4HomeSTART);
                                break;
                            case xeXavier_Indicator.xeXI_事件_暫停:    break;
                            case xeXavier_Indicator.xeXI_事件_異常:    break;

                            default:
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32ISRDelayCNT, xeXavier_T4_Job.tp4START);
                                break;
                        }
                    }
                    Xavier_Task4_Debugprintf("tp4START\r\n");
                    break;

                case xeXavier_T4_Job.tp4HomeSTART:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_確認電動缸組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                    Xavier_Task4_Debugprintf("tp4HomeSTART\r\n");
                    break;
                    case xeXavier_T4_Job.tp4Home_確認電動缸組可以進行復歸動作_從_tp6Home_告知工作門已關閉:
                        {
                            if(btp6Home_告知工作門已關閉 == true) { 
                                UIHelper.SetControlProperty(en_JoDell3D掃描,     () => en_JoDell3D掃描.Checked = true);
                                UIHelper.SetControlProperty(en_JoDell吸針嘴,     () => en_JoDell吸針嘴.Checked = true);
                                clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, 1); 
                                clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, 1);  

                                //所有IO Out需要回default, 除了燈
                                //完畢後, 載盤要破真空一下

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_電動缸組_抽針嘴_3D掃描_回安全位);
                            } else { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_確認電動缸組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                            }
                        }
                        Xavier_Task4_Debugprintf("tp4Home_確認電動缸組可以進行復歸動作_從_tp6Home_告知工作門已關閉\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Home_電動缸組_抽針嘴_3D掃描_回安全位:
                        {
                            dbapiJoDell吸針嘴(dbJoDell吸針嘴_Home位);
                            dbapiJoDell3D掃描(dbJoDell3D掃描_Home位);

                            double dbrsltJoDell3D掃描 = dbapiJoDell3D掃描(dbCheckArrived);
                            double dbrsltJoDell吸針嘴 = dbapiJoDell吸針嘴(dbCheckArrived);
                            if( dbrsltJoDell3D掃描 == dbAxisMoveOk &&
                                dbrsltJoDell吸針嘴 == dbAxisMoveOk ) {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_告知載盤組_電動缸無干涉);
                            } else { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_電動缸組_抽針嘴_3D掃描_回安全位);
                            }
                        }
                        Xavier_Task4_Debugprintf("tp4Home_電動缸組_抽針嘴_3D掃描_回安全位\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Home_告知載盤組_電動缸無干涉:
                        {
                            btp4Home_告知載盤組_電動缸無干涉 = true;

                            UIHelper.SetControlProperty(en_IAI,     () => en_IAI.Checked = false);
                            UIHelper.SetControlProperty(en_JoDell植針嘴相機,     () => en_JoDell植針嘴相機.Checked = false);
                            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 0); 
                            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,  0); 
                            clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaI_MotorOn, 0); 

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT+100, xeXavier_T4_Job.tp4Home_電動缸組_IAI相機_植針相機_回home);
                        }
                        Xavier_Task4_Debugprintf("tp4Home_告知載盤組_電動缸無干涉\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Home_電動缸組_IAI相機_植針相機_回home:
                        {
                            UIHelper.SetControlProperty(en_IAI,     () => en_IAI.Checked = true);
                            UIHelper.SetControlProperty(en_JoDell植針嘴相機,     () => en_JoDell植針嘴相機.Checked = true);
                            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_BrakeOff, 1);               
                            clsServoControlWMX3.WMX3_IAI(addr_IAI.pxeaI_MotorOn,  1);                
                            clsServoControlWMX3.WMX3_JoDell植針嘴相機(addr_JODELL.pxeaI_MotorOn, 1); 

                            Thread.Sleep(100);
                            dbapiIAI(dbIAI_Home位);

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT+500, xeXavier_T4_Job.tp4Home_確認電動缸組_抽針嘴_3D掃描_可以進行復歸動作_從_tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作);
                        }
                        Xavier_Task4_Debugprintf("tp4Hotp4Home_電動缸組_IAI相機_植針相機_回homemeSTART\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Home_確認電動缸組_抽針嘴_3D掃描_可以進行復歸動作_從_tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作:
                        {
                            double dbIAIpos = dbapiIAI(dbRead);

                            if( (btp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作 == true) &&
                                (dbIAIpos == 0) ) { 
                                UIHelper.SetControlProperty(en_JoDell吸針嘴,     () => en_JoDell吸針嘴.Checked = false);
                                UIHelper.SetControlProperty(en_JoDell3D掃描,     () => en_JoDell3D掃描.Checked = false);
                                clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, 0);
                                clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, 0); 

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_電動缸組_抽針嘴_3D掃描_回home);
                            } else { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_確認電動缸組_抽針嘴_3D掃描_可以進行復歸動作_從_tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作);
                            }
                        }
                        Xavier_Task4_Debugprintf("tp4Home_確認電動缸組_抽針嘴_3D掃描_可以進行復歸動作_從_tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Home_電動缸組_抽針嘴_3D掃描_回home:
                        {
                            UIHelper.SetControlProperty(en_JoDell3D掃描,     () => en_JoDell3D掃描.Checked = true);
                            UIHelper.SetControlProperty(en_JoDell吸針嘴,     () => en_JoDell吸針嘴.Checked = true);
                            clsServoControlWMX3.WMX3_JoDell3D掃描(addr_JODELL.pxeaI_MotorOn, 1); 
                            clsServoControlWMX3.WMX3_JoDell吸針嘴(addr_JODELL.pxeaI_MotorOn, 1);  

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_告知電動缸組已回home完畢);
                        }
                        Xavier_Task4_Debugprintf("tp4Home_電動缸組_抽針嘴_3D掃描_回home\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Home_告知電動缸組已回home完畢:
                        {
                            btp4Home_告知電動缸組已回home完畢 = true;

                            dbapiIAI(dbIAI_預備位);
                            dbapiJoDell3D掃描(dbJoDell3D掃描_Home位);
                            dbapiJoDell吸針嘴(dbJoDell吸針嘴_Home位);
                            dbapiJoDell植針嘴相機(dbJoDell植針嘴相機_Home位);

                            if(btp6Home_告知系統回home完畢 == true) { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4START);
                            } else { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32HomeDelayCNT, xeXavier_T4_Job.tp4Home_告知電動缸組已回home完畢);
                            }
                        }
                        Xavier_Task4_Debugprintf("tp4Home_告知電動缸組已回home完畢\r\n");
                        break;

                case xeXavier_T4_Job.tp4TakeAndDiscardSTART:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4TakeAndDiscardSTART);
                    Xavier_Task4_Debugprintf("tp4TakeAndDiscardSTART\r\n");
                    break;

                case xeXavier_T4_Job.tp4InsertSTART:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_電動缸組_抽針嘴_3D掃描_回安全位);
                    Xavier_Task4_Debugprintf("tp4InsertSTART\r\n");
                    break;
                    case xeXavier_T4_Job.tp4Insert_電動缸組_抽針嘴_3D掃描_回安全位:
                        {
                            dbapiJoDell吸針嘴(dbJoDell吸針嘴_Home位);
                            dbapiJoDell3D掃描(dbJoDell3D掃描_Home位);

                            double dbrsltJoDell3D掃描 = dbapiJoDell3D掃描(dbCheckArrived);
                            double dbrsltJoDell吸針嘴 = dbapiJoDell吸針嘴(dbCheckArrived);
                            if( dbrsltJoDell3D掃描 == dbAxisMoveOk &&
                                dbrsltJoDell吸針嘴 == dbAxisMoveOk ) {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_告知載盤組_電動缸無干涉);
                            } else { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_電動缸組_抽針嘴_3D掃描_回安全位);
                            }
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_電動缸組_抽針嘴_3D掃描_回安全位\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_告知載盤組_電動缸無干涉:
                        {
                            btp4Insert_告知載盤組_電動缸無干涉 = true;

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_Socket孔檢測相機移至拍照位);
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_告知載盤組_電動缸無干涉\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_Socket孔檢測相機移至拍照位:
                        {
                            double dbSocketCamera; {
                                dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                                dbapiIAI(dbSocketCamera);
                            }

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_告知Socket孔檢測相機已至拍照位);
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_Socket孔檢測相機移至拍照位\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_告知Socket孔檢測相機已至拍照位:
                        {
                            btp4Insert_告知Socket孔檢測相機已至拍照位 = true;

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_堵料檢查植針嘴相機移至拍照位);
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_告知Socket孔檢測相機已至拍照位\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_堵料檢查植針嘴相機移至拍照位:
                        {
                            double CheckCameraZ; {
                                CheckCameraZ = apiParaReadIndex("SaveParameterJason.json", 32);
                            }
                            dbapiJoDell植針嘴相機(CheckCameraZ);

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_告知堵料檢查植針嘴相機已至拍照位);
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_堵料檢查植針嘴相機移至拍照位\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_告知堵料檢查植針嘴相機已至拍照位:
                        {
                            btp4Insert_告知堵料檢查植針嘴相機已至拍照位 = true;

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤檢測機制開始);
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_告知堵料檢查植針嘴相機已至拍照位\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_柔震盤檢測機制開始:
                        {
                            //Vibration LED
                            clsVibration.apiEstablishTCPVibration(); {
                                clsVibration.u32LED_Level = (uint)SB_VBLED.Value;
                                clsVibration.SetVibrationLED(clsVibration.u32LED_Level);
                                UIHelper.SetControlProperty(lblVBLED,    () => lblVBLED.Text = "Light:" + (uint)SB_VBLED.Value);
                            }

                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_柔震盤檢測機制開始\r\n");
                        break;
                    case xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料:
                        {
                            if(btp6Insert_告知系統已拿到目標植針資料_To_Tp4 == true) { 
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp4 = false;

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_有植針資料);
                            } else if(btp6Insert_告知系統無目標植針資料_To_Tp4 == true) { 
                                btp6Insert_告知系統無目標植針資料_To_Tp4 = false;

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_無植針資料);
                            } else { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                            }
                        }
                        Xavier_Task4_Debugprintf("tp4Insert_進行柔震盤物料確認_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料\r\n");
                        break;
                        case xeXavier_T4_Job.tp4Insert_無植針資料:
                            { 
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤檢測機制完成);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_無植針資料\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_有植針資料:
                            {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料拍照前作業);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_有植針資料\r\n");
                            break;                           
                        case xeXavier_T4_Job.tp4Insert_進行柔震盤物料拍照前作業:
                            {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_進行柔震盤物料拍照前作業\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照:
                            {
                                if(btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照 == true) { 
                                    btp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照 = false;

                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數重設);
                                } else {
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照);
                                }
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_進行柔震盤物料確認_從_tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數重設:
                            {
                                i柔震盤物料檢測retry次數 = 306789527;

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤物料檢測retry次數重設\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數:
                            {
                                if(i柔震盤物料檢測retry次數 > 0) { 
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數大於0);
                                } else 
                                if(i柔震盤物料檢測retry次數 == 0) { 
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數等於0);
                                }
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤物料檢測retry次數\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數等於0:
                            {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料異常_告知系統中止);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤物料檢測retry次數等於0\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤物料異常_告知系統中止:
                            //預設Retry次數到達後異常停止
                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料異常_告知系統中止);
                            
                            //料過Retry次數異常停, 繼續植針
                            //Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料確認_從_tp2Insert_告知電動缸組吸嘴軸組不干擾柔震取料拍照);
                            
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤物料異常_告知系統中止\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數大於0:
                            {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_檢查柔震是否有物料);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤物料檢測retry次數大於0\r\n");
                            break;                                                
                        case xeXavier_T4_Job.tp4Insert_檢查柔震是否有物料:
                            {
                                UIHelper.RunOnUIThread(this, () => { btn_取得PinInfo_Click(null, EventArgs.Empty); });
                                if(b柔震盤有料_tmrTakePinTick == true) { 
                                    //柔震有料
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤有物料);
                                } else { 
                                    //柔震無料
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤無物料);
                                }
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_檢查柔震是否有物料\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤有物料:
                            {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_告知吸嘴軸組柔震盤物料座標);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤有物料\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤無物料:
                            {
                                //先啟動震盤料倉震動
                                i柔震TypeStep = 1;

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤啟動震動);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤無物料\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤啟動震動:
                            {
                                switch(i柔震TypeStep) { 
                                    case 1:
                                        //柔震盤料倉震動
                                        UIHelper.SetControlProperty(lbl震散,    () => lbl震散.BackColor   = Color.Green);
                                        UIHelper.SetControlProperty(lbl上下收,    () => lbl上下收.BackColor = Color.Green);
                                        UIHelper.SetControlProperty(lbl左右收,    () => lbl左右收.BackColor = Color.Green);
                                        UIHelper.SetControlProperty(lbl料倉,    () => lbl料倉.BackColor   = Color.Red);
                                        UIHelper.RunOnUIThread(this, () => { btnVibrationInit_Click(null, EventArgs.Empty); });

                                        i柔震TypeStep = 2;  //柔震盤上下震動:

                                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT+10, xeXavier_T4_Job.tp4Insert_柔震盤啟動震動);
                                        break; 

                                    case 2:
                                        //柔震盤上下震動: 
                                        UIHelper.SetControlProperty(lbl震散,    () => lbl震散.BackColor   = Color.Green);
                                        UIHelper.SetControlProperty(lbl上下收,    () => lbl上下收.BackColor = Color.Red);
                                        UIHelper.SetControlProperty(lbl左右收,    () => lbl左右收.BackColor = Color.Green);
                                        UIHelper.SetControlProperty(lbl料倉,    () => lbl料倉.BackColor   = Color.Green);
                                        UIHelper.RunOnUIThread(this, () => { btnVibrationInit_Click(null, EventArgs.Empty); });

                                        i柔震TypeStep = 3;  //柔震盤左右震動:

                                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT+25, xeXavier_T4_Job.tp4Insert_柔震盤啟動震動);
                                        break; 

                                    case 3:
                                        //柔震盤左右震動:
                                        UIHelper.SetControlProperty(lbl震散,    () => lbl震散.BackColor   = Color.Green);
                                        UIHelper.SetControlProperty(lbl上下收,    () => lbl上下收.BackColor = Color.Green); 
                                        UIHelper.SetControlProperty(lbl左右收,    () => lbl左右收.BackColor = Color.Red);
                                        UIHelper.SetControlProperty(lbl料倉,    () => lbl料倉.BackColor   = Color.Green);
                                        UIHelper.RunOnUIThread(this, () => { btnVibrationInit_Click(null, EventArgs.Empty); });

                                        i柔震TypeStep = 4;  //柔震盤散震震動

                                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT+25, xeXavier_T4_Job.tp4Insert_柔震盤啟動震動);
                                        break; 

                                    case 4:
                                        //柔震盤散震震動:
                                        UIHelper.SetControlProperty(lbl震散,    () => lbl震散.BackColor   = Color.Red);
                                        UIHelper.SetControlProperty(lbl上下收,    () => lbl上下收.BackColor = Color.Green);
                                        UIHelper.SetControlProperty(lbl左右收,    () => lbl左右收.BackColor = Color.Green);
                                        UIHelper.SetControlProperty(lbl料倉,    () => lbl料倉.BackColor   = Color.Green);
                                        UIHelper.RunOnUIThread(this, () => { btnVibrationInit_Click(null, EventArgs.Empty); });

                                        i柔震TypeStep = 0;  //柔震盤停止

                                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT+25, xeXavier_T4_Job.tp4Insert_柔震盤啟動震動);
                                        break;

                                    default:
                                    case 0:
                                        //柔震盤停止:
                                        UIHelper.RunOnUIThread(this, () => { btnVibrationStop_Click(null, EventArgs.Empty); });
                                        
                                        i柔震TypeStep = 0;  //柔震盤停止

                                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT+25, xeXavier_T4_Job.tp4Insert_柔震盤停止震動);
                                        break;
                                }
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤啟動震動\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_柔震盤停止震動:
                            {
                                i柔震盤物料檢測retry次數--;

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_跳回_至_tp4Insert_柔震盤物料檢測retry次數);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_柔震盤停止震動\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_跳回_至_tp4Insert_柔震盤物料檢測retry次數:
                            {
                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤物料檢測retry次數);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_跳回_至_tp4Insert_柔震盤物料檢測retry次數\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_告知吸嘴軸組柔震盤物料座標:
                            {
                                btp4Insert_告知吸嘴軸組柔震盤物料座標 = true;

                                Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_清除tp4Insert_告知吸嘴軸組柔震盤有物料_從_tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標);
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_告知吸嘴軸組柔震盤物料座標\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_清除tp4Insert_告知吸嘴軸組柔震盤有物料_從_tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標:
                            {
                                if(btp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標 == true) { 
                                    btp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標 = false;

                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_跳回_至_tp4Insert_進行柔震盤物料拍照前作業);
                                } else { 
                                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_清除tp4Insert_告知吸嘴軸組柔震盤有物料_從_tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標);
                                }
                            }
                            Xavier_Task4_Debugprintf("tp4Insert_清除tp4Insert_告知吸嘴軸組柔震盤有物料_從_tp2Insert_告知電動缸組_已取出當前柔震盤目標物料座標\r\n");
                            break;
                        case xeXavier_T4_Job.tp4Insert_跳回_至_tp4Insert_進行柔震盤物料拍照前作業:
                            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_進行柔震盤物料拍照前作業);
                            Xavier_Task4_Debugprintf("tp4Insert_跳回_至_tp4Insert_進行柔震盤物料拍照前作業\r\n");
                            break;
                    case xeXavier_T4_Job.tp4Insert_柔震盤檢測機制完成:
                        Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4Insert_柔震盤檢測機制完成);
                        Xavier_Task4_Debugprintf("tp4Insert_柔震盤檢測機制完成\r\n");
                        break;

                case xeXavier_T4_Job.tp4RemoveSTART:
                    Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, u32InsertDelayCNT, xeXavier_T4_Job.tp4RemoveSTART);
                    Xavier_Task4_Debugprintf("tp4RemoveSTART\r\n");
                    break;

                default:
                    break;
            }

            Xavier_Task4_proc(xeXavier_T4_proc.pt4SET, xeXavier_T4_Job.tp4Idle);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_T4_delayCase(xeXavier_T4_proc deJob, uint delayCNT, xeXavier_T4_Job excuteJob) {
            switch (deJob) {
                case xeXavier_T4_proc.pt4SET:
                    Xavier_T4_dC_decdelayCNT = delayCNT + 2;
                    Xavier_T4_dC_GetInJob = excuteJob;
                    break;

                case xeXavier_T4_proc.pt4Interrupt:
                    if (Xavier_T4_dC_GetInJob != excuteJob) {
                        Xavier_Task4_ISR_CNTTmp(xeXavier_T4_proc.pt4SET, (xeXavier_T4_Job)Xavier_T4_dC_decdelayCNT);
                        Xavier_Task4_ISR_JobTmp(xeXavier_T4_proc.pt4SET, Xavier_T4_dC_GetInJob);

                        Xavier_T4_dC_GetInJob = excuteJob;
                        Xavier_T4_dC_decdelayCNT = 2;  // equal to excute pt4deExcute to get Xavier_Task4_proc(pt4SET,GetInJob);
                    }
                    break;

                case xeXavier_T4_proc.pt4ResISR:
                    Xavier_T4_dC_decdelayCNT = (uint)Xavier_Task4_ISR_CNTTmp(xeXavier_T4_proc.pt4GET, Xavier_T4_dC_GetInJob) + 2;
                    Xavier_T4_dC_GetInJob    =       Xavier_Task4_ISR_JobTmp(xeXavier_T4_proc.pt4GET, Xavier_T4_dC_GetInJob);

                    Xavier_Task4_ISR_CNTTmp(xeXavier_T4_proc.pt4SET, (xeXavier_T4_Job)2);
                    Xavier_Task4_ISR_JobTmp(xeXavier_T4_proc.pt4SET, xeXavier_T4_Job.tp4Empty);
                    break;

                case xeXavier_T4_proc.pt4deExcute:
                    if (Xavier_T4_dC_decdelayCNT > 0) {
                        Xavier_T4_dC_decdelayCNT--;
                    }

                    if (Xavier_T4_dC_decdelayCNT == 1) {
                        Xavier_Task4_proc(xeXavier_T4_proc.pt4SET, Xavier_T4_dC_GetInJob);
                    }
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T4_Job Xavier_Task4_proc(xeXavier_T4_proc rtFun, xeXavier_T4_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T4_proc.pt4SET:
                    Xavier_Task4_p_ret = ptValue;
                    break;

                case xeXavier_T4_proc.pt4GET:
                    break;
            }

            return Xavier_Task4_p_ret;
        }
        //---------------------------------------------------------------------------------------
        public void Task4CallJob(xeXavier_T4_Job excuteJob) {
            Xavier_T4_delayCase(xeXavier_T4_proc.pt4Interrupt, 0, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task4CallJobWithDelay(xeXavier_T4_Job excuteJob, uint delayCNT) {
            Xavier_T4_delayCase(xeXavier_T4_proc.pt4SET, delayCNT, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task4ResumeJob() {
            Xavier_T4_delayCase(xeXavier_T4_proc.pt4ResISR, 0, 0);
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T4_Job Xavier_Task4_ISR_JobTmp(xeXavier_T4_proc rtFun, xeXavier_T4_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T4_proc.pt4SET:
                    Xavier_Task4_ISR_JT_retmp = ptValue;
                    break;

                case xeXavier_T4_proc.pt4GET:
                    break;
            }

            return Xavier_Task4_ISR_JT_retmp;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T4_Job Xavier_Task4_ISR_CNTTmp(xeXavier_T4_proc rtFun, xeXavier_T4_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T4_proc.pt4SET:
                    Xavier_Task4_ISR_CT_retmp = ptValue;
                    break;

                case xeXavier_T4_proc.pt4GET:
                    break;
            }

            return Xavier_Task4_ISR_CT_retmp;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Debug Method----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task4_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task4_Info, () => lbldbg_Task4_Info.Text = message);

            XavierLogger.Log("Task4", message);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T4 -------------------------------
        //---------------------------------------------------------------------------------------
        #endregion



        #region XavierTaskFlowEngine_T5
        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T5 -------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Private Variables---------
        public static int iRetrySocket孔無法植針檢查     = 0;
        public static int iRetrySocket孔無法植針檢查次數 = 2;

        MX PerspectiveTransformMatrix = new MX();
        double dbPinHolePositionX = 0.0;
        double dbPinHolePositionY = 0.0;

        Stopwatch stopwatch = new Stopwatch();

        // ----------Global Variables----------
        public static uint Xavier_T5_dC_decdelayCNT  = 0;
        public static xeXavier_T5_Job Xavier_T5_dC_GetInJob     = 0;
        public static xeXavier_T5_Job Xavier_Task5_p_ret        = 0;
        public static xeXavier_T5_Job Xavier_Task5_ISR_JT_retmp = xeXavier_T5_Job.tp5_ISR01_START;
        public static xeXavier_T5_Job Xavier_Task5_ISR_CT_retmp = xeXavier_T5_Job.tp5_ISR01_START;

        // ----------Enumerations----------
        public enum xeXavier_T5_proc {
            pT5SET = 1,
            pT5GET,
            pT5Interrupt,
            pT5ResISR,
            pT5deExcute,
        }

        public enum xeXavier_T5_Job {
            tp5Empty = 0,
            tp5Init,
            
            tp5_ISR01_START,
            tp5_ISR01_STEP1,
            tp5_ISR01_STEP2,
            tp5_ISR01_END,

            tp5_ISR02_START,
            tp5_ISR02_STEP1,
            tp5_ISR02_STEP2,
            tp5_ISR02_END,
            
            tp5Idle,
            tp5START,  //判斷動作種類

            //載盤組
            tp5HomeSTART,
                tp5Home_確認載盤組可以進行復歸動作_從_tp6Home_告知工作門已關閉,
                tp5Home_確認載盤組可以進行復歸動作_從_tp3Home_告知載盤組_植針軸組無干涉,
                tp5Home_確認載盤組可以進行復歸動作_從_tp4Home_告知載盤組_電動缸無干涉,
                tp5Home_載盤組XY復歸,
                tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作,
                tp5Home_告知載盤組已回home完畢,

            tp5TakeAndDiscardSTART,

            tp5InsertSTART,
                tp5Insert_載盤與Soket吸真空,
                tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp3Insert_告知載盤組_植針軸組無干涉,
                tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知載盤組_電動缸無干涉,
                tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知Socket孔檢測相機已至拍照位,
                tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp6Insert_告知載盤組已拿到兩點校正資料,
                tp5Insert_開始載盤組XY兩點校正程序,
                    tp5Insert_載盤組XY移動至兩點校正孔第1點,
                        tp5Insert_載盤組XY取得兩點校正孔第1點校正參數,
                        tp5Insert_載盤組XY移動至兩點校正孔第1點補正位,       
                        tp5Insert_儲存兩點校正孔第1點補正值,     
                    tp5Insert_載盤組XY移動至兩點校正孔第2點,
                        tp5Insert_載盤組XY取得兩點校正孔第2點校正參數,
                        tp5Insert_載盤組XY移動至兩點校正孔第2點補正位,    
                        tp5Insert_儲存兩點校正孔第2點補正值,  
                    tp5Insert_告知檔案組已完成兩點校正,
                tp5Insert_完成載盤組XY兩點校正程序,
                tp5Insert_載盤植針前置作業,
                tp5Insert_確認進行載盤植針位定位_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料,
                tp5Insert_有植針資料,                                    tp5Insert_無植針資料,
                tp5Insert_載盤組移至植針拍照位,
                tp5Insert_載盤組進行植針拍照位補正,
                tp5Insert_載盤組移至植針位,
                tp5Insert_告知植針軸組載盤組已移至植針位,
                tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢,
                tp5Insert_載盤組移至植針拍照位檢查植針況狀,
                tp5Insert_載盤組進行拍照位檢查植針況狀,
                tp5Insert_植針成功,                                     tp5Insert_植針失敗,
                tp5Insert_告知系統植針成功,                             tp5Insert_告知系統植針失敗,
                tp5Insert_跳回_至_tp5Insert_載盤植針前置作業,           tp5Insert_等待是否進行堵料補光_從_tp3Insert_告知載盤組進行補光_或_tp3Insert_告知系統賭料排除異常_告知系統中止,
                                                                        tp5Insert_載盤組移至補光位,                                                             tp5Insert_植針異常停止_告知系統停止,
                                                                        tp5Insert_告知載盤組已至補光位,
                                                                        tp5Insert_等待堵料檢查結果_從_tp3Insert_告知植針軸組判斷未堵料_或_tp3Insert_告知植針軸組判斷堵料,
                                                                        tp5Insert_得知植針嘴已堵料,                                                             tp5Insert_得知植針嘴未堵料,
                                                                        tp5Insert_載盤組XY移動至堵料收廢料位,                                                   tp5Insert_跳回_至_tp5Insert_載盤組移至植針拍照位,
                                                                        tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位,
                                                                        tp5Insert_堵料排除完成_從_tp3Insert_告知植針軸組堵料吹氣完畢,
                                                                        tp5Insert_跳回_至_tp5Insert_告知系統植針失敗_從_tp3Insert_告知植針軸組堵料吹氣完畢,
                                                                        tp5Insert_完成載盤植針,
            tp5InsertEND,

            tp5RemoveSTART,
        }

        // ----------Methods----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_TASK5() {  //載盤組
            xeXavier_T5_Job priTASK = 0;
            Xavier_T5_delayCase(xeXavier_T5_proc.pT5deExcute, (uint)xeXavier_T5_Job.tp5Empty, xeXavier_T5_Job.tp5Empty);
            priTASK = Xavier_Task5_proc(xeXavier_T5_proc.pT5GET, 0);

            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;

                case xeXavier_Indicator.xeXI_狀態_停止:
                case xeXavier_Indicator.xeXI_狀態_急停:
                case xeXavier_Indicator.xeXI_事件_暫停:
                case xeXavier_Indicator.xeXI_事件_異常: 
                default:
                    priTASK = xeXavier_T5_Job.tp5START;
                    break;
            }

            if( (xeXavier_T5_Job.tp5InsertSTART <= priTASK) &&
                (priTASK <= xeXavier_T5_Job.tp5InsertEND) ) {
                iTask5_CNT = (int)priTASK;
            }

            switch (priTASK) {
                case xeXavier_T5_Job.tp5Empty:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Init);
                    Xavier_Task5_Debugprintf("tp5Empty\r\n");
                    break;

                case xeXavier_T5_Job.tp5Init:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5START);
                    Xavier_Task5_Debugprintf("tp5Init\r\n");
                    break;

                //======ISR Job======
                case xeXavier_T5_Job.tp5_ISR01_START:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5_ISR01_STEP1);
                    Xavier_Task5_Debugprintf("tp5_ISR01_START\r\n");
                    break;

                case xeXavier_T5_Job.tp5_ISR01_STEP1:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5_ISR01_STEP2);
                    Xavier_Task5_Debugprintf("tp5_ISR01_STEP1\r\n");
                    break;

                case xeXavier_T5_Job.tp5_ISR01_STEP2:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5_ISR01_END);
                    Xavier_Task5_Debugprintf("tp5_ISR01_STEP2\r\n");
                    break;

                case xeXavier_T5_Job.tp5_ISR01_END:
                    //Xavier_Task5_ISR_CNTTmp(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT);
                    //Xavier_Task5_ISR_JobTmp(xeXavier_T5_proc.pT5SET, xeXavier_T5_Job.tp5STEP2);

                    Task5ResumeJob();
                    //Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp5_ISR);  //尚未加入此TASK ISR
                    Xavier_Task5_Debugprintf("tp5_ISR01_end\r\n");
                    break;
                //======ISR Job======
                case xeXavier_T5_Job.tp5_ISR02_START:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5_ISR02_STEP1);
                    Xavier_Task5_Debugprintf("tp5_ISR02_START\r\n");
                    break;

                case xeXavier_T5_Job.tp5_ISR02_STEP1:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5_ISR02_STEP2);
                    Xavier_Task5_Debugprintf("tp5_ISR02_STEP1\r\n");
                    break;

                case xeXavier_T5_Job.tp5_ISR02_STEP2:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5_ISR02_END);
                    Xavier_Task5_Debugprintf("tp5_ISR02_STEP2\r\n");
                    break;

                case xeXavier_T5_Job.tp5_ISR02_END:
                    if(eNeedleType == xeXavier_NeedleType.pT6Place) {
                        //檔案為植針檔案

                    } else {
                        //檔案為取針檔案
                        Xavier_Task5_ISR_CNTTmp(xeXavier_T5_proc.pT5SET, (xeXavier_T5_Job)u32ISRDelayCNT);
                        Xavier_Task5_ISR_JobTmp(xeXavier_T5_proc.pT5SET, xeXavier_T5_Job.tp5RemoveSTART);
                    }

                    Task5ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp5_ISR);
                    Xavier_Task5_Debugprintf("tp5_ISR02_end\r\n");
                    break;
                //======ISR Job======
                
                case xeXavier_T5_Job.tp5Idle:  //reserve
                    break;

                case xeXavier_T5_Job.tp5START:  //判斷動作種類
                    { 
                        xeXavier_Indicator rslt = apiGetMachineAction();
                        switch(rslt) {
                            case xeXavier_Indicator.xeXI_狀態_運行: 
                                if(btp6Home_告知系統回home完畢 == true) {
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5InsertSTART);
                                } else {
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5HomeSTART);
                                }
                                break;
                            case xeXavier_Indicator.xeXI_狀態_停止:
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5START);
                                break;
                            case xeXavier_Indicator.xeXI_狀態_急停:
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5START);
                                break;

                            case xeXavier_Indicator.xeXI_事件_復歸:
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5HomeSTART);
                                break;
                            case xeXavier_Indicator.xeXI_事件_暫停:    break;
                            case xeXavier_Indicator.xeXI_事件_異常:    break;

                            default:
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32ISRDelayCNT, xeXavier_T5_Job.tp5START);
                                break;
                        }
                    }
                    Xavier_Task5_Debugprintf("tp5START\r\n");
                    break;

                case xeXavier_T5_Job.tp5HomeSTART:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                    Xavier_Task5_Debugprintf("tp5HomeSTART\r\n");
                    break;
                    case xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp6Home_告知工作門已關閉:
                        if(btp6Home_告知工作門已關閉 == true) { 
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp3Home_告知載盤組_植針軸組無干涉);
                        } else { 
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp6Home_告知工作門已關閉);
                        }
                        Xavier_Task5_Debugprintf("tp5Home_確認載盤組可以進行復歸動作_從_tp6Home_告知工作門已關閉\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp3Home_告知載盤組_植針軸組無干涉:
                        if(btp3Home_告知載盤組_植針軸組無干涉 == true) { 
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp4Home_告知載盤組_電動缸無干涉);
                        } else { 
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp3Home_告知載盤組_植針軸組無干涉);
                        }
                        Xavier_Task5_Debugprintf("tp5Home_確認載盤組可以進行復歸動作_從_tp3Home_告知載盤組_植針軸組無干涉\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp4Home_告知載盤組_電動缸無干涉:
                        if(btp4Home_告知載盤組_電動缸無干涉 == true) { 
                            UIHelper.SetControlProperty(en_載盤X軸,     () => en_載盤X軸.Checked = true);
                            UIHelper.SetControlProperty(en_載盤Y軸,     () => en_載盤Y軸.Checked = true);
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤X軸, true);
                            clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.載盤Y軸, true);

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_載盤組XY復歸);
                        } else { 
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_確認載盤組可以進行復歸動作_從_tp4Home_告知載盤組_電動缸無干涉);
                        }
                        Xavier_Task5_Debugprintf("tp5Home_確認載盤組可以進行復歸動作_從_tp4Home_告知載盤組_電動缸無干涉\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Home_載盤組XY復歸:
                        {
                            dbapiCarrierX_defaultSpeed(dbCarrierX_Home位);
                            dbapiCarrierY_defaultSpeed(dbCarrierY_Home位);
                            if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_載盤組XY復歸);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Home_載盤組XY復歸\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作:
                        {
                            btp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作 = true;

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_告知載盤組已回home完畢);
                        }
                        Xavier_Task5_Debugprintf("tp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Home_告知載盤組已回home完畢:
                        {
                            btp5Home_告知載盤組已回home完畢 = true;

                            if (btp6Home_告知系統回home完畢 == true) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5START);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32HomeDelayCNT, xeXavier_T5_Job.tp5Home_告知載盤組已回home完畢);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Home_告知載盤組已回home完畢\r\n");
                        break;

                case xeXavier_T5_Job.tp5TakeAndDiscardSTART:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5TakeAndDiscardSTART);
                    Xavier_Task5_Debugprintf("tp5TakeAndDiscardSTART\r\n");
                    break;

                case xeXavier_T5_Job.tp5InsertSTART:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤與Soket吸真空);
                    Xavier_Task5_Debugprintf("tp5InsertSTART\r\n");
                    break;
                    case xeXavier_T5_Job.tp5Insert_載盤與Soket吸真空:
                        {
                            digitalWrite((int)WMX3IO對照.pxeIO_載盤真空閥,  HIGH);
                            digitalWrite((int)WMX3IO對照.pxeIO_Socket真空1, HIGH);
                            digitalWrite((int)WMX3IO對照.pxeIO_Socket真空2, HIGH);

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp3Insert_告知載盤組_植針軸組無干涉);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤與Soket吸真空\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp3Insert_告知載盤組_植針軸組無干涉:
                        {
                            if(btp3Insert_告知載盤組_植針軸組無干涉 == true) { 
                                btp3Insert_告知載盤組_植針軸組無干涉 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知載盤組_電動缸無干涉);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp3Insert_告知載盤組_植針軸組無干涉);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp3Insert_告知載盤組_植針軸組無干涉\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知載盤組_電動缸無干涉:
                        {
                            if(btp4Insert_告知載盤組_電動缸無干涉 == true) { 
                                btp4Insert_告知載盤組_電動缸無干涉 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知Socket孔檢測相機已至拍照位);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知載盤組_電動缸無干涉);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知載盤組_電動缸無干涉\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知Socket孔檢測相機已至拍照位:
                        {
                            if(btp4Insert_告知Socket孔檢測相機已至拍照位 == true) { 
                                btp4Insert_告知Socket孔檢測相機已至拍照位 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp6Insert_告知載盤組已拿到兩點校正資料);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知Socket孔檢測相機已至拍照位);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp4Insert_告知Socket孔檢測相機已至拍照位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp6Insert_告知載盤組已拿到兩點校正資料:
                        {
                            if(btp6Insert_告知載盤組已拿到兩點校正資料 == true) { 
                                btp6Insert_告知載盤組已拿到兩點校正資料 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_開始載盤組XY兩點校正程序);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp6Insert_告知載盤組已拿到兩點校正資料);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_確認載盤組可以移動至兩點校正孔第1孔_從_tp6Insert_告知載盤組已拿到兩點校正資料\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_開始載盤組XY兩點校正程序:
                        {                    
                            //開啟參數表視窗
                            UIHelper.RunOnUIThread(this, () => { btn_參數_Click(null, EventArgs.Empty); });

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第1點);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_開始載盤組XY兩點校正程序\r\n");
                        break;
                        case xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第1點:
                            {
                                //Get Real Pxy
                                double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                                double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                                double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                                double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                                double dbTargetX = rlAx;
                                double dbTargetY = rlAy;
                                dbapiCarrierX_InsertSpeed(dbTargetX);
                                dbapiCarrierY_InsertSpeed(dbTargetY);

                                if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY取得兩點校正孔第1點校正參數);
                                } else { 
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第1點);
                                }
                            }
                            Xavier_Task5_Debugprintf("tp5Insert_載盤組XY移動至兩點校正孔第1點\r\n");
                            break;
                            case xeXavier_T5_Job.tp5Insert_載盤組XY取得兩點校正孔第1點校正參數:
                                {
                                    UIHelper.RunOnUIThread(this, () => { btn_socket相機兩點定位_Click(null, EventArgs.Empty); });

                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第1點補正位);
                                }
                                Xavier_Task5_Debugprintf("tp5Insert_載盤組XY取得兩點校正孔第1點校正參數\r\n");
                                break;
                            case xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第1點補正位:
                                {
                                    //Get Real Pxy
                                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                                    double dbTargetX = rlAx - dbCameraCalibrationX;
                                    double dbTargetY = rlAy + dbCameraCalibrationY;

                                    UIHelper.SetControlProperty(fmParameterFormHandle.dataGridView1, () => 
                                        {
                                            fmParameterFormHandle.dataGridView1.Rows[0].Cells[1].Value = dbTargetX;
                                            fmParameterFormHandle.dataGridView1.Rows[1].Cells[1].Value = dbTargetY;
                                        }
                                    );

                                    dbapiCarrierX_InsertSpeed(dbTargetX);
                                    dbapiCarrierY_InsertSpeed(dbTargetY);

                                    if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_儲存兩點校正孔第1點補正值);
                                    } else { 
                                        Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第1點補正位);
                                    }
                                }
                                Xavier_Task5_Debugprintf("tp5Insert_載盤組XY移動至兩點校正孔第1點補正位\r\n");
                                break;       
                            case xeXavier_T5_Job.tp5Insert_儲存兩點校正孔第1點補正值:
                                {
                                    UIHelper.RunOnUIThread(fmParameterFormHandle, () => { fmParameterFormHandle.btn_Save_Click(null, EventArgs.Empty); });

                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第2點);
                                }
                                Xavier_Task5_Debugprintf("tp5Insert_儲存兩點校正孔第1點補正值\r\n");
                                break;     
                        case xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第2點:
                            {
                                //Get Real Pxy
                                double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                                double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                                double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                                double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                                double dbTargetX = rlBx;
                                double dbTargetY = rlBy;
                                dbapiCarrierX_InsertSpeed(dbTargetX);
                                dbapiCarrierY_InsertSpeed(dbTargetY);

                                if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY取得兩點校正孔第2點校正參數);
                                } else { 
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第2點);
                                }
                            }
                            Xavier_Task5_Debugprintf("tp5Insert_載盤組XY移動至兩點校正孔第2點\r\n");
                            break;
                            case xeXavier_T5_Job.tp5Insert_載盤組XY取得兩點校正孔第2點校正參數:
                                {
                                    UIHelper.RunOnUIThread(this, () => { btn_socket相機兩點定位_Click(null, EventArgs.Empty); });

                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第2點補正位);
                                }
                                Xavier_Task5_Debugprintf("tp5Insert_載盤組XY取得兩點校正孔第2點校正參數\r\n");
                                break;
                            case xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第2點補正位:
                                {
                                    //Get Real Pxy
                                    double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                                    double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                                    double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                                    double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                                    double dbTargetX = rlBx - dbCameraCalibrationX;
                                    double dbTargetY = rlBy + dbCameraCalibrationY;

                                    fmParameterFormHandle.dataGridView1.Rows[2].Cells[1].Value = dbTargetX;
                                    fmParameterFormHandle.dataGridView1.Rows[3].Cells[1].Value = dbTargetY;

                                    dbapiCarrierX_InsertSpeed(dbTargetX);
                                    dbapiCarrierY_InsertSpeed(dbTargetY);

                                    if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                        (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                        Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_儲存兩點校正孔第2點補正值);
                                    } else { 
                                        Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至兩點校正孔第2點補正位);
                                    }
                                }
                                Xavier_Task5_Debugprintf("tp5Insert_載盤組XY移動至兩點校正孔第2點補正位\r\n");
                                break;    
                            case xeXavier_T5_Job.tp5Insert_儲存兩點校正孔第2點補正值:
                                {
                                    UIHelper.RunOnUIThread(fmParameterFormHandle, () => { fmParameterFormHandle.btn_Save_Click(null, EventArgs.Empty); });

                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_告知檔案組已完成兩點校正);
                                }
                                Xavier_Task5_Debugprintf("tp5Insert_儲存兩點校正孔第2點補正值\r\n");
                                break;  
                        case xeXavier_T5_Job.tp5Insert_告知檔案組已完成兩點校正:
                            {
                                btp5Insert_告知檔案組已完成兩點校正 = true;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_完成載盤組XY兩點校正程序);
                            }
                            Xavier_Task5_Debugprintf("tp5Insert_告知檔案組已完成兩點校正\r\n");
                            break;
                    case xeXavier_T5_Job.tp5Insert_完成載盤組XY兩點校正程序:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤植針前置作業);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_完成載盤組XY兩點校正程序\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_載盤植針前置作業:
                        {
                            {  // start of Matrix Calibration
                                //Get Real Pxy
                                double rlAx = apiParaReadIndex("SaveParameterJason.json", 0);
                                double rlAy = apiParaReadIndex("SaveParameterJason.json", 1);
                                double rlBx = apiParaReadIndex("SaveParameterJason.json", 2);
                                double rlBy = apiParaReadIndex("SaveParameterJason.json", 3);

                                //Get Ideal Pxy
                                double idlpAx = 0, idlpAy = 0, idlpBx = 0, idlpBy = 0;

                                string Cal2pFileName = apiParaReadStr("SaveParameterJason.json",   8);
                                int PointLeft  = (int)apiParaReadIndex("SaveParameterJason.json",  9);
                                int PointRight = (int)apiParaReadIndex("SaveParameterJason.json", 10);

                                apiReadNeedleInfo(Cal2pFileName, PointLeft,  ref idlpAx, ref idlpAy);
                                apiReadNeedleInfo(Cal2pFileName, PointRight, ref idlpBx, ref idlpBy);

                                //Calculate Cal 2p
                                {
                                    Normal calculate = new Normal();

                                    // 定義 PointA, PointB 的數據
                                    Normal.Point idealA = new Normal.Point(idlpAx, idlpAy);
                                    Normal.Point idealB = new Normal.Point(idlpBx, idlpBy);
                                    Normal.Point realA  = new Normal.Point(rlAx,   rlAy  );
                                    Normal.Point realB  = new Normal.Point(rlBx,   rlBy  );

                                    // 宣告 PointForward 和 PointBackward 變數
                                    Normal.Point idealAForward  = new Normal.Point();
                                    Normal.Point idealABackward = new Normal.Point();
                                    Normal.Point realAForward   = new Normal.Point();
                                    Normal.Point realABackward  = new Normal.Point();

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
                            }  // end of Matrix Calibration
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認進行載盤植針位定位_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤植針前置作業\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_確認進行載盤植針位定位_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料:
                        {
                            if(btp6Insert_告知系統已拿到目標植針資料_To_Tp5 == true) { 
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp5 = false;

                                iRetrySocket孔無法植針檢查 = iRetrySocket孔無法植針檢查次數;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_有植針資料);
                            } else if(btp6Insert_告知系統無目標植針資料_To_Tp5 == true) { 
                                btp6Insert_告知系統無目標植針資料_To_Tp5 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_無植針資料);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認進行載盤植針位定位_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_確認進行載盤植針位定位_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_無植針資料:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_完成載盤植針);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_無植針資料\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_有植針資料:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_有植針資料\r\n");
                        break;                                    
                    case xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位:
                        {
                            double dbTargetX = dbPinHolePositionX;
                            double dbTargetY = dbPinHolePositionY;

                            dbapiCarrierX_InsertSpeed(dbTargetX);
                            dbapiCarrierY_InsertSpeed(dbTargetY);

                            if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT+20, xeXavier_T5_Job.tp5Insert_載盤組進行植針拍照位補正);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組移至植針拍照位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_載盤組進行植針拍照位補正:
                        {
                            UIHelper.RunOnUIThread(this, () => { btn_Socket孔檢查_Click(null, EventArgs.Empty); });

                            if(cB_料盤有料.Checked == true) {
                                b有看到校正孔 = true;
                            }

                            if(b有看到校正孔 == true) { 
                                double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX;
                                double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY;

                                dbapiCarrierX_InsertSpeed(dbTargetX);
                                dbapiCarrierY_InsertSpeed(dbTargetY);

                                if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT+20, xeXavier_T5_Job.tp5Insert_載盤組移至植針位);
                                } else { 
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組進行植針拍照位補正);
                                }
                            } else { 
                                //iRetrySocket孔無法植針檢查--;
                                //if(iRetrySocket孔無法植針檢查==0) { 
                                    //拿下一筆植針孔位
                                    btp5Insert_告知系統植針成功_To_Tp6 = true;
                                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認進行載盤植針位定位_從_tp6Insert_告知系統已拿到目標植針資料_或_tp6Insert_告知系統無目標植針資料);
                                //} else { 
                                //    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組進行植針拍照位補正);
                                //}
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組進行植針拍照位補正\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_載盤組移至植針位:
                        {
                            double SetPinOffsetX, SetPinOffsetY; {
                                SetPinOffsetX = apiParaReadIndex("SaveParameterJason.json", 13);
                                SetPinOffsetY = apiParaReadIndex("SaveParameterJason.json", 14);
                            }

                            double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX + SetPinOffsetX;
                            double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY + SetPinOffsetY;

                            dbapiCarrierX_InsertSpeed(dbTargetX);
                            dbapiCarrierY_InsertSpeed(dbTargetY);

                            if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT+20, xeXavier_T5_Job.tp5Insert_告知植針軸組載盤組已移至植針位);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至植針位);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組移至植針位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_告知植針軸組載盤組已移至植針位:
                        {
                            btp5Insert_告知植針軸組載盤組已移至植針位 = true;

                            Xavier_Task5_Debugprintf("tp5Insert_告知植針軸組載盤組已移至植針位(1)\r\n");
                            while(iTask3_CNT!=(int)xeXavier_T3_Job.tp3Insert_確認植針軸可進行植針_從_tp5Insert_告知植針軸組載盤組已移至植針位) {
                                xeXavier_Indicator rslt = apiGetMachineAction();
                                if(rslt== xeXavier_Indicator.xeXI_狀態_停止) {
                                    break;
                                }
                            }

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_告知植針軸組載盤組已移至植針位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢:
                        {
                            Xavier_Task5_Debugprintf("tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢(2)\r\n");
                            while(iTask3_CNT!=(int)xeXavier_T3_Job.tp3Insert_告知載盤組_植針軸植針完畢) {
                                xeXavier_Indicator rslt = apiGetMachineAction();
                                if(rslt== xeXavier_Indicator.xeXI_狀態_停止) {
                                    break;
                                }
                            }

                            if(btp3Insert_告知載盤組_植針軸植針完畢 == true) { 
                                btp3Insert_告知載盤組_植針軸植針完畢 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位檢查植針況狀);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_確認可進行植針檢查_從_tp3Insert_告知載盤組_植針軸植針完畢\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位檢查植針況狀:
                        {
                            double dbTargetX = dbPinHolePositionX + dbCameraCalibrationX;
                            double dbTargetY = dbPinHolePositionY + dbCameraCalibrationY;

                            dbapiCarrierX_InsertSpeed(dbTargetX);
                            dbapiCarrierY_InsertSpeed(dbTargetY);
                            if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT+20, xeXavier_T5_Job.tp5Insert_載盤組進行拍照位檢查植針況狀);    
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位檢查植針況狀);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組移至植針拍照位檢查植針況狀\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_載盤組進行拍照位檢查植針況狀:
                        {
                            bool success = false;
                                double dbSetNeedleStatus; {
                                    dbSetNeedleStatus = apiParaReadIndex("SaveParameterJason.json", 36);
                                }
                                switch(dbSetNeedleStatus) { 
                                    //強制判斷植針ng
                                    case 0:  success = false;  break;

                                    //強制判斷植針ok
                                    case 1:  success = true;   break;

                                    //依照視覺判斷
                                    case 2: 
                                        UIHelper.RunOnUIThread(this, () => { btn_Socket孔檢查_Click(null, EventArgs.Empty); });
                                        {
                                            //取得校正攝影機校正參數
                                            success = inspector1.xInspSocket植針後檢查();
                                            UIHelper.SetControlProperty(label7, () => label7.Text = (success) ? "植針後檢查 OK" : "植針後檢查 NG");

                                            rtb_Status_AppendMessage(rtb_Status, $"植針 {(success ? "OK":"NG")}");
                                        }
                                        break;
                                }  // end of switch(dbSetPinStatus) { 
                            if(success == true) { 
                                //植針ok
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_植針成功);
                            } else { 
                                //植針ng
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_植針失敗);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組進行拍照位檢查植針況狀\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_植針成功:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_告知系統植針成功);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_植針成功\r\n");
                        break;                                     
                    case xeXavier_T5_Job.tp5Insert_告知系統植針成功:
                        { 
                            btp5Insert_告知系統植針成功_To_Tp6 = true;
                            btp5Insert_告知系統植針成功_To_Tp3 = true;

                            stopwatch.Stop();
                            UIHelper.SetControlProperty(lbl_CycleTime, () => lbl_CycleTime.Text = "執行時間（秒）: " + (double)(stopwatch.ElapsedMilliseconds/1000.0));
                            stopwatch.Reset();
                            stopwatch.Start();

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_跳回_至_tp5Insert_載盤植針前置作業);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_告知系統植針成功\r\n");
                        break;                             
                    case xeXavier_T5_Job.tp5Insert_跳回_至_tp5Insert_載盤植針前置作業:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤植針前置作業);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_跳回_至_tp5Insert_載盤植針前置作業\r\n");
                        break;           
                    case xeXavier_T5_Job.tp5Insert_植針失敗:
                        { 
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_告知系統植針失敗);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_植針失敗\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_告知系統植針失敗:
                        {
                            btp5Insert_告知系統植針失敗 = true;

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_等待是否進行堵料補光_從_tp3Insert_告知載盤組進行補光_或_tp3Insert_告知系統賭料排除異常_告知系統中止);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_告知系統植針失敗\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_等待是否進行堵料補光_從_tp3Insert_告知載盤組進行補光_或_tp3Insert_告知系統賭料排除異常_告知系統中止:
                        {
                            if(btp3Insert_告知載盤組進行補光 == true) { 
                                btp3Insert_告知載盤組進行補光 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至補光位);
                            } else if(btp3Insert_告知系統賭料排除異常_告知系統中止 == true) {
                                btp3Insert_告知系統賭料排除異常_告知系統中止 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_植針異常停止_告知系統停止);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_等待是否進行堵料補光_從_tp3Insert_告知載盤組進行補光_或_tp3Insert_告知系統賭料排除異常_告知系統中止);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_等待是否進行堵料補光_從_tp3Insert_告知載盤組進行補光_或_tp3Insert_告知系統賭料排除異常_告知系統中止\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_植針異常停止_告知系統停止:
                        Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_植針異常停止_告知系統停止);
                        Xavier_Task5_Debugprintf("tp5Insert_植針異常停止_告知系統停止\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_載盤組移至補光位:
                        {
                            double CheckCarryX, CheckCarryY; {
                                CheckCarryX = apiParaReadIndex("SaveParameterJason.json", 28);
                                CheckCarryY = apiParaReadIndex("SaveParameterJason.json", 29);
                            }
                            dbapiCarrierX_InsertSpeed(CheckCarryX);
                            dbapiCarrierY_InsertSpeed(CheckCarryY);

                            if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_告知載盤組已至補光位);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至補光位);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組移至補光位\r\n");
                        break;                         
                    case xeXavier_T5_Job.tp5Insert_告知載盤組已至補光位:
                        { 
                            btp5Insert_告知載盤組已至補光位 = true;

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_等待堵料檢查結果_從_tp3Insert_告知植針軸組判斷未堵料_或_tp3Insert_告知植針軸組判斷堵料);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_告知載盤組已至補光位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_等待堵料檢查結果_從_tp3Insert_告知植針軸組判斷未堵料_或_tp3Insert_告知植針軸組判斷堵料:
                        {
                            if(btp3Insert_告知植針軸組判斷未堵料 == true) { 
                                btp3Insert_告知植針軸組判斷未堵料 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_得知植針嘴未堵料);
                            } else if(btp3Insert_告知植針軸組判斷堵料 == true) { 
                                btp3Insert_告知植針軸組判斷堵料 = false;

                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_得知植針嘴已堵料);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_等待堵料檢查結果_從_tp3Insert_告知植針軸組判斷未堵料_或_tp3Insert_告知植針軸組判斷堵料);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_等待堵料檢查結果_從_tp3Insert_告知植針軸組判斷未堵料_或_tp3Insert_告知植針軸組判斷堵料\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_得知植針嘴未堵料:
                        {
                            //如果堵料排除後 要繼續下一個孔
                            btp5Insert_告知系統植針成功_To_Tp6 = true;

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_跳回_至_tp5Insert_載盤組移至植針拍照位);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_得知植針嘴未堵料\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_跳回_至_tp5Insert_載盤組移至植針拍照位:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組移至植針拍照位);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_跳回_至_tp5Insert_載盤組移至植針拍照位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_得知植針嘴已堵料:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至堵料收廢料位);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_得知植針嘴已堵料\r\n");
                        break;                         
                    case xeXavier_T5_Job.tp5Insert_載盤組XY移動至堵料收廢料位:
                        {
                            double MakeClearCarryY; {
                                MakeClearCarryY = apiParaReadIndex("SaveParameterJason.json", 34);
                            }
                            dbapiCarrierY_InsertSpeed(MakeClearCarryY);
                            if( (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_載盤組XY移動至堵料收廢料位);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_載盤組XY移動至堵料收廢料位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位:
                        {
                            btp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位 = true;

                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_堵料排除完成_從_tp3Insert_告知植針軸組堵料吹氣完畢);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_告知植針嘴組_載盤組XY已至堵料收廢料位\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_堵料排除完成_從_tp3Insert_告知植針軸組堵料吹氣完畢:
                        {
                            if(btp3Insert_告知植針軸組堵料吹氣完畢 == true) { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_跳回_至_tp5Insert_告知系統植針失敗_從_tp3Insert_告知植針軸組堵料吹氣完畢);
                            } else { 
                                Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_堵料排除完成_從_tp3Insert_告知植針軸組堵料吹氣完畢);
                            }
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_堵料排除完成_從_tp3Insert_告知植針軸組堵料吹氣完畢\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_跳回_至_tp5Insert_告知系統植針失敗_從_tp3Insert_告知植針軸組堵料吹氣完畢:
                        {
                            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_告知系統植針失敗);
                        }
                        Xavier_Task5_Debugprintf("tp5Insert_跳回_至_tp5Insert_告知系統植針失敗_從_tp3Insert_告知植針軸組堵料吹氣完畢\r\n");
                        break;
                    case xeXavier_T5_Job.tp5Insert_完成載盤植針:
                        Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5Insert_完成載盤植針);
                        Xavier_Task5_Debugprintf("tp5Insert_完成載盤植針\r\n");
                        break;

                case xeXavier_T5_Job.tp5RemoveSTART:
                    Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, u32InsertDelayCNT, xeXavier_T5_Job.tp5RemoveSTART);
                    Xavier_Task5_Debugprintf("tp5RemoveSTART\r\n");
                    break;

                default:
                    break;
            }

            Xavier_Task5_proc(xeXavier_T5_proc.pT5SET, xeXavier_T5_Job.tp5Idle);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_T5_delayCase(xeXavier_T5_proc deJob, uint delayCNT, xeXavier_T5_Job excuteJob) {
            switch (deJob) {
                case xeXavier_T5_proc.pT5SET:
                    Xavier_T5_dC_decdelayCNT = delayCNT + 2;
                    Xavier_T5_dC_GetInJob = excuteJob;
                    break;

                case xeXavier_T5_proc.pT5Interrupt:
                    if (Xavier_T5_dC_GetInJob != excuteJob) {
                        Xavier_Task5_ISR_CNTTmp(xeXavier_T5_proc.pT5SET, (xeXavier_T5_Job)Xavier_T5_dC_decdelayCNT);
                        Xavier_Task5_ISR_JobTmp(xeXavier_T5_proc.pT5SET, Xavier_T5_dC_GetInJob);

                        Xavier_T5_dC_GetInJob = excuteJob;
                        Xavier_T5_dC_decdelayCNT = 2;  // equal to excute pT5deExcute to get Xavier_Task5_proc(pT5SET,GetInJob);
                    }
                    break;

                case xeXavier_T5_proc.pT5ResISR:
                    Xavier_T5_dC_decdelayCNT = (uint)Xavier_Task5_ISR_CNTTmp(xeXavier_T5_proc.pT5GET, Xavier_T5_dC_GetInJob) + 2;
                    Xavier_T5_dC_GetInJob    =       Xavier_Task5_ISR_JobTmp(xeXavier_T5_proc.pT5GET, Xavier_T5_dC_GetInJob);

                    Xavier_Task5_ISR_CNTTmp(xeXavier_T5_proc.pT5SET, (xeXavier_T5_Job)2);
                    Xavier_Task5_ISR_JobTmp(xeXavier_T5_proc.pT5SET, xeXavier_T5_Job.tp5Empty);
                    break;

                case xeXavier_T5_proc.pT5deExcute:
                    if (Xavier_T5_dC_decdelayCNT > 0) {
                        Xavier_T5_dC_decdelayCNT--;
                    }

                    if (Xavier_T5_dC_decdelayCNT == 1) {
                        Xavier_Task5_proc(xeXavier_T5_proc.pT5SET, Xavier_T5_dC_GetInJob);
                    }
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T5_Job Xavier_Task5_proc(xeXavier_T5_proc rtFun, xeXavier_T5_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T5_proc.pT5SET:
                    Xavier_Task5_p_ret = ptValue;
                    break;

                case xeXavier_T5_proc.pT5GET:
                    break;
            }

            return Xavier_Task5_p_ret;
        }
        //---------------------------------------------------------------------------------------
        public void Task5CallJob(xeXavier_T5_Job excuteJob) {
            Xavier_T5_delayCase(xeXavier_T5_proc.pT5Interrupt, 0, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task5CallJobWithDelay(xeXavier_T5_Job excuteJob, uint delayCNT) {
            Xavier_T5_delayCase(xeXavier_T5_proc.pT5SET, delayCNT, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task5ResumeJob() {
            Xavier_T5_delayCase(xeXavier_T5_proc.pT5ResISR, 0, 0);
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T5_Job Xavier_Task5_ISR_JobTmp(xeXavier_T5_proc rtFun, xeXavier_T5_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T5_proc.pT5SET:
                    Xavier_Task5_ISR_JT_retmp = ptValue;
                    break;

                case xeXavier_T5_proc.pT5GET:
                    break;
            }

            return Xavier_Task5_ISR_JT_retmp;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T5_Job Xavier_Task5_ISR_CNTTmp(xeXavier_T5_proc rtFun, xeXavier_T5_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T5_proc.pT5SET:
                    Xavier_Task5_ISR_CT_retmp = ptValue;
                    break;

                case xeXavier_T5_proc.pT5GET:
                    break;
            }

            return Xavier_Task5_ISR_CT_retmp;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Debug Method----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task5_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task5_Info, () => lbldbg_Task5_Info.Text = message);

            XavierLogger.Log("Task5", message);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T5 -------------------------------
        //---------------------------------------------------------------------------------------
        #endregion



        #region XavierTaskFlowEngine_T6
        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T6 -------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Global Variables----------
        public static uint Xavier_T6_dC_decdelayCNT  = 0;
        public static xeXavier_T6_Job Xavier_T6_dC_GetInJob     = 0;
        public static xeXavier_T6_Job Xavier_Task6_p_ret        = 0;
        public static xeXavier_T6_Job Xavier_Task6_ISR_JT_retmp = xeXavier_T6_Job.tp6_ISR01_START;
        public static xeXavier_T6_Job Xavier_Task6_ISR_CT_retmp = xeXavier_T6_Job.tp6_ISR01_START;

        // ----------Enumerations----------
        public enum xeXavier_T6_proc {
            pT6SET = 1,
            pT6GET,
            pT6Interrupt,
            pT6ResISR,
            pT6deExcute,
        }

        public enum xeXavier_T6_Job {
            tp6Empty = 0,
            tp6Init,
            
            tp6_ISR01_START,
            tp6_ISR01_STEP1,
            tp6_ISR01_STEP2,
            tp6_ISR01_END,

            tp6_ISR02_START,
            tp6_ISR02_STEP1,
            tp6_ISR02_STEP2,
            tp6_ISR02_END,
            
            tp6Idle,
            tp6START,  //判斷動作種類

            //IO檢查_工作門_檔案組
            tp6HomeSTART,
                tp6Home_工作門關閉,
                tp6Home_告知工作門已關閉,
                tp6Home_確認吸嘴軸組回home完畢_從_tp2Home_告知吸嘴軸組已回home完畢,
                tp6Home_確認植針軸組回home完畢_從_tp3Home_告知植針軸組已回home完畢,
                tp6Home_確認電動缸組回home完畢_從_tp4Home_告知電動缸組已回home完畢,
                tp6Home_確認載盤組回home完畢_從_tp5Home_告知載盤組已回home完畢,
                tp6Home_告知系統回home完畢,
                tp6Home_工作門開啟,

            tp6TakeAndDiscardSTART,

            tp6InsertSTART,
                tp6Insert_讀取兩點校正檔,
                tp6Insert_告知載盤組已拿到兩點校正資料,
                tp6Insert_確認載盤組完成XY兩點校正程序_從_tp5Insert_告知檔案組已完成兩點校正,
                tp6Insert_開始讀取植針資料檔,
                    tp6Insert_讀取植針資料檔,
                    tp6Insert_植針資料檔資料確認,
                    tp6Insert_取出目標植針資料確認,
                    tp6Insert_無資料不需要值針,                     tp6Insert_有資料確定需要值針,                                    
                    tp6Insert_告知系統無目標植針資料,               tp6Insert_告知系統已拿到目標植針資料,                            
                                                                    tp6Insert_等待系統植針動作完成_從_tp5Insert_告知系統植針成功,
                                                                    tp6Insert_清除告知系統已拿到目標植針資料,
                                                                    tp6Insert_跳回_至_tp6Insert_取出目標植針資料確認,
                                                                    tp6Insert_完成讀取植針資料檔,

            tp6RemoveSTART,  //無複合動作，合併至此做單一循環
            tp6Remove_讀取抽針資料檔,
            tp6Remove_檢查是否需要抽針,
                tp6Remove_不須抽針,                                 tp6Remove_需要抽針,
                                                                        tp6Remove_載盤XY移置抽料位,
                                                                        tp6Remove_抽料Z軸至抽料位,
                                                                        tp6Remove_抽料電磁閥開啟,
                                                                        tp6Remove_抽料電磁閥關閉,
                                                                        tp6Remove_抽料Z軸回0,
                                                                    tp6Remove_載盤XY移置拍照檢查位,
                                                                    tp6Remove_檢查有無抽針成功,
                                                                    tp6Remove_抽針號遞增,
            tp6Remove_完成讀取抽針資料檔,
        }

        public enum xeXavier_NeedleType {
            pT6Null  = 0,
            pT6Place,
            pT6Remove,
        }

        // ---------Private Variables----------
        public bool bForceToLoadCalibrationJson = false;
        public bool bForceToLoadInsertJson      = false;
        public xeXavier_NeedleType eNeedleType  = xeXavier_NeedleType.pT6Null;
        public int iSocketHoleNum        = -1;
        public int iSocketHoleArrayIndex = -1;
        public int iSocketHoleIndex      = -1;
        public int iRemoveRetryCNT       = 0;

        // ----------Methods----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_TASK6() {  //IO檢查_工作門_檔案組
            xeXavier_T6_Job priTASK = xeXavier_T6_Job.tp6Empty;
            Xavier_T6_delayCase(xeXavier_T6_proc.pT6deExcute, (uint)xeXavier_T6_Job.tp6Empty, xeXavier_T6_Job.tp6Empty);
            priTASK = Xavier_Task6_proc(xeXavier_T6_proc.pT6GET, 0);

            xeXavier_Indicator rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_事件);
            if(rslt_event == xeXavier_Indicator.xeXI_事件_空) {
                rslt_event = apiIndicator(xeXavier_Indicator.xeXI_讀_狀態);
            }
            switch (rslt_event) {
                case xeXavier_Indicator.xeXI_狀態_運行:
                case xeXavier_Indicator.xeXI_事件_復歸:
                    break;

                case xeXavier_Indicator.xeXI_狀態_停止:
                case xeXavier_Indicator.xeXI_狀態_急停:
                case xeXavier_Indicator.xeXI_事件_暫停:
                case xeXavier_Indicator.xeXI_事件_異常: 
                default:
                    priTASK = xeXavier_T6_Job.tp6START;
                    break;
            }

            switch (priTASK) {
                case xeXavier_T6_Job.tp6Empty:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Init);
                    Xavier_Task6_Debugprintf("tp6Empty\r\n");
                    break;

                case xeXavier_T6_Job.tp6Init:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6START);
                    Xavier_Task6_Debugprintf("tp6Init\r\n");
                    break;

                //======ISR Job======
                case xeXavier_T6_Job.tp6_ISR01_START:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6_ISR01_STEP1);
                    Xavier_Task6_Debugprintf("tp6_ISR01_START\r\n");
                    break;

                case xeXavier_T6_Job.tp6_ISR01_STEP1:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6_ISR01_STEP2);
                    Xavier_Task6_Debugprintf("tp6_ISR01_STEP1\r\n");
                    break;

                case xeXavier_T6_Job.tp6_ISR01_STEP2:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6_ISR01_END);
                    Xavier_Task6_Debugprintf("tp6_ISR01_STEP2\r\n");
                    break;

                case xeXavier_T6_Job.tp6_ISR01_END:
                    //Xavier_Task6_ISR_CNTTmp(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT);
                    //Xavier_Task6_ISR_JobTmp(xeXavier_T6_proc.pT6SET, xeXavier_T6_Job.tp6STEP2);

                    Task6ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp6_ISR);  //尚未加入此TASK ISR
                    Xavier_Task6_Debugprintf("tp6_ISR01_end\r\n");
                    break;
                //======ISR Job======
                case xeXavier_T6_Job.tp6_ISR02_START:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6_ISR02_STEP1);
                    Xavier_Task6_Debugprintf("tp6_ISR02_START\r\n");
                    break;

                case xeXavier_T6_Job.tp6_ISR02_STEP1:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6_ISR02_STEP2);
                    Xavier_Task6_Debugprintf("tp6_ISR02_STEP1\r\n");
                    break;

                case xeXavier_T6_Job.tp6_ISR02_STEP2:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6_ISR02_END);
                    Xavier_Task6_Debugprintf("tp6_ISR02_STEP2\r\n");
                    break;

                case xeXavier_T6_Job.tp6_ISR02_END:
                    if(eNeedleType == xeXavier_NeedleType.pT6Place) {
                        //檔案為植針檔案

                    } else {
                        //檔案為取針檔案
                        Xavier_Task6_ISR_CNTTmp(xeXavier_T6_proc.pT6SET, (xeXavier_T6_Job)u32ISRDelayCNT);
                        Xavier_Task6_ISR_JobTmp(xeXavier_T6_proc.pT6SET, xeXavier_T6_Job.tp6RemoveSTART);
                    }

                    Task6ResumeJob();
                    Xavier_ResumeTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp6_ISR);  //尚未加入此TASK ISR
                    Xavier_Task6_Debugprintf("tp6_ISR02_end\r\n");
                    break;
                //======ISR Job======
                
                case xeXavier_T6_Job.tp6Idle:  //reserve
                    break;

                case xeXavier_T6_Job.tp6START:  //判斷動作種類
                    { 
                        xeXavier_Indicator rslt = apiGetMachineAction();
                        switch(rslt) {
                            case xeXavier_Indicator.xeXI_狀態_運行:
                                if(btp6Home_告知系統回home完畢 == true) {
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6InsertSTART);
                                } else {
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6HomeSTART);
                                }
                                break;
                            case xeXavier_Indicator.xeXI_狀態_停止:
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6START);
                                break;
                            case xeXavier_Indicator.xeXI_狀態_急停:
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6START);
                                break;

                            case xeXavier_Indicator.xeXI_事件_復歸:
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6HomeSTART);
                                break;
                            case xeXavier_Indicator.xeXI_事件_暫停:    break;
                            case xeXavier_Indicator.xeXI_事件_異常:    break;

                            default:
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32ISRDelayCNT, xeXavier_T6_Job.tp6START);
                                break;
                        }
                    }
                    Xavier_Task6_Debugprintf("tp6START\r\n");
                    break;

                case xeXavier_T6_Job.tp6HomeSTART:
                    { 
                        btp2Home_告知植針軸組可以進行復歸動作                = false;
                        btp2Home_告知吸嘴軸組已回home完畢                    = false;
                        btp3Home_告知吸嘴軸組_植針軸組無干涉                 = false;
                        btp3Home_告知載盤組_植針軸組無干涉                   = false;
                        btp3Home_告知植針軸組已回home完畢                    = false;
                        btp4Home_告知載盤組_電動缸無干涉                     = false;
                        btp4Home_告知電動缸組已回home完畢                    = false;
                        btp5Home_告知電動缸組_抽針嘴_3D掃描_可以進行復歸動作 = false;
                        btp5Home_告知載盤組已回home完畢                      = false;
                        btp6Home_告知工作門已關閉                            = false;
                        btp6Home_告知系統回home完畢                          = false;

                        UIHelper.SetControlProperty(en_工作門, () => en_工作門.Checked = true);

                        clsServoControlWMX3.WMX3_ServoOnOff((int)WMX3軸定義.工作門, true);

                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_工作門關閉);
                    }
                    Xavier_Task6_Debugprintf("tp6HomeSTART\r\n");
                    break;
                    case xeXavier_T6_Job.tp6Home_工作門關閉:
                        { 
                            digitalWrite((int)WMX3IO對照.pxeIO_Buzzer, HIGH);

                            dbapiGate_defaultSpeed(dbGate_關門);
                            if(dbapiGate(dbCheckArrived, 0) == dbAxisMoveOk) {
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_告知工作門已關閉);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_工作門關閉);
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Home_工作門關閉\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_告知工作門已關閉:
                        {
                            btp6Home_告知工作門已關閉 = true;
                            digitalWrite((int)WMX3IO對照.pxeIO_Buzzer, LOW);
                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認吸嘴軸組回home完畢_從_tp2Home_告知吸嘴軸組已回home完畢);
                        }
                        Xavier_Task6_Debugprintf("tp6Home_告知工作門已關閉\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_確認吸嘴軸組回home完畢_從_tp2Home_告知吸嘴軸組已回home完畢:
                        {
                            if(btp2Home_告知吸嘴軸組已回home完畢 == true) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認植針軸組回home完畢_從_tp3Home_告知植針軸組已回home完畢);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認吸嘴軸組回home完畢_從_tp2Home_告知吸嘴軸組已回home完畢);
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Home_確認吸嘴軸組回home完畢_從_tp2Home_告知吸嘴軸組已回home完畢\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_確認植針軸組回home完畢_從_tp3Home_告知植針軸組已回home完畢:
                        {
                            if(btp3Home_告知植針軸組已回home完畢 == true) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認電動缸組回home完畢_從_tp4Home_告知電動缸組已回home完畢);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認植針軸組回home完畢_從_tp3Home_告知植針軸組已回home完畢);
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Home_確認植針軸組回home完畢_從_tp3Home_告知植針軸組已回home完畢\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_確認電動缸組回home完畢_從_tp4Home_告知電動缸組已回home完畢:
                        {
                            if(btp4Home_告知電動缸組已回home完畢 == true) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認載盤組回home完畢_從_tp5Home_告知載盤組已回home完畢);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認電動缸組回home完畢_從_tp4Home_告知電動缸組已回home完畢);
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Home_確認電動缸組回home完畢_從_tp4Home_告知電動缸組已回home完畢\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_確認載盤組回home完畢_從_tp5Home_告知載盤組已回home完畢:
                        {
                            if(btp5Home_告知載盤組已回home完畢 == true) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_告知系統回home完畢);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_確認載盤組回home完畢_從_tp5Home_告知載盤組已回home完畢);
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Home_確認載盤組回home完畢_從_tp5Home_告知載盤組已回home完畢\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_告知系統回home完畢:
                        {
                            btp6Home_告知系統回home完畢 = true;
                            digitalWrite((int)WMX3IO對照.pxeIO_Buzzer, HIGH);
                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6Home_工作門開啟);
                        }
                        Xavier_Task6_Debugprintf("tp6Home_告知系統回home完畢\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Home_工作門開啟:
                        {
                            dbapiGate_defaultSpeed(dbGate_開門);
                            digitalWrite((int)WMX3IO對照.pxeIO_Buzzer, LOW);
                            apiIndicator(xeXavier_Indicator.xeXI_狀態_停止);

                            CleanAllBoolFlag();

                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32HomeDelayCNT, xeXavier_T6_Job.tp6START);
                        }
                        Xavier_Task6_Debugprintf("tp6Home_工作門開啟\r\n");
                        break;


                case xeXavier_T6_Job.tp6TakeAndDiscardSTART:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6TakeAndDiscardSTART);
                    Xavier_Task6_Debugprintf("tp6TakeAndDiscardSTART\r\n");
                    break;

                case xeXavier_T6_Job.tp6InsertSTART:
                    {
                        bForceToLoadCalibrationJson = false;
                        bForceToLoadInsertJson      = false;

                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_讀取兩點校正檔);
                    }
                    Xavier_Task6_Debugprintf("tp6InsertSTART\r\n");
                    break;
                    case xeXavier_T6_Job.tp6Insert_讀取兩點校正檔:
                        {
                            if(bForceToLoadCalibrationJson == false) {
                                bForceToLoadCalibrationJson = true;

                                if (OpenFile(this))  {
                                    tsmi_SaveFile.Enabled = true;
                                    UIHelper.SetControlProperty(btn_SaveFile, () => btn_SaveFile.Enabled = true);

                                    show_grp_BarcodeInfo(grp_BarcodeInfo);
                                    find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                                    // 檢查是否需要在主執行緒上執行
                                    if (pic_Needles.InvokeRequired) {
                                        // 如果是其他執行緒，使用 Invoke 方法
                                        pic_Needles.Invoke(new Action( 
                                                                            () => {
                                                                                pic_Needles.Refresh();
                                                                            }
                                                                        )
                                                            );
                                    } else {
                                        // 如果是在主執行緒，直接執行
                                        pic_Needles.Refresh();
                                    }
                                }

                                int igetCount = get_NeedleCount();
                                if(igetCount == 2) { 
                                    bForceToLoadCalibrationJson = false;
                                    //讀取校正檔案 確認有看到兩個點資料
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_告知載盤組已拿到兩點校正資料);
                                } else { 
                                    bForceToLoadCalibrationJson = false;
                                    //讀取校正檔案 點數資料數量錯誤
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_讀取兩點校正檔);
                                }
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Insert_讀取兩點校正檔\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Insert_告知載盤組已拿到兩點校正資料:
                        {
                            btp6Insert_告知載盤組已拿到兩點校正資料 = true;

                            btp6Insert_告知系統已拿到目標植針資料_To_Tp3 = true;
                            btp6Insert_告知系統已拿到目標植針資料_To_Tp5 = true;
                            btp6Insert_告知系統已拿到目標植針資料_To_Tp4 = true;
                            btp6Insert_告知系統已拿到目標植針資料_To_Tp2 = true; 

                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_確認載盤組完成XY兩點校正程序_從_tp5Insert_告知檔案組已完成兩點校正);
                        }
                        Xavier_Task6_Debugprintf("tp6Insert_告知載盤組已拿到兩點校正資料\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Insert_確認載盤組完成XY兩點校正程序_從_tp5Insert_告知檔案組已完成兩點校正:
                        {
                            if(btp5Insert_告知檔案組已完成兩點校正 == true) { 
                                btp5Insert_告知檔案組已完成兩點校正 = false;

                                btp6Insert_告知系統已拿到目標植針資料_To_Tp3 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp5 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp4 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp2 = false; 

                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_開始讀取植針資料檔);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_確認載盤組完成XY兩點校正程序_從_tp5Insert_告知檔案組已完成兩點校正);   
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Insert_確認載盤組完成XY兩點校正程序_從_tp5Insert_告知檔案組已完成兩點校正\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Insert_開始讀取植針資料檔:
                        {
                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_讀取植針資料檔);
                        }
                        Xavier_Task6_Debugprintf("tp6Insert_開始讀取植針資料檔\r\n");
                        break;
                        case xeXavier_T6_Job.tp6Insert_讀取植針資料檔:
                            {
                                if(bForceToLoadInsertJson == false) {
                                    bForceToLoadInsertJson = true;

                                    if (OpenFile(this))  {
                                        tsmi_SaveFile.Enabled = true;
                                        UIHelper.SetControlProperty(btn_SaveFile, () => btn_SaveFile.Enabled = true);

                                        show_grp_BarcodeInfo(grp_BarcodeInfo);
                                        find_Json_Boundary(Json, pic_Needles.Width, pic_Needles.Height);

                                        // 檢查是否需要在主執行緒上執行
                                        if (pic_Needles.InvokeRequired) {
                                            // 如果是其他執行緒，使用 Invoke 方法
                                            pic_Needles.Invoke(new Action( 
                                                                             () => {
                                                                                 pic_Needles.Refresh();
                                                                             }
                                                                         )
                                                              );
                                        } else {
                                            // 如果是在主執行緒，直接執行
                                            pic_Needles.Refresh();
                                        }
                                    }

                                    int igetCount = get_NeedleCount();
                                    if(igetCount > 0) { 
                                        bForceToLoadInsertJson = false;
                                        //讀取校正檔案 確認有看到兩個點資料
                                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_植針資料檔資料確認);
                                    } else { 
                                        bForceToLoadInsertJson = false;
                                        //讀取校正檔案 點數資料數量錯誤
                                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_讀取植針資料檔);
                                    }
                                }
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_讀取植針資料檔\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_植針資料檔資料確認:
                            {
                                //讀取Socket植針孔數量
                                int PlaceCNT  = find_PlaceNeedles();
                                int RemoveCNT = find_RemoveNeedles();

                                if(PlaceCNT > 0) { 
                                    eNeedleType    = xeXavier_NeedleType.pT6Place;
                                    iSocketHoleNum = PlaceCNT;

                                    //有資料
                                    //初始化index
                                    iSocketHoleIndex       = 0;
                                    iSocketHoleArrayIndex  = 0;

                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_取出目標植針資料確認);
                                } else if(RemoveCNT > 0) { 
                                    eNeedleType    = xeXavier_NeedleType.pT6Remove;
                                    iSocketHoleNum = RemoveCNT;

                                    //移除Task
                                    Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp2_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);  //吸嘴軸, 復歸後保護
                                    Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp3_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);  //植針嘴, 復歸後保護

                                    //設定至Remove Task
                                    Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp4_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);  //電動缸, 不動作, 授權至T6Remove內動作
                                    Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp5_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);  //載盤,   不動作, 授權至T6Remove內動作
                                  //Xavier_CallTaskInterrupt(xeXavier_FlowTaskISR.xeXFTI_tp6_ISR, xeXavier_FlowTask_ISR_ID.xeFTII_ISR02);  //檔案, 不需要中斷 直接跳至RemoveSTART

                                    //有資料
                                    //初始化index
                                    iSocketHoleIndex       = 0;
                                    iSocketHoleArrayIndex  = 0;

                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6RemoveSTART);
                                } else { 
                                    //PlaceCNT  == 0
                                    //RemoveCNT == 0

                                    //無資料
                                    //清除index
                                    iSocketHoleIndex       = -1;
                                    iSocketHoleArrayIndex  = -1;

                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_取出目標植針資料確認);
                                }
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_植針資料檔資料確認\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_取出目標植針資料確認:
                            {
                                btp6Insert_告知系統無目標植針資料_To_Tp3     = false;               
                                btp6Insert_告知系統無目標植針資料_To_Tp5     = false;               
                                btp6Insert_告知系統無目標植針資料_To_Tp4     = false;               
                                btp6Insert_告知系統無目標植針資料_To_Tp2     = false;               
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp3 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp5 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp4 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp2 = false; 

                                if( (                     0 <  iSocketHoleNum   ) &&
                                    ( iSocketHoleArrayIndex <= iSocketHoleNum-1 ) ) { 

                                    try {
                                        iSocketHoleIndex = PlaceNeedles[iSocketHoleArrayIndex].Index; 
                                    } catch (Exception ex) {
                                        // 捕捉其他類型的異常
                                        Console.WriteLine("發生錯誤：" + ex.Message);
                                        iSocketHoleIndex = -1;
                                    }

                                    //取得目前植針ID的位置
                                    if (iSocketHoleIndex >= 0) {
                                        //有資料
                                        double dbX = 0.0, dbY = 0.0;

                                        find_Needle_Position(PerspectiveTransformMatrix, iSocketHoleIndex, ref dbX, ref dbY);
                                        FocusedNeedle = PlaceNeedles[iSocketHoleArrayIndex];
                                        show_grp_NeedleInfo(grp_NeedleInfo);

                                        // 檢查是否需要在主執行緒上執行
                                        if (pic_Needles.InvokeRequired) {
                                            // 如果是其他執行緒，使用 Invoke 方法
                                            pic_Needles.Invoke(new Action( 
                                                                             () => {
                                                                                 pic_Needles.Refresh();
                                                                             }
                                                                         )
                                                              );
                                        } else {
                                            // 如果是在主執行緒，直接執行
                                            pic_Needles.Refresh();
                                        }

                                        UIHelper.SetControlProperty(txt_HoldIndex, () => txt_HoldIndex.Text = iSocketHoleIndex.ToString());

                                        dbPinHolePositionX = dbX;
                                        dbPinHolePositionY = dbY;

                                        UIHelper.SetControlProperty(label14, () => label14.Text = dbX.ToString());
                                        UIHelper.SetControlProperty(label15, () => label15.Text = dbY.ToString());

                                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_有資料確定需要值針);
                                    } else
                                    if (iSocketHoleIndex == -1) {
                                        //錯誤
                                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_無資料不需要值針);
                                    } 
                                } else { 
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_無資料不需要值針);
                                }
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_取出目標植針資料確認\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_無資料不需要值針:
                            {
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_告知系統無目標植針資料);
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_無資料不需要值針\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_告知系統無目標植針資料:
                            {
                                btp6Insert_告知系統無目標植針資料_To_Tp3 = true;               
                                btp6Insert_告知系統無目標植針資料_To_Tp5 = true;               
                                btp6Insert_告知系統無目標植針資料_To_Tp4 = true;               
                                btp6Insert_告知系統無目標植針資料_To_Tp2 = true; 

                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_完成讀取植針資料檔);
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_告知系統無目標植針資料\r\n");
                            break;            
                        case xeXavier_T6_Job.tp6Insert_有資料確定需要值針:
                            {
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_告知系統已拿到目標植針資料);
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_有資料確定需要值針\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_告知系統已拿到目標植針資料:
                            {
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp3 = true;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp5 = true;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp4 = true;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp2 = true;

                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_等待系統植針動作完成_從_tp5Insert_告知系統植針成功);
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_告知系統已拿到目標植針資料\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_等待系統植針動作完成_從_tp5Insert_告知系統植針成功:
                            {
                                if(btp5Insert_告知系統植針成功_To_Tp6 == true) { 
                                    btp5Insert_告知系統植針成功_To_Tp6 = false;

                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_清除告知系統已拿到目標植針資料);
                                } else { 
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_等待系統植針動作完成_從_tp5Insert_告知系統植針成功);
                                }
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_等待系統植針動作完成_從_tp5Insert_告知系統植針成功\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_清除告知系統已拿到目標植針資料:
                            {
                                btp6Insert_告知系統無目標植針資料_To_Tp3     = false;               
                                btp6Insert_告知系統無目標植針資料_To_Tp5     = false;               
                                btp6Insert_告知系統無目標植針資料_To_Tp4     = false;               
                                btp6Insert_告知系統無目標植針資料_To_Tp2     = false;               
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp3 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp5 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp4 = false;
                                btp6Insert_告知系統已拿到目標植針資料_To_Tp2 = false;

                              //btp5Insert_告知系統植針成功_To_Tp6    = false;
                              //btp5Insert_告知系統植針成功_To_Tp3    = false;
                              //btp5Insert_告知系統植針失敗           = false;

                                iSocketHoleArrayIndex++;

                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_跳回_至_tp6Insert_取出目標植針資料確認);
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_清除告知系統已拿到目標植針資料\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_跳回_至_tp6Insert_取出目標植針資料確認:
                            {
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_取出目標植針資料確認);
                            }
                            Xavier_Task6_Debugprintf("tp6Insert_跳回_至_tp6Insert_取出目標植針資料確認\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Insert_完成讀取植針資料檔:
                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Insert_完成讀取植針資料檔);
                            Xavier_Task6_Debugprintf("tp6Insert_完成讀取植針資料檔\r\n");
                            break;

                case xeXavier_T6_Job.tp6RemoveSTART:  //無複合動作，合併至此做單一循環
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_讀取抽針資料檔);
                    Xavier_Task6_Debugprintf("tp6RemoveSTART\r\n");
                    break;
                case xeXavier_T6_Job.tp6Remove_讀取抽針資料檔:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_檢查是否需要抽針);
                    Xavier_Task6_Debugprintf("tp6Remove_讀取抽針資料檔\r\n");
                    break;
                case xeXavier_T6_Job.tp6Remove_檢查是否需要抽針:
                    if( (                     0 <  iSocketHoleNum   ) &&
                        ( iSocketHoleArrayIndex <= iSocketHoleNum-1 ) ) { 

                        try {
                            iSocketHoleIndex = RemoveNeedles[iSocketHoleArrayIndex].Index; 
                        } catch (Exception ex) {
                            // 捕捉其他類型的異常
                            Console.WriteLine("發生錯誤：" + ex.Message);
                            iSocketHoleIndex = -1;
                        }

                        //取得目前植針ID的位置
                        if (iSocketHoleIndex >= 0) {
                            //有資料
                            double dbX = 0.0, dbY = 0.0;

                            find_Needle_Position(PerspectiveTransformMatrix, iSocketHoleIndex, ref dbX, ref dbY);
                            FocusedNeedle = RemoveNeedles[iSocketHoleArrayIndex];
                            show_grp_NeedleInfo(grp_NeedleInfo);

                            // 檢查是否需要在主執行緒上執行
                            if (pic_Needles.InvokeRequired) {
                                // 如果是其他執行緒，使用 Invoke 方法
                                pic_Needles.Invoke(new Action( 
                                                                    () => {
                                                                        pic_Needles.Refresh();
                                                                    }
                                                                )
                                                    );
                            } else {
                                // 如果是在主執行緒，直接執行
                                pic_Needles.Refresh();
                            }

                            UIHelper.SetControlProperty(txt_HoldIndex, () => txt_HoldIndex.Text = iSocketHoleIndex.ToString());

                            dbPinHolePositionX = dbX;
                            dbPinHolePositionY = dbY;

                            UIHelper.SetControlProperty(label14, () => label14.Text = dbX.ToString());
                            UIHelper.SetControlProperty(label15, () => label15.Text = dbY.ToString());

                            //設定抽針retry重抽次數
                            iRemoveRetryCNT = 3;

                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_需要抽針);
                        } else
                        if (iSocketHoleIndex == -1) {
                            //錯誤
                            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_不須抽針);
                        } 
                    } else { 
                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_不須抽針);
                    }
                    Xavier_Task6_Debugprintf("tp6Remove_檢查是否需要抽針\r\n");
                    break;
                    case xeXavier_T6_Job.tp6Remove_不須抽針:
                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_完成讀取抽針資料檔);
                        Xavier_Task6_Debugprintf("tp6Remove_不須抽針\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Remove_需要抽針:
                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_載盤XY移置抽料位);
                        Xavier_Task6_Debugprintf("tp6Remove_需要抽針\r\n");
                        break;
                        case xeXavier_T6_Job.tp6Remove_載盤XY移置抽料位:
                            {
                                double dbSocketCamera; {
                                    dbSocketCamera = apiParaReadIndex("SaveParameterJason.json", 17);
                                    dbapiIAI(dbSocketCamera);
                                }

                                double SetPinOffsetX, SetPinOffsetY; {
                                    SetPinOffsetX = apiParaReadIndex("SaveParameterJason.json", 15);
                                    SetPinOffsetY = apiParaReadIndex("SaveParameterJason.json", 16);

                                    double dbTargetX = dbPinHolePositionX + SetPinOffsetX;
                                    double dbTargetY = dbPinHolePositionY + SetPinOffsetY;

                                    dbapiCarrierX_defaultSpeed(dbTargetX);
                                    dbapiCarrierY_defaultSpeed(dbTargetY);
                                }
                                if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                    (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_抽料Z軸至抽料位);
                                } else { 
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_載盤XY移置抽料位);
                                }
                            }
                            Xavier_Task6_Debugprintf("tp6Remove_載盤XY移置抽料位\r\n");
                            break;
                        case xeXavier_T6_Job.tp6Remove_抽料Z軸至抽料位:
                            double RemovePinZHight; {
                                RemovePinZHight = apiParaReadIndex("SaveParameterJason.json", 12);
                                dbapiJoDell吸針嘴(RemovePinZHight);
                            }
                            if(dbapiJoDell吸針嘴(dbCheckArrived) == dbAxisMoveOk) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_抽料電磁閥開啟);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_抽料Z軸至抽料位);
                            }
                            Xavier_Task6_Debugprintf("tp6Remove_抽料Z軸至抽料位\r\n");
                            break;
                            case xeXavier_T6_Job.tp6Remove_抽料電磁閥開啟:
                                digitalWrite((int)WMX3IO對照.pxeIO_吸料真空電磁閥, HIGH);

                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_抽料電磁閥關閉);
                                Xavier_Task6_Debugprintf("tp6Remove_抽料電磁閥開啟\r\n");
                                break;
                            case xeXavier_T6_Job.tp6Remove_抽料電磁閥關閉:
                                digitalWrite((int)WMX3IO對照.pxeIO_吸料真空電磁閥, LOW);

                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, 10, xeXavier_T6_Job.tp6Remove_抽料Z軸回0);
                                Xavier_Task6_Debugprintf("tp6Remove_抽料電磁閥關閉\r\n");
                                break;
                        case xeXavier_T6_Job.tp6Remove_抽料Z軸回0:
                            dbapiJoDell吸針嘴(dbJoDell吸針嘴_Home位);
                            if(dbapiJoDell吸針嘴(dbCheckArrived) == dbAxisMoveOk) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_載盤XY移置拍照檢查位);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_抽料Z軸回0);
                            }
                            Xavier_Task6_Debugprintf("tp6Remove_抽料Z軸回0\r\n");
                            break;
                    case xeXavier_T6_Job.tp6Remove_載盤XY移置拍照檢查位:
                        {
                            double dbTargetX = dbPinHolePositionX;
                            double dbTargetY = dbPinHolePositionY;

                            dbapiCarrierX_defaultSpeed(dbTargetX);
                            dbapiCarrierY_defaultSpeed(dbTargetY);

                            if( (dbapiCarrierX(dbCheckArrived, 0) == dbAxisMoveOk) &&
                                (dbapiCarrierY(dbCheckArrived, 0) == dbAxisMoveOk) ) { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_檢查有無抽針成功);
                            } else { 
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_載盤XY移置拍照檢查位);
                            }
                        }
                        Xavier_Task6_Debugprintf("tp6Remove_載盤XY移置拍照檢查位\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Remove_檢查有無抽針成功:
                        {
                            bool success = false;
                            UIHelper.RunOnUIThread(this, () => { btn_Socket孔檢查_Click(null, EventArgs.Empty); });
                            {
                                //取得校正攝影機校正參數
                                success = inspector1.xInspSocket植針後檢查();
                                UIHelper.SetControlProperty(label7, () => label7.Text = (success == false) ? "抽針檢查 OK" : "抽針檢查 NG");

                                rtb_Status_AppendMessage(rtb_Status, $"抽針 {(success ? "NG":"OK")}");
                            }

                            //視覺檢查有無抽針成功
                            if(success == false) { 
                                //抽料成功
                                //有孔

                                //執行下一個抽針位
                                Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_抽針號遞增);
                            } else { 
                                //抽料失敗
                                //沒有孔

                                //檢查重抽retry次數
                                if(iRemoveRetryCNT>0) { 
                                    //Retry次數內, 沒問題
                                    iRemoveRetryCNT--;

                                    //再抽一次
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_載盤XY移置抽料位);
                                } else { 
                                    //Retry次數==0, 沒機會了
                                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_不須抽針);
                                }
                            }  // end of if(bCheckRemovePinSuccess == true) { 
                        }
                        Xavier_Task6_Debugprintf("tp6Remove_檢查有無抽針成功\r\n");
                        break;
                    case xeXavier_T6_Job.tp6Remove_抽針號遞增:
                        iSocketHoleArrayIndex++;

                        Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_檢查是否需要抽針);
                        Xavier_Task6_Debugprintf("tp6Remove_抽針號遞增\r\n");
                        break;
                case xeXavier_T6_Job.tp6Remove_完成讀取抽針資料檔:
                    Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, u32InsertDelayCNT, xeXavier_T6_Job.tp6Remove_完成讀取抽針資料檔);
                    Xavier_Task6_Debugprintf("tp6Remove_完成讀取抽針資料檔\r\n");
                    break;

                default:
                    break;
            }

            Xavier_Task6_proc(xeXavier_T6_proc.pT6SET, xeXavier_T6_Job.tp6Idle);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_T6_delayCase(xeXavier_T6_proc deJob, uint delayCNT, xeXavier_T6_Job excuteJob) {
            switch (deJob) {
                case xeXavier_T6_proc.pT6SET:
                    Xavier_T6_dC_decdelayCNT = delayCNT + 2;
                    Xavier_T6_dC_GetInJob    = excuteJob;
                    break;

                case xeXavier_T6_proc.pT6Interrupt:
                    if (Xavier_T6_dC_GetInJob != excuteJob) {
                        Xavier_Task6_ISR_CNTTmp(xeXavier_T6_proc.pT6SET, (xeXavier_T6_Job)Xavier_T6_dC_decdelayCNT);
                        Xavier_Task6_ISR_JobTmp(xeXavier_T6_proc.pT6SET, Xavier_T6_dC_GetInJob);

                        Xavier_T6_dC_GetInJob = excuteJob;
                        Xavier_T6_dC_decdelayCNT = 2;  // equal to excute pT6deExcute to get Xavier_Task6_proc(pT6SET,GetInJob);
                    }
                    break;

                case xeXavier_T6_proc.pT6ResISR:
                    Xavier_T6_dC_decdelayCNT = (uint)Xavier_Task6_ISR_CNTTmp(xeXavier_T6_proc.pT6GET, Xavier_T6_dC_GetInJob) + 2;
                    Xavier_T6_dC_GetInJob    =       Xavier_Task6_ISR_JobTmp(xeXavier_T6_proc.pT6GET, Xavier_T6_dC_GetInJob);

                    Xavier_Task6_ISR_CNTTmp(xeXavier_T6_proc.pT6SET, (xeXavier_T6_Job)2);
                    Xavier_Task6_ISR_JobTmp(xeXavier_T6_proc.pT6SET, xeXavier_T6_Job.tp6Empty);
                    break;

                case xeXavier_T6_proc.pT6deExcute:
                    if (Xavier_T6_dC_decdelayCNT > 0) {
                        Xavier_T6_dC_decdelayCNT--;
                    }

                    if (Xavier_T6_dC_decdelayCNT == 1) {
                        Xavier_Task6_proc(xeXavier_T6_proc.pT6SET, Xavier_T6_dC_GetInJob);
                    }
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T6_Job Xavier_Task6_proc(xeXavier_T6_proc rtFun, xeXavier_T6_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T6_proc.pT6SET:
                    Xavier_Task6_p_ret = ptValue;
                    break;

                case xeXavier_T6_proc.pT6GET:
                    break;
            }

            return Xavier_Task6_p_ret;
        }
        //---------------------------------------------------------------------------------------
        public void Task6CallJob(xeXavier_T6_Job excuteJob) {
            Xavier_T6_delayCase(xeXavier_T6_proc.pT6Interrupt, 0, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task6CallJobWithDelay(xeXavier_T6_Job excuteJob, uint delayCNT) {
            Xavier_T6_delayCase(xeXavier_T6_proc.pT6SET, delayCNT, excuteJob);
        }
        //---------------------------------------------------------------------------------------
        public void Task6ResumeJob() {
            Xavier_T6_delayCase(xeXavier_T6_proc.pT6ResISR, 0, 0);
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T6_Job Xavier_Task6_ISR_JobTmp(xeXavier_T6_proc rtFun, xeXavier_T6_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T6_proc.pT6SET:
                    Xavier_Task6_ISR_JT_retmp = ptValue;
                    break;

                case xeXavier_T6_proc.pT6GET:
                    break;
            }

            return Xavier_Task6_ISR_JT_retmp;
        }
        //---------------------------------------------------------------------------------------
        public xeXavier_T6_Job Xavier_Task6_ISR_CNTTmp(xeXavier_T6_proc rtFun, xeXavier_T6_Job ptValue) {
            switch (rtFun) {
                case xeXavier_T6_proc.pT6SET:
                    Xavier_Task6_ISR_CT_retmp = ptValue;
                    break;

                case xeXavier_T6_proc.pT6GET:
                    break;
            }

            return Xavier_Task6_ISR_CT_retmp;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        // ----------Debug Method----------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public void Xavier_Task6_Debugprintf(string message) {
            UIHelper.SetControlProperty(lbldbg_Task6_Info, () => lbldbg_Task6_Info.Text = message);

            XavierLogger.Log("Task6", message);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //------------------------------- XavierTaskFlowEngine_T6 -------------------------------
        //---------------------------------------------------------------------------------------
        #endregion



        //---------------------------------------------------------------------------------------
        //-------------------------------- Xavier TaskFlow Engine -------------------------------
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
    //解決呼叫一大堆Invoke
    public static class UIHelper
    {
        public static void SetControlProperty(Control control, Action action) {
            if (control == null || control.IsDisposed || !control.IsHandleCreated) { 
                return; // 控制項已經不存在或未建立，直接略過
            }
            if (control.InvokeRequired) {
                try {
                    control.Invoke(action);
                } catch (ObjectDisposedException) {
                    // 控制項已經被釋放，略過
                } catch (InvalidOperationException) {
                    // 控制項已經關閉，略過
                }
            } else {
                if (!control.IsDisposed) {
                    action();
                }
            }
        }
        public static T GetControlProperty<T>(Control control, Func<T> getter) {
            if (control.InvokeRequired) {
                return (T)control.Invoke(getter);
            } else {
                return getter();
            }
        }

        public static void RunOnUIThread(Control control, Action action) {
            if (control.InvokeRequired) { 
                control.Invoke(action);
            } else {
                action();
            }
        }

        public static void SetIndicator(Control control, bool isOn) {
            SetControlProperty(control, () =>
                {
                    control.BackColor = isOn ? Color.Green : Color.Red;
                }
            );
        }
    }
    //---------------------------------------------------------------------------------------
    public static class XavierLogger
    {
        private static BlockingCollection<string> _logQueue = new BlockingCollection<string>();
        private static Thread _logWriterThread;
        private static volatile bool _isRunning = true;

        private static DateTime _currentLogHour;
        private static StreamWriter _writer;

        static XavierLogger()
        {
            _currentLogHour = DateTime.Now;

            _logWriterThread = new Thread(() =>
            {
                while (_isRunning || _logQueue.Count > 0)
                {
                    if (_logQueue.TryTake(out var log, Timeout.Infinite))
                    {
                        var now = DateTime.Now;
                        var logHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

                        if (_writer == null || logHour != _currentLogHour)
                        {
                            // 換檔案
                            _writer?.Dispose();
                            _currentLogHour = logHour;

                            string newFileName = $"XavierLog_{now:yyyyMMdd_HH}.txt";
                            _writer = new StreamWriter(newFileName, true);
                        }

                        _writer.WriteLine(log);
                        _writer.Flush();
                    }
                }

                // 最後結束時寫入剩餘資料
                _writer?.Dispose();
            });

            _logWriterThread.IsBackground = false;
            _logWriterThread.Start();
        }

        public static void Log(string threadName, string message)
        {
            string log = $"{DateTime.Now:HH:mm:ss.fff} [{threadName}] {message}";
            _logQueue.Add(log);
        }

        public static void XavierLogger_Shutdown()
        {
            _isRunning = false;
            _logQueue.CompleteAdding();
            _logWriterThread.Join();
        }
    }
    //---------------------------------------------------------------------------------------
}  // end of namespace InjectorInspector
