using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class LoadingManager :Singleton<LoadingManager>
{
 #region ----Enums----
	private enum GameState
	{
		LoadBuildingData,
		Done
	}
	#endregion

	#region ----Variables----
	[SerializeField]
	private GameState dataGameState;
	private bool isDataLoading = false;
	#endregion

	protected  void Update()
	{
		UpdateGameStates();
		// Debug.Log("DataLoadManager Update:" + dataGameState);
	}

	async void UpdateGameStates()
	{
		if (!IsDone() || isDataLoading == true) return;

		switch (dataGameState)
		{
			case GameState.LoadBuildingData:
				//await LoadTemplateData();
				BuildingManager.Instance.Load();				
				break;
		
		

			case GameState.Done:
				break;
		}
	}

	protected  void Init()
	{
		//SetState(GameState.LoadTemplateData);
	}

	private void SetState(GameState state)
	{
		dataGameState = state;
	}

	protected  bool IsDone()
	{
		return dataGameState == GameState.Done;
	}

	
   
}
