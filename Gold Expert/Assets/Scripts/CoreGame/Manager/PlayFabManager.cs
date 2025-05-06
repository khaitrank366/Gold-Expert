using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayFabManager : Singleton<PlayFabManager>
{
    public Dictionary<string, string> DataDictionary = new();
    public string PlayfabId;
  
    private async void Start()
    {
        await LoadData();
    }

    private async Task<bool> LoadData()
    {
        InitPlayerData();
        await Login();
        await GetAllPlayerData();
        await CurrencyManager.Instance.LoadCurrencies();
 
        return true;
        
    }
    
    private async Task Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        var loginTask = new TaskCompletionSource<LoginResult>();

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log("✅ Đăng nhập PlayFab thành công");
                PlayfabId = result.PlayFabId;
                loginTask.SetResult(result);
            },
            error =>
            {
                Debug.LogError("❌ Lỗi đăng nhập: " + error.GenerateErrorReport());
                loginTask.SetException(new Exception(error.GenerateErrorReport()));
            });

        // Đợi kết quả đăng nhập
        await loginTask.Task;
    }
    private void InitPlayerData()
    {
        DataDictionary = new Dictionary<string, string>();
        DataDictionary.Add(Common.PlayerDataKeyHelper.ToKey(PlayerDataKey.LastOnline),"");
        DataDictionary.Add(Common.PlayerDataKeyHelper.ToKey(PlayerDataKey.Coin),"0");
        DataDictionary.Add(Common.PlayerDataKeyHelper.ToKey(PlayerDataKey.CurrentBuildingData),"0");
    }
    
    #region Load & Save All Data

    private async Task GetAllPlayerData(Action onComplete = null)
    {
        var rsPlayerData = new TaskCompletionSource<GetUserDataResult>();
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null)
                {
                    foreach (var pair in result.Data)
                        DataDictionary[pair.Key] = pair.Value.Value;
                    Debug.Log("✅ Player data loaded.");
                    rsPlayerData.SetResult(result);
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.Log("🤩 RESULT DATA NULL");
                }
            },
            error =>
            {
                Debug.LogError($"❌ GetAllPlayerData Error: {error.GenerateErrorReport()}");
                rsPlayerData.SetException(new Exception(error.GenerateErrorReport()));
            });
       await rsPlayerData.Task;
    }
    
    private async Task SaveAll()
    {
        var tcs = new TaskCompletionSource<bool>();
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = DataDictionary
            },
            result =>
            {
                Debug.Log("✅ All data saved.");
                tcs.SetResult(true);
            },
            error =>
            {
                Debug.LogError($"❌ SaveAllAsync Error: {error.GenerateErrorReport()}");
                tcs.SetException(new Exception(error.ErrorMessage));
            });

        await tcs.Task;
    }
   
    public async Task SaveSingleData(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("❌ Key không hợp lệ.");
            return;
        }

        var tcs = new TaskCompletionSource<bool>();

        // Cập nhật dữ liệu trong bộ nhớ trước
        DataDictionary[key] = value;

        // Gửi dữ liệu lên PlayFab
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { key, value } }
            },
            result =>
            {
                Debug.Log($"✅ Dữ liệu {key} đã được lưu thành công.");
                tcs.SetResult(true);
            },
            error =>
            {
                Debug.LogError($"❌ Lỗi khi lưu dữ liệu {key}: {error.GenerateErrorReport()}");
                tcs.SetException(new Exception(error.ErrorMessage));
            });

        await tcs.Task;
    }
    public void SaveData(string key, string value)
    {
        if(!DataDictionary.ContainsKey(key))
        {
            Debug.Log(key + " Not found");
        }
        DataDictionary[key] = value;
    }
    #endregion

    #region OnQuit
    private void OnApplicationQuit()
    {
        Debug.Log("🛑 Game is quitting.");
        DataDictionary[Common.PlayerDataKeyHelper.ToKey(PlayerDataKey.LastOnline)] = System.DateTime.UtcNow.ToString("o");
        // Nếu bạn có SaveManager, có thể gọi:
        SaveAll();
    }
    

    #endregion
    #region DEBUG

    [Button]
    private void SaveAllDebug()
    {
        SaveAll();
    }
    

    #endregion
}