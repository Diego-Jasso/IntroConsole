using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
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
    public async Task<IEnumerable<AccountDtoOut>> Get() 
    {
       return await _service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDtoOut>> GetById(int id) 
    {
        var account = await _service.GetDtoById(id);
       
       if (account is null){
        return AccountNotFound(id);
       }
       return account;
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public async Task<IActionResult> Create(AccountDtoIn account)
    {
        string validationResult = await ValidateAccount(account);
        if(!validationResult.Equals("Valid"))
            return BadRequest(new {message = validationResult});
        
        var newAccount = await _service.Create(account);

        return CreatedAtAction(nameof(GetById), new { id = newAccount.Id},newAccount);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,AccountDtoIn account)
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

    [Authorize(Policy = "AdminPolicy")]
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
    public async Task<string> ValidateAccount(AccountDtoIn account)
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