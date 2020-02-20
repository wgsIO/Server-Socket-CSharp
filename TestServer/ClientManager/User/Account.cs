using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TestServer.ServerManager;
using TestServer.Utils;

namespace TestServer.ClientManager.User
{
    public class Account
    {

        public static Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        public static string bypass = "92a9686e2ebaf600770f21592816fee3288a8fd232bd393dbb39342a";

        public int id { get; set; }
        public long user_id { get; set; }
        public string username { get; set; }
        public string nickname { get; set; }
        public bool hasConnected { get; set; }
        public AccountData accountData = new AccountData();


        public bool hasNickname()
        {
            if (hasConnected)
            {
                var query = Server.sql.NewQuery($"SELECT nickname FROM Account WHERE username = '{username}'");
                string _nickname = query.ExecuteReader().GetString(0);
                query.Dispose();
                return _nickname != null && nickname != null;
            }
            return false;
        }

        public bool setNickname(string _nickname)
        {
            if (hasConnected)
            {
                if (!hasNickname())
                {
                    var query = Server.sql.NewQuery($"UPDATE Account SET nickname = '{_nickname}' WHERE username = '{username}'");
                    query.ExecuteNonQuery();
                    query.Dispose();
                    nickname = _nickname;
                    return true;
                }
            }
            return false;
        }

        //extern
        #region statics
        public static int Login(int _id, string _username, string _password)
        {
            if (ExistsAccount(_username))
            {
                //Query
                using (var query = Server.sql.NewQuery($"SELECT uid, username, password FROM Account WHERE username = '{_username}' and password = '{Cipher.Encrypt(_password, bypass)}'"))
                {
                    var _result = query.ExecuteReader();

                    if (_result.HasRows)
                    {

                        _result.Read();

                        if (accounts.ContainsKey(_username))
                        {
                            Logout(_username);
                        }

                        //Loading data
                        Account account = new Account();
                        account.id = _id;
                        account.user_id = _result.GetInt64(0);
                        account.username = _result.GetString(1);
                        account.nickname = _result.GetString(2);

                        //Authenticating
                        accounts.Add(_username, account);
                        Server.clients[_id].account = account;
                        return 2;
                    }
                    return 1;
                }
            }
            return 0;
        }

        public static bool Logout(string _username)
        {
            if (accounts.ContainsKey(_username))
            {
                Account account = accounts[_username];
                Server.clients[account.id].account = null;
                accounts.Remove(_username);

                //TODO: disconnect account
                return true;
            }
            return false;
        }

        public static int Create(string _username, string _email, string _password)
        {
            if (!ExistsAccount(_username))
            {
                if (!hasEmailRegistred(_email))
                {
                    string token = UIDCorrelation.Generate(_username).getTwo();
                    Console.WriteLine(token);
                    while (existsUID(token))
                        token = UIDCorrelation.Generate(_username).getTwo();
                    var query = Server.sql.NewQuery($"INSERT INTO Account (uid, username, email, password) VALUES ('{token}', '{_username}', '{_email}', '{Cipher.Encrypt(_password, bypass)}')");
                    query.ExecuteNonQuery();
                    query.Dispose();
                    return 2;
                }
                return 1;
            }
            return 0;
        }

        public static bool ExistsAccount(string _username)
        {
            var query = Server.sql.NewQuery($"SELECT username FROM Account WHERE username = '{_username}'");
            bool _result = query.ExecuteReader().HasRows;
            query.Dispose();
            return _result;
        }

        public static bool hasEmailRegistred(string _email)
        {
            var query = Server.sql.NewQuery($"SELECT username FROM Account WHERE email = '{_email}'");
            bool _result = query.ExecuteReader().HasRows;
            query.Dispose();
            return _result;
        }

        public static bool existsUID(string _uid)
        {
            var query = Server.sql.NewQuery($"SELECT username FROM Account WHERE uid = '{_uid}'");
            bool _result = query.ExecuteReader().HasRows;
            query.Dispose();
            return _result;
        }

        public static long getUID(string _username)
        {
            return accounts[_username].user_id;
        }
        #endregion

    }

    class UIDCorrelation
    {
        public static MultiValue<string, string> Generate(string uniqueId)
        {
            const string availableChars = "0123456789";
            using (var generator = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[9];
                generator.GetBytes(bytes);
                var chars = bytes.Select(b => availableChars[b % availableChars.Length]);
                var token = new string(chars.ToArray());
                return new MultiValue<string, string>(uniqueId, token);
            }
        }
    }
}
