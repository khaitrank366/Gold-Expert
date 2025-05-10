using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;


public class HandleAPI : Singleton<HandleAPI>   
{
    #region Base API HANDLE
    public static async Task<object> CallCloudScriptAsync(string functionName, object parameters = null)
    {
        var tcs = new TaskCompletionSource<object>();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = functionName,
            FunctionParameter = parameters,
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
            {
                if (result.FunctionResult != null)
                {
                    Debug.Log($"✅ CloudScript '{functionName}' Success: {result.FunctionResult}");
                    tcs.SetResult(result.FunctionResult);
                }
                else
                {
                    Debug.LogWarning($"⚠️ CloudScript '{functionName}' returned null result.");
                    tcs.SetResult(null);
                }
            },
            error =>
            {
                Debug.LogError($"❌ CloudScript '{functionName}' Error: {error.GenerateErrorReport()}");
                tcs.SetException(new PlayFabException(error));
            });

        return await tcs.Task;
    }
    

    #endregion

    #region API 
    [Button]
    public async void CallHelloWorldAsync()
    {
        try
        {
            var result = await CallCloudScriptAsync("helloWorld");
            Debug.Log("✅ Đã gọi CloudScript thành công!");
            Debug.Log("📩 Kết quả trả về: " + result.ToString());
        }
        catch (PlayFabException ex)
        {
          
            Debug.LogError("❌ Lỗi khi gọi CloudScript: " + ex.Message);
        }
    }

    public async void AddToSharedGroupAsync()
    {
        try
        {
            var result = await CallCloudScriptAsync("AddToSharedGroup", new
            {
                targetPlayFabId = PlayFabManager.Instance.PlayfabId
            });

            Debug.Log("✅ CloudScript Success: " + result.ToString());
        }
        catch (PlayFabException ex)
        {
            Debug.LogError("❌ CloudScript Failed: " + ex.Message);
        }
    }
    [Button]
    public async Task<PlayerDataResponse> GetRandomUserAsync()
    {
        PlayerDataResponse data=null;
        try
        {
            var result = await CallCloudScriptAsync("GetRandomUserFromGroup", new
            {
                currentPlayerId = PlayFabManager.Instance.PlayfabId
            });
              data = JsonConvert.DeserializeObject<PlayerDataResponse>(result.ToString());
         //   Debug.Log("✅ CloudScript Success: " + result.ToString()+"/"+data.playerData.Count);
        }
        catch (PlayFabException ex)
        {
            Debug.LogError("❌ CloudScript error: " + ex.Message);
        }

        return data;
    }
    [Button]
    public async Task<RobCoinResult> RobCoinVictimAsync()
    {
        RobCoinResult data = null;
        try
        {
            var result = await CallCloudScriptAsync("RobCoin", new
            {
                currentPlayerId = PlayFabManager.Instance.PlayfabId,
                victimId = "6409D2F127B7DCE2",
                stealPercent = 0.2f
            });

             data = JsonUtility.FromJson<RobCoinResult>(result.ToString());
          //  Debug.Log($"💰 Cướp {data.coinStolen} coin từ {data.victimId}");
            //need update coin cache
            CurrencyManager.Instance.CurrentCoin=data.myNewCoin;
        }
        catch (PlayFabException ex)
        {
            Debug.LogError("❌ Gọi RobCoin thất bại: " + ex.Message);
        }

        return data;
    }

     [Button]
    public async void AttackBuilding(int indexKeyBD,string targetId)
    {
      
        try
        {
            var result = await CallCloudScriptAsync("AttackBuilding", new
            {
                targetId = targetId,
                
                buildingKey = indexKeyBD.ToString()
            });

            Debug.Log("✅ CloudScript Success: " + result.ToString());  
          //  Debug.Log($"💰 Cướp {data.coinStolen} coin từ {data.victimId}");
            //need update coin cache
        
        }
        catch (PlayFabException ex)
        {
            Debug.LogError("❌ Gọi RobCoin thất bại: " + ex.Message);
        }
    }

    #endregion
 
    
   
}
public class PlayFabException : System.Exception
{
    public PlayFabError Error { get; private set; }
    public PlayFabException(PlayFabError error) : base(error.ErrorMessage)
    {
        Error = error;
    }
}