namespace WinFormsIdleLoop
{
    partial class CIdleLoopForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
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
        private void InitializeComponent ()
        {
            this.RenderBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.RenderBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // RenderBox1
            // 
            this.RenderBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RenderBox1.Location = new System.Drawing.Point(12, 12);
            this.RenderBox1.Margin = new System.Windows.Forms.Padding(0);
            this.RenderBox1.Name = "RenderBox1";
            this.RenderBox1.Size = new System.Drawing.Size(608, 579);
            this.RenderBox1.TabIndex = 0;
            this.RenderBox1.TabStop = false;
            this.RenderBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderBox1_Paint);
            // 
            // CIdleLoopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 603);
            this.Controls.Add(this.RenderBox1);
            this.KeyPreview = true;
            this.Name = "CIdleLoopForm";
            this.Text = "Idle Loop Form";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CMainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CMainForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.RenderBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox RenderBox1;
    }
}

