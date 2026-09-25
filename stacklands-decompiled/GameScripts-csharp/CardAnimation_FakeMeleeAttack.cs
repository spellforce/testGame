using UnityEngine;

public class CardAnimation_FakeMeleeAttack : CardAnimation
{
	private GameCard startCard;

	private GameCard endCard;

	private bool attacked;

	public CardAnimation_FakeMeleeAttack(GameCard start, GameCard end)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		startCard = start;
		endCard = end;
		StartPosition = startCard.Position;
		EndPosition = endCard.Position;
		Position = (TargetPosition = StartPosition);
	}

	public override void Update()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		timer += Time.deltaTime * WorldManager.instance.CombatSpeed;
		float num = WorldManager.instance.CombatFlatPositionCurve.Evaluate(timer);
		float num2 = WorldManager.instance.CombatYPosition.Evaluate(timer);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Lerp(StartPosition.x, EndPosition.x, num);
		zero.y = EndPosition.y + num2;
		zero.z = Mathf.Lerp(StartPosition.z, EndPosition.z, num);
		Position = (TargetPosition = zero);
		if (timer >= 0.5f && !attacked)
		{
			attacked = true;
			endCard.SetHitEffect();
			AudioManager.me.PlaySound2D(AudioManager.me.HitMelee, Random.Range(0.8f, 1.2f), 0.2f);
		}
		if (timer >= 1f)
		{
			IsDone = true;
		}
	}
}
