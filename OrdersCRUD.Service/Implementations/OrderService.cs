using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersCRUD.Data.Interfaces;
using OrdersCRUD.Model.ViewModels;
using OrdersCRUD.Model.Models;
using OrdersCRUD.Model.Response;
using OrdersCRUD.Service.Interfaces;

namespace OrdersCRUD.Service.Implementations;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// проверка, является ли введенное значение количества положительным числом
    /// </summary>
    /// <param name="qtyStr">Количество, введенное на форме</param>
    /// <param name="qtyDec">число для записи в БД</param>
    /// <returns></returns>
    private bool QuantityIsValid(string qtyStr, out decimal qtyDec)
    {
        return Decimal.TryParse(qtyStr.Replace(".", ","), out qtyDec) && qtyDec > 0;
    }
    
    public IBaseResponse<Order> GetOrderById(int id)
    {
        var baseResponse = new BaseResponse<Order>();
        try
        {
            Order? order = _unitOfWork.Orders.GetById(id);
            if (order == null)
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = $"Order {id} not found";
                return baseResponse;
            }

            baseResponse.Data = order;
            baseResponse.StatusCode = ActivityStatusCode.Ok;
            return baseResponse;
        }
        catch (Exception e)
        {
            return new BaseResponse<Order>()
            {
                Description = $"{e.Message}",
                StatusCode = ActivityStatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<SelectList>> GetProvidersList()
    {
        try
        {
            List<Provider> providers = await _unitOfWork.Providers.GetProviders();
            return new BaseResponse<SelectList>()
            {
                StatusCode = ActivityStatusCode.Ok,
                Data = new SelectList(providers, "Id", "Name")
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<SelectList>()
            {
                StatusCode = ActivityStatusCode.Error,
                Description = e.Message
            };
        }
    }

    public async Task<IBaseResponse<OrdersListVM>> GetOrdersList(int[] provider, string[] number, 
        string dateFromStr, string dateToStr)
    {
        var baseResponse = new BaseResponse<OrdersListVM>();
        IEnumerable<Order> orders = new List<Order>();
        List<string> numbers = new List<string>();
        List<Provider> providers = new List<Provider>();
        DateTime outDate;
        DateTime? dateFrom =  DateTime.TryParse(dateFromStr, out outDate) ? outDate : null;
        DateTime? dateTo =  DateTime.TryParse(dateToStr, out outDate) ? outDate : null;
        try
        {
            orders = await _unitOfWork.Orders.GetFilteredList(provider, number, dateFrom, dateTo);
            numbers = await _unitOfWork.Orders.SelectOrderNumbers();
            providers = await _unitOfWork.Providers.GetProviders();
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
        }

        OrdersListVM ordersListVm = new OrdersListVM
        {
            Orders= orders,
            Providers = new MultiSelectList(providers, "Id", "Name", provider),
            Numbers = new MultiSelectList(numbers, number),
        };

        baseResponse.StatusCode = ActivityStatusCode.Ok;
        baseResponse.Data = ordersListVm;
        
        return baseResponse;
    }
    
    public async Task<IBaseResponse<int>> CreateOrderAsync(OrderCreateVM orderVm)
    {
        var baseResponse = new BaseResponse<int>();
        try
        {
            orderVm.Provider = _unitOfWork.Providers.GetByIdWithOrders(orderVm.ProviderId, orderVm.Id);
            var order = await _unitOfWork.Orders.CreateAsync(orderVm);
            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = order.Id;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            baseResponse.Data = 0;
            return baseResponse;
        }
    }

    public IBaseResponse<OrderItemVM> CreateOrderItem(int orderId)
    {
        var baseResponse = new BaseResponse<OrderItemVM>();
        try
        {
            var order = _unitOfWork.Orders.GetById(orderId);
            if (order == null)
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = $"Заказ {orderId} не найден";
                return baseResponse;
            }
            OrderItemVM orderItemVm = new OrderItemVM
            {
                OrderId = order.Id,
                OrderNumber = order.Number
            };
            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = orderItemVm;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            return baseResponse;
        }
        
    }

    public async Task<IBaseResponse<int>> SaveOrderItem(OrderItemVM orderItemVm)
    {
        var baseResponse = new BaseResponse<int>();
        if (!QuantityIsValid(orderItemVm.Quantity, out decimal qty))
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = "Количество должно быть числом больше 0";
            return baseResponse;
        }

        orderItemVm.QuantityDec = qty;
        try
        {
            orderItemVm.Order = _unitOfWork.Orders.GetById(orderItemVm.OrderId);
            var orderItem = await _unitOfWork.OrderItems.CreateAsync(orderItemVm);
            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = orderItem.OrderId;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            baseResponse.Data = 0;
            return baseResponse;
        }
        
    }
    
    public async Task<IBaseResponse<OrderVM>> GetOrderVm(int id, string[] name, string[] unit)
    {
        var baseResponse = new BaseResponse<OrderVM>();
        try
        {
            Order? order = _unitOfWork.Orders.GetById(id);
            if (order == null)
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = $"Заказ {id} не найден";
                return baseResponse;
            }
            OrderVM orderVm = new OrderVM
            {
                Id = order.Id,
                Date = order.Date.ToShortDateString(),
                Number = order.Number,
                ProviderName = order.Provider.Name,

            };
            List<OrderItem> orderItems = await _unitOfWork.OrderItems.GetFilteredList(id, name, unit);
            List<string> names = await _unitOfWork.OrderItems.SelectNames(id);
            List<string> units = await _unitOfWork.OrderItems.SelectUnits(id);
            OrderItemsListVM orderItemsList = new OrderItemsListVM
            {
                OrderItems= orderItems,
                Names = new MultiSelectList(names, name),
                Units = new MultiSelectList(units, unit)
            };
            orderVm.OrderItems = orderItemsList;

            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = orderVm;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            return baseResponse;
        }
    }
    
    public async Task<IBaseResponse<OrderCreateVM>> GetOrderForEdit(int id)
    {
        var baseResponse = new BaseResponse<OrderCreateVM>();
        try
        {
            Order? order = _unitOfWork.Orders.GetById(id);
            if (order == null)
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = $"Заказ {id} не найден";
                return baseResponse;
            }

            OrderCreateVM orderCreateVm = new OrderCreateVM()
            {
                Id = order.Id,
                Date = order.Date,
                Number = order.Number,
                ProviderId = order.Provider.Id
            };

            IEnumerable<OrderItem> orderItems = await _unitOfWork.OrderItems.GetListAsync(id);
            orderCreateVm.OrderItems = orderItems.Select(item => new OrderItemVM()
            {
                Id = item.Id,
                OrderId = item.OrderId,
                OrderNumber = item.Order.Number,
                Name = item.Name,
                Quantity = $"{item.Quantity:0.##}",
                QuantityDec = item.Quantity,
                Unit = item.Unit
            }).ToList();

            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = orderCreateVm;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            return baseResponse;
        }
    }
    
    public async Task<IBaseResponse<bool>> UpdateOrderAsync(OrderCreateVM orderVm)
    {
        var baseResponse = new BaseResponse<bool>();
        foreach (var item in orderVm.OrderItems)
        {
            if (!QuantityIsValid(item.Quantity, out decimal qty))
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = "Количество должно быть числом больше 0";
                return baseResponse;
            }
            item.QuantityDec = qty;
        }
        
        try
        {
            orderVm.Provider = _unitOfWork.Providers.GetByIdWithOrders(orderVm.ProviderId, orderVm.Id);
            await _unitOfWork.Orders.UpdateAsync(orderVm);
            var order = _unitOfWork.Orders.GetByIdLight(orderVm.Id);
            foreach (OrderItemVM oi in orderVm.OrderItems)
            {
                oi.Order = order;
                await _unitOfWork.OrderItems.UpdateAsync(oi);
            }
            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = true;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            return baseResponse;
        }
    }
    
    public async Task<IBaseResponse<bool>> DeleteOrderAsync(int id)
    {
        var baseResponse = new BaseResponse<bool>();
        try
        {
            if (!await _unitOfWork.Orders.DeleteAsync(id))
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = $"Заказ {id} не найден";
                return baseResponse;
            }

            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = true;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            return baseResponse;
        }
    }
    
    public async Task<IBaseResponse<int>> DeleteOrderItemAsync(int id)
    {
        var baseResponse = new BaseResponse<int>();
        try
        {
            int orderId = await _unitOfWork.OrderItems.DeleteAsync(id);
            if (orderId == 0)
            {
                baseResponse.StatusCode = ActivityStatusCode.Error;
                baseResponse.Description = $"Строка заказа не найдена";
                baseResponse.Data = 0;
                return baseResponse;
            }

            baseResponse.StatusCode = ActivityStatusCode.Ok;
            baseResponse.Data = orderId;
            return baseResponse;
        }
        catch (Exception e)
        {
            baseResponse.StatusCode = ActivityStatusCode.Error;
            baseResponse.Description = e.Message;
            return baseResponse;
        }
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }
}