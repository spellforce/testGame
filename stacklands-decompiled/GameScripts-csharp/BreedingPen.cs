using UnityEngine;

public class BreedingPen : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		switch (MyGameCard.GetChildCount())
		{
		case 0:
			if (otherCard is Animal animal)
			{
				return animal.IsBreedable;
			}
			return false;
		case 1:
			return MyGameCard.Child.CardData.Id == otherCard.Id;
		default:
			return false;
		}
	}

	public override void UpdateCard()
	{
		if (MyGameCard.GetChildCount() == 2)
		{
			MyGameCard.StartTimer(120f, BreedAnimals, SokLoc.Translate("action_breeding_status"), GetActionId("BreedAnimals"));
		}
		else if (MyGameCard.GetChildCount() > 2)
		{
			GameCard gameCard = MyGameCard.TryGetNthChild(3);
			if ((Object)(object)gameCard != (Object)null)
			{
				gameCard.RemoveFromParent();
			}
			MyGameCard.CancelTimer(GetActionId("BreedAnimals"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("BreedAnimals"));
		}
		base.UpdateCard();
	}

	[TimedAction("breed_animals")]
	public void BreedAnimals()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(((Component)this).transform.position, MyGameCard.Child.CardData.Id);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
		GameCard child = MyGameCard.Child;
		if ((Object)(object)child.Child != (Object)null)
		{
			GameCard child2 = child.Child;
			child2.RemoveFromStack();
			WorldManager.instance.StackSend(child2, OutputDir);
		}
		QuestManager.instance.SpecialActionComplete("breed_" + cardData.Id);
		child.RemoveFromStack();
		WorldManager.instance.StackSend(child, OutputDir);
	}
}
