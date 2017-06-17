using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NthDeveloper.AppFramework.Commands
{
    public interface ICommand : INotifyPropertyChanged
    {
        string Name { get; }
        string Text { get; set; }
        string Description { get; set; }
        bool IsEnabled { get; }        
        object Icon { get; set; }
        IList<int> Hotkeys { get; set; }

        void Run();
        void Run(IDictionary<string, object> context);
    }
}
