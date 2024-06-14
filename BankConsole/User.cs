namespace BankConsole;

public class User{
    private int ID {get;set;}
    private string Name {get; set;}
    private string Email { get; set; }
    private decimal Balance {get;set;}
    private DateTime RegisterDate { get; set; }

    public User(){
        this.Balance = 1000;
    }

    public User(int ID, string Name, string Email, decimal Balance){
        this.ID = ID;
        this.Name = Name;
        this.Email = Email;
        SetBalance(Balance);
        this.RegisterDate = DateTime.Now;
    }

    public void SetBalance(decimal amount){
        decimal quantity = 0; 
        if(amount <0){
            quantity = 0;
        }else{
            quantity = amount;  
        }
        
        this.Balance += quantity;
    }
    
    public string ShowData(){
        return $"Nombre: {this.Name}, Correo: {this.Email}, Saldo: {this.Balance}, fecha de registro: {this.RegisterDate}."; 
    }
    
    public string ShowData(string initialMessage){
        return $"{initialMessage} - > Nombre: {this.Name}, Correo: {this.Email}, Saldo: {this.Balance}, fecha de registro: {this.RegisterDate}."; 
    }
}
