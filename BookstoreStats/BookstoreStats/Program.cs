using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using NewBookSystem;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BookstoreStats
{
    class Program
    {
        static void Main(string[] args)
        {

            int SoldBookCost = 0;
            int SoldBookProceeds = 0;
            int SoldBooks = 0;

            int WsBookCost = 0;
            int WsBookValue = 0;
            int WsBooks = 0;

            int LostBookCost = 0;
            int LostBooks = 0;

            int Found = 0;

            // get all purchased books

            DataSet DsPb = DA.ExecuteDataSet("select * from pos_t_purchasedbook order by isbn,price desc;", new object[0]);
            DataTable DtPb = DsPb.Tables[0];

            DataSet DsSb = DA.ExecuteDataSet("select *,mid( isbn,length(isbn)-9,9 )+0 as isbn9 from pos_t_soldbook where (length(isbn) = 13 or length(isbn) = 10);", new object[0]);
            DataTable DtSb = DsSb.Tables[0];

            for (int I = 0; I < DtPb.Rows.Count; I++)
            {
                Console.WriteLine("working on " + I.ToString() + "/" + DtPb.Rows.Count.ToString());
                DataRow Dr = DtPb.Rows[I],DrFound = null;

                //Console.WriteLine(((int)(UInt32)Dr["isbn9"]).ToString());

                //Console.WriteLine("isbn9 = " + ((UInt32)Dr["Isbn9"]).ToString());

               // for (int J = 0; J < DtSb.Rows.Count; J++)
               // {

                    //if ((Int32)(UInt32)Dr["isbn9"] == (Int32)(Double)DtSb.Rows[J]["isbn9"])
                    //if ( Common.ToIsbn9(  (string)Dr["isbn"] ) == Common.ToIsbn9( (string)DtSb.Rows[J]["isbn"] ) )
                    if (Common.ToIsbn9((string)Dr["isbn"]) == Common.ToIsbn9("9780077221416"))
                    {

                        //DrFound = DtSb.Rows[J];
                  //      break;
                    }

                //}




                //if ( DtSb.
                //DataRow[] Matches = DtSb.Select("convert(isbn9,'System.Int64') = convert(" + ((UInt32)Dr["Isbn9"]).ToString() + ",'System.Int64')");
                ////31244844
                //DataRow[] Matches = DtSb.Select("Isbn9 = 7337961");

                if ( DrFound != null) // then we can match it with a sold book
                {

                    SoldBookCost += (int)(UInt32)Dr["price"];
                    SoldBookProceeds += (int)(UInt32)DrFound["price"];

                    SoldBooks++;

                    DtSb.Rows.Remove(DrFound);
                }
                else
                {

                    // Look for a wholesale offer
                    DataTable DtWsOffer = BD.GetSortedWholeSaleOffers((string)Dr["isbn"]);

                    // this means it's found
                    if (DtWsOffer.Rows.Count > 0)
                    {
                        WsBookCost += (int)(UInt32)Dr["price"];
                        WsBookValue += (int)(UInt32)DtWsOffer.Rows[0]["usedoffer"];
                        WsBooks++;
                    }
                    else // this means it's not
                    {

                        LostBooks++;
                        LostBookCost += (int)(UInt32)Dr["price"];

                    }


                }


            }

            Console.WriteLine("sold book (" + SoldBooks.ToString() + ") cost/proceeds:  " + string.Format("{0:c}", SoldBookCost / 100) + "/" + string.Format("{0:c}", SoldBookProceeds / 100) + " = " + (SoldBookCost / SoldBookProceeds).ToString());

            Console.WriteLine("W.s. book (" + WsBooks.ToString() + ") cost/value:  " + string.Format("{0:c}", WsBookCost / 100) + "/" + string.Format("{0:c}", WsBookValue / 100) + " = " + (WsBookCost/WsBookValue).ToString());

            Console.WriteLine("Lost book (" + LostBooks.ToString() + ") cost:  " + string.Format("{0:c}", LostBookCost/100));


            Console.ReadLine();
        }
    }
}




