using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BankAPI.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class BankTransactionController : ControllerBase
{
    private readonly ClientService clientService;
    private readonly AccountService accountService;
    private readonly TransactionTypeService transactionTypeService;
    private readonly BankTransactionService bankTransactionService;

    public BankTransactionController(ClientService clientService,AccountService accountService,TransactionTypeService transactionTypeService,BankTransactionService bankTransactionService)
    {
       this.clientService = clientService; 
       this.accountService = accountService; 
       this.transactionTypeService = transactionTypeService;
       this.bankTransactionService = bankTransactionService;
    }

    [Authorize(Policy = "ClientPolicy")]
    [HttpGet("GetAccounts")]
    public async Task<IEnumerable<AccountDtoOut>> GetAccounts() 
    {
        var client = await GetClientFromToken();
        if(client == null){
            return new List<AccountDtoOut>();
        }
        return await accountService.GetAllFromClient(client.Id);
    }
    [Authorize(Policy = "ClientPolicy")]
    [HttpPost]
    public async Task<IActionResult> Transaction(BankTransactionDto transactionDto) 
    {
        var client = await GetClientFromToken();
        if(client == null){
            return BadRequest(new {message = $"Token invalido."});
        }
        var account = await accountService.GetById(transactionDto.AccountId);
        if(account == null){
            return BadRequest(new {message = $"La cuenta con Id = {transactionDto.AccountId} no existe."});
        }
        if(account.ClientId != client.Id)
        {
            return BadRequest(new {message = $"La cuenta con Id = {transactionDto.AccountId} existe pero no es de este cliente."});
        }
        var transactionType = await transactionTypeService.GetTransactionType(transactionDto.TransactionType);
        if(transactionType == null){
            return BadRequest(new {message = $"El tipo de transacción con Id = {transactionDto.TransactionType} no existe."});
        }
        switch(transactionDto.TransactionType)
        {
            case 1: 
                if(transactionDto.ExternalAccount != null)
                {
                    return BadRequest(new {message = "Los depósitos en efectivo solo pueden realizarse a cuentas internas."});
                }
                account.Balance += transactionDto.Amount;
            break;
            case 2:
                if(transactionDto.ExternalAccount != null)
                {
                    return BadRequest(new {message = "Los retiros en efectivo solo pueden realizarse a cuentas internas."});
                }
                if((account.Balance - transactionDto.Amount)<0) 
                {
                    return BadRequest(new {message = "La cuenta no tiene fondos suficientes para realizar el retiro."});
                }
                account.Balance -= transactionDto.Amount;
            break;
            case 3:
                return BadRequest(new {message = "Por el momento los depósitos vía transferencia estan deshabilitados."});
            break;
            case 4:
                if((account.Balance - transactionDto.Amount)<0) 
                    {
                        return BadRequest(new {message = "La cuenta no tiene fondos suficientes para realizar el retiro."});
                    }
                    account.Balance -= transactionDto.Amount;
            break;
            default:
                            return BadRequest(new {message = "La transacción no se reconoce."});
            break;
        }
        var accountDtoIn = new AccountDtoIn();
        accountDtoIn.Id = account.Id;
        accountDtoIn.Balance = account.Balance;
        accountDtoIn.AccountType = account.AccountType;
        accountDtoIn.ClientId = account.ClientId;
        await accountService.Update(accountDtoIn);
        await bankTransactionService.Create(transactionDto);
        return Ok();
    } 

    [Authorize(Policy = "ClientPolicy")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var account = await accountService.GetById(id);
        var transaction = await bankTransactionService.GetAny(id);
        if(account == null){
            return BadRequest(new {message = $"La cuenta con Id = {id} no existe."});
        }else if(account.Balance != 0)
        {
            return BadRequest(new {message = $"La cuenta no puede ser eliminada porque aún dispone de ${account.Balance}."});
        }else if(transaction is not null) 
        {
            return BadRequest(new {message = "La cuenta no puede ser eliminada porque se realizaron transacciones con ella."});
        }

        await accountService.Delete(id);
        return Ok();
    }

    public async Task<Client> GetClientFromToken()
    {
        var token = await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme,"access_token");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        int id = Convert.ToInt32(jwt.Claims.First(c => c.Type == "ID").Value); 
        
        var client = await clientService.GetById(id);

        return client;
    }
}