using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Model.ViewModels;

/// <summary>
/// модель заказа для просмотра в списке и фильтрации
/// </summary>
public class OrdersListVM
{
    
    public IEnumerable<Order> Orders { get; set; }
    public MultiSelectList Numbers { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public MultiSelectList Providers { get; set; }
    public OrdersListVM()
    {
          Orders = new List<Order>();
    }

}