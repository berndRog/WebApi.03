using System;
namespace WebApi.Core.DomainModel.Dto; 
public class OwnerDto {
   public Guid     Id       { get; set; } = Guid.Empty;
   public string   Name     { get; set; } = string.Empty;
   public DateTime Birthdate{ get; set; } = DateTime.UtcNow;
   public string   Email    { get; set; } = string.Empty;
   // no Navigation property
   
}
