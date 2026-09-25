using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CutsceneStep_CreateCard : CutsceneStep
{
	public enum SpawnLocation
	{
		Random,
		MiddleOfBoard,
		AtCard,
		AtFocussed
	}

	[Card]
	public string CardId;

	public SpawnLocation Location;

	[Card]
	public string OtherCardId;

	[Header("Options")]
	public bool FindOrCreate;

	public bool SendCard;

	public bool MakeSmoke;

	public override IEnumerator Process()
	{
		if (!FindOrCreate || !((Object)(object)WorldManager.instance.GetCard(CardId) != (Object)null))
		{
			Vector3 val = Vector3.zero;
			if (Location == SpawnLocation.MiddleOfBoard)
			{
				val = WorldManager.instance.MiddleOfBoard();
			}
			else if (Location == SpawnLocation.Random)
			{
				val = WorldManager.instance.GetRandomSpawnPosition();
			}
			else if (Location == SpawnLocation.AtCard)
			{
				val = ((Component)WorldManager.instance.GetCard(OtherCardId)).transform.position;
			}
			else if (Location == SpawnLocation.AtFocussed)
			{
				IGameCardOrCardData targetCardOverride = GameCamera.instance.TargetCardOverride;
				val = ((targetCardOverride == null) ? WorldManager.instance.MiddleOfBoard() : (targetCardOverride.Position + Vector3.left * 1.5f));
			}
			CardData cardData = WorldManager.instance.CreateCard(val, CardId, faceUp: true, checkAddToStack: false);
			if (MakeSmoke)
			{
				WorldManager.instance.CreateSmoke(val);
			}
			if (SendCard)
			{
				cardData.MyGameCard.SendIt();
			}
		}
		yield break;
	}
}
