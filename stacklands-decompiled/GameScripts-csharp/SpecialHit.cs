using System;

[Serializable]
public class SpecialHit
{
	public float Chance = 1f;

	public SpecialHitType HitType;

	public SpecialHitTarget Target;

	public string GetText()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		string text = SokLoc.Translate("target_" + Target.ToString().ToLower());
		return SokLoc.Translate("specialhit_" + HitType.ToString().ToLower() + "_long", (LocParam[])(object)new LocParam[2]
		{
			LocParam.Create("chance", Chance.ToString()),
			LocParam.Create("target", text)
		});
	}

	public bool IsDebuff()
	{
		if (HitType == SpecialHitType.Poison || HitType == SpecialHitType.Stun || HitType == SpecialHitType.LifeSteal || HitType == SpecialHitType.Bleeding || HitType == SpecialHitType.Damage || HitType == SpecialHitType.Crit || HitType == SpecialHitType.Sick || HitType == SpecialHitType.Anxious)
		{
			return true;
		}
		return false;
	}
}
