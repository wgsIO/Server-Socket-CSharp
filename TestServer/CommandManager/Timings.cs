using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using TestServer.Handlers;
using TestServer.Utils;

namespace TestServer.CommandManager
{

    class Timings : ServerCommand
    {

        public override void PreRun() {
            SetDescription("This command is used to calculate the server's timings.");
            
        }

        public override void Run()
        {
            bool isCalculing = true;

            double frameInterval = 1000 / 30;
            Stopwatch msCalc = new Stopwatch();
            Stopwatch tpsCalc = new Stopwatch();

            double tpsMax = 30d;
            double tpsCount = 0d;

            while (isCalculing)
            {
                msCalc.Start();
                tpsCalc.Start();

                if (msCalc.ElapsedMilliseconds > frameInterval)
                {
                    tpsCount++;
                    msCalc.Reset();
                }
                if (tpsCount >= tpsMax)
                {
                    isCalculing = false;

                    msCalc.Stop();
                    tpsCalc.Stop();

                    double time = (tpsCalc.ElapsedMilliseconds / 1000d);

                    double oneM = Math.Round((tpsMax / time), 2);
                    double fiveM = Math.Round(((tpsMax / (time * 5)) * (tpsMax / 5)) - 6, 2); //(tpsMax / (time * 5)) * (tpsMax / 10);
                    double fifTeenM = Math.Round(((tpsMax/ (time * 15)) * (tpsMax / 15)) * 6, 2);

                    ConsoleSender.Send(MessageType.Normal, $"Tps from 1m, 5m, 15m: {oneM}, {fiveM}, {fifTeenM}");
                }
            }
        }
    }
}
