using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Pacco.Services.Pricing.Core.Exceptions;
using Pacco.Services.Pricing.Core.Repositories;
using Pacco.Services.Pricing.Core.Services;
using Pacco.Services.Pricing.DTO;

namespace Pacco.Services.Pricing.Queries.Handlers
{
    internal sealed class GetOrderPricingHandler : IQueryHandler<GetOrderPricing, OrderPricingDto>
    {
        private readonly ICustomersRepository _repository;
        private readonly ICustomerDiscountsService _service;

        public GetOrderPricingHandler(ICustomersRepository repository, ICustomerDiscountsService service)
        {
            _repository = repository;
            _service = service;
        }


        public async Task<OrderPricingDto> HandleAsync(GetOrderPricing query)
        {
            var customer = await _repository.GetAsync(query.CustomerId);

            if (customer is null)
            {
                throw new CustomerNotFoundException(query.CustomerId);
            }

            var customerDiscount = _service.CalculateCustomerDiscount(customer, query.OrderPrice);

            return new OrderPricingDto
            {
                OrderId = query.OrderId,
                CustomerDiscount = customerDiscount,
                OrderPrice = query.OrderPrice
            };
        }
    }
}