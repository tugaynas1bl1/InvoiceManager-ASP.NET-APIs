using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.Extensions;
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

builder.Services.AddSwagger()
                .AddTaskFlowDbContext(builder.Configuration)
                .AddIdentityAndDb(builder.Configuration)
                .AddJwtAuthenticationAndAuthorization(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapperAndOtherDI();


var app = builder.Build();

app.UseTaskFlowPipeline();

app.Run();
