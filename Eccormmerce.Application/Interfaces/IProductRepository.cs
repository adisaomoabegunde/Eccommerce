using Eccommerce.Domain.Common.Pagination;
using Eccommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize);

        Task<Product?> GetByIdAsync(Guid id);
        Task UpdateAsync(Product product);
        Task<List<Product>> GetLowStockAsync(int threshold);
        Task<List<Product>> SearchByNameAsync(string keyword);
        
    }
}
