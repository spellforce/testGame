using UnityEngine;

public class TravellingCart : CardData
{
	public CardBag MyCardBag;

	public int GoldToUse = 3;

	[ExtraData("items_bought")]
	public int ItemsBought;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if ((Object)(object)otherCard.MyGameCard == (Object)null)
		{
			return otherCard.Id == "gold";
		}
		if (WorldManager.instance.BoughtWithGold(otherCard.MyGameCard, GoldToUse, checkStackAllSame: true) || WorldManager.instance.BoughtWithGoldChest(otherCard.MyGameCard, GoldToUse))
		{
			return true;
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild)
		{
			GameCard child = MyGameCard.Child;
			if (WorldManager.instance.BoughtWithGold(child, GoldToUse))
			{
				WorldManager.instance.RemoveCardsFromStackPred(child, GoldToUse, (GameCard x) => x.CardData.Id == "gold");
				Buy();
			}
			else if (WorldManager.instance.BoughtWithGoldChest(child, GoldToUse))
			{
				WorldManager.instance.BuyWithChest(child, GoldToUse);
				Buy();
			}
		}
		base.UpdateCard();
	}

	private void Buy()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ICardId cardId = MyCardBag.GetCard(removeCard: false);
		if (ItemsBought == 5 && WorldManager.instance.GetCardCount("goblet") == 0)
		{
			cardId = (CardId)"goblet";
		}
		QuestManager.instance.SpecialActionComplete("travelling_cart_buy", this);
		WorldManager.instance.CreateCard(((Component)this).transform.position, cardId, faceUp: true, checkAddToStack: false).MyGameCard.SendIt();
		ItemsBought++;
	}
}
