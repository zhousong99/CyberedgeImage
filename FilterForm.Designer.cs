namespace CyberedgeImageProcess2024
{
    partial class FilterForm
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
            numericSize = new NumericUpDown();
            label2 = new Label();
            label3 = new Label();
            txt1 = new TextBox();
            txt2 = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            chkPreview = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numericSize).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 28);
            label1.Name = "label1";
            label1.Size = new Size(63, 24);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // numericSize
            // 
            numericSize.Location = new Point(134, 26);
            numericSize.Name = "numericSize";
            numericSize.Size = new Size(100, 30);
            numericSize.TabIndex = 1;
            numericSize.Value = new decimal(new int[] { 3, 0, 0, 0 });
            numericSize.ValueChanged += numericSize_ValueChanged;
            numericSize.Leave += numericSize_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(43, 77);
            label2.Name = "label2";
            label2.Size = new Size(63, 24);
            label2.TabIndex = 2;
            label2.Text = "label2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(43, 124);
            label3.Name = "label3";
            label3.Size = new Size(63, 24);
            label3.TabIndex = 3;
            label3.Text = "label3";
            // 
            // txt1
            // 
            txt1.Location = new Point(134, 77);
            txt1.Name = "txt1";
            txt1.Size = new Size(100, 30);
            txt1.TabIndex = 4;
            // 
            // txt2
            // 
            txt2.Location = new Point(134, 124);
            txt2.Name = "txt2";
            txt2.Size = new Size(100, 30);
            txt2.TabIndex = 5;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(43, 228);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(77, 34);
            btnOK.TabIndex = 6;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(157, 228);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(77, 34);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // chkPreview
            // 
            chkPreview.AutoSize = true;
            chkPreview.Location = new Point(55, 192);
            chkPreview.Name = "chkPreview";
            chkPreview.Size = new Size(72, 28);
            chkPreview.TabIndex = 8;
            chkPreview.Text = "预览";
            chkPreview.UseVisualStyleBackColor = true;
            chkPreview.CheckedChanged += chkPreview_CheckedChanged;
            // 
            // FilterForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(308, 287);
            Controls.Add(chkPreview);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(txt2);
            Controls.Add(txt1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(numericSize);
            Controls.Add(label1);
            Name = "FilterForm";
            Text = "FilterForm";
            Load += FilterForm_Load;
            ((System.ComponentModel.ISupportInitialize)numericSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown numericSize;
        private Label label2;
        private Label label3;
        private TextBox txt1;
        private TextBox txt2;
        private Button btnOK;
        private Button btnCancel;
        private CheckBox chkPreview;
    }
}