using System;
using UnityEngine;

public class EnergyCableDrawer : ShapeDrawer
{
	public Renderer CableRenderer;

	private MaterialPropertyBlock propBlock;

	public Material LowVoltageMaterial;

	public Material BehindMaterial;

	public Material HighVoltageMaterial;

	private int start = Shader.PropertyToID("_Start");

	private int end = Shader.PropertyToID("_End");

	private int middle = Shader.PropertyToID("_Middle");

	public override Type DrawingType => typeof(EnergyCable);

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		propBlock = new MaterialPropertyBlock();
	}

	private Material GetCurrentMaterial(EnergyCable cable)
	{
		if (WorldManager.instance.CurrentView == ViewType.Energy)
		{
			if (cable.IsLowVoltage)
			{
				return LowVoltageMaterial;
			}
			return HighVoltageMaterial;
		}
		return BehindMaterial;
	}

	public override void UpdateShape()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		EnergyCable cable = (EnergyCable)(object)base.MyShape;
		CableRenderer.sharedMaterial = GetCurrentMaterial(cable);
		CableRenderer.GetPropertyBlock(propBlock);
		propBlock.SetVector(start, new Vector4(cable.Start.x, cable.Start.z));
		propBlock.SetVector(end, new Vector4(cable.End.x, cable.End.z));
		propBlock.SetVector(middle, new Vector4(cable.Middle.x, cable.Middle.z));
		CableRenderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(cable.Start, cable.End, 0.5f);
		position.y = Mathf.Min(cable.Start.y, cable.End.y);
		if (WorldManager.instance.CurrentView != ViewType.Energy)
		{
			position.y = 0f;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(cable.End.x - cable.Start.x), 1f, Mathf.Abs(cable.End.z - cable.Start.z));
		((Component)this).transform.position = position;
		((Component)CableRenderer).transform.localScale = new Vector3(val.x + 1.5f, val.z + 1.5f, 1f);
	}
}
