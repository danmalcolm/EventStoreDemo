using System;
using System.Linq;
using System.Net;
using System.Text;
using ClientApiDemo.Model.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Spikes
{
    [TestFixture]
    public class StreamTests
    {
        private IEventStoreConnection CreateConnection()
        {
            var settings = ConnectionSettings.Create()
                                             .UseConsoleLogger();

            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, 1113), "DemoConnection");
            return connection;
        }

        [Test]
        public void creating_connection()
        {
            using (var connection = CreateConnection())
            {
                connection.Connect();
            }
        }

        [Test]
        public void adding_events()
        {
            using (var connection = CreateConnection())
            {
                connection.Connect();
                var events = from n in Enumerable.Range(1, 10)
                             select new CustomerCreated {Code = "cust-" + n, Name = "Customer " + n};
                var eventData = events.Select(CreateEventData).ToArray();

                connection.AppendToStream("crm", ExpectedVersion.Any, eventData, new UserCredentials("admin", "changeit"));
            }
        }

        private EventData CreateEventData(object @event)
        {
            var data = Serialize(@event);
            var metadata = Serialize(new { EventDate = DateTime.Now, ClrType = @event.GetType().FullName });
            return new EventData(Guid.NewGuid(), "CustomerCreated", true, data, metadata);
        }

        private byte[] Serialize(object value)
        {
            var json = JsonConvert.SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            return data;
        }



    }
}