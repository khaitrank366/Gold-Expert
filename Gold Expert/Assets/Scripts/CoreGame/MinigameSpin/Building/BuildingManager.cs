using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class BuildingManager : Singleton<BuildingManager>, IModalUI
{
    #region UI References
    [Header("UI References")]
    [SerializeField] private GameObject objBuildingUI;
    #endregion

    #region Data
    [Header("Map & Player Progress")]
    [SerializeField] private JsonMapDatabase mapDatabase;
    private PlayerMapProgress playerProgress;
    private const string MAP_JSON_PATH = "Json/Map";
    private const string BUILDING_STATES_KEY = "BuildingStates";
    #endregion

    #region Initialization
    private void Start()
    {
        InitializeMapDatabase();
    }

    private void InitializeMapDatabase()
    {
        mapDatabase = LoadMapConfig();
        if (mapDatabase == null)
        {
            Debug.LogError("❌ Failed to initialize map database");
        }
    }

    public async Task Load()
    {
        try
        {
            if (await TryLoadExistingData())
            {
                return;
            }

            await CreateAndSaveDefaultData();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error loading building data: {e.Message}");
            await CreateAndSaveDefaultData(); // Fallback to default data
        }
    }

    private async Task<bool> TryLoadExistingData()
    {
        string data = PlayFabManager.Instance.DataDictionary[PlayerDataKey.CurrentBuildingData.ToString()];
          
        if (data == null || data == "null")
        {
            return false;
        }

        string json = PlayFabManager.Instance.GetData(PlayerDataKey.CurrentBuildingData.ToString());
        if (string.IsNullOrEmpty(json) || json == "null")
        {
            return false;
        }

        LoadProgress(json);
        Debug.Log("✅ Load thành công: " + json);
        return true;
    }

    private async Task CreateAndSaveDefaultData()
    {
        var defaultData = CreateDefaultBuildingData();
        string defaultJson = JsonConvert.SerializeObject(defaultData);
        
        await SaveDataToPlayFab(defaultJson);
        SaveDataToPlayerPrefs(defaultJson);
        LoadProgress(defaultJson);
        
        Debug.Log("✅ Đã tạo và load data mặc định: " + defaultJson);
    }

    private PlayerMapProgress CreateDefaultBuildingData()
    {
        return new PlayerMapProgress
        {
            currentMapId = "Map1",
            buildings = new Dictionary<string, BuildingState>
            {
                ["1"] = new BuildingState { level = 1, needRepair = false },
                ["2"] = new BuildingState { level = 1, needRepair = false },
                ["3"] = new BuildingState { level = 1, needRepair = false }
            }
        };
    }

    private async Task SaveDataToPlayFab(string json)
    {
        await PlayFabManager.Instance.SaveSingleData(PlayerDataKey.CurrentBuildingData.ToString(), json);
    }

    private void SaveDataToPlayerPrefs(string json)
    {
        PlayerPrefs.SetString(BUILDING_STATES_KEY, json);
        PlayerPrefs.Save();
    }

    private JsonMapDatabase LoadMapConfig()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(MAP_JSON_PATH);
        if (jsonAsset == null)
        {
            Debug.LogError($"❌ Không tìm thấy file {MAP_JSON_PATH} trong Resources");
            return null;
        }

        return JsonUtility.FromJson<JsonMapDatabase>(jsonAsset.text);
    }
    #endregion

    #region Building Logic
    public int GetCurrentLevel(string buildingName)
    {
        if (playerProgress?.buildings == null || !playerProgress.buildings.TryGetValue(buildingName, out var state))
        {
            return 0;
        }
        return state.level;
    }

    public int GetUpgradeCost(string buildingName, int currentLevel)
    {
        var currentMap = GetCurrentMap();
        if (currentMap == null) return 0;

        var buildingCfg = currentMap.buildings.FirstOrDefault(b => b.name == buildingName);
        if (buildingCfg == null) return 0;

        // Get building state to check if it needs repair
        if (playerProgress.buildings.TryGetValue(buildingName, out var state) && state.needRepair)
        {
            return 2000; // Double cost for repair
        }

        return 1000; // Normal upgrade cost
    }

    [Button]
    public bool UpgradeBuilding(string buildingName)
    {
        if (!ValidateBuildingUpgrade(buildingName, out var buildingCfg, out var state))
        {
            return false;
        }

        if (state.needRepair)
        {
            // Handle repair
            state.needRepair = false;
            Debug.Log($"🔧 Đã sửa chữa {buildingName}");
            SaveProgress();
            return true;
        }

        if (state.level >= buildingCfg.maxLevel)
        {
            Debug.Log($"✅ {buildingName} đã đạt cấp tối đa.");
            return false;
        }

        PerformUpgrade(buildingName, state);
        return true;
    }

    private bool ValidateBuildingUpgrade(string buildingName, out BuildingConfig buildingCfg, out BuildingState state)
    {
        buildingCfg = null;
        state = null;

        var currentMap = GetCurrentMap();
        if (currentMap == null)
        {
            Debug.LogError("❌ Không tìm thấy map hiện tại");
            return false;
        }

        buildingCfg = currentMap.buildings.FirstOrDefault(b => b.name == buildingName);
        if (buildingCfg == null)
        {
            Debug.LogError($"❌ Không tìm thấy cấu hình công trình {buildingName}");
            return false;
        }

        if (!playerProgress.buildings.TryGetValue(buildingName, out state))
        {
            Debug.LogError($"❌ Không tìm thấy trạng thái công trình {buildingName}");
            return false;
        }

        return true;
    }

    private void PerformUpgrade(string buildingName, BuildingState state)
    {
        state.level++;
        Debug.Log($"⬆️ Đã nâng {buildingName} lên cấp {state.level}");

        if (IsCurrentMapCompleted())
        {
            Debug.Log("🎉 Đã hoàn thành map hiện tại, chuyển sang map kế tiếp.");
            SwitchToNextMap();
        }

        SaveProgress();
    }

    public BuildingState GetBuildingState(string buildingName)
    {
        if (playerProgress?.buildings == null || !playerProgress.buildings.TryGetValue(buildingName, out var state))
        {
            return null;
        }
        return state;
    }
    #endregion

    #region Map Handling
    private MapData GetCurrentMap()
    {
        return mapDatabase?.maps.FirstOrDefault(m => m.mapId == playerProgress.currentMapId);
    }

    private bool IsCurrentMapCompleted()
    {
        var currentMap = GetCurrentMap();
        if (currentMap == null) return false;

        return currentMap.buildings.All(building =>
            playerProgress.buildings.TryGetValue(building.name, out var state)
            && state.level >= building.maxLevel
        );
    }

    private void SwitchToNextMap()
    {
        int currentIndex = mapDatabase.maps.FindIndex(m => m.mapId == playerProgress.currentMapId);
        if (currentIndex + 1 >= mapDatabase.maps.Count)
        {
            Debug.Log("🏁 Không còn map nào tiếp theo.");
            return;
        }

        var nextMap = mapDatabase.maps[currentIndex + 1];
        playerProgress.currentMapId = nextMap.mapId;
        ResetBuildingStates(nextMap);
        Debug.Log($"🌍 Đã chuyển sang map: {nextMap.mapId}");
    }

    private void ResetBuildingStates(MapData nextMap)
    {
        playerProgress.buildings.Clear();
        foreach (var b in nextMap.buildings)
        {
            playerProgress.buildings[b.name] = new BuildingState { level = 1, needRepair = false };
        }
    }
    #endregion

    #region Persistence
    public void LoadProgress(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("⚠️ Không có dữ liệu BuildingStates");
            return;
        }

        try
        {
            playerProgress = JsonConvert.DeserializeObject<PlayerMapProgress>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Lỗi khi parse JSON: {e.Message}");
        }
    }

    [Button]
    private async void SaveProgress()
    {
        try
        {
            var json = JsonConvert.SerializeObject(playerProgress);
            SaveDataToPlayerPrefs(json);
            await SaveDataToPlayFab(json);
            Debug.Log($"💾 Đã lưu tiến trình: {json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Lỗi khi lưu tiến trình: {e.Message}");
        }
    }
    #endregion

    #region UI Control
    public void Show()
    {
        objBuildingUI.SetActive(true);
    }

    public void Hide()
    {
        objBuildingUI.SetActive(false);
    }

    public bool IsVisible()
    {
        return objBuildingUI.activeSelf;
    }

    public void SwitchToSpinGame()
    {
        Hide();
        SlotMachine.Instance.Show();
    }
    #endregion
}
