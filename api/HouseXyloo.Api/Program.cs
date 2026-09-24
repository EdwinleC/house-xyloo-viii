
using HouseXyloo.Api.Data;
using HouseXyloo.Api.Services;
using Microsoft.EntityFrameworkCore;
using HouseXyloo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HouseXylooDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("HouseXylooDatabase")
    )
);

builder.Services.AddScoped<HouseStatusService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGet("/", () =>
    Results.Redirect("/swagger"));

app.Run();