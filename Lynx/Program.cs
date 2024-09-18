using FluentValidation;
using Lynx;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository;
using Lynx.Infrastructure.Repository.Interfaces;
using Lynx.IServices;
using Lynx.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceStack;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantMapper, TenantMapper>();
builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
builder.Services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

builder.Services.AddHttpClient<IEmailService, OneService>(opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration.GetSection("Email:Uri").Value!);
});
builder.Services.AddHttpClient<IEmailService, TwoService>(opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration.GetSection("Email:Uri").Value!);
});

builder.Services.AddScoped<OneService>();
builder.Services.AddScoped<TwoService>();
builder.Services.AddScoped<IEmailService>(provider =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var serviceType = httpContextAccessor.HttpContext?.Request.Headers["x-provider-version"].ToString();

    if (serviceType == "1")
    {
        return provider.GetRequiredService<OneService>();
    }
    else if (serviceType == "2")
    {
        return provider.GetRequiredService<TwoService>();
    }
    else
    {
        // For Postman
        //throw new Exception("Invalid Email Service Type header.");

        //For Swagger
        return provider.GetRequiredService<TwoService>();

    }
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ProducerService>();
builder.Services.AddScoped<IUserMapper, UserMapper>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.IncludeErrorDetails = true;
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthSettings.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/event-producing", async (ProducerService producer, CancellationToken c) =>
{
    await producer.ProduceAsync(c);
    return "Even Sent!";
}).WithOpenApi();
app.Run();