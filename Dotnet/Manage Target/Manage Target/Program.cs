using Manage_Target.Context;
using Manage_Target.DataServices.AsyncBusClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ManageContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("mssql")));

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ManageContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
