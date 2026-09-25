using System.Linq;
using UnityEngine;

public class BaseVillager : Combatable
{
	public bool CanOverrideCardFromEquipment;

	[ExtraData("age")]
	public int Age;

	public bool ChangesCardOnStage;

	[HideInInspector]
	public bool AteUncookedFood;

	public bool CanBreed = true;

	public LifeStage MyLifeStage
	{
		get
		{
			if (!WorldManager.instance.CurseIsActive(CurseType.Death))
			{
				return LifeStage.Adult;
			}
			return DetermineLifeStageFromAge(Age);
		}
	}

	public override bool HasInventory
	{
		get
		{
			if (Id == "trained_monkey")
			{
				return false;
			}
			return true;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is BaseVillager) && otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Equipable && !(otherCard is Food { CanBePlacedOnVillager: not false }))
		{
			return otherCard.Id == "naming_stone";
		}
		return true;
	}

	public override void UpdateCardText()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		descriptionOverride = SokLoc.Translate(DescriptionTerm);
		descriptionOverride += "\n\n";
		if (WorldManager.instance.CurseIsActive(CurseType.Death))
		{
			descriptionOverride = descriptionOverride + "<i>" + SokLoc.Translate("label_villager_age_description", (LocParam[])(object)new LocParam[1] { LocParam.Plural("age", Age + 1) }) + "<i>\n";
		}
		descriptionOverride = descriptionOverride + "<i>" + GetCombatableDescription() + "</i>";
		if (AdvancedSettingsScreen.AdvancedCombatStatsEnabled || GameCanvas.instance.CurrentScreen is CardopediaScreen)
		{
			descriptionOverride = descriptionOverride + "\n\n<i>" + GetCombatableDescriptionAdvanced() + "</i>";
		}
		bool flag = !ChangesCardOnStage || !string.IsNullOrEmpty(CustomName);
		string text = NameTerm;
		if (flag)
		{
			if (MyLifeStage == LifeStage.Adult)
			{
				text = NameTerm;
			}
			else if (MyLifeStage == LifeStage.Teenager)
			{
				text = NameTerm + "_young";
			}
			else if (MyLifeStage == LifeStage.Elderly)
			{
				text = NameTerm + "_old";
			}
			else if (MyLifeStage == LifeStage.Dead)
			{
				text = NameTerm + "_old";
			}
		}
		nameOverride = SokLoc.Translate(text);
		if (string.IsNullOrEmpty(CustomName))
		{
			return;
		}
		if (flag)
		{
			if (MyLifeStage == LifeStage.Adult)
			{
				nameOverride = CustomName;
			}
			else if (MyLifeStage == LifeStage.Teenager)
			{
				nameOverride = SokLoc.Translate("label_villager_young", (LocParam[])(object)new LocParam[1] { LocParam.Create("villager", CustomName) });
			}
			else if (MyLifeStage == LifeStage.Elderly)
			{
				nameOverride = SokLoc.Translate("label_villager_old", (LocParam[])(object)new LocParam[1] { LocParam.Create("villager", CustomName) });
			}
			else if (MyLifeStage == LifeStage.Dead)
			{
				nameOverride = SokLoc.Translate("label_villager_old", (LocParam[])(object)new LocParam[1] { LocParam.Create("villager", CustomName) });
			}
		}
		else
		{
			nameOverride = CustomName;
		}
	}

	public override void UpdateCard()
	{
		if (WorldManager.instance.TimeScale > 0f && !WorldManager.instance.InAnimation)
		{
			UpdateLifeStage();
		}
		base.UpdateCard();
	}

	public virtual int GetRequiredFoodCount()
	{
		if (Id == "trained_monkey")
		{
			return 0;
		}
		if (Id == "dog")
		{
			return 1;
		}
		return 2;
	}

	public override void Die()
	{
		WorldManager.instance.KillVillager(this);
		if (WorldManager.instance.GetCardCount<BaseVillager>() == 2 && MyLifeStage == LifeStage.Elderly)
		{
			WorldManager.instance.QueueCutsceneIfNotPlayed("death_middle_villager");
		}
		if (MyConflict != null)
		{
			MyConflict.LeaveConflict(this);
		}
	}

	public float GetActionTimeModifier(string actionId, CardData baseCard)
	{
		float num = 1f;
		ActionTimeParams parameters = new ActionTimeParams(this, actionId, baseCard);
		foreach (ActionTimeBase actionTimeBasis in WorldManager.instance.actionTimeBases)
		{
			if (actionTimeBasis.Matches(parameters))
			{
				num = actionTimeBasis.BaseSpeed;
			}
		}
		foreach (ActionTimeModifier actionTimeModifier in WorldManager.instance.actionTimeModifiers)
		{
			if (actionTimeModifier.Matches(parameters))
			{
				num *= actionTimeModifier.SpeedModifier;
			}
		}
		return num;
	}

	public override void OnEquipItem(Equipable equipable)
	{
		if (equipable.Id == "royal_crown")
		{
			WorldManager.instance.QueueCutscene(GreedCutscenes.GreedWearCrown());
			MyGameCard.Unequip(equipable);
			return;
		}
		if (CanOverrideCardFromEquipment && !string.IsNullOrEmpty(equipable.VillagerTypeOverride) && equipable.VillagerTypeOverride != Id)
		{
			WorldManager.instance.ChangeToCard(MyGameCard, equipable.VillagerTypeOverride);
		}
		base.OnEquipItem(equipable);
	}

	private Equipable GetOverrideEquipable()
	{
		if (!CanOverrideCardFromEquipment)
		{
			return null;
		}
		return GetAllEquipables().FirstOrDefault((Equipable x) => !string.IsNullOrEmpty(x.VillagerTypeOverride));
	}

	public override void OnUnequipItem(Equipable equipable)
	{
		if (CanOverrideCardFromEquipment)
		{
			if ((Object)(object)GetOverrideEquipable() == (Object)null && Id != "villager")
			{
				(WorldManager.instance.ChangeToCard(MyGameCard, "villager") as Villager).UpdateLifeStage();
			}
			else if ((Object)(object)GetOverrideEquipable() != (Object)null && GetOverrideEquipable().VillagerTypeOverride != Id)
			{
				(WorldManager.instance.ChangeToCard(MyGameCard, GetOverrideEquipable().VillagerTypeOverride) as BaseVillager).UpdateLifeStage();
			}
		}
		base.OnUnequipItem(equipable);
	}

	public void UpdateLifeStage()
	{
		if (ChangesCardOnStage)
		{
			string text = DetermineCardFromStage(MyLifeStage);
			if (text != null && text != Id)
			{
				WorldManager.instance.ChangeToCard(MyGameCard, text);
			}
		}
	}

	public string DetermineCardFromStage(LifeStage stage)
	{
		if (Id == "teenage_villager" || Id == "villager" || Id == "old_villager")
		{
			switch (stage)
			{
			case LifeStage.Teenager:
				return "teenage_villager";
			case LifeStage.Adult:
				return "villager";
			case LifeStage.Elderly:
				QuestManager.instance.SpecialActionComplete("villager_old");
				return "old_villager";
			}
		}
		if (Id == "puppy" || Id == "dog" || Id == "old_dog")
		{
			switch (stage)
			{
			case LifeStage.Teenager:
				return "puppy";
			case LifeStage.Adult:
				return "dog";
			case LifeStage.Elderly:
				return "old_dog";
			}
		}
		if (Id == "kitten" || Id == "cat" || Id == "old_cat")
		{
			switch (stage)
			{
			case LifeStage.Teenager:
				return "kitten";
			case LifeStage.Adult:
				return "cat";
			case LifeStage.Elderly:
				return "old_cat";
			}
		}
		return null;
	}

	public LifeStage DetermineLifeStageFromAge(int age)
	{
		if (age < 2)
		{
			return LifeStage.Teenager;
		}
		if (age <= 6)
		{
			return LifeStage.Adult;
		}
		if (age <= 8)
		{
			return LifeStage.Elderly;
		}
		return LifeStage.Dead;
	}

	public bool WillChangeLifeStage()
	{
		return DetermineLifeStageFromAge(Age) != DetermineLifeStageFromAge(Age + 1);
	}
}
