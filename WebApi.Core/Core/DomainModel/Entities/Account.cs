using System;
using System.Collections.Generic;
using System.Transactions;
using WebApi.Core.DomainModel.NullEntities;
using WebApi.Core.Dto;
using WebApi.Core.Misc;

namespace WebApi.Core.DomainModel.Entities;

public class Account: AEntity {
   
   #region fields
   private string _iban = string.Empty;
   #endregion
   
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string Iban { 
      get => _iban;
      init => _iban = Utils.CheckIban(value);
   }
   public double Balance { get; set; } 
   
   // Navigation-Property Account --> Owner [1]  
   public Owner Owner{ get; set; } = NullOwner.Instance;
   public Guid OwnerId{ get; set; } = NullOwner.Instance.Id;
   #endregion
   
   #region ctor
   public Account() {}
   public Account(AccountDto dto) {
      Id = dto.Id;
      Iban = Utils.CheckIban(dto.Iban);
      Balance = dto.Balance;
      OwnerId = dto.OwnerId;
   }
   #endregion
}