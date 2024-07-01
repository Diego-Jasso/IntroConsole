using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BankAPI.Data;
using BankAPI.Data.DTOs;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class TransactionTypeService
{
    private readonly BankContext _context;
    public TransactionTypeService(BankContext context)
    {
        _context = context;
    }

    public async Task<TransactionType?> GetTransactionType(int id) 
    {
        return await _context.TransactionTypes.FindAsync(id);
    } 
}