using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NthDeveloper.AppFramework.ServiceLocator
{
    public interface IServiceLocator
    {
        void RegisterService<T>(object serv);
        T GetService<T>();
    }
}
