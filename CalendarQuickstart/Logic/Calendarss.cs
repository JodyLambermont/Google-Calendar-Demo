using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarQuickstart.Logic
{
    public class Calendarss
    {
        // can a calendar be undeleted? 

        static CalendarService service;

        public Calendarss()
        {
            service = GService.service;
        }

        public static IList<CalendarListEntry> getCalenders()
        {
            var calenders = service.CalendarList.List().Execute().Items;

            return calenders;
        }

        public static Calendar getCalender(Calendar calenderToGet)
        {        
            Calendar calander = service.Calendars.Get(calenderToGet.Id).Execute();

            return calander;
        }
        //primary is the main calendar of this project
        public static Calendar getCalenderById(String id = "primary")
        {
            Calendar calander = service.Calendars.Get(id).Execute();

            return calander;
        }
        //this is not with type Calander, make sure to use var or CalendarListEntry when using
        public static CalendarListEntry getCalenderByName(String name)
        {     
            var calenders = getCalenders();

            foreach (CalendarListEntry cal in calenders)
            {
                if (cal.Summary == name)
                {
                    return cal;
                }
            }

            //calendar doesn't excist
            return null;
        }

        //you can only clear the primary calendar
        //BE AWARE this is a very desctructive method, all events will be deleted
        //test string
        //returns id if failed
        public static string clearCalendar() {

            return service.Calendars.Clear("primary").Execute();
        }

        //creates secondary calendar
        public static Calendar newCalendar(string name, string location, string despcription, string secundaryCalendarId, string timeZone = "America/Los_Angeles")
        {
            Calendar newCalendar = new Calendar
            {
                Summary = name,
                Location = location,
                TimeZone = timeZone,
                Description = despcription,
                Id = secundaryCalendarId
            };

            return service.Calendars.Insert(newCalendar).Execute();
        }

        //Cannot change ID!
        public static Calendar updateCalendarById(string newName, string id = "primary", string newLocation = "unknown", string newDescription = "none", string newTimeZone = "America/Los_Angeles")
        {
            Calendar calendar = service.Calendars.Get(id).Execute();

            // Make a change
            calendar.Summary = newName;
            calendar.Location = newLocation;
            calendar.TimeZone = newTimeZone;
            calendar.Description = newDescription;

            // Update the altered calendar
            return service.Calendars.Update(calendar, calendar.Id).Execute();
        }
        public static Calendar updateCalendar(Calendar calendar, string newName, string newLocation = "unknown", string newDescription = "none", string newTimeZone = "America/Los_Angeles")
        {
            // Make a change
            calendar.Summary = newName;
            calendar.Location = newLocation;
            calendar.TimeZone = newTimeZone;
            calendar.Description = newDescription;

            // Update the altered calendar
            return service.Calendars.Update(calendar, calendar.Id).Execute();
        }
        //not yet tested!!
        public static Calendar updateCalendarByName(string oldName, string newName, string newLocation = "unknown", string newDescription = "none", string newTimeZone = "America/Los_Angeles")
        {
            var calenders = getCalenders();
            CalendarListEntry foundCalendar = null;

            foreach (CalendarListEntry cal in calenders)
            {
                if (cal.Summary == oldName)
                {
                    foundCalendar = cal;
                }
            }

            // Make a change
            //because foundCalendar is a CalendarListEntry we have to convert it to a Calendar
            Calendar calendar = null;
            calendar.Summary = newName;
            calendar.Location = newLocation;
            calendar.Description = newDescription;
            calendar.TimeZone = newTimeZone;
            calendar.Id = foundCalendar.Id;

            // Update the altered calendar
            return service.Calendars.Update(calendar, foundCalendar.Id).Execute();

        }

        //whole block can only delete a secondary calendar
        //returns id if failed
        public static string deleteCalendar(Calendar calendar)
        {
            return service.Calendars.Delete(calendar.Id).Execute();
        }
        public static string deleteCalendarById(string id)
        {
            return service.Calendars.Delete(id).Execute();
        }
        public static string deleteCalendarByName(string name)
        {
            var calenders = getCalenders();
            CalendarListEntry foundCalander = null;

            foreach (CalendarListEntry cal in calenders)
            {
                if (cal.Summary == name)
                {
                    foundCalander = cal;
                }
            }
            return service.Calendars.Delete(foundCalander.Id).Execute();
        }
    }
}
