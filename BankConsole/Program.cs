using BankConsole;

Client diego = new Client(1,"Diego","1234@gmail.com",3500,'M');

diego.SetBalance(3900);

Console.WriteLine(diego.ShowData());

Employee pedro = new Employee(2,"Juan","12354@gmail.com",3000,"IT");

pedro.SetBalance(700);

Console.WriteLine(pedro.ShowData());
