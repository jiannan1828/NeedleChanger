﻿using System.Windows.Forms;

namespace InjectorInspector
{
    partial class Form1
    {

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmr_ReadWMX3 = new System.Windows.Forms.Timer(this.components);
            this.tabJob = new System.Windows.Forms.TabPage();
            this.btn_minus = new System.Windows.Forms.Button();
            this.btn_plus = new System.Windows.Forms.Button();
            this.edit_diff_value = new System.Windows.Forms.TextBox();
            this.btn_置換 = new System.Windows.Forms.Button();
            this.btn_取針 = new System.Windows.Forms.Button();
            this.btn_植針 = new System.Windows.Forms.Button();
            this.btn_拋料 = new System.Windows.Forms.Button();
            this.btn_參數 = new System.Windows.Forms.Button();
            this.lbl_吸嘴破真空流量閥 = new System.Windows.Forms.Label();
            this.vcb_吸嘴破真空流量閥 = new System.Windows.Forms.VScrollBar();
            this.button5 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_manual = new System.Windows.Forms.Button();
            this.lblVBLED = new System.Windows.Forms.Label();
            this.SB_VBLED = new System.Windows.Forms.HScrollBar();
            this.lbl料倉 = new System.Windows.Forms.Label();
            this.lbl上下收 = new System.Windows.Forms.Label();
            this.lbl震散 = new System.Windows.Forms.Label();
            this.lbl左右收 = new System.Windows.Forms.Label();
            this.lbl_debug = new System.Windows.Forms.Label();
            this.lbl_植針吹氣流量閥 = new System.Windows.Forms.Label();
            this.btn_minus_d001 = new System.Windows.Forms.Button();
            this.btn_plus_d001 = new System.Windows.Forms.Button();
            this.btn_minus_d01 = new System.Windows.Forms.Button();
            this.btn_plus_d01 = new System.Windows.Forms.Button();
            this.btn_minus_d1 = new System.Windows.Forms.Button();
            this.btn_plus_d1 = new System.Windows.Forms.Button();
            this.vcb_植針吹氣流量閥 = new System.Windows.Forms.VScrollBar();
            this.btnVibrationStop = new System.Windows.Forms.Button();
            this.btnVibrationInit = new System.Windows.Forms.Button();
            this.lbl_JoDell吸針嘴_Convert = new System.Windows.Forms.Label();
            this.lbl_JoDell吸針嘴_Back = new System.Windows.Forms.Label();
            this.lbl_JoDell吸針嘴_RAW = new System.Windows.Forms.Label();
            this.en_JoDell吸針嘴 = new System.Windows.Forms.CheckBox();
            this.lbl_acpos_JoDell吸針嘴 = new System.Windows.Forms.Label();
            this.lbl_acpos_JoDell吸針嘴_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_JoDell吸針嘴 = new System.Windows.Forms.Label();
            this.lbl_spd_JoDell吸針嘴_lbl = new System.Windows.Forms.Label();
            this.select_JoDell吸針嘴 = new System.Windows.Forms.RadioButton();
            this.lbl_JoDell3D掃描_Convert = new System.Windows.Forms.Label();
            this.lbl_JoDell3D掃描_Back = new System.Windows.Forms.Label();
            this.lbl_JoDell3D掃描_RAW = new System.Windows.Forms.Label();
            this.en_JoDell3D掃描 = new System.Windows.Forms.CheckBox();
            this.lbl_acpos_JoDell3D掃描 = new System.Windows.Forms.Label();
            this.lbl_acpos_JoDell3D掃描_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_JoDell3D掃描 = new System.Windows.Forms.Label();
            this.lbl_spd_JoDell3D掃描_lbl = new System.Windows.Forms.Label();
            this.select_JoDell3D掃描 = new System.Windows.Forms.RadioButton();
            this.lbl_JoDell植針嘴_Convert = new System.Windows.Forms.Label();
            this.lbl_JoDell植針嘴_Back = new System.Windows.Forms.Label();
            this.lbl_JoDell植針嘴_RAW = new System.Windows.Forms.Label();
            this.en_JoDell植針嘴 = new System.Windows.Forms.CheckBox();
            this.lbl_acpos_JoDell植針嘴 = new System.Windows.Forms.Label();
            this.lbl_acpos_JoDell植針嘴_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_JoDell植針嘴 = new System.Windows.Forms.Label();
            this.lbl_spd_JoDell植針嘴_lbl = new System.Windows.Forms.Label();
            this.select_JoDell植針嘴 = new System.Windows.Forms.RadioButton();
            this.lbl_IAI_Convert = new System.Windows.Forms.Label();
            this.lbl_IAI_Back = new System.Windows.Forms.Label();
            this.lbl_IAI_RAW = new System.Windows.Forms.Label();
            this.en_IAI = new System.Windows.Forms.CheckBox();
            this.lbl_acpos_IAI = new System.Windows.Forms.Label();
            this.lbl_acpos_IAI_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_IAI = new System.Windows.Forms.Label();
            this.lbl_spd_IAI_lbl = new System.Windows.Forms.Label();
            this.select_Socket檢測 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblBuzzer = new System.Windows.Forms.Label();
            this.lbl_NA_31 = new System.Windows.Forms.Label();
            this.lbl綠燈 = new System.Windows.Forms.Label();
            this.lbl左按鈕紅燈 = new System.Windows.Forms.Label();
            this.lbl黃燈 = new System.Windows.Forms.Label();
            this.lbl中按鈕綠燈 = new System.Windows.Forms.Label();
            this.lbl紅燈 = new System.Windows.Forms.Label();
            this.lbl右按鈕綠燈 = new System.Windows.Forms.Label();
            this.lbl艙內燈 = new System.Windows.Forms.Label();
            this.lbl取料吸嘴破新 = new System.Windows.Forms.Label();
            this.lblHEPA = new System.Windows.Forms.Label();
            this.lbl植針Z煞車 = new System.Windows.Forms.Label();
            this.lbl下後右門鎖 = new System.Windows.Forms.Label();
            this.lbl取料吸嘴破舊 = new System.Windows.Forms.Label();
            this.lbl下後左門鎖 = new System.Windows.Forms.Label();
            this.lbl取料吸嘴吸 = new System.Windows.Forms.Label();
            this.lbl擺放破真空 = new System.Windows.Forms.Label();
            this.lblsk破真空1 = new System.Windows.Forms.Label();
            this.lbl擺放座真空 = new System.Windows.Forms.Label();
            this.lblsk真空1 = new System.Windows.Forms.Label();
            this.lblsk破真空2 = new System.Windows.Forms.Label();
            this.lbl載盤破真空 = new System.Windows.Forms.Label();
            this.lblsk真空2 = new System.Windows.Forms.Label();
            this.lbl載盤真空閥 = new System.Windows.Forms.Label();
            this.lbl_NA_25 = new System.Windows.Forms.Label();
            this.lbl堵料吹氣 = new System.Windows.Forms.Label();
            this.lbl收料區缸 = new System.Windows.Forms.Label();
            this.lbl植針吹氣 = new System.Windows.Forms.Label();
            this.lbl接料區缸 = new System.Windows.Forms.Label();
            this.lbl堵料吹氣缸 = new System.Windows.Forms.Label();
            this.lbl吸料真空閥 = new System.Windows.Forms.Label();
            this.lbl擺放蓋板 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_NA_24 = new System.Windows.Forms.Label();
            this.lbl下右左門 = new System.Windows.Forms.Label();
            this.lbl_NA_23 = new System.Windows.Forms.Label();
            this.lbl下右右門 = new System.Windows.Forms.Label();
            this.lbl下後右門 = new System.Windows.Forms.Label();
            this.lbl下左左門 = new System.Windows.Forms.Label();
            this.lbl下後左門 = new System.Windows.Forms.Label();
            this.lbl下左右門 = new System.Windows.Forms.Label();
            this.lbl_NA_20 = new System.Windows.Forms.Label();
            this.lbl上後左門 = new System.Windows.Forms.Label();
            this.lbl螢幕小門 = new System.Windows.Forms.Label();
            this.lbl上後右門 = new System.Windows.Forms.Label();
            this.lbl上右左門 = new System.Windows.Forms.Label();
            this.lbl上左左門 = new System.Windows.Forms.Label();
            this.lbl上右右門 = new System.Windows.Forms.Label();
            this.lbl上左右門 = new System.Windows.Forms.Label();
            this.lbl_NA_19 = new System.Windows.Forms.Label();
            this.lbl_NA_18 = new System.Windows.Forms.Label();
            this.lbl_NA_17 = new System.Windows.Forms.Label();
            this.lbl_NA_16 = new System.Windows.Forms.Label();
            this.lbl_NA_15 = new System.Windows.Forms.Label();
            this.lbl_擺放座關 = new System.Windows.Forms.Label();
            this.lbl_NA_13 = new System.Windows.Forms.Label();
            this.lbl_擺放座開 = new System.Windows.Forms.Label();
            this.lbl_NA_11 = new System.Windows.Forms.Label();
            this.lbl急停鈕 = new System.Windows.Forms.Label();
            this.lbl_NA_10 = new System.Windows.Forms.Label();
            this.lbl停止鈕 = new System.Windows.Forms.Label();
            this.lbl_NA_09 = new System.Windows.Forms.Label();
            this.lbl啟動鈕 = new System.Windows.Forms.Label();
            this.lbl_NA_08 = new System.Windows.Forms.Label();
            this.lbl復歸鈕 = new System.Windows.Forms.Label();
            this.lbl吸料盒 = new System.Windows.Forms.Label();
            this.lbl兩點壓2 = new System.Windows.Forms.Label();
            this.lbl堵料盒 = new System.Windows.Forms.Label();
            this.lbl兩點壓1 = new System.Windows.Forms.Label();
            this.lbl取料ng盒 = new System.Windows.Forms.Label();
            this.lbl吸嘴空2 = new System.Windows.Forms.Label();
            this.lbl_NA_07 = new System.Windows.Forms.Label();
            this.lbl吸嘴空1 = new System.Windows.Forms.Label();
            this.lbl擺放空2 = new System.Windows.Forms.Label();
            this.lblsk1空2 = new System.Windows.Forms.Label();
            this.lbl擺放空1 = new System.Windows.Forms.Label();
            this.lblsk1空1 = new System.Windows.Forms.Label();
            this.lblsk2空2 = new System.Windows.Forms.Label();
            this.lbl載盤空2 = new System.Windows.Forms.Label();
            this.lblsk2空1 = new System.Windows.Forms.Label();
            this.lbl載盤空1 = new System.Windows.Forms.Label();
            this.lbl_NA_06 = new System.Windows.Forms.Label();
            this.lbl載盤X後 = new System.Windows.Forms.Label();
            this.lbl_NA_05 = new System.Windows.Forms.Label();
            this.lbl載盤X前 = new System.Windows.Forms.Label();
            this.lbl_NA_04 = new System.Windows.Forms.Label();
            this.lbl植針Z前 = new System.Windows.Forms.Label();
            this.lbl_NA_03 = new System.Windows.Forms.Label();
            this.lbl植針Z後 = new System.Windows.Forms.Label();
            this.lbl_NA_02 = new System.Windows.Forms.Label();
            this.lbl取料X前 = new System.Windows.Forms.Label();
            this.lbl_NA_01 = new System.Windows.Forms.Label();
            this.lbl取料X後 = new System.Windows.Forms.Label();
            this.lbl取料Y前 = new System.Windows.Forms.Label();
            this.lbl載盤Y前 = new System.Windows.Forms.Label();
            this.lbl取料Y後 = new System.Windows.Forms.Label();
            this.lbl載盤Y後 = new System.Windows.Forms.Label();
            this.lbl_工作門_Convert = new System.Windows.Forms.Label();
            this.lbl_植針R軸_Convert = new System.Windows.Forms.Label();
            this.lbl_植針Z軸_Convert = new System.Windows.Forms.Label();
            this.lbl_載盤Y軸_Convert = new System.Windows.Forms.Label();
            this.lbl_載盤X軸_Convert = new System.Windows.Forms.Label();
            this.lbl_吸嘴R軸_Convert = new System.Windows.Forms.Label();
            this.lbl_吸嘴Z軸_Convert = new System.Windows.Forms.Label();
            this.lbl_吸嘴Y軸_Convert = new System.Windows.Forms.Label();
            this.lbl_吸嘴X軸_Convert = new System.Windows.Forms.Label();
            this.lbl_工作門_Back = new System.Windows.Forms.Label();
            this.lbl_工作門_RAW = new System.Windows.Forms.Label();
            this.lbl_植針R軸_Back = new System.Windows.Forms.Label();
            this.lbl_植針R軸_RAW = new System.Windows.Forms.Label();
            this.lbl_植針Z軸_Back = new System.Windows.Forms.Label();
            this.lbl_植針Z軸_RAW = new System.Windows.Forms.Label();
            this.lbl_載盤Y軸_Back = new System.Windows.Forms.Label();
            this.lbl_載盤Y軸_RAW = new System.Windows.Forms.Label();
            this.lbl_載盤X軸_Back = new System.Windows.Forms.Label();
            this.lbl_載盤X軸_RAW = new System.Windows.Forms.Label();
            this.lbl_吸嘴R軸_Back = new System.Windows.Forms.Label();
            this.lbl_吸嘴R軸_RAW = new System.Windows.Forms.Label();
            this.lbl_吸嘴Z軸_Back = new System.Windows.Forms.Label();
            this.lbl_吸嘴Z軸_RAW = new System.Windows.Forms.Label();
            this.lbl_吸嘴Y軸_Back = new System.Windows.Forms.Label();
            this.lbl_吸嘴Y軸_RAW = new System.Windows.Forms.Label();
            this.lbl_吸嘴X軸_Back = new System.Windows.Forms.Label();
            this.lbl_吸嘴X軸_RAW = new System.Windows.Forms.Label();
            this.btn_minus_10 = new System.Windows.Forms.Button();
            this.btn_minus_1 = new System.Windows.Forms.Button();
            this.btn_plus_10 = new System.Windows.Forms.Button();
            this.btn_plus_1 = new System.Windows.Forms.Button();
            this.btnABSMove = new System.Windows.Forms.Button();
            this.txtABSpos = new System.Windows.Forms.TextBox();
            this.en_工作門 = new System.Windows.Forms.CheckBox();
            this.en_植針R軸 = new System.Windows.Forms.CheckBox();
            this.en_植針Z軸 = new System.Windows.Forms.CheckBox();
            this.en_載盤Y軸 = new System.Windows.Forms.CheckBox();
            this.en_載盤X軸 = new System.Windows.Forms.CheckBox();
            this.en_吸嘴R軸 = new System.Windows.Forms.CheckBox();
            this.en_吸嘴Z軸 = new System.Windows.Forms.CheckBox();
            this.en_吸嘴Y軸 = new System.Windows.Forms.CheckBox();
            this.en_吸嘴X軸 = new System.Windows.Forms.CheckBox();
            this.lbl_acpos_工作門 = new System.Windows.Forms.Label();
            this.lbl_acpos_植針R軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_植針Z軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_載盤Y軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_載盤X軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴R軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴Z軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴Y軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴X軸 = new System.Windows.Forms.Label();
            this.lbl_acpos_工作門_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_植針R軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_植針Z軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_載盤Y軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_載盤X軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴R軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴Z軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴Y軸_lbl = new System.Windows.Forms.Label();
            this.lbl_acpos_吸嘴X軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_工作門 = new System.Windows.Forms.Label();
            this.lbl_spd_植針R軸 = new System.Windows.Forms.Label();
            this.lbl_spd_植針Z軸 = new System.Windows.Forms.Label();
            this.lbl_spd_載盤Y軸 = new System.Windows.Forms.Label();
            this.lbl_spd_載盤X軸 = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴R軸 = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴Z軸 = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴Y軸 = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴X軸 = new System.Windows.Forms.Label();
            this.lbl_spd_工作門_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_植針R軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_植針Z軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_載盤Y軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_載盤X軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴R軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴Z軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴Y軸_lbl = new System.Windows.Forms.Label();
            this.lbl_spd_吸嘴X軸_lbl = new System.Windows.Forms.Label();
            this.select_工作門 = new System.Windows.Forms.RadioButton();
            this.select_植針R軸 = new System.Windows.Forms.RadioButton();
            this.select_植針Z軸 = new System.Windows.Forms.RadioButton();
            this.select_載盤Y軸 = new System.Windows.Forms.RadioButton();
            this.select_載盤X軸 = new System.Windows.Forms.RadioButton();
            this.select_吸嘴R軸 = new System.Windows.Forms.RadioButton();
            this.select_吸嘴Z軸 = new System.Windows.Forms.RadioButton();
            this.select_吸嘴Y軸 = new System.Windows.Forms.RadioButton();
            this.select_吸嘴X軸 = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbl_2pCalibraLog = new System.Windows.Forms.Label();
            this.btn_兩點校正 = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.btn抽針 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btn_socket相機兩點定位 = new System.Windows.Forms.Button();
            this.cB_料盤有料 = new System.Windows.Forms.CheckBox();
            this.cB_AlwaysResume = new System.Windows.Forms.CheckBox();
            this.lbl_PinIndex = new System.Windows.Forms.Label();
            this.btn_SwitchPointAB = new System.Windows.Forms.Button();
            this.btn_ToPointB = new System.Windows.Forms.Button();
            this.btn_ToPointA = new System.Windows.Forms.Button();
            this.lbl_計算角度 = new System.Windows.Forms.Label();
            this.tB_By = new System.Windows.Forms.TextBox();
            this.tB_Ay = new System.Windows.Forms.TextBox();
            this.tB_Bx = new System.Windows.Forms.TextBox();
            this.tB_Ax = new System.Windows.Forms.TextBox();
            this.lbl_竹蜻蜓座標B = new System.Windows.Forms.Label();
            this.lbl_竹蜻蜓座標A = new System.Windows.Forms.Label();
            this.btn_tmrClear = new System.Windows.Forms.Button();
            this.txt_HoldIndex = new System.Windows.Forms.TextBox();
            this.btn_Resume = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btn_取得目標座標 = new System.Windows.Forms.Button();
            this.lbl_CycleTime = new System.Windows.Forms.Label();
            this.btn_tmrPause = new System.Windows.Forms.Button();
            this.btn_tmrStop = new System.Windows.Forms.Button();
            this.btn上膛 = new System.Windows.Forms.Button();
            this.lblLog = new System.Windows.Forms.Label();
            this.txt_取料循環 = new System.Windows.Forms.TextBox();
            this.btn_TakePin = new System.Windows.Forms.Button();
            this.btn_home = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btn_植針嘴檢查 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_Socket孔檢查 = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSetHome = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_取得PinInfo = new System.Windows.Forms.Button();
            this.btn_AlarmRST = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.inspector1 = new Inspector.Inspector();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.rtb_Status = new System.Windows.Forms.RichTextBox();
            this.lbl_ZoomFactor = new System.Windows.Forms.Label();
            this.lbl_Offset = new System.Windows.Forms.Label();
            this.lbl_PicMousePos = new System.Windows.Forms.Label();
            this.lbl_RealMousePos = new System.Windows.Forms.Label();
            this.grp_NeedleInfo = new System.Windows.Forms.GroupBox();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.chk_Reserve1 = new System.Windows.Forms.CheckBox();
            this.rad_Replace = new System.Windows.Forms.RadioButton();
            this.rad_Remove = new System.Windows.Forms.RadioButton();
            this.rad_Place = new System.Windows.Forms.RadioButton();
            this.lbl_Index = new System.Windows.Forms.Label();
            this.txt_Index = new System.Windows.Forms.TextBox();
            this.chk_Disable = new System.Windows.Forms.CheckBox();
            this.chk_Display = new System.Windows.Forms.CheckBox();
            this.txt_Diameter = new System.Windows.Forms.TextBox();
            this.lbl_Diameter = new System.Windows.Forms.Label();
            this.txt_PosX = new System.Windows.Forms.TextBox();
            this.txt_PosY = new System.Windows.Forms.TextBox();
            this.lbl_PosX = new System.Windows.Forms.Label();
            this.txt_Id = new System.Windows.Forms.TextBox();
            this.lbl_Id = new System.Windows.Forms.Label();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.tab_Needles = new System.Windows.Forms.TabControl();
            this.tp_Needles = new System.Windows.Forms.TabPage();
            this.pic_Needles = new System.Windows.Forms.PictureBox();
            this.cms_pic_Needles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_Place = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Replace = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Display = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Enable = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Reserve1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tp_NeedlesJudge = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.grp_目前作業項目 = new System.Windows.Forms.GroupBox();
            this.grp_GroupPin2 = new System.Windows.Forms.GroupBox();
            this.txt_PogoPin2已植數量2 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2已植數量2 = new System.Windows.Forms.Label();
            this.txt_PogoPin1已植數量2 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1已植數量2 = new System.Windows.Forms.Label();
            this.txt_PogoPin2Qty4 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2Qty4 = new System.Windows.Forms.Label();
            this.txt_PogoPin1Qty4 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1Qty4 = new System.Windows.Forms.Label();
            this.grp_GroupPin1 = new System.Windows.Forms.GroupBox();
            this.txt_PogoPin2已植數量1 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2已植數量1 = new System.Windows.Forms.Label();
            this.txt_PogoPin1已植數量1 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1已植數量1 = new System.Windows.Forms.Label();
            this.txt_PogoPin2Qty3 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2Qty3 = new System.Windows.Forms.Label();
            this.txt_PogoPin1Qty3 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1Qty3 = new System.Windows.Forms.Label();
            this.rad_ChangeGroupPin = new System.Windows.Forms.RadioButton();
            this.rad_ChangeAllNewPin = new System.Windows.Forms.RadioButton();
            this.txt_PogoPin2已植數量 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2已植數量 = new System.Windows.Forms.Label();
            this.txt_PogoPin1已植數量 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1已植數量 = new System.Windows.Forms.Label();
            this.txt_PogoPin2Qty2 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2Qty2 = new System.Windows.Forms.Label();
            this.txt_PogoPin1Qty2 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1Qty2 = new System.Windows.Forms.Label();
            this.grp_SocketInfo = new System.Windows.Forms.GroupBox();
            this.grp_設備治具資訊 = new System.Windows.Forms.GroupBox();
            this.lbl_PogoPin3Qty1 = new System.Windows.Forms.Label();
            this.lbl_PogoPin2Qty1 = new System.Windows.Forms.Label();
            this.lbl_PogoPin1Qty1 = new System.Windows.Forms.Label();
            this.txt_下針導正模組 = new System.Windows.Forms.TextBox();
            this.lbl_下針導正模組 = new System.Windows.Forms.Label();
            this.txt_取針模組PI = new System.Windows.Forms.TextBox();
            this.lbl_取針模組PI = new System.Windows.Forms.Label();
            this.txt_PogoPin3Qty1 = new System.Windows.Forms.TextBox();
            this.txt_PogoPin2Qty1 = new System.Windows.Forms.TextBox();
            this.txt_PogoPin1Qty1 = new System.Windows.Forms.TextBox();
            this.grp_BarcodeInfo = new System.Windows.Forms.GroupBox();
            this.btn_SaveFile = new System.Windows.Forms.Button();
            this.btn_OpenFile = new System.Windows.Forms.Button();
            this.txt_儲位 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin2Qty = new System.Windows.Forms.Label();
            this.txt_板全號 = new System.Windows.Forms.TextBox();
            this.lbl_PogoPin1Qty = new System.Windows.Forms.Label();
            this.txt_型號 = new System.Windows.Forms.TextBox();
            this.lbl_Socket定位座 = new System.Windows.Forms.Label();
            this.txt_客戶 = new System.Windows.Forms.TextBox();
            this.lbl_FileName = new System.Windows.Forms.Label();
            this.txt_短編號 = new System.Windows.Forms.TextBox();
            this.lbl_Socket = new System.Windows.Forms.Label();
            this.txt_Barcode = new System.Windows.Forms.TextBox();
            this.lbl_條碼輸入欄位 = new System.Windows.Forms.Label();
            this.grp_儲存資訊 = new System.Windows.Forms.GroupBox();
            this.btn_停止 = new System.Windows.Forms.Button();
            this.btn_開始 = new System.Windows.Forms.Button();
            this.ms_Viewer = new System.Windows.Forms.MenuStrip();
            this.tsm_Account = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Login = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Logout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_File = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_OpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_SaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_CloseFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tmr_Home = new System.Windows.Forms.Timer(this.components);
            this.tmr_TakePin = new System.Windows.Forms.Timer(this.components);
            this.tmr_Warning = new System.Windows.Forms.Timer(this.components);
            this.ttp_NeedleInfo = new System.Windows.Forms.ToolTip(this.components);
            this.tmrBarCodeScanner = new System.Windows.Forms.Timer(this.components);
            this.tmr_2p_Calibration = new System.Windows.Forms.Timer(this.components);
            this.tabJob.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.grp_NeedleInfo.SuspendLayout();
            this.tab_Needles.SuspendLayout();
            this.tp_Needles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Needles)).BeginInit();
            this.cms_pic_Needles.SuspendLayout();
            this.tp_NeedlesJudge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grp_目前作業項目.SuspendLayout();
            this.grp_GroupPin2.SuspendLayout();
            this.grp_GroupPin1.SuspendLayout();
            this.grp_SocketInfo.SuspendLayout();
            this.grp_設備治具資訊.SuspendLayout();
            this.grp_BarcodeInfo.SuspendLayout();
            this.grp_儲存資訊.SuspendLayout();
            this.ms_Viewer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmr_ReadWMX3
            // 
            this.tmr_ReadWMX3.Enabled = true;
            this.tmr_ReadWMX3.Interval = 125;
            this.tmr_ReadWMX3.Tick += new System.EventHandler(this.tmr_ReadWMX3_Tick);
            // 
            // tabJob
            // 
            this.tabJob.Controls.Add(this.btn_minus);
            this.tabJob.Controls.Add(this.btn_plus);
            this.tabJob.Controls.Add(this.edit_diff_value);
            this.tabJob.Controls.Add(this.btn_置換);
            this.tabJob.Controls.Add(this.btn_取針);
            this.tabJob.Controls.Add(this.btn_植針);
            this.tabJob.Controls.Add(this.btn_拋料);
            this.tabJob.Controls.Add(this.btn_參數);
            this.tabJob.Controls.Add(this.lbl_吸嘴破真空流量閥);
            this.tabJob.Controls.Add(this.vcb_吸嘴破真空流量閥);
            this.tabJob.Controls.Add(this.button5);
            this.tabJob.Controls.Add(this.button2);
            this.tabJob.Controls.Add(this.btn_manual);
            this.tabJob.Controls.Add(this.lblVBLED);
            this.tabJob.Controls.Add(this.SB_VBLED);
            this.tabJob.Controls.Add(this.lbl料倉);
            this.tabJob.Controls.Add(this.lbl上下收);
            this.tabJob.Controls.Add(this.lbl震散);
            this.tabJob.Controls.Add(this.lbl左右收);
            this.tabJob.Controls.Add(this.lbl_debug);
            this.tabJob.Controls.Add(this.lbl_植針吹氣流量閥);
            this.tabJob.Controls.Add(this.btn_minus_d001);
            this.tabJob.Controls.Add(this.btn_plus_d001);
            this.tabJob.Controls.Add(this.btn_minus_d01);
            this.tabJob.Controls.Add(this.btn_plus_d01);
            this.tabJob.Controls.Add(this.btn_minus_d1);
            this.tabJob.Controls.Add(this.btn_plus_d1);
            this.tabJob.Controls.Add(this.vcb_植針吹氣流量閥);
            this.tabJob.Controls.Add(this.btnVibrationStop);
            this.tabJob.Controls.Add(this.btnVibrationInit);
            this.tabJob.Controls.Add(this.lbl_JoDell吸針嘴_Convert);
            this.tabJob.Controls.Add(this.lbl_JoDell吸針嘴_Back);
            this.tabJob.Controls.Add(this.lbl_JoDell吸針嘴_RAW);
            this.tabJob.Controls.Add(this.en_JoDell吸針嘴);
            this.tabJob.Controls.Add(this.lbl_acpos_JoDell吸針嘴);
            this.tabJob.Controls.Add(this.lbl_acpos_JoDell吸針嘴_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_JoDell吸針嘴);
            this.tabJob.Controls.Add(this.lbl_spd_JoDell吸針嘴_lbl);
            this.tabJob.Controls.Add(this.select_JoDell吸針嘴);
            this.tabJob.Controls.Add(this.lbl_JoDell3D掃描_Convert);
            this.tabJob.Controls.Add(this.lbl_JoDell3D掃描_Back);
            this.tabJob.Controls.Add(this.lbl_JoDell3D掃描_RAW);
            this.tabJob.Controls.Add(this.en_JoDell3D掃描);
            this.tabJob.Controls.Add(this.lbl_acpos_JoDell3D掃描);
            this.tabJob.Controls.Add(this.lbl_acpos_JoDell3D掃描_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_JoDell3D掃描);
            this.tabJob.Controls.Add(this.lbl_spd_JoDell3D掃描_lbl);
            this.tabJob.Controls.Add(this.select_JoDell3D掃描);
            this.tabJob.Controls.Add(this.lbl_JoDell植針嘴_Convert);
            this.tabJob.Controls.Add(this.lbl_JoDell植針嘴_Back);
            this.tabJob.Controls.Add(this.lbl_JoDell植針嘴_RAW);
            this.tabJob.Controls.Add(this.en_JoDell植針嘴);
            this.tabJob.Controls.Add(this.lbl_acpos_JoDell植針嘴);
            this.tabJob.Controls.Add(this.lbl_acpos_JoDell植針嘴_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_JoDell植針嘴);
            this.tabJob.Controls.Add(this.lbl_spd_JoDell植針嘴_lbl);
            this.tabJob.Controls.Add(this.select_JoDell植針嘴);
            this.tabJob.Controls.Add(this.lbl_IAI_Convert);
            this.tabJob.Controls.Add(this.lbl_IAI_Back);
            this.tabJob.Controls.Add(this.lbl_IAI_RAW);
            this.tabJob.Controls.Add(this.en_IAI);
            this.tabJob.Controls.Add(this.lbl_acpos_IAI);
            this.tabJob.Controls.Add(this.lbl_acpos_IAI_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_IAI);
            this.tabJob.Controls.Add(this.lbl_spd_IAI_lbl);
            this.tabJob.Controls.Add(this.select_Socket檢測);
            this.tabJob.Controls.Add(this.groupBox2);
            this.tabJob.Controls.Add(this.groupBox1);
            this.tabJob.Controls.Add(this.lbl_工作門_Convert);
            this.tabJob.Controls.Add(this.lbl_植針R軸_Convert);
            this.tabJob.Controls.Add(this.lbl_植針Z軸_Convert);
            this.tabJob.Controls.Add(this.lbl_載盤Y軸_Convert);
            this.tabJob.Controls.Add(this.lbl_載盤X軸_Convert);
            this.tabJob.Controls.Add(this.lbl_吸嘴R軸_Convert);
            this.tabJob.Controls.Add(this.lbl_吸嘴Z軸_Convert);
            this.tabJob.Controls.Add(this.lbl_吸嘴Y軸_Convert);
            this.tabJob.Controls.Add(this.lbl_吸嘴X軸_Convert);
            this.tabJob.Controls.Add(this.lbl_工作門_Back);
            this.tabJob.Controls.Add(this.lbl_工作門_RAW);
            this.tabJob.Controls.Add(this.lbl_植針R軸_Back);
            this.tabJob.Controls.Add(this.lbl_植針R軸_RAW);
            this.tabJob.Controls.Add(this.lbl_植針Z軸_Back);
            this.tabJob.Controls.Add(this.lbl_植針Z軸_RAW);
            this.tabJob.Controls.Add(this.lbl_載盤Y軸_Back);
            this.tabJob.Controls.Add(this.lbl_載盤Y軸_RAW);
            this.tabJob.Controls.Add(this.lbl_載盤X軸_Back);
            this.tabJob.Controls.Add(this.lbl_載盤X軸_RAW);
            this.tabJob.Controls.Add(this.lbl_吸嘴R軸_Back);
            this.tabJob.Controls.Add(this.lbl_吸嘴R軸_RAW);
            this.tabJob.Controls.Add(this.lbl_吸嘴Z軸_Back);
            this.tabJob.Controls.Add(this.lbl_吸嘴Z軸_RAW);
            this.tabJob.Controls.Add(this.lbl_吸嘴Y軸_Back);
            this.tabJob.Controls.Add(this.lbl_吸嘴Y軸_RAW);
            this.tabJob.Controls.Add(this.lbl_吸嘴X軸_Back);
            this.tabJob.Controls.Add(this.lbl_吸嘴X軸_RAW);
            this.tabJob.Controls.Add(this.btn_minus_10);
            this.tabJob.Controls.Add(this.btn_minus_1);
            this.tabJob.Controls.Add(this.btn_plus_10);
            this.tabJob.Controls.Add(this.btn_plus_1);
            this.tabJob.Controls.Add(this.btnABSMove);
            this.tabJob.Controls.Add(this.txtABSpos);
            this.tabJob.Controls.Add(this.en_工作門);
            this.tabJob.Controls.Add(this.en_植針R軸);
            this.tabJob.Controls.Add(this.en_植針Z軸);
            this.tabJob.Controls.Add(this.en_載盤Y軸);
            this.tabJob.Controls.Add(this.en_載盤X軸);
            this.tabJob.Controls.Add(this.en_吸嘴R軸);
            this.tabJob.Controls.Add(this.en_吸嘴Z軸);
            this.tabJob.Controls.Add(this.en_吸嘴Y軸);
            this.tabJob.Controls.Add(this.en_吸嘴X軸);
            this.tabJob.Controls.Add(this.lbl_acpos_工作門);
            this.tabJob.Controls.Add(this.lbl_acpos_植針R軸);
            this.tabJob.Controls.Add(this.lbl_acpos_植針Z軸);
            this.tabJob.Controls.Add(this.lbl_acpos_載盤Y軸);
            this.tabJob.Controls.Add(this.lbl_acpos_載盤X軸);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴R軸);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴Z軸);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴Y軸);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴X軸);
            this.tabJob.Controls.Add(this.lbl_acpos_工作門_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_植針R軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_植針Z軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_載盤Y軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_載盤X軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴R軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴Z軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴Y軸_lbl);
            this.tabJob.Controls.Add(this.lbl_acpos_吸嘴X軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_工作門);
            this.tabJob.Controls.Add(this.lbl_spd_植針R軸);
            this.tabJob.Controls.Add(this.lbl_spd_植針Z軸);
            this.tabJob.Controls.Add(this.lbl_spd_載盤Y軸);
            this.tabJob.Controls.Add(this.lbl_spd_載盤X軸);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴R軸);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴Z軸);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴Y軸);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴X軸);
            this.tabJob.Controls.Add(this.lbl_spd_工作門_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_植針R軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_植針Z軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_載盤Y軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_載盤X軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴R軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴Z軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴Y軸_lbl);
            this.tabJob.Controls.Add(this.lbl_spd_吸嘴X軸_lbl);
            this.tabJob.Controls.Add(this.select_工作門);
            this.tabJob.Controls.Add(this.select_植針R軸);
            this.tabJob.Controls.Add(this.select_植針Z軸);
            this.tabJob.Controls.Add(this.select_載盤Y軸);
            this.tabJob.Controls.Add(this.select_載盤X軸);
            this.tabJob.Controls.Add(this.select_吸嘴R軸);
            this.tabJob.Controls.Add(this.select_吸嘴Z軸);
            this.tabJob.Controls.Add(this.select_吸嘴Y軸);
            this.tabJob.Controls.Add(this.select_吸嘴X軸);
            this.tabJob.Location = new System.Drawing.Point(4, 29);
            this.tabJob.Name = "tabJob";
            this.tabJob.Padding = new System.Windows.Forms.Padding(3);
            this.tabJob.Size = new System.Drawing.Size(1228, 792);
            this.tabJob.TabIndex = 2;
            this.tabJob.Text = "tabJob";
            this.tabJob.UseVisualStyleBackColor = true;
            // 
            // btn_minus
            // 
            this.btn_minus.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_minus.Location = new System.Drawing.Point(34, 401);
            this.btn_minus.Name = "btn_minus";
            this.btn_minus.Size = new System.Drawing.Size(34, 23);
            this.btn_minus.TabIndex = 235;
            this.btn_minus.Text = "-";
            this.btn_minus.UseVisualStyleBackColor = true;
            this.btn_minus.Click += new System.EventHandler(this.btn_plus_minus_Click);
            // 
            // btn_plus
            // 
            this.btn_plus.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_plus.Location = new System.Drawing.Point(34, 377);
            this.btn_plus.Name = "btn_plus";
            this.btn_plus.Size = new System.Drawing.Size(34, 23);
            this.btn_plus.TabIndex = 234;
            this.btn_plus.Text = "+";
            this.btn_plus.UseVisualStyleBackColor = true;
            this.btn_plus.Click += new System.EventHandler(this.btn_plus_minus_Click);
            // 
            // edit_diff_value
            // 
            this.edit_diff_value.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.edit_diff_value.Location = new System.Drawing.Point(74, 389);
            this.edit_diff_value.Name = "edit_diff_value";
            this.edit_diff_value.Size = new System.Drawing.Size(59, 23);
            this.edit_diff_value.TabIndex = 233;
            this.edit_diff_value.Text = "0.0";
            // 
            // btn_置換
            // 
            this.btn_置換.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_置換.Location = new System.Drawing.Point(534, 427);
            this.btn_置換.Name = "btn_置換";
            this.btn_置換.Size = new System.Drawing.Size(75, 23);
            this.btn_置換.TabIndex = 232;
            this.btn_置換.Text = "btn_置換";
            this.btn_置換.UseVisualStyleBackColor = true;
            this.btn_置換.Click += new System.EventHandler(this.btn_置換_Click);
            // 
            // btn_取針
            // 
            this.btn_取針.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_取針.Location = new System.Drawing.Point(534, 398);
            this.btn_取針.Name = "btn_取針";
            this.btn_取針.Size = new System.Drawing.Size(75, 23);
            this.btn_取針.TabIndex = 230;
            this.btn_取針.Text = "btn_取針";
            this.btn_取針.UseVisualStyleBackColor = true;
            this.btn_取針.Click += new System.EventHandler(this.btn_取針_Click);
            // 
            // btn_植針
            // 
            this.btn_植針.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_植針.Location = new System.Drawing.Point(453, 398);
            this.btn_植針.Name = "btn_植針";
            this.btn_植針.Size = new System.Drawing.Size(75, 23);
            this.btn_植針.TabIndex = 228;
            this.btn_植針.Text = "btn_植針";
            this.btn_植針.UseVisualStyleBackColor = true;
            this.btn_植針.Click += new System.EventHandler(this.btn_植針_Click);
            // 
            // btn_拋料
            // 
            this.btn_拋料.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_拋料.Location = new System.Drawing.Point(453, 427);
            this.btn_拋料.Name = "btn_拋料";
            this.btn_拋料.Size = new System.Drawing.Size(75, 23);
            this.btn_拋料.TabIndex = 226;
            this.btn_拋料.Text = "btn_拋料";
            this.btn_拋料.UseVisualStyleBackColor = true;
            this.btn_拋料.Click += new System.EventHandler(this.btn_拋料_Click);
            // 
            // btn_參數
            // 
            this.btn_參數.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_參數.Location = new System.Drawing.Point(368, 427);
            this.btn_參數.Name = "btn_參數";
            this.btn_參數.Size = new System.Drawing.Size(75, 23);
            this.btn_參數.TabIndex = 225;
            this.btn_參數.Text = "btn_參數";
            this.btn_參數.UseVisualStyleBackColor = true;
            this.btn_參數.Click += new System.EventHandler(this.btn_參數_Click);
            // 
            // lbl_吸嘴破真空流量閥
            // 
            this.lbl_吸嘴破真空流量閥.AutoSize = true;
            this.lbl_吸嘴破真空流量閥.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴破真空流量閥.Location = new System.Drawing.Point(935, 20);
            this.lbl_吸嘴破真空流量閥.Name = "lbl_吸嘴破真空流量閥";
            this.lbl_吸嘴破真空流量閥.Size = new System.Drawing.Size(129, 13);
            this.lbl_吸嘴破真空流量閥.TabIndex = 224;
            this.lbl_吸嘴破真空流量閥.Text = "lbl_吸嘴破真空流量閥";
            // 
            // vcb_吸嘴破真空流量閥
            // 
            this.vcb_吸嘴破真空流量閥.Location = new System.Drawing.Point(940, 45);
            this.vcb_吸嘴破真空流量閥.Maximum = 110;
            this.vcb_吸嘴破真空流量閥.Minimum = -10;
            this.vcb_吸嘴破真空流量閥.Name = "vcb_吸嘴破真空流量閥";
            this.vcb_吸嘴破真空流量閥.Size = new System.Drawing.Size(20, 200);
            this.vcb_吸嘴破真空流量閥.TabIndex = 223;
            this.vcb_吸嘴破真空流量閥.Value = 110;
            this.vcb_吸嘴破真空流量閥.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vcb流量閥_Scroll);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(703, 372);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 220;
            this.button5.Text = "讀黨";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(703, 343);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 219;
            this.button2.Text = "存檔";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btn_manual
            // 
            this.btn_manual.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_manual.Location = new System.Drawing.Point(368, 398);
            this.btn_manual.Name = "btn_manual";
            this.btn_manual.Size = new System.Drawing.Size(75, 23);
            this.btn_manual.TabIndex = 218;
            this.btn_manual.Text = "btn_manual";
            this.btn_manual.UseVisualStyleBackColor = true;
            this.btn_manual.Click += new System.EventHandler(this.btn_manual_Click);
            // 
            // lblVBLED
            // 
            this.lblVBLED.AutoSize = true;
            this.lblVBLED.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblVBLED.Location = new System.Drawing.Point(368, 363);
            this.lblVBLED.Name = "lblVBLED";
            this.lblVBLED.Size = new System.Drawing.Size(59, 13);
            this.lblVBLED.TabIndex = 217;
            this.lblVBLED.Text = "lblVBLED";
            // 
            // SB_VBLED
            // 
            this.SB_VBLED.Location = new System.Drawing.Point(430, 359);
            this.SB_VBLED.Maximum = 50;
            this.SB_VBLED.Minimum = 5;
            this.SB_VBLED.Name = "SB_VBLED";
            this.SB_VBLED.Size = new System.Drawing.Size(176, 20);
            this.SB_VBLED.TabIndex = 216;
            this.SB_VBLED.Value = 33;
            this.SB_VBLED.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_VBLED_Scroll);
            // 
            // lbl料倉
            // 
            this.lbl料倉.AutoSize = true;
            this.lbl料倉.BackColor = System.Drawing.Color.Green;
            this.lbl料倉.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl料倉.Location = new System.Drawing.Point(552, 305);
            this.lbl料倉.Name = "lbl料倉";
            this.lbl料倉.Size = new System.Drawing.Size(45, 13);
            this.lbl料倉.TabIndex = 215;
            this.lbl料倉.Text = "lbl料倉";
            this.lbl料倉.Click += new System.EventHandler(this.lbl柔震index);
            // 
            // lbl上下收
            // 
            this.lbl上下收.AutoSize = true;
            this.lbl上下收.BackColor = System.Drawing.Color.Green;
            this.lbl上下收.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上下收.Location = new System.Drawing.Point(432, 305);
            this.lbl上下收.Name = "lbl上下收";
            this.lbl上下收.Size = new System.Drawing.Size(58, 13);
            this.lbl上下收.TabIndex = 214;
            this.lbl上下收.Text = "lbl上下收";
            this.lbl上下收.Click += new System.EventHandler(this.lbl柔震index);
            // 
            // lbl震散
            // 
            this.lbl震散.AutoSize = true;
            this.lbl震散.BackColor = System.Drawing.Color.Red;
            this.lbl震散.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl震散.Location = new System.Drawing.Point(387, 305);
            this.lbl震散.Name = "lbl震散";
            this.lbl震散.Size = new System.Drawing.Size(45, 13);
            this.lbl震散.TabIndex = 213;
            this.lbl震散.Text = "lbl震散";
            this.lbl震散.Click += new System.EventHandler(this.lbl柔震index);
            // 
            // lbl左右收
            // 
            this.lbl左右收.AutoSize = true;
            this.lbl左右收.BackColor = System.Drawing.Color.Green;
            this.lbl左右收.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl左右收.Location = new System.Drawing.Point(492, 305);
            this.lbl左右收.Name = "lbl左右收";
            this.lbl左右收.Size = new System.Drawing.Size(58, 13);
            this.lbl左右收.TabIndex = 212;
            this.lbl左右收.Text = "lbl左右收";
            this.lbl左右收.Click += new System.EventHandler(this.lbl柔震index);
            // 
            // lbl_debug
            // 
            this.lbl_debug.AutoSize = true;
            this.lbl_debug.Location = new System.Drawing.Point(784, 354);
            this.lbl_debug.Name = "lbl_debug";
            this.lbl_debug.Size = new System.Drawing.Size(81, 19);
            this.lbl_debug.TabIndex = 211;
            this.lbl_debug.Text = "lbl_debug";
            // 
            // lbl_植針吹氣流量閥
            // 
            this.lbl_植針吹氣流量閥.AutoSize = true;
            this.lbl_植針吹氣流量閥.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針吹氣流量閥.Location = new System.Drawing.Point(1090, 20);
            this.lbl_植針吹氣流量閥.Name = "lbl_植針吹氣流量閥";
            this.lbl_植針吹氣流量閥.Size = new System.Drawing.Size(116, 13);
            this.lbl_植針吹氣流量閥.TabIndex = 210;
            this.lbl_植針吹氣流量閥.Text = "lbl_植針吹氣流量閥";
            // 
            // btn_minus_d001
            // 
            this.btn_minus_d001.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_minus_d001.Location = new System.Drawing.Point(250, 299);
            this.btn_minus_d001.Name = "btn_minus_d001";
            this.btn_minus_d001.Size = new System.Drawing.Size(75, 23);
            this.btn_minus_d001.TabIndex = 209;
            this.btn_minus_d001.Text = "-0.001";
            this.btn_minus_d001.UseVisualStyleBackColor = true;
            this.btn_minus_d001.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_plus_d001
            // 
            this.btn_plus_d001.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_plus_d001.Location = new System.Drawing.Point(155, 299);
            this.btn_plus_d001.Name = "btn_plus_d001";
            this.btn_plus_d001.Size = new System.Drawing.Size(75, 23);
            this.btn_plus_d001.TabIndex = 208;
            this.btn_plus_d001.Text = "+0.001";
            this.btn_plus_d001.UseVisualStyleBackColor = true;
            this.btn_plus_d001.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_minus_d01
            // 
            this.btn_minus_d01.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_minus_d01.Location = new System.Drawing.Point(250, 328);
            this.btn_minus_d01.Name = "btn_minus_d01";
            this.btn_minus_d01.Size = new System.Drawing.Size(75, 23);
            this.btn_minus_d01.TabIndex = 207;
            this.btn_minus_d01.Text = "-0.01";
            this.btn_minus_d01.UseVisualStyleBackColor = true;
            this.btn_minus_d01.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_plus_d01
            // 
            this.btn_plus_d01.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_plus_d01.Location = new System.Drawing.Point(155, 328);
            this.btn_plus_d01.Name = "btn_plus_d01";
            this.btn_plus_d01.Size = new System.Drawing.Size(75, 23);
            this.btn_plus_d01.TabIndex = 206;
            this.btn_plus_d01.Text = "+0.01";
            this.btn_plus_d01.UseVisualStyleBackColor = true;
            this.btn_plus_d01.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_minus_d1
            // 
            this.btn_minus_d1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_minus_d1.Location = new System.Drawing.Point(250, 357);
            this.btn_minus_d1.Name = "btn_minus_d1";
            this.btn_minus_d1.Size = new System.Drawing.Size(75, 23);
            this.btn_minus_d1.TabIndex = 205;
            this.btn_minus_d1.Text = "-0.1";
            this.btn_minus_d1.UseVisualStyleBackColor = true;
            this.btn_minus_d1.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_plus_d1
            // 
            this.btn_plus_d1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_plus_d1.Location = new System.Drawing.Point(155, 357);
            this.btn_plus_d1.Name = "btn_plus_d1";
            this.btn_plus_d1.Size = new System.Drawing.Size(75, 23);
            this.btn_plus_d1.TabIndex = 204;
            this.btn_plus_d1.Text = "+0.1";
            this.btn_plus_d1.UseVisualStyleBackColor = true;
            this.btn_plus_d1.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // vcb_植針吹氣流量閥
            // 
            this.vcb_植針吹氣流量閥.Location = new System.Drawing.Point(1095, 45);
            this.vcb_植針吹氣流量閥.Maximum = 110;
            this.vcb_植針吹氣流量閥.Minimum = -10;
            this.vcb_植針吹氣流量閥.Name = "vcb_植針吹氣流量閥";
            this.vcb_植針吹氣流量閥.Size = new System.Drawing.Size(20, 200);
            this.vcb_植針吹氣流量閥.TabIndex = 202;
            this.vcb_植針吹氣流量閥.Value = 110;
            this.vcb_植針吹氣流量閥.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vcb流量閥_Scroll);
            // 
            // btnVibrationStop
            // 
            this.btnVibrationStop.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnVibrationStop.Location = new System.Drawing.Point(490, 328);
            this.btnVibrationStop.Name = "btnVibrationStop";
            this.btnVibrationStop.Size = new System.Drawing.Size(116, 23);
            this.btnVibrationStop.TabIndex = 199;
            this.btnVibrationStop.Text = "btnVibrationStop";
            this.btnVibrationStop.UseVisualStyleBackColor = true;
            this.btnVibrationStop.Click += new System.EventHandler(this.btnVibrationStop_Click);
            // 
            // btnVibrationInit
            // 
            this.btnVibrationInit.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnVibrationInit.Location = new System.Drawing.Point(368, 328);
            this.btnVibrationInit.Name = "btnVibrationInit";
            this.btnVibrationInit.Size = new System.Drawing.Size(116, 23);
            this.btnVibrationInit.TabIndex = 198;
            this.btnVibrationInit.Text = "btnVibrationInit";
            this.btnVibrationInit.UseVisualStyleBackColor = true;
            this.btnVibrationInit.Click += new System.EventHandler(this.btnVibrationInit_Click);
            // 
            // lbl_JoDell吸針嘴_Convert
            // 
            this.lbl_JoDell吸針嘴_Convert.AutoSize = true;
            this.lbl_JoDell吸針嘴_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell吸針嘴_Convert.Location = new System.Drawing.Point(590, 245);
            this.lbl_JoDell吸針嘴_Convert.Name = "lbl_JoDell吸針嘴_Convert";
            this.lbl_JoDell吸針嘴_Convert.Size = new System.Drawing.Size(139, 13);
            this.lbl_JoDell吸針嘴_Convert.TabIndex = 197;
            this.lbl_JoDell吸針嘴_Convert.Text = "lbl_JoDell吸針嘴_Convert";
            // 
            // lbl_JoDell吸針嘴_Back
            // 
            this.lbl_JoDell吸針嘴_Back.AutoSize = true;
            this.lbl_JoDell吸針嘴_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell吸針嘴_Back.Location = new System.Drawing.Point(770, 245);
            this.lbl_JoDell吸針嘴_Back.Name = "lbl_JoDell吸針嘴_Back";
            this.lbl_JoDell吸針嘴_Back.Size = new System.Drawing.Size(125, 13);
            this.lbl_JoDell吸針嘴_Back.TabIndex = 196;
            this.lbl_JoDell吸針嘴_Back.Text = "lbl_JoDell吸針嘴_Back";
            // 
            // lbl_JoDell吸針嘴_RAW
            // 
            this.lbl_JoDell吸針嘴_RAW.AutoSize = true;
            this.lbl_JoDell吸針嘴_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell吸針嘴_RAW.Location = new System.Drawing.Point(420, 245);
            this.lbl_JoDell吸針嘴_RAW.Name = "lbl_JoDell吸針嘴_RAW";
            this.lbl_JoDell吸針嘴_RAW.Size = new System.Drawing.Size(130, 13);
            this.lbl_JoDell吸針嘴_RAW.TabIndex = 195;
            this.lbl_JoDell吸針嘴_RAW.Text = "lbl_JoDell吸針嘴_RAW";
            // 
            // en_JoDell吸針嘴
            // 
            this.en_JoDell吸針嘴.AutoSize = true;
            this.en_JoDell吸針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_JoDell吸針嘴.Location = new System.Drawing.Point(25, 245);
            this.en_JoDell吸針嘴.Name = "en_JoDell吸針嘴";
            this.en_JoDell吸針嘴.Size = new System.Drawing.Size(58, 17);
            this.en_JoDell吸針嘴.TabIndex = 194;
            this.en_JoDell吸針嘴.Text = "Enable";
            this.en_JoDell吸針嘴.UseVisualStyleBackColor = true;
            this.en_JoDell吸針嘴.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // lbl_acpos_JoDell吸針嘴
            // 
            this.lbl_acpos_JoDell吸針嘴.AutoSize = true;
            this.lbl_acpos_JoDell吸針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_JoDell吸針嘴.Location = new System.Drawing.Point(340, 245);
            this.lbl_acpos_JoDell吸針嘴.Name = "lbl_acpos_JoDell吸針嘴";
            this.lbl_acpos_JoDell吸針嘴.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_JoDell吸針嘴.TabIndex = 193;
            this.lbl_acpos_JoDell吸針嘴.Text = "0.00";
            // 
            // lbl_acpos_JoDell吸針嘴_lbl
            // 
            this.lbl_acpos_JoDell吸針嘴_lbl.AutoSize = true;
            this.lbl_acpos_JoDell吸針嘴_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_JoDell吸針嘴_lbl.Location = new System.Drawing.Point(290, 245);
            this.lbl_acpos_JoDell吸針嘴_lbl.Name = "lbl_acpos_JoDell吸針嘴_lbl";
            this.lbl_acpos_JoDell吸針嘴_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_JoDell吸針嘴_lbl.TabIndex = 192;
            this.lbl_acpos_JoDell吸針嘴_lbl.Text = "acpos";
            // 
            // lbl_spd_JoDell吸針嘴
            // 
            this.lbl_spd_JoDell吸針嘴.AutoSize = true;
            this.lbl_spd_JoDell吸針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_JoDell吸針嘴.Location = new System.Drawing.Point(230, 245);
            this.lbl_spd_JoDell吸針嘴.Name = "lbl_spd_JoDell吸針嘴";
            this.lbl_spd_JoDell吸針嘴.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_JoDell吸針嘴.TabIndex = 191;
            this.lbl_spd_JoDell吸針嘴.Text = "0.00";
            // 
            // lbl_spd_JoDell吸針嘴_lbl
            // 
            this.lbl_spd_JoDell吸針嘴_lbl.AutoSize = true;
            this.lbl_spd_JoDell吸針嘴_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_JoDell吸針嘴_lbl.Location = new System.Drawing.Point(180, 245);
            this.lbl_spd_JoDell吸針嘴_lbl.Name = "lbl_spd_JoDell吸針嘴_lbl";
            this.lbl_spd_JoDell吸針嘴_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_JoDell吸針嘴_lbl.TabIndex = 190;
            this.lbl_spd_JoDell吸針嘴_lbl.Text = "speed";
            // 
            // select_JoDell吸針嘴
            // 
            this.select_JoDell吸針嘴.AutoSize = true;
            this.select_JoDell吸針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_JoDell吸針嘴.Location = new System.Drawing.Point(95, 245);
            this.select_JoDell吸針嘴.Name = "select_JoDell吸針嘴";
            this.select_JoDell吸針嘴.Size = new System.Drawing.Size(64, 17);
            this.select_JoDell吸針嘴.TabIndex = 189;
            this.select_JoDell吸針嘴.Text = "吸針嘴";
            this.select_JoDell吸針嘴.UseVisualStyleBackColor = true;
            this.select_JoDell吸針嘴.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // lbl_JoDell3D掃描_Convert
            // 
            this.lbl_JoDell3D掃描_Convert.AutoSize = true;
            this.lbl_JoDell3D掃描_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell3D掃描_Convert.Location = new System.Drawing.Point(590, 225);
            this.lbl_JoDell3D掃描_Convert.Name = "lbl_JoDell3D掃描_Convert";
            this.lbl_JoDell3D掃描_Convert.Size = new System.Drawing.Size(141, 13);
            this.lbl_JoDell3D掃描_Convert.TabIndex = 188;
            this.lbl_JoDell3D掃描_Convert.Text = "lbl_JoDell3D掃描_Convert";
            // 
            // lbl_JoDell3D掃描_Back
            // 
            this.lbl_JoDell3D掃描_Back.AutoSize = true;
            this.lbl_JoDell3D掃描_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell3D掃描_Back.Location = new System.Drawing.Point(770, 225);
            this.lbl_JoDell3D掃描_Back.Name = "lbl_JoDell3D掃描_Back";
            this.lbl_JoDell3D掃描_Back.Size = new System.Drawing.Size(127, 13);
            this.lbl_JoDell3D掃描_Back.TabIndex = 187;
            this.lbl_JoDell3D掃描_Back.Text = "lbl_JoDell3D掃描_Back";
            // 
            // lbl_JoDell3D掃描_RAW
            // 
            this.lbl_JoDell3D掃描_RAW.AutoSize = true;
            this.lbl_JoDell3D掃描_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell3D掃描_RAW.Location = new System.Drawing.Point(420, 225);
            this.lbl_JoDell3D掃描_RAW.Name = "lbl_JoDell3D掃描_RAW";
            this.lbl_JoDell3D掃描_RAW.Size = new System.Drawing.Size(132, 13);
            this.lbl_JoDell3D掃描_RAW.TabIndex = 186;
            this.lbl_JoDell3D掃描_RAW.Text = "lbl_JoDell3D掃描_RAW";
            // 
            // en_JoDell3D掃描
            // 
            this.en_JoDell3D掃描.AutoSize = true;
            this.en_JoDell3D掃描.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_JoDell3D掃描.Location = new System.Drawing.Point(25, 225);
            this.en_JoDell3D掃描.Name = "en_JoDell3D掃描";
            this.en_JoDell3D掃描.Size = new System.Drawing.Size(58, 17);
            this.en_JoDell3D掃描.TabIndex = 185;
            this.en_JoDell3D掃描.Text = "Enable";
            this.en_JoDell3D掃描.UseVisualStyleBackColor = true;
            this.en_JoDell3D掃描.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // lbl_acpos_JoDell3D掃描
            // 
            this.lbl_acpos_JoDell3D掃描.AutoSize = true;
            this.lbl_acpos_JoDell3D掃描.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_JoDell3D掃描.Location = new System.Drawing.Point(340, 225);
            this.lbl_acpos_JoDell3D掃描.Name = "lbl_acpos_JoDell3D掃描";
            this.lbl_acpos_JoDell3D掃描.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_JoDell3D掃描.TabIndex = 184;
            this.lbl_acpos_JoDell3D掃描.Text = "0.00";
            // 
            // lbl_acpos_JoDell3D掃描_lbl
            // 
            this.lbl_acpos_JoDell3D掃描_lbl.AutoSize = true;
            this.lbl_acpos_JoDell3D掃描_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_JoDell3D掃描_lbl.Location = new System.Drawing.Point(290, 225);
            this.lbl_acpos_JoDell3D掃描_lbl.Name = "lbl_acpos_JoDell3D掃描_lbl";
            this.lbl_acpos_JoDell3D掃描_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_JoDell3D掃描_lbl.TabIndex = 183;
            this.lbl_acpos_JoDell3D掃描_lbl.Text = "acpos";
            // 
            // lbl_spd_JoDell3D掃描
            // 
            this.lbl_spd_JoDell3D掃描.AutoSize = true;
            this.lbl_spd_JoDell3D掃描.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_JoDell3D掃描.Location = new System.Drawing.Point(230, 225);
            this.lbl_spd_JoDell3D掃描.Name = "lbl_spd_JoDell3D掃描";
            this.lbl_spd_JoDell3D掃描.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_JoDell3D掃描.TabIndex = 182;
            this.lbl_spd_JoDell3D掃描.Text = "0.00";
            // 
            // lbl_spd_JoDell3D掃描_lbl
            // 
            this.lbl_spd_JoDell3D掃描_lbl.AutoSize = true;
            this.lbl_spd_JoDell3D掃描_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_JoDell3D掃描_lbl.Location = new System.Drawing.Point(180, 225);
            this.lbl_spd_JoDell3D掃描_lbl.Name = "lbl_spd_JoDell3D掃描_lbl";
            this.lbl_spd_JoDell3D掃描_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_JoDell3D掃描_lbl.TabIndex = 181;
            this.lbl_spd_JoDell3D掃描_lbl.Text = "speed";
            // 
            // select_JoDell3D掃描
            // 
            this.select_JoDell3D掃描.AutoSize = true;
            this.select_JoDell3D掃描.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_JoDell3D掃描.Location = new System.Drawing.Point(95, 225);
            this.select_JoDell3D掃描.Name = "select_JoDell3D掃描";
            this.select_JoDell3D掃描.Size = new System.Drawing.Size(66, 17);
            this.select_JoDell3D掃描.TabIndex = 180;
            this.select_JoDell3D掃描.Text = "3D掃描";
            this.select_JoDell3D掃描.UseVisualStyleBackColor = true;
            this.select_JoDell3D掃描.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // lbl_JoDell植針嘴_Convert
            // 
            this.lbl_JoDell植針嘴_Convert.AutoSize = true;
            this.lbl_JoDell植針嘴_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell植針嘴_Convert.Location = new System.Drawing.Point(590, 265);
            this.lbl_JoDell植針嘴_Convert.Name = "lbl_JoDell植針嘴_Convert";
            this.lbl_JoDell植針嘴_Convert.Size = new System.Drawing.Size(139, 13);
            this.lbl_JoDell植針嘴_Convert.TabIndex = 179;
            this.lbl_JoDell植針嘴_Convert.Text = "lbl_JoDell植針嘴_Convert";
            // 
            // lbl_JoDell植針嘴_Back
            // 
            this.lbl_JoDell植針嘴_Back.AutoSize = true;
            this.lbl_JoDell植針嘴_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell植針嘴_Back.Location = new System.Drawing.Point(770, 265);
            this.lbl_JoDell植針嘴_Back.Name = "lbl_JoDell植針嘴_Back";
            this.lbl_JoDell植針嘴_Back.Size = new System.Drawing.Size(125, 13);
            this.lbl_JoDell植針嘴_Back.TabIndex = 178;
            this.lbl_JoDell植針嘴_Back.Text = "lbl_JoDell植針嘴_Back";
            // 
            // lbl_JoDell植針嘴_RAW
            // 
            this.lbl_JoDell植針嘴_RAW.AutoSize = true;
            this.lbl_JoDell植針嘴_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_JoDell植針嘴_RAW.Location = new System.Drawing.Point(420, 265);
            this.lbl_JoDell植針嘴_RAW.Name = "lbl_JoDell植針嘴_RAW";
            this.lbl_JoDell植針嘴_RAW.Size = new System.Drawing.Size(130, 13);
            this.lbl_JoDell植針嘴_RAW.TabIndex = 177;
            this.lbl_JoDell植針嘴_RAW.Text = "lbl_JoDell植針嘴_RAW";
            // 
            // en_JoDell植針嘴
            // 
            this.en_JoDell植針嘴.AutoSize = true;
            this.en_JoDell植針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_JoDell植針嘴.Location = new System.Drawing.Point(25, 265);
            this.en_JoDell植針嘴.Name = "en_JoDell植針嘴";
            this.en_JoDell植針嘴.Size = new System.Drawing.Size(58, 17);
            this.en_JoDell植針嘴.TabIndex = 176;
            this.en_JoDell植針嘴.Text = "Enable";
            this.en_JoDell植針嘴.UseVisualStyleBackColor = true;
            this.en_JoDell植針嘴.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // lbl_acpos_JoDell植針嘴
            // 
            this.lbl_acpos_JoDell植針嘴.AutoSize = true;
            this.lbl_acpos_JoDell植針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_JoDell植針嘴.Location = new System.Drawing.Point(340, 265);
            this.lbl_acpos_JoDell植針嘴.Name = "lbl_acpos_JoDell植針嘴";
            this.lbl_acpos_JoDell植針嘴.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_JoDell植針嘴.TabIndex = 175;
            this.lbl_acpos_JoDell植針嘴.Text = "0.00";
            // 
            // lbl_acpos_JoDell植針嘴_lbl
            // 
            this.lbl_acpos_JoDell植針嘴_lbl.AutoSize = true;
            this.lbl_acpos_JoDell植針嘴_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_JoDell植針嘴_lbl.Location = new System.Drawing.Point(290, 265);
            this.lbl_acpos_JoDell植針嘴_lbl.Name = "lbl_acpos_JoDell植針嘴_lbl";
            this.lbl_acpos_JoDell植針嘴_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_JoDell植針嘴_lbl.TabIndex = 174;
            this.lbl_acpos_JoDell植針嘴_lbl.Text = "acpos";
            // 
            // lbl_spd_JoDell植針嘴
            // 
            this.lbl_spd_JoDell植針嘴.AutoSize = true;
            this.lbl_spd_JoDell植針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_JoDell植針嘴.Location = new System.Drawing.Point(230, 265);
            this.lbl_spd_JoDell植針嘴.Name = "lbl_spd_JoDell植針嘴";
            this.lbl_spd_JoDell植針嘴.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_JoDell植針嘴.TabIndex = 173;
            this.lbl_spd_JoDell植針嘴.Text = "0.00";
            // 
            // lbl_spd_JoDell植針嘴_lbl
            // 
            this.lbl_spd_JoDell植針嘴_lbl.AutoSize = true;
            this.lbl_spd_JoDell植針嘴_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_JoDell植針嘴_lbl.Location = new System.Drawing.Point(180, 265);
            this.lbl_spd_JoDell植針嘴_lbl.Name = "lbl_spd_JoDell植針嘴_lbl";
            this.lbl_spd_JoDell植針嘴_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_JoDell植針嘴_lbl.TabIndex = 172;
            this.lbl_spd_JoDell植針嘴_lbl.Text = "speed";
            // 
            // select_JoDell植針嘴
            // 
            this.select_JoDell植針嘴.AutoSize = true;
            this.select_JoDell植針嘴.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_JoDell植針嘴.Location = new System.Drawing.Point(95, 265);
            this.select_JoDell植針嘴.Name = "select_JoDell植針嘴";
            this.select_JoDell植針嘴.Size = new System.Drawing.Size(64, 17);
            this.select_JoDell植針嘴.TabIndex = 171;
            this.select_JoDell植針嘴.Text = "植針嘴";
            this.select_JoDell植針嘴.UseVisualStyleBackColor = true;
            this.select_JoDell植針嘴.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // lbl_IAI_Convert
            // 
            this.lbl_IAI_Convert.AutoSize = true;
            this.lbl_IAI_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_IAI_Convert.Location = new System.Drawing.Point(590, 205);
            this.lbl_IAI_Convert.Name = "lbl_IAI_Convert";
            this.lbl_IAI_Convert.Size = new System.Drawing.Size(86, 13);
            this.lbl_IAI_Convert.TabIndex = 170;
            this.lbl_IAI_Convert.Text = "lbl_IAI_Convert";
            // 
            // lbl_IAI_Back
            // 
            this.lbl_IAI_Back.AutoSize = true;
            this.lbl_IAI_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_IAI_Back.Location = new System.Drawing.Point(770, 205);
            this.lbl_IAI_Back.Name = "lbl_IAI_Back";
            this.lbl_IAI_Back.Size = new System.Drawing.Size(72, 13);
            this.lbl_IAI_Back.TabIndex = 169;
            this.lbl_IAI_Back.Text = "lbl_IAI_Back";
            // 
            // lbl_IAI_RAW
            // 
            this.lbl_IAI_RAW.AutoSize = true;
            this.lbl_IAI_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_IAI_RAW.Location = new System.Drawing.Point(420, 205);
            this.lbl_IAI_RAW.Name = "lbl_IAI_RAW";
            this.lbl_IAI_RAW.Size = new System.Drawing.Size(77, 13);
            this.lbl_IAI_RAW.TabIndex = 168;
            this.lbl_IAI_RAW.Text = "lbl_IAI_RAW";
            // 
            // en_IAI
            // 
            this.en_IAI.AutoSize = true;
            this.en_IAI.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_IAI.Location = new System.Drawing.Point(25, 205);
            this.en_IAI.Name = "en_IAI";
            this.en_IAI.Size = new System.Drawing.Size(58, 17);
            this.en_IAI.TabIndex = 167;
            this.en_IAI.Text = "Enable";
            this.en_IAI.UseVisualStyleBackColor = true;
            this.en_IAI.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // lbl_acpos_IAI
            // 
            this.lbl_acpos_IAI.AutoSize = true;
            this.lbl_acpos_IAI.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_IAI.Location = new System.Drawing.Point(340, 205);
            this.lbl_acpos_IAI.Name = "lbl_acpos_IAI";
            this.lbl_acpos_IAI.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_IAI.TabIndex = 166;
            this.lbl_acpos_IAI.Text = "0.00";
            // 
            // lbl_acpos_IAI_lbl
            // 
            this.lbl_acpos_IAI_lbl.AutoSize = true;
            this.lbl_acpos_IAI_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_IAI_lbl.Location = new System.Drawing.Point(290, 205);
            this.lbl_acpos_IAI_lbl.Name = "lbl_acpos_IAI_lbl";
            this.lbl_acpos_IAI_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_IAI_lbl.TabIndex = 165;
            this.lbl_acpos_IAI_lbl.Text = "acpos";
            // 
            // lbl_spd_IAI
            // 
            this.lbl_spd_IAI.AutoSize = true;
            this.lbl_spd_IAI.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_IAI.Location = new System.Drawing.Point(230, 205);
            this.lbl_spd_IAI.Name = "lbl_spd_IAI";
            this.lbl_spd_IAI.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_IAI.TabIndex = 164;
            this.lbl_spd_IAI.Text = "0.00";
            // 
            // lbl_spd_IAI_lbl
            // 
            this.lbl_spd_IAI_lbl.AutoSize = true;
            this.lbl_spd_IAI_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_IAI_lbl.Location = new System.Drawing.Point(180, 205);
            this.lbl_spd_IAI_lbl.Name = "lbl_spd_IAI_lbl";
            this.lbl_spd_IAI_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_IAI_lbl.TabIndex = 163;
            this.lbl_spd_IAI_lbl.Text = "speed";
            // 
            // select_Socket檢測
            // 
            this.select_Socket檢測.AutoSize = true;
            this.select_Socket檢測.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_Socket檢測.Location = new System.Drawing.Point(95, 205);
            this.select_Socket檢測.Name = "select_Socket檢測";
            this.select_Socket檢測.Size = new System.Drawing.Size(83, 17);
            this.select_Socket檢測.TabIndex = 162;
            this.select_Socket檢測.Text = "Socket檢測";
            this.select_Socket檢測.UseVisualStyleBackColor = true;
            this.select_Socket檢測.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblBuzzer);
            this.groupBox2.Controls.Add(this.lbl_NA_31);
            this.groupBox2.Controls.Add(this.lbl綠燈);
            this.groupBox2.Controls.Add(this.lbl左按鈕紅燈);
            this.groupBox2.Controls.Add(this.lbl黃燈);
            this.groupBox2.Controls.Add(this.lbl中按鈕綠燈);
            this.groupBox2.Controls.Add(this.lbl紅燈);
            this.groupBox2.Controls.Add(this.lbl右按鈕綠燈);
            this.groupBox2.Controls.Add(this.lbl艙內燈);
            this.groupBox2.Controls.Add(this.lbl取料吸嘴破新);
            this.groupBox2.Controls.Add(this.lblHEPA);
            this.groupBox2.Controls.Add(this.lbl植針Z煞車);
            this.groupBox2.Controls.Add(this.lbl下後右門鎖);
            this.groupBox2.Controls.Add(this.lbl取料吸嘴破舊);
            this.groupBox2.Controls.Add(this.lbl下後左門鎖);
            this.groupBox2.Controls.Add(this.lbl取料吸嘴吸);
            this.groupBox2.Controls.Add(this.lbl擺放破真空);
            this.groupBox2.Controls.Add(this.lblsk破真空1);
            this.groupBox2.Controls.Add(this.lbl擺放座真空);
            this.groupBox2.Controls.Add(this.lblsk真空1);
            this.groupBox2.Controls.Add(this.lblsk破真空2);
            this.groupBox2.Controls.Add(this.lbl載盤破真空);
            this.groupBox2.Controls.Add(this.lblsk真空2);
            this.groupBox2.Controls.Add(this.lbl載盤真空閥);
            this.groupBox2.Controls.Add(this.lbl_NA_25);
            this.groupBox2.Controls.Add(this.lbl堵料吹氣);
            this.groupBox2.Controls.Add(this.lbl收料區缸);
            this.groupBox2.Controls.Add(this.lbl植針吹氣);
            this.groupBox2.Controls.Add(this.lbl接料區缸);
            this.groupBox2.Controls.Add(this.lbl堵料吹氣缸);
            this.groupBox2.Controls.Add(this.lbl吸料真空閥);
            this.groupBox2.Controls.Add(this.lbl擺放蓋板);
            this.groupBox2.Location = new System.Drawing.Point(719, 456);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 210);
            this.groupBox2.TabIndex = 161;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "WriteIO";
            // 
            // lblBuzzer
            // 
            this.lblBuzzer.AutoSize = true;
            this.lblBuzzer.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBuzzer.Location = new System.Drawing.Point(320, 180);
            this.lblBuzzer.Name = "lblBuzzer";
            this.lblBuzzer.Size = new System.Drawing.Size(52, 13);
            this.lblBuzzer.TabIndex = 191;
            this.lblBuzzer.Text = "lblBuzzer";
            this.lblBuzzer.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl_NA_31
            // 
            this.lbl_NA_31.AutoSize = true;
            this.lbl_NA_31.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_31.Location = new System.Drawing.Point(320, 160);
            this.lbl_NA_31.Name = "lbl_NA_31";
            this.lbl_NA_31.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_31.TabIndex = 190;
            this.lbl_NA_31.Text = "lbl_NA_31";
            this.lbl_NA_31.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl綠燈
            // 
            this.lbl綠燈.AutoSize = true;
            this.lbl綠燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl綠燈.Location = new System.Drawing.Point(320, 140);
            this.lbl綠燈.Name = "lbl綠燈";
            this.lbl綠燈.Size = new System.Drawing.Size(45, 13);
            this.lbl綠燈.TabIndex = 189;
            this.lbl綠燈.Text = "lbl綠燈";
            this.lbl綠燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl左按鈕紅燈
            // 
            this.lbl左按鈕紅燈.AutoSize = true;
            this.lbl左按鈕紅燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl左按鈕紅燈.Location = new System.Drawing.Point(320, 120);
            this.lbl左按鈕紅燈.Name = "lbl左按鈕紅燈";
            this.lbl左按鈕紅燈.Size = new System.Drawing.Size(84, 13);
            this.lbl左按鈕紅燈.TabIndex = 188;
            this.lbl左按鈕紅燈.Text = "lbl左按鈕紅燈";
            this.lbl左按鈕紅燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl黃燈
            // 
            this.lbl黃燈.AutoSize = true;
            this.lbl黃燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl黃燈.Location = new System.Drawing.Point(320, 100);
            this.lbl黃燈.Name = "lbl黃燈";
            this.lbl黃燈.Size = new System.Drawing.Size(45, 13);
            this.lbl黃燈.TabIndex = 187;
            this.lbl黃燈.Text = "lbl黃燈";
            this.lbl黃燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl中按鈕綠燈
            // 
            this.lbl中按鈕綠燈.AutoSize = true;
            this.lbl中按鈕綠燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl中按鈕綠燈.Location = new System.Drawing.Point(320, 80);
            this.lbl中按鈕綠燈.Name = "lbl中按鈕綠燈";
            this.lbl中按鈕綠燈.Size = new System.Drawing.Size(84, 13);
            this.lbl中按鈕綠燈.TabIndex = 186;
            this.lbl中按鈕綠燈.Text = "lbl中按鈕綠燈";
            this.lbl中按鈕綠燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl紅燈
            // 
            this.lbl紅燈.AutoSize = true;
            this.lbl紅燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl紅燈.Location = new System.Drawing.Point(320, 60);
            this.lbl紅燈.Name = "lbl紅燈";
            this.lbl紅燈.Size = new System.Drawing.Size(45, 13);
            this.lbl紅燈.TabIndex = 185;
            this.lbl紅燈.Text = "lbl紅燈";
            this.lbl紅燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl右按鈕綠燈
            // 
            this.lbl右按鈕綠燈.AutoSize = true;
            this.lbl右按鈕綠燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl右按鈕綠燈.Location = new System.Drawing.Point(320, 40);
            this.lbl右按鈕綠燈.Name = "lbl右按鈕綠燈";
            this.lbl右按鈕綠燈.Size = new System.Drawing.Size(84, 13);
            this.lbl右按鈕綠燈.TabIndex = 184;
            this.lbl右按鈕綠燈.Text = "lbl右按鈕綠燈";
            this.lbl右按鈕綠燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl艙內燈
            // 
            this.lbl艙內燈.AutoSize = true;
            this.lbl艙內燈.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl艙內燈.Location = new System.Drawing.Point(220, 180);
            this.lbl艙內燈.Name = "lbl艙內燈";
            this.lbl艙內燈.Size = new System.Drawing.Size(58, 13);
            this.lbl艙內燈.TabIndex = 183;
            this.lbl艙內燈.Text = "lbl艙內燈";
            this.lbl艙內燈.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl取料吸嘴破新
            // 
            this.lbl取料吸嘴破新.AutoSize = true;
            this.lbl取料吸嘴破新.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料吸嘴破新.Location = new System.Drawing.Point(220, 160);
            this.lbl取料吸嘴破新.Name = "lbl取料吸嘴破新";
            this.lbl取料吸嘴破新.Size = new System.Drawing.Size(97, 13);
            this.lbl取料吸嘴破新.TabIndex = 182;
            this.lbl取料吸嘴破新.Text = "lbl取料吸嘴破新";
            this.lbl取料吸嘴破新.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lblHEPA
            // 
            this.lblHEPA.AutoSize = true;
            this.lblHEPA.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHEPA.Location = new System.Drawing.Point(220, 140);
            this.lblHEPA.Name = "lblHEPA";
            this.lblHEPA.Size = new System.Drawing.Size(51, 13);
            this.lblHEPA.TabIndex = 181;
            this.lblHEPA.Text = "lblHEPA";
            this.lblHEPA.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl植針Z煞車
            // 
            this.lbl植針Z煞車.AutoSize = true;
            this.lbl植針Z煞車.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl植針Z煞車.Location = new System.Drawing.Point(220, 120);
            this.lbl植針Z煞車.Name = "lbl植針Z煞車";
            this.lbl植針Z煞車.Size = new System.Drawing.Size(78, 13);
            this.lbl植針Z煞車.TabIndex = 180;
            this.lbl植針Z煞車.Text = "lbl植針Z煞車";
            this.lbl植針Z煞車.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl下後右門鎖
            // 
            this.lbl下後右門鎖.AutoSize = true;
            this.lbl下後右門鎖.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下後右門鎖.Location = new System.Drawing.Point(220, 100);
            this.lbl下後右門鎖.Name = "lbl下後右門鎖";
            this.lbl下後右門鎖.Size = new System.Drawing.Size(84, 13);
            this.lbl下後右門鎖.TabIndex = 179;
            this.lbl下後右門鎖.Text = "lbl下後右門鎖";
            this.lbl下後右門鎖.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl取料吸嘴破舊
            // 
            this.lbl取料吸嘴破舊.AutoSize = true;
            this.lbl取料吸嘴破舊.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料吸嘴破舊.Location = new System.Drawing.Point(220, 80);
            this.lbl取料吸嘴破舊.Name = "lbl取料吸嘴破舊";
            this.lbl取料吸嘴破舊.Size = new System.Drawing.Size(97, 13);
            this.lbl取料吸嘴破舊.TabIndex = 178;
            this.lbl取料吸嘴破舊.Text = "lbl取料吸嘴破舊";
            this.lbl取料吸嘴破舊.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl下後左門鎖
            // 
            this.lbl下後左門鎖.AutoSize = true;
            this.lbl下後左門鎖.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下後左門鎖.Location = new System.Drawing.Point(220, 60);
            this.lbl下後左門鎖.Name = "lbl下後左門鎖";
            this.lbl下後左門鎖.Size = new System.Drawing.Size(84, 13);
            this.lbl下後左門鎖.TabIndex = 177;
            this.lbl下後左門鎖.Text = "lbl下後左門鎖";
            this.lbl下後左門鎖.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl取料吸嘴吸
            // 
            this.lbl取料吸嘴吸.AutoSize = true;
            this.lbl取料吸嘴吸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料吸嘴吸.Location = new System.Drawing.Point(220, 40);
            this.lbl取料吸嘴吸.Name = "lbl取料吸嘴吸";
            this.lbl取料吸嘴吸.Size = new System.Drawing.Size(84, 13);
            this.lbl取料吸嘴吸.TabIndex = 176;
            this.lbl取料吸嘴吸.Text = "lbl取料吸嘴吸";
            this.lbl取料吸嘴吸.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl擺放破真空
            // 
            this.lbl擺放破真空.AutoSize = true;
            this.lbl擺放破真空.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl擺放破真空.Location = new System.Drawing.Point(120, 180);
            this.lbl擺放破真空.Name = "lbl擺放破真空";
            this.lbl擺放破真空.Size = new System.Drawing.Size(84, 13);
            this.lbl擺放破真空.TabIndex = 175;
            this.lbl擺放破真空.Text = "lbl擺放破真空";
            this.lbl擺放破真空.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lblsk破真空1
            // 
            this.lblsk破真空1.AutoSize = true;
            this.lblsk破真空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk破真空1.Location = new System.Drawing.Point(120, 160);
            this.lblsk破真空1.Name = "lblsk破真空1";
            this.lblsk破真空1.Size = new System.Drawing.Size(75, 13);
            this.lblsk破真空1.TabIndex = 174;
            this.lblsk破真空1.Text = "lblsk破真空1";
            this.lblsk破真空1.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl擺放座真空
            // 
            this.lbl擺放座真空.AutoSize = true;
            this.lbl擺放座真空.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl擺放座真空.Location = new System.Drawing.Point(120, 140);
            this.lbl擺放座真空.Name = "lbl擺放座真空";
            this.lbl擺放座真空.Size = new System.Drawing.Size(84, 13);
            this.lbl擺放座真空.TabIndex = 173;
            this.lbl擺放座真空.Text = "lbl擺放座真空";
            this.lbl擺放座真空.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lblsk真空1
            // 
            this.lblsk真空1.AutoSize = true;
            this.lblsk真空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk真空1.Location = new System.Drawing.Point(120, 120);
            this.lblsk真空1.Name = "lblsk真空1";
            this.lblsk真空1.Size = new System.Drawing.Size(62, 13);
            this.lblsk真空1.TabIndex = 172;
            this.lblsk真空1.Text = "lblsk真空1";
            this.lblsk真空1.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lblsk破真空2
            // 
            this.lblsk破真空2.AutoSize = true;
            this.lblsk破真空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk破真空2.Location = new System.Drawing.Point(120, 100);
            this.lblsk破真空2.Name = "lblsk破真空2";
            this.lblsk破真空2.Size = new System.Drawing.Size(75, 13);
            this.lblsk破真空2.TabIndex = 171;
            this.lblsk破真空2.Text = "lblsk破真空2";
            this.lblsk破真空2.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl載盤破真空
            // 
            this.lbl載盤破真空.AutoSize = true;
            this.lbl載盤破真空.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤破真空.Location = new System.Drawing.Point(120, 80);
            this.lbl載盤破真空.Name = "lbl載盤破真空";
            this.lbl載盤破真空.Size = new System.Drawing.Size(84, 13);
            this.lbl載盤破真空.TabIndex = 170;
            this.lbl載盤破真空.Text = "lbl載盤破真空";
            this.lbl載盤破真空.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lblsk真空2
            // 
            this.lblsk真空2.AutoSize = true;
            this.lblsk真空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk真空2.Location = new System.Drawing.Point(120, 60);
            this.lblsk真空2.Name = "lblsk真空2";
            this.lblsk真空2.Size = new System.Drawing.Size(62, 13);
            this.lblsk真空2.TabIndex = 169;
            this.lblsk真空2.Text = "lblsk真空2";
            this.lblsk真空2.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl載盤真空閥
            // 
            this.lbl載盤真空閥.AutoSize = true;
            this.lbl載盤真空閥.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤真空閥.Location = new System.Drawing.Point(120, 40);
            this.lbl載盤真空閥.Name = "lbl載盤真空閥";
            this.lbl載盤真空閥.Size = new System.Drawing.Size(84, 13);
            this.lbl載盤真空閥.TabIndex = 168;
            this.lbl載盤真空閥.Text = "lbl載盤真空閥";
            this.lbl載盤真空閥.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl_NA_25
            // 
            this.lbl_NA_25.AutoSize = true;
            this.lbl_NA_25.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_25.Location = new System.Drawing.Point(20, 180);
            this.lbl_NA_25.Name = "lbl_NA_25";
            this.lbl_NA_25.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_25.TabIndex = 167;
            this.lbl_NA_25.Text = "lbl_NA_25";
            this.lbl_NA_25.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl堵料吹氣
            // 
            this.lbl堵料吹氣.AutoSize = true;
            this.lbl堵料吹氣.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl堵料吹氣.Location = new System.Drawing.Point(20, 160);
            this.lbl堵料吹氣.Name = "lbl堵料吹氣";
            this.lbl堵料吹氣.Size = new System.Drawing.Size(71, 13);
            this.lbl堵料吹氣.TabIndex = 166;
            this.lbl堵料吹氣.Text = "lbl堵料吹氣";
            this.lbl堵料吹氣.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl收料區缸
            // 
            this.lbl收料區缸.AutoSize = true;
            this.lbl收料區缸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl收料區缸.Location = new System.Drawing.Point(20, 140);
            this.lbl收料區缸.Name = "lbl收料區缸";
            this.lbl收料區缸.Size = new System.Drawing.Size(71, 13);
            this.lbl收料區缸.TabIndex = 165;
            this.lbl收料區缸.Text = "lbl收料區缸";
            this.lbl收料區缸.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl植針吹氣
            // 
            this.lbl植針吹氣.AutoSize = true;
            this.lbl植針吹氣.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl植針吹氣.Location = new System.Drawing.Point(20, 120);
            this.lbl植針吹氣.Name = "lbl植針吹氣";
            this.lbl植針吹氣.Size = new System.Drawing.Size(71, 13);
            this.lbl植針吹氣.TabIndex = 164;
            this.lbl植針吹氣.Text = "lbl植針吹氣";
            this.lbl植針吹氣.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl接料區缸
            // 
            this.lbl接料區缸.AutoSize = true;
            this.lbl接料區缸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl接料區缸.Location = new System.Drawing.Point(20, 100);
            this.lbl接料區缸.Name = "lbl接料區缸";
            this.lbl接料區缸.Size = new System.Drawing.Size(71, 13);
            this.lbl接料區缸.TabIndex = 163;
            this.lbl接料區缸.Text = "lbl接料區缸";
            this.lbl接料區缸.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl堵料吹氣缸
            // 
            this.lbl堵料吹氣缸.AutoSize = true;
            this.lbl堵料吹氣缸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl堵料吹氣缸.Location = new System.Drawing.Point(20, 80);
            this.lbl堵料吹氣缸.Name = "lbl堵料吹氣缸";
            this.lbl堵料吹氣缸.Size = new System.Drawing.Size(84, 13);
            this.lbl堵料吹氣缸.TabIndex = 162;
            this.lbl堵料吹氣缸.Text = "lbl堵料吹氣缸";
            this.lbl堵料吹氣缸.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl吸料真空閥
            // 
            this.lbl吸料真空閥.AutoSize = true;
            this.lbl吸料真空閥.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl吸料真空閥.Location = new System.Drawing.Point(20, 60);
            this.lbl吸料真空閥.Name = "lbl吸料真空閥";
            this.lbl吸料真空閥.Size = new System.Drawing.Size(84, 13);
            this.lbl吸料真空閥.TabIndex = 161;
            this.lbl吸料真空閥.Text = "lbl吸料真空閥";
            this.lbl吸料真空閥.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // lbl擺放蓋板
            // 
            this.lbl擺放蓋板.AutoSize = true;
            this.lbl擺放蓋板.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl擺放蓋板.Location = new System.Drawing.Point(20, 40);
            this.lbl擺放蓋板.Name = "lbl擺放蓋板";
            this.lbl擺放蓋板.Size = new System.Drawing.Size(71, 13);
            this.lbl擺放蓋板.TabIndex = 160;
            this.lbl擺放蓋板.Text = "lbl擺放蓋板";
            this.lbl擺放蓋板.Click += new System.EventHandler(this.lbl_SetIO_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_NA_24);
            this.groupBox1.Controls.Add(this.lbl下右左門);
            this.groupBox1.Controls.Add(this.lbl_NA_23);
            this.groupBox1.Controls.Add(this.lbl下右右門);
            this.groupBox1.Controls.Add(this.lbl下後右門);
            this.groupBox1.Controls.Add(this.lbl下左左門);
            this.groupBox1.Controls.Add(this.lbl下後左門);
            this.groupBox1.Controls.Add(this.lbl下左右門);
            this.groupBox1.Controls.Add(this.lbl_NA_20);
            this.groupBox1.Controls.Add(this.lbl上後左門);
            this.groupBox1.Controls.Add(this.lbl螢幕小門);
            this.groupBox1.Controls.Add(this.lbl上後右門);
            this.groupBox1.Controls.Add(this.lbl上右左門);
            this.groupBox1.Controls.Add(this.lbl上左左門);
            this.groupBox1.Controls.Add(this.lbl上右右門);
            this.groupBox1.Controls.Add(this.lbl上左右門);
            this.groupBox1.Controls.Add(this.lbl_NA_19);
            this.groupBox1.Controls.Add(this.lbl_NA_18);
            this.groupBox1.Controls.Add(this.lbl_NA_17);
            this.groupBox1.Controls.Add(this.lbl_NA_16);
            this.groupBox1.Controls.Add(this.lbl_NA_15);
            this.groupBox1.Controls.Add(this.lbl_擺放座關);
            this.groupBox1.Controls.Add(this.lbl_NA_13);
            this.groupBox1.Controls.Add(this.lbl_擺放座開);
            this.groupBox1.Controls.Add(this.lbl_NA_11);
            this.groupBox1.Controls.Add(this.lbl急停鈕);
            this.groupBox1.Controls.Add(this.lbl_NA_10);
            this.groupBox1.Controls.Add(this.lbl停止鈕);
            this.groupBox1.Controls.Add(this.lbl_NA_09);
            this.groupBox1.Controls.Add(this.lbl啟動鈕);
            this.groupBox1.Controls.Add(this.lbl_NA_08);
            this.groupBox1.Controls.Add(this.lbl復歸鈕);
            this.groupBox1.Controls.Add(this.lbl吸料盒);
            this.groupBox1.Controls.Add(this.lbl兩點壓2);
            this.groupBox1.Controls.Add(this.lbl堵料盒);
            this.groupBox1.Controls.Add(this.lbl兩點壓1);
            this.groupBox1.Controls.Add(this.lbl取料ng盒);
            this.groupBox1.Controls.Add(this.lbl吸嘴空2);
            this.groupBox1.Controls.Add(this.lbl_NA_07);
            this.groupBox1.Controls.Add(this.lbl吸嘴空1);
            this.groupBox1.Controls.Add(this.lbl擺放空2);
            this.groupBox1.Controls.Add(this.lblsk1空2);
            this.groupBox1.Controls.Add(this.lbl擺放空1);
            this.groupBox1.Controls.Add(this.lblsk1空1);
            this.groupBox1.Controls.Add(this.lblsk2空2);
            this.groupBox1.Controls.Add(this.lbl載盤空2);
            this.groupBox1.Controls.Add(this.lblsk2空1);
            this.groupBox1.Controls.Add(this.lbl載盤空1);
            this.groupBox1.Controls.Add(this.lbl_NA_06);
            this.groupBox1.Controls.Add(this.lbl載盤X後);
            this.groupBox1.Controls.Add(this.lbl_NA_05);
            this.groupBox1.Controls.Add(this.lbl載盤X前);
            this.groupBox1.Controls.Add(this.lbl_NA_04);
            this.groupBox1.Controls.Add(this.lbl植針Z前);
            this.groupBox1.Controls.Add(this.lbl_NA_03);
            this.groupBox1.Controls.Add(this.lbl植針Z後);
            this.groupBox1.Controls.Add(this.lbl_NA_02);
            this.groupBox1.Controls.Add(this.lbl取料X前);
            this.groupBox1.Controls.Add(this.lbl_NA_01);
            this.groupBox1.Controls.Add(this.lbl取料X後);
            this.groupBox1.Controls.Add(this.lbl取料Y前);
            this.groupBox1.Controls.Add(this.lbl載盤Y前);
            this.groupBox1.Controls.Add(this.lbl取料Y後);
            this.groupBox1.Controls.Add(this.lbl載盤Y後);
            this.groupBox1.Location = new System.Drawing.Point(25, 456);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 210);
            this.groupBox1.TabIndex = 160;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ReadIO";
            // 
            // lbl_NA_24
            // 
            this.lbl_NA_24.AutoSize = true;
            this.lbl_NA_24.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_24.Location = new System.Drawing.Point(580, 180);
            this.lbl_NA_24.Name = "lbl_NA_24";
            this.lbl_NA_24.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_24.TabIndex = 223;
            this.lbl_NA_24.Text = "lbl_NA_24";
            // 
            // lbl下右左門
            // 
            this.lbl下右左門.AutoSize = true;
            this.lbl下右左門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下右左門.Location = new System.Drawing.Point(580, 160);
            this.lbl下右左門.Name = "lbl下右左門";
            this.lbl下右左門.Size = new System.Drawing.Size(71, 13);
            this.lbl下右左門.TabIndex = 222;
            this.lbl下右左門.Text = "lbl下右左門";
            // 
            // lbl_NA_23
            // 
            this.lbl_NA_23.AutoSize = true;
            this.lbl_NA_23.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_23.Location = new System.Drawing.Point(580, 140);
            this.lbl_NA_23.Name = "lbl_NA_23";
            this.lbl_NA_23.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_23.TabIndex = 221;
            this.lbl_NA_23.Text = "lbl_NA_23";
            // 
            // lbl下右右門
            // 
            this.lbl下右右門.AutoSize = true;
            this.lbl下右右門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下右右門.Location = new System.Drawing.Point(580, 120);
            this.lbl下右右門.Name = "lbl下右右門";
            this.lbl下右右門.Size = new System.Drawing.Size(71, 13);
            this.lbl下右右門.TabIndex = 220;
            this.lbl下右右門.Text = "lbl下右右門";
            // 
            // lbl下後右門
            // 
            this.lbl下後右門.AutoSize = true;
            this.lbl下後右門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下後右門.Location = new System.Drawing.Point(580, 100);
            this.lbl下後右門.Name = "lbl下後右門";
            this.lbl下後右門.Size = new System.Drawing.Size(71, 13);
            this.lbl下後右門.TabIndex = 219;
            this.lbl下後右門.Text = "lbl下後右門";
            // 
            // lbl下左左門
            // 
            this.lbl下左左門.AutoSize = true;
            this.lbl下左左門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下左左門.Location = new System.Drawing.Point(580, 80);
            this.lbl下左左門.Name = "lbl下左左門";
            this.lbl下左左門.Size = new System.Drawing.Size(71, 13);
            this.lbl下左左門.TabIndex = 218;
            this.lbl下左左門.Text = "lbl下左左門";
            // 
            // lbl下後左門
            // 
            this.lbl下後左門.AutoSize = true;
            this.lbl下後左門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下後左門.Location = new System.Drawing.Point(580, 60);
            this.lbl下後左門.Name = "lbl下後左門";
            this.lbl下後左門.Size = new System.Drawing.Size(71, 13);
            this.lbl下後左門.TabIndex = 217;
            this.lbl下後左門.Text = "lbl下後左門";
            // 
            // lbl下左右門
            // 
            this.lbl下左右門.AutoSize = true;
            this.lbl下左右門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl下左右門.Location = new System.Drawing.Point(580, 40);
            this.lbl下左右門.Name = "lbl下左右門";
            this.lbl下左右門.Size = new System.Drawing.Size(71, 13);
            this.lbl下左右門.TabIndex = 216;
            this.lbl下左右門.Text = "lbl下左右門";
            // 
            // lbl_NA_20
            // 
            this.lbl_NA_20.AutoSize = true;
            this.lbl_NA_20.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_20.Location = new System.Drawing.Point(500, 180);
            this.lbl_NA_20.Name = "lbl_NA_20";
            this.lbl_NA_20.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_20.TabIndex = 215;
            this.lbl_NA_20.Text = "lbl_NA_20";
            // 
            // lbl上後左門
            // 
            this.lbl上後左門.AutoSize = true;
            this.lbl上後左門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上後左門.Location = new System.Drawing.Point(500, 160);
            this.lbl上後左門.Name = "lbl上後左門";
            this.lbl上後左門.Size = new System.Drawing.Size(71, 13);
            this.lbl上後左門.TabIndex = 214;
            this.lbl上後左門.Text = "lbl上後左門";
            // 
            // lbl螢幕小門
            // 
            this.lbl螢幕小門.AutoSize = true;
            this.lbl螢幕小門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl螢幕小門.Location = new System.Drawing.Point(500, 140);
            this.lbl螢幕小門.Name = "lbl螢幕小門";
            this.lbl螢幕小門.Size = new System.Drawing.Size(71, 13);
            this.lbl螢幕小門.TabIndex = 213;
            this.lbl螢幕小門.Text = "lbl螢幕小門";
            // 
            // lbl上後右門
            // 
            this.lbl上後右門.AutoSize = true;
            this.lbl上後右門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上後右門.Location = new System.Drawing.Point(500, 120);
            this.lbl上後右門.Name = "lbl上後右門";
            this.lbl上後右門.Size = new System.Drawing.Size(71, 13);
            this.lbl上後右門.TabIndex = 212;
            this.lbl上後右門.Text = "lbl上後右門";
            // 
            // lbl上右左門
            // 
            this.lbl上右左門.AutoSize = true;
            this.lbl上右左門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上右左門.Location = new System.Drawing.Point(500, 100);
            this.lbl上右左門.Name = "lbl上右左門";
            this.lbl上右左門.Size = new System.Drawing.Size(71, 13);
            this.lbl上右左門.TabIndex = 211;
            this.lbl上右左門.Text = "lbl上右左門";
            // 
            // lbl上左左門
            // 
            this.lbl上左左門.AutoSize = true;
            this.lbl上左左門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上左左門.Location = new System.Drawing.Point(500, 80);
            this.lbl上左左門.Name = "lbl上左左門";
            this.lbl上左左門.Size = new System.Drawing.Size(71, 13);
            this.lbl上左左門.TabIndex = 210;
            this.lbl上左左門.Text = "lbl上左左門";
            // 
            // lbl上右右門
            // 
            this.lbl上右右門.AutoSize = true;
            this.lbl上右右門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上右右門.Location = new System.Drawing.Point(500, 60);
            this.lbl上右右門.Name = "lbl上右右門";
            this.lbl上右右門.Size = new System.Drawing.Size(71, 13);
            this.lbl上右右門.TabIndex = 209;
            this.lbl上右右門.Text = "lbl上右右門";
            // 
            // lbl上左右門
            // 
            this.lbl上左右門.AutoSize = true;
            this.lbl上左右門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl上左右門.Location = new System.Drawing.Point(500, 40);
            this.lbl上左右門.Name = "lbl上左右門";
            this.lbl上左右門.Size = new System.Drawing.Size(71, 13);
            this.lbl上左右門.TabIndex = 208;
            this.lbl上左右門.Text = "lbl上左右門";
            // 
            // lbl_NA_19
            // 
            this.lbl_NA_19.AutoSize = true;
            this.lbl_NA_19.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_19.Location = new System.Drawing.Point(420, 180);
            this.lbl_NA_19.Name = "lbl_NA_19";
            this.lbl_NA_19.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_19.TabIndex = 207;
            this.lbl_NA_19.Text = "lbl_NA_19";
            // 
            // lbl_NA_18
            // 
            this.lbl_NA_18.AutoSize = true;
            this.lbl_NA_18.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_18.Location = new System.Drawing.Point(420, 160);
            this.lbl_NA_18.Name = "lbl_NA_18";
            this.lbl_NA_18.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_18.TabIndex = 206;
            this.lbl_NA_18.Text = "lbl_NA_18";
            // 
            // lbl_NA_17
            // 
            this.lbl_NA_17.AutoSize = true;
            this.lbl_NA_17.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_17.Location = new System.Drawing.Point(420, 140);
            this.lbl_NA_17.Name = "lbl_NA_17";
            this.lbl_NA_17.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_17.TabIndex = 205;
            this.lbl_NA_17.Text = "lbl_NA_17";
            // 
            // lbl_NA_16
            // 
            this.lbl_NA_16.AutoSize = true;
            this.lbl_NA_16.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_16.Location = new System.Drawing.Point(420, 120);
            this.lbl_NA_16.Name = "lbl_NA_16";
            this.lbl_NA_16.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_16.TabIndex = 204;
            this.lbl_NA_16.Text = "lbl_NA_16";
            // 
            // lbl_NA_15
            // 
            this.lbl_NA_15.AutoSize = true;
            this.lbl_NA_15.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_15.Location = new System.Drawing.Point(420, 100);
            this.lbl_NA_15.Name = "lbl_NA_15";
            this.lbl_NA_15.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_15.TabIndex = 203;
            this.lbl_NA_15.Text = "lbl_NA_15";
            // 
            // lbl_擺放座關
            // 
            this.lbl_擺放座關.AutoSize = true;
            this.lbl_擺放座關.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_擺放座關.Location = new System.Drawing.Point(420, 80);
            this.lbl_擺放座關.Name = "lbl_擺放座關";
            this.lbl_擺放座關.Size = new System.Drawing.Size(77, 13);
            this.lbl_擺放座關.TabIndex = 202;
            this.lbl_擺放座關.Text = "lbl_擺放座關";
            // 
            // lbl_NA_13
            // 
            this.lbl_NA_13.AutoSize = true;
            this.lbl_NA_13.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_13.Location = new System.Drawing.Point(420, 60);
            this.lbl_NA_13.Name = "lbl_NA_13";
            this.lbl_NA_13.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_13.TabIndex = 201;
            this.lbl_NA_13.Text = "lbl_NA_13";
            // 
            // lbl_擺放座開
            // 
            this.lbl_擺放座開.AutoSize = true;
            this.lbl_擺放座開.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_擺放座開.Location = new System.Drawing.Point(420, 40);
            this.lbl_擺放座開.Name = "lbl_擺放座開";
            this.lbl_擺放座開.Size = new System.Drawing.Size(77, 13);
            this.lbl_擺放座開.TabIndex = 200;
            this.lbl_擺放座開.Text = "lbl_擺放座開";
            // 
            // lbl_NA_11
            // 
            this.lbl_NA_11.AutoSize = true;
            this.lbl_NA_11.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_11.Location = new System.Drawing.Point(340, 180);
            this.lbl_NA_11.Name = "lbl_NA_11";
            this.lbl_NA_11.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_11.TabIndex = 199;
            this.lbl_NA_11.Text = "lbl_NA_11";
            // 
            // lbl急停鈕
            // 
            this.lbl急停鈕.AutoSize = true;
            this.lbl急停鈕.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl急停鈕.Location = new System.Drawing.Point(340, 160);
            this.lbl急停鈕.Name = "lbl急停鈕";
            this.lbl急停鈕.Size = new System.Drawing.Size(58, 13);
            this.lbl急停鈕.TabIndex = 198;
            this.lbl急停鈕.Text = "lbl急停鈕";
            // 
            // lbl_NA_10
            // 
            this.lbl_NA_10.AutoSize = true;
            this.lbl_NA_10.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_10.Location = new System.Drawing.Point(340, 140);
            this.lbl_NA_10.Name = "lbl_NA_10";
            this.lbl_NA_10.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_10.TabIndex = 197;
            this.lbl_NA_10.Text = "lbl_NA_10";
            // 
            // lbl停止鈕
            // 
            this.lbl停止鈕.AutoSize = true;
            this.lbl停止鈕.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl停止鈕.Location = new System.Drawing.Point(340, 120);
            this.lbl停止鈕.Name = "lbl停止鈕";
            this.lbl停止鈕.Size = new System.Drawing.Size(58, 13);
            this.lbl停止鈕.TabIndex = 196;
            this.lbl停止鈕.Text = "lbl停止鈕";
            // 
            // lbl_NA_09
            // 
            this.lbl_NA_09.AutoSize = true;
            this.lbl_NA_09.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_09.Location = new System.Drawing.Point(340, 100);
            this.lbl_NA_09.Name = "lbl_NA_09";
            this.lbl_NA_09.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_09.TabIndex = 195;
            this.lbl_NA_09.Text = "lbl_NA_09";
            // 
            // lbl啟動鈕
            // 
            this.lbl啟動鈕.AutoSize = true;
            this.lbl啟動鈕.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl啟動鈕.Location = new System.Drawing.Point(340, 80);
            this.lbl啟動鈕.Name = "lbl啟動鈕";
            this.lbl啟動鈕.Size = new System.Drawing.Size(58, 13);
            this.lbl啟動鈕.TabIndex = 194;
            this.lbl啟動鈕.Text = "lbl啟動鈕";
            // 
            // lbl_NA_08
            // 
            this.lbl_NA_08.AutoSize = true;
            this.lbl_NA_08.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_08.Location = new System.Drawing.Point(340, 60);
            this.lbl_NA_08.Name = "lbl_NA_08";
            this.lbl_NA_08.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_08.TabIndex = 193;
            this.lbl_NA_08.Text = "lbl_NA_08";
            // 
            // lbl復歸鈕
            // 
            this.lbl復歸鈕.AutoSize = true;
            this.lbl復歸鈕.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl復歸鈕.Location = new System.Drawing.Point(340, 40);
            this.lbl復歸鈕.Name = "lbl復歸鈕";
            this.lbl復歸鈕.Size = new System.Drawing.Size(58, 13);
            this.lbl復歸鈕.TabIndex = 192;
            this.lbl復歸鈕.Text = "lbl復歸鈕";
            // 
            // lbl吸料盒
            // 
            this.lbl吸料盒.AutoSize = true;
            this.lbl吸料盒.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl吸料盒.Location = new System.Drawing.Point(260, 180);
            this.lbl吸料盒.Name = "lbl吸料盒";
            this.lbl吸料盒.Size = new System.Drawing.Size(58, 13);
            this.lbl吸料盒.TabIndex = 191;
            this.lbl吸料盒.Text = "lbl吸料盒";
            // 
            // lbl兩點壓2
            // 
            this.lbl兩點壓2.AutoSize = true;
            this.lbl兩點壓2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl兩點壓2.Location = new System.Drawing.Point(260, 160);
            this.lbl兩點壓2.Name = "lbl兩點壓2";
            this.lbl兩點壓2.Size = new System.Drawing.Size(64, 13);
            this.lbl兩點壓2.TabIndex = 190;
            this.lbl兩點壓2.Text = "lbl兩點壓2";
            // 
            // lbl堵料盒
            // 
            this.lbl堵料盒.AutoSize = true;
            this.lbl堵料盒.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl堵料盒.Location = new System.Drawing.Point(260, 140);
            this.lbl堵料盒.Name = "lbl堵料盒";
            this.lbl堵料盒.Size = new System.Drawing.Size(58, 13);
            this.lbl堵料盒.TabIndex = 189;
            this.lbl堵料盒.Text = "lbl堵料盒";
            // 
            // lbl兩點壓1
            // 
            this.lbl兩點壓1.AutoSize = true;
            this.lbl兩點壓1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl兩點壓1.Location = new System.Drawing.Point(260, 120);
            this.lbl兩點壓1.Name = "lbl兩點壓1";
            this.lbl兩點壓1.Size = new System.Drawing.Size(64, 13);
            this.lbl兩點壓1.TabIndex = 188;
            this.lbl兩點壓1.Text = "lbl兩點壓1";
            // 
            // lbl取料ng盒
            // 
            this.lbl取料ng盒.AutoSize = true;
            this.lbl取料ng盒.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料ng盒.Location = new System.Drawing.Point(260, 100);
            this.lbl取料ng盒.Name = "lbl取料ng盒";
            this.lbl取料ng盒.Size = new System.Drawing.Size(70, 13);
            this.lbl取料ng盒.TabIndex = 187;
            this.lbl取料ng盒.Text = "lbl取料ng盒";
            // 
            // lbl吸嘴空2
            // 
            this.lbl吸嘴空2.AutoSize = true;
            this.lbl吸嘴空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl吸嘴空2.Location = new System.Drawing.Point(260, 80);
            this.lbl吸嘴空2.Name = "lbl吸嘴空2";
            this.lbl吸嘴空2.Size = new System.Drawing.Size(64, 13);
            this.lbl吸嘴空2.TabIndex = 186;
            this.lbl吸嘴空2.Text = "lbl吸嘴空2";
            // 
            // lbl_NA_07
            // 
            this.lbl_NA_07.AutoSize = true;
            this.lbl_NA_07.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_07.Location = new System.Drawing.Point(260, 60);
            this.lbl_NA_07.Name = "lbl_NA_07";
            this.lbl_NA_07.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_07.TabIndex = 185;
            this.lbl_NA_07.Text = "lbl_NA_07";
            // 
            // lbl吸嘴空1
            // 
            this.lbl吸嘴空1.AutoSize = true;
            this.lbl吸嘴空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl吸嘴空1.Location = new System.Drawing.Point(260, 40);
            this.lbl吸嘴空1.Name = "lbl吸嘴空1";
            this.lbl吸嘴空1.Size = new System.Drawing.Size(64, 13);
            this.lbl吸嘴空1.TabIndex = 184;
            this.lbl吸嘴空1.Text = "lbl吸嘴空1";
            // 
            // lbl擺放空2
            // 
            this.lbl擺放空2.AutoSize = true;
            this.lbl擺放空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl擺放空2.Location = new System.Drawing.Point(180, 180);
            this.lbl擺放空2.Name = "lbl擺放空2";
            this.lbl擺放空2.Size = new System.Drawing.Size(64, 13);
            this.lbl擺放空2.TabIndex = 183;
            this.lbl擺放空2.Text = "lbl擺放空2";
            // 
            // lblsk1空2
            // 
            this.lblsk1空2.AutoSize = true;
            this.lblsk1空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk1空2.Location = new System.Drawing.Point(180, 160);
            this.lblsk1空2.Name = "lblsk1空2";
            this.lblsk1空2.Size = new System.Drawing.Size(55, 13);
            this.lblsk1空2.TabIndex = 182;
            this.lblsk1空2.Text = "lblsk1空2";
            // 
            // lbl擺放空1
            // 
            this.lbl擺放空1.AutoSize = true;
            this.lbl擺放空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl擺放空1.Location = new System.Drawing.Point(180, 140);
            this.lbl擺放空1.Name = "lbl擺放空1";
            this.lbl擺放空1.Size = new System.Drawing.Size(64, 13);
            this.lbl擺放空1.TabIndex = 181;
            this.lbl擺放空1.Text = "lbl擺放空1";
            // 
            // lblsk1空1
            // 
            this.lblsk1空1.AutoSize = true;
            this.lblsk1空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk1空1.Location = new System.Drawing.Point(180, 120);
            this.lblsk1空1.Name = "lblsk1空1";
            this.lblsk1空1.Size = new System.Drawing.Size(55, 13);
            this.lblsk1空1.TabIndex = 180;
            this.lblsk1空1.Text = "lblsk1空1";
            // 
            // lblsk2空2
            // 
            this.lblsk2空2.AutoSize = true;
            this.lblsk2空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk2空2.Location = new System.Drawing.Point(180, 100);
            this.lblsk2空2.Name = "lblsk2空2";
            this.lblsk2空2.Size = new System.Drawing.Size(55, 13);
            this.lblsk2空2.TabIndex = 179;
            this.lblsk2空2.Text = "lblsk2空2";
            // 
            // lbl載盤空2
            // 
            this.lbl載盤空2.AutoSize = true;
            this.lbl載盤空2.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤空2.Location = new System.Drawing.Point(180, 80);
            this.lbl載盤空2.Name = "lbl載盤空2";
            this.lbl載盤空2.Size = new System.Drawing.Size(64, 13);
            this.lbl載盤空2.TabIndex = 178;
            this.lbl載盤空2.Text = "lbl載盤空2";
            // 
            // lblsk2空1
            // 
            this.lblsk2空1.AutoSize = true;
            this.lblsk2空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblsk2空1.Location = new System.Drawing.Point(180, 60);
            this.lblsk2空1.Name = "lblsk2空1";
            this.lblsk2空1.Size = new System.Drawing.Size(55, 13);
            this.lblsk2空1.TabIndex = 177;
            this.lblsk2空1.Text = "lblsk2空1";
            // 
            // lbl載盤空1
            // 
            this.lbl載盤空1.AutoSize = true;
            this.lbl載盤空1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤空1.Location = new System.Drawing.Point(180, 40);
            this.lbl載盤空1.Name = "lbl載盤空1";
            this.lbl載盤空1.Size = new System.Drawing.Size(64, 13);
            this.lbl載盤空1.TabIndex = 176;
            this.lbl載盤空1.Text = "lbl載盤空1";
            // 
            // lbl_NA_06
            // 
            this.lbl_NA_06.AutoSize = true;
            this.lbl_NA_06.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_06.Location = new System.Drawing.Point(100, 180);
            this.lbl_NA_06.Name = "lbl_NA_06";
            this.lbl_NA_06.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_06.TabIndex = 175;
            this.lbl_NA_06.Text = "lbl_NA_06";
            // 
            // lbl載盤X後
            // 
            this.lbl載盤X後.AutoSize = true;
            this.lbl載盤X後.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤X後.Location = new System.Drawing.Point(100, 160);
            this.lbl載盤X後.Name = "lbl載盤X後";
            this.lbl載盤X後.Size = new System.Drawing.Size(67, 13);
            this.lbl載盤X後.TabIndex = 174;
            this.lbl載盤X後.Text = "lbl載盤X後";
            // 
            // lbl_NA_05
            // 
            this.lbl_NA_05.AutoSize = true;
            this.lbl_NA_05.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_05.Location = new System.Drawing.Point(100, 140);
            this.lbl_NA_05.Name = "lbl_NA_05";
            this.lbl_NA_05.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_05.TabIndex = 173;
            this.lbl_NA_05.Text = "lbl_NA_05";
            // 
            // lbl載盤X前
            // 
            this.lbl載盤X前.AutoSize = true;
            this.lbl載盤X前.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤X前.Location = new System.Drawing.Point(100, 120);
            this.lbl載盤X前.Name = "lbl載盤X前";
            this.lbl載盤X前.Size = new System.Drawing.Size(67, 13);
            this.lbl載盤X前.TabIndex = 172;
            this.lbl載盤X前.Text = "lbl載盤X前";
            // 
            // lbl_NA_04
            // 
            this.lbl_NA_04.AutoSize = true;
            this.lbl_NA_04.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_04.Location = new System.Drawing.Point(100, 100);
            this.lbl_NA_04.Name = "lbl_NA_04";
            this.lbl_NA_04.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_04.TabIndex = 171;
            this.lbl_NA_04.Text = "lbl_NA_04";
            // 
            // lbl植針Z前
            // 
            this.lbl植針Z前.AutoSize = true;
            this.lbl植針Z前.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl植針Z前.Location = new System.Drawing.Point(100, 80);
            this.lbl植針Z前.Name = "lbl植針Z前";
            this.lbl植針Z前.Size = new System.Drawing.Size(65, 13);
            this.lbl植針Z前.TabIndex = 170;
            this.lbl植針Z前.Text = "lbl植針Z前";
            // 
            // lbl_NA_03
            // 
            this.lbl_NA_03.AutoSize = true;
            this.lbl_NA_03.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_03.Location = new System.Drawing.Point(100, 60);
            this.lbl_NA_03.Name = "lbl_NA_03";
            this.lbl_NA_03.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_03.TabIndex = 169;
            this.lbl_NA_03.Text = "lbl_NA_03";
            // 
            // lbl植針Z後
            // 
            this.lbl植針Z後.AutoSize = true;
            this.lbl植針Z後.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl植針Z後.Location = new System.Drawing.Point(100, 40);
            this.lbl植針Z後.Name = "lbl植針Z後";
            this.lbl植針Z後.Size = new System.Drawing.Size(65, 13);
            this.lbl植針Z後.TabIndex = 168;
            this.lbl植針Z後.Text = "lbl植針Z後";
            // 
            // lbl_NA_02
            // 
            this.lbl_NA_02.AutoSize = true;
            this.lbl_NA_02.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_02.Location = new System.Drawing.Point(20, 180);
            this.lbl_NA_02.Name = "lbl_NA_02";
            this.lbl_NA_02.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_02.TabIndex = 167;
            this.lbl_NA_02.Text = "lbl_NA_02";
            // 
            // lbl取料X前
            // 
            this.lbl取料X前.AutoSize = true;
            this.lbl取料X前.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料X前.Location = new System.Drawing.Point(20, 160);
            this.lbl取料X前.Name = "lbl取料X前";
            this.lbl取料X前.Size = new System.Drawing.Size(67, 13);
            this.lbl取料X前.TabIndex = 166;
            this.lbl取料X前.Text = "lbl取料X前";
            // 
            // lbl_NA_01
            // 
            this.lbl_NA_01.AutoSize = true;
            this.lbl_NA_01.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_NA_01.Location = new System.Drawing.Point(20, 140);
            this.lbl_NA_01.Name = "lbl_NA_01";
            this.lbl_NA_01.Size = new System.Drawing.Size(61, 13);
            this.lbl_NA_01.TabIndex = 165;
            this.lbl_NA_01.Text = "lbl_NA_01";
            // 
            // lbl取料X後
            // 
            this.lbl取料X後.AutoSize = true;
            this.lbl取料X後.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料X後.Location = new System.Drawing.Point(20, 120);
            this.lbl取料X後.Name = "lbl取料X後";
            this.lbl取料X後.Size = new System.Drawing.Size(67, 13);
            this.lbl取料X後.TabIndex = 164;
            this.lbl取料X後.Text = "lbl取料X後";
            // 
            // lbl取料Y前
            // 
            this.lbl取料Y前.AutoSize = true;
            this.lbl取料Y前.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料Y前.Location = new System.Drawing.Point(20, 100);
            this.lbl取料Y前.Name = "lbl取料Y前";
            this.lbl取料Y前.Size = new System.Drawing.Size(67, 13);
            this.lbl取料Y前.TabIndex = 163;
            this.lbl取料Y前.Text = "lbl取料Y前";
            // 
            // lbl載盤Y前
            // 
            this.lbl載盤Y前.AutoSize = true;
            this.lbl載盤Y前.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤Y前.Location = new System.Drawing.Point(20, 80);
            this.lbl載盤Y前.Name = "lbl載盤Y前";
            this.lbl載盤Y前.Size = new System.Drawing.Size(67, 13);
            this.lbl載盤Y前.TabIndex = 162;
            this.lbl載盤Y前.Text = "lbl載盤Y前";
            // 
            // lbl取料Y後
            // 
            this.lbl取料Y後.AutoSize = true;
            this.lbl取料Y後.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl取料Y後.Location = new System.Drawing.Point(20, 60);
            this.lbl取料Y後.Name = "lbl取料Y後";
            this.lbl取料Y後.Size = new System.Drawing.Size(67, 13);
            this.lbl取料Y後.TabIndex = 161;
            this.lbl取料Y後.Text = "lbl取料Y後";
            // 
            // lbl載盤Y後
            // 
            this.lbl載盤Y後.AutoSize = true;
            this.lbl載盤Y後.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl載盤Y後.Location = new System.Drawing.Point(20, 40);
            this.lbl載盤Y後.Name = "lbl載盤Y後";
            this.lbl載盤Y後.Size = new System.Drawing.Size(67, 13);
            this.lbl載盤Y後.TabIndex = 160;
            this.lbl載盤Y後.Text = "lbl載盤Y後";
            // 
            // lbl_工作門_Convert
            // 
            this.lbl_工作門_Convert.AutoSize = true;
            this.lbl_工作門_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_工作門_Convert.Location = new System.Drawing.Point(590, 185);
            this.lbl_工作門_Convert.Name = "lbl_工作門_Convert";
            this.lbl_工作門_Convert.Size = new System.Drawing.Size(108, 13);
            this.lbl_工作門_Convert.TabIndex = 95;
            this.lbl_工作門_Convert.Text = "lbl_工作門_Convert";
            // 
            // lbl_植針R軸_Convert
            // 
            this.lbl_植針R軸_Convert.AutoSize = true;
            this.lbl_植針R軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針R軸_Convert.Location = new System.Drawing.Point(590, 165);
            this.lbl_植針R軸_Convert.Name = "lbl_植針R軸_Convert";
            this.lbl_植針R軸_Convert.Size = new System.Drawing.Size(116, 13);
            this.lbl_植針R軸_Convert.TabIndex = 94;
            this.lbl_植針R軸_Convert.Text = "lbl_植針R軸_Convert";
            // 
            // lbl_植針Z軸_Convert
            // 
            this.lbl_植針Z軸_Convert.AutoSize = true;
            this.lbl_植針Z軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針Z軸_Convert.Location = new System.Drawing.Point(590, 145);
            this.lbl_植針Z軸_Convert.Name = "lbl_植針Z軸_Convert";
            this.lbl_植針Z軸_Convert.Size = new System.Drawing.Size(115, 13);
            this.lbl_植針Z軸_Convert.TabIndex = 93;
            this.lbl_植針Z軸_Convert.Text = "lbl_植針Z軸_Convert";
            // 
            // lbl_載盤Y軸_Convert
            // 
            this.lbl_載盤Y軸_Convert.AutoSize = true;
            this.lbl_載盤Y軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_載盤Y軸_Convert.Location = new System.Drawing.Point(590, 125);
            this.lbl_載盤Y軸_Convert.Name = "lbl_載盤Y軸_Convert";
            this.lbl_載盤Y軸_Convert.Size = new System.Drawing.Size(117, 13);
            this.lbl_載盤Y軸_Convert.TabIndex = 92;
            this.lbl_載盤Y軸_Convert.Text = "lbl_載盤Y軸_Convert";
            // 
            // lbl_載盤X軸_Convert
            // 
            this.lbl_載盤X軸_Convert.AutoSize = true;
            this.lbl_載盤X軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_載盤X軸_Convert.Location = new System.Drawing.Point(590, 105);
            this.lbl_載盤X軸_Convert.Name = "lbl_載盤X軸_Convert";
            this.lbl_載盤X軸_Convert.Size = new System.Drawing.Size(117, 13);
            this.lbl_載盤X軸_Convert.TabIndex = 91;
            this.lbl_載盤X軸_Convert.Text = "lbl_載盤X軸_Convert";
            // 
            // lbl_吸嘴R軸_Convert
            // 
            this.lbl_吸嘴R軸_Convert.AutoSize = true;
            this.lbl_吸嘴R軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴R軸_Convert.Location = new System.Drawing.Point(590, 85);
            this.lbl_吸嘴R軸_Convert.Name = "lbl_吸嘴R軸_Convert";
            this.lbl_吸嘴R軸_Convert.Size = new System.Drawing.Size(116, 13);
            this.lbl_吸嘴R軸_Convert.TabIndex = 90;
            this.lbl_吸嘴R軸_Convert.Text = "lbl_吸嘴R軸_Convert";
            // 
            // lbl_吸嘴Z軸_Convert
            // 
            this.lbl_吸嘴Z軸_Convert.AutoSize = true;
            this.lbl_吸嘴Z軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴Z軸_Convert.Location = new System.Drawing.Point(590, 65);
            this.lbl_吸嘴Z軸_Convert.Name = "lbl_吸嘴Z軸_Convert";
            this.lbl_吸嘴Z軸_Convert.Size = new System.Drawing.Size(115, 13);
            this.lbl_吸嘴Z軸_Convert.TabIndex = 89;
            this.lbl_吸嘴Z軸_Convert.Text = "lbl_吸嘴Z軸_Convert";
            // 
            // lbl_吸嘴Y軸_Convert
            // 
            this.lbl_吸嘴Y軸_Convert.AutoSize = true;
            this.lbl_吸嘴Y軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴Y軸_Convert.Location = new System.Drawing.Point(590, 45);
            this.lbl_吸嘴Y軸_Convert.Name = "lbl_吸嘴Y軸_Convert";
            this.lbl_吸嘴Y軸_Convert.Size = new System.Drawing.Size(117, 13);
            this.lbl_吸嘴Y軸_Convert.TabIndex = 88;
            this.lbl_吸嘴Y軸_Convert.Text = "lbl_吸嘴Y軸_Convert";
            // 
            // lbl_吸嘴X軸_Convert
            // 
            this.lbl_吸嘴X軸_Convert.AutoSize = true;
            this.lbl_吸嘴X軸_Convert.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴X軸_Convert.Location = new System.Drawing.Point(590, 25);
            this.lbl_吸嘴X軸_Convert.Name = "lbl_吸嘴X軸_Convert";
            this.lbl_吸嘴X軸_Convert.Size = new System.Drawing.Size(117, 13);
            this.lbl_吸嘴X軸_Convert.TabIndex = 87;
            this.lbl_吸嘴X軸_Convert.Text = "lbl_吸嘴X軸_Convert";
            // 
            // lbl_工作門_Back
            // 
            this.lbl_工作門_Back.AutoSize = true;
            this.lbl_工作門_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_工作門_Back.Location = new System.Drawing.Point(770, 185);
            this.lbl_工作門_Back.Name = "lbl_工作門_Back";
            this.lbl_工作門_Back.Size = new System.Drawing.Size(94, 13);
            this.lbl_工作門_Back.TabIndex = 86;
            this.lbl_工作門_Back.Text = "lbl_工作門_Back";
            // 
            // lbl_工作門_RAW
            // 
            this.lbl_工作門_RAW.AutoSize = true;
            this.lbl_工作門_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_工作門_RAW.Location = new System.Drawing.Point(420, 185);
            this.lbl_工作門_RAW.Name = "lbl_工作門_RAW";
            this.lbl_工作門_RAW.Size = new System.Drawing.Size(99, 13);
            this.lbl_工作門_RAW.TabIndex = 85;
            this.lbl_工作門_RAW.Text = "lbl_工作門_RAW";
            // 
            // lbl_植針R軸_Back
            // 
            this.lbl_植針R軸_Back.AutoSize = true;
            this.lbl_植針R軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針R軸_Back.Location = new System.Drawing.Point(770, 165);
            this.lbl_植針R軸_Back.Name = "lbl_植針R軸_Back";
            this.lbl_植針R軸_Back.Size = new System.Drawing.Size(102, 13);
            this.lbl_植針R軸_Back.TabIndex = 83;
            this.lbl_植針R軸_Back.Text = "lbl_植針R軸_Back";
            // 
            // lbl_植針R軸_RAW
            // 
            this.lbl_植針R軸_RAW.AutoSize = true;
            this.lbl_植針R軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針R軸_RAW.Location = new System.Drawing.Point(420, 165);
            this.lbl_植針R軸_RAW.Name = "lbl_植針R軸_RAW";
            this.lbl_植針R軸_RAW.Size = new System.Drawing.Size(107, 13);
            this.lbl_植針R軸_RAW.TabIndex = 82;
            this.lbl_植針R軸_RAW.Text = "lbl_植針R軸_RAW";
            // 
            // lbl_植針Z軸_Back
            // 
            this.lbl_植針Z軸_Back.AutoSize = true;
            this.lbl_植針Z軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針Z軸_Back.Location = new System.Drawing.Point(770, 145);
            this.lbl_植針Z軸_Back.Name = "lbl_植針Z軸_Back";
            this.lbl_植針Z軸_Back.Size = new System.Drawing.Size(101, 13);
            this.lbl_植針Z軸_Back.TabIndex = 80;
            this.lbl_植針Z軸_Back.Text = "lbl_植針Z軸_Back";
            // 
            // lbl_植針Z軸_RAW
            // 
            this.lbl_植針Z軸_RAW.AutoSize = true;
            this.lbl_植針Z軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_植針Z軸_RAW.Location = new System.Drawing.Point(420, 145);
            this.lbl_植針Z軸_RAW.Name = "lbl_植針Z軸_RAW";
            this.lbl_植針Z軸_RAW.Size = new System.Drawing.Size(106, 13);
            this.lbl_植針Z軸_RAW.TabIndex = 79;
            this.lbl_植針Z軸_RAW.Text = "lbl_植針Z軸_RAW";
            // 
            // lbl_載盤Y軸_Back
            // 
            this.lbl_載盤Y軸_Back.AutoSize = true;
            this.lbl_載盤Y軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_載盤Y軸_Back.Location = new System.Drawing.Point(770, 125);
            this.lbl_載盤Y軸_Back.Name = "lbl_載盤Y軸_Back";
            this.lbl_載盤Y軸_Back.Size = new System.Drawing.Size(103, 13);
            this.lbl_載盤Y軸_Back.TabIndex = 77;
            this.lbl_載盤Y軸_Back.Text = "lbl_載盤Y軸_Back";
            // 
            // lbl_載盤Y軸_RAW
            // 
            this.lbl_載盤Y軸_RAW.AutoSize = true;
            this.lbl_載盤Y軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_載盤Y軸_RAW.Location = new System.Drawing.Point(420, 125);
            this.lbl_載盤Y軸_RAW.Name = "lbl_載盤Y軸_RAW";
            this.lbl_載盤Y軸_RAW.Size = new System.Drawing.Size(108, 13);
            this.lbl_載盤Y軸_RAW.TabIndex = 76;
            this.lbl_載盤Y軸_RAW.Text = "lbl_載盤Y軸_RAW";
            // 
            // lbl_載盤X軸_Back
            // 
            this.lbl_載盤X軸_Back.AutoSize = true;
            this.lbl_載盤X軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_載盤X軸_Back.Location = new System.Drawing.Point(770, 105);
            this.lbl_載盤X軸_Back.Name = "lbl_載盤X軸_Back";
            this.lbl_載盤X軸_Back.Size = new System.Drawing.Size(103, 13);
            this.lbl_載盤X軸_Back.TabIndex = 74;
            this.lbl_載盤X軸_Back.Text = "lbl_載盤X軸_Back";
            // 
            // lbl_載盤X軸_RAW
            // 
            this.lbl_載盤X軸_RAW.AutoSize = true;
            this.lbl_載盤X軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_載盤X軸_RAW.Location = new System.Drawing.Point(420, 105);
            this.lbl_載盤X軸_RAW.Name = "lbl_載盤X軸_RAW";
            this.lbl_載盤X軸_RAW.Size = new System.Drawing.Size(108, 13);
            this.lbl_載盤X軸_RAW.TabIndex = 73;
            this.lbl_載盤X軸_RAW.Text = "lbl_載盤X軸_RAW";
            // 
            // lbl_吸嘴R軸_Back
            // 
            this.lbl_吸嘴R軸_Back.AutoSize = true;
            this.lbl_吸嘴R軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴R軸_Back.Location = new System.Drawing.Point(770, 85);
            this.lbl_吸嘴R軸_Back.Name = "lbl_吸嘴R軸_Back";
            this.lbl_吸嘴R軸_Back.Size = new System.Drawing.Size(102, 13);
            this.lbl_吸嘴R軸_Back.TabIndex = 71;
            this.lbl_吸嘴R軸_Back.Text = "lbl_吸嘴R軸_Back";
            // 
            // lbl_吸嘴R軸_RAW
            // 
            this.lbl_吸嘴R軸_RAW.AutoSize = true;
            this.lbl_吸嘴R軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴R軸_RAW.Location = new System.Drawing.Point(420, 85);
            this.lbl_吸嘴R軸_RAW.Name = "lbl_吸嘴R軸_RAW";
            this.lbl_吸嘴R軸_RAW.Size = new System.Drawing.Size(107, 13);
            this.lbl_吸嘴R軸_RAW.TabIndex = 70;
            this.lbl_吸嘴R軸_RAW.Text = "lbl_吸嘴R軸_RAW";
            // 
            // lbl_吸嘴Z軸_Back
            // 
            this.lbl_吸嘴Z軸_Back.AutoSize = true;
            this.lbl_吸嘴Z軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴Z軸_Back.Location = new System.Drawing.Point(770, 65);
            this.lbl_吸嘴Z軸_Back.Name = "lbl_吸嘴Z軸_Back";
            this.lbl_吸嘴Z軸_Back.Size = new System.Drawing.Size(101, 13);
            this.lbl_吸嘴Z軸_Back.TabIndex = 68;
            this.lbl_吸嘴Z軸_Back.Text = "lbl_吸嘴Z軸_Back";
            // 
            // lbl_吸嘴Z軸_RAW
            // 
            this.lbl_吸嘴Z軸_RAW.AutoSize = true;
            this.lbl_吸嘴Z軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴Z軸_RAW.Location = new System.Drawing.Point(420, 65);
            this.lbl_吸嘴Z軸_RAW.Name = "lbl_吸嘴Z軸_RAW";
            this.lbl_吸嘴Z軸_RAW.Size = new System.Drawing.Size(106, 13);
            this.lbl_吸嘴Z軸_RAW.TabIndex = 67;
            this.lbl_吸嘴Z軸_RAW.Text = "lbl_吸嘴Z軸_RAW";
            // 
            // lbl_吸嘴Y軸_Back
            // 
            this.lbl_吸嘴Y軸_Back.AutoSize = true;
            this.lbl_吸嘴Y軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴Y軸_Back.Location = new System.Drawing.Point(770, 45);
            this.lbl_吸嘴Y軸_Back.Name = "lbl_吸嘴Y軸_Back";
            this.lbl_吸嘴Y軸_Back.Size = new System.Drawing.Size(103, 13);
            this.lbl_吸嘴Y軸_Back.TabIndex = 65;
            this.lbl_吸嘴Y軸_Back.Text = "lbl_吸嘴Y軸_Back";
            // 
            // lbl_吸嘴Y軸_RAW
            // 
            this.lbl_吸嘴Y軸_RAW.AutoSize = true;
            this.lbl_吸嘴Y軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴Y軸_RAW.Location = new System.Drawing.Point(420, 45);
            this.lbl_吸嘴Y軸_RAW.Name = "lbl_吸嘴Y軸_RAW";
            this.lbl_吸嘴Y軸_RAW.Size = new System.Drawing.Size(108, 13);
            this.lbl_吸嘴Y軸_RAW.TabIndex = 64;
            this.lbl_吸嘴Y軸_RAW.Text = "lbl_吸嘴Y軸_RAW";
            // 
            // lbl_吸嘴X軸_Back
            // 
            this.lbl_吸嘴X軸_Back.AutoSize = true;
            this.lbl_吸嘴X軸_Back.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴X軸_Back.Location = new System.Drawing.Point(770, 25);
            this.lbl_吸嘴X軸_Back.Name = "lbl_吸嘴X軸_Back";
            this.lbl_吸嘴X軸_Back.Size = new System.Drawing.Size(103, 13);
            this.lbl_吸嘴X軸_Back.TabIndex = 62;
            this.lbl_吸嘴X軸_Back.Text = "lbl_吸嘴X軸_Back";
            // 
            // lbl_吸嘴X軸_RAW
            // 
            this.lbl_吸嘴X軸_RAW.AutoSize = true;
            this.lbl_吸嘴X軸_RAW.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_吸嘴X軸_RAW.Location = new System.Drawing.Point(420, 25);
            this.lbl_吸嘴X軸_RAW.Name = "lbl_吸嘴X軸_RAW";
            this.lbl_吸嘴X軸_RAW.Size = new System.Drawing.Size(108, 13);
            this.lbl_吸嘴X軸_RAW.TabIndex = 61;
            this.lbl_吸嘴X軸_RAW.Text = "lbl_吸嘴X軸_RAW";
            // 
            // btn_minus_10
            // 
            this.btn_minus_10.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_minus_10.Location = new System.Drawing.Point(250, 419);
            this.btn_minus_10.Name = "btn_minus_10";
            this.btn_minus_10.Size = new System.Drawing.Size(75, 23);
            this.btn_minus_10.TabIndex = 59;
            this.btn_minus_10.Text = "-10";
            this.btn_minus_10.UseVisualStyleBackColor = true;
            this.btn_minus_10.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_minus_1
            // 
            this.btn_minus_1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_minus_1.Location = new System.Drawing.Point(250, 389);
            this.btn_minus_1.Name = "btn_minus_1";
            this.btn_minus_1.Size = new System.Drawing.Size(75, 23);
            this.btn_minus_1.TabIndex = 58;
            this.btn_minus_1.Text = "-1";
            this.btn_minus_1.UseVisualStyleBackColor = true;
            this.btn_minus_1.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_plus_10
            // 
            this.btn_plus_10.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_plus_10.Location = new System.Drawing.Point(155, 419);
            this.btn_plus_10.Name = "btn_plus_10";
            this.btn_plus_10.Size = new System.Drawing.Size(75, 23);
            this.btn_plus_10.TabIndex = 57;
            this.btn_plus_10.Text = "+10";
            this.btn_plus_10.UseVisualStyleBackColor = true;
            this.btn_plus_10.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btn_plus_1
            // 
            this.btn_plus_1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_plus_1.Location = new System.Drawing.Point(155, 389);
            this.btn_plus_1.Name = "btn_plus_1";
            this.btn_plus_1.Size = new System.Drawing.Size(75, 23);
            this.btn_plus_1.TabIndex = 56;
            this.btn_plus_1.Text = "+1";
            this.btn_plus_1.UseVisualStyleBackColor = true;
            this.btn_plus_1.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // btnABSMove
            // 
            this.btnABSMove.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnABSMove.Location = new System.Drawing.Point(25, 332);
            this.btnABSMove.Name = "btnABSMove";
            this.btnABSMove.Size = new System.Drawing.Size(111, 23);
            this.btnABSMove.TabIndex = 55;
            this.btnABSMove.Text = "移動至指定位置";
            this.btnABSMove.UseVisualStyleBackColor = true;
            this.btnABSMove.Click += new System.EventHandler(this.btn_adjust_JOG);
            // 
            // txtABSpos
            // 
            this.txtABSpos.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtABSpos.Location = new System.Drawing.Point(25, 302);
            this.txtABSpos.Name = "txtABSpos";
            this.txtABSpos.Size = new System.Drawing.Size(111, 23);
            this.txtABSpos.TabIndex = 54;
            this.txtABSpos.Text = "0.0";
            // 
            // en_工作門
            // 
            this.en_工作門.AutoSize = true;
            this.en_工作門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_工作門.Location = new System.Drawing.Point(25, 185);
            this.en_工作門.Name = "en_工作門";
            this.en_工作門.Size = new System.Drawing.Size(58, 17);
            this.en_工作門.TabIndex = 53;
            this.en_工作門.Text = "Enable";
            this.en_工作門.UseVisualStyleBackColor = true;
            this.en_工作門.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_植針R軸
            // 
            this.en_植針R軸.AutoSize = true;
            this.en_植針R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_植針R軸.Location = new System.Drawing.Point(25, 165);
            this.en_植針R軸.Name = "en_植針R軸";
            this.en_植針R軸.Size = new System.Drawing.Size(58, 17);
            this.en_植針R軸.TabIndex = 52;
            this.en_植針R軸.Text = "Enable";
            this.en_植針R軸.UseVisualStyleBackColor = true;
            this.en_植針R軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_植針Z軸
            // 
            this.en_植針Z軸.AutoSize = true;
            this.en_植針Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_植針Z軸.Location = new System.Drawing.Point(25, 145);
            this.en_植針Z軸.Name = "en_植針Z軸";
            this.en_植針Z軸.Size = new System.Drawing.Size(58, 17);
            this.en_植針Z軸.TabIndex = 51;
            this.en_植針Z軸.Text = "Enable";
            this.en_植針Z軸.UseVisualStyleBackColor = true;
            this.en_植針Z軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_載盤Y軸
            // 
            this.en_載盤Y軸.AutoSize = true;
            this.en_載盤Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_載盤Y軸.Location = new System.Drawing.Point(25, 125);
            this.en_載盤Y軸.Name = "en_載盤Y軸";
            this.en_載盤Y軸.Size = new System.Drawing.Size(58, 17);
            this.en_載盤Y軸.TabIndex = 50;
            this.en_載盤Y軸.Text = "Enable";
            this.en_載盤Y軸.UseVisualStyleBackColor = true;
            this.en_載盤Y軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_載盤X軸
            // 
            this.en_載盤X軸.AutoSize = true;
            this.en_載盤X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_載盤X軸.Location = new System.Drawing.Point(25, 105);
            this.en_載盤X軸.Name = "en_載盤X軸";
            this.en_載盤X軸.Size = new System.Drawing.Size(58, 17);
            this.en_載盤X軸.TabIndex = 49;
            this.en_載盤X軸.Text = "Enable";
            this.en_載盤X軸.UseVisualStyleBackColor = true;
            this.en_載盤X軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_吸嘴R軸
            // 
            this.en_吸嘴R軸.AutoSize = true;
            this.en_吸嘴R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_吸嘴R軸.Location = new System.Drawing.Point(25, 85);
            this.en_吸嘴R軸.Name = "en_吸嘴R軸";
            this.en_吸嘴R軸.Size = new System.Drawing.Size(58, 17);
            this.en_吸嘴R軸.TabIndex = 48;
            this.en_吸嘴R軸.Text = "Enable";
            this.en_吸嘴R軸.UseVisualStyleBackColor = true;
            this.en_吸嘴R軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_吸嘴Z軸
            // 
            this.en_吸嘴Z軸.AutoSize = true;
            this.en_吸嘴Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_吸嘴Z軸.Location = new System.Drawing.Point(25, 65);
            this.en_吸嘴Z軸.Name = "en_吸嘴Z軸";
            this.en_吸嘴Z軸.Size = new System.Drawing.Size(58, 17);
            this.en_吸嘴Z軸.TabIndex = 47;
            this.en_吸嘴Z軸.Text = "Enable";
            this.en_吸嘴Z軸.UseVisualStyleBackColor = true;
            this.en_吸嘴Z軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_吸嘴Y軸
            // 
            this.en_吸嘴Y軸.AutoSize = true;
            this.en_吸嘴Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_吸嘴Y軸.Location = new System.Drawing.Point(25, 45);
            this.en_吸嘴Y軸.Name = "en_吸嘴Y軸";
            this.en_吸嘴Y軸.Size = new System.Drawing.Size(58, 17);
            this.en_吸嘴Y軸.TabIndex = 46;
            this.en_吸嘴Y軸.Text = "Enable";
            this.en_吸嘴Y軸.UseVisualStyleBackColor = true;
            this.en_吸嘴Y軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            // 
            // en_吸嘴X軸
            // 
            this.en_吸嘴X軸.AutoSize = true;
            this.en_吸嘴X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.en_吸嘴X軸.Location = new System.Drawing.Point(25, 25);
            this.en_吸嘴X軸.Name = "en_吸嘴X軸";
            this.en_吸嘴X軸.Size = new System.Drawing.Size(58, 17);
            this.en_吸嘴X軸.TabIndex = 45;
            this.en_吸嘴X軸.Text = "Enable";
            this.en_吸嘴X軸.UseVisualStyleBackColor = true;
            this.en_吸嘴X軸.CheckedChanged += new System.EventHandler(this.en_Group_Click);
            this.en_吸嘴X軸.Click += new System.EventHandler(this.en_Group_Click);
            // 
            // lbl_acpos_工作門
            // 
            this.lbl_acpos_工作門.AutoSize = true;
            this.lbl_acpos_工作門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_工作門.Location = new System.Drawing.Point(340, 185);
            this.lbl_acpos_工作門.Name = "lbl_acpos_工作門";
            this.lbl_acpos_工作門.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_工作門.TabIndex = 44;
            this.lbl_acpos_工作門.Text = "0.00";
            // 
            // lbl_acpos_植針R軸
            // 
            this.lbl_acpos_植針R軸.AutoSize = true;
            this.lbl_acpos_植針R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_植針R軸.Location = new System.Drawing.Point(340, 165);
            this.lbl_acpos_植針R軸.Name = "lbl_acpos_植針R軸";
            this.lbl_acpos_植針R軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_植針R軸.TabIndex = 43;
            this.lbl_acpos_植針R軸.Text = "0.00";
            // 
            // lbl_acpos_植針Z軸
            // 
            this.lbl_acpos_植針Z軸.AutoSize = true;
            this.lbl_acpos_植針Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_植針Z軸.Location = new System.Drawing.Point(340, 145);
            this.lbl_acpos_植針Z軸.Name = "lbl_acpos_植針Z軸";
            this.lbl_acpos_植針Z軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_植針Z軸.TabIndex = 42;
            this.lbl_acpos_植針Z軸.Text = "0.00";
            // 
            // lbl_acpos_載盤Y軸
            // 
            this.lbl_acpos_載盤Y軸.AutoSize = true;
            this.lbl_acpos_載盤Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_載盤Y軸.Location = new System.Drawing.Point(340, 125);
            this.lbl_acpos_載盤Y軸.Name = "lbl_acpos_載盤Y軸";
            this.lbl_acpos_載盤Y軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_載盤Y軸.TabIndex = 41;
            this.lbl_acpos_載盤Y軸.Text = "0.00";
            // 
            // lbl_acpos_載盤X軸
            // 
            this.lbl_acpos_載盤X軸.AutoSize = true;
            this.lbl_acpos_載盤X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_載盤X軸.Location = new System.Drawing.Point(340, 105);
            this.lbl_acpos_載盤X軸.Name = "lbl_acpos_載盤X軸";
            this.lbl_acpos_載盤X軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_載盤X軸.TabIndex = 40;
            this.lbl_acpos_載盤X軸.Text = "0.00";
            // 
            // lbl_acpos_吸嘴R軸
            // 
            this.lbl_acpos_吸嘴R軸.AutoSize = true;
            this.lbl_acpos_吸嘴R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴R軸.Location = new System.Drawing.Point(340, 85);
            this.lbl_acpos_吸嘴R軸.Name = "lbl_acpos_吸嘴R軸";
            this.lbl_acpos_吸嘴R軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_吸嘴R軸.TabIndex = 39;
            this.lbl_acpos_吸嘴R軸.Text = "0.00";
            // 
            // lbl_acpos_吸嘴Z軸
            // 
            this.lbl_acpos_吸嘴Z軸.AutoSize = true;
            this.lbl_acpos_吸嘴Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴Z軸.Location = new System.Drawing.Point(340, 65);
            this.lbl_acpos_吸嘴Z軸.Name = "lbl_acpos_吸嘴Z軸";
            this.lbl_acpos_吸嘴Z軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_吸嘴Z軸.TabIndex = 38;
            this.lbl_acpos_吸嘴Z軸.Text = "0.00";
            // 
            // lbl_acpos_吸嘴Y軸
            // 
            this.lbl_acpos_吸嘴Y軸.AutoSize = true;
            this.lbl_acpos_吸嘴Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴Y軸.Location = new System.Drawing.Point(340, 45);
            this.lbl_acpos_吸嘴Y軸.Name = "lbl_acpos_吸嘴Y軸";
            this.lbl_acpos_吸嘴Y軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_吸嘴Y軸.TabIndex = 37;
            this.lbl_acpos_吸嘴Y軸.Text = "0.00";
            // 
            // lbl_acpos_吸嘴X軸
            // 
            this.lbl_acpos_吸嘴X軸.AutoSize = true;
            this.lbl_acpos_吸嘴X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴X軸.Location = new System.Drawing.Point(340, 25);
            this.lbl_acpos_吸嘴X軸.Name = "lbl_acpos_吸嘴X軸";
            this.lbl_acpos_吸嘴X軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_acpos_吸嘴X軸.TabIndex = 36;
            this.lbl_acpos_吸嘴X軸.Text = "0.00";
            // 
            // lbl_acpos_工作門_lbl
            // 
            this.lbl_acpos_工作門_lbl.AutoSize = true;
            this.lbl_acpos_工作門_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_工作門_lbl.Location = new System.Drawing.Point(290, 185);
            this.lbl_acpos_工作門_lbl.Name = "lbl_acpos_工作門_lbl";
            this.lbl_acpos_工作門_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_工作門_lbl.TabIndex = 35;
            this.lbl_acpos_工作門_lbl.Text = "acpos";
            // 
            // lbl_acpos_植針R軸_lbl
            // 
            this.lbl_acpos_植針R軸_lbl.AutoSize = true;
            this.lbl_acpos_植針R軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_植針R軸_lbl.Location = new System.Drawing.Point(290, 165);
            this.lbl_acpos_植針R軸_lbl.Name = "lbl_acpos_植針R軸_lbl";
            this.lbl_acpos_植針R軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_植針R軸_lbl.TabIndex = 34;
            this.lbl_acpos_植針R軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_植針Z軸_lbl
            // 
            this.lbl_acpos_植針Z軸_lbl.AutoSize = true;
            this.lbl_acpos_植針Z軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_植針Z軸_lbl.Location = new System.Drawing.Point(290, 145);
            this.lbl_acpos_植針Z軸_lbl.Name = "lbl_acpos_植針Z軸_lbl";
            this.lbl_acpos_植針Z軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_植針Z軸_lbl.TabIndex = 33;
            this.lbl_acpos_植針Z軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_載盤Y軸_lbl
            // 
            this.lbl_acpos_載盤Y軸_lbl.AutoSize = true;
            this.lbl_acpos_載盤Y軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_載盤Y軸_lbl.Location = new System.Drawing.Point(290, 125);
            this.lbl_acpos_載盤Y軸_lbl.Name = "lbl_acpos_載盤Y軸_lbl";
            this.lbl_acpos_載盤Y軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_載盤Y軸_lbl.TabIndex = 32;
            this.lbl_acpos_載盤Y軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_載盤X軸_lbl
            // 
            this.lbl_acpos_載盤X軸_lbl.AutoSize = true;
            this.lbl_acpos_載盤X軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_載盤X軸_lbl.Location = new System.Drawing.Point(290, 105);
            this.lbl_acpos_載盤X軸_lbl.Name = "lbl_acpos_載盤X軸_lbl";
            this.lbl_acpos_載盤X軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_載盤X軸_lbl.TabIndex = 31;
            this.lbl_acpos_載盤X軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_吸嘴R軸_lbl
            // 
            this.lbl_acpos_吸嘴R軸_lbl.AutoSize = true;
            this.lbl_acpos_吸嘴R軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴R軸_lbl.Location = new System.Drawing.Point(290, 85);
            this.lbl_acpos_吸嘴R軸_lbl.Name = "lbl_acpos_吸嘴R軸_lbl";
            this.lbl_acpos_吸嘴R軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_吸嘴R軸_lbl.TabIndex = 30;
            this.lbl_acpos_吸嘴R軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_吸嘴Z軸_lbl
            // 
            this.lbl_acpos_吸嘴Z軸_lbl.AutoSize = true;
            this.lbl_acpos_吸嘴Z軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴Z軸_lbl.Location = new System.Drawing.Point(290, 65);
            this.lbl_acpos_吸嘴Z軸_lbl.Name = "lbl_acpos_吸嘴Z軸_lbl";
            this.lbl_acpos_吸嘴Z軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_吸嘴Z軸_lbl.TabIndex = 29;
            this.lbl_acpos_吸嘴Z軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_吸嘴Y軸_lbl
            // 
            this.lbl_acpos_吸嘴Y軸_lbl.AutoSize = true;
            this.lbl_acpos_吸嘴Y軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴Y軸_lbl.Location = new System.Drawing.Point(290, 45);
            this.lbl_acpos_吸嘴Y軸_lbl.Name = "lbl_acpos_吸嘴Y軸_lbl";
            this.lbl_acpos_吸嘴Y軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_吸嘴Y軸_lbl.TabIndex = 28;
            this.lbl_acpos_吸嘴Y軸_lbl.Text = "acpos";
            // 
            // lbl_acpos_吸嘴X軸_lbl
            // 
            this.lbl_acpos_吸嘴X軸_lbl.AutoSize = true;
            this.lbl_acpos_吸嘴X軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_acpos_吸嘴X軸_lbl.Location = new System.Drawing.Point(290, 25);
            this.lbl_acpos_吸嘴X軸_lbl.Name = "lbl_acpos_吸嘴X軸_lbl";
            this.lbl_acpos_吸嘴X軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_acpos_吸嘴X軸_lbl.TabIndex = 27;
            this.lbl_acpos_吸嘴X軸_lbl.Text = "acpos";
            // 
            // lbl_spd_工作門
            // 
            this.lbl_spd_工作門.AutoSize = true;
            this.lbl_spd_工作門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_工作門.Location = new System.Drawing.Point(230, 185);
            this.lbl_spd_工作門.Name = "lbl_spd_工作門";
            this.lbl_spd_工作門.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_工作門.TabIndex = 26;
            this.lbl_spd_工作門.Text = "0.00";
            // 
            // lbl_spd_植針R軸
            // 
            this.lbl_spd_植針R軸.AutoSize = true;
            this.lbl_spd_植針R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_植針R軸.Location = new System.Drawing.Point(230, 165);
            this.lbl_spd_植針R軸.Name = "lbl_spd_植針R軸";
            this.lbl_spd_植針R軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_植針R軸.TabIndex = 25;
            this.lbl_spd_植針R軸.Text = "0.00";
            // 
            // lbl_spd_植針Z軸
            // 
            this.lbl_spd_植針Z軸.AutoSize = true;
            this.lbl_spd_植針Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_植針Z軸.Location = new System.Drawing.Point(230, 145);
            this.lbl_spd_植針Z軸.Name = "lbl_spd_植針Z軸";
            this.lbl_spd_植針Z軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_植針Z軸.TabIndex = 24;
            this.lbl_spd_植針Z軸.Text = "0.00";
            // 
            // lbl_spd_載盤Y軸
            // 
            this.lbl_spd_載盤Y軸.AutoSize = true;
            this.lbl_spd_載盤Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_載盤Y軸.Location = new System.Drawing.Point(230, 125);
            this.lbl_spd_載盤Y軸.Name = "lbl_spd_載盤Y軸";
            this.lbl_spd_載盤Y軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_載盤Y軸.TabIndex = 23;
            this.lbl_spd_載盤Y軸.Text = "0.00";
            // 
            // lbl_spd_載盤X軸
            // 
            this.lbl_spd_載盤X軸.AutoSize = true;
            this.lbl_spd_載盤X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_載盤X軸.Location = new System.Drawing.Point(230, 105);
            this.lbl_spd_載盤X軸.Name = "lbl_spd_載盤X軸";
            this.lbl_spd_載盤X軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_載盤X軸.TabIndex = 22;
            this.lbl_spd_載盤X軸.Text = "0.00";
            // 
            // lbl_spd_吸嘴R軸
            // 
            this.lbl_spd_吸嘴R軸.AutoSize = true;
            this.lbl_spd_吸嘴R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴R軸.Location = new System.Drawing.Point(230, 85);
            this.lbl_spd_吸嘴R軸.Name = "lbl_spd_吸嘴R軸";
            this.lbl_spd_吸嘴R軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_吸嘴R軸.TabIndex = 21;
            this.lbl_spd_吸嘴R軸.Text = "0.00";
            // 
            // lbl_spd_吸嘴Z軸
            // 
            this.lbl_spd_吸嘴Z軸.AutoSize = true;
            this.lbl_spd_吸嘴Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴Z軸.Location = new System.Drawing.Point(230, 65);
            this.lbl_spd_吸嘴Z軸.Name = "lbl_spd_吸嘴Z軸";
            this.lbl_spd_吸嘴Z軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_吸嘴Z軸.TabIndex = 20;
            this.lbl_spd_吸嘴Z軸.Text = "0.00";
            // 
            // lbl_spd_吸嘴Y軸
            // 
            this.lbl_spd_吸嘴Y軸.AutoSize = true;
            this.lbl_spd_吸嘴Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴Y軸.Location = new System.Drawing.Point(230, 45);
            this.lbl_spd_吸嘴Y軸.Name = "lbl_spd_吸嘴Y軸";
            this.lbl_spd_吸嘴Y軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_吸嘴Y軸.TabIndex = 19;
            this.lbl_spd_吸嘴Y軸.Text = "0.00";
            // 
            // lbl_spd_吸嘴X軸
            // 
            this.lbl_spd_吸嘴X軸.AutoSize = true;
            this.lbl_spd_吸嘴X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴X軸.Location = new System.Drawing.Point(230, 25);
            this.lbl_spd_吸嘴X軸.Name = "lbl_spd_吸嘴X軸";
            this.lbl_spd_吸嘴X軸.Size = new System.Drawing.Size(28, 13);
            this.lbl_spd_吸嘴X軸.TabIndex = 18;
            this.lbl_spd_吸嘴X軸.Text = "0.00";
            // 
            // lbl_spd_工作門_lbl
            // 
            this.lbl_spd_工作門_lbl.AutoSize = true;
            this.lbl_spd_工作門_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_工作門_lbl.Location = new System.Drawing.Point(180, 185);
            this.lbl_spd_工作門_lbl.Name = "lbl_spd_工作門_lbl";
            this.lbl_spd_工作門_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_工作門_lbl.TabIndex = 17;
            this.lbl_spd_工作門_lbl.Text = "speed";
            // 
            // lbl_spd_植針R軸_lbl
            // 
            this.lbl_spd_植針R軸_lbl.AutoSize = true;
            this.lbl_spd_植針R軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_植針R軸_lbl.Location = new System.Drawing.Point(180, 165);
            this.lbl_spd_植針R軸_lbl.Name = "lbl_spd_植針R軸_lbl";
            this.lbl_spd_植針R軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_植針R軸_lbl.TabIndex = 16;
            this.lbl_spd_植針R軸_lbl.Text = "speed";
            // 
            // lbl_spd_植針Z軸_lbl
            // 
            this.lbl_spd_植針Z軸_lbl.AutoSize = true;
            this.lbl_spd_植針Z軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_植針Z軸_lbl.Location = new System.Drawing.Point(180, 145);
            this.lbl_spd_植針Z軸_lbl.Name = "lbl_spd_植針Z軸_lbl";
            this.lbl_spd_植針Z軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_植針Z軸_lbl.TabIndex = 15;
            this.lbl_spd_植針Z軸_lbl.Text = "speed";
            // 
            // lbl_spd_載盤Y軸_lbl
            // 
            this.lbl_spd_載盤Y軸_lbl.AutoSize = true;
            this.lbl_spd_載盤Y軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_載盤Y軸_lbl.Location = new System.Drawing.Point(180, 125);
            this.lbl_spd_載盤Y軸_lbl.Name = "lbl_spd_載盤Y軸_lbl";
            this.lbl_spd_載盤Y軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_載盤Y軸_lbl.TabIndex = 14;
            this.lbl_spd_載盤Y軸_lbl.Text = "speed";
            // 
            // lbl_spd_載盤X軸_lbl
            // 
            this.lbl_spd_載盤X軸_lbl.AutoSize = true;
            this.lbl_spd_載盤X軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_載盤X軸_lbl.Location = new System.Drawing.Point(180, 105);
            this.lbl_spd_載盤X軸_lbl.Name = "lbl_spd_載盤X軸_lbl";
            this.lbl_spd_載盤X軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_載盤X軸_lbl.TabIndex = 13;
            this.lbl_spd_載盤X軸_lbl.Text = "speed";
            // 
            // lbl_spd_吸嘴R軸_lbl
            // 
            this.lbl_spd_吸嘴R軸_lbl.AutoSize = true;
            this.lbl_spd_吸嘴R軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴R軸_lbl.Location = new System.Drawing.Point(180, 85);
            this.lbl_spd_吸嘴R軸_lbl.Name = "lbl_spd_吸嘴R軸_lbl";
            this.lbl_spd_吸嘴R軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_吸嘴R軸_lbl.TabIndex = 12;
            this.lbl_spd_吸嘴R軸_lbl.Text = "speed";
            // 
            // lbl_spd_吸嘴Z軸_lbl
            // 
            this.lbl_spd_吸嘴Z軸_lbl.AutoSize = true;
            this.lbl_spd_吸嘴Z軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴Z軸_lbl.Location = new System.Drawing.Point(180, 65);
            this.lbl_spd_吸嘴Z軸_lbl.Name = "lbl_spd_吸嘴Z軸_lbl";
            this.lbl_spd_吸嘴Z軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_吸嘴Z軸_lbl.TabIndex = 11;
            this.lbl_spd_吸嘴Z軸_lbl.Text = "speed";
            // 
            // lbl_spd_吸嘴Y軸_lbl
            // 
            this.lbl_spd_吸嘴Y軸_lbl.AutoSize = true;
            this.lbl_spd_吸嘴Y軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴Y軸_lbl.Location = new System.Drawing.Point(180, 45);
            this.lbl_spd_吸嘴Y軸_lbl.Name = "lbl_spd_吸嘴Y軸_lbl";
            this.lbl_spd_吸嘴Y軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_吸嘴Y軸_lbl.TabIndex = 10;
            this.lbl_spd_吸嘴Y軸_lbl.Text = "speed";
            // 
            // lbl_spd_吸嘴X軸_lbl
            // 
            this.lbl_spd_吸嘴X軸_lbl.AutoSize = true;
            this.lbl_spd_吸嘴X軸_lbl.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_spd_吸嘴X軸_lbl.Location = new System.Drawing.Point(180, 25);
            this.lbl_spd_吸嘴X軸_lbl.Name = "lbl_spd_吸嘴X軸_lbl";
            this.lbl_spd_吸嘴X軸_lbl.Size = new System.Drawing.Size(34, 13);
            this.lbl_spd_吸嘴X軸_lbl.TabIndex = 9;
            this.lbl_spd_吸嘴X軸_lbl.Text = "speed";
            // 
            // select_工作門
            // 
            this.select_工作門.AutoSize = true;
            this.select_工作門.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_工作門.Location = new System.Drawing.Point(95, 185);
            this.select_工作門.Name = "select_工作門";
            this.select_工作門.Size = new System.Drawing.Size(64, 17);
            this.select_工作門.TabIndex = 8;
            this.select_工作門.Text = "工作門";
            this.select_工作門.UseVisualStyleBackColor = true;
            this.select_工作門.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_植針R軸
            // 
            this.select_植針R軸.AutoSize = true;
            this.select_植針R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_植針R軸.Location = new System.Drawing.Point(95, 165);
            this.select_植針R軸.Name = "select_植針R軸";
            this.select_植針R軸.Size = new System.Drawing.Size(72, 17);
            this.select_植針R軸.TabIndex = 7;
            this.select_植針R軸.Text = "植針R軸";
            this.select_植針R軸.UseVisualStyleBackColor = true;
            this.select_植針R軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_植針Z軸
            // 
            this.select_植針Z軸.AutoSize = true;
            this.select_植針Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_植針Z軸.Location = new System.Drawing.Point(95, 145);
            this.select_植針Z軸.Name = "select_植針Z軸";
            this.select_植針Z軸.Size = new System.Drawing.Size(71, 17);
            this.select_植針Z軸.TabIndex = 6;
            this.select_植針Z軸.Text = "植針Z軸";
            this.select_植針Z軸.UseVisualStyleBackColor = true;
            this.select_植針Z軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_載盤Y軸
            // 
            this.select_載盤Y軸.AutoSize = true;
            this.select_載盤Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_載盤Y軸.Location = new System.Drawing.Point(95, 125);
            this.select_載盤Y軸.Name = "select_載盤Y軸";
            this.select_載盤Y軸.Size = new System.Drawing.Size(73, 17);
            this.select_載盤Y軸.TabIndex = 5;
            this.select_載盤Y軸.Text = "載盤Y軸";
            this.select_載盤Y軸.UseVisualStyleBackColor = true;
            this.select_載盤Y軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_載盤X軸
            // 
            this.select_載盤X軸.AutoSize = true;
            this.select_載盤X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_載盤X軸.Location = new System.Drawing.Point(95, 105);
            this.select_載盤X軸.Name = "select_載盤X軸";
            this.select_載盤X軸.Size = new System.Drawing.Size(73, 17);
            this.select_載盤X軸.TabIndex = 4;
            this.select_載盤X軸.Text = "載盤X軸";
            this.select_載盤X軸.UseVisualStyleBackColor = true;
            this.select_載盤X軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_吸嘴R軸
            // 
            this.select_吸嘴R軸.AutoSize = true;
            this.select_吸嘴R軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_吸嘴R軸.Location = new System.Drawing.Point(95, 85);
            this.select_吸嘴R軸.Name = "select_吸嘴R軸";
            this.select_吸嘴R軸.Size = new System.Drawing.Size(72, 17);
            this.select_吸嘴R軸.TabIndex = 3;
            this.select_吸嘴R軸.Text = "吸嘴R軸";
            this.select_吸嘴R軸.UseVisualStyleBackColor = true;
            this.select_吸嘴R軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_吸嘴Z軸
            // 
            this.select_吸嘴Z軸.AutoSize = true;
            this.select_吸嘴Z軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_吸嘴Z軸.Location = new System.Drawing.Point(95, 65);
            this.select_吸嘴Z軸.Name = "select_吸嘴Z軸";
            this.select_吸嘴Z軸.Size = new System.Drawing.Size(71, 17);
            this.select_吸嘴Z軸.TabIndex = 2;
            this.select_吸嘴Z軸.Text = "吸嘴Z軸";
            this.select_吸嘴Z軸.UseVisualStyleBackColor = true;
            this.select_吸嘴Z軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_吸嘴Y軸
            // 
            this.select_吸嘴Y軸.AutoSize = true;
            this.select_吸嘴Y軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_吸嘴Y軸.Location = new System.Drawing.Point(95, 45);
            this.select_吸嘴Y軸.Name = "select_吸嘴Y軸";
            this.select_吸嘴Y軸.Size = new System.Drawing.Size(73, 17);
            this.select_吸嘴Y軸.TabIndex = 1;
            this.select_吸嘴Y軸.Text = "吸嘴Y軸";
            this.select_吸嘴Y軸.UseVisualStyleBackColor = true;
            this.select_吸嘴Y軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // select_吸嘴X軸
            // 
            this.select_吸嘴X軸.AutoSize = true;
            this.select_吸嘴X軸.Checked = true;
            this.select_吸嘴X軸.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_吸嘴X軸.Location = new System.Drawing.Point(95, 25);
            this.select_吸嘴X軸.Name = "select_吸嘴X軸";
            this.select_吸嘴X軸.Size = new System.Drawing.Size(73, 17);
            this.select_吸嘴X軸.TabIndex = 0;
            this.select_吸嘴X軸.TabStop = true;
            this.select_吸嘴X軸.Text = "吸嘴X軸";
            this.select_吸嘴X軸.UseVisualStyleBackColor = true;
            this.select_吸嘴X軸.CheckedChanged += new System.EventHandler(this.RadioGroupChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbl_2pCalibraLog);
            this.tabPage2.Controls.Add(this.btn_兩點校正);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.btn抽針);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.btn_socket相機兩點定位);
            this.tabPage2.Controls.Add(this.cB_料盤有料);
            this.tabPage2.Controls.Add(this.cB_AlwaysResume);
            this.tabPage2.Controls.Add(this.lbl_PinIndex);
            this.tabPage2.Controls.Add(this.btn_SwitchPointAB);
            this.tabPage2.Controls.Add(this.btn_ToPointB);
            this.tabPage2.Controls.Add(this.btn_ToPointA);
            this.tabPage2.Controls.Add(this.lbl_計算角度);
            this.tabPage2.Controls.Add(this.tB_By);
            this.tabPage2.Controls.Add(this.tB_Ay);
            this.tabPage2.Controls.Add(this.tB_Bx);
            this.tabPage2.Controls.Add(this.tB_Ax);
            this.tabPage2.Controls.Add(this.lbl_竹蜻蜓座標B);
            this.tabPage2.Controls.Add(this.lbl_竹蜻蜓座標A);
            this.tabPage2.Controls.Add(this.btn_tmrClear);
            this.tabPage2.Controls.Add(this.txt_HoldIndex);
            this.tabPage2.Controls.Add(this.btn_Resume);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.btn_取得目標座標);
            this.tabPage2.Controls.Add(this.lbl_CycleTime);
            this.tabPage2.Controls.Add(this.btn_tmrPause);
            this.tabPage2.Controls.Add(this.btn_tmrStop);
            this.tabPage2.Controls.Add(this.btn上膛);
            this.tabPage2.Controls.Add(this.lblLog);
            this.tabPage2.Controls.Add(this.txt_取料循環);
            this.tabPage2.Controls.Add(this.btn_TakePin);
            this.tabPage2.Controls.Add(this.btn_home);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.btn_植針嘴檢查);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.btn_Socket孔檢查);
            this.tabPage2.Controls.Add(this.btnStop);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btnSetHome);
            this.tabPage2.Controls.Add(this.btn_Disconnect);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btn_取得PinInfo);
            this.tabPage2.Controls.Add(this.btn_AlarmRST);
            this.tabPage2.Controls.Add(this.btn_Connect);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1228, 792);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbl_2pCalibraLog
            // 
            this.lbl_2pCalibraLog.AutoSize = true;
            this.lbl_2pCalibraLog.Location = new System.Drawing.Point(26, 743);
            this.lbl_2pCalibraLog.Name = "lbl_2pCalibraLog";
            this.lbl_2pCalibraLog.Size = new System.Drawing.Size(137, 19);
            this.lbl_2pCalibraLog.TabIndex = 243;
            this.lbl_2pCalibraLog.Text = "lbl_2pCalibraLog";
            // 
            // btn_兩點校正
            // 
            this.btn_兩點校正.Location = new System.Drawing.Point(472, 255);
            this.btn_兩點校正.Name = "btn_兩點校正";
            this.btn_兩點校正.Size = new System.Drawing.Size(129, 40);
            this.btn_兩點校正.TabIndex = 242;
            this.btn_兩點校正.Text = "兩點校正";
            this.btn_兩點校正.UseVisualStyleBackColor = true;
            this.btn_兩點校正.Click += new System.EventHandler(this.btn_兩點校正_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(626, 417);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(104, 19);
            this.label19.TabIndex = 241;
            this.label19.Text = "下視覺相關";
            // 
            // btn抽針
            // 
            this.btn抽針.Location = new System.Drawing.Point(472, 210);
            this.btn抽針.Name = "btn抽針";
            this.btn抽針.Size = new System.Drawing.Size(129, 40);
            this.btn抽針.TabIndex = 240;
            this.btn抽針.Text = "抽針";
            this.btn抽針.UseVisualStyleBackColor = true;
            this.btn抽針.Click += new System.EventHandler(this.btn抽針_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(912, 545);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 19);
            this.label16.TabIndex = 239;
            this.label16.Text = "label16";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(912, 517);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(62, 19);
            this.label17.TabIndex = 238;
            this.label17.Text = "label17";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(912, 487);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(62, 19);
            this.label18.TabIndex = 237;
            this.label18.Text = "label18";
            // 
            // btn_socket相機兩點定位
            // 
            this.btn_socket相機兩點定位.Location = new System.Drawing.Point(916, 426);
            this.btn_socket相機兩點定位.Name = "btn_socket相機兩點定位";
            this.btn_socket相機兩點定位.Size = new System.Drawing.Size(200, 38);
            this.btn_socket相機兩點定位.TabIndex = 236;
            this.btn_socket相機兩點定位.Text = "socket相機兩點定位";
            this.btn_socket相機兩點定位.UseVisualStyleBackColor = true;
            this.btn_socket相機兩點定位.Click += new System.EventHandler(this.btn_socket相機兩點定位_Click);
            // 
            // cB_料盤有料
            // 
            this.cB_料盤有料.AutoSize = true;
            this.cB_料盤有料.Location = new System.Drawing.Point(342, 436);
            this.cB_料盤有料.Name = "cB_料盤有料";
            this.cB_料盤有料.Size = new System.Drawing.Size(133, 23);
            this.cB_料盤有料.TabIndex = 235;
            this.cB_料盤有料.Text = "cB_料盤有料";
            this.cB_料盤有料.UseVisualStyleBackColor = true;
            // 
            // cB_AlwaysResume
            // 
            this.cB_AlwaysResume.AutoSize = true;
            this.cB_AlwaysResume.Location = new System.Drawing.Point(165, 468);
            this.cB_AlwaysResume.Name = "cB_AlwaysResume";
            this.cB_AlwaysResume.Size = new System.Drawing.Size(170, 23);
            this.cB_AlwaysResume.TabIndex = 234;
            this.cB_AlwaysResume.Text = "cB_AlwaysResume";
            this.cB_AlwaysResume.UseVisualStyleBackColor = true;
            // 
            // lbl_PinIndex
            // 
            this.lbl_PinIndex.AutoSize = true;
            this.lbl_PinIndex.Location = new System.Drawing.Point(34, 436);
            this.lbl_PinIndex.Name = "lbl_PinIndex";
            this.lbl_PinIndex.Size = new System.Drawing.Size(102, 19);
            this.lbl_PinIndex.TabIndex = 233;
            this.lbl_PinIndex.Text = "lbl_PinIndex";
            // 
            // btn_SwitchPointAB
            // 
            this.btn_SwitchPointAB.Location = new System.Drawing.Point(395, 540);
            this.btn_SwitchPointAB.Name = "btn_SwitchPointAB";
            this.btn_SwitchPointAB.Size = new System.Drawing.Size(80, 40);
            this.btn_SwitchPointAB.TabIndex = 232;
            this.btn_SwitchPointAB.Text = "AB交換";
            this.btn_SwitchPointAB.UseVisualStyleBackColor = true;
            this.btn_SwitchPointAB.Click += new System.EventHandler(this.btn_ToPointAB);
            // 
            // btn_ToPointB
            // 
            this.btn_ToPointB.Location = new System.Drawing.Point(850, 50);
            this.btn_ToPointB.Name = "btn_ToPointB";
            this.btn_ToPointB.Size = new System.Drawing.Size(70, 40);
            this.btn_ToPointB.TabIndex = 231;
            this.btn_ToPointB.Text = "To P:B";
            this.btn_ToPointB.UseVisualStyleBackColor = true;
            this.btn_ToPointB.Click += new System.EventHandler(this.btn_ToPointAB);
            // 
            // btn_ToPointA
            // 
            this.btn_ToPointA.Location = new System.Drawing.Point(780, 50);
            this.btn_ToPointA.Name = "btn_ToPointA";
            this.btn_ToPointA.Size = new System.Drawing.Size(70, 40);
            this.btn_ToPointA.TabIndex = 230;
            this.btn_ToPointA.Text = "To P:A";
            this.btn_ToPointA.UseVisualStyleBackColor = true;
            this.btn_ToPointA.Click += new System.EventHandler(this.btn_ToPointAB);
            // 
            // lbl_計算角度
            // 
            this.lbl_計算角度.AutoSize = true;
            this.lbl_計算角度.Location = new System.Drawing.Point(34, 608);
            this.lbl_計算角度.Name = "lbl_計算角度";
            this.lbl_計算角度.Size = new System.Drawing.Size(156, 19);
            this.lbl_計算角度.TabIndex = 229;
            this.lbl_計算角度.Text = "lbl_計算角度 : 0度";
            // 
            // tB_By
            // 
            this.tB_By.Location = new System.Drawing.Point(330, 564);
            this.tB_By.Name = "tB_By";
            this.tB_By.Size = new System.Drawing.Size(50, 30);
            this.tB_By.TabIndex = 228;
            this.tB_By.Text = "0";
            this.tB_By.TextChanged += new System.EventHandler(this.tB_PointAB_Calculate);
            // 
            // tB_Ay
            // 
            this.tB_Ay.Location = new System.Drawing.Point(330, 527);
            this.tB_Ay.Name = "tB_Ay";
            this.tB_Ay.Size = new System.Drawing.Size(50, 30);
            this.tB_Ay.TabIndex = 227;
            this.tB_Ay.Text = "0";
            this.tB_Ay.TextChanged += new System.EventHandler(this.tB_PointAB_Calculate);
            // 
            // tB_Bx
            // 
            this.tB_Bx.Location = new System.Drawing.Point(193, 564);
            this.tB_Bx.Name = "tB_Bx";
            this.tB_Bx.Size = new System.Drawing.Size(50, 30);
            this.tB_Bx.TabIndex = 226;
            this.tB_Bx.Text = "0";
            this.tB_Bx.TextChanged += new System.EventHandler(this.tB_PointAB_Calculate);
            // 
            // tB_Ax
            // 
            this.tB_Ax.Location = new System.Drawing.Point(194, 527);
            this.tB_Ax.Name = "tB_Ax";
            this.tB_Ax.Size = new System.Drawing.Size(50, 30);
            this.tB_Ax.TabIndex = 225;
            this.tB_Ax.Text = "0";
            this.tB_Ax.TextChanged += new System.EventHandler(this.tB_PointAB_Calculate);
            // 
            // lbl_竹蜻蜓座標B
            // 
            this.lbl_竹蜻蜓座標B.AutoSize = true;
            this.lbl_竹蜻蜓座標B.Location = new System.Drawing.Point(34, 575);
            this.lbl_竹蜻蜓座標B.Name = "lbl_竹蜻蜓座標B";
            this.lbl_竹蜻蜓座標B.Size = new System.Drawing.Size(292, 19);
            this.lbl_竹蜻蜓座標B.TabIndex = 224;
            this.lbl_竹蜻蜓座標B.Text = "lbl_竹蜻蜓 座標Bx               座標By";
            // 
            // lbl_竹蜻蜓座標A
            // 
            this.lbl_竹蜻蜓座標A.AutoSize = true;
            this.lbl_竹蜻蜓座標A.Location = new System.Drawing.Point(34, 538);
            this.lbl_竹蜻蜓座標A.Name = "lbl_竹蜻蜓座標A";
            this.lbl_竹蜻蜓座標A.Size = new System.Drawing.Size(294, 19);
            this.lbl_竹蜻蜓座標A.TabIndex = 223;
            this.lbl_竹蜻蜓座標A.Text = "lbl_竹蜻蜓 座標Ax               座標Ay";
            // 
            // btn_tmrClear
            // 
            this.btn_tmrClear.Location = new System.Drawing.Point(336, 300);
            this.btn_tmrClear.Name = "btn_tmrClear";
            this.btn_tmrClear.Size = new System.Drawing.Size(129, 40);
            this.btn_tmrClear.TabIndex = 222;
            this.btn_tmrClear.Text = "清除流程";
            this.btn_tmrClear.UseVisualStyleBackColor = true;
            this.btn_tmrClear.Click += new System.EventHandler(this.btn_tmrClear_Click);
            // 
            // txt_HoldIndex
            // 
            this.txt_HoldIndex.Location = new System.Drawing.Point(38, 458);
            this.txt_HoldIndex.Name = "txt_HoldIndex";
            this.txt_HoldIndex.Size = new System.Drawing.Size(50, 30);
            this.txt_HoldIndex.TabIndex = 221;
            this.txt_HoldIndex.Text = "0";
            // 
            // btn_Resume
            // 
            this.btn_Resume.Location = new System.Drawing.Point(165, 430);
            this.btn_Resume.Name = "btn_Resume";
            this.btn_Resume.Size = new System.Drawing.Size(116, 31);
            this.btn_Resume.TabIndex = 220;
            this.btn_Resume.Text = "btn_Resume";
            this.btn_Resume.UseVisualStyleBackColor = true;
            this.btn_Resume.Click += new System.EventHandler(this.btn_Resume_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(164, 346);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(62, 19);
            this.label15.TabIndex = 219;
            this.label15.Text = "label15";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(164, 323);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 19);
            this.label14.TabIndex = 218;
            this.label14.Text = "label14";
            // 
            // btn_取得目標座標
            // 
            this.btn_取得目標座標.Location = new System.Drawing.Point(29, 323);
            this.btn_取得目標座標.Name = "btn_取得目標座標";
            this.btn_取得目標座標.Size = new System.Drawing.Size(134, 42);
            this.btn_取得目標座標.TabIndex = 217;
            this.btn_取得目標座標.Text = "取得目標座標";
            this.btn_取得目標座標.UseVisualStyleBackColor = true;
            this.btn_取得目標座標.Click += new System.EventHandler(this.btn_取得目標座標_Click);
            // 
            // lbl_CycleTime
            // 
            this.lbl_CycleTime.AutoSize = true;
            this.lbl_CycleTime.Location = new System.Drawing.Point(25, 387);
            this.lbl_CycleTime.Name = "lbl_CycleTime";
            this.lbl_CycleTime.Size = new System.Drawing.Size(138, 19);
            this.lbl_CycleTime.TabIndex = 216;
            this.lbl_CycleTime.Text = "取針循環時間 : ";
            // 
            // btn_tmrPause
            // 
            this.btn_tmrPause.Location = new System.Drawing.Point(336, 255);
            this.btn_tmrPause.Name = "btn_tmrPause";
            this.btn_tmrPause.Size = new System.Drawing.Size(129, 40);
            this.btn_tmrPause.TabIndex = 215;
            this.btn_tmrPause.Text = "循環暫停";
            this.btn_tmrPause.UseVisualStyleBackColor = true;
            this.btn_tmrPause.Click += new System.EventHandler(this.btn_tmrPause_Click);
            // 
            // btn_tmrStop
            // 
            this.btn_tmrStop.Location = new System.Drawing.Point(200, 255);
            this.btn_tmrStop.Name = "btn_tmrStop";
            this.btn_tmrStop.Size = new System.Drawing.Size(129, 40);
            this.btn_tmrStop.TabIndex = 214;
            this.btn_tmrStop.Text = "循環停止";
            this.btn_tmrStop.UseVisualStyleBackColor = true;
            this.btn_tmrStop.Click += new System.EventHandler(this.btn_tmrStop_Click);
            // 
            // btn上膛
            // 
            this.btn上膛.Location = new System.Drawing.Point(336, 210);
            this.btn上膛.Name = "btn上膛";
            this.btn上膛.Size = new System.Drawing.Size(129, 40);
            this.btn上膛.TabIndex = 213;
            this.btn上膛.Text = "上膛";
            this.btn上膛.UseVisualStyleBackColor = true;
            this.btn上膛.Click += new System.EventHandler(this.btn上膛_Click);
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(25, 715);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(57, 19);
            this.lblLog.TabIndex = 212;
            this.lblLog.Text = "lblLog";
            // 
            // txt_取料循環
            // 
            this.txt_取料循環.Location = new System.Drawing.Point(90, 218);
            this.txt_取料循環.Name = "txt_取料循環";
            this.txt_取料循環.Size = new System.Drawing.Size(100, 30);
            this.txt_取料循環.TabIndex = 31;
            this.txt_取料循環.Text = "1";
            // 
            // btn_TakePin
            // 
            this.btn_TakePin.Location = new System.Drawing.Point(200, 210);
            this.btn_TakePin.Name = "btn_TakePin";
            this.btn_TakePin.Size = new System.Drawing.Size(129, 40);
            this.btn_TakePin.TabIndex = 30;
            this.btn_TakePin.Text = "取針丟棄";
            this.btn_TakePin.UseVisualStyleBackColor = true;
            this.btn_TakePin.Click += new System.EventHandler(this.btn_TakePin_Click);
            // 
            // btn_home
            // 
            this.btn_home.Location = new System.Drawing.Point(25, 160);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(165, 40);
            this.btn_home.TabIndex = 29;
            this.btn_home.Text = "home";
            this.btn_home.UseVisualStyleBackColor = true;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(626, 506);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 19);
            this.label8.TabIndex = 28;
            this.label8.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(626, 478);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 19);
            this.label9.TabIndex = 27;
            this.label9.Text = "label9";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(626, 448);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 19);
            this.label10.TabIndex = 26;
            this.label10.Text = "label10";
            // 
            // btn_植針嘴檢查
            // 
            this.btn_植針嘴檢查.Location = new System.Drawing.Point(769, 248);
            this.btn_植針嘴檢查.Name = "btn_植針嘴檢查";
            this.btn_植針嘴檢查.Size = new System.Drawing.Size(151, 38);
            this.btn_植針嘴檢查.TabIndex = 25;
            this.btn_植針嘴檢查.Text = "btn_植針嘴檢查";
            this.btn_植針嘴檢查.UseVisualStyleBackColor = true;
            this.btn_植針嘴檢查.Click += new System.EventHandler(this.btn_植針嘴檢查_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(625, 335);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 19);
            this.label7.TabIndex = 24;
            this.label7.Text = "label7";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(625, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 19);
            this.label6.TabIndex = 23;
            this.label6.Text = "label6";
            // 
            // btn_Socket孔檢查
            // 
            this.btn_Socket孔檢查.Location = new System.Drawing.Point(629, 248);
            this.btn_Socket孔檢查.Name = "btn_Socket孔檢查";
            this.btn_Socket孔檢查.Size = new System.Drawing.Size(134, 41);
            this.btn_Socket孔檢查.TabIndex = 22;
            this.btn_Socket孔檢查.Text = "Socket孔檢查";
            this.btn_Socket孔檢查.UseVisualStyleBackColor = true;
            this.btn_Socket孔檢查.Click += new System.EventHandler(this.btn_Socket孔檢查_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(200, 50);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(165, 40);
            this.btnStop.TabIndex = 21;
            this.btnStop.Text = "btnStop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 19);
            this.label1.TabIndex = 15;
            this.label1.Text = "label1";
            // 
            // btnSetHome
            // 
            this.btnSetHome.Location = new System.Drawing.Point(380, 50);
            this.btnSetHome.Name = "btnSetHome";
            this.btnSetHome.Size = new System.Drawing.Size(165, 40);
            this.btnSetHome.TabIndex = 14;
            this.btnSetHome.Text = "btnSetHome";
            this.btnSetHome.UseVisualStyleBackColor = true;
            this.btnSetHome.Click += new System.EventHandler(this.btnSetHome_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.Location = new System.Drawing.Point(25, 100);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(165, 40);
            this.btn_Disconnect.TabIndex = 9;
            this.btn_Disconnect.Text = "btn_Disconnect";
            this.btn_Disconnect.UseVisualStyleBackColor = true;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(625, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 19);
            this.label5.TabIndex = 8;
            this.label5.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(625, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(625, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "label3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(625, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            // 
            // btn_取得PinInfo
            // 
            this.btn_取得PinInfo.Location = new System.Drawing.Point(630, 50);
            this.btn_取得PinInfo.Name = "btn_取得PinInfo";
            this.btn_取得PinInfo.Size = new System.Drawing.Size(150, 40);
            this.btn_取得PinInfo.TabIndex = 4;
            this.btn_取得PinInfo.Text = "btn_取得PinInfo";
            this.btn_取得PinInfo.UseVisualStyleBackColor = true;
            this.btn_取得PinInfo.Click += new System.EventHandler(this.btn_取得PinInfo_Click);
            // 
            // btn_AlarmRST
            // 
            this.btn_AlarmRST.Location = new System.Drawing.Point(200, 100);
            this.btn_AlarmRST.Name = "btn_AlarmRST";
            this.btn_AlarmRST.Size = new System.Drawing.Size(165, 40);
            this.btn_AlarmRST.TabIndex = 3;
            this.btn_AlarmRST.Text = "btn_AlarmRST";
            this.btn_AlarmRST.UseVisualStyleBackColor = true;
            this.btn_AlarmRST.Click += new System.EventHandler(this.btn_AlarmRST_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(25, 50);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(165, 40);
            this.btn_Connect.TabIndex = 1;
            this.btn_Connect.Text = "btn_Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.inspector1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1228, 792);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // inspector1
            // 
            this.inspector1.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.inspector1.Location = new System.Drawing.Point(58, 7);
            this.inspector1.Margin = new System.Windows.Forms.Padding(5);
            this.inspector1.Name = "inspector1";
            this.inspector1.Size = new System.Drawing.Size(1112, 744);
            this.inspector1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabJob);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1236, 825);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.rtb_Status);
            this.tabPage3.Controls.Add(this.lbl_ZoomFactor);
            this.tabPage3.Controls.Add(this.lbl_Offset);
            this.tabPage3.Controls.Add(this.lbl_PicMousePos);
            this.tabPage3.Controls.Add(this.lbl_RealMousePos);
            this.tabPage3.Controls.Add(this.grp_NeedleInfo);
            this.tabPage3.Controls.Add(this.tab_Needles);
            this.tabPage3.Controls.Add(this.grp_目前作業項目);
            this.tabPage3.Controls.Add(this.grp_SocketInfo);
            this.tabPage3.Controls.Add(this.grp_儲存資訊);
            this.tabPage3.Controls.Add(this.ms_Viewer);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1228, 792);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // rtb_Status
            // 
            this.rtb_Status.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtb_Status.Location = new System.Drawing.Point(271, 605);
            this.rtb_Status.Name = "rtb_Status";
            this.rtb_Status.ReadOnly = true;
            this.rtb_Status.Size = new System.Drawing.Size(329, 180);
            this.rtb_Status.TabIndex = 31;
            this.rtb_Status.Text = "";
            // 
            // lbl_ZoomFactor
            // 
            this.lbl_ZoomFactor.AutoSize = true;
            this.lbl_ZoomFactor.Location = new System.Drawing.Point(12, 734);
            this.lbl_ZoomFactor.Name = "lbl_ZoomFactor";
            this.lbl_ZoomFactor.Size = new System.Drawing.Size(110, 19);
            this.lbl_ZoomFactor.TabIndex = 30;
            this.lbl_ZoomFactor.Text = "縮放比例  :  ";
            // 
            // lbl_Offset
            // 
            this.lbl_Offset.AutoSize = true;
            this.lbl_Offset.Location = new System.Drawing.Point(48, 715);
            this.lbl_Offset.Name = "lbl_Offset";
            this.lbl_Offset.Size = new System.Drawing.Size(69, 19);
            this.lbl_Offset.TabIndex = 29;
            this.lbl_Offset.Text = "Offset : ";
            // 
            // lbl_PicMousePos
            // 
            this.lbl_PicMousePos.AutoSize = true;
            this.lbl_PicMousePos.Location = new System.Drawing.Point(17, 695);
            this.lbl_PicMousePos.Name = "lbl_PicMousePos";
            this.lbl_PicMousePos.Size = new System.Drawing.Size(100, 19);
            this.lbl_PicMousePos.TabIndex = 28;
            this.lbl_PicMousePos.Text = "繪圖座標 : ";
            // 
            // lbl_RealMousePos
            // 
            this.lbl_RealMousePos.AutoSize = true;
            this.lbl_RealMousePos.Location = new System.Drawing.Point(17, 676);
            this.lbl_RealMousePos.Name = "lbl_RealMousePos";
            this.lbl_RealMousePos.Size = new System.Drawing.Size(100, 19);
            this.lbl_RealMousePos.TabIndex = 27;
            this.lbl_RealMousePos.Text = "真實座標 : ";
            // 
            // grp_NeedleInfo
            // 
            this.grp_NeedleInfo.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.grp_NeedleInfo.Controls.Add(this.btn_Reset);
            this.grp_NeedleInfo.Controls.Add(this.label13);
            this.grp_NeedleInfo.Controls.Add(this.chk_Reserve1);
            this.grp_NeedleInfo.Controls.Add(this.rad_Replace);
            this.grp_NeedleInfo.Controls.Add(this.rad_Remove);
            this.grp_NeedleInfo.Controls.Add(this.rad_Place);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Index);
            this.grp_NeedleInfo.Controls.Add(this.txt_Index);
            this.grp_NeedleInfo.Controls.Add(this.chk_Disable);
            this.grp_NeedleInfo.Controls.Add(this.chk_Display);
            this.grp_NeedleInfo.Controls.Add(this.txt_Diameter);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Diameter);
            this.grp_NeedleInfo.Controls.Add(this.txt_PosX);
            this.grp_NeedleInfo.Controls.Add(this.txt_PosY);
            this.grp_NeedleInfo.Controls.Add(this.lbl_PosX);
            this.grp_NeedleInfo.Controls.Add(this.txt_Id);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Id);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Name);
            this.grp_NeedleInfo.Controls.Add(this.txt_Name);
            this.grp_NeedleInfo.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grp_NeedleInfo.Location = new System.Drawing.Point(610, 676);
            this.grp_NeedleInfo.Margin = new System.Windows.Forms.Padding(2);
            this.grp_NeedleInfo.Name = "grp_NeedleInfo";
            this.grp_NeedleInfo.Padding = new System.Windows.Forms.Padding(2);
            this.grp_NeedleInfo.Size = new System.Drawing.Size(594, 114);
            this.grp_NeedleInfo.TabIndex = 26;
            this.grp_NeedleInfo.TabStop = false;
            this.grp_NeedleInfo.Text = "植針資訊";
            // 
            // btn_Reset
            // 
            this.btn_Reset.BackColor = System.Drawing.Color.LightCoral;
            this.btn_Reset.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_Reset.Location = new System.Drawing.Point(451, 85);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(60, 25);
            this.btn_Reset.TabIndex = 32;
            this.btn_Reset.Text = "清除";
            this.btn_Reset.UseVisualStyleBackColor = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(158, 74);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 16);
            this.label13.TabIndex = 25;
            this.label13.Text = "座標Y";
            // 
            // chk_Reserve1
            // 
            this.chk_Reserve1.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_Reserve1.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chk_Reserve1.Location = new System.Drawing.Point(451, 63);
            this.chk_Reserve1.Margin = new System.Windows.Forms.Padding(2);
            this.chk_Reserve1.Name = "chk_Reserve1";
            this.chk_Reserve1.Size = new System.Drawing.Size(60, 23);
            this.chk_Reserve1.TabIndex = 24;
            this.chk_Reserve1.Text = "保留";
            this.chk_Reserve1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_Reserve1.UseVisualStyleBackColor = true;
            this.chk_Reserve1.CheckedChanged += new System.EventHandler(this.chk_Reserve1_CheckedChanged);
            // 
            // rad_Replace
            // 
            this.rad_Replace.AutoSize = true;
            this.rad_Replace.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rad_Replace.Location = new System.Drawing.Point(520, 77);
            this.rad_Replace.Margin = new System.Windows.Forms.Padding(2);
            this.rad_Replace.Name = "rad_Replace";
            this.rad_Replace.Size = new System.Drawing.Size(57, 20);
            this.rad_Replace.TabIndex = 2;
            this.rad_Replace.TabStop = true;
            this.rad_Replace.Text = "置換";
            this.rad_Replace.UseVisualStyleBackColor = true;
            // 
            // rad_Remove
            // 
            this.rad_Remove.AutoSize = true;
            this.rad_Remove.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rad_Remove.Location = new System.Drawing.Point(520, 54);
            this.rad_Remove.Margin = new System.Windows.Forms.Padding(2);
            this.rad_Remove.Name = "rad_Remove";
            this.rad_Remove.Size = new System.Drawing.Size(57, 20);
            this.rad_Remove.TabIndex = 1;
            this.rad_Remove.TabStop = true;
            this.rad_Remove.Text = "取針";
            this.rad_Remove.UseVisualStyleBackColor = true;
            // 
            // rad_Place
            // 
            this.rad_Place.AutoSize = true;
            this.rad_Place.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rad_Place.Location = new System.Drawing.Point(520, 31);
            this.rad_Place.Margin = new System.Windows.Forms.Padding(2);
            this.rad_Place.Name = "rad_Place";
            this.rad_Place.Size = new System.Drawing.Size(57, 20);
            this.rad_Place.TabIndex = 0;
            this.rad_Place.TabStop = true;
            this.rad_Place.Text = "植針";
            this.rad_Place.UseVisualStyleBackColor = true;
            // 
            // lbl_Index
            // 
            this.lbl_Index.AutoSize = true;
            this.lbl_Index.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Index.Location = new System.Drawing.Point(3, 42);
            this.lbl_Index.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Index.Name = "lbl_Index";
            this.lbl_Index.Size = new System.Drawing.Size(55, 16);
            this.lbl_Index.TabIndex = 22;
            this.lbl_Index.Text = "流水號";
            // 
            // txt_Index
            // 
            this.txt_Index.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Index.Location = new System.Drawing.Point(63, 35);
            this.txt_Index.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Index.Name = "txt_Index";
            this.txt_Index.Size = new System.Drawing.Size(85, 28);
            this.txt_Index.TabIndex = 23;
            // 
            // chk_Disable
            // 
            this.chk_Disable.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_Disable.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chk_Disable.Location = new System.Drawing.Point(451, 41);
            this.chk_Disable.Margin = new System.Windows.Forms.Padding(2);
            this.chk_Disable.Name = "chk_Disable";
            this.chk_Disable.Size = new System.Drawing.Size(60, 23);
            this.chk_Disable.TabIndex = 21;
            this.chk_Disable.Text = "禁用";
            this.chk_Disable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_Disable.UseVisualStyleBackColor = true;
            this.chk_Disable.CheckedChanged += new System.EventHandler(this.chk_Disable_CheckedChanged);
            // 
            // chk_Display
            // 
            this.chk_Display.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_Display.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chk_Display.Location = new System.Drawing.Point(451, 19);
            this.chk_Display.Margin = new System.Windows.Forms.Padding(2);
            this.chk_Display.Name = "chk_Display";
            this.chk_Display.Size = new System.Drawing.Size(60, 23);
            this.chk_Display.TabIndex = 20;
            this.chk_Display.Text = "顯示";
            this.chk_Display.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_Display.UseVisualStyleBackColor = true;
            this.chk_Display.CheckedChanged += new System.EventHandler(this.chk_Display_CheckedChanged);
            // 
            // txt_Diameter
            // 
            this.txt_Diameter.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Diameter.Location = new System.Drawing.Point(341, 67);
            this.txt_Diameter.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Diameter.Name = "txt_Diameter";
            this.txt_Diameter.ReadOnly = true;
            this.txt_Diameter.Size = new System.Drawing.Size(85, 28);
            this.txt_Diameter.TabIndex = 16;
            // 
            // lbl_Diameter
            // 
            this.lbl_Diameter.AutoSize = true;
            this.lbl_Diameter.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Diameter.Location = new System.Drawing.Point(298, 73);
            this.lbl_Diameter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Diameter.Name = "lbl_Diameter";
            this.lbl_Diameter.Size = new System.Drawing.Size(39, 16);
            this.lbl_Diameter.TabIndex = 15;
            this.lbl_Diameter.Text = "直徑";
            // 
            // txt_PosX
            // 
            this.txt_PosX.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_PosX.Location = new System.Drawing.Point(63, 68);
            this.txt_PosX.Margin = new System.Windows.Forms.Padding(2);
            this.txt_PosX.Name = "txt_PosX";
            this.txt_PosX.ReadOnly = true;
            this.txt_PosX.Size = new System.Drawing.Size(85, 28);
            this.txt_PosX.TabIndex = 14;
            // 
            // txt_PosY
            // 
            this.txt_PosY.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_PosY.Location = new System.Drawing.Point(209, 68);
            this.txt_PosY.Margin = new System.Windows.Forms.Padding(2);
            this.txt_PosY.Name = "txt_PosY";
            this.txt_PosY.ReadOnly = true;
            this.txt_PosY.Size = new System.Drawing.Size(85, 28);
            this.txt_PosY.TabIndex = 13;
            // 
            // lbl_PosX
            // 
            this.lbl_PosX.AutoSize = true;
            this.lbl_PosX.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PosX.Location = new System.Drawing.Point(12, 74);
            this.lbl_PosX.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_PosX.Name = "lbl_PosX";
            this.lbl_PosX.Size = new System.Drawing.Size(47, 16);
            this.lbl_PosX.TabIndex = 12;
            this.lbl_PosX.Text = "座標X";
            // 
            // txt_Id
            // 
            this.txt_Id.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Id.Location = new System.Drawing.Point(341, 35);
            this.txt_Id.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Id.Name = "txt_Id";
            this.txt_Id.Size = new System.Drawing.Size(85, 28);
            this.txt_Id.TabIndex = 11;
            // 
            // lbl_Id
            // 
            this.lbl_Id.AutoSize = true;
            this.lbl_Id.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Id.Location = new System.Drawing.Point(298, 42);
            this.lbl_Id.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Id.Name = "lbl_Id";
            this.lbl_Id.Size = new System.Drawing.Size(39, 16);
            this.lbl_Id.TabIndex = 10;
            this.lbl_Id.Text = "編號";
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Name.Location = new System.Drawing.Point(159, 41);
            this.lbl_Name.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(39, 16);
            this.lbl_Name.TabIndex = 9;
            this.lbl_Name.Text = "名稱";
            // 
            // txt_Name
            // 
            this.txt_Name.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Name.Location = new System.Drawing.Point(209, 35);
            this.txt_Name.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(85, 28);
            this.txt_Name.TabIndex = 8;
            // 
            // tab_Needles
            // 
            this.tab_Needles.Controls.Add(this.tp_Needles);
            this.tab_Needles.Controls.Add(this.tp_NeedlesJudge);
            this.tab_Needles.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tab_Needles.Location = new System.Drawing.Point(606, 37);
            this.tab_Needles.Name = "tab_Needles";
            this.tab_Needles.SelectedIndex = 0;
            this.tab_Needles.Size = new System.Drawing.Size(609, 635);
            this.tab_Needles.TabIndex = 24;
            // 
            // tp_Needles
            // 
            this.tp_Needles.Controls.Add(this.pic_Needles);
            this.tp_Needles.Location = new System.Drawing.Point(4, 26);
            this.tp_Needles.Name = "tp_Needles";
            this.tp_Needles.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Needles.Size = new System.Drawing.Size(601, 605);
            this.tp_Needles.TabIndex = 0;
            this.tp_Needles.Text = "植針資訊";
            this.tp_Needles.UseVisualStyleBackColor = true;
            // 
            // pic_Needles
            // 
            this.pic_Needles.BackColor = System.Drawing.Color.Honeydew;
            this.pic_Needles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_Needles.ContextMenuStrip = this.cms_pic_Needles;
            this.pic_Needles.Location = new System.Drawing.Point(0, 0);
            this.pic_Needles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pic_Needles.Name = "pic_Needles";
            this.pic_Needles.Size = new System.Drawing.Size(600, 600);
            this.pic_Needles.TabIndex = 0;
            this.pic_Needles.TabStop = false;
            this.pic_Needles.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Needles_Paint);
            this.pic_Needles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseDown);
            this.pic_Needles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseMove);
            this.pic_Needles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseUp);
            this.pic_Needles.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseWheel);
            // 
            // cms_pic_Needles
            // 
            this.cms_pic_Needles.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cms_pic_Needles.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cms_pic_Needles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Place,
            this.tsmi_Remove,
            this.tsmi_Replace,
            this.tsmi_Display,
            this.tsmi_Enable,
            this.tsmi_Reset,
            this.tsmi_Reserve1});
            this.cms_pic_Needles.Name = "cms_pic_Needles";
            this.cms_pic_Needles.Size = new System.Drawing.Size(115, 158);
            this.cms_pic_Needles.Opened += new System.EventHandler(this.cms_pic_Needles_Opened);
            // 
            // tsmi_Place
            // 
            this.tsmi_Place.Enabled = false;
            this.tsmi_Place.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsmi_Place.Name = "tsmi_Place";
            this.tsmi_Place.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Place.Text = "植針";
            // 
            // tsmi_Remove
            // 
            this.tsmi_Remove.Enabled = false;
            this.tsmi_Remove.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsmi_Remove.Name = "tsmi_Remove";
            this.tsmi_Remove.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Remove.Text = "取針";
            // 
            // tsmi_Replace
            // 
            this.tsmi_Replace.Enabled = false;
            this.tsmi_Replace.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsmi_Replace.Name = "tsmi_Replace";
            this.tsmi_Replace.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Replace.Text = "置換";
            // 
            // tsmi_Display
            // 
            this.tsmi_Display.Enabled = false;
            this.tsmi_Display.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsmi_Display.Name = "tsmi_Display";
            this.tsmi_Display.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Display.Text = "顯示";
            // 
            // tsmi_Enable
            // 
            this.tsmi_Enable.Enabled = false;
            this.tsmi_Enable.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsmi_Enable.Name = "tsmi_Enable";
            this.tsmi_Enable.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Enable.Text = "禁用";
            // 
            // tsmi_Reset
            // 
            this.tsmi_Reset.Enabled = false;
            this.tsmi_Reset.Name = "tsmi_Reset";
            this.tsmi_Reset.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Reset.Text = "清除";
            // 
            // tsmi_Reserve1
            // 
            this.tsmi_Reserve1.Enabled = false;
            this.tsmi_Reserve1.Name = "tsmi_Reserve1";
            this.tsmi_Reserve1.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Reserve1.Text = "保留";
            // 
            // tp_NeedlesJudge
            // 
            this.tp_NeedlesJudge.Controls.Add(this.label11);
            this.tp_NeedlesJudge.Controls.Add(this.label12);
            this.tp_NeedlesJudge.Controls.Add(this.pictureBox2);
            this.tp_NeedlesJudge.Controls.Add(this.pictureBox1);
            this.tp_NeedlesJudge.Location = new System.Drawing.Point(4, 26);
            this.tp_NeedlesJudge.Name = "tp_NeedlesJudge";
            this.tp_NeedlesJudge.Padding = new System.Windows.Forms.Padding(3);
            this.tp_NeedlesJudge.Size = new System.Drawing.Size(601, 605);
            this.tp_NeedlesJudge.TabIndex = 1;
            this.tp_NeedlesJudge.Text = "判等";
            this.tp_NeedlesJudge.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(427, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 25);
            this.label11.TabIndex = 3;
            this.label11.Text = "NG";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(116, 1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 25);
            this.label12.TabIndex = 2;
            this.label12.Text = "Pass";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Honeydew;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Location = new System.Drawing.Point(307, 33);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(294, 568);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Honeydew;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(0, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(301, 567);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // grp_目前作業項目
            // 
            this.grp_目前作業項目.Controls.Add(this.grp_GroupPin2);
            this.grp_目前作業項目.Controls.Add(this.grp_GroupPin1);
            this.grp_目前作業項目.Controls.Add(this.rad_ChangeGroupPin);
            this.grp_目前作業項目.Controls.Add(this.rad_ChangeAllNewPin);
            this.grp_目前作業項目.Controls.Add(this.txt_PogoPin2已植數量);
            this.grp_目前作業項目.Controls.Add(this.lbl_PogoPin2已植數量);
            this.grp_目前作業項目.Controls.Add(this.txt_PogoPin1已植數量);
            this.grp_目前作業項目.Controls.Add(this.lbl_PogoPin1已植數量);
            this.grp_目前作業項目.Controls.Add(this.txt_PogoPin2Qty2);
            this.grp_目前作業項目.Controls.Add(this.lbl_PogoPin2Qty2);
            this.grp_目前作業項目.Controls.Add(this.txt_PogoPin1Qty2);
            this.grp_目前作業項目.Controls.Add(this.lbl_PogoPin1Qty2);
            this.grp_目前作業項目.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grp_目前作業項目.Location = new System.Drawing.Point(361, 37);
            this.grp_目前作業項目.Name = "grp_目前作業項目";
            this.grp_目前作業項目.Size = new System.Drawing.Size(239, 562);
            this.grp_目前作業項目.TabIndex = 22;
            this.grp_目前作業項目.TabStop = false;
            this.grp_目前作業項目.Text = "目前作業項目";
            // 
            // grp_GroupPin2
            // 
            this.grp_GroupPin2.Controls.Add(this.txt_PogoPin2已植數量2);
            this.grp_GroupPin2.Controls.Add(this.lbl_PogoPin2已植數量2);
            this.grp_GroupPin2.Controls.Add(this.txt_PogoPin1已植數量2);
            this.grp_GroupPin2.Controls.Add(this.lbl_PogoPin1已植數量2);
            this.grp_GroupPin2.Controls.Add(this.txt_PogoPin2Qty4);
            this.grp_GroupPin2.Controls.Add(this.lbl_PogoPin2Qty4);
            this.grp_GroupPin2.Controls.Add(this.txt_PogoPin1Qty4);
            this.grp_GroupPin2.Controls.Add(this.lbl_PogoPin1Qty4);
            this.grp_GroupPin2.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grp_GroupPin2.Location = new System.Drawing.Point(7, 378);
            this.grp_GroupPin2.Name = "grp_GroupPin2";
            this.grp_GroupPin2.Size = new System.Drawing.Size(226, 164);
            this.grp_GroupPin2.TabIndex = 26;
            this.grp_GroupPin2.TabStop = false;
            this.grp_GroupPin2.Text = "Group Pin 2";
            // 
            // txt_PogoPin2已植數量2
            // 
            this.txt_PogoPin2已植數量2.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2已植數量2.Location = new System.Drawing.Point(140, 122);
            this.txt_PogoPin2已植數量2.Name = "txt_PogoPin2已植數量2";
            this.txt_PogoPin2已植數量2.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin2已植數量2.TabIndex = 30;
            // 
            // lbl_PogoPin2已植數量2
            // 
            this.lbl_PogoPin2已植數量2.AutoSize = true;
            this.lbl_PogoPin2已植數量2.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2已植數量2.Location = new System.Drawing.Point(50, 126);
            this.lbl_PogoPin2已植數量2.Name = "lbl_PogoPin2已植數量2";
            this.lbl_PogoPin2已植數量2.Size = new System.Drawing.Size(63, 14);
            this.lbl_PogoPin2已植數量2.TabIndex = 29;
            this.lbl_PogoPin2已植數量2.Text = "已植數量";
            // 
            // txt_PogoPin1已植數量2
            // 
            this.txt_PogoPin1已植數量2.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1已植數量2.Location = new System.Drawing.Point(140, 56);
            this.txt_PogoPin1已植數量2.Name = "txt_PogoPin1已植數量2";
            this.txt_PogoPin1已植數量2.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin1已植數量2.TabIndex = 28;
            // 
            // lbl_PogoPin1已植數量2
            // 
            this.lbl_PogoPin1已植數量2.AutoSize = true;
            this.lbl_PogoPin1已植數量2.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1已植數量2.Location = new System.Drawing.Point(50, 60);
            this.lbl_PogoPin1已植數量2.Name = "lbl_PogoPin1已植數量2";
            this.lbl_PogoPin1已植數量2.Size = new System.Drawing.Size(63, 14);
            this.lbl_PogoPin1已植數量2.TabIndex = 27;
            this.lbl_PogoPin1已植數量2.Text = "已植數量";
            // 
            // txt_PogoPin2Qty4
            // 
            this.txt_PogoPin2Qty4.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2Qty4.Location = new System.Drawing.Point(141, 89);
            this.txt_PogoPin2Qty4.Name = "txt_PogoPin2Qty4";
            this.txt_PogoPin2Qty4.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin2Qty4.TabIndex = 26;
            // 
            // lbl_PogoPin2Qty4
            // 
            this.lbl_PogoPin2Qty4.AutoSize = true;
            this.lbl_PogoPin2Qty4.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2Qty4.Location = new System.Drawing.Point(5, 93);
            this.lbl_PogoPin2Qty4.Name = "lbl_PogoPin2Qty4";
            this.lbl_PogoPin2Qty4.Size = new System.Drawing.Size(98, 14);
            this.lbl_PogoPin2Qty4.TabIndex = 25;
            this.lbl_PogoPin2Qty4.Text = "Pogo Pin2 Qty";
            // 
            // txt_PogoPin1Qty4
            // 
            this.txt_PogoPin1Qty4.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1Qty4.Location = new System.Drawing.Point(141, 23);
            this.txt_PogoPin1Qty4.Name = "txt_PogoPin1Qty4";
            this.txt_PogoPin1Qty4.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin1Qty4.TabIndex = 24;
            // 
            // lbl_PogoPin1Qty4
            // 
            this.lbl_PogoPin1Qty4.AutoSize = true;
            this.lbl_PogoPin1Qty4.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1Qty4.Location = new System.Drawing.Point(7, 27);
            this.lbl_PogoPin1Qty4.Name = "lbl_PogoPin1Qty4";
            this.lbl_PogoPin1Qty4.Size = new System.Drawing.Size(98, 14);
            this.lbl_PogoPin1Qty4.TabIndex = 23;
            this.lbl_PogoPin1Qty4.Text = "Pogo Pin1 Qty";
            // 
            // grp_GroupPin1
            // 
            this.grp_GroupPin1.Controls.Add(this.txt_PogoPin2已植數量1);
            this.grp_GroupPin1.Controls.Add(this.lbl_PogoPin2已植數量1);
            this.grp_GroupPin1.Controls.Add(this.txt_PogoPin1已植數量1);
            this.grp_GroupPin1.Controls.Add(this.lbl_PogoPin1已植數量1);
            this.grp_GroupPin1.Controls.Add(this.txt_PogoPin2Qty3);
            this.grp_GroupPin1.Controls.Add(this.lbl_PogoPin2Qty3);
            this.grp_GroupPin1.Controls.Add(this.txt_PogoPin1Qty3);
            this.grp_GroupPin1.Controls.Add(this.lbl_PogoPin1Qty3);
            this.grp_GroupPin1.Font = new System.Drawing.Font("標楷體", 10F);
            this.grp_GroupPin1.Location = new System.Drawing.Point(6, 216);
            this.grp_GroupPin1.Name = "grp_GroupPin1";
            this.grp_GroupPin1.Size = new System.Drawing.Size(226, 156);
            this.grp_GroupPin1.TabIndex = 25;
            this.grp_GroupPin1.TabStop = false;
            this.grp_GroupPin1.Text = "Group Pin 1";
            // 
            // txt_PogoPin2已植數量1
            // 
            this.txt_PogoPin2已植數量1.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2已植數量1.Location = new System.Drawing.Point(141, 123);
            this.txt_PogoPin2已植數量1.Name = "txt_PogoPin2已植數量1";
            this.txt_PogoPin2已植數量1.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin2已植數量1.TabIndex = 30;
            // 
            // lbl_PogoPin2已植數量1
            // 
            this.lbl_PogoPin2已植數量1.AutoSize = true;
            this.lbl_PogoPin2已植數量1.Font = new System.Drawing.Font("標楷體", 10F);
            this.lbl_PogoPin2已植數量1.Location = new System.Drawing.Point(51, 129);
            this.lbl_PogoPin2已植數量1.Name = "lbl_PogoPin2已植數量1";
            this.lbl_PogoPin2已植數量1.Size = new System.Drawing.Size(63, 14);
            this.lbl_PogoPin2已植數量1.TabIndex = 29;
            this.lbl_PogoPin2已植數量1.Text = "已植數量";
            // 
            // txt_PogoPin1已植數量1
            // 
            this.txt_PogoPin1已植數量1.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1已植數量1.Location = new System.Drawing.Point(141, 57);
            this.txt_PogoPin1已植數量1.Name = "txt_PogoPin1已植數量1";
            this.txt_PogoPin1已植數量1.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin1已植數量1.TabIndex = 28;
            // 
            // lbl_PogoPin1已植數量1
            // 
            this.lbl_PogoPin1已植數量1.AutoSize = true;
            this.lbl_PogoPin1已植數量1.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1已植數量1.Location = new System.Drawing.Point(51, 61);
            this.lbl_PogoPin1已植數量1.Name = "lbl_PogoPin1已植數量1";
            this.lbl_PogoPin1已植數量1.Size = new System.Drawing.Size(63, 14);
            this.lbl_PogoPin1已植數量1.TabIndex = 27;
            this.lbl_PogoPin1已植數量1.Text = "已植數量";
            // 
            // txt_PogoPin2Qty3
            // 
            this.txt_PogoPin2Qty3.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2Qty3.Location = new System.Drawing.Point(141, 90);
            this.txt_PogoPin2Qty3.Name = "txt_PogoPin2Qty3";
            this.txt_PogoPin2Qty3.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin2Qty3.TabIndex = 26;
            // 
            // lbl_PogoPin2Qty3
            // 
            this.lbl_PogoPin2Qty3.AutoSize = true;
            this.lbl_PogoPin2Qty3.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2Qty3.Location = new System.Drawing.Point(6, 99);
            this.lbl_PogoPin2Qty3.Name = "lbl_PogoPin2Qty3";
            this.lbl_PogoPin2Qty3.Size = new System.Drawing.Size(98, 14);
            this.lbl_PogoPin2Qty3.TabIndex = 25;
            this.lbl_PogoPin2Qty3.Text = "Pogo Pin2 Qty";
            // 
            // txt_PogoPin1Qty3
            // 
            this.txt_PogoPin1Qty3.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1Qty3.Location = new System.Drawing.Point(141, 22);
            this.txt_PogoPin1Qty3.Name = "txt_PogoPin1Qty3";
            this.txt_PogoPin1Qty3.Size = new System.Drawing.Size(77, 23);
            this.txt_PogoPin1Qty3.TabIndex = 24;
            // 
            // lbl_PogoPin1Qty3
            // 
            this.lbl_PogoPin1Qty3.AutoSize = true;
            this.lbl_PogoPin1Qty3.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1Qty3.Location = new System.Drawing.Point(6, 28);
            this.lbl_PogoPin1Qty3.Name = "lbl_PogoPin1Qty3";
            this.lbl_PogoPin1Qty3.Size = new System.Drawing.Size(98, 14);
            this.lbl_PogoPin1Qty3.TabIndex = 23;
            this.lbl_PogoPin1Qty3.Text = "Pogo Pin1 Qty";
            // 
            // rad_ChangeGroupPin
            // 
            this.rad_ChangeGroupPin.AutoSize = true;
            this.rad_ChangeGroupPin.Font = new System.Drawing.Font("標楷體", 10F);
            this.rad_ChangeGroupPin.Location = new System.Drawing.Point(6, 189);
            this.rad_ChangeGroupPin.Name = "rad_ChangeGroupPin";
            this.rad_ChangeGroupPin.Size = new System.Drawing.Size(137, 18);
            this.rad_ChangeGroupPin.TabIndex = 24;
            this.rad_ChangeGroupPin.TabStop = true;
            this.rad_ChangeGroupPin.Text = "Change Group Pin";
            this.rad_ChangeGroupPin.UseVisualStyleBackColor = true;
            // 
            // rad_ChangeAllNewPin
            // 
            this.rad_ChangeAllNewPin.AutoSize = true;
            this.rad_ChangeAllNewPin.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rad_ChangeAllNewPin.Location = new System.Drawing.Point(6, 26);
            this.rad_ChangeAllNewPin.Name = "rad_ChangeAllNewPin";
            this.rad_ChangeAllNewPin.Size = new System.Drawing.Size(151, 18);
            this.rad_ChangeAllNewPin.TabIndex = 23;
            this.rad_ChangeAllNewPin.TabStop = true;
            this.rad_ChangeAllNewPin.Text = "Change All New Pin";
            this.rad_ChangeAllNewPin.UseVisualStyleBackColor = true;
            // 
            // txt_PogoPin2已植數量
            // 
            this.txt_PogoPin2已植數量.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2已植數量.Location = new System.Drawing.Point(152, 156);
            this.txt_PogoPin2已植數量.Name = "txt_PogoPin2已植數量";
            this.txt_PogoPin2已植數量.Size = new System.Drawing.Size(72, 23);
            this.txt_PogoPin2已植數量.TabIndex = 22;
            // 
            // lbl_PogoPin2已植數量
            // 
            this.lbl_PogoPin2已植數量.AutoSize = true;
            this.lbl_PogoPin2已植數量.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2已植數量.Location = new System.Drawing.Point(66, 160);
            this.lbl_PogoPin2已植數量.Name = "lbl_PogoPin2已植數量";
            this.lbl_PogoPin2已植數量.Size = new System.Drawing.Size(63, 14);
            this.lbl_PogoPin2已植數量.TabIndex = 21;
            this.lbl_PogoPin2已植數量.Text = "已植數量";
            // 
            // txt_PogoPin1已植數量
            // 
            this.txt_PogoPin1已植數量.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1已植數量.Location = new System.Drawing.Point(152, 90);
            this.txt_PogoPin1已植數量.Name = "txt_PogoPin1已植數量";
            this.txt_PogoPin1已植數量.Size = new System.Drawing.Size(72, 23);
            this.txt_PogoPin1已植數量.TabIndex = 20;
            // 
            // lbl_PogoPin1已植數量
            // 
            this.lbl_PogoPin1已植數量.AutoSize = true;
            this.lbl_PogoPin1已植數量.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1已植數量.Location = new System.Drawing.Point(66, 94);
            this.lbl_PogoPin1已植數量.Name = "lbl_PogoPin1已植數量";
            this.lbl_PogoPin1已植數量.Size = new System.Drawing.Size(63, 14);
            this.lbl_PogoPin1已植數量.TabIndex = 19;
            this.lbl_PogoPin1已植數量.Text = "已植數量";
            // 
            // txt_PogoPin2Qty2
            // 
            this.txt_PogoPin2Qty2.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2Qty2.Location = new System.Drawing.Point(152, 123);
            this.txt_PogoPin2Qty2.Name = "txt_PogoPin2Qty2";
            this.txt_PogoPin2Qty2.Size = new System.Drawing.Size(72, 23);
            this.txt_PogoPin2Qty2.TabIndex = 15;
            // 
            // lbl_PogoPin2Qty2
            // 
            this.lbl_PogoPin2Qty2.AutoSize = true;
            this.lbl_PogoPin2Qty2.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2Qty2.Location = new System.Drawing.Point(18, 127);
            this.lbl_PogoPin2Qty2.Name = "lbl_PogoPin2Qty2";
            this.lbl_PogoPin2Qty2.Size = new System.Drawing.Size(98, 14);
            this.lbl_PogoPin2Qty2.TabIndex = 14;
            this.lbl_PogoPin2Qty2.Text = "Pogo Pin2 Qty";
            // 
            // txt_PogoPin1Qty2
            // 
            this.txt_PogoPin1Qty2.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1Qty2.Location = new System.Drawing.Point(152, 58);
            this.txt_PogoPin1Qty2.Name = "txt_PogoPin1Qty2";
            this.txt_PogoPin1Qty2.Size = new System.Drawing.Size(74, 23);
            this.txt_PogoPin1Qty2.TabIndex = 13;
            // 
            // lbl_PogoPin1Qty2
            // 
            this.lbl_PogoPin1Qty2.AutoSize = true;
            this.lbl_PogoPin1Qty2.Font = new System.Drawing.Font("標楷體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1Qty2.Location = new System.Drawing.Point(18, 64);
            this.lbl_PogoPin1Qty2.Name = "lbl_PogoPin1Qty2";
            this.lbl_PogoPin1Qty2.Size = new System.Drawing.Size(98, 14);
            this.lbl_PogoPin1Qty2.TabIndex = 12;
            this.lbl_PogoPin1Qty2.Text = "Pogo Pin1 Qty";
            // 
            // grp_SocketInfo
            // 
            this.grp_SocketInfo.Controls.Add(this.grp_設備治具資訊);
            this.grp_SocketInfo.Controls.Add(this.grp_BarcodeInfo);
            this.grp_SocketInfo.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grp_SocketInfo.Location = new System.Drawing.Point(14, 37);
            this.grp_SocketInfo.Name = "grp_SocketInfo";
            this.grp_SocketInfo.Size = new System.Drawing.Size(341, 562);
            this.grp_SocketInfo.TabIndex = 21;
            this.grp_SocketInfo.TabStop = false;
            this.grp_SocketInfo.Text = "Socket資訊";
            // 
            // grp_設備治具資訊
            // 
            this.grp_設備治具資訊.Controls.Add(this.lbl_PogoPin3Qty1);
            this.grp_設備治具資訊.Controls.Add(this.lbl_PogoPin2Qty1);
            this.grp_設備治具資訊.Controls.Add(this.lbl_PogoPin1Qty1);
            this.grp_設備治具資訊.Controls.Add(this.txt_下針導正模組);
            this.grp_設備治具資訊.Controls.Add(this.lbl_下針導正模組);
            this.grp_設備治具資訊.Controls.Add(this.txt_取針模組PI);
            this.grp_設備治具資訊.Controls.Add(this.lbl_取針模組PI);
            this.grp_設備治具資訊.Controls.Add(this.txt_PogoPin3Qty1);
            this.grp_設備治具資訊.Controls.Add(this.txt_PogoPin2Qty1);
            this.grp_設備治具資訊.Controls.Add(this.txt_PogoPin1Qty1);
            this.grp_設備治具資訊.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grp_設備治具資訊.Location = new System.Drawing.Point(7, 338);
            this.grp_設備治具資訊.Name = "grp_設備治具資訊";
            this.grp_設備治具資訊.Size = new System.Drawing.Size(328, 208);
            this.grp_設備治具資訊.TabIndex = 1;
            this.grp_設備治具資訊.TabStop = false;
            this.grp_設備治具資訊.Text = "設備治具資訊";
            // 
            // lbl_PogoPin3Qty1
            // 
            this.lbl_PogoPin3Qty1.AutoSize = true;
            this.lbl_PogoPin3Qty1.Font = new System.Drawing.Font("標楷體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin3Qty1.Location = new System.Drawing.Point(6, 102);
            this.lbl_PogoPin3Qty1.Name = "lbl_PogoPin3Qty1";
            this.lbl_PogoPin3Qty1.Size = new System.Drawing.Size(87, 15);
            this.lbl_PogoPin3Qty1.TabIndex = 16;
            this.lbl_PogoPin3Qty1.Text = "Probe Bore";
            // 
            // lbl_PogoPin2Qty1
            // 
            this.lbl_PogoPin2Qty1.AutoSize = true;
            this.lbl_PogoPin2Qty1.Font = new System.Drawing.Font("標楷體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2Qty1.Location = new System.Drawing.Point(6, 66);
            this.lbl_PogoPin2Qty1.Name = "lbl_PogoPin2Qty1";
            this.lbl_PogoPin2Qty1.Size = new System.Drawing.Size(95, 15);
            this.lbl_PogoPin2Qty1.TabIndex = 15;
            this.lbl_PogoPin2Qty1.Text = "Probe Plate";
            // 
            // lbl_PogoPin1Qty1
            // 
            this.lbl_PogoPin1Qty1.AutoSize = true;
            this.lbl_PogoPin1Qty1.Font = new System.Drawing.Font("標楷體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1Qty1.Location = new System.Drawing.Point(7, 29);
            this.lbl_PogoPin1Qty1.Name = "lbl_PogoPin1Qty1";
            this.lbl_PogoPin1Qty1.Size = new System.Drawing.Size(71, 15);
            this.lbl_PogoPin1Qty1.TabIndex = 14;
            this.lbl_PogoPin1Qty1.Text = "VCM_Pick";
            // 
            // txt_下針導正模組
            // 
            this.txt_下針導正模組.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_下針導正模組.Location = new System.Drawing.Point(128, 168);
            this.txt_下針導正模組.Name = "txt_下針導正模組";
            this.txt_下針導正模組.Size = new System.Drawing.Size(194, 26);
            this.txt_下針導正模組.TabIndex = 9;
            this.txt_下針導正模組.Text = "RL20240703-SA01-B001-A";
            // 
            // lbl_下針導正模組
            // 
            this.lbl_下針導正模組.AutoSize = true;
            this.lbl_下針導正模組.Font = new System.Drawing.Font("標楷體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_下針導正模組.Location = new System.Drawing.Point(6, 174);
            this.lbl_下針導正模組.Name = "lbl_下針導正模組";
            this.lbl_下針導正模組.Size = new System.Drawing.Size(111, 15);
            this.lbl_下針導正模組.TabIndex = 8;
            this.lbl_下針導正模組.Text = "Probe Gripper";
            // 
            // txt_取針模組PI
            // 
            this.txt_取針模組PI.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_取針模組PI.Location = new System.Drawing.Point(128, 132);
            this.txt_取針模組PI.Name = "txt_取針模組PI";
            this.txt_取針模組PI.Size = new System.Drawing.Size(194, 26);
            this.txt_取針模組PI.TabIndex = 7;
            this.txt_取針模組PI.Text = "RL20240703-SA01-A001-A";
            // 
            // lbl_取針模組PI
            // 
            this.lbl_取針模組PI.AutoSize = true;
            this.lbl_取針模組PI.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_取針模組PI.Location = new System.Drawing.Point(6, 136);
            this.lbl_取針模組PI.Name = "lbl_取針模組PI";
            this.lbl_取針模組PI.Size = new System.Drawing.Size(95, 16);
            this.lbl_取針模組PI.TabIndex = 6;
            this.lbl_取針模組PI.Text = "Socket Tray";
            // 
            // txt_PogoPin3Qty1
            // 
            this.txt_PogoPin3Qty1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin3Qty1.Location = new System.Drawing.Point(128, 96);
            this.txt_PogoPin3Qty1.Name = "txt_PogoPin3Qty1";
            this.txt_PogoPin3Qty1.Size = new System.Drawing.Size(194, 26);
            this.txt_PogoPin3Qty1.TabIndex = 5;
            this.txt_PogoPin3Qty1.Text = "RL20240703-SA01-D002-A";
            // 
            // txt_PogoPin2Qty1
            // 
            this.txt_PogoPin2Qty1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin2Qty1.Location = new System.Drawing.Point(128, 60);
            this.txt_PogoPin2Qty1.Name = "txt_PogoPin2Qty1";
            this.txt_PogoPin2Qty1.Size = new System.Drawing.Size(194, 26);
            this.txt_PogoPin2Qty1.TabIndex = 3;
            this.txt_PogoPin2Qty1.Text = "RL20240703-SA01-D001-A";
            // 
            // txt_PogoPin1Qty1
            // 
            this.txt_PogoPin1Qty1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PogoPin1Qty1.Location = new System.Drawing.Point(128, 23);
            this.txt_PogoPin1Qty1.Name = "txt_PogoPin1Qty1";
            this.txt_PogoPin1Qty1.Size = new System.Drawing.Size(194, 26);
            this.txt_PogoPin1Qty1.TabIndex = 1;
            this.txt_PogoPin1Qty1.Text = "RL20240703-SA01-C001-A";
            // 
            // grp_BarcodeInfo
            // 
            this.grp_BarcodeInfo.Controls.Add(this.btn_SaveFile);
            this.grp_BarcodeInfo.Controls.Add(this.btn_OpenFile);
            this.grp_BarcodeInfo.Controls.Add(this.txt_儲位);
            this.grp_BarcodeInfo.Controls.Add(this.lbl_PogoPin2Qty);
            this.grp_BarcodeInfo.Controls.Add(this.txt_板全號);
            this.grp_BarcodeInfo.Controls.Add(this.lbl_PogoPin1Qty);
            this.grp_BarcodeInfo.Controls.Add(this.txt_型號);
            this.grp_BarcodeInfo.Controls.Add(this.lbl_Socket定位座);
            this.grp_BarcodeInfo.Controls.Add(this.txt_客戶);
            this.grp_BarcodeInfo.Controls.Add(this.lbl_FileName);
            this.grp_BarcodeInfo.Controls.Add(this.txt_短編號);
            this.grp_BarcodeInfo.Controls.Add(this.lbl_Socket);
            this.grp_BarcodeInfo.Controls.Add(this.txt_Barcode);
            this.grp_BarcodeInfo.Controls.Add(this.lbl_條碼輸入欄位);
            this.grp_BarcodeInfo.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grp_BarcodeInfo.Location = new System.Drawing.Point(6, 29);
            this.grp_BarcodeInfo.Name = "grp_BarcodeInfo";
            this.grp_BarcodeInfo.Size = new System.Drawing.Size(329, 303);
            this.grp_BarcodeInfo.TabIndex = 0;
            this.grp_BarcodeInfo.TabStop = false;
            this.grp_BarcodeInfo.Text = "配件條碼資訊";
            // 
            // btn_SaveFile
            // 
            this.btn_SaveFile.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_SaveFile.Enabled = false;
            this.btn_SaveFile.Font = new System.Drawing.Font("標楷體", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_SaveFile.Location = new System.Drawing.Point(10, 244);
            this.btn_SaveFile.Name = "btn_SaveFile";
            this.btn_SaveFile.Size = new System.Drawing.Size(158, 53);
            this.btn_SaveFile.TabIndex = 13;
            this.btn_SaveFile.Text = "更新";
            this.btn_SaveFile.UseVisualStyleBackColor = false;
            this.btn_SaveFile.Click += new System.EventHandler(this.btn_SaveFile_Click);
            // 
            // btn_OpenFile
            // 
            this.btn_OpenFile.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_OpenFile.Font = new System.Drawing.Font("標楷體", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_OpenFile.Location = new System.Drawing.Point(174, 244);
            this.btn_OpenFile.Name = "btn_OpenFile";
            this.btn_OpenFile.Size = new System.Drawing.Size(149, 53);
            this.btn_OpenFile.TabIndex = 12;
            this.btn_OpenFile.Text = "查詢";
            this.btn_OpenFile.UseVisualStyleBackColor = false;
            this.btn_OpenFile.Click += new System.EventHandler(this.btn_OpenFile_Click);
            // 
            // txt_儲位
            // 
            this.txt_儲位.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_儲位.Location = new System.Drawing.Point(101, 208);
            this.txt_儲位.MaxLength = 256;
            this.txt_儲位.Name = "txt_儲位";
            this.txt_儲位.Size = new System.Drawing.Size(222, 26);
            this.txt_儲位.TabIndex = 11;
            // 
            // lbl_PogoPin2Qty
            // 
            this.lbl_PogoPin2Qty.AutoSize = true;
            this.lbl_PogoPin2Qty.Font = new System.Drawing.Font("標楷體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin2Qty.Location = new System.Drawing.Point(7, 214);
            this.lbl_PogoPin2Qty.Name = "lbl_PogoPin2Qty";
            this.lbl_PogoPin2Qty.Size = new System.Drawing.Size(39, 15);
            this.lbl_PogoPin2Qty.TabIndex = 10;
            this.lbl_PogoPin2Qty.Text = "儲位";
            // 
            // txt_板全號
            // 
            this.txt_板全號.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_板全號.Location = new System.Drawing.Point(101, 172);
            this.txt_板全號.MaxLength = 256;
            this.txt_板全號.Name = "txt_板全號";
            this.txt_板全號.Size = new System.Drawing.Size(222, 26);
            this.txt_板全號.TabIndex = 9;
            // 
            // lbl_PogoPin1Qty
            // 
            this.lbl_PogoPin1Qty.AutoSize = true;
            this.lbl_PogoPin1Qty.Font = new System.Drawing.Font("標楷體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_PogoPin1Qty.Location = new System.Drawing.Point(7, 178);
            this.lbl_PogoPin1Qty.Name = "lbl_PogoPin1Qty";
            this.lbl_PogoPin1Qty.Size = new System.Drawing.Size(55, 15);
            this.lbl_PogoPin1Qty.TabIndex = 8;
            this.lbl_PogoPin1Qty.Text = "板全號";
            // 
            // txt_型號
            // 
            this.txt_型號.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_型號.Location = new System.Drawing.Point(101, 136);
            this.txt_型號.MaxLength = 256;
            this.txt_型號.Name = "txt_型號";
            this.txt_型號.Size = new System.Drawing.Size(222, 26);
            this.txt_型號.TabIndex = 7;
            // 
            // lbl_Socket定位座
            // 
            this.lbl_Socket定位座.AutoSize = true;
            this.lbl_Socket定位座.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Socket定位座.Location = new System.Drawing.Point(6, 139);
            this.lbl_Socket定位座.Name = "lbl_Socket定位座";
            this.lbl_Socket定位座.Size = new System.Drawing.Size(39, 16);
            this.lbl_Socket定位座.TabIndex = 6;
            this.lbl_Socket定位座.Text = "型號";
            // 
            // txt_客戶
            // 
            this.txt_客戶.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_客戶.Location = new System.Drawing.Point(101, 100);
            this.txt_客戶.MaxLength = 256;
            this.txt_客戶.Name = "txt_客戶";
            this.txt_客戶.Size = new System.Drawing.Size(222, 27);
            this.txt_客戶.TabIndex = 5;
            // 
            // lbl_FileName
            // 
            this.lbl_FileName.AutoSize = true;
            this.lbl_FileName.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_FileName.Location = new System.Drawing.Point(6, 104);
            this.lbl_FileName.Name = "lbl_FileName";
            this.lbl_FileName.Size = new System.Drawing.Size(39, 16);
            this.lbl_FileName.TabIndex = 4;
            this.lbl_FileName.Text = "客戶";
            // 
            // txt_短編號
            // 
            this.txt_短編號.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_短編號.Location = new System.Drawing.Point(101, 64);
            this.txt_短編號.MaxLength = 256;
            this.txt_短編號.Name = "txt_短編號";
            this.txt_短編號.Size = new System.Drawing.Size(222, 26);
            this.txt_短編號.TabIndex = 3;
            // 
            // lbl_Socket
            // 
            this.lbl_Socket.AutoSize = true;
            this.lbl_Socket.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_Socket.Location = new System.Drawing.Point(6, 68);
            this.lbl_Socket.Name = "lbl_Socket";
            this.lbl_Socket.Size = new System.Drawing.Size(55, 16);
            this.lbl_Socket.TabIndex = 2;
            this.lbl_Socket.Text = "短編號";
            // 
            // txt_Barcode
            // 
            this.txt_Barcode.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Barcode.Location = new System.Drawing.Point(101, 23);
            this.txt_Barcode.MaxLength = 256;
            this.txt_Barcode.Name = "txt_Barcode";
            this.txt_Barcode.Size = new System.Drawing.Size(222, 26);
            this.txt_Barcode.TabIndex = 1;
            // 
            // lbl_條碼輸入欄位
            // 
            this.lbl_條碼輸入欄位.AutoSize = true;
            this.lbl_條碼輸入欄位.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_條碼輸入欄位.Location = new System.Drawing.Point(6, 27);
            this.lbl_條碼輸入欄位.Name = "lbl_條碼輸入欄位";
            this.lbl_條碼輸入欄位.Size = new System.Drawing.Size(71, 16);
            this.lbl_條碼輸入欄位.TabIndex = 0;
            this.lbl_條碼輸入欄位.Text = "配件編號";
            // 
            // grp_儲存資訊
            // 
            this.grp_儲存資訊.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.grp_儲存資訊.Controls.Add(this.btn_停止);
            this.grp_儲存資訊.Controls.Add(this.btn_開始);
            this.grp_儲存資訊.Location = new System.Drawing.Point(14, 605);
            this.grp_儲存資訊.Name = "grp_儲存資訊";
            this.grp_儲存資訊.Size = new System.Drawing.Size(251, 67);
            this.grp_儲存資訊.TabIndex = 20;
            this.grp_儲存資訊.TabStop = false;
            // 
            // btn_停止
            // 
            this.btn_停止.BackColor = System.Drawing.Color.Red;
            this.btn_停止.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_停止.Location = new System.Drawing.Point(135, 9);
            this.btn_停止.Name = "btn_停止";
            this.btn_停止.Size = new System.Drawing.Size(109, 52);
            this.btn_停止.TabIndex = 2;
            this.btn_停止.Text = "停止";
            this.btn_停止.UseVisualStyleBackColor = false;
            // 
            // btn_開始
            // 
            this.btn_開始.BackColor = System.Drawing.Color.Lime;
            this.btn_開始.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_開始.Location = new System.Drawing.Point(6, 9);
            this.btn_開始.Name = "btn_開始";
            this.btn_開始.Size = new System.Drawing.Size(109, 52);
            this.btn_開始.TabIndex = 1;
            this.btn_開始.Text = "開始";
            this.btn_開始.UseVisualStyleBackColor = false;
            // 
            // ms_Viewer
            // 
            this.ms_Viewer.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ms_Viewer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_Account,
            this.tsm_File});
            this.ms_Viewer.Location = new System.Drawing.Point(0, 0);
            this.ms_Viewer.Name = "ms_Viewer";
            this.ms_Viewer.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.ms_Viewer.Size = new System.Drawing.Size(1228, 24);
            this.ms_Viewer.TabIndex = 19;
            this.ms_Viewer.Text = "menuStrip1";
            // 
            // tsm_Account
            // 
            this.tsm_Account.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Login,
            this.tsmi_Logout});
            this.tsm_Account.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsm_Account.Name = "tsm_Account";
            this.tsm_Account.Size = new System.Drawing.Size(51, 20);
            this.tsm_Account.Text = "帳戶";
            // 
            // tsmi_Login
            // 
            this.tsmi_Login.Name = "tsmi_Login";
            this.tsmi_Login.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Login.Text = "登入";
            // 
            // tsmi_Logout
            // 
            this.tsmi_Logout.Name = "tsmi_Logout";
            this.tsmi_Logout.Size = new System.Drawing.Size(114, 22);
            this.tsmi_Logout.Text = "登出";
            // 
            // tsm_File
            // 
            this.tsm_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_OpenFile,
            this.tsmi_SaveFile,
            this.tsmi_CloseFile});
            this.tsm_File.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsm_File.Name = "tsm_File";
            this.tsm_File.Size = new System.Drawing.Size(51, 20);
            this.tsm_File.Text = "檔案";
            // 
            // tsmi_OpenFile
            // 
            this.tsmi_OpenFile.Name = "tsmi_OpenFile";
            this.tsmi_OpenFile.Size = new System.Drawing.Size(114, 22);
            this.tsmi_OpenFile.Text = "開啟";
            this.tsmi_OpenFile.Click += new System.EventHandler(this.tsmi_OpenFile_Click);
            // 
            // tsmi_SaveFile
            // 
            this.tsmi_SaveFile.Enabled = false;
            this.tsmi_SaveFile.Name = "tsmi_SaveFile";
            this.tsmi_SaveFile.Size = new System.Drawing.Size(114, 22);
            this.tsmi_SaveFile.Text = "儲存";
            this.tsmi_SaveFile.Click += new System.EventHandler(this.tsmi_SaveFile_Click);
            // 
            // tsmi_CloseFile
            // 
            this.tsmi_CloseFile.Name = "tsmi_CloseFile";
            this.tsmi_CloseFile.Size = new System.Drawing.Size(114, 22);
            this.tsmi_CloseFile.Text = "關閉";
            this.tsmi_CloseFile.Click += new System.EventHandler(this.tsmi_CloseFile_Click);
            // 
            // tmr_Home
            // 
            this.tmr_Home.Enabled = true;
            this.tmr_Home.Tick += new System.EventHandler(this.tmr_Home_Tick);
            // 
            // tmr_TakePin
            // 
            this.tmr_TakePin.Enabled = true;
            this.tmr_TakePin.Interval = 5;
            this.tmr_TakePin.Tick += new System.EventHandler(this.tmr_TakePin_Tick);
            // 
            // tmr_Warning
            // 
            this.tmr_Warning.Enabled = true;
            this.tmr_Warning.Interval = 300;
            this.tmr_Warning.Tick += new System.EventHandler(this.tmr_Buzzer_Tick);
            // 
            // tmrBarCodeScanner
            // 
            this.tmrBarCodeScanner.Enabled = true;
            this.tmrBarCodeScanner.Interval = 10;
            this.tmrBarCodeScanner.Tick += new System.EventHandler(this.tmrBarCodeScanner_Tick);
            // 
            // tmr_2p_Calibration
            // 
            this.tmr_2p_Calibration.Enabled = true;
            this.tmr_2p_Calibration.Tick += new System.EventHandler(this.tmr_2p_Calibration_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1256, 847);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabJob.ResumeLayout(false);
            this.tabJob.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.grp_NeedleInfo.ResumeLayout(false);
            this.grp_NeedleInfo.PerformLayout();
            this.tab_Needles.ResumeLayout(false);
            this.tp_Needles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Needles)).EndInit();
            this.cms_pic_Needles.ResumeLayout(false);
            this.tp_NeedlesJudge.ResumeLayout(false);
            this.tp_NeedlesJudge.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grp_目前作業項目.ResumeLayout(false);
            this.grp_目前作業項目.PerformLayout();
            this.grp_GroupPin2.ResumeLayout(false);
            this.grp_GroupPin2.PerformLayout();
            this.grp_GroupPin1.ResumeLayout(false);
            this.grp_GroupPin1.PerformLayout();
            this.grp_SocketInfo.ResumeLayout(false);
            this.grp_設備治具資訊.ResumeLayout(false);
            this.grp_設備治具資訊.PerformLayout();
            this.grp_BarcodeInfo.ResumeLayout(false);
            this.grp_BarcodeInfo.PerformLayout();
            this.grp_儲存資訊.ResumeLayout(false);
            this.ms_Viewer.ResumeLayout(false);
            this.ms_Viewer.PerformLayout();
            this.ResumeLayout(false);

        }

        public void Initialize_grp_NeedleInfo_ChildControlChanged_Listener(GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.TextChanged += grp_NeedleInfo_ChildControlChanged;
                        textBox.KeyPress += grp_NeedleInfo_Search; //20250110 4xuan added : 新增 Enter 查詢
                        break;

                    case RadioButton radioButton:
                        radioButton.CheckedChanged += grp_NeedleInfo_ChildControlChanged;
                        break;

                    case CheckBox checkBox:
                        checkBox.Click += grp_NeedleInfo_ChildControlChanged;
                        break;

                    case Button button:
                        button.Click += grp_NeedleInfo_ChildControlChanged;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Initialize_grp_BarcodeInfo_ChildControlChanged_Listener(GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.TextChanged += grp_BarcodeInfo_ChildControlChanged;
                        break;

                    default:
                        break;
                }
            }
        }

        public void Initialize_cms_pic_Needles_ItemClicked_Listener(ContextMenuStrip contextMenuStrip)
        {
            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                item.Click += cms_pic_Needles_ItemClicked;
            }
        }

        #endregion
        public System.Windows.Forms.Timer tmr_ReadWMX3;
        public System.Windows.Forms.TabPage tabJob;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label lblBuzzer;
        public System.Windows.Forms.Label lbl_NA_31;
        public System.Windows.Forms.Label lbl綠燈;
        public System.Windows.Forms.Label lbl左按鈕紅燈;
        public System.Windows.Forms.Label lbl黃燈;
        public System.Windows.Forms.Label lbl中按鈕綠燈;
        public System.Windows.Forms.Label lbl紅燈;
        public System.Windows.Forms.Label lbl右按鈕綠燈;
        public System.Windows.Forms.Label lbl艙內燈;
        public System.Windows.Forms.Label lbl取料吸嘴破新;
        public System.Windows.Forms.Label lblHEPA;
        public System.Windows.Forms.Label lbl植針Z煞車;
        public System.Windows.Forms.Label lbl下後右門鎖;
        public System.Windows.Forms.Label lbl取料吸嘴破舊;
        public System.Windows.Forms.Label lbl下後左門鎖;
        public System.Windows.Forms.Label lbl取料吸嘴吸;
        public System.Windows.Forms.Label lbl擺放破真空;
        public System.Windows.Forms.Label lblsk破真空1;
        public System.Windows.Forms.Label lbl擺放座真空;
        public System.Windows.Forms.Label lblsk真空1;
        public System.Windows.Forms.Label lblsk破真空2;
        public System.Windows.Forms.Label lbl載盤破真空;
        public System.Windows.Forms.Label lblsk真空2;
        public System.Windows.Forms.Label lbl載盤真空閥;
        public System.Windows.Forms.Label lbl_NA_25;
        public System.Windows.Forms.Label lbl堵料吹氣;
        public System.Windows.Forms.Label lbl收料區缸;
        public System.Windows.Forms.Label lbl植針吹氣;
        public System.Windows.Forms.Label lbl接料區缸;
        public System.Windows.Forms.Label lbl堵料吹氣缸;
        public System.Windows.Forms.Label lbl吸料真空閥;
        public System.Windows.Forms.Label lbl擺放蓋板;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label lbl_NA_24;
        public System.Windows.Forms.Label lbl下右左門;
        public System.Windows.Forms.Label lbl_NA_23;
        public System.Windows.Forms.Label lbl下右右門;
        public System.Windows.Forms.Label lbl下後右門;
        public System.Windows.Forms.Label lbl下左左門;
        public System.Windows.Forms.Label lbl下後左門;
        public System.Windows.Forms.Label lbl下左右門;
        public System.Windows.Forms.Label lbl_NA_20;
        public System.Windows.Forms.Label lbl上後左門;
        public System.Windows.Forms.Label lbl螢幕小門;
        public System.Windows.Forms.Label lbl上後右門;
        public System.Windows.Forms.Label lbl上右左門;
        public System.Windows.Forms.Label lbl上左左門;
        public System.Windows.Forms.Label lbl上右右門;
        public System.Windows.Forms.Label lbl上左右門;
        public System.Windows.Forms.Label lbl_NA_19;
        public System.Windows.Forms.Label lbl_NA_18;
        public System.Windows.Forms.Label lbl_NA_17;
        public System.Windows.Forms.Label lbl_NA_16;
        public System.Windows.Forms.Label lbl_NA_15;
        public System.Windows.Forms.Label lbl_擺放座關;
        public System.Windows.Forms.Label lbl_NA_13;
        public System.Windows.Forms.Label lbl_擺放座開;
        public System.Windows.Forms.Label lbl_NA_11;
        public System.Windows.Forms.Label lbl急停鈕;
        public System.Windows.Forms.Label lbl_NA_10;
        public System.Windows.Forms.Label lbl停止鈕;
        public System.Windows.Forms.Label lbl_NA_09;
        public System.Windows.Forms.Label lbl啟動鈕;
        public System.Windows.Forms.Label lbl_NA_08;
        public System.Windows.Forms.Label lbl復歸鈕;
        public System.Windows.Forms.Label lbl吸料盒;
        public System.Windows.Forms.Label lbl兩點壓2;
        public System.Windows.Forms.Label lbl堵料盒;
        public System.Windows.Forms.Label lbl兩點壓1;
        public System.Windows.Forms.Label lbl取料ng盒;
        public System.Windows.Forms.Label lbl吸嘴空2;
        public System.Windows.Forms.Label lbl_NA_07;
        public System.Windows.Forms.Label lbl吸嘴空1;
        public System.Windows.Forms.Label lbl擺放空2;
        public System.Windows.Forms.Label lblsk1空2;
        public System.Windows.Forms.Label lbl擺放空1;
        public System.Windows.Forms.Label lblsk1空1;
        public System.Windows.Forms.Label lblsk2空2;
        public System.Windows.Forms.Label lbl載盤空2;
        public System.Windows.Forms.Label lblsk2空1;
        public System.Windows.Forms.Label lbl載盤空1;
        public System.Windows.Forms.Label lbl_NA_06;
        public System.Windows.Forms.Label lbl載盤X後;
        public System.Windows.Forms.Label lbl_NA_05;
        public System.Windows.Forms.Label lbl載盤X前;
        public System.Windows.Forms.Label lbl_NA_04;
        public System.Windows.Forms.Label lbl植針Z前;
        public System.Windows.Forms.Label lbl_NA_03;
        public System.Windows.Forms.Label lbl植針Z後;
        public System.Windows.Forms.Label lbl_NA_02;
        public System.Windows.Forms.Label lbl取料X前;
        public System.Windows.Forms.Label lbl_NA_01;
        public System.Windows.Forms.Label lbl取料X後;
        public System.Windows.Forms.Label lbl取料Y前;
        public System.Windows.Forms.Label lbl載盤Y前;
        public System.Windows.Forms.Label lbl取料Y後;
        public System.Windows.Forms.Label lbl載盤Y後;
        public System.Windows.Forms.Label lbl_工作門_Convert;
        public System.Windows.Forms.Label lbl_植針R軸_Convert;
        public System.Windows.Forms.Label lbl_植針Z軸_Convert;
        public System.Windows.Forms.Label lbl_載盤Y軸_Convert;
        public System.Windows.Forms.Label lbl_載盤X軸_Convert;
        public System.Windows.Forms.Label lbl_吸嘴R軸_Convert;
        public System.Windows.Forms.Label lbl_吸嘴Z軸_Convert;
        public System.Windows.Forms.Label lbl_吸嘴Y軸_Convert;
        public System.Windows.Forms.Label lbl_吸嘴X軸_Convert;
        public System.Windows.Forms.Label lbl_工作門_Back;
        public System.Windows.Forms.Label lbl_工作門_RAW;
        public System.Windows.Forms.Label lbl_植針R軸_Back;
        public System.Windows.Forms.Label lbl_植針R軸_RAW;
        public System.Windows.Forms.Label lbl_植針Z軸_Back;
        public System.Windows.Forms.Label lbl_植針Z軸_RAW;
        public System.Windows.Forms.Label lbl_載盤Y軸_Back;
        public System.Windows.Forms.Label lbl_載盤Y軸_RAW;
        public System.Windows.Forms.Label lbl_載盤X軸_Back;
        public System.Windows.Forms.Label lbl_載盤X軸_RAW;
        public System.Windows.Forms.Label lbl_吸嘴R軸_Back;
        public System.Windows.Forms.Label lbl_吸嘴R軸_RAW;
        public System.Windows.Forms.Label lbl_吸嘴Z軸_Back;
        public System.Windows.Forms.Label lbl_吸嘴Z軸_RAW;
        public System.Windows.Forms.Label lbl_吸嘴Y軸_Back;
        public System.Windows.Forms.Label lbl_吸嘴Y軸_RAW;
        public System.Windows.Forms.Label lbl_吸嘴X軸_Back;
        public System.Windows.Forms.Label lbl_吸嘴X軸_RAW;
        public System.Windows.Forms.Button btn_minus_10;
        public System.Windows.Forms.Button btn_minus_1;
        public System.Windows.Forms.Button btn_plus_10;
        public System.Windows.Forms.Button btn_plus_1;
        public System.Windows.Forms.Button btnABSMove;
        public  System.Windows.Forms.TextBox txtABSpos;
        public System.Windows.Forms.CheckBox en_工作門;
        public System.Windows.Forms.CheckBox en_植針R軸;
        public System.Windows.Forms.CheckBox en_植針Z軸;
        public System.Windows.Forms.CheckBox en_載盤Y軸;
        public System.Windows.Forms.CheckBox en_載盤X軸;
        public System.Windows.Forms.CheckBox en_吸嘴R軸;
        public System.Windows.Forms.CheckBox en_吸嘴Z軸;
        public System.Windows.Forms.CheckBox en_吸嘴Y軸;
        public System.Windows.Forms.CheckBox en_吸嘴X軸;
        public System.Windows.Forms.Label lbl_acpos_工作門;
        public System.Windows.Forms.Label lbl_acpos_植針R軸;
        public System.Windows.Forms.Label lbl_acpos_植針Z軸;
        public System.Windows.Forms.Label lbl_acpos_載盤Y軸;
        public System.Windows.Forms.Label lbl_acpos_載盤X軸;
        public System.Windows.Forms.Label lbl_acpos_吸嘴R軸;
        public System.Windows.Forms.Label lbl_acpos_吸嘴Z軸;
        public System.Windows.Forms.Label lbl_acpos_吸嘴Y軸;
        public System.Windows.Forms.Label lbl_acpos_吸嘴X軸;
        public System.Windows.Forms.Label lbl_acpos_工作門_lbl;
        public System.Windows.Forms.Label lbl_acpos_植針R軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_植針Z軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_載盤Y軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_載盤X軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_吸嘴R軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_吸嘴Z軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_吸嘴Y軸_lbl;
        public System.Windows.Forms.Label lbl_acpos_吸嘴X軸_lbl;
        public System.Windows.Forms.Label lbl_spd_工作門;
        public System.Windows.Forms.Label lbl_spd_植針R軸;
        public System.Windows.Forms.Label lbl_spd_植針Z軸;
        public System.Windows.Forms.Label lbl_spd_載盤Y軸;
        public System.Windows.Forms.Label lbl_spd_載盤X軸;
        public System.Windows.Forms.Label lbl_spd_吸嘴R軸;
        public System.Windows.Forms.Label lbl_spd_吸嘴Z軸;
        public System.Windows.Forms.Label lbl_spd_吸嘴Y軸;
        public System.Windows.Forms.Label lbl_spd_吸嘴X軸;
        public System.Windows.Forms.Label lbl_spd_工作門_lbl;
        public System.Windows.Forms.Label lbl_spd_植針R軸_lbl;
        public System.Windows.Forms.Label lbl_spd_植針Z軸_lbl;
        public System.Windows.Forms.Label lbl_spd_載盤Y軸_lbl;
        public System.Windows.Forms.Label lbl_spd_載盤X軸_lbl;
        public System.Windows.Forms.Label lbl_spd_吸嘴R軸_lbl;
        public System.Windows.Forms.Label lbl_spd_吸嘴Z軸_lbl;
        public System.Windows.Forms.Label lbl_spd_吸嘴Y軸_lbl;
        public System.Windows.Forms.Label lbl_spd_吸嘴X軸_lbl;
        public  System.Windows.Forms.RadioButton select_吸嘴X軸;
        public  System.Windows.Forms.RadioButton select_吸嘴Y軸;
        public  System.Windows.Forms.RadioButton select_吸嘴Z軸;
        public  System.Windows.Forms.RadioButton select_吸嘴R軸;
        public  System.Windows.Forms.RadioButton select_載盤X軸;
        public  System.Windows.Forms.RadioButton select_載盤Y軸;
        public  System.Windows.Forms.RadioButton select_植針Z軸;
        public  System.Windows.Forms.RadioButton select_植針R軸;
        public  System.Windows.Forms.RadioButton select_工作門;
        public  System.Windows.Forms.RadioButton select_Socket檢測;
        public  System.Windows.Forms.RadioButton select_JoDell3D掃描;
        public  System.Windows.Forms.RadioButton select_JoDell吸針嘴;
        public  System.Windows.Forms.RadioButton select_JoDell植針嘴;
        public System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnSetHome;
        public System.Windows.Forms.Button btn_Disconnect;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btn_取得PinInfo;
        public System.Windows.Forms.Button btn_AlarmRST;
        public System.Windows.Forms.Button btn_Connect;
        public System.Windows.Forms.TabPage tabPage1;
        public Inspector.Inspector inspector1;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.Label lbl_IAI_Convert;
        public System.Windows.Forms.Label lbl_IAI_Back;
        public System.Windows.Forms.Label lbl_IAI_RAW;
        public System.Windows.Forms.CheckBox en_IAI;
        public System.Windows.Forms.Label lbl_acpos_IAI;
        public System.Windows.Forms.Label lbl_acpos_IAI_lbl;
        public System.Windows.Forms.Label lbl_spd_IAI;
        public System.Windows.Forms.Label lbl_spd_IAI_lbl;
        public System.Windows.Forms.Label lbl_JoDell吸針嘴_Convert;
        public System.Windows.Forms.Label lbl_JoDell吸針嘴_Back;
        public System.Windows.Forms.Label lbl_JoDell吸針嘴_RAW;
        public System.Windows.Forms.CheckBox en_JoDell吸針嘴;
        public System.Windows.Forms.Label lbl_acpos_JoDell吸針嘴;
        public System.Windows.Forms.Label lbl_acpos_JoDell吸針嘴_lbl;
        public System.Windows.Forms.Label lbl_spd_JoDell吸針嘴;
        public System.Windows.Forms.Label lbl_spd_JoDell吸針嘴_lbl;
        public System.Windows.Forms.Label lbl_JoDell3D掃描_Convert;
        public System.Windows.Forms.Label lbl_JoDell3D掃描_Back;
        public System.Windows.Forms.Label lbl_JoDell3D掃描_RAW;
        public System.Windows.Forms.CheckBox en_JoDell3D掃描;
        public System.Windows.Forms.Label lbl_acpos_JoDell3D掃描;
        public System.Windows.Forms.Label lbl_acpos_JoDell3D掃描_lbl;
        public System.Windows.Forms.Label lbl_spd_JoDell3D掃描;
        public System.Windows.Forms.Label lbl_spd_JoDell3D掃描_lbl;
        public System.Windows.Forms.Label lbl_JoDell植針嘴_Convert;
        public System.Windows.Forms.Label lbl_JoDell植針嘴_Back;
        public System.Windows.Forms.Label lbl_JoDell植針嘴_RAW;
        public System.Windows.Forms.CheckBox en_JoDell植針嘴;
        public System.Windows.Forms.Label lbl_acpos_JoDell植針嘴;
        public System.Windows.Forms.Label lbl_acpos_JoDell植針嘴_lbl;
        public System.Windows.Forms.Label lbl_spd_JoDell植針嘴;
        public System.Windows.Forms.Label lbl_spd_JoDell植針嘴_lbl;
        public System.Windows.Forms.Button btnVibrationStop;
        public System.Windows.Forms.Button btnVibrationInit;
        public System.Windows.Forms.VScrollBar vcb_植針吹氣流量閥;
        public System.Windows.Forms.Button btn_minus_d1;
        public System.Windows.Forms.Button btn_plus_d1;
        public System.Windows.Forms.Button btn_minus_d001;
        public System.Windows.Forms.Button btn_plus_d001;
        public System.Windows.Forms.Button btn_minus_d01;
        public System.Windows.Forms.Button btn_plus_d01;
        public System.Windows.Forms.TabPage tabPage3;
        public System.Windows.Forms.Timer tmr_Home;
        public System.Windows.Forms.Button btn_Socket孔檢查;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Button btn_植針嘴檢查;
        public System.Windows.Forms.Label lbl_植針吹氣流量閥;
        public System.Windows.Forms.Label lbl_debug;
        public System.Windows.Forms.Button btn_home;
        public System.Windows.Forms.Timer tmr_TakePin;
        public System.Windows.Forms.Button btn_TakePin;
        public System.Windows.Forms.TextBox txt_取料循環;
        public System.Windows.Forms.Label lbl上下收;
        public System.Windows.Forms.Label lbl震散;
        public System.Windows.Forms.Label lbl左右收;
        public System.Windows.Forms.Label lbl料倉;
        public System.Windows.Forms.HScrollBar SB_VBLED;
        public System.Windows.Forms.Label lblVBLED;
        public System.Windows.Forms.Label lblLog;
        public System.Windows.Forms.Timer tmr_Warning;
        public System.Windows.Forms.Button btn上膛;
        public System.Windows.Forms.Button btn_tmrPause;
        public System.Windows.Forms.Button btn_tmrStop;
        public System.Windows.Forms.Label lbl_CycleTime;
        public  System.Windows.Forms.Button btn_manual;
        public System.Windows.Forms.Button button5;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.TabControl tab_Needles;
        public System.Windows.Forms.TabPage tp_Needles;
        public System.Windows.Forms.TabPage tp_NeedlesJudge;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.Label label12;
        public System.Windows.Forms.PictureBox pictureBox2;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.GroupBox grp_目前作業項目;
        public System.Windows.Forms.GroupBox grp_GroupPin2;
        public System.Windows.Forms.TextBox txt_PogoPin2已植數量2;
        public System.Windows.Forms.Label lbl_PogoPin2已植數量2;
        public System.Windows.Forms.TextBox txt_PogoPin1已植數量2;
        public System.Windows.Forms.Label lbl_PogoPin1已植數量2;
        public System.Windows.Forms.TextBox txt_PogoPin2Qty4;
        public System.Windows.Forms.Label lbl_PogoPin2Qty4;
        public System.Windows.Forms.TextBox txt_PogoPin1Qty4;
        public System.Windows.Forms.Label lbl_PogoPin1Qty4;
        public System.Windows.Forms.GroupBox grp_GroupPin1;
        public System.Windows.Forms.TextBox txt_PogoPin2已植數量1;
        public System.Windows.Forms.Label lbl_PogoPin2已植數量1;
        public System.Windows.Forms.TextBox txt_PogoPin1已植數量1;
        public System.Windows.Forms.Label lbl_PogoPin1已植數量1;
        public System.Windows.Forms.TextBox txt_PogoPin2Qty3;
        public System.Windows.Forms.Label lbl_PogoPin2Qty3;
        public System.Windows.Forms.TextBox txt_PogoPin1Qty3;
        public System.Windows.Forms.Label lbl_PogoPin1Qty3;
        public System.Windows.Forms.RadioButton rad_ChangeGroupPin;
        public System.Windows.Forms.RadioButton rad_ChangeAllNewPin;
        public System.Windows.Forms.TextBox txt_PogoPin2已植數量;
        public System.Windows.Forms.Label lbl_PogoPin2已植數量;
        public System.Windows.Forms.TextBox txt_PogoPin1已植數量;
        public System.Windows.Forms.Label lbl_PogoPin1已植數量;
        public System.Windows.Forms.TextBox txt_PogoPin2Qty2;
        public System.Windows.Forms.Label lbl_PogoPin2Qty2;
        public System.Windows.Forms.TextBox txt_PogoPin1Qty2;
        public System.Windows.Forms.Label lbl_PogoPin1Qty2;
        public System.Windows.Forms.GroupBox grp_SocketInfo;
        public System.Windows.Forms.GroupBox grp_設備治具資訊;
        public System.Windows.Forms.Label lbl_PogoPin3Qty1;
        public System.Windows.Forms.Label lbl_PogoPin2Qty1;
        public System.Windows.Forms.Label lbl_PogoPin1Qty1;
        public System.Windows.Forms.TextBox txt_下針導正模組;
        public System.Windows.Forms.Label lbl_下針導正模組;
        public System.Windows.Forms.TextBox txt_取針模組PI;
        public System.Windows.Forms.Label lbl_取針模組PI;
        public System.Windows.Forms.TextBox txt_PogoPin3Qty1;
        public System.Windows.Forms.TextBox txt_PogoPin2Qty1;
        public System.Windows.Forms.TextBox txt_PogoPin1Qty1;
        public System.Windows.Forms.GroupBox grp_BarcodeInfo;
        public System.Windows.Forms.Button btn_SaveFile;
        public System.Windows.Forms.Button btn_OpenFile;
        public System.Windows.Forms.TextBox txt_儲位;
        public System.Windows.Forms.Label lbl_PogoPin2Qty;
        public System.Windows.Forms.TextBox txt_板全號;
        public System.Windows.Forms.Label lbl_PogoPin1Qty;
        public System.Windows.Forms.TextBox txt_型號;
        public System.Windows.Forms.Label lbl_Socket定位座;
        public System.Windows.Forms.TextBox txt_客戶;
        public System.Windows.Forms.Label lbl_FileName;
        public System.Windows.Forms.TextBox txt_短編號;
        public System.Windows.Forms.Label lbl_Socket;
        public System.Windows.Forms.TextBox txt_Barcode;
        public System.Windows.Forms.Label lbl_條碼輸入欄位;
        public System.Windows.Forms.GroupBox grp_儲存資訊;
        public System.Windows.Forms.Button btn_停止;
        public System.Windows.Forms.Button btn_開始;
        public System.Windows.Forms.MenuStrip ms_Viewer;
        public System.Windows.Forms.ToolStripMenuItem tsm_Account;
        public System.Windows.Forms.ToolStripMenuItem tsmi_Login;
        public System.Windows.Forms.ToolStripMenuItem tsmi_Logout;
        public System.Windows.Forms.ToolStripMenuItem tsm_File;
        public System.Windows.Forms.ToolStripMenuItem tsmi_OpenFile;
        public System.Windows.Forms.ToolStripMenuItem tsmi_SaveFile;
        public System.Windows.Forms.PictureBox pic_Needles;
        public System.Windows.Forms.GroupBox grp_NeedleInfo;
        public System.Windows.Forms.RadioButton rad_Replace;
        public System.Windows.Forms.RadioButton rad_Remove;
        public System.Windows.Forms.RadioButton rad_Place;
        public System.Windows.Forms.Label lbl_Index;
        public System.Windows.Forms.TextBox txt_Index;
        public System.Windows.Forms.CheckBox chk_Disable;
        public System.Windows.Forms.CheckBox chk_Display;
        public System.Windows.Forms.TextBox txt_Diameter;
        public System.Windows.Forms.Label lbl_Diameter;
        public System.Windows.Forms.TextBox txt_PosX;
        public System.Windows.Forms.TextBox txt_PosY;
        public System.Windows.Forms.Label lbl_PosX;
        public System.Windows.Forms.TextBox txt_Id;
        public System.Windows.Forms.Label lbl_Id;
        public System.Windows.Forms.Label lbl_Name;
        public System.Windows.Forms.TextBox txt_Name;
        public Button btn_取得目標座標;
        public Label label15;
        public Label label14;
        public Button btn_Resume;
        public TextBox txt_HoldIndex;
        public Label lbl_PicMousePos;
        public Label lbl_RealMousePos;
        public Label lbl_Offset;
        public Label lbl_ZoomFactor;
        public ContextMenuStrip cms_pic_Needles;
        public ToolStripMenuItem tsmi_Place;
        public ToolStripMenuItem tsmi_Remove;
        public ToolStripMenuItem tsmi_Replace;
        public ToolStripMenuItem tsmi_Display;
        public ToolStripMenuItem tsmi_Enable;
        public ToolStripMenuItem tsmi_Reset;
        public CheckBox chk_Reserve1;
        public ToolStripMenuItem tsmi_Reserve1;
        public Button btn_tmrClear;
        public ToolTip ttp_NeedleInfo;
        public Label label13;
        public Label lbl_吸嘴破真空流量閥;
        public VScrollBar vcb_吸嘴破真空流量閥;
        public Label lbl_竹蜻蜓座標B;
        public Label lbl_竹蜻蜓座標A;
        public TextBox tB_Bx;
        public TextBox tB_Ax;
        public TextBox tB_By;
        public TextBox tB_Ay;
        public Label lbl_計算角度;
        public Button btn_ToPointB;
        public Button btn_ToPointA;
        public Button btn_SwitchPointAB;
        public CheckBox cB_AlwaysResume;
        public Label lbl_PinIndex;
        public CheckBox cB_料盤有料;
        public RichTextBox rtb_Status;
        public Timer tmrBarCodeScanner;
        public Label label16;
        public Label label17;
        public Label label18;
        public Button btn_socket相機兩點定位;
        private System.ComponentModel.IContainer components;
        public Button btn抽針;
        public Button btn_Reset;
        private ToolStripMenuItem tsmi_CloseFile;
        public Button btn_取針;
        public Button btn_植針;
        public Button btn_拋料;
        public Button btn_參數;
        public Button btn_置換;
        public Button btn_minus;
        public Button btn_plus;
        public TextBox edit_diff_value;
        public Label label19;
        public Button btn_兩點校正;
        public Timer tmr_2p_Calibration;
        public Label lbl_2pCalibraLog;
    }
}

