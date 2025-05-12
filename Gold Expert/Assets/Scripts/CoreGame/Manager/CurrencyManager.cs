using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public event Action<int> OnCoinChanged;
    public event Action<int> OnLightningChanged;
    public event Action<int> OnShieldChanged;

    private int _currentCoin;
    public int CurrentCoin
    {
        get => _currentCoin;
         set
        {
            if (_currentCoin != value)
            {
                _currentCoin = value;
                OnCoinChanged?.Invoke(_currentCoin);
            }
        }
    }
    private int _currentLightning;
    public int CurrentLightning
    {
        get => _currentLightning;
         set
        {
            if (_currentLightning != value)
            {
                _currentLightning = value;
                OnLightningChanged?.Invoke(_currentLightning);
            }
        }
    }
    private int _currentShield;
    public int CurrentShield
    {
        get => _currentShield;
         set
        {
            if (_currentShield != value)
            {
                _currentShield = value;
                OnShieldChanged?.Invoke(_currentShield);
            }
        }
    }
    
    private const string COIN_KEY = "CO";
    private const string LIGHTNING_KEY = "LI";
    private const string SHIELD_KEY = "SH";
    
    #region Load
    public async Task LoadCurrencies()
    {
        var tcs = new TaskCompletionSource<GetUserInventoryResult>();

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                tcs.SetResult(result);
            },
            error =>
            {
                Debug.LogError($"❌ LoadCurrencies Error: {error.GenerateErrorReport()}");
                tcs.SetException(new Exception(error.GenerateErrorReport()));
            });

        var inventoryResult = await tcs.Task;
        
        CurrentLightning = inventoryResult.VirtualCurrency.ContainsKey(LIGHTNING_KEY) ? inventoryResult.VirtualCurrency[LIGHTNING_KEY] : 0;
        CurrentShield = inventoryResult.VirtualCurrency.ContainsKey(SHIELD_KEY) ? inventoryResult.VirtualCurrency[SHIELD_KEY] : 0;
        CurrentCoin = inventoryResult.VirtualCurrency.ContainsKey(COIN_KEY) ? inventoryResult.VirtualCurrency[COIN_KEY] : 0;
       
        Debug.Log($"💰 Coin: {CurrentCoin}");
        Debug.Log($"⚡ Lightning: {CurrentLightning}");
        Debug.Log($"🛡️ Shield: {CurrentShield}");
    }

    #endregion

    #region Public Methods

    public async void AddCoin(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ AddCoin amount <= 0 is invalid.");
            return;
        }
        
        int newBalance = await ModifyCurrencyAsync(COIN_KEY, amount);
        CurrentCoin = newBalance;
    }

    public async void AddLightning(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ AddLightning amount <= 0 is invalid.");
            return;
        }
        
        int newBalance = await ModifyCurrencyAsync(LIGHTNING_KEY, amount);
        CurrentLightning = newBalance;
    }

    public async void AddShield(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ AddShield amount <= 0 is invalid.");
            return;
        }
        
        int newBalance = await ModifyCurrencyAsync(SHIELD_KEY, amount);
        CurrentShield = newBalance;
    }

    public async Task<bool> SpendLightning(int amount)
    {
        if (CurrentLightning < amount)
        {
            Debug.Log("⚡ Not enough Lightning");
            return false;
        }
        
        int newBalance = await ModifyCurrencyAsync(LIGHTNING_KEY, -amount);
        CurrentLightning = newBalance;
        return true;
    }

    public async Task<bool> SpendShield(int amount)
    {
        if (CurrentShield < amount)
        {
            Debug.Log("🛡️ Not enough Shield");
            return false;
        }
        
        int newBalance = await ModifyCurrencyAsync(SHIELD_KEY, -amount);
        CurrentShield = newBalance;
        return true;
    }

    public async Task<bool> SpendCoin(int amount)
    {
        if (CurrentCoin < amount)
        {
            Debug.Log("💰 Not enough Coin");
            return false;
        }
        
        int newBalance = await ModifyCurrencyAsync(COIN_KEY, -amount);
        CurrentCoin = newBalance;
        return true;
    }
    #endregion

    #region Core Logic

    public async Task<int> ModifyCurrencyAsync(string currencyKey, int amount)
    {
        if (amount == 0)
        {
            Debug.LogWarning("⚠️ ModifyCurrency called with 0 amount");
            return 0;
        }

        var tcs = new TaskCompletionSource<int>();

        if (amount > 0)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = currencyKey,
                Amount = amount
            };

            PlayFabClientAPI.AddUserVirtualCurrency(request,
                result => tcs.SetResult(result.Balance),
                error =>
                {
                    Debug.LogError($"❌ Add {currencyKey} Error: {error.GenerateErrorReport()}");
                    tcs.SetException(new Exception(error.GenerateErrorReport()));
                });
        }
        else
        {
            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = currencyKey,
                Amount = Mathf.Abs(amount)
            };

            PlayFabClientAPI.SubtractUserVirtualCurrency(request,
                result => tcs.SetResult(result.Balance),
                error =>
                {
                    Debug.LogError($"❌ Subtract {currencyKey} Error: {error.GenerateErrorReport()}");
                    tcs.SetException(new Exception(error.GenerateErrorReport()));
                });
        }

        return await tcs.Task;
    }

    #endregion
}
