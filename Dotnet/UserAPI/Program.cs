using UserAPI.Context;
using Microsoft.EntityFrameworkCore;
using Manage_Target.Context;
using Manage_Target.DataServices.AsyncMessageBus;
using Manage_Target.DataServices.EventProcessing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<UserContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("mssql")));

builder.Services.AddScoped<IReportRepo, ReportRepo>();

builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
