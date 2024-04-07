using Microsoft.Extensions.DependencyInjection;
using WebApi.Core;
using WebApi.Persistence;
namespace WebApi.Di;

public static class DiPersistence {
   public static void AddPersistence(
      this IServiceCollection services
   ){
      services.AddScoped<IOwnersRepository, OwnersRepositoryFake>();
      services.AddScoped<IAccountsRepository, AccountsRepositoryFake>();
      services.AddScoped<IDataContext, DataContextFake>();
      
   }
}