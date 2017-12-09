namespace DestinyTest
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
            this.executeButton = new System.Windows.Forms.Button();
            this.msgTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(12, 12);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 5;
            this.executeButton.Text = "Execute";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // msgTextBox
            // 
            this.msgTextBox.Location = new System.Drawing.Point(12, 41);
            this.msgTextBox.Multiline = true;
            this.msgTextBox.Name = "msgTextBox";
            this.msgTextBox.ReadOnly = true;
            this.msgTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.msgTextBox.Size = new System.Drawing.Size(928, 455);
            this.msgTextBox.TabIndex = 4;
            this.msgTextBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 510);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.msgTextBox);
            this.Name = "MainForm";
            this.Text = "DestinyTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.TextBox msgTextBox;
    }
}

