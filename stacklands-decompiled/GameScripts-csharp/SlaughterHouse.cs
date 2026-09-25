using UnityEngine;

public class SlaughterHouse : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		int num = GetChildCount() + (1 + otherCard.GetChildCount());
		if (otherCard is Animal)
		{
			return num <= 5;
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && MyGameCard.Child.CardData is Animal)
		{
			MyGameCard.StartTimer(60f, SlaughterAnimal, SokLoc.Translate("action_slaughtering_status"), GetActionId("SlaughterAnimal"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("SlaughterAnimal"));
		}
		base.UpdateCard();
	}

	[TimedAction("slaughter_animal")]
	public void SlaughterAnimal()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.HasChild && MyGameCard.Child.CardData is Animal)
		{
			GameCard child = MyGameCard.Child;
			RemoveFirstChildFromStack();
			child.DestroyCard();
			CardData cardData = ((child.CardData.MyCardType == CardType.Fish) ? WorldManager.instance.CreateCard(((Component)this).transform.position, "raw_fish") : ((!(child.CardData.Id == "crab")) ? WorldManager.instance.CreateCard(((Component)this).transform.position, "raw_meat") : WorldManager.instance.CreateCard(((Component)this).transform.position, "raw_crab_meat")));
			WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
			WorldManager.instance.CreateSmoke(((Component)this).transform.position);
		}
	}
}
