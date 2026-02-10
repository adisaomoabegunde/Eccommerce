using Eccormmerce.Application.Interfaces;
using Eccormmerce.Application.Queries.GetProducts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Queries.SearchProducts
{
    public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, List<ProductResponse>> 
    {
        private readonly IProductRepository _productRepository;

        public SearchProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductResponse>> Handle(SearchProductsQuery request,  CancellationToken cancellationToken)
        {
            var products = await _productRepository.SearchByNameAsync(request.Keyword);

            return products.Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Price,
                p.Stock
                )).ToList();
        }
    }

}
