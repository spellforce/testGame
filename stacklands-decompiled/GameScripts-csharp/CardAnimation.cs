using UnityEngine;

public class CardAnimation
{
	public Vector3 StartPosition;

	public Vector3 EndPosition;

	public Vector3 Position;

	public Vector3 TargetPosition;

	public bool IsDone;

	public bool HasStarted;

	protected float timer;

	public bool IsBlocking = true;

	public virtual void Start()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Position = (TargetPosition = StartPosition);
	}

	public virtual void Update()
	{
		timer += Time.deltaTime;
	}
}
