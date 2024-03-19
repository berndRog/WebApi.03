using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Core;
using WebApi.Core.DomainModel;
using WebApi.Controllers;
using WebApi.Persistence;

public class Program {

   static void Main(string[] args) {

      // WebApplication Builder Pattern
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
      
      // DI-Container is part of the WebApplication
      // IServiceCollection services = builder.Services;
      
      // add http logging 
      builder.Services.AddHttpLogging(opts =>
         opts.LoggingFields = HttpLoggingFields.All);
      // add Controllers
      builder.Services.AddControllers();
      
      // add DataContext
      builder.Services.AddScoped<IDataContext, DataContextFake>();
      // add Repositories
      builder.Services.AddScoped<IOwnersRepository, OwnersRepositoryFake>();
      builder.Services.AddScoped<IAccountsRepository, AccountsRepositoryFake>();

      // Build the WebApplication
      WebApplication app = builder.Build();
      // use http logging
      app.UseHttpLogging();
      // routing
      app.MapControllers();
      // Run the WebApplication
      app.Run();

   }
}