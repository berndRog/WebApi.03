using System;
using WebApi.Core.DomainModel.NullEntities;
using WebApi.Core.Dto;
namespace WebApi.Core.DomainModel.Entities;

public class Account: AEntity {
   
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string Iban { get; init; } = string.Empty;
   public double  Balance  { get; set; }
   
   // Navigation property
   public Owner Owner   { get; set; } = NullOwner.Instance;
   public Guid  OwnerId { get; set; } = NullOwner.Instance.Id;
   #endregion
   
   #region ctor
   public Account() {
      Iban = CheckIban(Iban);
   }
   public Account(AccountDto dto) {
      Id = dto.Id;
      Iban = CheckIban(dto.Iban);
      Balance = dto.Balance;
      OwnerId = dto.OwnerId;
   }
   #endregion

   #region methods
   private string CheckIban(string iban) {
      if (iban.Length >= 8) 
         return iban;
      var random = new Random();
      var newIban = "DE" +
         random.Next(10, 99).ToString() +
         random.Next(10000000, 99999999).ToString() +
         random.Next(10000000, 99999999).ToString();
      return newIban;
   }
   #endregion
   
}