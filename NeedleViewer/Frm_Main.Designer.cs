using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace NeedleViewer
{
    partial class Frm_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_Needles = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Diameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Place = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remove = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Replace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Display = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reserve1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reserve2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reserve3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reserve4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reserve5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbl_MousePos = new System.Windows.Forms.Label();
            this.menu_NeedleViewer = new System.Windows.Forms.MenuStrip();
            this.tsmi_File = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_OpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_SaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.pic_Needles = new System.Windows.Forms.PictureBox();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grp_NeedleInfo = new System.Windows.Forms.GroupBox();
            this.rad_Replace = new System.Windows.Forms.RadioButton();
            this.rad_Remove = new System.Windows.Forms.RadioButton();
            this.rad_Place = new System.Windows.Forms.RadioButton();
            this.lbl_Index = new System.Windows.Forms.Label();
            this.txt_Index = new System.Windows.Forms.TextBox();
            this.chk_Enable = new System.Windows.Forms.CheckBox();
            this.chk_Display = new System.Windows.Forms.CheckBox();
            this.txt_Diameter = new System.Windows.Forms.TextBox();
            this.lbl_Diameter = new System.Windows.Forms.Label();
            this.txt_PosX = new System.Windows.Forms.TextBox();
            this.txt_PosY = new System.Windows.Forms.TextBox();
            this.lbl_Pos = new System.Windows.Forms.Label();
            this.txt_Id = new System.Windows.Forms.TextBox();
            this.lbl_Id = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Needles)).BeginInit();
            this.menu_NeedleViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Needles)).BeginInit();
            this.grp_NeedleInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_Needles
            // 
            this.dgv_Needles.AllowUserToAddRows = false;
            this.dgv_Needles.AllowUserToDeleteRows = false;
            this.dgv_Needles.AllowUserToResizeRows = false;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle25.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Needles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle25;
            this.dgv_Needles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Needles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.ColumnName,
            this.Id,
            this.PosX,
            this.PosY,
            this.Diameter,
            this.Place,
            this.Remove,
            this.Replace,
            this.Display,
            this.Enable,
            this.Reserve1,
            this.Reserve2,
            this.Reserve3,
            this.Reserve4,
            this.Reserve5});
            dataGridViewCellStyle28.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle28.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle28.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle28.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle28.SelectionBackColor = System.Drawing.Color.Brown;
            dataGridViewCellStyle28.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle28.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Needles.DefaultCellStyle = dataGridViewCellStyle28;
            this.dgv_Needles.Location = new System.Drawing.Point(21, 760);
            this.dgv_Needles.MultiSelect = false;
            this.dgv_Needles.Name = "dgv_Needles";
            this.dgv_Needles.ReadOnly = true;
            this.dgv_Needles.RowHeadersVisible = false;
            this.dgv_Needles.RowHeadersWidth = 82;
            this.dgv_Needles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Needles.Size = new System.Drawing.Size(1251, 197);
            this.dgv_Needles.TabIndex = 3;
            this.dgv_Needles.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Needles_CellMouseEnter);
            this.dgv_Needles.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Needles_CellMouseLeave);
            this.dgv_Needles.SelectionChanged += new System.EventHandler(this.dgv_Needles_SelectionChanged);
            // 
            // Index
            // 
            dataGridViewCellStyle26.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Index.DefaultCellStyle = dataGridViewCellStyle26;
            this.Index.HeaderText = "流水號";
            this.Index.MinimumWidth = 10;
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Width = 200;
            // 
            // ColumnName
            // 
            dataGridViewCellStyle27.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ColumnName.DefaultCellStyle = dataGridViewCellStyle27;
            this.ColumnName.HeaderText = "名稱";
            this.ColumnName.MinimumWidth = 10;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Width = 200;
            // 
            // Id
            // 
            this.Id.HeaderText = "編號";
            this.Id.MinimumWidth = 10;
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Width = 200;
            // 
            // PosX
            // 
            this.PosX.HeaderText = "座標X";
            this.PosX.MinimumWidth = 10;
            this.PosX.Name = "PosX";
            this.PosX.ReadOnly = true;
            this.PosX.Width = 125;
            // 
            // PosY
            // 
            this.PosY.HeaderText = "座標Y";
            this.PosY.MinimumWidth = 10;
            this.PosY.Name = "PosY";
            this.PosY.ReadOnly = true;
            this.PosY.Width = 125;
            // 
            // Diameter
            // 
            this.Diameter.HeaderText = "直徑";
            this.Diameter.MinimumWidth = 10;
            this.Diameter.Name = "Diameter";
            this.Diameter.ReadOnly = true;
            this.Diameter.Width = 200;
            // 
            // Place
            // 
            this.Place.HeaderText = "放置";
            this.Place.MinimumWidth = 10;
            this.Place.Name = "Place";
            this.Place.ReadOnly = true;
            this.Place.Width = 200;
            // 
            // Remove
            // 
            this.Remove.HeaderText = "移除";
            this.Remove.MinimumWidth = 10;
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            this.Remove.Width = 200;
            // 
            // Replace
            // 
            this.Replace.HeaderText = "置換";
            this.Replace.MinimumWidth = 10;
            this.Replace.Name = "Replace";
            this.Replace.ReadOnly = true;
            this.Replace.Width = 200;
            // 
            // Display
            // 
            this.Display.HeaderText = "顯示";
            this.Display.MinimumWidth = 10;
            this.Display.Name = "Display";
            this.Display.ReadOnly = true;
            this.Display.Width = 200;
            // 
            // Enable
            // 
            this.Enable.HeaderText = "啟用";
            this.Enable.MinimumWidth = 10;
            this.Enable.Name = "Enable";
            this.Enable.ReadOnly = true;
            this.Enable.Width = 200;
            // 
            // Reserve1
            // 
            this.Reserve1.HeaderText = "保留1";
            this.Reserve1.MinimumWidth = 10;
            this.Reserve1.Name = "Reserve1";
            this.Reserve1.ReadOnly = true;
            this.Reserve1.Width = 200;
            // 
            // Reserve2
            // 
            this.Reserve2.HeaderText = "保留2";
            this.Reserve2.MinimumWidth = 10;
            this.Reserve2.Name = "Reserve2";
            this.Reserve2.ReadOnly = true;
            this.Reserve2.Width = 200;
            // 
            // Reserve3
            // 
            this.Reserve3.HeaderText = "保留3";
            this.Reserve3.MinimumWidth = 10;
            this.Reserve3.Name = "Reserve3";
            this.Reserve3.ReadOnly = true;
            this.Reserve3.Width = 200;
            // 
            // Reserve4
            // 
            this.Reserve4.HeaderText = "保留4";
            this.Reserve4.MinimumWidth = 10;
            this.Reserve4.Name = "Reserve4";
            this.Reserve4.ReadOnly = true;
            this.Reserve4.Width = 200;
            // 
            // Reserve5
            // 
            this.Reserve5.HeaderText = "保留5";
            this.Reserve5.MinimumWidth = 10;
            this.Reserve5.Name = "Reserve5";
            this.Reserve5.ReadOnly = true;
            this.Reserve5.Width = 200;
            // 
            // lbl_MousePos
            // 
            this.lbl_MousePos.AutoSize = true;
            this.lbl_MousePos.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_MousePos.Location = new System.Drawing.Point(1274, 711);
            this.lbl_MousePos.Name = "lbl_MousePos";
            this.lbl_MousePos.Size = new System.Drawing.Size(164, 43);
            this.lbl_MousePos.TabIndex = 5;
            this.lbl_MousePos.Text = "滑鼠座標 ";
            // 
            // menu_NeedleViewer
            // 
            this.menu_NeedleViewer.BackColor = System.Drawing.Color.Thistle;
            this.menu_NeedleViewer.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menu_NeedleViewer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_File});
            this.menu_NeedleViewer.Location = new System.Drawing.Point(0, 0);
            this.menu_NeedleViewer.Name = "menu_NeedleViewer";
            this.menu_NeedleViewer.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menu_NeedleViewer.Size = new System.Drawing.Size(1438, 51);
            this.menu_NeedleViewer.TabIndex = 7;
            this.menu_NeedleViewer.Text = "menuStrip1";
            // 
            // tsmi_File
            // 
            this.tsmi_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_OpenFile,
            this.tsmi_SaveFile});
            this.tsmi_File.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tsmi_File.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmi_File.Name = "tsmi_File";
            this.tsmi_File.Size = new System.Drawing.Size(101, 47);
            this.tsmi_File.Text = "檔案";
            // 
            // tsmi_OpenFile
            // 
            this.tsmi_OpenFile.Name = "tsmi_OpenFile";
            this.tsmi_OpenFile.Size = new System.Drawing.Size(248, 48);
            this.tsmi_OpenFile.Text = "開啟檔案";
            this.tsmi_OpenFile.Click += new System.EventHandler(this.tsmi_OpenFile_Click);
            // 
            // tsmi_SaveFile
            // 
            this.tsmi_SaveFile.Name = "tsmi_SaveFile";
            this.tsmi_SaveFile.Size = new System.Drawing.Size(248, 48);
            this.tsmi_SaveFile.Text = "儲存檔案";
            this.tsmi_SaveFile.Click += new System.EventHandler(this.tsmi_SaveFile_Click);
            // 
            // pic_Needles
            // 
            this.pic_Needles.BackColor = System.Drawing.Color.Honeydew;
            this.pic_Needles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_Needles.Location = new System.Drawing.Point(473, 53);
            this.pic_Needles.Name = "pic_Needles";
            this.pic_Needles.Size = new System.Drawing.Size(799, 700);
            this.pic_Needles.TabIndex = 0;
            this.pic_Needles.TabStop = false;
            this.pic_Needles.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Needles_Paint);
            this.pic_Needles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseDown);
            this.pic_Needles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseMove);
            this.pic_Needles.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pic_Needles_MouseWheel);
            // 
            // txt_Name
            // 
            this.txt_Name.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Name.Location = new System.Drawing.Point(96, 99);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(198, 50);
            this.txt_Name.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 99);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 43);
            this.label1.TabIndex = 9;
            this.label1.Text = "名稱";
            // 
            // grp_NeedleInfo
            // 
            this.grp_NeedleInfo.BackColor = System.Drawing.Color.FloralWhite;
            this.grp_NeedleInfo.Controls.Add(this.rad_Replace);
            this.grp_NeedleInfo.Controls.Add(this.rad_Remove);
            this.grp_NeedleInfo.Controls.Add(this.rad_Place);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Index);
            this.grp_NeedleInfo.Controls.Add(this.txt_Index);
            this.grp_NeedleInfo.Controls.Add(this.chk_Enable);
            this.grp_NeedleInfo.Controls.Add(this.chk_Display);
            this.grp_NeedleInfo.Controls.Add(this.txt_Diameter);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Diameter);
            this.grp_NeedleInfo.Controls.Add(this.txt_PosX);
            this.grp_NeedleInfo.Controls.Add(this.txt_PosY);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Pos);
            this.grp_NeedleInfo.Controls.Add(this.txt_Id);
            this.grp_NeedleInfo.Controls.Add(this.lbl_Id);
            this.grp_NeedleInfo.Controls.Add(this.label1);
            this.grp_NeedleInfo.Controls.Add(this.txt_Name);
            this.grp_NeedleInfo.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grp_NeedleInfo.Location = new System.Drawing.Point(19, 53);
            this.grp_NeedleInfo.Margin = new System.Windows.Forms.Padding(2);
            this.grp_NeedleInfo.Name = "grp_NeedleInfo";
            this.grp_NeedleInfo.Padding = new System.Windows.Forms.Padding(2);
            this.grp_NeedleInfo.Size = new System.Drawing.Size(449, 389);
            this.grp_NeedleInfo.TabIndex = 10;
            this.grp_NeedleInfo.TabStop = false;
            this.grp_NeedleInfo.Text = "植針資訊";
            // 
            // rad_Replace
            // 
            this.rad_Replace.AutoSize = true;
            this.rad_Replace.Location = new System.Drawing.Point(330, 130);
            this.rad_Replace.Name = "rad_Replace";
            this.rad_Replace.Size = new System.Drawing.Size(108, 47);
            this.rad_Replace.TabIndex = 2;
            this.rad_Replace.TabStop = true;
            this.rad_Replace.Text = "置換";
            this.rad_Replace.UseVisualStyleBackColor = true;
            // 
            // rad_Remove
            // 
            this.rad_Remove.AutoSize = true;
            this.rad_Remove.Location = new System.Drawing.Point(330, 86);
            this.rad_Remove.Name = "rad_Remove";
            this.rad_Remove.Size = new System.Drawing.Size(108, 47);
            this.rad_Remove.TabIndex = 1;
            this.rad_Remove.TabStop = true;
            this.rad_Remove.Text = "移除";
            this.rad_Remove.UseVisualStyleBackColor = true;
            // 
            // rad_Place
            // 
            this.rad_Place.AutoSize = true;
            this.rad_Place.Location = new System.Drawing.Point(330, 42);
            this.rad_Place.Name = "rad_Place";
            this.rad_Place.Size = new System.Drawing.Size(108, 47);
            this.rad_Place.TabIndex = 0;
            this.rad_Place.TabStop = true;
            this.rad_Place.Text = "放置";
            this.rad_Place.UseVisualStyleBackColor = true;
            // 
            // lbl_Index
            // 
            this.lbl_Index.AutoSize = true;
            this.lbl_Index.Location = new System.Drawing.Point(4, 47);
            this.lbl_Index.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Index.Name = "lbl_Index";
            this.lbl_Index.Size = new System.Drawing.Size(121, 43);
            this.lbl_Index.TabIndex = 22;
            this.lbl_Index.Text = "流水號";
            // 
            // txt_Index
            // 
            this.txt_Index.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Index.Location = new System.Drawing.Point(119, 43);
            this.txt_Index.Name = "txt_Index";
            this.txt_Index.ReadOnly = true;
            this.txt_Index.Size = new System.Drawing.Size(163, 50);
            this.txt_Index.TabIndex = 23;
            // 
            // chk_Enable
            // 
            this.chk_Enable.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_Enable.AutoSize = true;
            this.chk_Enable.Location = new System.Drawing.Point(330, 267);
            this.chk_Enable.Margin = new System.Windows.Forms.Padding(2);
            this.chk_Enable.Name = "chk_Enable";
            this.chk_Enable.Size = new System.Drawing.Size(97, 53);
            this.chk_Enable.TabIndex = 21;
            this.chk_Enable.Text = "啟用";
            this.chk_Enable.UseVisualStyleBackColor = true;
            // 
            // chk_Display
            // 
            this.chk_Display.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_Display.AutoSize = true;
            this.chk_Display.Location = new System.Drawing.Point(330, 204);
            this.chk_Display.Margin = new System.Windows.Forms.Padding(2);
            this.chk_Display.Name = "chk_Display";
            this.chk_Display.Size = new System.Drawing.Size(97, 53);
            this.chk_Display.TabIndex = 20;
            this.chk_Display.Text = "顯示";
            this.chk_Display.UseVisualStyleBackColor = true;
            this.chk_Display.CheckedChanged += new System.EventHandler(this.chk_Display_CheckedChanged);
            // 
            // txt_Diameter
            // 
            this.txt_Diameter.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Diameter.Location = new System.Drawing.Point(96, 323);
            this.txt_Diameter.Name = "txt_Diameter";
            this.txt_Diameter.ReadOnly = true;
            this.txt_Diameter.Size = new System.Drawing.Size(198, 50);
            this.txt_Diameter.TabIndex = 16;
            // 
            // lbl_Diameter
            // 
            this.lbl_Diameter.AutoSize = true;
            this.lbl_Diameter.Location = new System.Drawing.Point(4, 326);
            this.lbl_Diameter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Diameter.Name = "lbl_Diameter";
            this.lbl_Diameter.Size = new System.Drawing.Size(87, 43);
            this.lbl_Diameter.TabIndex = 15;
            this.lbl_Diameter.Text = "直徑";
            // 
            // txt_PosX
            // 
            this.txt_PosX.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_PosX.Location = new System.Drawing.Point(96, 211);
            this.txt_PosX.Name = "txt_PosX";
            this.txt_PosX.ReadOnly = true;
            this.txt_PosX.Size = new System.Drawing.Size(198, 50);
            this.txt_PosX.TabIndex = 14;
            // 
            // txt_PosY
            // 
            this.txt_PosY.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_PosY.Location = new System.Drawing.Point(96, 267);
            this.txt_PosY.Name = "txt_PosY";
            this.txt_PosY.ReadOnly = true;
            this.txt_PosY.Size = new System.Drawing.Size(198, 50);
            this.txt_PosY.TabIndex = 13;
            // 
            // lbl_Pos
            // 
            this.lbl_Pos.AutoSize = true;
            this.lbl_Pos.Location = new System.Drawing.Point(4, 214);
            this.lbl_Pos.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Pos.Name = "lbl_Pos";
            this.lbl_Pos.Size = new System.Drawing.Size(87, 43);
            this.lbl_Pos.TabIndex = 12;
            this.lbl_Pos.Text = "座標";
            // 
            // txt_Id
            // 
            this.txt_Id.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txt_Id.Location = new System.Drawing.Point(96, 155);
            this.txt_Id.Name = "txt_Id";
            this.txt_Id.Size = new System.Drawing.Size(198, 50);
            this.txt_Id.TabIndex = 11;
            // 
            // lbl_Id
            // 
            this.lbl_Id.AutoSize = true;
            this.lbl_Id.Location = new System.Drawing.Point(4, 158);
            this.lbl_Id.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Id.Name = "lbl_Id";
            this.lbl_Id.Size = new System.Drawing.Size(87, 43);
            this.lbl_Id.TabIndex = 10;
            this.lbl_Id.Text = "編號";
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1281, 978);
            this.Controls.Add(this.grp_NeedleInfo);
            this.Controls.Add(this.pic_Needles);
            this.Controls.Add(this.lbl_MousePos);
            this.Controls.Add(this.dgv_Needles);
            this.Controls.Add(this.menu_NeedleViewer);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MainMenuStrip = this.menu_NeedleViewer;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Frm_Main";
            this.Text = "NeedleViewer";
            this.Load += new System.EventHandler(this.Frm_Main_Load);
            this.SizeChanged += new System.EventHandler(this.Frm_Main_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Frm_Main_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Needles)).EndInit();
            this.menu_NeedleViewer.ResumeLayout(false);
            this.menu_NeedleViewer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Needles)).EndInit();
            this.grp_NeedleInfo.ResumeLayout(false);
            this.grp_NeedleInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void Initialize_grp_NeedleInfo_ChildControlChanged_Listener(GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.TextChanged += grp_NeedleInfo_ChildControlChanged;
                        break;

                    case RadioButton radioButton:
                        radioButton.CheckedChanged += grp_NeedleInfo_ChildControlChanged;
                        break;

                    case CheckBox checkBox:
                        checkBox.CheckedChanged += grp_NeedleInfo_ChildControlChanged;
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
        private ColumnHeader col_PointPos1;
        private DataGridView dgv_Needles;
        private Label lbl_MousePos;
        private MenuStrip menu_NeedleViewer;
        private ToolStripMenuItem tsmi_File;
        private ToolStripMenuItem tsmi_OpenFile;
        private ToolStripMenuItem tsmi_SaveFile;
        private PictureBox pic_Needles;
        private TextBox txt_Name;
        private Label label1;
        private GroupBox grp_NeedleInfo;
        private TextBox txt_Id;
        private Label lbl_Id;
        private Label lbl_Pos;
        private TextBox txt_Diameter;
        private Label lbl_Diameter;
        private TextBox txt_PosX;
        private TextBox txt_PosY;
        private TextBox txt_Index;
        private Label lbl_Index;
        private CheckBox chk_Enable;
        private CheckBox chk_Display;
        private DataGridViewTextBoxColumn Index;
        private DataGridViewTextBoxColumn ColumnName;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn PosX;
        private DataGridViewTextBoxColumn PosY;
        private DataGridViewTextBoxColumn Diameter;
        private DataGridViewTextBoxColumn Place;
        private DataGridViewTextBoxColumn Remove;
        private DataGridViewTextBoxColumn Replace;
        private DataGridViewTextBoxColumn Display;
        private DataGridViewTextBoxColumn Enable;
        private DataGridViewTextBoxColumn Reserve1;
        private DataGridViewTextBoxColumn Reserve2;
        private DataGridViewTextBoxColumn Reserve3;
        private DataGridViewTextBoxColumn Reserve4;
        private DataGridViewTextBoxColumn Reserve5;
        private RadioButton rad_Place;
        private RadioButton rad_Replace;
        private RadioButton rad_Remove;
    }
}
