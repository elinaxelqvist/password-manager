using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace password_manager
{
    public class Vault
    {

        public static void ServerFileStructure(string filepath, byte[] vaultKey)
        {
            // Strukturen för Serverfilen skapas 
            Dictionary<string, string> serverFile = new Dictionary<string, string>();

            // En IV genereras
            byte[] iv = Aes_Kryptering.GenerateRandomIV();

            // AES-objektet skapas
            Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

            // Okrypterade valvet skapas 
            Dictionary<string, string> uncryptedVault = new Dictionary<string, string>();

            // Valvet krypteras med aes objektet
            string encryptedVault = EncryptVault(uncryptedVault, aes);




            // IV och det krypterade valvet sparas/lagras i filen

            // Konvertera IV till en Base64-sträng
            string stringIV = Convert.ToBase64String(iv);

            // Lägg till IV och det krypterade valvet till serverFile-dictionary
            serverFile.Add("IV", stringIV);
            serverFile.Add("EncryptedVault", encryptedVault);

            // Konvertera serverFile till JSON-sträng
            string serverJson = JsonSerializer.Serialize(serverFile, new JsonSerializerOptions { WriteIndented = true });

            // Skriv JSON-strängen till filen
            File.WriteAllText(filepath, serverJson);
        }



        public static string EncryptVault(Dictionary<string, string> uncryptedVault, Aes aes)
        {
            // Konvertera Dictionary till JSON-sträng
            string json = JsonSerializer.Serialize(uncryptedVault);

            // Skapa krypterare
            ICryptoTransform encryptor = aes.CreateEncryptor();

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // Konvertera JSON-sträng till byte-array och skriv till krypteringsströmmen
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                    csEncrypt.Write(jsonBytes, 0, jsonBytes.Length);
                }

                // Returnera det krypterade byte-arrayet som Base64-sträng
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }



        public static bool CanDecryptVault(string encryptedData, Aes aes)
        {
            try
            {
                // Konvertera Base64-strängen till byte-array
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                // Skapa dekrypterare
                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Försök att dekryptera, men vi behöver inte faktiskt läsa in data här
                        // Vi kommer bara hit om dekrypteringen lyckas
                        return true;
                    }
                }
            }
            catch (FormatException)
            {
                // Felaktig Base64-sträng
                return false;
            }
            catch (CryptographicException)
            {
                // Felaktig nyckel eller IV
                return false;
            }
        }





        public static string DecryptVault(string encryptedData, Aes aes)
        {
            try
            {
                // Konvertera Base64-strängen till byte-array
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                // Skapa dekrypterare
                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Läs dekrypterade data från krypteringsströmmen
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string decryptedJson = srDecrypt.ReadToEnd();
                            return decryptedJson;
                        }
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Base64 string.");
                return null;
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Decryption failed. Incorrect key or IV.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during decryption: {ex.Message}");
                return null;
            }
        }


















        //    public static void ServerFileStructure(string filePath, string vaultIV, string prop, string password)
        //    {
        //        // Generera en ny initieringsvektor för varje inmatning i valvet
        //        byte[] ivBytes = Convert.FromBase64String(vaultIV);

        //        VaultData vaultData = new VaultData();

        //        // Kontrollera om filen redan finns
        //        if (File.Exists(filePath))
        //        {
        //            // Läs innehållet från filen, dekryptera det och deserialisera det till VaultData
        //            string encryptedJson = File.ReadAllText(filePath);
        //            if (!string.IsNullOrEmpty(encryptedJson))
        //            {
        //                string decryptedJson = DecryptData(encryptedJson, ivBytes);
        //                vaultData = JsonSerializer.Deserialize<VaultData>(decryptedJson);
        //            }
        //        }

        //        // Lägg till nytt (prop, password)-par
        //        vaultData.Entries.Add(new VaultEntry { Prop = prop, Password = password });

        //        // Konvertera till JSON-sträng, kryptera och spara i filen
        //        string jsonOutput = JsonSerializer.Serialize(vaultData, new JsonSerializerOptions { WriteIndented = true });
        //        string encryptedOutput = EncryptData(jsonOutput, ivBytes);
        //        File.WriteAllText(filePath, encryptedOutput);

        //        Console.WriteLine("Lösenord tillagt i serverfilen.");
        //    }

        //    private static string EncryptData(string data, byte[] iv)
        //    {
        //        using (Aes aesAlg = Aes.Create())
        //        {
        //            aesAlg.IV = iv;

        //            // Använd en nyckel och IV för att kryptera data
        //            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //            using (MemoryStream msEncrypt = new MemoryStream())
        //            {
        //                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //                {
        //                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                    {
        //                        swEncrypt.Write(data);
        //                    }
        //                }
        //                return Convert.ToBase64String(msEncrypt.ToArray());
        //            }
        //        }
        //    }

        //    private static string DecryptData(string encryptedData, byte[] iv)
        //    {
        //        using (Aes aesAlg = Aes.Create())
        //        {
        //            aesAlg.IV = iv;

        //            // Använd en nyckel och IV för att dekryptera data
        //            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
        //            {
        //                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        return srDecrypt.ReadToEnd();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //// En klass för att representera hela valvet
        //public class VaultData
        //{
        //    public List<VaultEntry> Entries { get; set; } = new List<VaultEntry>();
        //}

        //// En klass för att representera varje inmatning i valvet
        //public class VaultEntry
        //{
        //    public string Prop { get; set; }
        //    public string Password { get; set; }
        //}
    }
}













