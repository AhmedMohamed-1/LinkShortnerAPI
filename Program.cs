using FluentValidation;
using LinkShorterAPI.Authentication.Token;
using LinkShorterAPI.Background_Service;
using LinkShorterAPI.Models;
using LinkShorterAPI.Repositories.LinkRepository;
using LinkShorterAPI.Repositories.PremiumFeaturesRepository;
using LinkShorterAPI.Repositories.UserRepository;
using LinkShorterAPI.Services.EmailServices;
using LinkShorterAPI.Services.LinkService;
using LinkShorterAPI.Services.PremiumFeaturesServices;
using LinkShorterAPI.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register the mapper Profiles
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

// Register FluentValidation for automatic validation
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// Register FluentValidation Manual Approach
//builder.Services.AddScoped<IValidator<SignUpDTO>, SignUpDTOValidator>();
//builder.Services.AddScoped<IValidator<SignInDTO>, SignInDTOValidator>();
//builder.Services.AddScoped<IValidator<UpdateUserDTO>, UpdateUserDTOValidator>();

// Add Swager Service For Api Documntaion
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LinkShortnerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LinkShortnerConnectionString")));

// ✅ Configure Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing from configuration.")
            )
        ),
        ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes clock skew
    };
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LinkShortener API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });
});


// Register repositories and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserToken, UserToken>();
builder.Services.AddScoped<IClickRepository, ClickRepository>();
builder.Services.AddScoped<IClickTrackingService, ClickTrackingService>();

// Register Link services and repositories
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<ILinkService, LinkService>();

builder.Services.AddScoped<IEmailService, SendGridEmailService>();
builder.Services.AddHostedService<ExpirationCheckService>();


builder.WebHost.UseUrls("https://localhost:5001");



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

app.MapControllers();

app.Run();
