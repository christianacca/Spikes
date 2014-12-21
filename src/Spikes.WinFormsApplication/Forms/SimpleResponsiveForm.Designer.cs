namespace Eca.Spikes.WinFormsApplication.Forms
{
    partial class SimpleResponsiveForm
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
            this.doUnresponsiveWorkButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // doUnresponsiveWorkButton
            // 
            this.doUnresponsiveWorkButton.Location = new System.Drawing.Point(0, 12);
            this.doUnresponsiveWorkButton.Name = "doUnresponsiveWorkButton";
            this.doUnresponsiveWorkButton.Size = new System.Drawing.Size(421, 35);
            this.doUnresponsiveWorkButton.TabIndex = 0;
            this.doUnresponsiveWorkButton.Text = "Click to do unresponsive work. Then try moving another window briefly over this o" +
                "ne";
            this.doUnresponsiveWorkButton.UseVisualStyleBackColor = true;
            this.doUnresponsiveWorkButton.Click += new System.EventHandler(this.doUnresponsiveWorkButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 250);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(433, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // SimpleResponsiveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 273);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.doUnresponsiveWorkButton);
            this.Name = "SimpleResponsiveForm";
            this.Text = "SimpleResponsiveForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button doUnresponsiveWorkButton;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}