using System.Text;
using API;
using API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Using Extension methods to add the services to the container
// Created Extension Methods using classes ApplicationServiceExtension and IdentityServiceExtension

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

//Ordering of placing these middle wares is important
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();
