using UnityEngine;

public class Tentacle : Enemy
{
	protected override void Move()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector2 val = ((Vector2)(ref insideUnitCircle)).normalized * 1f;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(val.x, 0f, val.y);
		MyGameCard.Velocity = new Vector3(val2.x, 0f, val2.z);
	}

	public override void Die()
	{
		bool flag = false;
		if (WorldManager.instance.GetCardCount<Tentacle>() == 1)
		{
			flag = true;
		}
		if (flag)
		{
			WorldManager.instance.QueueCutscene(Cutscenes.SpawnKraken());
		}
		base.Die();
	}
}
