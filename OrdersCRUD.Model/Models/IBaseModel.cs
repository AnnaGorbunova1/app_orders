using System.ComponentModel.DataAnnotations;

namespace OrdersCRUD.Model.Models;

public interface IBaseModel
{
    [Key]
    public int Id { get; set; }    
}