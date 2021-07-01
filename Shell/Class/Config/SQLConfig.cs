using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Shell.Class.Tools;

namespace Shell.Class.Config
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

        public void Load()
        {
            if (!File.Exists("configSQL.json"))
            {
                Console.Write("\rAucune connexion à charger.");
                return;
            }

            string jsonGet = File.ReadAllText("configSQL.json");
            dynamic json = JsonConvert.DeserializeObject(jsonGet);
            a_server = json.a_server;
            a_db = json.a_db;
            a_user = json.a_user;
            a_pwd = json.a_pwd;

            SQLConn = new SqlConnection();
            SQLConn.ConnectionString = ConnString;

            //Console.Write("\rConnexion chargé.");
            Console.WriteLine("");
        }
        public void Init(string server, string db, string user, string pwd)
        {
            a_server = server;
            a_db = db;
            a_user = user;
            a_pwd = pwd;
            SQLConn = new SqlConnection();
            SQLConn.ConnectionString = ConnString;

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

        public void Infos()
        {
            Console.WriteLine("serv :" + a_server);
            Console.WriteLine("data :" + a_db);
            Console.WriteLine("user :" + a_user);
            Console.WriteLine("pswd :" + new String('*', a_pwd.Length));
        }

        public int TestConnection()
        {
            try
            {
                if (SQLConn == null) Load();
                SQLConn.Open();
                SQLConn.Close();
                Console.WriteLine("Aucun problème n'est survenue lors du test SQL.");
                return 1;
            }
            catch (SqlException)
            {
                Console.WriteLine("Impossible de se connecter au serveur SQL.");
                return 0;
            }
        }

        public void Select(string request)
        {
            if(SQLConn == null)
            {
                Load();
            }

            if(request[0] == '"' && request.Length > 1)
            {
                request = request.Substring(1, request.LastIndexOf('"') - 1);
            }
            else
            {
                return;
            }


            List<int> length = new List<int>();
            Dictionary<int, List<string>> values = new Dictionary<int, List<string>>();
            SQLConn.Open();
            SqlCommand SQLCmd = new SqlCommand(request, SQLConn);

            int row = 0;
            try
            {
                using (SqlDataReader reader = SQLCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        values.Add(row, new List<string>());
                        if(row == 0)
                        {
                            for (int i = 0; i < reader.VisibleFieldCount; i++)
                            {
                                
                                if (length.Count <= i)
                                {
                                    length.Add(reader.GetName(i).Length);
                                }
                                else if (length[i] < reader.GetName(i).Length)
                                {
                                    length[i] = reader.GetName(i).Length;
                                }

                                values[row].Add(reader.GetName(i).ToString());
                            }
                            row++;
                        }
                        for (int i = 0; i < reader.VisibleFieldCount; i++)
                        {
                            if (length.Count <= i)
                            {
                                length.Add(reader[i].ToString().Length);
                            }
                            else if (length[i] < reader[i].ToString().Length)
                            {
                                length[i] = reader[i].ToString().Length;
                            }

                            values[row].Add(reader[i].ToString());
                        }
                        row++;
                    }
                }
            }
            catch
            {
                Console.WriteLine("La requête n'a pu aboutir.");
            }

            foreach (var item in values)
            {
                int x = 0;
                foreach (var value in item.Value)
                {
                    Console.Write(" " + TextTool.AddBlankRight(value.Trim(), length[x]) + "|");
                    x++;
                }
                Console.WriteLine("");
            }
            
            SQLConn.Close();
        }

    }
}
