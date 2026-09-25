using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_AddPollution : CardRequirementResult
{
	public int Amount;

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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Pollution obj = WorldManager.instance.CreateCard(card.Position, "pollution") as Pollution;
		obj.PollutionAmount = Amount;
		obj.MyGameCard.SendIt();
		card.CardData.UpdateRequirementResultsInStack(RequirementType.Pollution, -Amount, card);
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
	}
}
