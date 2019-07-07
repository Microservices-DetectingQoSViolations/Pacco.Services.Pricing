using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Pacco.Services.Pricing.Core.Services;
using Pacco.Services.Pricing.DTO;
using Pacco.Services.Pricing.Exceptions;
using Pacco.Services.Pricing.Services.Clients;

namespace Pacco.Services.Pricing.Queries.Handlers
{
    internal sealed class GetOrderPricingHandler : IQueryHandler<GetOrderPricing, OrderPricingDto>
    {
        private readonly ICustomersServiceClient _client;
        private readonly ICustomerDiscountsService _service;

        public GetOrderPricingHandler(ICustomersServiceClient client, ICustomerDiscountsService service)
        {
            _client = client;
            _service = service;
        }

        public async Task<OrderPricingDto> HandleAsync(GetOrderPricing query)
        {
            var customer = await _client.GetAsync(query.CustomerId);

            if (customer is null)
            {
                throw new CustomerNotFoundException(query.CustomerId);
            }

            var customerDiscount = _service.CalculateDiscount(customer.AsEntity());

            return new OrderPricingDto
            {
                CustomerDiscount = customerDiscount,
                OrderPrice = query.OrderPrice
            };
        }
    }
}