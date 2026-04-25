using Microsoft.AspNetCore.Mvc;
using SubscriptionBilling.Application;

namespace SubscriptionBilling.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BillingController(IMediator mediator) => _mediator = mediator;

        [HttpPost("customers")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerCommand cmd) => Ok(await _mediator.Send(cmd));

        [HttpPost("subscriptions")]
        public async Task<IActionResult> CreateSubscription(CreateSubscriptionCommand cmd) => Ok(await _mediator.Send(cmd));

        [HttpPost("subscriptions/{id}/cancel")]
        public async Task<IActionResult> CancelSubscription(Guid id)
        {
            await _mediator.Send(new CancelSubscriptionCommand(id));
            return NoContent();
        }

        [HttpPost("invoices/{id}/pay")]
        public async Task<IActionResult> PayInvoice(Guid id)
        {
            await _mediator.Send(new PayInvoiceCommand(id));
            return NoContent();
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices() => Ok(await _mediator.Send(new GetInvoicesQuery()));
    }
}
