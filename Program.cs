using System;
using System.IO;
using System.Text.Json;

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
        var clientFilePath = args[0];
        var serverFilePath = args[1];

        // Skapa filerna
        CreateFileIfNotExists(clientFilePath);
        CreateFileIfNotExists(serverFilePath);
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
} //Detta är första delen av koden