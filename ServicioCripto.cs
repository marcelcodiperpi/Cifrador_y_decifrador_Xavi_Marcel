using System.Security.Cryptography;

namespace CajaFuerte_Codina_Perpinya;

internal static class ServicioCripto
{
    // Recibe el contenido original y devuelve el fichero .caja completo (en bytes).
    public static byte[] Cifrar(byte[] plain, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(FormatoCaja.SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(FormatoCaja.NonceSize);
        byte[] key = DerivarClave(password, salt);

        byte[] cipher = new byte[plain.Length];
        byte[] tag = new byte[FormatoCaja.TagSize];
        byte[] cabecera = FormatoCaja.ConstruirCabecera(salt);

        try
        {
            using var aes = new AesGcm(key, FormatoCaja.TagSize);
            aes.Encrypt(nonce, plain, cipher, tag, cabecera);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }

        // Montar: cabecera + nonce + tag + cifrado
        byte[] resultado = new byte[FormatoCaja.OffCipher + cipher.Length];
        cabecera.CopyTo(resultado, 0);
        nonce.CopyTo(resultado, FormatoCaja.OffNonce);
        tag.CopyTo(resultado, FormatoCaja.OffTag);
        cipher.CopyTo(resultado, FormatoCaja.OffCipher);
        return resultado;
    }

    // Recibe un .caja ya validado. Lanza CryptographicException si el tag no coincide.
    public static byte[] Descifrar(byte[] datos, string password)
    {
        byte[] salt = datos[FormatoCaja.OffSalt..FormatoCaja.OffNonce];
        byte[] nonce = datos[FormatoCaja.OffNonce..FormatoCaja.OffTag];
        byte[] tag = datos[FormatoCaja.OffTag..FormatoCaja.OffCipher];
        byte[] cipher = datos[FormatoCaja.OffCipher..];
        byte[] cabecera = datos[..FormatoCaja.HeaderSize];

        byte[] key = DerivarClave(password, salt);
        byte[] plain = new byte[cipher.Length];

        try
        {
            using var aes = new AesGcm(key, FormatoCaja.TagSize);
            aes.Decrypt(nonce, cipher, tag, plain, cabecera);   // verifica el tag
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }

        return plain;
    }

    // PBKDF2-HMAC-SHA256, 600.000 iteraciones, 32 bytes.
    private static byte[] DerivarClave(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, FormatoCaja.Iterations,
                                  HashAlgorithmName.SHA256, FormatoCaja.KeySize);
}
