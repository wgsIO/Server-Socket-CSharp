using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TestServer.Manager;
using TestServer.ServerManager;
using TestServer.Utils;
using System.Numerics;
using TestServer.ClientManager.User;

namespace TestServer.ClientManager
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public UDP udp;

        public Account account;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);

                //TODO: Send welcome packet
                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                } catch (Exception e)
                {
                    ConsoleSender.Send(MessageType.Error, $"Sending packet data to user {id} via TCP: {e}");
                }
            }

            private void ReceiveCallBack(IAsyncResult _result)
            {
                try
                {
                    if (_result != null)
                    {
                        int _byteLenght = 0;
                        try
                        {
                            _byteLenght = stream.EndRead(_result);
                        } catch (Exception e) { }
                        if (_byteLenght <= 0)
                        {
                            //TODO: Disconnect
                            Server.clients[id].Disconnect();
                            return;
                        }

                        byte[] _data = new byte[_byteLenght];
                        Array.Copy(receiveBuffer, _data, _byteLenght);

                        //TODO: Handle data
                        receivedData.Reset(HandleData(_data));
                        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
                    }
                } catch (Exception e)
                {
                    //TODO: Disconnect
                    ConsoleSender.Send(MessageType.Error, $"receiving TCP data: {e}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLenght = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLenght = receivedData.ReadInt();
                    if (_packetLenght <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLenght > 0 && _packetLenght <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLenght);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLenght = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLenght = receivedData.ReadInt();
                        if (_packetLenght <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLenght <= 1)
                {
                    return true;
                }

                return false;

            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;

                if (Server.clients[id].account != null)
                {
                    Account.accounts.Remove(Server.clients[id].account.username);
                    Server.clients[id].account = null;
                }
                
                if (PacketCounterManager.packetCounters.ContainsKey(Server.clients[id]) && PacketCounterManager.packetCounters[Server.clients[id]].Count > 0)
                    foreach (string _name in PacketCounterManager.packetCounters[Server.clients[id]].Keys)
                        PacketCounterManager.packetCounters[Server.clients[id]].Remove(_name);

                //Server.clients.Remove(id);
            }

        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int _id)
            {
                id = _id;
            } 

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData)
            {
                int _packetLenght = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLenght);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        public void SendMessageBox(string _messager, string _type, string _msg)
        {
            ServerSend.MessageBox(id, _messager, _type, _msg);
        }

        public void Disconnect()
        {
            if (tcp.socket != null)
            {
                ConsoleSender.Send(MessageType.Warning, $"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

                tcp.Disconnect();
                udp.Disconnect();
            }
        }

    }

}
