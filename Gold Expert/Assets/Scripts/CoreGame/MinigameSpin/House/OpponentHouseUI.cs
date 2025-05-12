using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Newtonsoft.Json;

public class OpponentHouseUI : MonoBehaviour, IModalUI
{
	[SerializeField] private List<ElementOpponentConstruction> elementConstructions = new List<ElementOpponentConstruction>();
	public TextMeshProUGUI opponentNameText;
	

	public void ShowHouse(PlayerDataResponse userData)
	{
		if (userData?.playerData == null || !userData.playerData.ContainsKey("CurrentBuildingData"))
		{
			Debug.LogError("Invalid player data received");
			return;
		}

		Show();
		try
		{
			var buildingData = userData.playerData["CurrentBuildingData"].Value.ToString();
			var dataHouse = JsonConvert.DeserializeObject<PlayerMapProgress>(buildingData);
			
			if (dataHouse?.buildings == null)
			{
				Debug.LogError("Invalid building data");
				return;
			}

			var buildings = dataHouse.buildings.ToList();
			for (int i = 0; i < buildings.Count && i < elementConstructions.Count; i++)
			{
				var building = buildings[i];
				var element = elementConstructions[i];
				
				element.Setup(building.Key, building.Value, () => OnTargetSelected(i, userData.selectedPlayFabId));
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Error processing building data: {e.Message}");
		}
	}

	private void OnTargetSelected(int index, string targetId)
	{
		if (string.IsNullOrEmpty(targetId))
		{
			Debug.LogError("Invalid target ID");
			return;
		}

		HandleAPI.Instance.AttackBuilding(index, targetId);
		CurrencyManager.Instance.AddCoin(1000);
		Debug.Log($"💥 Attacked building {index} → received 1000 coins");

		Hide();
		SlotMachine.Instance.BackToSpin();
	}


	public void Show() => gameObject.SetActive(true);

	public void Hide() => gameObject.SetActive(false);

	public bool IsVisible() => gameObject.activeSelf;


}
