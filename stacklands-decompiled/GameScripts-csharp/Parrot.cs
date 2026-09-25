using UnityEngine;

public class Parrot : Animal
{
	public override void StoppedDragging()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == "pirate")
		{
			CardData cardData = WorldManager.instance.ChangeToCard(MyGameCard.Parent, "friendly_pirate");
			MyGameCard.DestroyCard();
			WorldManager.instance.CreateSmoke(((Component)cardData).transform.position);
			cardData.MyGameCard.SendIt();
			QuestManager.instance.SpecialActionComplete("befriend_pirate");
		}
		else
		{
			base.StoppedDragging();
		}
	}
}
