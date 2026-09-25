using System.Collections.Generic;
using UnityEngine;

public class WorkerBlueprint : Blueprint
{
	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(((Component)rootCard).transform.position, print.ResultCard, faceUp: false, checkAddToStack: false);
		Apartment apartment = null;
		for (int num = involvedCards.Count - 1; num >= 0; num--)
		{
			GameCard gameCard = involvedCards[num];
			gameCard.RemoveFromStack();
			if (gameCard.CardData is Apartment)
			{
				apartment = (Apartment)gameCard.CardData;
			}
			else
			{
				gameCard.SendIt();
			}
		}
		if ((Object)(object)apartment != (Object)null)
		{
			cardData.MyGameCard.SetParent(apartment.MyGameCard);
		}
	}
}
