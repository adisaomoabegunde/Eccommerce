using Eccommerce.Domain.Common.Pagination;
using Eccommerce.Domain.Entities;
using Eccommerce.Infrastructure.Persistence;
using Eccormmerce.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccommerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); 

            return new PagedResult<Product>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id); 
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetLowStockAsync(int threshold)
        {
            return await _context.Products
                .Where(p => !p.IsDeleted && p.Stock <= threshold)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchByNameAsync(string keyword)
        {
            return await _context.Products
                .Where(p => !p.IsDeleted && p.Name.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();
        }
    }
}
