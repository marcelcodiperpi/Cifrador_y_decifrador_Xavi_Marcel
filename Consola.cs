using System.Text;

namespace CajaFuerte_Codina_Perpinya;

internal static class Consola
{
    // Lee la contrasena sin mostrarla en pantalla.
    public static string LeerPassword(string mensaje)
    {
        Console.Write(mensaje);
        var sb = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo k = Console.ReadKey(intercept: true);
            if (k.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
            if (k.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0) sb.Length--;
            }
            else if (!char.IsControl(k.KeyChar))
            {
                sb.Append(k.KeyChar);
            }
        }
        return sb.ToString();
    }
}
