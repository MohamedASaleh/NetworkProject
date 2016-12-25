namespace NetworkProject
{
    partial class WinningForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinningForm));
            this.Winner = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Winner
            // 
            this.Winner.AutoSize = true;
            this.Winner.Font = new System.Drawing.Font("Tahoma", 48F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Winner.ForeColor = System.Drawing.Color.DarkRed;
            this.Winner.Location = new System.Drawing.Point(12, 9);
            this.Winner.Name = "Winner";
            this.Winner.Size = new System.Drawing.Size(528, 77);
            this.Winner.TabIndex = 0;
            this.Winner.Text = "The Winner Is: ";
            // 
            // WinningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 113);
            this.Controls.Add(this.Winner);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WinningForm";
            this.Text = "WinningForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinningForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Winner;
    }
}