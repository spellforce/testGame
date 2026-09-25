using UnityEngine;

public class EquipmentPosition : MonoBehaviour
{
	public SpriteRenderer ShadowRenderer;

	public SpriteRenderer IconRenderer;

	private Vector3 startOffset;

	private GameCard parentCard;

	private float alpha;

	public bool IsWorkerPosition;

	private float timer;

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		parentCard = ((Component)this).GetComponentInParent<GameCard>();
		startOffset = ((Component)this).transform.localPosition;
		timer = Random.Range(0f, 10f);
		SpriteRenderer shadowRenderer = ShadowRenderer;
		SpriteRenderer iconRenderer = IconRenderer;
		Color color = default(Color);
		((Color)(ref color))._002Ector(1f, 1f, 1f, 0f);
		iconRenderer.color = color;
		shadowRenderer.color = color;
		SpriteRenderer shadowRenderer2 = ShadowRenderer;
		bool enabled = (((Renderer)IconRenderer).enabled = false);
		((Renderer)shadowRenderer2).enabled = enabled;
	}

	private void Update()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)parentCard.MyBoard == (Object)null) && parentCard.MyBoard.IsCurrent)
		{
			timer += Time.deltaTime * WorldManager.instance.TimeScale;
			alpha = Mathf.Lerp(alpha, (parentCard.ShowInventory && ((parentCard.IsWorkerInventory && IsWorkerPosition) || (!parentCard.IsWorkerInventory && !IsWorkerPosition))) ? 1f : 0f, Time.deltaTime * 20f);
			SpriteRenderer shadowRenderer = ShadowRenderer;
			SpriteRenderer iconRenderer = IconRenderer;
			Color color = default(Color);
			((Color)(ref color))._002Ector(1f, 1f, 1f, alpha);
			iconRenderer.color = color;
			shadowRenderer.color = color;
			SpriteRenderer shadowRenderer2 = ShadowRenderer;
			bool enabled = (((Renderer)IconRenderer).enabled = alpha > 0.01f);
			((Renderer)shadowRenderer2).enabled = enabled;
			float x = timer * 0.5f;
			((Component)this).transform.localPosition = startOffset + new Vector3(Perlin(x, 0.2f), Perlin(x, 0.6f)) * 0.01f;
		}
	}

	private float Perlin(float x, float y)
	{
		return Mathf.PerlinNoise(x, y) * 2f - 1f;
	}
}
