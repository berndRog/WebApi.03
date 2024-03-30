using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Dto;

namespace WebApi.Controllers; 

[Route("banking")]
[ApiController]
public class AccountsController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<AccountsController> logger
) : ControllerBase {
   
   // Get all accounts of a given owner as Dto
   // http://localhost:5100/banking/owners/{ownerId:Guid}/accounts
   [HttpGet("owners/{ownerId:Guid}/accounts")]
   public ActionResult<IEnumerable<AccountDto>> GetAccountsByOwner(
      Guid ownerId
   ) {
      logger.LogDebug("GetAccountsByOwner ownerId={ownerId}", ownerId);
      
      // get all accounts of a given owner
      IEnumerable<Account> accounts = accountsRepository.SelectByOwnerId(ownerId);
      
      // map DomainModel to Dto
      IEnumerable<AccountDto>? accountDtos = 
         mapper.Map<IEnumerable<AccountDto>>(accounts);
      return Ok(accountDtos);  
   }

   // Get account by Id as Dto
   // http://localhost:5100/banking/accounts/{id}
   [HttpGet("accounts/{id}")]
   public ActionResult<AccountDto> GetAccountById(Guid id) {
      logger.LogDebug("GetAccountById id={id}", id);
      
      switch (accountsRepository.FindById(id)) {
         // map DomainModel to Dto      
         case Account account: 
            return Ok(mapper.Map<AccountDto>(account));
         case null:            
            return NotFound($"Account with given Id not found");
      }
   }

   // Get account by IBAN as Dto
   // http://localhost:5100/banking/accounts/iban?iban={iban}
   [HttpGet("accounts/iban")]
   public ActionResult<AccountDto> GetAccountByIban(
      [FromQuery] string iban
   ) {
      logger.LogDebug("GetAccountByIban iban={iban}", iban);
      
      switch (accountsRepository.FindByIban(iban)) {
         case Account account: 
            return Ok(mapper.Map<Account, AccountDto>(account));
         case null:            
            return NotFound($"Account with given Id not found");
      }
   }
   
   // Create a new account for a given owner
   // http://localhost:5100/banking/owners/{ownerId}/accounts
   [HttpPost("owners/{ownerId:Guid}/accounts")]
   public ActionResult<AccountDto> CreateAccount(
      [FromRoute] Guid ownerId,
      [FromBody]  AccountDto accountDto
   ) {
      logger.LogDebug("CreateAccount iban={iban}", accountDto.Iban);
      
      // map Dto to DomainModel
      Account account = mapper.Map<Account>(accountDto);
      
      // check if ownerId exists
      Owner? owner = ownersRepository.FindById(ownerId);
      if (owner == null)
         return BadRequest("Bad request: ownerId does't exists.");

      // check if account with given Id already exists   
      if(accountsRepository.FindById(account.Id) != null) 
         return Conflict($"Account with given Id already exists");
      
      // update owner in DomainModel
      owner.Add(account);
      
      // add account to repository
      accountsRepository.Add(account); 
      // save to datastore
      dataContext.SaveAllChanges();
      
      // return created account as Dto      
      string requestPath = null!;
      if(Request == null) requestPath = $"http://localhost:5100/banking/owners/{ownerId}";
      else                requestPath = Request.Path;
      var uri = new Uri($"{requestPath}/accounts/{account.Id}", UriKind.Relative);
      return Created(uri, mapper.Map<AccountDto>(account));     
   }
   
   // Delete an account for a given owner
   // http://localhost:5100/banking/owners/{ownerId}/accounts
   [HttpDelete("owners/{ownerId:Guid}/accounts/{id}")]
   public IActionResult DeleteAccount(
      [FromRoute] Guid ownerId,
      [FromRoute] Guid id
   ) {
      logger.LogDebug("DeleteAccount ownerId={ownerId} id={id}", ownerId, id);
      
      // check if account with given Id already exists   
      Account? account = accountsRepository.FindById(id); 
      if(account == null)
         return NotFound($"UpdateAccount: Account not found.");

      // save to repository and write to database 
      accountsRepository.Remove(account);
      dataContext.SaveAllChanges();

      // return no content
      return NoContent(); 
   }
}