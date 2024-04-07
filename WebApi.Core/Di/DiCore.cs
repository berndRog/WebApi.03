using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Mapping;
namespace WebApi.Di; 

public static class DiCore {
   public static void AddCore(
      this IServiceCollection services
   ) {
      // add this assemlby to scan for profiles
      services.AddAutoMapper( typeof(Owner) );
      // Auto Mapper Configurations
      var mapperConfig = new MapperConfiguration(config => {
         config.AddProfile(new MappingProfile());
      });
   }
}