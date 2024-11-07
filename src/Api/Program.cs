using Amazon.Runtime;
using Amazon.S3;
using Api.Modules;
using Api.OptionsSetup;
using Application;
using Application.Common.Interfaces.Services;
using Infrastructure;
using Infrastructure.Services.FileServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddSingleton<IFileStorageService, S3FileStorageService>();
var awsOptions = builder.Configuration.GetAWSOptions();
awsOptions.Credentials = new BasicAWSCredentials(
    builder.Configuration["AWS:AccessKeyId"],
    builder.Configuration["AWS:SecretAccessKey"]);

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.InitializeDb();
app.MapControllers();
app.UseCors("AllowOrigin");
app.Run();

public partial class Program;