using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitText : MonoBehaviour
{
	public TextMeshPro TextMesh;

	[HideInInspector]
	public Combatable TargetCombatable;

	public bool IsMiss;

	public SpriteRenderer BackgroundRenderer;

	private float timer;

	private Vector3 startScale;

	public float InitialScaleUp = 2f;

	public TextMeshPro VeryEffectiveText;

	public bool IsVeryEffective;

	private void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		startScale = ((Component)this).transform.localScale;
		Transform transform = ((Component)this).transform;
		transform.localScale *= InitialScaleUp;
	}

	public void SetVeryEffective(bool veryEffective)
	{
		IsVeryEffective = veryEffective;
		if (IsVeryEffective)
		{
			VeryEffectiveText = Object.Instantiate<TextMeshPro>(PrefabManager.instance.IsVeryEffectiveText);
		}
	}

	private void Start()
	{
		SetPosition();
	}

	private void OnDestroy()
	{
		if ((Object)(object)VeryEffectiveText != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)VeryEffectiveText).gameObject);
		}
	}

	private void Update()
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		timer += Time.deltaTime;
		if ((Object)(object)TargetCombatable.CurrentHitText != (Object)(object)this || (Object)(object)TargetCombatable == (Object)null)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		if (timer >= 0.8f)
		{
			float a = ((Graphic)TextMesh).color.a;
			Color color = ((Graphic)TextMesh).color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 12f);
			((Graphic)TextMesh).color = color;
			if ((Object)(object)BackgroundRenderer != (Object)null)
			{
				color = BackgroundRenderer.color;
				color.a = a;
				BackgroundRenderer.color = color;
			}
			if ((Object)(object)VeryEffectiveText != (Object)null)
			{
				color = ((Graphic)VeryEffectiveText).color;
				color.a = a;
				((Graphic)VeryEffectiveText).color = color;
			}
			if (color.a < 0.01f)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
		}
		SetPosition();
		((Component)this).transform.localEulerAngles = new Vector3(((Component)this).transform.localEulerAngles.x, ((Component)this).transform.localEulerAngles.y, 0f);
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, startScale, Time.deltaTime * 10f);
		if ((Object)(object)VeryEffectiveText != (Object)null)
		{
			VeryEffectiveText.transform.localScale = Vector3.Lerp(VeryEffectiveText.transform.localScale, Vector3.one, Time.deltaTime * 10f);
		}
	}

	private void SetPosition()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		GameCard myGameCard = TargetCombatable.MyGameCard;
		if ((Object)(object)VeryEffectiveText != (Object)null)
		{
			VeryEffectiveText.transform.rotation = Quaternion.AngleAxis(-14f, Vector3.up) * ((Component)myGameCard).transform.rotation;
			VeryEffectiveText.transform.position = ((Component)myGameCard).transform.position;
		}
		if (!IsMiss)
		{
			((Component)this).transform.rotation = ((Component)myGameCard).transform.rotation;
			((Component)this).transform.position = myGameCard.HitTextPosition.position;
		}
		else
		{
			((Component)this).transform.rotation = ((Component)Camera.main).transform.rotation;
		}
	}
}
