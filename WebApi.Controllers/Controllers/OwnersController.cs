using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;

namespace WebApi.Controllers; 
[Route("banking")]
[ApiController]
public class OwnersController(
   // Dependency injection
   IOwnersRepository ownersRepository,
   IDataContext dataContext
) : ControllerBase {
   
   // Get all owners
   // http://localhost:5100/banking/owners
   [HttpGet("owners")]
   public ActionResult<IEnumerable<Owner>> Get() {
      return Ok(ownersRepository.Select());  
   }

   // Get owner by Id
   // http://localhost:5100/banking/owners/{id}
   [HttpGet("owners/{id}")]
   public ActionResult<Owner> GetOwnerById(Guid id) {
      switch (ownersRepository.FindById(id)) {
         case Owner owner: return Ok(owner);
         case null:        return NotFound($"Owner with given Id not found");
      }
   }

   // Create a new owner
   // http://localhost:5100/banking/owners
   [HttpPost("owners")]
   public ActionResult<Owner> CreateOwner(
      [FromBody] Owner owner
   ) {
      // check if owner.Id is set, else generate new Id
      if(owner.Id == Guid.Empty) owner.Id = Guid.NewGuid();
      // check if owner with given Id already exists   
      if(ownersRepository.FindById(owner.Id) != null) 
         return BadRequest($"CreateOwner: Owner with the given id already exists");
      
      // add owner to repository
      ownersRepository.Add(owner); 
      // save to datastore
      dataContext.SaveAllChanges();
      
      // return created owner      
      var uri = new Uri($"{Request.Path}/{owner.Id}", UriKind.Relative);
      return Created(uri: uri, value: owner);     
   }
   
   // Update owner
   // http://localhost:5100/banking/owners/{id}
   [HttpPut("owners/{id:Guid}")] 
   public ActionResult<Owner> UpdateOwner(
      [FromRoute] Guid id,
      [FromBody]  Owner updOwner
   ) {
      if(id != updOwner.Id) 
         return BadRequest($"UpdateOwner: Id in the route and body do not match.");
      
//    Owner updOwner = mapper.Map<Owner>(updPersonDto);
      
      Owner? owner = ownersRepository.FindById(id);
      if (owner == null)
         return NotFound($"UpdateOwner: Owner with given id not found.");

      // Update person
      owner = new Owner(updOwner);
      
      // save to repository and write to database 
      ownersRepository.Update(owner);
      dataContext.SaveAllChanges();

      return Ok(owner); //mapper.Map<PersonDto>(owner));
   }
   
   // Delete owner
   // http://localhost:5100/banking/owners/{id}
   [HttpDelete("owners/{id:Guid}")] 
   public ActionResult<Owner> DeleteOwner(
      [FromRoute] Guid id
   ) {
      Owner? owner = ownersRepository.FindById(id);
      if (owner == null)
         return NotFound($"DeleteOwner: Owner with given id not found.");

      // delete in repository and write to database 
      ownersRepository.Remove(owner);
      dataContext.SaveAllChanges();

      return Ok(owner); //mapper.Map<PersonDto>(owner));
   }
}