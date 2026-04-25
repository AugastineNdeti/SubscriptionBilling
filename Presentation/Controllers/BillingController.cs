using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionBilling.Application;
using SubscriptionBilling.Domain.Common;

namespace SubscriptionBilling.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BillingController(IMediator mediator) => _mediator = mediator;

        [HttpPost("customers")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            var response = new CreatedIdResponseDto(id);
            return Ok(new ApiResponse<CreatedIdResponseDto>(true, "Customer created successfully", response));
        }

        [HttpPost("subscriptions")]
        public async Task<IActionResult> CreateSubscription(CreateSubscriptionCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return Ok(new ApiResponse<Guid>(true, "Subscription created and first invoice generated", id));
        }

        [HttpPost("subscriptions/{id}/cancel")]
        public async Task<IActionResult> CancelSubscription(Guid id)
        {
            await _mediator.Send(new CancelSubscriptionCommand(id));
            return NoContent();
        } // this won't use the ApiResponse wrapper since it's a 204 No Content response

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            var invoices = await _mediator.Send(new GetInvoicesQuery());
            return Ok(new ApiResponse<List<InvoiceDto>>(true, "Invoices retrieved", invoices));
        }

        [HttpPost("invoices/{id}/pay")]
        public async Task<IActionResult> PayInvoice(Guid id)
        {
            await _mediator.Send(new PayInvoiceCommand(id));
            return Ok(new ApiResponse<object>(true, "Payment processed successfully", null));
        }


    }
}
