using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.OleDb;
using System.IO;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace LoadProfData
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.WriteLine(Directory.GetCurrentDirectory());

            string FilePath = @"C:\Users\Bryce Lobdell\Software\bookstore_software\LoadProfData\";
            string FileName = "professors_complete.csv";
            //string FileName = "ALT_ISBN_060109.txt";

            string FileConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                              "Data Source=" + FilePath + ";" +
                              "Extended Properties=\"text;HDR=No;FMT=Delimited\";";


            string ConnStr = @"server = 127.0.0.1;
                                    database = textalt_prod_v2;
                                    user id = textaltpos;
                                    password = btdbr14;
                                    port = 3307;";


            MySqlConnection Conn = new MySqlConnection(ConnStr);

            Conn.Open();

            // MySqlCommand Cmd = new MySqlCommand("INSERT INTO iupui_t_oldprofessors3 (id,listed_name,last_name,first_name,email,comments ) VALUES (@Id,@ListedName,@LastName,@FirstName,@Email,@Courses );", Conn);
            MySqlCommand Cmd = new MySqlCommand("update iupui_t_professors set last_name = @LastName,first_name = @FirstName,email = @Email where Id = @Id;", Conn);

            Cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@ListedName",
                DbType = DbType.String
            });

            Cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@LastName",
                DbType = DbType.String
            });

            Cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@FirstName",
                DbType = DbType.String
            });

            Cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Email",
                DbType = DbType.String
            });

            Cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Courses",
                DbType = DbType.String
            });


            Cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Id",
                DbType = DbType.Int32
            });

            OleDbConnection FileConn = new OleDbConnection(FileConnectionString);
            FileConn.Open();

            OleDbCommand FileCmd = new OleDbCommand("SELECT * from " + FileName + ";", FileConn);
            OleDbDataReader FileRdr = FileCmd.ExecuteReader();

            string ListedName, LastName, FirstName, Email, Courses;


            // get rid of the title row
            FileRdr.Read();

            int Id;

            while (FileRdr.Read())
            {

                //Console.Write(FileRdr[0].ToString());
                //Console.Write((string)FileRdr[1]);
                //Console.Write((string)FileRdr[2]);
                //Console.Write((string)FileRdr[3]);
                //Console.Write((string)FileRdr[4]);
                //Console.WriteLine((string)FileRdr[5]);

                Id = (int)FileRdr[0];

                if (FileRdr[1] == DBNull.Value)
                    ListedName = string.Empty;
                else
                    ListedName = ((string)FileRdr[1]).Trim();

                if (FileRdr[4] == DBNull.Value)
                    Email = string.Empty;
                else
                    Email = ((string)FileRdr[4]).Trim();

                if (FileRdr[2] == DBNull.Value)
                    LastName = string.Empty;
                else
                    LastName = ((string)FileRdr[2]).Trim();

                if (FileRdr[3] == DBNull.Value)
                    FirstName = string.Empty;
                else
                    FirstName = ((string)FileRdr[3]).Trim();

                //if (FileRdr[5] == DBNull.Value)
                //    Courses = string.Empty;
                //else
                //    Courses = ((string)FileRdr[5]).Trim();

                Courses = string.Empty;

                Console.WriteLine(ListedName + "--" + FirstName + "--" + LastName + "--" + Email + "--" + Courses);

                Cmd.Parameters[0].Value = ListedName;
                Cmd.Parameters[1].Value = LastName;
                Cmd.Parameters[2].Value = FirstName;
                Cmd.Parameters[3].Value = Email;
                Cmd.Parameters[4].Value = Courses;
                Cmd.Parameters[5].Value = Id;

                Cmd.ExecuteNonQuery();

            }

            FileRdr.Close();
            FileConn.Close();

            Console.ReadLine();

        }
    }
}
