using System;
using System.Collections.Generic;
using System.ComponentModel;
using NthDeveloper.AppFramework.Extensibility;

namespace NthDeveloper.AppFramework.Commands
{
    public class Command : ICommand
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command()
        {
            m_Hotkeys = new List<int>();
        }

        public Command(IPluginHost host) : this()
        {
            this.PluginHost = host;            
        }

        protected IPluginHost PluginHost { get; private set; }

        #region ICommand Members

        public string Name { get; protected set; }

        string m_Text;
        public string Text 
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                RaisePropertyChanged("Text");
            }
        }

        string m_Description;
        public string Description 
        {
            get { return m_Description; }
            set
            {
                m_Description = value;
                RaisePropertyChanged("Description");
            }
        }

        bool m_IsEnabled;
        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            protected set
            {
                m_IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        object m_Icon;
        public object Icon
        {
            get { return m_Icon; }
            set
            {
                m_Icon = value;
                RaisePropertyChanged("Icon");
            }
        }

        IList<int> m_Hotkeys;
        public IList<int> Hotkeys
        {
            get { return m_Hotkeys; }
            set
            {
                m_Hotkeys = value;
                RaisePropertyChanged("Hotkeys");
            }
        }

        public virtual void Run()
        {
        }

        public virtual void Run(IDictionary<string, object> context)
        {
            Run();
        }

        #endregion  
      
        protected void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
