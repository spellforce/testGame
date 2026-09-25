public class Corpse : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Corpse;
	}

	public override void UpdateCardText()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		string text = SokLoc.Translate(NameTerm);
		if (!string.IsNullOrEmpty(CustomName))
		{
			text = SokLoc.Translate("card_corpse_name_long", (LocParam[])(object)new LocParam[1] { LocParam.Create("name", CustomName) });
		}
		nameOverride = text;
	}
}
