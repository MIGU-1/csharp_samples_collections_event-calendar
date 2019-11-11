using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using EventCalendar.Entities;
using static System.String;

namespace EventCalendar.Logic
{
    public class Controller
    {
        private readonly ICollection<Event> _events;
        public int EventsCount => _events.Count;

        public Controller()
        {
            _events = new List<Event>();
        }


        /// <summary>
        /// Ein Event mit dem angegebenen Titel und dem Termin wird für den Einlader angelegt.
        /// Der Titel muss innerhalb der Veranstaltungen eindeutig sein und das Datum darf nicht
        /// in der Vergangenheit liegen.
        /// Mit dem optionalen Parameter maxParticipators kann eine Obergrenze für die Teilnehmer festgelegt
        /// werden.
        /// </summary>
        /// <param name="invitor"></param>
        /// <param name="title"></param>
        /// <param name="dateTime"></param>
        /// <param name="maxParticipators"></param>
        /// <returns>Wurde die Veranstaltung angelegt</returns>
        public bool CreateEvent(Person invitor, string title, DateTime dateTime, int maxParticipators = 0)
        {
            bool eventCreated = false;

            if (CheckParameters(invitor, title, dateTime))
            {
                Event newEvent;

                if (maxParticipators > 0)
                {
                    newEvent = new EventLimited(title, dateTime, invitor, maxParticipators);
                }
                else
                {
                    newEvent = new Event(title, dateTime, invitor);
                }

                _events.Add(newEvent);
                eventCreated = true;
            }

            return eventCreated;
        }

        /// <summary>
        /// Liefert die Veranstaltung mit dem Titel
        /// </summary>
        /// <param name="title"></param>
        /// <returns>Event oder null, falls es keine Veranstaltung mit dem Titel gibt</returns>
        public Event GetEvent(string title)
        {
            Event retEvent = null;

            foreach (Event myEvent in _events)
            {
                if (myEvent.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase))
                    retEvent = myEvent;
            }

            return retEvent;
        }

        /// <summary>
        /// Person registriert sich für Veranstaltung.
        /// Eine Person kann sich zu einer Veranstaltung nur einmal registrieren.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="ev">Veranstaltung</param>
        /// <returns>War die Registrierung erfolgreich?</returns>
        public bool RegisterPersonForEvent(Person person, Event ev)
        {
            bool registerOk = true;

            if (ev != null && person != null)
            {
                foreach (Event myEvent in _events)
                {
                    foreach (Person myPerson in myEvent.Persons)
                    {
                        if (myPerson.Equals(person) && ev.DateTimeEvent == myEvent.DateTimeEvent)
                        {
                            registerOk = false;
                        }
                    }
                }

                if (registerOk && !ev.Persons.Contains<Person>(person))
                {
                    if (ev.GetType().Equals(typeof(EventLimited)))
                    {
                        EventLimited tmp = ev as EventLimited;
                        if (tmp.Persons.Count < tmp.MaxParticipators)
                        {
                            person.Events.Add(ev);
                            ev.Persons.Add(person);
                        }
                        else
                        {
                            registerOk = false;
                        }
                    }
                    else
                    {
                        person.Events.Add(ev);
                        ev.Persons.Add(person);
                    }

                }
                else
                {
                    registerOk = false;
                }
            }
            else
            {
                registerOk = false;
            }

            return registerOk;
        }

        /// <summary>
        /// Person meldet sich von Veranstaltung ab
        /// </summary>
        /// <param name="person"></param>
        /// <param name="ev">Veranstaltung</param>
        /// <returns>War die Abmeldung erfolgreich?</returns>
        public bool UnregisterPersonForEvent(Person person, Event ev)
        {
            bool unregisterOk = false;

            if (person != null && ev != null)
            {
                unregisterOk = ev.Persons.Contains<Person>(person);
                if (unregisterOk)
                {
                    ev.Persons.Remove(person);
                    person.Events.Remove(ev);
                }
            }

            return unregisterOk;
        }

        /// <summary>
        /// Liefert alle Teilnehmer an der Veranstaltung.
        /// Sortierung absteigend nach der Anzahl der Events der Personen.
        /// Bei gleicher Anzahl nach dem Namen der Person (aufsteigend).
        /// </summary>
        /// <param name="ev"></param>
        /// <returns>Liste der Teilnehmer oder null im Fehlerfall</returns>
        public IList<Person> GetParticipatorsForEvent(Event ev)
        {
            if (ev != null)
                ev.Persons.Sort();

            return ev != null ? ev.Persons : null;
        }

        /// <summary>
        /// Liefert alle Veranstaltungen der Person nach Datum (aufsteigend) sortiert.
        /// </summary>
        /// <param name="person"></param>
        /// <returns>Liste der Veranstaltungen oder null im Fehlerfall</returns>
        public List<Event> GetEventsForPerson(Person person)
        {
            if (person != null)
                person.Events.Sort();

            return person != null ? person.Events : null;
        }

        /// <summary>
        /// Liefert die Anzahl der Veranstaltungen, für die die Person registriert ist.
        /// </summary>
        /// <param name="participator"></param>
        /// <returns>Anzahl oder 0 im Fehlerfall</returns>
        public int CountEventsForPerson(Person participator)
        {
            return participator != null ? participator.Events.Count : 0;
        }
        private bool CheckParameters(Person invitor, string title, DateTime dateTime)
        {
            bool titleUnique = true;

            foreach (Event myEvent in _events)
            {
                if (myEvent.Title.Equals(title))
                    titleUnique = false;
            }

            return invitor != null && title != null && title != "" && dateTime.CompareTo(DateTime.Now) > 0 && titleUnique;
        }

    }
}
