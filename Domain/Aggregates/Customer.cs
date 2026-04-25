using SubscriptionBilling.Domain.Common;

namespace SubscriptionBilling.Domain.Aggregates
{
    // CUSTOMER AGGREGATE
    public class Customer : AggregateRoot
    {
        public string Name { get; private set; }
        public string Email { get; private set; }

        public Customer(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("Name is required.");
            Name = name;
            Email = email;
        }
    }
}
