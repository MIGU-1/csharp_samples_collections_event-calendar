using System;
using EventCalendar.Entities;

namespace EventCalendar.Logic
{
    public class EventLimited : Event
    {
        public int MaxParticipators { get; set; }

        public EventLimited(string title, DateTime dateTime, Person invitor, int maxParticipators)
            : base(title, dateTime, invitor)
        {
            MaxParticipators = maxParticipators;
        }
    }
}