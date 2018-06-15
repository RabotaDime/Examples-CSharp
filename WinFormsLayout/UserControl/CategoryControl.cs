using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;

namespace WinFormsLayout
{
    public partial class CategoryControl : UserControl
    {
        public class LinkLabelEx : LinkLabel
        {
            protected override void OnMouseEnter (EventArgs e)
            {
                base.OnMouseEnter(e);
                OverrideCursor = SystemCursorsFix.GetHandCursor();
            }

            protected override void OnMouseLeave (EventArgs e)
            {
                base.OnMouseLeave(e);
                OverrideCursor = null;
            }

            protected override void OnMouseMove (MouseEventArgs e)
            {
                base.OnMouseMove(e);
                OverrideCursor = SystemCursorsFix.GetHandCursor();
            }
        }



        public LinkData TitleData = new LinkData () { Caption = "", Address = "", Style = 1 };

        public Color Title_TextColor_Normal = Color.DodgerBlue;
        public Color Title_TextColor_Active = Color.MediumVioletRed;
        public Color Links_TextColor_Normal = Color.DodgerBlue;
        public Color Links_TextColor_Active = Color.MediumVioletRed;

        public const int TitleBorder = 5;
        public const int TitleMaxHeight = 300;
        private Brush TitleBackFill = Brushes.Plum;
        private Brush LinksBackFill = Brushes.GreenYellow;

        public Padding TitlePadding = new Padding (0)
        {
            Left    = TitleBorder,
            Right   = TitleBorder - 5,
            Top     = TitleBorder,
            Bottom  = TitleBorder + 5
        };

        public Padding TitleMargin = new Padding (0) { Bottom = 15 };

        public Padding LinksPadding = new Padding (0)
        {
            Left    = TitleBorder,
            Right   = TitleBorder - 5,
            Top     = TitleBorder,
            Bottom  = TitleBorder + 5
        };

        public Padding LinksMargin = new Padding (0) { Bottom = 4 };

        public struct LinkData
        {
            public string Caption;
            public string Address;
            public int Style;
        }

        public List<LinkData> LinksList = new List<LinkData> ()
        {
            new LinkData () { Style = 1, Caption = "Каждый", Address = "" },
            new LinkData () { Style = 1, Caption = "Охотник", Address = "" },
            new LinkData () { Style = 1, Caption = "Желает", Address = "" },
            new LinkData () { Style = 1, Caption = "Знать", Address = "" },
            new LinkData () { Style = 1, Caption = "Где", Address = "" },
            new LinkData () { Style = 1, Caption = "Сидит", Address = "" },
            new LinkData () { Style = 1, Caption = "Фазан", Address = "" },
            new LinkData () { Style = 1, Caption = "И\u00A0сиреневый перпендикуляр", Address = "" },
            new LinkData () { Style = 1, Caption = "Нейромонах", Address = "" },
            new LinkData () { Style = 1, Caption = "Феофан", Address = "" },
        };

        public List<LinkLabelEx> LinksObjs = new List<LinkLabelEx> ()
        {
        };



        public CategoryControl ()
        {
            InitializeComponent();

            LinkTitle.MaximumSize = new Size
            (
                this.ClientRectangle.Width - TitlePadding.Horizontal,
                TitleMaxHeight
            );

            LinkTitle.SetBounds
            (
                TitlePadding.Left,
                TitlePadding.Top,
                0,
                0,
                BoundsSpecified.Location
            );

            LinkTitle.Cursor = SystemCursorsFix.GetHandCursor();

            LinkTitle.LinkColor         = this.Title_TextColor_Normal;
            LinkTitle.ActiveLinkColor   = this.Title_TextColor_Active;

            RecreateObjects(true);
        }

        private void RecreateByList (string[] Data, bool CaptionsData = true)
        {
            string[] ListCaptions   = (CaptionsData     ? Data : null);
            string[] ListAddresses  = (! CaptionsData   ? Data : null);

            //  Обновляем список. 
            int MaxCount = Math.Min(LinksList.Count, Data.Length);

            for (int I = 0; I < MaxCount; I++)
            {
                LinkData OldItem = LinksList[I];
                if (ListCaptions    != null) OldItem.Caption = ListCaptions[I];
                if (ListAddresses   != null) OldItem.Address = ListAddresses[I];
                LinksList[I] = OldItem;
            }

            while (LinksList.Count > MaxCount) LinksList.RemoveAt(MaxCount);

            while (LinksList.Count < Data.Length) LinksList.Add(
                new LinkData()
                {
                    Caption = (ListCaptions  == null ? "" : ListCaptions[LinksList.Count]),
                    Address = (ListAddresses == null ? "" : ListAddresses[LinksList.Count]),
                }
            );
        }

        private void RecreateObjects (bool ModLayout = false)
        {
            foreach (LinkLabelEx LObj in LinksObjs)
            {
                this.Controls.Remove(LObj);
            }

            LinksObjs.Clear();

            if (ModLayout) this.SuspendLayout();

            int LIndex = 0;
            int LPos = LinksPadding.Top;

            if (LinkTitle.Visible)
            {
                LPos += LinkTitle.ClientRectangle.Height + TitlePadding.Bottom + TitleMargin.Bottom;
            }

            //LinkLabelEx LPrevObj = null;
            foreach (var L in LinksList)
            {
                LinkLabelEx LObj = new LinkLabelEx
                {
                    AutoSize        = true,
                    BackColor       = Color.Transparent,
                    Font            = this.Font,
                    LinkBehavior    = LinkBehavior.AlwaysUnderline,
                    Margin          = new Padding(0),
                    Name            = "LObj_" + (++LIndex),
                    Location        = new Point(this.LinksPadding.Left, LPos),
                    Size            = new Size(this.ClientRectangle.Width - this.LinksPadding.Horizontal - this.LinksMargin.Horizontal, 5),
                    MaximumSize     = new Size(this.ClientRectangle.Width - this.LinksPadding.Horizontal - this.LinksMargin.Horizontal, 100),
                    TabIndex        = LIndex,
                    TabStop         = true,
                    Text            = L.Caption,
                    TextAlign       = ContentAlignment.TopLeft,
                    Tag             = L,
                    Cursor          = SystemCursorsFix.GetHandCursor(),

                    LinkColor         = this.Links_TextColor_Normal,
                    ActiveLinkColor   = this.Links_TextColor_Active,
                };

                this.Controls.Add(LObj);
                LinksObjs.Add(LObj);

                if (ModLayout) LObj.PerformLayout();

                LPos += LObj.ClientRectangle.Height + LinksMargin.Bottom;
            }

            if (ModLayout) this.ResumeLayout(false);
            if (ModLayout) this.PerformLayout();
        }



        private void CategoryControl_Paint (object sender, PaintEventArgs e)
        {
            Graphics Canvas = e.Graphics;

            int DrawPos = 0;

            if (LinkTitle.Visible)
            {
                Canvas.FillRectangle
                (
                    TitleBackFill,
                    0,
                    0,
                    this.ClientSize.Width,
                    LinkTitle.ClientRectangle.Bottom + TitlePadding.Bottom
                );

                DrawPos = LinkTitle.ClientRectangle.Height + TitlePadding.Bottom + TitleMargin.Bottom;
            }

            if (LinksList.Count > 0)
            {
                Canvas.FillRectangle
                (
                    LinksBackFill,
                    0,
                    DrawPos,
                    this.ClientSize.Width,
                    this.ClientSize.Height
                );
            }


            #region Unused Code 1
            /*/
            Canvas.DrawLine
            (
                Pens.DimGray,
                this.TitleBorder,
                LinkTitle.ClientSize.Height + TitleBorder,
                this.ClientSize.Width - (this.TitleBorder * 2),
                LinkTitle.ClientSize.Height + TitleBorder
            );

            //Canvas.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            float Border = 5.0f;

            string TitleText = "Лазуревый перпендикуляр Йеромонаха Афдота";
            float TitleWidth = this.ClientRectangle.Width - (Border * 2);
            //SizeF TitleSize = Canvas.MeasureString(TitleText, LabelTitle.Font, (int) TitleWidth);

            Size TitleSize = TextRenderer.MeasureText(TitleText, LabelTitle.Font, new Size((int) TitleWidth, 0));

            LabelTitle.Text = TitleText;
            //LabelTitle.Height = (int) TitleSize.Height;
            LabelTitle.BackColor = Color.Transparent;
            LabelTitle.SetBounds((int) Border, 0, (int) TitleSize.Width, (int) TitleSize.Height);

            Canvas.DrawString
            (
                TitleText,
                LabelTitle.Font,
                Brushes.DimGray,
                new RectangleF
                (
                    Border,
                    0f,
                    TitleSize.Width,
                    TitleSize.Height
                ),
                new StringFormat()
                {
                    LineAlignment = StringAlignment.Far,
                }
            );
            /**/
            #endregion
        }



        private void CategoryControl_Layout (object sender, LayoutEventArgs e)
        {
            LinkTitle.MaximumSize = new Size(this.ClientSize.Width - TitlePadding.Horizontal, TitleMaxHeight);

            int LPos = LinksPadding.Top;

            if (LinkTitle.Visible)
            {
                LPos += LinkTitle.ClientRectangle.Height + TitlePadding.Bottom + TitleMargin.Bottom;
            }

            foreach (LinkLabelEx L in LinksObjs)
            {
                L.SetBounds
                (
                    this.LinksPadding.Left,
                    LPos,
                    0,
                    0,
                    BoundsSpecified.Location
                );

                L.MaximumSize = new Size
                (
                    this.ClientRectangle.Width - this.LinksPadding.Horizontal - this.LinksMargin.Horizontal,
                    0
                );

                LPos += L.ClientRectangle.Height + LinksMargin.Bottom;
            }
        }



        private void CategoryControl_Resize (object sender, EventArgs e)
        {
            this.Invalidate();
        }



        [Description("Текст заглавной категории"), Category("MyData")]
        public string TitleText
        {
            get { return LinkTitle.Text; }
            set
            {
                LinkTitle.Text = value;
                LinkTitle.Visible = (value.Length > 0);

                this.Invalidate();
                this.PerformLayout();
            }
        }

        [Description("Адрес заглавной категории"), Category("MyData")]
        public string TitleAddress
        {
            get { return TitleData.Address; }
            set
            {
                TitleData.Address = value;
                //this.PerformLayout();
            }
        }

        [Description("Цвет фона ссылки-заголовка"), Category("MyData")]
        public Color TitleBackground
        {
            get { return (TitleBackFill as SolidBrush).Color; }
            set
            {
                TitleBackFill = new SolidBrush(value);
                this.Invalidate();
            }
        }

        [Description("Цвет фона списка ссылок"), Category("MyData")]
        public Color LinksBackground
        {
            get { return (LinksBackFill as SolidBrush).Color; }
            set
            {
                LinksBackFill = new SolidBrush(value);
                this.Invalidate();
            }
        }

        [Description("Цвет ссылки-заголовка в обычном состоянии"), Category("MyData")]
        public Color TitleLinkNColor
        {
            get { return Title_TextColor_Normal; }
            set
            {
                Title_TextColor_Normal = value;
                LinkTitle.LinkColor = value;
            }
        }

        [Description("Цвет активной (нажатой) ссылки-заголовка"), Category("MyData")]
        public Color TitleLinkAColor
        {
            get { return Title_TextColor_Active; }
            set
            {
                Title_TextColor_Active = value;
                LinkTitle.ActiveLinkColor = value;
            }
        }

        [Description("Цвет ссылки из списка в обычном состоянии"), Category("MyData")]
        public Color ListLinksNColor
        {
            get { return Links_TextColor_Normal; }
            set
            {
                Links_TextColor_Normal = value;
                foreach (LinkLabelEx L in LinksObjs)
                    L.LinkColor = value;
            }
        }

        [Description("Цвет активной (нажатой) ссылки из списка"), Category("MyData")]
        public Color ListLinksAColor
        {
            get { return Links_TextColor_Active; }
            set
            {
                Links_TextColor_Active = value;
                foreach (LinkLabelEx L in LinksObjs)
                    L.ActiveLinkColor = value;
            }
        }

        [Description("Наименования списка ссылок"), Category("MyData")]
        public string[] LinksNames
        {
            get
            {
                List<string> R = new List<string> ();

                foreach (LinkLabelEx L in LinksObjs)
                {
                    R.Add(L.Text);
                }

                return R.ToArray();
            }

            set
            {
                RecreateByList(value, true);
                RecreateObjects(false);

                this.Invalidate();
                this.PerformLayout();
            }
        }

        [Description("Адреса списка ссылок"), Category("MyData")]
        public string[] LinksAddresses
        {
            get
            {
                List<string> R = new List<string> ();

                foreach (LinkLabelEx L in LinksObjs)
                {
                    R.Add(((LinkData) L.Tag).Address);
                }

                return R.ToArray();
            }

            set
            {
                RecreateByList(value, false);
                RecreateObjects(false);

                this.Invalidate();
                this.PerformLayout();
            }
        }
    }
}
