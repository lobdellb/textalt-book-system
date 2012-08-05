using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NewBookSystem;

namespace GenerateEmployeeCodes
{
    class Program
    {
        static void Main(string[] args)
        {

            char[] Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            int Len = 15;

            int HowMany = 3;


            Random Rand = new Random();
            int Rnd;

            for (int n = 0; n < HowMany; n++)
            {

                StringBuilder sb = new StringBuilder();

                for (int m = 0; m < Len; m ++ )
                {

                    Rnd = (int)Math.Floor( Rand.NextDouble() * Characters.Length );

                    sb.Append( Characters[Rnd].ToString() );

                }

                Console.WriteLine( sb.ToString() );
                DA.ExecuteNonQuery("insert into pos_t_users (barcode) value ('" + sb.ToString() + "');", new object[0]);
            }

            Console.ReadLine();
        }
    }
}
