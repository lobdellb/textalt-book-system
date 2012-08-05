using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;

using NewBookSystem;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace LoadInventory
{
    class Program
    {
        static void Main(string[] args)
        {

            throw new Exception();

            FileStream Fp = new FileStream(@"C:\Users\lobdellb\Downloads\inventory_barcodes.txt",FileMode.Open);

            byte[] Buffer = new byte[Fp.Length];

            Fp.Read(Buffer, 0, (int)Fp.Length);

            string Str;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            Str = enc.GetString(Buffer);


            char[] Separator = {'\n'};

            string[] Lines = Str.Split(Separator);

//            Console.WriteLine(Buffer.Length.ToString());

//            Console.WriteLine(Str.Length.ToString());

//            Console.WriteLine( Lines.Length.ToString() );


            for (int I = 0; I < Lines.Length; I++)
            {

                string Isbn = null;

                if (Lines[I].Length >= 13)
                {
                    Isbn = Lines[I].Substring(0, 13);
                }
                else
                {
                    if (Lines[I].Length == 10)
                    {
                        Isbn = Lines[I];
                    }
                }


                if (Isbn != null)
                {

                    string Isbn9 = Isbn.Substring(3, 9);

                    Console.WriteLine(Isbn);

                    object[] Params = new object[1];

                    Params[0] = DA.CreateParameter("@Isbn9", DbType.String, Isbn9);
                                                    
                    DA.ExecuteNonQuery("insert into inventory_t_corrected1 (Isbn9) value (@Isbn9);", Params);
                }

            }

            Console.ReadLine();



        }
    }
}
