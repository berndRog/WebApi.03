using System;
using System.Collections.Generic;
using WebApi.Core.DomainModel.Entities;
namespace WebApi.Core;

public interface IAccountsRepository {
   IEnumerable<Account> Select();
   Account? FindById(Guid id);
   void Add(Account account);
   void Update(Account account);
   void Remove(Account account);

   IEnumerable<Account> SelectByOwnerId(Guid ownerId);
}