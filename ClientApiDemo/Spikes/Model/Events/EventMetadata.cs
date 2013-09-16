using System;

namespace Spikes.Model.Events
{
    public class EventMetadata
    {
        public DateTime Date { get; set; }

        public Type ClrType { get; set; }
    }
}