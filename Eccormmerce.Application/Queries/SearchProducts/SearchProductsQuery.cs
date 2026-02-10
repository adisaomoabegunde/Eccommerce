using Eccormmerce.Application.Queries.GetProducts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Queries.SearchProducts
{
    public record SearchProductsQuery(string Keyword) : IRequest<List<ProductResponse>>;
    
}
