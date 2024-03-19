using System;
namespace WebApi.Core.Misc; 
public static class Utils {
   public static string As8(this Guid guid) => guid.ToString()[..8];
}