namespace Eccommerce.Api.Contracts
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock {  get; set; }
    }
}
