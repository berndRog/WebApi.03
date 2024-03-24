using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using WebApi.Core;
using WebApi.Core.DomainModel.Dto;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Misc;

namespace WebApi.Controllers; 
[Route("banking/owners")]
[ApiController]
public class OwnersController(
   // Dependency injection
   IOwnersRepository ownersRepository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<OwnersController> logger
) : ControllerBase {
   
   // Get all owners as Dtos
   // http://localhost:5100/banking/owners
   [HttpGet("")]
   public ActionResult<IEnumerable<OwnerDto>> Get() {
      logger.LogDebug("GetOwners()");
      var owners = ownersRepository.Select();
      var ownerDtos = mapper.Map<IEnumerable<Owner>, IEnumerable<OwnerDto>>(owners);
      return Ok(ownerDtos);  
   }
   
   // Get owner by Id as Dto
   // http://localhost:5100/banking/owners/{id}
   [HttpGet("{id}")]
   public ActionResult<OwnerDto> GetOwnerById(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("GetOwnerById() id={id}", id.As8());
      switch (ownersRepository.FindById(id)) {
         case Owner owner: 
            return Ok(mapper.Map<Owner,OwnerDto>(owner));
         case null:        
            return NotFound($"Owner with given Id not found");
      }
   }

   // Get owners by name as Dto
   // http://localhost:5100/banking/owners/name
   [HttpGet("name")]
   public ActionResult<IEnumerable<OwnerDto>> GetOwnersByName(
      [FromQuery] string name
   ) {
      logger.LogDebug("GetOwnersByName() name={name}", name);
      switch (ownersRepository.SelectByName(name)) {
         case IEnumerable<Owner> owners: 
            return Ok(mapper.Map<IEnumerable<Owner>, IEnumerable<OwnerDto>>(owners));
         case null: 
            return NotFound($"Owners with given name not found");
      }
   }

   // Get owner by email as Dto
   // http://localhost:5100/banking/owners/email
   [HttpGet("owners/email")]
   public ActionResult<OwnerDto?> GetOwnerByEmail(
      [FromQuery] string email
   ) {
      switch (ownersRepository.FindByEmail(email)) {
         case Owner owner: return Ok(mapper.Map<Owner,OwnerDto>(owner));
         case null:        return NotFound($"Owner with given email not found");
      }
   }

   // Get owners by birthdate as Dtos
   // http://localhost:5100/banking/owners/birthdate/?from=yyyy-MM-dd&to=yyyy-MM-dd
   [HttpGet("birthdate")]
   public ActionResult<IEnumerable<OwnerDto>> GetOwnerByBirthdate(
      [FromQuery] string from,   // Date must be in the format yyyy-MM-dd
                                 // MM = 01 for January through 12 for December
      [FromQuery] string to      
   ) {
      logger.LogDebug("GetOwnerByBirthdate() from={from} to={to}", from, to);

      // Convert string to DateTime
      var (errorFrom, dateFrom) = ConvertToDateTime(from);
      if(errorFrom) 
         return BadRequest($"GetOwnerByBirthdate: Invalid date 'from': {from}");
      var (errorTo, dateTo) = ConvertToDateTime(to);
      if(errorTo) 
         return BadRequest($"GetOwnerByBirthdate: Invalid date 'to': {to}");

      var owners = ownersRepository.SelectByBirthDate(dateFrom, dateTo);   
      
      return Ok(mapper.Map<IEnumerable<Owner>, IEnumerable<OwnerDto>>(owners));
   }
   
   // Convert string in German format dd.MM.yyyy to DateTime
   private (bool, DateTime) ConvertToDateTime(string date) {
      try {
         var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
         return (false, dateTime );
      } catch (Exception e) {
         return (true, DateTime.MinValue);
      }
   }
   
   // Create a new owner
   // http://localhost:5100/banking/owners
   [HttpPost("")]
   public ActionResult<Owner> CreateOwner(
      [FromBody] OwnerDto ownerDto
   ) {
      logger.LogDebug("CreateOwner() ownerDto={ownerDto}", ownerDto.Name);
      
      var owner = mapper.Map<OwnerDto, Owner>(ownerDto);
      
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
      [FromBody]  OwnerDto updOwnerDto
   ) {
      
      logger.LogDebug("UpdateOwner() id={id} updOwnerDto={updOwnerDto}", id.As8(), updOwnerDto.Name);
      
      var updOwner = mapper.Map<OwnerDto, Owner>(updOwnerDto);

      // check if Id in the route and body match
      if(id != updOwner.Id) 
         return BadRequest($"UpdateOwner: Id in the route and body do not match.");
      
      // check if owner with given Id exists
      Owner? owner = ownersRepository.FindById(id);
      if (owner == null)
         return NotFound($"UpdateOwner: Owner with given id not found.");

      // Update person
      owner.Update(updOwner.Name, updOwner.Email);
      
      // save to repository and write to database 
      ownersRepository.Update(owner);
      dataContext.SaveAllChanges();

      return Ok(owner); 
   }
   
   // Delete owner
   // http://localhost:5100/banking/owners/{id}
   [HttpDelete("owners/{id:Guid}")] 
   public ActionResult<Owner> DeleteOwner(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("DeleteOwner {id}", id.As8());
      
      // check if owner with given Id exists
      Owner? owner = ownersRepository.FindById(id);
      if (owner == null)
         return NotFound($"DeleteOwner: Owner with given id not found.");

      // delete in repository and write to database 
      ownersRepository.Remove(owner);
      dataContext.SaveAllChanges();

      return Ok(owner); 
   }
}