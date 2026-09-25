using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CutsceneStep_PlaceGeniusOnTimeMachine : CutsceneStep
{
	private CardData GetGenius()
	{
		CardData card = WorldManager.instance.GetCard("genius");
		if ((Object)(object)card != (Object)null)
		{
			return card;
		}
		CardData card2 = WorldManager.instance.GetCard("robot_genius");
		if ((Object)(object)card2 != (Object)null)
		{
			return card2;
		}
		return null;
	}

	public override IEnumerator Process()
	{
		CardData genius = GetGenius();
		CardData card = WorldManager.instance.GetCard("time_machine");
		if ((Object)(object)genius != (Object)null && (Object)(object)card != (Object)null)
		{
			genius.MyGameCard.RemoveFromStack();
			genius.MyGameCard.SetParent(card.MyGameCard);
			card.MyGameCard.RotWobble(1f);
		}
		yield break;
	}
}
