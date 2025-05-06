using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        public Dictionary<string, PlayerDataEntry> playerData;
        public Dictionary<string, int> virtualCurrency;
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
    }

    [System.Serializable]
    public class JsonMapDatabase
    {
        public List<MapData> maps;
    }
