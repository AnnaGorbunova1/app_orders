
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OrdersCRUD.Model;
using OrdersCRUD.Model.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;


namespace OrdersCRUD.Data;

public class ApplicationContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Provider> Providers { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //поставщиков через приложение не добавляем
        modelBuilder.Entity<Provider>().HasData(
            new Provider { Id = 1, Name = "Butcher"},
            new Provider { Id = 2, Name = "Milkman"},
            new Provider { Id = 3, Name = "Bakery"}
        );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        SQLitePCL.Batteries.Init();
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var validationErrors = ChangeTracker
            .Entries<IValidatableObject>()
            .Where(e => e.State is EntityState.Modified or EntityState.Added)
            .SelectMany(e => e.Entity.Validate(null))
            .Where(r => r != ValidationResult.Success);

        if(validationErrors.Any())
        {
            string errorMessage = "";
            foreach (var error in validationErrors)
            {
                errorMessage += error.ErrorMessage;
            }
            throw new ValidationException(errorMessage);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}