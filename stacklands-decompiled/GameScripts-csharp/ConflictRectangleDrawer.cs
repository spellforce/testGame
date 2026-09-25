using System;
using UnityEngine;

public class ConflictRectangleDrawer : ShapeDrawer
{
	public MeshRenderer Renderer;

	private MaterialPropertyBlock propBlock;

	public ConflictRectangle Rectangle => (ConflictRectangle)(object)base.MyShape;

	public override Type DrawingType => typeof(ConflictRectangle);

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = Rectangle.Center;
		((Component)Renderer).transform.localScale = new Vector3(Rectangle.Size.x, Rectangle.Size.y, 1f) + Vector3.one;
		((Renderer)Renderer).GetPropertyBlock(propBlock);
		propBlock.SetVector("_Size", new Vector4(Rectangle.Size.x, Rectangle.Size.y));
		((Renderer)Renderer).SetPropertyBlock(propBlock);
	}
}
