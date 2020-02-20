using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TestServer.ClientManager;
using TestServer.Handlers;
using TestServer.Utils;
using TestServer.Manager;
using TestServer.ClientManager.User;

namespace TestServer.ServerManager
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static MySQLManager sql;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();
            Console.WriteLine("Starting mysql connection...");
            sql = new MySQLManager();
            sql.ipAdress = Program.sc.MySQL_Host;
            sql.username = Program.sc.MySQL_Username;
            sql.password = Program.sc.MySQL_Password;
            sql.databaseName = Program.sc.MySQL_Database;

            int rI = 1;
            while (!sql.Connect() && rI <= 3)
            {
                Console.WriteLine("Failed to connect to MySQL, reconnecting...");
                rI++;
                if (rI <= 3)
                {
                    int i = 1;
                    while (i < (1e+9))
                        i++;
                }
            }
            if (rI > 3){
                Program.CloseProgram();
                return;
            }

            var query = sql.Query("CREATE TABLE IF NOT EXISTS `Account` (" +
                "id INT AUTO_INCREMENT PRIMARY KEY," +
                "uid LONG," +
                "username TEXT," +
                "email TEXT," +
                "password TEXT," +
                "nickname TEXT" +
                ")"); ;
            query.ExecuteNonQuery();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            try
            {
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallBack), null);
            } catch (Exception e)
            {
                Console.WriteLine($"{e}\nFailed to start the server, the server port is already in use, check if it contains any applications using this port.");
                Program.CloseProgram();
                return;
            }

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallBack, null);

            //Server started and send logo && change status
            Console.Clear();
            if (Program.sc.defaultLogo)
                Console.WriteLine(Program.Logo);
            else
                Console.WriteLine(Program.lg.Generate(Program.sc.logoText));
            Console.WriteLine("\n______________________________________________________________");
            Console.Title = $"{Program.sc.serverName} | Started | Port: {Port}";
            Console.WriteLine($"Server started on port {Port}.");
            ConsoleSender.Send(MessageType.Normal, "Server ready to receive/send packets.");

            Program.ServerStarted();

            //Account.Create("Teste1", "teste123@gmail.com", "123");
            //Account.Create("Teste2", "teste2123@gmail.com", "123");

        }

        private static void TCPConnectCallBack(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallBack), null);

            ConsoleSender.Send(MessageType.Normal, $"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            ConsoleSender.Send(MessageType.Error, $"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void UDPReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallBack, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            } catch (Exception e)
            {
                ConsoleSender.Send(MessageType.Error, $"receiving UDP data: {e}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            } catch (Exception e)
            {
                ConsoleSender.Send(MessageType.Error, $"Sending packet data to user {_clientEndPoint} via UDP: {e}");
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.sendCredentials, ServerHandle.ReceivedCredentials},
                {(int)ClientPackets.RegisterAccount, ServerHandle.RegisterAccount},
                {(int)ClientPackets.LoginAccount, ServerHandle.LoginAccount},
                {(int)ClientPackets.UpdatePlayerPosition, ServerHandle.UpdatePlayerPosition},
            };
            Console.WriteLine("Packets initialized.");
        }

    }

}
