using UnityEngine;

public class GridQuad : MonoBehaviour
{
	public MeshRenderer meshRenderer;

	private void Update()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = GameCamera.instance.ScreenPosToWorldPos(new Vector3((float)Screen.width, (float)Screen.height) * 0.5f);
		position.y = 0.2f;
		((Component)this).transform.position = position;
		((Renderer)meshRenderer).enabled = WorldManager.instance.gridAlpha >= 0.001f;
	}
}
