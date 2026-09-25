using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RectanglePacker : MonoBehaviour
{
	private class SpriteMetadata
	{
		public Sprite Sprite;

		public Vector2 Size;

		public float Area => Size.x * Size.y;
	}

	public List<Sprite> Sprites;

	public Vector2 RectangleSize;

	public Vector2 CurrentRectangleSize;

	public Vector2 CurrentRectanglePivot;

	public float Padding = 0.4f;

	public int SpriteCount = 250;

	public Transform NewSpritesParent;

	public Transform ExistingSpritesParent;

	public SpriteRenderer SpriteRendererPrefab;

	[HideInInspector]
	public List<PlacedSprite> PlacedSprites;

	private Vector2 DetermineSpriteSize(Sprite spr)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = spr.rect;
		float width = ((Rect)(ref rect)).width;
		rect = spr.rect;
		return new Vector2(width, ((Rect)(ref rect)).height) / spr.pixelsPerUnit;
	}

	private List<SpriteMetadata> CreateSpriteMetadatas(List<Sprite> sprites)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		List<SpriteMetadata> list = new List<SpriteMetadata>();
		foreach (Sprite sprite in sprites)
		{
			Vector2 size = DetermineSpriteSize(sprite);
			SpriteMetadata item = new SpriteMetadata
			{
				Sprite = sprite,
				Size = size
			};
			list.Add(item);
		}
		return list;
	}

	public List<PlacedSprite> GetExistingSprites()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		List<PlacedSprite> list = new List<PlacedSprite>();
		SpriteRenderer[] componentsInChildren = ((Component)ExistingSpritesParent).GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer val in componentsInChildren)
		{
			list.Add(new PlacedSprite
			{
				Transform = ((Component)val).transform,
				Size = PadSpriteSize(DetermineSpriteSize(val.sprite)),
				Sprite = val.sprite,
				Position = WorldPosToLocalPos(((Component)val).transform.position)
			});
		}
		return list;
	}

	private Vector2 WorldPosToLocalPos(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - new Vector3(RectangleSize.x * 0.5f, 0f, RectangleSize.y * 0.5f);
		return new Vector2(pos.x - val.x, pos.z - val.z);
	}

	private Vector2 PadSpriteSize(Vector2 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		size += Vector2.one * Padding;
		size.x = Mathf.Max(0.02f, size.x);
		size.y = Mathf.Max(0.02f, size.y);
		return size;
	}

	public void Pack(List<PlacedSprite> initial)
	{
		PlacedSprites = SpawnNewSprites(initial);
		CreateSprites(PlacedSprites);
	}

	public List<PlacedSprite> SpawnNewSprites(List<PlacedSprite> initial)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		List<SpriteMetadata> list = CreateSpriteMetadatas(Sprites);
		WeightedRandomBag<SpriteMetadata> weightedRandomBag = new WeightedRandomBag<SpriteMetadata>();
		foreach (SpriteMetadata item in list)
		{
			weightedRandomBag.AddEntry(item, 1f);
		}
		if (Sprites.Count > 0)
		{
			Vector2 position = default(Vector2);
			for (int i = 0; i < SpriteCount; i++)
			{
				SpriteMetadata spriteMetadata = weightedRandomBag.Choose();
				Vector2 size = spriteMetadata.Size;
				float num = Random.Range(size.x * 0.5f, RectangleSize.x - size.x * 0.5f);
				float num2 = Random.Range(size.y * 0.5f, size.y);
				((Vector2)(ref position))._002Ector(num, num2);
				PlacedSprite placedSprite = new PlacedSprite
				{
					Sprite = spriteMetadata.Sprite,
					Size = PadSpriteSize(size),
					Position = position
				};
				int num3 = 0;
				while (OverlapsWithAny(initial, placedSprite))
				{
					if (num3 % 5 == 0)
					{
						placedSprite.Position.y += 1f;
					}
					else
					{
						placedSprite.Position.x = Random.Range(size.x * 0.5f, RectangleSize.x - size.x * 0.5f);
					}
					num3++;
				}
				initial.Add(placedSprite);
			}
		}
		initial.RemoveAll((PlacedSprite x) => x.Position.y > RectangleSize.y);
		return initial;
	}

	public void UpdateActiveSprites()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		foreach (PlacedSprite placedSprite in PlacedSprites)
		{
			placedSprite.IsVisible = PositionInCurrentRectangle(placedSprite.Position);
			((Component)placedSprite.Transform).gameObject.SetActiveFast(placedSprite.IsVisible);
		}
	}

	private void CreateSprites(List<PlacedSprite> sprites)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in ((IEnumerable)NewSpritesParent).Cast<Transform>().ToList())
		{
			Object.DestroyImmediate((Object)(object)((Component)item).gameObject);
		}
		foreach (PlacedSprite sprite in sprites)
		{
			if (!((Object)(object)sprite.Transform != (Object)null))
			{
				SpriteRenderer val = Object.Instantiate<SpriteRenderer>(SpriteRendererPrefab);
				((Component)val).transform.SetParent(NewSpritesParent);
				Vector3 worldPos = GetWorldPos(sprite);
				worldPos.y = -0.02f;
				((Component)val).transform.position = worldPos;
				((Object)((Component)val).gameObject).name = ((Object)sprite.Sprite).name;
				val.sprite = sprite.Sprite;
				sprite.Transform = ((Component)val).transform;
			}
		}
	}

	private bool OverlapsWithAny(List<PlacedSprite> existing, PlacedSprite newSprite)
	{
		foreach (PlacedSprite item in existing)
		{
			if (Overlaps(item, newSprite))
			{
				return true;
			}
		}
		return false;
	}

	private bool Overlaps(PlacedSprite a, PlacedSprite b)
	{
		if (a.Left < b.Right && a.Right > b.Left && a.Top > b.Bottom && a.Bottom < b.Top)
		{
			return true;
		}
		return false;
	}

	private Vector3 GetWorldPos(PlacedSprite p)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return LocalPosToWorldPos(p.Position);
	}

	private Vector3 LocalPosToWorldPos(Vector2 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position - new Vector3(RectangleSize.x * 0.5f, 0f, RectangleSize.y * 0.5f) + new Vector3(pos.x, 0f, pos.y);
	}

	private Vector2 GetCurrentRectanglePosition()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(CurrentRectanglePivot.x * RectangleSize.x, CurrentRectanglePivot.y * RectangleSize.y);
		Vector2 currentRectanglePivot = CurrentRectanglePivot;
		currentRectanglePivot.x = (1f - currentRectanglePivot.x) * 2f - 1f;
		currentRectanglePivot.y = (1f - currentRectanglePivot.y) * 2f - 1f;
		return currentRectanglePivot * CurrentRectangleSize * 0.5f + val;
	}

	private bool PositionInCurrentRectangle(Vector2 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Vector2 currentRectanglePosition = GetCurrentRectanglePosition();
		float num = currentRectanglePosition.x - CurrentRectangleSize.x * 0.5f;
		float num2 = currentRectanglePosition.x + CurrentRectangleSize.x * 0.5f;
		float num3 = currentRectanglePosition.y + CurrentRectangleSize.y * 0.5f;
		float num4 = currentRectanglePosition.y - CurrentRectangleSize.y * 0.5f;
		if (pos.x > num && pos.x < num2 && pos.y > num4)
		{
			return pos.y < num3;
		}
		return false;
	}

	private bool InCurrentRectangle(PlacedSprite spr)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector2 currentRectanglePosition = GetCurrentRectanglePosition();
		float num = currentRectanglePosition.x - CurrentRectangleSize.x * 0.5f;
		float num2 = currentRectanglePosition.x + CurrentRectangleSize.x * 0.5f;
		float num3 = currentRectanglePosition.y + CurrentRectangleSize.y * 0.5f;
		float num4 = currentRectanglePosition.y - CurrentRectangleSize.y * 0.5f;
		if (spr.Left < num2 && spr.Right > num && spr.Top > num4 && spr.Bottom < num3)
		{
			return true;
		}
		return false;
	}

	public Bounds GetCurrentWorldBounds()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(LocalPosToWorldPos(GetCurrentRectanglePosition()), new Vector3(CurrentRectangleSize.x, 0.1f, CurrentRectangleSize.y));
	}

	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.white;
		Gizmos.color = Color.red;
		Bounds currentWorldBounds = GetCurrentWorldBounds();
		Gizmos.DrawWireCube(((Bounds)(ref currentWorldBounds)).center, ((Bounds)(ref currentWorldBounds)).size);
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.blue;
		if (PlacedSprites == null)
		{
			return;
		}
		foreach (PlacedSprite placedSprite in PlacedSprites)
		{
			Gizmos.color = (PositionInCurrentRectangle(placedSprite.Position) ? Color.yellow : Color.blue);
			Gizmos.DrawWireCube(GetWorldPos(placedSprite), new Vector3(placedSprite.Size.x, 0.1f, placedSprite.Size.y));
		}
	}
}
