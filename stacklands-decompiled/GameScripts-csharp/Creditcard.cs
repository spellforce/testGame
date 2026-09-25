using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creditcard : CardData, ICurrency
{
	[ExtraData("dollar_count")]
	[HideInInspector]
	public int DollarCount;

	public int MaxDollarCount = 1000;

	public string BankDescriptionTerm;

	public CardData Card => this;

	public int CurrencyValue
	{
		get
		{
			return DollarCount;
		}
		set
		{
			DollarCount = value;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Dollar || otherCard.Id == Id)
		{
			return true;
		}
		return false;
	}

	public override void UpdateCard()
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (IsDamaged)
		{
			base.UpdateCard();
			return;
		}
		List<Dollar> list = (from x in MyGameCard.GetChildCards()
			where x.CardData is Dollar
			select x.CardData as Dollar).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			GameCard myGameCard = list[num].MyGameCard;
			if (myGameCard.CardData is Dollar dollar)
			{
				Creditcard creditcardWithSpace = GetCreditcardWithSpace();
				if ((Object)(object)creditcardWithSpace != (Object)null)
				{
					int num2 = creditcardWithSpace.MaxDollarCount - creditcardWithSpace.DollarCount;
					if (num2 > 0)
					{
						if (dollar.DollarValue > num2)
						{
							int value = dollar.DollarValue - num2;
							creditcardWithSpace.DollarCount = creditcardWithSpace.MaxDollarCount;
							myGameCard.DestroyCard();
							list.AddRange(from x in WorldManager.instance.CreateDollarsFromValue(value, base.Position)
								select x.CardData as Dollar);
						}
						else
						{
							creditcardWithSpace.DollarCount += dollar.DollarValue;
							myGameCard.DestroyCard();
						}
						if ((Object)(object)myGameCard.CardData == (Object)(object)list.Last())
						{
							WorldManager.instance.CreateSmoke(base.Position);
						}
					}
				}
				else
				{
					myGameCard.RemoveFromParent();
				}
			}
			WorldManager.instance.Restack(list.Select((Dollar x) => x.MyGameCard).ToList());
		}
		CitiesValue = DollarCount;
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		GameCard myGameCard = MyGameCard;
		if (myGameCard != null && myGameCard.CardConnectorChildren.Count > 0 && MyGameCard.IsHovered)
		{
			descriptionOverride = SokLoc.Translate(BankDescriptionTerm, (LocParam[])(object)new LocParam[3]
			{
				LocParam.Create("count", DollarCount.ToString()),
				LocParam.Create("max_count", MaxDollarCount.ToString()),
				LocParam.Create("icon", Icons.Dollar)
			});
			descriptionOverride = descriptionOverride + "\n\n<i>" + GetConnectorInfoString(MyGameCard) + "</i>";
		}
	}

	private Creditcard GetCreditcardWithSpace()
	{
		GameCard gameCard = MyGameCard.GetAllCardsInStack().FirstOrDefault((GameCard x) => x.CardData is Creditcard creditcard && creditcard.DollarCount < creditcard.MaxDollarCount);
		if ((Object)(object)gameCard == (Object)null)
		{
			return null;
		}
		return gameCard.CardData as Creditcard;
	}

	public override void Clicked()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (DollarCount > 0)
		{
			int num = Mathf.Min(DollarCount, 100);
			WorldManager.instance.CreateDollarsFromValue(num, base.Position, checkAddToStack: false);
			DollarCount -= num;
			WorldManager.instance.CreateSmoke(base.Position);
		}
	}

	public void UseCurrency(int currencyAmount, bool spawnSmoke = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (spawnSmoke)
		{
			WorldManager.instance.CreateSmoke(base.Position);
		}
		DollarCount -= currencyAmount;
	}
}
