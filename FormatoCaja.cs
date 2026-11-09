/*
 * FORMATO DEL FICHERO .caja  (offsets en bytes)
 *
 *  Offset  Tamano  Contenido
 *  0       4       Magic "CAJA"
 *  4       1       Version (1)
 *  5       16      SAL (PBKDF2)
 *  21      12      NONCE (AES-GCM)
 *  33      16      TAG de autenticacion
 *  49      N       TEXTO CIFRADO (hasta el final del fichero)
 *
 *  Los bytes 0..20 (magic + version + sal) se autentican como AAD.
 */

using System.Text;

namespace CajaFuerte_Codina_Perpinya;

internal static class FormatoCaja
{
    public static readonly byte[] Magic = Encoding.ASCII.GetBytes("CAJA");
    public const byte Version = 1;

    public const int SaltSize = 16;
    public const int NonceSize = 12;
    public const int TagSize = 16;
    public const int KeySize = 32;          // 32 bytes = AES-256
    public const int Iterations = 600_000;

    public const int OffSalt = 5;
    public const int OffNonce = OffSalt + SaltSize;     // 21
    public const int OffTag = OffNonce + NonceSize;     // 33
    public const int OffCipher = OffTag + TagSize;      // 49
    public const int HeaderSize = OffNonce;             // 21

    // Cabecera autenticada: magic + version + sal.
    public static byte[] ConstruirCabecera(byte[] salt)
    {
        byte[] h = new byte[HeaderSize];
        Magic.CopyTo(h, 0);
        h[4] = Version;
        salt.CopyTo(h, OffSalt);
        return h;
    }

    // Devuelve null si el fichero es valido, o el mensaje de error.
    public static string? Validar(byte[] datos)
    {
        if (datos.Length < OffCipher)
            return "fichero demasiado corto, no es un .caja valido.";
        if (!datos.AsSpan(0, 4).SequenceEqual(Magic))
            return "no es un fichero CajaFuerte (cabecera incorrecta).";
        if (datos[4] != Version)
            return $"version de formato no soportada ({datos[4]}).";
        return null;
    }
}
