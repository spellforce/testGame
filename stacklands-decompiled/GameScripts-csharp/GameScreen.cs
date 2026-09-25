using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameScreen : SokScreen
{
	public TextMeshProUGUI MoneyText;

	public TextMeshProUGUI FoodText;

	public TextMeshProUGUI CardText;

	public TextMeshProUGUI TimeText;

	public TextMeshProUGUI HappinessText;

	public TextMeshProUGUI EnergyText;

	public TextMeshProUGUI WellbeingText;

	public TextMeshProUGUI DollarText;

	public TextMeshProUGUI WorkerText;

	public TextMeshProUGUI InfoTitle;

	public TextMeshProUGUI InfoText;

	public GameObject InfoDividerPrefab;

	private List<GameObject> dividerList = new List<GameObject>();

	public RectTransform InfoLayoutGroup;

	public Image TimeFill;

	public GameObject InfoBox;

	public RectTransform ResourceRect;

	public RectTransform TimeRect;

	public RectTransform ViewRect;

	public TextMeshProUGUI Valuetext;

	public GameObject ValueParent;

	public GameObject FoodCardBox;

	public ShowInfoBox ShowInfoBoxMoney;

	public ShowInfoBox ShowInfoBoxFood;

	public ShowInfoBox ShowInfoBoxTime;

	public ShowInfoBox ShowInfoBoxCard;

	public ShowInfoBox ShowInfoBoxHappiness;

	public ShowInfoBox ShowInfoBoxEnergy;

	public ShowInfoBox ShowInfoBoxWellbeing;

	public ShowInfoBox ShowInfoBoxDollar;

	public ShowInfoBox ShowInfoBoxWorker;

	public ShowInfoBox ShowInfoBoxEnergyButton;

	public Image Crosshair;

	public static string InfoBoxTitle;

	public static string InfoBoxText;

	public Image DebugScreen;

	public CustomButton GameSpeedButton;

	public CustomButton ViewButton;

	public TextMeshProUGUI NextPackToUnlockText;

	public static GameScreen instance;

	public TextMeshProUGUI NoIdeasYetText;

	public RectTransform QuestsParent;

	public RectTransform QuestsTab;

	public RectTransform IdeasTab;

	public TextMeshProUGUI PausedText;

	public CustomButton QuestsButton;

	public CustomButton IdeasButton;

	public TMP_InputField IdeaSearchField;

	public RectTransform NotificationsParent;

	public CustomButton MinimizeButton;

	public GameObject IdeasTabNew;

	public GameObject QuestsTabNew;

	public Image FoldedNewIcon;

	public ScrollRect QuestsScrollRect;

	public ScrollRect IdeasScrollRect;

	private bool isMinimized;

	public bool ControllerIsInUI;

	private List<CustomButton> questButtons;

	private List<CustomButton> ideaButtons;

	private string previousInfoText = "";

	public string HappinessSummaryText;

	public string EnergySummaryText;

	public string WellbeingSummaryText;

	public RectTransform ViewDropdown;

	public CustomButton DefaultViewButton;

	public CustomButton EnergyViewButton;

	public CustomButton TransportViewButton;

	public CustomButton SewageViewButton;

	public CustomButton CalamityViewButton;

	private float pauseBlinkTimer;

	private bool gameSpeedButtonClicked;

	private bool questTabOpen = true;

	public List<AchievementElement> questElements;

	private List<QuestGroup> questGroupOrder = new List<QuestGroup>
	{
		QuestGroup.Starter,
		QuestGroup.MainQuest,
		QuestGroup.Island_Beginnings,
		QuestGroup.Island_Combat,
		QuestGroup.Island_Cooking,
		QuestGroup.Island_Misc,
		QuestGroup.Island_MainQuest,
		QuestGroup.Forest_MainQuest,
		QuestGroup.Fighting,
		QuestGroup.Equipment,
		QuestGroup.Cooking,
		QuestGroup.Exploration,
		QuestGroup.Resources,
		QuestGroup.Building,
		QuestGroup.Survival,
		QuestGroup.Discover_Spirits,
		QuestGroup.Other
	};

	public RectTransform IdeaElementsParent;

	private List<BlueprintGroup> groups = new List<BlueprintGroup>
	{
		BlueprintGroup.Basic,
		BlueprintGroup.Important,
		BlueprintGroup.Building,
		BlueprintGroup.Cooking,
		BlueprintGroup.Military,
		BlueprintGroup.Resources,
		BlueprintGroup.Island,
		BlueprintGroup.Sailing,
		BlueprintGroup.Fishing,
		BlueprintGroup.Happiness,
		BlueprintGroup.Greed,
		BlueprintGroup.Death,
		BlueprintGroup.Power,
		BlueprintGroup.Automation,
		BlueprintGroup.Landmark
	};

	private List<IdeaElement> ideaElements;

	private List<ExpandableLabel> ideaLabels;

	private int foundCount;

	public float prePauseSpeed = 1f;

	public RectTransform SideTransform;

	public ShowInfoBox MinimizeButtonInfoBox;

	private Dictionary<string, CardData> stackRequirements = new Dictionary<string, CardData>();

	private Dictionary<string, int> stackRequirementAmount = new Dictionary<string, int>();

	private float redTextBlinkTimer;

	private bool redBlink;

	public Image GameSpeedIcon;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		instance = this;
		((Component)DebugScreen).gameObject.SetActive(false);
		GameSpeedButton.Clicked += delegate
		{
			gameSpeedButtonClicked = true;
		};
		((Component)ViewDropdown).gameObject.SetActive(false);
		ViewButton.Clicked += delegate
		{
			((Component)ViewDropdown).gameObject.SetActive(true);
		};
		DefaultViewButton.Clicked += delegate
		{
			SetView(ViewType.Default);
		};
		EnergyViewButton.Clicked += delegate
		{
			SetView(ViewType.Energy);
		};
		TransportViewButton.Clicked += delegate
		{
			SetView(ViewType.Transport);
		};
		SewageViewButton.Clicked += delegate
		{
			SetView(ViewType.Sewer);
		};
		CalamityViewButton.Clicked += delegate
		{
			SetView(ViewType.Calamity);
		};
		MinimizeButton.Clicked += delegate
		{
			ToggleMinimize();
		};
		((UnityEvent<string>)(object)IdeaSearchField.onValueChanged).AddListener((UnityAction<string>)delegate
		{
			UpdateIdeasLog();
		});
		QuestsButton.Clicked += delegate
		{
			questTabOpen = true;
			((Component)QuestsTab).gameObject.SetActive(true);
			((Component)IdeasTab).gameObject.SetActive(false);
		};
		QuestsButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			List<CustomButton> list = (questTabOpen ? questButtons : ideaButtons);
			((Navigation)(ref nav)).selectOnDown = (Selectable)(object)((list != null && list.Count > 0) ? list[0] : null);
			((Navigation)(ref nav)).selectOnRight = (Selectable)(object)IdeasButton;
			return nav;
		};
		IdeasButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			List<CustomButton> list = (questTabOpen ? questButtons : ideaButtons);
			((Navigation)(ref nav)).selectOnDown = (Selectable)(object)((list != null && list.Count > 0) ? list[0] : null);
			((Navigation)(ref nav)).selectOnLeft = (Selectable)(object)QuestsButton;
			return nav;
		};
		IdeasButton.Clicked += delegate
		{
			questTabOpen = false;
			((Component)QuestsTab).gameObject.SetActive(false);
			((Component)IdeasTab).gameObject.SetActive(true);
		};
		GameSpeedButton.IsSelectableAction = () => false;
		MinimizeButton.IsSelectableAction = () => false;
		((Component)QuestsTab).gameObject.SetActive(true);
		((Component)IdeasTab).gameObject.SetActive(false);
		((Component)PausedText).gameObject.SetActive(false);
		((Component)NotificationsParent).gameObject.SetActive(true);
		SetViewdropdownTexts();
		InitIdeaElements();
		SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
	}

	private void SetView(ViewType viewType)
	{
		WorldManager.instance.SetViewType(viewType);
		((Component)ViewDropdown).gameObject.SetActive(false);
	}

	public void CloseViewDropdown()
	{
		((Component)ViewDropdown).gameObject.SetActive(false);
	}

	private string GetIconForView(ViewType viewType)
	{
		return viewType switch
		{
			ViewType.Default => Icons.Card, 
			ViewType.Energy => Icons.Energy, 
			ViewType.Sewer => Icons.Sewer, 
			ViewType.Transport => Icons.Transport, 
			ViewType.Calamity => Icons.Calamity, 
			_ => throw new ArgumentException(), 
		};
	}

	private string GetLabelForViewType(ViewType viewType)
	{
		switch (viewType)
		{
		case ViewType.Default:
			return SokLoc.Translate("label_view_default");
		case ViewType.Energy:
			return SokLoc.Translate("label_view_energy");
		case ViewType.Sewer:
			return SokLoc.Translate("label_view_sewage");
		case ViewType.Calamity:
			return SokLoc.Translate("label_view_calamity");
		case ViewType.Transport:
			if (!(WorldManager.instance.GetCurrentBoardSafe().Id == "cities"))
			{
				return SokLoc.Translate("label_view_transport_default");
			}
			return SokLoc.Translate("label_view_transport");
		default:
			throw new ArgumentException();
		}
	}

	private void SetViewdropdownTexts()
	{
		((TMP_Text)DefaultViewButton.TextMeshPro).text = GetLabelForViewType(ViewType.Default) + GetIconForView(ViewType.Default);
		((TMP_Text)EnergyViewButton.TextMeshPro).text = GetLabelForViewType(ViewType.Energy) + GetIconForView(ViewType.Energy);
		((TMP_Text)TransportViewButton.TextMeshPro).text = GetLabelForViewType(ViewType.Transport) + GetIconForView(ViewType.Transport);
		((TMP_Text)SewageViewButton.TextMeshPro).text = GetLabelForViewType(ViewType.Sewer) + GetIconForView(ViewType.Sewer);
		((TMP_Text)CalamityViewButton.TextMeshPro).text = GetLabelForViewType(ViewType.Calamity) + GetIconForView(ViewType.Calamity);
	}

	public void OnBoardChange()
	{
		SetViewdropdownTexts();
	}

	private void Instance_LanguageChanged()
	{
		UpdateIdeasLog();
		UpdateQuestLog();
		SetViewdropdownTexts();
	}

	private void OnDestroy()
	{
		if ((Object)(object)SokLoc.instance != (Object)null)
		{
			SokLoc.instance.LanguageChanged -= Instance_LanguageChanged;
		}
	}

	public DebugScreen GetDebugComponent()
	{
		Image debugScreen = DebugScreen;
		if (debugScreen == null)
		{
			return null;
		}
		return ((Component)debugScreen).GetComponent<DebugScreen>();
	}

	public void SetQuestTab()
	{
		questTabOpen = true;
		((Component)QuestsTab).gameObject.SetActive(true);
		((Component)IdeasTab).gameObject.SetActive(false);
	}

	public void ScrollToQuest(Quest quest)
	{
		((MonoBehaviour)this).StartCoroutine(ScrollToQuestCoroutine(quest));
	}

	private IEnumerator ScrollToQuestCoroutine(Quest quest)
	{
		UpdateQuestLog();
		SetQuestTab();
		yield return null;
		ExpandableLabel expandableLabel = ((Component)QuestsParent).GetComponentsInChildren<ExpandableLabel>().FirstOrDefault((ExpandableLabel x) => (QuestGroup)x.Tag == quest.QuestGroup);
		if ((Object)(object)expandableLabel != (Object)null)
		{
			expandableLabel.SetExpanded(expanded: true);
		}
		AchievementElement achievementElement = questElements.FirstOrDefault((AchievementElement x) => x.MyQuest == quest);
		if ((Object)(object)achievementElement != (Object)null)
		{
			ScrollRect questsScrollRect = QuestsScrollRect;
			Transform transform = ((Component)achievementElement).transform;
			GameCanvas.SetScrollRectPosition(questsScrollRect, (RectTransform)(object)((transform is RectTransform) ? transform : null));
		}
	}

	public void SetMinimize(bool minimized)
	{
		isMinimized = minimized;
		if (isMinimized)
		{
			((Component)QuestsTab).gameObject.SetActive(false);
			((Component)IdeasTab).gameObject.SetActive(false);
		}
		else
		{
			((Component)QuestsTab).gameObject.SetActive(questTabOpen);
			((Component)IdeasTab).gameObject.SetActive(!questTabOpen);
		}
	}

	public void ToggleMinimize()
	{
		SetMinimize(!isMinimized);
	}

	private void OnEnable()
	{
		if (!((Object)(object)QuestManager.instance == (Object)null))
		{
			UpdateQuestLog();
			UpdateIdeasLog();
		}
	}

	public void UpdateQuestLog()
	{
		if ((Object)(object)QuestManager.instance == (Object)null)
		{
			return;
		}
		Dictionary<object, bool> dictionary = wasExpandedDict(((Component)QuestsParent).GetComponentsInChildren<ExpandableLabel>());
		IEnumerable<Quest> source = (((Object)(object)WorldManager.instance.CurrentBoard == (Object)null || WorldManager.instance.CurrentBoard.Id == "main") ? ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation != Location.Death && x.QuestLocation != Location.Greed && x.QuestLocation != Location.Happiness && x.QuestLocation != Location.Cities) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Starter)) : ((WorldManager.instance.CurrentBoard.Id == "island") ? ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Island_Beginnings && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation != Location.Death && x.QuestLocation != Location.Greed && x.QuestLocation != Location.Happiness && x.QuestLocation != Location.Cities) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Island_Beginnings)) : ((!(WorldManager.instance.CurrentBoard.Id == "forest")) ? QuestManager.instance.AllQuests : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Forest))));
		if (WorldManager.instance.CurrentRunVariables != null && !WorldManager.instance.CurrentRunVariables.VisitedIsland)
		{
			source = source.Where((Quest x) => x.QuestLocation != Location.Island);
		}
		if (WorldManager.instance.CurrentBoard?.Id == "happiness")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Happiness_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Happiness) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Happiness_Starter));
		}
		else if (WorldManager.instance.CurrentBoard?.Id == "greed")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Greed_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Greed) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Greed_Starter));
		}
		else if (WorldManager.instance.CurrentBoard?.Id == "death")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Death_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Death) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Death_Starter));
		}
		if (WorldManager.instance.CurrentBoard?.Id == "cities")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Cities_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Cities) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Cities_Starter));
			if (!WorldManager.instance.HasFoundCard("blueprint_barrack"))
			{
				source = source.Where((Quest x) => x.QuestGroup != QuestGroup.Cities_Freedom);
			}
		}
		bool flag = WorldManager.instance.CurrentRunVariables.FinishedDemon || QuestManager.instance.QuestIsComplete("kill_demon");
		if (!WorldManager.instance.IsSpiritDlcActive() || !flag)
		{
			source = source.Where((Quest x) => x.QuestGroup != QuestGroup.Discover_Spirits);
		}
		questElements = CreateQuestElements(QuestsParent, source.ToList());
		questButtons = (from x in ((Component)QuestsParent).GetComponentsInChildren<CustomButton>()
			where ((Behaviour)x).enabled
			select x).ToList();
		for (int num = 0; num < questButtons.Count - 1; num++)
		{
			questButtons[num].ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				int num2 = questButtons.IndexOf(cb);
				((Navigation)(ref nav)).selectOnUp = (Selectable)(object)((num2 == 0) ? QuestsButton : questButtons[num2 - 1]);
				((Navigation)(ref nav)).selectOnDown = (Selectable)(object)questButtons[num2 + 1];
				return nav;
			};
		}
		ExpandableLabel[] componentsInChildren = ((Component)QuestsParent).GetComponentsInChildren<ExpandableLabel>();
		foreach (AchievementElement questElement in questElements)
		{
			if (questElement.IsNew)
			{
				dictionary[questElement.MyQuest.QuestGroup] = true;
			}
		}
		SetFromWasExpandedDict(componentsInChildren, dictionary);
	}

	private string GetAchievementGroupName(QuestGroup group)
	{
		string text = "questgroup_";
		return SokLoc.Translate(group switch
		{
			QuestGroup.Starter => text + "starter", 
			QuestGroup.MainQuest => text + "mainquest", 
			QuestGroup.Fighting => text + "fighting", 
			QuestGroup.Cooking => text + "cooking", 
			QuestGroup.Exploration => text + "exploration", 
			QuestGroup.Resources => text + "resources", 
			QuestGroup.Building => text + "building", 
			QuestGroup.Survival => text + "survival", 
			QuestGroup.Other => text + "other", 
			QuestGroup.Island_Misc => text + "island", 
			_ => text + group.ToString().ToLower(), 
		});
	}

	private List<AchievementElement> CreateQuestElements(RectTransform parent, List<Quest> quests, bool addLabels = true)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		List<AchievementElement> list = new List<AchievementElement>();
		foreach (Transform item in (Transform)parent)
		{
			Transform val = item;
			if (!((Object)val).name.StartsWith("DontDestroy"))
			{
				Object.Destroy((Object)(object)((Component)val).gameObject);
			}
		}
		List<Quest> source = new List<Quest>(quests);
		quests = (from x in quests
			orderby !x.IsMainQuest, questGroupOrder.IndexOf(x.QuestGroup)
			select x).ToList();
		quests.RemoveAll((Quest x) => !QuestManager.instance.QuestIsVisible(x));
		Quest quest = null;
		ExpandableLabel expandableLabel = null;
		bool flag = (from x in quests
			group x by x.QuestGroup).Count() > 1;
		for (int num = 0; num < quests.Count; num++)
		{
			Quest cur = quests[num];
			Quest quest2 = ((num == quests.Count - 1) ? null : quests[num + 1]);
			if (addLabels)
			{
				if (flag && (quest == null || quest.IsMainQuest != cur.IsMainQuest))
				{
					RectTransform obj = Object.Instantiate<RectTransform>(PrefabManager.instance.NormalLabelPrefab);
					((Component)obj).transform.SetParentClean((Transform)(object)parent);
					((Behaviour)((Component)obj).GetComponent<CustomButton>()).enabled = false;
					((Behaviour)((Component)obj).GetComponent<Image>()).enabled = false;
					TextMeshProUGUI componentInChildren = ((Component)obj).GetComponentInChildren<TextMeshProUGUI>();
					((TMP_Text)componentInChildren).fontStyle = (FontStyles)1;
					((TMP_Text)componentInChildren).text = (cur.IsMainQuest ? SokLoc.Translate("label_main_quests") : SokLoc.Translate("label_side_quests"));
				}
				if (quest == null || quest.QuestGroup != cur.QuestGroup)
				{
					expandableLabel = Object.Instantiate<GameObject>(PrefabManager.instance.AchievementElementLabelPrefab).GetComponent<ExpandableLabel>();
					((Component)expandableLabel).transform.SetParentClean((Transform)(object)parent);
					expandableLabel.Tag = cur.QuestGroup;
					int num2 = source.Count((Quest x) => x.QuestGroup == cur.QuestGroup);
					int num3 = source.Count((Quest x) => x.QuestGroup == cur.QuestGroup && QuestManager.instance.QuestIsComplete(x));
					string achievementGroupName = GetAchievementGroupName(cur.QuestGroup);
					achievementGroupName = ((num2 != num3) ? (achievementGroupName + $" ({num3}/{num2})") : (achievementGroupName + " " + Icons.Checkmark));
					expandableLabel.SetText(achievementGroupName);
					if (flag)
					{
						expandableLabel.SetExpanded(expanded: false);
					}
				}
			}
			AchievementElement achievementElement = Object.Instantiate<AchievementElement>(PrefabManager.instance.AchievementElementPrefab);
			achievementElement.SetQuest(cur);
			expandableLabel.Children.Add(((Component)achievementElement).gameObject);
			if (flag)
			{
				((Component)achievementElement).gameObject.SetActive(false);
			}
			((Component)achievementElement).transform.SetParentClean((Transform)(object)parent);
			list.Add(achievementElement);
			if ((quest2 == null || cur.QuestGroup != quest2.QuestGroup) && source.Count((Quest x) => x.QuestGroup == cur.QuestGroup && !QuestManager.instance.QuestIsVisible(x)) > 0)
			{
				AchievementElement achievementElement2 = Object.Instantiate<AchievementElement>(PrefabManager.instance.EmptyAchievementElementPrefab);
				((Component)achievementElement2).transform.SetParentClean((Transform)(object)parent);
				expandableLabel.Children.Add(((Component)achievementElement2).gameObject);
				if (flag)
				{
					((Component)achievementElement2).gameObject.SetActive(false);
				}
			}
			quest = cur;
		}
		return list;
	}

	private Dictionary<object, bool> wasExpandedDict(ExpandableLabel[] labels)
	{
		Dictionary<object, bool> dictionary = new Dictionary<object, bool>();
		foreach (ExpandableLabel expandableLabel in labels)
		{
			dictionary[expandableLabel.Tag] = expandableLabel.IsExpanded;
		}
		return dictionary;
	}

	private void SetFromWasExpandedDict(ExpandableLabel[] labels, Dictionary<object, bool> wasExpanded)
	{
		foreach (ExpandableLabel expandableLabel in labels)
		{
			if (wasExpanded.ContainsKey(expandableLabel.Tag))
			{
				expandableLabel.SetExpanded(wasExpanded[expandableLabel.Tag]);
			}
		}
	}

	public void UpdateIdeaElements()
	{
		UpdateIdeasLog();
	}

	public void InitIdeaElements()
	{
		List<IKnowledge> list = new List<IKnowledge>();
		IEnumerable<Rumor> collection = WorldManager.instance.CardDataPrefabs.OfType<Rumor>();
		list.AddRange(collection);
		List<Blueprint> list2 = new List<Blueprint>(WorldManager.instance.BlueprintPrefabs);
		list2.RemoveAll((Blueprint x) => x.HideFromIdeasTab);
		list.AddRange(list2.Cast<IKnowledge>());
		List<IKnowledge> source = new List<IKnowledge>(list);
		list = (from k in list
			orderby groups.IndexOf(k.Group), k.KnowledgeName
			select k).ToList();
		_ = list.Count;
		IKnowledge knowledge = null;
		ExpandableLabel expandableLabel = null;
		ideaElements = new List<IdeaElement>();
		ideaLabels = new List<ExpandableLabel>();
		for (int num = 0; num < list.Count; num++)
		{
			IKnowledge cur = list[num];
			if (knowledge == null || knowledge.Group != cur.Group)
			{
				expandableLabel = Object.Instantiate<GameObject>(PrefabManager.instance.AchievementElementLabelPrefab).GetComponent<ExpandableLabel>();
				((Component)expandableLabel).transform.SetParentClean((Transform)(object)IdeaElementsParent);
				expandableLabel.Tag = cur.Group;
				source.Count((IKnowledge k) => k.Group == cur.Group);
				source.Count((IKnowledge k) => k.Group == cur.Group && KnowledgeWasFound(k));
				expandableLabel.SetText(GetBlueprintGroupText(cur.Group));
				expandableLabel.SetCallback(UpdateIdeaElements);
				ideaLabels.Add(expandableLabel);
			}
			IdeaElement ideaElement = Object.Instantiate<IdeaElement>(PrefabManager.instance.IdeaElementPrefab);
			((Component)ideaElement).transform.SetParentClean((Transform)(object)IdeaElementsParent);
			expandableLabel.Children.Add(((Component)ideaElement).gameObject);
			ideaElement.SetKnowledge(cur);
			ideaElements.Add(ideaElement);
			knowledge = cur;
		}
	}

	public void UpdateIdeasLog()
	{
		string searchTerm = "";
		if (!string.IsNullOrEmpty(IdeaSearchField.text))
		{
			searchTerm = IdeaSearchField.text;
		}
		List<IKnowledge> currentKnowledges = new List<IKnowledge>();
		IEnumerable<Rumor> collection = WorldManager.instance.CardDataPrefabs.OfType<Rumor>();
		currentKnowledges.AddRange(collection);
		List<Blueprint> list = new List<Blueprint>(WorldManager.instance.BlueprintPrefabs);
		list.RemoveAll((Blueprint x) => x.HideFromIdeasTab);
		currentKnowledges.AddRange(list.Cast<IKnowledge>());
		new List<IKnowledge>(currentKnowledges);
		currentKnowledges = currentKnowledges.OrderBy((IKnowledge k) => groups.IndexOf(k.Group)).ThenBy((IKnowledge x) => x.KnowledgeName).ToList();
		if (WorldManager.instance.CurrentBoard?.Id == "cities")
		{
			currentKnowledges = currentKnowledges.Where((IKnowledge x) => WorldManager.instance.GameDataLoader.GetCardFromId(x.CardId).CardUpdateType == CardUpdateType.Cities).ToList();
		}
		else
		{
			currentKnowledges = currentKnowledges.Where((IKnowledge x) => WorldManager.instance.GameDataLoader.GetCardFromId(x.CardId).CardUpdateType != CardUpdateType.Cities).ToList();
		}
		_ = currentKnowledges.Count;
		Dictionary<object, bool> dictionary = wasExpandedDict(((Component)IdeaElementsParent).GetComponentsInChildren<ExpandableLabel>());
		foreach (IdeaElement element in ideaElements)
		{
			IKnowledge knowledge = currentKnowledges.Find((IKnowledge x) => x.CardId == element.MyKnowledge.CardId);
			if (knowledge == null)
			{
				((Component)element).gameObject.SetActive(false);
				continue;
			}
			element.SetKnowledge(knowledge);
			if (KnowledgeWasFound(element.MyKnowledge))
			{
				if (element.IsNew)
				{
					dictionary[element.MyKnowledge.Group] = true;
				}
				if (!string.IsNullOrEmpty(searchTerm))
				{
					if (KnowledgeMatchesSearch(element.MyKnowledge, searchTerm))
					{
						((Component)element).gameObject.SetActive(true);
						continue;
					}
				}
				else if (dictionary.ContainsKey(element.MyKnowledge.Group) && dictionary[element.MyKnowledge.Group])
				{
					((Component)element).gameObject.SetActive(true);
					continue;
				}
			}
			((Component)element).gameObject.SetActive(false);
		}
		foreach (ExpandableLabel ideaLabel in ideaLabels)
		{
			ideaLabel.SetText(GetBlueprintGroupText((BlueprintGroup)ideaLabel.Tag));
			if (ideaLabel.Children.Count((GameObject x) => HasFoundKnowledge(x, out var _) && currentKnowledges.Contains(knowledge2)) > 0)
			{
				if (string.IsNullOrEmpty(searchTerm))
				{
					((Component)ideaLabel).gameObject.SetActive(true);
					ideaLabel.IsExpanded = dictionary.ContainsKey(ideaLabel.Tag) && dictionary[ideaLabel.Tag];
					continue;
				}
				if (ideaLabel.Children.Count((GameObject x) => HasFoundKnowledge(x, out knowledge2) && KnowledgeMatchesSearch(x.GetComponent<IdeaElement>().MyKnowledge, searchTerm) && currentKnowledges.Contains(x.GetComponent<IdeaElement>().MyKnowledge)) > 0)
				{
					((Component)ideaLabel).gameObject.SetActive(true);
					ideaLabel.IsExpanded = true;
					continue;
				}
			}
			((Component)ideaLabel).gameObject.SetActive(false);
		}
		foundCount = ideaElements.Where((IdeaElement x) => KnowledgeWasFound(x.MyKnowledge)).Count();
		ideaButtons = ((Component)IdeaElementsParent).GetComponentsInChildren<CustomButton>().ToList();
		for (int num = 0; num < ideaButtons.Count - 1; num++)
		{
			ideaButtons[num].ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
			{
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				int num2 = ideaButtons.IndexOf(cb);
				if (num2 == 0)
				{
					((Navigation)(ref nav)).selectOnUp = (Selectable)(object)IdeasButton;
				}
				else if (((Component)ideaButtons[num2 - 1]).gameObject.activeInHierarchy)
				{
					((Navigation)(ref nav)).selectOnUp = (Selectable)(object)ideaButtons[num2 - 1];
				}
				else
				{
					((Navigation)(ref nav)).selectOnUp = (Selectable)(object)getFirstActiveFromIndexUp(num2 - 1, ideaButtons);
				}
				if (((Component)ideaButtons[num2 + 1]).gameObject.activeInHierarchy)
				{
					((Navigation)(ref nav)).selectOnDown = (Selectable)(object)ideaButtons[num2 + 1];
				}
				else
				{
					((Navigation)(ref nav)).selectOnDown = (Selectable)(object)getFirstActiveFromIndexDown(num2 + 1, ideaButtons);
				}
				return nav;
			};
		}
		((Component)NoIdeasYetText).gameObject.SetActive(foundCount == 0);
	}

	private bool HasFoundKnowledge(GameObject obj, out IKnowledge knowledge)
	{
		knowledge = null;
		IdeaElement component = obj.GetComponent<IdeaElement>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		knowledge = component.MyKnowledge;
		return KnowledgeWasFound(component.MyKnowledge);
	}

	private bool KnowledgeMatchesSearch(IKnowledge knowledge, string searchTerm)
	{
		string text;
		string value;
		if (ShouldKeepAccents())
		{
			text = knowledge.KnowledgeName.ToLower().Replace(" ", "");
			value = searchTerm.ToLower().Replace(" ", "");
		}
		else
		{
			text = RemoveAccents(knowledge.KnowledgeName.ToLower().Replace(" ", ""));
			value = RemoveAccents(searchTerm.ToLower().Replace(" ", ""));
		}
		if (text.Contains(value))
		{
			return true;
		}
		return false;
	}

	private bool ShouldKeepAccents()
	{
		if (!(SokLoc.instance.CurrentLanguage == "Chinese (Traditional)") && !(SokLoc.instance.CurrentLanguage == "Chinese (Simplified)") && !(SokLoc.instance.CurrentLanguage == "Japanese"))
		{
			return SokLoc.instance.CurrentLanguage == "Korean";
		}
		return true;
	}

	private static string RemoveAccents(string input)
	{
		string s = input.Normalize(NormalizationForm.FormKD);
		byte[] bytes = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(""), new DecoderReplacementFallback("")).GetBytes(s);
		return Encoding.ASCII.GetString(bytes);
	}

	private CustomButton getFirstActiveFromIndexDown(int index, List<CustomButton> buttonList)
	{
		for (int i = index; i < buttonList.Count - 1; i++)
		{
			if (((Component)buttonList[i]).gameObject.activeInHierarchy)
			{
				return buttonList[i];
			}
		}
		return null;
	}

	private CustomButton getFirstActiveFromIndexUp(int index, List<CustomButton> buttonList)
	{
		for (int num = index; num >= 0; num--)
		{
			if (((Component)buttonList[num]).gameObject.activeInHierarchy)
			{
				return buttonList[num];
			}
		}
		return null;
	}

	private bool KnowledgeWasFound(IKnowledge knowledge)
	{
		return WorldManager.instance.CurrentSave.FoundCardIds.Contains(knowledge.CardId);
	}

	private string GetBlueprintGroupText(BlueprintGroup group)
	{
		return SokLoc.Translate("ideagroup_" + group.ToString().ToLower());
	}

	private bool CompletedFirstAchievement()
	{
		return QuestManager.instance.QuestIsComplete(QuestManager.instance.AllQuests[0]);
	}

	public void SetControllerInUI(bool inUI)
	{
		if (ControllerIsInUI != inUI)
		{
			ControllerIsInUI = inUI;
			if (!ControllerIsInUI)
			{
				EventSystem.current.SetSelectedGameObject((GameObject)null);
				WorldManager.instance.SpeedUp = prePauseSpeed;
			}
			else
			{
				prePauseSpeed = WorldManager.instance.SpeedUp;
				WorldManager.instance.SpeedUp = 0f;
			}
		}
	}

	private void Update()
	{
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1559: Unknown result type (might be due to invalid IL or missing references)
		//IL_155e: Unknown result type (might be due to invalid IL or missing references)
		if (WorldManager.instance.CurseIsActive(CurseType.Happiness))
		{
			((Component)ShowInfoBoxHappiness).gameObject.SetActiveFast(active: true);
		}
		else
		{
			((Component)ShowInfoBoxHappiness).gameObject.SetActiveFast(active: false);
		}
		((Component)ShowInfoBoxEnergy).gameObject.SetActiveFast(active: false);
		if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			((Component)ViewRect).gameObject.SetActiveFast(active: true);
			((Component)SewageViewButton).gameObject.SetActiveFast(active: true);
			((Component)CalamityViewButton).gameObject.SetActiveFast(active: true);
			((Component)EnergyViewButton).gameObject.SetActiveFast(active: true);
			((Component)ShowInfoBoxMoney).gameObject.SetActiveFast(active: false);
			((Component)ShowInfoBoxDollar).gameObject.SetActiveFast(active: true);
			((Component)ShowInfoBoxWorker).gameObject.SetActiveFast(active: true);
			((Component)ShowInfoBoxWellbeing).gameObject.SetActiveFast(active: true);
		}
		else
		{
			if (WorldManager.instance.GetCardCount<RoadBuilder>() > 0)
			{
				((Component)ViewRect).gameObject.SetActiveFast(active: true);
				((Component)SewageViewButton).gameObject.SetActiveFast(active: false);
				((Component)CalamityViewButton).gameObject.SetActiveFast(active: false);
				((Component)EnergyViewButton).gameObject.SetActiveFast(active: false);
			}
			else
			{
				((Component)ViewRect).gameObject.SetActiveFast(active: false);
			}
			((Component)ShowInfoBoxMoney).gameObject.SetActiveFast(active: true);
			((Component)ShowInfoBoxDollar).gameObject.SetActiveFast(active: false);
			((Component)ShowInfoBoxWorker).gameObject.SetActiveFast(active: false);
			((Component)ShowInfoBoxWellbeing).gameObject.SetActiveFast(active: false);
		}
		questButtons.RemoveAll((CustomButton x) => (Object)(object)x == (Object)null);
		ideaButtons.RemoveAll((CustomButton x) => (Object)(object)x == (Object)null);
		foreach (GameObject divider in dividerList)
		{
			divider.SetActive(false);
		}
		if (InputController.instance.PanelCollapse_Triggered())
		{
			ToggleMinimize();
		}
		if (InputController.instance.ActivateUI_Triggered())
		{
			SetControllerInUI(!ControllerIsInUI);
		}
		if (!InputController.instance.CurrentSchemeIsController)
		{
			SetControllerInUI(inUI: false);
		}
		QuestsTabNew.gameObject.SetActiveFast(questElements.Any((AchievementElement x) => x.IsNew));
		IdeasTabNew.gameObject.SetActiveFast(ideaElements.Any((IdeaElement x) => x.IsNew && ((Component)x).gameObject.activeSelf));
		((Component)FoldedNewIcon).gameObject.SetActiveFast(isMinimized && (QuestsTabNew.gameObject.activeInHierarchy || IdeasTabNew.gameObject.activeInHierarchy));
		MinimizeButtonInfoBox.InfoBoxTitle = SokLoc.Translate("label_toggle_panel_title");
		MinimizeButtonInfoBox.InfoBoxText = SokLoc.Translate("label_toggle_panel_text", (LocParam[])(object)new LocParam[1] { Extensions.LocParam_Action("panel_collapse") });
		UpdateSidePanelPosition();
		((Component)MinimizeButton).transform.localScale = (Vector3)(isMinimized ? Vector3.one : new Vector3(-1f, 1f, 1f));
		((Graphic)QuestsButton.Image).color = (((Component)QuestsTab).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)IdeasButton.Image).color = (((Component)IdeasTab).gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((TMP_Text)MoneyText).text = $"{WorldManager.instance.GetGoldCount(includeInChest: true)} {Icons.Gold}";
		((TMP_Text)DollarText).text = $"{WorldManager.instance.GetDollarCount(includeInChest: true)}{Icons.Dollar}";
		int foodCount = WorldManager.instance.GetFoodCount();
		int requiredFoodCount = WorldManager.instance.GetRequiredFoodCount();
		int cardCount = WorldManager.instance.GetCardCount();
		int maxCardCount = WorldManager.instance.GetMaxCardCount();
		int happinessCount = WorldManager.instance.GetHappinessCount();
		int requiredHappinessCount = WorldManager.instance.GetRequiredHappinessCount();
		int wellbeing = CitiesManager.instance.Wellbeing;
		CityState cityState = CitiesManager.instance.CityState;
		string text = GameCanvas.FormatTime(WorldManager.instance.MonthTime - WorldManager.instance.MonthTimer);
		ShowInfoBoxTime.InfoBoxTitle = SokLoc.Translate("label_time");
		ShowInfoBoxTime.InfoBoxText = SokLoc.Translate("label_time_infobox", (LocParam[])(object)new LocParam[3]
		{
			LocParam.Create("time_left", text.ToString()),
			Extensions.LocParam_Action("time_pause"),
			Extensions.LocParam_Action("time_toggle")
		});
		ShowInfoBoxEnergyButton.InfoBoxTitle = SokLoc.Translate("label_energy_view");
		ShowInfoBoxEnergyButton.InfoBoxText = SokLoc.Translate("label_energy_view_infobox", (LocParam[])(object)new LocParam[1] { Extensions.LocParam_Action("toggle_view") });
		((TMP_Text)FoodText).text = $"{foodCount}/{requiredFoodCount} {Icons.Food}";
		ShowInfoBoxMoney.InfoBoxTitle = SokLoc.Translate("label_coin_infobox_title");
		ShowInfoBoxMoney.InfoBoxText = SokLoc.Translate("label_coin_infobox_text");
		ShowInfoBoxFood.InfoBoxTitle = SokLoc.Translate("cardtype_food");
		ShowInfoBoxFood.InfoBoxText = SokLoc.Translate("label_food_infobox", (LocParam[])(object)new LocParam[3]
		{
			LocParam.Create("foodicon", Icons.Food),
			LocParam.Create("required_food_count", requiredFoodCount.ToString()),
			LocParam.Create("food_count", foodCount.ToString())
		});
		((TMP_Text)CardText).text = $"{cardCount}/{maxCardCount} {Icons.Card}";
		ShowInfoBoxCard.InfoBoxTitle = SokLoc.Translate("label_card_cap");
		ShowInfoBoxCard.InfoBoxText = SokLoc.Translate("label_cards_infobox", (LocParam[])(object)new LocParam[3]
		{
			LocParam.Create("cardicon", Icons.Card),
			LocParam.Create("card_count", cardCount.ToString()),
			LocParam.Create("max_card_count", maxCardCount.ToString())
		});
		if (WorldManager.instance.ForestMoonEnabled)
		{
			FoodCardBox.SetActiveFast(active: false);
		}
		else
		{
			FoodCardBox.SetActiveFast(active: true);
		}
		if (foodCount < requiredFoodCount || cardCount > maxCardCount || WorldManager.instance.DebugNoFoodEnabled || WorldManager.instance.DebugNoEnergyEnabled || wellbeing < 40 || (WorldManager.instance.CurseIsActive(CurseType.Happiness) && happinessCount < requiredHappinessCount))
		{
			redTextBlinkTimer += Time.deltaTime;
			if (redTextBlinkTimer >= 0.5f)
			{
				redTextBlinkTimer = 0f;
				redBlink = !redBlink;
			}
		}
		else
		{
			redTextBlinkTimer = 0f;
			redBlink = false;
		}
		if (foodCount < requiredFoodCount || WorldManager.instance.DebugNoFoodEnabled)
		{
			((Graphic)FoodText).color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			ShowInfoBox showInfoBoxFood = ShowInfoBoxFood;
			showInfoBoxFood.InfoBoxText = showInfoBoxFood.InfoBoxText + ". " + SokLoc.Translate("label_food_infobox_warning", (LocParam[])(object)new LocParam[1] { LocParam.Create("foodicon", Icons.Food) });
		}
		else
		{
			((Graphic)FoodText).color = ColorManager.instance.TextColor;
		}
		if (cardCount > maxCardCount)
		{
			((Graphic)CardText).color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			ShowInfoBox showInfoBoxCard = ShowInfoBoxCard;
			showInfoBoxCard.InfoBoxText = showInfoBoxCard.InfoBoxText + ". " + SokLoc.Translate("label_cards_infobox_warning", (LocParam[])(object)new LocParam[1] { LocParam.Create("cardicon", Icons.Card) });
		}
		else
		{
			((Graphic)CardText).color = ColorManager.instance.TextColor;
		}
		if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			((TMP_Text)TimeText).text = SokLoc.Translate("label_timetext_peaceful", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", WorldManager.instance.CurrentMonth.ToString()) });
		}
		else if (WorldManager.instance.ForestMoonEnabled)
		{
			string text2 = SokLoc.Translate("label_timetext", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", "??") });
			string text3 = SokLoc.Translate("label_wave", (LocParam[])(object)new LocParam[1] { LocParam.Create("wave", WorldManager.instance.CurrentRunVariables.ForestWave.ToString()) });
			((TMP_Text)TimeText).text = text2 + " - " + text3;
		}
		else
		{
			((TMP_Text)TimeText).text = SokLoc.Translate("label_timetext", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", WorldManager.instance.CurrentMonth.ToString()) });
		}
		if (((Behaviour)HappinessText).isActiveAndEnabled)
		{
			((TMP_Text)HappinessText).text = $"{happinessCount}/{requiredHappinessCount} {Icons.Happiness}";
			ShowInfoBoxHappiness.InfoBoxTitle = SokLoc.Translate("cardtype_happiness");
			ShowInfoBoxHappiness.InfoBoxText = SokLoc.Translate("label_happiness_infobox", (LocParam[])(object)new LocParam[3]
			{
				LocParam.Create("happinessicon", Icons.Happiness),
				LocParam.Create("required_happiness_count", requiredHappinessCount.ToString()),
				LocParam.Create("happiness_count", happinessCount.ToString())
			});
			if (happinessCount < requiredHappinessCount || WorldManager.instance.DebugNoFoodEnabled)
			{
				((Graphic)HappinessText).color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
				ShowInfoBox showInfoBoxHappiness = ShowInfoBoxHappiness;
				showInfoBoxHappiness.InfoBoxText = showInfoBoxHappiness.InfoBoxText + ". " + SokLoc.Translate("label_happiness_infobox_warning", (LocParam[])(object)new LocParam[1] { LocParam.Create("happinessicon", Icons.Happiness) });
			}
			else
			{
				((Graphic)HappinessText).color = ColorManager.instance.TextColor;
			}
			HappinessSummaryText = ShowInfoBoxHappiness.InfoBoxText;
		}
		if (((Behaviour)WorkerText).isActiveAndEnabled)
		{
			int num = CitiesManager.instance.HousingConsumers.Sum((HousingConsumer x) => x.GetHousingSpaceRequired());
			int num2 = WorldManager.instance.GetCards<Apartment>().Sum((Apartment x) => x.HousingSpace);
			((TMP_Text)WorkerText).text = $"{num}/{num2}{Icons.Housing}";
			ShowInfoBoxWorker.InfoBoxTitle = SokLoc.Translate("label_info_worker_space_title");
			ShowInfoBoxWorker.InfoBoxText = SokLoc.Translate("label_info_worker_space_text", (LocParam[])(object)new LocParam[3]
			{
				LocParam.Create("workers", num.ToString()),
				LocParam.Create("space", num2.ToString()),
				LocParam.Create("icon", Icons.Housing)
			});
			if (num > num2)
			{
				ShowInfoBox showInfoBoxWorker = ShowInfoBoxWorker;
				showInfoBoxWorker.InfoBoxText = showInfoBoxWorker.InfoBoxText + ". " + SokLoc.Translate("label_info_worker_space_text_1", (LocParam[])(object)new LocParam[1] { LocParam.Create("icon", Icons.Housing) });
				((Graphic)WorkerText).color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			}
			else
			{
				((Graphic)WorkerText).color = ColorManager.instance.TextColor;
			}
		}
		if (((Behaviour)WellbeingText).isActiveAndEnabled)
		{
			ShowInfoBoxWellbeing.InfoBoxTitle = SokLoc.Translate("label_wellbeing");
			ShowInfoBoxWellbeing.InfoBoxText = SokLoc.Translate("label_wellbeing_infobox_" + cityState.ToString().ToLower());
			((TMP_Text)WellbeingText).text = $"{wellbeing} {Icons.Wellbeing}";
			if (CitiesManager.instance.Wellbeing < 10)
			{
				((Graphic)WellbeingText).color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			}
			else
			{
				((Graphic)WellbeingText).color = ColorManager.instance.TextColor;
			}
		}
		if (((Component)ShowInfoBoxDollar).gameObject.activeSelf)
		{
			ShowInfoBoxDollar.InfoBoxTitle = SokLoc.Translate("label_dollar");
			ShowInfoBoxDollar.InfoBoxText = SokLoc.Translate("label_dollar_infobox", (LocParam[])(object)new LocParam[2]
			{
				LocParam.Create("amount", WorldManager.instance.GetDollarCount(includeInChest: true).ToString()),
				LocParam.Create("icon", Icons.Dollar)
			});
		}
		TimeFill.fillAmount = (WorldManager.instance.ForestMoonEnabled ? 0f : (WorldManager.instance.MonthTimer / WorldManager.instance.MonthTime));
		BoosterpackData boosterpackData = QuestManager.instance.NextPackUnlock();
		((Component)NextPackToUnlockText).gameObject.SetActiveFast((Object)(object)boosterpackData != (Object)null && CompletedFirstAchievement());
		if ((Object)(object)boosterpackData != (Object)null)
		{
			int num3 = QuestManager.instance.RemainingQuestCountToComplete(boosterpackData);
			((TMP_Text)NextPackToUnlockText).text = SokLoc.Translate("label_complete_more_quests", (LocParam[])(object)new LocParam[1] { LocParam.Plural("remaining", num3) });
		}
		GameCard gameCard = null;
		if ((Object)(object)WorldManager.instance.DraggingCard != (Object)null)
		{
			gameCard = WorldManager.instance.DraggingCard;
		}
		else if ((Object)(object)WorldManager.instance.HoveredCard != (Object)null)
		{
			gameCard = WorldManager.instance.HoveredCard;
		}
		string text4 = "";
		if ((Object)(object)gameCard == (Object)null)
		{
			Boosterpack boosterpack = null;
			if (WorldManager.instance.DraggingDraggable is Boosterpack)
			{
				boosterpack = WorldManager.instance.DraggingDraggable as Boosterpack;
			}
			else if (WorldManager.instance.HoveredDraggable is Boosterpack)
			{
				boosterpack = WorldManager.instance.HoveredDraggable as Boosterpack;
			}
			if ((Object)(object)boosterpack != (Object)null)
			{
				((TMP_Text)InfoTitle).text = boosterpack.Name ?? "";
				((TMP_Text)InfoText).text = SokLoc.Translate("label_click_this_pack");
			}
			else
			{
				((TMP_Text)InfoTitle).text = "";
				((TMP_Text)InfoText).text = "";
			}
		}
		else if ((Object)(object)WorldManager.instance.CurrentHoverable == (Object)null)
		{
			CardValue stackValue = WorldManager.instance.GetStackValue(gameCard);
			List<GameCard> allCardsInStack = gameCard.GetAllCardsInStack();
			if (!gameCard.IsPartOfStack())
			{
				((TMP_Text)InfoTitle).text = gameCard.CardData.FullName;
				string text5 = gameCard.CardData.Description;
				if (gameCard.CardData.RequirementHolders.Count > 0 && WorldManager.instance.GetCurrentBoardSafe().Id == "cities")
				{
					string requirementDescription = gameCard.CardData.GetRequirementDescription(gameCard, 1, onlyShowCurrentlySatisfied: false);
					if (!string.IsNullOrEmpty(requirementDescription))
					{
						text5 = text5 + "\\d<i>" + SokLoc.Translate("label_at_end_moon") + "</i>\n" + requirementDescription;
					}
				}
				((TMP_Text)InfoText).text = text5;
				GameCard cardWithStatusInStack = gameCard.GetCardWithStatusInStack();
				if ((Object)(object)cardWithStatusInStack != (Object)null)
				{
					((TMP_Text)InfoTitle).text = cardWithStatusInStack.Status + "..";
					((TMP_Text)InfoText).text = GameCanvas.FormatTimeLeft(cardWithStatusInStack.TargetTimerTime - cardWithStatusInStack.CurrentTimerTime) + " \n\n" + gameCard.CardData.Description;
				}
				else if (gameCard.CardData.GetValue() > 0)
				{
					if (!(WorldManager.instance.NearbyCardTarget is SellBox))
					{
						if (WorldManager.instance.NearbyCardTarget is BuyBoosterBox)
						{
							_ = (BuyBoosterBox)WorldManager.instance.NearbyCardTarget;
						}
						else
						{
							text4 = stackValue.ToValueString(WorldManager.instance.CurrentBoard);
						}
					}
				}
				else if (gameCard.CardData.GetValue() == -1)
				{
					text4 = SokLoc.Translate("label_cant_be_sold");
				}
			}
			else
			{
				GameCard cardWithStatusInStack2 = gameCard.GetCardWithStatusInStack();
				GameCard cardInCombatInStack = gameCard.GetCardInCombatInStack();
				if ((Object)(object)cardWithStatusInStack2 != (Object)null)
				{
					((TMP_Text)InfoTitle).text = cardWithStatusInStack2.Status + "..";
					((TMP_Text)InfoText).text = GameCanvas.FormatTimeLeft(cardWithStatusInStack2.TargetTimerTime - cardWithStatusInStack2.CurrentTimerTime);
				}
				else if (Object.op_Implicit((Object)(object)cardInCombatInStack))
				{
					((TMP_Text)InfoTitle).text = gameCard.CardData.FullName;
					((TMP_Text)InfoText).text = gameCard.CardData.Description;
				}
				else
				{
					((TMP_Text)InfoTitle).text = SokLoc.Translate("label_stack_of_cards");
					((TMP_Text)InfoText).text = gameCard.GetStackSummary();
					string text6 = "";
					if (allCardsInStack.Any((GameCard x) => x.CardData.RequirementHolders.Count > 0) && WorldManager.instance.GetCurrentBoardSafe().Id == "cities")
					{
						stackRequirements.Clear();
						stackRequirementAmount.Clear();
						foreach (GameCard item in allCardsInStack)
						{
							if (stackRequirements.ContainsKey(item.CardData.Id))
							{
								stackRequirementAmount[item.CardData.Id]++;
							}
							else if (item.CardData.RequirementHolders.Count > 0)
							{
								stackRequirements[item.CardData.Id] = item.CardData;
								stackRequirementAmount[item.CardData.Id] = 1;
							}
						}
						bool flag = true;
						bool flag2 = stackRequirements.Count > 1;
						foreach (CardData value in stackRequirements.Values)
						{
							int num4 = stackRequirementAmount[value.Id];
							string requirementDescription2 = value.GetRequirementDescription(value.MyGameCard, num4, onlyShowCurrentlySatisfied: false);
							if (!string.IsNullOrEmpty(requirementDescription2))
							{
								if (flag)
								{
									flag = false;
									text6 = text6 + "\\d<i>" + SokLoc.Translate("label_at_end_moon") + "</i>";
								}
								else
								{
									text6 += "\n";
								}
								if (flag2)
								{
									string text7 = ((num4 != 1) ? $"{num4}x {SokLoc.Translate(value.NameTerm)}" : (value.Name ?? ""));
									text6 = text6 + "\n<i>(" + text7 + ")</i>\n" + requirementDescription2;
								}
								else
								{
									text6 = text6 + "\n" + requirementDescription2;
								}
							}
						}
					}
					TextMeshProUGUI infoText = InfoText;
					((TMP_Text)infoText).text = ((TMP_Text)infoText).text + text6;
					CardData cardData = null;
					foreach (GameCard item2 in allCardsInStack)
					{
						if (item2.CardData.GetValue() == -1)
						{
							cardData = item2.CardData;
						}
					}
					if ((Object)(object)cardData != (Object)null)
					{
						text4 = SokLoc.Translate("label_cant_be_sold");
						if (WorldManager.instance.NearbyCardTarget is BuyBoosterBox)
						{
							_ = (BuyBoosterBox)WorldManager.instance.NearbyCardTarget;
						}
					}
					else if (WorldManager.instance.NearbyCardTarget is SellBox)
					{
						TextMeshProUGUI infoText2 = InfoText;
						((TMP_Text)infoText2).text = ((TMP_Text)infoText2).text + "\n" + SokLoc.Translate("label_drop_to_sell", (LocParam[])(object)new LocParam[1] { LocParam.Create("value", stackValue.ToValueString(WorldManager.instance.CurrentBoard)) });
					}
					else
					{
						text4 = stackValue.ToValueString(WorldManager.instance.CurrentBoard);
					}
				}
			}
			if (((TMP_Text)InfoText).text.Contains("\\d"))
			{
				string[] array = ((TMP_Text)InfoText).text.Split(new string[1] { "\\d" }, StringSplitOptions.None);
				for (int num5 = 0; num5 < array.Length; num5++)
				{
					string text8 = array[num5];
					if (num5 == 0)
					{
						((TMP_Text)InfoText).text = text8;
						continue;
					}
					GameObject obj = FindOrInstantiateDivider();
					obj.SetActiveFast(active: true);
					TextMeshProUGUI componentInChildren = obj.GetComponentInChildren<TextMeshProUGUI>();
					if ((Object)(object)componentInChildren != (Object)null)
					{
						((TMP_Text)componentInChildren).text = text8;
					}
				}
			}
		}
		if (ControllerIsInUI)
		{
			((TMP_Text)InfoText).text = InfoBoxText;
			((TMP_Text)InfoTitle).text = InfoBoxTitle;
		}
		if ((Object)(object)WorldManager.instance.CurrentHoverable != (Object)null)
		{
			((TMP_Text)InfoTitle).text = (InfoBoxTitle = WorldManager.instance.CurrentHoverable.GetTitle());
			((TMP_Text)InfoText).text = (InfoBoxText = WorldManager.instance.CurrentHoverable.GetDescription());
			text4 = null;
		}
		if (InputController.instance.CurrentSchemeIsMouseKeyboard && InputController.instance.InputCount > 0 && (!InputController.instance.GetInput(0) || !GameCanvas.instance.PositionIsOverUI(InputController.instance.GetInputPosition(0))))
		{
			CloseViewDropdown();
		}
		((TMP_Text)ViewButton.TextMeshPro).text = GetLabelForViewType(WorldManager.instance.CurrentView) + GetIconForView(WorldManager.instance.CurrentView);
		((Component)Crosshair).gameObject.SetActiveFast(InputController.instance.CurrentSchemeIsController);
		((Component)IdeaSearchField).gameObject.SetActiveFast(foundCount > 0 && !InputController.instance.CurrentSchemeIsController);
		ValueParent.gameObject.SetActiveFast(!string.IsNullOrEmpty(text4));
		((TMP_Text)Valuetext).text = text4;
		bool flag3 = true;
		if (WorldManager.instance.InAnimation || GameCanvas.instance.ModalIsOpen)
		{
			flag3 = false;
		}
		if (flag3)
		{
			if (InputController.instance.TimeToggleTriggered())
			{
				if (WorldManager.instance.SpeedUp == 0f)
				{
					WorldManager.instance.SpeedUp = 1f;
				}
				else if (WorldManager.instance.SpeedUp == 1f)
				{
					WorldManager.instance.SpeedUp = 5f;
				}
				else if (WorldManager.instance.SpeedUp == 5f)
				{
					WorldManager.instance.SpeedUp = 1f;
				}
			}
			if (gameSpeedButtonClicked)
			{
				gameSpeedButtonClicked = false;
				if (WorldManager.instance.SpeedUp == 0f)
				{
					WorldManager.instance.SpeedUp = 1f;
				}
				else if (WorldManager.instance.SpeedUp == 1f)
				{
					WorldManager.instance.SpeedUp = 5f;
				}
				else if (WorldManager.instance.SpeedUp == 5f)
				{
					WorldManager.instance.SpeedUp = 0f;
				}
			}
			if (InputController.instance.Time1_Triggered())
			{
				WorldManager.instance.SpeedUp = 1f;
			}
			if (InputController.instance.Time2_Triggered())
			{
				WorldManager.instance.SpeedUp = 5f;
			}
			if (InputController.instance.Time3_Triggered())
			{
				prePauseSpeed = WorldManager.instance.SpeedUp;
				WorldManager.instance.SpeedUp = 0f;
			}
			if (InputController.instance.TimePauseTriggered())
			{
				TimePause();
			}
		}
		if (WorldManager.instance.SpeedUp == 5f)
		{
			GameSpeedIcon.sprite = SpriteManager.instance.Speed10;
		}
		else if (WorldManager.instance.SpeedUp == 1f)
		{
			GameSpeedIcon.sprite = SpriteManager.instance.Speed1;
		}
		else if (WorldManager.instance.SpeedUp == 0f)
		{
			GameSpeedIcon.sprite = SpriteManager.instance.Speed0;
		}
		bool flag4 = WorldManager.instance.SpeedUp == 0f;
		((Component)PausedText).gameObject.SetActive(flag4);
		if (flag4)
		{
			QuestManager.instance.SpecialActionComplete("pause_game");
			pauseBlinkTimer += Time.deltaTime;
			if (pauseBlinkTimer >= 0.5f)
			{
				((Behaviour)PausedText).enabled = !((Behaviour)PausedText).enabled || !AccessibilityScreen.FlashingPausedEnabled;
				pauseBlinkTimer = 0f;
			}
		}
		else
		{
			pauseBlinkTimer = 0f;
		}
		_ = previousInfoText != ((TMP_Text)InfoText).text;
		previousInfoText = ((TMP_Text)InfoText).text;
	}

	private GameObject FindOrInstantiateDivider()
	{
		foreach (GameObject divider in dividerList)
		{
			if (!divider.activeSelf)
			{
				return divider;
			}
		}
		GameObject val = Object.Instantiate<GameObject>(InfoDividerPrefab, (Transform)(object)InfoLayoutGroup);
		dividerList.Add(val);
		return val;
	}

	public void UpdateSidePanelPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 anchoredPosition = SideTransform.anchoredPosition;
		anchoredPosition.x = (isMinimized ? (-340f) : 5f);
		SideTransform.anchoredPosition = anchoredPosition;
	}

	public void TimePause()
	{
		if (WorldManager.instance.SpeedUp >= 1f)
		{
			prePauseSpeed = WorldManager.instance.SpeedUp;
			WorldManager.instance.SpeedUp = 0f;
		}
		else
		{
			WorldManager.instance.SpeedUp = (ControllerIsInUI ? prePauseSpeed : ((prePauseSpeed != 0f) ? prePauseSpeed : 1f));
		}
	}

	public void AddNotification(string title, string text, Action onClicked = null)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		NotificationElement notificationElement = Object.Instantiate<NotificationElement>(PrefabManager.instance.NotificationElementPrefab);
		((Component)notificationElement).transform.SetParent((Transform)(object)NotificationsParent);
		((Component)notificationElement).transform.localScale = Vector3.one;
		((Component)notificationElement).transform.localPosition = Vector3.zero;
		((Component)notificationElement).transform.localRotation = Quaternion.identity;
		((TMP_Text)notificationElement.NotificationText).text = text;
		((TMP_Text)notificationElement.NotificationTitle).text = title;
		notificationElement.OnClicked = onClicked;
		if (((Transform)NotificationsParent).childCount > 5)
		{
			Object.Destroy((Object)(object)((Component)((Transform)NotificationsParent).GetChild(0)).gameObject);
		}
	}

	private void LateUpdate()
	{
		if (((TMP_Text)InfoTitle).text == "")
		{
			((TMP_Text)InfoTitle).text = InfoBoxTitle;
			((TMP_Text)InfoText).text = InfoBoxText;
		}
		InfoBoxText = "";
		InfoBoxTitle = "";
	}
}
