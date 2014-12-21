namespace Eca.Spikes.WinFormsApplication.Forms
{
    partial class DrawingExamples
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
            this.drawGraphicsButton = new System.Windows.Forms.Button();
            this.drawPictureButton = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.drawImageOnFormButton = new System.Windows.Forms.Button();
            this.saveGraphicsButton = new System.Windows.Forms.Button();
            this.updateGraphicsButton = new System.Windows.Forms.Button();
            this.drawModifiedIcon = new System.Windows.Forms.Button();
            this.drawTextButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // drawGraphicsButton
            // 
            this.drawGraphicsButton.Location = new System.Drawing.Point(13, 272);
            this.drawGraphicsButton.Name = "drawGraphicsButton";
            this.drawGraphicsButton.Size = new System.Drawing.Size(87, 23);
            this.drawGraphicsButton.TabIndex = 0;
            this.drawGraphicsButton.Text = "Draw Graphics";
            this.drawGraphicsButton.UseVisualStyleBackColor = true;
            this.drawGraphicsButton.Click += new System.EventHandler(this.drawGraphicsButton_Click);
            // 
            // drawPictureButton
            // 
            this.drawPictureButton.Location = new System.Drawing.Point(107, 272);
            this.drawPictureButton.Name = "drawPictureButton";
            this.drawPictureButton.Size = new System.Drawing.Size(75, 23);
            this.drawPictureButton.TabIndex = 1;
            this.drawPictureButton.Text = "Draw Image";
            this.drawPictureButton.UseVisualStyleBackColor = true;
            this.drawPictureButton.Click += new System.EventHandler(this.drawPictureButton_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(13, 13);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(419, 253);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.Visible = false;
            // 
            // drawImageOnFormButton
            // 
            this.drawImageOnFormButton.Location = new System.Drawing.Point(192, 272);
            this.drawImageOnFormButton.Name = "drawImageOnFormButton";
            this.drawImageOnFormButton.Size = new System.Drawing.Size(123, 23);
            this.drawImageOnFormButton.TabIndex = 3;
            this.drawImageOnFormButton.Text = "Draw Image On Form";
            this.drawImageOnFormButton.UseVisualStyleBackColor = true;
            this.drawImageOnFormButton.Click += new System.EventHandler(this.drawImageOnFormButton_Click);
            // 
            // saveGraphicsButton
            // 
            this.saveGraphicsButton.Location = new System.Drawing.Point(322, 272);
            this.saveGraphicsButton.Name = "saveGraphicsButton";
            this.saveGraphicsButton.Size = new System.Drawing.Size(110, 23);
            this.saveGraphicsButton.TabIndex = 4;
            this.saveGraphicsButton.Text = "Save Graphics";
            this.saveGraphicsButton.UseVisualStyleBackColor = true;
            this.saveGraphicsButton.Click += new System.EventHandler(this.saveGraphicsButton_Click);
            // 
            // updateGraphicsButton
            // 
            this.updateGraphicsButton.Location = new System.Drawing.Point(322, 302);
            this.updateGraphicsButton.Name = "updateGraphicsButton";
            this.updateGraphicsButton.Size = new System.Drawing.Size(110, 23);
            this.updateGraphicsButton.TabIndex = 5;
            this.updateGraphicsButton.Text = "Update Graphics";
            this.updateGraphicsButton.UseVisualStyleBackColor = true;
            this.updateGraphicsButton.Click += new System.EventHandler(this.updateGraphicsButton_Click);
            // 
            // drawModifiedIcon
            // 
            this.drawModifiedIcon.Location = new System.Drawing.Point(192, 302);
            this.drawModifiedIcon.Name = "drawModifiedIcon";
            this.drawModifiedIcon.Size = new System.Drawing.Size(123, 23);
            this.drawModifiedIcon.TabIndex = 6;
            this.drawModifiedIcon.Text = "Draw modified Icon";
            this.drawModifiedIcon.UseVisualStyleBackColor = true;
            this.drawModifiedIcon.Click += new System.EventHandler(this.drawModifiedIcon_Click);
            // 
            // drawTextButton
            // 
            this.drawTextButton.Location = new System.Drawing.Point(107, 302);
            this.drawTextButton.Name = "drawTextButton";
            this.drawTextButton.Size = new System.Drawing.Size(75, 23);
            this.drawTextButton.TabIndex = 7;
            this.drawTextButton.Text = "Draw Text";
            this.drawTextButton.UseVisualStyleBackColor = true;
            this.drawTextButton.Click += new System.EventHandler(this.drawTextButton_Click);
            // 
            // DrawingExamples
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 334);
            this.Controls.Add(this.drawTextButton);
            this.Controls.Add(this.drawModifiedIcon);
            this.Controls.Add(this.updateGraphicsButton);
            this.Controls.Add(this.saveGraphicsButton);
            this.Controls.Add(this.drawImageOnFormButton);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.drawPictureButton);
            this.Controls.Add(this.drawGraphicsButton);
            this.Name = "DrawingExamples";
            this.Text = "DrawingExamples";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingExamples_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button drawGraphicsButton;
        private System.Windows.Forms.Button drawPictureButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button drawImageOnFormButton;
        private System.Windows.Forms.Button saveGraphicsButton;
        private System.Windows.Forms.Button updateGraphicsButton;
        private System.Windows.Forms.Button drawModifiedIcon;
        private System.Windows.Forms.Button drawTextButton;
    }
}