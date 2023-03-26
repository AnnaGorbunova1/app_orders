
namespace OrdersCRUD.Model.ViewModels;

/// <summary>
/// модель заказа для просмотра и фильтрации строк
/// </summary>
public class OrderVM
{
    public int Id { get; set; }
    public string Number { get; set; }
    public string Date { get; set; }
    public string ProviderName { get; set; }
    public OrderItemsListVM OrderItems { get; set; }

    public OrderVM()
    {
        OrderItems = new OrderItemsListVM();
        Number = "";
        Date = "";
        ProviderName = "";
    }
}