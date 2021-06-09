using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Class
{
    public class SQLConfig
    {
        public string a_server = "";
        public string a_db = "";
        public string a_user = "";
        public string a_pwd = "";

        public string ConnString
        {
            get => "server=" + a_server + ";database=" + a_db + ";uid=" + a_user + ";pwd=" + a_pwd;
        }

        public void Init(string server, string db, string user, string pwd)
        {
            a_server = server;
            a_db = db;
            a_user = user;
            a_pwd = pwd;
        }
    }
}
