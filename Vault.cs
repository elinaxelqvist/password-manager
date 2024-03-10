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


        //Metod som skapar strukturen för serverfilen och lagrar IV och ett krypterat, tomt valv
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
            try
            {
                // Konvertera Dictionary till JSON-sträng
                string json = JsonSerializer.Serialize(uncryptedVault);

                // Skapa krypterare
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Skriv JSON-strängen direkt till krypteringsströmmen
                        swEncrypt.Write(json);
                    }

                    // Returnera det krypterade byte-arrayet som Base64-sträng
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Encryption failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during encryption: {ex.Message}");
                return null;
            }
        }


        public static bool CanDecryptVault(string encryptedData, Aes aes)
        {
            try
            {
                // Konvertera Base64-strängen till byte-array
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
              
                string aesKey = Convert.ToBase64String(aes.Key);
                Console.WriteLine(aesKey);
                string aesIV = Convert.ToBase64String(aes.IV);
                Console.WriteLine(aesIV);

                // Skapa dekrypterare
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);


                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Försök att dekryptera
                        // Läs in data från CryptoStream och skriv till en temporär buffer
                        byte[] buffer = new byte[1024];
                        int bytesRead = csDecrypt.Read(buffer, 0, buffer.Length);

                        // Om det inte finns några bytes att läsa in, dekrypteringen misslyckades
                        if (bytesRead == 0)
                        {
                            return false;
                        }

                        // Om vi når hit, dekrypteringen lyckades och det finns data att läsa
                        return true;
                    }
                }
            }
            catch (FormatException ex)
            {
                // Felaktig Base64-sträng
                Console.WriteLine($"Felaktig Base64-sträng: {ex.Message}");
                return false;
            }
            catch (CryptographicException ex)
            {
                // Felaktig nyckel eller IV
                Console.WriteLine($"Felaktig nyckel eller IV: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Övriga oväntade fel
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
                                Console.WriteLine("Decrypted JSON is empty.");
                                return "{}"; // Returnera en tom dictionary-sträng
                            }

                            return decryptedJson;
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Invalid Base64 string: {ex.Message}");
                return null;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Decryption failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during decryption: {ex.Message}");
                return null;
            }
        }
    }
}













