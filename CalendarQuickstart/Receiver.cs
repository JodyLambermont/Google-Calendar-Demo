using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Xml;
using CalendarQuickstart.Logic;

namespace CalendarQuickstart
{
    class ReceiveMessage
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "10.3.56.27", UserName = "", Password = "", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                channel.QueueBind(queue: "Planning", exchange: "logs", routingKey: "");
                Console.WriteLine("[*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] {0}", message);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(message);
                    XmlNodeList xmlList = doc.GetElementsByTagName("Message"); //Represents an ordered collection of nodes.
                    string messageType = xmlList[0].InnerText.ToLower(); // !!! the message is taken in ToLower

                    switch (messageType)
                    {
                        case "createevent": // create calendar - still to implement
                            new ReceiveMessage().createCalendar(doc);
                            break;

                        case "updateevent": // update calendar - still to implement
                            new ReceiveMessage().updateCalendar(doc);
                            break;

                        case "deleteevent": // delete calendar - still to implement
                            new ReceiveMessage().deleteCalendar(doc);
                            break;

                        case "createsession": // create event
                            new ReceiveMessage().createEvent(doc);
                            break;

                        case "updatesession": // update event
                            new ReceiveMessage().updateEvent(doc);
                            break;

                        case "deletesession": // delete event
                            new ReceiveMessage().deleteEvent(doc);
                            break;

                        default:
                            Console.WriteLine("This option is invalid");
                            break;
                    }
                };
                channel.BasicConsume(queue: "Planning", autoAck: true, consumer: consumer);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey(true);

            }
        }

        public void createCalendar(XmlDocument doc)
        {
            XmlNodeList titel = doc.GetElementsByTagName("name");
            XmlNodeList uuid = doc.GetElementsByTagName("id");

            Console.WriteLine(titel[0].InnerText);
            Console.WriteLine(uuid[0].InnerText);

            Calendarss calendarss = new Calendarss(); // connectie maken - service
            Calendarss.newCalendar(titel[0].InnerText, uuid[0].InnerText);
        }

        public void updateCalendar(XmlDocument doc)
        {
            XmlNodeList titel = doc.GetElementsByTagName("name");
            //XmlNodeList old_titel = doc.GetElementsByTagName("old_titel"); // not really needed
            XmlNodeList uuid = doc.GetElementsByTagName("id");
            if (uuid == null)
            {
                if (uuid[0].InnerText == String.Empty)
                {
                    uuid[0].InnerText = "primary";
                }
            }

            Console.WriteLine(titel[0].InnerText);
            //Console.WriteLine(old_titel[0].InnerText); // not really needed
            Console.WriteLine(uuid[0].InnerText);

            Calendarss calendarss = new Calendarss(); // connectie maken - service
            Calendarss.updateCalendarById(titel[0].InnerText, uuid[0].InnerText);
        }

        public void deleteCalendar(XmlDocument doc)
        {
            XmlNodeList uuid = doc.GetElementsByTagName("id");

            Console.WriteLine(uuid[0].InnerText);

            Calendarss calendarss = new Calendarss(); // connectie maken - service
            Calendarss.deleteCalendarById(uuid[0].InnerText);
        }


        public void createEvent(XmlDocument doc)
        {
            XmlNodeList titel = doc.GetElementsByTagName("titel");
            XmlNodeList local = doc.GetElementsByTagName("local");
            XmlNodeList desc = doc.GetElementsByTagName("desc");
            XmlNodeList start = doc.GetElementsByTagName("start");
            XmlNodeList end = doc.GetElementsByTagName("end");

            Console.WriteLine(titel[0].InnerText);
            Console.WriteLine(local[0].InnerText);
            Console.WriteLine(desc[0].InnerText);
            Console.WriteLine(start[0].InnerText);
            Console.WriteLine(end[0].InnerText);

            //reformatting time to google api time
            //null iformatprovider argument as string is time represenation
            DateTime event_begin = DateTime.ParseExact(start[0].InnerText, "d/m/yyyy HH:mm:ss", null);
            DateTime event_end = DateTime.ParseExact(end[0].InnerText, "d/m/yyyy HH:mm:ss", null);

            // still to implement create from google cal function with these arguments
            Eventss eventss = new Eventss(); // connectie maken - service
            //Eventss.newEvent(titel[0].InnerText, local[0].InnerText, desc[0].InnerText, event_begin, event_end, ); //attendee list get all als parameter

        }

        public void updateEvent(XmlDocument doc)
        {
            XmlNodeList titel = doc.GetElementsByTagName("titel");
            XmlNodeList desc = doc.GetElementsByTagName("desc");
            XmlNodeList local = doc.GetElementsByTagName("local");
            XmlNodeList start = doc.GetElementsByTagName("start");
            XmlNodeList end = doc.GetElementsByTagName("end");
            XmlNodeList old_titel = doc.GetElementsByTagName("old_titel");


            Console.WriteLine(titel[0].InnerText);
            Console.WriteLine(desc[0].InnerText);
            Console.WriteLine(local[0].InnerText);
            Console.WriteLine(start[0].InnerText);
            Console.WriteLine(end[0].InnerText);
            Console.WriteLine(old_titel[0].InnerText);

            //reformatting time to google api time
            //null iformatprovider argument as string is time represenation
            DateTime event_begin = DateTime.ParseExact(start[0].InnerText, "d/m/yyyy HH:mm:ss", null);
            DateTime event_end = DateTime.ParseExact(end[0].InnerText, "d/m/yyyy HH:mm:ss", null);

            // still to implement update from google cal function with these arguments
            Eventss eventss = new Eventss(); // connectie maken - service
            //Eventss.updateEventByName(old_titel[0].InnerText, titel[0].InnerText, local[0].InnerText, desc[0].InnerText, event_begin, event_end,  ); //attendee list get all als parameter
        }

        public void deleteEvent(XmlDocument doc)
        {
            XmlNodeList titel = doc.GetElementsByTagName("titel");
            Console.WriteLine(titel[0].InnerText);

            Eventss eventss = new Eventss(); // connectie maken - service
            Eventss.DeleteEventByName(titel[0].InnerText);
        }

    }
}