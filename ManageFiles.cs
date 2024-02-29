using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.ComponentModel.Design;

namespace password_manager
{

    public class ManageFiles
    {
        public static void CreateFileIfNotExists(string filePath)
        {
            // Kontrollera om filen redan finns
            if (!File.Exists(filePath))
            {
                // Skapa filen
                using (var stream = File.Create(filePath))
                {
                    // Filen har skapats, du kan initialisera den här om nödvändigt
                    Console.WriteLine($"Filen skapades: {filePath}");
                }
            }
            else
            {
                Console.WriteLine($"Filen finns redan: {filePath}");
            }
        }


        public static void CreateOrOverwriteClientFile(string filePath)
        {
            // Försöker skapa eller skriva över filen
            try
            {
                // Om filen redan finns, kommer detta att skriva över den
                File.WriteAllText(filePath, ""); // Skapar en tom fil eller tömmer den om den redan finns
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skapande av filen '{filePath}': {ex.Message}");
            }

            //Instansierar ett objekt av klassen SecretKey så vi kan använda metoderna där
            var secretKeyHandler = new SecretKey();

            //Vi anropar metoden GenerateSecretKey() och lagrar nyckeln i byte arrayen secretKey
            string secretKey = secretKeyHandler.GenerateSecretKey();

            secretKeyHandler.SaveSecretKeyToFile(filePath, secretKey);
        }




        public static void CreateOrOverwriteServerFile(string filePath)
        {
            // Försöker skapa eller skriva över filen
            try
            {
                // Om filen redan finns, kommer detta att skriva över den
                File.WriteAllText(filePath, ""); // Skapar en tom fil eller tömmer den om den redan finns
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skapande av filen '{filePath}': {ex.Message}");
            }









        }
    }
}