using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectElement : Hoverable
{
	[HideInInspector]
	public StatusEffect MyStatusEffect;

	public SpriteRenderer StatusRenderer;

	public Vector3 TargetLocalPosition;

	private Vector3 startScale;

	public bool DestroyMe;

	public GameCard ParentCard;

	public TextMeshPro TextMesh;

	private MaterialPropertyBlock propBlock;

	private void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		startScale = ((Component)this).transform.localScale;
		propBlock = new MaterialPropertyBlock();
		((Renderer)StatusRenderer).GetPropertyBlock(propBlock);
	}

	public void SetStatusEffect(GameCard parentCard, StatusEffect effect)
	{
		MyStatusEffect = effect;
		ParentCard = parentCard;
		StatusRenderer.sprite = effect.Sprite;
	}

	public void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = startScale;
		if (DestroyMe)
		{
			zero = Vector3.zero;
			if (zero.x < 0.001f)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
				ParentCard.StatusEffectElements.Remove(this);
			}
		}
		if (MyStatusEffect.FillAmount.HasValue)
		{
			propBlock.SetFloat("_FillAmount", MyStatusEffect.FillAmount.Value);
		}
		else
		{
			propBlock.SetFloat("_FillAmount", 2f);
		}
		propBlock.SetTexture("_MainTex", (Texture)(object)MyStatusEffect.Sprite.texture);
		propBlock.SetColor("_ColorA", MyStatusEffect.ColorA);
		propBlock.SetColor("_ColorB", MyStatusEffect.ColorB);
		((Renderer)StatusRenderer).SetPropertyBlock(propBlock);
		if (MyStatusEffect.StatusNumberColor.HasValue)
		{
			((Graphic)TextMesh).color = MyStatusEffect.StatusNumberColor.Value;
		}
		if (MyStatusEffect.StatusNumber.HasValue)
		{
			((TMP_Text)TextMesh).text = MyStatusEffect.StatusNumber.ToString();
		}
		else
		{
			((TMP_Text)TextMesh).text = string.Empty;
		}
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, zero, Time.deltaTime * 12f);
		((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, TargetLocalPosition, Time.deltaTime * 12f);
	}

	public override string GetTitle()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return SokLoc.Translate("label_status_effect", (LocParam[])(object)new LocParam[1] { LocParam.Create("status", MyStatusEffect.Name) });
	}

	public override string GetDescription()
	{
		if (!string.IsNullOrEmpty(MyStatusEffect.Lore))
		{
			return "<i>" + MyStatusEffect.Lore + "</i>\n\n" + MyStatusEffect.Description;
		}
		return MyStatusEffect.Description;
	}
}
