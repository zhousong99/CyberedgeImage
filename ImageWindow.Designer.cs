

namespace CyberedgeImageProcess2024
{
    partial class ImageWindow
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
            lblSubTitle = new Label();
            PictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PictureBox).BeginInit();
            SuspendLayout();
            // 
            // lblSubTitle
            // 
            lblSubTitle.AutoSize = true;
            lblSubTitle.Dock = DockStyle.Top;
            lblSubTitle.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblSubTitle.Location = new Point(0, 0);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Padding = new Padding(0, 0, 0, 7);
            lblSubTitle.Size = new Size(89, 27);
            lblSubTitle.TabIndex = 1;
            lblSubTitle.Text = "SubTitle";
            lblSubTitle.Click += lblSubTitle_Click;
            // 
            // PictureBox
            // 
            PictureBox.Dock = DockStyle.Fill;
            PictureBox.Location = new Point(0, 27);
            PictureBox.Margin = new Padding(4, 5, 4, 5);
            PictureBox.Name = "PictureBox";
            PictureBox.Size = new Size(1107, 720);
            PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox.TabIndex = 2;
            PictureBox.TabStop = false;
            PictureBox.Click += PictureBox_Click;
            PictureBox.MouseDown += PictureBox_MouseDown;
            PictureBox.MouseMove += PictureBox_MouseMove;
            PictureBox.MouseUp += PictureBox_MouseUp;
            // 
            // ImageWindow
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1107, 747);
            Controls.Add(PictureBox);
            Controls.Add(lblSubTitle);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ImageWindow";
            Text = "ImageWindow";
            Load += ImageWindow_Load;
            SizeChanged += ImageWindow_SizeChanged;
            KeyPress += ImageWindow_KeyPress;
            ((System.ComponentModel.ISupportInitialize)PictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.PictureBox PictureBox;
    }
}