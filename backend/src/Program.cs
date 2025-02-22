using Mafia;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // ?

// Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// Build
var app = builder.Build();

// Configure
startup.Configure(app, app.Environment);

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run(); // This internally starts the application