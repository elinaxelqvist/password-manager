using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.ComponentModel.Design;

namespace password_manager
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string command = args[0];

                switch (command)
                {
                    case "init":

                        //kod som anopas om första ordet är init

                        if (args.Length == 2)
                        {
                            if (!File.Exists(args[1]) || !File.Exists(args[2]))
                            {

                            }
                            else if (File.Exists(args[1]))
                            {

                            }
                            else if (!File.Exists(args[2]))
                            {

                            }
                        }



                        



                        break;

                    case "create":

                        //kod som anropas om första ordet är create

                        break;

                    case "get":
                        //kod som anropas om första ordet är get
                        break;

                    case "set":

                        //kod som anropas om första ordet är set

                        break;

                    case "delete":

                        //kod som anropas om första ordet är delete

                        break;

                    case "secret":

                        //kod som anropas om första ordet är secret
                        Command.SecretCommand(args);
                        break;

                    default:
                        Console.WriteLine("Ogiltigt kommande");
                        break;
                }
            }


            else
            {
                Console.WriteLine("Inget kommando angivet");
            }
                
            
            
            
            
            
            
            
            // Kontrollera att användaren har angett rätt antal argument
            //if (args.Length < 2)
            //{
               // Console.WriteLine("Användning: <program> <clientfilväg> <serverfilväg>");
               // return;
           // }



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

    
