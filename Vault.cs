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
        //Skapar strukturen för serverfilen och lagrar IV samt krypterat tomt valv
        public static void ServerFileStructure(string filepath, byte[] vaultKey)
        {
            Dictionary<string, string> serverFile = new Dictionary<string, string>();
            byte[] iv = Aes_Kryptering.GenerateRandomIV();

            Aes aes = Aes_Kryptering.CreateAesObject(vaultKey, iv);

            Dictionary<string, string> uncryptedVault = new Dictionary<string, string>();

            string encryptedVault = EncryptVault(uncryptedVault, aes);
            string stringIV = Convert.ToBase64String(iv);

            serverFile.Add("IV", stringIV);
            serverFile.Add("EncryptedVault", encryptedVault);

            string serverJson = JsonSerializer.Serialize(serverFile, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filepath, serverJson);
        }

        //Krypterar valv
        public static string EncryptVault(Dictionary<string, string> uncryptedVault, Aes aes)
        {
            try
            {
                string json = JsonSerializer.Serialize(uncryptedVault);
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(json);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Krypteringen misslyckades: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade under krypteringen: {ex.Message}");
                return null;
            }
        }

        //Kollar om AES-objektet kan dekryptera
        public static bool CanDecryptVault(string encryptedData, Aes aes)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = csDecrypt.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Felaktig Base64-sträng: {ex.Message}");
                return false;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Felaktig nyckel eller IV: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett oväntat fel uppstod: {ex.Message}");
                return false;
            }
        }

        public static string DecryptVault(string encryptedData, Aes aes)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string decryptedJson = srDecrypt.ReadToEnd();

                            // Kontrollera om dekrypterad JSON är tom
                            if (string.IsNullOrEmpty(decryptedJson))
                            {
                                Console.WriteLine("Valvet är tomt");
                                return "{}"; // Returnera en tom dictionary-sträng
                            }

                            return decryptedJson;
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Ett fel inträffade: {ex.Message}");
                return null;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Dekrypteringen missslyckades: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade under dekrypteringen: {ex.Message}");
                return null;
            }
        }
    }
}













