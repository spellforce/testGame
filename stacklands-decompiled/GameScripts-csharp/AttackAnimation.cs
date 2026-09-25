using UnityEngine;

public class AttackAnimation
{
	public bool HasStarted;

	public bool IsDone;

	public Combatable Origin;

	public Combatable Target;

	public Vector3 AttackStartPosition;

	public Vector3 AttackTargetPosition;

	public Vector3 Position;

	public Vector3 TargetPosition;

	protected Vector3 knockback;

	public virtual bool IsBlocking { get; }

	public virtual void Start()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		HasStarted = true;
		Position = (TargetPosition = AttackStartPosition);
	}

	public virtual void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		knockback = Vector3.Lerp(knockback, Vector3.zero, Time.deltaTime * 6f * WorldManager.instance.TimeScale);
	}

	public virtual void SetKnockback(Projectile p)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = p.TargetPosition - p.StartPosition;
		knockback = -((Vector3)(ref val)).normalized * p.KnockbackMultiplier;
		if ((Object)(object)Origin != (Object)null)
		{
			Transform transform = ((Component)Origin.MyGameCard).transform;
			transform.localScale *= 0.9f;
		}
	}
}
