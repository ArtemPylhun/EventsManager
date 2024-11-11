using Amazon.Runtime;
using Amazon.S3;
using Api.Modules;
using Api.OptionsSetup;
using Application;
using Application.Common.Interfaces.Services;
using Infrastructure;
using Infrastructure.Services.FileServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApplication();

//Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventManager", Version = "v1" });
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Authentication and Authorization setup
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    "928ba67e924129647366756eaf32035f2acf13cd1474298bff7cdcad5719a4091589d1fa6d2f5e97d8c6b953830f5adc1154c605cbdde1584fed31c4f173a44b9d8aa6f3e10c7afc9756cab404d0a90175cb3ff74ac1826239ed6cfbae8791a21d750831e47fb9f4b608e61f9c110e217ea87b8798acb9cd1db164a1d4be8ce8e1e05eddd36ab9c4a3536c7f976d8703fae0c86538955865d74dc9fc1912f8bb05b30df6bbe228aee0fa7bb6368b3d890cda1f0f83c30e5ab989abb240e747ac50aa8d7461416943ae38bd658ff38b5e90b2ce5a79b394c03f5e4d6184fa4f3a545f0443aa0c1606d2c8cd3a149a7edfd9fd13c235951456088ec234bf64d443"u8
                        .ToArray()),
            ValidIssuer = "YourIssuer",
            ValidAudience = "YourAudience",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

// Configure JWT options
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

// S3 File Storage service
builder.Services.AddSingleton<IFileStorageService, S3FileStorageService>();
var awsOptions = builder.Configuration.GetAWSOptions();
awsOptions.Credentials = new BasicAWSCredentials(
    builder.Configuration["AWS:AccessKeyId"],
    builder.Configuration["AWS:SecretAccessKey"]);

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();

// CORS policy setup
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
        options => options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
await app.InitializeDb();
app.MapControllers();

app.Run();


public partial class Program;