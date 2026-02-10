using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Commands.Products
{
    public record CreateProductCommand(
        string Name,
        decimal Price,
        int Stock
        ) : IRequest<Guid>;
    
        
    
}
