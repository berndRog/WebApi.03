using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;

[assembly: InternalsVisibleTo("WebApiTest")]
namespace WebApi.Persistence;
internal class AccountsRepositoryFake(
   IDataContext dataContext
): IAccountsRepository {
   
   public IEnumerable<Account> Select() {
      return dataContext.Accounts.Values.ToList();
   }

   public Account? FindById(Guid id) { 
      dataContext.Accounts.TryGetValue(id, out Account? account);
      return account;
   }

   public void Add(Account account) =>
      dataContext.Accounts.Add(account.Id, account);
   
   public void Update(Account account) {
      dataContext.Accounts.Remove(account.Id);
      dataContext.Accounts.Add(account.Id, account);
   }

   public void Remove(Account account) =>
      dataContext.Accounts.Remove(account.Id);
   
   public IEnumerable<Account> SelectByOwnerId(Guid ownerId) =>
      dataContext.Owners.Values
         .Where(o => o.Id == ownerId)
         .SelectMany(o => o.Accounts)
         .ToList();

   public Account? FindByIban(string iban) =>
      dataContext.Accounts.Values
         .FirstOrDefault(a => a.Iban == iban);
}
