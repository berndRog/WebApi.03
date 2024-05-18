using System;
namespace WebApi.Core.Misc; 
public static class Utils {
   
   public static string CheckIban(string? iban) {
      //"DEkk BBBB BBBB CCCC CCCC CC"
      if (iban != null) {
         iban = iban.Replace(" ", "").ToUpper();
         if (iban.Length == 18) return iban;
      }
      // if iban is not valid, create a new one
      var random = new Random();
      return "DE" + Digits2(random)+ " "+ Digits4(random) + 
         Digits4(random) + Digits4(random) + Digits2(random);
   }

   private static string Digits2(Random random) =>
      random.Next(1, 99).ToString("D2");
   private static string Digits4(Random random) =>
      random.Next(0, 1000).ToString("D4") + " ";

   
   public static string As8(this Guid guid) => guid.ToString()[..8];

}