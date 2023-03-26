using Microsoft.EntityFrameworkCore;
using OrdersCRUD.Data.Interfaces;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Data.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly ApplicationContext _dbContext;
    private DbSet<Provider> _dbSet;

    public ProviderRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Providers;
    }
    
    public Provider? GetById(int id)
    {
        return _dbSet.FirstOrDefault(o => o.Id == id);
    }
    public Provider? GetByIdWithOrders(int id, int orderId)
    {
        return _dbSet.Include(p => p.Orders.Where(o => o.Id != orderId)).FirstOrDefault(o => o.Id == id);
    }
    
    public async Task<List<Provider>> GetProviders()
    {
        return await _dbSet.ToListAsync();
    }
}