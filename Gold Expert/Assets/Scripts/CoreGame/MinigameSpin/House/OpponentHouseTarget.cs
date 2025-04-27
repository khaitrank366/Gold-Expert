using UnityEngine;
using UnityEngine.UI;

public class OpponentHouseTarget : MonoBehaviour
{
	public Image image;
	public Button button;
	private int index;

	public void Setup(Sprite sprite, int i)
	{
		image.sprite = sprite;
		index = i;
		button.interactable = true;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(() => OpponentHouseUI.Instance.OnTargetSelected(index));
	}

	public void Disable()
	{
		button.interactable = false;
	}
}
