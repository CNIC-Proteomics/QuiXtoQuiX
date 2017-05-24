namespace QuiXoT.Forms
{
    partial class frmInvisible
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInvisible));
            this.myRaw = new AxXRAWFILELib.AxXRawfile();
            ((System.ComponentModel.ISupportInitialize)(this.myRaw)).BeginInit();
            this.SuspendLayout();
            // 
            // myRaw
            // 
            this.myRaw.Enabled = true;
            this.myRaw.Location = new System.Drawing.Point(79, 12);
            this.myRaw.Name = "myRaw";
            this.myRaw.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("myRaw.OcxState")));
            this.myRaw.Size = new System.Drawing.Size(35, 33);
            this.myRaw.TabIndex = 0;
            // 
            // frmInvisible
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 71);
            this.Controls.Add(this.myRaw);
            this.Name = "frmInvisible";
            ((System.ComponentModel.ISupportInitialize)(this.myRaw)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        //private AxXRAWFILELib.AxXRawfile myRaw;


    }
}