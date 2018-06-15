namespace WinFormsLayout
{
    partial class CategoryControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.LinkTitle = new LinkLabelEx();
            this.SuspendLayout();
            // 
            // LinkTitle
            // 
            this.LinkTitle.AutoSize = true;
            this.LinkTitle.BackColor = System.Drawing.Color.Transparent;
            this.LinkTitle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LinkTitle.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.LinkTitle.Location = new System.Drawing.Point(0, 0);
            this.LinkTitle.Margin = new System.Windows.Forms.Padding(0);
            this.LinkTitle.Name = "LinkTitle";
            this.LinkTitle.Size = new System.Drawing.Size(450, 24);
            this.LinkTitle.TabIndex = 1;
            this.LinkTitle.TabStop = true;
            this.LinkTitle.Text = "Лазуревый перпендикуляр иеромонаха Афдота";
            this.LinkTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CategoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Controls.Add(this.LinkTitle);
            this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "CategoryControl";
            this.Size = new System.Drawing.Size(284, 364);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CategoryControl_Paint);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CategoryControl_Layout);
            this.Resize += new System.EventHandler(this.CategoryControl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private LinkLabelEx LinkTitle;
    }
}
