using UnityEngine;

public class CardTarget : Interactable
{
	private Vector3 startScale;

	public bool Hovered => (Object)(object)WorldManager.instance.NearbyCardTarget == (Object)(object)this;

	public override bool CanBePushed()
	{
		return false;
	}

	protected override void Start()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		WorldManager.instance.CardTargets.Add(this);
		startScale = ((Component)this).transform.localScale;
		PushDir = new Vector3(0f, 0f, 1f);
		TargetPosition = ((Component)this).transform.position;
		MyBoard = DetermineParentBoard();
		base.Start();
	}

	protected override void OnDestroy()
	{
		if ((Object)(object)WorldManager.instance != (Object)null)
		{
			WorldManager.instance.CardTargets.Remove(this);
		}
		base.OnDestroy();
	}

	protected override void Update()
	{
	}

	protected override void LateUpdate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, Hovered ? (startScale * 1.05f) : startScale, Time.deltaTime * 20f);
		base.LateUpdate();
	}

	public virtual void CardDropped(GameCard card)
	{
	}

	public virtual bool CanHaveCard(GameCard card)
	{
		return true;
	}

	protected override void ClampPos()
	{
	}
}
