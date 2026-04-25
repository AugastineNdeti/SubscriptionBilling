using Microsoft.EntityFrameworkCore;
using SubscriptionBilling.Infrastructure;
using SubscriptionBilling.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// adding MediatR to handle our CQRS and Domain Events
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Setting up the In-Memory DB
builder.Services.AddDbContext<BillingDbContext>(opt => opt.UseInMemoryDatabase("BillingDb"));

builder.Services.AddControllers();
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

app.UseMiddleware<ExceptionHandlingMiddleware>();// added this for global error handling

app.MapControllers();

app.Run();
