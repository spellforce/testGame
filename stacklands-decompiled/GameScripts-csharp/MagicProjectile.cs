using UnityEngine;

public class MagicProjectile : Projectile
{
	public float WaitTime = 0.5f;

	private float waitTimer;

	private Vector3 startScale;

	private bool knockedBack;

	protected override void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		startScale = ((Component)this).transform.localScale;
		((Component)this).transform.localScale = Vector3.zero;
	}

	protected override void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = TargetPosition - StartPosition;
		waitTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		if (waitTimer >= WaitTime)
		{
			position += ((Vector3)(ref val)).normalized * Speed * Time.deltaTime * WorldManager.instance.TimeScale;
			if (!knockedBack)
			{
				knockedBack = true;
				AudioManager.me.PlaySound2D(AudioManager.me.MagicRelease, Random.Range(0.8f, 1.2f), 0.5f);
				OriginAnimation.SetKnockback(this);
			}
		}
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, startScale, Time.deltaTime * 6f);
		base.Update();
	}
}
