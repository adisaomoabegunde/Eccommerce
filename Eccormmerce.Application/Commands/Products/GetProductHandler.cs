using Eccommerce.Domain.Common.Pagination;
using Eccormmerce.Application.Interfaces;
using Eccormmerce.Application.Queries.GetProducts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Commands.Products
{
    public class GetProductHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductResponse>>
    {
        private readonly IProductRepository _repository;

        public GetProductHandler(IProductRepository repository) { 
            _repository = repository;
        }

        public async Task<PagedResult<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(request.PageNumber, request.PageSize);
            return new PagedResult<ProductResponse>
            {
                Items = result.Items.Select(p => new ProductResponse(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock
                    )).ToList(),

                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

    }
}
