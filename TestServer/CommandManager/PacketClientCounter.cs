using System;
using System.Collections.Generic;
using System.Text;
using TestServer.ClientManager;
using TestServer.Handlers;
using TestServer.Manager;
using TestServer.ServerManager;
using TestServer.Utils;

namespace TestServer.CommandManager
{
    class PacketClientCounter : ServerCommand
    {

        public override void PreRun() {
            SetDescription("This command has the function of showing packet client counter.");
            sc = new SubCommand(command);
            var _scr = sc.getCommand(sendClientPacketCount, "view", "v", "views", "show", "s");
            _scr.addParam(1, "client");
            _scr.setTotalArguments(1);
            _scr.Done(SubCommand.RegisterAction.CREATE);
        }

        public override void Run()
        {
            readSubCommand();
        }

        public void sendClientPacketCount()
        {

            int _clientId = convertArgToValue<int>(1);
            if (_clientId > 0)
            {
                if (Server.clients.ContainsKey(_clientId))
                {

                    Client _client = Server.clients[_clientId];
                    if (PacketCounterManager.packetCounters.ContainsKey(_client) && PacketCounterManager.packetCounters[_client].Count > 0)
                    {

                        ConsoleSender.Send(MessageType.Normal, $"Client selected: {_clientId}");
                        ConsoleSender.Send(MessageType.Normal, $"INSTANCES | CPS/MAX");

                        var _data = PacketCounterManager.packetCounters[_client];

                        foreach (string _name in _data.Keys)
                        {
                            ConsoleSender.Send(MessageType.Warning, $"{_name} | {_data[_name].count}/{_data[_name].countPermitible}");
                        }
                    } else
                        ConsoleSender.Send(MessageType.Error, $"No instances found from client {_clientId}.");
                }
                else
                    ConsoleSender.Send(MessageType.Error, "It does not contain any clients with this ID.");
            } else
                ConsoleSender.Send(MessageType.Error, "The CLIENT ID must be above '1'.");
        }

    }
}
