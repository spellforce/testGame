using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Draggable : MonoBehaviour
{
	public bool BeingDragged;

	[HideInInspector]
	public GameBoard MyBoard;

	[HideInInspector]
	public Vector3 TargetPosition;

	[HideInInspector]
	public Vector3 DragStartPosition;

	public Vector3? Velocity;

	public const float Drag = 0.93f;

	public const float Gravity = 30f;

	public const float Bounciness = 0.6f;

	public Vector3? PushDir;

	public BoxCollider boxCollider;

	[HideInInspector]
	public string DragTag;

	[HideInInspector]
	public Draggable ClickedObject;

	[HideInInspector]
	public float MinY;

	private float dragStartTime;

	protected Collider[] hits = (Collider[])(object)new Collider[50];

	private int layerMask;

	protected Bounds debugBounds;

	protected bool wasPushed;

	public virtual bool IsHovered => (Object)(object)WorldManager.instance.HoveredDraggable == (Object)(object)this;

	public virtual bool CanBeAutoMovedTo => true;

	public virtual Vector3 AutoMoveSnapPosition => ((Component)this).transform.position;

	public Bounds DraggableBounds
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			boxCollider.ToWorldSpaceBox(out var center, out var halfExtents, out var _);
			return new Bounds(center, halfExtents * 2f);
		}
	}

	protected virtual bool HasPhysics => false;

	protected bool DragThresholdReached
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = DragStartPosition - ((Component)this).transform.position;
			val.y = 0f;
			if (Time.time - dragStartTime <= 0.2f && ((Vector3)(ref val)).magnitude <= 0.1f)
			{
				return false;
			}
			return true;
		}
	}

	protected virtual float Mass => 1f;

	public virtual bool CanBePushedBy(Draggable draggable)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (draggable is GameCard gameCard && (gameCard.IsEquipped || gameCard.IsWorking))
		{
			return false;
		}
		if (draggable is InventoryInteractable || draggable is OnOffInteractable || draggable is FloatingStatus || draggable is CardConnector)
		{
			return false;
		}
		if (draggable is DirectionCircleElement)
		{
			return false;
		}
		if (!draggable.BeingDragged)
		{
			if (draggable.Velocity.HasValue)
			{
				return draggable.Velocity.Value.y < 0f;
			}
			return true;
		}
		return false;
	}

	public virtual bool CanBePushed()
	{
		return true;
	}

	protected virtual void Awake()
	{
		if ((Object)(object)boxCollider == (Object)null)
		{
			BoxCollider[] components = ((Component)this).GetComponents<BoxCollider>();
			boxCollider = ((IEnumerable<BoxCollider>)components).FirstOrDefault((Func<BoxCollider, bool>)((BoxCollider x) => !((Collider)x).isTrigger));
		}
		WorldManager.instance.AllDraggables.Add(this);
		if (HasPhysics)
		{
			WorldManager.instance.PhysicsDraggables.Add(this);
		}
		layerMask = LayerMask.GetMask(new string[1] { "Pushables" });
	}

	protected virtual void OnDestroy()
	{
		if ((Object)(object)WorldManager.instance != (Object)null)
		{
			WorldManager.instance.AllDraggables.Remove(this);
			if (HasPhysics)
			{
				WorldManager.instance.PhysicsDraggables.Remove(this);
			}
		}
	}

	protected virtual void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TargetPosition = ((Component)this).transform.position;
	}

	public virtual void SendIt()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (WorldManager.instance.CurrentBoard.Id == "forest")
		{
			Velocity = new Vector3(0f, 10f, -10f);
			return;
		}
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector2 val = ((Vector2)(ref insideUnitCircle)).normalized * 3f * 1.5f;
		Velocity = new Vector3(val.x, 5f, val.y);
	}

	public virtual void SendDirection(Vector3 direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Velocity = new Vector3(direction.x * 6f, 5f, direction.z * 7f);
	}

	public virtual void SendToPosition(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = position - ((Component)this).transform.position;
		Velocity = new Vector3(val.x * 4f, 5f, val.z * 4f);
	}

	protected virtual void Bounce()
	{
	}

	public void UpdatePhysics(float dt)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (!Velocity.HasValue)
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		position += Velocity.Value * dt;
		Vector3 value = Velocity.Value;
		if (position.y <= MinY && value.y < 0f)
		{
			value.x *= 0.8f;
			value.z *= 0.8f;
			value.y *= -0.6f;
			position.y = MinY;
			Bounce();
			if (!Velocity.HasValue)
			{
				return;
			}
		}
		value.x *= 0.93f;
		value.y -= 30f * dt;
		value.z *= 0.93f;
		Vector3 val = value;
		val.y = 0f;
		if (((Vector3)(ref val)).magnitude < 0.01f && position.y <= MinY + 0.1f)
		{
			Velocity = null;
		}
		else
		{
			Velocity = value;
		}
		Vector3 targetPosition = (((Component)this).transform.position = position);
		TargetPosition = targetPosition;
	}

	protected virtual void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector3 targetPosition = TargetPosition;
		targetPosition.y = (0f - targetPosition.z) * 0.001f;
		targetPosition.y += (BeingDragged ? 0.1f : 0f);
		if (IsHovered)
		{
			targetPosition.y += 0.03f;
		}
		if (this is Boosterpack)
		{
			targetPosition.y += 0.03f;
		}
		((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, targetPosition, Time.deltaTime * 20f);
	}

	protected virtual void LateUpdate()
	{
		PushAwayFromOthers();
		ClampPos();
	}

	public virtual bool CanBeDragged()
	{
		return !BeingDragged;
	}

	public virtual void Clicked()
	{
	}

	public virtual void StartDragging()
	{
		dragStartTime = Time.time;
		Velocity = null;
		BeingDragged = true;
	}

	public virtual void StopDragging()
	{
		if (!DragThresholdReached)
		{
			if ((Object)(object)ClickedObject == (Object)null)
			{
				Clicked();
			}
			else
			{
				ClickedObject.Clicked();
			}
		}
		ClickedObject = null;
		DragTag = null;
		BeingDragged = false;
	}

	protected virtual void PushAwayFromOthers()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanBePushed())
		{
			return;
		}
		Draggable draggable = this;
		BoxCollider box = draggable.boxCollider;
		BoxCollider val = null;
		if (this is GameCard gameCard)
		{
			if ((Object)(object)gameCard.Parent != (Object)null)
			{
				return;
			}
			draggable = gameCard.GetRootCard();
			val = gameCard.GetLeafCard().boxCollider;
		}
		GetComponentCacher<Draggable> draggableLookup = WorldManager.instance.DraggableLookup;
		int num = ((!((Object)(object)val == (Object)null)) ? PhysicsExtensions.OverlapTwoBoxNonAlloc(box, val, hits, layerMask, (QueryTriggerInteraction)0) : PhysicsExtensions.OverlapBoxNonAlloc(boxCollider, hits, layerMask, (QueryTriggerInteraction)0));
		for (int i = 0; i < num; i++)
		{
			Collider val2 = hits[i];
			Draggable component = draggableLookup.GetComponent(((Component)val2).gameObject);
			if (!((Object)(object)component == (Object)null) && !((Object)(object)component == (Object)(object)this) && !component.BeingDragged && CanBePushedBy(component) && (!(this is GameCard gameCard2) || !((Object)(object)gameCard2.GetRootCard() != (Object)null) || !(gameCard2.GetRootCard().CardData is HeavyFoundation) || !(component is GameCard gameCard3) || gameCard3.CardData is HeavyFoundation))
			{
				Vector3 val3 = ((Component)component).transform.position - ((Component)draggable).transform.position;
				val3.y = 0f;
				float num2 = draggable.Mass + component.Mass;
				float num3 = 1f - draggable.Mass / num2;
				if (component.PushDir.HasValue)
				{
					val3 = component.PushDir.Value;
					num3 = 1f;
				}
				Draggable draggable2 = draggable;
				draggable2.TargetPosition -= num3 * ((Vector3)(ref val3)).normalized * 2f * Time.deltaTime;
				break;
			}
		}
	}

	protected virtual void ClampPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		Bounds worldBounds = MyBoard.WorldBounds;
		boxCollider.ToWorldSpaceBox(out var _, out var halfExtents, out var _);
		float num = 0.1f;
		position.x = Mathf.Clamp(position.x, ((Bounds)(ref worldBounds)).min.x + halfExtents.x + num, ((Bounds)(ref worldBounds)).max.x - halfExtents.x - num);
		position.z = Mathf.Clamp(position.z, ((Bounds)(ref worldBounds)).min.z + halfExtents.y + num, ((Bounds)(ref worldBounds)).max.z - halfExtents.y - num);
		((Component)this).transform.position = position;
	}

	protected GameBoard DetermineParentBoard()
	{
		return ((Component)this).GetComponentInParent<GameBoard>();
	}
}
