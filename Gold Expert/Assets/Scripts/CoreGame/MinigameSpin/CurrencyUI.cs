using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI lightningText;

    void Start()
    {
        if (PlayFabManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinChanged += UpdateCoinUI;
            CurrencyManager.Instance.OnLightningChanged += UpdateLightningUI;

            UpdateCoinUI(CurrencyManager.Instance.CurrentCoin);
            UpdateLightningUI(CurrencyManager.Instance.CurrentLightning);
        }
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinChanged -= UpdateCoinUI;
            CurrencyManager.Instance.OnLightningChanged -= UpdateLightningUI;
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
}
