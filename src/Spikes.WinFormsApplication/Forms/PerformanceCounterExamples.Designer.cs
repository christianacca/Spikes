namespace Eca.Spikes.WinFormsApplication.Forms
{
    partial class PerformanceCounterExamples
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
            this.performanceCounter = new System.Diagnostics.PerformanceCounter();
            this.measureExceptionsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter)).BeginInit();
            this.SuspendLayout();
            // 
            // performanceCounter
            // 
            this.performanceCounter.CategoryName = ".NET CLR Exceptions";
            this.performanceCounter.CounterName = "# of Exceps Thrown";
            this.performanceCounter.InstanceName = "Eca.Spikes.WinFormsApplication.vshost";
            // 
            // measureExceptionsButton
            // 
            this.measureExceptionsButton.Location = new System.Drawing.Point(71, 25);
            this.measureExceptionsButton.Name = "measureExceptionsButton";
            this.measureExceptionsButton.Size = new System.Drawing.Size(147, 46);
            this.measureExceptionsButton.TabIndex = 0;
            this.measureExceptionsButton.Text = "Throw exceptions and display perf counter value";
            this.measureExceptionsButton.UseVisualStyleBackColor = true;
            this.measureExceptionsButton.Click += new System.EventHandler(this.measureExceptionsButton_Click);
            // 
            // PerformanceCounterExamples
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.measureExceptionsButton);
            this.Name = "PerformanceCounterExamples";
            this.Text = "PerformanceCounterExamples";
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Diagnostics.PerformanceCounter performanceCounter;
        private System.Windows.Forms.Button measureExceptionsButton;
    }
}