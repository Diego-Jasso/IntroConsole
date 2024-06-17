using BankConsole;

if(args.Length == 0){
    EmailService.SendMail();
}else{
    ShowMenu();
}

void ShowMenu(){
    Console.Clear();
    Console.WriteLine("Selecciona una opción:");
    Console.WriteLine("1 - Crear un Usuario nuevo.");
    Console.WriteLine("2 - Eliminar un Usuario existente.");
    Console.WriteLine("3 - Salir.");

    int option = 0;
    do{
        string input = Console.ReadLine();

        if(!int.TryParse(input, out option)){
            Console.WriteLine("Debes ingresar un número (1, 2 o 3).");
        }else if(option > 3 || option < 1){
            Console.WriteLine("Debes ingresar un número válido (1, 2 o 3).");
        }
    }while(option == 0 || option > 3 || option < 1);

    switch(option){
        case 1:
            CreateUser();
            break;
        case 2: 
            DeleteUser();
            break;
        case 3:
            Environment.Exit(0);
            break;
    }
}

void CreateUser(){
    Console.Clear();
    Console.WriteLine("Ingresa la información del usuario:");

    Console.Write("ID: ");
    int ID = ValidateID(false);
    
    Console.Write("Nombre: ");
    string name = Console.ReadLine(); 

    Console.Write("Email: ");
    string email = ValidateEmail(); 

    Console.Write("Saldo: ");
    decimal balance = ValidateBalance(); 

    Console.Write("Escribe 'c' si el usuario es Cliente, 'e' si es Empleado: ");
    char userType = ValidateUserType();

    User newUser;

    if(userType.Equals('c')){
        Console.Write("Regimen Fiscal: ");
        char TaxRegime = char.Parse(Console.ReadLine());
        newUser = new Client(ID,name,email,balance,TaxRegime);
    }else{
        Console.Write("Departament: ");
        string Departament = Console.ReadLine();
        newUser = new Employee(ID,name,email,balance,Departament);
    }

    Storage.AddUser(newUser);

    Console.WriteLine("Usuario creado.");
    Thread.Sleep(2000);
    ShowMenu();
}

void DeleteUser(){
    Console.Clear();
    Console.Write("Ingresa el ID del usuario a eliminar: ");
    int ID = ValidateID(true);

    string result = Storage.DeleteUser(ID);
    if(result.Equals("Success")){
        Console.Write("Usuario eliminado.");
        Thread.Sleep(2000);
        ShowMenu();
    }
}

int ValidateID(bool delete){
    int ID = 0;
    
    do{
        string input = Console.ReadLine();
        if(!int.TryParse(input, out ID)){
            Console.WriteLine("Debes ingresar un número entero.");
        }else if(ID < 1){
            Console.WriteLine("Debes ingresar un número entero positivo.");
        }else if(!Storage.ExistsID(ID,delete)){
            if(delete){
                Console.WriteLine("El ID ingresado no existe, asegurate que sea correcto.");
            }else{
                Console.WriteLine("El ID ingresado ya existe, debes ingresar un ID diferente.");
            }
            
        }
    }while(ID == 0 || ID < 1 || !Storage.ExistsID(ID,delete));

    return ID;
}

string ValidateEmail(){
    string input = "";
    while(true){
        input = Console.ReadLine();
        try{
           var email = new System.Net.Mail.MailAddress(input);
        }catch (FormatException){
            Console.WriteLine("Debes ingresar una dirección de correo valida.");
            continue;
        }
        break;
    }
    return input;
}

decimal ValidateBalance(){
    decimal balance = 0;
    do{
        string input = Console.ReadLine();
        if(!decimal.TryParse(input, out balance)){
            Console.WriteLine("Debes ingresar un saldo decimal.");
        }else if(balance < 0){
            Console.WriteLine("Debes ingresar un saldo positivo.");}
    }while(balance == 0 || balance <0);

    return balance;
}

char ValidateUserType(){
    char userType;
    bool valid = false;
    do{
        string input = Console.ReadLine();
        if(!char.TryParse(input, out userType)){
            Console.WriteLine("Debes ingresar un solo caracter ('c', 'e').");
        }else if(!userType.Equals('e') && !userType.Equals('c')){
            Console.WriteLine("Debes ingresar un caracter valido ('c', 'e').");
        }else{
            valid = true;
        }
    }while(!valid);

    return userType;
}