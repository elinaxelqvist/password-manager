using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace password_manager
{
    public class SecretKey
    {
        public string GenerateSecretKey()
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





        public void SaveSecretKeyToFile(string filePath, string secretKey)
        {
            Dictionary<string, string> userSecretKeys = LoadUserSecrets(filePath);

            string key = "Secret Key";

            // Lägg till eller uppdatera användarens hemliga nyckel

                userSecretKeys.Add(key, secretKey);

                // Konvertera dictionary till JSON-sträng
                string updatedJson = JsonSerializer.Serialize(userSecretKeys, new JsonSerializerOptions { WriteIndented = true });

                // Skriv JSON-strängen till filen
                File.WriteAllText(filePath, updatedJson);
        }





        private Dictionary<string, string> LoadUserSecrets(string filePath)
        {
            // Kontrollera om filen finns
            if (File.Exists(filePath))
            {
                // Läs in befintliga användare och deras hemliga nycklar från filen
                string existingJson = File.ReadAllText(filePath);

                // Kontrollera om JSON-strängen är tom
                if (!string.IsNullOrEmpty(existingJson))
                {
                    return JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson) ?? new Dictionary<string, string>();
                }
            }

            // Returnera en ny dictionary om filen inte finns eller är tom
            return new Dictionary<string, string>();
        }
    }

}

