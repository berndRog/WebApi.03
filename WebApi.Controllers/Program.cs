using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Di;

namespace WebApi;
public class Program {

   static void Main(string[] args) {

      // WebApplication Builder Pattern
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
      
      // Configure logging
      // ---------------------------------------------------------------------
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
      
      // Configure DI-Container aka builder.Services:IServiceCollection
      // ---------------------------------------------------------------------
      // add http logging 
      builder.Services.AddHttpLogging(opts =>
         opts.LoggingFields = HttpLoggingFields.All);
      // add Controllers
      builder.Services.AddControllers();
      // // add auto Mapper Configurations
      // var mapperConfig = new MapperConfiguration(config => {
      //    config.AddProfile(new MappingProfile());
      // });
      builder.Services.AddCore();
      
      // add DataContext
      // builder.Services.AddScoped<IDataContext, DataContextFake>();
      // add Repositories
      // builder.Services.AddScoped<IOwnersRepository, OwnersRepositoryFake>();
      // add Mapping
      // builder.Services.AddAutoMapper(typeof(MappingProfile));
      // add Persistence to DI-Container
      builder.Services.AddPersistence();
      
      // Build the WebApplication
      // -------------------------------------------------------------------
      WebApplication app = builder.Build();

      var mapper = app.Services.GetService<IMapper>();
      var logger = app.Services.GetService<ILogger<Program>>();
      
      // use http logging
      app.UseHttpLogging();
      // routing
      app.MapControllers();
      // Run the WebApplication
      app.Run();

   }
}