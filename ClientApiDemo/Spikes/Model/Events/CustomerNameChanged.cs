namespace Spikes.Model.Events
{
    public class CustomerNameChanged
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("EventType: CustomerNameChanged, Name: {0}", Name);
        }
    }
}