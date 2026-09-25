using UnityEngine;

public class InventoryInteractable : Interactable
{
	public GameCard ParentCard;

	private Vector3 startScale;

	public string TooltipTerm;

	public string gameObjectTerm;

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
			if (!ParentCard.ShowInventory)
			{
				return !ParentCard.BeingDragged;
			}
			return false;
		}
	}

	public override string GetTooltipText()
	{
		return SokLoc.Translate(TooltipTerm);
	}

	public override void Clicked()
	{
		ParentCard.ToggleInventory();
	}

	public override bool CanBeDragged()
	{
		return false;
	}

	protected override void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		startScale = ((Component)this).transform.localScale;
		base.Start();
	}

	protected override void Update()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		MyBoard = ParentCard.MyBoard;
		((Object)((Component)this).gameObject).name = SokLoc.Translate(gameObjectTerm);
		Vector3 val = (IsHovered ? (startScale * 1.1f) : startScale);
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, val, Time.deltaTime * 12f);
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
