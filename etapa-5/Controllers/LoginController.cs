using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BankAPI.Controller;

[ApiController]
[Route("api/[controller]")]

public class LoginController : ControllerBase
{
    private readonly LoginService loginService;
    private readonly ClientService clientService;

    private IConfiguration config;
    public LoginController(LoginService loginService,ClientService clientService,IConfiguration config)
    {
        this.loginService = loginService;
        this.config = config;
        this.clientService = clientService;
    }

    [HttpPost("admin/authenticate")]
    public async Task<IActionResult> AdminLogin(AdminDto adminDto)
    {
        var admin = await loginService.GetAdmin(adminDto);

        if(admin == null){
            return BadRequest(new {message = "Credenciales invalidas. "});
        }

        string jwtToken = GenerateTokenAdmin(admin);
        return Ok( new {token = jwtToken});
    }

    [HttpPost("client/authenticate")]
    public async Task<IActionResult> ClientLogin(ClientDto clientDto)
    {
        var client = await clientService.GetClient(clientDto);

        if(client == null){
            return BadRequest(new {message = "Credenciales invalidas. "});
        }

        string jwtToken = GenerateTokenClient(client);
        return Ok( new {token = jwtToken});
    }

    private string GenerateTokenAdmin(Administrator admin)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType",admin.AdminType)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value)); 
        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: creds);
        
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }

    private string GenerateTokenClient(Client client)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email, client.Email),
            new Claim("ID",client.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value)); 
        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: creds);
        
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }
}