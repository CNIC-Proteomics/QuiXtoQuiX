namespace QuiXoT
{
    partial class OPfitWindow
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lBoxSteps = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lBoxSteps
            // 
            this.lBoxSteps.FormattingEnabled = true;
            this.lBoxSteps.Location = new System.Drawing.Point(13, 14);
            this.lBoxSteps.Name = "lBoxSteps";
            this.lBoxSteps.Size = new System.Drawing.Size(1122, 498);
            this.lBoxSteps.TabIndex = 0;
            // 
            // OPfitWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 536);
            this.Controls.Add(this.lBoxSteps);
            this.Name = "OPfitWindow";
            this.Text = "OPfitWindow";
            this.Load += new System.EventHandler(this.OPfitWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lBoxSteps;
    }
}