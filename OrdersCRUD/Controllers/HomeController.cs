using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrdersCRUD.Model.ViewModels;
using OrdersCRUD.Models;
using OrdersCRUD.Service.Interfaces;


namespace OrdersCRUD.Controllers;

public class HomeController : Controller
{
    private readonly IOrderService _orderService;

    public HomeController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    /// <summary>
    /// список заказов
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index(int[] provider, string[] number, string dateFromStr, string dateToStr)
    {
        if (string.IsNullOrEmpty(dateFromStr))
        {
            dateFromStr = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
        }
        if (string.IsNullOrEmpty(dateToStr))
        {
            dateToStr = DateTime.Today.ToString("yyyy-MM-dd");
        }
        
        ViewData["dateFrom"] = dateFromStr;
        ViewData["dateTo"] = dateToStr;
        var response =
            await _orderService.GetOrdersList(provider, number, dateFromStr, dateToStr);
        if (response.StatusCode == ActivityStatusCode.Ok)
        {
            return View(response.Data);
        }
        
        return RedirectToAction("Error",new {message = response.Description});
        
    }
    
    /// <summary>
    /// создать заказ
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Create()
    {
        var responce = await _orderService.GetProvidersList();
        if (responce.StatusCode == ActivityStatusCode.Ok)
        {
            ViewBag.ProvidersList = responce.Data; //TODO extract from data
            return View();
        }

        return RedirectToAction("Error",new {message = responce.Description});
    }

    
    /// <summary>
    /// создать заголовок заказа и перейти к созданию строки
    /// </summary>
    /// <param name="orderVm"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SaveCreateItem(OrderCreateVM orderVm)
    {
        var response = await _orderService.CreateOrderAsync(orderVm);
        if (response is { StatusCode: ActivityStatusCode.Ok, Data: > 0 })
        {
            return RedirectToAction("CreateItem", new { id = response.Data });
        }
        ModelState.AddModelError("", response.Description);
        
        var responceProvidersList = await _orderService.GetProvidersList();
        if (responceProvidersList.StatusCode == ActivityStatusCode.Ok)
        {
            ViewBag.ProvidersList = responceProvidersList.Data; 
            return View("Create",orderVm);
        }
        return RedirectToAction("Error", new {message = response.Description});
    }
    
    /// <summary>
    /// сохраняем заказ в БД
    /// </summary>
    /// <param name="orderVm"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateVM orderVm)
    {
        var response = await _orderService.CreateOrderAsync(orderVm);
        if (response is { StatusCode: ActivityStatusCode.Ok, Data: > 0 })
        {
            return RedirectToAction("Index");
        }
        
        ModelState.AddModelError("", response.Description);
        var responceProvidersList = await _orderService.GetProvidersList();
        if (responceProvidersList.StatusCode == ActivityStatusCode.Ok)
        {
            ViewBag.ProvidersList = responceProvidersList.Data; 
            return View();
        }
        return RedirectToAction("Error", new {message = response.Description});
    }
    
    /// <summary>
    /// создание строки заказа
    /// </summary>
    /// <param name="id">идентификатор заказа</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult CreateItem(int id)
    {
        var response = _orderService.CreateOrderItem(id);
        if (response.StatusCode == ActivityStatusCode.Ok)
        {
            return View(response.Data);
        }
        
        return RedirectToAction("Error", new {message = response.Description});
    }
    
    /// <summary>
    /// сохраняем в БД строку заказа
    /// </summary>
    /// <param name="orderItemVm"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateItem(OrderItemVM orderItemVm)
    {
        if (!ModelState.IsValid) return View(orderItemVm);
        var response = await _orderService.SaveOrderItem(orderItemVm);
        if (response.StatusCode == ActivityStatusCode.Ok)
        {
            return RedirectToAction("EditOrder", new { id = response.Data });
        }
        ModelState.AddModelError("", response.Description);
        return View(orderItemVm);
    }
    
    /// <summary>
    /// удаление строки заказа
    /// </summary>
    /// <param name="id">идентификатор строки заказа</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var response = await _orderService.DeleteOrderItemAsync(id);
        if (response.StatusCode != ActivityStatusCode.Ok)
            return RedirectToAction("Error", new { message = response.Description });
        if (response.Data == 0) return RedirectToAction("Index");
        return RedirectToAction("EditOrder", new { id = response.Data });

    }
    
    /// <summary>
    /// просмотр заказа
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Order(int id, string[] name, string[] unit)
    {
        var response = await _orderService.GetOrderVm(id, name, unit);
        if (response.StatusCode == ActivityStatusCode.Ok)
        {
            return View(response.Data);
        }

        return RedirectToAction("Error", new { message = response.Description });
    }
    
    /// <summary>
    /// удаление заказа
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteOrder(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
      
        var response = await _orderService.DeleteOrderAsync(id.Value);
        if (response.StatusCode == ActivityStatusCode.Ok)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Error",new {message = response.Description});
    }

    /// <summary>
    /// редактирование заказа
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> EditOrder(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var responceProviders = await _orderService.GetProvidersList();
        if (responceProviders.StatusCode == ActivityStatusCode.Ok)
        {
            ViewBag.ProvidersList = responceProviders.Data;
        }

        var response = await _orderService.GetOrderForEdit(id.Value);
        if (response.StatusCode == ActivityStatusCode.Ok)
        {
            return View(response.Data);
        }

        return RedirectToAction("Error",new {message = response.Description});
    }
    
    /// <summary>
    /// обновление заказа
    /// </summary>
    /// <param name="orderVm"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> EditOrder(OrderCreateVM orderVm)
    {
        if (ModelState.IsValid)
        {
            var response = await _orderService.UpdateOrderAsync(orderVm);
            if (response is { StatusCode: ActivityStatusCode.Ok, Data: true })
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Description);
        }

        var responceProviders = await _orderService.GetProvidersList();
        if (responceProviders.StatusCode == ActivityStatusCode.Error)
            return RedirectToAction("Error", new { message = responceProviders.Description });
        ViewBag.ProvidersList = responceProviders.Data;
        return View(orderVm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string message)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier , Message = message});
    }
    
    protected override void Dispose(bool disposing)
    {
        _orderService.Dispose();
        base.Dispose(disposing);
    }
}
