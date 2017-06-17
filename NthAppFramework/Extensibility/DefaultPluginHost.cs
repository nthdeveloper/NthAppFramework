using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using NthDeveloper.AppFramework.ServiceLocator;

namespace NthDeveloper.AppFramework.Extensibility
{
    public class DefaultPluginHost : IPluginHost
    {
        List<IPlugin> m_Plugins;

        public DefaultPluginHost(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.ServiceLocator.RegisterService<IPluginHost>(this);

            m_Plugins = new List<IPlugin>();
        }

        #region IPluginHost Members

        public IServiceLocator ServiceLocator { get; private set; }

        public bool LoadModule(string filePath)
        {
            Assembly _asm = null;

            try
            {
                if (!File.Exists(filePath))
                    return false;

                _asm = Assembly.LoadFile(filePath);
                return LoadModule(_asm);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool LoadModule(Assembly asm)
        {
            try
            {
                List<Type> _typeList = findInterfaces(typeof(IPlugin), asm);

                for (int i = 0; i < _typeList.Count; i++)
                {
                    IPlugin _plugin = (IPlugin)Activator.CreateInstance(_typeList[i]);
                    _plugin.Initialize(this);
                    m_Plugins.Add(_plugin);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool LoadPlugin(IPlugin plugin)
        {
            try
            {
                plugin.Initialize(this);
                m_Plugins.Add(plugin);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public void ShutDown()
        {
            for (int i = 0; i < m_Plugins.Count; i++)
            {
                try { m_Plugins[i].ShutDown(); }
                catch (Exception ex)
                { }
            }
        }

        #endregion

        private Type findInterface(Type typToFind, Assembly asm)
        {
            Type _foundType = null;
            Type[] _types = asm.GetExportedTypes();

            foreach (Type typ in _types)
            {
                if (!typ.IsClass) continue;

                Type[] _interfaces = typ.GetInterfaces();
                if (_interfaces.Length == 0) continue;

                for (int i = 0; i < _interfaces.Length; i++)
                    if (_interfaces[i] == typToFind)
                    {
                        _foundType = typ;
                        break;
                    }

                if (_foundType != null) break;
            }

            return _foundType;
        }

        private List<Type> findInterfaces(Type typToFind, Assembly asm)
        {
            List<Type> _foundTypes = new List<Type>();
            Type[] _types = asm.GetExportedTypes();

            foreach (Type typ in _types)
            {
                if (!typ.IsClass) continue;

                Type[] _interfaces = typ.GetInterfaces();
                if (_interfaces.Length == 0) continue;

                for (int i = 0; i < _interfaces.Length; i++)
                    if (_interfaces[i] == typToFind)
                    {
                        _foundTypes.Add(typ);
                    }
            }

            return _foundTypes;
        }
    }
}
