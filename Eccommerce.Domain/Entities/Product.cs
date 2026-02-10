using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccommerce.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int Stock {  get; private set; }
        public DateTime CreatedAt { get; private set; }

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private Product () { }

        public Product(string name, decimal price, int stock)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Product name is required");
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero");
            if (stock < 0)
                throw new ArgumentException("Stock cannot be negatve");

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            Stock = stock;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, decimal price, int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void SoftDelete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Product already deleted");

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
