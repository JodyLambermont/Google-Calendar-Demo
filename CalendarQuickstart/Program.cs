using CalendarQuickstart.Logic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarQuickstart
{
	class Program
	{


		 static void Main(string[] args) {

			//setup for the service
			GService testGService = new GService();
			Eventss testevent = new Eventss();
			Calendarss testcalender = new Calendarss();
			Attendee at = new Attendee();

			at.createAttendees("jeff", "josh", "123");

			/**
			//creat calendar
			Calendarss.newCalendar("demo 1", "somwhere");

			Console.Read();

			//show all calendars
			var calendars = Calendarss.getCalenders();
			foreach (CalendarListEntry cal in calendars)
			{
				Console.WriteLine(cal.Summary);
			}

			Console.Read();

			//update calendar
			foreach (CalendarListEntry cal in calendars)
			{
				if (cal.Summary == "demo 1")
				{			
					Calendarss.updateCalendarById(cal.Id, "demotest 2");
				}
			}

			Console.Read();

			DateTime startDate = new DateTime(2019, 5, 6, 6, 30, 00);
			DateTime endDate = new DateTime(2019, 5, 6, 7, 30, 00);
			//new event
			Eventss.newEvent("demoTestEvent", "somewhere", "none", startDate, endDate);

			Console.Read();

			//getting all not deleted events in calendar + display
			Events events = Eventss.getAllnotDeletedEvents();

			Console.Read();

			//searching and updating previous event
			foreach (var ev in events.Items)
			{
				if (ev.Summary == "demoTestEvent")
				{
					Eventss.updateEvent(ev);
				}

			}

			Console.Read();

			//searching previous made event and deleting
			foreach (var ev in events.Items)
			{
				if (ev.Summary == "demo new event name")
				{
					Eventss.DeleteEvent(ev);
				}

			}

			Console.Read();

			//delete calendar
			Calendarss.deleteCalendarByName("demo 1");

			Console.Read();*/





		}
			

	}

	}


	
