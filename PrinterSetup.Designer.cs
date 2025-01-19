namespace CyberedgeImageProcess2024
{
    partial class PrinterSetup
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
            numScaling = new NumericUpDown();
            chkDrawBorder = new CheckBox();
            button1 = new Button();
            label2 = new Label();
            chkCenter = new CheckBox();
            chkPrintTitle = new CheckBox();
            chkRotate = new CheckBox();
            chkActualSize = new CheckBox();
            button3 = new Button();
            chkPrintSelection = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numScaling).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 28);
            label1.Name = "label1";
            label1.Size = new Size(82, 24);
            label1.TabIndex = 0;
            label1.Text = "缩放比例";
            // 
            // numScaling
            // 
            numScaling.Location = new Point(144, 24);
            numScaling.Name = "numScaling";
            numScaling.Size = new Size(75, 30);
            numScaling.TabIndex = 1;
            numScaling.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // chkDrawBorder
            // 
            chkDrawBorder.AutoSize = true;
            chkDrawBorder.Location = new Point(83, 76);
            chkDrawBorder.Name = "chkDrawBorder";
            chkDrawBorder.Size = new Size(108, 28);
            chkDrawBorder.TabIndex = 2;
            chkDrawBorder.Text = "画出边框";
            chkDrawBorder.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.DialogResult = DialogResult.OK;
            button1.Location = new Point(120, 310);
            button1.Name = "button1";
            button1.Size = new Size(81, 34);
            button1.TabIndex = 3;
            button1.Text = "确定";
            button1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(225, 26);
            label2.Name = "label2";
            label2.Size = new Size(26, 24);
            label2.TabIndex = 4;
            label2.Text = "%";
            // 
            // chkCenter
            // 
            chkCenter.AutoSize = true;
            chkCenter.Checked = true;
            chkCenter.CheckState = CheckState.Checked;
            chkCenter.Location = new Point(83, 110);
            chkCenter.Name = "chkCenter";
            chkCenter.Size = new Size(108, 28);
            chkCenter.TabIndex = 5;
            chkCenter.Text = "页面居中";
            chkCenter.UseVisualStyleBackColor = true;
            // 
            // chkPrintTitle
            // 
            chkPrintTitle.AutoSize = true;
            chkPrintTitle.Location = new Point(83, 177);
            chkPrintTitle.Name = "chkPrintTitle";
            chkPrintTitle.Size = new Size(108, 28);
            chkPrintTitle.TabIndex = 6;
            chkPrintTitle.Text = "打印标题";
            chkPrintTitle.UseVisualStyleBackColor = true;
            // 
            // chkRotate
            // 
            chkRotate.AutoSize = true;
            chkRotate.Location = new Point(83, 211);
            chkRotate.Name = "chkRotate";
            chkRotate.Size = new Size(101, 28);
            chkRotate.TabIndex = 7;
            chkRotate.Text = "旋转90°";
            chkRotate.UseVisualStyleBackColor = true;
            // 
            // chkActualSize
            // 
            chkActualSize.AutoSize = true;
            chkActualSize.Location = new Point(83, 245);
            chkActualSize.Name = "chkActualSize";
            chkActualSize.Size = new Size(144, 28);
            chkActualSize.TabIndex = 8;
            chkActualSize.Text = "实际大小打印";
            chkActualSize.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.DialogResult = DialogResult.Cancel;
            button3.Location = new Point(207, 310);
            button3.Name = "button3";
            button3.Size = new Size(81, 34);
            button3.TabIndex = 10;
            button3.Text = "取消";
            button3.UseVisualStyleBackColor = true;
            // 
            // chkPrintSelection
            // 
            chkPrintSelection.AutoSize = true;
            chkPrintSelection.Enabled = false;
            chkPrintSelection.Location = new Point(83, 144);
            chkPrintSelection.Name = "chkPrintSelection";
            chkPrintSelection.Size = new Size(162, 28);
            chkPrintSelection.TabIndex = 11;
            chkPrintSelection.Text = "只打印选中区域";
            chkPrintSelection.UseVisualStyleBackColor = true;
            // 
            // PrinterSetup
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(311, 365);
            Controls.Add(chkPrintSelection);
            Controls.Add(button3);
            Controls.Add(chkActualSize);
            Controls.Add(chkRotate);
            Controls.Add(chkPrintTitle);
            Controls.Add(chkCenter);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(chkDrawBorder);
            Controls.Add(numScaling);
            Controls.Add(label1);
            Name = "PrinterSetup";
            Text = "PrinterSetup";
            Load += PrinterSetup_Load;
            ((System.ComponentModel.ISupportInitialize)numScaling).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown numScaling;
        private CheckBox chkDrawBorder;
        private Button button1;
        private Label label2;
        private CheckBox chkCenter;
        private CheckBox chkPrintTitle;
        private CheckBox chkRotate;
        private CheckBox chkActualSize;
        private Button button3;
        private CheckBox chkPrintSelection;
    }
}