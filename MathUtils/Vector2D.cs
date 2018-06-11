using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace My.MathUtils
{
    public struct Vector3D
    {
        public float A;
        public float B;
        public float C;



        public static Vector3D operator * (Vector3D V1, Vector3D V2)
        {
            Vector3D First     = new Vector3D { A = V1.A, B = V1.B, C = 0 };
            Vector3D Second    = new Vector3D { A = V2.A, B = V2.B, C = 0 };

            Vector3D CrossProduct = new Vector3D
            {
                A = (First.B * Second.C) - (First.C * Second.B),
                B = (First.C * Second.A) - (First.A * Second.C),
                C = (First.A * Second.B) - (First.B * Second.A),
            };

            return CrossProduct;
        }
    }



    public struct Vector2D
    {
        public float A;
        public float B;



        public Vector2D (float aA, float aB)
        {
            A = aA;
            B = aB;
        }



        public override string ToString ()
        {
            return "[" + ((double) A).ToString() + ", " + ((double) B).ToString() + "]";
        }



        public float Length
        {
            get { return (float) Math.Sqrt((A * A) + (B * B)); }
        }

        public float LengthEx
        {
            get
            {
                float Result = (float) Math.Sqrt((A * A) + (B * B));
                return (B < 0) ? -Result : +Result;
            }
        }

        public Angle Angle
        {
            get
            {
                float AngleVal = (float) Math.Atan2(B, A);

                return new Angle(AngleVal, AngleType.Radians);
            }
        }

        public Angle AbsAngle
        {
            get
            {
                float AngleVal = (float) Math.Atan2(B, A);
                Angle ResultAngle = new Angle(AngleVal, AngleType.Radians);
                
                //  Изменяем угол для нижних квадрантов (третьего (III) и четвертого (IV))
                //  Переводим значение угла от -0.0 до -180.0  -->  +180.0 до +360.0
                if (B < 0)
                    ResultAngle.Add(360.0f, AngleType.Degrees);

                return ResultAngle;
            }
        }



        public static Vector2D XAxis { get { return new Vector2D (1, 0); } }
        public static Vector2D YAxis { get { return new Vector2D (0, 1); } }

        public static Vector2D UnitVectorFromAngle (Angle aAngle)
        {
            //     
            // 1.0 --...           Unit Circle
            //     .     /| .     / 
            //     .    / |    . /  
            //     .   /  |      .
            //     .V /   | Vy = sin
            //     . /    |        .
            //     ./    _|         .
            //     /____|_|.........|
            //        Vx = cos     1.0 
            //
            return new Vector2D
            (
                (float) Math.Cos( aAngle.Radians ),
                (float) Math.Sin( aAngle.Radians )
            );
        }



        public Vector2D UnitVector
        {
            get
            {
                float Len = this.Length;

                return new Vector2D
                (
                    A / Len,
                    B / Len
                );
            }
        }



        public void Rotate (Angle aAngle)
        {
            float Ang = aAngle.Radians;

            float A_Rotated = (float)( (A * Math.Cos(Ang)) - (B * Math.Sin(Ang)) );
            float B_Rotated = (float)( (A * Math.Sin(Ang)) + (B * Math.Cos(Ang)) );

            A = A_Rotated;
            B = B_Rotated;
        }



        public float DotProduct (Vector2D aOtherVector)
        {
            //  Прямое перемножение компонентов вектора. 
            return (this.A * aOtherVector.A) + (this.B * aOtherVector.B);
        }

        public float CrossProduct (Vector2D aOtherVector)
        {
            //  Крестообразное перемножение компонентов вектора. 
            return (this.A * aOtherVector.B) - (this.B * aOtherVector.A);
        }



        public Angle AngleBetween1 (Vector2D aOtherVector)
        {
            float AngleVal = (float) Math.Acos
            (
                this.DotProduct(aOtherVector) /
                (this.Length * aOtherVector.Length)
            );

            return new Angle(AngleVal, AngleType.Radians);
        }

        public Angle AngleBetween2 (Vector2D aOtherVector)
        {
            float Cross  = this.CrossProduct  (aOtherVector);
            float Dot    = this.DotProduct    (aOtherVector);

            float AngleVal = (float) Math.Atan2(Cross, Dot);

            return new Angle(AngleVal, AngleType.Radians);
        }

        public Angle AngleBetween3 (Vector2D aOtherVector)
        {
            float Angle1 = this         .AbsAngle.Degrees;
            float Angle2 = aOtherVector .AbsAngle.Degrees;

            float AngleVal = Angle1 - Angle2;

            return new Angle(AngleVal, AngleType.Radians);
        }



        public Angle AngleBetween (Vector2D aOtherVector)
        {
            if ((this.A == aOtherVector.A) && (this.B == aOtherVector.B))
                return new Angle (0, AngleType.Degrees);
            else if ((this.A == -aOtherVector.A) && (this.B == -aOtherVector.B))
                return new Angle (180, AngleType.Degrees);
            else
            {
                float BaseAngle = this.Angle.Radians;

                //Vector2D AVec = this;
                Vector2D BVec = aOtherVector;

                Angle RotAngle = new Angle ( BaseAngle * -1.0f , AngleType.Radians);
                //AVec.Rotate(RotAngle);
                BVec.Rotate(RotAngle);

                return BVec.Angle;
            }
        }



        public static Vector2D operator + (Vector2D V1, Vector2D V2)
        {
            return new Vector2D (V1.A + V2.A, V1.B + V2.B);
        }

        public static Vector2D operator - (Vector2D V1, Vector2D V2)
        {
            return new Vector2D (V1.A - V2.A, V1.B - V2.B);
        }
    }
}
