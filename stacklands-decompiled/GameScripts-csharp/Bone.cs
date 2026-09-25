using UnityEngine;

public class Bone : Resource
{
	public override void StoppedDragging()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == "wolf")
		{
			MyGameCard.Parent.DestroyCard();
			MyGameCard.DestroyCard();
			CardData cardData = WorldManager.instance.CreateCard(((Component)this).transform.position, "dog");
			WorldManager.instance.CreateSmoke(((Component)cardData).transform.position);
			cardData.MyGameCard.SendIt();
		}
		else
		{
			base.StoppedDragging();
		}
	}
}
