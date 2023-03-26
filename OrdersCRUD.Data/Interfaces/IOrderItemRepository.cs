using OrdersCRUD.Model.Models;
using OrdersCRUD.Model.ViewModels;

namespace OrdersCRUD.Data.Interfaces;

public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    Task<List<OrderItem>> GetFilteredList(int id, string[] name, string[] unit);
    Task<List<OrderItem>> GetListAsync(int id);
    Task<List<string>> SelectNames(int id);
    Task<List<string>> SelectUnits(int id);
    Task<OrderItem> CreateAsync(OrderItemVM orderItemVm);
    Task UpdateAsync(OrderItemVM orderItemVm);
    Task<int> DeleteAsync(int id);
}