using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using TestServer.ClientManager;
using TestServer.ClientManager.Game;
using TestServer.ClientManager.User;
using TestServer.Manager;
using TestServer.ServerManager;
using TestServer.Utils;

namespace TestServer.Handlers
{
    class ServerHandle
    {
        //public static void WelcomeReceived(int _fromClient, Packet _packet)
        //{
        //int _clientIdCheck = _packet.ReadInt();
        //string _username = _packet.ReadString();

        //ConsoleSender.Send(MessageType.Normal, $"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now user {_fromClient}.");

        //if (_fromClient != _clientIdCheck)
        //{
        //ConsoleSender.Send(MessageType.Warning, $"User \"{_username}\" (ID: {_fromClient} has assumed the wrong client ID ({_clientIdCheck})!)");
        //}

        //TODO: Send user to next stage
        //}

        #region Login
        public static void ReceivedCredentials(int _fromClient, Packet _packet)
        {
            var pcm = PacketCounterManager.createIfNotExists(Server.clients[_fromClient], "Credentials", 5);
            pcm.AddMoreCount();
            pcm.Verify();
            if (!pcm.cancelled)
            {
                string gameName = _packet.ReadString();
                string gameType = _packet.ReadString();
                string gameMain = _packet.ReadString();
                string gameVersion = _packet.ReadString();

                if (gameName != Program.sc.gameName || gameType != Program.sc.gameType || gameMain != Program.sc.gameMain || gameVersion != Program.sc.gameVersion)
                {
                    ConsoleSender.Send(MessageType.Warning, $"User({_fromClient}) send credentials not equals, disconneting...");
                    if (gameVersion != Program.sc.gameVersion)
                    {
                        ConsoleSender.Send(MessageType.Warning, $"User({_fromClient}) game version not equals, disconneting...");
                        //TODO: Send version not equals
                    }
                    Thread.Sleep(1000);
                    Server.clients[_fromClient].Disconnect();
                    return;
                }
            }
        }

        public static void RegisterAccount(int _fromClient, Packet _packet)
        {
            var pcm = PacketCounterManager.createIfNotExists(Server.clients[_fromClient], "AccountLogin", 5);
            pcm.AddMoreCount();
            pcm.Verify();
            if (!pcm.cancelled)
            {
                string _username = _packet.ReadString();
                string _email = _packet.ReadString();
                string _password = _packet.ReadString();

                int _result = Account.Create(_username, _email, _password);
                switch (_result)
                {
                    case 2:
                        Server.clients[_fromClient].SendMessageBox("LoginMessage", "sucess", "Registrado com sucesso.");
                        break;
                    case 1:
                        Server.clients[_fromClient].SendMessageBox("LoginMessage", "error", "O email inserido já está registrado em uma conta.");
                        break;
                    case 0:
                        Server.clients[_fromClient].SendMessageBox("LoginMessage", "error", "Já existe uma conta com este usuário.");
                        break;
                }
            }
        }

        public static void LoginAccount(int _fromClient, Packet _packet)
        {
            var pcm = PacketCounterManager.createIfNotExists(Server.clients[_fromClient], "AccountLogin", 5);
            pcm.AddMoreCount();
            pcm.Verify();
            if (!pcm.cancelled)
            {
                string _username = _packet.ReadString();
                string _password = _packet.ReadString();

                int _result = Account.Login(_fromClient, _username, _password);
                if (_result == 2)
                    ServerSend.LoginSuccefully(_fromClient, Server.clients[_fromClient].account);
                else
                    Server.clients[_fromClient].SendMessageBox("LoginMessage", "error", "Senha ou Usuário incorretos.");
            }
        }
        #endregion

        #region InGame
        public static void UpdatePlayerPosition(int _fromClient, Packet _packet)
        {
            var pcm = PacketCounterManager.createIfNotExists(Server.clients[_fromClient], "PositionUpdate", long.MaxValue);
            pcm.AddMoreCount();
            pcm.Verify();
            if (!pcm.cancelled)
                Player.UpdatePositions(_fromClient, _packet.ReadVector3(), _packet.ReadQuartenion(), _packet.ReadBool(), _packet.ReadInt());
        }
        #endregion

    }

}
