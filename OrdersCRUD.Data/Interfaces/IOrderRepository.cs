using OrdersCRUD.Model.Models;
using OrdersCRUD.Model.ViewModels;

namespace OrdersCRUD.Data.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetFilteredList(int[] provider, string[] number,
        DateTime? dateFrom, DateTime? dateTo);

    Task<List<string>> SelectOrderNumbers();
    
    Task<Order> CreateAsync(OrderCreateVM orderVm);
    Task UpdateAsync(OrderCreateVM orderVm);
    Task<bool> DeleteAsync(int id);
    Order? GetByIdLight(int id);
}