using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace My.Utils
{
    //   Пример расширения 
    public static class ShortCollections
    {
        //   Два малопроизводительных (в худшем случае O(n)) вспомогательных 
        //   метода для перебора небольших множеств. 
        public static T PreviousTo <T> (this IEnumerable<T> set, T known_item, T default_marker = default(T))
        {
            T prev = default_marker;

            //   Проход по всему перечисляемому множеству. 
            foreach (T item in set)
            {
                //   Если мы встретили элемент множества, который является 
                //   заявленным (известным), то возвращаем предыдущий
                //   сохраненный элемент при переборе, либо нулевое значение. 
                if (EqualityComparer<T>.Default.Equals(item, known_item))
                {
                    return prev;
                }

                prev = item;
            }

            return default(T);
        }

        public static T NextTo <T> (this IEnumerable<T> set, T known_item, T default_marker = default(T))
        {
            T prev = default_marker;

            //   Проход по всему перечисляемому множеству. 
            foreach (T item in set)
            {
                //   Если мы попали в ситуацию, когда заявленный (известный)
                //   элемент множества стал равен предыдущему (то есть, мы зашли 
                //   вперед на один шаг), значит текущий элемент это искомый элемент,
                //   следующий за известным. 
                if (EqualityComparer<T>.Default.Equals(prev, known_item))
                {
                    return item;
                }

                prev = item;
            }

            return default(T);
        }
    }
}

