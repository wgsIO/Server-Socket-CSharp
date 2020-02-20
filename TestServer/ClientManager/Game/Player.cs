using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using TestServer.ServerManager;
using TestServer.Utils;
using TestServer.ClientManager.User;
using System.Threading;

namespace TestServer.ClientManager.Game
{
    public class AccountDataManager
    {

        public int id { get; set; }

        public AccountData getAccountData { get => Server.clients[id].account.accountData; set => Server.clients[id].account.accountData = value; }

        public Player getPlayer { get => Server.clients[id].account.accountData.player; set => Server.clients[id].account.accountData.player = value; }

    }

    public class Player
    {

        public float speed = 0.17f;
        public float runSpeed = 1f;

        public bool fixing = false;

        public bool hasSpawned;
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }

        //public void setRunningSpeed

        public static bool Spawn(int _id, Vector3 _position, Quaternion _rotation) {
            AccountDataManager adm = new AccountDataManager();
            adm.id = _id;
            if (!adm.getPlayer.hasSpawned)
            {
                adm.getPlayer.position = _position;
                adm.getPlayer.rotation = _rotation;
                adm.getPlayer.hasSpawned = true;
            }
            return true;
        }

        public static bool UpdatePositions(int _id, Vector3 _position, Quaternion _rotation, bool _running, int _y)
        {
            AccountDataManager adm = new AccountDataManager();
            adm.id = _id;

            if (!adm.getPlayer.hasSpawned)
                return false;

            adm.getPlayer.rotation = _rotation;

            Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), adm.getPlayer.rotation) * _y;

            if (!adm.getPlayer.fixing)
            {
                var _result = VerifyPositions(_id, _position, _forward, _running, _y);
                if (_result.getOne())
                {
                    adm.getPlayer.position = _position;
                }
                else //if (_y != 0)
                {
                    adm.getPlayer.position = _result.getTwo();
                    ServerSend.FixPlayerPosition(_id, adm.getPlayer);
                    //Thread.Sleep(150);
                    adm.getPlayer.fixing = false;
                    return true;
                }
            } else
                adm.getPlayer.position = _position;

            ServerSend.SendPlayerPosition(_id, adm.getPlayer, _running, _y);

            return true;
        }

        public static MultiValue<bool, Vector3> VerifyPositions(int _id, Vector3 _position, Vector3 _forward, bool _running, int _y)
        {
            AccountDataManager adm = new AccountDataManager();
            adm.id = _id;

            if (!adm.getPlayer.hasSpawned)
                return new MultiValue<bool, Vector3>(false, _position);

            float _speed = adm.getPlayer.speed;
            if (_running)
                _speed = (((_speed * ((adm.getPlayer.runSpeed / 1.7f) * adm.getPlayer.runSpeed)) / 2.5f) * 4);

            Vector3 _vel = Vector3.Normalize(_forward) * _speed;

            Vector3 _vposition = adm.getPlayer.position + _vel;

            if (_vposition == _position)
                return new MultiValue<bool, Vector3>(true, _vposition);

            return new MultiValue<bool, Vector3>(PositionPermitible(_y, _position, _vposition), _vposition);
        }

        public static bool PositionPermitible(int _id, Vector3 _position, Vector3 _vposition)
        {
            ///bool _accepted = false;
            //switch (_y)
            //{
            //    case 1:
            //        if (_position.X ==_vposition.X || _position.Y == _vposition.Y || _position.Z == _vposition.Z)
            //            _accepted = true;
            //        break;
            //    case -1:
            //        if (_position.X >= _vposition.X || _position.Y >= _vposition.Y || _position.Z >= _vposition.Z)
            //            _accepted = true;
            //        break;
            //}
            return PosPermitibleVerify(_id, _position, _vposition);
        }

        public static bool PosPermitibleVerify(int _id, Vector3 _position, Vector3 _vposition)
        {
            bool _accepted = true;
            float _distance = MathF.Round(Vector3.Distance(_position, _vposition));
            if (_distance > 0)
            {
                if (_distance < 5)
                {
                    _accepted = false;
                    AccountDataManager adm = new AccountDataManager();
                    adm.id = _id;
                    adm.getPlayer.fixing = true;
                }
                //Console.WriteLine($"Distancia: {Vector3.Distance(_position, _vposition)}");
            }
            return _accepted;
        }

        public static bool PosPermitibleVerifyDEPRECATED(Vector3 _position, Vector3 _vposition)
        {
            bool _accepted = true;
            _vposition *= 4;

            #region XPOS
            //X > 0
            if (_vposition.X > 0)
                if (_position.X <= _vposition.X)
                    _accepted = true;
                else
                    _accepted = false;

            //X < 0
            else if (_vposition.X < 0)
                if (_position.X >= _vposition.X)
                    _accepted = true;
                else
                    _accepted = false;
            #endregion

            #region YPOS
            //Y > 0
            if (_vposition.Y > 0)
                if (_position.Y <= _vposition.Y)
                    _accepted = true;
                else
                    _accepted = false;

            //Y < 0
            else if (_vposition.Y < 0)
                if (_position.Y >= _vposition.Y)
                    _accepted = true;
                else
                    _accepted = false;
            #endregion

            #region ZPOS
            //Z > 0
            if (_vposition.Z > 0)
                if (_position.Z <= _vposition.Z)
                    _accepted = true;
                else
                    _accepted = false;

            //Z < 0
            else if (_vposition.Z < 0)
                if (_position.Z >= _vposition.X)
                    _accepted = true;
                else
                    _accepted = false;
            #endregion

            return _accepted;

        }

    }
}
