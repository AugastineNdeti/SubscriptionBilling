using MediatR;
using Microsoft.EntityFrameworkCore;
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
            // 1. Identify all entities with events
            var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .ToList();

            // 2. Extract the events
            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // 3. Clear events BEFORE publishing to prevent recursion
            entitiesWithEvents.ForEach(e => e.Entity.ClearDomainEvents());

            // 4. Save to DB
            var result = await base.SaveChangesAsync(ct);

            // 5. Publish events (Handlers might call SaveChanges again, but events are now cleared)
            foreach (var @event in domainEvents)
            {
                await _publisher.Publish(@event, ct);
            }

            return result;
        }
    }
}
