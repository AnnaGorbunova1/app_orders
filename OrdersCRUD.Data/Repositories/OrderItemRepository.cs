using System.ComponentModel.DataAnnotations;
using OrdersCRUD.Data.Interfaces;
using OrdersCRUD.Model.Models;
using Microsoft.EntityFrameworkCore;
using OrdersCRUD.Model.ViewModels;

namespace OrdersCRUD.Data.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly ApplicationContext _dbContext;
    private readonly DbSet<OrderItem> _dbSet;

    public OrderItemRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.OrderItems;
    }
    public OrderItem? GetById(int id)
    {
        return _dbSet.FirstOrDefault(o => o.Id == id);
    }

    public async Task<List<OrderItem>> GetFilteredList(int id, string[] name, string[] unit)
    {
        var orderItems = _dbSet.Where(oi => oi.OrderId == id);
        if (name.Length > 0)
        {
            orderItems = orderItems.Where(o => name.Any(n => n == o.Name));
        }
        if (unit.Length > 0)
        {
            orderItems = orderItems.Where(o => unit.Any(n => n == o.Unit));
        }

        return await orderItems.ToListAsync();
    }
    
    public async Task<List<OrderItem>> GetListAsync(int id)
    {
        return await _dbSet.Where(oi => oi.OrderId == id).ToListAsync();
    }
    public async Task<List<string>> SelectNames(int id)
    {
        return await _dbSet.Where(oi => oi.OrderId == id).Select(o => o.Name).Distinct().ToListAsync();
    }
    public async Task<List<string>> SelectUnits(int id)
    {
        return await _dbSet.Where(oi => oi.OrderId == id).Select(o => o.Unit).Distinct().ToListAsync();
    }

    public async Task<OrderItem> CreateAsync(OrderItemVM orderItemVm)
    {
        if (orderItemVm.Order != null)
        {
            OrderItem orderItem = new OrderItem()
            {
                OrderId = orderItemVm.OrderId,
                Order = orderItemVm.Order,
                Name = orderItemVm.Name,
                Quantity = orderItemVm.QuantityDec,
                Unit = orderItemVm.Unit
            };
            
            await _dbSet.AddAsync(orderItem);
            await _dbContext.SaveChangesAsync();
            return orderItem;
        }
        throw new ValidationException($"Заказ {orderItemVm.OrderId} не найден");
    }
    
    public async Task UpdateAsync(OrderItemVM orderItemVm)
    {
        if (orderItemVm.Order != null)
        {
            OrderItem orderItem = new OrderItem
            {
                Id = orderItemVm.Id,
                Order = orderItemVm.Order,
                Name = orderItemVm.Name,
                Quantity = orderItemVm.QuantityDec,
                Unit = orderItemVm.Unit,
                OrderId = orderItemVm.OrderId
            };
           
            _dbSet.Update(orderItem);
            await _dbContext.SaveChangesAsync();
            return;
        }
        throw new ValidationException($"Заказ {orderItemVm.OrderId} не найден");
    }
    
    public async Task<int> DeleteAsync(int id)
    {
        OrderItem? orderItem = await _dbSet.FirstOrDefaultAsync(o => o.Id == id);
        if (orderItem == null) return 0;
        _dbSet.Remove(orderItem);
        await _dbContext.SaveChangesAsync();
        return orderItem.OrderId;

    }
}