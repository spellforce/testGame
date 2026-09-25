using UnityEngine;

public class Projectile : MonoBehaviour
{
	[HideInInspector]
	public Vector3 StartPosition;

	[HideInInspector]
	public Vector3 TargetPosition;

	[HideInInspector]
	public Combatable ShotBy;

	[HideInInspector]
	public Combatable Target;

	public AttackAnimation OriginAnimation;

	public float KnockbackMultiplier = 0.3f;

	public float Speed = 3f;

	public float WobbleSpeed = 1f;

	public float WobbleAmplitude = 0.1f;

	private float timer2;

	protected Vector3 position;

	private float distanceToTravel;

	protected virtual void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		position = ((Component)this).transform.position;
		Vector3 val = TargetPosition - StartPosition;
		distanceToTravel = ((Vector3)(ref val)).magnitude;
	}

	protected virtual void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = TargetPosition - StartPosition;
		timer2 += Time.deltaTime;
		((Component)this).transform.position = position + Extensions.Perlin(timer2 * WobbleSpeed) * WobbleAmplitude;
		((Component)this).transform.rotation = Quaternion.LookRotation(val);
		Vector3 val2 = position - StartPosition;
		if (((Vector3)(ref val2)).magnitude >= distanceToTravel)
		{
			if ((Object)(object)ShotBy != (Object)null)
			{
				ShotBy.PerformAttack(Target, OriginAnimation.AttackTargetPosition);
				OriginAnimation.IsDone = true;
			}
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
