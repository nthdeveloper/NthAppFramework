using System;
using System.Collections.Generic;

namespace NthDeveloper.AppFramework.ServiceLocator
{
    static class ServiceLocator
    {
        static Dictionary<Type, object> m_Services;

        static ServiceLocator()
        {
            m_Services = new Dictionary<Type, object>(12);
        }

        public static void RegisterService<T>(object serv)
        {
            m_Services.Add(typeof(T), serv);
        }

        public static T GetService<T>()
        {
            if(m_Services.ContainsKey(typeof(T)))
                return (T)m_Services[typeof(T)];

            return default(T);
        }
    }
}
