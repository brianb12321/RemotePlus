using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class SingleCommand : StandordCommandClass
    {
        CommandDelegate _tempCommand;
        string _tempName;
        public SingleCommand(string name, CommandDelegate command)
        {
            _tempName = name;
            _tempCommand = command;
        }
        public override void AddCommands()
        {
            Commands.Add(_tempName, _tempCommand);
        }
    }
}
