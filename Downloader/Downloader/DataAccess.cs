using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Configuration;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace Downloader
{
    class DA
    {


        public static MySqlParameter CreateParameter(string ParameterName, DbType Type, object Value)
        {

            return new MySqlParameter
            {
                ParameterName = ParameterName,
                DbType = Type,
                Value = Value
            };
        }

        public static MySqlConnection GetConnection()
        {

            // In the future this will need to be expanded.
            MySqlConnection Conn = null;
            string DbConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

            bool Success = false;
            while (!Success)
            {
                try
                {
                    Conn = new MySqlConnection(DbConnectionString);
                    Success = true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Database error, press enter to try again: " + Ex.Message);
                    Console.ReadLine();
                }
            }

            return Conn;
        }






        public static object ExecuteScalar(string Command, object[] Params)
        {

            object ReturnValue = null;

            bool Success = false;
            while (!Success)
            {
                try
                {

                    using (MySqlConnection Conn = GetConnection())
                    {
                        Conn.Open();
                        using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                        {

                            int I;
                            for (I = 0; I < Params.Length; I++)
                                Cmd.Parameters.Add((MySqlParameter)Params[I]);


                            ReturnValue = Cmd.ExecuteScalar();
                        }
                        Conn.Close();
                    }

                    Success = true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Database error, press enter to try again: " + Ex.Message);
                    Console.ReadLine();
                }
            }


            return ReturnValue;
        }




        public static int ExecuteNonQuery(string Command, object[] Params)
        {

            int RowsEffected = 0;

            bool Success = false;
            while (!Success)
            {
                try
                {
                    using (MySqlConnection Conn = GetConnection())
                    {
                        Conn.Open();
                        using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                        {
                            int I;
                            for (I = 0; I < Params.Length; I++)
                                Cmd.Parameters.Add((MySqlParameter)Params[I]);

                            RowsEffected = Cmd.ExecuteNonQuery();
                        }
                        Conn.Close();
                    }

                    Success = true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Database error, press enter to try again: " + Ex.Message);
                    Console.ReadLine();
                }
            }

            return RowsEffected;

        }



        public static DataSet ExecuteDataSet(string Command, object[] Params)
        {
            DataSet ds = new DataSet();


            bool Success = false;
            while (!Success)
            {
                try
                {

                    using (MySqlConnection Conn = GetConnection())
                    {
                        Conn.Open();
                        using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                        {

                            int I;

                            for (I = 0; I < Params.Length; I++)
                                Cmd.Parameters.Add((MySqlParameter)Params[I]);

                            MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                            da.Fill(ds);


                        }
                        Conn.Close();
                    }


                    Success = true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Database error, press enter to try again: " + Ex.Message);
                    Console.ReadLine();
                }
            }


            return ds;

        }
        
    }
}
