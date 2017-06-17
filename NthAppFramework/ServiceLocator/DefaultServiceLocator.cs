using System;

namespace NthDeveloper.AppFramework.ServiceLocator
{
    public class DefaultServiceLocator : IServiceLocator
    {
        #region IServiceLocator Members

        public void RegisterService<T>(object serv)
        {
            ServiceLocator.RegisterService<T>(serv);
        }

        public T GetService<T>()
        {
            return ServiceLocator.GetService<T>();
        }

        #endregion
    }
}
