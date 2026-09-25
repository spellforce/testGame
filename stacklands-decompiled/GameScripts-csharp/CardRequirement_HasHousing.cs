using System;
using UnityEngine;

[Serializable]
public class CardRequirement_HasHousing : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_housing", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", text) });
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_housing_negative", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", text) });
	}

	public override bool Satisfied(GameCard card)
	{
		if (card.CardData is HousingConsumer housingConsumer && ((Object)(object)housingConsumer.Housing == (Object)null || housingConsumer.Housing.IsDamaged || !housingConsumer.Housing.HasEnergyInput()))
		{
			return false;
		}
		return true;
	}
}
