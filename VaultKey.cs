﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace password_manager
{
    public class VaultKeyGenerator
    {

        //Metod som genererar en valvnyckel
        public static byte[] GenerateVaultKey(string masterPassword, string secretKey)
        {
            // byte[] secretKeyBytes = Convert.FromBase64String(secretKey); //SecretKey är hela innehållet i client file
            //byte[] masterPasswordBytes = Convert.FromBase64String(masterPassword); //Detta är en klartext

            // Kontrollera om masterPassword eller secretKey är null
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

            int iterations = 10000; // Antal iterationer (kan justeras efter behov)
            int keyLength = 32; // Längden på valvnyckeln i byte (kan justeras efter behov)
            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(masterPasswordBytes, secretKeyBytes, iterations))
            {
                return deriveBytes.GetBytes(keyLength);
            }
        }
    }
}

