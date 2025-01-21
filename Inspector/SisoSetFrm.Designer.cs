namespace Camera
{
    partial class SisoSetFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        public System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.KeyList = new System.Windows.Forms.ComboBox();
            this.btn_Search = new System.Windows.Forms.Button();
            this.btn_Set = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.GammaBar = new System.Windows.Forms.HScrollBar();
            this.ExposureBar = new System.Windows.Forms.HScrollBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ed_Speed = new System.Windows.Forms.TextBox();
            this.lb_Gamma = new System.Windows.Forms.Label();
            this.lb_Exposure = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ed_ScaleX = new System.Windows.Forms.TextBox();
            this.ed_ScaleY = new System.Windows.Forms.TextBox();
            this.ck_MirrorX = new System.Windows.Forms.CheckBox();
            this.ck_MirrorY = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ed_Dxy = new System.Windows.Forms.TextBox();
            this.ed_Dxx = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ed_Dyy = new System.Windows.Forms.TextBox();
            this.ed_Dyx = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btn_SaveConfig = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_SaveParam = new System.Windows.Forms.Button();
            this.ck_Binning = new System.Windows.Forms.CheckBox();
            this.cb_Rotate = new System.Windows.Forms.ComboBox();
            this.btn_LoadConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Key：";
            // 
            // KeyList
            // 
            this.KeyList.FormattingEnabled = true;
            this.KeyList.Location = new System.Drawing.Point(82, 14);
            this.KeyList.Name = "KeyList";
            this.KeyList.Size = new System.Drawing.Size(369, 29);
            this.KeyList.TabIndex = 1;
            // 
            // btn_Search
            // 
            this.btn_Search.Location = new System.Drawing.Point(457, 6);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(76, 42);
            this.btn_Search.TabIndex = 2;
            this.btn_Search.Text = "Search";
            this.btn_Search.UseVisualStyleBackColor = true;
            this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
            // 
            // btn_Set
            // 
            this.btn_Set.Location = new System.Drawing.Point(539, 6);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(76, 42);
            this.btn_Set.TabIndex = 3;
            this.btn_Set.Text = "Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Gamma：";
            // 
            // GammaBar
            // 
            this.GammaBar.LargeChange = 2;
            this.GammaBar.Location = new System.Drawing.Point(122, 57);
            this.GammaBar.Maximum = 40;
            this.GammaBar.Name = "GammaBar";
            this.GammaBar.Size = new System.Drawing.Size(413, 36);
            this.GammaBar.TabIndex = 5;
            this.GammaBar.Value = 10;
            this.GammaBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.GammaBar_Scroll);
            // 
            // ExposureBar
            // 
            this.ExposureBar.LargeChange = 500;
            this.ExposureBar.Location = new System.Drawing.Point(122, 102);
            this.ExposureBar.Maximum = 30000;
            this.ExposureBar.Minimum = 200;
            this.ExposureBar.Name = "ExposureBar";
            this.ExposureBar.Size = new System.Drawing.Size(413, 36);
            this.ExposureBar.SmallChange = 100;
            this.ExposureBar.TabIndex = 7;
            this.ExposureBar.Value = 200;
            this.ExposureBar.Visible = false;
            this.ExposureBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ExposureBar_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 21);
            this.label3.TabIndex = 6;
            this.label3.Text = "Exposure：";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 21);
            this.label4.TabIndex = 8;
            this.label4.Text = "Speed：";
            // 
            // ed_Speed
            // 
            this.ed_Speed.Location = new System.Drawing.Point(122, 141);
            this.ed_Speed.Name = "ed_Speed";
            this.ed_Speed.Size = new System.Drawing.Size(102, 33);
            this.ed_Speed.TabIndex = 9;
            this.ed_Speed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_Speed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // lb_Gamma
            // 
            this.lb_Gamma.Location = new System.Drawing.Point(538, 65);
            this.lb_Gamma.Name = "lb_Gamma";
            this.lb_Gamma.Size = new System.Drawing.Size(70, 18);
            this.lb_Gamma.TabIndex = 10;
            this.lb_Gamma.Text = "1.0";
            this.lb_Gamma.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_Exposure
            // 
            this.lb_Exposure.Location = new System.Drawing.Point(538, 110);
            this.lb_Exposure.Name = "lb_Exposure";
            this.lb_Exposure.Size = new System.Drawing.Size(70, 20);
            this.lb_Exposure.TabIndex = 11;
            this.lb_Exposure.Text = "5000";
            this.lb_Exposure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(320, 147);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 21);
            this.label7.TabIndex = 12;
            this.label7.Text = "Scale：";
            // 
            // ed_ScaleX
            // 
            this.ed_ScaleX.Location = new System.Drawing.Point(397, 141);
            this.ed_ScaleX.Name = "ed_ScaleX";
            this.ed_ScaleX.Size = new System.Drawing.Size(102, 33);
            this.ed_ScaleX.TabIndex = 13;
            this.ed_ScaleX.Text = "0.0098785";
            this.ed_ScaleX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_ScaleX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // ed_ScaleY
            // 
            this.ed_ScaleY.Location = new System.Drawing.Point(505, 141);
            this.ed_ScaleY.Name = "ed_ScaleY";
            this.ed_ScaleY.Size = new System.Drawing.Size(102, 33);
            this.ed_ScaleY.TabIndex = 14;
            this.ed_ScaleY.Text = "0.0098785";
            this.ed_ScaleY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_ScaleY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // ck_MirrorX
            // 
            this.ck_MirrorX.AutoSize = true;
            this.ck_MirrorX.Location = new System.Drawing.Point(22, 224);
            this.ck_MirrorX.Name = "ck_MirrorX";
            this.ck_MirrorX.Size = new System.Drawing.Size(97, 25);
            this.ck_MirrorX.TabIndex = 15;
            this.ck_MirrorX.Text = "MirrorX";
            this.ck_MirrorX.UseVisualStyleBackColor = true;
            this.ck_MirrorX.Visible = false;
            this.ck_MirrorX.Click += new System.EventHandler(this.ck_MirrorX_Click);
            // 
            // ck_MirrorY
            // 
            this.ck_MirrorY.AutoSize = true;
            this.ck_MirrorY.Location = new System.Drawing.Point(127, 224);
            this.ck_MirrorY.Name = "ck_MirrorY";
            this.ck_MirrorY.Size = new System.Drawing.Size(97, 25);
            this.ck_MirrorY.TabIndex = 16;
            this.ck_MirrorY.Text = "MirrorY";
            this.ck_MirrorY.UseVisualStyleBackColor = true;
            this.ck_MirrorY.Visible = false;
            this.ck_MirrorY.Click += new System.EventHandler(this.ck_MirrorY_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(39, 186);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 21);
            this.label8.TabIndex = 17;
            this.label8.Text = "Rotate：";
            // 
            // ed_Dxy
            // 
            this.ed_Dxy.Location = new System.Drawing.Point(505, 180);
            this.ed_Dxy.Name = "ed_Dxy";
            this.ed_Dxy.Size = new System.Drawing.Size(102, 33);
            this.ed_Dxy.TabIndex = 21;
            this.ed_Dxy.Text = "0.0098785";
            this.ed_Dxy.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_Dxy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // ed_Dxx
            // 
            this.ed_Dxx.Location = new System.Drawing.Point(397, 180);
            this.ed_Dxx.Name = "ed_Dxx";
            this.ed_Dxx.Size = new System.Drawing.Size(102, 33);
            this.ed_Dxx.TabIndex = 20;
            this.ed_Dxx.Text = "0.0098785";
            this.ed_Dxx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_Dxx.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(336, 186);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 21);
            this.label9.TabIndex = 19;
            this.label9.Text = "Dx：";
            // 
            // ed_Dyy
            // 
            this.ed_Dyy.Location = new System.Drawing.Point(505, 219);
            this.ed_Dyy.Name = "ed_Dyy";
            this.ed_Dyy.Size = new System.Drawing.Size(102, 33);
            this.ed_Dyy.TabIndex = 24;
            this.ed_Dyy.Text = "0.0098785";
            this.ed_Dyy.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_Dyy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // ed_Dyx
            // 
            this.ed_Dyx.Location = new System.Drawing.Point(397, 219);
            this.ed_Dyx.Name = "ed_Dyx";
            this.ed_Dyx.Size = new System.Drawing.Size(102, 33);
            this.ed_Dyx.TabIndex = 23;
            this.ed_Dyx.Text = "0.0098785";
            this.ed_Dyx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ed_Dyx.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_KeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(336, 225);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 21);
            this.label10.TabIndex = 22;
            this.label10.Text = "Dy：";
            // 
            // btn_SaveConfig
            // 
            this.btn_SaveConfig.Location = new System.Drawing.Point(22, 283);
            this.btn_SaveConfig.Name = "btn_SaveConfig";
            this.btn_SaveConfig.Size = new System.Drawing.Size(128, 36);
            this.btn_SaveConfig.TabIndex = 25;
            this.btn_SaveConfig.Text = "SaveConfig";
            this.btn_SaveConfig.UseVisualStyleBackColor = true;
            this.btn_SaveConfig.Click += new System.EventHandler(this.btn_SaveConfig_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(479, 283);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(128, 36);
            this.btn_Close.TabIndex = 26;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_SaveParam
            // 
            this.btn_SaveParam.Location = new System.Drawing.Point(156, 283);
            this.btn_SaveParam.Name = "btn_SaveParam";
            this.btn_SaveParam.Size = new System.Drawing.Size(128, 36);
            this.btn_SaveParam.TabIndex = 27;
            this.btn_SaveParam.Text = "SaveParam";
            this.btn_SaveParam.UseVisualStyleBackColor = true;
            this.btn_SaveParam.Click += new System.EventHandler(this.btn_SaveParam_Click);
            // 
            // ck_Binning
            // 
            this.ck_Binning.AutoSize = true;
            this.ck_Binning.Location = new System.Drawing.Point(22, 252);
            this.ck_Binning.Name = "ck_Binning";
            this.ck_Binning.Size = new System.Drawing.Size(92, 25);
            this.ck_Binning.TabIndex = 28;
            this.ck_Binning.Text = "Binning";
            this.ck_Binning.UseVisualStyleBackColor = true;
            this.ck_Binning.Visible = false;
            this.ck_Binning.Click += new System.EventHandler(this.ck_Binning_Click);
            // 
            // cb_Rotate
            // 
            this.cb_Rotate.FormattingEnabled = true;
            this.cb_Rotate.Items.AddRange(new object[] {
            "0",
            "90",
            "180",
            "270"});
            this.cb_Rotate.Location = new System.Drawing.Point(122, 183);
            this.cb_Rotate.Name = "cb_Rotate";
            this.cb_Rotate.Size = new System.Drawing.Size(102, 29);
            this.cb_Rotate.TabIndex = 29;
            this.cb_Rotate.DropDown += new System.EventHandler(this.cb_Rotate_DropDown);
            this.cb_Rotate.DropDownClosed += new System.EventHandler(this.cb_Rotate_DropDownClosed);
            // 
            // btn_LoadConfig
            // 
            this.btn_LoadConfig.Location = new System.Drawing.Point(22, 325);
            this.btn_LoadConfig.Name = "btn_LoadConfig";
            this.btn_LoadConfig.Size = new System.Drawing.Size(128, 36);
            this.btn_LoadConfig.TabIndex = 30;
            this.btn_LoadConfig.Text = "LoadConfig";
            this.btn_LoadConfig.UseVisualStyleBackColor = true;
            this.btn_LoadConfig.Click += new System.EventHandler(this.btn_LoadConfig_Click);
            // 
            // SisoSetFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(625, 366);
            this.Controls.Add(this.btn_LoadConfig);
            this.Controls.Add(this.cb_Rotate);
            this.Controls.Add(this.ck_Binning);
            this.Controls.Add(this.btn_SaveParam);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_SaveConfig);
            this.Controls.Add(this.ed_Dyy);
            this.Controls.Add(this.ed_Dyx);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.ed_Dxy);
            this.Controls.Add(this.ed_Dxx);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ck_MirrorY);
            this.Controls.Add(this.ck_MirrorX);
            this.Controls.Add(this.ed_ScaleY);
            this.Controls.Add(this.ed_ScaleX);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lb_Exposure);
            this.Controls.Add(this.lb_Gamma);
            this.Controls.Add(this.ed_Speed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ExposureBar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GammaBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_Set);
            this.Controls.Add(this.btn_Search);
            this.Controls.Add(this.KeyList);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SisoSetFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iRaypleSetFrm";
            this.Load += new System.EventHandler(this.MVSAreaGESetFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox KeyList;
        public System.Windows.Forms.Button btn_Search;
        public System.Windows.Forms.Button btn_Set;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.HScrollBar GammaBar;
        public System.Windows.Forms.HScrollBar ExposureBar;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox ed_Speed;
        public System.Windows.Forms.Label lb_Gamma;
        public System.Windows.Forms.Label lb_Exposure;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox ed_ScaleX;
        public System.Windows.Forms.TextBox ed_ScaleY;
        public System.Windows.Forms.CheckBox ck_MirrorX;
        public System.Windows.Forms.CheckBox ck_MirrorY;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox ed_Dxy;
        public System.Windows.Forms.TextBox ed_Dxx;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox ed_Dyy;
        public System.Windows.Forms.TextBox ed_Dyx;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Button btn_SaveConfig;
        public System.Windows.Forms.Button btn_Close;
        public System.Windows.Forms.Button btn_SaveParam;
        public System.Windows.Forms.CheckBox ck_Binning;
        public System.Windows.Forms.ComboBox cb_Rotate;
        public System.Windows.Forms.Button btn_LoadConfig;
    }
}