
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    public Dictionary<string, BuildingData> BuildingStates = new Dictionary<string, BuildingData>();
    [Button]
    public void ResetBuildings()
    {
        InitDefaultBuildings();
        SaveBuildingStates();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadBuildingStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveBuildingStates()
    {
        string json = JsonUtility.ToJson(new SerializableBuildingStates(BuildingStates));
        PlayerPrefs.SetString("BuildingStates", json);
    }

    public void LoadBuildingStates()
    {
        if (PlayerPrefs.HasKey("BuildingStates"))
        {
            string json = PlayerPrefs.GetString("BuildingStates");
            BuildingStates = JsonUtility.FromJson<SerializableBuildingStates>(json).ToDictionary();
        }
        else
        {
            InitDefaultBuildings();
        }
    }

    private void InitDefaultBuildings()
    {
        BuildingStates.Clear();

        BuildingStates.Add("Tent", new BuildingData { BuildingId = "Tent", CurrentLevel = 0 });
        BuildingStates.Add("Excavator", new BuildingData { BuildingId = "Excavator", CurrentLevel = 0 });
        BuildingStates.Add("DinoHead", new BuildingData { BuildingId = "DinoHead", CurrentLevel = 0 });

        SaveBuildingStates();
    }


    public void UpgradeBuilding(string buildingId)
    {
        if (BuildingStates.ContainsKey(buildingId))
        {
            var data = BuildingStates[buildingId];
            if (data.CurrentLevel >= 5)
            {
                Debug.Log("Max level.");
                return;
            }

            int cost = GetUpgradeCost(buildingId, data.CurrentLevel);
            if (CoinManager.Instance.Coin >= cost)
            {
                CoinManager.Instance.SpendCoin(cost);
                data.CurrentLevel++;
                SaveBuildingStates();
            }
            else
            {
                Debug.Log("Not enough coin.");
            }
        }
    }

    public int GetUpgradeCost(string buildingId, int currentLevel)
    {
        int[] costTable = { 1000, 2000, 5000, 10000, 20000 };
        return costTable[currentLevel];
    }

    public int GetCurrentLevel(string buildingId)
    {
        return BuildingStates.ContainsKey(buildingId) ? BuildingStates[buildingId].CurrentLevel : 0;
    }
}
