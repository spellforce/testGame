using UnityEngine;

public class Animal : Mob
{
	[Header("Animal")]
	public float CreateTime = 10f;

	[Card]
	public string CreateCard;

	public bool IsBreedable;

	[ExtraData("age")]
	public int Age;

	public bool IsOld;

	[ExtraData("createtimer")]
	[HideInInspector]
	public float CreateTimer;

	[ExtraData("itemscreated")]
	[HideInInspector]
	public int ItemsCreated;

	public override bool CanBeDragged => true;

	public override bool CanMove
	{
		get
		{
			if (!InAnimalPen && !MyGameCard.HasParent && !MyGameCard.HasChild)
			{
				return (Object)(object)MyGameCard.GetCardWithStatusInStack() == (Object)null;
			}
			return false;
		}
	}

	public AnimalPen RootPen
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as AnimalPen;
		}
	}

	public BreedingPen RootBreedingPen
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as BreedingPen;
		}
	}

	public ResourceMagnet RootMagnet
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as ResourceMagnet;
		}
	}

	public SlaughterHouse RootSlaughterHouse
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as SlaughterHouse;
		}
	}

	public PettingZoo RootPettingZoo
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as PettingZoo;
		}
	}

	private CardData rootStructure
	{
		get
		{
			GameCard gameCard = MyGameCard.GetRootCard();
			if (gameCard.CardData is HeavyFoundation && gameCard.HasChild)
			{
				gameCard = gameCard.Child;
			}
			return gameCard.CardData;
		}
	}

	public bool InAnimalPen
	{
		get
		{
			if (!((Object)(object)RootPen != (Object)null) && !((Object)(object)RootBreedingPen != (Object)null) && !((Object)(object)RootMagnet != (Object)null) && !((Object)(object)RootSlaughterHouse != (Object)null))
			{
				return (Object)(object)RootPettingZoo != (Object)null;
			}
			return true;
		}
	}

	public virtual bool CanCreate
	{
		get
		{
			if (string.IsNullOrEmpty(CreateCard))
			{
				return false;
			}
			if (base.InConflict)
			{
				return false;
			}
			return true;
		}
	}

	public float TimeUntilCreate
	{
		get
		{
			if (!CanCreate)
			{
				return -1f;
			}
			return CreateTime - CreateTimer;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is NamingStone)
		{
			return true;
		}
		if (otherCard is Animal)
		{
			return false;
		}
		if (otherCard.Id == "wheat")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	public override void UpdateCardText()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(CustomName))
		{
			if (IsOld)
			{
				nameOverride = SokLoc.Translate("card_animal_old_name", (LocParam[])(object)new LocParam[1] { LocParam.Create("name", CustomName) });
			}
			else
			{
				nameOverride = CustomName;
			}
		}
		else if (IsOld)
		{
			nameOverride = SokLoc.Translate(NameTerm + "_old");
		}
		else
		{
			nameOverride = SokLoc.Translate(NameTerm);
		}
		base.UpdateCardText();
	}

	[TimedAction("eat_wheat")]
	private void EatWheat()
	{
		if (HasCardOnTop("wheat", out var cardData))
		{
			ConsumeWheat(cardData);
		}
	}

	public void ConsumeWheat(CardData wheat)
	{
		MyGameCard.GetRootCard().CardData.RestackChildrenMatchingPredicate((CardData x) => (Object)(object)x == (Object)(object)wheat);
		wheat.MyGameCard.DestroyCard();
		CreateTimer = 0f;
		TryCreateItem();
		MyGameCard.RotWobble(1f);
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
		if (CanCreate)
		{
			CreateTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		}
		if (CreateTimer >= CreateTime && (moveFlag || InAnimalPen || MyGameCard.GetRootCard().CardData is HeavyFoundation))
		{
			CreateTimer -= CreateTime;
			TryCreateItem();
		}
		if (HasCardOnTop("wheat", out var _) && !InAnimalPen)
		{
			MyGameCard.StartTimer(5f, EatWheat, SokLoc.Translate("card_animal_eating_status"), "eat_wheat");
		}
		else
		{
			MyGameCard.CancelTimer("eat_wheat");
		}
		if (!MyGameCard.BeingDragged && !InAnimalPen && !base.InConflict && HasCardOnTop(out Animal card))
		{
			card.MyGameCard.RemoveFromStack();
		}
	}

	private void TryCreateItem()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!CanCreate)
		{
			return;
		}
		CardData cardData = WorldManager.instance.CreateCard(((Component)MyGameCard).transform.position, CreateCard, faceUp: true, checkAddToStack: false);
		if ((Object)(object)RootBreedingPen != (Object)null || (Object)(object)RootSlaughterHouse != (Object)null || (Object)(object)RootPen != (Object)null)
		{
			WorldManager.instance.StackSendCheckTarget(rootStructure.MyGameCard, cardData.MyGameCard, OutputDir);
		}
		else
		{
			WorldManager.instance.StackSend(cardData.MyGameCard, OutputDir);
		}
		ItemsCreated++;
		if (WorldManager.instance.CurseIsActive(CurseType.Death) && ItemsCreated % 4 == 0 && CreateCard != "poop")
		{
			CardData cardData2 = WorldManager.instance.CreateCard(((Component)MyGameCard).transform.position, "poop", faceUp: true, checkAddToStack: false);
			if ((Object)(object)RootBreedingPen != (Object)null || (Object)(object)RootSlaughterHouse != (Object)null || (Object)(object)RootPen != (Object)null)
			{
				WorldManager.instance.StackSendCheckTarget(rootStructure.MyGameCard, cardData2.MyGameCard, OutputDir);
			}
			else
			{
				WorldManager.instance.StackSend(cardData2.MyGameCard, OutputDir);
			}
		}
	}

	public override void Clicked()
	{
		if (!InAnimalPen)
		{
			if (!MyGameCard.Velocity.HasValue)
			{
				MoveTimer = MoveTime;
			}
			base.Clicked();
		}
	}

	protected override void Move()
	{
		AudioManager.me.PlaySound2D(AudioManager.me.AnimalMove, Random.Range(0.8f, 1.2f), 0.2f);
		base.Move();
	}

	public override void Die()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!IsAggressive)
		{
			WorldManager.instance.TryCreateUnhappiness(((Component)this).transform.position, 2);
		}
		base.Die();
	}

	private bool CanGrowOld()
	{
		return WorldManager.instance.CurseIsActive(CurseType.Death);
	}
}
