using System;
using System.Text.Json.Serialization;
using WebApi.Core.DomainModel.NullEntities;
namespace WebApi.Core.DomainModel.Entities;

public class Account: AEntity {
   public override Guid Id { get; init; } = Guid.Empty;
   public string  Iban     { get; init; } = string.Empty;
   public double  Balance  { get;  set; } = 0;
   
   // Navigation property
   public Owner Owner   { get; set; } = NullOwner.Instance;
   public Guid  OwnerId { get; set; } = NullOwner.Instance.Id;
   
   #region ctor
   public Account() { }
   #endregion
   
}