using System;

namespace NthDeveloper.AppFramework.ServiceLocator
{
    public class Provider<T>
    {
        T m_Value;

        Func<T> m_ProviderFunc;

        public T Value
        {
            get
            {
                if (m_Value != null)
                    return m_Value;

                if (m_ProviderFunc == null)
                    m_Value = ServiceLocator.GetService<T>();
                else
                    m_Value = m_ProviderFunc();

                return m_Value;
            }
        }

        public Provider()
        {
        }

        public Provider(T val)
        {
            m_Value = val;
        }

        public Provider(Func<T> providerFunc)
        {
            m_ProviderFunc = providerFunc;
        }
    }
}
