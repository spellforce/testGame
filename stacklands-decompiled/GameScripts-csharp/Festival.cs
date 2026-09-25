using UnityEngine;

public class Festival : EventCard
{
	protected override void ExecuteEvent()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		MyGameCard.StartTimer(5f, StopEvent, SokLoc.Translate(EventText), GetActionId("StopEvent"));
		WorldManager.instance.QueueCutscene("cities_festival");
		CardData cardData = WorldManager.instance.CreateCard(base.Position, "merch", faceUp: true, checkAddToStack: false);
		WorldManager.instance.CreateWellbeingPlus(base.Position);
		cardData.MyGameCard.SendIt();
		EventIsActive = true;
	}

	[TimedAction("stop_event")]
	public void StopEvent()
	{
		base.EndEvent();
	}

	protected override void EndEvent()
	{
		base.EndEvent();
	}

	public override void UpdateCardText()
	{
		if ((Object)(object)MyGameCard != (Object)null && MyGameCard.TimerRunning && MyGameCard.TimerActionId == GetActionId("StopEvent"))
		{
			descriptionOverride = SokLoc.Translate(EventText);
		}
		base.UpdateCardText();
	}
}
