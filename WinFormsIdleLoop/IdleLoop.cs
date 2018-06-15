using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WinFormsIdleLoop
{
    public interface IIdleListener
    {
        void IdleLoop (float delta_time);
    }



    public static class CAppIdleLoop
    {
        private static Stopwatch               IdleTimer;
        private static bool                    SkipFirstCycle;
        private static HashSet<IIdleListener>  IdleSubscribers;

        public static void AddListener (IIdleListener new_subscriber)
        {
            IdleSubscribers.Add(new_subscriber);
        }

        public static void Init ()
        {
            IdleSubscribers = new HashSet<IIdleListener> ();

            IdleTimer = Stopwatch.StartNew();
            SkipFirstCycle = true;

            Application.Idle += new EventHandler (CAppIdleLoop.IdleProc);

            //   Иньекция обновляющего таймера, который будет выводить
            //   цикл сообщений для приложения из неактивности.
            Timer wakeup_timer = new Timer
            {
                Interval = 5,
                Enabled = true,
            };
            //   В данный момент класс таймера входит в цикл сообщений даже
            //   если у него нет ни одного события на обработчике. Но если потребуется
            //   метод заглушка будет таким.
            //wakeup_timer.Tick += new EventHandler(CAppIdleLoop.RefreshProc);
        }

        static void IdleProc (object sender, EventArgs e)
        {
            //   Пропуск первого цикла при запуске программы, чтобы избежать
            //   события с рывком дельта-времени, если приложение очень долго
            //   запускалось. 
            if (SkipFirstCycle)
            {
                SkipFirstCycle = false;
                IdleTimer.Restart();
                return;
            }

            //   Обычный цикл
            float delta_time;
            IdleTimer.Stop();
            {
                long ticks_per_second = Stopwatch.Frequency;
                delta_time = (IdleTimer.ElapsedTicks / (float) ticks_per_second);
            }
            IdleTimer.Restart();

            //   Обработка подписчиков
            foreach (IIdleListener sub in IdleSubscribers)
                sub.IdleLoop(delta_time);
        }

        //public static void RefreshProc (object sender, EventArgs e)
        //{
        //    return;
        //}

        public static long ElapsedMS
        {
            get { return IdleTimer.ElapsedMilliseconds; }
        }

        public static long ElapsedTicks
        {
            get { return IdleTimer.ElapsedTicks; }
        }
    }
}
