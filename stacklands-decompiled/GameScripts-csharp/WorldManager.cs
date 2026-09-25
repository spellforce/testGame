using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
	public enum GameState
	{
		Playing,
		Paused,
		GameOver,
		InMenu
	}

	public static WorldManager instance;

	public float CardOverlayOffset = 0.1f;

	public float CollapsedCardOverlayOffset = 0.02f;

	public float CombatOffset = 1f;

	public float HorizonalCombatOffset = 0.2f;

	public float CombatMissOffset = 0.2f;

	public float CardOverlayHeightOffset = 0.001f;

	public float ConflictWidthIncrease;

	public float ConflictHeightIncrease;

	public float ConflictArrowLengthDecrease = 0.2f;

	private List<ParticleSystem> smokeBuffer = new List<ParticleSystem>();

	private List<ParticleSystem> energyMinusBuffer = new List<ParticleSystem>();

	private List<ParticleSystem> wellbeingPlusBuffer = new List<ParticleSystem>();

	private List<FloatingStatus> floatingTextBuffer = new List<FloatingStatus>();

	public AnimationCurve CombatYPosition;

	public AnimationCurve CombatFlatPositionCurve;

	public AnimationCurve CombatKnockbackCurve;

	public float CombatSpeed = 5f;

	public List<Draggable> AllDraggables = new List<Draggable>();

	public List<Draggable> PhysicsDraggables = new List<Draggable>();

	public GetComponentCacher<Draggable> DraggableLookup = new GetComponentCacher<Draggable>();

	public GetComponentCacher<Interactable> InteractableLookup = new GetComponentCacher<Interactable>();

	public GetComponentCacher<Hoverable> HoverableLookup = new GetComponentCacher<Hoverable>();

	public Draggable HoveredDraggable;

	public Draggable DraggingDraggable;

	public Interactable HoveredInteractable;

	public Hoverable CurrentHoverable;

	[HideInInspector]
	public List<GameCard> AllCards = new List<GameCard>();

	[HideInInspector]
	public Dictionary<string, GameCard> UniqueIdToCard = new Dictionary<string, GameCard>();

	[HideInInspector]
	public List<CardTarget> CardTargets = new List<CardTarget>();

	public List<Boosterpack> AllBoosters = new List<Boosterpack>();

	public List<string> BoughtBoosterIds = new List<string>();

	public List<BuyBoosterBox> AllBoosterBoxes = new List<BuyBoosterBox>();

	public Material HitMaterial;

	public ViewType CurrentView = ViewType.Default;

	public List<GameBoard> Boards;

	public float MonthTimer;

	public float AnimationTime;

	public BoardMonths BoardMonths;

	public int OldCurrentMonth;

	public bool DebugScreenOpened;

	public float CardTargetSnapDistance = 0.2f;

	public GameState CurrentGameState;

	public bool CanUseTransport;

	[HideInInspector]
	public Vector3 mouseWorldPosition;

	private RaycastHit[] hits = (RaycastHit[])(object)new RaycastHit[40];

	[HideInInspector]
	public Vector3 grabOffset;

	public GameDataLoader GameDataLoader;

	private GameDataValidator validator;

	[HideInInspector]
	public bool DebugEndlessMoonEnabled;

	[HideInInspector]
	public bool DebugNoFoodEnabled;

	[HideInInspector]
	public bool ForestMoonEnabled;

	[HideInInspector]
	public bool DebugDontNeedVillagers;

	[HideInInspector]
	public bool DebugNoEnergyEnabled;

	public List<SerializedKeyValuePair> RoundExtraKeyValues = new List<SerializedKeyValuePair>();

	private bool IsLoadingSaveRound;

	private bool clickStartedGrabbing;

	[HideInInspector]
	public bool CutsceneBoardView;

	private TMP_InputField currentSelectedInput;

	public List<Curse> ActiveCurses = new List<Curse>();

	public List<ActionTimeBase> actionTimeBases = new List<ActionTimeBase>();

	public List<ActionTimeModifier> actionTimeModifiers = new List<ActionTimeModifier>();

	public CardTarget NearbyCardTarget;

	public float SpeedUp = 1f;

	public QueuedAnimation currentAnimation;

	private List<QueuedAnimation> queuedAnimations = new List<QueuedAnimation>();

	private float physicsTimer;

	private float preAutoPauseSpeed;

	private bool isAutoPaused;

	public bool IsShiftDragging;

	public float GridWidth = 0.75f;

	public float GridHeight = 0.85f;

	public float gridAlpha;

	private HashSet<string> doesntCountTowardsCount = new HashSet<string> { "gold", "shell", "happiness", "unhappiness", "pollution" };

	private List<CityHall> townhalls = new List<CityHall>();

	private List<Dollar> dollars = new List<Dollar>();

	private List<Creditcard> creditcards = new List<Creditcard>();

	public bool ShowContinueButton;

	public string ContinueButtonText = "";

	public string CutsceneText = "";

	public string CutsceneTitle = "";

	public bool RemovingCards;

	public bool ConnectConnectors;

	[HideInInspector]
	public bool ContinueClicked;

	[HideInInspector]
	public int ContinueButtonIndex;

	public bool InEatingAnimation;

	public float EndOfMonthSpeedup;

	public bool VillagersStarvedAtEndOfMoon;

	public bool VillagersAngryAtEndOfMoon;

	public Coroutine currentAnimationRoutine;

	private WeightedRandomBag<CardChance> chanceBag = new WeightedRandomBag<CardChance>();

	public int QuestsCompleted;

	public int NewCardsFound;

	public RunOptions CurrentRunOptions;

	public RunVariables CurrentRunVariables;

	public List<string> GivenCards = new List<string>();

	public GameCard DraggingCard => DraggingDraggable as GameCard;

	public GameCard HoveredCard => HoveredDraggable as GameCard;

	public List<CardData> CardDataPrefabs => GameDataLoader.CardDataPrefabs;

	public List<Blueprint> BlueprintPrefabs => GameDataLoader.BlueprintPrefabs;

	public List<BoosterpackData> BoosterPackDatas => GameDataLoader.BoosterpackDatas;

	public SaveGame CurrentSave => SaveManager.instance.CurrentSave;

	public int CurrentMonth
	{
		get
		{
			if (BoardMonths != null)
			{
				return BoardMonths.GetCurrentMonth();
			}
			return 0;
		}
	}

	public float MonthTime
	{
		get
		{
			if (CurrentRunOptions != null)
			{
				if (CurrentRunOptions.MoonLength == MoonLength.Short)
				{
					return 90f;
				}
				if (CurrentRunOptions.MoonLength == MoonLength.Normal)
				{
					return 120f;
				}
				if (CurrentRunOptions.MoonLength == MoonLength.Long)
				{
					return 200f;
				}
			}
			return 120f;
		}
	}

	public bool IsPlaying => CurrentGameState == GameState.Playing;

	public bool InAnimation
	{
		get
		{
			if (currentAnimationRoutine == null)
			{
				return currentAnimation != null;
			}
			return true;
		}
	}

	public bool CanInteract
	{
		get
		{
			if (IsPlaying && ((currentAnimationRoutine == null && currentAnimation == null) || RemovingCards) && !GameScreen.instance.ControllerIsInUI)
			{
				return !GameCanvas.instance.ModalIsOpen;
			}
			return false;
		}
	}

	public List<SerializedKeyValuePair> SaveExtraKeyValues => CurrentSave.ExtraKeyValues;

	public GameBoard CurrentBoard { get; private set; }

	public Boosterpack IntroPack => AllBoosters.FirstOrDefault((Boosterpack x) => x.IsIntroPack && x.MyBoard.IsCurrent);

	public float TimeScale
	{
		get
		{
			if (!IsPlaying)
			{
				return 0f;
			}
			if (currentAnimationRoutine != null || currentAnimation != null)
			{
				return 0f;
			}
			if (GameCanvas.instance.ModalIsOpen)
			{
				return 0f;
			}
			if (TransitionScreen.InTransition)
			{
				return 0f;
			}
			return SpeedUp;
		}
	}

	public float PhysicsTimeScale
	{
		get
		{
			if (!IsPlaying)
			{
				return 0f;
			}
			if (SpeedUp == 0f)
			{
				return 1f;
			}
			return SpeedUp;
		}
	}

	private void Awake()
	{
		instance = this;
		AllCards = new List<GameCard>();
		Boards = Object.FindObjectsOfType<GameBoard>().ToList();
		CurrentGameState = GameState.InMenu;
		GameDataLoader = new GameDataLoader(SpiritDLCInstalled(), CitiesDLCInstalled());
		Subprint.UpdateAnyVillagerCardIds();
		Subprint.UpdateAnyWorkerCardIds();
		if (Application.isEditor)
		{
			DebugEndlessMoonEnabled = DebugOptions.Default.EndlessMoonEnabled;
			DebugNoFoodEnabled = DebugOptions.Default.NoFoodEnabled;
			DebugDontNeedVillagers = DebugOptions.Default.DontNeedVillagers;
			DebugNoEnergyEnabled = DebugOptions.Default.NoEnergyEnabled;
			CurrentSave.FinishedDeath = DebugOptions.Default.CursedFinished;
			CurrentSave.FinishedGreed = DebugOptions.Default.CursedFinished;
			CurrentSave.FinishedHappiness = DebugOptions.Default.CursedFinished;
			((Component)this).gameObject.AddComponent<Screenshotter>();
		}
		Shader.SetGlobalFloat("_WorldSizeIncrease", 0f);
		Shader.SetGlobalFloat("_WorldSizeIncreaseNormalized", 0f);
		SokLoc.instance.LanguageChanged += OnLanguageChange;
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		InitializeBaseVillagerSpeedRules();
	}

	private void Start()
	{
		OptionsScreen.LoadSettings();
		QuestManager.instance.CheckSteamAchievements();
		CheckForceReloadSave();
	}

	private void InitActionTimeBases()
	{
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "dog" || p.villager.Id == "cat", 2f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "fisher" && p.actionId == "complete_harvest" && p.baseCard.Id == "fishing_spot", 0.5f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "explorer", 1.25f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "explorer" && p.actionId == "complete_harvest" && p.baseCard.MyCardType == CardType.Locations, 0.5f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "lumberjack" && p.actionId == "complete_harvest" && (p.baseCard.Id == "lumbercamp" || p.baseCard.Id == "apple_tree" || p.baseCard.Id == "tree" || p.baseCard.Id == "olive_tree"), 0.5f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "builder" && p.actionId == "finish_blueprint", 0.5f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.Id == "miner" && p.actionId == "complete_harvest" && (p.baseCard.Id == "gold_mine" || p.baseCard.Id == "mine" || p.baseCard.Id == "rock" || p.baseCard.Id == "quarry" || p.baseCard.Id == "iron_deposit"), 0.5f));
		actionTimeBases.Add(new ActionTimeBase((ActionTimeParams p) => p.villager.HasEquipableWithId("scythe") && p.actionId == "complete_harvest" && (p.baseCard.Id == "berrybush" || p.baseCard.Id == "olive_tree" || p.baseCard.Id == "apple_tree" || p.baseCard.Id == "grape_vine" || p.baseCard.Id == "tomato_plant"), 0.5f));
	}

	private void InitActionTimeModifiers()
	{
		actionTimeModifiers.Add(new ActionTimeModifier((ActionTimeParams p) => p.villager.HasStatusEffectOfType<StatusEffect_Drunk>(), 2f));
		actionTimeModifiers.Add(new ActionTimeModifier((ActionTimeParams p) => p.villager.HasStatusEffectOfType<StatusEffect_Anxious>(), 2.5f));
		actionTimeModifiers.Add(new ActionTimeModifier((ActionTimeParams p) => p.villager.HasStatusEffectOfType<StatusEffect_WellFed>(), 0.5f));
		actionTimeModifiers.Add(new ActionTimeModifier((ActionTimeParams p) => p.villager.MyLifeStage == LifeStage.Teenager, 0.75f));
		actionTimeModifiers.Add(new ActionTimeModifier((ActionTimeParams p) => p.villager.MyLifeStage == LifeStage.Elderly, 1.25f));
	}

	public void InitializeBaseVillagerSpeedRules()
	{
		InitActionTimeBases();
		InitActionTimeModifiers();
	}

	public HashSet<string> FindMissingCardsInSave()
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (SavedCard savedCard in CurrentSave.LastPlayedRound.SavedCards)
		{
			if ((Object)(object)GameDataLoader.GetCardFromId(savedCard.CardPrefabId) == (Object)null)
			{
				hashSet.Add(savedCard.CardPrefabId);
			}
		}
		return hashSet;
	}

	public void LoadPreviousRound()
	{
		LoadSaveRound(CurrentSave.LastPlayedRound);
		foreach (GameBoard board in Boards)
		{
			board.WorldSizeIncrease = DetermineTargetWorldSize(board);
		}
		if (QuestManager.instance.QuestIsComplete(AllQuests.KillDemon) && !CurrentSave.GotIslandIntroPack)
		{
			QueueCutscene(Cutscenes.IslandIntroPack());
		}
	}

	public SaveRound GetSaveRound()
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		SaveRound saveRound = new SaveRound();
		saveRound.SaveVersion = 3;
		saveRound.SavedCards = new List<SavedCard>();
		saveRound.SavedBoosters = new List<SavedBooster>();
		saveRound.SavedBoosterBoxes = new List<SavedBoosterBox>();
		saveRound.SavedConflicts = new List<SavedConflict>();
		saveRound.RunVariables = CurrentRunVariables;
		saveRound.RunOptions = CurrentRunOptions;
		saveRound.BoughtBoosterIds = BoughtBoosterIds;
		saveRound.CurrentBoardId = CurrentBoard.Id;
		saveRound.ExtraKeyValues = RoundExtraKeyValues;
		foreach (GameCard allCard in AllCards)
		{
			saveRound.SavedCards.Add(allCard.ToSavedCard());
		}
		foreach (Boosterpack allBooster in AllBoosters)
		{
			saveRound.SavedBoosters.Add(new SavedBooster
			{
				BoosterId = allBooster.BoosterId,
				TimesOpened = allBooster.TimesOpened,
				BoardId = allBooster.MyBoard.Id,
				Position = allBooster.TargetPosition
			});
		}
		foreach (BuyBoosterBox allBoosterBox in AllBoosterBoxes)
		{
			saveRound.SavedBoosterBoxes.Add(new SavedBoosterBox
			{
				BoosterId = allBoosterBox.BoosterId,
				StoredCostAmount = allBoosterBox.StoredCostAmount
			});
		}
		saveRound.MonthTimer = MonthTimer;
		saveRound.CurrentMonth = CurrentMonth;
		saveRound.OldCurrentMonth = OldCurrentMonth;
		saveRound.BoardMonths = BoardMonths.ToSavedMonth();
		saveRound.NewCardsFound = NewCardsFound;
		saveRound.QuestsCompleted = QuestsCompleted;
		saveRound.CitiesWellbeing = CitiesManager.instance.Wellbeing;
		saveRound.CitiesConflictMonth = CitiesManager.instance.NextConflictMonth;
		saveRound.CitiesDisaster = CitiesManager.instance.ActiveEvent;
		foreach (Conflict allConflict in GetAllConflicts())
		{
			saveRound.SavedConflicts.Add(new SavedConflict
			{
				Id = allConflict.Id,
				InitiatorCardId = allConflict.Initiator.UniqueId,
				InvolvedCards = allConflict.Participants.Select((Combatable x) => x.UniqueId).ToList(),
				StartPosition = allConflict.ConflictStartPosition
			});
		}
		return saveRound;
	}

	private void OnApplicationQuit()
	{
		Shader.SetGlobalFloat("_WorldSizeIncrease", 0f);
		Shader.SetGlobalFloat("_WorldSizeIncreaseNormalized", 0f);
		if (CurrentGameState == GameState.Playing && currentAnimationRoutine == null && currentAnimation == null)
		{
			SaveManager.instance.Save(saveRound: true);
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)SokLoc.instance != (Object)null)
		{
			SokLoc.instance.LanguageChanged -= OnLanguageChange;
		}
	}

	public void SaveAndGoBackToMenu()
	{
		SaveManager.instance.Save(saveRound: true);
		RestartGame();
	}

	public void UpdateCardTargets()
	{
		foreach (CardTarget cardTarget in CardTargets)
		{
			if (cardTarget is BuyBoosterBox buyBoosterBox)
			{
				buyBoosterBox.UpdateUndiscoveredCards();
			}
		}
	}

	public void Play()
	{
		GameCanvas.instance.SetScreen<GameScreen>();
		CurrentGameState = GameState.Playing;
		GameCamera.instance.CenterOnBoard(CurrentBoard);
		QuestManager.instance.CheckPacksUnlocked();
		GameScreen.instance.OnBoardChange();
		UpdateCardTargets();
	}

	public Vector3 MiddleOfBoard()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CurrentBoard.MiddleOfBoard();
	}

	public void SetViewType(ViewType type)
	{
		CurrentView = type;
		foreach (GameCard allCard in AllCards)
		{
			allCard.UpdateCardPalette();
		}
		CitiesManager.instance.StopDrawCable(null);
	}

	public void SavePreset(SavedPreset preset)
	{
		string content = JsonUtility.ToJson((object)preset);
		FileHelper.SavePresetFile(preset.SaveId, content);
	}

	public void StartNewRound()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ClearRound();
		CurrentBoard = GetBoardWithId("main");
		CreateBoosterpack(MiddleOfBoard(), "starter");
	}

	public void ClearSaveAndRestart()
	{
		SaveGame saveGame = new SaveGame();
		saveGame.LastSavedUtc = DateTime.UtcNow;
		saveGame.SaveId = CurrentSave.SaveId;
		string content = JsonUtility.ToJson((object)saveGame);
		FileHelper.SaveFile(CurrentSave.SaveId, content);
		SaveManager.instance.Save(saveGame);
		RestartGame();
	}

	public void QuestCompleted(Quest quest)
	{
		Debug.Log((object)("Completed quest " + quest.Id));
		if ((Object)(object)GameScreen.instance != (Object)null && quest.QuestLocation == CurrentBoard.Location)
		{
			GameScreen.instance.AddNotification(SokLoc.Translate("label_quest_completed"), quest.Description, delegate
			{
				GameScreen.instance.ScrollToQuest(quest);
			});
		}
		AudioManager.me.PlaySound2D(AudioManager.me.QuestComplete, 1f, 0.1f);
		BoosterpackData boosterpackData = QuestManager.instance.JustUnlockedPack();
		if ((Object)(object)boosterpackData != (Object)null)
		{
			bool flag = boosterpackData.BoosterLocation == quest.QuestLocation;
			if (boosterpackData.BoosterLocation != CurrentBoard.Location)
			{
				flag = false;
			}
			if (TransitionScreen.InTransition)
			{
				flag = false;
			}
			if (flag)
			{
				QueueCutscene(Cutscenes.JustUnlockedPack(boosterpackData));
			}
		}
	}

	public void OnLanguageChange()
	{
		foreach (GameCard allCard in AllCards)
		{
			allCard.CardData.OnLanguageChange();
		}
		foreach (CardData cardDataPrefab in GameDataLoader.CardDataPrefabs)
		{
			cardDataPrefab.OnLanguageChange();
		}
	}

	private void QueueAnimation(QueuedAnimation anim)
	{
		queuedAnimations.Add(anim);
	}

	private void CheckQueuedAnimations()
	{
		if (!InAnimation && !GameCanvas.instance.ModalIsOpen && IsPlaying && currentAnimation == null && queuedAnimations.Count > 0)
		{
			CloseOpenInventories();
			SetViewType(ViewType.Default);
			currentAnimation = queuedAnimations[0];
			currentAnimation.OnActivate();
			queuedAnimations.RemoveAt(0);
		}
	}

	public void QueueCutscene(IEnumerator coroutine)
	{
		QueueAnimation(new QueuedAnimation(delegate
		{
			((MonoBehaviour)this).StartCoroutine(coroutine);
		}));
	}

	public void QueueCutsceneIfNotQueued(IEnumerator coroutine, string id)
	{
		if (!queuedAnimations.Any((QueuedAnimation x) => x.Id == id))
		{
			QueueAnimation(new QueuedAnimation(delegate
			{
				((MonoBehaviour)this).StartCoroutine(coroutine);
			}, id));
		}
	}

	public void QueueCutsceneIfNotPlayed(string cutsceneId)
	{
		if (!CurrentRunVariables.PlayedCutsceneIds.Contains(cutsceneId))
		{
			CurrentRunVariables.PlayedCutsceneIds.Add(cutsceneId);
			QueueCutscene(cutsceneId);
		}
	}

	public void QueueCutscene(string cutsceneId)
	{
		ScriptableCutscene cutsceneWithId = GameDataLoader.GetCutsceneWithId(cutsceneId);
		QueueCutscene(cutsceneWithId);
	}

	public void QueueCutscene(ScriptableCutscene cutscene)
	{
		QueueAnimation(new QueuedAnimation(delegate
		{
			if (!CurrentRunVariables.PlayedCutsceneIds.Contains(cutscene.CutsceneId))
			{
				CurrentRunVariables.PlayedCutsceneIds.Add(cutscene.CutsceneId);
			}
			((MonoBehaviour)this).StartCoroutine(Cutscenes.RunScriptableCutscene(cutscene));
		}));
	}

	public void ModalAbandonCity()
	{
		GameCanvas.instance.AbandonCityPrompt(AbandonCity, null);
	}

	public void AbandonCity()
	{
		GameBoard citiesBoard = GetCurrentBoardSafe();
		GoToBoard(GetBoardWithId("main"), delegate
		{
			RemoveAllCardsFromBoard(citiesBoard.Id);
			ResetBoughtBoostersOnLocation(citiesBoard.Location);
			ResetCityVariables();
			if (CurrentGameState == GameState.Paused)
			{
				TogglePause();
			}
		}, "cities");
	}

	private void ResetCityVariables()
	{
		CurrentRunVariables.HasCitiesBoard = false;
		CurrentRunVariables.BuiltLandmarks = new List<string>();
		CurrentRunVariables.OpenedFirstTrash = false;
		BoardMonths.CitiesMonth = 1;
		CitiesManager.instance.Wellbeing = 15;
	}

	public void TogglePause()
	{
		if (currentAnimationRoutine == null && currentAnimation == null && (CurrentGameState == GameState.Paused || CurrentGameState == GameState.Playing) && !GameCanvas.instance.ModalIsOpen)
		{
			if (CurrentGameState == GameState.Playing)
			{
				CurrentGameState = GameState.Paused;
			}
			else if (CurrentGameState == GameState.Paused)
			{
				CurrentGameState = GameState.Playing;
			}
			bool num = CurrentGameState == GameState.Paused;
			SetViewType(ViewType.Default);
			if (num)
			{
				GameCanvas.instance.SetScreen<PauseScreen>();
			}
			else
			{
				GameCanvas.instance.SetScreen<GameScreen>();
			}
		}
	}

	private void CheckDisableInput()
	{
		if ((Object)(object)EventSystem.current.currentSelectedGameObject != (Object)null)
		{
			TMP_InputField component = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
			if ((Object)(object)component != (Object)null)
			{
				currentSelectedInput = component;
				InputController.instance.DisableAllInput = true;
				return;
			}
		}
		InputController.instance.DisableAllInput = false;
		currentSelectedInput = null;
	}

	public float DetermineTargetWorldSize(GameBoard board)
	{
		float num = Mathf.Max(0.15f, board.PackLineWidth - 8.86f);
		float num2 = 4.5f;
		num = Mathf.Max(num, board.BoardOptions.BaseBoardSize);
		if (board.Id == "cities")
		{
			num2 = 10f;
		}
		return Mathf.Clamp(Mathf.Clamp(num + (float)CardCapIncrease(board) * 0.05f, num, num2) + (float)BoardSizeIncrease(board) * 0.05f, num, num2 + 3f);
	}

	public Vector3 ScreenPosToWorldPos(Vector3 screenPos, out Ray ray)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		ray = Camera.main.ScreenPointToRay(screenPos);
		Plane val = default(Plane);
		((Plane)(ref val))._002Ector(Vector3.up, Vector3.zero);
		float num = default(float);
		((Plane)(ref val)).Raycast(ray, ref num);
		return ((Ray)(ref ray)).origin + ((Ray)(ref ray)).direction * num;
	}

	public void IncrementMonth()
	{
		BoardMonths.IncrementMonth();
	}

	private void Update()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isEditor)
		{
			CheckDebugInput();
		}
		if ((Object)(object)CurrentBoard != (Object)null && CurrentBoard.Id == "forest")
		{
			ForestMoonEnabled = true;
		}
		else
		{
			ForestMoonEnabled = false;
		}
		Shader.SetGlobalFloat("_GridWidth", GridWidth);
		Shader.SetGlobalFloat("_GridHeight", GridHeight);
		Shader.SetGlobalFloat("_AnimationTime", AnimationTime);
		if ((Object)(object)CurrentBoard != (Object)null)
		{
			Shader.SetGlobalColor("_BoardBackgroundA", ((Color)(ref CurrentBoard.BoardOptions.CardBackgroundPallete.Color)).linear);
			Shader.SetGlobalColor("_BoardBackgroundB", ((Color)(ref CurrentBoard.BoardOptions.CardBackgroundPallete.Color2)).linear);
		}
		gridAlpha = Mathf.Lerp(gridAlpha, 0f, Time.deltaTime * 3f);
		Shader.SetGlobalFloat("_GridAlpha", gridAlpha);
		UpdatePhysics();
		CheckQueuedAnimations();
		CheckResetCanDropItem();
		clickStartedGrabbing = false;
		bool flag = true;
		if ((Object)(object)IntroPack != (Object)null && !IntroPack.WasClicked)
		{
			flag = false;
		}
		if (currentAnimationRoutine != null || currentAnimation != null)
		{
			flag = false;
		}
		if (ForestMoonEnabled)
		{
			flag = false;
		}
		if (flag && !DebugEndlessMoonEnabled)
		{
			MonthTimer += Time.deltaTime * TimeScale;
		}
		AnimationTime += Time.deltaTime * TimeScale;
		if (!DebugEndlessMoonEnabled && MonthTimer >= MonthTime && currentAnimationRoutine == null)
		{
			MonthTimer -= MonthTime;
			IncrementMonth();
			EndOfMonth();
		}
		Ray ray;
		if (InputController.instance.CurrentSchemeIsController)
		{
			mouseWorldPosition = ScreenPosToWorldPos(Vector2.op_Implicit(new Vector2((float)Screen.width, (float)Screen.height) * 0.5f), out ray);
		}
		else if (InputController.instance.CurrentSchemeIsTouch)
		{
			mouseWorldPosition = ScreenPosToWorldPos(Vector2.op_Implicit(InputController.instance.GetSafeTouchPosition(0)), out ray);
		}
		else
		{
			mouseWorldPosition = ScreenPosToWorldPos(Vector2.op_Implicit(InputController.instance.ClampedMousePosition()), out ray);
		}
		CheckDisableInput();
		HoveredDraggable = null;
		HoveredInteractable = null;
		CurrentHoverable = null;
		CanUseTransport = GetCardCount<RoadBuilder>() > 0;
		if (InputController.instance.ToggleViewTriggered() && !InAnimation && !TransitionScreen.InTransition && (CurrentGameState == GameState.Playing || CurrentGameState == GameState.Paused))
		{
			if (CurrentBoard.Id == "cities")
			{
				int viewType = (int)((CurrentView == ViewType.Calamity) ? ViewType.Default : (CurrentView + 1));
				SetViewType((ViewType)viewType);
			}
			else if (CanUseTransport)
			{
				if (CurrentView == ViewType.Transport)
				{
					SetViewType(ViewType.Default);
				}
				else
				{
					SetViewType(ViewType.Transport);
				}
			}
		}
		if (GetCurrentBoardSafe().Id != "cities" && !CanUseTransport)
		{
			SetViewType(ViewType.Default);
		}
		if (InputController.instance.PauseTriggered())
		{
			TogglePause();
		}
		if (IsPlaying && InputController.instance.SnapCardsTriggered())
		{
			SnapCardsToGrid();
		}
		bool flag2 = InputController.instance.GetInput(0) && GameCanvas.instance.PositionIsOverUI(InputController.instance.GetInputPosition(0));
		bool flag3 = AccessibilityScreen.ClickToDragEnabled;
		if (InputController.instance.CurrentSchemeIsController || InputController.instance.CurrentSchemeIsTouch)
		{
			flag3 = false;
		}
		if (InputController.instance.CurrentScheme == ControlScheme.KeyboardMouse)
		{
			flag2 = GameCanvas.instance.MousePositionIsOverUI();
		}
		if (InputController.instance.CurrentSchemeIsController)
		{
			flag2 = false;
		}
		if (!flag2 && (InputController.instance.CurrentSchemeIsMouseKeyboard || InputController.instance.CurrentSchemeIsController))
		{
			int num = Physics.RaycastNonAlloc(ray, hits);
			float num2 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				RaycastHit val = hits[i];
				if (!((Component)((RaycastHit)(ref val)).collider).gameObject.activeInHierarchy)
				{
					continue;
				}
				Draggable component = DraggableLookup.GetComponent(((Component)((RaycastHit)(ref val)).collider).gameObject);
				if ((Object)(object)component != (Object)null)
				{
					float distance = ((RaycastHit)(ref val)).distance;
					if (distance < num2)
					{
						num2 = distance;
						if ((Object)(object)DraggingDraggable == (Object)null)
						{
							HoveredDraggable = component;
						}
					}
				}
				Interactable component2 = InteractableLookup.GetComponent(((Component)((RaycastHit)(ref val)).collider).gameObject);
				if ((Object)(object)component2 != (Object)null)
				{
					HoveredInteractable = component2;
				}
				Hoverable component3 = HoverableLookup.GetComponent(((Component)((RaycastHit)(ref val)).collider).gameObject);
				if ((Object)(object)component3 != (Object)null)
				{
					CurrentHoverable = component3;
				}
			}
		}
		if (InputController.instance.CurrentSchemeIsTouch)
		{
			int num3 = Physics.RaycastNonAlloc(ray, hits);
			float num4 = float.MaxValue;
			for (int j = 0; j < num3; j++)
			{
				RaycastHit val2 = hits[j];
				if (!((Component)((RaycastHit)(ref val2)).collider).gameObject.activeInHierarchy)
				{
					continue;
				}
				Draggable component4 = DraggableLookup.GetComponent(((Component)((RaycastHit)(ref val2)).collider).gameObject);
				if ((Object)(object)component4 != (Object)null)
				{
					float distance2 = ((RaycastHit)(ref val2)).distance;
					if (distance2 < num4)
					{
						num4 = distance2;
						if ((Object)(object)DraggingDraggable == (Object)null)
						{
							HoveredDraggable = component4;
						}
					}
				}
				Interactable component5 = InteractableLookup.GetComponent(((Component)((RaycastHit)(ref val2)).collider).gameObject);
				if ((Object)(object)component5 != (Object)null)
				{
					HoveredInteractable = component5;
				}
				Hoverable component6 = HoverableLookup.GetComponent(((Component)((RaycastHit)(ref val2)).collider).gameObject);
				if ((Object)(object)component6 != (Object)null)
				{
					CurrentHoverable = component6;
				}
			}
		}
		if ((Object)(object)HoveredInteractable != (Object)null)
		{
			GameScreen.InfoBoxTitle = ((Object)HoveredInteractable).name;
			GameScreen.InfoBoxText = HoveredInteractable.GetTooltipText();
		}
		if ((Object)(object)CurrentHoverable != (Object)null)
		{
			GameScreen.InfoBoxTitle = CurrentHoverable.GetTitle();
			GameScreen.InfoBoxText = CurrentHoverable.GetDescription();
		}
		if (CanInteract || ConnectConnectors)
		{
			if (ConnectConnectors && (Object)(object)HoveredDraggable != (Object)null && !(HoveredDraggable is CardConnector))
			{
				return;
			}
			bool flag4 = InputController.instance.StartedGrabbing();
			if ((InputController.instance.GetInputBegan(0) || flag4) && !flag2)
			{
				if ((Object)(object)HoveredDraggable != (Object)null)
				{
					if (HoveredDraggable.CanBeDragged())
					{
						Draggable draggable = HoveredDraggable;
						draggable.DragTag = null;
						draggable.ClickedObject = null;
						if (HoveredDraggable is GameCard gameCard && InputController.instance.GetKey((Key)51))
						{
							IsShiftDragging = true;
							draggable = gameCard.GetRootCard();
							draggable.ClickedObject = gameCard;
						}
						else
						{
							IsShiftDragging = false;
						}
						DraggingDraggable = draggable;
						DraggingDraggable.DragStartPosition = ((Component)DraggingDraggable).transform.position;
						grabOffset = mouseWorldPosition - ((Component)DraggingDraggable).transform.position;
						DraggingDraggable.StartDragging();
						if (flag3)
						{
							clickStartedGrabbing = true;
						}
					}
					else
					{
						HoveredDraggable.Clicked();
						GameCard gameCard2 = HoveredDraggable as GameCard;
						if ((Object)(object)gameCard2 != (Object)null)
						{
							gameCard2.RotWobble(1f);
						}
					}
				}
				else if ((Object)(object)HoveredInteractable != (Object)null)
				{
					HoveredInteractable.Click();
				}
				else if ((InputController.instance.CurrentSchemeIsMouseKeyboard || InputController.instance.CurrentSchemeIsTouch) && !InputController.instance.GetRightMouseBegan())
				{
					GameCamera.instance.StartDragging();
				}
				else
				{
					GameCamera.instance.Clicked();
				}
			}
			if (InputController.instance.ToggleInventoryTriggered() && !flag2 && (Object)(object)HoveredCard != (Object)null && HoveredCard.CardData.HasInventory)
			{
				HoveredCard.ShowInventory = !HoveredCard.ShowInventory;
			}
			if (InputController.instance.SellTriggered() && !flag2 && (Object)(object)HoveredCard != (Object)null && CardCanBeSold(HoveredCard))
			{
				SellCard(((Component)HoveredCard).transform.position, HoveredCard.GetRootCard());
			}
		}
		else if (CutsceneBoardView)
		{
			bool flag5 = InputController.instance.StartedGrabbing();
			if ((InputController.instance.GetInputBegan(0) || flag5) && !flag2 && (Object)(object)HoveredDraggable == (Object)null)
			{
				if ((InputController.instance.CurrentSchemeIsMouseKeyboard || InputController.instance.CurrentSchemeIsTouch) && !InputController.instance.GetRightMouseBegan())
				{
					GameCamera.instance.StartDragging();
				}
				else
				{
					GameCamera.instance.Clicked();
				}
			}
		}
		else if ((InputController.instance.CurrentSchemeIsMouseKeyboard || InputController.instance.CurrentSchemeIsTouch) && InputController.instance.GetInputBegan(0) && !flag2 && !InAnimation)
		{
			GameCamera.instance.StartDragging();
		}
		bool flag6 = false;
		if (InputController.instance.CurrentSchemeIsController && AccessibilityScreen.AutoPauseWhenUsingController)
		{
			flag6 = true;
		}
		if (InputController.instance.CurrentScheme == ControlScheme.KeyboardMouse && AccessibilityScreen.AutoPauseWhenUsingKeyboardMouse)
		{
			flag6 = true;
		}
		if (flag6)
		{
			bool flag7 = (Object)(object)DraggingDraggable != (Object)null;
			if (flag7 && !isAutoPaused)
			{
				isAutoPaused = true;
				preAutoPauseSpeed = SpeedUp;
				SpeedUp = 0f;
			}
			if (isAutoPaused && !flag7)
			{
				isAutoPaused = false;
				SpeedUp = preAutoPauseSpeed;
			}
		}
		NearbyCardTarget = null;
		float num5 = float.MaxValue;
		if ((Object)(object)DraggingDraggable != (Object)null)
		{
			DraggingDraggable.TargetPosition = mouseWorldPosition - grabOffset;
			if ((Object)(object)DraggingCard != (Object)null)
			{
				DraggingCard.Clampieee();
			}
		}
		if ((Object)(object)DraggingCard != (Object)null)
		{
			foreach (CardTarget cardTarget in CardTargets)
			{
				if (cardTarget.CanHaveCard(DraggingCard))
				{
					float num6 = Vector3.Distance(DraggingCard.TargetPosition + grabOffset, ((Component)cardTarget).transform.position);
					num6 = Mathf.Min(Vector3.Distance(DraggingCard.TargetPosition, ((Component)cardTarget).transform.position), num6);
					if (num6 < CardTargetSnapDistance && num6 < num5)
					{
						NearbyCardTarget = cardTarget;
						DraggingCard.TargetPosition = ((Component)cardTarget).transform.position;
					}
				}
			}
		}
		if (!CanInteract && (Object)(object)DraggingDraggable != (Object)null)
		{
			DraggingDraggable.StopDragging();
			SetDraggingDraggableToNull();
		}
		bool flag8 = InputController.instance.StoppedGrabbing();
		if (InputController.instance.GetInputEnded(0) || flag8 || !CanInteract)
		{
			if (flag3)
			{
				if (InputController.instance.GetLeftMouseEnded())
				{
					DropCard();
				}
			}
			else
			{
				DropCard();
			}
		}
		if (flag3 && InputController.instance.GetRightMouseBegan() && !clickStartedGrabbing)
		{
			DropCard();
		}
		if (CurrentGameState == GameState.Playing && currentAnimationRoutine == null && currentAnimation == null && (Object)(object)IntroPack == (Object)null && !TransitionScreen.InTransition)
		{
			if (CheckAllVillagersDead())
			{
				if (CurrentBoard.Id == "main")
				{
					CurrentGameState = GameState.GameOver;
					GameCanvas.instance.SetScreen<GameOverScreen>();
				}
				else if (CurrentBoard.Id == "island")
				{
					QueueCutscene(Cutscenes.EveryoneOnIslandDead());
				}
				else if (CurrentBoard.BoardOptions.IsSpiritWorld)
				{
					QueueCutscene(Cutscenes.EveryoneInSpiritWorldDead(CurrentBoard.Id));
				}
				else if (!(CurrentBoard.Id == "forest") && !(CurrentBoard.Id == "cities"))
				{
					QueueCutscene(Cutscenes.EveryoneOnIslandDead());
				}
			}
			CitiesManager.instance.CheckCityHealth();
		}
		if (TimeScale > 0f)
		{
			CheckSpiritCutscenes();
		}
		DebugUpdate();
	}

	public void SetDraggingDraggableToNull()
	{
		DraggingDraggable = null;
		foreach (Draggable allDraggable in AllDraggables)
		{
			allDraggable.BeingDragged = false;
		}
	}

	public Sprite GetCurrencyIcon(BoardCurrency? currency)
	{
		if (currency.HasValue)
		{
			if (currency == BoardCurrency.Gold)
			{
				return SpriteManager.instance.CoinIcon;
			}
			if (currency == BoardCurrency.Shell)
			{
				return SpriteManager.instance.ShellIcon;
			}
			if (currency == BoardCurrency.Dollar)
			{
				return SpriteManager.instance.DollarIcon;
			}
		}
		return SpriteManager.instance.CoinIcon;
	}

	private void CheckResetCanDropItem()
	{
		if (!((Object)(object)CurrentBoard == (Object)null) && CurrentBoard.BoardOptions.ResetItemDrops && !CurrentRunVariables.CanDropItem && GetCardCount((CardData x) => x is Enemy) == 0)
		{
			CurrentRunVariables.CanDropItem = true;
		}
	}

	private void DropCard()
	{
		if ((Object)(object)DraggingCard != (Object)null)
		{
			if ((Object)(object)NearbyCardTarget != (Object)null)
			{
				NearbyCardTarget.CardDropped(DraggingCard);
			}
			else
			{
				CheckIfCanAddOnStack(DraggingCard);
			}
		}
		if ((Object)(object)DraggingDraggable != (Object)null)
		{
			DraggingDraggable.StopDragging();
			SetDraggingDraggableToNull();
		}
	}

	public void OnBoosterOpened(string boosterId)
	{
		if (CurrentBoard.Id == "greed" && boosterId == "greed_intro")
		{
			QueueCutscene(Cutscenes.GreedIntro());
		}
		else if (CurrentBoard.Id == "happiness" && boosterId == "happiness_intro")
		{
			QueueCutscene(Cutscenes.HappinessIntro());
		}
		else if (CurrentBoard.Id == "death" && boosterId == "death_intro")
		{
			QueueCutscene(Cutscenes.DeathIntro());
		}
		else if (CurrentBoard.Id == "cities" && boosterId == "cities_intro")
		{
			QueueCutscene("cities_start");
		}
	}

	public void CloseOpenInventories()
	{
		foreach (GameCard allCard in AllCards)
		{
			allCard.ShowInventory = false;
		}
	}

	private void UpdatePhysics()
	{
		physicsTimer += Time.deltaTime * PhysicsTimeScale;
		while (physicsTimer >= 0.02f)
		{
			physicsTimer -= 0.02f;
			foreach (Draggable physicsDraggable in PhysicsDraggables)
			{
				physicsDraggable.UpdatePhysics(0.02f);
			}
		}
	}

	public void SnapCardsToGrid()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		gridAlpha = 1f;
		foreach (GameCard allCard in AllCards)
		{
			if (!allCard.HasParent && !(allCard.CardData is Mob))
			{
				Vector3 position = ((Component)allCard).transform.position;
				position.x = (float)Mathf.RoundToInt(position.x / GridWidth) * GridWidth;
				position.z = (float)Mathf.RoundToInt(position.z / GridHeight) * GridHeight;
				allCard.TargetPosition = position;
			}
		}
	}

	public void DissolveStack(GameCard card)
	{
		GameCard gameCard = card.GetLeafCard();
		while ((Object)(object)gameCard != (Object)null)
		{
			GameCard parent = gameCard.Parent;
			gameCard.RemoveFromStack();
			gameCard = parent;
		}
	}

	public bool CardCanBeSold(GameCard card, bool checkStatus = true, bool checkSpeedup = false)
	{
		if (checkSpeedup && SpeedUp == 0f)
		{
			return false;
		}
		if (card.IsEquipped || card.IsWorking)
		{
			return false;
		}
		if (card.MyBoard.Id == "forest")
		{
			return false;
		}
		if (card.WorkerChildren.Count > 0)
		{
			return false;
		}
		if (card.InConflict)
		{
			return false;
		}
		List<GameCard> allCardsInStack = card.GetAllCardsInStack();
		if (allCardsInStack.Any((GameCard x) => x.CardData is ResourceChest resourceChest && resourceChest.ResourceCount > 0))
		{
			return false;
		}
		if (allCardsInStack.Any((GameCard x) => x.CardData is Chest chest && chest.CoinCount > 0))
		{
			return false;
		}
		if (allCardsInStack.Any((GameCard x) => x.CardData is Creditcard creditcard && creditcard.DollarCount > 0))
		{
			return false;
		}
		if (allCardsInStack.Any((GameCard x) => x.CardData is FoodWarehouse foodWarehouse && foodWarehouse.FoodValue > 0))
		{
			return false;
		}
		if (checkStatus)
		{
			GameCard cardWithStatusInStack = card.GetCardWithStatusInStack();
			if ((Object)(object)cardWithStatusInStack != (Object)null && !(cardWithStatusInStack.CardData is EnergyGenerator))
			{
				return false;
			}
		}
		return !allCardsInStack.Any((GameCard x) => x.CardData.GetValue() == -1);
	}

	public void ClearRoundAndRestart()
	{
		CurrentSave.LastPlayedRound = null;
		SaveManager.instance.Save(saveRound: false);
		RestartGame();
	}

	public static void RestartGame()
	{
		SaveManager.instance.Load();
		instance.CurrentGameState = GameState.InMenu;
		instance.SpeedUp = 1f;
		((Behaviour)GameCamera.instance.PauseVolume).enabled = false;
		((Component)GameCamera.instance.PauseVolume).gameObject.SetActive(false);
		GameCanvas.instance.SetScreen<MainMenu>();
		GameCamera.instance.OnRestartGame();
		CardopediaScreen.instance.RefreshCardopedia();
		QuestManager.instance.UpdateCurrentQuests();
		foreach (BuyBoosterBox allBoosterBox in instance.AllBoosterBoxes)
		{
			allBoosterBox.UpdateUndiscoveredCards();
		}
		instance.ClearRound();
		Shader.SetGlobalFloat("_WorldSizeIncrease", 0f);
		Shader.SetGlobalFloat("_WorldSizeIncreaseNormalized", 0f);
		CheckForceReloadSave();
	}

	private static void CheckForceReloadSave()
	{
		if (SaveManager.IsForceReload)
		{
			SaveManager.IsForceReload = false;
			instance.LoadPreviousRound();
			instance.Play();
		}
	}

	public static void RebootGame()
	{
		Debug.Log((object)"attempting to reboot game..");
		if (!PlatformHelper.UseSteam)
		{
			Application.quitting += RelaunchProcess;
			Application.Quit();
		}
		else if (SteamManager.Initialized)
		{
			Application.quitting += RelaunchSteam;
			Application.Quit();
		}
		else
		{
			Debug.Log((object)"cant figure out how to reboot game");
		}
	}

	private static void RelaunchProcess()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		string fileName = (((int)Application.platform != 1) ? Path.Combine(Application.dataPath, "..", "Stacklands.exe") : Path.Combine(Application.dataPath, "MacOS/Stacklands"));
		Process.Start(fileName);
	}

	private static void RelaunchSteam()
	{
		Application.OpenURL("steam://rungameid/1948280");
	}

	public GameCard CreateCardStack(Vector3 pos, int amount, string cardId, bool checkAddToStack = true)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (amount == 0)
		{
			return null;
		}
		GameCard gameCard = null;
		while (amount > 0)
		{
			int num = Mathf.Min(amount, 30);
			gameCard = null;
			for (int i = 0; i < num; i++)
			{
				GameCard myGameCard = CreateCard(pos, cardId, faceUp: true, checkAddToStack).MyGameCard;
				if ((Object)(object)gameCard != (Object)null)
				{
					myGameCard.SetParent(gameCard);
				}
				gameCard = myGameCard;
			}
			amount -= num;
		}
		return gameCard;
	}

	public void StartCursePlaythrough(CurseType curse, Action onArrive)
	{
		GameBoard newBoard = null;
		switch (curse)
		{
		case CurseType.Happiness:
			newBoard = GetBoardWithId("happiness");
			break;
		case CurseType.Death:
			newBoard = GetBoardWithId("death");
			break;
		case CurseType.Greed:
			newBoard = GetBoardWithId("greed");
			break;
		}
		GameCanvas.instance.SetScreen<EmptyScreen>();
		GoToBoard(newBoard, delegate
		{
			GameCanvas.instance.SetScreen<GameScreen>();
			onArrive();
		}, "spirit");
	}

	public List<GameCard> CreateDollarsFromValue(int value, Vector3 pos, bool checkAddToStack = true)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (value <= 0)
		{
			return new List<GameCard>();
		}
		int result = 0;
		int amount = Math.DivRem(value, 100, out result);
		GameCard gameCard = CreateCardStack(pos, amount, "hundred_dollar", checkAddToStack: false);
		amount = Math.DivRem(result, 50, out result);
		GameCard gameCard2 = CreateCardStack(pos, amount, "fifty_dollar", checkAddToStack: false);
		amount = Math.DivRem(result, 20, out result);
		GameCard gameCard3 = CreateCardStack(pos, amount, "twenty_dollar", checkAddToStack: false);
		amount = Math.DivRem(result, 10, out result);
		GameCard gameCard4 = CreateCardStack(pos, amount, "ten_dollar", checkAddToStack: false);
		List<GameCard> list = new List<GameCard>();
		if ((Object)(object)gameCard != (Object)null)
		{
			list.AddRange(gameCard.GetAllCardsInStack());
		}
		if ((Object)(object)gameCard2 != (Object)null)
		{
			list.AddRange(gameCard2.GetAllCardsInStack());
		}
		if ((Object)(object)gameCard3 != (Object)null)
		{
			list.AddRange(gameCard3.GetAllCardsInStack());
		}
		if ((Object)(object)gameCard4 != (Object)null)
		{
			list.AddRange(gameCard4.GetAllCardsInStack());
		}
		Restack(list);
		if (checkAddToStack)
		{
			CheckIfCanAddOnStack(list.First());
		}
		else
		{
			list.First().SendIt();
		}
		return list;
	}

	public GameCard SellCard(Vector3 pos, GameCard card, float multiplier = 1f, bool checkAddToStack = true)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if (card.CardData.Id == "coin_chest" || card.CardData.Id == "shell_chest")
		{
			multiplier = 1f;
		}
		CardValue stackValue = GetStackValue(card);
		bool flag = false;
		foreach (GameCard item in card.GetAllCardsInStack())
		{
			if ((Object)(object)item.CardData != (Object)null)
			{
				item.CardData.OnSellCard();
			}
			if (item.CardData is Worker)
			{
				flag = true;
			}
			CreateSmoke(((Component)item).transform.position);
		}
		DestroyStack(card);
		GameCard gameCard = null;
		if (flag)
		{
			QuestManager.instance.SpecialActionComplete("worker_removed");
		}
		if (CurrentBoard.Id == "cities")
		{
			if (stackValue.TotalValue >= 10)
			{
				List<GameCard> list = CreateDollarsFromValue(stackValue.TotalValue, pos);
				gameCard = list?.FirstOrDefault();
				if (list != null)
				{
					Dollar dollar = null;
					for (int i = 0; i < list.Count; i++)
					{
						Dollar dollar2 = list[i].CardData as Dollar;
						if ((Object)(object)dollar == (Object)null || dollar2.DollarValue > dollar.DollarValue)
						{
							dollar = dollar2;
						}
					}
					if ((Object)(object)dollar != (Object)null)
					{
						AudioManager.me.PlaySound2D(dollar.PickupSound, Random.Range(0.8f, 1.2f), 0.8f);
					}
				}
			}
		}
		else
		{
			string cardId = (CurrentBoard.BoardOptions.UsesShells ? "shell" : "gold");
			gameCard = CreateCardStack(pos, Mathf.CeilToInt(multiplier * (float)stackValue.TotalValue), cardId, checkAddToStack);
			if ((Object)(object)gameCard != (Object)null)
			{
				QuestManager.instance.SpecialActionComplete("sell_card", gameCard.CardData);
				AudioManager.me.PlaySound2D(AudioManager.me.Coin, Random.Range(0.8f, 1.2f), 0.8f);
			}
		}
		return gameCard;
	}

	public void DestroyStack(GameCard card)
	{
		GameCard gameCard = card;
		while ((Object)(object)gameCard != (Object)null)
		{
			gameCard.Destroyed = true;
			AllCards.Remove(card);
			UniqueIdToCard.Remove(card.CardData.UniqueId);
			Object.Destroy((Object)(object)((Component)gameCard).gameObject);
			gameCard = gameCard.Child;
		}
	}

	public Vector3 GetRandomSpawnPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = CurrentBoard.WorldBounds;
		float num = Mathf.Lerp(((Bounds)(ref worldBounds)).min.x, ((Bounds)(ref worldBounds)).max.x, Random.Range(0.1f, 0.9f));
		float num2 = Mathf.Lerp(((Bounds)(ref worldBounds)).min.z, ((Bounds)(ref worldBounds)).max.z, Random.Range(0.1f, 0.9f));
		return new Vector3(num, 0f, num2);
	}

	private void DebugUpdate()
	{
		bool flag = false;
		if (Application.isEditor && InputController.instance.GetKeyDown((Key)94))
		{
			flag = true;
		}
		if (!Application.isEditor && InputController.instance.GetKeyDown((Key)94) && InputController.instance.GetKey((Key)25) && InputController.instance.GetKey((Key)29))
		{
			flag = true;
		}
		if (flag)
		{
			GameScreen gameScreen = GameScreen.instance;
			if (gameScreen != null)
			{
				Image debugScreen = gameScreen.DebugScreen;
				if (debugScreen != null)
				{
					((Component)debugScreen).gameObject.SetActive(!((Component)GameScreen.instance.DebugScreen).gameObject.activeInHierarchy);
				}
			}
			DebugScreenOpened = true;
		}
		if (DebugScreenOpened && InputController.instance.GetKeyDown((Key)98))
		{
			((Component)GameCanvas.instance).gameObject.SetActive(!((Component)GameCanvas.instance).gameObject.activeInHierarchy);
		}
	}

	public void CheckStackOrders()
	{
		if (validator == null)
		{
			validator = new GameDataValidator(GameDataLoader);
		}
		validator.CheckStackOrders();
	}

	public void SpawnAndDestroyEveryCard()
	{
		((MonoBehaviour)this).StartCoroutine(SpawnAndDestroyCards());
	}

	private IEnumerator SpawnAndDestroyCards()
	{
		foreach (CardData cardDataPrefab in CardDataPrefabs)
		{
			CardData card = CreateCard(GetRandomSpawnPosition(), cardDataPrefab, faceUp: true);
			yield return null;
			card.MyGameCard.DestroyCard();
		}
	}

	public void GoToBoard(GameBoard newBoard, Action onComplete = null, string transitionId = "default")
	{
		if (newBoard.BoardOptions.IsSpiritWorld || CurrentBoard.BoardOptions.IsSpiritWorld)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.SpiritTransitionEnter, 1f, 0.5f);
		}
		else if (newBoard.Id == "cities" || CurrentBoard.Id == "cities")
		{
			AudioManager.me.PlaySound2D(AudioManager.me.CitiesTransitionEnter, 1f, 0.5f);
		}
		TransitionScreen.instance.StartTransition(delegate
		{
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)DraggingDraggable != (Object)null)
			{
				DraggingDraggable.StopDragging();
				SetDraggingDraggableToNull();
			}
			GameBoard currentBoard = CurrentBoard;
			CurrentRunVariables.PreviouseBoard = currentBoard.Id;
			CurrentBoard = newBoard;
			QuestManager.instance.SpecialActionComplete("board_" + CurrentBoard.Id);
			if (CurrentBoard.Id == "island")
			{
				if (!CurrentRunVariables.VisitedIsland)
				{
					QueueCutscene(Cutscenes.IslandIntro());
				}
				CurrentRunVariables.VisitedIsland = true;
				CheckSpawnIslandBooster();
			}
			if (CurrentBoard.Id == "cities" && !CurrentRunVariables.HasCitiesBoard)
			{
				CreateBoosterIfNotExists(CurrentBoard.MiddleOfBoard(), "cities_intro");
				CurrentRunVariables.HasCitiesBoard = true;
				CitiesManager.instance.Wellbeing = CitiesManager.instance.WellbeingStart;
			}
			if (CurrentBoard.Id == "greed")
			{
				CreateBoosterIfNotExists(CurrentBoard.MiddleOfBoard(), "greed_intro");
			}
			if (CurrentBoard.Id == "happiness")
			{
				CreateBoosterIfNotExists(CurrentBoard.MiddleOfBoard(), "happiness_intro");
			}
			if (CurrentBoard.Id == "death")
			{
				CreateBoosterIfNotExists(CurrentBoard.MiddleOfBoard(), "death_intro");
			}
			onComplete?.Invoke();
			if (newBoard.BoardOptions.IsSpiritWorld || CurrentBoard.BoardOptions.IsSpiritWorld)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.SpiritTransitionExit, 1f, 0.5f);
			}
			else if (newBoard.Id == "cities" || CurrentBoard.Id == "cities")
			{
				AudioManager.me.PlaySound2D(AudioManager.me.CitiesTransitionExit, 1f, 0.5f);
			}
			GameScreen.instance.OnBoardChange();
			GameScreen.instance.UpdateQuestLog();
			GameScreen.instance.UpdateIdeasLog();
			GameScreen.instance.SetQuestTab();
			CurrentRunVariables.LastGoToBoardMonth = CurrentMonth;
			if (currentBoard.Id != "forest" && CurrentBoard.Id != "forest")
			{
				MonthTimer = 0f;
			}
			SpeedUp = 1f;
			if (currentBoard.Id == "cities" && !HasFoundCard("blueprint_road_builder"))
			{
				CreateCard(CurrentBoard.MiddleOfBoard(), "blueprint_road_builder");
			}
			GameCamera.instance.CenterOnBoard(CurrentBoard);
			QuestManager.instance.CheckPacksUnlocked();
			SetViewType(ViewType.Default);
			SaveManager.instance.Save(saveRound: true);
		}, transitionId, 2f);
	}

	private void CreateBoosterIfNotExists(Vector3 pos, string boosterId)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (AllBoosters.Count((Boosterpack x) => x.BoosterId == boosterId && x.MyBoard.IsCurrent) <= 0)
		{
			CreateBoosterpack(pos, boosterId);
		}
	}

	private void CheckSpawnIslandBooster()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!(CurrentBoard.Id != "island") && GetCardCount() == 0 && AllBoosters.Count((Boosterpack x) => x.MyBoard.Id == "island") == 0)
		{
			CreateBoosterpack(CurrentBoard.NormalizedPosToWorldPos(new Vector2(0.6f, 0.5f)), "island1");
		}
	}

	public void SendStackToBoard(GameCard rootCard, GameBoard newBoard, Vector2 normalizedPos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		rootCard = rootCard.GetRootCard();
		SendToBoard(rootCard, newBoard, normalizedPos);
	}

	public void SendToBoard(GameCard rootCard, GameBoard newBoard, Vector2 normalizedPos)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		rootCard.MyBoard = newBoard;
		foreach (GameCard childCard in rootCard.GetChildCards())
		{
			childCard.MyBoard = newBoard;
		}
		foreach (GameCard item in rootCard.GetAllCardsInStack())
		{
			foreach (GameCard equipmentChild in item.EquipmentChildren)
			{
				equipmentChild.MyBoard = newBoard;
			}
		}
		((Component)rootCard).transform.position = (rootCard.TargetPosition = newBoard.NormalizedPosToWorldPos(normalizedPos));
		rootCard.UpdateChildPositions(hardSetPos: true);
	}

	public void Restack(List<GameCard> cards)
	{
		foreach (GameCard card in cards)
		{
			card.RemoveFromStack();
		}
		for (int i = 0; i < cards.Count; i++)
		{
			if (i > 0)
			{
				cards[i].SetParent(cards[i - 1]);
			}
		}
	}

	public bool CheckIfCanAddOnStack(GameCard topCard)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		List<GameCard> overlappingCards = topCard.GetOverlappingCards();
		float num = float.MaxValue;
		GameCard gameCard = null;
		foreach (GameCard item in overlappingCards)
		{
			if ((Object)(object)item == (Object)(object)topCard || item.IsChildOf(topCard))
			{
				continue;
			}
			bool num2 = (Object)(object)topCard == (Object)(object)item.removedChild;
			GameCard leafCard = item.GetLeafCard();
			if (!num2)
			{
				GameCard cardWithStatusInStack = leafCard.GetCardWithStatusInStack();
				if ((Object)(object)cardWithStatusInStack != (Object)null && !cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus())
				{
					continue;
				}
			}
			if (leafCard.CardData.CanHaveCardOnTop(topCard.CardData))
			{
				Vector3 val = ((Component)topCard).transform.position - ((Component)item).transform.position;
				val.y = 0f;
				if (((Vector3)(ref val)).magnitude < num)
				{
					gameCard = leafCard;
					num = ((Vector3)(ref val)).magnitude;
				}
			}
		}
		if ((Object)(object)gameCard != (Object)null)
		{
			topCard.SetParent(gameCard);
			return true;
		}
		return false;
	}

	public GameBoard GetCurrentBoardSafe()
	{
		if ((Object)(object)CurrentBoard != (Object)null)
		{
			return CurrentBoard;
		}
		if (IsCitiesDlcActive())
		{
			return GetBoardWithId("cities");
		}
		if (IsSpiritDlcActive())
		{
			return GetBoardWithId("death");
		}
		return GetBoardWithId("main");
	}

	public int GetCardCount(string id)
	{
		return GetCardCount(id, CurrentBoard);
	}

	public int GetCardCount(string id, GameBoard board)
	{
		int num = 0;
		for (int num2 = AllCards.Count - 1; num2 >= 0; num2--)
		{
			GameCard gameCard = AllCards[num2];
			if (!((Object)(object)gameCard.MyBoard != (Object)(object)board) && gameCard.CardData.Id == id)
			{
				num++;
			}
		}
		return num;
	}

	public int GetCardCountWithChest(string id)
	{
		return GetCardCountWithChest(id, CurrentBoard);
	}

	public int GetCardCountWithChest(string id, GameBoard board)
	{
		int num = 0;
		foreach (GameCard allCard in AllCards)
		{
			if (!((Object)(object)allCard.MyBoard != (Object)(object)board))
			{
				if (allCard.CardData.Id == id)
				{
					num++;
				}
				if (allCard.CardData is ResourceChest resourceChest && resourceChest.HeldCardId == id)
				{
					num += resourceChest.ResourceCount;
				}
			}
		}
		return num;
	}

	public int GetCardCount(Predicate<CardData> pred)
	{
		int num = 0;
		foreach (GameCard allCard in AllCards)
		{
			if (!((Object)(object)allCard.MyBoard != (Object)(object)CurrentBoard) && pred(allCard.CardData))
			{
				num++;
			}
		}
		return num;
	}

	public int GetCardCountInStack(GameCard card, Predicate<CardData> pred)
	{
		int num = 0;
		foreach (GameCard item in card.GetAllCardsInStack())
		{
			if (pred(item.CardData))
			{
				num++;
			}
		}
		return num;
	}

	public CardData GetCardPrefab(string id, bool showError = true)
	{
		return GameDataLoader.GetCardFromId(id, showError);
	}

	public T GetCardPrefab<T>(string id, bool showError = true) where T : CardData
	{
		CardData cardFromId = GameDataLoader.GetCardFromId(id, showError);
		if (!(cardFromId is T))
		{
			Debug.LogError((object)$"Card {id} is not of type {typeof(T)}");
			return null;
		}
		return cardFromId as T;
	}

	public BoosterpackData GetBoosterData(string boosterId)
	{
		return GameDataLoader.GetBoosterData(boosterId);
	}

	public Boosterpack CreateBoosterpack(Vector3 position, string boosterId)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Boosterpack boosterpack = Object.Instantiate<Boosterpack>(PrefabManager.instance.BoosterpackPrefab);
		BoosterpackData boosterpackData = (boosterpack.PackData = Object.Instantiate<BoosterpackData>(GetBoosterData(boosterId)));
		((Component)boosterpack).transform.position = position;
		boosterpack.MyBoard = CurrentBoard;
		if (!CurrentSave.FoundBoosterIds.Contains(boosterId))
		{
			CurrentSave.FoundBoosterIds.Add(boosterId);
		}
		foreach (BoosterAddition boosterAddition in boosterpackData.BoosterAdditions)
		{
			if (boosterAddition.Filter.IsMet())
			{
				boosterpackData.CardBags.AddRange(boosterAddition.CardBags);
			}
		}
		boosterpack.TotalCardsInPack = boosterpackData.CardBags.Sum((CardBag x) => x.CardsInPack);
		return boosterpack;
	}

	public void StackSend(GameCard myCard, Vector3 outputDirection, GameCard initialParent = null, bool sendToChest = true)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		if (TrySendToMagnet(myCard) || (sendToChest && TrySendToChest(myCard)) || (Object)(object)myCard.BounceTarget != (Object)null)
		{
			return;
		}
		GameCard gameCard = null;
		float num = float.MaxValue;
		Vector3 value = Vector3.zero;
		foreach (GameCard allCard in AllCards)
		{
			if (!allCard.MyBoard.IsCurrent || (Object)(object)allCard == (Object)(object)myCard)
			{
				continue;
			}
			GameCard cardWithStatusInStack = allCard.GetCardWithStatusInStack();
			if ((!((Object)(object)cardWithStatusInStack != (Object)null) || cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus()) && !((Object)(object)allCard.GetCardInCombatInStack() != (Object)null) && !allCard.BeingDragged && !allCard.IsChildOf(myCard) && !allCard.IsParentOf(myCard) && (!((Object)(object)initialParent != (Object)null) || (!allCard.IsChildOf(initialParent) && !((Object)(object)allCard == (Object)(object)initialParent))) && !allCard.HasChild && allCard.CardData.CanHaveCardOnTop(myCard.CardData) && allCard.CardData.Id == myCard.CardData.Id)
			{
				Vector3 val = ((Component)allCard).transform.position - ((Component)myCard).transform.position;
				val.y = 0f;
				if (((Vector3)(ref val)).magnitude <= 2f && ((Vector3)(ref val)).magnitude <= num)
				{
					gameCard = allCard;
					num = ((Vector3)(ref val)).magnitude;
					value = new Vector3(val.x * 4f, 7f, val.z * 4f);
				}
			}
		}
		if ((Object)(object)gameCard != (Object)null)
		{
			myCard.BounceTarget = gameCard;
			myCard.Velocity = value;
		}
		else
		{
			myCard.SendIt();
		}
	}

	public void StackSendCheckTarget(GameCard origin, GameCard myCard, Vector3 outputDirection, GameCard initialParent = null, bool sendToChest = true, int outputIndex = -1)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!TrySendWithPipe(myCard, origin, outputIndex) && !TrySendToMagnet(myCard) && !((Object)(object)myCard.BounceTarget != (Object)null))
		{
			StackSend(myCard, outputDirection, initialParent, sendToChest);
		}
	}

	public GameCard GetTargetCard(GameCard origin, CardData inputCardDataPrefab, Vector3 direction, bool allowDraggedCards, GameCard inputCard = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return GetBestCardInDirection(origin, direction, allowDraggedCards, delegate(GameCard gameCard)
		{
			if ((Object)(object)inputCard != (Object)null && (Object)(object)gameCard == (Object)(object)inputCard)
			{
				return false;
			}
			if ((Object)(object)gameCard == (Object)(object)inputCardDataPrefab)
			{
				return false;
			}
			if (!OutputCardAllowed(gameCard, inputCardDataPrefab))
			{
				return false;
			}
			if ((Object)(object)inputCardDataPrefab.MyGameCard != (Object)null && (Object)(object)inputCardDataPrefab.MyGameCard == (Object)(object)gameCard)
			{
				return false;
			}
			return !gameCard.IsPartOfSameStack(origin);
		});
	}

	public GameCard GetBestCardInDirection(GameCard origin, Vector3 direction, bool allowDraggedCards, Func<GameCard, bool> pred)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)origin).transform.position;
		float num = float.MinValue;
		GameCard result = null;
		float num2 = float.MaxValue;
		foreach (GameCard item in AllCards.Where((GameCard x) => (Object)(object)x.MyBoard == (Object)(object)CurrentBoard))
		{
			if ((Object)(object)item == (Object)(object)origin || (!allowDraggedCards && item.BeingDragged) || !pred(item))
			{
				continue;
			}
			Vector3 val = ((Component)item).transform.position - position;
			float num3 = Vector3.Dot(direction, val);
			if (num3 <= 0f)
			{
				continue;
			}
			float num4 = num3 / ((Vector3)(ref val)).sqrMagnitude;
			if (num4 > 0.5f && num4 > num)
			{
				num = num4;
				Vector3 val2 = ((Component)item).transform.position - position;
				val2.y = 0f;
				if (((Vector3)(ref val2)).magnitude <= 2f && ((Vector3)(ref val2)).magnitude <= num2)
				{
					result = item;
					num2 = ((Vector3)(ref val2)).magnitude;
				}
			}
		}
		return result;
	}

	private bool OutputCardAllowed(GameCard gameCard, CardData inputCardDataPrefab)
	{
		if (gameCard.CardData.Id == "heavy_foundation")
		{
			return false;
		}
		if (gameCard.Velocity.HasValue || (Object)(object)gameCard.BounceTarget != (Object)null)
		{
			return false;
		}
		if (gameCard.HasChild)
		{
			return false;
		}
		if (!((Component)gameCard).gameObject.activeInHierarchy)
		{
			return false;
		}
		if ((Object)(object)gameCard.MyBoard == (Object)null)
		{
			Debug.Log((object)(((object)gameCard)?.ToString() + " does not have a board"));
			return false;
		}
		if (!gameCard.MyBoard.IsCurrent)
		{
			return false;
		}
		try
		{
			if (!gameCard.CardData.CanHaveCardOnTop(inputCardDataPrefab, isPrefab: true))
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			if (Application.isEditor)
			{
				Debug.LogError((object)ex);
			}
			return false;
		}
		return true;
	}

	public void StackSendTo(GameCard myCard, GameCard target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)target).transform.position - ((Component)myCard).transform.position;
		val.y = 0f;
		Vector3 value = default(Vector3);
		((Vector3)(ref value))._002Ector(val.x * 4f, 7f, val.z * 4f);
		if (target.GetChildCount() > 0)
		{
			target = target.GetChildCards().Last();
		}
		if ((Object)(object)target != (Object)null && target.CardData.CanHaveCardOnTop(myCard.CardData))
		{
			myCard.BounceTarget = target.GetRootCard();
			myCard.Velocity = value;
		}
		else
		{
			myCard.SendIt();
		}
	}

	public CardData CreateCard(Vector3 position, ICardId cardId, bool faceUp = true, bool checkAddToStack = true, bool playSound = true)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (cardId is CardIdWithEquipment cardIdWithEquipment)
		{
			Combatable combatable = (Combatable)CreateCard(position, cardId.Id, faceUp, checkAddToStack, playSound);
			{
				foreach (string item in cardIdWithEquipment.Equipment)
				{
					combatable.CreateAndEquipCard(item, markAsFound: false);
				}
				return combatable;
			}
		}
		return CreateCard(position, cardId.Id, faceUp, checkAddToStack, playSound);
	}

	public CardData CreateCard(Vector3 position, string cardId, bool faceUp = true, bool checkAddToStack = true, bool playSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		CardData cardPrefab = GetCardPrefab(cardId);
		Vector2 val = Random.insideUnitCircle * 0.001f;
		position += new Vector3(val.x, 0f, val.y);
		return CreateCard(position, cardPrefab, faceUp, checkAddToStack, playSound);
	}

	private string GetUniqueId()
	{
		return Guid.NewGuid().ToString().Substring(0, 12);
	}

	public bool HasFoundCard(string cardId)
	{
		return CurrentSave.FoundCardIds.Contains(cardId);
	}

	public void FoundCard(CardData card)
	{
		if (!CurrentSave.FoundCardIds.Contains(card.Id))
		{
			if (card.MyCardType == CardType.Ideas || card.MyCardType == CardType.Rumors)
			{
				CurrentSave.NewKnowledgeIds.Add(card.Id);
			}
			CurrentSave.NewCardopediaIds.Add(card.Id);
			CurrentSave.FoundCardIds.Add(card.Id);
			NewCardsFound++;
			UpdateCardTargets();
			card.MyGameCard.IsNew = true;
		}
	}

	public void DebugUnlockIdeas(bool justBasegame)
	{
		foreach (CardData item in CardDataPrefabs.Where((CardData x) => x.MyCardType == CardType.Ideas && !x.HideFromCardopedia).ToList())
		{
			if ((!justBasegame || item.CardUpdateType != CardUpdateType.Spirit) && !HasFoundCard(item.Id))
			{
				if (item.MyCardType == CardType.Ideas || item.MyCardType == CardType.Rumors)
				{
					CurrentSave.NewKnowledgeIds.Add(item.Id);
				}
				CurrentSave.NewCardopediaIds.Add(item.Id);
				CurrentSave.FoundCardIds.Add(item.Id);
				NewCardsFound++;
			}
		}
	}

	public CardData ChangeToCard(GameCard card, string cardId)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		CardData cardPrefab = GetCardPrefab(cardId);
		CardData cardData = card.CardData;
		CardData cardData2 = Object.Instantiate<CardData>(cardPrefab);
		cardData2.StatusEffects = cardData.StatusEffects;
		foreach (StatusEffect statusEffect in cardData2.StatusEffects)
		{
			statusEffect.ParentCard = cardData2;
		}
		card.StatusEffectsChanged();
		cardData2.SetExtraCardData(card.CardData.GetExtraCardData());
		cardData2.UniqueId = card.CardData.UniqueId;
		((Component)cardData2).transform.SetParent(((Component)card).transform);
		((Component)cardData2).transform.localPosition = Vector3.zero;
		cardData2.MyGameCard = card;
		card.CardData = cardData2;
		if (cardData.IsFoil)
		{
			cardData2.SetFoil();
		}
		((Object)((Component)card).gameObject).name = ((Object)((Component)cardPrefab).gameObject).name;
		if (!IsLoadingSaveRound)
		{
			QuestManager.instance.CardCreated(cardData2);
		}
		cardData2.MyGameCard.IsNew = false;
		FoundCard(cardData2);
		if ((Object)(object)GameScreen.instance != (Object)null && (cardData2.MyCardType == CardType.Ideas || cardData2.MyCardType == CardType.Rumors))
		{
			GameScreen.instance.UpdateIdeasLog();
		}
		if (cardData is Combatable combatable && cardData2 is Combatable combatable2)
		{
			if (combatable.InConflict)
			{
				combatable.MyConflict.SwapParticipant(combatable, combatable2);
			}
			combatable2.HealthPoints = Mathf.Min(combatable2.HealthPoints, combatable2.ProcessedCombatStats.MaxHealth);
		}
		Object.Destroy((Object)(object)((Component)cardData).gameObject);
		card.UpdateIcon();
		card.UpdateCardPalette();
		CreateSmoke(((Component)card).transform.position + Vector3.up * 0.05f);
		return cardData2;
	}

	public CardData CreateCard(Vector3 position, CardData cardDataPrefab, bool faceUp, bool checkAddToStack = true, bool playSound = true, bool markAsFound = true)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cardDataPrefab == (Object)null)
		{
			return null;
		}
		if (playSound)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.CardCreate, 1f, 0.1f);
		}
		GameCard gameCard = Object.Instantiate<GameCard>(PrefabManager.instance.GameCardPrefab);
		((Component)gameCard).transform.position = position;
		gameCard.MyBoard = CurrentBoard;
		CardData cardData = Object.Instantiate<CardData>(cardDataPrefab);
		((Component)cardData).transform.SetParent(((Component)gameCard).transform);
		((Component)cardData).transform.localPosition = Vector3.zero;
		cardData.CreationMonth = CurrentMonth;
		cardData.UniqueId = GetUniqueId();
		gameCard.CardData = cardData;
		cardData.MyGameCard = gameCard;
		((Object)((Component)gameCard).gameObject).name = ((Object)((Component)cardDataPrefab).gameObject).name;
		gameCard.SetFaceUp(faceUp);
		if (checkAddToStack)
		{
			CheckIfCanAddOnStack(gameCard);
		}
		((Component)gameCard).transform.position = position;
		AllCards.Add(gameCard);
		if (!IsLoadingSaveRound)
		{
			UniqueIdToCard[cardData.UniqueId] = gameCard;
		}
		if (gameCard.CardData is Curse item)
		{
			ActiveCurses.Add(item);
		}
		if (!IsLoadingSaveRound)
		{
			QuestManager.instance.CardCreated(cardData);
		}
		if (markAsFound)
		{
			FoundCard(cardData);
		}
		if ((Object)(object)GameScreen.instance != (Object)null && (cardData.MyCardType == CardType.Ideas || cardData.MyCardType == CardType.Rumors))
		{
			GameScreen.instance.UpdateIdeasLog();
		}
		if (cardData.WorkerAmount > 0)
		{
			gameCard.WorkerTransformHolder.UpdateWorkerAmount(cardData.WorkerAmount);
		}
		if (cardData.EnergyConnectors.Count > 0)
		{
			gameCard.CreateCardConnectors();
		}
		if (!IsLoadingSaveRound)
		{
			TrySendToMagnet(gameCard);
			cardData.OnInitialCreate();
		}
		((Component)cardData).gameObject.SetActive(true);
		return cardData;
	}

	public CardData GetNearestCardMatchingPred(GameCard card, Predicate<GameCard> pred)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		CardData result = null;
		float num = float.MaxValue;
		foreach (GameCard item in from x in AllCards.FindAll(pred)
			where instance.CurrentBoard.Id == x.MyBoard.Id
			select x)
		{
			Vector3 val = ((Component)item).transform.position - ((Component)card).transform.position;
			val.y = 0f;
			if (((Vector3)(ref val)).sqrMagnitude < num)
			{
				num = ((Vector3)(ref val)).sqrMagnitude;
				result = item.CardData;
			}
		}
		return result;
	}

	public bool TrySendWithPipe(GameCard card, GameCard origin, int outputIndex = -1)
	{
		if (origin.CardConnectorChildren.Any((CardConnector x) => x.ConnectionType == ConnectionType.Transport && x.CardDirection == CardDirection.output && (Object)(object)x.ConnectedNode != (Object)null))
		{
			List<CardConnector> list = origin.CardConnectorChildren.FindAll((CardConnector x) => x.ConnectionType == ConnectionType.Transport && x.CardDirection == CardDirection.output && (Object)(object)x.ConnectedNode != (Object)null);
			CardConnector cardConnector = null;
			if (outputIndex >= 0 && outputIndex < list.Count)
			{
				cardConnector = list[outputIndex];
			}
			if ((Object)(object)cardConnector == (Object)null)
			{
				for (int num = 0; num < list.Count; num++)
				{
					int index = (origin.ConnectorOutputIndex + num) % list.Count;
					cardConnector = list[index];
					if ((Object)(object)cardConnector != (Object)null && (Object)(object)cardConnector.ConnectedNode != (Object)null)
					{
						break;
					}
				}
			}
			if ((Object)(object)cardConnector != (Object)null)
			{
				origin.ConnectorOutputIndex = list.IndexOf(cardConnector) + 1;
				GameCard parent = cardConnector.ConnectedNode.Parent;
				StackSendTo(card, parent.GetLeafCard());
				return true;
			}
		}
		return false;
	}

	public bool TrySendToMagnet(GameCard card)
	{
		CardData nearestCardMatchingPred = GetNearestCardMatchingPred(card, (GameCard x) => x.CardData is ResourceMagnet resourceMagnet && resourceMagnet.PullCardId == card.CardData.Id && (Object)(object)x.MyBoard == (Object)(object)card.MyBoard && resourceMagnet.MyGameCard.GetStackCount() < 30);
		if ((Object)(object)nearestCardMatchingPred == (Object)null)
		{
			return false;
		}
		StackSendTo(card, nearestCardMatchingPred.MyGameCard.GetLeafCard());
		QuestManager.instance.SpecialActionComplete("use_magnet");
		return true;
	}

	public bool TrySendToChest(GameCard card)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		GameCard gameCard = null;
		float num = float.MaxValue;
		List<GameCard> list = new List<GameCard>();
		list = ((!(card.CardData.Id == "gold") && !(card.CardData.Id == "shell")) ? AllCards.FindAll((GameCard x) => x.CardData is ResourceChest resourceChest && resourceChest.HeldCardId == card.CardData.Id && resourceChest.ResourceCount < resourceChest.MaxResourceCount) : AllCards.FindAll((GameCard x) => x.CardData is Chest chest && chest.HeldCardId == card.CardData.Id && chest.CoinCount < chest.MaxCoinCount));
		foreach (GameCard item in list)
		{
			Vector3 val = ((Component)item).transform.position - ((Component)card).transform.position;
			val.y = 0f;
			if (((Vector3)(ref val)).magnitude <= 2f && ((Vector3)(ref val)).magnitude <= num)
			{
				gameCard = item;
				num = ((Vector3)(ref val)).magnitude;
			}
		}
		if ((Object)(object)gameCard != (Object)null)
		{
			StackSendTo(card, gameCard.GetLeafCard());
			return true;
		}
		return false;
	}

	public CardValue GetStackValue(GameCard card)
	{
		CardValue cardValue = new CardValue(card.CardData.GetValue());
		if (card.IsPartOfStack())
		{
			foreach (GameCard item in card.GetAllCardsInStack())
			{
				if ((Object)(object)item != (Object)(object)card)
				{
					cardValue.BaseValue += item.CardData.GetValue();
				}
			}
		}
		cardValue.BaseValue = Mathf.Max(0, cardValue.BaseValue);
		return cardValue;
	}

	public bool StackAllSame(GameCard card)
	{
		List<GameCard> allCardsInStack = card.GetAllCardsInStack();
		return AllCardsSame(allCardsInStack);
	}

	public bool AllCardsSame(List<GameCard> cards)
	{
		return cards.Select((GameCard x) => x.CardData.Id).Distinct().Count() == 1;
	}

	public bool AllCardsInStackMatchPred(GameCard card, Predicate<GameCard> pred)
	{
		return card.GetAllCardsInStack().All((GameCard x) => pred(x));
	}

	private bool CountsTowardCardCount(GameCard card)
	{
		CardData cardData = card.CardData;
		if (cardData is Poop && CurseIsActive(CurseType.Death))
		{
			return false;
		}
		if (doesntCountTowardsCount.Contains(cardData.Id))
		{
			return false;
		}
		if (!(cardData is Dollar) && !(cardData is Energy) && cardData.MyCardType != CardType.Weather)
		{
			return !(cardData is Worker);
		}
		return false;
	}

	public int GetCardCount()
	{
		int num = 0;
		bool canTravelToIsland = CurrentBoard.BoardOptions.CanTravelToIsland;
		for (int i = 0; i < AllCards.Count; i++)
		{
			GameCard gameCard = AllCards[i];
			if ((Object)(object)gameCard.MyBoard != (Object)(object)CurrentBoard || gameCard.IsEquipped || !CountsTowardCardCount(gameCard) || gameCard.CardData is Food { IsConsumed: not false })
			{
				continue;
			}
			GameCard rootCard = gameCard.GetRootCard();
			if ((!canTravelToIsland || !rootCard.CardData.AnyChildMatchesPredicate((CardData x) => x is Boat boat2 && boat2.InSailOff)) && !(rootCard.CardData is Boat { InSailOff: not false }))
			{
				if (gameCard.CardData is ResourceChest resourceChest)
				{
					num += ((CurrentBoard.Id == "cities") ? 1 : resourceChest.ResourceCount);
				}
				num++;
			}
		}
		return num;
	}

	public int GetMaxCardCount()
	{
		return GetMaxCardCount(CurrentBoard);
	}

	public int GetMaxCardCount(GameBoard board)
	{
		return board.BoardOptions.BaseCardCount + CardCapIncrease(board);
	}

	public int CardCapIncrease(GameBoard board)
	{
		if (board.Id == "cities")
		{
			return GetTownHallIncrease(board);
		}
		int num = 0;
		for (int num2 = AllCards.Count - 1; num2 >= 0; num2--)
		{
			GameCard gameCard = AllCards[num2];
			if (!((Object)(object)gameCard.MyBoard != (Object)(object)board))
			{
				if (gameCard.CardData.Id == "shed")
				{
					num += 4;
				}
				else if (gameCard.CardData.Id == "warehouse")
				{
					num += 14;
				}
				else if (gameCard.CardData.Id == "lighthouse")
				{
					num += 14;
				}
			}
		}
		return num;
	}

	public int GetTownHallIncrease(GameBoard board)
	{
		GetCardsNonAlloc(board, townhalls);
		if (townhalls.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		foreach (CityHall townhall in townhalls)
		{
			num += townhall.DollarAmount;
		}
		return Mathf.FloorToInt((float)(num / CityHall.DollarPerCardcap));
	}

	public int BoardSizeIncrease(GameBoard board)
	{
		return GetCardCount("lighthouse", board) * 10;
	}

	public int GetFoodCount(bool allowDebug = true)
	{
		if (DebugNoFoodEnabled && allowDebug)
		{
			return 99;
		}
		int num = 0;
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.MyBoard.IsCurrent && allCard.CardData is Food food)
			{
				num += food.FoodValue;
			}
		}
		return num;
	}

	public int GetHappinessCount(bool allowDebug = true, bool includeInChest = true)
	{
		if (DebugNoFoodEnabled && allowDebug)
		{
			return 99;
		}
		int num = 0;
		if (includeInChest)
		{
			num = GetCountInChests("happiness");
		}
		return GetCardCount("happiness") + num;
	}

	private int GetCountInChests(string cardId)
	{
		int num = 0;
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.MyBoard.IsCurrent)
			{
				if (allCard.CardData is ResourceChest resourceChest && resourceChest.HeldCardId == cardId)
				{
					num += resourceChest.ResourceCount;
				}
				if (allCard.CardData is Chest chest && chest.HeldCardId == cardId)
				{
					num += chest.CoinCount;
				}
			}
		}
		return num;
	}

	public int GetShellCount(bool includeInChest)
	{
		int num = 0;
		if (includeInChest)
		{
			num = GetCountInChests("shell");
		}
		return GetCardCount<Shell>() + num;
	}

	public int GetGoldCount(bool includeInChest)
	{
		int num = 0;
		if (includeInChest)
		{
			num = GetCountInChests("gold");
		}
		return GetCardCount<Gold>() + num;
	}

	public int GetDollarCount(bool includeInChest)
	{
		int num = 0;
		if (includeInChest)
		{
			num = GetDollarInBank();
		}
		GetCardsNonAlloc(dollars);
		int num2 = 0;
		for (int i = 0; i < dollars.Count; i++)
		{
			num2 += dollars[i].DollarValue;
		}
		return num2 + num;
	}

	public int GetDollarInBank()
	{
		GetCardsNonAlloc(creditcards);
		int num = 0;
		for (int i = 0; i < creditcards.Count; i++)
		{
			num += creditcards[i].DollarCount;
		}
		return num;
	}

	public int GetIdeaCount()
	{
		int num = 0;
		foreach (string foundCardId in CurrentSave.FoundCardIds)
		{
			if (foundCardId.StartsWith("blueprint"))
			{
				num++;
			}
		}
		return num;
	}

	public int GetCardCount<T>(Predicate<T> pred) where T : CardData
	{
		int num = 0;
		GameBoard currentBoard = CurrentBoard;
		for (int num2 = AllCards.Count - 1; num2 >= 0; num2--)
		{
			GameCard gameCard = AllCards[num2];
			if (!((Object)(object)gameCard.MyBoard != (Object)(object)currentBoard) && gameCard.CardData is T obj && (pred == null || pred(obj)))
			{
				num++;
			}
		}
		return num;
	}

	public int GetCardCount<T>() where T : CardData
	{
		return GetCardCount<T>(null);
	}

	public T GetCard<T>() where T : CardData
	{
		for (int i = 0; i < AllCards.Count; i++)
		{
			GameCard gameCard = AllCards[i];
			if (gameCard.MyBoard.IsCurrent && gameCard.CardData is T)
			{
				return (T)gameCard.CardData;
			}
		}
		return null;
	}

	public T GetCard<T>(GameBoard board) where T : CardData
	{
		foreach (GameCard allCard in AllCards)
		{
			if (!(allCard.MyBoard.Id != board.Id) && allCard.CardData is T)
			{
				return (T)allCard.CardData;
			}
		}
		return null;
	}

	public CardData GetCard(string cardId)
	{
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.MyBoard.IsCurrent && allCard.CardData.Id == cardId)
			{
				return allCard.CardData;
			}
		}
		return null;
	}

	public List<GameCard> GetAllCardsOnBoard(string board)
	{
		return AllCards.Where((GameCard card) => card.MyBoard.Id == board).ToList();
	}

	public List<CardData> GetCards(string cardId)
	{
		List<CardData> list = new List<CardData>();
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.MyBoard.IsCurrent && allCard.CardData.Id == cardId)
			{
				list.Add(allCard.CardData);
			}
		}
		return list;
	}

	public List<T> GetCardsImplementingInterface<T>()
	{
		if (!typeof(T).IsInterface)
		{
			throw new ArgumentException();
		}
		List<T> list = new List<T>();
		GameBoard currentBoard = CurrentBoard;
		foreach (GameCard allCard in AllCards)
		{
			if (!((Object)(object)allCard.MyBoard != (Object)(object)currentBoard) && allCard.CardData is T item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<T> GetCardsImplementingInterfaceNonAlloc<T>(List<T> list)
	{
		if (!typeof(T).IsInterface)
		{
			throw new ArgumentException();
		}
		list.Clear();
		GameBoard currentBoard = CurrentBoard;
		for (int num = AllCards.Count - 1; num >= 0; num--)
		{
			GameCard gameCard = AllCards[num];
			if (!((Object)(object)gameCard.MyBoard != (Object)(object)currentBoard) && gameCard.CardData is T item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<T> GetCards<T>() where T : CardData
	{
		List<T> list = new List<T>();
		for (int i = 0; i < AllCards.Count; i++)
		{
			GameCard gameCard = AllCards[i];
			if (gameCard.MyBoard.IsCurrent && gameCard.CardData is T item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void GetCardsNonAlloc<T>(List<T> list) where T : CardData
	{
		list.Clear();
		GameBoard currentBoard = CurrentBoard;
		foreach (GameCard allCard in AllCards)
		{
			if (!((Object)(object)allCard.MyBoard != (Object)(object)currentBoard) && allCard.CardData is T item)
			{
				list.Add(item);
			}
		}
	}

	public List<T> GetCards<T>(GameBoard board) where T : CardData
	{
		List<T> list = new List<T>();
		foreach (GameCard allCard in AllCards)
		{
			if (!(allCard.MyBoard.Id != board.Id) && allCard.CardData is T item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void GetCardsNonAlloc<T>(GameBoard board, List<T> list) where T : CardData
	{
		list.Clear();
		foreach (GameCard allCard in AllCards)
		{
			if (!(allCard.MyBoard.Id != board.Id) && allCard.CardData is T item)
			{
				list.Add(item);
			}
		}
	}

	public List<Boosterpack> GetAllBoostersOnBoard(string board)
	{
		return AllBoosters.Where((Boosterpack booster) => booster.MyBoard.Id == board).ToList();
	}

	public int GetRequiredFoodCount()
	{
		if (DebugNoFoodEnabled)
		{
			return 0;
		}
		int num = 0;
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.MyBoard.IsCurrent)
			{
				num += GetCardRequiredFoodCount(allCard);
			}
		}
		return num;
	}

	public int GetCardRequiredFoodCount(GameCard c)
	{
		if (c.CardData is BaseVillager baseVillager)
		{
			return baseVillager.GetRequiredFoodCount();
		}
		if (c.CardData is Kid)
		{
			if (!(CurrentBoard.Id == "cities"))
			{
				return 1;
			}
			return 0;
		}
		if (c.CardData is Apartment apartment)
		{
			return apartment.RequirementHolders.Sum((RequirementHolder x) => x.CardRequirements.Sum((CardRequirement cardRequirement) => (cardRequirement is CardRequirement_TakeFood cardRequirement_TakeFood) ? cardRequirement_TakeFood.Amount : 0));
		}
		return 0;
	}

	public int GetRequiredHappinessCount()
	{
		if (DebugNoFoodEnabled)
		{
			return 0;
		}
		int num = 0;
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.MyBoard.IsCurrent)
			{
				num += GetCardRequiredHappinessCount(allCard);
			}
		}
		return num;
	}

	public int GetCardRequiredHappinessCount(GameCard c)
	{
		if (c.CardData is BaseVillager)
		{
			return 1;
		}
		if (c.CardData is Kid)
		{
			return 1;
		}
		if (c.CardData is Unhappiness)
		{
			return 1;
		}
		if (c.CardData is ResourceChest { HeldCardId: "unhappiness" } resourceChest)
		{
			return resourceChest.ResourceCount;
		}
		return 0;
	}

	public Blueprint GetBlueprintWithId(string id)
	{
		foreach (Blueprint blueprintPrefab in BlueprintPrefabs)
		{
			if (blueprintPrefab.Id == id)
			{
				return blueprintPrefab;
			}
		}
		return null;
	}

	public string GetStackSummary(List<GameCard> cards)
	{
		List<string> list = cards.Select((GameCard x) => x.CardData.FullName).Distinct().ToList();
		string text = "";
		for (int num = 0; num < list.Count; num++)
		{
			string card = list[num];
			int num2 = cards.Count((GameCard x) => x.CardData.FullName == card);
			text += $"{num2}x {card}";
			if (num < list.Count - 1)
			{
				text += ", ";
			}
		}
		return text;
	}

	private void EndOfMonth(EndOfMonthParameters param = null)
	{
		if (param == null)
		{
			param = new EndOfMonthParameters();
		}
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		CloseOpenInventories();
		if (CurrentBoard.Id == "cities")
		{
			currentAnimationRoutine = ((MonoBehaviour)this).StartCoroutine(EndOfMonthCitiesRoutine(param));
		}
		else
		{
			currentAnimationRoutine = ((MonoBehaviour)this).StartCoroutine(EndOfMonthRoutine(param));
		}
		if (GameScreen.instance.ControllerIsInUI)
		{
			GameScreen.instance.SetControllerInUI(inUI: false);
		}
		SpeedUp = 1f;
		QuestManager.instance.SpecialActionComplete("month_end");
	}

	public IEnumerator FinishDemand(Demand demand, DemandEvent demandEvent)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		CloseOpenInventories();
		yield return FinishDemandRoutine(demand, demandEvent);
		if (GameScreen.instance.ControllerIsInUI)
		{
			GameScreen.instance.SetControllerInUI(inUI: false);
		}
		SpeedUp = 1f;
		QuestManager.instance.SpecialActionComplete("first_demand");
	}

	public void ForceEndOfMoon(EndOfMonthParameters param)
	{
		MonthTimer = 0f;
		IncrementMonth();
		EndOfMonth(param);
	}

	private IEnumerator WaitForContinueClicked(string text)
	{
		ContinueClicked = false;
		ContinueButtonText = text;
		ShowContinueButton = true;
		while (!ContinueClicked)
		{
			yield return null;
		}
		ShowContinueButton = false;
	}

	private IEnumerator EndOfMonthRoutine(EndOfMonthParameters param)
	{
		if (CurrentView != ViewType.Default)
		{
			SetViewType(ViewType.Default);
		}
		foreach (TravellingCart card in GetCards<TravellingCart>())
		{
			card.MyGameCard.DestroyCard(spawnSmoke: true);
		}
		foreach (CardData card2 in GetCards("trained_monkey"))
		{
			ChangeToCard(card2.MyGameCard, "monkey").UpdateCardText();
		}
		AudioManager.me.PlaySound2D(AudioManager.me.EndOfMoon, 0.8f, 0.2f);
		if (param.CutsceneTitle == null)
		{
			CutsceneTitle = SokLoc.Translate("label_end_of_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", (CurrentMonth - 1).ToString()) });
		}
		else
		{
			CutsceneTitle = param.CutsceneTitle;
		}
		if (!DebugNoFoodEnabled)
		{
			VillagersStarvedAtEndOfMoon = false;
			yield return EndOfMonthCutscenes.FeedVillagers();
			if (VillagersStarvedAtEndOfMoon)
			{
				yield break;
			}
			if (CurseIsActive(CurseType.Happiness))
			{
				VillagersAngryAtEndOfMoon = false;
				yield return EndOfMonthCutscenes.UseHappiness();
				if (VillagersAngryAtEndOfMoon)
				{
					yield break;
				}
			}
			else
			{
				CurrentRunVariables.VillagersUnhappyMonthCount = 0;
			}
		}
		if (CurseIsActive(CurseType.Death))
		{
			CardData fountain = GetCard("fountain_of_youth");
			if ((Object)(object)fountain != (Object)null)
			{
				CutsceneTitle = SokLoc.Translate("label_fountain_title");
				CutsceneText = SokLoc.Translate("label_fountain_text");
				yield return (object)new WaitForSeconds(1f);
				GameCamera.instance.TargetPositionOverride = ((Component)fountain.MyGameCard).transform.position;
				yield return (object)new WaitForSeconds(2f);
			}
			else
			{
				List<BaseVillager> villagersToAge = EndOfMonthCutscenes.GetVillagersToAge();
				if (villagersToAge.Any((BaseVillager x) => x.DetermineLifeStageFromAge(x.Age) == LifeStage.Elderly))
				{
					QuestManager.instance.SpecialActionComplete("villager_old");
				}
				if (EndOfMonthCutscenes.AnyVillagerWillChangeLifeStage(villagersToAge))
				{
					yield return EndOfMonthCutscenes.AgeVillagers(villagersToAge);
				}
				else
				{
					foreach (BaseVillager item in villagersToAge)
					{
						item.Age++;
					}
				}
				if (!VillagersStarvedAtEndOfMoon)
				{
					bool flag = false;
					if (CurrentBoard.Id == "death" && BoardMonths.DeathMonth >= 6)
					{
						flag = true;
					}
					else if ((CurrentBoard.Id == "main" || CurrentBoard.Id == "island") && CurrentMonth > 6)
					{
						flag = true;
					}
					if (flag)
					{
						yield return EndOfMonthCutscenes.CheckMakeSick();
					}
					if (BoardMonths.DeathMonth == 4 && CurrentBoard.Id == "death")
					{
						yield return EndOfMonthCutscenes.NewVillagerSpawnsInDeath();
					}
				}
			}
			if (!VillagersStarvedAtEndOfMoon)
			{
				List<Animal> cards = GetCards<Animal>();
				foreach (Animal item2 in cards)
				{
					if (CurrentMonth - item2.CreationMonth >= 3)
					{
						item2.IsOld = true;
					}
					else
					{
						item2.IsOld = false;
					}
				}
				if (EndOfMonthCutscenes.AnyAnimalWillDie(cards))
				{
					yield return EndOfMonthCutscenes.KillAnimals((from x in GetCards<Animal>()
						where CurrentMonth - x.CreationMonth >= 5
						select x).ToList());
				}
			}
		}
		yield return (object)new WaitForSeconds(1.5f);
		yield return EndOfMonthCutscenes.MaxCardCount();
		if (CurseIsActive(CurseType.Greed))
		{
			yield return DemandManager.instance.CheckDemands(CurrentMonth);
		}
		if (IsCitiesDlcActive() && CurrentBoard.Id == "main" && GetCards<Food>().Sum((Food x) => x.FoodValue) >= 25 && (Object)(object)GetCard("event_industrial_revolution") == (Object)null)
		{
			yield return EndOfMonthCutscenes.IndustrialRevolutionEvent();
		}
		if (param.CutsceneTitle == null)
		{
			CutsceneTitle = SokLoc.Translate("label_start_of_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", CurrentMonth.ToString()) });
		}
		else
		{
			CutsceneTitle = param.CutsceneTitle;
		}
		if (!param.SkipSpecialEvents)
		{
			yield return EndOfMonthCutscenes.SpecialEvents();
		}
		if (param.CutsceneTitle == null)
		{
			CutsceneTitle = SokLoc.Translate("label_start_of_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", CurrentMonth.ToString()) });
		}
		else
		{
			CutsceneTitle = param.CutsceneTitle;
		}
		CutsceneText = "";
		if (!param.SkipEndConfirmation)
		{
			yield return WaitForContinueClicked(SokLoc.Translate("label_start_new_moon"));
		}
		GameCanvas.instance.SetScreen<GameScreen>();
		if (param.OnDone != null)
		{
			param.OnDone?.Invoke();
		}
		GameCamera.instance.TargetPositionOverride = null;
		currentAnimationRoutine = null;
		SaveManager.instance.Save(saveRound: true);
		if ((Object)(object)DebugScreen.instance != (Object)null)
		{
			DebugScreen.instance.AutoSave();
		}
	}

	public IEnumerator EndOfMonthCitiesRoutine(EndOfMonthParameters param)
	{
		AudioManager.me.PlaySound2D(AudioManager.me.EndOfMoon, 0.8f, 0.2f);
		if (CurrentView != ViewType.Default)
		{
			SetViewType(ViewType.Default);
		}
		if (param.CutsceneTitle == null)
		{
			CutsceneTitle = SokLoc.Translate("label_end_of_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", (CurrentMonth - 1).ToString()) });
		}
		else
		{
			CutsceneTitle = param.CutsceneTitle;
		}
		CutsceneScreen.instance.EnableWellbeingBar(CitiesManager.instance.Wellbeing);
		yield return (object)new WaitForSeconds(1f);
		List<CardData> requirementsCards = (from x in GetCards<CardData>()
			where x.RequirementHolders != null && x.RequirementHolders.Count > 0 && (!((Object)(object)x.MyGameCard.GetCardWithStatusInStack() != (Object)null) || !(x.MyGameCard.GetCardWithStatusInStack().TimerActionId == "finish_blueprint"))
			select x).ToList();
		int previousWellbeing = CitiesManager.instance.Wellbeing;
		(from x in GetCards<Enemy>()
			where x.InConflict
			select x).ToList();
		GetCardCount<CitiesCombatable>();
		foreach (CardData requirementCard in requirementsCards)
		{
			foreach (RequirementHolder requirementHolder in requirementCard.RequirementHolders)
			{
				bool flag = true;
				foreach (CardRequirement cardRequirement in requirementHolder.CardRequirements)
				{
					flag = cardRequirement.Satisfied(requirementCard.MyGameCard);
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					foreach (CardRequirementResult positiveResult in requirementHolder.PositiveResults)
					{
						if ((Object)(object)requirementCard.MyGameCard != (Object)null)
						{
							yield return positiveResult.Perform(requirementCard.MyGameCard);
						}
					}
					continue;
				}
				foreach (CardRequirementResult negativeResult in requirementHolder.NegativeResults)
				{
					if ((Object)(object)requirementCard.MyGameCard != (Object)null)
					{
						yield return negativeResult.Perform(requirementCard.MyGameCard);
					}
				}
			}
		}
		List<CardData> rootWithResults = (from x in GetCards<CardData>()
			where x.MonthlyRequirementResult != null
			select x).ToList();
		CutsceneScreen.instance.WellbeingAmount = CitiesManager.instance.Wellbeing;
		if (CitiesManager.instance.Wellbeing - previousWellbeing > 0)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.AddWellbeing, 1f, 0.5f);
		}
		else if (CitiesManager.instance.Wellbeing - previousWellbeing < 0)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.LostWellbeing, 1f, 0.5f);
		}
		if (CitiesManager.instance.Wellbeing - previousWellbeing >= 5)
		{
			QuestManager.instance.SpecialActionComplete("cities_wellbeing_gained_5");
		}
		else if (CitiesManager.instance.Wellbeing - previousWellbeing <= -5)
		{
			QuestManager.instance.SpecialActionComplete("cities_wellbeing_lost_5");
		}
		bool lostGame = false;
		if (CitiesManager.instance.Wellbeing > 0)
		{
			foreach (CardData item in rootWithResults)
			{
				int num = 0;
				foreach (KeyValuePair<string, MonthlyResult> result in item.MonthlyRequirementResult.results)
				{
					if (result.Value.Amount != 0)
					{
						CreateFloatingText(item.MyGameCard, result.Value.Amount > 0, result.Value.Amount, result.Value.Card.CardData.GetRequirementDescription(result.Value.Card, result.Value.CardAmount), GetIconStringFromRequirementType(result.Value.Type), result.Value.Amount > 0, num, 0f);
					}
					num++;
				}
			}
			CutsceneBoardView = true;
			yield return EndOfMonthCutscenes.MaxCardCount();
			CutsceneBoardView = false;
			if (CitiesManager.instance.Wellbeing > 25 && !CurrentRunOptions.IsPeacefulMode && CurrentMonth == CitiesManager.instance.NextConflictMonth && CitiesManager.instance.NextConflictMonth != -1)
			{
				Vector3 randomSpawnPosition = GetRandomSpawnPosition();
				GameCamera.instance.TargetPositionOverride = randomSpawnPosition;
				CutsceneTitle = SokLoc.Translate("label_goblin_conflict_title");
				CutsceneText = SokLoc.Translate("label_goblin_conflict_text");
				CreateCard(randomSpawnPosition, "event_goblin_attack");
				yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
				GameCamera.instance.TargetPositionOverride = null;
			}
			CutsceneBoardView = true;
			CutsceneScreen.instance.CanMoveScreen = true;
			int num2 = CitiesManager.instance.Wellbeing - previousWellbeing;
			CutsceneTitle = SokLoc.Translate("label_end_of_moon_cities_wellbeing");
			string text = Mathf.Abs(num2).ToString();
			if (num2 > 0)
			{
				CutsceneText = SokLoc.Translate("label_end_of_moon_cities_gained", (LocParam[])(object)new LocParam[2]
				{
					LocParam.Create("amount", text),
					LocParam.Create("icon", Icons.Wellbeing)
				});
			}
			else if (num2 == 0)
			{
				CutsceneText = SokLoc.Translate("label_end_of_moon_cities_same", (LocParam[])(object)new LocParam[1] { LocParam.Create("icon", Icons.Wellbeing) });
			}
			else
			{
				CutsceneText = SokLoc.Translate("label_end_of_moon_cities_lost", (LocParam[])(object)new LocParam[2]
				{
					LocParam.Create("amount", text),
					LocParam.Create("icon", Icons.Wellbeing)
				});
			}
			if (num2 != 0)
			{
				CutsceneText = CutsceneText + ", " + SokLoc.Translate("label_hover_status_wellbeing");
			}
			yield return WaitForContinueClicked(SokLoc.Translate((num2 >= 0) ? "label_nice" : "label_uh_oh"));
			if (param.CutsceneTitle == null)
			{
				CutsceneTitle = SokLoc.Translate("label_start_of_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", CurrentMonth.ToString()) });
			}
			else
			{
				CutsceneTitle = param.CutsceneTitle;
			}
			CutsceneText = "";
			if (!param.SkipEndConfirmation)
			{
				yield return WaitForContinueClicked(SokLoc.Translate("label_start_new_moon"));
			}
			GameCamera.instance.TargetPositionOverride = null;
		}
		else
		{
			lostGame = true;
			CutsceneTitle = SokLoc.Translate("label_cities_game_over_title");
			CutsceneText = SokLoc.Translate("label_cities_game_over_text");
			yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
			CutsceneText = SokLoc.Translate("label_cities_game_over_text_1");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			GameCamera.instance.TargetPositionOverride = null;
		}
		CutsceneScreen.instance.CanMoveScreen = false;
		if (!lostGame)
		{
			CloseAllFloatingTextObjects();
			foreach (CardData requirementCard in requirementsCards.Where((CardData x) => (Object)(object)x != (Object)null))
			{
				foreach (RequirementHolder requirementHolder2 in requirementCard.RequirementHolders)
				{
					bool flag2 = true;
					foreach (CardRequirement cardRequirement2 in requirementHolder2.CardRequirements)
					{
						flag2 = cardRequirement2.Satisfied(requirementCard.MyGameCard);
						if (!flag2)
						{
							break;
						}
					}
					if (flag2)
					{
						foreach (CardRequirementResult positiveResult2 in requirementHolder2.PositiveResults)
						{
							yield return positiveResult2.EndOfCutscenePerform(requirementCard.MyGameCard);
						}
						continue;
					}
					foreach (CardRequirementResult negativeResult2 in requirementHolder2.NegativeResults)
					{
						yield return negativeResult2.EndOfCutscenePerform(requirementCard.MyGameCard);
					}
				}
			}
			foreach (CardData item2 in rootWithResults)
			{
				item2.MonthlyRequirementResult = null;
			}
			if (CurrentMonth % 4 == 0)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.EndOfMoon, 0.5f, 0.5f);
				CutsceneTitle = SokLoc.Translate("label_weather_report_title");
				CutsceneText = SokLoc.Translate("label_weather_report_text");
				GameCamera.instance.TargetPositionOverride = MiddleOfBoard();
				yield return (object)new WaitForSeconds(0.5f);
				Boosterpack pack = CreateBoosterpack(MiddleOfBoard(), "cities_weather");
				AudioManager.me.PlaySound2D(AudioManager.me.CardCreate, 1f, 0.5f);
				yield return (object)new WaitForSeconds(0.5f);
				for (int i = 0; i < pack.TotalCardsInPack; i++)
				{
					pack.Clicked();
					yield return (object)new WaitForSeconds(0.2f);
				}
				yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
				GameCamera.instance.TargetPositionOverride = null;
			}
		}
		QuestManager.instance.SpecialActionComplete("cities_wellbeing_changed");
		GameCanvas.instance.SetScreen<GameScreen>();
		if (param.OnDone != null)
		{
			param.OnDone?.Invoke();
		}
		currentAnimationRoutine = null;
		CutsceneBoardView = false;
		GameCamera.instance.TargetPositionOverride = null;
		CutsceneTitle = "";
		CutsceneText = "";
		if (lostGame)
		{
			GameBoard citiesBoard = GetCurrentBoardSafe();
			GoToBoard(GetBoardWithId("main"), delegate
			{
				RemoveAllCardsFromBoard(citiesBoard.Id);
				ResetBoughtBoostersOnLocation(citiesBoard.Location);
				ResetCityVariables();
			}, "cities");
		}
		else
		{
			SaveManager.instance.Save(saveRound: true);
			if ((Object)(object)DebugScreen.instance != (Object)null)
			{
				DebugScreen.instance.AutoSave();
			}
			QueueCutsceneIfNotPlayed("cities_first_moon");
		}
	}

	public string GetIconStringFromRequirementType(RequirementType type)
	{
		return type switch
		{
			RequirementType.Food => Icons.Food, 
			RequirementType.WellBeing => Icons.Wellbeing, 
			RequirementType.Energy => Icons.Energy, 
			RequirementType.Dollar => Icons.Dollar, 
			RequirementType.Pollution => Icons.Pollution, 
			RequirementType.Health => Icons.Health, 
			_ => throw new NotImplementedException("Icon is not implemented"), 
		};
	}

	private IEnumerator FinishDemandRoutine(Demand demand, DemandEvent demandEvent)
	{
		if (demand.Amount == demandEvent.AmountGiven)
		{
			yield return GreedCutscenes.FinishDemandSuccessPreMoon(demand);
			demandEvent.Successful = true;
			CurrentRunVariables.LastDemandMonth = CurrentMonth + 1;
		}
		else if (GetCardCount((CardData x) => x.Id == demand.CardToGet) >= demand.Amount - demandEvent.AmountGiven)
		{
			yield return GreedCutscenes.FinishDemandSuccess(demandEvent);
			demandEvent.Successful = true;
			CurrentRunVariables.LastDemandMonth = CurrentMonth;
		}
		else
		{
			yield return GreedCutscenes.FinishDemandFailed(demand);
			demandEvent.Successful = false;
			CurrentRunVariables.LastDemandMonth = CurrentMonth;
		}
		CurrentRunVariables.PreviousDemandEvents.Add(demandEvent);
		CurrentRunVariables.ActiveDemand = null;
		if (demandEvent.Successful)
		{
			QuestManager.instance.SpecialActionComplete("demand_success");
		}
		else
		{
			QuestManager.instance.SpecialActionComplete("demand_failed");
		}
	}

	private void LateUpdate()
	{
		if (!ShowContinueButton)
		{
			ContinueClicked = false;
		}
	}

	public void KillVillager(Combatable combatable, Action onComplete = null, Action onCreateCorpse = null)
	{
		currentAnimationRoutine = ((MonoBehaviour)this).StartCoroutine(KillVillagerCoroutine(combatable, delegate
		{
			currentAnimationRoutine = null;
			onComplete?.Invoke();
		}, onCreateCorpse));
	}

	public IEnumerator KillVillagerCoroutine(Combatable combatable, Action onComplete, Action onCreateCorpse, bool resetTargetOnDeath = true)
	{
		GameCamera.instance.TargetPositionOverride = ((Component)combatable.MyGameCard).transform.position;
		yield return (object)new WaitForSeconds(1f);
		List<Equipable> allEquipables = combatable.GetAllEquipables();
		List<ExtraCardData> extraCardData = combatable.GetExtraCardData();
		if (combatable.MyGameCard.HasParent && combatable.MyGameCard.HasChild)
		{
			GameCard parent = combatable.MyGameCard.Parent;
			GameCard child = combatable.MyGameCard.Child;
			combatable.MyGameCard.RemoveFromStack();
			child.SetParent(parent);
		}
		combatable.MyGameCard.DestroyCard(spawnSmoke: true);
		if (combatable is Animal)
		{
			combatable.Die();
		}
		else if (combatable.Id != "trained_monkey")
		{
			CreateCard(((Component)combatable.MyGameCard).transform.position, "corpse", faceUp: true, checkAddToStack: false).SetExtraCardData(extraCardData);
			onCreateCorpse?.Invoke();
			TryCreateUnhappiness(((Component)combatable.MyGameCard).transform.position, 2);
		}
		foreach (Equipable item in allEquipables)
		{
			if ((Object)(object)item != (Object)null && !string.IsNullOrEmpty(item.Id))
			{
				CreateCard(((Component)combatable).transform.position, item.Id, faceUp: true, checkAddToStack: false, playSound: false).MyGameCard.SendIt();
			}
		}
		yield return (object)new WaitForSeconds(1f);
		if (resetTargetOnDeath)
		{
			GameCamera.instance.TargetPositionOverride = null;
		}
		onComplete?.Invoke();
	}

	public bool CheckAllVillagersDead()
	{
		if (DebugDontNeedVillagers)
		{
			return false;
		}
		return GetCardCount<BaseVillager>() <= 0;
	}

	public void CreateSmoke(Vector3 pos)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem val = null;
		foreach (ParticleSystem item in smokeBuffer)
		{
			if (!item.isPlaying)
			{
				val = item;
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			val = Object.Instantiate<GameObject>(PrefabManager.instance.SmokeParticlePrefab).GetComponentInChildren<ParticleSystem>();
			smokeBuffer.Add(val);
		}
		val.Play();
		((Component)val).transform.position = pos + Vector3.up * 0.05f;
	}

	public void CreateMinusElectricity(Vector3 pos)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem val = null;
		foreach (ParticleSystem item in energyMinusBuffer)
		{
			if (!item.isPlaying)
			{
				val = item;
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			val = Object.Instantiate<GameObject>(PrefabManager.instance.ElectricityMinusParticlePrefab).GetComponentInChildren<ParticleSystem>();
			energyMinusBuffer.Add(val);
		}
		val.Play();
		((Component)val).transform.position = pos + Vector3.up * 0.05f;
	}

	public void CreateWellbeingPlus(Vector3 pos)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem val = null;
		foreach (ParticleSystem item in wellbeingPlusBuffer)
		{
			if (!item.isPlaying)
			{
				val = item;
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			val = Object.Instantiate<GameObject>(PrefabManager.instance.WellbeingPlusParticlePrefab).GetComponentInChildren<ParticleSystem>();
			wellbeingPlusBuffer.Add(val);
		}
		val.Play();
		((Component)val).transform.position = pos + Vector3.up * 0.05f;
	}

	public void CloseAllFloatingTextObjects()
	{
		foreach (FloatingStatus item in floatingTextBuffer)
		{
			if ((Object)(object)item != (Object)null && Object.op_Implicit((Object)(object)item.ParentCard))
			{
				item.StopAnimation();
			}
		}
	}

	public void CreateFloatingText(GameCard parent, bool isPositive, int amount, string descriptionText, string iconTag, bool desiredBehaviour, int index = 1, float timer = 1f, bool closeOnHover = false)
	{
		FloatingStatus floatingStatus = null;
		foreach (FloatingStatus item in floatingTextBuffer)
		{
			if (!item.InAnimation)
			{
				floatingStatus = item;
			}
		}
		if ((Object)(object)floatingStatus == (Object)null)
		{
			floatingStatus = Object.Instantiate<GameObject>(PrefabManager.instance.FloatingTextPrefab).GetComponentInChildren<FloatingStatus>();
			floatingTextBuffer.Add(floatingStatus);
		}
		floatingStatus.StartAnimation(parent, isPositive, amount, descriptionText, iconTag, desiredBehaviour, index, timer, closeOnHover);
	}

	public void TryCreateHappiness(Vector3 position, int amount)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (CurseIsActive(CurseType.Happiness))
		{
			for (int i = 0; i < amount; i++)
			{
				CardData cardData = CreateCard(position, "happiness", faceUp: true, checkAddToStack: false);
				CreateSmoke(position);
				StackSend(cardData.MyGameCard, Vector3.zero);
			}
		}
	}

	public void TryCreateUnhappiness(Vector3 position, int amount)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (CurseIsActive(CurseType.Happiness))
		{
			for (int i = 0; i < amount; i++)
			{
				CardData cardData = CreateCard(position, "unhappiness", faceUp: true, checkAddToStack: false);
				CreateSmoke(position);
				StackSend(cardData.MyGameCard, Vector3.zero);
			}
		}
	}

	public ICardId GetRandomCard(List<CardChance> chances, bool removeCard)
	{
		chanceBag.Clear();
		foreach (CardChance chance in chances)
		{
			if ((chance.HasMaxCount && GetCurrentCardCount(chance.Id) >= chance.MaxCountToGive) || (chance.HasPrerequisiteCard && !GivenCards.Contains(chance.PrerequisiteCardId)))
			{
				continue;
			}
			if (chance.IsEnemy)
			{
				if (chance.Chance != 0)
				{
					chanceBag.AddEntry(chance, chance.Chance);
				}
				continue;
			}
			CardData cardPrefab = GetCardPrefab(chance.Id);
			if ((!CurrentRunOptions.IsPeacefulMode || !(cardPrefab is Enemy)) && (!CurrentRunOptions.IsPeacefulMode || !(cardPrefab.Id == "catacombs")) && ((cardPrefab.MyCardType != CardType.Ideas && cardPrefab.MyCardType != CardType.Rumors) || !CurrentSave.FoundCardIds.Contains(chance.Id)) && chance.Chance != 0)
			{
				chanceBag.AddEntry(chance, chance.Chance);
			}
		}
		CardChance cardChance = chanceBag.Choose();
		if (cardChance != null)
		{
			if (cardChance.IsEnemy && !CurrentRunOptions.IsPeacefulMode)
			{
				return CardBag.GetCardIdForEnemyBag(cardChance.EnemyBag, cardChance.Strength);
			}
			return (CardId)cardChance.Id;
		}
		return null;
	}

	private int GetCurrentCardCount(string cardId)
	{
		return AllCards.Count((GameCard c) => c.CardData.Id == cardId && c.MyBoard.IsCurrent);
	}

	public GameCard GetCardWithUniqueId(string uniqueId)
	{
		if (!UniqueIdToCard.TryGetValue(uniqueId, out var value))
		{
			return null;
		}
		return value;
	}

	public int GetTimesAnyBoosterWasBought()
	{
		return BoughtBoosterIds.Count;
	}

	public int GetTimesBoosterWasBoughtOnLocation(Location location)
	{
		int num = 0;
		foreach (string boughtBoosterId in BoughtBoosterIds)
		{
			BoosterpackData boosterData = GetBoosterData(boughtBoosterId);
			if ((Object)(object)boosterData != (Object)null && boosterData.BoosterLocation == location)
			{
				num++;
			}
		}
		return num;
	}

	public void ResetBoughtBoostersOnLocation(Location location)
	{
		BoughtBoosterIds.RemoveAll(delegate(string boosterId)
		{
			BoosterpackData boosterData = GetBoosterData(boosterId);
			return ((Object)(object)boosterData != (Object)null && boosterData.BoosterLocation == location) ? true : false;
		});
	}

	public List<Conflict> GetAllConflicts()
	{
		List<Conflict> list = new List<Conflict>();
		foreach (GameCard allCard in AllCards)
		{
			if (allCard.InConflict && !list.Contains(allCard.Combatable.MyConflict))
			{
				list.Add(allCard.Combatable.MyConflict);
			}
		}
		return list;
	}

	public void LoadSaveRound(SaveRound saveRound)
	{
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		IsLoadingSaveRound = true;
		ClearRound();
		AllCards.Clear();
		UniqueIdToCard.Clear();
		if (Application.isEditor)
		{
			Debug.Log((object)$"Loading Run with {saveRound.RunOptions.MoonLength} moon length and peaceful mode: {saveRound.RunOptions.IsPeacefulMode}");
		}
		MonthTimer = saveRound.MonthTimer;
		OldCurrentMonth = saveRound.CurrentMonth;
		BoardMonths = new BoardMonths(saveRound.BoardMonths);
		GivenCards = saveRound.GivenCards;
		BoughtBoosterIds = saveRound.BoughtBoosterIds;
		CurrentBoard = GetBoardWithId(saveRound.CurrentBoardId);
		CurrentRunOptions = saveRound.RunOptions;
		CurrentRunVariables = saveRound.RunVariables;
		RoundExtraKeyValues = saveRound.ExtraKeyValues;
		if ((Object)(object)CitiesManager.instance == (Object)null)
		{
			new Exception("CitiesManager should be active before loading the saveRound");
		}
		CitiesManager.instance.Wellbeing = saveRound.CitiesWellbeing;
		CitiesManager.instance.NextConflictMonth = saveRound.CitiesConflictMonth;
		CitiesManager.instance.ActiveEvent = saveRound.CitiesDisaster;
		if (CurrentRunVariables.ActiveDemand != null && string.IsNullOrEmpty(CurrentRunVariables.ActiveDemand.DemandId))
		{
			CurrentRunVariables.ActiveDemand = null;
		}
		foreach (SavedCard savedCard in saveRound.SavedCards)
		{
			CardData cardData = CreateCard(savedCard.CardPosition, savedCard.CardPrefabId, savedCard.FaceUp, checkAddToStack: false, playSound: false);
			if ((Object)(object)cardData == (Object)null)
			{
				continue;
			}
			cardData.MyGameCard.MyBoard = GetBoardWithId(savedCard.BoardId);
			cardData.UniqueId = savedCard.UniqueId;
			UniqueIdToCard[cardData.UniqueId] = cardData.MyGameCard;
			cardData.ParentUniqueId = savedCard.ParentUniqueId;
			cardData.EquipmentHolderUniqueId = savedCard.EquipmentHolderUniqueId;
			cardData.WorkerHolderUniqueId = savedCard.WorkerHolderUniqueId;
			cardData.WorkerIndex = savedCard.WorkerIndex;
			cardData.SetExtraCardData(savedCard.ExtraCardData);
			if (savedCard.IsFoil)
			{
				cardData.SetFoil();
			}
			cardData.IsDamaged = savedCard.IsDamaged;
			cardData.DamageType = savedCard.DamageType;
			if (savedCard.StatusEffects != null && savedCard.StatusEffects.Count > 0)
			{
				List<StatusEffect> list = savedCard.StatusEffects.Select((SavedStatusEffect x) => StatusEffect.FromSavedStatusEffect(x)).ToList();
				list.RemoveAll((StatusEffect x) => x == null);
				foreach (StatusEffect item in list)
				{
					item.ParentCard = cardData;
				}
				cardData.StatusEffects = list;
				cardData.MyGameCard.StatusEffectsChanged();
			}
			else
			{
				cardData.StatusEffects = new List<StatusEffect>();
			}
			if (savedCard.CardConnectors != null && savedCard.CardConnectors.Count > 0)
			{
				List<SavedCardConnector> cardConnectors = savedCard.CardConnectors;
				cardConnectors.RemoveAll((SavedCardConnector x) => x == null || string.IsNullOrEmpty(x.ConnectedNodeUniqueId));
				for (int num = 0; num < cardData.MyGameCard.CardConnectorChildren.Count; num++)
				{
					CardConnector cardConnector = cardData.MyGameCard.CardConnectorChildren[num];
					string myUniqueId = cardConnector.GetConnectorUniqueId();
					SavedCardConnector savedCardConnector = cardConnectors.Find((SavedCardConnector x) => x.UniqueId == myUniqueId);
					if (savedCardConnector != null)
					{
						cardConnector.UniqueId = savedCardConnector.UniqueId;
						cardConnector.ConnectedNodeUniqueId = savedCardConnector.ConnectedNodeUniqueId;
					}
				}
			}
			if (savedCard.TimerRunning)
			{
				TimerAction delegateForActionId = cardData.GetDelegateForActionId(savedCard.TimerActionId);
				if (delegateForActionId != null)
				{
					cardData.MyGameCard.StartTimer(savedCard.TargetTimerTime, delegateForActionId, savedCard.Status, savedCard.TimerActionId, savedCard.WithStatusBar, skipWorkerEnergyCheck: true);
					cardData.MyGameCard.CurrentTimerTime = savedCard.CurrentTimerTime;
					cardData.MyGameCard.TimerBlueprintId = savedCard.TimerBlueprintId;
					cardData.MyGameCard.TimerSubprintIndex = savedCard.SubprintIndex;
					cardData.MyGameCard.SkipCitiesChecks = savedCard.SkipCitiesChecks;
				}
			}
		}
		foreach (GameCard allCard in AllCards)
		{
			foreach (CardConnector connector in allCard.CardConnectorChildren)
			{
				if (!string.IsNullOrEmpty(connector.ConnectedNodeUniqueId))
				{
					CardConnector cardConnector2 = (from x in AllCards.SelectMany((GameCard x) => x.CardConnectorChildren)
						where x.UniqueId == connector.ConnectedNodeUniqueId
						select x).FirstOrDefault();
					if ((Object)(object)cardConnector2 != (Object)null)
					{
						connector.ConnectedNode = cardConnector2;
					}
				}
			}
		}
		foreach (GameCard allCard2 in AllCards)
		{
			if (!string.IsNullOrEmpty(allCard2.CardData.ParentUniqueId))
			{
				GameCard cardWithUniqueId = GetCardWithUniqueId(allCard2.CardData.ParentUniqueId);
				if ((Object)(object)cardWithUniqueId != (Object)null)
				{
					allCard2.SetParent(cardWithUniqueId);
				}
			}
		}
		foreach (GameCard allCard3 in AllCards)
		{
			if (!string.IsNullOrEmpty(allCard3.CardData.EquipmentHolderUniqueId))
			{
				GameCard cardWithUniqueId2 = GetCardWithUniqueId(allCard3.CardData.EquipmentHolderUniqueId);
				if ((Object)(object)cardWithUniqueId2 != (Object)null)
				{
					cardWithUniqueId2.EquipmentChildren.Add(allCard3);
					allCard3.EquipmentHolder = cardWithUniqueId2;
					allCard3.IsEquipped = true;
				}
			}
		}
		foreach (GameCard allCard4 in AllCards)
		{
			if (allCard4.CardData.WorkerAmount > 0)
			{
				allCard4.WorkerTransformHolder.UpdateWorkerAmount(allCard4.CardData.WorkerAmount);
			}
			if (string.IsNullOrEmpty(allCard4.CardData.WorkerHolderUniqueId))
			{
				continue;
			}
			GameCard cardWithUniqueId3 = GetCardWithUniqueId(allCard4.CardData.WorkerHolderUniqueId);
			if ((Object)(object)cardWithUniqueId3 != (Object)null)
			{
				if (cardWithUniqueId3.WorkerChildren.Count < cardWithUniqueId3.CardData.WorkerAmount)
				{
					cardWithUniqueId3.WorkerChildren.Add(allCard4);
					allCard4.WorkerHolder = cardWithUniqueId3;
					allCard4.IsWorking = true;
				}
				else
				{
					allCard4.CardData.WorkerHolderUniqueId = null;
					allCard4.IsWorking = false;
				}
			}
		}
		foreach (GameCard allCard5 in AllCards)
		{
			allCard5.StatusEffectsChanged();
		}
		foreach (SavedConflict savedConflict in saveRound.SavedConflicts)
		{
			Conflict.CreateFromSavedConflict(savedConflict);
		}
		foreach (SavedBooster savedBooster2 in saveRound.SavedBoosters)
		{
			Boosterpack boosterpack = CreateBoosterpack(savedBooster2.Position, savedBooster2.BoosterId);
			if (!((Object)(object)boosterpack != (Object)null))
			{
				continue;
			}
			boosterpack.MyBoard = GetBoardWithId(savedBooster2.BoardId);
			int num2 = savedBooster2.TimesOpened;
			boosterpack.TimesOpened = savedBooster2.TimesOpened;
			for (int num3 = 0; num3 < boosterpack.CardBags.Count; num3++)
			{
				CardBag cardBag = boosterpack.CardBags[num3];
				int num4 = Mathf.Min(num2, cardBag.CardsInPack);
				cardBag.CardsInPack -= num4;
				num2 -= num4;
				if (num2 <= 0)
				{
					break;
				}
			}
		}
		foreach (SavedBoosterBox savedBooster in saveRound.SavedBoosterBoxes)
		{
			BuyBoosterBox buyBoosterBox = AllBoosterBoxes.Find((BuyBoosterBox x) => x.BoosterId == savedBooster.BoosterId);
			if ((Object)(object)buyBoosterBox != (Object)null)
			{
				buyBoosterBox.StoredCostAmount = savedBooster.StoredCostAmount;
			}
		}
		if (saveRound.SaveVersion != 3)
		{
			PerformSaveRoundMigration(saveRound.SaveVersion, 3);
		}
		IsLoadingSaveRound = false;
	}

	public void PerformSaveRoundMigration(int oldVersion, int newVersion)
	{
		if (oldVersion == 0 && newVersion == 1)
		{
			Debug.Log((object)$"Performing save round migration from v{oldVersion} to v{newVersion}");
			foreach (GameCard allCard in AllCards)
			{
				if (allCard.CardData is BaseVillager baseVillager)
				{
					baseVillager.HealthPoints = Mathf.Min(baseVillager.ProcessedCombatStats.MaxHealth, baseVillager.HealthPoints * 3);
				}
			}
			for (int num = AllCards.Count - 1; num >= 0; num--)
			{
				if (AllCards[num].CardData is Combatable combatable)
				{
					if (combatable.Id == "swordsman")
					{
						combatable.CreateAndEquipCard("sword", markAsFound: true);
					}
					if (combatable.Id == "explorer")
					{
						combatable.CreateAndEquipCard("map", markAsFound: true);
					}
					if (combatable.Id == "militia")
					{
						combatable.CreateAndEquipCard("spear", markAsFound: true);
					}
					if (combatable.Id == "fisher")
					{
						combatable.CreateAndEquipCard("fishing_rod", markAsFound: true);
					}
				}
			}
		}
		if (oldVersion == 1 && newVersion == 2 && BoardMonths.IsEmpty && MonthTimer > 0f)
		{
			BoardMonths = new BoardMonths();
			BoardMonths.MainMonth = OldCurrentMonth - CurrentRunVariables.IslandMonths;
			BoardMonths.IslandMonth = CurrentRunVariables.IslandMonths;
			BoardMonths.DeathMonth = Mathf.Max(1, CurrentRunVariables.DeathMonths);
		}
		if (oldVersion == 2 && newVersion == 3)
		{
			List<GameCard> list = AllCards.Where((GameCard x) => x.CardData.Id == "strange_portal").ToList();
			for (int num2 = 0; num2 < list.Count - 1; num2++)
			{
				list[num2].DestroyCard();
			}
		}
	}

	public GameBoard GetBoardWithId(string id)
	{
		foreach (GameBoard board in Boards)
		{
			if (board.Id == id)
			{
				return board;
			}
		}
		return null;
	}

	public GameBoard GetBoardWithLocation(Location loc)
	{
		foreach (GameBoard board in Boards)
		{
			if (board.Location == loc)
			{
				return board;
			}
		}
		return null;
	}

	public bool BoughtWithGoldChest(GameCard card, int count)
	{
		return BoughtWithChest(card, count, "gold");
	}

	public bool BoughtWithShellChest(GameCard card, int count)
	{
		return BoughtWithChest(card, count, "shell");
	}

	private bool BoughtWithChest(GameCard card, int count, string heldCardId)
	{
		return card.GetAllCardsInStack().Sum((GameCard x) => (x.CardData is Chest chest && chest.HeldCardId == heldCardId) ? chest.CoinCount : 0) >= count;
	}

	public int GetAmountInChest(GameCard card, string heldCardId)
	{
		return card.GetAllCardsInStack().Sum((GameCard x) => (x.CardData is Chest chest && chest.HeldCardId == heldCardId) ? chest.CoinCount : 0);
	}

	public void BuyWithChest(GameCard childCard, int toUse)
	{
		List<Chest> list = (from x in childCard.GetAllCardsInStack()
			where x.CardData is Chest
			select x.CardData as Chest).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			Chest chest = list[num];
			int num2 = Mathf.Min(toUse, chest.CoinCount);
			chest.CoinCount -= num2;
			toUse -= num2;
			if (toUse <= 0)
			{
				break;
			}
		}
		if (childCard.HasParent)
		{
			childCard.RemoveFromStack();
			childCard.SendIt();
		}
	}

	public bool BoughtWithGold(GameCard card, int count, bool checkStackAllSame = false)
	{
		return GetCardCountInStack(card, (CardData x) => x.Id == "gold") >= count;
	}

	public bool BoughtWithShells(GameCard card, int count, bool checkStackAllSame = false)
	{
		return GetCardCountInStack(card, (CardData x) => x.Id == "shell") >= count;
	}

	public int GetDollarsInCreditcard(GameCard card)
	{
		return card.GetAllCardsInStack().Sum((GameCard x) => (x.CardData is Creditcard creditcard) ? creditcard.DollarCount : 0);
	}

	public void BuyWithCreditcard(GameCard childCard, int toUse)
	{
		List<Creditcard> list = (from x in childCard.GetAllCardsInStack()
			where x.CardData is Creditcard
			select x.CardData as Creditcard).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			Creditcard creditcard = list[num];
			int num2 = Mathf.Min(toUse, creditcard.DollarCount);
			creditcard.DollarCount -= num2;
			toUse -= num2;
			if (toUse <= 0)
			{
				break;
			}
		}
		if (childCard.HasParent)
		{
			childCard.RemoveFromStack();
			childCard.SendIt();
		}
	}

	public void RemoveCardsFromStack(GameCard childCard, int toRemove)
	{
		for (int i = 0; i < toRemove; i++)
		{
			childCard.GetLeafCard().DestroyCard(spawnSmoke: true);
		}
		if ((Object)(object)childCard != (Object)null && childCard.HasParent)
		{
			childCard.RemoveFromParent();
		}
	}

	public void RemoveCardsFromStackPred(GameCard card, int toRemove, Predicate<GameCard> pred)
	{
		List<GameCard> list = card.GetAllCardsInStack().FindAll(pred);
		List<GameCard> allCardsInStack = card.GetAllCardsInStack();
		int num = 0;
		foreach (GameCard item in list)
		{
			if (num == toRemove)
			{
				break;
			}
			allCardsInStack.Remove(item);
			item.RemoveFromStack();
			item.DestroyCard(spawnSmoke: true);
			num++;
		}
		Restack(allCardsInStack);
	}

	private void ClearRound()
	{
		QuestsCompleted = 0;
		NewCardsFound = 0;
		MonthTimer = 0f;
		BoardMonths = new BoardMonths();
		if ((Object)(object)CitiesManager.instance != (Object)null)
		{
			CitiesManager.instance.Wellbeing = CitiesManager.instance.WellbeingStart;
			CitiesManager.instance.NextConflictMonth = 0;
		}
		GivenCards.Clear();
		BoughtBoosterIds.Clear();
		for (int num = AllCards.Count - 1; num >= 0; num--)
		{
			if (num <= 0 || num < AllCards.Count)
			{
				AllCards[num].DestroyCard(spawnSmoke: false, playSound: false);
			}
		}
		for (int num2 = AllBoosters.Count - 1; num2 >= 0; num2--)
		{
			Object.Destroy((Object)(object)((Component)AllBoosters[num2]).gameObject);
		}
	}

	public HitText CreateHitText(Vector3 pos, string text, HitText prefab)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		HitText hitText = Object.Instantiate<HitText>(prefab);
		((Component)hitText).transform.position = pos;
		((TMP_Text)hitText.TextMesh).text = text;
		return hitText;
	}

	public void CheckDebugInput()
	{
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		if (InputController.instance.GetKeyDown((Key)8))
		{
			CitiesManager.instance.AddWellbeing(5);
		}
		if (InputController.instance.GetKeyDown((Key)7))
		{
			CitiesManager.instance.AddWellbeing(-5);
		}
		if ((Object)(object)HoveredCard != (Object)null && InputController.instance.GetKeyDown((Key)40))
		{
			GameCard cardWithStatusInStack = HoveredCard.GetCardWithStatusInStack();
			if ((Object)(object)cardWithStatusInStack != (Object)null)
			{
				cardWithStatusInStack.CurrentTimerTime = cardWithStatusInStack.TargetTimerTime;
			}
		}
		if (InputController.instance.GetKeyDown((Key)27))
		{
			AudioManager.me.SkipSong();
		}
		if (InputController.instance.GetKeyDown((Key)35) && (Object)(object)HoveredCard != (Object)null)
		{
			HoveredCard.CardData.StatusEffects.Clear();
			HoveredCard.CardData.IsDamaged = false;
			HoveredCard.CardData.DamageType = CardDamageType.None;
			HoveredCard.StatusEffectsChanged();
		}
		if (InputController.instance.GetKeyDown((Key)26) && (Object)(object)HoveredCard != (Object)null)
		{
			BaseVillager card = GetCard<BaseVillager>();
			HoveredCard.CardAnimations.Add(new CardAnimation_FakeMeleeAttack(HoveredCard, card.MyGameCard));
		}
		if (InputController.instance.GetKeyDown((Key)22))
		{
			Combatable combatable = HoveredCard?.CardData as Combatable;
			if ((Object)(object)combatable != (Object)null)
			{
				combatable.HealthPoints = combatable.ProcessedCombatStats.MaxHealth;
			}
		}
		if (InputController.instance.GetKeyDown((Key)32) && ((Object)(object)HoveredCard != (Object)null || (Object)(object)HoveredDraggable != (Object)null))
		{
			if ((Object)(object)HoveredCard != (Object)null)
			{
				if (HoveredCard.CardData is Combatable combatable2)
				{
					combatable2.Damage(100);
				}
				else
				{
					HoveredCard.DestroyCard();
				}
			}
			else if ((Object)(object)HoveredDraggable != (Object)null && HoveredDraggable is Boosterpack)
			{
				Object.Destroy((Object)(object)((Component)HoveredDraggable).gameObject);
			}
		}
		if (InputController.instance.GetKeyDown((Key)21) && (Object)(object)HoveredCard != (Object)null)
		{
			if (!HoveredCard.CardData.IsFoil)
			{
				HoveredCard.CardData.SetFoil();
			}
			else
			{
				HoveredCard.CardData.IsFoil = false;
				if (HoveredCard.CardData.Value != -1)
				{
					HoveredCard.CardData.Value /= 5;
				}
				if (HoveredCard.CardData.CitiesValue != -1)
				{
					HoveredCard.CardData.CitiesValue /= 5;
				}
			}
		}
		if (InputController.instance.GetKeyDown((Key)20) && (Object)(object)HoveredCard != (Object)null)
		{
			CreateFloatingText(HoveredCard, isPositive: true, 5, "Test hovered", Icons.Wellbeing, desiredBehaviour: true, 0, 0f, closeOnHover: true);
		}
		if (InputController.instance.GetKeyDown((Key)17) && (Object)(object)HoveredCard != (Object)null)
		{
			CardData cardData = CreateCard(((Component)HoveredCard).transform.position, HoveredCard.CardData, faceUp: true, checkAddToStack: false);
			if (cardData is Worker worker)
			{
				worker.Housing = null;
			}
			StackSendTo(cardData.MyGameCard, HoveredCard);
		}
		if (InputController.instance.GetKeyDown((Key)29) && (Object)(object)HoveredCard != (Object)null && HoveredCard.CardData is BaseVillager baseVillager)
		{
			baseVillager.Age++;
		}
	}

	private bool SpiritDLCInstalled()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isEditor)
		{
			return DebugOptions.Default.SpiritDlcEnabled;
		}
		if (SteamManager.Initialized && SteamApps.BIsDlcInstalled(new AppId_t(2446110u)))
		{
			Debug.Log((object)"Load Spirit DLC");
			return true;
		}
		return false;
	}

	private bool CitiesDLCInstalled()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isEditor)
		{
			return DebugOptions.Default.CitiesDlcEnabled;
		}
		if (SteamManager.Initialized && SteamApps.BIsDlcInstalled(new AppId_t(2867570u)))
		{
			Debug.Log((object)"Load Cities DLC");
			return true;
		}
		return false;
	}

	public bool IsSpiritDlcActive()
	{
		return GameDataLoader.SpiritDlcLoaded;
	}

	public bool IsCitiesDlcActive()
	{
		return GameDataLoader.CitiesDlcLoaded;
	}

	public bool CurseIsActive(CurseType curseType)
	{
		foreach (Curse activeCurse in ActiveCurses)
		{
			if (activeCurse.MyGameCard.MyBoard.IsCurrent && activeCurse.CurseType == curseType)
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveAllCardsFromBoard(string boardId, bool removeBoosters = true)
	{
		foreach (GameCard item in GetAllCardsOnBoard(boardId))
		{
			item.DestroyCard();
		}
		if (removeBoosters)
		{
			RemoveAllBoostersFromBoard(boardId);
		}
	}

	public void RemoveAllBoostersFromBoard(string boardId)
	{
		foreach (Boosterpack item in GetAllBoostersOnBoard(boardId))
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
	}

	public void CheckSpiritCutscenes()
	{
		if (CurseIsActive(CurseType.Happiness) && CurrentBoard.Id == "happiness")
		{
			int happinessCount = GetHappinessCount();
			if (happinessCount >= 5)
			{
				QueueCutsceneIfNotPlayed("happiness_middle");
			}
			if (happinessCount >= 10)
			{
				QueueCutsceneIfNotPlayed("happiness_end");
			}
		}
		if (CurseIsActive(CurseType.Death) && CurrentBoard.Id == "death" && AllBoosterBoxes.Any((BuyBoosterBox x) => x.BoosterId == "death_locations" && x.Booster.IsUnlocked))
		{
			QueueCutsceneIfNotPlayed("death_end");
		}
	}
}
