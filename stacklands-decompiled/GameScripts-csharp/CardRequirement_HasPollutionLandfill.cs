using System;
using UnityEngine;

[Serializable]
public class CardRequirement_HasPollutionLandfill : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_pollution_landfill", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", text) });
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_pollution_landfill_negative", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", text) });
	}

	public override bool Satisfied(GameCard card)
	{
		Landfill landfill = card.CardData as Landfill;
		RecyclingCenter recyclingCenter = card.CardData as RecyclingCenter;
		if ((Object)(object)landfill != (Object)null && landfill.StoredPollution >= Amount)
		{
			return true;
		}
		if ((Object)(object)recyclingCenter != (Object)null && recyclingCenter.StoredPollution >= Amount)
		{
			return true;
		}
		return false;
	}
}
