using ComputingEPOS.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputingEPOS.Backend.Models;

public class NoForeignDbContext : BaseDbContext  {
    const string CONNECTION_STR = "Filename=data/main.db;foreign keys=false;";

    public NoForeignDbContext(DbContextOptions<NoForeignDbContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(CONNECTION_STR);
}