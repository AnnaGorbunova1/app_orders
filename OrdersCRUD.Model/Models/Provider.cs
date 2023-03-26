using System.ComponentModel.DataAnnotations;

namespace OrdersCRUD.Model.Models;

public class Provider : IBaseModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public ICollection<Order> Orders { get; set; }

    public Provider()
    {
        Name = "";
    }
}