using UnityEngine;

public class Crab : Animal
{
	public override void Die()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			WorldManager.instance.CurrentRunVariables.CrabsKilled++;
			if (WorldManager.instance.CurrentRunVariables.CrabsKilled % 3 == 0)
			{
				CardData mommaCrab = WorldManager.instance.CreateCard(((Component)this).transform.position, "momma_crab", faceUp: false, checkAddToStack: false);
				WorldManager.instance.QueueCutscene(Cutscenes.MommaCrab(mommaCrab));
			}
		}
		base.Die();
	}
}
