using UnityEngine;
using UnityEngine.UI;

public class OpponentHouseUI : MonoBehaviour
{
	public static OpponentHouseUI Instance;

	public CanvasGroup canvasGroup;
	public OpponentHouseTarget[] targets;
	public Text opponentNameText;

	private void Awake()
	{
		Instance = this;
		HideInstant();
	}

	public void ShowHouse(OpponentHouseData data)
	{
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;

		for (int i = 0; i < targets.Length; i++)
		{
			bool canBreak = data.breakableIndices == null || data.breakableIndices.Length == 0 || System.Array.Exists(data.breakableIndices, x => x == i);
			targets[i].Setup(data.houseSprites[i], i);
			targets[i].button.interactable = canBreak;
		}

		if (opponentNameText != null)
			opponentNameText.text = $"Đang phá nhà của: {data.opponentName}";
	}

    public void OnTargetSelected(int index)
    {
        foreach (var t in targets)
            t.Disable();

        PlayFabManager.Instance.AddCoin(1000);
        Debug.Log($"💥 Phá slot {index} → nhận 1000 coin");

        Hide();
        SlotMachine2.Instance.BackToSpin();
    }


    public void Hide()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	public void HideInstant()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
}
