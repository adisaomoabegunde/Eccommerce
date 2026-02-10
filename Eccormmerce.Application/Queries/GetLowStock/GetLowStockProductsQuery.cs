using Eccormmerce.Application.Queries.GetProducts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Queries.GetLowStock
{
    public record GetLowStockProductsQuery(int Threshold = 10) :IRequest<List<ProductResponse>>;
    
}
