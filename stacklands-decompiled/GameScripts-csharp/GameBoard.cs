using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
	public string Id = "";

	public BoxCollider WorldCollider;

	public BoxCollider TightWorldCollider;

	public Transform TopBgElements;

	public Transform BottomBgElements;

	public Transform LeftBgElements;

	public Transform RightBgElements;

	public Transform CameraIntroPosition;

	public Color CardHighlightColor;

	[HideInInspector]
	public TextAsset BoardPreset;

	[HideInInspector]
	public float WorldSizeIncrease;

	private float PreviousWorldSizeIncrease;

	public BoardBackground boardBackground;

	public List<string> BoosterIds;

	[HideInInspector]
	public float PackLineWidth;

	public BoardOptions BoardOptions;

	private Location _location;

	private bool locationSet;

	private Bounds cachedTightBounds;

	private bool hasCachedTightBounds;

	[HideInInspector]
	public Material MyMaterial;

	public Location Location
	{
		get
		{
			if (!locationSet)
			{
				locationSet = true;
				if (Id == "forest")
				{
					_location = Location.Forest;
				}
				else if (Id == "main")
				{
					_location = Location.Mainland;
				}
				else if (Id == "island")
				{
					_location = Location.Island;
				}
				else if (Id == "happiness")
				{
					_location = Location.Happiness;
				}
				else if (Id == "greed")
				{
					_location = Location.Greed;
				}
				else if (Id == "death")
				{
					_location = Location.Death;
				}
				else
				{
					if (!(Id == "cities"))
					{
						throw new ArgumentException();
					}
					_location = Location.Cities;
				}
			}
			return _location;
		}
	}

	public bool IsCurrent => (Object)(object)WorldManager.instance.CurrentBoard == (Object)(object)this;

	public Bounds WorldBounds
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			WorldCollider.ToWorldSpaceBox(out var center, out var halfExtents, out var _);
			return new Bounds(center, halfExtents * 2f + WorldSizeIncrease * new Vector3(1f, 0f, 0.58f) * 2f);
		}
	}

	public Bounds TightWorldBounds
	{
		get
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (!hasCachedTightBounds)
			{
				TightWorldCollider.ToWorldSpaceBox(out var center, out var halfExtents, out var _);
				cachedTightBounds = new Bounds(center, halfExtents * 2f + WorldSizeIncrease * new Vector3(1f, 0f, 0.58f) * 2f);
				hasCachedTightBounds = true;
			}
			return cachedTightBounds;
		}
	}

	public string BoardName => SokLoc.Translate("board_" + Id + "_name");

	private void Awake()
	{
		if ((Object)(object)BoardOptions.PostProcessVolume != (Object)null)
		{
			((Behaviour)BoardOptions.PostProcessVolume).enabled = false;
		}
		MyMaterial = ((Renderer)((Component)this).GetComponent<MeshRenderer>()).sharedMaterial;
	}

	private void Start()
	{
		CreatePackLine componentInChildren = ((Component)this).GetComponentInChildren<CreatePackLine>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			componentInChildren.CreateBoosterBoxes(BoosterIds, BoardOptions.Currency);
			PackLineWidth = componentInChildren.TotalWidth;
		}
	}

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (IsCurrent)
		{
			Shader.SetGlobalFloat("_WorldSizeIncrease", WorldSizeIncrease);
		}
		float num = WorldManager.instance.DetermineTargetWorldSize(this);
		WorldSizeIncrease = Mathf.Lerp(WorldSizeIncrease, num, Time.deltaTime * 12f);
		if (WorldSizeIncrease != PreviousWorldSizeIncrease && (Object)(object)boardBackground != (Object)null)
		{
			boardBackground.UpdateBoardBackground();
		}
		TopBgElements.localPosition = Vector3.forward * WorldSizeIncrease * 0.58f;
		BottomBgElements.localPosition = Vector3.back * WorldSizeIncrease * 0.58f;
		LeftBgElements.localPosition = Vector3.left * WorldSizeIncrease;
		RightBgElements.localPosition = Vector3.right * WorldSizeIncrease;
		PreviousWorldSizeIncrease = WorldSizeIncrease;
		if ((Object)(object)BoardOptions.PostProcessVolume != (Object)null)
		{
			((Behaviour)BoardOptions.PostProcessVolume).enabled = IsCurrent;
		}
		hasCachedTightBounds = false;
	}

	public Vector3 NormalizedPosToWorldPos(Vector2 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = WorldBounds;
		float num = Mathf.Lerp(((Bounds)(ref worldBounds)).min.x, ((Bounds)(ref worldBounds)).max.x, pos.x);
		float num2 = Mathf.Lerp(((Bounds)(ref worldBounds)).min.z, ((Bounds)(ref worldBounds)).max.z, pos.y);
		return new Vector3(num, 0f, num2);
	}

	public Vector2 WorldPosToNormalizedPos(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = WorldBounds;
		float num = Mathf.InverseLerp(((Bounds)(ref worldBounds)).min.x, ((Bounds)(ref worldBounds)).max.x, pos.x);
		float num2 = Mathf.InverseLerp(((Bounds)(ref worldBounds)).min.z, ((Bounds)(ref worldBounds)).max.z, pos.z);
		return new Vector2(num, num2);
	}

	public Vector3 MiddleOfBoard()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return NormalizedPosToWorldPos(new Vector2(0.5f, 0.5f));
	}
}
