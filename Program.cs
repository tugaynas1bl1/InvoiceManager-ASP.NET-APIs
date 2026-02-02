using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.Mapping;
using ASP_NET_Final_Proj.Services.Classes;
using ASP_NET_Final_Proj.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
    new OpenApiInfo
    {
        Version = "v1",
        Title = "InvoiceManager API",
        Description = "This API includes fully CRUD operations for the InvoiceManager Project",
        Contact = new OpenApiContact
        {
            Name = "InvoiceManager Team",
            Email = "support@invoicemanager.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("http://opensource.org/license/mit")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<InvoiceManagerDbContext>(
    options => options.UseSqlServer(connectionString)
    );

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInvoiceRowService, InvoiceRowService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
