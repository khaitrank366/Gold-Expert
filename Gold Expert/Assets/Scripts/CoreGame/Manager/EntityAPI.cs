using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

    [System.Serializable]
    public  class RobCoinResult
    {
        public string victimId;
        public int coinStolen;
        public int victimCoinLeft;
        public int myNewCoin;
    }
    [System.Serializable]
    public class PlayerDataResponse
    {
        public string selectedPlayFabId;
        public string displayName;
        public Dictionary<string, PlayerDataEntry> playerData;  // Changed to use PlayerDataEntry
        public Dictionary<string, int> virtualCurrency;
        
        // Helper properties to get the parsed data
        public string LastOnline => playerData != null && playerData.ContainsKey("LastOnline") ? playerData["LastOnline"].Value : null;
        
        public PlayerMapProgress CrrentBuildingData
        {
            get
            {
              
                if (playerData != null && playerData.ContainsKey("CurrentBuildingData"))
                {
                    // Use JsonConvert from Newtonsoft.Json instead of JsonUtility since the data contains a Dictionary
                    var buildingData = JsonConvert.DeserializeObject<PlayerMapProgress>(playerData["CurrentBuildingData"].Value);
                   
                    return buildingData;
                }
                return null;
            }
        }
       
    }

    [System.Serializable]
    public class PlayerDataEntry
    {
        public string Value;
        public string LastUpdated;
        public string Permission;
    }

 
    [System.Serializable]
    public class PlayerMapProgress
    {
        public string currentMapId;
        public Dictionary<string, BuildingState> buildings = new();
    }

    [System.Serializable]
    public class BuildingState
    {
        public int level;
        public bool needRepair;
    }
    [System.Serializable]
    public class MapData
    {
        public string mapId; // Ví dụ: "Map1", "Map2"
        public List<BuildingConfig> buildings;
    }

    [System.Serializable]
    public class BuildingConfig
    {
        public string name; // "Statue", "Castle", "Gate"
        public int maxLevel;
        public int baseCost = 100; // Base cost for upgrades
    }

    [System.Serializable]
    public class JsonMapDatabase
    {
        public List<MapData> maps;
    }
    public class AttackBuildingResponse
{
    public bool result;
    public string message;
    public bool shieldUsed;
}
