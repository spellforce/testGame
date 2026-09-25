using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CutsceneStep_FocusCameraOnGenius : CutsceneStep
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
		if ((Object)(object)genius != (Object)null)
		{
			GameCamera.instance.TargetCardOverride = genius;
		}
		yield break;
	}
}
