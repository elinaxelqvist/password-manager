using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

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

            //Användaren får skriva in sitt användarnamn
            Console.WriteLine("Skriv ditt användarnamn");
            string user = Console.ReadLine();


            //Instansierar ett objekt av klassen SecretKey så vi kan använda metoderna där

            var secretKeyHandler = new SecretKey();

            //Vi anropar metoden GenerateSecretKey() och lagrar nyckeln i byte arrayen secretKey
            byte[] secretKey = secretKeyHandler.GenerateSecretKey();


            


            //Vi anropar metoden SaveSecretKeyToFile och skickar in namnet på klientfilen, user och byte arrayen 
            secretKeyHandler.SaveSecretKeyToFile(clientFilePath, user, secretKey);

            //Anropar slumpmässig initieringvektor
            byte[] iv = Aes_Kryptering.GenerateRandomIV();

            Aes_Kryptering.PrintByteArray(iv);



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

    
