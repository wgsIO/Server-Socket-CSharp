using System;
using System.Collections.Generic;
using System.Text;
using TestServer.Utils;

namespace TestServer.Handlers
{
    public abstract class ServerCommand
    {
        public string preDescription = "Undefined";
        public string command;
        public bool preRunned;

        public string[] args = new string[0];

        public SubCommand sc;

        public void SetDescription(string _description) => preDescription = _description;

        public class SubCommand
        {

            private string command;
            private Dictionary<string, ThreeValue<Action, string, MultiValue<int, string>>> sub_cmds;
            public List<string> real_scmds { get; private set; }
            public List<string> values_scmds { get; private set; }

            public SubCommand(string _cmd)
            {
                command = _cmd;
                sub_cmds = new Dictionary<string, ThreeValue<Action, string, MultiValue<int, string>>>();
                real_scmds = new List<string>();
                values_scmds = new List<string>();
            }

            public RegisterSubCommand getCommand(Action _action, params string[] _scmds)
            {
                return new RegisterSubCommand(this, _action, _scmds);
                
            }

            public object getCommands() => sub_cmds;


            public enum RegisterAction
            {
                CREATE,
                REMOVE,
                EDIT
            }

            public class RegisterSubCommand
            {

                private SubCommand sc;
                private string[] scmds;
                private Action action;

                private string description = "";

                private int argumentsSize;
                private Dictionary<int, string> arguments;

                public RegisterSubCommand(SubCommand _sc, Action _action, String[] _scmds)
                {
                    sc = _sc;
                    scmds = _scmds;
                    action = _action;
                    arguments = new Dictionary<int, string>();
                    if (sc.sub_cmds.ContainsKey(_scmds[0]))
                    {
                        description = sc.sub_cmds[_scmds[0]].getTwo();

                        string[] _args = sc.sub_cmds[_scmds[0]].getThree().getTwo().Replace("<", "").Replace(">", "").Split(" ");
                        if (_args.Length > 0)
                        {
                            for (int i = 0; i < _args.Length; i++)
                            {
                                arguments.Add(i, _args[i]);
                            }
                        }
                    }
                }

                public RegisterSubCommand setDescription(string _description) { description = _description; return this; }

                public string getDescription() => description;

                public RegisterSubCommand addParam(int _index, string _param) { arguments.Add(_index, _param); return this; }
                public RegisterSubCommand setParam(int _index, string _param) { arguments[_index] = _param; return this; }
                public RegisterSubCommand removeParam(int _index) { arguments.Remove(_index); return this; }

                public RegisterSubCommand setTotalArguments(int _total) { argumentsSize = _total; return this;  }


                public void Done(RegisterAction _raction)
                {

                    switch (_raction)
                    {
                        case RegisterAction.CREATE:
                            {
                                string _params = "Not defined";
                                if (arguments.Count > 0)
                                {
                                    _params = "";
                                    foreach (string _param in arguments.Values)
                                    {
                                        _params = _params + " <" + _param + ">";
                                    }
                                }

                                bool registred = false;
                                foreach (string _scmd in scmds)
                                {
                                    if (!sc.sub_cmds.ContainsKey(_scmd))
                                    {
                                        if (!registred)
                                            if (!sc.real_scmds.Contains(_scmd))
                                                sc.real_scmds.Add(_scmd);
                                        registred = true;
                                        sc.sub_cmds.Add(_scmd, new ThreeValue<Action, string, MultiValue<int, string>>(action, description, new MultiValue<int, string>(argumentsSize, _params)));
                                    }
                                    else
                                        ConsoleSender.Send(MessageType.Error, $"The sub command \"{_scmd}\" is already registered in {sc.command} command.");
                                }
                                break;
                            }
                        case RegisterAction.REMOVE:
                            {
                                bool deleted = false;
                                foreach (string _scmd in scmds)
                                {
                                    if (sc.sub_cmds.ContainsKey(_scmd))
                                    {
                                        if (!deleted)
                                            if (sc.real_scmds.Contains(_scmd))
                                                sc.real_scmds.Remove(_scmd);
                                        sc.sub_cmds.Remove(_scmd);
                                    } else
                                        ConsoleSender.Send(MessageType.Error, $"The sub command \"{_scmd}\" is not registered in {sc.command} command.");
                                }
                                break;
                            }
                        case RegisterAction.EDIT:
                            {
                                string _params = "Not defined";
                                if (arguments.Count > 0)
                                {
                                    _params = "";
                                    foreach (string _param in arguments.Values)
                                    {
                                        _params = _params + "<" + _param + "> ";
                                    }
                                }
                                foreach (string _scmd in scmds)
                                {

                                    if (!sc.sub_cmds.ContainsKey(_scmd))
                                        sc.sub_cmds[_scmd] = new ThreeValue<Action, string, MultiValue<int, string>>(action, description, new MultiValue<int, string>(argumentsSize, _params));
                                    else
                                        ConsoleSender.Send(MessageType.Error, $"The sub command \"{_scmd}\" is not registered in {sc.command} command.");
                                }
                                break;
                            }
                    }

                }


            }


        }

        public dynamic convertArgToValue<T>(int _index)
        {
            var _values = sc.values_scmds;
            dynamic v = null;
            if (typeof(T) == typeof(int))
                try{ v = int.Parse(_values[_index]); } catch (Exception _e) { v = 0; }
            else if (typeof(T) == typeof(float))
                try { v = float.Parse(_values[_index]); } catch (Exception _e) { v = 0f; }
            else if (typeof(T) == typeof(long))
                try { v = long.Parse(_values[_index]); } catch (Exception _e) { v = 0l; }
            else if (typeof(T) == typeof(double))
                try { v = double.Parse(_values[_index]); } catch (Exception _e) { v = 0d; }
            else if (typeof(T) == typeof(bool))
                try { v = bool.Parse(_values[_index]); } catch (Exception _e) { v = false; }
            else if (typeof(T) == typeof(string))
                v = _values[_index];
            return v;
        }

        public bool readSubCommand()
        {
            var _scmds = (Dictionary<string, ThreeValue<Action, string, MultiValue<int, string>>>)sc.getCommands();
            if (args.Length > 0)
            {
                if (_scmds.ContainsKey(args[0]))
                {
                    if (args.Length >= _scmds[args[0]].getThree().getOne() + 1)
                    {
                        string[] _vcmds = ServerUtils.ReplaceFirst(string.Join(" ", args), args[0], "").Split(" ");
                        sc.values_scmds.Clear();
                        if (_vcmds.Length > 0)
                            for (int i = 0; i < _vcmds.Length; i++)
                                sc.values_scmds.Add(_vcmds[i]);
                        _scmds[args[0]].getOne()();
                    }  else
                    {
                        ConsoleSender.Send(MessageType.Error, $"Correct mode: {command} {args[0]} {_scmds[args[0]].getThree().getTwo()}");
                    }
                    return true;
                } else
                {
                    ConsoleSender.Send(MessageType.Error, $"The entered sub command not found or nonexistent in {command} command.");
                }
            } else
            {
                ConsoleSender.Send(MessageType.Error, $"Sub commands of {command}: ");
                foreach (var _scmd in sc.real_scmds)
                {
                    ConsoleSender.Send(MessageType.Error, $"-> {_scmd} {_scmds[_scmd].getThree().getTwo()}");
                }
            }
            return false;
        }

        public abstract void PreRun();

        public abstract void Run();
    }

    class CommandHandle
    {
        public static CommandHandle instance;

        public Dictionary<string, ServerCommand> commands;
        public Dictionary<string, string> descriptions;

        public void Start()
        {
            instance = this;
            commands = new Dictionary<string, ServerCommand>();
            descriptions = new Dictionary<string, string>();
        }

        public bool callCommand(string _cmd)
        {
            string[] _values = _cmd.Split(" ");
            if (_values.Length > 0 && commands.ContainsKey(_values[0]))
            {
                if (_values.Length > 1)
                {
                    string[] _args = ServerUtils.ReplaceFirst(ServerUtils.ReplaceFirst(_cmd, $"{_values[0]} ", ""), _values[0], "").Split(" ");
                    commands[_values[0]].args = _args;
                }
                commands[_values[0]].Run();

                return true;
            }
            return false;
        }

        public void RegisterCommand(ServerCommand _class, params string[] _cmds)
        {
            bool isPreRun = false;
            string _rcmd = "";
            foreach (string _cmd in _cmds)
            {
                if (!commands.ContainsKey(_cmd))
                {
                    if (!isPreRun)
                    {
                        _class.PreRun();
                        _rcmd = _cmd;
                    }
                    _class.command = _rcmd;
                    _class.preRunned = isPreRun;
                    commands.Add(_cmd, _class);
                    if (!isPreRun)
                    {
                        isPreRun = true;
                        if (_class.preDescription != null)
                            setDescription(_cmd, _class.preDescription);
                    }

                }
                else
                    ConsoleSender.Send(MessageType.Error, $"The command \"{_cmd}\" is already registered.");
            }
        }
        
        public void setCommands(Dictionary<string, ServerCommand> _cmds)
        {
            commands = _cmds;
        }

        public void setDescription(string _cmd, string _description)
        {
            if (commands.ContainsKey(_cmd))
                if (descriptions.ContainsKey(_cmd))
                    descriptions[_cmd] = _description;
                else
                    descriptions.Add(_cmd, _description);
            else
                ConsoleSender.Send(MessageType.Error, $"The command \"{_cmd}\" not registered.");
        }

        public string getDescription(string _cmd)
        {
            if (commands.ContainsKey(_cmd))
                return descriptions[_cmd];
            else
                ConsoleSender.Send(MessageType.Error, $"The command \"{_cmd}\" not registered.");
            return null;
        }

        public Dictionary<string, string> getDescriptions()
        {
            return descriptions;
        }

    }
}
