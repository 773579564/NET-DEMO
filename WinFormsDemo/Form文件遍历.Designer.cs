
namespace WinFormsDemo
{
    partial class Form文件遍历
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
            this.text执行过程 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.text目录 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // text执行过程
            // 
            this.text执行过程.Location = new System.Drawing.Point(34, 116);
            this.text执行过程.Multiline = true;
            this.text执行过程.Name = "text执行过程";
            this.text执行过程.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.text执行过程.Size = new System.Drawing.Size(923, 434);
            this.text执行过程.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(34, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(171, 42);
            this.button2.TabIndex = 8;
            this.button2.Text = "解析数据";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // text目录
            // 
            this.text目录.Location = new System.Drawing.Point(115, 28);
            this.text目录.Name = "text目录";
            this.text目录.Size = new System.Drawing.Size(497, 23);
            this.text目录.TabIndex = 9;
            this.text目录.Text = "F:\\ZL-公司\\ZL-代码\\SVN\\Net\\清单模板编制工具\\Common\\trunk\\Code杭钢\\API";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "文件目录";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(441, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(171, 42);
            this.button1.TabIndex = 11;
            this.button1.Text = "解析updata数据";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form文件遍历
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 562);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.text目录);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.text执行过程);
            this.Name = "Form文件遍历";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form文件遍历";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text执行过程;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox text目录;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}