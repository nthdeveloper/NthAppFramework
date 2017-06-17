using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace NthDeveloper.AppFramework.Events
{
    public class DefaultEventHub : IEventHub
    {        
        SynchronizationContext m_SynchronizationContext;
        
        public SynchronizationContext SynchronizationContext
        {
            get
            {
                if (m_SynchronizationContext == null)
                {
                    m_SynchronizationContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
                }
                return m_SynchronizationContext;
            }
            set { m_SynchronizationContext = value; }
        }

        private readonly IDictionary<SubscriberInfo, WaitRegistration> m_WaitingHandles = new Dictionary<SubscriberInfo, WaitRegistration>();
        private readonly Dictionary<Type, List<SubscriberInfo>> handlers = new Dictionary<Type, List<SubscriberInfo>>();
        private readonly object locker = new object();

        public DefaultEventHub()
        {
        }

        public void Subscribe<T>(Action<T> handler)
        {
            Subscribe(handler, InvokeOption.ThreadPool, InvokePriority.Normal, 0);
        }

        public void Subscribe<T>(Action<T> handler, InvokeOption invokeOption)
        {
            Subscribe(handler, invokeOption, InvokePriority.Normal, 0);
        }

        public void Subscribe<T>(Action<T> handler, InvokeOption invokeOption, InvokePriority priority)
        {

            Subscribe(handler, invokeOption, priority, 0);
        }

        public void Subscribe<T>(Action<T> handler, InvokeOption invokeOption, int delay)
        {
            Subscribe(handler, invokeOption, InvokePriority.Low, delay);
        }

        public void Subscribe<T>(Action<T> handler, InvokeOption invokeOption, InvokePriority priority, int delay)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers[typeof(T)] = new List<SubscriberInfo>();
            }

            handlers[typeof(T)].Add(new SubscriberInfo
            {
                Priority = priority,
                Handler = new WeakAction(handler),
                Option = invokeOption,
                Delay = delay,
            });
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handlers.ContainsKey(typeof(T)))
            {
                List<SubscriberInfo> subscriberInfos = handlers[typeof(T)];
                subscriberInfos.RemoveAll(x => x.Handler.Equals(handler));
            }
        }

        public int GetSubscriberCount(Type type)
        {
            if (handlers.ContainsKey(type))
            {
                List<SubscriberInfo> subscriberInfos = handlers[type];
                return subscriberInfos.Count;
            }
            return -1;
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            //Debug.WriteLine("PUBLISHING: " + type.Name);
            if (handlers.ContainsKey(type))
            {
                var subscribers = handlers[type].OrderBy(x => x.Priority).ToList();

                for (int i = subscribers.Count-1; i >= 0; i--)
                {
                    var subscriber = subscribers[i];

                    //eðer subscriber beklemeli ise, bekletelim
                    if (subscriber.Delay > 0 && !subscriber.IsDelayCompleted)
                    {
                        UnregisterDelay(subscriber);
                        RegisterDelay(subscriber, eventData);
                        continue;
                    }

                    if (InvokeSubscriberHandler(subscriber, eventData) == false)
                        subscribers.RemoveAt(i);
                }
            }
        }

        public void Publish<T>(T eventData, InvokeOption invokeOption)
        {
            var type = typeof(T);
            if (handlers.ContainsKey(type))
            {
                var subscribers = handlers[type].OrderBy(x => x.Priority).ToList();

                for (int i = subscribers.Count - 1; i >= 0; i--)
                {
                    var subscriber = subscribers[i];

                    if (subscriber.Delay > 0 && !subscriber.IsDelayCompleted)
                    {
                        UnregisterDelay(subscriber);
                        RegisterDelay(subscriber, eventData);
                        continue;
                    }

                    if (InvokeSubscriberHandler(subscriber, eventData, invokeOption) == false)
                        subscribers.RemoveAt(i);
                }
            }
        }

        public void Publish<T>()
        {
            Publish(default(T));
        }

        public void Reset()
        {
            handlers.Clear();
        }

        private bool InvokeSubscriberHandler<T>(SubscriberInfo subscriber, T eventData)
        {
            if (subscriber.Delay > 0)
            {
                UnregisterDelay(subscriber);
            }

            var handler = subscriber.Handler.CreateAction<T>();
            if (handler == null)
                return false;

            switch (subscriber.Option)
            {
                case InvokeOption.UIThread:
                    SynchronizationContext.Post(state => handler(eventData), eventData);
                    break;
                case InvokeOption.ThreadPool:
                    ThreadPool.QueueUserWorkItem(obj => handler(eventData));
                    break;
                case InvokeOption.Blocking:
                    handler(eventData);
                    break;
            }
            return true;
        }

        private bool InvokeSubscriberHandler<T>(SubscriberInfo subscriber, T eventData, InvokeOption invokeOption)
        {
            if (subscriber.Delay > 0)
            {
                UnregisterDelay(subscriber);
            }

            var handler = subscriber.Handler.CreateAction<T>();
            if (handler == null)
                return false;

            switch (invokeOption)
            {
                case InvokeOption.UIThread:
                    SynchronizationContext.Post(state => handler(eventData), eventData);
                    break;
                case InvokeOption.ThreadPool:
                    ThreadPool.QueueUserWorkItem(obj => handler(eventData));
                    break;
                case InvokeOption.Blocking:
                    handler(eventData);
                    break;
            }
            return true;
        }

        private void UnregisterDelay(SubscriberInfo subscriber)
        {
            lock (locker)
            {
                WaitRegistration waitRegistration;
                if (m_WaitingHandles.TryGetValue(subscriber, out waitRegistration))
                {
                    waitRegistration.RegisteredWaitHandle.Unregister(waitRegistration.Handle);
                    m_WaitingHandles.Remove(subscriber);
                }
                subscriber.IsDelayCompleted = false;
            }
        }
        private void RegisterDelay<T>(SubscriberInfo subscriber, T eventData)
        {
            if (subscriber == null)
                throw new ArgumentNullException("subscriber");

            lock (locker)
            {
                var handle = new ManualResetEvent(false);
                var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(handle, (state, timeOut) =>
                {
                    subscriber.IsDelayCompleted = true;
                    InvokeSubscriberHandler(subscriber, (T)state);
                }, eventData, subscriber.Delay, true);

                m_WaitingHandles.Add(subscriber, new WaitRegistration
                {
                    Handle = handle,
                    RegisteredWaitHandle = registeredWaitHandle
                });
            }
        }

        private class WaitRegistration
        {
            public WaitHandle Handle { get; set; }
            public RegisteredWaitHandle RegisteredWaitHandle { get; set; }
        }

        private class SubscriberInfo
        {
            public WeakAction Handler { get; set;}
            public InvokeOption Option { get; set;}
            public InvokePriority Priority { get; set; }
            public int Delay { get; set; }
            public bool IsDelayCompleted { get; set; }

            public override string ToString()
            {
                return string.Format("Option: {0}, Priority: {1}", Option, Priority);
            }

            private bool Equals(SubscriberInfo other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Equals(other.Handler, Handler) && Equals(other.Option, Option) && Equals(other.Priority, Priority) && other.Delay == Delay;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof (SubscriberInfo))
                    return false;
                return Equals((SubscriberInfo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (Handler != null ? Handler.GetHashCode() : 0);
                    result = (result*397) ^ Option.GetHashCode();
                    result = (result*397) ^ Priority.GetHashCode();
                    result = (result*397) ^ Delay;
                    return result;
                }
            }
        }

        private class WeakAction : WeakReference, IEquatable<Delegate>
        {

            private readonly MethodInfo m_MethodInfo;
            private readonly Delegate m_handler;
            public WeakAction(Delegate handler) : base(handler.Target)
            {
                m_handler = handler;
                m_MethodInfo = handler.Method;
            }

            public override bool IsAlive
            {
                get
                {
                    return m_MethodInfo.IsStatic || base.IsAlive;
                }
            }

            public Action<T> CreateAction<T>()
            {
                if (!IsAlive)
                    return null;

                try
                {
                    if (m_MethodInfo.IsStatic)
                        return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), m_MethodInfo);
                    else
                        return (Action<T>)Delegate.CreateDelegate(typeof (Action<T>), base.Target, m_MethodInfo.Name);
                }
                catch
                {
                    return null;
                }
            }

            public bool Equals(Delegate other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                return Equals(other, m_handler);
            }

            public override int GetHashCode()
            {
                return (m_handler != null ? m_handler.GetHashCode() : 0);
            }
        }
    }    
}