namespace BankConsole;

public class Client : User
{
    public char Taxregime {get;set;}
    public Client(int ID, string Name, string Email, decimal Balance,char Taxregime) : base(ID, Name, Email, Balance)
    {
        this.Taxregime = Taxregime;
    }

    public override void SetBalance(decimal amount)
    {
        base.SetBalance(amount);

        if(Taxregime.Equals('M')){
            Balance += (amount * 0.02m);
        }
    }

    public override string ShowData()
    {
        return base.ShowData() + $", Regimen Fiscal: {this.Taxregime}";
    }
}