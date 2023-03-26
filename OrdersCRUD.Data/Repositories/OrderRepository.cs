using System.ComponentModel.DataAnnotations;
using OrdersCRUD.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using OrdersCRUD.Model.Models;
using OrdersCRUD.Model.ViewModels;

namespace OrdersCRUD.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationContext _dbContext;
    private readonly DbSet<Order> _dbSet;

    public OrderRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Orders;
    }
    public Order? GetById(int id)
    {
        return _dbSet
            .Include(o => o.Provider)
            .Include(o => o.OrderItems)
            .FirstOrDefault(o => o.Id == id);
    }
    public Order? GetByIdLight(int id)
    {
        return _dbSet.FirstOrDefault(o => o.Id == id);
    }
    
    public async Task<IEnumerable<Order>> GetFilteredList(int[] provider, string[] number, 
        DateTime? dateFrom, DateTime? dateTo)
    {
        IQueryable<Order> orders = _dbSet.Include(o => o.Provider);
        
        if (provider.Length > 0)
        {
            orders = orders.Where(o=> provider.Any(p => p==o.ProviderId));
        }
        if (number.Length > 0)
        {
            orders = orders.Where(o => number.Any(n => n == o.Number));
        }
        
        if (dateFrom != null)
        {
            orders = orders.Where(o => o.Date >= dateFrom.Value);
        }
        if (dateTo != null)
        {
            orders = orders.Where(o => o.Date <= dateTo.Value);
        }

        return await orders.ToListAsync();
        
    }

    public async Task<List<string>> SelectOrderNumbers()
    {
        return await _dbSet.Select(o => o.Number).Distinct().ToListAsync();
    }

    public async Task<Order> CreateAsync(OrderCreateVM orderVm)
    {
        if (orderVm.Provider != null)
        {
            Order order = new Order
            {
                Number = orderVm.Number,
                Date = orderVm.Date,
                ProviderId = orderVm.ProviderId,
                Provider = orderVm.Provider,
                OrderItems = new List<OrderItem>()
            };

            
            await _dbSet.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }
        throw new ValidationException($"Не найден поставщик {orderVm.ProviderId}");
    }

    public async Task UpdateAsync(OrderCreateVM orderVm)
    {
        if (orderVm.Provider != null)
        {
            Order order = new Order
            {
                Id = orderVm.Id,
                Number = orderVm.Number,
                Date = orderVm.Date,
                ProviderId = orderVm.ProviderId, 
                Provider = orderVm.Provider
            };
            
            _dbSet.Update(order);
            await _dbContext.SaveChangesAsync();
            return;
        }
        throw new ValidationException($"Не найден поставщик {orderVm.ProviderId}");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Order? order = await _dbSet.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return false;
        _dbSet.Remove(order);
        await _dbContext.SaveChangesAsync();
        return true;

    }
}