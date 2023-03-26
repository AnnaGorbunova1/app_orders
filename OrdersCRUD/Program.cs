using Microsoft.EntityFrameworkCore;
using OrdersCRUD.Data;
using OrdersCRUD.Data.Interfaces;
using OrdersCRUD.Service.Implementations;
using OrdersCRUD.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//connection to database
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data Source=OrdersCRUD.db"));

// Add services to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();