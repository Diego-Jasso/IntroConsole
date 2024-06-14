using BankConsole;

User diego = new User(1,"Diego","1234@gmail.com",3500);

diego.SetBalance(-30);

Console.WriteLine(diego.ShowData("La info del usuario es"));

User juan = new User(2,"Juan","1234@uanl.edu.mx",1340);

Console.WriteLine(juan.ShowData()); 