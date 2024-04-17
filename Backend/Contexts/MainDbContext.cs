using ComputingEPOS.Common;
using Microsoft.EntityFrameworkCore;

namespace ComputingEPOS.Common;

public class MainDbContext : BaseDbContext {
    const string CONNECTION_STR = "Filename=data/main.db;foreign keys=true;";

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(CONNECTION_STR);
}