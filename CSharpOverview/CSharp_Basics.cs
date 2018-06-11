using System;
using static System.Math;
using Cardinal = System.UInt32;

namespace MyCode
{
    enum AllowedChars : short
    {
    }

    struct Life
    {
    }

    interface IFurry
    {
    }

    class Creature
    {
        protected Creature (int HiddenLifesCounter)
        {
        }

        public static Kitty Elizabeth = new Kitty ("Lizzy");
    }

    class Kitty : Creature, ISoft, IWarm, ISizable, ISpherical, IFurry
    {
        public Kitty (string Name) : base (7)
        {

        }
    }
}

