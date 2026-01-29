using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using MS_Auth.Domain.Repositories;
using MS_Auth.Domain.Models;
using MS_Auth.Infrastructure.Auth.Interface;
using MS_Auth.Infrastructure.Auth.Service;
using MS_Auth.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.AddScoped<IGerarToken, GerarToken>();

builder.Services.AddScoped<IRepositoryBase<User>>(provider =>
    new JsonRepositoryBase<User>("users.json"));

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.Run();