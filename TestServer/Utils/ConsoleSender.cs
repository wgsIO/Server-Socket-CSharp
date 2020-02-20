using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.Utils
{
    public enum MessageType
    {
        Normal,
        Warning,
        Error
    }

    class ConsoleSender
    {

        public static void Send(string _msg)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} INFO]: {_msg}");
        }
        public static void Send(MessageType _type, string _msg)
        {
            int i = (int)_type;
            string _warn = "";
            switch (_type)
            {
                case MessageType.Normal:
                    _warn = "INFO";
                    break;
                case MessageType.Warning:
                    _warn = "WARNING";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Error:
                    _warn = "ERROR";
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} {_warn}]: {_msg}");
            Console.ResetColor();
        }

    }
}
