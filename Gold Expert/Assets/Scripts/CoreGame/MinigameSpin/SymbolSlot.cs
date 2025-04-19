using UnityEngine;

public class SymbolSlot : MonoBehaviour
{
	public float speed = 0f;
	public float loopHeight = 1200f;
	private bool isMoving = false;

	public void SetSpeed(float s)
	{
		speed = s;
		isMoving = speed > 0f;
	}

	public void StopMoving()
	{
		speed = 0f;
		isMoving = false;
	}

	void Update()
	{
		if (!isMoving || loopHeight <= 0f) return;

		float delta = speed * Time.deltaTime;
		Vector3 newPos = transform.localPosition - new Vector3(0, delta, 0);

		if (newPos.y < -loopHeight / 2f)
		{
			float overshoot = newPos.y + loopHeight / 2f;
			newPos.y = loopHeight / 2f + (overshoot % loopHeight);
		}

		transform.localPosition = newPos;
	}
}
