using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.ComponentModel.Design;


namespace password_manager
{
    public class ManageFiles
    {
        //Kollar om en fil existerar, om inte skapas en ny fil 
        public static void CreateFileIfNotExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                using (var stream = File.Create(filePath))
                {
                    Console.WriteLine($"Filen skapades: {filePath}");
                }
            }
            else
            {
                Console.WriteLine($"Filen finns redan: {filePath}");
            }
        }

        //Skapar eller skriver över klientfilen samt lagrar en secret key i den
        public static void CreateOrOverwriteClientFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skapande av filen '{filePath}': {ex.Message}");
            }
            string secretKey = SecretKey.GenerateSecretKey();

            SecretKey.SaveSecretKeyToFile(filePath, secretKey);
        }

        //Skapar eller skriver över filen
        public static void CreateOrOverwriteServerFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skapande av filen '{filePath}': {ex.Message}");
            }

        }

        //Skapar en ny klient-fil med befintlig secretKey
        public static void CreateNewClientFile(string filePath, string secretKey)
        {
            try
            {
                CreateFileIfNotExists(filePath);
                File.WriteAllText(filePath, "");
                SecretKey.SaveSecretKeyToFile(filePath, secretKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid skapande av filen '{filePath}': {ex.Message}");
            }
        }
    }
}