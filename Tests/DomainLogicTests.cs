using SubscriptionBilling.Domain.Aggregates;
using SubscriptionBilling.Domain.Common;
using Xunit;

namespace SubscriptionBilling.Tests
{
    public class DomainLogicTests
    {
        [Fact]
        public void MarkAsPaid_ShouldSucceed_WhenInvoiceIsUnpaid()
        {
            // Arrange
            var invoice = new Invoice(Guid.NewGuid(), 100m);

            // Act
            invoice.MarkAsPaid();

            // Assert
            Assert.True(invoice.IsPaid);
            // We also check if the domain event was recorded
            Assert.Contains(invoice.DomainEvents, e => e is PaymentReceived);
        }

        [Fact]
        public void MarkAsPaid_ShouldThrowException_WhenInvoiceIsAlreadyPaid()
        {
            // Arrange
            // We enforce our invariant: Invoice cannot be paid twice
            var invoice = new Invoice(Guid.NewGuid(), 100m);
            invoice.MarkAsPaid();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => invoice.MarkAsPaid());
            Assert.Equal("This invoice is already paid.", exception.Message);
        }

        [Fact]
        public void CreateSubscription_ShouldRaise_SubscriptionActivatedEvent()
        {
            // Arrange & Act
            var sub = new Subscription(Guid.NewGuid(), 50m);

            // Assert
            Assert.True(sub.IsActive);
            Assert.Single(sub.DomainEvents.OfType<SubscriptionActivated>());
        }

        [Fact]
        public void CreateCustomer_ShouldThrow_WhenNameIsEmpty()
        {
            // Asserting that we enforce data integrity at the domain level
            Assert.Throws<Exception>(() => new Customer("", "test@test.com"));
        }
    }
}
