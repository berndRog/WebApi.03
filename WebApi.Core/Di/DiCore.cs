using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Core.Mapping;
namespace WebApi.Di; 
public static class DiCore {
   public static IServiceCollection AddCore(
      this IServiceCollection services
   ){
      services.AddAutoMapper(typeof(MappingProfile));
      // Auto Mapper Configurations
      var mapperConfig = new MapperConfiguration(config => {
         config.AddProfile(new MappingProfile());
      });
      
      return services;
   }
}