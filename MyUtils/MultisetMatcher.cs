using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSet = System.Collections.Generic.HashSet<string>;
using MultiSet = System.Collections.Generic.Dictionary<char, System.Collections.Generic.HashSet<string>>;



namespace My.Utils
{
    //   Пример для входных данных = "кораллы"
    //
    //   0.  matcher -> begin (кора, коралл, коралловый, нектар)
    //       matcher = { глубина = 0, [к] = (кора, коралл, коралловый), [н] = (нектар) }
    //                                       ^     ^       ^                   ^
    //
    //   1.  оставшиеся_варианты = matcher[ match_key = 'к' ] = 3
    //       matcher = { глубина = 0, [к] = (кора, коралл, коралловый) }
    //                                       ^     ^       ^ 
    //       matcher -> continue ('к')
    //       matcher = { глубина = 1, [о] = (кора, коралл, коралловый) }
    //                                        ^     ^       ^ 
    //       matcher.match_result = ( нет строк, которые бы полностью совпали )
    //
    //   ...
    //                                
    //   4.  оставшиеся_варианты = matcher[ match_key = 'а' ] = 3
    //       matcher = { глубина = 3, [а] = (кора, коралл, коралловый) }
    //                                          ^     ^       ^ 
    //       matcher -> continue ('а')
    //       matcher = { глубина = 4, [л] = (коралл, коралловый) }
    //                                           ^       ^ 
    //       matcher.match_result = ( кора )
    //       matcher.first_match = "кора"
    //
    //   Здесь мы получаем первый совпавший ("выживший") результат.
    //   Можно продолжить сравнение до последнего "выжившего" маркера. 
    //                                
    //   6.  оставшиеся_варианты = matcher[ match_key = 'л' ] = 3
    //       matcher = { глубина = 5, [а] = (коралл, коралловый) }
    //                                            ^     ^ 
    //       matcher -> continue ('л')
    //       matcher = { глубина = 6, [о] = (коралловый) }
    //                                             ^ 
    //       matcher.match_result = ( кора, коралл )
    //       matcher.first_match = "кора"
    //  
    //   7.  оставшиеся_варианты = matcher[ match_key = 'ы' ] = 3
    //       matcher = { глубина = 6, [о] = (коралловый) }
    //                                             ^ 
    //       matcher -> continue ('ы')
    //       matcher = { глубина = 7, [о] = () }
    //   
    //       matcher.match_result = ( кора, коралл )
    //       matcher.first_match = "кора"
    //
    //   На шаге 7 и далее метод Continue уже будет выдавать ноль. 
    //   Продолжать сравнение нет смысла, так как выживших маркеров не осталось,
    //   а прошедшие проверку уже не изменятся. 
    //

    //   Позволяет создать набор строк с быстрой выборкой, беря на себя
    //   обслуживание самих строк, счетчиков и пр. Строки при этом 
    //   не перестраиваются в памяти. 

    public class CStringMatcher
    {
        MultiSet Map;
        List<string> MatchResultList;

        public int AliveCount { get; private set; }
        public int LookupDepth { get; private set; }
        public int MatchCount { get { return MatchResultList.Count; } }

        public CStringMatcher (string m1, params string[] markers)
        {
            Map = new MultiSet();
            MatchResultList = new List<string> ();

            Begin(m1, markers);
        }

        //   Сброс и начало нового цикла. 
        public void Begin (string m1, params string[] markers)
        {
            MatchResultList.Clear();
            Map.Clear();

            AliveCount = 0;
            LookupDepth = 0;
            Insert(m1);
            foreach (string m in markers) Insert(m);
        }

        //   Выдача текущих результатов совпадения на этом этапе отбора (эволюции). 
        public int this [char match_key]
        {
            get
            {
                if (Map.ContainsKey(match_key))
                {
                    int result = Map[match_key].Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        //   Переход к следующей эволюции строк. Не совпавшие "отмирают",
        //   совпавшие полностью получают "место под солнцем",
        //   а кандидаты на совпадение переходят в следующую итерацию. 
        public void Continue (char valid_key)
        {
            if (! Map.ContainsKey(valid_key))
                throw new ArgumentException("Некорректный ключ для продолжения операции совпадения");

            StringSet valid_markers = Map[valid_key];
            Map.Clear();
            AliveCount = 0;

            LookupDepth++;
            foreach (string m in valid_markers)
            {
                //   Если проверка на совпадение еще не закончена,
                //   добавляем строку в следующую итерацию. 
                if (m.Length > LookupDepth)
                    Insert(m, LookupDepth);
                //   Иначе, если строка полностью совпала, добавляем
                //   ее в итоговый список совпавших строк.
                else if (m.Length == LookupDepth)
                    MatchResultList.Add(m);
            }
        }

        //   Вставка строки в карту для сравнения. 
        public void Insert (string marker, int lookup_index = 0)
        {
            if (lookup_index < 0)
            {
                throw new ArgumentOutOfRangeException("Недопустимое значение обзорного индекса");
            }

            if (marker.Length <= lookup_index)
            {
                throw new ArgumentOutOfRangeException(
                    "Обзорный индекс слишком велик для данной маркерной строки. Она уже не проходит проверку. " +
                    $"{nameof(marker)} = [{marker}] " +
                    $"{nameof(lookup_index)} = {lookup_index}"
                );
            }

            char key = marker[lookup_index];

            if (! Map.ContainsKey(key))
            {
                Map.Add(key, new StringSet ());
            }

            if (Map[key].Add(marker)) AliveCount++;
        }

        //   Итоговый список совпавших ("выживших" проверку) маркеров. 
        public IEnumerable<string> MatchResult
        {
            get { foreach (string m in MatchResultList) yield return m; }
        }

        //   Первый совпавший маркер (самый короткий). 
        public string FirstMatch
        {
            get { return (MatchResultList.Count > 0) ? MatchResultList[0] : string.Empty; }
        }
    }
}


