using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace TestServer.Manager
{
    class MySQLManager
    {

        public string ipAdress { get; set; }
        public string databaseName { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        MySqlConnection connection;
        MySqlCommand commander;

        public bool Connect()
        {
            string pS = "";
            if (ipAdress != null)
                pS = $"server={ipAdress}";
            if (username != null)
                pS = $"{pS};user id={username}";
            if (password != null)
                pS = $"{pS};password={password}";
            if (databaseName != null)
                pS = $"{pS};database={databaseName.ToLower()}";
            string cS = @$"{pS}";
            connection = new MySqlConnection(cS);
            try
            {
                connection.Open();
                commander = new MySqlCommand();
                commander.Connection = connection;
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public MySqlCommand Query(string command)
        {
            commander.CommandText = command;
            return commander;
        }

        public MySqlCommand NewQuery(string command)
        {
            var commander = new MySqlCommand();
            commander.Connection = connection;
            commander.CommandText = command;
            return commander;
        }


    }
}
