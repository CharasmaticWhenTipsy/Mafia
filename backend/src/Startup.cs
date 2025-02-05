using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mafia.Hubs;

namespace Mafia
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method is used to configure services for dependency injection
        public void ConfigureServices(IServiceCollection services)
        {

            // The ConfigureServices method is where you register services that the application will use, including:
            // Controllers: Register MVC controllers or API controllers.
            // Database context: Add services for interacting with databases, such as DbContext for Entity Framework.
            // Authentication & Authorization: Add services related to security, such as authentication, JWT, or cookies.
            // SignalR: Register SignalR services if you’re using real-time communication.
            // Dependency Injection: Register application services (e.g., IUserService, repositories, etc.) using AddScoped, AddSingleton, or AddTransient.
            // CORS: Configure CORS policies to allow cross-origin requests.

            // Register SignalR
            services.AddSignalR();

            // Register authentication/authorization services
            services.AddAuthentication();
            services.AddAuthorization();

            // Add other services like background tasks, logging, etc.
        }

        // This method is used to configure the HTTP request pipeline (middleware)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // The Configure method sets up the HTTP request pipeline. This is where you add various middleware components that handle requests as they come into the application. Common middleware includes:
            // Error Handling: Handle errors, logging, or sending user-friendly error pages.
            // Routing: Enable routing for controllers, Razor Pages, or SignalR hubs.
            // Authentication/Authorization: Add middleware to check for user authentication/authorization.
            // Static File Serving: Serve static files (CSS, JS, images, etc.) from the wwwroot folder.
            // SignalR: Set up SignalR hubs to enable real-time communication.

            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage(); // Show detailed error pages in dev
            // }
            // else
            // {
            //     app.UseExceptionHandler("/Home/Error"); // Show custom error page in production
            //     app.UseHsts(); // HTTP Strict Transport Security in production
            // }

            app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
            app.UseStaticFiles(); // Serve static files (e.g., images, CSS, JS)

            app.UseRouting(); // Add routing middleware

            app.UseAuthentication(); // Authentication middleware (if you're using authentication)
            app.UseAuthorization(); // Authorization middleware (if you're using authorization)

            // Configure the endpoints for controllers and Razor pages
            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers(); // Maps attribute-based routing (e.g., ApiController)
                endpoints.MapHub<LobbyHub>("/lobby"); // Map a SignalR Hub if you're using SignalR
            });
        }
    }
}
