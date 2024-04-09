using Microsoft.EntityFrameworkCore;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Common;

public abstract class BaseDbContext : DbContext {
    public BaseDbContext(DbContextOptions options) : base(options) {}

    protected abstract override void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

    public DbSet<ClockInOut> ClockInOut { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Menu> Menus { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Menu_MenuItem> Menu_MenuItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Shift> Shifts { get; set; } = null!;
    public DbSet<Stock> Stock { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
}