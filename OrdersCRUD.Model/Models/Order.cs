using System.ComponentModel.DataAnnotations;

namespace OrdersCRUD.Model.Models;

public class Order : IBaseModel, IValidatableObject
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Number { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int ProviderId { get; set; }
    public Provider Provider { get; set; }
    public ICollection<OrderItem> OrderItems{ get; set; }
    public Order()
    {
        OrderItems = new List<OrderItem>();
        Number = "";
    }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Provider.Orders.Where(o => o.Id != Id).Any(o => o.Number == Number))
        {
            
            yield return new ValidationResult($"Уже есть заказ {Number} у выбранного поставщика\n");
        }
        else
        {
            yield return ValidationResult.Success;
        }
    }
}