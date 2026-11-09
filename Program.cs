namespace CajaFuerte_Codina_Perpinya;

internal static class Program
{
    private static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== CajaFuerte ===");
            Console.WriteLine("1) Cifrar un fichero .txt");
            Console.WriteLine("2) Descifrar un fichero .caja");
            Console.WriteLine("3) Salir");
            Console.Write("Opcion: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": Operaciones.Cifrar(); break;
                case "2": Operaciones.Descifrar(); break;
                case "3": return;
                default: Console.WriteLine("Opcion no valida."); break;
            }
        }
    }
}
