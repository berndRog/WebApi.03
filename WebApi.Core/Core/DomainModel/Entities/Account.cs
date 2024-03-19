using System;
namespace WebApi.Core.DomainModel.Entities;

public record Account: IEntity {
   public Guid    Id       { get; set; } = Guid.Empty;
   public string  Iban     { get; set; } = string.Empty;
   public double  Balance  { get; set; } = 0;
   
   // Navigation property
   public Owner Owner   { get; set; } = null!;
   public Guid  OwnerId { get; set; } = Guid.Empty;
   
   #region ctor
   public Account (Account source) {
      Id = source.Id;
      Iban = source.Iban;
      Balance = source.Balance;
   }
   #endregion
   
}