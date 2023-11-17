using ComputingEPOS.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputingEPOS.Models;

public class NoForeignDbContext : BaseDbContext  {
    const string CONNECTION_STR = "Filename=data/main.db;foreign keys=false;";

    public NoForeignDbContext(DbContextOptions<NoForeignDbContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(CONNECTION_STR);
}