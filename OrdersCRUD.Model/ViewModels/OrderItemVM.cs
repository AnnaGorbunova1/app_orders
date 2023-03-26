using System.ComponentModel.DataAnnotations;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Model.ViewModels;

/// <summary>
/// модель строк заказа для создания/редактирования
/// </summary>
public class OrderItemVM
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public string OrderNumber { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Quantity { get; set; }
    public decimal QuantityDec { get; set; }
    
    [Required]
    public string Unit { get; set; }
    
    public OrderItemVM()
    {
        OrderNumber = "";
        Name = "";
        Quantity = "";
        Unit = "";
    }
}