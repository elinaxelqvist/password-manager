using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace password_manager
{
    public class VaultKeyGenerator
    {
        //Genererar valvnyckel
        public static byte[] GenerateVaultKey(string masterPassword, string secretKey)
        {
            // Kontrollerar om master password eller secret key är null
            if (masterPassword == null)
            {
                Console.WriteLine("Fel: Master password är null.");
                return null;
            }
            if (secretKey == null)
            {
                Console.WriteLine("Fel: Secret key är null.");
                return null;
            }

            byte[] masterPasswordBytes = Encoding.UTF8.GetBytes(masterPassword);
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            int iterations = 10000;
            int keyLength = 32;
            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(masterPasswordBytes, secretKeyBytes, iterations))
            {
                return deriveBytes.GetBytes(keyLength);
            }
        }
    }
}

