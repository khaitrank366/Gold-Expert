using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;
    public string PlayfabId;
    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Login();
    }

    // Hàm đăng nhập với PlayFab
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, 
            result =>   
            {
                Debug.Log("✅ Đăng nhập PlayFab thành công");
                PlayfabId=result.PlayFabId;
                LoadPlayerData();
            },
            error =>
            {
                Debug.LogError("❌ Lỗi đăng nhập: " + error.GenerateErrorReport());
            });
    }

    // Hàm tải dữ liệu người chơi
    private void LoadPlayerData()
    {
        // Sau khi đăng nhập thành công, tải dữ liệu người chơi (ví dụ: số dư tiền tệ)
        CurrencyManager.Instance.LoadCurrencies();
    }
}