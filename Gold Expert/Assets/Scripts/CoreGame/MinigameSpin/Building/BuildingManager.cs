using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class BuildingManager : Singleton<BuildingManager>,IModalUI
{
   [Header("UI References")]
    [SerializeField] private GameObject objBuildingUI;

    [Header("Map & Player Progress")]
    public JsonMapDatabase mapDatabase;
    private PlayerMapProgress playerProgress;

    #region Initialization

    private void Start()
    {
        mapDatabase = LoadMapConfig();

     
    }
    public void Load(){
      //  string json = PlayerPrefs.GetString("BuildingStates", "");
      if (PlayFabManager.Instance.DataDictionary.ContainsKey("CurrentBuildingData"))
        {
            string json =PlayFabManager.Instance.GetData("CurrentBuildingData");
            LoadProgress(json);
            Debug.Log("✅ Load thành công: " + json);
        }
     
    }
    private JsonMapDatabase LoadMapConfig()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("Json/Map");
        if (jsonAsset == null)
        {
            Debug.LogError("❌ Không tìm thấy file Map.json trong Resources/Json/");
            return null;
        }

        return JsonUtility.FromJson<JsonMapDatabase>(jsonAsset.text);
    }

    #endregion

    #region Building Logic

    [Button]
    public bool UpgradeBuilding(string buildingName)
    {
        var currentMap = GetCurrentMap();
        var buildingCfg = currentMap.buildings.FirstOrDefault(b => b.name == buildingName);

        if (buildingCfg == null)
        {
            Debug.LogError($"❌ Không tìm thấy cấu hình công trình {buildingName}");
            return false;
        }

        if (!playerProgress.buildings.TryGetValue(buildingName, out var state))
        {
            Debug.LogError($"❌ Không tìm thấy trạng thái công trình {buildingName}");
            return false;
        }

        if (state.needRepair)
        {
            Debug.LogWarning($"🔧 {buildingName} cần sửa chữa trước khi nâng cấp.");
            return false;
        }

        if (state.level >= buildingCfg.maxLevel)
        {
            Debug.Log($"✅ {buildingName} đã đạt cấp tối đa.");
            return false;
        }

        state.level++;
        Debug.Log($"⬆️ Đã nâng {buildingName} lên cấp {state.level}");

        if (IsCurrentMapCompleted())
        {
            Debug.Log("🎉 Đã hoàn thành map hiện tại, chuyển sang map kế tiếp.");
            SwitchToNextMap();
        }

        SaveProgress();
        return true;
    }

    private bool IsCurrentMapCompleted()
    {
        var currentMap = GetCurrentMap();

        return currentMap.buildings.All(building =>
            playerProgress.buildings.TryGetValue(building.name, out var state)
            && state.level >= building.maxLevel
        );
    }

    #endregion

    #region Map Handling

    private MapData GetCurrentMap()
    {
        return mapDatabase.maps.FirstOrDefault(m => m.mapId == playerProgress.currentMapId);
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

        // Reset trạng thái công trình
        playerProgress.buildings.Clear();
        foreach (var b in nextMap.buildings)
        {
            playerProgress.buildings[b.name] = new BuildingState { level = 0, needRepair = false };
        }

        Debug.Log($"🌍 Đã chuyển sang map: {nextMap.mapId}");
    }

    #endregion

    #region Persistence

    public void LoadProgress(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            playerProgress = JsonConvert.DeserializeObject<PlayerMapProgress>(json);
        }
        else
        {
            Debug.LogWarning("⚠️ Không có dữ liệu BuildingStates trong PlayerPrefs.");
        }
    }

    [Button]
    private void SaveProgress()
    {
        var json = JsonConvert.SerializeObject(playerProgress);
        PlayerPrefs.SetString("BuildingStates", json);
        PlayerPrefs.Save();
        Debug.Log($"💾 Đã lưu tiến trình: {json}");
        // TODO: Gọi PlayFab API nếu cần đồng bộ lên cloud
        PlayFabManager.Instance.SaveSingleData(PlayerDataKey.CurrentBuildingData.ToString(), json);
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
