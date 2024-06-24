using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Data.DTOs;

namespace BankAPI.Controller;

[ApiController]
[Route("[controller]")]
public class AccountController: ControllerBase
{
    private readonly ClientService _clientService;
    private readonly AccountService _service;
    private readonly AccountTypeService _accountTypeService;

    public AccountController(AccountService service,ClientService clientService,AccountTypeService accountTypeService)
    {
        _clientService = clientService;
        _service = service;
        _accountTypeService = accountTypeService;
    }

    [HttpGet]
    public async Task<IEnumerable<Account>> Get() 
    {
       return await _service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetById(int id) 
    {
        var account = await _service.GetById(id);
       
       if (account is null){
        return AccountNotFound(id);
       }
       return account;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccountDTO account)
    {
        string validationResult = await ValidateAccount(account);
        if(!validationResult.Equals("Valid"))
            return BadRequest(new {message = validationResult});
        
        var newAccount = await _service.Create(account);

        return CreatedAtAction(nameof(GetById), new { id = newAccount.Id},newAccount);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,AccountDTO account)
    {
        if(id != account.Id)
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({account.Id}) del cuerpo de la solicitud."});

        var accountToUpdate = _service.GetById(id);
        if(accountToUpdate is not null)
        {
            string validationResult = await ValidateAccount(account);
            if(!validationResult.Equals("Valid"))
                return BadRequest(new {message = validationResult});

            await _service.Update(account);
            return NoContent();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(int id)
    {
        var accountToDelete = _service.GetById(id);
        if(accountToDelete is not null)
        {
            await _service.Delete(id);
            return Ok();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    [ApiExplorerSettings(IgnoreApi=true)]
    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"La cuenta con ID = {id} no existe."});
    }

    [ApiExplorerSettings(IgnoreApi=true)]
    public async Task<string> ValidateAccount(AccountDTO account)
    {
        string result = "Valid";

        var AccountType = await _accountTypeService.GetById(account.AccountType);

        if(AccountType is null)
            result = $"El tipo de cuenta {account.AccountType} no existe.";

        var ClientId = account.ClientId.GetValueOrDefault();
        var client = await _clientService.GetById(ClientId);
        if(client is null)
            result = $"El cliente {ClientId} no existe.";

        return result;
    }
}