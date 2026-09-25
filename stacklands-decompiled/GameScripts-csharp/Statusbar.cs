using UnityEngine;

public class Statusbar : MonoBehaviour
{
	public float FillAmount;

	public float StatusTime;

	public bool DestroyMe;

	public GameCard ParentCard;

	public Vector3 Offset;

	public Vector3 ExtraOffset;

	public MeshRenderer Renderer;

	private MaterialPropertyBlock propBlock;

	private Vector3 startScale;

	private Vector3 hiddenScale;

	public bool Paused;

	private float blinkTimer;

	private bool blink;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		startScale = ((Component)this).transform.localScale;
		hiddenScale = startScale;
		hiddenScale.y = 0f;
		((Component)this).transform.localScale = hiddenScale;
		propBlock = new MaterialPropertyBlock();
		((Renderer)Renderer).GetPropertyBlock(propBlock);
	}

	private void Update()
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		float fillAmount = Mathf.Clamp01(ParentCard.CurrentTimerTime / ParentCard.TargetTimerTime);
		if (Paused)
		{
			blinkTimer += Time.deltaTime;
			if (blinkTimer >= 0.3f)
			{
				blink = !blink;
				blinkTimer = 0f;
			}
		}
		if ((Object)(object)ParentCard != (Object)null && (Object)(object)ParentCard.CurrentStatusbar != (Object)(object)this)
		{
			DestroyMe = true;
		}
		FillAmount = fillAmount;
		propBlock.SetFloat("_FillAmount", FillAmount);
		propBlock.SetFloat("_Pause", (Paused && blink) ? 1f : 0f);
		((Renderer)Renderer).SetPropertyBlock(propBlock);
		Vector3 val;
		if (DestroyMe)
		{
			val = hiddenScale;
			if (((Component)this).transform.localScale.y < 0.01f)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
		}
		else
		{
			val = startScale;
		}
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, val, Time.deltaTime * 22f);
		if ((Object)(object)ParentCard != (Object)null)
		{
			Vector3 val2 = Vector3.zero;
			GameCard rootCard = ParentCard.GetRootCard();
			if (rootCard.StatusEffectElements.Count > 0)
			{
				val2 += ExtraOffset;
			}
			((Component)this).transform.position = ((Component)rootCard).transform.position + Offset + val2;
		}
		else
		{
			DestroyMe = true;
		}
	}
}
