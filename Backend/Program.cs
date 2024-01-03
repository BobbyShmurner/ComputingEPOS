using ComputingEPOS.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend;

static class Program {
    static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<BaseDbContext, NoForeignDbContext>();
        
        // Add services to the container.
        builder.Services.AddScoped<IMenusService, MenusService>();
        builder.Services.AddScoped<IStockService, StockService>();
        builder.Services.AddScoped<IOrdersService, OrdersService>();
        builder.Services.AddScoped<IOrderItemsService, OrderItemsService>();
        builder.Services.AddScoped<ITransactionsService, TransactionsService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
