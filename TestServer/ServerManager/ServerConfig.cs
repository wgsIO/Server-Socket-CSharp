using Nancy.Json;
using System;
using System.IO;

namespace TestServer.ServerManager
{

    public class Configs
    {
        public static string configFile = "server/serverconfig.json";

        public static void createFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    return;
                Directory.CreateDirectory(path);
            } catch (Exception e)
            {

            }
        }

    }
    class ServerConfig : ServerSettings<ServerConfig>
    {

        public string serverName = "Game Server";
        public bool defaultLogo = true;
        public string logoText = "Requires you to disable defaultLogo and insert a Logo Text.";

        public string MySQL_Host = "localhost";
        public string MySQL_Username = "root";
        public string MySQL_Password = "";
        public string MySQL_Database = "GameServer";

        public string gameName = "GameServer";
        public string gameType = "FPS";
        public string gameMain = "Login";
        public string gameVersion = "1.0";

    }

    public class ServerSettings<T> where T : new()
    {

        private const string DEFAULT_FILENAME = "settings.json";

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            T t = new T();
            if (File.Exists(fileName))
                t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(fileName));
            return t;

        }

    }
}
