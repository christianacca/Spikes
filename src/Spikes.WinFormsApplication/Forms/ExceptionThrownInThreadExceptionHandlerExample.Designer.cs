namespace Eca.Spikes.WinFormsApplication
{
    partial class ExceptionThrownInThreadExceptionHandlerExample
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
            this.triggerExceptionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // triggerExceptionButton
            // 
            this.triggerExceptionButton.Location = new System.Drawing.Point(28, 63);
            this.triggerExceptionButton.Name = "triggerExceptionButton";
            this.triggerExceptionButton.Size = new System.Drawing.Size(571, 38);
            this.triggerExceptionButton.TabIndex = 4;
            this.triggerExceptionButton.Text = "Exception thrown in Application.ThreadException handler";
            this.triggerExceptionButton.UseVisualStyleBackColor = true;
            this.triggerExceptionButton.Click += new System.EventHandler(this.triggerExceptionButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(569, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "To run this demo you must be running the compiled version of this app";
            // 
            // ExceptionThrownInThreadExceptionHandlerExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Ivory;
            this.ClientSize = new System.Drawing.Size(618, 214);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.triggerExceptionButton);
            this.Name = "ExceptionThrownInThreadExceptionHandlerExample";
            this.Text = "ExceptionThrownInThreadExceptionHandlerExample";
            this.Load += new System.EventHandler(this.ExceptionThrownInThreadExceptionHandlerExample_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button triggerExceptionButton;
        private System.Windows.Forms.Label label1;
    }
}