using System;
using UnityEngine;

public class TransportArrowDrawer : ShapeDrawer
{
	public Renderer Renderer;

	private MaterialPropertyBlock propBlock;

	public Material FrontMaterial;

	public Material BehindMaterial;

	private int start = Shader.PropertyToID("_Start");

	private int end = Shader.PropertyToID("_End");

	private int middle = Shader.PropertyToID("_Middle");

	public override Type DrawingType => typeof(TransportArrow);

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		TransportArrow transportArrow = (TransportArrow)(object)base.MyShape;
		Renderer.sharedMaterial = ((WorldManager.instance.CurrentView == ViewType.Transport) ? FrontMaterial : BehindMaterial);
		Renderer.GetPropertyBlock(propBlock);
		propBlock.SetVector(start, new Vector4(transportArrow.Start.x, transportArrow.Start.z));
		propBlock.SetVector(end, new Vector4(transportArrow.End.x, transportArrow.End.z));
		propBlock.SetVector(middle, new Vector4(transportArrow.Middle.x, transportArrow.Middle.z));
		Renderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(transportArrow.Start, transportArrow.End, 0.5f);
		position.y = Mathf.Min(transportArrow.Start.y, transportArrow.End.y);
		if (WorldManager.instance.CurrentView != ViewType.Transport)
		{
			position.y = 0f;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(transportArrow.End.x - transportArrow.Start.x), 1f, Mathf.Abs(transportArrow.End.z - transportArrow.Start.z));
		((Component)this).transform.position = position;
		((Component)Renderer).transform.localScale = new Vector3(val.x + 1.5f, val.z + 1.5f, 1f);
	}
}
