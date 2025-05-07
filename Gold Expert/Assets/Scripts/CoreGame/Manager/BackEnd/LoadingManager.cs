using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingManager : Singleton<LoadingManager>
{
	#region ----Enums----
	private enum GameState
	{
		Init,
		LoadCurrencyData,
		LoadBuildingData,
		Done
	}
	#endregion

	#region ----Variables----
	[SerializeField]
	private GameState dataGameState;
	private bool isDataLoading = false;
	#endregion
			
	public async Task LoadGameData()
	{
		try
		{
			// Bắt đầu từ trạng thái Init
			SetState(GameState.Init);
			
			//Load Currency Data
			SetState(GameState.LoadCurrencyData);
			await CurrencyManager.Instance.LoadCurrencies();

			// Load Building Data
			SetState(GameState.LoadBuildingData);
			await BuildingManager.Instance.Load();

			// Hoàn thành
			SetState(GameState.Done);
			Debug.Log("✅ Load game data completed");
		}
		catch (System.Exception e)
		{
			Debug.LogError($"❌ Error loading game data: {e.Message}");
		}
		finally
		{
			isDataLoading = false;
		}
	}

	private void SetState(GameState state)
	{
		dataGameState = state;
		Debug.Log($"🔄 Loading State: {state}");
	}

	public bool IsDone()
	{
		return dataGameState == GameState.Done;
	}
}
