using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBilling.Domain.Aggregates;
using SubscriptionBilling.Domain.Common;

namespace SubscriptionBilling.Application
{
    // --- DTOS ---
    public record InvoiceDto(Guid Id, decimal Amount, bool IsPaid, DateTime IssuedAt);

    // --- COMMANDS ---
    public record CreateCustomerCommand(string Name, string Email) : IRequest<Guid>;
    public record CreateSubscriptionCommand(Guid CustomerId, decimal Amount) : IRequest<Guid>;
    public record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest;
    public record PayInvoiceCommand(Guid InvoiceId) : IRequest;

    // --- QUERIES ---
    public record GetInvoicesQuery() : IRequest<List<InvoiceDto>>;//This Query must specify what it returns

    // --- HANDLERS ---
    public class BillingHandlers :
        IRequestHandler<CreateCustomerCommand, Guid>,
        IRequestHandler<CreateSubscriptionCommand, Guid>,
        IRequestHandler<CancelSubscriptionCommand>,
        IRequestHandler<PayInvoiceCommand>,//this returns void, so we use IRequest without a type parameter
        IRequestHandler<GetInvoicesQuery,List<InvoiceDto>>,//this returns a list of InvoiceDto, so we specify that as the type parameter
        INotificationHandler<SubscriptionActivated> // Handling domain event
    {
        private readonly Infrastructure.BillingDbContext _context;

        public BillingHandlers(Infrastructure.BillingDbContext context) => _context = context;

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
        {
            // 1. Check if the email is already taken
            // We do this here because the database (In-Memory) won't stop us
            var exists = await _context.Customers
                .AnyAsync(c => c.Email.ToLower() == request.Email.ToLower(), ct);

            if (exists)
            {
                // Because we added the Global Exception Middleware earlier, 
                // throwing this will automatically return a nice 400 Bad Request to the user
                throw new InvalidOperationException($"A customer with the email '{request.Email}' already exists.");
            }

            var customer = new Customer(request.Name, request.Email);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(ct);
            return customer.Id;
        }

        public async Task<Guid> Handle(CreateSubscriptionCommand request, CancellationToken ct)
        {
            var sub = new Subscription(request.CustomerId, request.Amount);
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync(ct); // This will trigger SubscriptionActivated event
            return sub.Id;
        }

        public async Task Handle(CancelSubscriptionCommand request, CancellationToken ct)
        {
            var sub = await _context.Subscriptions.FindAsync(request.SubscriptionId);
            sub?.Cancel();
            await _context.SaveChangesAsync(ct);
        }

        public async Task Handle(PayInvoiceCommand request, CancellationToken ct)
        {
            var invoice = await _context.Invoices.FindAsync(request.InvoiceId);
            if (invoice == null) throw new Exception("Invoice not found");

            invoice.MarkAsPaid();
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken ct)
        {
            // Manual mapping as requested
            return await _context.Invoices
                .Select(i => new InvoiceDto(i.Id, i.Amount, i.IsPaid, i.IssuedAt))
                .ToListAsync(ct);
        }

        // This handles the domain event to fulfill the business rule: 
        // "Activating a subscription generates first invoice"
        public async Task Handle(SubscriptionActivated notification, CancellationToken ct)
        {
            var invoice = new Invoice(notification.SubscriptionId, notification.Amount);
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(ct);
        }
    }
}
