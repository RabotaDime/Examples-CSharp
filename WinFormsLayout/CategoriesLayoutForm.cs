using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WinFormsLayout
{
    public partial class CategoriesLayoutForm : Form
    {
        public int CCount = 3;
        public int RCount = 2;

        public int HBorder = 30;
        public int VBorder = 10;

        public List<CategoryControl> MyPanels = new List<CategoryControl> ();

        public CategoriesLayoutForm ()
        {
            InitializeComponent();

            this.SuspendLayout();

            RecreateObjects();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void RecreateObjects ()
        {
            for (int y = 0; y < RCount; y++)
            {
                for (int x = 0; x < CCount; x++)
                {
                    int i = x + (y * CCount);

                    CategoryControl CatObj = new CategoryControl
                    {
                        BackColor           = TestCat.BackColor,
                        LinksBackground     = TestCat.LinksBackground,
                        ListLinksAColor     = TestCat.ListLinksAColor,
                        ListLinksNColor     = TestCat.ListLinksNColor,
                        TitleBackground     = TestCat.TitleBackground,
                        TitleLinkAColor     = TestCat.TitleLinkAColor,
                        TitleLinkNColor     = TestCat.TitleLinkNColor,

                        Font = TestCat.Font,

                        TitleText = "Заголовок с текстом, в\u00A0котором каждая буковка и\u00A0каждое слово словно стремится уехать за\u00A0экран и\u00A0помешать всему выравниванию на\u00A0экране",
                        TitleAddress = "",
                        LinksAddresses = new string[]
                        {
                            "",
                            "",
                            "",
                        },
                        LinksNames = new string[]
                        {
                            "Большой",
                            "Длинный",
                            "Огромнейший я\u00A0бы\u00A0даже сказал",
                            "Список разных",
                            "Ссылок",
                            "На\u00A0всякие такие вот разнообразной длины",
                            "Объекты",
                        },

                        Location = new System.Drawing.Point(28, 12),
                        Name = "Cat" + (i + 1),
                        Size = new System.Drawing.Size(279, 433),
                        TabIndex = 0,
                    };

                    this.Controls.Add(CatObj);
                    MyPanels.Add(CatObj);
                }
            }
        }

        private void MainForm_Load (object sender, EventArgs e)
        {

        }

        private void MainForm_Resize (object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void MainForm_Layout (object sender, LayoutEventArgs e)
        {
            int CSize = (this.ClientRectangle.Width  - (HBorder * (CCount + 1))) / CCount;
            int RSize = (this.ClientRectangle.Height - (HBorder * (RCount - 1)) - (VBorder * 2)) / RCount;

            for (int Y = 0; Y < RCount; Y++)
            {
                for (int X = 0; X < CCount; X++)
                {
                    int I = X + (Y * CCount);

                    MyPanels[I].SetBounds
                    (
                        HBorder + (X * (CSize + HBorder)),
                        VBorder + (Y * (RSize + HBorder)),
                        CSize,
                        RSize
                    );
                }
            } 
        }
    }
}
