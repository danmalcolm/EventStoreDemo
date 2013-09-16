namespace ClientApiDemo.Model
{
    public class Customer
    {
        public Customer(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}