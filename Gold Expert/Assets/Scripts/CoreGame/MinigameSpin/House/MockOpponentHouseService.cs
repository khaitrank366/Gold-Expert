using UnityEngine;

public static class MockOpponentHouseService
{
	public static OpponentHouseData GetRandomHouse(Sprite[] availableSprites)
	{
		Sprite[] houseSprites = new Sprite[3];
		for (int i = 0; i < 3; i++)
		{
			int r = Random.Range(0, availableSprites.Length);
			houseSprites[i] = availableSprites[r];
		}

		return new OpponentHouseData
		{
			houseSprites = houseSprites,
			opponentName = GetRandomName(),
			breakableIndices = new int[] { 0, 1, 2 }
		};
	}

	private static string GetRandomName()
	{
		string[] names = { "nghi", "nghi1", "nghi2", "nghi3", "nghi4" };
		return names[Random.Range(0, names.Length)];
	}
}
