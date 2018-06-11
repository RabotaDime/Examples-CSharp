using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace My.Tracer
{
    public enum HostFormAlignment
    {
        None    = 0,
        Top     = 1,
        Right   = 2,
        Bottom  = 3,
        Left    = 4,
    }



    public class CListTracer : CTextTracer
    {
        //static VisualTracers



        ListForm OutputForm;
        Form HostForm;
        HostFormAlignment HostAlignment;

        public CListTracer (Form host_form, HostFormAlignment alignment = HostFormAlignment.None)
        {
            HostForm = host_form;

            OutputForm = new ListForm ();

            HostForm.Move += HostForm_Move;
        }

        private void HostForm_Move (object sender, EventArgs e)
        {
            if (HostAlignment == HostFormAlignment.Bottom)
            {

            }
        }
    }
}
