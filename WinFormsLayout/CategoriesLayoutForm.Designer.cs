namespace WinFormsLayout
{
    partial class CategoriesLayoutForm
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
            this.TestCat = new WinFormsLayout.CategoryControl();
            this.SuspendLayout();
            // 
            // TestCat
            // 
            this.TestCat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(122)))));
            this.TestCat.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TestCat.LinksAddresses = new string[] {
        "",
        "",
        "",
        "",
        "",
        "",
        ""};
            this.TestCat.LinksBackground = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(122)))));
            this.TestCat.LinksNames = new string[] {
        "Большой",
        "Длинный",
        "Огромнейший",
        "Список разных",
        "Ссылок",
        "На всякие такие вот разнообразной длины",
        "Объекты"};
            this.TestCat.ListLinksAColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(218)))), ((int)(((byte)(121)))));
            this.TestCat.ListLinksNColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(174)))), ((int)(((byte)(98)))));
            this.TestCat.Location = new System.Drawing.Point(12, 12);
            this.TestCat.Name = "TestCat";
            this.TestCat.Size = new System.Drawing.Size(279, 433);
            this.TestCat.TabIndex = 0;
            this.TestCat.TitleAddress = "";
            this.TestCat.TitleBackground = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(122)))));
            this.TestCat.TitleLinkAColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(177)))), ((int)(((byte)(66)))));
            this.TestCat.TitleLinkNColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(218)))), ((int)(((byte)(121)))));
            this.TestCat.TitleText = "Заголовок с текстом, в котором каждая буковка и каждое слово словно стремится уех" +
    "ать за экран и помешать всему выравниванию на экране";
            this.TestCat.Visible = false;
            // 
            // CategoriesLayoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(135)))));
            this.ClientSize = new System.Drawing.Size(1134, 671);
            this.Controls.Add(this.TestCat);
            this.Name = "CategoriesLayoutForm";
            this.Text = "Мой UserControl с автовыравниванием текста по высоте";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.MainForm_Layout);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private CategoryControl TestCat;
    }
}

