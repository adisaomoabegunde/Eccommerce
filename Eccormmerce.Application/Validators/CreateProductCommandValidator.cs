using Eccormmerce.Application.Commands.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator() {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(x => x.Price)
                .GreaterThan(0);
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0);
        
        }
    }
}
