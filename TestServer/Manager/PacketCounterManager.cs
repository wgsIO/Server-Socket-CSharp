using System;
using System.Collections.Generic;
using System.Text;
using TestServer.ClientManager;
using TestServer.Utils;

namespace TestServer.Manager
{

    class PacketCounterManager
    {

        public static Dictionary<Client, Dictionary<string, PacketCounterManager>> packetCounters = new Dictionary<Client, Dictionary<string, PacketCounterManager>>();
        public static PacketCounterManager getPacketCounter(Client _client, string _name)
        {
            PacketCounterManager pcm = null;
            if (packetCounters.ContainsKey(_client))
                if (packetCounters[_client].ContainsKey(_name))
                    pcm = packetCounters[_client][_name];
            return pcm;
        }

        public static PacketCounterManager createIfNotExists(Client _client, string _name, long _count)
        {
            PacketCounterManager pcm = getPacketCounter(_client, _name);
            if (pcm == null) {
                pcm = new PacketCounterManager().setCountPermitiblePerSecond(_count).Register(_client, _name);
                pcm.RecauculateLastTime();
            }
            return pcm;
        }

        public long lastTime { get; private set; }
        public bool cancelled { get; private set; }
        public long countPermitible { get; private set; }
        public Client client { get; private set; }

        public long count { get; private set; }

        public string name { get; private set; }

        public PacketCounterManager setCountPermitiblePerSecond(long _count)
        {
            countPermitible = _count;
            return this;
        }

        public PacketCounterManager RecauculateLastTime()
        {
            lastTime = Cooldown.getInstance().getCurrentTime();
            return this;
        }

        public PacketCounterManager Register(Client _client, string _name)
        {
            cancelled = false;
            count = 0;
            if (!packetCounters.ContainsKey(_client))
                packetCounters.Add(_client, new Dictionary<string, PacketCounterManager>());
            if (!packetCounters[_client].ContainsKey(_name))
                packetCounters[_client].Add(_name, this);
            name = _name;
            client = _client;
            return this;
        }

        public void AddMoreCount() => count++;

        public void ResetCount() => count = 0;

        public void Reset()
        {
            RecauculateLastTime();
            ResetCount();
        }

        public void Verify()
        {
            long _time = Cooldown.getInstance().getCurrentTime();
            //Console.WriteLine(_time + "< New");
            //Console.WriteLine(lastTime + "< Old");
            //Console.WriteLine((lastTime + 1) + "< NOld");
            if (_time > lastTime + 1)
            {
                Reset();
            } else {
                if (count > countPermitible)
                {
                    Console.WriteLine($"Cancelado: ID: {client.id} {count}/{countPermitible} de {name}");
                    cancelled = true;
                }
            }
        }

        public void printPacketCount() => ConsoleSender.Send(MessageType.Warning, $"[{client.id}] send {count}/{countPermitible} Packets.");


    }
}
