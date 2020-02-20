using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.ServerManager.ConsoleUtils
{
    class LogoGenerator
    {

        private Dictionary<string, string> tLogo;
        //private char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public void Init()
        {
            tLogo = new Dictionary<string, string>();
            tLogo.Add(" ",
                " " + "{cend}" +
                " " + "{cend}" +
                " " + "{cend}" +
                " " + "{cend}" +
                " "
            );

            tLogo.Add("A",
                "    _    " + "{cend}" +
                "   / \\   " + "{cend}" +
                "  / _ \\  " + "{cend}" +
                " / ___ \\ " + "{cend}" +
                "/_/   \\_\\"
            );

            tLogo.Add("B",
                " ____  " + "{cend}" +
                "| __ ) " + "{cend}" +
                "|  _ \\ " + "{cend}" +
                "| |_) |" + "{cend}" +
                "|____/ "
            );

            tLogo.Add("C",
                "  ____ " + "{cend}" +
                " / ___|" + "{cend}" +
                "| |    " + "{cend}" +
                "| |___ " + "{cend}" +
                " \\____|"
            );

            tLogo.Add("D",
                " ____  " + "{cend}" +
                "|  _ \\ " + "{cend}" +
                "| | | |" + "{cend}" +
                "| |_| |" + "{cend}" +
                "|____/ "
            );

            tLogo.Add("E",
                " _____ " + "{cend}" +
                "| ____|" + "{cend}" +
                "|  _|  " + "{cend}" +
                "| |___ " + "{cend}" +
                "|_____|"
            );

            tLogo.Add("F",
                " _____ " + "{cend}" +
                "|  ___|" + "{cend}" +
                "| |_   " + "{cend}" +
                "|  _|  " + "{cend}" +
                "|_|    "
            );

            tLogo.Add("G",
                "  ____ " + "{cend}" +
                " / ___|" + "{cend}" +
                "| |  _ " + "{cend}" +
                "| |_| |" + "{cend}" +
                " \\____|"
            );

            tLogo.Add("H",
               " _   _ " + "{cend}" +
               "| | | |" + "{cend}" +
               "| |_| |" + "{cend}" +
               "|  _  |" + "{cend}" +
               "|_| |_|"
           );

            tLogo.Add("I",
               " ___ " + "{cend}" +
               "|_ _|" + "{cend}" +
               " | | " + "{cend}" +
               " | | " + "{cend}" +
               "|___|"
           );

            tLogo.Add("J",
               "     _ " + "{cend}" +
               "    | |" + "{cend}" +
               " _  | |" + "{cend}" +
               "| |_| |" + "{cend}" +
               " \\___/ "
           );

            tLogo.Add("K",
               " _  __" + "{cend}" +
               "| |/ /" + "{cend}" +
               "| ' / " + "{cend}" +
               "| . \\ " + "{cend}" +
               "|_|\\_\\"
           );

            tLogo.Add("L",
               " _     " + "{cend}" +
               "| |    " + "{cend}" +
               "| |    " + "{cend}" +
               "| |___ " + "{cend}" +
               "|_____|"
           );

            tLogo.Add("M",
               " __  __ " + "{cend}" +
               "|  \\/  |" + "{cend}" +
               "| |\\/| |" + "{cend}" +
               "| |  | |" + "{cend}" +
               "|_|  |_|"
           );

            tLogo.Add("N",
               " _   _ " + "{cend}" +
               "| \\ | |" + "{cend}" +
               "|  \\| |" + "{cend}" +
               "| |\\  |" + "{cend}" +
               "|_| \\_|"
           );



        }

        public string Generate(string text)
        {
            string[] logo = new string[]{ "", "", "", "", "" };
            char[] chars = text.ToCharArray();

            foreach (char l in chars)
            {
                string letter = l.ToString();
                string tlg = tLogo[letter];

                string[] tlgS = tlg.Split("{cend}");

                for (int i = 0; i < 5; i++)
                {
                    logo[i] = logo[i] + tlgS[i];
                }

            }

            string flog = "";

            foreach (string lg in logo)
            {
                flog = flog + lg + "\n";
            }

            return flog;
        }

    }
}
