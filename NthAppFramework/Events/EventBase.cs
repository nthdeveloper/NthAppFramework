using System;

namespace NthDeveloper.AppFramework.Events
{
    public class EventBase
    {
        public DateTime EventTime { get; protected set; }

        public EventBase()
        {
            this.EventTime = DateTime.Now;
        }
    } 
}
