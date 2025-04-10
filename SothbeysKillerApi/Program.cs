using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SothbeysKillerApi.Contexts;
using SothbeysKillerApi.Entities;
using SothbeysKillerApi.ExceptionHandlers;
using SothbeysKillerApi.Exceptions;
using SothbeysKillerApi.Repository;
using SothbeysKillerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DB")));

builder.Services.AddTransient<IAuctionService, DbAuctionService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddExceptionHandler<AuctionValidationExceptionHandler>();
builder.Services.AddExceptionHandler<ServerExceptionsHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddIdentity<AuctionUser, IdentityRole<Guid>>(opt =>
    {
        opt.User.RequireUniqueEmail = true;

        opt.Password.RequireDigit = true;
        opt.Password.RequiredLength = 6;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AuctionDbContext>()
    .AddDefaultTokenProviders();

var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnmm123456789"));

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = secret
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();