using System;
using UnityEngine;

public class TransportArrowMainlandDrawer : ShapeDrawer
{
	public Renderer Renderer;

	private MaterialPropertyBlock propBlock;

	public Material FrontMaterial;

	public Material BehindMaterial;

	public TransportArrowMainland Cable => (TransportArrowMainland)(object)base.MyShape;

	public override Type DrawingType => typeof(TransportArrowMainland);

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		Renderer.sharedMaterial = ((WorldManager.instance.CurrentView == ViewType.Transport) ? FrontMaterial : BehindMaterial);
		Renderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(Cable.Start.x, Cable.Start.z));
		propBlock.SetVector("_End", new Vector4(Cable.End.x, Cable.End.z));
		propBlock.SetVector("_Middle", new Vector4(Cable.Middle.x, Cable.Middle.z));
		Renderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(Cable.Start, Cable.End, 0.5f);
		position.y = Mathf.Min(Cable.Start.y, Cable.End.y);
		if (WorldManager.instance.CurrentView != ViewType.Transport)
		{
			position.y = 0f;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(Cable.End.x - Cable.Start.x), 1f, Mathf.Abs(Cable.End.z - Cable.Start.z));
		((Component)this).transform.position = position;
		((Component)Renderer).transform.localScale = new Vector3(val.x + 1.5f, val.z + 1.5f, 1f);
	}
}
