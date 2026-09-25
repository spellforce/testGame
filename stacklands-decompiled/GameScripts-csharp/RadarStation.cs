public class RadarStation : CardData
{
	public override void UpdateCard()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		int currentMonth = WorldManager.instance.CurrentMonth;
		int nextConflictMonth = CitiesManager.instance.NextConflictMonth;
		if (currentMonth >= nextConflictMonth - 3 && currentMonth < nextConflictMonth)
		{
			descriptionOverride = SokLoc.Translate(DescriptionTerm) + ". " + SokLoc.Translate("statuseffect_radar_description", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", (CitiesManager.instance.NextConflictMonth - 1).ToString()) });
			AddStatusEffect(new StatusEffect_Radar());
		}
		else
		{
			RemoveStatusEffect<StatusEffect_Radar>();
		}
		base.UpdateCard();
	}
}
