namespace DarkDemo
{
    partial class reportsprint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(reportsprint));
            this.dataSet1BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSet1 = new DarkDemo.DataSet1();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.bunifuPanel1 = new Bunifu.UI.WinForms.BunifuPanel();
            this.bpanel1 = new Bunifu.UI.WinForms.BunifuPanel();
            this.datetimenow = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.lblsection = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.bunifuLabel1 = new Bunifu.UI.WinForms.BunifuLabel();
            this.btnPrint = new Guna.UI2.WinForms.Guna2Button();
            this.cmbsection = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cmbeventfilter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.bunifuImageButton1 = new Bunifu.UI.WinForms.BunifuImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.bpanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataSet1BindingSource
            // 
            this.dataSet1BindingSource.DataSource = this.dataSet1;
            this.dataSet1BindingSource.Position = 0;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "DataSet1";
            this.dataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.dataSet1BindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "DarkDemo.Report1.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(34, 107);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(1000, 613);
            this.reportViewer1.TabIndex = 0;
            this.reportViewer1.Load += new System.EventHandler(this.reportViewer1_Load);
            // 
            // bunifuPanel1
            // 
            this.bunifuPanel1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(45)))));
            this.bunifuPanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bunifuPanel1.BackgroundImage")));
            this.bunifuPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bunifuPanel1.BorderColor = System.Drawing.Color.Transparent;
            this.bunifuPanel1.BorderRadius = 12;
            this.bunifuPanel1.BorderThickness = 1;
            this.bunifuPanel1.Location = new System.Drawing.Point(10, 93);
            this.bunifuPanel1.Name = "bunifuPanel1";
            this.bunifuPanel1.ShowBorders = true;
            this.bunifuPanel1.Size = new System.Drawing.Size(1048, 685);
            this.bunifuPanel1.TabIndex = 1;
            // 
            // bpanel1
            // 
            this.bpanel1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(45)))));
            this.bpanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bpanel1.BackgroundImage")));
            this.bpanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bpanel1.BorderColor = System.Drawing.Color.Transparent;
            this.bpanel1.BorderRadius = 12;
            this.bpanel1.BorderThickness = 1;
            this.bpanel1.Controls.Add(this.datetimenow);
            this.bpanel1.Controls.Add(this.lblsection);
            this.bpanel1.Controls.Add(this.bunifuLabel1);
            this.bpanel1.Controls.Add(this.btnPrint);
            this.bpanel1.Controls.Add(this.cmbsection);
            this.bpanel1.Controls.Add(this.cmbeventfilter);
            this.bpanel1.Location = new System.Drawing.Point(10, 43);
            this.bpanel1.Name = "bpanel1";
            this.bpanel1.ShowBorders = true;
            this.bpanel1.Size = new System.Drawing.Size(1044, 235);
            this.bpanel1.TabIndex = 2;
            // 
            // datetimenow
            // 
            this.datetimenow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(45)))));
            this.datetimenow.BorderRadius = 20;
            this.datetimenow.Checked = true;
            this.datetimenow.FillColor = System.Drawing.Color.White;
            this.datetimenow.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datetimenow.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.datetimenow.Location = new System.Drawing.Point(24, 4);
            this.datetimenow.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.datetimenow.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.datetimenow.Name = "datetimenow";
            this.datetimenow.Size = new System.Drawing.Size(208, 40);
            this.datetimenow.TabIndex = 10;
            this.datetimenow.Value = new System.DateTime(2024, 10, 31, 15, 4, 15, 370);
            // 
            // lblsection
            // 
            this.lblsection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.lblsection.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblsection.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblsection.Location = new System.Drawing.Point(786, 18);
            this.lblsection.Name = "lblsection";
            this.lblsection.Size = new System.Drawing.Size(58, 18);
            this.lblsection.TabIndex = 9;
            this.lblsection.Text = "SECTION";
            // 
            // bunifuLabel1
            // 
            this.bunifuLabel1.AllowParentOverrides = false;
            this.bunifuLabel1.AutoEllipsis = false;
            this.bunifuLabel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.bunifuLabel1.CursorType = System.Windows.Forms.Cursors.Default;
            this.bunifuLabel1.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuLabel1.ForeColor = System.Drawing.Color.White;
            this.bunifuLabel1.Location = new System.Drawing.Point(478, 10);
            this.bunifuLabel1.Name = "bunifuLabel1";
            this.bunifuLabel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.bunifuLabel1.Size = new System.Drawing.Size(91, 30);
            this.bunifuLabel1.TabIndex = 8;
            this.bunifuLabel1.Text = "Filter By:";
            this.bunifuLabel1.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.bunifuLabel1.TextFormat = Bunifu.UI.WinForms.BunifuLabel.TextFormattingOptions.Default;
            // 
            // btnPrint
            // 
            this.btnPrint.Animated = true;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.btnPrint.BorderRadius = 20;
            this.btnPrint.BorderThickness = 1;
            this.btnPrint.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.btnPrint.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPrint.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPrint.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPrint.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPrint.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.btnPrint.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnPrint.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(170)))), ((int)(((byte)(231)))));
            this.btnPrint.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(170)))), ((int)(((byte)(231)))));
            this.btnPrint.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnPrint.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPrint.Location = new System.Drawing.Point(894, 7);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(139, 37);
            this.btnPrint.TabIndex = 10;
            this.btnPrint.Text = "PRINT";
            this.btnPrint.TextOffset = new System.Drawing.Point(1, 0);
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // cmbsection
            // 
            this.cmbsection.BackColor = System.Drawing.Color.Transparent;
            this.cmbsection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.cmbsection.BorderRadius = 20;
            this.cmbsection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbsection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbsection.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.cmbsection.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbsection.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbsection.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbsection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbsection.ItemHeight = 30;
            this.cmbsection.Location = new System.Drawing.Point(745, 9);
            this.cmbsection.Name = "cmbsection";
            this.cmbsection.Size = new System.Drawing.Size(139, 36);
            this.cmbsection.TabIndex = 10;
            this.cmbsection.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmbsection.SelectedIndexChanged += new System.EventHandler(this.cmbsection_SelectedIndexChanged);
            // 
            // cmbeventfilter
            // 
            this.cmbeventfilter.BackColor = System.Drawing.Color.Transparent;
            this.cmbeventfilter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.cmbeventfilter.BorderRadius = 20;
            this.cmbeventfilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbeventfilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbeventfilter.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.cmbeventfilter.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbeventfilter.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbeventfilter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbeventfilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbeventfilter.ItemHeight = 30;
            this.cmbeventfilter.Location = new System.Drawing.Point(596, 10);
            this.cmbeventfilter.Name = "cmbeventfilter";
            this.cmbeventfilter.Size = new System.Drawing.Size(139, 36);
            this.cmbeventfilter.TabIndex = 9;
            this.cmbeventfilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmbeventfilter.SelectedIndexChanged += new System.EventHandler(this.cmbeventfilter_SelectedIndexChanged);
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.ActiveImage = null;
            this.bunifuImageButton1.AllowAnimations = true;
            this.bunifuImageButton1.AllowBuffering = false;
            this.bunifuImageButton1.AllowToggling = false;
            this.bunifuImageButton1.AllowZooming = true;
            this.bunifuImageButton1.AllowZoomingOnFocus = false;
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.bunifuImageButton1.ErrorImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.ErrorImage")));
            this.bunifuImageButton1.FadeWhenInactive = false;
            this.bunifuImageButton1.Flip = Bunifu.UI.WinForms.BunifuImageButton.FlipOrientation.Normal;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.ImageLocation = null;
            this.bunifuImageButton1.ImageMargin = 10;
            this.bunifuImageButton1.ImageSize = new System.Drawing.Size(25, 24);
            this.bunifuImageButton1.ImageZoomSize = new System.Drawing.Size(35, 34);
            this.bunifuImageButton1.InitialImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.InitialImage")));
            this.bunifuImageButton1.Location = new System.Drawing.Point(1023, 3);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Rotation = 0;
            this.bunifuImageButton1.ShowActiveImage = true;
            this.bunifuImageButton1.ShowCursorChanges = true;
            this.bunifuImageButton1.ShowImageBorders = true;
            this.bunifuImageButton1.ShowSizeMarkers = false;
            this.bunifuImageButton1.Size = new System.Drawing.Size(35, 34);
            this.bunifuImageButton1.TabIndex = 3;
            this.bunifuImageButton1.ToolTipText = "";
            this.bunifuImageButton1.WaitOnLoad = false;
            this.bunifuImageButton1.Zoom = 10;
            this.bunifuImageButton1.ZoomSpeed = 10;
            this.bunifuImageButton1.Click += new System.EventHandler(this.bunifuImageButton1_Click);
            // 
            // reportsprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(1067, 762);
            this.Controls.Add(this.bunifuImageButton1);
            this.Controls.Add(this.reportViewer1);
            this.Controls.Add(this.bunifuPanel1);
            this.Controls.Add(this.bpanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "reportsprint";
            this.Text = "reportsprint";
            this.Load += new System.EventHandler(this.reportsprint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.bpanel1.ResumeLayout(false);
            this.bpanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource dataSet1BindingSource;
        private DataSet1 dataSet1;
        private Bunifu.UI.WinForms.BunifuPanel bunifuPanel1;
        private Bunifu.UI.WinForms.BunifuPanel bpanel1;
        private Bunifu.UI.WinForms.BunifuLabel bunifuLabel1;
        private Guna.UI2.WinForms.Guna2Button btnPrint;
        private Guna.UI2.WinForms.Guna2ComboBox cmbsection;
        private Guna.UI2.WinForms.Guna2ComboBox cmbeventfilter;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblsection;
        private Bunifu.UI.WinForms.BunifuImageButton bunifuImageButton1;
        private Guna.UI2.WinForms.Guna2DateTimePicker datetimenow;
    }
}