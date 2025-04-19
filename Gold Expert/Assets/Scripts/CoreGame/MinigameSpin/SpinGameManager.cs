using UnityEngine;
using System;

public class SpinGameManager : MonoBehaviour
{
	public static SpinGameManager Instance { get; private set; }

	private int coin = 999;
	private int lightning = 10;

	public int Coin => coin;
	public int Lightning => lightning;

	public event Action<int> OnCoinChanged;
	public event Action<int> OnLightningChanged;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		Debug.Log("✅ SpinGameManager initialized");
	}


	// --- Coin ---
	public void AddCoin(int amount)
	{
		coin += amount;
		Debug.Log("🪙 Coin added: " + amount + " → total: " + coin);
		OnCoinChanged?.Invoke(coin);
	}

	public bool SpendCoin(int amount)
	{
		if (coin < amount) return false;
		coin -= amount;
		OnCoinChanged?.Invoke(coin);
		return true;
	}

	// --- Lightning ---
	public void AddLightning(int amount)
	{
		lightning += amount;
		OnLightningChanged?.Invoke(lightning);
	}

	public bool SpendLightning(int amount)
	{
		if (lightning < amount)
		{
			Debug.LogWarning("⚡ Not enough lightning!");
			return false;
		}

		lightning -= amount;
		Debug.Log("⚡ Lightning spent: " + amount + " → remaining: " + lightning);
		OnLightningChanged?.Invoke(lightning);
		return true;
	}
}
