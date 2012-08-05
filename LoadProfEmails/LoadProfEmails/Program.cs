using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data;
using System.IO;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.OleDb;

using System.Configuration;

using TextAltPos;


namespace LoadProfEmails
{
    class Program
    {
        static void Main(string[] args)
        {

            string FilePath = @"C:\Users\lobdellb\Desktop\";

            string FileConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                              "Data Source=" + FilePath + ";" +
                              "Extended Properties=\"text;HDR=No;FMT=Delimited\";";

            OleDbConnection FileConn = FileConn = new OleDbConnection(FileConnectionString);
            FileConn.Open();

            OleDbCommand FileCmd = new OleDbCommand("select * from ProfessorsComplete.csv;", FileConn);

            OleDbDataReader Dr = FileCmd.ExecuteReader();


            int Id;
            string ListedName,FirstName, LastName, Email;

            object[] Params = new object[4];

            Params[0] = DA.CreateParameter("@Id", DbType.Int32, 0);
            Params[1] = DA.CreateParameter("@LastName", DbType.String, "");
            Params[2] = DA.CreateParameter("@FirstName", DbType.String, "");
            Params[3] = DA.CreateParameter("@Email", DbType.String, "");

            string UpdateCmd = "update iupui_t_professors set last_name = @LastName, first_name = @FirstName, email = @Email where id = @id;";

            // Take up the first line which has the headings
            Dr.Read();

            while ( Dr.Read() )
            {
                // Console.WriteLine(((string)Dr[2]));

                Id = (int)Dr[0];
                ListedName = HandleDbNull(Dr[1]);
                LastName = HandleDbNull(Dr[2]);
                FirstName = HandleDbNull(Dr[3]);
                Email = HandleDbNull(Dr[4]);

                ((MySqlParameter)Params[0]).Value = Id;
                ((MySqlParameter)Params[1]).Value = LastName;
                ((MySqlParameter)Params[2]).Value = FirstName;
                ((MySqlParameter)Params[3]).Value = Email;

                DA.ExecuteNonQuery(UpdateCmd, Params);

                 
                Console.WriteLine(Id.ToString() + "-" + ListedName + "-" + LastName + "-" + FirstName + "-" + Email);
              

                //Console.WriteLine(Dr[0].GetType().ToString() + "--" + Dr[1].GetType().ToString() + "--" + Dr[2].GetType().ToString() + "--" + Dr[3].GetType().ToString() + "--" + Dr[4].GetType().ToString()); 
            }


            Console.ReadLine();

        }


        // Convert to blank if it's null.
        static string HandleDbNull(object Obj)
        {

            if (Obj == DBNull.Value)
            {
                return string.Empty;
            }
            else
            {
                return (string)Obj;
            }

        }


    }



}
