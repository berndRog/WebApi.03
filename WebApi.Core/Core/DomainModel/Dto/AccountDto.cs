using System;
using System.Text.Json.Serialization;
namespace WebApi.Core.DomainModel.Dto;

// immutable data class
public record AccountDto(
   Guid    Id,     
   string  Iban,    
   double  Balance, 
   // Navigation property
   Guid    OwnerId  
);
