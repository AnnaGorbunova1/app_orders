using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Model.ViewModels;

/// <summary>
/// модель строк заказа для просмотра и фильтрации
/// </summary>
public class OrderItemsListVM
{
    public  IEnumerable<OrderItem> OrderItems { get; set; }
    public MultiSelectList Names { get; set; }
    public MultiSelectList Units { get; set; }

    public OrderItemsListVM()
    {
        OrderItems = new List<OrderItem>();
    }
}