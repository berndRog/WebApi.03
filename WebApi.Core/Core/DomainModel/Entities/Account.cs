using System;
using WebApi.Core.DomainModel.NullEntities;
namespace WebApi.Core.DomainModel.Entities;

public class Account: AEntity {
   
   #region properties
   public override Guid Id { get; init; } = Guid.Empty;
   public string  Iban     { get; init; } = string.Empty;
   public double  Balance  { get;  set; } = 0;
   
   // Navigation property
   public Owner Owner   { get; set; } = NullOwner.Instance;
   public Guid  OwnerId { get; set; } = NullOwner.Instance.Id;
   #endregion
   
   #region ctor
   public Account() {
      if (Iban.StartsWith("DE") && Iban.Length >= 11) return;
      var random = new Random();
      Iban = "DE" +
         random.Next(10, 99).ToString() +
         random.Next(10000000, 99999999).ToString() +
         random.Next(10000000, 99999999).ToString();
   }
   #endregion
   
}