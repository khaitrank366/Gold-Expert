using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class SlotReel2 : MonoBehaviour
{

	public GameObject symbolPrefab;
	public Sprite[] symbolSprites;
	public int symbolCount = 7;
	public float symbolHeight = 150f;
	public float gap = 20f;
	public int? forcedWinSpriteIndex = null;
	private float effectiveSpacing => symbolHeight + gap;
	private float loopHeight => effectiveSpacing * symbolCount;
	private List<SymbolSlot> symbols = new();

	public void Init()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);
		symbols.Clear();

		float centerIndex = (symbolCount - 1) / 2f;

		for (int i = 0; i < symbolCount; i++)
		{
			GameObject symbol = Instantiate(symbolPrefab, transform);
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

			var logic = symbol.AddComponent<SymbolSlot>();
			logic.loopHeight = loopHeight;
			symbols.Add(logic);
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

	public void StopWithTween(float duration = 0.5f)
	{
		Stop();

		SymbolSlot target = null;
		float offset = 0f;

		if (forcedWinSpriteIndex != null)
		{
			foreach (var s in symbols)
			{
				Sprite sprite = s.GetComponent<Image>().sprite;
				int spriteIndex = System.Array.IndexOf(symbolSprites, sprite);
				if (spriteIndex == forcedWinSpriteIndex.Value)
				{
					target = s;
					offset = s.transform.localPosition.y;
					break;
				}
			}
		}

		if (target == null)
		{
			float minDistance = float.MaxValue;
			foreach (var s in symbols)
			{
				float dist = Mathf.Abs(s.transform.localPosition.y);
				if (dist < minDistance)
				{
					minDistance = dist;
					target = s;
					offset = s.transform.localPosition.y;
				}
			}
		}

		if (target != null)
		{
			foreach (var s in symbols)
			{
				s.transform.DOKill();
				Vector3 current = s.transform.localPosition;
				Vector3 destination = current - new Vector3(0, offset, 0);

				s.transform.DOLocalMove(destination, duration).SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						if (Mathf.Abs(s.transform.localPosition.y) < 0.01f)
						{
							s.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
						}
					});
			}
		}
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

}
