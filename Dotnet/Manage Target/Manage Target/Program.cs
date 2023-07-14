using Manage_Target.Context;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ManageContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("mssqldocker")));
   // opts.UseSqlServer(builder.Configuration.GetConnectionString("mssql")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ManageContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
