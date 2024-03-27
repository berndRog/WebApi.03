using AutoMapper;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Dto;

public class MappingProfile : Profile {
   public MappingProfile() {
      //        Source Destination
      CreateMap<Owner, OwnerDto>();
      CreateMap<OwnerDto, Owner>()
         .ForMember(m => m.Accounts, options => options.Ignore());

      CreateMap<Account, AccountDto>();
      CreateMap<AccountDto, Account>()
         .ForMember(m => m.Owner, options => options.Ignore());
   }
}