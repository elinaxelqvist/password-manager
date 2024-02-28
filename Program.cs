﻿using System;
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
            string secretKey = secretKeyHandler.GenerateSecretKey();


            


            //Vi anropar metoden SaveSecretKeyToFile och skickar in namnet på klientfilen, user och byte arrayen 
            secretKeyHandler.SaveSecretKeyToFile(clientFilePath, user, secretKey);

            //Anropar slumpmässig initieringvektor
            byte[] iv = Aes_Kryptering.GenerateRandomIV();

            Aes_Kryptering.PrintByteArray(iv);


            Console.WriteLine("Skapa ett lösenord");
            string masterPassword = Console.ReadLine();

            


            Console.WriteLine("Det här är din hemliga nyckel. Kom ihåg den: " + secretKey);

            



            VaultKeyGenerator generator = new VaultKeyGenerator(secretKey);
            byte[] vaultKey = generator.GenerateVaultKey(masterPassword, secretKey);

            Console.WriteLine("Valvnyckel genererad: " + Convert.ToBase64String(vaultKey));

            // Användning av ServerFileStructure-metoden för att skapa en serverfilstruktur


            string stringIV = Convert.ToBase64String(iv);

            Console.WriteLine("Skriv in prop");
            string prop = Console.ReadLine();

            Console.WriteLine("Skriv lösenordet du vill lagra i valvet:");
            string password= Console.ReadLine();

            

           Vault.ServerFileStructure(serverFilePath,stringIV,prop, password);


            Console.WriteLine("Skriv in det kommando du vill göra");
            string input=Console.ReadLine();


            Command.CommandType(input);
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

    
