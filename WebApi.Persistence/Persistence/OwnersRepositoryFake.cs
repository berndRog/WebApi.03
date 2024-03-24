using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;

namespace WebApi.Persistence;

internal class OwnersRepositoryFake(
   IDataContext dataContext
): IOwnersRepository {
   
   public IEnumerable<Owner> Select() {
      return dataContext.Owners.Values
         .ToList();
   }

   public Owner? FindById(Guid id) { 
      dataContext.Owners.TryGetValue(id, out Owner? owner);
      return owner;
   }

   public void Add(Owner owner) =>
      dataContext.Owners.Add(owner.Id, owner);
   
   public void Update(Owner owner) {
      dataContext.Owners.Remove(owner.Id);
      dataContext.Owners.Add(owner.Id, owner);
   }

   public void Remove(Owner owner) =>
      dataContext.Owners.Remove(owner.Id);

   public IEnumerable<Owner> SelectByName(string name) =>
      dataContext.Owners.Values
         .Where(owner => owner.Name.Contains(name))
         .ToList();

   public Owner? FindByEmail(string email) =>
      dataContext.Owners.Values
         .FirstOrDefault(owner => owner.Email == email);

   public IEnumerable<Owner> SelectByBirthDate(DateTime from, DateTime to) =>
      dataContext.Owners.Values
         .Where(owner =>
            owner.Birthdate >= from &&
            owner.Birthdate <= to)
         .ToList();

}