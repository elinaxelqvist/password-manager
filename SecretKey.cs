using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace password_manager
{
    public class SecretKey
    {

        //Metod som genererar en Secret Key
        public static string GenerateSecretKey()
        {
            // Skapa en byte-array för att lagra den hemliga nyckeln
            byte[] secretKey = new byte[32]; // 256 bitar

            // Skapa och använd en instans av RandomNumberGenerator
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // Fyll byte-arrayen med slumpmässiga värden
                rng.GetBytes(secretKey);
            }

            string stringSecretKey = Convert.ToBase64String(secretKey);

            // Returnera den genererade nyckeln
            return stringSecretKey;
        }


        //Metod som skapar strukturen för en ny klient-fil, och lagrar secretKey i den
        public static void SaveSecretKeyToFile(string filePath, string secretKey)
        {
            // Skapa ett dictionary för att lagra key-value-par
            Dictionary<string, string> userSecretKeys = new Dictionary<string, string>();

            // Lägg till eller uppdatera användarens hemliga nyckel
            userSecretKeys["SecretKey"] = secretKey;

            // Konvertera dictionary till JSON-sträng
            string updatedJson = JsonSerializer.Serialize(userSecretKeys, new JsonSerializerOptions { WriteIndented = true });

            // Skriv JSON-strängen till filen
            File.WriteAllText(filePath, updatedJson);
        }

    }
}

