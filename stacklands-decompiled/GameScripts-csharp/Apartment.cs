using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Apartment : EnergyConsumer
{
	public AudioClip SpawnWorkerSound;

	public int HousingSpace = 2;

	[HideInInspector]
	public int FreeSpace;

	[HideInInspector]
	[ExtraData("used_space")]
	public int UsedSpace;

	public bool CanHouseRobotWorkers;

	private float updateTimer = 1f;

	public void UpdateUsedSpace()
	{
		int num = 0;
		foreach (HousingConsumer housingConsumer in CitiesManager.instance.HousingConsumers)
		{
			if ((Object)(object)housingConsumer.Housing == (Object)(object)this)
			{
				num += housingConsumer.GetHousingSpaceRequired();
			}
		}
		UsedSpace = num;
	}

	public override void UpdateCard()
	{
		updateTimer -= Time.deltaTime;
		if (updateTimer <= 0f)
		{
			updateTimer = Random.Range(0.5f, 1f);
			UpdateUsedSpace();
		}
		FreeSpace = HousingSpace - UsedSpace;
		if (FreeSpace > 0)
		{
			if (!HasStatusEffectOfType<StatusEffect_Space>())
			{
				AddStatusEffect(new StatusEffect_Space());
			}
		}
		else
		{
			RemoveStatusEffect<StatusEffect_Space>();
		}
		if (CitiesManager.instance.HomelessHousingConsumers.Count > 0 && FreeSpace > 0)
		{
			for (int i = 0; i < CitiesManager.instance.HomelessHousingConsumers.Count; i++)
			{
				HousingConsumer housingConsumer = CitiesManager.instance.HomelessHousingConsumers[i];
				if (housingConsumer != null)
				{
					if (housingConsumer.GetGameCard().Destroyed || (housingConsumer.GetWorkerType() == WorkerType.Robot && !CanHouseRobotWorkers) || (CanHouseRobotWorkers && housingConsumer.GetWorkerType() != WorkerType.Robot))
					{
						continue;
					}
					if (housingConsumer.GetHousingSpaceRequired() <= FreeSpace)
					{
						housingConsumer.Housing = this;
						UsedSpace += housingConsumer.GetHousingSpaceRequired();
						FreeSpace = HousingSpace - UsedSpace;
						CitiesManager.instance.HomelessHousingConsumers.RemoveAt(i);
					}
				}
				if (FreeSpace <= 0)
				{
					break;
				}
			}
		}
		if (MyGameCard.HasChild && MyGameCard.Child.CardData is ICurrency)
		{
			if (ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList().Sum((ICurrency x) => x.CurrencyValue) >= 20)
			{
				if (!MyGameCard.TimerRunning)
				{
					MyGameCard.StartTimer(5f, NewWorker, SokLoc.Translate("label_recruiting_worker"), GetActionId("NewWorker"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("NewWorker"));
			}
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("NewWorker"));
		}
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		descriptionOverride = SokLoc.Translate(DescriptionTerm, (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", HousingSpace.ToString()) });
		if (FreeSpace != 0 && (Object)(object)MyGameCard != (Object)null && !MyGameCard.IsDemoCard)
		{
			descriptionOverride = descriptionOverride + ". " + SokLoc.Translate("card_apartment_free_space", (LocParam[])(object)new LocParam[1] { LocParam.Create("free", FreeSpace.ToString()) });
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Apartment || otherCard is ICurrency || otherCard.Id == "copper_bar")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	[TimedAction("new_worker")]
	public void NewWorker()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		List<ICurrency> list = ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList();
		if (list.Sum((ICurrency x) => x.CurrencyValue) >= 20)
		{
			CitiesManager.instance.TryUseDollars(list, 20, onlyTakeIfAmountMet: true, spawnSmoke: true);
			CardData cardData = WorldManager.instance.CreateCard(((Component)this).transform.position, "worker");
			cardData.MyGameCard.RemoveFromStack();
			WorldManager.instance.StackSend(cardData.MyGameCard, OutputDir);
		}
	}
}
