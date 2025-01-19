
namespace CyberedgeImageProcess2024
{
    partial class ResizeDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtScaleX = new System.Windows.Forms.TextBox();
            this.txtScaleY = new System.Windows.Forms.TextBox();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbInterpolation = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.chkCreateNewWindow = new System.Windows.Forms.CheckBox();
            this.txtNewWindowTitle = new System.Windows.Forms.TextBox();
            this.lblNewWindowTitle = new System.Windows.Forms.Label();
            this.chkAspect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "X轴比例";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y轴比例";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "宽(像素)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "高(像素)";
            // 
            // txtScaleX
            // 
            this.txtScaleX.Location = new System.Drawing.Point(134, 16);
            this.txtScaleX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtScaleX.Name = "txtScaleX";
            this.txtScaleX.Size = new System.Drawing.Size(112, 28);
            this.txtScaleX.TabIndex = 4;
            this.txtScaleX.Text = "0.5";
            this.txtScaleX.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            // 
            // txtScaleY
            // 
            this.txtScaleY.Location = new System.Drawing.Point(134, 58);
            this.txtScaleY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtScaleY.Name = "txtScaleY";
            this.txtScaleY.Size = new System.Drawing.Size(112, 28);
            this.txtScaleY.TabIndex = 5;
            this.txtScaleY.Text = "0.5";
            this.txtScaleY.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(134, 100);
            this.txtWidth.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(112, 28);
            this.txtWidth.TabIndex = 6;
            this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(134, 142);
            this.txtHeight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(112, 28);
            this.txtHeight.TabIndex = 7;
            this.txtHeight.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 19);
            this.label5.TabIndex = 8;
            this.label5.Text = "插值";
            // 
            // cbInterpolation
            // 
            this.cbInterpolation.FormattingEnabled = true;
            this.cbInterpolation.Items.AddRange(new object[] {
            "最近邻",
            "双线性",
            "双三次",
            "区域",
            "Lanczos",
            "精确双线性",
            "精确最近邻"});
            this.cbInterpolation.Location = new System.Drawing.Point(134, 196);
            this.cbInterpolation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbInterpolation.Name = "cbInterpolation";
            this.cbInterpolation.Size = new System.Drawing.Size(112, 26);
            this.cbInterpolation.TabIndex = 9;
            this.cbInterpolation.SelectedIndexChanged += new System.EventHandler(this.cbInterpolation_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(65, 398);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 34);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancle.Location = new System.Drawing.Point(153, 398);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(81, 34);
            this.btnCancle.TabIndex = 11;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // chkCreateNewWindow
            // 
            this.chkCreateNewWindow.AutoSize = true;
            this.chkCreateNewWindow.Location = new System.Drawing.Point(120, 309);
            this.chkCreateNewWindow.Name = "chkCreateNewWindow";
            this.chkCreateNewWindow.Size = new System.Drawing.Size(126, 23);
            this.chkCreateNewWindow.TabIndex = 12;
            this.chkCreateNewWindow.Text = "建立新窗口";
            this.chkCreateNewWindow.UseVisualStyleBackColor = true;
            this.chkCreateNewWindow.CheckedChanged += new System.EventHandler(this.chkCreateNewWindow_CheckedChanged);
            // 
            // txtNewWindowTitle
            // 
            this.txtNewWindowTitle.Location = new System.Drawing.Point(134, 341);
            this.txtNewWindowTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNewWindowTitle.Name = "txtNewWindowTitle";
            this.txtNewWindowTitle.Size = new System.Drawing.Size(112, 28);
            this.txtNewWindowTitle.TabIndex = 14;
            this.txtNewWindowTitle.Visible = false;
            // 
            // lblNewWindowTitle
            // 
            this.lblNewWindowTitle.AutoSize = true;
            this.lblNewWindowTitle.Location = new System.Drawing.Point(24, 344);
            this.lblNewWindowTitle.Name = "lblNewWindowTitle";
            this.lblNewWindowTitle.Size = new System.Drawing.Size(104, 19);
            this.lblNewWindowTitle.TabIndex = 13;
            this.lblNewWindowTitle.Text = "新窗口标题";
            this.lblNewWindowTitle.Visible = false;
            // 
            // chkAspect
            // 
            this.chkAspect.AutoSize = true;
            this.chkAspect.Checked = true;
            this.chkAspect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAspect.Location = new System.Drawing.Point(120, 250);
            this.chkAspect.Name = "chkAspect";
            this.chkAspect.Size = new System.Drawing.Size(126, 23);
            this.chkAspect.TabIndex = 15;
            this.chkAspect.Text = "保持纵横比";
            this.chkAspect.UseVisualStyleBackColor = true;
            this.chkAspect.CheckedChanged += new System.EventHandler(this.chkAspect_CheckedChanged);
            // 
            // ResizeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 444);
            this.Controls.Add(this.chkAspect);
            this.Controls.Add(this.txtNewWindowTitle);
            this.Controls.Add(this.lblNewWindowTitle);
            this.Controls.Add(this.chkCreateNewWindow);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbInterpolation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.txtScaleY);
            this.Controls.Add(this.txtScaleX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResizeDialog";
            this.Text = "图像调整大小";
            this.Load += new System.EventHandler(this.ResizeDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtScaleX;
        private System.Windows.Forms.TextBox txtScaleY;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbInterpolation;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.CheckBox chkCreateNewWindow;
        private System.Windows.Forms.TextBox txtNewWindowTitle;
        private System.Windows.Forms.Label lblNewWindowTitle;
        private System.Windows.Forms.CheckBox chkAspect;
    }
}