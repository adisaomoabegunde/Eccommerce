using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Commands.Auth.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        string Role
        ) : IRequest<Guid>;
   
}
