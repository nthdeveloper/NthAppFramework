using System;
using System.Collections.Generic;

namespace NthDeveloper.AppFramework.Commands
{
    public interface ICommandManager
    {
        IList<ICommand> Commands { get; }

        void RegisterCommand(ICommand op);
        ICommand GetCommand(string name);
        void RunCommand(string name);
        void RunCommand(string name, IDictionary<string, object> context);
    }
}
