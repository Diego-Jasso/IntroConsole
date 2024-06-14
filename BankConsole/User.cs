using Newtonsoft.Json;

namespace BankConsole;

public class User: Person{
    [JsonProperty]
    protected int ID {get;set;}
    [JsonProperty]
    protected string Name {get; set;}
    [JsonProperty]
    protected string Email { get; set; }
    [JsonProperty]
    protected decimal Balance {get;set;}
    [JsonProperty]
    protected DateTime RegisterDate { get; set; }

    public User(int ID, string Name, string Email, decimal Balance){
        this.ID = ID;
        this.Name = Name;
        this.Email = Email;
        SetBalance(Balance);
        this.RegisterDate = DateTime.Now;
    }

    public virtual void SetBalance(decimal amount){
        decimal quantity = 0; 
        if(amount <0){
            quantity = 0;
        }else{
            quantity = amount;  
        }
        
        this.Balance += quantity;
    }
    
    public virtual string ShowData(){
        return $"Nombre: {this.Name}, Correo: {this.Email}, Saldo: {this.Balance}, fecha de registro: {this.RegisterDate.ToShortDateString()}"; 
    }
    
    public string ShowData(string initialMessage){
        return $"{initialMessage} - > Nombre: {this.Name}, Correo: {this.Email}, Saldo: {this.Balance}, fecha de registro: {this.RegisterDate}."; 
    }

    public override string GetName()
    {
        return Name;
    }
}
