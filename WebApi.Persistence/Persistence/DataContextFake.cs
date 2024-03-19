using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.Core;
using WebApi.Core.DomainModel.Entities;

namespace WebApi.Persistence;

public class DataContextFake: IDataContext {

   private readonly string _filePath = Environment.GetFolderPath(
         Environment.SpecialFolder.ApplicationData) + $"WebApi.03.json";      

   public Dictionary<Guid, Owner>   Owners   { get; }
   public Dictionary<Guid, Account> Accounts { get; }
   
   private class CombinedDictionaries {
      public Dictionary<Guid, Owner>   Owners   { get; set; } = new();
      public Dictionary<Guid, Account> Accounts { get; set; } = new();
   }
   
   public DataContextFake() {
      try {
         if(!File.Exists(_filePath)) {
            Owners = new Dictionary<Guid, Owner>();
            Accounts = new Dictionary<Guid, Account>();
         } else {
            // Read the JSON file
            string json = File.ReadAllText(_filePath, Encoding.UTF8);
            
            // Deserialize the JSON string back into the CombinedDictionaries class
            var combinedDictionaries = JsonSerializer.Deserialize<CombinedDictionaries>(
               json,
               GetJsonSerializerOptions()
            ) ?? throw new ApplicationException("Deserialization failed");
            
            // Get the dictionaries from the deserialized object
            Owners = combinedDictionaries.Owners;
            Accounts = combinedDictionaries.Accounts;
         }
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
   }
   
   private JsonSerializerOptions GetJsonSerializerOptions() {
      return new JsonSerializerOptions {
         PropertyNameCaseInsensitive = true,
//       ReferenceHandler = ReferenceHandler.Preserve,
//       ReferenceHandler = ReferenceHandler.IgnoreCycles,
         WriteIndented = true
      };
   }
   
   public bool SaveAllChanges() {
      try {
         // Combine the dictionaries into one object
         var combinedDictionaries = new {
            Owners = Owners, 
            Accounts = Accounts
         };

         // Serialize to JSON
         string json = JsonSerializer.Serialize(
            combinedDictionaries, 
            GetJsonSerializerOptions() 
         );
         // Write JSON string to file
         File.WriteAllText(_filePath, json, Encoding.UTF8);
         return true;
      }
      catch (Exception e) {
         Console.WriteLine(e.Message);
         return false;
      }
   }
   
}