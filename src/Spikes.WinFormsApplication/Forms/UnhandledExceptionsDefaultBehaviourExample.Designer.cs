namespace Eca.Spikes.WinFormsApplication
{
    partial class UnhandledExceptionsDefaultBehaviourExample
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
            this.mainUiThreadUheButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.workerThreadUheButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainUiThreadUheButton
            // 
            this.mainUiThreadUheButton.Location = new System.Drawing.Point(16, 56);
            this.mainUiThreadUheButton.Name = "mainUiThreadUheButton";
            this.mainUiThreadUheButton.Size = new System.Drawing.Size(571, 36);
            this.mainUiThreadUheButton.TabIndex = 0;
            this.mainUiThreadUheButton.Text = "Unhandled exception in main UI thread";
            this.mainUiThreadUheButton.UseVisualStyleBackColor = true;
            this.mainUiThreadUheButton.Click += new System.EventHandler(this.mainUiThreadUheButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(569, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "To run this demo you must be running the compiled version of this app";
            // 
            // workerThreadUheButton
            // 
            this.workerThreadUheButton.Location = new System.Drawing.Point(16, 115);
            this.workerThreadUheButton.Name = "workerThreadUheButton";
            this.workerThreadUheButton.Size = new System.Drawing.Size(571, 36);
            this.workerThreadUheButton.TabIndex = 2;
            this.workerThreadUheButton.Text = "Unhandled exception in worker thread";
            this.workerThreadUheButton.UseVisualStyleBackColor = true;
            this.workerThreadUheButton.Click += new System.EventHandler(this.workerThreadUheButton_Click);
            // 
            // UnhandledExceptionsDefaultBehaviourExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSalmon;
            this.ClientSize = new System.Drawing.Size(607, 194);
            this.Controls.Add(this.workerThreadUheButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mainUiThreadUheButton);
            this.Name = "UnhandledExceptionsDefaultBehaviourExample";
            this.Text = "UnhandledExceptionsDefaultBehaviourExample";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mainUiThreadUheButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button workerThreadUheButton;
    }
}