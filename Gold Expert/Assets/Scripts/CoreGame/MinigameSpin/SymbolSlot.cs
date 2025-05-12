using UnityEngine;
using DG.Tweening;

public class SymbolSlot : MonoBehaviour
{
	public float loopHeight;
	private float speed = 0f;
	private bool isMoving = false;
	private Sequence moveSequence;

	private void Update()
	{
		if (!isMoving) return;

		transform.localPosition += Vector3.down * speed * Time.deltaTime;

		if (transform.localPosition.y <= -loopHeight / 2)
		{
			transform.localPosition += Vector3.up * loopHeight;
		}
	}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
		isMoving = speed > 0;
	}

	public void StopMoving()
	{
		isMoving = false;
		speed = 0f;
		moveSequence?.Kill();
	}

	private void OnDestroy()
	{
		moveSequence?.Kill();
	}
}
