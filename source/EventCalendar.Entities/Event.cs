using System;
using System.Collections.Generic;

namespace EventCalendar.Entities
{
    public class Event : IComparable<Event>
    {
        public string Title { get; private set; }
        public Person Invitor { get; set; }
        public DateTime DateTimeEvent { get; set; }
        public List<Person> Persons { get; set; }

        public Event(string title, DateTime dateTime, Person invitor)
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));
            if (dateTime == null)
                throw new ArgumentNullException(nameof(dateTime));
            if (invitor == null)
                throw new ArgumentNullException(nameof(invitor));

            Title = title;
            DateTimeEvent = dateTime;
            Invitor = invitor;
            Persons = new List<Person>();
        }

        public int CompareTo(Event other)
        {
            return this.DateTimeEvent.CompareTo(other.DateTimeEvent);
        }
    }
}
