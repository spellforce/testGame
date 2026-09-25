using System;

[Serializable]
public class CardRequirement_TakeFood : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_food", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", text) });
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_food_negative", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", text) });
	}

	public override bool Satisfied(GameCard card)
	{
		return CitiesManager.instance.GetFoodToUse(Amount).Count > 0;
	}
}
