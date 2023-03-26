using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Data.Interfaces;

public interface IProviderRepository : IBaseRepository<Provider>
{
    Task<List<Provider>> GetProviders();
    Provider? GetByIdWithOrders(int id, int orderId);
}