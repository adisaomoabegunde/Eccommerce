using Eccormmerce.Application.Interfaces;
using Eccormmerce.Application.Queries.GetProductById;
using Eccormmerce.Application.Queries.GetProducts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Commands.Products
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository) { 
            _repository = repository;
            
        }
        public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);

            if (product is null)
                throw new KeyNotFoundException("Product not found");

            return new ProductResponse(
                product.Id,
                product.Name,
                product.Price,
                product.Stock

                );
            
                
            
        }
    }
}
