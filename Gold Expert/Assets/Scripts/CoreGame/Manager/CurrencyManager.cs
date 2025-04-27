using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public event Action<int> OnCoinChanged;
    public event Action<int> OnLightningChanged;

    public int CurrentCoin { get; private set; }
    public int CurrentLightning { get; private set; }

    private const string COIN_KEY = "CO";
    private const string LIGHTNING_KEY = "LI";

    #region Load
    public void LoadCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                CurrentCoin = result.VirtualCurrency.ContainsKey(COIN_KEY) ? result.VirtualCurrency[COIN_KEY] : 0;
                CurrentLightning = result.VirtualCurrency.ContainsKey(LIGHTNING_KEY) ? result.VirtualCurrency[LIGHTNING_KEY] : 0;

                OnCoinChanged?.Invoke(CurrentCoin);
                OnLightningChanged?.Invoke(CurrentLightning);

                Debug.Log($"💰 Coin: {CurrentCoin}, ⚡ Lightning: {CurrentLightning}");
            },
            error => Debug.LogError($"❌ LoadCurrencies Error: {error.GenerateErrorReport()}")
        );
    }
    #endregion

    #region Public Methods

    public void AddCoin(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ AddCoin amount <= 0 is invalid.");
            return;
        }

        ModifyCurrency(COIN_KEY, amount, newBalance =>
        {
            CurrentCoin = newBalance;
            OnCoinChanged?.Invoke(CurrentCoin);
        });
    }

    public void AddLightning(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("⚠️ AddLightning amount <= 0 is invalid.");
            return;
        }

        ModifyCurrency(LIGHTNING_KEY, amount, newBalance =>
        {
            CurrentLightning = newBalance;
            OnLightningChanged?.Invoke(CurrentLightning);
        });
    }

    public bool SpendLightning(int amount)
    {
        if (CurrentLightning < amount)
        {
            Debug.Log("⚡ Not enough Lightning");
            return false;
        }

        ModifyCurrency(LIGHTNING_KEY, -amount, newBalance =>
        {
            CurrentLightning = newBalance;
            OnLightningChanged?.Invoke(CurrentLightning);
        });

        return true;
    }

    #endregion

    #region Core Logic

    private void ModifyCurrency(string currencyKey, int amount, Action<int> onSuccess)
    {
        if (amount == 0)
        {
            Debug.LogWarning("⚠️ ModifyCurrency called with 0 amount");
            return;
        }

        if (amount > 0)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = currencyKey,
                Amount = amount
            };

            PlayFabClientAPI.AddUserVirtualCurrency(request,
                result => onSuccess?.Invoke(result.Balance),
                error => Debug.LogError($"❌ Add {currencyKey} Error: {error.GenerateErrorReport()}")
            );
        }
        else
        {
            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = currencyKey,
                Amount = Mathf.Abs(amount)
            };

            PlayFabClientAPI.SubtractUserVirtualCurrency(request,
                result => onSuccess?.Invoke(result.Balance),
                error => Debug.LogError($"❌ Subtract {currencyKey} Error: {error.GenerateErrorReport()}")
            );
        }
    }

    #endregion
}
