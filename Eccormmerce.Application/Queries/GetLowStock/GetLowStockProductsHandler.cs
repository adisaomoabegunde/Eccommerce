using Eccormmerce.Application.Interfaces;
using Eccormmerce.Application.Queries.GetProducts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Queries.GetLowStock
{
    public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetLowStockProductsHandler(IProductRepository productRepository) { 
            _productRepository = productRepository;
            
        }

        public async Task<List<ProductResponse>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetLowStockAsync(request.Threshold);

            return products.Select(p => new ProductResponse
            (
                 p.Id,
                 p.Name,
                 p.Price,
                 p.Stock
            )).ToList();
        }
    }
  
}
