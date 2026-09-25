using UnityEngine;

public class Conveyor : CardData
{
	public float ExtraSideDistance = 0.01f;

	[ExtraData("direction")]
	[HideInInspector]
	public int Direction;

	public float TotalTime = 5f;

	private Vector2[] corners = (Vector2[])(object)new Vector2[4];

	private Vector3 directionVector
	{
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (Direction == 0)
			{
				return Vector3.back;
			}
			if (Direction == 1)
			{
				return Vector3.left;
			}
			if (Direction == 2)
			{
				return Vector3.forward;
			}
			if (Direction == 3)
			{
				return Vector3.right;
			}
			return Vector3.back;
		}
	}

	protected override bool CanToggleOnOff()
	{
		if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			return true;
		}
		return false;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return false;
	}

	private bool CanBeInputCard(CardData card)
	{
		if (card.MyGameCard.Velocity.HasValue || (Object)(object)card.MyGameCard.BounceTarget != (Object)null)
		{
			return false;
		}
		if (MyGameCard.IsParentOf(card.MyGameCard))
		{
			return false;
		}
		if (card is ResourceChest resourceChest)
		{
			if (string.IsNullOrEmpty(resourceChest.HeldCardId))
			{
				return false;
			}
			return CanBeConveyed(resourceChest.HeldCardId);
		}
		if (card is ResourceMagnet resourceMagnet)
		{
			if (string.IsNullOrEmpty(resourceMagnet.PullCardId))
			{
				return false;
			}
			return CanBeConveyed(resourceMagnet.PullCardId);
		}
		if (CanBeConveyed(card))
		{
			if (card.MyGameCard.HasChild)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private bool CanBeConveyed(string cardId)
	{
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(cardId);
		return CanBeConveyed(cardPrefab);
	}

	private CardData GetConveyableCardFromInputCard(CardData card)
	{
		if (card is ResourceChest { ResourceCount: >0 } resourceChest)
		{
			return resourceChest.RemoveResources(1).CardData;
		}
		if (card is ResourceMagnet resourceMagnet && resourceMagnet.MyGameCard.HasChild)
		{
			return resourceMagnet.MyGameCard.GetLeafCard().CardData;
		}
		if (CanBeConveyed(card))
		{
			return card;
		}
		return null;
	}

	private bool InputCardHasConveyableCard(CardData card)
	{
		if (card is ResourceChest resourceChest)
		{
			return resourceChest.ResourceCount > 0;
		}
		if (card is ResourceMagnet resourceMagnet && resourceMagnet.MyGameCard.HasChild)
		{
			return true;
		}
		if (CanBeConveyed(card))
		{
			return true;
		}
		return false;
	}

	private CardData GetPrefabForId(string id)
	{
		return WorldManager.instance.GetCardPrefab(id);
	}

	private CardData GetInputCardConveyablePrefab(CardData card)
	{
		if (card is ResourceChest resourceChest)
		{
			return GetPrefabForId(resourceChest.HeldCardId);
		}
		if (card is ResourceMagnet resourceMagnet)
		{
			return GetPrefabForId(resourceMagnet.PullCardId);
		}
		if (CanBeConveyed(card))
		{
			return GetPrefabForId(card.Id);
		}
		return null;
	}

	private CardData GetInputCard(bool allowDraggingCards)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return WorldManager.instance.GetBestCardInDirection(MyGameCard, directionVector, allowDraggingCards, (GameCard card) => CanBeInputCard(card.CardData))?.CardData;
	}

	private bool CanBeConveyed(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Food && otherCard.MyCardType != CardType.Humans)
		{
			if (otherCard is Mob mob)
			{
				return !mob.IsAggressive;
			}
			return false;
		}
		return true;
	}

	public override void UpdateCard()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.IsDemoCard)
		{
			return;
		}
		bool flag = true;
		if (MyGameCard.Velocity.HasValue)
		{
			flag = false;
		}
		CardData cardData = null;
		if (flag)
		{
			cardData = GetInputCard(allowDraggingCards: true);
		}
		if ((Object)(object)cardData != (Object)null && InputCardHasConveyableCard(cardData))
		{
			CardData inputCardConveyablePrefab = GetInputCardConveyablePrefab(cardData);
			string status = SokLoc.Translate("card_conveyor_status", (LocParam[])(object)new LocParam[1] { LocParam.Create("resource", inputCardConveyablePrefab.Name) });
			MyGameCard.StartTimer(TotalTime, LoadCard, status, GetActionId("LoadCard"));
		}
		else
		{
			MyGameCard.CancelAnyTimer();
		}
		CardData outputCard = null;
		if ((Object)(object)cardData != (Object)null)
		{
			CardData inputCardConveyablePrefab2 = GetInputCardConveyablePrefab(cardData);
			if ((Object)(object)inputCardConveyablePrefab2 != (Object)null)
			{
				outputCard = WorldManager.instance.GetTargetCard(MyGameCard, inputCardConveyablePrefab2, -directionVector, allowDraggedCards: true, cardData.MyGameCard)?.CardData;
			}
		}
		DrawArrows(cardData, outputCard);
		base.UpdateCard();
	}

	public override void Clicked()
	{
		Direction = (Direction + 1) % 4;
		base.Clicked();
	}

	[TimedAction("load_card")]
	public void LoadCard()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		CardData inputCard = GetInputCard(allowDraggingCards: false);
		if ((Object)(object)inputCard == (Object)null)
		{
			return;
		}
		CardData conveyableCardFromInputCard = GetConveyableCardFromInputCard(inputCard);
		if ((Object)(object)conveyableCardFromInputCard == (Object)null)
		{
			return;
		}
		conveyableCardFromInputCard.MyGameCard.RemoveFromStack();
		GameCard targetCard = WorldManager.instance.GetTargetCard(MyGameCard, conveyableCardFromInputCard, -directionVector, allowDraggedCards: false, inputCard.MyGameCard);
		if ((Object)(object)targetCard != (Object)null)
		{
			SendToTargetCard(conveyableCardFromInputCard.MyGameCard, targetCard);
		}
		else
		{
			if ((Object)(object)conveyableCardFromInputCard.MyGameCard.BounceTarget == (Object)(object)inputCard.MyGameCard)
			{
				conveyableCardFromInputCard.MyGameCard.BounceTarget = null;
			}
			conveyableCardFromInputCard.MyGameCard.SendToPosition(((Component)MyGameCard).transform.position - directionVector);
		}
		QuestManager.instance.SpecialActionComplete("use_conveyor");
	}

	private void SendToTargetCard(GameCard card, GameCard targetCard)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)targetCard).transform.position - ((Component)card).transform.position;
		val.y = 0f;
		Vector3 value = default(Vector3);
		((Vector3)(ref value))._002Ector(val.x * 4f, 7f, val.z * 4f);
		card.BounceTarget = targetCard.GetRootCard();
		card.Velocity = value;
	}

	private Vector2 GetPointOnCardEdge(Vector2 start, Vector2 end, GameCard card)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = card.GetBounds();
		corners[0] = new Vector2(((Bounds)(ref bounds)).min.x, ((Bounds)(ref bounds)).min.z);
		corners[1] = new Vector2(((Bounds)(ref bounds)).max.x, ((Bounds)(ref bounds)).min.z);
		corners[2] = new Vector2(((Bounds)(ref bounds)).max.x, ((Bounds)(ref bounds)).max.z);
		corners[3] = new Vector2(((Bounds)(ref bounds)).min.x, ((Bounds)(ref bounds)).max.z);
		for (int i = 0; i < 4; i++)
		{
			Vector2 p = corners[i];
			Vector2 p2 = corners[(i + 1) % 4];
			if (MathHelper.LineSegmentsIntersection(start, end, p, p2, out var intersection, out var _))
			{
				return intersection;
			}
		}
		return start;
	}

	private Vector3 TransformToEdge(Vector3 start, Vector3 end, GameCard card, float dir)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Vector2 start2 = default(Vector2);
		((Vector2)(ref start2))._002Ector(start.x, start.z);
		Vector2 end2 = default(Vector2);
		((Vector2)(ref end2))._002Ector(end.x, end.z);
		Vector2 pointOnCardEdge = GetPointOnCardEdge(start2, end2, card);
		Vector3 val = new Vector3(pointOnCardEdge.x, 0f, pointOnCardEdge.y);
		Vector3 val2 = start - end;
		return val + ((Vector3)(ref val2)).normalized * ExtraSideDistance * dir;
	}

	private void DrawInputArrow(CardData inputCard)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)MyGameCard).transform.position;
		Vector3 start = ((!((Object)(object)inputCard != (Object)null)) ? (((Component)MyGameCard).transform.position + directionVector * 0.5f) : TransformToEdge(((Component)inputCard).transform.position, position, inputCard.MyGameCard, -1f));
		position = TransformToEdge(start, position, MyGameCard, 1f);
		DrawManager.instance.DrawShape(new ConveyorArrow
		{
			Start = start,
			End = position
		});
	}

	private void DrawOutputArrow(CardData outputCard)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)MyGameCard).transform.position;
		Vector3 end = ((!((Object)(object)outputCard != (Object)null)) ? (((Component)MyGameCard).transform.position - directionVector * 0.5f) : TransformToEdge(position, ((Component)outputCard).transform.position, outputCard.MyGameCard, 1f));
		position = TransformToEdge(position, end, MyGameCard, -1f);
		DrawManager.instance.DrawShape(new ConveyorArrow
		{
			Start = position,
			End = end
		});
	}

	private void DrawArrows(CardData inputCard, CardData outputCard)
	{
		DrawInputArrow(inputCard);
		DrawOutputArrow(outputCard);
	}
}
