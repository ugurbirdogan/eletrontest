namespace GMPInfoteks.controls
{
    partial class ucDisplay
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdPLU = new System.Windows.Forms.Button();
            this.txtDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdPLU
            // 
            this.cmdPLU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.cmdPLU.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmdPLU.FlatAppearance.BorderSize = 0;
            this.cmdPLU.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdPLU.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cmdPLU.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.cmdPLU.Location = new System.Drawing.Point(0, 0);
            this.cmdPLU.Name = "cmdPLU";
            this.cmdPLU.Size = new System.Drawing.Size(24, 38);
            this.cmdPLU.TabIndex = 173;
            this.cmdPLU.UseVisualStyleBackColor = false;
            this.cmdPLU.Visible = false;
            this.cmdPLU.Click += new System.EventHandler(this.cmdPLU_Click);
            // 
            // txtDisplay
            // 
            this.txtDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDisplay.Font = new System.Drawing.Font("Segoe UI Symbol", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisplay.ForeColor = System.Drawing.Color.DimGray;
            this.txtDisplay.Location = new System.Drawing.Point(24, 0);
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtDisplay.Size = new System.Drawing.Size(528, 38);
            this.txtDisplay.TabIndex = 174;
            // 
            // ucDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.cmdPLU);
            this.Name = "ucDisplay";
            this.Size = new System.Drawing.Size(552, 38);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdPLU;
        private System.Windows.Forms.Label txtDisplay;


    }
}
