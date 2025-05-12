using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class SlotReel2 : MonoBehaviour
{
	[Header("References")]
	public GameObject symbolPrefab;
	public Sprite[] symbolSprites;

	[Header("Settings")]
	public int symbolCount = 7;
	public float symbolHeight = 150f;
	public float gap = 20f;

	public float bounceScale = 1.2f;
	public float bounceDuration = 0.2f;

	private float effectiveSpacing => symbolHeight + gap;
	private float loopHeight => effectiveSpacing * symbolCount;
	private List<SymbolSlot> symbols = new();
	private Queue<SymbolSlot> symbolPool = new();
	private int? forcedWinSpriteIndex = null;
	private Sequence currentAnimation;

	public int? ForcedWinSpriteIndex
	{
		get => forcedWinSpriteIndex;
		set => forcedWinSpriteIndex = value;
	}

	public void Init()
	{
		ResetReel();
		CreateSymbols();
	}

	private void ResetReel()
	{
		// Return all symbols to pool
		foreach (var symbol in symbols)
		{
			symbol.gameObject.SetActive(false);
			symbolPool.Enqueue(symbol);
		}
		symbols.Clear();
		currentAnimation?.Kill();
	}

	private SymbolSlot GetSymbolFromPool()
	{
		SymbolSlot symbol;
		if (symbolPool.Count > 0)
		{
			symbol = symbolPool.Dequeue();
			symbol.gameObject.SetActive(true);
		}
		else
		{
			GameObject newSymbol = Instantiate(symbolPrefab, transform);
			symbol = newSymbol.AddComponent<SymbolSlot>();
		}
		return symbol;
	}

	private void CreateSymbols()
	{
		float centerIndex = (symbolCount - 1) / 2f;

		for (int i = 0; i < symbolCount; i++)
		{
			SymbolSlot symbol = GetSymbolFromPool();
			float y = (centerIndex - i) * effectiveSpacing;
			symbol.transform.localPosition = new Vector3(0, y, 0);

			Image img = symbol.GetComponent<Image>();
			if (i == Mathf.RoundToInt(centerIndex) && forcedWinSpriteIndex != null)
			{
				
				img.sprite = symbolSprites[forcedWinSpriteIndex.Value];
			}
			else
			{
				int r;
				do
				{
					r = Random.Range(0, symbolSprites.Length);
				}
				while (forcedWinSpriteIndex != null && r == forcedWinSpriteIndex.Value);

				img.sprite = symbolSprites[r];
			}

			symbol.loopHeight = loopHeight;
			symbols.Add(symbol);
		}
	}

	public void SetSpeed(float speed)
	{
		foreach (var s in symbols)
			s.SetSpeed(speed);
	}

	public void Stop()
	{
		foreach (var s in symbols)
			s.StopMoving();
	}

	// Phương thức này dùng để dừng các symbol trong reel với hiệu ứng chuyển động mượt mà
	public void StopWithTween(float duration = 0.5f) 
	{
		// Dừng chuyển động hiện tại của các symbol
		Stop();
		// Hủy animation đang chạy (nếu có)
		currentAnimation?.Kill();

		// Tìm symbol đích cần căn chỉnh (symbol sẽ nằm ở giữa)
		SymbolSlot target = FindTargetSymbol();
		if (target == null) return;

		// Lấy khoảng cách cần dịch chuyển để đưa symbol đích về vị trí giữa (y=0)
		float offset = target.transform.localPosition.y;
		// Tạo sequence animation mới
		currentAnimation = DOTween.Sequence();

		// Xử lý từng symbol trong reel
		foreach (var s in symbols)
		{
			// Hủy animation cũ của symbol này
			s.transform.DOKill();
			// Lấy vị trí hiện tại
			Vector3 current = s.transform.localPosition;
			// Tính vị trí đích đến = vị trí hiện tại - offset
			Vector3 destination = current - new Vector3(0, offset, 0);

			// Thêm animation di chuyển vào sequence
			currentAnimation.Join(s.transform.DOLocalMove(destination, duration)
				.SetEase(Ease.OutCubic) // Hiệu ứng chuyển động mượt mà
				.OnComplete(() => // Khi di chuyển xong
				{
					// Nếu symbol này nằm ở giữa (y gần bằng 0)
					if (Mathf.Abs(s.transform.localPosition.y) < 0.01f)
					{
						s.transform.DOKill();
						// Thêm hiệu ứng co giãn cho symbol ở giữa
						s.transform.DOScale(bounceScale, bounceDuration)
							.SetLoops(2, LoopType.Yoyo) // Lặp 2 lần (phóng to rồi thu nhỏ)
							.SetEase(Ease.OutBack); // Hiệu ứng co giãn mượt mà
					}
				}));
		}
	}

	private SymbolSlot FindTargetSymbol()
	{
		if (forcedWinSpriteIndex != null)
		{
			foreach (var s in symbols)
			{
				Sprite sprite = s.GetComponent<Image>().sprite;
				int spriteIndex = System.Array.IndexOf(symbolSprites, sprite);
				if (spriteIndex == forcedWinSpriteIndex.Value)
				{
					return s;
				}
			}
		}

		float minDistance = float.MaxValue;
		SymbolSlot target = null;
		foreach (var s in symbols)
		{
			float dist = Mathf.Abs(s.transform.localPosition.y);
			if (dist < minDistance)
			{
				minDistance = dist;
				target = s;
			}
		}

		return target;
	}

	public Sprite GetCenterSymbol()
	{
		float minDistance = float.MaxValue;
		Sprite centerSprite = null;

		foreach (var s in symbols)
		{
			float dist = Mathf.Abs(s.transform.localPosition.y);
			if (dist < minDistance)
			{
				minDistance = dist;
				centerSprite = s.GetComponent<Image>().sprite;
			}
		}

		return centerSprite;
	}

	private void OnDestroy()
	{
		currentAnimation?.Kill();
		// Clean up pool
		while (symbolPool.Count > 0)
		{
			var symbol = symbolPool.Dequeue();
			if (symbol != null)
				Destroy(symbol.gameObject);
		}
	}
}
