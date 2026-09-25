using System.Collections.Generic;
using UnityEngine;

public class WishingWell : CardData
{
	public int WishCost = 500;

	public List<AudioClip> WishSound;

	public Sprite SpecialIcon;

	[ExtraData("coin_count")]
	[HideInInspector]
	public int CoinCount;

	[ExtraData("wish_count")]
	[HideInInspector]
	public int WishCount;

	private string HeldCardId = "gold";

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == HeldCardId))
		{
			if (otherCard is Chest chest)
			{
				return chest.HeldCardId == HeldCardId;
			}
			return false;
		}
		return true;
	}

	public override void UpdateCardText()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (WishCount > 0)
		{
			descriptionOverride = SokLoc.Translate("card_wishing_well_description_long", (LocParam[])(object)new LocParam[2]
			{
				LocParam.Plural("amount", WishCount),
				LocParam.Create("count", WishCost.ToString())
			});
		}
		else
		{
			descriptionOverride = SokLoc.Translate("card_wishing_well_description", (LocParam[])(object)new LocParam[1] { LocParam.Create("count", WishCost.ToString()) });
		}
	}

	public override void UpdateCard()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (childCard.CardData is Chest chest)
				{
					if (chest.CoinCount < WishCost - CoinCount)
					{
						CoinCount += chest.CoinCount;
						chest.CoinCount = 0;
						WorldManager.instance.CreateSmoke(((Component)MyGameCard).transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
					else if (chest.CoinCount >= WishCost - CoinCount)
					{
						chest.CoinCount -= WishCost - CoinCount;
						CoinCount = WishCost;
						WorldManager.instance.CreateSmoke(((Component)MyGameCard).transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
				}
				if (!(childCard.CardData.Id != HeldCardId))
				{
					if (CoinCount >= WishCost)
					{
						childCard.RemoveFromParent();
						break;
					}
					childCard.DestroyCard(spawnSmoke: true);
					CoinCount++;
				}
			}
			if (CoinCount == WishCost)
			{
				GiveWish();
			}
		}
		base.UpdateCard();
	}

	private void GiveWish()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		AudioManager.me.PlaySound2D(WishSound, 1f, 0.1f);
		WorldManager.instance.CreateSmoke(((Component)this).transform.position);
		CoinCount = 0;
		WishCount++;
		switch (WishCount)
		{
		case 1:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish1(this));
			break;
		case 2:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish2(this));
			break;
		case 5:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish5(this));
			break;
		case 10:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish10(this));
			break;
		case 20:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish20(this));
			break;
		case 50:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish50(this));
			break;
		}
	}
}
