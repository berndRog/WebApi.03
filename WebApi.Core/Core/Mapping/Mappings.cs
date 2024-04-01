using System.Collections.Generic;
using System.Linq;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Dto;
namespace WebApi.Core.Mapping;

public static class Mappings {
   
   public static OwnerDto ToOwnerDto(this Owner owner) =>
      new OwnerDto(
         Id: owner.Id,
         Name:owner.Name,
         Birthdate: owner.Birthdate,
         Email:owner.Email
      );
   
   public static Owner ToOwner(this OwnerDto ownerDto) =>
      new Owner {
         Id = ownerDto.Id,
         Name = ownerDto.Name,
         Birthdate = ownerDto.Birthdate,
         Email = ownerDto.Email,
         Accounts = new List<Account>()
      };
   
   public static IEnumerable<OwnerDto> ToOwnerDtos(this IEnumerable<Owner> owners) =>
      //    owners.Select(o => o.ToOwnerDto());
      owners.Select(ToOwnerDto);
   
   public static IEnumerable<Owner> ToOwner(this IEnumerable<OwnerDto> ownerDtos) => 
      //    ownerDtos.Select(o => o.ToOwner());
      ownerDtos.Select(ToOwner);

   public static AccountDto ToAccountDto(this Account account) =>
      new AccountDto(
         Id: account.Id,
         Iban: account.Iban,
         Balance: account.Balance,
         OwnerId: account.OwnerId
      );

   public static Account ToAccount(this AccountDto accountDto) =>
      new Account {
         Id = accountDto.Id,
         Iban = accountDto.Iban,
         Balance = accountDto.Balance,
         OwnerId = accountDto.OwnerId
      };


}