using System;
using System.Collections.Generic;

// ====== CLASE BASE ======
// POO: Abstraccion - definimos una clase abstracta que representa el concepto general "CuentaBancaria".
public abstract class CuentaBancaria
{
    // POO: Encapsulacion - campo privado 'saldo' con acceso controlado por la propiedad Saldo.
    private decimal saldo;

    // Propiedades: encapsulan los datos y controlan acceso (get publico, set privado).
    public string NumeroCuenta { get; private set; } // Encapsulacion
    public string Cliente { get; private set; }      // Encapsulacion

    // Propiedad Saldo con setter 'protected' para que solo la propia clase y derivadas puedan modificarlo.
    public decimal Saldo
    {
        get { return saldo; }
        protected set { saldo = value; }
    }

    // Constructor (crea instancias de objetos -> uso de Objetos)
    public CuentaBancaria(string numeroCuenta, string cliente, decimal saldoInicial)
    {
        NumeroCuenta = numeroCuenta;
        Cliente = cliente;
        saldo = saldoInicial;
    }

    // Metodo virtual: puede ser sobrescrito por clases hijas -> Polimorfismo
    public virtual void Depositar(decimal monto)
    {
        Saldo += monto;
        Console.WriteLine($"Deposito de {monto:C} realizado. Nuevo saldo: {Saldo:C}");
    }

    // Metodo virtual por defecto; las clases derivadas pueden sobrescribirlo -> Polimorfismo
    public virtual void Retirar(decimal monto)
    {
        if (monto <= Saldo)
        {
            Saldo -= monto;
            Console.WriteLine($"Retiro de {monto:C} realizado. Nuevo saldo: {Saldo:C}");
        }
        else
        {
            Console.WriteLine("Fondos insuficientes.");
        }
    }

    // Metodo abstracto: obliga a clases derivadas a implementar su propia version.
    // POO: Abstraccion + Polimorfismo (implementaciones concretas en subclases).
    public abstract void AplicarInteres();
}

// ====== CLASE DERIVADA: Cuenta de Ahorro ======
// POO: Herencia - CuentaAhorro hereda de CuentaBancaria
public class CuentaAhorro : CuentaBancaria
{
    private decimal tasaInteres;

    public CuentaAhorro(string numeroCuenta, string cliente, decimal saldoInicial, decimal tasaInteres)
        : base(numeroCuenta, cliente, saldoInicial) // Llama al constructor de la clase base
    {
        this.tasaInteres = tasaInteres;
    }

    // POO: Polimorfismo - implementacion especifica de AplicarInteres para cuentas de ahorro
    public override void AplicarInteres()
    {
        decimal interes = Saldo * tasaInteres;
        Depositar(interes); // Reutiliza metodo Depositar (herencia / reutilizacion)
        Console.WriteLine($"Interes aplicado a cuenta de ahorro ({tasaInteres:P}): {interes:C}");
    }
}

// ====== CLASE DERIVADA: Cuenta Corriente ======
// POO: Herencia - CuentaCorriente hereda de CuentaBancaria
public class CuentaCorriente : CuentaBancaria
{
    private decimal sobregiroPermitido;

    public CuentaCorriente(string numeroCuenta, string cliente, decimal saldoInicial, decimal sobregiroPermitido)
        : base(numeroCuenta, cliente, saldoInicial)
    {
        this.sobregiroPermitido = sobregiroPermitido;
    }

    // POO: Sobrescritura (override) - cambia el comportamiento de Retirar para permitir sobregiro
    public override void Retirar(decimal monto)
    {
        if (monto <= Saldo + sobregiroPermitido)
        {
            Saldo -= monto;
            Console.WriteLine($"Retiro de {monto:C} realizado (con sobregiro). Nuevo saldo: {Saldo:C}");
        }
        else
        {
            Console.WriteLine("Limite de sobregiro excedido.");
        }
    }

    // Polimorfismo: la cuenta corriente no aplica interes positivo en este diseño
    public override void AplicarInteres()
    {
        Console.WriteLine("La cuenta corriente no genera intereses.");
    }
}

// ====== CLASE BANCO ======
// POO: Composicion - Banco "tiene" una coleccion (lista) de cuentas.
// Uso de tipo base (CuentaBancaria) para almacenar diferentes tipos de cuentas -> Polimorfismo en la coleccion.
public class Banco
{
    private List<CuentaBancaria> cuentas = new List<CuentaBancaria>();

    // Agregar cuenta -> uso de objetos
    public void AgregarCuenta(CuentaBancaria cuenta)
    {
        cuentas.Add(cuenta);
    }

    // Buscar cuenta por numero (devuelve referencia a la cuenta concreta)
    public CuentaBancaria BuscarCuenta(string numeroCuenta)
    {
        return cuentas.Find(c => c.NumeroCuenta == numeroCuenta);
    }

    // Mostrar todas las cuentas: cada elemento puede ser de distintos tipos (CuentaAhorro, CuentaCorriente)
    // Aqui vemos Polimorfismo: todas son tratadas como CuentaBancaria, pero al invocar metodos se ejecuta la implementacion concreta.
    public void MostrarCuentas()
    {
        Console.WriteLine("\n=== Listado de cuentas ===");
        foreach (var cuenta in cuentas)
        {
            Console.WriteLine($"Cliente: {cuenta.Cliente}, Cuenta: {cuenta.NumeroCuenta}, Saldo: {cuenta.Saldo:C}");
        }
    }

    // ====== NUEVO MÉTODO: Transferencia ======
    // POO: Uso de objetos y polimorfismo, ya que se trabaja con cuentas que pueden ser de distintos tipos.
    public void Transferir(string origen, string destino, decimal monto)
    {
        var cuentaOrigen = BuscarCuenta(origen);
        var cuentaDestino = BuscarCuenta(destino);

        if (cuentaOrigen == null || cuentaDestino == null)
        {
            Console.WriteLine("Una de las cuentas no existe.");
            return;
        }

        // Guardamos saldo inicial para verificar si el retiro fue exitoso
        decimal saldoInicialOrigen = cuentaOrigen.Saldo;
        cuentaOrigen.Retirar(monto); // Polimorfismo: Retirar se comporta distinto segun el tipo de cuenta

        if (cuentaOrigen.Saldo < saldoInicialOrigen) // Significa que se retiró exitosamente
        {
            cuentaDestino.Depositar(monto); // Polimorfismo: Depositar puede ejecutarse de manera distinta
            Console.WriteLine($"Transferencia de {monto:C} realizada con exito.");
        }
        else
        {
            Console.WriteLine("La transferencia no pudo realizarse (fondos insuficientes).");
        }
    }
}

// ====== PROGRAMA PRINCIPAL (INTERFAZ EN CONSOLA) ======
// Aqui se demuestra la creacion de objetos, interaccion con el usuario y llamadas a metodos que usan polimorfismo.
class Program
{
    static void Main(string[] args)
    {
        Banco banco = new Banco();
        bool salir = false;

        while (!salir)
        {
            Console.WriteLine("\n=== MENÚ BANCO ===");
            Console.WriteLine("1. Crear cuenta de ahorro");
            Console.WriteLine("2. Crear cuenta corriente");
            Console.WriteLine("3. Depositar");
            Console.WriteLine("4. Retirar");
            Console.WriteLine("5. Aplicar interes");
            Console.WriteLine("6. Mostrar cuentas");
            Console.WriteLine("7. Transferir dinero"); // Nueva opción
            Console.WriteLine("8. Salir");
            Console.Write("Seleccione una opcion: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    {
                        Console.Write("Numero de cuenta: ");
                        string numAhorro = Console.ReadLine();
                        Console.Write("Nombre del cliente: ");
                        string clienteA = Console.ReadLine();
                        decimal saldoA = LeerDecimal("Saldo inicial: ");
                        decimal tasa = LeerDecimal("Tasa de interes (ej: 0.05 = 5%): ");
                        // POO: Creamos un objeto CuentaAhorro (instancia), que es una subclase de CuentaBancaria
                        banco.AgregarCuenta(new CuentaAhorro(numAhorro, clienteA, saldoA, tasa));
                        Console.WriteLine("Cuenta de ahorro creada con exito.");
                        break;
                    }

                case "2":
                    {
                        Console.Write("Numero de cuenta: ");
                        string numCorr = Console.ReadLine();
                        Console.Write("Nombre del cliente: ");
                        string clienteC = Console.ReadLine();
                        decimal saldoC = LeerDecimal("Saldo inicial: ");
                        decimal sobregiro = LeerDecimal("Sobregiro permitido: ");
                        // POO: Creamos un objeto CuentaCorriente (instancia).
                        banco.AgregarCuenta(new CuentaCorriente(numCorr, clienteC, saldoC, sobregiro));
                        Console.WriteLine("Cuenta corriente creada con exito.");
                        break;
                    }

                case "3":
                    {
                        Console.Write("Número de cuenta: ");
                        string numDep = Console.ReadLine();
                        var cuentaDep = banco.BuscarCuenta(numDep);
                        if (cuentaDep != null)
                        {
                            decimal montoDep = LeerDecimal("Monto a depositar: ");
                            // Polimorfismo: Depositar puede usar la implementacion base o sobrescrita
                            cuentaDep.Depositar(montoDep);
                        }
                        else
                        {
                            Console.WriteLine("Cuenta no encontrada.");
                        }
                        break;
                    }

                case "4":
                    {
                        Console.Write("Número de cuenta: ");
                        string numRet = Console.ReadLine();
                        var cuentaRet = banco.BuscarCuenta(numRet);
                        if (cuentaRet != null)
                        {
                            decimal montoRet = LeerDecimal("Monto a retirar: ");
                            // Polimorfismo: la llamada a Retirar ejecutara la version concreta (ej. CuentaCorriente permite sobregiro)
                            cuentaRet.Retirar(montoRet);
                        }
                        else
                        {
                            Console.WriteLine("Cuenta no encontrada.");
                        }
                        break;
                    }

                case "5":
                    {
                        Console.Write("Número de cuenta: ");
                        string numInt = Console.ReadLine();
                        var cuentaInt = banco.BuscarCuenta(numInt);
                        if (cuentaInt != null)
                        {
                            // Polimorfismo en tiempo de ejecucion: AplicarInteres ejecuta la implementacion de la subclase real.
                            cuentaInt.AplicarInteres();
                        }
                        else
                        {
                            Console.WriteLine("Cuenta no encontrada.");
                        }
                        break;
                    }

                case "6":
                    banco.MostrarCuentas();
                    break;

                case "7": // Nueva funcionalidad: Transferencia
                    {
                        Console.Write("Número de cuenta origen: ");
                        string origen = Console.ReadLine();
                        Console.Write("Número de cuenta destino: ");
                        string destino = Console.ReadLine();
                        decimal monto = LeerDecimal("Monto a transferir: ");
                        // Uso de objetos y polimorfismo al transferir entre diferentes tipos de cuentas
                        banco.Transferir(origen, destino, monto);
                        break;
                    }

                case "8":
                    salir = true;
                    Console.WriteLine("Gracias por usar el sistema bancario.");
                    break;

                default:
                    Console.WriteLine("Opcion invalida. Intente de nuevo.");
                    break;
            }
        }
    }

    // Metodo auxiliar para leer decimales de forma segura desde consola
    private static decimal LeerDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (decimal.TryParse(input, out decimal valor))
            {
                return valor;
            }
            Console.WriteLine("Entrada invalida. Por favor ingrese un numero valido (ej: 1000.50).");
        }
    }
}
