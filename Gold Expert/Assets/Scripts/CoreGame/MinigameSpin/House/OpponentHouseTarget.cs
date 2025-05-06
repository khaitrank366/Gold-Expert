using System;
using UnityEngine;
using UnityEngine.UI;

public class OpponentHouseTarget : MonoBehaviour
{
	public Image image;
	public Button button;
	private int index;
	
	public void Setup(Sprite sprite, int i,Action OnSelect)
	{
		image.sprite = sprite;
		index = i;
		button.interactable = true;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(() =>OnSelect());
	}

	public void Disable()
	{
		button.interactable = false;
	}
}
