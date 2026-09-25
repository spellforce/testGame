using System;

[Serializable]
public class CardRequirement_CardsExists : CardRequirement
{
	[Card]
	public string CardId;

	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(CardId);
		cardPrefab.UpdateCardText();
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_card", (LocParam[])(object)new LocParam[2]
		{
			LocParam.Create("amount", text),
			LocParam.Create("card", cardPrefab.Name)
		});
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(CardId);
		cardPrefab.UpdateCardText();
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_card_negative", (LocParam[])(object)new LocParam[2]
		{
			LocParam.Create("amount", text),
			LocParam.Create("card", cardPrefab.Name)
		});
	}

	public override bool Satisfied(GameCard card)
	{
		return WorldManager.instance.GetCards(CardId).Count >= Amount;
	}
}
