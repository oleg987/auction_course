using System.Globalization;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SothbeysKillerApi.Constants;
using SothbeysKillerApi.Contexts;
using SothbeysKillerApi.Entities;
using SothbeysKillerApi.ExceptionHandlers;
using SothbeysKillerApi.Infrastructure;
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

builder.Services.AddTransient<DbAuctionService>();

#if DEBUG
builder.Services.AddTransient<IAuctionService, AuctionServiceCacheDecorator>();
#else
builder.Services.AddTransient<IAuctionService, AuctionServiceCacheDecorator>();
#endif

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

        opt.Lockout.MaxFailedAccessAttempts = 3;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
        opt.Lockout.AllowedForNewUsers = true;
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

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(PolicyConstants.AdultOnly40Plus, policy =>
    {
        policy.RequireClaim(CustomClaimConstants.UserAge);
        policy.RequireAssertion(context =>
        {
            var age = context.User.Claims
                .Where(c => c.Type == CustomClaimConstants.UserAge)
                .Select(c => int.Parse(c.Value))
                .First();

            if (age >= 40)
            {
                return true;
            }
            
            return false;
        });
    });
});

// MemCache
builder.Services.AddMemoryCache();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = "localhost";
    opt.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
    {
        AbortOnConnectFail = true,
        EndPoints = { opt.Configuration }
    };
});

builder.Services.AddSingleton<IHybridCache, HybridCacheMemoryProvider>();

//builder.Services.AddSingleton<IHybridCache, HybridCacheDistributedProvider>();

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddAutoMapper(typeof(Program).Assembly);

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