using OrdersCRUD.Data.Interfaces;
using OrdersCRUD.Data.Repositories;

namespace OrdersCRUD.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _dbContext;
    private IOrderRepository _orders;
    private IOrderItemRepository _orderItems;
    private IProviderRepository _providers;

    public UnitOfWork(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IOrderRepository Orders
    {
        get
        {
            if (_orders == null)
            {
                _orders = new OrderRepository(_dbContext);
            }
            return _orders;
        }
    }
    public IOrderItemRepository OrderItems
    {
        get
        {
            if (_orderItems == null)
            {
                _orderItems = new OrderItemRepository(_dbContext);
            }
            return _orderItems;
        }
    }

    public IProviderRepository Providers
    {
        get
        {
            if (_providers == null)
            {
                _providers = new ProviderRepository(_dbContext);
            }
            return _providers;
        }
    }
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
}