namespace NetworkProject
{
    partial class GameStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameStart));
            this.btnStartAsServer = new System.Windows.Forms.Button();
            this.btnJoinAsClient = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartAsServer
            // 
            this.btnStartAsServer.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartAsServer.Location = new System.Drawing.Point(39, 44);
            this.btnStartAsServer.Name = "btnStartAsServer";
            this.btnStartAsServer.Size = new System.Drawing.Size(263, 84);
            this.btnStartAsServer.TabIndex = 0;
            this.btnStartAsServer.Text = "Start As Server";
            this.btnStartAsServer.UseVisualStyleBackColor = true;
            this.btnStartAsServer.Click += new System.EventHandler(this.btnStartAsServer_Click);
            // 
            // btnJoinAsClient
            // 
            this.btnJoinAsClient.Enabled = false;
            this.btnJoinAsClient.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJoinAsClient.Location = new System.Drawing.Point(39, 177);
            this.btnJoinAsClient.Name = "btnJoinAsClient";
            this.btnJoinAsClient.Size = new System.Drawing.Size(263, 84);
            this.btnJoinAsClient.TabIndex = 1;
            this.btnJoinAsClient.Text = "Join As Client";
            this.btnJoinAsClient.UseVisualStyleBackColor = true;
            this.btnJoinAsClient.Click += new System.EventHandler(this.btnJoinAsClient_Click);
            // 
            // GameStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 315);
            this.Controls.Add(this.btnJoinAsClient);
            this.Controls.Add(this.btnStartAsServer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameStart";
            this.Text = "Network Project";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartAsServer;
        private System.Windows.Forms.Button btnJoinAsClient;
    }
}

