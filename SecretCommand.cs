using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace password_manager
{
    public class Command 

    {
    //   // public static void CommandType(string input)
    //    {
    //      string[] words= input.Split(' ');

    //        if (words[0]== "secret")
    //        {
    //            SecretCommand(words);
    //        } 
            
        

        public static void SecretCommand(string[] input)
        {
            if (File.Exists(input[1]))
            {
                string json= File.ReadAllText(input[1]);

                // Deserialisera JSON-strängen till en Dictionary<string, string>
                Dictionary<string, string> clientData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                // Kontrollera om deserialiseringen var framgångsrik
                if (clientData != null)
                {
                    // Skriv ut varje par (nyckel, värde) i klientdata
                    foreach (var kvp in clientData)
                    {
                        Console.WriteLine($"Nyckel: {kvp.Key}, Värde: {kvp.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("Kunde inte deserialisera JSON-data.");
                }
            }
            else
            {
                Console.WriteLine("Filen existerar inte.");
            }
        }
    }



}
          
            
       

   
