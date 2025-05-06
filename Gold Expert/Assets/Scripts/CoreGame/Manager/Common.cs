
using System.Collections.Generic;

#region Enum
public enum PlayerDataKey
{
   Coin,
   LastOnline,
   CurrentBuildingData
}
#endregion
public static class Common 
{
 
   #region Funct Support

   

   #endregion

   #region Funct Helper

   public  class PlayerDataKeyHelper
   {
      private static readonly Dictionary<PlayerDataKey, string> KeyData = new()
      {
         { PlayerDataKey.LastOnline, "LastOnline" },
         { PlayerDataKey.Coin, "Coin" }
      };

      public static string ToKey(PlayerDataKey key) => KeyData[key];
   }


   #endregion
}
// Helper để convert enum sang string key lưu trong PlayFab

