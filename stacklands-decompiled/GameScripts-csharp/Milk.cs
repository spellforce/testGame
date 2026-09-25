using UnityEngine;

public class Milk : Food
{
	public override void StoppedDragging()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == "feral_cat" && WorldManager.instance.IsSpiritDlcActive())
		{
			MyGameCard.Parent.DestroyCard();
			MyGameCard.DestroyCard();
			CardData cardData = WorldManager.instance.CreateCard(((Component)this).transform.position, "cat");
			WorldManager.instance.CreateSmoke(((Component)cardData).transform.position);
			cardData.MyGameCard.SendIt();
		}
		else
		{
			base.StoppedDragging();
		}
	}
}
