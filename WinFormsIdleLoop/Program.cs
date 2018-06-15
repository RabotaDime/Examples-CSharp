using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsIdleLoop
{
    static class CProgram
    {
        [STAThread]
        static void Main ()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //  Подготовка к работе Idle-цикла для WindowsForms приложения. 
            CAppIdleLoop.Init();

            MainForm = new CIdleLoopForm();

            //  Подписка объекта на цикл. 
            CAppIdleLoop.AddListener(MainForm);

            Application.Run(MainForm);
        }

        public static CIdleLoopForm MainForm;
    }
}
