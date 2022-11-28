global using ApiMvc.Functions;
global using ApiMvc.Models;
global using Firebase.Database.Query;
using ApiMvc.Database;
using ApiMvc.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductRepository, ProductBase>();
builder.Services.AddScoped<IAddressRepository, AddressBase>();
builder.Services.AddScoped<ICardRepository, CardBase>();
builder.Services.AddScoped<ICartRepository, CartBase>();
builder.Services.AddScoped<IUserRepository, UserBase>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseBase>();
builder.Services.AddScoped<IReceiptRepository, ReceiptBase>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
