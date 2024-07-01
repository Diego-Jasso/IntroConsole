using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BankAPI.Data;
using BankAPI.Data.DTOs;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class LoginService
{
    private readonly BankContext _context;
    public LoginService(BankContext context)
    {
        _context = context;
    }

    public async Task<Administrator?> GetAdmin(AdminDto admin) 
    {
        return await _context.Administrators.
                SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwt == admin.Pwt);
    } 
}