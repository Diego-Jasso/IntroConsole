using System.Runtime.InteropServices;
using System.Transactions;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class BankTransactionService
{
    private readonly BankContext _context;

    public BankTransactionService(BankContext context){
        _context = context;
    }
    public async Task<BankTransactionDto?> GetAny(int id)
    {
        return  await _context.BankTransactions.
        Where(a =>a.AccountId == id).
        Select(a => new BankTransactionDto
        {
            AccountId = a.AccountId,
            ExternalAccount = a.ExternalAccount != null ? a.ExternalAccount: null,
            Amount = a.Amount,
            TransactionType = a.TransactionType
        }).FirstOrDefaultAsync();
    }
    public async Task<BankTransaction> Create(BankTransactionDto newBankTransactionDto)
    {
        var newBankTransaction = new BankTransaction();
        newBankTransaction.AccountId = newBankTransactionDto.AccountId;
        newBankTransaction.Amount = newBankTransactionDto.Amount;
        newBankTransaction.TransactionType = newBankTransactionDto.TransactionType;
        newBankTransaction.ExternalAccount = newBankTransactionDto.ExternalAccount;

        _context.BankTransactions.Add(newBankTransaction);
        await _context.SaveChangesAsync();  

        return newBankTransaction;
    }
}