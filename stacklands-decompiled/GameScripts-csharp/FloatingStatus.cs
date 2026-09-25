using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingStatus : Interactable
{
	public bool InAnimation;

	public GameCard ParentCard;

	public TextMeshPro Text;

	public Vector3 NoStatusOffset;

	public Vector3 StatusOffset;

	private float yOffset;

	private Vector3 endPos;

	private Vector3 positionVelocity;

	private float disappearTime;

	private string descriptionText;

	private bool closeOnHover;

	private bool RemoveTimerStarted;

	private float RemoveTimer;

	private float timer;

	private Vector3 scaleVelo;

	public void StartAnimation(GameCard parent, bool isPositive, int amount, string descriptionText, string iconTag, bool desiredBehaviour, int index = 1, float disappearTime = 1f, bool closeOnHover = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Transform parent2 = (((Object)(object)parent.WorkerHolder != (Object)null) ? ((Component)parent.WorkerHolder).transform : ((Component)parent).transform);
		((Component)this).transform.parent = parent2;
		((Component)this).transform.localPosition = Vector3.zero;
		((Component)this).transform.localRotation = Quaternion.identity;
		((Object)((Component)this).gameObject).name = parent.CardData.Name;
		ParentCard = parent;
		yOffset = 0.12f * (float)index;
		string arg = (isPositive ? "+" : "-");
		((Graphic)Text).color = (desiredBehaviour ? ColorManager.instance.FloatingTextColorSuccess : ColorManager.instance.FloatingTextColorFailed);
		((TMP_Text)Text).text = $"{arg}{Mathf.Abs(amount)}{iconTag}";
		this.disappearTime = disappearTime;
		this.descriptionText = descriptionText;
		this.closeOnHover = closeOnHover;
		timer = 0f;
		((TMP_Text)Text).alpha = 1f;
		InAnimation = true;
		((Component)this).gameObject.SetActive(true);
	}

	public void StopAnimation()
	{
		InAnimation = false;
		if ((Object)(object)((Component)this).transform != (Object)null)
		{
			((Component)this).transform.parent = null;
		}
		((Component)this).gameObject.SetActive(false);
	}

	protected override void Start()
	{
		MyBoard = WorldManager.instance.CurrentBoard;
		base.Start();
	}

	public override string GetTooltipText()
	{
		return descriptionText;
	}

	protected override void ClampPos()
	{
	}

	protected override void Update()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((ParentCard.CardData.HasAnyStatusEffect() || (Object)(object)ParentCard.GetCardWithStatusInStack() != (Object)null) ? StatusOffset : NoStatusOffset);
		val.y += yOffset;
		((Component)this).transform.localPosition = FRILerp.Spring(((Component)this).transform.localPosition, val, 12f, 12f, ref positionVelocity);
		if (InAnimation && (disappearTime > 0f || timer <= 1f) && Vector3.Distance(((Component)this).transform.localPosition, val) < 0.01f)
		{
			timer += Time.deltaTime;
			if (timer <= 1f && disappearTime != 0f)
			{
				((TMP_Text)Text).alpha = Mathf.Lerp(1f, 0f, timer * 4f);
			}
			if (disappearTime > 0f && timer > disappearTime)
			{
				StopAnimation();
			}
		}
		Vector3 target = Vector3.one;
		if (IsHovered)
		{
			if (closeOnHover)
			{
				RemoveTimerStarted = true;
				RemoveTimer = 0f;
			}
			target = Vector3.one * 1.2f;
			Tooltip.Text = descriptionText;
		}
		RemoveTimer += Time.deltaTime * 0.5f;
		if (RemoveTimerStarted && RemoveTimer > 1f)
		{
			RemoveTimerStarted = false;
			StopAnimation();
		}
		((Component)this).transform.localScale = FRILerp.Spring(((Component)this).transform.localScale, target, 30f, 30f, ref scaleVelo);
	}
}
