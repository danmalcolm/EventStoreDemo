using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using NUnit.Framework;
using Newtonsoft.Json;
using Spikes.Model.Events;

namespace Spikes
{
    [TestFixture]
    public class StreamTests
    {
        private UserCredentials credentials = new UserCredentials("admin", "changeit");

        private IEventStoreConnection CreateConnection()
        {
            var settings = ConnectionSettings
                .Create()
                .UseConsoleLogger();

            var endPoint = new IPEndPoint(IPAddress.Loopback, 1113);
            var connection = EventStoreConnection.Create(settings, endPoint, "DemoConnection");
            return connection;
        }

        [Test]
        public void connecting()
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
                             select new CustomerCreated { Code = "cust-" + n, Name = "Customer " + n };
                var eventData = events.Select(EventToEventData).ToArray();

                connection.AppendToStream("crm1", ExpectedVersion.Any, eventData, credentials);
            }
        }

        [Test]
        public void reading_events()
        {
            using (var connection = CreateConnection())
            {
                const string stream = "crm2";

                connection.Connect();
                var events1 = from n in Enumerable.Range(1, 10)
                             select new CustomerCreated { Code = "cust-" + n, Name = "Customer " + n };
                var eventData = events1.Select(EventToEventData).ToArray();
                connection.AppendToStream(stream, ExpectedVersion.Any, eventData, credentials);

                var resolvedEvents = ReadEvents(connection, stream, StreamPosition.Start);
                var events2 = resolvedEvents.Select(EventFromResolvedEvent).ToList();
            }
        }

        private IEnumerable<ResolvedEvent> ReadEvents(IEventStoreConnection connection, string stream, int start)
        {
            var slice = connection.ReadStreamEventsForward(stream, start, 200, false);
            IEnumerable<ResolvedEvent> events = slice.Events;
            if(!slice.IsEndOfStream)
                events = events.Concat(ReadEvents(connection, stream, slice.NextEventNumber));
            return events;
        }

        private object EventFromResolvedEvent(ResolvedEvent resolvedEvent)
        {
            var metadata = DeserializeObject(resolvedEvent.Event.Metadata, typeof (EventMetadata)) as EventMetadata;
            return DeserializeObject(resolvedEvent.Event.Data, metadata.ClrType);
        }

        private EventData EventToEventData(object @event)
        {
            var data = SerializeObject(@event);
            var metadata = SerializeObject(new EventMetadata { Date = DateTime.Now, ClrType = @event.GetType() });
            return new EventData(Guid.NewGuid(), "CustomerCreated", true, data, metadata);
        }

        private byte[] SerializeObject(object value)
        {
            var json = JsonConvert.SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            return data;
        }

        private object DeserializeObject(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);
            var value = JsonConvert.DeserializeObject(json, type);
            return value;
        }
    }
}