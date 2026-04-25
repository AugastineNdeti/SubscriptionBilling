using MediatR;

namespace SubscriptionBilling.Domain.Common
{
    // A simple base for our aggregates to handle domain events
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        private readonly List<object> _domainEvents = new();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }

    // Domain Events
    public record SubscriptionActivated(Guid SubscriptionId, Guid CustomerId, decimal Amount):INotification;
    public record InvoiceGenerated(Guid InvoiceId, Guid SubscriptionId, decimal Amount):INotification;
    public record PaymentReceived(Guid InvoiceId, decimal Amount):INotification;
}
