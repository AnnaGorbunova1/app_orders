using OrdersCRUD.Model;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Data;

public interface IBaseRepository<T> where T: class, IBaseModel
{
    T? GetById(int id);
}