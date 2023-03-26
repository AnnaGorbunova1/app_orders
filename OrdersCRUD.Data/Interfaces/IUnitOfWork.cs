
namespace OrdersCRUD.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProviderRepository Providers { get; }
    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }
    void Dispose();
}