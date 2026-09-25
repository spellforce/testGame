public class Dollar : Resource, ICurrency
{
	public int DollarValue;

	public CardData Card => this;

	public int CurrencyValue
	{
		get
		{
			return DollarValue;
		}
		set
		{
			DollarValue = value;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Dollar) && !(otherCard is Worker))
		{
			return otherCard is Resource;
		}
		return true;
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		nameOverride = SokLoc.Translate(NameTerm, (LocParam[])(object)new LocParam[1] { LocParam.Create("icon", Icons.Dollar) });
		descriptionOverride = SokLoc.Translate(DescriptionTerm, (LocParam[])(object)new LocParam[1] { LocParam.Create("icon", Icons.Dollar) });
	}

	public void UseCurrency(int currencyAmount, bool spawnSmoke = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (spawnSmoke)
		{
			WorldManager.instance.CreateSmoke(base.Position);
		}
		MyGameCard.DestroyCard();
	}
}
