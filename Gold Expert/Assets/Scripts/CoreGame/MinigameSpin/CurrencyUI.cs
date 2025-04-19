using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI coinText;
	[SerializeField] private TextMeshProUGUI lightningText;

	void Start()
	{
		// Đợi đến khi mọi Awake() đã chạy
		if (SpinGameManager.Instance != null)
		{
			SpinGameManager.Instance.OnCoinChanged += UpdateCoinUI;
			SpinGameManager.Instance.OnLightningChanged += UpdateLightningUI;

			// Cập nhật lần đầu
			UpdateCoinUI(SpinGameManager.Instance.Coin);
			UpdateLightningUI(SpinGameManager.Instance.Lightning);
		}
		else
		{
			Debug.LogError("SpinGameManager.Instance vẫn null trong Start()");
		}
	}

	void OnDisable()
	{
		if (SpinGameManager.Instance != null)
		{
			SpinGameManager.Instance.OnCoinChanged -= UpdateCoinUI;
			SpinGameManager.Instance.OnLightningChanged -= UpdateLightningUI;
		}
	}

	void UpdateCoinUI(int value)
	{
		Debug.Log("UpdateCoinUI: " + value);
		if (coinText != null)
			coinText.text = value.ToString();
	}

	void UpdateLightningUI(int value)
	{
		Debug.Log("UpdateLightningUI: " + value);
		if (lightningText != null)
			lightningText.text = value.ToString();
	}
}
