using System;
using System.Collections.Generic;
using System.Text;
using TestServer.ClientManager;
using TestServer.Manager;
using TestServer.ServerManager;
using System.Numerics;
using TestServer.ClientManager.User;
using TestServer.ClientManager.Game;

namespace TestServer.Utils
{
    class GameLogic
    {

        public static Vector3 positionSpawn = new Vector3(0, 0, 0);
        public static Quaternion rotationSpawn = new Quaternion(0, 0, 0, 0);

        public static void Update()
        {
            ThreadManager.UpdateMain();
        }

        public static void SendToGame(int _id)
        {
            ServerSend.SpawnPlayer(_id, positionSpawn, rotationSpawn, true);
            foreach(Account _account in Account.accounts.Values)
            {
                AccountDataManager adm = new AccountDataManager();
                if (_id != _account.id)
                {
                    adm.id = _account.id;
                    Player _player = adm.getPlayer;
                    Vector3 _position = positionSpawn;
                    Quaternion _rotation = rotationSpawn;
                    if (_player.hasSpawned) {
                        _position = _player.position;
                        _rotation = _player.rotation;
                    }
                    ServerSend.SpawnPlayer(_id, adm.id, _position, _rotation, false);
                }
            }
        }

    }
}
