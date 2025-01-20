using LoginMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoginMS.Custom;
using LoginMS.Services;
using Microsoft.OpenApi.Models;
using LoginMS.Interfaces;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

// Add Image Uploading Service
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Configure HTTP Client for Image Uploading Service
builder.Services.AddHttpClient<FileUploadService>(client =>
{
    //client.BaseAddress = new Uri("http://localhost:7010");

    // SSL Certificate Configuration => Delete this on "Production"
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
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
            new string[] { }
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));

// Utils Instantiation
builder.Services.AddSingleton<Utils>();

// JWT Authentication Configuration
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    /* RequireHttpsMetadata sets the API to only get HTTPS-based Tokens 
     If "true", the API will reject any token request sent via HTTP without HTTPS
        - Suitable for Production environments.
     If "false", the API will accept any token request":
        - Suitable for Development environments*/
    config.RequireHttpsMetadata = false;

    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        /* ValidateIssuer allows or prohibits external apps to access this API URL when published 
            - Use "true" for Production environments (Better Security)*/
        ValidateIssuer = false, // No access from external apps

        /* We can specify which systems (Apps, Server) can access this API URL 
            - Use "true" for Production environments (Better Security)*/
        ValidateAudience = false, // No access from external apps (It can be configured)

        /* Use time limit validation for Token Lifetime and Clock-related specifications */
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
    };
}).AddCookie(config =>
{
    config.Cookie.HttpOnly = true;
    config.Cookie.SecurePolicy = CookieSecurePolicy.None; // Use Always in Production
    config.Cookie.SameSite = SameSiteMode.Strict;
    config.Cookie.Name = "AuthToken";
});

// Enable Cors Policy to prevent execution blocking
builder.Services.AddCors(options =>
{
    options.AddPolicy("NewPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // Extended File Size limit up to 50 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("NewPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
