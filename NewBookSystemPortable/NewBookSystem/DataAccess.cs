﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace NewBookSystem
{
    public partial class DA
    {


        public static DataTable GetIUPUIInfo(string Isbn)
        {

            DataTable dt;
            DataSet ds = new DataSet();

            int Isbn9 = Common.ToIsbn9(Isbn);

            string SelectCommandStr = "SELECT * " +
                          "FROM iupui_t_books " +
                          "WHERE @isbn9 = isbn9;";

            using (MySqlConnection Conn = DA.GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(SelectCommandStr, Conn))
                {

                    Cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@isbn9",
                        DbType = DbType.Int32,
                        Value = Isbn9
                    });

                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);
                    da.Fill(ds);

                }
                Conn.Close();
            }

            dt = ds.Tables[0];

            return dt;

        }
        

        public static void LogError(Exception Ex,string Message)
        {

            object[] Params = new object[2];
            Params[0] = new MySqlParameter
            {
                ParameterName = "@Message",
                DbType = DbType.Single,
                Value = Message
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@XmlError",
                DbType = DbType.String,
                Value = Ex.ToString()
            };


            DA.ExecuteNonQuery("INSERT INTO log_t_errorlog (message,xmlerror) VALUE (@Message,@XmlError);", Params);
            
        }


        public static DataTable GetWholeSaleQuotes(string Isbn, DateTime When)
        {

            DataSet dsResults = new DataSet();
            DataTable dtResults;

            int Isbn9 = Common.ToIsbn9(Isbn);
 

            string SelectCommandStr = "SELECT title,author,publisher,edition,year,newprice,newoffer,usedoffer,isbn,name " +
                                      "FROM wholesale_t_wholesalebook " + 
                                      "JOIN wholesale_t_wholesalers wslrs " +
                                           "ON wslrs.pk = wholesaler_key " +
                                      "WHERE @isbn9 = isbn9;";

            using (MySqlConnection Conn = GetConnection())
            {
                using (MySqlCommand Cmd = new MySqlCommand(SelectCommandStr, Conn))
                {

                    MySqlParameter WhenParam = new MySqlParameter("@when", MySqlDbType.Date);

                    WhenParam.Value = When;
                    WhenParam.Direction = ParameterDirection.Input;

                    MySqlParameter Isbn9Param = new MySqlParameter("@isbn9", MySqlDbType.Int32);

                    Isbn9Param.Value = Isbn9;
                    Isbn9Param.Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add(WhenParam);
                    Cmd.Parameters.Add(Isbn9Param);

                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                    da.Fill(dsResults);
                    dtResults = dsResults.Tables[0];

                }
            }

            return dtResults;

        }


        public static double GetSalesTaxRate()
        {

            string SelectCommandStr = "SELECT value FROM sysconfig WHERE `key` = \"salestaxrate\"";

            using (MySqlConnection Conn = GetConnection())
            {
                using (MySqlCommand Cmd = new MySqlCommand(SelectCommandStr, Conn))
                {

                    MySqlDataReader dr = Cmd.ExecuteReader();

                    dr.Read();
                    return Double.Parse(dr[0].ToString());

                }
            }

        }


        public static DataTable GetSeasonInfo()
        {

            return new DataTable();
        }


//        static MySqlConnection GetConnection()
//        {

//            // In the future this will need to be expanded.

////            string DbConnectionString = @"server = localhost;
////                                    database = textalt_dev_v1;
////                                    user id = lobdellb;
////                                    password = elijah72;
////                                    port = 3306;";

//            string DbConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

//            MySqlConnection Conn = new MySqlConnection(DbConnectionString);

//            return Conn;
//        }


        public static void ChangeInventory(string Isbn, int Delta)
        {

            string InsertAddedStr = "INSERT INTO inventory_t_added (isbn9) VALUES (@Isbn9);";
            string InsertRemovedStr = "INSERT INTO inventory_t_removed (isbn9) VALUES (@Isbn9);";
            string CommandStr;
            int Isbn9;

            if ( Math.Abs( Delta ) > 0 )
            {

                if (Delta > 0)
                    CommandStr = InsertAddedStr;
                else
                    CommandStr = InsertRemovedStr;

                Delta = Math.Abs(Delta);

                using (MySqlConnection Conn = GetConnection())
                {
                    Conn.Open();
                    using (MySqlCommand Cmd = new MySqlCommand(CommandStr, Conn))
                    {
                        Isbn9 = Common.ToIsbn9(Isbn);

                        Cmd.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@Isbn9",
                            DbType = DbType.Int32,
                            Value = Isbn9
                        });


                        int I;

                        for (I = 0; I < Delta ; I++)
                            Cmd.ExecuteNonQuery();

                    }

                }
            }

        }



        public static int GetNumInInventory(string Isbn)
        {
            Int64 Added,Removed;
            int Isbn9;
            string AddedCommandStr = "SELECT COUNT(pk) FROM inventory_t_added WHERE Isbn9 = @Isbn9;";
            string RemovedCommandStr = "SELECT COUNT(pk) FROM inventory_t_removed WHERE Isbn9 = @Isbn9;";

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(AddedCommandStr,Conn))
                {
                    Isbn9 = Common.ToIsbn9(Isbn);

                    Cmd.Parameters.Add( new MySqlParameter {
                        ParameterName = "@Isbn9",
                        DbType = DbType.Int32,
                        Value = Isbn9 });

                    Added = (Int64)Cmd.ExecuteScalar();

                    Cmd.CommandText = RemovedCommandStr;

                    Removed = (Int64)Cmd.ExecuteScalar();
 
                    Conn.Close();
                }

            }


            return (int)(Added-Removed);

        }



    }
}
