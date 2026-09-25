using UnityEngine;

public class DirectionCircleElement : Interactable
{
	private Vector3 startScale;

	public GameCard ParentCard;

	public Sprite DirectionArrow;

	public Sprite RandomArrow;

	public SpriteRenderer DirectionSpriteRenderer;

	public override bool CanBeAutoMovedTo
	{
		get
		{
			if (!((Component)this).gameObject.activeInHierarchy)
			{
				return false;
			}
			if ((Object)(object)WorldManager.instance.DraggingCard != (Object)null)
			{
				return false;
			}
			return !ParentCard.BeingDragged;
		}
	}

	protected override void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		startScale = ((Component)this).transform.localScale;
		base.Start();
	}

	public override string GetTooltipText()
	{
		return "Toggle output direction";
	}

	public override void Clicked()
	{
		ParentCard.ToggleDirection();
	}

	protected override void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (ParentCard.CardData.OutputDir == Vector3.zero)
		{
			DirectionSpriteRenderer.sprite = RandomArrow;
		}
		else
		{
			DirectionSpriteRenderer.sprite = DirectionArrow;
		}
		((Component)this).transform.rotation = Quaternion.LookRotation(Vector3.down, ParentCard.CardData.OutputDir);
		Vector3 val = (IsHovered ? (startScale * 1.1f) : startScale);
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, val, Time.deltaTime * 12f);
	}

	public override bool CanBeDragged()
	{
		return false;
	}

	public override bool CanBePushed()
	{
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		return false;
	}

	protected override void ClampPos()
	{
	}
}
