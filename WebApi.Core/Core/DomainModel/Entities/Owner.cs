using System;
using System.Collections.Generic;
namespace WebApi.Core.DomainModel.Entities; 

public record Owner: IEntity {
   #region properties
   public Guid     Id       { get; set; } = Guid.Empty;
   public string   Name     { get; set; } = string.Empty;
   public DateTime Birthdate{ get; set; } = DateTime.UtcNow;
   public string   Email    { get; set; } = string.Empty;

   // Navigation property
   // One-to-many relationship Owner -> Account
   public ICollection<Account> Accounts { get; set; } = new List<Account>();
   #endregion
   
   #region ctor
   public Owner() { }
   
   public Owner (Owner source) {
      Id = source.Id;
      Name = source.Name;
      Birthdate = source.Birthdate;
      Email = source.Email;
   }
   #endregion
   
   #region methods
   public void Add(Account account) {
      if (account.OwnerId != Id)
         throw new ApplicationException("Account is already asigned to another owner");
      account.Owner = this;
      account.OwnerId = Id;
      Accounts.Add(account);
   }
   #endregion
   
}
