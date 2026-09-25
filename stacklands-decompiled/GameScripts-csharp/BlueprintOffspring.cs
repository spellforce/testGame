using System.Collections.Generic;
using UnityEngine;

public class BlueprintOffspring : Blueprint
{
	public List<Subprint> BaseSubprints;

	public List<Subprint> SpiritsSubprints;

	public override void Init(GameDataLoader loader)
	{
		base.Init(loader);
		Subprints = (loader.SpiritDlcLoaded ? SpiritsSubprints : BaseSubprints);
	}

	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(((Component)rootCard).transform.position, print.ResultCard, faceUp: false, checkAddToStack: false);
		cardData.MyGameCard.SendIt();
		House house = null;
		for (int num = involvedCards.Count - 1; num >= 0; num--)
		{
			GameCard gameCard = involvedCards[num];
			gameCard.RemoveFromStack();
			if (gameCard.CardData is House)
			{
				house = (House)gameCard.CardData;
			}
			gameCard.SendIt();
		}
		if ((Object)(object)house != (Object)null)
		{
			cardData.MyGameCard.SetParent(house.MyGameCard);
		}
	}
}
