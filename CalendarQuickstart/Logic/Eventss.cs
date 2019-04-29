using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarQuickstart.Logic
{
	//test
    public class Eventss
    {
        //don't use primary calendar in deplotment

        //what is the retun value when you delete an event
        public static CalendarService service;

        public Eventss()
        {
            service = GService.service;
        }

        public static Boolean testDate(DateTime? start, DateTime? end) {

            if (start < end)
            {
                return true;
            }
            //rabbitmq error
            else return false; 
        }
        public static Boolean testOverlapLocation(DateTime? start, DateTime? end, string location)
        {
            var events = getAllnotDeletedEvents();
            foreach (Event ev in events.Items) {

                if (ev.Location == location) {

                    if (Math.Max(end.Value.Ticks, ev.End.DateTime.Value.Ticks) - Math.Min(start.Value.Ticks, ev.Start.DateTime.Value.Ticks) < (end.Value.Ticks - start.Value.Ticks) + (ev.End.DateTime.Value.Ticks - ev.Start.DateTime.Value.Ticks)) {

                        return false;

                    }
                }
                    
                    }
            return true;
        }
        public static Boolean testOverlapAttendees(DateTime? start, DateTime? end, IList<EventAttendee> eventAttendees)
        {
            var events = getAllnotDeletedEvents();
            foreach (var ev in events.Items)
            {
                foreach(var attendee in eventAttendees)
                {
					if (ev.Attendees.Contains(attendee)){

						if (Math.Max(end.Value.Ticks, ev.End.DateTime.Value.Ticks) - Math.Min(start.Value.Ticks, ev.Start.DateTime.Value.Ticks) < (end.Value.Ticks - start.Value.Ticks) + (ev.End.DateTime.Value.Ticks - ev.Start.DateTime.Value.Ticks)) {

							return false;
						}
					}

                }

            }
            return true;
        }

        public static Events getAllEvents(string calendarId = "primary")
        {
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = true;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            return events;
        }
        public static Events getAllnotDeletedEvents(string calendarId = "primary")
        {
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            return events;       
        }
        public static Events getAllDeletedEvents(string calendarId = "primary")
        {
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = true;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();

            foreach (Event ev in events.Items) {
                //tests if the event has not been deleted
                if (ev.Status != "cancelled") {
                    events.Items.Remove(ev);
                }

            }
            return events;
        }

        //boolean? event excist: event is deleted / if return of methode is null event doesn't excist
        public static Dictionary<Event, Boolean> getEventById(string eventId, string calendarId = "primary")
        {
            Events events = getAllEvents();
            foreach (Event ev in events.Items)
            {
                if (ev.Status != "canceled")
                {
                    if (ev.Id == eventId)
                    {
                        Dictionary<Event, Boolean> returnvalue = null;
                        returnvalue.Add(ev, true);

                        return returnvalue;
                    }
                }
                else
                {
                    if (ev.Id == eventId)
                    {
                        Dictionary<Event, Boolean> returnvalue = null;
                        returnvalue.Add(ev, false);

                        return returnvalue;
                    }
                }
            }
            return null;
        }
        public static Dictionary<Event, Boolean> getEventByName(string name, string calendarId = "primary")
        {
            Events events = getAllEvents();
            foreach (Event ev in events.Items)
            {
                if (ev.Status != "canceled")
                {
                    if (ev.Summary == name)
                    {
                        Dictionary<Event, Boolean> returnvalue = null;
                        returnvalue.Add(ev, true);

                        return returnvalue;
                    }
                }
                else {
                    if (ev.Summary == name)
                    {
                        Dictionary<Event, Boolean> returnvalue = null;
                        returnvalue.Add(ev, false);

                        return returnvalue;
                    }
                }         
            }

            return null;
        }
        public static Dictionary<Event, Boolean> getEvent(Event eventToFind, string calendarId = "primary")
        {
            Events events = getAllEvents();
            foreach (Event ev in events.Items)
            {
                if (ev.Status != "canceled")
                {
                    if (ev.Id == eventToFind.Id)
                    {
                        Dictionary<Event, Boolean> returnvalue = null;
                        returnvalue.Add(ev, true);

                        return returnvalue;
                    }
                }
                else
                {
                    if (ev.Id == eventToFind.Id)
                    {
                        Dictionary<Event, Boolean> returnvalue = null;
                        returnvalue.Add(ev, false);

                        return returnvalue;
                    }
                }
            }

            return null;
        }

        public static Event newEvent(string summary, string location, string description, DateTime startDate, DateTime endDate, List<EventAttendee> eventAttendees, String TimeZone = "America/Los_Angeles", string calendarId = "primary")
        {
            if(!testDate(startDate, endDate) && !testOverlapLocation(startDate, endDate,location) && !testOverlapAttendees(startDate, endDate,eventAttendees)){
                return null;
            }
            
            Event myEvent = new Event
            {
                Summary = summary,
                Location = location,
                Description = description,
                Start = new EventDateTime()
                {
                    DateTime = startDate,
                    TimeZone = TimeZone
                },
                End = new EventDateTime()
                {
                    DateTime = endDate,
                    TimeZone = TimeZone
                },

                Attendees = eventAttendees,

                Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new Google.Apis.Calendar.v3.Data.EventReminder[]
                                {
                                                        new Google.Apis.Calendar.v3.Data.EventReminder() { Method = "email", Minutes = 30 },
                                                        new Google.Apis.Calendar.v3.Data.EventReminder() { Method = "popup", Minutes = 10 }
                                }
                }
            };
            

            return service.Events.Insert(myEvent, calendarId).Execute();
        }

        public static Event addAttendeesByName(List<EventAttendee> eventAttendees, string name, string calendarId = "primary")
        {
            Dictionary<Event, Boolean> events = getEventByName(name);
            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                    foreach (EventAttendee att in ev.Attendees)
                    {
                        eventAttendees.Add(att);
                    }
                    ev.Attendees = eventAttendees;
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                Console.Write("Event is marked as inactive, activate event to add attendees");
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
            }

            return service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id).Execute();
        }
        public static Event addAttendeesById(List<EventAttendee> eventAttendees, string eventId, string calendarId = "primary")
        {
            Dictionary<Event, Boolean> events = getEventById(eventId);
            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                    foreach (EventAttendee att in ev.Attendees)
                    {
                        eventAttendees.Add(att);
                    }
                    ev.Attendees = eventAttendees;
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                Console.Write("Event is marked as inactive, activate event to add attendees");
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
            }

            return service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id).Execute();   
        }
        public static Event addAttendees(List<EventAttendee> eventAttendees, Event eventToFind, string calendarId = "primary")
        {
            Dictionary<Event, Boolean> events = getEvent(eventToFind);
            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                    foreach (EventAttendee att in ev.Attendees)
                    {
                        eventAttendees.Add(att);
                    }
                    ev.Attendees = eventAttendees;
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                Console.Write("Event is marked as inactive, activate event to add attendees");
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
            }

            return service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id).Execute();
        }

        //these methodes will overrride predeclared values, example: eventAttendees will overide the old attendees not add them to the excisting ones
        //do not change calendarId, you can't update it
        //RESEARCH NEEDED to update calendar id
        public static Event updateEventByName(string oldSummery, string newSummary, string location, string description, DateTime startDate, DateTime endDate, List<EventAttendee> eventAttendees, String TimeZone = "America/Los_Angeles", string calendarId = "primary")
        {
            if (!testDate(startDate, endDate) && !testOverlapLocation(startDate, endDate, location) && !testOverlapAttendees(startDate, endDate, eventAttendees))
            {
                return null;
            }
            Dictionary<Event, Boolean> events = getEventByName(oldSummery);

            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                    ev.Summary = newSummary;
                    ev.Location = location;
                    ev.Description = description;
                    ev.Start = new EventDateTime()
                    {
                        DateTime = startDate,
                        TimeZone = TimeZone
                    };
                    ev.End = new EventDateTime()
                    {
                        DateTime = endDate,
                        TimeZone = TimeZone
                    };
                    ev.Attendees = eventAttendees;
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                Console.Write("Event is marked as inactive, activate event before updating it");
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
            }

            return service.Events.Update(eventToUpdate, "primary", eventToUpdate.Id).Execute();
        }
        public static Event updateEventById(string eventId, string summary, string location, string description, DateTime startDate, DateTime endDate, List<EventAttendee> eventAttendees, String TimeZone = "America/Los_Angeles", string calendarId = "primary")
        {
            if (!testDate(startDate, endDate) && !testOverlapLocation(startDate, endDate, location) && !testOverlapAttendees(startDate, endDate, eventAttendees))
            {
                return null;
            }
            Dictionary<Event, Boolean> events = getEventById(eventId);

            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                    ev.Summary = summary;
                    ev.Location = location;
                    ev.Description = description;
                    ev.Start = new EventDateTime()
                    {
                        DateTime = startDate,
                        TimeZone = TimeZone
                    };
                    ev.End = new EventDateTime()
                    {
                        DateTime = endDate,
                        TimeZone = TimeZone
                    };
                    ev.Attendees = eventAttendees;
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                Console.Write("Event is marked as inactive, activate event before updating it");
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
            }

            return service.Events.Update(eventToUpdate, "primary", eventToUpdate.Id).Execute();
        }
        public static Event updateEvent(Event eventUpdate, string summary, string location, string description, DateTime startDate, DateTime endDate, List<EventAttendee> eventAttendees, String TimeZone = "America/Los_Angeles", string calendarId = "primary")
        {
            if (!testDate(startDate, endDate) && !testOverlapLocation(startDate, endDate, location) && !testOverlapAttendees(startDate, endDate, eventAttendees))
            {
                return null;
            }
            Dictionary<Event, Boolean> events = getEvent(eventUpdate);

            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                    ev.Summary = summary;
                    ev.Location = location;
                    ev.Description = description;
                    ev.Start = new EventDateTime()
                    {
                        DateTime = startDate,
                        TimeZone = TimeZone
                    };
                    ev.End = new EventDateTime()
                    {
                        DateTime = endDate,
                        TimeZone = TimeZone
                    };
                    ev.Attendees = eventAttendees;
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                Console.Write("Event is marked as inactive, activate event before updating it");
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
            }

            return service.Events.Update(eventToUpdate, "primary", eventToUpdate.Id).Execute();
        }

        //you can recover "deleted" events
        //RESARCH NEEDED to know how long they will be able to be recovered
        //test string!!
        public static string DeleteEvent(Event eventToDelete, string calendarId = "primary")
        {
            return service.Events.Delete(calendarId, eventToDelete.Id).Execute();
        }
        //return values are incorrect
        public static string DeleteEventByName(string name, string calendarId = "primary")
        {
            var ev = getEventByName(name);
            if (ev.ContainsValue(true)) {
                foreach(Event eve in ev.Keys)
                return service.Events.Delete(calendarId, eve.Id).Execute();
            }
            if (ev.ContainsValue(false))
            {
                //wrong return value
                return "already deleted";
            }
            //doesn't excist
            else return null;
            
        }
        public static string DeleteEventById(string id, string calendarId = "primary")
        {
            return service.Events.Delete(calendarId, id).Execute();
        }

        public static Event UnDeleteEvent(Event eventToArchive, string calendarId = "primary")
        {
           
            Dictionary<Event, Boolean> events = getEvent(eventToArchive);

            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {
                 
                    ev.Status = "confirmed";
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                foreach (Event ev in events.Keys)
                {
                    if (!testDate(ev.Start.DateTime, ev.End.DateTime) && !testOverlapLocation(ev.Start.DateTime, ev.End.DateTime, ev.Location) && !testOverlapAttendees(ev.Start.DateTime, ev.End.DateTime, ev.Attendees))
                    {
                        return null;
                    }
                    ev.Status = "confirmed";
                    eventToUpdate = ev;
                }
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
                return null;
            }

            return service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id).Execute();
        }
        public static Event UnDeleteEventByName(string name, string calendarId = "primary")
        {
            
            Dictionary<Event, Boolean> events = getEventByName(name);

            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)
                {                 
                    ev.Status = "confirmed";
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                foreach (Event ev in events.Keys)
                {
                    if (!testDate(ev.Start.DateTime, ev.End.DateTime) && !testOverlapLocation(ev.Start.DateTime, ev.End.DateTime, ev.Location) && !testOverlapAttendees(ev.Start.DateTime, ev.End.DateTime, ev.Attendees))
                    {
                        return null;
                    }
                    ev.Status = "confirmed";
                    eventToUpdate = ev;
                }
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
                return null;
            }

            return service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id).Execute();
        }
        public static Event UnDeleteEventById(string eventId, string calendarId = "primary")
        {
           
            Dictionary<Event, Boolean> events = getEventById(eventId);

            Event eventToUpdate = null;
            if (events.ContainsValue(true))
            {
                foreach (Event ev in events.Keys)

                {                 
                    ev.Status = "confirmed";
                    eventToUpdate = ev;
                }
            }
            if (events.ContainsValue(false))
            {
                foreach (Event ev in events.Keys)
                {
                    if (!testDate(ev.Start.DateTime, ev.End.DateTime) && !testOverlapLocation(ev.Start.DateTime, ev.End.DateTime, ev.Location) && !testOverlapAttendees(ev.Start.DateTime, ev.End.DateTime, ev.Attendees))
                    {
                        return null;
                    }
                    ev.Status = "confirmed";
                    eventToUpdate = ev;
                }
            }
            if (events == null)
            {
                Console.Write("Event doesn't excist");
                return null;
            }

            return service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id).Execute();
        }

	
    }
}
