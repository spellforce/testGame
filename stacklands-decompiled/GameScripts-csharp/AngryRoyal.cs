using UnityEngine;

public class AngryRoyal : Enemy
{
	public override void Die()
	{
		WorldManager.instance.QueueCutscene(GreedCutscenes.KillRoyalLiftCurse());
		base.Die();
	}

	public void DieInCutscene()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		WorldManager.instance.CreateCard(((Component)this).transform.position, "royal_crown");
		WorldManager.instance.CreateSmoke(((Component)this).transform.position);
		RemoveAllStatusEffects();
		WorldManager.instance.ChangeToCard(MyGameCard, "corpse");
	}
}
