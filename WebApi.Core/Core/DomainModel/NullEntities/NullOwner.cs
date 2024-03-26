using System;
using WebApi.Core.DomainModel.Entities;
namespace WebApi.DomainModel.NullEntities;
// https://jonskeet.uk/csharp/singleton.html

public sealed class NullOwner: Owner {  
   // Singleton Skeet Version 4
   private static readonly NullOwner instance = new NullOwner();
   public static NullOwner Instance { get => instance; }
   
   static NullOwner() { }
   
   private NullOwner() { 
      Id = Guid.Empty;
      Birthdate = DateTime.MinValue.ToUniversalTime();
   }
}