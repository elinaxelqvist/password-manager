using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.Json;

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

        public void SaveSecretKeyToFile(string filePath, string user, byte[] secretKey)
        {
            Dictionary<string, string> userSecretKeys = new Dictionary<string, string>();

            // Läs in befintliga JSON-data från filen om den finns
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                userSecretKeys = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson);
            }

            // Lägg till eller uppdatera användarens hemliga nyckel
            if (userSecretKeys.ContainsKey(user))
            {
                Console.WriteLine($"Användaren '{user}' finns redan i filen. Hemlig nyckel kommer inte att uppdateras.");
            }
            else
            {
                userSecretKeys.Add(user, Convert.ToBase64String(secretKey));

                // Konvertera dictionary till JSON-sträng
                string updatedJson = JsonSerializer.Serialize(userSecretKeys, new JsonSerializerOptions { WriteIndented = true });

                // Skriv JSON-strängen till filen
                File.WriteAllText(filePath, updatedJson);

                Console.WriteLine($"Secret key för användare '{user}' har sparats till filen: {filePath}");
            }
        }
    }



    //   // Konvertera nyckeln till en sträng
    //   var secretKeyString = Convert.ToBase64String(secretKey);

    //    Console.WriteLine(secretKeyString);

    //    var secretKeyObject = new { User = user, SecretKey = secretKey };

    //    var json = JsonSerializer.Serialize(secretKeyObject, new JsonSerializerOptions { WriteIndented = true });

    //    //Skriv nyckeln till filen
    //    File.WriteAllText(filePath, json);
}








//        public void SaveSecretKeyToFile(string filePath, string user, byte[] secretKey)
//        {
//            // Läs in befintliga JSON-data från filen
//            List<UserSecretKey> userSecretKeys = new List<UserSecretKey>();
//            if (File.Exists(filePath))
//            {
//                string existingJson = File.ReadAllText(filePath);
//                userSecretKeys = JsonSerializer.Deserialize<List<UserSecretKey>>(existingJson);
//            }

//            // Lägg till den nya användaren till listan
//            userSecretKeys.Add(new UserSecretKey { User = user, SecretKey = Convert.ToBase64String(secretKey) });

//            // Konvertera listan till en JSON-sträng
//            string updatedJson = JsonSerializer.Serialize(userSecretKeys, new JsonSerializerOptions { WriteIndented = true });

//            // Skriv den uppdaterade JSON-strängen till filen
//            File.WriteAllText(filePath, updatedJson);

//            Console.WriteLine($"Secret key för användare '{user}' har sparats till filen: {filePath}");
//        }
//    }

//    public class UserSecretKey
//    {
//        public string User { get; set; }
//        public string SecretKey { get; set; }
//    }
//}


