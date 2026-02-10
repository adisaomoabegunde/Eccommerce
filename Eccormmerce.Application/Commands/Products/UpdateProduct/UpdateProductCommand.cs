using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Commands.Products.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        decimal Price,
        int Stock
        ) : IRequest<Unit>;
    
    
}
