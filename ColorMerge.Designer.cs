namespace CyberedgeImageProcess2024
{
    partial class ColorMerge
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
            label1 = new Label();
            cmbRed = new ComboBox();
            btnOK = new Button();
            btnCancel = new Button();
            cmbGreen = new ComboBox();
            label2 = new Label();
            cmbBlue = new ComboBox();
            label3 = new Label();
            cmbGray = new ComboBox();
            label4 = new Label();
            cmbCyan = new ComboBox();
            label5 = new Label();
            cmbMagenta = new ComboBox();
            label6 = new Label();
            cmbYellow = new ComboBox();
            label7 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(71, 36);
            label1.Name = "label1";
            label1.Size = new Size(46, 24);
            label1.TabIndex = 0;
            label1.Text = "红色";
            // 
            // cmbRed
            // 
            cmbRed.FormattingEnabled = true;
            cmbRed.Location = new Point(152, 33);
            cmbRed.Name = "cmbRed";
            cmbRed.Size = new Size(182, 32);
            cmbRed.TabIndex = 1;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(134, 406);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(112, 34);
            btnOK.TabIndex = 2;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(252, 406);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(112, 34);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "放弃";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // cmbGreen
            // 
            cmbGreen.FormattingEnabled = true;
            cmbGreen.Location = new Point(152, 80);
            cmbGreen.Name = "cmbGreen";
            cmbGreen.Size = new Size(182, 32);
            cmbGreen.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(71, 83);
            label2.Name = "label2";
            label2.Size = new Size(46, 24);
            label2.TabIndex = 4;
            label2.Text = "绿色";
            // 
            // cmbBlue
            // 
            cmbBlue.FormattingEnabled = true;
            cmbBlue.Location = new Point(152, 129);
            cmbBlue.Name = "cmbBlue";
            cmbBlue.Size = new Size(182, 32);
            cmbBlue.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(71, 132);
            label3.Name = "label3";
            label3.Size = new Size(46, 24);
            label3.TabIndex = 6;
            label3.Text = "蓝色";
            // 
            // cmbGray
            // 
            cmbGray.FormattingEnabled = true;
            cmbGray.Location = new Point(152, 178);
            cmbGray.Name = "cmbGray";
            cmbGray.Size = new Size(182, 32);
            cmbGray.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(71, 181);
            label4.Name = "label4";
            label4.Size = new Size(46, 24);
            label4.TabIndex = 8;
            label4.Text = "灰色";
            // 
            // cmbCyan
            // 
            cmbCyan.FormattingEnabled = true;
            cmbCyan.Location = new Point(152, 226);
            cmbCyan.Name = "cmbCyan";
            cmbCyan.Size = new Size(182, 32);
            cmbCyan.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(71, 229);
            label5.Name = "label5";
            label5.Size = new Size(46, 24);
            label5.TabIndex = 10;
            label5.Text = "青色";
            // 
            // cmbMagenta
            // 
            cmbMagenta.FormattingEnabled = true;
            cmbMagenta.Location = new Point(152, 279);
            cmbMagenta.Name = "cmbMagenta";
            cmbMagenta.Size = new Size(182, 32);
            cmbMagenta.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(71, 282);
            label6.Name = "label6";
            label6.Size = new Size(46, 24);
            label6.TabIndex = 12;
            label6.Text = "洋红";
            // 
            // cmbYellow
            // 
            cmbYellow.FormattingEnabled = true;
            cmbYellow.Location = new Point(152, 332);
            cmbYellow.Name = "cmbYellow";
            cmbYellow.Size = new Size(182, 32);
            cmbYellow.TabIndex = 15;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(71, 335);
            label7.Name = "label7";
            label7.Size = new Size(46, 24);
            label7.TabIndex = 14;
            label7.Text = "黄色";
            // 
            // ColorMerge
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(408, 486);
            Controls.Add(cmbYellow);
            Controls.Add(label7);
            Controls.Add(cmbMagenta);
            Controls.Add(label6);
            Controls.Add(cmbCyan);
            Controls.Add(label5);
            Controls.Add(cmbGray);
            Controls.Add(label4);
            Controls.Add(cmbBlue);
            Controls.Add(label3);
            Controls.Add(cmbGreen);
            Controls.Add(label2);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(cmbRed);
            Controls.Add(label1);
            Name = "ColorMerge";
            Text = "色彩合成";
            Load += ColorMerge_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbRed;
        private Button btnOK;
        private Button btnCancel;
        private ComboBox cmbGreen;
        private Label label2;
        private ComboBox cmbBlue;
        private Label label3;
        private ComboBox cmbGray;
        private Label label4;
        private ComboBox cmbCyan;
        private Label label5;
        private ComboBox cmbMagenta;
        private Label label6;
        private ComboBox cmbYellow;
        private Label label7;
    }
}