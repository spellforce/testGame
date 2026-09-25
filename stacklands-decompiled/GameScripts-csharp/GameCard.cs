using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameCard : Draggable, IGameCardOrCardData
{
	private enum PositionType
	{
		InConflict,
		InAttack,
		InStack,
		IsRoot,
		IsEquipped,
		InAnimation,
		None,
		IsWorking
	}

	public TextMeshPro CardNameText;

	public SpriteRenderer IconRenderer;

	public CardData CardData;

	public GameCard Parent;

	public GameCard Child;

	public GameCard LastParent;

	public Transform HitTextPosition;

	public Transform Visuals;

	public int ConnectorOutputIndex;

	public bool IsEquipped;

	public bool IsWorking;

	public bool ShowInventory;

	public GameCard EquipmentHolder;

	public List<GameCard> EquipmentChildren;

	public GameCard WorkerHolder;

	public List<GameCard> WorkerChildren = new List<GameCard>();

	public GameObject EnergyConnectorPrefab;

	public Transform EnergyConnectorTransform;

	private Vector3 startScale;

	public Renderer CardRenderer;

	public Rectangle HighlightRectangle;

	public SpriteRenderer CoinIcon;

	public TextMeshPro CoinText;

	public TextMeshPro SpecialText;

	public SpriteRenderer SpecialIcon;

	public SpriteRenderer CombatStatusCircle;

	public SpriteRenderer DropShadowRenderer;

	public Transform EquipmentRectangle;

	public Transform WorkerRectangle;

	public WorkerTransformHolder WorkerTransformHolder;

	public InventoryInteractable InventoryInteractable;

	public InventoryInteractable WorkerInventoryInteractable;

	public OnOffInteractable OnOffInteractable;

	private Vector3 onOffBasePosition;

	private Vector3 onOffTargetPosition;

	public SpriteRenderer HeadInventoryIcon;

	public SpriteRenderer TorsoInventoryIcon;

	public SpriteRenderer HandInventoryIcon;

	public SpriteRenderer WorkerInventoryIcon;

	public GameObject HeadEquipmentPosition;

	public GameObject TorsoEquipmentPosition;

	public GameObject HandEquipmentPosition;

	public Rectangle EquipmentButton;

	public Rectangle WorkerButton;

	public int? SpecialValue;

	public bool HighlightActive;

	private Vector3 lastPosition;

	public Vector3 SpawnRotation;

	private bool snappedToParent;

	private MaterialPropertyBlock propBlock;

	private MaterialPropertyBlock combatCirclePropBlock;

	public bool FaceUp;

	public SpriteRenderer NewCircle;

	private Vector3 newCircleStartSize;

	public ParticleSystem FoilParticles;

	protected List<MaterialChanger> materialChangers = new List<MaterialChanger>();

	[HideInInspector]
	public bool IsDemoCard;

	public GameCard BounceTarget;

	[HideInInspector]
	public bool PushEnabled = true;

	[HideInInspector]
	public bool SetY = true;

	[Header("Status")]
	public float DistanceBetweenStatusses = 0.01f;

	[HideInInspector]
	public List<StatusEffectElement> StatusEffectElements = new List<StatusEffectElement>();

	public Vector3 equipmentRectangleStartOffset;

	[HideInInspector]
	public bool ShowSpecialIcon;

	[HideInInspector]
	public bool StackUpdate;

	private CardPalette myCardPalette;

	public List<CardAnimation> CardAnimations = new List<CardAnimation>();

	[HideInInspector]
	private Action closeToTargetPositionCallback;

	[HideInInspector]
	public List<CardConnector> CardConnectorChildren = new List<CardConnector>();

	public Color CombatCircleColor;

	private int propColor = Shader.PropertyToID("_Color");

	private int propColor2 = Shader.PropertyToID("_Color2");

	private int propIconColor = Shader.PropertyToID("_IconColor");

	private int propHasSecondaryIcon = Shader.PropertyToID("_HasSecondaryIcon");

	private int propHasOutputDir = Shader.PropertyToID("_HasOutputDir");

	private int propSecondaryTex = Shader.PropertyToID("_SecondaryTex");

	private int propBigShineStrength = Shader.PropertyToID("_BigShineStrength");

	private int propShineStrength = Shader.PropertyToID("_ShineStrength");

	private int propFoil = Shader.PropertyToID("_Foil");

	private int propDamaged = Shader.PropertyToID("_Damaged");

	private int propIconTex = Shader.PropertyToID("_IconTex");

	[HideInInspector]
	public bool Destroyed;

	private List<GameCard> cardsInvolved = new List<GameCard>();

	public bool WasClicked;

	public bool IsNew;

	public float ZRotOffset;

	private Vector3 onOffVelocity;

	private Vector3 onOffTargetPos;

	private Color colOff = new Color(0f, 0f, 0f, 0.5f);

	private Color colOn = new Color(0f, 0f, 0f, 1f);

	private float ConnectorAmountOffset = 0.077f;

	private float CardTextOffset = 0.01f;

	public Rectangle StatusEffectBackground;

	private Transform statusEffectBackgroundTransform;

	private float statusEffectBackgroundWidth;

	private float flipTimer;

	public float RotWobbleAmp = 1f;

	public float RotWobbleSpeed = 1f;

	public float RotWobbleSpringiness = 1f;

	private float wobbleRotVelo;

	public bool AutoRotWobble;

	public float AutoRotWobbleTimer;

	public float AutoRotWobbleAmount = 0.1f;

	private float timer;

	private float rotWobbleTimer;

	private float curZ = 270f;

	public bool TimerRunning;

	public string Status;

	public float CurrentTimerTime;

	public float TargetTimerTime;

	public TimerAction TimerAction;

	public string TimerBlueprintId;

	public int TimerSubprintIndex;

	public bool SkipCitiesChecks;

	public string TimerActionId;

	public Statusbar CurrentStatusbar;

	[HideInInspector]
	public GameCard removedChild;

	public Transform StatusEffectElementParent;

	private float curHeight;

	[HideInInspector]
	public bool IsHit;

	protected override bool HasPhysics => true;

	public Vector3 Position => ((Component)this).transform.position;

	public override bool IsHovered => base.IsHovered;

	public static float CardHeight => PrefabManager.instance.GameCardPrefab.GetHeight();

	public bool BeingHovered
	{
		get
		{
			if ((Object)(object)WorldManager.instance.HoveredCard == (Object)(object)this)
			{
				return true;
			}
			if (IsParentOf(WorldManager.instance.HoveredCard) || IsChildOf(WorldManager.instance.HoveredCard))
			{
				return true;
			}
			return false;
		}
	}

	public override Vector3 AutoMoveSnapPosition
	{
		get
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)WorldManager.instance != (Object)null && (Object)(object)WorldManager.instance.DraggingCard != (Object)null)
			{
				return CardNameText.transform.position + new Vector3(0f, WorldManager.instance.CardOverlayHeightOffset, 0f - WorldManager.instance.CardOverlayOffset);
			}
			if ((Object)(object)Child == (Object)null && (Object)(object)Parent == (Object)null)
			{
				return ((Component)this).transform.position;
			}
			return CardNameText.transform.position;
		}
	}

	public override bool CanBeAutoMovedTo
	{
		get
		{
			if ((Object)(object)WorldManager.instance.DraggingCard != (Object)null && (Object)(object)Child != (Object)null)
			{
				return false;
			}
			if (IsEquipped && ((Object)(object)WorldManager.instance.DraggingCard == (Object)(object)EquipmentHolder || !EquipmentHolder.ShowInventory))
			{
				return false;
			}
			if (IsWorking && ((Object)(object)WorldManager.instance.DraggingCard == (Object)(object)WorkerHolder || !WorkerHolder.ShowInventory))
			{
				return false;
			}
			return !BeingDragged;
		}
	}

	public bool InventoryVisible => ShowInventory;

	public bool IsWorkerInventory
	{
		get
		{
			CardData cardData = CardData;
			if (cardData == null)
			{
				return false;
			}
			return cardData.WorkerAmount > 0;
		}
	}

	public bool TimerRunningInStack => GetAllCardsInStack().Any((GameCard x) => x.TimerRunning);

	public bool HasParent => (Object)(object)Parent != (Object)null;

	public bool HasChild => (Object)(object)Child != (Object)null;

	protected override float Mass
	{
		get
		{
			float num = 1f;
			if (CardData is Mob)
			{
				num += 50f;
			}
			if (CardData.MyCardType == CardType.Structures && CardData.IsBuilding)
			{
				num += 8f;
			}
			if (CardData is HeavyFoundation)
			{
				num += 1000f;
			}
			if ((Object)(object)Child != (Object)null)
			{
				num += Child.Mass;
			}
			return num;
		}
	}

	public bool IsCollapsed
	{
		get
		{
			if (!BeingDragged)
			{
				return false;
			}
			if ((Object)(object)WorldManager.instance.NearbyCardTarget != (Object)null)
			{
				return true;
			}
			if (GetRootCard().GetChildCount() >= 10 && !WorldManager.instance.IsShiftDragging)
			{
				return true;
			}
			return false;
		}
	}

	public Combatable Combatable => CardData as Combatable;

	public bool InConflict
	{
		get
		{
			if ((Object)(object)Combatable != (Object)null)
			{
				return Combatable.InConflict;
			}
			return false;
		}
	}

	public bool InAttack
	{
		get
		{
			if ((Object)(object)Combatable != (Object)null)
			{
				return Combatable.InAttack;
			}
			return false;
		}
	}

	public GameCard TryGetNthChild(int n)
	{
		GameCard gameCard = this;
		for (int i = 0; i < n; i++)
		{
			if ((Object)(object)gameCard.Child != (Object)null)
			{
				gameCard = gameCard.Child;
				continue;
			}
			return null;
		}
		return gameCard;
	}

	protected override void Awake()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		base.Awake();
		((Component)this).transform.rotation = Quaternion.Euler(270f, 90f, 90f);
		propBlock = new MaterialPropertyBlock();
		combatCirclePropBlock = new MaterialPropertyBlock();
		((Component)this).GetComponentsInChildren<MaterialChanger>(true, materialChangers);
		MaterialChanger component = ((Component)this).GetComponent<MaterialChanger>();
		if ((Object)(object)component != (Object)null)
		{
			materialChangers.Add(component);
		}
		foreach (MaterialChanger materialChanger in materialChangers)
		{
			materialChanger.Init();
		}
		((Component)CombatStatusCircle).gameObject.SetActiveFast(active: false);
		((Renderer)DropShadowRenderer).enabled = false;
		newCircleStartSize = ((Component)NewCircle).transform.localScale;
		((Component)NewCircle).gameObject.SetActiveFast(active: true);
		((Component)NewCircle).transform.localScale = Vector3.zero;
		CombatCircleColor = CombatStatusCircle.color;
		((Component)StatusEffectBackground).transform.localScale = Vector3.zero;
		EmissionModule emission = FoilParticles.emission;
		((EmissionModule)(ref emission)).enabled = false;
		((Component)EquipmentRectangle).gameObject.SetActiveFast(active: true);
		((Component)WorkerRectangle).gameObject.SetActiveFast(active: true);
		((Component)SpecialText).gameObject.SetActiveFast(active: false);
		((Component)CoinText).gameObject.SetActiveFast(active: false);
		((Component)CoinIcon).gameObject.SetActiveFast(active: false);
		statusEffectBackgroundTransform = ((Component)StatusEffectBackground).transform;
	}

	protected override void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		startScale = ((Component)this).transform.localScale;
		if (IsDemoCard)
		{
			startScale *= 0.2f;
			((Component)this).transform.localScale = startScale;
		}
		UpdateIcon();
		lastPosition = (TargetPosition = ((Component)this).transform.position);
		UpdateCardPalette();
		SetColors();
		((Behaviour)HighlightRectangle).enabled = false;
		if (!WorldManager.instance.AllCards.Contains(this) && !IsDemoCard)
		{
			WorldManager.instance.AllCards.Add(this);
		}
		if (!WorldManager.instance.UniqueIdToCard.ContainsKey(CardData.UniqueId))
		{
			WorldManager.instance.UniqueIdToCard[CardData.UniqueId] = this;
		}
		onOffBasePosition = ((Component)OnOffInteractable).transform.localPosition;
		onOffTargetPosition = onOffBasePosition + new Vector3(0.09f, 0f, 0f);
		onOffTargetPos = onOffBasePosition;
		((Component)OnOffInteractable).gameObject.SetActive(false);
		if (!CardData.HasInventory)
		{
			Object.Destroy((Object)(object)HeadEquipmentPosition);
			Object.Destroy((Object)(object)HandEquipmentPosition);
			Object.Destroy((Object)(object)TorsoEquipmentPosition);
		}
	}

	public void UpdateIcon()
	{
		if (CardData.MyCardType == CardType.Ideas)
		{
			if (CardData.CardUpdateType == CardUpdateType.Main)
			{
				IconRenderer.sprite = SpriteManager.instance.IdeaIcon;
			}
			else if (CardData.CardUpdateType == CardUpdateType.Island)
			{
				IconRenderer.sprite = SpriteManager.instance.IslandIdeaIcon;
			}
			else if (CardData.CardUpdateType == CardUpdateType.Spirit)
			{
				IconRenderer.sprite = SpriteManager.instance.SpiritIdeaIcon;
			}
			else if (CardData.CardUpdateType == CardUpdateType.Cities)
			{
				IconRenderer.sprite = SpriteManager.instance.CitiesIdeaIcon;
			}
			else
			{
				IconRenderer.sprite = SpriteManager.instance.IdeaIcon;
			}
		}
		if ((Object)(object)CardData.Icon != (Object)null)
		{
			IconRenderer.sprite = CardData.Icon;
		}
	}

	public void UpdateCardPalette()
	{
		myCardPalette = ColorManager.instance.GetPaletteForCard(CardData);
	}

	public void ToggleDirection()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (CardData.OutputDir == Vector3.zero)
		{
			CardData.OutputDir = Vector3.right;
		}
		else if (CardData.OutputDir == Vector3.right)
		{
			CardData.OutputDir = Vector3.back;
		}
		else if (CardData.OutputDir == Vector3.back)
		{
			CardData.OutputDir = Vector3.left;
		}
		else if (CardData.OutputDir == Vector3.left)
		{
			CardData.OutputDir = Vector3.forward;
		}
		else if (CardData.OutputDir == Vector3.forward)
		{
			CardData.OutputDir = Vector3.zero;
		}
		QuestManager.instance.SpecialActionComplete("output_direction_changed", CardData);
	}

	private void SetColors()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		CombatStatusCircle.color = CombatCircleColor;
		CombatStatusCircle.color = Color.red;
		if (myCardPalette == null)
		{
			Debug.LogError((object)"Could not find card color pallet");
			return;
		}
		Color val = myCardPalette.Color;
		Color val2 = myCardPalette.Color2;
		Color val3 = myCardPalette.Icon;
		if (IsHit)
		{
			CombatStatusCircle.color = Color.white;
			val = (val2 = (val3 = Color.white));
		}
		CardRenderer.shadowCastingMode = (ShadowCastingMode)((!IsEquipped && !IsWorking) ? 1 : 0);
		CardRenderer.GetPropertyBlock(propBlock, 2);
		propBlock.SetColor(propColor, val);
		propBlock.SetColor(propColor2, val2);
		propBlock.SetColor(propIconColor, val3);
		Texture2D val4 = null;
		bool flag = false;
		if (CardData is ResourceChest || CardData is FoodWarehouse)
		{
			val4 = SpriteManager.instance.ChestIconSecondary.texture;
		}
		else if (CardData is ResourceMagnet)
		{
			val4 = SpriteManager.instance.MagnetIconSecondary.texture;
		}
		bool flag2 = (Object)(object)val4 != (Object)null;
		propBlock.SetFloat(propHasSecondaryIcon, flag2 ? 1f : 0f);
		propBlock.SetFloat(propHasOutputDir, flag ? 1f : 0f);
		if ((Object)(object)val4 != (Object)null)
		{
			propBlock.SetTexture(propSecondaryTex, (Texture)(object)val4);
		}
		float num = ((CardData is Equipable) ? 0.3f : 1f);
		propBlock.SetFloat(propBigShineStrength, (CardData is Equipable) ? 0f : 1f);
		propBlock.SetFloat(propShineStrength, num);
		propBlock.SetFloat(propFoil, (CardData.IsFoil || CardData.IsShiny || CardData is Equipable) ? 1f : 0f);
		propBlock.SetFloat(propDamaged, CardData.IsDamaged ? 1f : 0f);
		if ((Object)(object)IconRenderer.sprite != (Object)null)
		{
			propBlock.SetTexture(propIconTex, (Texture)(object)IconRenderer.sprite.texture);
		}
		else
		{
			propBlock.SetTexture(propIconTex, (Texture)(object)SpriteManager.instance.EmptyTexture.texture);
		}
		CardRenderer.SetPropertyBlock(propBlock, 2);
		if (((Graphic)SpecialText).color != val)
		{
			((Graphic)SpecialText).color = val;
		}
		SpecialIcon.color = val3;
		IconRenderer.color = val3;
		if (((Graphic)CoinText).color != val)
		{
			((Graphic)CoinText).color = val;
		}
		CoinIcon.color = val3;
		if (((ShapeRenderer)EquipmentButton).Color != val)
		{
			((ShapeRenderer)EquipmentButton).Color = val;
		}
		if (((ShapeRenderer)WorkerButton).Color != val)
		{
			((ShapeRenderer)WorkerButton).Color = val;
		}
		Color val5 = val3;
		val5.a = 0.5f;
		WorkerInventoryIcon.color = (HasAnyWorkers() ? val3 : val5);
		if (((Graphic)CardNameText).color != val3)
		{
			((Graphic)CardNameText).color = val3;
		}
	}

	private static Sprite GetSpriteForAttackType(AttackType type)
	{
		return (Sprite)(type switch
		{
			AttackType.Magic => SpriteManager.instance.MagicFightIcon, 
			AttackType.Melee => SpriteManager.instance.MeleeFightIcon, 
			AttackType.Ranged => SpriteManager.instance.RangedFightIcon, 
			AttackType.Foot => SpriteManager.instance.FootFightIcon, 
			AttackType.Armour => SpriteManager.instance.ArmourFightIcon, 
			AttackType.Air => SpriteManager.instance.AirFightIcon, 
			_ => null, 
		});
	}

	protected override void OnDestroy()
	{
		if ((Object)(object)WorldManager.instance != (Object)null)
		{
			WorldManager.instance.AllCards.Remove(this);
			if (WorldManager.instance.UniqueIdToCard.ContainsKey(CardData.UniqueId) && (Object)(object)WorldManager.instance.UniqueIdToCard[CardData.UniqueId] == (Object)(object)this)
			{
				WorldManager.instance.UniqueIdToCard.Remove(CardData.UniqueId);
			}
		}
		base.OnDestroy();
	}

	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(((Bounds)(ref debugBounds)).center, ((Bounds)(ref debugBounds)).size);
	}

	public virtual void DestroyCard(bool spawnSmoke = false, bool playSound = true)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		RemoveFromStack();
		WorldManager.instance.AllCards.Remove(this);
		WorldManager.instance.UniqueIdToCard.Remove(CardData.UniqueId);
		Destroyed = true;
		CardData.OnDestroyCard();
		if (playSound)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.CardDestroy, Random.Range(0.8f, 1.2f), 0.3f);
		}
		if (spawnSmoke)
		{
			WorldManager.instance.CreateSmoke(((Component)this).transform.position);
		}
		if (CardData is Curse item)
		{
			WorldManager.instance.ActiveCurses.Remove(item);
		}
		if (CardData.HasInventory)
		{
			foreach (GameCard equipmentChild in EquipmentChildren)
			{
				equipmentChild.EquipmentHolder = null;
				equipmentChild.IsEquipped = false;
				equipmentChild.DestroyCard(spawnSmoke: false, playSound: false);
			}
		}
		if (CardData.WorkerAmount > 0)
		{
			foreach (GameCard workerChild in WorkerChildren)
			{
				workerChild.WorkerHolder = null;
				workerChild.IsWorking = false;
				workerChild.DestroyCard(spawnSmoke: false, playSound: false);
			}
		}
		if ((Object)(object)Combatable != (Object)null && Combatable.InConflict)
		{
			Combatable.MyConflict.LeaveConflict(Combatable);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void SetChild(GameCard card)
	{
		cardsInvolved.Clear();
		cardsInvolved.Add(this);
		if ((Object)(object)card == (Object)(object)this)
		{
			Debug.LogError((object)"Child is same as Parent");
		}
		else if ((Object)(object)card == (Object)null)
		{
			if ((Object)(object)Child != (Object)null)
			{
				cardsInvolved.Add(Child);
				Child.Parent = null;
			}
			Child = null;
			NotifyStackUpdate(cardsInvolved);
		}
		else
		{
			Child = card;
			card.Parent = this;
			cardsInvolved.Add(card);
			NotifyStackUpdate(cardsInvolved);
		}
	}

	public void SetParent(GameCard card)
	{
		cardsInvolved.Clear();
		cardsInvolved.Add(this);
		if ((Object)(object)card == (Object)(object)this)
		{
			Debug.LogError((object)"Child is same as Parent");
		}
		else if ((Object)(object)card == (Object)null)
		{
			if ((Object)(object)Parent != (Object)null)
			{
				cardsInvolved.Add(Parent);
				Parent.Child = null;
			}
			Parent = null;
			NotifyStackUpdate(cardsInvolved);
		}
		else
		{
			Parent = card;
			card.Child = this;
			cardsInvolved.Add(card);
			NotifyStackUpdate(cardsInvolved);
		}
	}

	public void RemoveFromStack()
	{
		SetParent(null);
		SetChild(null);
	}

	private void NotifyStackUpdate(List<GameCard> cardsInvolved)
	{
		foreach (GameCard item in cardsInvolved)
		{
			item.GetRootCard().StackUpdate = true;
			item.StackUpdate = true;
		}
	}

	public void RemoveFromParent()
	{
		if ((Object)(object)Parent != (Object)null)
		{
			Parent.SetChild(null);
		}
		SetParent(null);
	}

	public override bool CanBePushed()
	{
		if (CardData is Food && WorldManager.instance.InEatingAnimation)
		{
			return false;
		}
		if (CardData is Spirit || CardData is CityAdvisor)
		{
			return false;
		}
		if (IsWorking || IsEquipped)
		{
			return false;
		}
		if (!BeingDragged)
		{
			return PushEnabled;
		}
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		if (IsEquipped || IsWorking)
		{
			return false;
		}
		if (draggable is Boosterpack && WorldManager.instance.CurrentBoard.Id == "cities" && GetRootCard().CardData.MyCardType == CardType.Structures)
		{
			return false;
		}
		if (draggable is GameCard gameCard)
		{
			if (gameCard.IsChildOf(this) || gameCard.IsParentOf(this))
			{
				return false;
			}
			if ((Object)(object)gameCard.BounceTarget != (Object)null)
			{
				return false;
			}
			if (gameCard.Destroyed)
			{
				return false;
			}
			if (!gameCard.PushEnabled)
			{
				return false;
			}
			if (gameCard.CardData is Food && WorldManager.instance.InEatingAnimation)
			{
				return false;
			}
			if (WorldManager.instance.CurrentBoard.Id == "cities" && GetRootCard().CardData.MyCardType == CardType.Structures && (gameCard.CardData is Resource || gameCard.CardData is Food))
			{
				return false;
			}
			if (gameCard.CardData is Spirit || gameCard.CardData is CityAdvisor)
			{
				return false;
			}
			if (gameCard.CardData is Energy)
			{
				return false;
			}
			if (gameCard.IsEquipped || gameCard.IsWorking)
			{
				return false;
			}
			if (!CardData.CanBePushedBy(gameCard.CardData))
			{
				return false;
			}
		}
		return base.CanBePushedBy(draggable);
	}

	public override bool CanBeDragged()
	{
		if (CardData is Combatable { BeingAttacked: not false })
		{
			return false;
		}
		if (WorldManager.instance.RemovingCards && GetRootCard().CardData is Boat { InSailOff: not false })
		{
			return false;
		}
		if (!BeingDragged && CardData.CanBeDragged)
		{
			return FaceUp;
		}
		return false;
	}

	public override void Clicked()
	{
		if (!FaceUp)
		{
			FaceUp = true;
		}
		if (DragTag == "inventory")
		{
			InventoryInteractable.Clicked();
			WorkerInventoryInteractable.Clicked();
		}
		else
		{
			CardData.Clicked();
		}
		WasClicked = true;
		base.Clicked();
	}

	public void ForceUpdate()
	{
		Update();
	}

	public void Equip(Equipable equipable)
	{
		GameCard myGameCard = equipable.MyGameCard;
		EquipmentChildren.Add(myGameCard);
		myGameCard.EquipmentHolder = this;
		myGameCard.IsEquipped = true;
		myGameCard.RemoveFromStack();
		CardData.OnEquipItem(equipable);
	}

	public void Unequip(Equipable equipable)
	{
		GameCard myGameCard = equipable.MyGameCard;
		EquipmentChildren.Remove(myGameCard);
		myGameCard.EquipmentHolder = null;
		myGameCard.IsEquipped = false;
		CardData.OnUnequipItem(equipable);
		if ((Object)(object)Combatable != (Object)null && Combatable.HealthPoints > Combatable.ProcessedCombatStats.MaxHealth)
		{
			Combatable.HealthPoints = Combatable.ProcessedCombatStats.MaxHealth;
		}
	}

	public void EquipWorker(Worker worker, int index)
	{
		GameCard myGameCard = worker.MyGameCard;
		worker.WorkerIndex = index;
		WorkerChildren.Add(myGameCard);
		myGameCard.WorkerHolder = this;
		myGameCard.IsWorking = true;
		myGameCard.RemoveFromStack();
		CardData.OnEquipItem(null);
	}

	public void UnequipWorker(GameCard worker)
	{
		WorkerChildren.Remove(worker);
		worker.CardData.WorkerIndex = -1;
		worker.WorkerHolder = null;
		worker.IsWorking = false;
		GetRootCard().StackUpdate = true;
		CardData?.OnUnequipItem(null);
	}

	protected override void Bounce()
	{
		if (HasParent)
		{
			BounceTarget = null;
		}
		if ((Object)(object)BounceTarget != (Object)null)
		{
			GameCard gameCard = BounceTarget;
			if ((Object)(object)gameCard.Child != (Object)null)
			{
				gameCard = gameCard.GetLeafCard();
			}
			BounceTarget = null;
			if ((Object)(object)gameCard == (Object)(object)this || (Object)(object)gameCard.BounceTarget != (Object)null || (Object)(object)gameCard.GetCardInCombatInStack() != (Object)null || gameCard.BeingDragged)
			{
				return;
			}
			GameCard cardWithStatusInStack = gameCard.GetCardWithStatusInStack();
			if ((Object)(object)cardWithStatusInStack != (Object)null && !cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus())
			{
				return;
			}
			if (gameCard.CardData.CanHaveCardOnTop(CardData))
			{
				SetParent(gameCard);
				Velocity = null;
				AudioManager.me.PlaySound2D(AudioManager.me.DropOnStack, Random.Range(0.8f, 1.2f), 0.3f);
			}
		}
		base.Bounce();
	}

	protected override void Update()
	{
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0937: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDemoCard && !MyBoard.IsCurrent)
		{
			return;
		}
		if (HasChild && CardData.IsDamaged)
		{
			if (CardData.DamageType == CardDamageType.Fire && Child.CardData.Id == "water")
			{
				Child.DestroyCard();
				CardData.SetCardUndamaged();
				WorldManager.instance.CreateSmoke(Position);
				AudioManager.me.PlaySound2D(AudioManager.me.ExtinguishCardSound, Random.Range(0.9f, 1.1f), 0.3f);
			}
			else if (CardData.DamageType == CardDamageType.Drought && CardData.ChildrenMatchingPredicate((CardData x) => x.Id == "water").Count >= 3)
			{
				CardData.DestroyChildrenMatchingPredicateAndRestack((CardData x) => x.Id == "water", 3);
				CardData.SetCardUndamaged();
				WorldManager.instance.CreateSmoke(Position);
				AudioManager.me.PlaySound2D(AudioManager.me.DroughtSolved, Random.Range(0.9f, 1.1f), 0.3f);
			}
			else if (CardData.DamageType == CardDamageType.Damaged && Child.CardData is ICurrency && CardData.GetDollarCountInStack(includeInChest: true) >= CardData.GetRepairCost())
			{
				List<ICurrency> currencyList = CardData.ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList();
				CitiesManager.instance.TryUseDollars(currencyList, CardData.GetRepairCost(), onlyTakeIfAmountMet: true);
				CardData.SetCardUndamaged();
				AudioManager.me.PlaySound2D(AudioManager.me.RepairCardSound, Random.Range(0.9f, 1.1f), 0.3f);
			}
		}
		CardData.UpdateCard();
		SetColors();
		string name = CardData.Name;
		if (((TMP_Text)CardNameText).text != name)
		{
			((TMP_Text)CardNameText).text = CardData.Name;
		}
		Vector3 val = (IsNew ? newCircleStartSize : Vector3.zero);
		((Component)NewCircle).transform.localScale = Vector3.Lerp(((Component)NewCircle).transform.localScale, val, Time.deltaTime * 20f);
		bool flag = (Object)(object)WorldManager.instance.DraggingCard != (Object)null && WorldManager.instance.DraggingCard.CardData.Id == CardData.Id;
		if (BeingDragged || WasClicked || (Object)(object)Child != (Object)null || (Object)(object)Parent != (Object)null || InConflict || (Object)(object)GetCardWithStatusInStack() != (Object)null || flag || CardData is Spirit || CardData is CityAdvisor)
		{
			IsNew = false;
		}
		if ((Object)(object)Child != (Object)null && !(Child.CardData is Equipable))
		{
			ShowInventory = false;
		}
		if (BeingDragged)
		{
			ShowInventory = false;
		}
		if ((Object)(object)Combatable != (Object)null && Combatable.InAttack)
		{
			ShowInventory = false;
		}
		EmissionModule emission = FoilParticles.emission;
		((EmissionModule)(ref emission)).enabled = !IsDemoCard && (CardData.IsFoil || CardData.Id == "goblet");
		PerformanceHelper.SetActive(((Component)CombatStatusCircle).gameObject, InConflict || ((Object)(object)Combatable != (Object)null && Combatable is Enemy && !IsDemoCard));
		if ((Object)(object)Combatable != (Object)null)
		{
			CombatStatusCircle.sprite = GetSpriteForAttackType(Combatable.ProcessedAttackType);
			((Renderer)CombatStatusCircle).GetPropertyBlock(combatCirclePropBlock);
			float num = (Combatable.InConflict ? Combatable.TimeToAttackNormalized : 1f);
			combatCirclePropBlock.SetFloat("_FillAmount", num);
			((Renderer)CombatStatusCircle).SetPropertyBlock(combatCirclePropBlock);
		}
		PerformanceHelper.SetActive(((Component)SpecialText).gameObject, SpecialValue.HasValue);
		if (SpecialValue.HasValue)
		{
			((TMP_Text)SpecialText).text = SpecialValue.Value.ToStringCached();
		}
		PerformanceHelper.SetActive(((Component)SpecialIcon).gameObject, SpecialValue.HasValue || ShowSpecialIcon);
		int value = CardData.GetValue();
		if (value != -1)
		{
			((TMP_Text)CoinText).text = value.ToStringCached();
			PerformanceHelper.SetActive(((Component)CoinIcon).gameObject, active: true);
			PerformanceHelper.SetActive(((Component)CoinText).gameObject, active: true);
		}
		else
		{
			PerformanceHelper.SetActive(((Component)CoinIcon).gameObject, active: false);
			PerformanceHelper.SetActive(((Component)CoinText).gameObject, active: false);
		}
		UpdateShowInventory();
		UpdateShowWorkerInventory();
		if (CardData.HasInventory)
		{
			HandInventoryIcon.color = (CardData.HasEquipableOfEquipableType(EquipableType.Weapon) ? colOn : colOff);
			TorsoInventoryIcon.color = (CardData.HasEquipableOfEquipableType(EquipableType.Torso) ? colOn : colOff);
			HeadInventoryIcon.color = (CardData.HasEquipableOfEquipableType(EquipableType.Head) ? colOn : colOff);
		}
		((Renderer)DropShadowRenderer).enabled = IsEquipped && EquipmentHolder.ShowInventory;
		((Renderer)DropShadowRenderer).enabled = IsWorking && WorkerHolder.ShowInventory;
		((Component)OnOffInteractable).gameObject.SetActiveFast(active: false);
		Vector3 val2 = startScale;
		if ((IsEquipped || IsWorking) && !BeingDragged)
		{
			val2 = startScale * 0.8f;
		}
		if (!IsDemoCard)
		{
			((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, val2, Time.deltaTime * 12f);
		}
		if (!IsDemoCard)
		{
			UpdatePosition();
		}
		Vector3 position = ((Component)this).transform.position;
		position.y = (0f - position.z) * 0.001f;
		EquipmentRectangle.position = position + equipmentRectangleStartOffset;
		WorkerRectangle.position = position + equipmentRectangleStartOffset;
		if (!IsDemoCard && !FaceUp)
		{
			flipTimer += Time.deltaTime * WorldManager.instance.PhysicsTimeScale;
			if (flipTimer >= 0.1f)
			{
				FaceUp = true;
			}
		}
		wobbleRotVelo -= Time.deltaTime * RotWobbleSpringiness;
		if (wobbleRotVelo <= 0f)
		{
			wobbleRotVelo = 0f;
		}
		float num2 = RotWobbleAmp * Mathf.Sin(wobbleRotVelo * RotWobbleSpeed) * wobbleRotVelo;
		if (AutoRotWobble)
		{
			rotWobbleTimer += Time.deltaTime;
			if (rotWobbleTimer > AutoRotWobbleTimer)
			{
				rotWobbleTimer -= AutoRotWobbleTimer;
				RotWobble(AutoRotWobbleAmount);
			}
		}
		bool active = true;
		if (!IsDemoCard)
		{
			if (IsEquipped)
			{
				if (EquipmentHolder.ShowInventory && !BeingDragged)
				{
					Transform myEquipmentStackPosition = GetMyEquipmentStackPosition();
					((Component)this).transform.localRotation = ((Component)Camera.main).transform.localRotation;
					((Component)this).transform.localEulerAngles = new Vector3(((Component)this).transform.localEulerAngles.x, ((Component)this).transform.localEulerAngles.y, myEquipmentStackPosition.localEulerAngles.z);
				}
				else if (!BeingDragged)
				{
					active = false;
				}
			}
			else if (IsWorking)
			{
				if (WorkerHolder.ShowInventory && !BeingDragged)
				{
					Transform transformAtIndex = WorkerHolder.WorkerTransformHolder.GetTransformAtIndex(CardData.WorkerIndex);
					((Component)this).transform.localRotation = ((Component)Camera.main).transform.localRotation;
					((Component)this).transform.localEulerAngles = new Vector3(((Component)this).transform.localEulerAngles.x, ((Component)this).transform.localEulerAngles.y, transformAtIndex.localEulerAngles.z);
				}
				else if (!BeingDragged)
				{
					active = false;
				}
			}
			else
			{
				float num3 = (FaceUp ? 90f : 270f);
				curZ = Mathf.Lerp(curZ, num3, Time.deltaTime * 14f * WorldManager.instance.PhysicsTimeScale);
				if ((Object)(object)Parent != (Object)null)
				{
					curZ = num3;
				}
				((Component)this).transform.localRotation = Quaternion.Euler(curZ, 0f + num2 + ZRotOffset, 0f);
			}
		}
		else
		{
			SetDemoCardRotation();
		}
		PerformanceHelper.SetActive(((Component)Visuals).gameObject, active);
		if ((Object)(object)Parent == (Object)null)
		{
			snappedToParent = false;
		}
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null && HighlightActive)
		{
			((ShapeRenderer)HighlightRectangle).Color = WorldManager.instance.CurrentBoard.CardHighlightColor;
		}
		((Behaviour)HighlightRectangle).enabled = HighlightActive;
		if (HighlightActive)
		{
			Rectangle highlightRectangle = HighlightRectangle;
			highlightRectangle.DashOffset += Time.deltaTime;
			if (HighlightRectangle.DashOffset >= 1f)
			{
				Rectangle highlightRectangle2 = HighlightRectangle;
				highlightRectangle2.DashOffset -= 1f;
			}
		}
		lastPosition = ((Component)this).transform.position;
		UpdateTimer();
		if ((Object)(object)removedChild != (Object)null && !removedChild.BeingDragged)
		{
			removedChild = null;
			StackUpdate = true;
		}
		UpdateStatusEffectElements();
		UpdateCardAnimations();
		if (CardData.IsDamaged)
		{
			if (CardData.DamageType == CardDamageType.Damaged)
			{
				CardData.AddStatusEffect(new StatusEffect_Damaged());
			}
			if (CardData.DamageType == CardDamageType.Fire)
			{
				CardData.AddStatusEffect(new StatusEffect_OnFire());
			}
			if (CardData.DamageType == CardDamageType.Drought)
			{
				CardData.AddStatusEffect(new StatusEffect_Drought());
			}
		}
		else
		{
			CardData.RemoveStatusEffect<StatusEffect_Damaged>();
			CardData.RemoveStatusEffect<StatusEffect_OnFire>();
			CardData.RemoveStatusEffect<StatusEffect_Drought>();
		}
		if (IsHovered && CardData.IsDamaged)
		{
			if (CardData.DamageType == CardDamageType.Damaged)
			{
				Tooltip.Text = "<b>" + SokLoc.Translate("label_damaged") + "</b>\n" + SokLoc.Translate("label_damaged_card_cost", (LocParam[])(object)new LocParam[2]
				{
					LocParam.Create("amount", CardData.GetRepairCost().ToStringCached()),
					LocParam.Create("icon", Icons.Dollar)
				});
			}
			if (CardData.DamageType == CardDamageType.Fire)
			{
				Tooltip.Text = "<b>" + SokLoc.Translate("label_on_fire") + "</b>\n" + SokLoc.Translate("label_fire_card_cost");
			}
		}
	}

	private bool HasAnyWorkers()
	{
		List<GameCard> workerChildren = CardData.MyGameCard.WorkerChildren;
		for (int i = 0; i < workerChildren.Count; i++)
		{
			if ((Object)(object)workerChildren[i] != (Object)null)
			{
				return true;
			}
		}
		return false;
	}

	private void animateOnOffInteractable()
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (!CardData.WorkerAmountMet())
		{
			flag = false;
		}
		if (WorldManager.instance.CurrentView != ViewType.Default)
		{
			flag = true;
		}
		if (CardData.CanToggleCardOnOff())
		{
			Vector3 localPosition;
			if (!OnOffInteractable.Velocity.HasValue)
			{
				float magnitude = ((Vector3)(ref onOffBasePosition)).magnitude;
				localPosition = ((Component)OnOffInteractable).transform.localPosition;
				if (magnitude - ((Vector3)(ref localPosition)).magnitude < 0.001f)
				{
					float magnitude2 = ((Vector3)(ref onOffBasePosition)).magnitude;
					localPosition = ((Component)OnOffInteractable).transform.localPosition;
					if (magnitude2 - ((Vector3)(ref localPosition)).magnitude > -0.001f)
					{
						if (flag)
						{
							((Component)OnOffInteractable).gameObject.SetActive(true);
							onOffTargetPos = onOffTargetPosition;
						}
						else
						{
							((Component)OnOffInteractable).gameObject.SetActive(false);
						}
						goto IL_0163;
					}
				}
			}
			if (!flag && !OnOffInteractable.Velocity.HasValue)
			{
				float magnitude3 = ((Vector3)(ref onOffTargetPosition)).magnitude;
				localPosition = ((Component)OnOffInteractable).transform.localPosition;
				if (magnitude3 - ((Vector3)(ref localPosition)).magnitude < 0.001f)
				{
					float magnitude4 = ((Vector3)(ref onOffTargetPosition)).magnitude;
					localPosition = ((Component)OnOffInteractable).transform.localPosition;
					if (magnitude4 - ((Vector3)(ref localPosition)).magnitude > -0.001f)
					{
						onOffTargetPos = onOffBasePosition;
					}
				}
			}
		}
		else
		{
			((Component)OnOffInteractable).gameObject.SetActive(false);
		}
		goto IL_0163;
		IL_0163:
		((Component)OnOffInteractable).transform.localPosition = FRILerp.Spring(((Component)OnOffInteractable).transform.localPosition, onOffTargetPos, 20f, 30f, ref onOffVelocity);
	}

	public void UpdateCardAnimations()
	{
		for (int i = 0; i < CardAnimations.Count; i++)
		{
			CardAnimation cardAnimation = CardAnimations[i];
			if (!cardAnimation.HasStarted)
			{
				cardAnimation.Start();
			}
			cardAnimation.Update();
			if (cardAnimation.IsDone)
			{
				CardAnimations.RemoveAt(i);
				i--;
			}
			else if (cardAnimation.IsBlocking)
			{
				break;
			}
		}
	}

	public void CreateCardConnectors()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		CardData.EnergyConnectors.OrderBy((CardConnectorData x) => x.EnergyConnectionStrength);
		Vector3 localPosition = default(Vector3);
		foreach (CardConnectorData energyConnector in CardData.EnergyConnectors)
		{
			int energyConnectionAmount = energyConnector.EnergyConnectionAmount;
			float num = ((energyConnector.EnergyConnectionType == CardDirection.input) ? (-0.19f) : 0.19f);
			for (int num2 = 0; num2 < energyConnectionAmount; num2++)
			{
				((Vector3)(ref localPosition))._002Ector(num, (float)num2 * ConnectorAmountOffset - (float)(energyConnectionAmount / 2) * ConnectorAmountOffset + ConnectorAmountOffset / 2f * ((energyConnectionAmount % 2 == 0) ? 1f : 0f) - CardTextOffset, -0.03f);
				GameObject obj = Object.Instantiate<GameObject>(EnergyConnectorPrefab, Vector3.zero, ((Component)this).transform.rotation, EnergyConnectorTransform);
				obj.transform.localPosition = localPosition;
				CardConnector component = obj.GetComponent<CardConnector>();
				component.InitializeEnergyNode(energyConnector, this);
				CardConnectorChildren.Add(component);
			}
		}
	}

	private void UpdateConnectors()
	{
		foreach (CardConnector cardConnectorChild in CardConnectorChildren)
		{
			if (WorldManager.instance.CurrentBoard.Id != "cities")
			{
				((Component)cardConnectorChild).gameObject.SetActive(false);
				break;
			}
			if (WorldManager.instance.CurrentView == ViewType.Default)
			{
				((Component)cardConnectorChild).gameObject.SetActive(true);
			}
			else if (WorldManager.instance.CurrentView == ViewType.Energy)
			{
				((Component)cardConnectorChild).gameObject.SetActive(cardConnectorChild.ConnectionType == ConnectionType.LV || cardConnectorChild.ConnectionType == ConnectionType.HV);
			}
			else if (WorldManager.instance.CurrentView == ViewType.Sewer)
			{
				((Component)cardConnectorChild).gameObject.SetActive(cardConnectorChild.ConnectionType == ConnectionType.Sewer);
			}
			else if (WorldManager.instance.CurrentView == ViewType.Transport)
			{
				((Component)cardConnectorChild).gameObject.SetActive(cardConnectorChild.ConnectionType == ConnectionType.Transport);
			}
		}
	}

	private void UpdateShowInventory()
	{
		bool flag = CardData.WorkerAmount > 0 && (Object)(object)Child == (Object)null && !CardData.HasInventory;
		bool flag2 = CardData.HasInventory && (Object)(object)Child == (Object)null && EquipmentChildren.Count > 0;
		PerformanceHelper.SetActive(((Component)EquipmentButton).gameObject, flag2);
		PerformanceHelper.SetActive(((Component)InventoryInteractable).gameObject, flag2);
		if (ShowInventory && !flag2 && !flag)
		{
			ShowInventory = false;
		}
	}

	private void UpdateShowWorkerInventory()
	{
		bool active = CardData.WorkerAmount > 0 && (Object)(object)Child == (Object)null && !CardData.HasInventory && !IsDemoCard;
		PerformanceHelper.SetActive(((Component)WorkerButton).gameObject, active);
		PerformanceHelper.SetActive(((Component)WorkerInventoryInteractable).gameObject, active);
	}

	private PositionType DeterminePositionType()
	{
		if (CardAnimations.Count > 0)
		{
			return PositionType.InAnimation;
		}
		if (IsEquipped)
		{
			if (BeingDragged)
			{
				return PositionType.None;
			}
			return PositionType.IsEquipped;
		}
		if (IsWorking)
		{
			if (BeingDragged)
			{
				return PositionType.None;
			}
			return PositionType.IsWorking;
		}
		if (InConflict)
		{
			if (BeingDragged)
			{
				return PositionType.None;
			}
			if (InAttack)
			{
				return PositionType.InAttack;
			}
			return PositionType.InConflict;
		}
		if ((Object)(object)Parent != (Object)null)
		{
			return PositionType.InStack;
		}
		if ((Object)(object)Parent == (Object)null)
		{
			return PositionType.IsRoot;
		}
		return PositionType.None;
	}

	private void UpdatePosition()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		switch (DeterminePositionType())
		{
		case PositionType.InConflict:
			TargetPosition = Combatable.MyConflict.GetPositionInConflict(Combatable);
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, TargetPosition, Time.deltaTime * 20f);
			break;
		case PositionType.InAttack:
		{
			AttackAnimation currentAttackAnimation = Combatable.CurrentAttackAnimation;
			((Component)this).transform.position = currentAttackAnimation.Position;
			TargetPosition = currentAttackAnimation.TargetPosition;
			break;
		}
		case PositionType.InAnimation:
		{
			CardAnimation cardAnimation = CardAnimations[0];
			((Component)this).transform.position = cardAnimation.Position;
			TargetPosition = cardAnimation.TargetPosition;
			break;
		}
		case PositionType.IsEquipped:
			if (EquipmentHolder.InventoryVisible)
			{
				TargetPosition = GetMyEquipmentStackPosition().position;
				if (IsHovered)
				{
					TargetPosition -= ((Component)this).transform.forward * 0.1f;
				}
				((Component)this).transform.position = TargetPosition;
			}
			else
			{
				TargetPosition = ((Component)EquipmentHolder).transform.position + new Vector3(0f, -0.1f, 0f);
				((Component)this).transform.position = TargetPosition;
			}
			break;
		case PositionType.IsWorking:
			if (WorkerHolder.InventoryVisible)
			{
				TargetPosition = WorkerHolder.WorkerTransformHolder.GetTransformAtIndex(CardData.WorkerIndex).position;
				if (IsHovered)
				{
					TargetPosition -= ((Component)this).transform.forward * 0.1f;
				}
				((Component)this).transform.position = TargetPosition;
			}
			else
			{
				TargetPosition = ((Component)WorkerHolder).transform.position + new Vector3(0f, -0.1f, 0f);
				((Component)this).transform.position = TargetPosition;
			}
			break;
		case PositionType.InStack:
			SetToParentPosition();
			TargetPosition = ((Component)this).transform.position;
			break;
		case PositionType.IsRoot:
		case PositionType.None:
			if (!Velocity.HasValue)
			{
				Vector3 targetPosition = TargetPosition;
				float num = 20f;
				if (SetY)
				{
					targetPosition.y = (0f - targetPosition.z) * 0.001f;
					targetPosition.y += (BeingDragged ? 0.1f : 0f);
					if (IsHovered && CanBeDragged() && WorldManager.instance.CanInteract)
					{
						targetPosition.y += 0.06f;
					}
					if (CardData is Spirit || CardData is CityAdvisor)
					{
						targetPosition.y += 0.25f;
					}
				}
				else
				{
					num = 10f + WorldManager.instance.EndOfMonthSpeedup * 3f;
				}
				((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, targetPosition, Time.deltaTime * num);
			}
			UpdateChildPositions();
			break;
		}
		if (closeToTargetPositionCallback != null && Vector3.Distance(((Component)this).transform.position, TargetPosition) < 0.1f)
		{
			closeToTargetPositionCallback();
		}
	}

	public Transform GetEquipmentStackPosition(EquipableType equipableType)
	{
		return (Transform)(equipableType switch
		{
			EquipableType.Head => HeadEquipmentPosition.transform, 
			EquipableType.Torso => TorsoEquipmentPosition.transform, 
			EquipableType.Weapon => HandEquipmentPosition.transform, 
			_ => throw new ArgumentException($"EquipableType does not have a stack position set for {equipableType}"), 
		});
	}

	public void ToggleInventory()
	{
		OpenInventory(!ShowInventory);
	}

	public void ToggleCardOnOff()
	{
		CardData.ToggleCardOnOff();
	}

	public void OpenInventory(bool showInventory)
	{
		if (showInventory == ShowInventory)
		{
			return;
		}
		ShowInventory = showInventory;
		if (!ShowInventory)
		{
			return;
		}
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if ((Object)(object)allCard != (Object)(object)this && allCard.ShowInventory)
			{
				allCard.ShowInventory = false;
			}
		}
	}

	public void StatusEffectsChanged()
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		foreach (StatusEffect statusEffect in CardData.StatusEffects)
		{
			if (!ElementExistsForStatusEffect(statusEffect))
			{
				StatusEffectElement item = CreateElementForStatusEffect(statusEffect);
				StatusEffectElements.Add(item);
			}
		}
		for (int i = 0; i < StatusEffectElements.Count; i++)
		{
			if (!CardData.StatusEffects.Contains(StatusEffectElements[i].MyStatusEffect))
			{
				StatusEffectElements[i].DestroyMe = true;
			}
		}
		List<StatusEffectElement> list = StatusEffectElements.Where((StatusEffectElement x) => !x.DestroyMe).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			float num2 = (float)num * DistanceBetweenStatusses - (float)(CardData.StatusEffects.Count - 1) * DistanceBetweenStatusses * 0.5f;
			list[num].TargetLocalPosition = new Vector3(num2, 0f, -0.001f);
		}
	}

	private void UpdateStatusEffectElements()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((StatusEffectElements.Count == 0) ? Vector3.zero : Vector3.one);
		statusEffectBackgroundTransform.localScale = Vector3.Lerp(statusEffectBackgroundTransform.localScale, val, Time.deltaTime * 12f);
		GameObject gameObject = ((Component)StatusEffectBackground).gameObject;
		Vector3 localScale = statusEffectBackgroundTransform.localScale;
		PerformanceHelper.SetActive(gameObject, ((Vector3)(ref localScale)).sqrMagnitude > 0.001f);
		float num = 0.1125f + (float)(StatusEffectElements.Count - 1) * DistanceBetweenStatusses;
		statusEffectBackgroundWidth = Mathf.Lerp(statusEffectBackgroundWidth, num, Time.deltaTime * 12f);
		if (Mathf.Abs(statusEffectBackgroundWidth - StatusEffectBackground.Width) > 0.01f)
		{
			StatusEffectBackground.Width = statusEffectBackgroundWidth;
		}
	}

	public void SetDemoCardRotation()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (FaceUp)
		{
			((Component)this).transform.rotation = ((Component)Camera.main).transform.rotation;
		}
		else
		{
			((Component)this).transform.rotation = Quaternion.LookRotation(-((Component)Camera.main).transform.forward, ((Component)Camera.main).transform.up);
		}
	}

	private Transform GetMyEquipmentStackPosition()
	{
		if (!IsEquipped)
		{
			throw new Exception("Not equipped!");
		}
		return EquipmentHolder.GetEquipmentStackPosition(((Equipable)CardData).EquipableType);
	}

	protected override void LateUpdate()
	{
		if (!((Object)(object)MyBoard != (Object)null) || MyBoard.IsCurrent)
		{
			PushAwayFromOthers();
			if ((Object)(object)Parent == (Object)null && !IsEquipped && !IsWorking)
			{
				ClampPos();
			}
			if ((Object)(object)Parent != (Object)null)
			{
				LastParent = Parent;
			}
		}
	}

	public void SetFaceUp(bool faceUp)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		FaceUp = faceUp;
		curZ = (FaceUp ? 90f : 270f);
		((Component)this).transform.localRotation = Quaternion.Euler(curZ, 0f, 0f);
	}

	public override void SendIt()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (MyBoard.Id == "cities" && HasParent)
		{
			Velocity = GetRootCard().CardData.OutputDir * 7f;
		}
		else
		{
			base.SendIt();
		}
		RotWobble(1f);
	}

	public GameCard FindNextGameCardInDirection(Vector3 direction, CardType? type = null)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MinValue;
		GameCard result = null;
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (!((Component)allCard).gameObject.activeInHierarchy || (Object)(object)allCard == (Object)(object)WorldManager.instance.DraggingDraggable)
			{
				continue;
			}
			if ((Object)(object)allCard.MyBoard == (Object)null)
			{
				Debug.Log((object)(((object)allCard)?.ToString() + " does not have a board"));
			}
			else
			{
				if (!allCard.MyBoard.IsCurrent || !allCard.CanBeAutoMovedTo || (type.HasValue && allCard.CardData.MyCardType != type))
				{
					continue;
				}
				Vector3 val = allCard.AutoMoveSnapPosition - ((Component)this).transform.position;
				float num2 = Vector3.Dot(direction, val);
				if (!((double)num2 <= 0.3))
				{
					float num3 = num2 / ((Vector3)(ref val)).sqrMagnitude;
					if (num3 > num && ((Vector3)(ref val)).sqrMagnitude < 1f)
					{
						num = num3;
						result = allCard;
					}
				}
			}
		}
		return result;
	}

	public override void SendDirection(Vector3 direction)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		RotWobble(1f);
		base.SendDirection(direction);
	}

	public override void SendToPosition(Vector3 position)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		RotWobble(1f);
		base.SendToPosition(position);
	}

	public void SendToPositionCallback(Vector3 position, Action callback)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		RotWobble(1f);
		TargetPosition = position;
		closeToTargetPositionCallback = callback;
	}

	public void RotWobble(float amount)
	{
		wobbleRotVelo = amount;
	}

	private void SetToParentPosition(bool hardSetPos = false)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((!IsCollapsed) ? (((Component)Parent).transform.position + new Vector3(0f, WorldManager.instance.CardOverlayHeightOffset, 0f - WorldManager.instance.CardOverlayOffset)) : (((Component)Parent).transform.position + new Vector3(0f, WorldManager.instance.CardOverlayHeightOffset, 0f - WorldManager.instance.CollapsedCardOverlayOffset)));
		if (!snappedToParent)
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, val, Time.deltaTime * 20f);
			if (Vector3.Distance(((Component)this).transform.position, val) < 0.001f)
			{
				snappedToParent = true;
			}
		}
		else
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, val, Time.deltaTime * 20f);
			Vector3 position = ((Component)this).transform.position;
			position.y = val.y;
			((Component)this).transform.position = position;
		}
		if (hardSetPos)
		{
			((Component)this).transform.position = (TargetPosition = val);
		}
	}

	public void UpdateChildPositions(bool hardSetPos = false)
	{
		if (!((Object)(object)Child == (Object)null))
		{
			Child.SetToParentPosition(hardSetPos);
			Child.UpdateChildPositions(hardSetPos);
		}
	}

	public Conflict GetOverlappingConflict()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (Conflict allConflict in WorldManager.instance.GetAllConflicts())
		{
			Bounds bounds = allConflict.GetBounds();
			if (((Bounds)(ref bounds)).Intersects(base.DraggableBounds))
			{
				return allConflict;
			}
		}
		return null;
	}

	public List<GameCard> GetOverlappingCardsInBox(Vector3 center, Vector3 size)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		List<GameCard> list = new List<GameCard>();
		int num = Physics.OverlapBoxNonAlloc(center, size * 0.5f, hits, Quaternion.identity, -5, (QueryTriggerInteraction)1);
		for (int i = 0; i < num; i++)
		{
			GameCard component = ((Component)hits[i]).gameObject.GetComponent<GameCard>();
			if ((Object)(object)component != (Object)null && (Object)(object)component != (Object)(object)this)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public List<GameCard> GetOverlappingCards()
	{
		List<GameCard> list = new List<GameCard>();
		int num = PhysicsExtensions.OverlapBoxNonAlloc(boxCollider, hits, -5, (QueryTriggerInteraction)1);
		for (int i = 0; i < num; i++)
		{
			GameCard component = ((Component)hits[i]).gameObject.GetComponent<GameCard>();
			if ((Object)(object)component != (Object)null && (Object)(object)component != (Object)(object)this)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public void StartBlueprintTimer(float time, TimerAction a, string status, string actionId, string blueprintId, int subprintIndex, CardData consumer, bool skipWorkerEnergyCheck = false)
	{
		if (IsDemoCard || BeingDragged)
		{
			return;
		}
		GameCard gameCard = GetRootCard();
		if (gameCard.CardData is HeavyFoundation && gameCard.HasChild)
		{
			gameCard = gameCard.Child;
		}
		if ((!HasTransportCard() || !(actionId != "sail_off") || !(actionId != "leave_spirit") || !(actionId != "take_portal")) && (!((Object)(object)removedChild != (Object)null) || !removedChild.BeingDragged))
		{
			if (TimerActionId == actionId && TimerBlueprintId == blueprintId && TimerSubprintIndex == subprintIndex)
			{
				TargetTimerTime = time;
			}
			else if (CardData.IsOn && (skipWorkerEnergyCheck || gameCard.CardData.ShouldStartTimerWorkers(actionId)) && (skipWorkerEnergyCheck || gameCard.CardData.ShouldStartTimerEnergy(consumer, actionId)) && !gameCard.CardData.IsDamaged)
			{
				TimerBlueprintId = blueprintId;
				TimerSubprintIndex = subprintIndex;
				SkipCitiesChecks = skipWorkerEnergyCheck;
				InitTimer(time, a, status, actionId);
			}
		}
	}

	public void StartTimer(float time, TimerAction a, string status, string actionId, bool withStatusBar = true, bool skipWorkerEnergyCheck = false, bool skipDamageOnOffCheck = false)
	{
		if (!IsDemoCard && !BeingDragged)
		{
			if (TimerActionId == actionId)
			{
				TargetTimerTime = time;
			}
			else if ((CardData.IsOn || skipDamageOnOffCheck) && (skipWorkerEnergyCheck || CardData.ShouldStartTimerWorkers(actionId)) && (skipWorkerEnergyCheck || CardData.HasEnergyInput()) && (skipWorkerEnergyCheck || CardData.HasSewerConnected()) && (!CardData.IsDamaged || skipDamageOnOffCheck))
			{
				InitTimer(time, a, status, actionId, withStatusBar);
			}
		}
	}

	private void InitTimer(float time, TimerAction a, string status, string actionId, bool withStatusBar = true)
	{
		if (withStatusBar)
		{
			Statusbar statusbar = Object.Instantiate<Statusbar>(PrefabManager.instance.StatusBarPrefab);
			statusbar.StatusTime = time;
			statusbar.ParentCard = this;
			CurrentStatusbar = statusbar;
		}
		Status = status;
		TimerRunning = true;
		TimerAction = a;
		TimerActionId = actionId;
		CurrentTimerTime = 0f;
		TargetTimerTime = time;
	}

	public void CancelTimer(string actionId)
	{
		if ((!((Object)(object)removedChild != (Object)null) || !removedChild.BeingDragged) && TimerRunning && !(TimerActionId != actionId))
		{
			StopTimer();
		}
	}

	private void StopTimer()
	{
		TimerRunning = false;
		TimerActionId = "";
		Status = "";
		TimerBlueprintId = "";
		TimerSubprintIndex = 0;
		CurrentTimerTime = 0f;
		SkipCitiesChecks = false;
		if ((Object)(object)CurrentStatusbar != (Object)null)
		{
			CurrentStatusbar.DestroyMe = true;
			CurrentStatusbar = null;
		}
	}

	public void CancelAnyTimer()
	{
		if (TimerRunning)
		{
			StopTimer();
		}
	}

	public void UpdateTimer()
	{
		if (!TimerRunning)
		{
			return;
		}
		if ((Object)(object)removedChild == (Object)null || !removedChild.BeingDragged)
		{
			CurrentTimerTime += Time.deltaTime * WorldManager.instance.TimeScale;
		}
		if ((Object)(object)CurrentStatusbar != (Object)null)
		{
			CurrentStatusbar.Paused = (Object)(object)removedChild != (Object)null && removedChild.BeingDragged;
		}
		if (!(CurrentTimerTime >= TargetTimerTime))
		{
			return;
		}
		TimerRunning = false;
		if (!ShouldCompleteTimer(TimerActionId))
		{
			TimerActionId = "";
			Status = "";
			TimerBlueprintId = "";
			TimerSubprintIndex = 0;
			CurrentTimerTime = 0f;
			CurrentStatusbar.DestroyMe = true;
			CurrentStatusbar = null;
			return;
		}
		try
		{
			TimerAction();
		}
		catch (Exception ex)
		{
			Debug.LogError((object)ex);
		}
		if (TimerActionId == "finish_blueprint")
		{
			QuestManager.instance.ActionComplete(WorldManager.instance.GetBlueprintWithId(TimerBlueprintId), TimerActionId, CardData);
		}
		else
		{
			QuestManager.instance.ActionComplete(CardData, TimerActionId);
		}
		TimerActionId = "";
		Status = "";
		TimerBlueprintId = "";
		TimerSubprintIndex = 0;
		CurrentTimerTime = 0f;
		if ((Object)(object)CurrentStatusbar != (Object)null)
		{
			CurrentStatusbar.DestroyMe = true;
		}
		CurrentStatusbar = null;
	}

	public virtual bool ShouldCompleteTimer(string timerActionId)
	{
		return CardData.ShouldCompleteTimer(timerActionId);
	}

	public bool HasTransportCard()
	{
		GameCard gameCard = GetRootCard();
		if (gameCard.CardData is HeavyFoundation && gameCard.HasChild)
		{
			gameCard = gameCard.Child;
		}
		if (gameCard.CardData is Boat || gameCard.CardData is Spirit || gameCard.CardData is Portal)
		{
			return true;
		}
		return false;
	}

	public bool ElementExistsForStatusEffect(StatusEffect effect)
	{
		foreach (StatusEffectElement statusEffectElement in StatusEffectElements)
		{
			if (statusEffectElement.MyStatusEffect == effect)
			{
				return true;
			}
		}
		return false;
	}

	public StatusEffectElement CreateElementForStatusEffect(StatusEffect effect)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectElement statusEffectElement = Object.Instantiate<StatusEffectElement>(PrefabManager.instance.StatusEffectElementPrefab);
		statusEffectElement.SetStatusEffect(this, effect);
		((Component)statusEffectElement).transform.SetParent(StatusEffectElementParent);
		((Component)statusEffectElement).transform.localRotation = Quaternion.identity;
		((Component)statusEffectElement).transform.localScale = Vector3.zero;
		float num = (float)StatusEffectElements.Count * DistanceBetweenStatusses - (float)(StatusEffectElements.Count - 1) * DistanceBetweenStatusses * 0.5f;
		((Component)statusEffectElement).transform.localPosition = new Vector3(num, 0f, -0.001f);
		return statusEffectElement;
	}

	public bool IsPartOfStack()
	{
		if (!((Object)(object)Parent != (Object)null))
		{
			return (Object)(object)Child != (Object)null;
		}
		return true;
	}

	public GameCard GetCardWithStatusInStack()
	{
		GameCard gameCard = GetRootCard();
		while ((Object)(object)gameCard != (Object)null)
		{
			if (gameCard.TimerRunning)
			{
				return gameCard;
			}
			gameCard = gameCard.Child;
		}
		return null;
	}

	public int GetCardIndex()
	{
		GameCard gameCard = GetRootCard();
		int num = 0;
		while ((Object)(object)gameCard != (Object)null)
		{
			if ((Object)(object)gameCard == (Object)(object)this)
			{
				return num;
			}
			gameCard = gameCard.Child;
			num++;
		}
		return -1;
	}

	public GameCard GetCardInCombatInStack()
	{
		GameCard gameCard = GetRootCard();
		while ((Object)(object)gameCard != (Object)null)
		{
			if ((Object)(object)gameCard.Combatable != (Object)null && gameCard.Combatable.InConflict)
			{
				return gameCard;
			}
			gameCard = gameCard.Child;
		}
		return null;
	}

	public List<GameCard> GetAllCardsInStack()
	{
		GameCard rootCard = GetRootCard();
		List<GameCard> childCards = rootCard.GetChildCards();
		childCards.Insert(0, rootCard);
		return childCards;
	}

	public CardData HasCardInStack(Predicate<CardData> pred)
	{
		GameCard gameCard = GetRootCard();
		while ((Object)(object)gameCard != (Object)null)
		{
			if (pred(gameCard.CardData))
			{
				return gameCard.CardData;
			}
			gameCard = gameCard.Child;
		}
		return null;
	}

	public bool IsPartOfSameStack(GameCard otherCard)
	{
		GameCard gameCard = GetRootCard();
		while ((Object)(object)gameCard != (Object)null)
		{
			if ((Object)(object)gameCard == (Object)(object)otherCard)
			{
				return true;
			}
			gameCard = gameCard.Child;
		}
		return false;
	}

	public string GetStackSummary()
	{
		return WorldManager.instance.GetStackSummary(GetAllCardsInStack());
	}

	public bool IsChildOf(GameCard card)
	{
		if ((Object)(object)card == (Object)null)
		{
			return false;
		}
		for (GameCard parent = Parent; parent != null; parent = parent.Parent)
		{
			if ((Object)(object)parent == (Object)(object)card)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsParentOf(GameCard card)
	{
		if ((Object)(object)card == (Object)null)
		{
			return false;
		}
		for (GameCard child = Child; child != null; child = child.Child)
		{
			if ((Object)(object)child == (Object)(object)card)
			{
				return true;
			}
		}
		return false;
	}

	public void SetCollidersInStack(bool enabled)
	{
		for (GameCard gameCard = this; gameCard != null; gameCard = gameCard.Child)
		{
			((Collider)gameCard.boxCollider).enabled = enabled;
		}
	}

	public List<GameCard> GetChildCards()
	{
		List<GameCard> list = new List<GameCard>();
		GameCard child = Child;
		while ((Object)(object)child != (Object)null)
		{
			list.Add(child);
			child = child.Child;
		}
		return list;
	}

	public GameCard GetRootCard()
	{
		GameCard gameCard = this;
		while ((Object)(object)gameCard.Parent != (Object)null)
		{
			gameCard = gameCard.Parent;
		}
		return gameCard;
	}

	public GameCard GetLeafCard()
	{
		GameCard gameCard = this;
		while ((Object)(object)gameCard.Child != (Object)null)
		{
			gameCard = gameCard.Child;
		}
		return gameCard;
	}

	public int GetChildCount()
	{
		GameCard gameCard = this;
		int num = 0;
		while ((Object)(object)gameCard.Child != (Object)null)
		{
			num++;
			gameCard = gameCard.Child;
		}
		return num;
	}

	public int GetStackCount()
	{
		GameCard gameCard = GetRootCard();
		int num = 1;
		while ((Object)(object)gameCard.Child != (Object)null)
		{
			num++;
			gameCard = gameCard.Child;
		}
		return num;
	}

	private void NotifyChildDrag(GameCard card)
	{
		removedChild = card;
	}

	public override void StopDragging()
	{
		if ((Object)(object)Parent != (Object)null)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.DropOnStack, Random.Range(0.8f, 1.2f), 0.3f);
		}
		else if ((Object)(object)CardData.PickupSound != (Object)null && CardData.PickupSoundGroup == PickupSoundGroup.Custom)
		{
			AudioManager.me.PlaySound2D(CardData.PickupSound, Random.Range(0.8f, 1f), 0.5f);
		}
		else
		{
			List<AudioClip> soundForPickupSoundGroup = AudioManager.me.GetSoundForPickupSoundGroup(CardData.PickupSoundGroup);
			AudioManager.me.PlaySound2D(soundForPickupSoundGroup, Random.Range(0.8f, 1f), 0.5f);
		}
		GameCard child = Child;
		while ((Object)(object)child != (Object)null)
		{
			child.BeingDragged = false;
			child = child.Child;
		}
		CardData.StoppedDragging();
		StackUpdate = true;
		base.StopDragging();
	}

	public override void StartDragging()
	{
		if ((Object)(object)CardData.PickupSound != (Object)null && CardData.PickupSoundGroup == PickupSoundGroup.Custom)
		{
			AudioManager.me.PlaySound2D(CardData.PickupSound, Random.Range(1f, 1.2f), 0.5f);
		}
		else
		{
			List<AudioClip> soundForPickupSoundGroup = AudioManager.me.GetSoundForPickupSoundGroup(CardData.PickupSoundGroup);
			AudioManager.me.PlaySound2D(soundForPickupSoundGroup, Random.Range(1f, 1.2f), 0.5f);
		}
		GameCard parent = Parent;
		while ((Object)(object)parent != (Object)null)
		{
			parent.NotifyChildDrag(this);
			parent = parent.Parent;
		}
		if ((Object)(object)Parent != (Object)null)
		{
			SetParent(null);
		}
		parent = Child;
		while ((Object)(object)parent != (Object)null)
		{
			parent.BeingDragged = true;
			parent = parent.Child;
		}
		BounceTarget = null;
		base.StartDragging();
	}

	public void Clampieee()
	{
		ClampPos();
	}

	protected override void ClampPos()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDemoCard && SetY)
		{
			int childCount = GetChildCount();
			float num = (float)childCount * WorldManager.instance.CardOverlayOffset;
			if (IsCollapsed)
			{
				num = (float)childCount * WorldManager.instance.CollapsedCardOverlayOffset;
			}
			curHeight = Mathf.Lerp(curHeight, num, Time.deltaTime * 12f);
			((Component)this).transform.position = ClampPos2(((Component)this).transform.position);
			TargetPosition = ClampPos2(TargetPosition);
		}
	}

	public float GetHeight()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		PrefabManager.instance.GameCardPrefab.boxCollider.ToWorldSpaceBox(out var _, out var halfExtents, out var _);
		return halfExtents.y * 2f;
	}

	public float GetWidth()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		PrefabManager.instance.GameCardPrefab.boxCollider.ToWorldSpaceBox(out var _, out var halfExtents, out var _);
		return halfExtents.x * 2f;
	}

	public Bounds GetBounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(((Component)this).transform.position, new Vector3(GetWidth(), 0.01f, GetHeight()));
	}

	private Vector3 ClampPos2(Vector3 p)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = (BeingDragged ? MyBoard.WorldBounds : MyBoard.TightWorldBounds);
		boxCollider.ToWorldSpaceBox2(out var halfExtents);
		float num = 0.1f;
		p.x = Mathf.Clamp(p.x, ((Bounds)(ref val)).min.x + halfExtents.x + num, ((Bounds)(ref val)).max.x - halfExtents.x - num);
		p.z = Mathf.Clamp(p.z, ((Bounds)(ref val)).min.z + halfExtents.y + num + curHeight, ((Bounds)(ref val)).max.z - halfExtents.y - num);
		return p;
	}

	public SavedCard ToSavedCard()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SavedCard savedCard = new SavedCard();
		savedCard.CardPosition = ((Component)this).transform.position;
		savedCard.CardPrefabId = CardData.Id;
		savedCard.UniqueId = CardData.UniqueId;
		savedCard.IsFoil = CardData.IsFoil;
		savedCard.FaceUp = FaceUp;
		savedCard.IsDamaged = CardData.IsDamaged;
		savedCard.DamageType = CardData.DamageType;
		if ((Object)(object)Parent != (Object)null)
		{
			savedCard.ParentUniqueId = Parent.CardData.UniqueId;
		}
		if ((Object)(object)EquipmentHolder != (Object)null)
		{
			savedCard.EquipmentHolderUniqueId = EquipmentHolder.CardData.UniqueId;
		}
		if ((Object)(object)WorkerHolder != (Object)null)
		{
			savedCard.WorkerHolderUniqueId = WorkerHolder.CardData.UniqueId;
			savedCard.WorkerIndex = CardData.WorkerIndex;
		}
		savedCard.ExtraCardData = CardData.GetExtraCardData();
		savedCard.TimerRunning = TimerRunning;
		savedCard.WithStatusBar = (Object)(object)CurrentStatusbar != (Object)null;
		savedCard.TimerActionId = TimerActionId;
		savedCard.Status = Status;
		savedCard.CurrentTimerTime = CurrentTimerTime;
		savedCard.TargetTimerTime = TargetTimerTime;
		savedCard.TimerBlueprintId = TimerBlueprintId;
		savedCard.SkipCitiesChecks = SkipCitiesChecks;
		savedCard.SubprintIndex = TimerSubprintIndex;
		savedCard.BoardId = MyBoard.Id;
		savedCard.StatusEffects = CardData.StatusEffects.Select((StatusEffect x) => x.ToSavedStatusEffect()).ToList();
		savedCard.CardConnectors = (from x in CardConnectorChildren
			select x.ToSavedEnergyConnector() into x
			where x != null
			select x).ToList();
		return savedCard;
	}

	public void SetHitEffect(Action after = null)
	{
		IsHit = true;
		foreach (MaterialChanger mc in materialChangers)
		{
			if ((Object)(object)mc != (Object)null)
			{
				mc.SetMaterial(WorldManager.instance.HitMaterial);
				((MonoBehaviour)this).StartCoroutine(WaitFor(0.1f, delegate
				{
					mc.ResetMaterials();
				}));
			}
		}
		((MonoBehaviour)this).StartCoroutine(WaitFor(0.11f, delegate
		{
			IsHit = false;
			after?.Invoke();
		}));
	}

	public bool HasConnectorOfType(ConnectionType connectionType)
	{
		for (int i = 0; i < CardData.EnergyConnectors.Count; i++)
		{
			if (CardData.EnergyConnectors[i].EnergyConnectionStrength == connectionType)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator WaitFor(float time, Action a)
	{
		yield return (object)new WaitForSeconds(time);
		a?.Invoke();
	}
}
