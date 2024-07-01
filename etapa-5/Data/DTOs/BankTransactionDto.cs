namespace BankAPI.Data.DTOs;
public class BankTransactionDto
{
    public int AccountId { get; set; }
    public int TransactionType {get;set;}
    public decimal Amount { get; set; }
    public int? ExternalAccount { get; set; }

}