using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;


namespace password_manager
{
    public class SecretKey
    {

        //Genererar Secret Key
        public static string GenerateSecretKey()
        {
            byte[] secretKey = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secretKey);
            }

            string stringSecretKey = Convert.ToBase64String(secretKey);
            return stringSecretKey;
        }


        //Skapar struktur för en ny klient-fil och lagrar secretKey
        public static void SaveSecretKeyToFile(string filePath, string secretKey)
        {
            string jsonContent = $"{{ \"SecretKey\": \"{secretKey}\" }}";

            try
            {
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skrivning till filen '{filePath}': {ex.Message}");
            }
        }
    }
}

