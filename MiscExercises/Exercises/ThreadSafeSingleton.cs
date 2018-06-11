using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Exercises
{
    ///                                                            
    ///  Потокобезопасный Синглетон шаблон, использующий           
    ///  двойную проверку для избежания выделения ресурсов         
    ///  на лишние блокировки.                                     
    ///                                                            
    ///     https://en.m.wikipedia.org/wiki/Double-checked_locking 
    ///     https://msdn.microsoft.com/en-us/library/ff650316.aspx 
    ///                                                            
    class ThreadSafeSingleton
    {
        private static ThreadSafeSingleton _Instance = null;
        private static object _CriticalSection = new object ();

        //  Приватный конструктор, чтобы объект 
        //  нельзя было создавать извне. 
        private ThreadSafeSingleton ()
        {
        }

        public static ThreadSafeSingleton Instance
        {
            get
            {
                //  При запуске программы считаем, что переменная 
                //  "Instance" имеет одно конкретное значение 
                //  равное 0x00000000 или 0x0000000000000000.
                //  Тогда лишний вход в критическую секцию с большей 
                //  вероятностью можно избежать, сначала проверив 
                //  значение переменной на конкретный маркер-подсказку. 

                if (_Instance == null)
                {
                    lock (_CriticalSection)
                    {
                        //  Вторая проверка необходима для предотвращения
                        //  десинхронизации при входе в критическую секцию. 
                        //  Пока мы в этом процессе входили в нужное состояние
                        //  на предыдущей команде, другой процесс уже мог
                        //  изменить переменную. 

                        if (_Instance == null)
                        {
                            _Instance = new ThreadSafeSingleton();
                        }
                    }
                }

                return _Instance;
            }
        }
    }
}
