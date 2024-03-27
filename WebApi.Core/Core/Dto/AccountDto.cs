using System;
namespace WebApi.Core.Dto;

// immutable data class
public record AccountDto(
   Guid    Id,     
   string  Iban,    
   double  Balance, 
   // Navigation property
   Guid    OwnerId  
);
