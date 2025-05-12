using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI lightningText;
    [SerializeField] private TextMeshProUGUI shieldText;
    void Start()
    {
        if (PlayFabManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinChanged += UpdateCoinUI;
            CurrencyManager.Instance.OnLightningChanged += UpdateLightningUI;
            CurrencyManager.Instance.OnShieldChanged += UpdateShieldUI;

            UpdateCoinUI(CurrencyManager.Instance.CurrentCoin);
            UpdateLightningUI(CurrencyManager.Instance.CurrentLightning);
            UpdateShieldUI(CurrencyManager.Instance.CurrentShield);
        }
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinChanged -= UpdateCoinUI;
            CurrencyManager.Instance.OnLightningChanged -= UpdateLightningUI;
            CurrencyManager.Instance.OnShieldChanged -= UpdateShieldUI;
        }
    }

    void UpdateCoinUI(int value)
    {
        coinText.text = value.ToString();
    }

    void UpdateLightningUI(int value)
    {
        lightningText.text = value.ToString();
    }

    void UpdateShieldUI(int value)
    {
        shieldText.text = value.ToString();
    }   
}
