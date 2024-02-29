﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace password_manager
{
    public class Vault
    {
        public static void ServerFileStructure(string filePath, string vaultIV, string prop, string password)
        {
            // Generera en ny initieringsvektor för varje inmatning i valvet
            byte[] ivBytes = Convert.FromBase64String(vaultIV);

            VaultData vaultData = new VaultData();

            // Kontrollera om filen redan finns
            if (File.Exists(filePath))
            {
                // Läs innehållet från filen, dekryptera det och deserialisera det till VaultData
                string encryptedJson = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(encryptedJson))
                {
                    string decryptedJson = DecryptData(encryptedJson, ivBytes);
                    vaultData = JsonSerializer.Deserialize<VaultData>(decryptedJson);
                }
            }

            // Lägg till nytt (prop, password)-par
            vaultData.Entries.Add(new VaultEntry { Prop = prop, Password = password });

            // Konvertera till JSON-sträng, kryptera och spara i filen
            string jsonOutput = JsonSerializer.Serialize(vaultData, new JsonSerializerOptions { WriteIndented = true });
            string encryptedOutput = EncryptData(jsonOutput, ivBytes);
            File.WriteAllText(filePath, encryptedOutput);

            Console.WriteLine("Lösenord tillagt i serverfilen.");
        }

        private static string EncryptData(string data, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.IV = iv;

                // Använd en nyckel och IV för att kryptera data
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private static string DecryptData(string encryptedData, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.IV = iv;

                // Använd en nyckel och IV för att dekryptera data
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    // En klass för att representera hela valvet
    public class VaultData
    {
        public List<VaultEntry> Entries { get; set; } = new List<VaultEntry>();
    }

    // En klass för att representera varje inmatning i valvet
    public class VaultEntry
    {
        public string Prop { get; set; }
        public string Password { get; set; }
    }
}












