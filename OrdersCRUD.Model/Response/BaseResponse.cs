using System.Diagnostics;
using OrdersCRUD.Model.Models;

namespace OrdersCRUD.Model.Response;

public class BaseResponse<T> : IBaseResponse<T> //TODO add "where T: class" ??
{
    /// <summary>
    /// описание ошибки если в запросе что-то пошло не так
    /// </summary>
    public string Description { get; set; }
    public ActivityStatusCode StatusCode { get; set; }
    public T Data { get; set; }
}

public interface IBaseResponse<T> 
{
    T Data { get; }
    string Description { get; }
    ActivityStatusCode StatusCode { get; }
}