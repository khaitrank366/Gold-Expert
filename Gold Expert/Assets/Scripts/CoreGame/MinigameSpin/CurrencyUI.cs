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
            PlayFabManager.Instance.OnCoinChanged += UpdateCoinUI;
            PlayFabManager.Instance.OnLightningChanged += UpdateLightningUI;

            UpdateCoinUI(PlayFabManager.Instance.Coin);
            UpdateLightningUI(PlayFabManager.Instance.Lightning);
        }
    }

    void OnDisable()
    {
        if (PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.OnCoinChanged -= UpdateCoinUI;
            PlayFabManager.Instance.OnLightningChanged -= UpdateLightningUI;
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
