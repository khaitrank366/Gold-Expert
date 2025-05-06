using UnityEngine;
using UnityEngine.UI;

public class OpponentHouseUI : MonoBehaviour,IModalUI
{
	public OpponentHouseTarget[] targets;
	public Text opponentNameText;
	

	public void ShowHouse(OpponentHouseData data)
	{
		Show();
		for (int i = 0; i < targets.Length; i++)
		{
			bool canBreak = data.breakableIndices == null || data.breakableIndices.Length == 0 || System.Array.Exists(data.breakableIndices, x => x == i);
			targets[i].Setup(data.houseSprites[i], i, () =>
			{
				OnTargetSelected(i);
			});
			targets[i].button.interactable = canBreak;
		}
		
		if (opponentNameText != null)
			opponentNameText.text = $"Đang phá nhà của: {data.opponentName}";
	}

    private void OnTargetSelected(int index)
    {
        foreach (var t in targets)
            t.Disable();

        CurrencyManager.Instance.AddCoin(1000);
        Debug.Log($"💥 Phá slot {index} → nhận 1000 coin");

        Hide();
        SlotMachine.Instance.BackToSpin();
    }


    public void Show()
    {	Debug.Log("concac");
	    this.gameObject.SetActive(true);
    }

    public void Hide()
	{
		this.gameObject.SetActive(false);
	}

	public bool IsVisible()
	{
		return this.gameObject.activeSelf;
	}


}
