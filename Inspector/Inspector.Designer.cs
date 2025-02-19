﻿namespace Inspector
{
    partial class Inspector
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        public System.ComponentModel.IContainer components = null;

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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        public void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Win1 = new HalconDotNet.HWindowControl();
            this.Win4 = new HalconDotNet.HWindowControl();
            this.Win2 = new HalconDotNet.HWindowControl();
            this.Win5 = new HalconDotNet.HWindowControl();
            this.Win3 = new HalconDotNet.HWindowControl();
            this.Win6 = new HalconDotNet.HWindowControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ck_Measure = new System.Windows.Forms.CheckBox();
            this.num_Throshold = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.num_Pin寬Max = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.num_Pin寬Min = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.num_Pin長Max = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.num_Pin長Min = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ed_RecipeNo = new System.Windows.Forms.TextBox();
            this.ck_RealTray = new System.Windows.Forms.CheckBox();
            this.ck_PinArea = new System.Windows.Forms.CheckBox();
            this.ck_NuCarb = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_二孔校正 = new System.Windows.Forms.CheckBox();
            this.ck_Socket校正孔 = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.ed_針頭長 = new System.Windows.Forms.TextBox();
            this.ed_針頭寬 = new System.Windows.Forms.TextBox();
            this.ed_針尾長 = new System.Windows.Forms.TextBox();
            this.ed_針尾寬 = new System.Windows.Forms.TextBox();
            this.ck_Socket孔 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Throshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin寬Max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin寬Min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin長Max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin長Min)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.Win1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Win4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Win2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.Win5, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.Win3, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.Win6, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 115);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1121, 628);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // Win1
            // 
            this.Win1.BackColor = System.Drawing.Color.Black;
            this.Win1.BorderColor = System.Drawing.Color.Black;
            this.Win1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Win1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.Win1.Location = new System.Drawing.Point(3, 23);
            this.Win1.Name = "Win1";
            this.Win1.Size = new System.Drawing.Size(364, 288);
            this.Win1.TabIndex = 0;
            this.Win1.WindowSize = new System.Drawing.Size(364, 288);
            // 
            // Win4
            // 
            this.Win4.BackColor = System.Drawing.Color.Black;
            this.Win4.BorderColor = System.Drawing.Color.Black;
            this.Win4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Win4.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.Win4.Location = new System.Drawing.Point(3, 337);
            this.Win4.Name = "Win4";
            this.Win4.Size = new System.Drawing.Size(364, 288);
            this.Win4.TabIndex = 1;
            this.Win4.WindowSize = new System.Drawing.Size(364, 288);
            // 
            // Win2
            // 
            this.Win2.BackColor = System.Drawing.Color.Black;
            this.Win2.BorderColor = System.Drawing.Color.Black;
            this.Win2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Win2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.Win2.Location = new System.Drawing.Point(378, 23);
            this.Win2.Name = "Win2";
            this.Win2.Size = new System.Drawing.Size(364, 288);
            this.Win2.TabIndex = 2;
            this.Win2.WindowSize = new System.Drawing.Size(364, 288);
            // 
            // Win5
            // 
            this.Win5.BackColor = System.Drawing.Color.Black;
            this.Win5.BorderColor = System.Drawing.Color.Black;
            this.Win5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Win5.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.Win5.Location = new System.Drawing.Point(378, 337);
            this.Win5.Name = "Win5";
            this.Win5.Size = new System.Drawing.Size(364, 288);
            this.Win5.TabIndex = 3;
            this.Win5.WindowSize = new System.Drawing.Size(364, 288);
            // 
            // Win3
            // 
            this.Win3.BackColor = System.Drawing.Color.Black;
            this.Win3.BorderColor = System.Drawing.Color.Black;
            this.Win3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Win3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.Win3.Location = new System.Drawing.Point(753, 23);
            this.Win3.Name = "Win3";
            this.Win3.Size = new System.Drawing.Size(365, 288);
            this.Win3.TabIndex = 4;
            this.Win3.WindowSize = new System.Drawing.Size(365, 288);
            // 
            // Win6
            // 
            this.Win6.BackColor = System.Drawing.Color.Black;
            this.Win6.BorderColor = System.Drawing.Color.Black;
            this.Win6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Win6.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.Win6.Location = new System.Drawing.Point(753, 337);
            this.Win6.Name = "Win6";
            this.Win6.Size = new System.Drawing.Size(365, 288);
            this.Win6.TabIndex = 5;
            this.Win6.WindowSize = new System.Drawing.Size(365, 288);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(753, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(365, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "料倉區";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.DoubleClick += new System.EventHandler(this.入料CCD設定_DoubleClick);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(378, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(364, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Tray盤";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.DoubleClick += new System.EventHandler(this.TrayCCD設定_DoubleClick);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(364, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "吸嘴";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.DoubleClick += new System.EventHandler(this.吸嘴CCD設定_DoubleClick);
            // 
            // ck_Measure
            // 
            this.ck_Measure.AutoSize = true;
            this.ck_Measure.Location = new System.Drawing.Point(415, 67);
            this.ck_Measure.Name = "ck_Measure";
            this.ck_Measure.Size = new System.Drawing.Size(90, 23);
            this.ck_Measure.TabIndex = 42;
            this.ck_Measure.Text = "Measure";
            this.ck_Measure.UseVisualStyleBackColor = true;
            // 
            // num_Throshold
            // 
            this.num_Throshold.Location = new System.Drawing.Point(546, 3);
            this.num_Throshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.num_Throshold.Name = "num_Throshold";
            this.num_Throshold.Size = new System.Drawing.Size(75, 30);
            this.num_Throshold.TabIndex = 41;
            this.num_Throshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Throshold.Value = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.num_Throshold.ValueChanged += new System.EventHandler(this.num_Pin長Min_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(469, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 19);
            this.label10.TabIndex = 40;
            this.label10.Text = "閥值：";
            // 
            // num_Pin寬Max
            // 
            this.num_Pin寬Max.DecimalPlaces = 2;
            this.num_Pin寬Max.Location = new System.Drawing.Point(299, 75);
            this.num_Pin寬Max.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_Pin寬Max.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_Pin寬Max.Name = "num_Pin寬Max";
            this.num_Pin寬Max.Size = new System.Drawing.Size(75, 30);
            this.num_Pin寬Max.TabIndex = 34;
            this.num_Pin寬Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Pin寬Max.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.num_Pin寬Max.ValueChanged += new System.EventHandler(this.num_Pin長Min_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(274, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 19);
            this.label7.TabIndex = 33;
            this.label7.Text = "~";
            // 
            // num_Pin寬Min
            // 
            this.num_Pin寬Min.DecimalPlaces = 2;
            this.num_Pin寬Min.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_Pin寬Min.Location = new System.Drawing.Point(193, 75);
            this.num_Pin寬Min.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_Pin寬Min.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_Pin寬Min.Name = "num_Pin寬Min";
            this.num_Pin寬Min.Size = new System.Drawing.Size(75, 30);
            this.num_Pin寬Min.TabIndex = 32;
            this.num_Pin寬Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Pin寬Min.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_Pin寬Min.ValueChanged += new System.EventHandler(this.num_Pin長Min_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(116, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 19);
            this.label8.TabIndex = 31;
            this.label8.Text = "Pin寬：";
            // 
            // num_Pin長Max
            // 
            this.num_Pin長Max.DecimalPlaces = 2;
            this.num_Pin長Max.Location = new System.Drawing.Point(299, 39);
            this.num_Pin長Max.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_Pin長Max.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_Pin長Max.Name = "num_Pin長Max";
            this.num_Pin長Max.Size = new System.Drawing.Size(75, 30);
            this.num_Pin長Max.TabIndex = 30;
            this.num_Pin長Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Pin長Max.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.num_Pin長Max.ValueChanged += new System.EventHandler(this.num_Pin長Min_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(274, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 19);
            this.label6.TabIndex = 29;
            this.label6.Text = "~";
            // 
            // num_Pin長Min
            // 
            this.num_Pin長Min.DecimalPlaces = 2;
            this.num_Pin長Min.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_Pin長Min.Location = new System.Drawing.Point(193, 39);
            this.num_Pin長Min.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_Pin長Min.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_Pin長Min.Name = "num_Pin長Min";
            this.num_Pin長Min.Size = new System.Drawing.Size(75, 30);
            this.num_Pin長Min.TabIndex = 28;
            this.num_Pin長Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Pin長Min.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_Pin長Min.ValueChanged += new System.EventHandler(this.num_Pin長Min_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(116, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 19);
            this.label5.TabIndex = 27;
            this.label5.Text = "Pin長：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(82, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 19);
            this.label4.TabIndex = 26;
            this.label4.Text = "Recipe No：";
            // 
            // ed_RecipeNo
            // 
            this.ed_RecipeNo.Location = new System.Drawing.Point(193, 3);
            this.ed_RecipeNo.Name = "ed_RecipeNo";
            this.ed_RecipeNo.Size = new System.Drawing.Size(48, 30);
            this.ed_RecipeNo.TabIndex = 25;
            this.ed_RecipeNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ck_RealTray
            // 
            this.ck_RealTray.AutoSize = true;
            this.ck_RealTray.Location = new System.Drawing.Point(95, 48);
            this.ck_RealTray.Name = "ck_RealTray";
            this.ck_RealTray.Size = new System.Drawing.Size(15, 14);
            this.ck_RealTray.TabIndex = 44;
            this.ck_RealTray.UseVisualStyleBackColor = true;
            // 
            // ck_PinArea
            // 
            this.ck_PinArea.AutoSize = true;
            this.ck_PinArea.Location = new System.Drawing.Point(769, 4);
            this.ck_PinArea.Name = "ck_PinArea";
            this.ck_PinArea.Size = new System.Drawing.Size(87, 23);
            this.ck_PinArea.TabIndex = 46;
            this.ck_PinArea.Text = "PinArea";
            this.ck_PinArea.UseVisualStyleBackColor = true;
            // 
            // ck_NuCarb
            // 
            this.ck_NuCarb.AutoSize = true;
            this.ck_NuCarb.Location = new System.Drawing.Point(769, 33);
            this.ck_NuCarb.Name = "ck_NuCarb";
            this.ck_NuCarb.Size = new System.Drawing.Size(104, 23);
            this.ck_NuCarb.TabIndex = 47;
            this.ck_NuCarb.Text = "校正區域";
            this.ck_NuCarb.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(511, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 57);
            this.button1.TabIndex = 48;
            this.button1.Text = "下視覺建立特徵";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_二孔校正
            // 
            this.btn_二孔校正.AutoSize = true;
            this.btn_二孔校正.Location = new System.Drawing.Point(769, 61);
            this.btn_二孔校正.Name = "btn_二孔校正";
            this.btn_二孔校正.Size = new System.Drawing.Size(104, 23);
            this.btn_二孔校正.TabIndex = 49;
            this.btn_二孔校正.Text = "二孔校正";
            this.btn_二孔校正.UseVisualStyleBackColor = true;
            // 
            // ck_Socket校正孔
            // 
            this.ck_Socket校正孔.AutoSize = true;
            this.ck_Socket校正孔.Location = new System.Drawing.Point(769, 86);
            this.ck_Socket校正孔.Name = "ck_Socket校正孔";
            this.ck_Socket校正孔.Size = new System.Drawing.Size(134, 23);
            this.ck_Socket校正孔.TabIndex = 50;
            this.ck_Socket校正孔.Text = "Socket校正孔";
            this.ck_Socket校正孔.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(942, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 19);
            this.label9.TabIndex = 51;
            this.label9.Text = "針頭長：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(942, 37);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 19);
            this.label11.TabIndex = 52;
            this.label11.Text = "針頭寬：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(942, 68);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(85, 19);
            this.label12.TabIndex = 53;
            this.label12.Text = "針尾長：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(942, 96);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(85, 19);
            this.label13.TabIndex = 54;
            this.label13.Text = "針尾寬：";
            // 
            // ed_針頭長
            // 
            this.ed_針頭長.Location = new System.Drawing.Point(1033, 0);
            this.ed_針頭長.Name = "ed_針頭長";
            this.ed_針頭長.Size = new System.Drawing.Size(60, 30);
            this.ed_針頭長.TabIndex = 55;
            this.ed_針頭長.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_針頭長_KeyPress);
            // 
            // ed_針頭寬
            // 
            this.ed_針頭寬.Location = new System.Drawing.Point(1033, 31);
            this.ed_針頭寬.Name = "ed_針頭寬";
            this.ed_針頭寬.Size = new System.Drawing.Size(60, 30);
            this.ed_針頭寬.TabIndex = 56;
            this.ed_針頭寬.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_針頭長_KeyPress);
            // 
            // ed_針尾長
            // 
            this.ed_針尾長.Location = new System.Drawing.Point(1033, 62);
            this.ed_針尾長.Name = "ed_針尾長";
            this.ed_針尾長.Size = new System.Drawing.Size(60, 30);
            this.ed_針尾長.TabIndex = 57;
            this.ed_針尾長.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_針頭長_KeyPress);
            // 
            // ed_針尾寬
            // 
            this.ed_針尾寬.Location = new System.Drawing.Point(1033, 93);
            this.ed_針尾寬.Name = "ed_針尾寬";
            this.ed_針尾寬.Size = new System.Drawing.Size(60, 30);
            this.ed_針尾寬.TabIndex = 58;
            this.ed_針尾寬.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ed_針頭長_KeyPress);
            // 
            // ck_Socket孔
            // 
            this.ck_Socket孔.AutoSize = true;
            this.ck_Socket孔.Location = new System.Drawing.Point(629, 86);
            this.ck_Socket孔.Name = "ck_Socket孔";
            this.ck_Socket孔.Size = new System.Drawing.Size(96, 23);
            this.ck_Socket孔.TabIndex = 59;
            this.ck_Socket孔.Text = "Socket孔";
            this.ck_Socket孔.UseVisualStyleBackColor = true;
            // 
            // Inspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ck_Socket孔);
            this.Controls.Add(this.ed_針尾寬);
            this.Controls.Add(this.ed_針尾長);
            this.Controls.Add(this.ed_針頭寬);
            this.Controls.Add(this.ed_針頭長);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ck_Socket校正孔);
            this.Controls.Add(this.btn_二孔校正);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ck_NuCarb);
            this.Controls.Add(this.ck_PinArea);
            this.Controls.Add(this.ck_RealTray);
            this.Controls.Add(this.ck_Measure);
            this.Controls.Add(this.num_Throshold);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.num_Pin寬Max);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.num_Pin寬Min);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.num_Pin長Max);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.num_Pin長Min);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ed_RecipeNo);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "Inspector";
            this.Size = new System.Drawing.Size(1121, 743);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.num_Throshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin寬Max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin寬Min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin長Max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Pin長Min)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public HalconDotNet.HWindowControl Win1;
        public HalconDotNet.HWindowControl Win4;
        public HalconDotNet.HWindowControl Win2;
        public HalconDotNet.HWindowControl Win5;
        public HalconDotNet.HWindowControl Win3;
        public HalconDotNet.HWindowControl Win6;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox ck_Measure;
        public System.Windows.Forms.NumericUpDown num_Throshold;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.NumericUpDown num_Pin寬Max;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.NumericUpDown num_Pin寬Min;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.NumericUpDown num_Pin長Max;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown num_Pin長Min;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox ed_RecipeNo;
        public System.Windows.Forms.CheckBox ck_RealTray;
        public System.Windows.Forms.CheckBox ck_PinArea;
        public System.Windows.Forms.CheckBox ck_NuCarb;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.CheckBox btn_二孔校正;
        public System.Windows.Forms.CheckBox ck_Socket校正孔;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.Label label12;
        public System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox ed_針頭長;
        private System.Windows.Forms.TextBox ed_針頭寬;
        private System.Windows.Forms.TextBox ed_針尾長;
        private System.Windows.Forms.TextBox ed_針尾寬;
        public System.Windows.Forms.CheckBox ck_Socket孔;
    }
}
