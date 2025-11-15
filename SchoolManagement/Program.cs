using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Endpoints;
using SchoolManagement.Repositories;
using SchoolManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core with In-Memory Database
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseInMemoryDatabase("SchoolManagementDb"));

// Register repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();

// Register services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISchoolClassService, SchoolClassService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapStudentEndpoints();
app.MapSchoolClassEndpoints();

app.Run();
