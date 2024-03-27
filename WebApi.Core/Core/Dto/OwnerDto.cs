using System;
namespace WebApi.Core.Dto; 

// immutable data class
public record OwnerDto(
   Guid Id,            // generates: public Guid     Id       { get; init; } 
   string Name,        // generates: public string   Name     { get; init; }
   DateTime Birthdate, // generates: public DateTime Birthdate{ get; init; }
   string Email        // generates: public string   Email    { get; init; }
);
