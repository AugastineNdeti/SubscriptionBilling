using SubscriptionBilling.Domain.Aggregates;
using SubscriptionBilling.Domain.Common;
using System.Collections.Generic;

namespace SubscriptionBilling.Infrastructure
{
    public class BillingDbContext : DbContext
    {
        private readonly IPublisher _publisher;

        public BillingDbContext(DbContextOptions<BillingDbContext> options, IPublisher publisher)
            : base(options)
        {
            _publisher = publisher;
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // I grab all entities that have events before saving
            var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var result = await base.SaveChangesAsync(ct);

            // We dispatch events AFTER saving to ensure data consistency
            foreach (var entity in entitiesWithEvents)
            {
                foreach (var @event in entity.DomainEvents)
                {
                    await _publisher.Publish(@event, ct);
                }
                entity.ClearDomainEvents();
            }

            return result;
        }
    }
}
