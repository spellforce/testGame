using UnityEngine;

public class Market : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if ((Object)(object)otherCard.MyGameCard == (Object)null)
		{
			return otherCard.Value > 0;
		}
		return WorldManager.instance.CardCanBeSold(otherCard.MyGameCard);
	}

	public override void UpdateCard()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.HasChild && WorldManager.instance.CardCanBeSold(MyGameCard.Child, checkStatus: false) && (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation))
		{
			string status = SokLoc.Translate("new_selling_card", (LocParam[])(object)new LocParam[1] { LocParam.Create("card", MyGameCard.Child.CardData.FullName) });
			MyGameCard.StartTimer(60f, SellWithMarket, status, GetActionId("SellWithMarket"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("SellWithMarket"));
		}
		base.UpdateCard();
	}

	[TimedAction("sell_with_market")]
	public void SellWithMarket()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		GameCard child = MyGameCard.Child;
		if (!((Object)(object)child == (Object)null))
		{
			GameCard gameCard = null;
			if (child.HasChild && WorldManager.instance.CardCanBeSold(child.Child))
			{
				gameCard = child.Child;
			}
			child.RemoveFromStack();
			if ((Object)(object)gameCard != (Object)null)
			{
				MyGameCard.SetChild(gameCard);
			}
			QuestManager.instance.SpecialActionComplete("sell_at_market", this);
			GameCard gameCard2 = WorldManager.instance.SellCard(((Component)this).transform.position, child, 2f, checkAddToStack: false);
			if ((Object)(object)gameCard2 != (Object)null)
			{
				WorldManager.instance.StackSendCheckTarget(MyGameCard, gameCard2.GetRootCard(), OutputDir, MyGameCard);
			}
		}
	}
}
