
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersCRUD.Model.ViewModels;
using OrdersCRUD.Model.Models;
using OrdersCRUD.Model.Response;

namespace OrdersCRUD.Service.Interfaces;

public interface IOrderService
{
    IBaseResponse<Order> GetOrderById(int id);

    Task<IBaseResponse<OrdersListVM>> GetOrdersList(int[] provider, string[] number, 
        string dateFromStr, string dateToStr);

    Task<IBaseResponse<SelectList>> GetProvidersList();
    Task<IBaseResponse<int>> CreateOrderAsync(OrderCreateVM orderVm);
    IBaseResponse<OrderItemVM> CreateOrderItem(int orderId);
    Task<IBaseResponse<int>> SaveOrderItem(OrderItemVM orderItemVm);
    Task<IBaseResponse<OrderVM>> GetOrderVm(int id, string[] name, string[] unit);
    Task<IBaseResponse<OrderCreateVM>> GetOrderForEdit(int id);
    Task<IBaseResponse<bool>> UpdateOrderAsync(OrderCreateVM orderVm);
    Task<IBaseResponse<bool>> DeleteOrderAsync(int id);
    Task<IBaseResponse<int>> DeleteOrderItemAsync(int id);
    void Dispose();
}