public class ExportCenter : Factory
{
	public float ExportTime;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (WorldManager.instance.CardCanBeSold(otherCard.MyGameCard))
		{
			return otherCard.AllChildrenMatchPredicate((CardData x) => WorldManager.instance.CardCanBeSold(x.MyGameCard));
		}
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanToggleOnOff()
	{
		return true;
	}

	protected override bool CanSelectOutput()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && WorldManager.instance.CardCanBeSold(MyGameCard.GetLeafCard()) && !MyGameCard.TimerRunning)
		{
			MyGameCard.StartTimer(ExportTime, SellCard, SokLoc.Translate("card_export_center_status_1"), GetActionId("SellCard"));
		}
		else if (!MyGameCard.HasChild)
		{
			MyGameCard.CancelTimer(GetActionId("SellCard"));
		}
		base.UpdateCard();
	}

	[TimedAction("sell_card")]
	public void SellCard()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		GameCard leafCard = MyGameCard.GetLeafCard();
		leafCard.RemoveFromStack();
		GameCard gameCard = WorldManager.instance.SellCard(base.Position, leafCard, 1f, checkAddToStack: false);
		gameCard.RemoveFromParent();
		WorldManager.instance.StackSendCheckTarget(MyGameCard, gameCard, OutputDir);
	}
}
