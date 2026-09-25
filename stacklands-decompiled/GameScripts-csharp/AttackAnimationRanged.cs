using UnityEngine;

public class AttackAnimationRanged : AttackAnimation
{
	public override void Start()
	{
		Projectile projectile = Origin.CreateProjectile(PrefabManager.instance.RangedProjectilePrefab, Target, this);
		SetKnockback(projectile);
		AudioManager.me.PlaySound2D(AudioManager.me.RangedRelease, Random.Range(0.8f, 1.2f), 0.3f);
		base.Start();
	}

	public override void Update()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Position = (TargetPosition = AttackStartPosition + knockback);
		base.Update();
	}
}
