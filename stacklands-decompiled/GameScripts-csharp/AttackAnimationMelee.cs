using UnityEngine;

public class AttackAnimationMelee : AttackAnimation
{
	private bool attacked;

	private float timer;

	public override bool IsBlocking => true;

	public override void Update()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		timer += Time.deltaTime * WorldManager.instance.TimeScale * WorldManager.instance.CombatSpeed;
		float num = WorldManager.instance.CombatFlatPositionCurve.Evaluate(timer);
		float num2 = WorldManager.instance.CombatYPosition.Evaluate(timer);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Lerp(AttackStartPosition.x, AttackTargetPosition.x, num);
		zero.y = AttackTargetPosition.y + num2;
		zero.z = Mathf.Lerp(AttackStartPosition.z, AttackTargetPosition.z, num);
		Position = (TargetPosition = zero);
		if (timer >= 0.5f && !attacked)
		{
			attacked = true;
			Origin.PerformAttack(Target, AttackTargetPosition);
		}
		if (timer >= 1f)
		{
			IsDone = true;
		}
		base.Update();
	}
}
