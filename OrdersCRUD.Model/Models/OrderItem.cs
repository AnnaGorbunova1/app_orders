using System.ComponentModel.DataAnnotations;

namespace OrdersCRUD.Model.Models;

public class OrderItem : IBaseModel, IValidatableObject
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    public Order Order {get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public decimal Quantity { get; set; }
    [Required]
    public string Unit { get; set; }

    public OrderItem()
    {
        Name = "";
        Unit = "";
    }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name == Order.Number)
        {
            
            yield return new ValidationResult($"Название товара не может совпадать с номером заказа\n");
        }
        else
        {
            yield return ValidationResult.Success;
        }
    }
}