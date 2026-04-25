using SubscriptionBilling.Domain.Common;

namespace SubscriptionBilling.Domain.Aggregates
{
    // INVOICE AGGREGATE
    public class Invoice : AggregateRoot
    {
        public Guid SubscriptionId { get; private set; }
        public decimal Amount { get; private set; }
        public bool IsPaid { get; private set; }
        public DateTime IssuedAt { get; private set; }

        private Invoice() { }

        public Invoice(Guid subscriptionId, decimal amount)
        {
            SubscriptionId = subscriptionId;
            Amount = amount;
            IsPaid = false;
            IssuedAt = DateTime.UtcNow;

            AddDomainEvent(new InvoiceGenerated(Id, SubscriptionId, Amount));
        }

        public void MarkAsPaid()
        {
            // We must enforce the invariant: Invoice cannot be paid twice
            if (IsPaid) throw new InvalidOperationException("This invoice is already paid.");

            IsPaid = true;
            AddDomainEvent(new PaymentReceived(Id, Amount));
        }
    }
}
