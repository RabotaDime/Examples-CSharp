using System;
using System.Drawing;
using System.Linq;

using Цвет = System.Drawing.Color;



class ЧерепашкаХудожник
{
    //  Свойства черепашки
    public Цвет Цвет { get; set; }
    public Цвет Фон { get; set; }
    public int Ширина { get; set; }
    public int Скорость { get; set; }
    public float Угол { get; set; }

    //  Функция: Перейти вперед
    public void Вперед (int длина)
    {
    }

    //  Функции: Разворот
    public void РазворотНалево (int угол)
    {
        this.Угол = this.Угол + угол;
        while (this.Угол > 180)
            this.Угол = this.Угол - 180;
    }
    public void РазворотНаправо (int угол)
    {
        this.Угол = this.Угол - угол;
        while (this.Угол < -180)
            this.Угол = this.Угол - 180;
    }
}



class Слизняк
{
    public static int Размер;
    public string Цвет;

    public Слизняк (string какого_цвета)
    {
        Размер = 150;
        Цвет = какого_цвета;
    }

    public void ОписатьСлизня (string место_встречи)
    {
        Console.WriteLine("Вы находитесь в " + место_встречи);
        Console.WriteLine("Вы встретили слизняка размером " + Слизняк.Размер);
        Console.WriteLine("Он выглядит цветом " + this.Цвет);
    }
}

class ПрограммаСлизняков
{
    public static void ГлавнаяФункция ()
    {
        Цвет[] цвета = new Цвет[] { Цвет.Blue, Цвет.Red, Цвет.Yellow, Цвет.Green };
        ЧерепашкаХудожник черепашка = new ЧерепашкаХудожник();
        черепашка.Фон = Цвет.Black;
        foreach (int x in Enumerable.Range(0, 180))
        {
            черепашка.Цвет = цвета[x % 4];
            черепашка.Ширина = x / 100;
            черепашка.Вперед(x);
            черепашка.РазворотНалево(90);
            черепашка.Скорость = 100;
        }



        //Слизняк зеленый_слизень = new Слизняк("Зеленый");
        //Слизняк голубой_слизень = new Слизняк("Голубой");
        //Слизняк красный_слизень = new Слизняк("Красный");

        //зеленый_слизень.ОписатьСлизня("Пещера");
    }
}
