using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace My.Tracer
{
    public partial class ListForm : Form
    {
        public ListForm ()
        {
            InitializeComponent();
        }

        private void ListForm_Layout (object sender, LayoutEventArgs e)
        {
            listBox1.SetBounds(0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }
    }
}
