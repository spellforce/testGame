using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
	public TMP_InputField SearchField;

	public RectTransform GeneralRect;

	public RectTransform CardRect;

	public RectTransform ShortcutRect;

	public RectTransform EffectRect;

	public RectTransform SavesRect;

	public RectTransform PresetRect;

	public RectTransform CutscenesRect;

	public RectTransform GeneralContent;

	public RectTransform CardContent;

	public RectTransform ShortcutContent;

	public RectTransform EffectContent;

	public CustomButton GeneralButton;

	public CustomButton CardsButton;

	public CustomButton ShortcutButton;

	public CustomButton EffectButton;

	public CustomButton SavesButton;

	public CustomButton PresetButton;

	public CustomButton CutscenesButton;

	public CustomButton EndlessMoonButton;

	public CustomButton NeedVillagersButton;

	public CustomButton EndCurrentMoonButton;

	public CustomButton UnlockBaseGameIdeasButton;

	public CustomButton UnlockIdeasButton;

	public CustomButton UnlockBoostersButton;

	public CustomButton UnlockQuestsButton;

	public CustomButton PeacefulModeButton;

	public CustomButton NoFoodButton;

	public CustomButton NoEnergyButton;

	public CustomButton CoinChestButton;

	public CustomButton ResetRunButton;

	public CustomButton SpawnEnemiesButton;

	public CustomButton StartCitiesRunButton;

	public CustomButton DemonScenarioButton;

	public CustomButton KrakenScenarioButton;

	public CustomButton IslandScenarioButton;

	public CustomButton DemonLordScenarioButton;

	public CustomButton WitchForestButton;

	public RectTransform StatusEffectButtonParent;

	public RectTransform CutsceneButtonParent;

	public RectTransform SavesParent;

	public CustomButton SaveButton;

	public CustomButton OpenSavesDirectoryButton;

	public RectTransform PresetsParent;

	public CustomButton PresetSaveButton;

	public CustomButton OpenPresetsDirectoryButton;

	private DebugTab OpenTab;

	private StatusEffect SelectedStatusEffect;

	private List<CardData> cardData = new List<CardData>();

	private List<CustomButton> cards = new List<CustomButton>();

	public static DebugScreen instance;

	private void InitializeDebugScreen()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		cardData = WorldManager.instance.CardDataPrefabs.OrderBy((CardData x) => (x.MyCardType == CardType.Ideas) ? ("Idea: " + x.Name) : x.Name).ToList();
		cards.Clear();
		for (int num = 0; num < cardData.Count; num++)
		{
			CardData prefab = cardData[num];
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.DebugButtonPrefab);
			((Component)customButton).transform.SetParent((Transform)(object)CardContent);
			((Component)customButton).transform.localPosition = Vector3.zero;
			((Component)customButton).transform.localScale = Vector3.one;
			((Component)customButton).transform.localRotation = Quaternion.identity;
			((TMP_Text)customButton.TextMeshPro).text = prefab.FullName;
			customButton.Clicked += delegate
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), prefab, faceUp: true, checkAddToStack: false);
				WorldManager.instance.StackSend(cardData.MyGameCard, Vector3.zero);
			};
			cards.Add(customButton);
		}
		UpdateSaveElements();
	}

	private void Start()
	{
		SwitchTab(DebugTab.General);
		InitializeDebugScreen();
	}

	private void Update()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		CheckDebugInput();
		((Graphic)GeneralButton.Image).color = (((Component)GeneralRect).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)CardsButton.Image).color = (((Component)CardRect).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)ShortcutButton.Image).color = (((Component)ShortcutRect).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)EffectButton.Image).color = (((Component)EffectRect).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)SavesButton.Image).color = (((Component)SavesRect).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)CutscenesButton.Image).color = (((Component)CutscenesRect).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((TMP_Text)EndlessMoonButton.TextMeshPro).text = SokLoc.Translate("label_debug_endless_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(WorldManager.instance.DebugEndlessMoonEnabled)) });
		((TMP_Text)PeacefulModeButton.TextMeshPro).text = SokLoc.Translate("label_debug_toggle_peaceful_mode", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(WorldManager.instance.CurrentRunOptions.IsPeacefulMode)) });
		((TMP_Text)NoFoodButton.TextMeshPro).text = SokLoc.Translate("label_debug_toggle_no_food", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(WorldManager.instance.DebugNoFoodEnabled)) });
		((TMP_Text)NeedVillagersButton.TextMeshPro).text = SokLoc.Translate("label_debug_need_villagers", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(WorldManager.instance.DebugDontNeedVillagers)) });
		((TMP_Text)NoEnergyButton.TextMeshPro).text = SokLoc.Translate("label_debug_no_energy", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(WorldManager.instance.DebugNoEnergyEnabled)) });
	}

	private void Awake()
	{
		instance = this;
		SetHandlers();
	}

	private void CheckDebugInput()
	{
		if (SelectedStatusEffect != null && InputController.instance.GetInputBegan(0))
		{
			WorldManager.instance.HoveredCard?.CardData?.AddStatusEffect(SelectedStatusEffect);
			SelectedStatusEffect = null;
		}
		if (!Application.isEditor)
		{
			WorldManager.instance.CheckDebugInput();
		}
	}

	private void SwitchTab(DebugTab tab)
	{
		((Component)GeneralRect).gameObject.SetActive(tab == DebugTab.General);
		((Component)CardRect).gameObject.SetActive(tab == DebugTab.Cards);
		((Component)ShortcutRect).gameObject.SetActive(tab == DebugTab.Shortcuts);
		((Component)EffectRect).gameObject.SetActive(tab == DebugTab.Effects);
		((Component)SavesRect).gameObject.SetActive(tab == DebugTab.Saves);
		((Component)PresetRect).gameObject.SetActive(tab == DebugTab.Presets);
		((Component)CutscenesRect).gameObject.SetActive(tab == DebugTab.Cutscenes);
	}

	private void SetHandlers()
	{
		GeneralButton.Clicked += delegate
		{
			OpenTab = DebugTab.General;
			SwitchTab(OpenTab);
		};
		CardsButton.Clicked += delegate
		{
			OpenTab = DebugTab.Cards;
			SwitchTab(OpenTab);
		};
		ShortcutButton.Clicked += delegate
		{
			OpenTab = DebugTab.Shortcuts;
			SwitchTab(OpenTab);
		};
		EffectButton.Clicked += delegate
		{
			OpenTab = DebugTab.Effects;
			SwitchTab(OpenTab);
		};
		SavesButton.Clicked += delegate
		{
			OpenTab = DebugTab.Saves;
			SwitchTab(OpenTab);
		};
		CutscenesButton.Clicked += delegate
		{
			OpenTab = DebugTab.Cutscenes;
			SwitchTab(OpenTab);
		};
		PresetButton.Clicked += delegate
		{
			if (Application.isEditor)
			{
				OpenTab = DebugTab.Presets;
				SwitchTab(OpenTab);
			}
			else
			{
				GameCanvas.instance.ShowSimpleModal("This option is only available in editor", "Not available!");
			}
		};
		((UnityEvent<string>)(object)SearchField.onValueChanged).AddListener((UnityAction<string>)delegate(string value)
		{
			foreach (CustomButton card in cards)
			{
				((Component)card).gameObject.SetActive(false);
			}
			foreach (CustomButton item in cards.Where((CustomButton card) => ((TMP_Text)card.TextMeshPro).text.ToLower().Replace(" ", "").Contains(value.ToLower().Replace(" ", ""))).ToList())
			{
				((Component)item).gameObject.SetActive(true);
			}
		});
		EndlessMoonButton.Clicked += delegate
		{
			ToggleEndlessMoon();
		};
		NeedVillagersButton.Clicked += delegate
		{
			ToggleNeedVillagers();
		};
		EndCurrentMoonButton.Clicked += delegate
		{
			EndCurrentMoon();
		};
		StartCitiesRunButton.Clicked += delegate
		{
			StartCitiesRun();
		};
		UnlockBoostersButton.Clicked += delegate
		{
			UnlockBoosters();
		};
		UnlockIdeasButton.Clicked += delegate
		{
			UnlockIdeas();
		};
		UnlockBaseGameIdeasButton.Clicked += delegate
		{
			UnlockBaseGameIdeas();
		};
		UnlockQuestsButton.Clicked += delegate
		{
			UnlockQuests();
		};
		PeacefulModeButton.Clicked += delegate
		{
			TogglePeacefulMode();
		};
		NoFoodButton.Clicked += delegate
		{
			ToggleNoFood();
		};
		NoEnergyButton.Clicked += delegate
		{
			ToggleNoEnergy();
		};
		CoinChestButton.Clicked += delegate
		{
			SpawnFullCoinChest();
		};
		ResetRunButton.Clicked += delegate
		{
			ResetRunVariables();
		};
		SpawnEnemiesButton.Clicked += delegate
		{
			SpawnEnemies();
		};
		DemonScenarioButton.Clicked += delegate
		{
			ScenarioDemon();
		};
		KrakenScenarioButton.Clicked += delegate
		{
			ScenarioKraken();
		};
		IslandScenarioButton.Clicked += delegate
		{
			ScenarioIsland();
		};
		DemonLordScenarioButton.Clicked += delegate
		{
			ScenarioDemonLord();
		};
		WitchForestButton.Clicked += delegate
		{
			ScenarioWitchForest();
		};
		foreach (Type statusEffect in (from type in typeof(StatusEffect).Assembly.GetTypes()
			where type.IsSubclassOf(typeof(StatusEffect))
			select type).ToList())
		{
			StatusEffect statusEffect2 = Activator.CreateInstance(statusEffect) as StatusEffect;
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.DebugButtonPrefab);
			((TMP_Text)customButton.TextMeshPro).text = statusEffect2.Name;
			((Component)customButton).transform.SetParentClean((Transform)(object)StatusEffectButtonParent);
			customButton.Clicked += delegate
			{
				SelectedStatusEffect = Activator.CreateInstance(statusEffect) as StatusEffect;
			};
		}
		foreach (ScriptableCutscene cutscenes in WorldManager.instance.GameDataLoader.Cutscenes)
		{
			CustomButton customButton2 = Object.Instantiate<CustomButton>(PrefabManager.instance.DebugButtonPrefab);
			((TMP_Text)customButton2.TextMeshPro).text = cutscenes.CutsceneId;
			((Component)customButton2).transform.SetParentClean((Transform)(object)CutsceneButtonParent);
			customButton2.Clicked += delegate
			{
				WorldManager.instance.QueueCutscene(cutscenes);
			};
		}
		SaveButton.Clicked += delegate
		{
			SaveManager.instance.CreateDebugSaveWithId(DateTime.Now.ToString("dd_MM_HHmm_ss"));
			UpdateSaveElements();
		};
		OpenSavesDirectoryButton.Clicked += delegate
		{
			SaveManager.OpenSavesDirectory();
		};
		PresetSaveButton.Clicked += delegate
		{
			SavePreset();
			UpdatePresetsElements();
		};
		OpenPresetsDirectoryButton.Clicked += delegate
		{
			openPresetsDirectory();
		};
	}

	public void SavePreset()
	{
		SavedPreset savedPreset = new SavedPreset();
		savedPreset.SaveId = WorldManager.instance.CurrentBoard.Id + "_" + DateTime.Now.ToString("dd_MM_HHmm_ss");
		savedPreset.SavedCards = new List<SavedCard>();
		foreach (GameCard item in WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id))
		{
			savedPreset.SavedCards.Add(item.ToSavedCard());
		}
		WorldManager.instance.SavePreset(savedPreset);
	}

	private void openPresetsDirectory()
	{
		string text = Application.dataPath + "/PresetSaves";
		text = text.Replace("/", "\\");
		Process.Start("explorer.exe", text);
	}

	public void SpawnEnemies()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		foreach (CardIdWithEquipment item in SpawnHelper.GetEnemiesToSpawn(new List<SetCardBagType> { SetCardBagType.BasicEnemy }, 50f))
		{
			Combatable obj = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), item) as Combatable;
			obj.HealthPoints = obj.ProcessedCombatStats.MaxHealth;
			obj.MyGameCard.SendIt();
		}
	}

	public void ResetRunVariables()
	{
		WorldManager.instance.CurrentRunVariables.VisitedForest = false;
		WorldManager.instance.CurrentRunVariables.VisitedIsland = false;
		WorldManager.instance.CurrentRunVariables.ForestWave = 1;
	}

	public void ToggleEndlessMoon()
	{
		WorldManager.instance.DebugEndlessMoonEnabled = !WorldManager.instance.DebugEndlessMoonEnabled;
	}

	public void ToggleNeedVillagers()
	{
		WorldManager.instance.DebugDontNeedVillagers = !WorldManager.instance.DebugDontNeedVillagers;
	}

	public void ToggleNoEnergy()
	{
		WorldManager.instance.DebugNoEnergyEnabled = !WorldManager.instance.DebugNoEnergyEnabled;
	}

	public void EndCurrentMoon()
	{
		WorldManager.instance.MonthTimer = WorldManager.instance.MonthTime - 1f;
	}

	public void StartCitiesRun()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager");
		WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "event_industrial_revolution").MyGameCard.SetChild(cardData.MyGameCard);
	}

	public void UnlockBoosters()
	{
	}

	public void UnlockIdeas()
	{
		WorldManager.instance.DebugUnlockIdeas(justBasegame: false);
		GameScreen.instance.UpdateIdeasLog();
	}

	public void UnlockBaseGameIdeas()
	{
		WorldManager.instance.DebugUnlockIdeas(justBasegame: true);
		GameScreen.instance.UpdateIdeasLog();
	}

	public void UnlockQuests()
	{
		QuestManager.instance.DebugUnlockAllQuests();
	}

	public void TogglePeacefulMode()
	{
		WorldManager.instance.CurrentRunOptions.IsPeacefulMode = !WorldManager.instance.CurrentRunOptions.IsPeacefulMode;
	}

	public void ToggleNoFood()
	{
		WorldManager.instance.DebugNoFoodEnabled = !WorldManager.instance.DebugNoFoodEnabled;
	}

	public void SpawnFullCoinChest()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (WorldManager.instance.CurrentBoard.BoardOptions.UsesShells)
		{
			(WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "shell_chest", faceUp: true, checkAddToStack: false) as Chest).CoinCount = 100;
		}
		else
		{
			(WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "coin_chest", faceUp: true, checkAddToStack: false) as Chest).CoinCount = 100;
		}
	}

	public void ScenarioDemon()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "goblet", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "temple", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioKraken()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "sacred_key", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "sacred_chest", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioIsland()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "rowboat", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioDemonLord()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "island_relic", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "cathedral", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioWitchForest()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "stable_portal", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager", faceUp: true, checkAddToStack: false);
		CardData cardData3 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "sword", faceUp: true, checkAddToStack: false);
		cardData2.MyGameCard.SendIt();
		cardData.MyGameCard.SendIt();
		WorldManager.instance.StackSendTo(cardData3.MyGameCard, cardData2.MyGameCard);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public static string YesNo(bool a)
	{
		if (!a)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}

	private void UpdateSaveElements()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in (Transform)SavesParent)
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
		foreach (FileInfo file in SaveManager.GetDebugFiles())
		{
			CustomButton cb = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab);
			cb.HardSetText(file.Name.Replace("_", " ").Replace(".sav", ""));
			((TMP_Text)cb.TextMeshPro).fontSize = 20f;
			cb.Clicked += delegate
			{
				if (cb.WasRightClick)
				{
					FileHelper.ArchiveFile(file.FullName);
					UpdateSaveElements();
				}
				else
				{
					SaveManager.ForceReload(SaveManager.GetSaveFromFileInfo(file));
					WorldManager.RestartGame();
				}
			};
			((Component)cb).transform.SetParentClean((Transform)(object)SavesParent);
		}
	}

	private void UpdatePresetsElements()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in (Transform)PresetsParent)
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
		foreach (SavedPreset savedPreset in GetSavedPresets())
		{
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab);
			customButton.HardSetText(savedPreset.SaveId.Replace("_", " "));
			((TMP_Text)customButton.TextMeshPro).fontSize = 20f;
			customButton.Clicked += delegate
			{
			};
			((Component)customButton).transform.SetParentClean((Transform)(object)PresetsParent);
		}
	}

	private List<SavedPreset> GetSavedPresets()
	{
		List<SavedPreset> list = new List<SavedPreset>();
		foreach (FileInfo presetFile in GetPresetFiles())
		{
			if (!(presetFile.Extension == ".meta"))
			{
				SavedPreset savedPreset = JsonUtility.FromJson<SavedPreset>(File.ReadAllText(presetFile.FullName));
				savedPreset.FullPath = presetFile.FullName;
				list.Add(savedPreset);
			}
		}
		return list;
	}

	public void AutoSave()
	{
		SaveGame currentSave = WorldManager.instance.CurrentSave;
		string saveId = currentSave.SaveId;
		string arg = DateTime.Now.ToString("dd_MM_HHmm");
		currentSave.SaveId = $"auto_{arg}_moon_{WorldManager.instance.CurrentMonth}";
		currentSave.LastPlayedRound = WorldManager.instance.GetSaveRound();
		string content = JsonUtility.ToJson((object)currentSave);
		FileHelper.SaveFile(currentSave.SaveId, content, "AutoSave");
		Debug.Log((object)("Auto saved! (" + currentSave.SaveId + ")"));
		currentSave.SaveId = saveId;
		UpdateSaveElements();
	}

	private static List<FileInfo> GetPresetFiles()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/PresetSaves");
		List<FileInfo> list = new List<FileInfo>();
		list.AddRange(directoryInfo.GetFiles());
		List<FileInfo> list2 = list.OrderBy((FileInfo x) => x.Name).ToList();
		List<FileInfo> list3 = new List<FileInfo>();
		foreach (FileInfo item in list2)
		{
			if (item.Name.StartsWith("preset"))
			{
				list3.Add(item);
			}
		}
		return list3;
	}
}
