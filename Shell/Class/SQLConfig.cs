using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class
{
    public class SQLConfig
    {
        private SqlConnection SQLConn;
        private string a_server = "";
        private string a_db = "";
        private string a_user = "";
        private string a_pwd = "";

        public string ConnString
        {
            get => "server=" + a_server + ";database=" + a_db + ";uid=" + a_user + ";pwd=" + a_pwd;
        }

        public void Init(string server, string db, string user, string pwd)
        {
            Console.WriteLine("");
            a_server = server;
            a_db = db;
            a_user = user;
            a_pwd = pwd;

            string jsonFile = "";
            jsonFile += "{";
            jsonFile += "\"a_server\":\"" + a_server + "\",";
            jsonFile += "\"a_db\":\"" + a_db + "\",";
            jsonFile += "\"a_user\":\"" + a_user + "\",";
            jsonFile += "\"a_pwd\":\"" + a_pwd + "\"";
            jsonFile += "}";

            File.WriteAllText("configSQL.json", jsonFile);

            Console.Write("\rConnection sauvegardé.");
            Console.WriteLine("");
        }

        public int TestConnection()
        {
            try
            {
                SQLConn = new SqlConnection();
                SQLConn.ConnectionString = ConnString;
                SQLConn.Open();
                SQLConn.Close();
                Console.WriteLine("Aucun problème n'est survenue lors du test SQL.");
                return 0;
            }
            catch (SqlException)
            {
                Console.WriteLine("Impossible de se connecter au serveur SQL.");
                return 1;
            }
        }
    }
}
