using System;
using System.Collections.Generic;
using System.Text;
using TestServer.Handlers;
using TestServer.Utils;

namespace TestServer.CommandManager
{
    class Help : ServerCommand
    {

        private int page = 1;
        PageManager pages;

        public override void PreRun() {
            SetDescription("This command has the function of showing all commands.");
        }

        public override void Run()
        {
            if (args.Length > 0)
            {
                try
                {
                    page = int.Parse(args[0]);
                }
                catch (Exception e)
                {
                    page = 1;
                }
            }
            else
                page = 1;
            pages = new PageManager(10, new List<string>(CommandHandle.instance.descriptions.Keys).ToArray());
            if (page < 0)
                page = 1;
            if (page > pages.getTotalPages())
                page = pages.getTotalPages();
            sendPage();
        }

        public void sendPage()
        {
            string _result = pages.getPage(page);
            ConsoleSender.Send(MessageType.Warning, $"-------------------- Help: Index ({page}/{pages.getTotalPages()}) --------------------".Substring(0, 59));
            ConsoleSender.Send(MessageType.Normal, "Use \"help [n]\" to go to the next page(Note: '[n]' = Page number)");
            ConsoleSender.Send(MessageType.Normal, "");
            ConsoleSender.Send(MessageType.Warning, "Commands  :  Description");
            string[] _values = _result.Split("<split>");
            foreach (string _msg in _values)
            {
                string _char = _msg.ToCharArray()[0].ToString();
                ConsoleSender.Send(MessageType.Normal, $"{ServerUtils.ReplaceFirst(_msg, _char, _char.ToUpper())}: {CommandHandle.instance.descriptions[_msg]}");
            }
            ConsoleSender.Send(MessageType.Warning, "-----------------------------------------------------------");
        }
    }
}
