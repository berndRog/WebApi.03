using System;
using System.Collections.Generic;
using WebApi.Core.DomainModel.Entities;
namespace WebApi.Core;

public interface IOwnersRepository {
   IEnumerable<Owner> Select();
   Owner? FindById(Guid id);
   void Add(Owner owner);
   void Update(Owner owner);
   void Remove(Owner owner);

   IEnumerable<Owner> SelectByName(string name);
   Owner? FindByEmail(string email);
   IEnumerable<Owner> SelectByBirthDate(DateTime from, DateTime to);
}