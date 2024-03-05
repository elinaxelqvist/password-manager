﻿using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.ComponentModel.Design;

namespace password_manager
{
   public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string command = args[0];

                switch (command)
                {
                    case "init":

                        // Kontrollera att vi har rätt antal argument
                        if (args.Length == 3)
                        {
                            // Vi antar att både klient- och serverfilens sökvägar tillhandahålls
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];

                            // Skapar eller skriver över filer direkt
                            ManageFiles.CreateOrOverwriteClientFile(clientFilePath);
                            ManageFiles.CreateOrOverwriteServerFile(serverFilePath);

                            Console.WriteLine($"Klient-filen '{clientFilePath}' och server-filen '{serverFilePath}' har skapats eller skrivits över.");


                            //hämtar secret key och skriver ut i terminalen
                            string secretKey = File.ReadAllText(clientFilePath);

                            Console.WriteLine("Här är din hemliga nyckel. Skriv ned det och kom ihåg det");
                            Console.WriteLine(secretKey);


                            //användaren får skapa master password
                            string masterPassword = MasterPassword();

                            //Skapa valvnyckeln

                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            //Skapa aes, valv, kryptera valvet, spara iv och det krypterade valvet i serverfil

                            Vault.ServerFileStructure(serverFilePath, vaultKey);

                        }
                        else
                        {
                            Console.WriteLine("Fel antal argument. Använd: init <klientfilens sökväg> <serverfilens sökväg>");
                        }
                        break;

                    case "create":
                        if (args.Length == 3)
                        {
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];

                            string masterPassword = MasterPassword();

                            Console.WriteLine("Ange din hemliga nyckel");
                            string secretKey = Console.ReadLine();

                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            try
                            {
                                string serverFileContents = File.ReadAllText(serverFilePath);
                                Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                                string ivValue = serverFileDict["IV"];
                                string encryptedData = serverFileDict["EncryptedVault"];

                                byte[] iv = Convert.FromBase64String(ivValue);
                                Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

                                if (Vault.CanDecryptVault(encryptedData, aes))
                                {
                                    ManageFiles.CreateNewClientFile(clientFilePath, secretKey);
                                    Console.WriteLine("Det har skapats en ny klient-fil med ditt secret-key");
                                }
                                else
                                {
                                    Console.WriteLine("Error message");
                                }
                            }
                            catch (FileNotFoundException)
                            {
                                Console.WriteLine("Serverfilen finns inte.");
                            }
                            catch (JsonException)
                            {
                                Console.WriteLine("Ogiltig JSON i serverfilen.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ett fel uppstod: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Felaktigt antal argument.");
                        }
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


                        // vi måste ändra här, det ska ta emot secret <client> 

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
        }

        public static string MasterPassword()
        {
            while (true) // Loopa tills ett giltigt lösenord har matats in
            {
                Console.WriteLine("Skapa/ange ditt master lösenord med minst 8 tecken.");
                string input = Console.ReadLine();

                if (input.Length >= 8)
                {
                    return input; // Returnera det godkända lösenordet
                }
                else
                {
                    Console.WriteLine("Lösenordet är för kort, försök igen.");
                }
            }
        }
    }
}



/*//Vi anropar metoden SaveSecretKeyToFile och skickar in namnet på klientfilen, user och byte arrayen 
//secretKeyHandler.SaveSecretKeyToFile(clientFilePath, user, secretKey);

//Anropar slumpmässig initieringvektor
byte[] iv = Aes_Kryptering.GenerateRandomIV();

Aes_Kryptering.PrintByteArray(iv);





Console.WriteLine("Det här är din hemliga nyckel. Kom ihåg den: " + secretKey);


VaultKeyGenerator generator = new VaultKeyGenerator(secretKey);
byte[] vaultKey = generator.GenerateVaultKey(masterPassword, secretKey);

Console.WriteLine("Valvnyckel genererad: " + Convert.ToBase64String(vaultKey));



//Användning av ServerFileStructure-metoden för att skapa en serverfilstruktur


string stringIV = Convert.ToBase64String(iv);

Console.WriteLine("Skriv in prop");
string prop = Console.ReadLine();

Console.WriteLine("Skriv lösenordet du vill lagra i valvet:");
string password= Console.ReadLine();



Vault.ServerFileStructure(serverFilePath,stringIV,prop, password);


Console.WriteLine("Skriv in det kommando du vill göra");
string input=Console.ReadLine();*/

//funkar det??



