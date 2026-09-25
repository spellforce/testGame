using System;
using UnityEngine;

public class SewerPipeDrawer : ShapeDrawer
{
	public Renderer Renderer;

	private MaterialPropertyBlock propBlock;

	public Material SewerFrontMaterial;

	public Material SewerBehindMaterial;

	public override Type DrawingType => typeof(SewerPipe);

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		SewerPipe sewerPipe = (SewerPipe)(object)base.MyShape;
		Renderer.sharedMaterial = ((WorldManager.instance.CurrentView == ViewType.Sewer) ? SewerFrontMaterial : SewerBehindMaterial);
		Renderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(sewerPipe.Start.x, sewerPipe.Start.z));
		propBlock.SetVector("_End", new Vector4(sewerPipe.End.x, sewerPipe.End.z));
		propBlock.SetVector("_Middle", new Vector4(sewerPipe.Middle.x, sewerPipe.Middle.z));
		Renderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(sewerPipe.Start, sewerPipe.End, 0.5f);
		position.y = Mathf.Min(sewerPipe.Start.y, sewerPipe.End.y);
		if (WorldManager.instance.CurrentView != ViewType.Sewer)
		{
			position.y = 0f;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(sewerPipe.End.x - sewerPipe.Start.x), 1f, Mathf.Abs(sewerPipe.End.z - sewerPipe.Start.z));
		((Component)this).transform.position = position;
		((Component)Renderer).transform.localScale = new Vector3(val.x + 1.5f, val.z + 1.5f, 1f);
	}
}
