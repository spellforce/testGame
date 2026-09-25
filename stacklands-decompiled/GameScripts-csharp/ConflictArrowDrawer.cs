using System;
using Shapes;
using UnityEngine;

public class ConflictArrowDrawer : ShapeDrawer
{
	public Transform VeryEffectiveText;

	public Rectangle VeryEffectiveRect;

	public Color OutlineColor = Color.black;

	public float Length = 0.1f;

	public float Thickness = 0.1f;

	public float OutlineThickness = 0.05f;

	public Renderer ArrowRenderer;

	private MaterialPropertyBlock propBlock;

	public ConflictArrow Arrow => (ConflictArrow)(object)base.MyShape;

	public override Type DrawingType => typeof(ConflictArrow);

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		ArrowRenderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(Arrow.Start.x, Arrow.Start.z));
		propBlock.SetVector("_End", new Vector4(Arrow.End.x, Arrow.End.z));
		propBlock.SetColor("_Color", Arrow.Color);
		propBlock.SetColor("_OutlineColor", OutlineColor);
		ArrowRenderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(Arrow.Start, Arrow.End, 0.5f);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(Arrow.End.x - Arrow.Start.x), 1f, Mathf.Abs(Arrow.End.z - Arrow.Start.z));
		((Component)this).transform.position = position;
		((Component)ArrowRenderer).transform.localScale = new Vector3(val.x + 1f, val.z + 1f, 1f);
		((Component)VeryEffectiveText).transform.position = Vector3.Lerp(Arrow.Start, Arrow.End, 0.5f) + Vector3.up * 0.03f;
		((Component)VeryEffectiveText).gameObject.SetActive(Arrow.VeryEffective);
		((Component)VeryEffectiveRect).gameObject.SetActive(Arrow.VeryEffective);
		((Component)VeryEffectiveText).transform.rotation = ((Component)Camera.main).transform.rotation;
	}
}
