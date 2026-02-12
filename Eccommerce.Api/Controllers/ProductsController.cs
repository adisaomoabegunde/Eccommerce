using Eccormmerce.Application.Commands.Products;
using Eccormmerce.Application.Commands.Products.DeleteProduct;
using Eccormmerce.Application.Commands.Products.UpdateProduct;
using Eccormmerce.Application.Queries.GetLowStock;
using Eccormmerce.Application.Queries.GetProductById;
using Eccormmerce.Application.Queries.GetProducts;
using Eccormmerce.Application.Queries.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eccommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
// create product apiApI another 
        [Authorize]
        [HttpPost("create-product")]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                data = new { productId },
                message = "Product created successfully"
            });
        }

        [HttpGet("Get-product")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetProductsQuery(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("GetProductById{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("UpdateProduct{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand request)
        {
            var command = new UpdateProductCommand(
                id,
                request.Name,
                request.Price,
                request.Stock
                );

            await _mediator.Send(command);
            return Ok(new
            {
                success = true,
                message = "Product Updated"
            });
        }

        [HttpDelete("DeleteProduct{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return Ok(new
            {
                success = true,
                message = "Product Deleted"
            });
        }
        [HttpGet("low-stock")]
        [Authorize]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            var result = await _mediator.Send(
                new GetLowStockProductsQuery(threshold));

            return Ok(result);
        }
        [HttpGet("search-product")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Search keyword is required");

            var result = await _mediator.Send(new SearchProductsQuery(keyword));
            return Ok(result);
        }
    }
}
