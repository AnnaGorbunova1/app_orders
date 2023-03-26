using System.ComponentModel.DataAnnotations;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Model.ViewModels;

/// <summary>
/// модель заказа для создания/редактирования
/// </summary>
public class OrderCreateVM
{
    public int Id { get; set; }
    [Required]
    public string Number { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int ProviderId { get; set; }
    public Provider? Provider { get; set; }
    public List<OrderItemVM> OrderItems { get; set; }

    public OrderCreateVM()
    {
        OrderItems = new List<OrderItemVM>();
        Number = "";
    }
}