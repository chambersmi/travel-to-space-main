using System.Runtime.CompilerServices;
using API.Extensions;
using API.Middleware;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Register services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration); 
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
//Custom middleware to handle exceptions
app.UseMiddleware<ExceptionMiddleware>();

//Redirect user to the new Errors Controller
app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

// Use Swagger
//app.UseSwaggerDocumentation();

 // Serve static files
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Content")), RequestPath = "/Content"
});

//CORS
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToController("Index", "Fallback");


//manage lifetime of the services
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<StoreContext>(); //retrieves instance of storecontext
var identityContext = services.GetRequiredService<AppIdentityDbContext>(); //identity-related data
var userManager = services.GetRequiredService<UserManager<AppUser>>(); // retrives user manager service for managing user-related operations
var logger = services.GetRequiredService<ILogger<Program>>(); //logging service

try {
    // If database doesn't exist, create it.    
    await context.Database.MigrateAsync(); //ensures db was created
    await identityContext.Database.MigrateAsync(); //ensures identity db was created
    await StoreContextSeed.SeedAsync(context); //get seed data
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager); // get identity seed data
} catch (Exception ex) {
    logger.LogError(ex, "An error occured during migration.");
}


app.Run();
