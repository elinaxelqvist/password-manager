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
            byte[] secretKey = new byte[32]; // 256 bitar //Är ett helt JSON objekt, med måsvingar etc. Det är ingen byte array. Vi vill bara åt värdet av secretKey.

            // Skapa och använd en instans av RandomNumberGenerator
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // Fyll byte-arrayen med slumpmässiga värden
                rng.GetBytes(secretKey);
            }

            string stringSecretKey = Convert.ToBase64String(secretKey); //Denna är redan en sträng, kan ej konverteras till en sträng.. Base64 är ingen kryptering.

            // Returnera den genererade nyckeln
            return stringSecretKey;
        }


        //Metod som skapar strukturen för en ny klient-fil, och lagrar secretKey i den
        public static void SaveSecretKeyToFile(string filePath, string secretKey)
        {
            // Skapa en sträng med JSON-formatet manuellt
            string jsonContent = $"{{ \"SecretKey\": \"{secretKey}\" }}";

            try
            {
                // Skriv JSON-strängen till filen
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skrivning till filen '{filePath}': {ex.Message}");
            }
        }
    }
}

