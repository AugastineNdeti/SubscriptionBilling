using SubscriptionBilling.Domain.Common;

namespace SubscriptionBilling.Domain.Aggregates
{
    // SUBSCRIPTION AGGREGATE
    public class Subscription : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public bool IsActive { get; private set; }
        public decimal MonthlyAmount { get; private set; }

        private Subscription() { } // For EF

        public Subscription(Guid customerId, decimal amount)
        {
            CustomerId = customerId;
            MonthlyAmount = amount;
            IsActive = true;

            // I'm triggering the activation event here so the system knows to generate the first invoice
            AddDomainEvent(new SubscriptionActivated(Id, CustomerId, MonthlyAmount));
        }

        public void Cancel()
        {
            IsActive = false;
        }
    }
}
