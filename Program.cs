//Como su nombre lo indica es la función que muestra el menú y devuelve el valor seleccionado
int Menu(){
    bool valid = false;
    int opcion = 0;
    do {
        Console.Clear();
        Console.WriteLine("------------------------Banco CDIS------------------------");
        Console.WriteLine("1.Ingresar la cantidad de retiros hechos por los usuarios.");
        Console.WriteLine("2.Revisar la cantidad entregada de billetes y monedas.");
        Console.WriteLine("3.Salir.\n");
        Console.Write("Ingresa la opción: ");
        if (int.TryParse(Console.ReadLine(), out opcion))
        {
            if(opcion == 1 || opcion == 2 || opcion == 3){
                valid = true;
            }
        }
        if(!valid){
            Console.WriteLine("La opción seleccionada es incorrecta o no existe, presione cualquier tecla para volver a intentarlo...");
            Console.ReadKey(false);
            Console.Clear();
        }
    }while(!valid);
    return opcion;
}

//Esta función valida y registra los retiros
int[] IngresarRetiros(){
    int retiros=0;
    int cantidadRetiro =0;
    bool valid = false;
    Console.Clear();
    Console.WriteLine("¿Cuántos retiros se hicieron (máximo 10)?");
    if (int.TryParse(Console.ReadLine(), out retiros)){
        if (retiros <= 0 || retiros > 10)
        {
            Console.WriteLine("La cantidad de retiros debe ser minimo 1 y maximo 10, presione cualquier tecla para volver al menú...");
            Console.ReadKey(false);
            Console.Clear();
            return [];
        }
    }
    int[] montos = new int[retiros];
    for(int i =0; i< retiros;i++){
        valid = false;
        do{
            Console.WriteLine($"Ingresa la cantidad del retiro #{i+1}");
            if (int.TryParse(Console.ReadLine(), out cantidadRetiro)){
                if (cantidadRetiro > 0 && cantidadRetiro <= 50000){
                    montos[i]= cantidadRetiro;
                    valid = true;
                }
            }
            if(!valid){
                Console.WriteLine("La cantidad debe ser un entero mayor a 0 y con un maximo de 50,000.");
            }
        }while(!valid);
    }
    return montos;
}

//Esta función es la opcion 2 del menú, la cual imprime los billetes y monedas entregadas por retiro
void ConsultarRetiros(int[] retiros){
    Console.Clear();
    if(retiros.Length==0){
        Console.WriteLine("No hay retiros registrados\n");
    }
    for(int i = 0; i<retiros.Length;i++){
        Console.WriteLine($"Retiro #{i+1}:");
        int[] entregados = CuentaBilletesYMonedas(retiros[i]);
        Console.WriteLine($"Billetes entregados: {entregados[0]}");
        Console.WriteLine($"Monedas entregadas: {entregados[1]}\n");
    }
    Console.WriteLine("Presiona 'enter' para continuar...");
    while (Console.ReadKey().Key != ConsoleKey.Enter) { }
}

//Esta funcion se encarga de contar las monedas y billetes entregadas según el monto retirado
int[] CuentaBilletesYMonedas(int monto){
    int billetesAEntregar = 0;
    int monedasAEntregar = 0;
    do{
        int[] billetes = [500,200,100,50,20];
        int[] monedas = [10,5,1];
        foreach(int billete in billetes){
            int restante;
            int cociente = Math.DivRem(monto, billete, out restante);
            if(cociente > 0){
                billetesAEntregar += cociente;
                monto -= billete*cociente;
            }
        }
        foreach(int moneda in monedas){
            int restante;
            int cociente = Math.DivRem(monto, moneda, out restante);
            if(cociente > 0){
                monedasAEntregar += cociente;
                monto -= moneda*cociente;
            }
        }
    }while(monto !=0);
    return [billetesAEntregar,monedasAEntregar];
}


//Este es el loop principal del programa
bool salir = false;
int[] retiros = [];
do{
    int seleccion = Menu();
    
    if (seleccion == 1)
    {
        retiros = IngresarRetiros();
    }
    else if (seleccion == 2)
    {
        ConsultarRetiros(retiros);
    }
    else if (seleccion == 3)
    {
        Console.Clear();
        Console.WriteLine("Ha salido de la aplicación.");
        salir=true;
    }
} while (!salir);
