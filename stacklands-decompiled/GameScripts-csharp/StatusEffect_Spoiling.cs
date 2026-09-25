using System.Collections.Generic;
using UnityEngine;

public class StatusEffect_Spoiling : StatusEffect
{
	protected override string TermId => "spoiling";

	public override Sprite Sprite => SpriteManager.instance.SpoilingEffect;

	public override void Update()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		FillAmount = 1f - StatusTimer / WorldManager.instance.MonthTime;
		float monthTime = WorldManager.instance.MonthTime;
		bool flag = (Object)(object)base.ParentCard.MyGameCard.GetCardWithStatusInStack() == (Object)null;
		if (WorldManager.instance.InAnimation)
		{
			flag = false;
		}
		if (StatusTimer >= monthTime && flag)
		{
			Food obj = base.ParentCard as Food;
			obj.FoodValue -= 2;
			if (obj.FoodValue <= 0)
			{
				CardData cardData = WorldManager.instance.CreateCard(((Component)base.ParentCard).transform.position, "goop", faceUp: false, checkAddToStack: false);
				WorldManager.instance.StackSend(cardData.MyGameCard, base.ParentCard.OutputDir);
				List<GameCard> allCardsInStack = base.ParentCard.MyGameCard.GetAllCardsInStack();
				allCardsInStack.Remove(base.ParentCard.MyGameCard);
				base.ParentCard.MyGameCard.DestroyCard(spawnSmoke: true, playSound: false);
				WorldManager.instance.Restack(allCardsInStack);
			}
			StatusTimer -= monthTime;
		}
		base.Update();
	}
}
