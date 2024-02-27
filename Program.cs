using System;
using System.IO;
using System.Text.Json;

namespace password_manager
{
    class Program
    {


        static void Main(string[] args)
        {
            // Kontrollera att användaren har angett rätt antal argument
            if (args.Length < 2)
            {
                Console.WriteLine("Användning: <program> <clientfilväg> <serverfilväg>");
                return;
            }



            // Lagra filvägar baserat på de angivna argumenten
            string clientFilePath = args[0];
            string serverFilePath = args[1];

            // Skapa filerna
            CreateFileIfNotExists(clientFilePath);
            CreateFileIfNotExists(serverFilePath);


            //

            var secretKeyHandler = new SecretKey();
            byte[] secretKey = secretKeyHandler.GenerateSecretKey();

            secretKeyHandler.SaveSecretKeyToFile(clientFilePath, secretKey);





        }

        static void CreateFileIfNotExists(string filePath)
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
    }
}

    
