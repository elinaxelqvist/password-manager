using System;
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
                            string clientFile = File.ReadAllText(clientFilePath); //Är bara en sträng.... inte en klass.



                            Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile); //Deserialiserar innehållet 

                            

                            string secretKey = clientFileDict["SecretKey"];         //Vi hämtar endast ut value för SecretKey
                             
                           
                           
                            Console.WriteLine("Här är din hemliga nyckel. Skriv ned den och kom ihåg den");
                            Console.WriteLine(secretKey);


                            //användaren får skapa master password
                            string masterPassword = MasterPassword();

                            //Skapa valvnyckeln

                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey); //Detta är hela filen, inte själva värdet på nyckeln

                            //Skapa aes, valv och kryptera valvet. Spara iv och det krypterade valvet i serverfil

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

                            //Lagrar argumenten för klientfil och serverfil 
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];


                            // Användaren får ange sitt master password
                            string masterPassword = MasterPassword();


                            //Användaren får ange sin hemliga nyckel 
                            Console.WriteLine("Ange din hemliga nyckel");
                            string secretKey = Console.ReadLine();

                            //Metoden som genererar valvnyckeln anropas
                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            try
                            { 
                                //Vi hämtar innehållet som finns i serverfilen och deserialiserar det
                                string serverFileContents = File.ReadAllText(serverFilePath);
                                Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                                //Hämtar ut värdena som finns för IV och EncryptedVault i serverfilen, och lagrar dessa 
                                string ivValue = serverFileDict["IV"];
                                string encryptedData = serverFileDict["EncryptedVault"];

                                //Omvandlar iv till en byte[] (för att CreateAesObject tar emot iv som en byte[])
                                byte[] iv = Convert.FromBase64String(ivValue);

                                //Skapar ett aes-objekt utifrån vaultKey och iv
                                Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

                                
                                if (Vault.CanDecryptVault(encryptedData, aes))      

                                {
                                    //Om valvet kunde krypteras med aes-objektet, skapas en ny klientfil där secretKey lagras

                                    ManageFiles.CreateNewClientFile(clientFilePath, secretKey);
                                    Console.WriteLine("Det har skapats en ny klient-fil med din secret key");

                                }
                                else
                                {
                                    Console.WriteLine("Dekrypteringen misslyckades.");
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

                        if(args.Length==3 || args.Length == 4)
                        {
                            string clientFilePath = args[1];
                            string serverFilePath= args[2];
                            string propertyKey= args[3];

                            // Användaren får ange masterlösenord
                            string masterPassword = MasterPassword();


                            Console.WriteLine(masterPassword);



                            //hämtar secret key 
                            string clientFile = File.ReadAllText(clientFilePath); //Är bara en sträng.... inte en klass.



                            Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile); //Deserialiserar innehållet 



                            string secretKey = clientFileDict["SecretKey"];         //Vi hämtar endast ut value för SecretKey


                            // Generera valvnyckel
                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            // Läs in initieringsvektorn från serverfilen
                            string serverFileContents = File.ReadAllText(serverFilePath);
                            Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                            string ivValue = serverFileDict["IV"];
                            string encryptedData = serverFileDict["EncryptedVault"];

                            // Console.WriteLine(ivValue);
                            // Console.WriteLine(encryptedData);



                            byte[] iv = Convert.FromBase64String(ivValue);

                            // Skapa ett AES-objekt
                            Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

                            if (Vault.CanDecryptVault(encryptedData, aes))
                            {
                                // Dekryptera det befintliga valvet från serverfilen
                                string decryptedVault = Vault.DecryptVault(encryptedData, aes);
                                Dictionary<string, string> vaultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);


                                if(propertyKey == null)
                                {
                                    // Anropa funktionen för att hämta alla propertyKeys
                                    List<string> propertyKeys = GetAllPropertyKeys(vaultDict);

                                    // Skriv ut alla propertyKeys till konsolen
                                    Console.WriteLine("Alla propertyKeys i vaultDict:");
                                    foreach (string key in propertyKeys)
                                    {
                                        Console.WriteLine(key);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("hej");
                                }




                            }
                            else
                            {
                                Console.WriteLine("Dekrypteringen misslyckades");
                            }



                        }

                        break;


                    case "set":

                        if (args.Length == 4 || args.Length == 5)
                        {
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];
                            string propertyKey = args[3];

                            bool generatePassword = args.Length == 5 && (args[4] == "-g" || args[4] == "--generate");


                            if (propertyKey != null)
                            {
                                try
                                {
                                    // Användaren får ange masterlösenord
                                    string masterPassword = MasterPassword();


                                    Console.WriteLine(masterPassword);



                                    //hämtar secret key 
                                    string clientFile = File.ReadAllText(clientFilePath); //Är bara en sträng.... inte en klass.



                                    Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile); //Deserialiserar innehållet 



                                    string secretKey = clientFileDict["SecretKey"];         //Vi hämtar endast ut value för SecretKey

                                    

                                   // Console.WriteLine(secretKey);

                                    // Generera valvnyckel
                                    byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                                    // Läs in initieringsvektorn från serverfilen
                                    string serverFileContents = File.ReadAllText(serverFilePath);
                                    Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                                    string ivValue = serverFileDict["IV"];
                                    string encryptedData = serverFileDict["EncryptedVault"];

                                   // Console.WriteLine(ivValue);
                                   // Console.WriteLine(encryptedData);


                                    
                                    byte[] iv=Convert.FromBase64String(ivValue);

                                    // Skapa ett AES-objekt
                                    Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);


                                    if (Vault.CanDecryptVault(encryptedData, aes))

                                    {
                                        // Dekryptera det befintliga valvet från serverfilen
                                        string decryptedVault = Vault.DecryptVault(encryptedData, aes);
                                        Dictionary<string, string> vaultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);

                                        // Läs det befintliga lösenordet från det dekrypterade valvet
                                        string existingPassword = vaultDict.ContainsKey(propertyKey) ? vaultDict[propertyKey] : null;

                                        // Generera ett nytt lösenord antingen genom användaringång eller slumpmässigt
                                        string newPassword = generatePassword ? GenerateRandomPassword() : GetUserInputPassword();

                                        // Sätt det nya lösenordet för den angivna egenskapen i valvet
                                        vaultDict[propertyKey] = newPassword;

                                        // Kryptera hela valvet och spara tillbaka till serverfilen
                                        string encryptedVault = Vault.EncryptVault(vaultDict, aes);
                                        serverFileDict["EncryptedVault"] = encryptedVault;
                                        File.WriteAllText(serverFilePath, JsonSerializer.Serialize(serverFileDict));

                                        Console.WriteLine($"Lösenord för egenskapen {propertyKey} har lagts till/uppdaterats i valvet.");



                                    }
                                    else
                                    {
                                        Console.WriteLine("Dekrypteringen misslyckades.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ett fel inträffade: {ex.Message}");
                                }
                            }

                            else
                            {
                                Console.WriteLine("Du måste skriva in property");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Felaktigt antal argument.");
                        }
                        break;

                    case "delete":

                        

                        break;

                    case "secret":

                        
                        //Kontrollera att vi har rätt antal argument

                        if (args.Length == 2){

                            Command.SecretCommand(args);


                        }
                        else
                        {

                        }
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

        public static string GenerateRandomPassword()
        {
            // Här kan du implementera logiken för att generera ett slumpmässigt lösenord
            // till exempel en alfanumerisk sträng med 20 tecken.

            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            // Skapa en slumpmässig sträng med 20 tecken från allowedChars
            string randomPassword = new string(Enumerable.Repeat(allowedChars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            Console.WriteLine($"Slumpmässigt lösenord genererat: {randomPassword}");

            return randomPassword;
        }

        public static string GetUserInputPassword()
        {
            // Här kan du implementera logiken för att få inmatning från användaren
            // för att skapa ett lösenord.

            Console.WriteLine("Ange det önskade lösenordet:");
            string userInputPassword = Console.ReadLine();

            return userInputPassword;
        }


        static List<string> GetAllPropertyKeys(Dictionary<string, string> vaultDict)
        {
            // Skapa en ny lista för att hålla propertyKeys
            List<string> propertyKeys = new List<string>();

            // Loopa genom varje nyckel-värde-par i vaultDict
            foreach (var kvp in vaultDict)
            {
                // Lägg till nyckeln till listan
                propertyKeys.Add(kvp.Key);
            }

            // Returnera listan med propertyKeys
            return propertyKeys;
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

//funkar det???



