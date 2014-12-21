namespace Eca.Spikes.WinFormsApplication
{
    partial class MainForm
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
            this.demoUheButton = new System.Windows.Forms.Button();
            this.demoTrappingUheButton = new System.Windows.Forms.Button();
            this.demoExceptionInThreadExceptionHandlerButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.openDrawingExamplesButton = new System.Windows.Forms.Button();
            this.startMeInAnotherProcessButton = new System.Windows.Forms.Button();
            this.fileSystemSecurityTest = new System.Windows.Forms.Button();
            this.simpleResponsiveFormEg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.openPerformanceCounterExamplesButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // demoUheButton
            // 
            this.demoUheButton.Location = new System.Drawing.Point(6, 12);
            this.demoUheButton.Name = "demoUheButton";
            this.demoUheButton.Size = new System.Drawing.Size(443, 38);
            this.demoUheButton.TabIndex = 0;
            this.demoUheButton.Text = "Demonstrate default behaviour when unhandled exception is not trapped";
            this.demoUheButton.UseVisualStyleBackColor = true;
            this.demoUheButton.Click += new System.EventHandler(this.demoUheButton_Click);
            // 
            // demoTrappingUheButton
            // 
            this.demoTrappingUheButton.Location = new System.Drawing.Point(6, 56);
            this.demoTrappingUheButton.Name = "demoTrappingUheButton";
            this.demoTrappingUheButton.Size = new System.Drawing.Size(443, 38);
            this.demoTrappingUheButton.TabIndex = 1;
            this.demoTrappingUheButton.Text = "Demonstrate how to trap unhandled exception";
            this.demoTrappingUheButton.UseVisualStyleBackColor = true;
            this.demoTrappingUheButton.Click += new System.EventHandler(this.demoTrappingUheButton_Click);
            // 
            // demoExceptionInThreadExceptionHandlerButton
            // 
            this.demoExceptionInThreadExceptionHandlerButton.Location = new System.Drawing.Point(6, 100);
            this.demoExceptionInThreadExceptionHandlerButton.Name = "demoExceptionInThreadExceptionHandlerButton";
            this.demoExceptionInThreadExceptionHandlerButton.Size = new System.Drawing.Size(443, 38);
            this.demoExceptionInThreadExceptionHandlerButton.TabIndex = 2;
            this.demoExceptionInThreadExceptionHandlerButton.Text = "Demonstrate what happens if exception raised in Application.ThreadException handl" +
                "er";
            this.demoExceptionInThreadExceptionHandlerButton.UseVisualStyleBackColor = true;
            this.demoExceptionInThreadExceptionHandlerButton.Click += new System.EventHandler(this.demoExceptionInThreadExceptionHandlerButton_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.demoUheButton);
            this.panel1.Controls.Add(this.demoExceptionInThreadExceptionHandlerButton);
            this.panel1.Controls.Add(this.demoTrappingUheButton);
            this.panel1.Location = new System.Drawing.Point(2, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(463, 157);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-1, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Unhandled exception behaviour";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.openPerformanceCounterExamplesButton);
            this.panel2.Controls.Add(this.openDrawingExamplesButton);
            this.panel2.Controls.Add(this.startMeInAnotherProcessButton);
            this.panel2.Controls.Add(this.fileSystemSecurityTest);
            this.panel2.Controls.Add(this.simpleResponsiveFormEg);
            this.panel2.Location = new System.Drawing.Point(2, 240);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(463, 242);
            this.panel2.TabIndex = 5;
            // 
            // openDrawingExamplesButton
            // 
            this.openDrawingExamplesButton.Location = new System.Drawing.Point(6, 119);
            this.openDrawingExamplesButton.Name = "openDrawingExamplesButton";
            this.openDrawingExamplesButton.Size = new System.Drawing.Size(443, 23);
            this.openDrawingExamplesButton.TabIndex = 3;
            this.openDrawingExamplesButton.Text = "Drawing examples";
            this.openDrawingExamplesButton.UseVisualStyleBackColor = true;
            this.openDrawingExamplesButton.Click += new System.EventHandler(this.openDrawingExamplesButton_Click);
            // 
            // startMeInAnotherProcessButton
            // 
            this.startMeInAnotherProcessButton.Location = new System.Drawing.Point(6, 89);
            this.startMeInAnotherProcessButton.Name = "startMeInAnotherProcessButton";
            this.startMeInAnotherProcessButton.Size = new System.Drawing.Size(443, 23);
            this.startMeInAnotherProcessButton.TabIndex = 2;
            this.startMeInAnotherProcessButton.Text = "Click Me to start another instance of this app";
            this.startMeInAnotherProcessButton.UseVisualStyleBackColor = true;
            this.startMeInAnotherProcessButton.Click += new System.EventHandler(this.startMeInAnotherProcessButton_Click);
            // 
            // fileSystemSecurityTest
            // 
            this.fileSystemSecurityTest.Location = new System.Drawing.Point(6, 53);
            this.fileSystemSecurityTest.Name = "fileSystemSecurityTest";
            this.fileSystemSecurityTest.Size = new System.Drawing.Size(443, 29);
            this.fileSystemSecurityTest.TabIndex = 1;
            this.fileSystemSecurityTest.Text = "Security privaleges - click here to create a file";
            this.fileSystemSecurityTest.UseVisualStyleBackColor = true;
            this.fileSystemSecurityTest.Click += new System.EventHandler(this.fileSystemSecurityTest_Click);
            // 
            // simpleResponsiveFormEg
            // 
            this.simpleResponsiveFormEg.Location = new System.Drawing.Point(6, 14);
            this.simpleResponsiveFormEg.Name = "simpleResponsiveFormEg";
            this.simpleResponsiveFormEg.Size = new System.Drawing.Size(443, 32);
            this.simpleResponsiveFormEg.TabIndex = 0;
            this.simpleResponsiveFormEg.Text = "Threading - Simple example of adding responsiveness to a form";
            this.simpleResponsiveFormEg.UseVisualStyleBackColor = true;
            this.simpleResponsiveFormEg.Click += new System.EventHandler(this.simpleResponsiveFormEg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, 221);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Other";
            // 
            // openPerformanceCounterExamplesButton
            // 
            this.openPerformanceCounterExamplesButton.Location = new System.Drawing.Point(6, 149);
            this.openPerformanceCounterExamplesButton.Name = "openPerformanceCounterExamplesButton";
            this.openPerformanceCounterExamplesButton.Size = new System.Drawing.Size(443, 23);
            this.openPerformanceCounterExamplesButton.TabIndex = 4;
            this.openPerformanceCounterExamplesButton.Text = "Performance Counter examples";
            this.openPerformanceCounterExamplesButton.UseVisualStyleBackColor = true;
            this.openPerformanceCounterExamplesButton.Click += new System.EventHandler(this.openPerformanceCounterExamplesButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 494);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button demoUheButton;
        private System.Windows.Forms.Button demoTrappingUheButton;
        private System.Windows.Forms.Button demoExceptionInThreadExceptionHandlerButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button simpleResponsiveFormEg;
        private System.Windows.Forms.Button fileSystemSecurityTest;
        private System.Windows.Forms.Button startMeInAnotherProcessButton;
        private System.Windows.Forms.Button openDrawingExamplesButton;
        private System.Windows.Forms.Button openPerformanceCounterExamplesButton;
    }
}