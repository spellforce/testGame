using System.Linq;
using UnityEngine;

public class Chest : CardData
{
	[ExtraData("coin_count")]
	[HideInInspector]
	public int CoinCount;

	public int MaxCoinCount = 100;

	public string HeldCardId = "gold";

	public string ChestTerm = "card_coin_chest_description_long";

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == Id)
		{
			return true;
		}
		if (otherCard.Id == HeldCardId)
		{
			return (Object)(object)GetChestWithSpace() != (Object)null;
		}
		return false;
	}

	private Chest GetChestWithSpace()
	{
		GameCard gameCard = MyGameCard.GetAllCardsInStack().FirstOrDefault((GameCard x) => x.CardData is Chest chest && chest.CoinCount < chest.MaxCoinCount);
		if ((Object)(object)gameCard == (Object)null)
		{
			return null;
		}
		return gameCard.CardData as Chest;
	}

	public override void UpdateCard()
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Value = CoinCount;
		if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (!(childCard.CardData.Id != HeldCardId))
				{
					Chest chestWithSpace = GetChestWithSpace();
					if (!((Object)(object)chestWithSpace != (Object)null))
					{
						childCard.RemoveFromParent();
						break;
					}
					if (chestWithSpace.CoinCount < chestWithSpace.MaxCoinCount)
					{
						childCard.DestroyCard(spawnSmoke: true);
						chestWithSpace.CoinCount++;
					}
				}
			}
		}
		descriptionOverride = SokLoc.Translate(ChestTerm, (LocParam[])(object)new LocParam[4]
		{
			LocParam.Create("count", CoinCount.ToString()),
			LocParam.Create("max_count", MaxCoinCount.ToString()),
			LocParam.Create("goldicon", Icons.Gold),
			LocParam.Create("shellicon", Icons.Shell)
		});
		base.UpdateCard();
	}

	public override void Clicked()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		int num = 5;
		if (CoinCount > 0)
		{
			int num2 = Mathf.Min(num, CoinCount);
			GameCard gameCard = WorldManager.instance.CreateCardStack(((Component)this).transform.position + Vector3.up * 0.2f, num2, HeldCardId, checkAddToStack: false);
			WorldManager.instance.StackSend(gameCard.GetRootCard(), OutputDir, null, sendToChest: false);
			CoinCount -= num2;
		}
		base.Clicked();
	}
}
