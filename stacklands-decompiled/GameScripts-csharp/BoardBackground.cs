using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardBackground : MonoBehaviour
{
	public List<Transform> SpriteParents;

	public GameBoard MyBoard;

	public List<SpriteArea> SpriteAreas;

	[HideInInspector]
	public List<RectanglePacker> RectanglePackers;

	public RectanglePacker RectanglePackerPrefab;

	public List<Sprite> SpritesToPlace;

	public List<Sprite> InsideSpritesToPlace;

	public SpriteRenderer NormalSpritePrefab;

	public SpriteRenderer InsideSpritePrefab;

	public float InsidePadding = 1f;

	public float NormalPadding = 0.1f;

	private const float extraWidth = 20f;

	private const float extraHeight = 10f;

	private const float maxWidth = 20f;

	private const float maxHeight = 12f;

	public void SetUpDefaultSpriteAreas()
	{
		if (SpriteAreas.Count > 0)
		{
			Debug.LogError((object)"Clear sprite areas before setting up default ones!");
			return;
		}
		SpriteAreas.Clear();
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.BottomLeft,
			Padding = NormalPadding
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.BottomCenter,
			Padding = NormalPadding,
			Expansion = 10f
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.BottomRight,
			Padding = NormalPadding
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.MiddleLeft,
			Padding = NormalPadding,
			Expansion = 20f
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.MiddleCenter,
			Padding = InsidePadding
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.MiddleRight,
			Padding = NormalPadding,
			Expansion = 20f
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.TopLeft,
			Padding = NormalPadding
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.TopCenter,
			Padding = NormalPadding,
			Expansion = 10f
		});
		SpriteAreas.Add(new SpriteArea
		{
			Region = BackgroundRegion.TopRight,
			Padding = NormalPadding
		});
	}

	public void CreateRectanglePackers()
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in ((IEnumerable)((Component)this).transform).Cast<Transform>().ToList())
		{
			Object.DestroyImmediate((Object)(object)((Component)item).gameObject);
		}
		RectanglePackers = new List<RectanglePacker>();
		for (int i = 0; i < SpriteAreas.Count; i++)
		{
			SpriteArea spriteArea = SpriteAreas[i];
			RectanglePacker rectanglePacker = Object.Instantiate<RectanglePacker>(RectanglePackerPrefab);
			((Component)rectanglePacker).transform.SetParent(((Component)this).transform);
			List<Sprite> sprites;
			if (spriteArea.Region == BackgroundRegion.MiddleCenter)
			{
				sprites = InsideSpritesToPlace;
				rectanglePacker.SpriteRendererPrefab = InsideSpritePrefab;
			}
			else
			{
				sprites = SpritesToPlace;
				rectanglePacker.SpriteRendererPrefab = NormalSpritePrefab;
			}
			rectanglePacker.Padding = spriteArea.Padding;
			if (spriteArea.OverrideAllowedSprites)
			{
				sprites = spriteArea.AllowedSprites;
			}
			rectanglePacker.Sprites = sprites;
			((Object)((Component)rectanglePacker).gameObject).name = spriteArea.Region.ToString();
			Vector2 spriteAreaPosition = GetSpriteAreaPosition(spriteArea);
			((Component)rectanglePacker).transform.position = new Vector3(spriteAreaPosition.x, 0f, spriteAreaPosition.y);
			rectanglePacker.CurrentRectanglePivot = GetRegionPivot(spriteArea.Region);
			rectanglePacker.RectangleSize = GetSpriteAreaMaxSize(spriteArea);
			rectanglePacker.CurrentRectangleSize = GetSpriteAreaCurrentSize(spriteArea);
			RectanglePackers.Add(rectanglePacker);
		}
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		foreach (Transform spriteParent in SpriteParents)
		{
			list.AddRange(((Component)spriteParent).GetComponentsInChildren<SpriteRenderer>(true));
		}
		list = list.Distinct().ToList();
		foreach (SpriteRenderer item2 in list)
		{
			SpriteArea spriteArea2 = DetermineSpriteArea(new Vector2(((Component)item2).transform.position.x, ((Component)item2).transform.position.z));
			if (spriteArea2 == null)
			{
				Debug.LogError((object)("No sprite area found for " + ((Object)item2).name));
				continue;
			}
			SpriteRenderer obj = Object.Instantiate<SpriteRenderer>(item2);
			((Component)obj).gameObject.SetActive(true);
			RectanglePacker rectanglePacker2 = RectanglePackers[SpriteAreas.IndexOf(spriteArea2)];
			((Component)obj).transform.SetParent(rectanglePacker2.ExistingSpritesParent, true);
			((Component)obj).transform.position = ((Component)item2).transform.position;
		}
		foreach (RectanglePacker rectanglePacker3 in RectanglePackers)
		{
			rectanglePacker3.Pack(rectanglePacker3.GetExistingSprites());
			rectanglePacker3.UpdateActiveSprites();
		}
	}

	public void UpdateBoardBackground()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < SpriteAreas.Count; i++)
		{
			SpriteArea spriteArea = SpriteAreas[i];
			RectanglePacker rectanglePacker = RectanglePackers[i];
			rectanglePacker.CurrentRectangleSize = GetSpriteAreaCurrentSize(spriteArea);
			Vector2 spriteAreaPosition = GetSpriteAreaPosition(spriteArea);
			((Component)rectanglePacker).transform.position = new Vector3(spriteAreaPosition.x, 0f, spriteAreaPosition.y);
			rectanglePacker.UpdateActiveSprites();
		}
	}

	private void Awake()
	{
		UpdateBoardBackground();
	}

	private Vector2 GetRegionPivot(BackgroundRegion region)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector2 regionDirection = GetRegionDirection(region);
		if (regionDirection.x == 1f)
		{
			regionDirection.x = 0f;
		}
		else if (regionDirection.x == 0f)
		{
			regionDirection.x = 1f;
		}
		if (regionDirection.y == 1f)
		{
			regionDirection.y = 0f;
		}
		else if (regionDirection.y == 0f)
		{
			regionDirection.y = 1f;
		}
		return regionDirection;
	}

	private float GetPrevExpansion(SpriteArea spriteArea)
	{
		float num = 0f;
		for (int i = 0; i < SpriteAreas.Count; i++)
		{
			if (SpriteAreas[i] == spriteArea)
			{
				return num;
			}
			if (SpriteAreas[i].Region == spriteArea.Region)
			{
				num += SpriteAreas[i].Expansion;
			}
		}
		return num;
	}

	private Vector2 GetSpriteAreaCurrentSize(SpriteArea spriteArea)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = MyBoard.WorldBounds;
		BackgroundRegion region = spriteArea.Region;
		if (IsCornerRegion(spriteArea.Region))
		{
			return new Vector2(20f, 10f);
		}
		switch (region)
		{
		case BackgroundRegion.BottomCenter:
		case BackgroundRegion.TopCenter:
			return new Vector2(((Bounds)(ref worldBounds)).size.x, spriteArea.Expansion);
		case BackgroundRegion.MiddleLeft:
		case BackgroundRegion.MiddleRight:
			return new Vector2(spriteArea.Expansion, ((Bounds)(ref worldBounds)).size.z);
		case BackgroundRegion.MiddleCenter:
			return new Vector2(((Bounds)(ref worldBounds)).size.x, ((Bounds)(ref worldBounds)).size.z);
		default:
			throw new Exception($"{region} is not a valid region");
		}
	}

	private Vector2 GetSpriteAreaMaxSize(SpriteArea spriteArea)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		BackgroundRegion region = spriteArea.Region;
		if (IsCornerRegion(region))
		{
			return new Vector2(20f, 10f);
		}
		switch (region)
		{
		case BackgroundRegion.BottomCenter:
		case BackgroundRegion.TopCenter:
			return new Vector2(20f, spriteArea.Expansion);
		case BackgroundRegion.MiddleLeft:
		case BackgroundRegion.MiddleRight:
			return new Vector2(spriteArea.Expansion, 12f);
		case BackgroundRegion.MiddleCenter:
			return new Vector2(20f, 12f);
		default:
			throw new Exception($"{region} is not a valid region");
		}
	}

	private Vector2 GetSpriteAreaPosition(SpriteArea spriteArea)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Vector2 spriteAreaMaxSize = GetSpriteAreaMaxSize(spriteArea);
		Vector2 regionDirection = GetRegionDirection(spriteArea.Region);
		Bounds worldBounds = MyBoard.WorldBounds;
		Vector2 val = new Vector2(Mathf.Lerp(((Bounds)(ref worldBounds)).min.x, ((Bounds)(ref worldBounds)).max.x, regionDirection.x), Mathf.Lerp(((Bounds)(ref worldBounds)).min.z, ((Bounds)(ref worldBounds)).max.z, regionDirection.y));
		Vector2 val2 = regionDirection - new Vector2(0.5f, 0.5f);
		return val + val2 * GetPrevExpansion(spriteArea) * 2f + new Vector2(val2.x * spriteAreaMaxSize.x, val2.y * spriteAreaMaxSize.y);
	}

	private Vector2 GetRegionDirection(BackgroundRegion region)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)((int)region % 3) * 0.5f, (float)((int)region / 3) * 0.5f);
	}

	private bool IsCornerRegion(BackgroundRegion region)
	{
		if (region != BackgroundRegion.TopLeft && region != BackgroundRegion.TopRight && region != BackgroundRegion.BottomLeft)
		{
			return region == BackgroundRegion.BottomRight;
		}
		return true;
	}

	private Bounds GetSpriteAreaBounds(SpriteArea spriteArea)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		int index = SpriteAreas.IndexOf(spriteArea);
		return RectanglePackers[index].GetCurrentWorldBounds();
	}

	private SpriteArea DetermineSpriteArea(Vector2 pos)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		foreach (SpriteArea spriteArea in SpriteAreas)
		{
			Bounds spriteAreaBounds = GetSpriteAreaBounds(spriteArea);
			float x = ((Bounds)(ref spriteAreaBounds)).min.x;
			float x2 = ((Bounds)(ref spriteAreaBounds)).max.x;
			float z = ((Bounds)(ref spriteAreaBounds)).min.z;
			float z2 = ((Bounds)(ref spriteAreaBounds)).max.z;
			if (pos.x > x && pos.x < x2 && pos.y > z && pos.y < z2)
			{
				return spriteArea;
			}
		}
		return null;
	}
}
