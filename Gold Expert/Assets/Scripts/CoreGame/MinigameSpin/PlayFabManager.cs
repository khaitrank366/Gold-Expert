using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    public int Coin { get; private set; }
    public int Lightning { get; private set; }

    public event Action<int> OnCoinChanged;
    public event Action<int> OnLightningChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        Login();
    }

    void Login()
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
                LoadCurrencies();
            },
            error =>
            {
                Debug.LogError("❌ Lỗi đăng nhập: " + error.GenerateErrorReport());
            });
    }

    void LoadCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                Coin = result.VirtualCurrency.ContainsKey("CO") ? result.VirtualCurrency["CO"] : 0;
                Lightning = result.VirtualCurrency.ContainsKey("LI") ? result.VirtualCurrency["LI"] : 0;

                OnCoinChanged?.Invoke(Coin);
                OnLightningChanged?.Invoke(Lightning);

                Debug.Log($"💰 Coin: {Coin}, ⚡ Lightning: {Lightning}");
            },
            error =>
            {
                Debug.LogError("❌ Lỗi lấy số dư: " + error.GenerateErrorReport());
            });
    }

    public void AddCoin(int amount)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CO",
            Amount = amount
        },
        result =>
        {
            Coin += amount;
            OnCoinChanged?.Invoke(Coin);
        },
        error =>
        {
            Debug.LogError("❌ Lỗi AddCoin: " + error.GenerateErrorReport());
        });
    }
    public void AddLightning(int amount)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "LI",
            Amount = amount
        },
        result =>
        {
            Lightning += amount;
            OnLightningChanged?.Invoke(Lightning);
        },
        error => Debug.LogError("❌ AddLightning Error: " + error.GenerateErrorReport()));
    }


    public bool SpendLightning(int amount)
    {
        if (Lightning < amount)
        {
            Debug.Log("⚡ Không đủ Lightning");
            return false;
        }

        PlayFabClientAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "LI",
            Amount = amount
        },
        result =>
        {
            Lightning -= amount;
            OnLightningChanged?.Invoke(Lightning);
        },
        error =>
        {
            Debug.LogError("❌ Lỗi SpendLightning: " + error.GenerateErrorReport());
        });

        return true;
    }
}
