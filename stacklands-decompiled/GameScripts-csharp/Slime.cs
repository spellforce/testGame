using UnityEngine;

public class Slime : Enemy
{
	public override void Die()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			WorldManager.instance.CreateCard(((Component)this).transform.position, "small_slime", faceUp: true, checkAddToStack: false).MyGameCard.SendIt();
		}
		base.Die();
	}
}
