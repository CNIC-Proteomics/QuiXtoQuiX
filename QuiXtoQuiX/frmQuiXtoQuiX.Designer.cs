namespace QuiXtoQuiX
{
    partial class frmMain
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
            this.btnGo = new System.Windows.Forms.Button();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txbRaw = new System.Windows.Forms.TextBox();
            this.lblRaw = new System.Windows.Forms.Label();
            this.lblNumScans = new System.Windows.Forms.Label();
            this.txbNumScans = new System.Windows.Forms.TextBox();
            this.rbnScansInFile = new System.Windows.Forms.RadioButton();
            this.rbnFindPeak = new System.Windows.Forms.RadioButton();
            this.rbnAround = new System.Windows.Forms.RadioButton();
            this.txbWidthChrom = new System.Windows.Forms.TextBox();
            this.lblWidthChrom = new System.Windows.Forms.Label();
            this.txbTolerance = new System.Windows.Forms.TextBox();
            this.lblTolerance = new System.Windows.Forms.Label();
            this.cbxWritePeakOnly = new System.Windows.Forms.CheckBox();
            this.txbDeltaRTorScansBeforeLeaving = new System.Windows.Forms.TextBox();
            this.lblDeltaRTorScansBeforeLeaving = new System.Windows.Forms.Label();
            this.txbRTstepOrNoiseRatio = new System.Windows.Forms.TextBox();
            this.lblRTstepOrNoiseRatio = new System.Windows.Forms.Label();
            this.txbSchema = new System.Windows.Forms.TextBox();
            this.lblSchema = new System.Windows.Forms.Label();
            this.cbxSchema = new System.Windows.Forms.CheckBox();
            this.gbxOptions = new System.Windows.Forms.GroupBox();
            this.gbxMass = new System.Windows.Forms.GroupBox();
            this.rbnExperimentalMass = new System.Windows.Forms.RadioButton();
            this.rbnTheoreticalMass = new System.Windows.Forms.RadioButton();
            this.txbCacheSize = new System.Windows.Forms.TextBox();
            this.lblCacheSize = new System.Windows.Forms.Label();
            this.rbnSmoothing = new System.Windows.Forms.RadioButton();
            this.tipRTstepOrNoiseRatio = new System.Windows.Forms.ToolTip(this.components);
            this.tipDeltaRTorScansBeforeLeaving = new System.Windows.Forms.ToolTip(this.components);
            this.tipNumScans = new System.Windows.Forms.ToolTip(this.components);
            this.tipWidthChrom = new System.Windows.Forms.ToolTip(this.components);
            this.tipTolerance = new System.Windows.Forms.ToolTip(this.components);
            this.tipWritePeakFileOnly = new System.Windows.Forms.ToolTip(this.components);
            this.txbFile = new System.Windows.Forms.TextBox();
            this.cbxFirstScan = new System.Windows.Forms.CheckBox();
            this.tipFirstScan = new System.Windows.Forms.ToolTip(this.components);
            this.gbx18O = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txb18OdeltaMassR = new System.Windows.Forms.TextBox();
            this.txb18OcharacterR = new System.Windows.Forms.TextBox();
            this.txb18OdeltaMassK = new System.Windows.Forms.TextBox();
            this.lbl18OdeltaMass = new System.Windows.Forms.Label();
            this.txb18OcharacterK = new System.Windows.Forms.TextBox();
            this.lbl18Ocharacter = new System.Windows.Forms.Label();
            this.tipExperimentalMass = new System.Windows.Forms.ToolTip(this.components);
            this.tipTheoreticalMass = new System.Windows.Forms.ToolTip(this.components);
            this.gbxOptions.SuspendLayout();
            this.gbxMass.SuspendLayout();
            this.gbx18O.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(370, 431);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(256, 61);
            this.btnGo.TabIndex = 8;
            this.btnGo.Text = "Go!";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // lblFile
            // 
            this.lblFile.AllowDrop = true;
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(15, 18);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(99, 13);
            this.lblFile.TabIndex = 1;
            this.lblFile.Text = "Original QuiXML file";
            this.lblFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbFile_DragDrop);
            this.lblFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txbFile_DragEnter);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 458);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "(Status)";
            // 
            // txbRaw
            // 
            this.txbRaw.AllowDrop = true;
            this.txbRaw.Location = new System.Drawing.Point(120, 41);
            this.txbRaw.Name = "txbRaw";
            this.txbRaw.Size = new System.Drawing.Size(490, 20);
            this.txbRaw.TabIndex = 1;
            this.txbRaw.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbRaw_DragDrop);
            this.txbRaw.DragEnter += new System.Windows.Forms.DragEventHandler(this.lblRaw_DragEnter);
            // 
            // lblRaw
            // 
            this.lblRaw.AllowDrop = true;
            this.lblRaw.AutoSize = true;
            this.lblRaw.Location = new System.Drawing.Point(15, 44);
            this.lblRaw.Name = "lblRaw";
            this.lblRaw.Size = new System.Drawing.Size(62, 13);
            this.lblRaw.TabIndex = 4;
            this.lblRaw.Text = "RAW folder";
            this.lblRaw.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbRaw_DragDrop);
            this.lblRaw.DragEnter += new System.Windows.Forms.DragEventHandler(this.lblRaw_DragEnter);
            // 
            // lblNumScans
            // 
            this.lblNumScans.Location = new System.Drawing.Point(278, 49);
            this.lblNumScans.Name = "lblNumScans";
            this.lblNumScans.Size = new System.Drawing.Size(243, 13);
            this.lblNumScans.TabIndex = 6;
            this.lblNumScans.Text = "number of full scans to take per identification scan";
            this.lblNumScans.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txbNumScans
            // 
            this.txbNumScans.Enabled = false;
            this.txbNumScans.Location = new System.Drawing.Point(527, 46);
            this.txbNumScans.Name = "txbNumScans";
            this.txbNumScans.Size = new System.Drawing.Size(70, 20);
            this.txbNumScans.TabIndex = 5;
            this.txbNumScans.Text = "0";
            // 
            // rbnScansInFile
            // 
            this.rbnScansInFile.AutoSize = true;
            this.rbnScansInFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnScansInFile.Location = new System.Drawing.Point(39, 69);
            this.rbnScansInFile.Name = "rbnScansInFile";
            this.rbnScansInFile.Size = new System.Drawing.Size(211, 17);
            this.rbnScansInFile.TabIndex = 1;
            this.rbnScansInFile.Text = "all full scans between first and last scan";
            this.rbnScansInFile.UseVisualStyleBackColor = true;
            this.rbnScansInFile.CheckedChanged += new System.EventHandler(this.rbnScansInFile_CheckedChanged);
            // 
            // rbnFindPeak
            // 
            this.rbnFindPeak.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnFindPeak.Location = new System.Drawing.Point(8, 111);
            this.rbnFindPeak.Name = "rbnFindPeak";
            this.rbnFindPeak.Size = new System.Drawing.Size(242, 44);
            this.rbnFindPeak.TabIndex = 3;
            this.rbnFindPeak.Text = "find the chromatographic peak using the sweep algorithm";
            this.rbnFindPeak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnFindPeak.UseVisualStyleBackColor = true;
            this.rbnFindPeak.CheckedChanged += new System.EventHandler(this.rbnFindPeak_CheckedChanged);
            // 
            // rbnAround
            // 
            this.rbnAround.AutoSize = true;
            this.rbnAround.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnAround.Location = new System.Drawing.Point(64, 96);
            this.rbnAround.Name = "rbnAround";
            this.rbnAround.Size = new System.Drawing.Size(186, 17);
            this.rbnAround.TabIndex = 2;
            this.rbnAround.Text = "just take N scans per MSMS scan";
            this.rbnAround.UseVisualStyleBackColor = true;
            this.rbnAround.CheckedChanged += new System.EventHandler(this.rbnAround_CheckedChanged);
            // 
            // txbWidthChrom
            // 
            this.txbWidthChrom.Enabled = false;
            this.txbWidthChrom.Location = new System.Drawing.Point(527, 19);
            this.txbWidthChrom.Name = "txbWidthChrom";
            this.txbWidthChrom.Size = new System.Drawing.Size(70, 20);
            this.txbWidthChrom.TabIndex = 4;
            this.txbWidthChrom.Text = "2";
            this.txbWidthChrom.Visible = false;
            // 
            // lblWidthChrom
            // 
            this.lblWidthChrom.Enabled = false;
            this.lblWidthChrom.Location = new System.Drawing.Point(308, 22);
            this.lblWidthChrom.Name = "lblWidthChrom";
            this.lblWidthChrom.Size = new System.Drawing.Size(213, 13);
            this.lblWidthChrom.TabIndex = 12;
            this.lblWidthChrom.Text = "width of chromatographic window (in mins)";
            this.lblWidthChrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblWidthChrom.Visible = false;
            // 
            // txbTolerance
            // 
            this.txbTolerance.Location = new System.Drawing.Point(527, 72);
            this.txbTolerance.Name = "txbTolerance";
            this.txbTolerance.Size = new System.Drawing.Size(70, 20);
            this.txbTolerance.TabIndex = 6;
            this.txbTolerance.Text = "10";
            // 
            // lblTolerance
            // 
            this.lblTolerance.Enabled = false;
            this.lblTolerance.Location = new System.Drawing.Point(365, 73);
            this.lblTolerance.Name = "lblTolerance";
            this.lblTolerance.Size = new System.Drawing.Size(156, 17);
            this.lblTolerance.TabIndex = 15;
            this.lblTolerance.Text = "mz tolerance (ppm)";
            this.lblTolerance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbxWritePeakOnly
            // 
            this.cbxWritePeakOnly.AutoSize = true;
            this.cbxWritePeakOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxWritePeakOnly.Enabled = false;
            this.cbxWritePeakOnly.Location = new System.Drawing.Point(446, 408);
            this.cbxWritePeakOnly.Name = "cbxWritePeakOnly";
            this.cbxWritePeakOnly.Size = new System.Drawing.Size(180, 17);
            this.cbxWritePeakOnly.TabIndex = 7;
            this.cbxWritePeakOnly.Text = "write peak file only (not the XML)";
            this.cbxWritePeakOnly.UseVisualStyleBackColor = true;
            this.cbxWritePeakOnly.CheckedChanged += new System.EventHandler(this.cbxWritePeakOnly_CheckedChanged);
            // 
            // txbDeltaRTorScansBeforeLeaving
            // 
            this.txbDeltaRTorScansBeforeLeaving.Enabled = false;
            this.txbDeltaRTorScansBeforeLeaving.Location = new System.Drawing.Point(527, 98);
            this.txbDeltaRTorScansBeforeLeaving.Name = "txbDeltaRTorScansBeforeLeaving";
            this.txbDeltaRTorScansBeforeLeaving.Size = new System.Drawing.Size(70, 20);
            this.txbDeltaRTorScansBeforeLeaving.TabIndex = 7;
            this.txbDeltaRTorScansBeforeLeaving.Text = "1";
            this.txbDeltaRTorScansBeforeLeaving.Visible = false;
            // 
            // lblDeltaRTorScansBeforeLeaving
            // 
            this.lblDeltaRTorScansBeforeLeaving.Enabled = false;
            this.lblDeltaRTorScansBeforeLeaving.Location = new System.Drawing.Point(341, 101);
            this.lblDeltaRTorScansBeforeLeaving.Name = "lblDeltaRTorScansBeforeLeaving";
            this.lblDeltaRTorScansBeforeLeaving.Size = new System.Drawing.Size(180, 17);
            this.lblDeltaRTorScansBeforeLeaving.TabIndex = 18;
            this.lblDeltaRTorScansBeforeLeaving.Text = "delta RT (in mins)";
            this.lblDeltaRTorScansBeforeLeaving.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDeltaRTorScansBeforeLeaving.Visible = false;
            // 
            // txbRTstepOrNoiseRatio
            // 
            this.txbRTstepOrNoiseRatio.Enabled = false;
            this.txbRTstepOrNoiseRatio.Location = new System.Drawing.Point(527, 124);
            this.txbRTstepOrNoiseRatio.Name = "txbRTstepOrNoiseRatio";
            this.txbRTstepOrNoiseRatio.Size = new System.Drawing.Size(70, 20);
            this.txbRTstepOrNoiseRatio.TabIndex = 8;
            this.txbRTstepOrNoiseRatio.Text = "0";
            this.txbRTstepOrNoiseRatio.Visible = false;
            // 
            // lblRTstepOrNoiseRatio
            // 
            this.lblRTstepOrNoiseRatio.Enabled = false;
            this.lblRTstepOrNoiseRatio.Location = new System.Drawing.Point(329, 127);
            this.lblRTstepOrNoiseRatio.Name = "lblRTstepOrNoiseRatio";
            this.lblRTstepOrNoiseRatio.Size = new System.Drawing.Size(192, 17);
            this.lblRTstepOrNoiseRatio.TabIndex = 20;
            this.lblRTstepOrNoiseRatio.Text = "RT step (in mins)";
            this.lblRTstepOrNoiseRatio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRTstepOrNoiseRatio.Visible = false;
            // 
            // txbSchema
            // 
            this.txbSchema.AllowDrop = true;
            this.txbSchema.Location = new System.Drawing.Point(120, 90);
            this.txbSchema.Name = "txbSchema";
            this.txbSchema.Size = new System.Drawing.Size(490, 20);
            this.txbSchema.TabIndex = 3;
            this.txbSchema.Visible = false;
            this.txbSchema.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbSchema_DragDrop);
            this.txbSchema.DragEnter += new System.Windows.Forms.DragEventHandler(this.txbSchema_DragEnter);
            // 
            // lblSchema
            // 
            this.lblSchema.AllowDrop = true;
            this.lblSchema.AutoSize = true;
            this.lblSchema.Location = new System.Drawing.Point(15, 93);
            this.lblSchema.Name = "lblSchema";
            this.lblSchema.Size = new System.Drawing.Size(85, 13);
            this.lblSchema.TabIndex = 23;
            this.lblSchema.Text = "QuiXML schema";
            this.lblSchema.Visible = false;
            this.lblSchema.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbSchema_DragDrop);
            this.lblSchema.DragEnter += new System.Windows.Forms.DragEventHandler(this.txbSchema_DragEnter);
            // 
            // cbxSchema
            // 
            this.cbxSchema.AutoSize = true;
            this.cbxSchema.Checked = true;
            this.cbxSchema.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxSchema.Location = new System.Drawing.Point(458, 67);
            this.cbxSchema.Name = "cbxSchema";
            this.cbxSchema.Size = new System.Drawing.Size(152, 17);
            this.cbxSchema.TabIndex = 2;
            this.cbxSchema.Text = "use default schema for HR";
            this.cbxSchema.UseVisualStyleBackColor = true;
            this.cbxSchema.CheckedChanged += new System.EventHandler(this.cbxSchema_CheckedChanged);
            // 
            // gbxOptions
            // 
            this.gbxOptions.Controls.Add(this.gbxMass);
            this.gbxOptions.Controls.Add(this.txbCacheSize);
            this.gbxOptions.Controls.Add(this.lblCacheSize);
            this.gbxOptions.Controls.Add(this.rbnSmoothing);
            this.gbxOptions.Controls.Add(this.rbnScansInFile);
            this.gbxOptions.Controls.Add(this.lblNumScans);
            this.gbxOptions.Controls.Add(this.txbNumScans);
            this.gbxOptions.Controls.Add(this.rbnFindPeak);
            this.gbxOptions.Controls.Add(this.rbnAround);
            this.gbxOptions.Controls.Add(this.txbRTstepOrNoiseRatio);
            this.gbxOptions.Controls.Add(this.lblWidthChrom);
            this.gbxOptions.Controls.Add(this.lblRTstepOrNoiseRatio);
            this.gbxOptions.Controls.Add(this.txbWidthChrom);
            this.gbxOptions.Controls.Add(this.txbDeltaRTorScansBeforeLeaving);
            this.gbxOptions.Controls.Add(this.lblDeltaRTorScansBeforeLeaving);
            this.gbxOptions.Controls.Add(this.lblTolerance);
            this.gbxOptions.Controls.Add(this.txbTolerance);
            this.gbxOptions.Location = new System.Drawing.Point(18, 116);
            this.gbxOptions.Name = "gbxOptions";
            this.gbxOptions.Size = new System.Drawing.Size(608, 236);
            this.gbxOptions.TabIndex = 4;
            this.gbxOptions.TabStop = false;
            this.gbxOptions.Text = "options";
            // 
            // gbxMass
            // 
            this.gbxMass.Controls.Add(this.rbnExperimentalMass);
            this.gbxMass.Controls.Add(this.rbnTheoreticalMass);
            this.gbxMass.Location = new System.Drawing.Point(440, 171);
            this.gbxMass.Name = "gbxMass";
            this.gbxMass.Size = new System.Drawing.Size(172, 75);
            this.gbxMass.TabIndex = 25;
            this.gbxMass.TabStop = false;
            this.gbxMass.Text = "mass used";
            // 
            // rbnExperimentalMass
            // 
            this.rbnExperimentalMass.AutoSize = true;
            this.rbnExperimentalMass.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnExperimentalMass.Checked = true;
            this.rbnExperimentalMass.Location = new System.Drawing.Point(33, 19);
            this.rbnExperimentalMass.Name = "rbnExperimentalMass";
            this.rbnExperimentalMass.Size = new System.Drawing.Size(131, 17);
            this.rbnExperimentalMass.TabIndex = 23;
            this.rbnExperimentalMass.TabStop = true;
            this.rbnExperimentalMass.Text = "use experimental mass";
            this.rbnExperimentalMass.UseVisualStyleBackColor = true;
            // 
            // rbnTheoreticalMass
            // 
            this.rbnTheoreticalMass.AutoSize = true;
            this.rbnTheoreticalMass.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnTheoreticalMass.Location = new System.Drawing.Point(43, 42);
            this.rbnTheoreticalMass.Name = "rbnTheoreticalMass";
            this.rbnTheoreticalMass.Size = new System.Drawing.Size(121, 17);
            this.rbnTheoreticalMass.TabIndex = 24;
            this.rbnTheoreticalMass.Text = "use theoretical mass";
            this.rbnTheoreticalMass.UseVisualStyleBackColor = true;
            // 
            // txbCacheSize
            // 
            this.txbCacheSize.Location = new System.Drawing.Point(527, 150);
            this.txbCacheSize.Name = "txbCacheSize";
            this.txbCacheSize.Size = new System.Drawing.Size(70, 20);
            this.txbCacheSize.TabIndex = 21;
            this.txbCacheSize.Text = "1000";
            // 
            // lblCacheSize
            // 
            this.lblCacheSize.Location = new System.Drawing.Point(329, 151);
            this.lblCacheSize.Name = "lblCacheSize";
            this.lblCacheSize.Size = new System.Drawing.Size(195, 17);
            this.lblCacheSize.TabIndex = 22;
            this.lblCacheSize.Text = "spectrum cache size";
            this.lblCacheSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rbnSmoothing
            // 
            this.rbnSmoothing.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnSmoothing.Checked = true;
            this.rbnSmoothing.Location = new System.Drawing.Point(8, 31);
            this.rbnSmoothing.Name = "rbnSmoothing";
            this.rbnSmoothing.Size = new System.Drawing.Size(242, 32);
            this.rbnSmoothing.TabIndex = 0;
            this.rbnSmoothing.TabStop = true;
            this.rbnSmoothing.Text = "find the chromatographic peak using the quadratic smoothing algorithm";
            this.rbnSmoothing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbnSmoothing.UseVisualStyleBackColor = true;
            // 
            // tipRTstepOrNoiseRatio
            // 
            this.tipRTstepOrNoiseRatio.AutomaticDelay = 1000;
            this.tipRTstepOrNoiseRatio.AutoPopDelay = 10000;
            this.tipRTstepOrNoiseRatio.BackColor = System.Drawing.Color.LightGreen;
            this.tipRTstepOrNoiseRatio.InitialDelay = 1000;
            this.tipRTstepOrNoiseRatio.IsBalloon = true;
            this.tipRTstepOrNoiseRatio.ReshowDelay = 100;
            this.tipRTstepOrNoiseRatio.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // tipDeltaRTorScansBeforeLeaving
            // 
            this.tipDeltaRTorScansBeforeLeaving.AutomaticDelay = 1000;
            this.tipDeltaRTorScansBeforeLeaving.AutoPopDelay = 10000;
            this.tipDeltaRTorScansBeforeLeaving.BackColor = System.Drawing.Color.LightGreen;
            this.tipDeltaRTorScansBeforeLeaving.InitialDelay = 1000;
            this.tipDeltaRTorScansBeforeLeaving.IsBalloon = true;
            this.tipDeltaRTorScansBeforeLeaving.ReshowDelay = 100;
            this.tipDeltaRTorScansBeforeLeaving.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // tipNumScans
            // 
            this.tipNumScans.AutomaticDelay = 1000;
            this.tipNumScans.AutoPopDelay = 10000;
            this.tipNumScans.BackColor = System.Drawing.Color.LightGreen;
            this.tipNumScans.InitialDelay = 1000;
            this.tipNumScans.IsBalloon = true;
            this.tipNumScans.ReshowDelay = 100;
            this.tipNumScans.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // tipWidthChrom
            // 
            this.tipWidthChrom.AutomaticDelay = 1000;
            this.tipWidthChrom.AutoPopDelay = 10000;
            this.tipWidthChrom.BackColor = System.Drawing.Color.LightGreen;
            this.tipWidthChrom.InitialDelay = 1000;
            this.tipWidthChrom.IsBalloon = true;
            this.tipWidthChrom.ReshowDelay = 100;
            this.tipWidthChrom.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // tipTolerance
            // 
            this.tipTolerance.AutomaticDelay = 1000;
            this.tipTolerance.AutoPopDelay = 10000;
            this.tipTolerance.BackColor = System.Drawing.Color.LightGreen;
            this.tipTolerance.InitialDelay = 1000;
            this.tipTolerance.IsBalloon = true;
            this.tipTolerance.ReshowDelay = 100;
            this.tipTolerance.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // tipWritePeakFileOnly
            // 
            this.tipWritePeakFileOnly.AutomaticDelay = 1000;
            this.tipWritePeakFileOnly.AutoPopDelay = 10000;
            this.tipWritePeakFileOnly.BackColor = System.Drawing.Color.LightGreen;
            this.tipWritePeakFileOnly.InitialDelay = 1000;
            this.tipWritePeakFileOnly.IsBalloon = true;
            this.tipWritePeakFileOnly.ReshowDelay = 100;
            this.tipWritePeakFileOnly.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // txbFile
            // 
            this.txbFile.AllowDrop = true;
            this.txbFile.Location = new System.Drawing.Point(120, 15);
            this.txbFile.Name = "txbFile";
            this.txbFile.Size = new System.Drawing.Size(490, 20);
            this.txbFile.TabIndex = 0;
            this.txbFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbFile_DragDrop);
            this.txbFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txbFile_DragEnter);
            // 
            // cbxFirstScan
            // 
            this.cbxFirstScan.AutoSize = true;
            this.cbxFirstScan.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxFirstScan.Location = new System.Drawing.Point(410, 385);
            this.cbxFirstScan.Name = "cbxFirstScan";
            this.cbxFirstScan.Size = new System.Drawing.Size(216, 17);
            this.cbxFirstScan.TabIndex = 6;
            this.cbxFirstScan.Text = "save graph of first peptide match in XML";
            this.cbxFirstScan.UseVisualStyleBackColor = true;
            // 
            // tipFirstScan
            // 
            this.tipFirstScan.AutomaticDelay = 1000;
            this.tipFirstScan.AutoPopDelay = 10000;
            this.tipFirstScan.BackColor = System.Drawing.Color.LightGreen;
            this.tipFirstScan.InitialDelay = 1000;
            this.tipFirstScan.IsBalloon = true;
            this.tipFirstScan.ReshowDelay = 100;
            this.tipFirstScan.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // gbx18O
            // 
            this.gbx18O.Controls.Add(this.label2);
            this.gbx18O.Controls.Add(this.label1);
            this.gbx18O.Controls.Add(this.txb18OdeltaMassR);
            this.gbx18O.Controls.Add(this.txb18OcharacterR);
            this.gbx18O.Controls.Add(this.txb18OdeltaMassK);
            this.gbx18O.Controls.Add(this.lbl18OdeltaMass);
            this.gbx18O.Controls.Add(this.txb18OcharacterK);
            this.gbx18O.Controls.Add(this.lbl18Ocharacter);
            this.gbx18O.Location = new System.Drawing.Point(18, 364);
            this.gbx18O.Name = "gbx18O";
            this.gbx18O.Size = new System.Drawing.Size(320, 91);
            this.gbx18O.TabIndex = 5;
            this.gbx18O.TabStop = false;
            this.gbx18O.Text = "18O extra options (provisional)";
            // 
            // label2
            // 
            this.label2.AllowDrop = true;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(231, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "R (arginine)";
            // 
            // label1
            // 
            this.label1.AllowDrop = true;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "K (lysine)";
            // 
            // txb18OdeltaMassR
            // 
            this.txb18OdeltaMassR.AllowDrop = true;
            this.txb18OdeltaMassR.Location = new System.Drawing.Point(213, 58);
            this.txb18OdeltaMassR.MaxLength = 30;
            this.txb18OdeltaMassR.Name = "txb18OdeltaMassR";
            this.txb18OdeltaMassR.Size = new System.Drawing.Size(95, 20);
            this.txb18OdeltaMassR.TabIndex = 3;
            this.txb18OdeltaMassR.Text = "4.008491";
            // 
            // txb18OcharacterR
            // 
            this.txb18OcharacterR.AllowDrop = true;
            this.txb18OcharacterR.Location = new System.Drawing.Point(213, 32);
            this.txb18OcharacterR.MaxLength = 1;
            this.txb18OcharacterR.Name = "txb18OcharacterR";
            this.txb18OcharacterR.Size = new System.Drawing.Size(95, 20);
            this.txb18OcharacterR.TabIndex = 2;
            // 
            // txb18OdeltaMassK
            // 
            this.txb18OdeltaMassK.AllowDrop = true;
            this.txb18OdeltaMassK.Location = new System.Drawing.Point(112, 58);
            this.txb18OdeltaMassK.MaxLength = 30;
            this.txb18OdeltaMassK.Name = "txb18OdeltaMassK";
            this.txb18OdeltaMassK.Size = new System.Drawing.Size(95, 20);
            this.txb18OdeltaMassK.TabIndex = 1;
            this.txb18OdeltaMassK.Text = "4.008491";
            // 
            // lbl18OdeltaMass
            // 
            this.lbl18OdeltaMass.AllowDrop = true;
            this.lbl18OdeltaMass.AutoSize = true;
            this.lbl18OdeltaMass.Location = new System.Drawing.Point(6, 61);
            this.lbl18OdeltaMass.Name = "lbl18OdeltaMass";
            this.lbl18OdeltaMass.Size = new System.Drawing.Size(103, 13);
            this.lbl18OdeltaMass.TabIndex = 32;
            this.lbl18OdeltaMass.Text = "18O label deltaMass";
            // 
            // txb18OcharacterK
            // 
            this.txb18OcharacterK.AllowDrop = true;
            this.txb18OcharacterK.Location = new System.Drawing.Point(112, 32);
            this.txb18OcharacterK.MaxLength = 1;
            this.txb18OcharacterK.Name = "txb18OcharacterK";
            this.txb18OcharacterK.Size = new System.Drawing.Size(95, 20);
            this.txb18OcharacterK.TabIndex = 0;
            // 
            // lbl18Ocharacter
            // 
            this.lbl18Ocharacter.AllowDrop = true;
            this.lbl18Ocharacter.AutoSize = true;
            this.lbl18Ocharacter.Location = new System.Drawing.Point(6, 35);
            this.lbl18Ocharacter.Name = "lbl18Ocharacter";
            this.lbl18Ocharacter.Size = new System.Drawing.Size(100, 13);
            this.lbl18Ocharacter.TabIndex = 30;
            this.lbl18Ocharacter.Text = "18O label character";
            // 
            // tipExperimentalMass
            // 
            this.tipExperimentalMass.AutomaticDelay = 1000;
            this.tipExperimentalMass.AutoPopDelay = 10000;
            this.tipExperimentalMass.BackColor = System.Drawing.Color.LightGreen;
            this.tipExperimentalMass.InitialDelay = 1000;
            this.tipExperimentalMass.IsBalloon = true;
            this.tipExperimentalMass.ReshowDelay = 100;
            this.tipExperimentalMass.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // tipTheoreticalMass
            // 
            this.tipTheoreticalMass.AutomaticDelay = 1000;
            this.tipTheoreticalMass.AutoPopDelay = 10000;
            this.tipTheoreticalMass.BackColor = System.Drawing.Color.LightGreen;
            this.tipTheoreticalMass.InitialDelay = 1000;
            this.tipTheoreticalMass.IsBalloon = true;
            this.tipTheoreticalMass.ReshowDelay = 100;
            this.tipTheoreticalMass.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnGo;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 512);
            this.Controls.Add(this.gbx18O);
            this.Controls.Add(this.cbxFirstScan);
            this.Controls.Add(this.gbxOptions);
            this.Controls.Add(this.cbxSchema);
            this.Controls.Add(this.txbSchema);
            this.Controls.Add(this.lblSchema);
            this.Controls.Add(this.cbxWritePeakOnly);
            this.Controls.Add(this.txbRaw);
            this.Controls.Add(this.lblRaw);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txbFile);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.btnGo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "QuiX to QuiX";
            this.gbxOptions.ResumeLayout(false);
            this.gbxOptions.PerformLayout();
            this.gbxMass.ResumeLayout(false);
            this.gbxMass.PerformLayout();
            this.gbx18O.ResumeLayout(false);
            this.gbx18O.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txbRaw;
        private System.Windows.Forms.Label lblRaw;
        private System.Windows.Forms.Label lblNumScans;
        private System.Windows.Forms.TextBox txbNumScans;
        private System.Windows.Forms.RadioButton rbnScansInFile;
        private System.Windows.Forms.RadioButton rbnFindPeak;
        private System.Windows.Forms.RadioButton rbnAround;
        private System.Windows.Forms.TextBox txbWidthChrom;
        private System.Windows.Forms.Label lblWidthChrom;
        private System.Windows.Forms.TextBox txbTolerance;
        private System.Windows.Forms.Label lblTolerance;
        private System.Windows.Forms.CheckBox cbxWritePeakOnly;
        private System.Windows.Forms.TextBox txbDeltaRTorScansBeforeLeaving;
        private System.Windows.Forms.Label lblDeltaRTorScansBeforeLeaving;
        private System.Windows.Forms.TextBox txbRTstepOrNoiseRatio;
        private System.Windows.Forms.Label lblRTstepOrNoiseRatio;
        private System.Windows.Forms.TextBox txbSchema;
        private System.Windows.Forms.Label lblSchema;
        private System.Windows.Forms.CheckBox cbxSchema;
        private System.Windows.Forms.GroupBox gbxOptions;
        private System.Windows.Forms.RadioButton rbnSmoothing;
        private System.Windows.Forms.ToolTip tipRTstepOrNoiseRatio;
        private System.Windows.Forms.ToolTip tipDeltaRTorScansBeforeLeaving;
        private System.Windows.Forms.ToolTip tipNumScans;
        private System.Windows.Forms.ToolTip tipWidthChrom;
        private System.Windows.Forms.ToolTip tipTolerance;
        private System.Windows.Forms.ToolTip tipWritePeakFileOnly;
        private System.Windows.Forms.TextBox txbFile;
        private System.Windows.Forms.CheckBox cbxFirstScan;
        private System.Windows.Forms.ToolTip tipFirstScan;
        private System.Windows.Forms.GroupBox gbx18O;
        private System.Windows.Forms.TextBox txb18OcharacterK;
        private System.Windows.Forms.Label lbl18Ocharacter;
        private System.Windows.Forms.TextBox txb18OdeltaMassK;
        private System.Windows.Forms.Label lbl18OdeltaMass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txb18OdeltaMassR;
        private System.Windows.Forms.TextBox txb18OcharacterR;
        private System.Windows.Forms.TextBox txbCacheSize;
        private System.Windows.Forms.Label lblCacheSize;
        private System .Windows .Forms .RadioButton rbnTheoreticalMass;
        private System .Windows .Forms .RadioButton rbnExperimentalMass;
        private System .Windows .Forms .ToolTip tipExperimentalMass;
        private System .Windows .Forms .ToolTip tipTheoreticalMass;
        private System.Windows.Forms.GroupBox gbxMass;
    }
}

