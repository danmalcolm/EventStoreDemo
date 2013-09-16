namespace Spikes.Model.Events
{
    public class CustomerCreated
    {
        public string Code { get; set; } 

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("EventType: CustomerCreated, Code: {0}, Name: {1}", Code, Name);
        }
    }
}