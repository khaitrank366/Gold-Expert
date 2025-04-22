using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class HandleAPI : MonoBehaviour
{

    [Button]
    public void CallHelloWorld()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "helloWorld", // 👈 tên hàm bạn viết trong CloudScript
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccess, OnError);
    }

    private void OnSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log("✅ Đã gọi CloudScript thành công!");

        if (result.FunctionResult != null)
        {
            string raw = result.FunctionResult.ToString();
            Debug.Log("📩 Kết quả trả về: " + raw);
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("❌ Lỗi khi gọi CloudScript: " + error.GenerateErrorReport());
    }
    [Button]
    public void CreateOrAddToGroup(string groupId)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "CreateOrAddToSharedGroup",
            FunctionParameter = new
            {
                sharedGroupId = groupId,
                targetPlayFabId = PlayFabManager.Instance.PlayfabId
            },
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
            {
                Debug.Log("✅ CloudScript Success: " + result.FunctionResult.ToString());
            },
            error =>
            {
                Debug.LogError("❌ CloudScript Failed: " + error.GenerateErrorReport());
            });
    }  

}
