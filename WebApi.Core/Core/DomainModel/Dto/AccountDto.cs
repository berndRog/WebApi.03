using System;
namespace WebApi.Core.DomainModel.Entities;

public class AccountDto {
   public Guid    Id       { get; set; } = Guid.Empty;
   public string  Iban     { get; set; } = string.Empty;
   public double  Balance  { get; set; } = 0;
   
   // Navigation property
   public Guid  OwnerId { get; set; } = Guid.Empty;
}