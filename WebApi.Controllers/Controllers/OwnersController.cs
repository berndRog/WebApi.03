using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Dto;
using WebApi.Core.Misc;

namespace WebApi.Controllers; 
[Route("banking/owners")]
[ApiController]
public class OwnersController(
   // Dependency injection
   IOwnersRepository repository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<OwnersController> logger
) : ControllerBase {
   
   // Get all owners as Dtos
   // http://localhost:5100/banking/owners
   [HttpGet("")]
   public ActionResult<IEnumerable<OwnerDto>> Get() {
      logger.LogDebug("GetOwners()");
      
      // get all owners
      var owners = repository.Select();
      
      // return owners as Dtos
      var ownerDtos = mapper.Map<IEnumerable<OwnerDto>>(owners);
      return Ok(ownerDtos);  
   }
   
   // Get owner by Id as Dto
   // http://localhost:5100/banking/owners/{id}
   [HttpGet("{id:guid}")]
   public ActionResult<OwnerDto> GetOwnerById(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("GetOwnerById() id={id}", id.As8());

      return repository.FindById(id) switch {
         // return owner as Dto
         { } owner => Ok(mapper.Map<OwnerDto>(owner)),
         // return Not Found
         null => NotFound("Owner with given Id not found")
      };
   }

   // Get owners by name as Dto
   // http://localhost:5100/banking/owners/name?name=abc
   [HttpGet("name")]
   public ActionResult<OwnerDto> GetOwnerByName(
      [FromQuery] string name
   ) {
      logger.LogDebug("GetOwnerByName() name={name}", name);

      return repository.FindByName(name) switch {
         // return owners as Dtos
         { } owner => Ok(mapper.Map<OwnerDto>(owner)),
         // return Not Found
         null => NotFound("Owners with given name not found")
      };
   }

   // Get owner by email as Dto
   // http://localhost:5100/banking/owners/email?email=abc
   [HttpGet("email")]
   public ActionResult<OwnerDto?> GetOwnerByEmail(
      [FromQuery] string email
   ) {
      logger.LogDebug("GetOwnersByName() email={email}", email);

      return repository.FindByEmail(email) switch {
         // return owner as Dto
         { } owner => Ok(mapper.Map<OwnerDto>(owner)),
         // return Not Found
         null => NotFound("Owner with given email not found")
      };
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

      // get owners by birthdate
      var owners = repository.SelectByBirthDate(dateFrom, dateTo);   
      
      // return owners as Dtos
      return Ok(mapper.Map<IEnumerable<OwnerDto>>(owners));
   }
   
   // Convert string in German format dd.MM.yyyy to DateTime
   private (bool, DateTime) ConvertToDateTime(string date) {
      try {
         var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
         return (false, dateTime );
      } catch {
         return (true, DateTime.MinValue);
      }
   }
   
   // Create a new owner
   // http://localhost:5100/banking/owners
   [HttpPost("")]
   public ActionResult<OwnerDto> CreateOwner(
      [FromBody] OwnerDto ownerDto
   ) {
      logger.LogDebug("CreateOwner() ownerDto={ownerDto}", ownerDto.Name);
      
      // map Dto to DomainModel.Entity
      var owner = mapper.Map<OwnerDto, Owner>(ownerDto);
      
      // check if owner with given Id already exists   
      if(repository.FindById(owner.Id) != null) 
         return Conflict("CreateOwner: Owner with the given id already exists");
      
      // add owner to repository
      repository.Add(owner); 
      // save to datastore
      dataContext.SaveAllChanges();
      
      // return created owner as Dto
      string requestPath = null!;
      if(Request == null) requestPath = "http://localhost:5100/banking/owners";
      else                requestPath = Request.Path;
      var uri = new Uri($"{requestPath}/{owner.Id}", UriKind.Relative);
      return Created(uri, mapper.Map<OwnerDto>(owner));     
   }
   
   // Update owner
   // http://localhost:5100/banking/owners/{id}
   [HttpPut("{id:guid}")] 
   public ActionResult<OwnerDto> UpdateOwner(
      [FromRoute] Guid id,
      [FromBody]  OwnerDto updOwnerDto
   ) {
      
      logger.LogDebug("UpdateOwner() id={id} updOwnerDto={updOwnerDto}", id.As8(), updOwnerDto.Name);
      
      // map Dto to DomainModel.Entity
      var updOwner = mapper.Map<OwnerDto, Owner>(updOwnerDto);

      // check if Id in the route and body match
      if(id != updOwner.Id) 
         return BadRequest("UpdateOwner: Id in the route and body do not match.");
      
      // check if owner with given Id exists
      var owner = repository.FindById(id);
      if (owner == null)
         return NotFound("UpdateOwner: Owner with given id not found.");

      // Update person
      owner.Update(updOwner.Name, updOwner.Email);
      
      // save to repository and write to database 
      repository.Update(owner);
      dataContext.SaveAllChanges();

      // return updated owner as Dto
      return Ok(mapper.Map<OwnerDto>(owner));
   }
   
   // Delete owner
   // http://localhost:5100/banking/owners/{id}
   [HttpDelete("{id:guid}")] 
   public ActionResult<Owner> DeleteOwner(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("DeleteOwner {id}", id.As8());
      
      // check if owner with given Id exists
      var owner = repository.FindById(id);
      if (owner == null)
         return NotFound("DeleteOwner: Owner with given id not found.");

      // delete in repository and write to database 
      repository.Remove(owner);
      dataContext.SaveAllChanges();

      // return NoContent
      return NoContent(); 
   }
}