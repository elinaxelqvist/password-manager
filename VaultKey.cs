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
        

        public byte[] GenerateVaultKey(string masterPassword, string secretKey)
        {
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] masterPasswordBytes = Encoding.UTF8.GetBytes(masterPassword);

            int iterations = 10000; // Antal iterationer (kan justeras efter behov)
            int keyLength = 32; // Längden på valvnyckeln i byte (kan justeras efter behov)
            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(masterPasswordBytes, secretKeyBytes, iterations))
            {
                return deriveBytes.GetBytes(keyLength);
            }
        }
    }
}

