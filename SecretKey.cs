using System;
using System.IO;
using System.Security.Cryptography;

namespace password_manager
{
    public class SecretKey
    {
        // Metod för att generera och returnera en hemlig nyckel
        public byte[] GenerateSecretKey()
        {
            // Skapa en byte-array för att lagra den hemliga nyckeln
            byte[] secretKey = new byte[32]; // 256 bitar

            // Skapa och använd en instans av RandomNumberGenerator
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // Fyll byte-arrayen med slumpmässiga värden
                rng.GetBytes(secretKey);
            }

            // Returnera den genererade nyckeln
            return secretKey;
        }

        public void SaveSecretKeyToFile(string filePath, byte[] secretKey)
        {
            // Konvertera nyckeln till en sträng
            var secretKeyString = Convert.ToBase64String(secretKey);

            Console.WriteLine(secretKeyString);

            // Skriv nyckeln till filen
            File.WriteAllText(filePath, secretKeyString);
        }
    }
}