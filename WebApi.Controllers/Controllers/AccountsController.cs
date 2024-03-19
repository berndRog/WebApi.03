using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;
using WebApi.Core.Misc;

namespace WebApi.Controllers; 
[Route("banking")]
[ApiController]
public class AccountsController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IDataContext dataContext,
   IMapperBase mapper,
   ILogger<AccountsController> logger
) : ControllerBase {
   
   // Get all accounts of a given owner
   // http://localhost:5010/banking/owners/{ownerId:Guid}/accounts
   [HttpGet("owners/{ownerId:Guid}/accounts")]
   public ActionResult<IEnumerable<Account>> GetAccountsByOwner(
      Guid ownerId
   ) {
      logger.LogDebug("Get");
      return Ok(accountsRepository.Select());  
   }

   // Get account by Id
   // http://localhost:5010/banking/accounts/{id}
   [HttpGet("accounts/{id}")]
   public ActionResult<Account> GetAccountById(Guid id) {
      logger.LogDebug("Get {id}", id);
      switch (accountsRepository.FindById(id)) {
         case Account account: return Ok(account);
         case null:            return NotFound($"Account with given Id not found");
      }
   }

   // Create a new account for a given owner
   // http://localhost:5010/banking/owner/{ownerId}/accounts
   [HttpPost("owners/{ownerId:Guid}/accounts")]
   public ActionResult<AccountDto> CreateAccount(
      [FromRoute] Guid ownerId,
      [FromBody]  AccountDto accountDto
   ) {
      logger.LogDebug("CreateAccount {iban}", accountDto.Iban);
      
      // map AccountDto to Account
      Account account = mapper.Map<Account>(accountDto);
      
      // check if ownerId exists
      var owner = ownersRepository.FindById(ownerId);
      if (owner == null)
         return BadRequest("Bad request: ownerId does't exists.");
   
      // check if ownerId from route matches ownerId in account
      if (account.Id == Guid.Empty) account.Id = ownerId;
      if (account.OwnerId != ownerId)
         return BadRequest("Bad request: ownerId from route does not match ownerId in account.");
      
      // check if account with given Id already exists   
      if(accountsRepository.FindById(account.Id) != null) 
         return BadRequest($"Account with given Id already exists");
      
      // update owner in DomainModel
      owner.Add(account);
      
      // add account to repository
      accountsRepository.Add(account); 
      // save to datastore
      dataContext.SaveAllChanges();
      
      // return created account      
      var uri = new Uri($"{Request.Path}/accounts/{account.Id}", UriKind.Relative);
      accountDto = mapper.Map<AccountDto>(account);
      return Created(uri, accountDto);     
   }

   // Update a new account for a given owner
   // http://localhost:5010/banking/owners/{ownerId}/accounts
   [HttpPut("owners/{ownerId:Guid}/accounts/{id}")]
   public IActionResult UpdateAccount(
      [FromRoute] Guid ownerId,
      [FromRoute] Guid id,
      [FromBody]  Account updAccount
   ) {
      logger.LogDebug("Put {id}", id);

      if(id != updAccount.Id) 
         return BadRequest($"UpdateAccount: Id in the route and body do not match.");
      
      // check if ownerId exists
      if(ownersRepository.FindById(ownerId) == null)
         return NotFound("UpdateAccount: Owner not found.");
      
      // check if account with given Id already exists   
      Account? account = accountsRepository.FindById(id); 
      if(account == null)
         return NotFound($"UpdateAccount: Account not found.");

      // Update person
      account = new Account(updAccount);
      
      // save to repository and write to database 
      accountsRepository.Update(account);
      dataContext.SaveAllChanges();

      return Ok(account); //mapper.Map<PersonDto>(account));
   }
   
   // Delete an account for a given owner
   // http://localhost:5010/banking/owner/{ownerId}/accounts
   [HttpPost("owners/{ownerId:Guid}/accounts/{id}")]
   public IActionResult DeleteAccount(
      [FromRoute] Guid ownerId,
      [FromRoute] Guid id
   ) {
      logger.LogDebug("DeleteAccount {id}", id.As8());
      
      // check if account with given Id already exists   
      Account? account = accountsRepository.FindById(id); 
      if(account == null)
         return NotFound($"UpdateAccount: Account not found.");

      // remove the account from the owners account list is not necessary,
      // there is no datafield in the dabase referencing the account
      // i.e. the foreign key is the ownerId in the account table
      
      // save to repository and write to database 
      accountsRepository.Remove(account);
      dataContext.SaveAllChanges();

      return NoContent(); //mapper.Map<PersonDto>(account));
   }
}