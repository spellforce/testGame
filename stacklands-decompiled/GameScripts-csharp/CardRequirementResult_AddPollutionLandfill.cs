using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_AddPollutionLandfill : CardRequirementResult
{
	public int Amount;

	public bool IsNegative;

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		return null;
	}

	public override RequirementType GetRequirementType()
	{
		return RequirementType.Pollution;
	}

	public override IEnumerator Perform(GameCard card)
	{
		Landfill landfill = card.CardData as Landfill;
		if ((Object)(object)landfill != (Object)null && landfill.StoredPollution >= Amount)
		{
			landfill.StoredPollution += Amount;
		}
		card.CardData.UpdateRequirementResultsInStack(RequirementType.Pollution, -Amount, card);
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (IsNegative)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
		}
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (IsNegative)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
		}
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
	}
}
