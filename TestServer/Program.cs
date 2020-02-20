using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using TestServer.ClientManager;
using TestServer.CommandManager;
using TestServer.Handlers;
using TestServer.Manager;
using TestServer.ServerManager;
using TestServer.ServerManager.ConsoleUtils;
using TestServer.Utils;


namespace TestServer
{
    class Program
    {
        public static string Logo =
            (
            "   _____                       _____                         " + "\n" +
             "  / ____|                     / ____|                         " + "\n" +
             " | |  __  __ _ _ __ ___   ___| (___   ___ _ ____   _____ _ __ " + "\n" +
             " | | |_ |/ _` | '_ ` _ \\ / _ \\\\___ \\ / _ \\ '__\\ \\ / / _ \\ '__|" + "\n" +
             " | |__| | (_| | | | | | |  __/____) |  __/ |   \\ V /  __/ |   " + "\n" +
             "  \\_____|\\__,_|_| |_| |_|\\___|_____/ \\___|_|    \\_/ \\___|_|   "
            );
        public static ServerConfig sc;
        public static LogoGenerator lg;

        private static bool isRunning = false;

        private static Stopwatch timings = System.Diagnostics.Stopwatch.StartNew();

        static void Main(string[] args)
        {
            Console.Title = "Please wait | Loading...";
            isRunning = true;

            //Set main threading
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            lg = new LogoGenerator();
            lg.Init();

            string path = Assembly.GetExecutingAssembly().Location;
            Configs.createFolder(Path.GetDirectoryName(path) + "\\server");
            sc = ServerConfig.Load(Configs.configFile);
            sc.Save(Configs.configFile);

            Console.Title = $"{sc.serverName} | Starting...";

            Cooldown.getInstance().language = CooldownLanguage.ENGLISH;
            Cooldown.LoadData();

            Server.Start(50, 26950);

        }

        public static void ServerStarted()
        {
            ConsoleSender.Send(MessageType.Normal, "Registering commands, please wait...");

            CommandHandle cmdHandle = new CommandHandle();
            cmdHandle.Start();

            cmdHandle.RegisterCommand(new Timings(), "timings", "tps");
            cmdHandle.RegisterCommand(new Help(), "help", "?");
            cmdHandle.RegisterCommand(new PacketClientCounter(), "packetcount", "pc", "packetscounts", "pscs");

            timings.Stop();

            double timing = (timings.ElapsedMilliseconds / 1000d);

            ConsoleSender.Send(MessageType.Normal, $"Full boot in <{timing.ToString().Replace(",", ",")}s>! For help, type \"help\" or \"?\".");

            Thread serverReading = new Thread(new ThreadStart(ServerReading));
            serverReading.Start();

        }

        public static void CloseProgram()
        {
            Console.WriteLine("Press any key to close the server application.");
            char c = Console.ReadKey().KeyChar;
            System.Environment.Exit(0);
        }

        public static void ServerReading()
        {
            while (isRunning)
            {
                string _cmd = Console.ReadLine();
                if (_cmd.Length < 1 || CommandHandle.instance.callCommand(_cmd.ToLower())) {
                   
                }
                else
                    ConsoleSender.Send(MessageType.Error, "The entered command not found or nonexistent.");
            }
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started, running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {

                while (_nextLoop < DateTime.Now)
                {
                    //GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    try
                    {
                        GameLogic.Update();

                        //foreach (Client _client in PacketCounterManager.packetCounters.Keys)
                        //    foreach (string _name in PacketCounterManager.packetCounters[_client].Keys)
                          //      PacketCounterManager.packetCounters[_client][_name].NormalReset();

                        if (_nextLoop > DateTime.Now)
                        {
                            TimeSpan temp = _nextLoop - DateTime.Now;
                            //ConsoleSender.Send(MessageType.Warning, $"Next loop time: {temp.TotalMilliseconds} {temp}");
                            if (temp.TotalMilliseconds > 0)
                                Thread.Sleep(temp);
                            else
                            {
                                _nextLoop = DateTime.Now;
                                ConsoleSender.Send(MessageType.Error, "Loop count time is below 0, recalculating time...");
                            }



                        }
                    }
                    catch (Exception e)
                    {
                        ConsoleSender.Send(MessageType.Error, $"Failed to update game logic: {e}");
                    }
                }
            }
        }

    }
}
