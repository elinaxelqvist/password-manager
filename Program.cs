using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.ComponentModel.Design;
using System.Collections;


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

                        // Kontrollerar att vi har rätt antal argument
                        if (args.Length == 3)
                        {
                            // Lagrar klient- och serverfilens sökvägar
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];

                            // Skapar eller skriver över filer
                            ManageFiles.CreateOrOverwriteClientFile(clientFilePath);
                            ManageFiles.CreateOrOverwriteServerFile(serverFilePath);

                            Console.WriteLine($"Klient-filen '{clientFilePath}' och server-filen '{serverFilePath}' har skapats eller skrivits över.");


                            //Hämtar secret key och skriver ut i terminalen
                            string clientFile = File.ReadAllText(clientFilePath);

                            Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile);

                            string secretKey = clientFileDict["SecretKey"];

                            Console.WriteLine("Här är din hemliga nyckel. Skriv ned den och kom ihåg den");
                            Console.WriteLine(secretKey);


                            //Användare får skapa master password
                            string masterPassword = MasterPassword();

                            //Valvnyckel genereras
                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            //Skapa AES, valv och kryptera valvet. Spara IV och det krypterade valvet i serverfil
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

                            //Användaren får ange sin secret key 
                            Console.WriteLine("Ange din hemliga nyckel");
                            string secretKey = Console.ReadLine();

                            //Valvnyckel genereras
                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            try
                            { 
                                //Innehållet som finns i serverfilen hämtas och deserialiseras
                                string serverFileContents = File.ReadAllText(serverFilePath);
                                Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                                //Hämtar ut värdena som finns för IV och EncryptedVault i serverfilen och lagrar dessa 
                                string ivValue = serverFileDict["IV"];
                                string encryptedData = serverFileDict["EncryptedVault"];

                                //Omvandlar IV till en byte[]
                                byte[] iv = Convert.FromBase64String(ivValue);

                                //Skapar AES-objekt
                                Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);


                                if (encryptedData != null && aes != null)
                                {
                                    if (Vault.CanDecryptVault(encryptedData, aes))
                                    {
                                        // Om valvet kunde krypteras med AES-objektet skapas en ny klientfil där secret key lagras
                                        ManageFiles.CreateNewClientFile(clientFilePath, secretKey);
                                        Console.WriteLine("Det har skapats en ny klient-fil med din secret key");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Fel: Ogiltigt masterpassword eller secretKey.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Fel: Kunde inte kontrollera valvet med angiven data.");
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

                        if (args.Length == 3 || args.Length == 4)
                        {
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];

                            // Användaren får ange master password
                            string masterPassword = MasterPassword();

                            //Hämtar secret key 
                            string clientFile = File.ReadAllText(clientFilePath);

                            //Deserialiserar innehållet
                            Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile);

                            string secretKey = clientFileDict["SecretKey"];

                            // Genererar valvnyckel
                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            // Läser in IV från serverfilen
                            string serverFileContents = File.ReadAllText(serverFilePath);
                            Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                            string ivValue = serverFileDict["IV"];
                            string encryptedData = serverFileDict["EncryptedVault"];

                            byte[] iv = Convert.FromBase64String(ivValue);

                            // Skapar AES-objekt
                            Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

                            if (args.Length == 4)
                            {
                                string propertyKey = args[3];

                                if(Vault.CanDecryptVault(encryptedData, aes))
                                {
                                    // Dekrypterar valvet från serverfilen
                                    string decryptedVault = Vault.DecryptVault(encryptedData, aes);
                                    Dictionary<string, string> vaultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);

                                    // Anropar funktionen för att hämta alla propertyKeys
                                    List<string> propertyKeys = GetAllPropertyKeys(vaultDict);

                                    bool foundKey = false;

                                    // Loopar igenom varje propertyKey
                                    for (int i = 0; i < propertyKeys.Count; i++)
                                    {
                                        // Kontrollerar om propertyKey matchar en nyckel i vaultDict
                                        if (propertyKeys[i] == propertyKey)
                                        {
                                            // Hämtar lösenordet för den matchande propertyKey
                                            string password = vaultDict[propertyKey];
                                            Console.WriteLine(password);
                                            foundKey = true;
                                            break;
                                        }
                                    }
                                    // Om propertyKey inte hittas
                                    if (!foundKey)
                                    {
                                        Console.WriteLine($"Fel: PropertyKey \"{propertyKey}\" finns inte i valvet.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Dekrypteringen misslyckades");
                                }
                            }
                            else
                            {
                                string propertyKey = null;

                                if (Vault.CanDecryptVault(encryptedData, aes))
                                {
                                    // Dekrypterar valvet från serverfilen
                                    string decryptedVault = Vault.DecryptVault(encryptedData, aes);
                                    Dictionary<string, string> vaultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);

                                    if (propertyKey == null)
                                    {
                                        // Anropar funktionen för att hämta alla propertyKeys
                                        List<string> propertyKeys = GetAllPropertyKeys(vaultDict);

                                        // Skriv ut alla propertyKeys
                                        Console.WriteLine("Alla propertyKeys i vaultDict:");
                                        foreach (string key in propertyKeys)
                                        {
                                            Console.WriteLine(key);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Dekrypteringen misslyckades");
                                    }
                                }

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
                                    // Användaren får ange master password
                                    string masterPassword = MasterPassword();

                                    Console.WriteLine(masterPassword);

                                    //Hämtar secret key 
                                    string clientFile = File.ReadAllText(clientFilePath);

                                    Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile);

                                    string secretKey = clientFileDict["SecretKey"];


                                    // Genererar valvnyckel
                                    byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                                    // Läs in IV från serverfilen
                                    string serverFileContents = File.ReadAllText(serverFilePath);
                                    Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                                    string ivValue = serverFileDict["IV"];
                                    string encryptedData = serverFileDict["EncryptedVault"];

                                    byte[] iv=Convert.FromBase64String(ivValue);

                                    // Skapar AES-objekt
                                    Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);


                                    if (Vault.CanDecryptVault(encryptedData, aes))

                                    {
                                        // Dekrypterar valvet från serverfilen
                                        string decryptedVault = Vault.DecryptVault(encryptedData, aes);
                                        Dictionary<string, string> vaultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);

                                        // Läser lösenordet från valvet
                                        string existingPassword = vaultDict.ContainsKey(propertyKey) ? vaultDict[propertyKey] : null;

                                        // Genererar nytt lösenord
                                        string newPassword = generatePassword ? GenerateRandomPassword() : GetUserInputPassword();

                                        // Sätter det nya lösenordet
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
                                Console.WriteLine("Du måste skriva in en property");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Felaktigt antal argument.");
                        }
                        break;

                    case "delete":
                        if (args.Length == 4)
                        {
                            string clientFilePath = args[1];
                            string serverFilePath = args[2];
                            string propertyKey = args[3];

                            string masterPassword = MasterPassword();

                            //Hämtar secret key 
                            string clientFile = File.ReadAllText(clientFilePath);

                            Dictionary<string, string> clientFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(clientFile);

                            string secretKey = clientFileDict["SecretKey"];


                            // Genererar valvnyckel
                            byte[] vaultKey = VaultKeyGenerator.GenerateVaultKey(masterPassword, secretKey);

                            // Läser in IV från serverfilen
                            string serverFileContents = File.ReadAllText(serverFilePath);
                            Dictionary<string, string> serverFileDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverFileContents);

                            string ivValue = serverFileDict["IV"];
                            string encryptedData = serverFileDict["EncryptedVault"];

                            byte[] iv = Convert.FromBase64String(ivValue);

                            // Skapar AES-objekt
                            Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

                            if (Vault.CanDecryptVault(encryptedData, aes))

                            {
                                // Dekrypterar valvet från serverfilen
                                string decryptedVault = Vault.DecryptVault(encryptedData, aes);
                                Dictionary<string, string> vaultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);

                                // Anropar funktionen för att hämta alla propertyKeys
                                List<string> propertyKeys = GetAllPropertyKeys(vaultDict);

                                bool foundKey = false;

                                // Loopar igenom varje propertyKey
                                for (int i = 0; i < propertyKeys.Count; i++)
                                {
                                    // Kontrollerar om propertyKey matchar en nyckel i vaultDict
                                    if (propertyKeys[i] == propertyKey)
                                    {
                                        // Ta bort propertyKey från vaultDict
                                        vaultDict.Remove(propertyKey);
                                        Console.WriteLine($"PropertyKey \"{propertyKey}\" och dess lösenord har tagits bort från valvet.");

                                        // Krypterar valvet och sparar tillbaka till serverfilen
                                        string encryptedVault = Vault.EncryptVault(vaultDict, aes);
                                        serverFileDict["EncryptedVault"] = encryptedVault;
                                        File.WriteAllText(serverFilePath, JsonSerializer.Serialize(serverFileDict));

                                        foundKey = true;
                                        break;
                                    }
                                }
                                // Om propertyKey inte hittades
                                if (!foundKey)
                                {
                                    Console.WriteLine($"Fel: PropertyKey \"{propertyKey}\" finns inte i valvet.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Dekrypteringen misslyckades");
                            }
                            }
                            else
                        {
                            Console.WriteLine("Felaktigt antal argument");
                        }
                            break;

                    case "secret":
                        //Kontrollerar att vi har rätt antal argument

                        if (args.Length == 2)
                        {
                            string secretKey = Command.SecretCommand(args);

                            if(secretKey != null)
                            {
                                Console.WriteLine(secretKey);
                            }
                            else
                            {
                                Console.WriteLine("Filen existerar inte");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Felaktigt antal argument");
                        }
                        break;
                    default:
                        Console.WriteLine("Ogiltigt kommando");
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
            while (true) // Loopar tills ett giltigt lösenord har matats in
            {
                Console.WriteLine("Skapa/ange ditt master lösenord med minst 8 tecken.");
                string input = Console.ReadLine();

                if (input.Length >= 8)
                {
                    return input; // Returnerar det godkända lösenordet
                }
                else
                {
                    Console.WriteLine("Lösenordet är för kort, försök igen.");
                }
            }
        }

        public static string GenerateRandomPassword()
        {
            // Genererar slumpmässigt lösenord
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            // Skapar slumpmässig sträng
            string randomPassword = new string(Enumerable.Repeat(allowedChars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            Console.WriteLine($"Slumpmässigt lösenord genererat: {randomPassword}");

            return randomPassword;
        }

        public static string GetUserInputPassword()
        {
            Console.WriteLine("Ange det önskade lösenordet:");
            string userInputPassword = Console.ReadLine();

            return userInputPassword;
        }

        static List<string> GetAllPropertyKeys(Dictionary<string, string> vaultDict)
        {
            // Skapar en lista för att hålla propertyKeys
            List<string> propertyKeys = new List<string>();

            // Loopar genom varje nyckel-värde-par
            foreach (var kvp in vaultDict)
            {
                // Lägger till nyckeln till listan
                propertyKeys.Add(kvp.Key);
            }
            // Returnerar listan med propertyKeys
            return propertyKeys;
        }
    }

}

