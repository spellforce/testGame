using System;
using UnityEngine;

public class ConveyorArrowDrawer : ShapeDrawer
{
	public Color OutlineColor = Color.black;

	public float Length = 0.1f;

	public float Thickness = 0.1f;

	public float OutlineThickness = 0.05f;

	public Renderer ArrowRenderer;

	private MaterialPropertyBlock propBlock;

	public ConveyorArrow Arrow => (ConveyorArrow)(object)base.MyShape;

	public override Type DrawingType => typeof(ConveyorArrow);

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
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		ArrowRenderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(Arrow.Start.x, Arrow.Start.z));
		propBlock.SetVector("_End", new Vector4(Arrow.End.x, Arrow.End.z));
		propBlock.SetColor("_OutlineColor", OutlineColor);
		ArrowRenderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(Arrow.Start, Arrow.End, 0.5f);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(Arrow.End.x - Arrow.Start.x), 1f, Mathf.Abs(Arrow.End.z - Arrow.Start.z));
		((Component)this).transform.position = position;
		((Component)ArrowRenderer).transform.localScale = new Vector3(val.x + 1f, val.z + 1f, 1f);
	}
}
