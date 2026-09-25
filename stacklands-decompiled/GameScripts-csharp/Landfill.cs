using UnityEngine;

public class Landfill : SewerCard
{
	public int PollutionOverflowMin;

	public int PollutionOverflowMax;

	[ExtraData("is_overflowing")]
	public bool IsOverflowing;

	public Sprite EmptyIcon;

	public Sprite HalfFullIcon;

	public Sprite FullIcon;

	[HideInInspector]
	[ExtraData("stored_pollution")]
	public int StoredPollution;

	[HideInInspector]
	[ExtraData("pollution_overflow")]
	public int PollutionOverflow;

	public int PollutionRemovalRate = 5;

	public override void OnInitialCreate()
	{
		PollutionOverflow = Random.Range(PollutionOverflowMin, PollutionOverflowMax);
		base.OnInitialCreate();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!IsOverflowing)
		{
			return otherCard.Id == "pollution";
		}
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public override void UpdateCardText()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOverflowing)
		{
			if (StoredPollution <= 0)
			{
				descriptionOverride = SokLoc.Translate("card_landfill_description", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", PollutionOverflowMin.ToString()) });
			}
			else
			{
				descriptionOverride = SokLoc.Translate("card_landfill_description_long", (LocParam[])(object)new LocParam[2]
				{
					LocParam.Create("amount", PollutionOverflowMin.ToString()),
					LocParam.Create("current", StoredPollution.ToString())
				});
			}
		}
		else
		{
			nameOverride = SokLoc.Translate("card_overflowing_landfill_name");
			descriptionOverride = SokLoc.Translate("card_overflowing_landfill_description");
		}
	}

	public override void UpdateCard()
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOverflowing)
		{
			if (MyGameCard.HasChild && AllChildrenMatchPredicate((CardData x) => x is Pollution))
			{
				foreach (Pollution item in ChildrenMatchingPredicate((CardData x) => x is Pollution))
				{
					StoredPollution += item.PollutionAmount;
					item.PollutionAmount -= item.PollutionAmount;
					if (item.PollutionAmount == 0)
					{
						item.MyGameCard.DestroyCard(spawnSmoke: true);
					}
					if (StoredPollution >= PollutionOverflow)
					{
						IsOverflowing = true;
						GameCamera.instance.Screenshake = 1f;
						item.MyGameCard.RemoveFromParent();
						AudioManager.me.PlaySound(AudioManager.me.LandfillOverflow, ((Component)this).transform, 1f, 0.3f);
						WorldManager.instance.QueueCutscene("cities_landfill_overflow");
						break;
					}
				}
			}
			if (!MyGameCard.TimerRunning && StoredPollution > 0)
			{
				MyGameCard.StartTimer(60f, DumpPollution, SokLoc.Translate("card_landfill_status_1", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", PollutionRemovalRate.ToString()) }), GetActionId("DumpPollution"));
			}
			if (StoredPollution >= PollutionOverflowMin / 2)
			{
				Icon = HalfFullIcon;
				MyGameCard.UpdateIcon();
			}
			else
			{
				Icon = EmptyIcon;
				MyGameCard.UpdateIcon();
			}
			MyGameCard.SpecialValue = StoredPollution;
		}
		else
		{
			Icon = FullIcon;
			MyGameCard.UpdateIcon();
			MyGameCard.CancelTimer(GetActionId("DumpPollution"));
			if (!MyGameCard.TimerRunning)
			{
				MyGameCard.StartTimer(120f, ResolveOverflow, SokLoc.Translate("card_landfill_status_2"), GetActionId("ResolveOverflow"));
			}
		}
		MyGameCard.SpecialIcon.sprite = SpriteManager.instance.PollutionIcon;
		base.UpdateCard();
	}

	[TimedAction("resolve_overflow")]
	public void ResolveOverflow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		WorldManager.instance.CreateSmoke(base.Position);
		StoredPollution = PollutionOverflow - 10;
		IsOverflowing = false;
	}

	[TimedAction("dump_pollution")]
	public void DumpPollution()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (StoredPollution > 0)
		{
			int num = Mathf.Min(StoredPollution, PollutionRemovalRate);
			StoredPollution -= num;
			AudioManager.me.PlaySound(AudioManager.me.ClearPollution, ((Component)this).transform, 1f, 0.3f);
			WorldManager.instance.CreateSmoke(base.Position);
		}
	}
}
