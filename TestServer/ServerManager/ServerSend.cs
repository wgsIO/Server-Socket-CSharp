using System;
using System.Collections.Generic;
using System.Text;
using TestServer.ClientManager;
using TestServer.ClientManager.Game;
using TestServer.ClientManager.User;
using System.Numerics;
using TestServer.Utils;
using System.Threading;

namespace TestServer.ServerManager
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for(int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }

        #region Packets
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void MessageBox(int _toClient, string _messager, string _type, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.MessageBox))
            {
                _packet.Write(_messager);
                _packet.Write(_type);
                _packet.Write(_msg);

                SendUDPData(_toClient, _packet);
            }
        }

        public static void LoginSuccefully(int _toClient, Account account)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LoginSuccefully))
            {
                _packet.Write(account);

                SendUDPData(_toClient, _packet);

                Thread.Sleep(150);
                GameLogic.SendToGame(_toClient);
            }
        }

        #region InGame
        public static void SpawnPlayer(int _toClient, Vector3 _position, Quaternion _rotation, bool sendToAll) => SpawnPlayer(_toClient, _toClient, _position, _rotation, sendToAll);
        public static void SpawnPlayer(int _toClient, int _ClientID, Vector3 _position, Quaternion _rotation, bool sendToAll)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SpawnPlayer))
            {
                if (Player.Spawn(_toClient, _position, _rotation)) {

                    AccountDataManager adm = new AccountDataManager();
                    adm.id = _ClientID;
                    Player _player = adm.getPlayer;

                    _packet.Write(_ClientID);
                    _packet.Write(_player.position);
                    _packet.Write(_player.rotation);

                    if (sendToAll)
                        SendTCPDataToAll(_packet);
                    else
                        SendTCPData(_toClient, _packet);
                }
            }
        }
        public static void SendPlayerPosition(int _toClient, Player _player, bool _running, int _y)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SendPlayerPosition))
            {
                _packet.Write(_toClient);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);
                _packet.Write(_running);
                _packet.Write(_y);

                SendUDPDataToAll(_toClient, _packet);
            }
        }
        public static void FixPlayerPosition(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SendPlayerPosition))
            {
                _packet.Write(_toClient);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);
                _packet.Write(false);
                _packet.Write(0);

                SendUDPDataToAll(_packet);
            }
        }
        #endregion
        #endregion
    }
}
