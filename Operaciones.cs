using System.Security.Cryptography;

namespace CajaFuerte_Codina_Perpinya;

internal static class Operaciones
{
    public static void Cifrar()
    {
        Console.Write("Ruta del .txt a cifrar: ");
        string? ruta = Console.ReadLine()?.Trim().Trim('"');

        if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta))
        { Console.WriteLine("ERROR: el fichero no existe."); return; }
        if (!ruta.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        { Console.WriteLine("ERROR: el fichero de entrada debe ser .txt."); return; }

        string pass1 = Consola.LeerPassword("Contrasena: ");
        string pass2 = Consola.LeerPassword("Repite la contrasena: ");
        if (pass1 != pass2 || pass1.Length == 0)
        { Console.WriteLine("ERROR: las contrasenas no coinciden o estan vacias."); return; }

        byte[] plain = File.ReadAllBytes(ruta);          // bytes, no string
        byte[] fichero = ServicioCripto.Cifrar(plain, pass1);

        string salida = Path.ChangeExtension(ruta, ".caja");
        File.WriteAllBytes(salida, fichero);

        Console.WriteLine($"OK: fichero cifrado en '{salida}' ({fichero.Length} bytes).");
    }

    public static void Descifrar()
    {
        Console.Write("Ruta del .caja a descifrar: ");
        string? ruta = Console.ReadLine()?.Trim().Trim('"');

        if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta))
        { Console.WriteLine("ERROR: el fichero no existe."); return; }

        byte[] datos = File.ReadAllBytes(ruta);

        string? error = FormatoCaja.Validar(datos);
        if (error != null)
        { Console.WriteLine("ERROR: " + error); return; }

        string pass = Consola.LeerPassword("Contrasena: ");

        byte[] plain;
        try
        {
            plain = ServicioCripto.Descifrar(datos, pass);
        }
        catch (CryptographicException)
        {
            // GCM no distingue contrasena mala de fichero alterado.
            Console.WriteLine("ERROR DE INTEGRIDAD: contrasena incorrecta o fichero manipulado/corrupto.");
            Console.WriteLine("No se ha generado ningun fichero de salida.");
            return;
        }

        string dir = Path.GetDirectoryName(Path.GetFullPath(ruta))!;
        string nombre = Path.GetFileNameWithoutExtension(ruta);
        string salida = Path.Combine(dir, nombre + "_descifrado.txt");

        File.WriteAllBytes(salida, plain);
        Console.WriteLine($"OK: integridad verificada. Fichero descifrado en '{salida}'.");
    }
}
