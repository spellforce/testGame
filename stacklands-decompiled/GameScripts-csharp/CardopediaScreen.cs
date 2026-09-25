using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardopediaScreen : SokScreen
{
	public RectTransform EntriesParent;

	public CustomButton BackButton;

	public ScrollRect ScrollRect;

	public TMP_InputField SearchField;

	public ExpandableLabelCardopedia LabelPrefab;

	public CardopediaEntryElement CardopediaEntryPrefab;

	public CardopediaEntryElement HoveredEntry;

	public TextMeshProUGUI CardFoundAmount;

	public TextMeshProUGUI CardDescription;

	public Transform TargetCardPos;

	public Transform CardopediaBackground;

	public CustomButton All;

	public CustomButton Main;

	public CustomButton Island;

	public CustomButton Forest;

	public CustomButton Order;

	public CustomButton Spirit;

	public CustomButton Cities;

	public CustomButton Modded;

	private List<CustomButton> tabButtons;

	private CustomButton activeTab;

	private CardUpdateType? activeCardUpdateType;

	private List<CardopediaEntryElement> entries = new List<CardopediaEntryElement>();

	private List<ExpandableLabelCardopedia> labels = new List<ExpandableLabelCardopedia>();

	private CardopediaEntryElement lastHoveredEntry;

	private GameCard demoCard;

	private List<object> listChildren = new List<object>();

	public static CardopediaScreen instance;

	private bool SearchDisabled;

	private int totalFoundCount;

	private int currentTotalCardCount;

	public bool IsSearching => !string.IsNullOrEmpty(SearchField.text);

	private void Awake()
	{
		instance = this;
		tabButtons = new List<CustomButton> { All, Main, Island, Forest, Order, Spirit, Cities, Modded };
		BackButton.Clicked += delegate
		{
			((Component)CardopediaBackground).gameObject.SetActive(false);
			ClearScreen();
			if (WorldManager.instance.CurrentGameState == WorldManager.GameState.InMenu)
			{
				GameCanvas.instance.SetScreen<MainMenu>();
			}
			else
			{
				GameCanvas.instance.SetScreen<PauseScreen>();
			}
		};
		((UnityEvent<string>)(object)SearchField.onValueChanged).AddListener((UnityAction<string>)delegate(string value)
		{
			FilterEntries();
			foreach (ExpandableLabelCardopedia label in labels)
			{
				if (GetActiveLabelChildrenCount(label) > 0 && !string.IsNullOrEmpty(value))
				{
					label.SetExpanded(expanded: true);
					label.ShowChildrenCardopedia();
				}
			}
		});
		SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
		AddTabListeners();
		CardopediaBackground = ((Component)GameCamera.instance).transform.Find("CardopediaBackground");
		TargetCardPos = ((Component)GameCamera.instance).transform.Find("TargetCardPos");
		((Component)CardopediaBackground).gameObject.SetActive(false);
		CreateEntries();
		if (!PlatformHelper.HasModdingSupport)
		{
			((Component)Modded).gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)SokLoc.instance != (Object)null)
		{
			SokLoc.instance.LanguageChanged -= Instance_LanguageChanged;
		}
	}

	private void Instance_LanguageChanged()
	{
		foreach (CardopediaEntryElement entry in entries)
		{
			entry.UpdateText();
		}
		if ((Object)(object)demoCard != (Object)null)
		{
			demoCard.CardData.OnLanguageChange();
		}
		UpdateLabels();
	}

	public void RefreshCardopedia()
	{
		foreach (CardopediaEntryElement entry in entries)
		{
			entry.SetCardData(entry.MyCardData);
			entry.UpdateText();
		}
		UpdateLabels();
	}

	private void OnEnable()
	{
		RefreshCardopedia();
		((Component)((TMP_Text)CardDescription).transform.parent).gameObject.SetActive(false);
		((Component)CardopediaBackground).gameObject.SetActive(true);
		totalFoundCount = DetermineFoundCount();
		SwitchActiveTab(All);
		ScrollRect.verticalNormalizedPosition = 1f;
	}

	private int DetermineFoundCount(CardUpdateType? updateType = null)
	{
		List<string> foundCardIds = WorldManager.instance.CurrentSave.FoundCardIds;
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in foundCardIds)
		{
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
			}
		}
		int num = 0;
		List<CardData> list = WorldManager.instance.CardDataPrefabs;
		if (updateType.HasValue)
		{
			list = list.Where((CardData x) => x.CardUpdateType == updateType).ToList();
		}
		foreach (CardData item2 in list)
		{
			if (!item2.HideFromCardopedia && hashSet.Contains(item2.Id))
			{
				num++;
			}
		}
		return num;
	}

	private void AddTabListeners()
	{
		if ((Object)(object)activeTab == (Object)null)
		{
			SwitchActiveTab(All);
		}
		foreach (CustomButton tab in tabButtons)
		{
			tab.Clicked += delegate
			{
				SwitchActiveTab(tab);
			};
			tab.ExplicitNavigationChanged += delegate(CustomButton but, Navigation nav)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				((Navigation)(ref nav)).selectOnUp = null;
				((Navigation)(ref nav)).selectOnDown = GetFirstSelectableInList();
				return nav;
			};
		}
	}

	private Selectable GetFirstSelectableInList()
	{
		return (Selectable)(object)labels.FirstOrDefault((ExpandableLabelCardopedia x) => ((Component)x).gameObject.activeInHierarchy).MyButton;
	}

	private void SwitchActiveTab(CustomButton tab)
	{
		activeTab = tab;
		ScrollRect.verticalScrollbarVisibility = (ScrollbarVisibility)0;
		SearchDisabled = false;
		((Component)CardFoundAmount).gameObject.SetActive(true);
		if ((Object)(object)activeTab == (Object)(object)All)
		{
			activeCardUpdateType = null;
			FilterEntriesCardUpdateType(null);
		}
		else if ((Object)(object)activeTab == (Object)(object)Main)
		{
			activeCardUpdateType = CardUpdateType.Main;
			FilterEntriesCardUpdateType(CardUpdateType.Main);
		}
		else if ((Object)(object)activeTab == (Object)(object)Island)
		{
			activeCardUpdateType = CardUpdateType.Island;
			FilterEntriesCardUpdateType(CardUpdateType.Island);
		}
		else if ((Object)(object)activeTab == (Object)(object)Forest)
		{
			activeCardUpdateType = CardUpdateType.Forest;
			FilterEntriesCardUpdateType(CardUpdateType.Forest);
		}
		else if ((Object)(object)activeTab == (Object)(object)Order)
		{
			activeCardUpdateType = CardUpdateType.Order;
			FilterEntriesCardUpdateType(CardUpdateType.Order);
		}
		else if ((Object)(object)activeTab == (Object)(object)Spirit)
		{
			activeCardUpdateType = CardUpdateType.Spirit;
			FilterEntriesCardUpdateType(CardUpdateType.Spirit);
			if (!WorldManager.instance.IsSpiritDlcActive())
			{
				SetTempDemoCard(WorldManager.instance.CardDataPrefabs.Find((CardData x) => x.Id == "card_display_spirit_dlc"));
				ScrollRect.verticalScrollbarVisibility = (ScrollbarVisibility)1;
				SearchDisabled = true;
				((Component)CardFoundAmount).gameObject.SetActive(false);
			}
		}
		else if ((Object)(object)activeTab == (Object)(object)Cities)
		{
			activeCardUpdateType = CardUpdateType.Cities;
			FilterEntriesCardUpdateType(CardUpdateType.Cities);
			if (!WorldManager.instance.IsCitiesDlcActive())
			{
				SetTempDemoCard(WorldManager.instance.CardDataPrefabs.Find((CardData x) => x.Id == "display_2000_dlc"));
				ScrollRect.verticalScrollbarVisibility = (ScrollbarVisibility)1;
				SearchDisabled = true;
				((Component)CardFoundAmount).gameObject.SetActive(false);
			}
		}
		else if ((Object)(object)activeTab == (Object)(object)Modded)
		{
			activeCardUpdateType = CardUpdateType.Mod;
			FilterEntriesCardUpdateType(CardUpdateType.Mod);
		}
		currentTotalCardCount = WorldManager.instance.CardDataPrefabs.Count((CardData x) => !x.HideFromCardopedia);
		if ((Object)(object)activeTab != (Object)(object)All)
		{
			currentTotalCardCount = WorldManager.instance.CardDataPrefabs.Count((CardData x) => !x.HideFromCardopedia && x.CardUpdateType == activeCardUpdateType);
			SearchField.text = "";
		}
	}

	private void FilterEntriesCardUpdateType(CardUpdateType? cardUpdateType)
	{
		foreach (CardopediaEntryElement entry in entries)
		{
			if (!cardUpdateType.HasValue || entry.MyCardData.CardUpdateType == cardUpdateType)
			{
				entry.IsFilteredUpdate = true;
			}
			else
			{
				entry.IsFilteredUpdate = false;
			}
		}
		UpdateLabels();
		UpdateEntries();
	}

	private void FilterEntries()
	{
		string text = SearchField.text;
		if (!string.IsNullOrEmpty(text))
		{
			if ((Object)(object)activeTab != (Object)(object)All)
			{
				SwitchActiveTab(All);
			}
			text = text.ToLower().Replace(" ", "");
			foreach (CardopediaEntryElement entry in entries)
			{
				if (entry.MyCardData.Name.ToLower().Replace(" ", "").Contains(text))
				{
					entry.IsFiltered = true;
				}
				else
				{
					entry.IsFiltered = false;
				}
			}
		}
		else
		{
			foreach (CardopediaEntryElement entry2 in entries)
			{
				entry2.IsFiltered = true;
			}
		}
		UpdateLabels();
	}

	public void UpdateLabels()
	{
		foreach (ExpandableLabelCardopedia label in labels)
		{
			label.ShowChildrenCardopedia();
			if (IsSearching)
			{
				if (GetActiveLabelChildrenCountSearch(label) > 0)
				{
					CardType type = label.Children[0].MyCardData.MyCardType;
					int num = WorldManager.instance.CardDataPrefabs.Count((CardData x) => x.MyCardType == type && !x.HideFromCardopedia);
					label.SetText(CardTypeToText(type) + $" ({GetActiveLabelChildrenCountSearch(label)}/{num})");
					((Component)label).gameObject.SetActive(true);
					continue;
				}
			}
			else if (label.Children.Count((CardopediaEntryElement x) => x.IsFilteredUpdate) > 0)
			{
				CardType type2 = label.Children[0].MyCardData.MyCardType;
				int num2 = (activeCardUpdateType.HasValue ? WorldManager.instance.CardDataPrefabs.Count((CardData x) => x.MyCardType == type2 && !x.HideFromCardopedia && x.CardUpdateType == activeCardUpdateType) : WorldManager.instance.CardDataPrefabs.Count((CardData x) => x.MyCardType == type2 && !x.HideFromCardopedia));
				label.SetText(CardTypeToText(type2) + $" ({GetActiveLabelChildrenCount(label)}/{num2})");
				((Component)label).gameObject.SetActive(true);
				continue;
			}
			((Component)label).gameObject.SetActive(false);
		}
		totalFoundCount = DetermineFoundCount(activeCardUpdateType);
	}

	private int GetActiveLabelChildrenCountSearch(ExpandableLabelCardopedia label)
	{
		return label.Children.Count((CardopediaEntryElement x) => x.IsFiltered && x.wasFound);
	}

	private int GetActiveLabelChildrenCount(ExpandableLabelCardopedia label)
	{
		return label.Children.Count((CardopediaEntryElement x) => x.IsFilteredUpdate && x.wasFound);
	}

	public void UpdateEntries()
	{
		float verticalNormalizedPosition = ScrollRect.verticalNormalizedPosition;
		FilterEntries();
		UpdatePositions();
		ScrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
		UpdatePositions();
	}

	private void CreateEntries()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		List<CardData> cardDataPrefabs = WorldManager.instance.CardDataPrefabs;
		cardDataPrefabs = (from x in cardDataPrefabs
			orderby x.MyCardType, x.FullName
			select x).ToList();
		cardDataPrefabs.RemoveAll((CardData x) => x.HideFromCardopedia);
		new List<Transform>();
		foreach (Transform item in (Transform)EntriesParent)
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
		ExpandableLabelCardopedia expandableLabelCardopedia = null;
		labels = new List<ExpandableLabelCardopedia>();
		entries.Clear();
		listChildren.Clear();
		for (int num = 0; num < cardDataPrefabs.Count; num++)
		{
			CardData c = cardDataPrefabs[num];
			if (num == 0 || cardDataPrefabs[num - 1].MyCardType != cardDataPrefabs[num].MyCardType)
			{
				ExpandableLabelCardopedia label = Object.Instantiate<ExpandableLabelCardopedia>(LabelPrefab);
				((Component)label).transform.SetParentClean((Transform)(object)EntriesParent);
				int num2 = cardDataPrefabs.Count((CardData x) => x.MyCardType == c.MyCardType);
				int num3 = cardDataPrefabs.Count((CardData x) => x.MyCardType == c.MyCardType && WorldManager.instance.CurrentSave.FoundCardIds.Contains(x.Id));
				label.SetText(CardTypeToText(cardDataPrefabs[num].MyCardType) + $" ({num3}/{num2})");
				label.Tag = cardDataPrefabs[num].MyCardType;
				label.SetCallback(delegate
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_0057: Unknown result type (might be due to invalid IL or missing references)
					//IL_005c: Unknown result type (might be due to invalid IL or missing references)
					//IL_006a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0091: Unknown result type (might be due to invalid IL or missing references)
					float num4 = 0f - ((Component)label).transform.localPosition.y - ((Transform)EntriesParent).localPosition.y;
					UpdateEntries();
					Vector3 localPosition = ((Component)EntriesParent).transform.localPosition;
					localPosition.y = 0f - ((Component)label).transform.localPosition.y - num4;
					((Component)EntriesParent).transform.localPosition = localPosition;
				});
				label.SetExpanded(expanded: false);
				label.MyButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
				{
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					if ((Object)(object)cb == (Object)(object)GetFirstSelectableInList())
					{
						((Navigation)(ref nav)).selectOnUp = (Selectable)(object)activeTab;
					}
					Selectable selectOnLeft = (((Navigation)(ref nav)).selectOnRight = null);
					((Navigation)(ref nav)).selectOnLeft = selectOnLeft;
					return nav;
				};
				listChildren.Add(label);
				labels.Add(label);
				expandableLabelCardopedia = label;
			}
			CardopediaEntryElement cardopediaEntryElement = Object.Instantiate<CardopediaEntryElement>(CardopediaEntryPrefab);
			((Component)cardopediaEntryElement).transform.SetParentClean((Transform)(object)EntriesParent);
			cardopediaEntryElement.SetCardData(c);
			cardopediaEntryElement.IsEnabled = false;
			cardopediaEntryElement.IsFiltered = false;
			cardopediaEntryElement.IsFilteredUpdate = true;
			cardopediaEntryElement.Button.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				Selectable selectOnLeft = (((Navigation)(ref nav)).selectOnRight = null);
				((Navigation)(ref nav)).selectOnLeft = selectOnLeft;
				return nav;
			};
			expandableLabelCardopedia.Children.Add(cardopediaEntryElement);
			entries.Add(cardopediaEntryElement);
			listChildren.Add(cardopediaEntryElement);
		}
		foreach (ExpandableLabelCardopedia l in labels)
		{
			if (entries.Any((CardopediaEntryElement e) => e.IsNew && e.MyCardData.MyCardType == (CardType)l.Tag))
			{
				l.SetExpanded(expanded: true);
			}
		}
	}

	private string CardTypeToText(CardType type)
	{
		return type.TranslateEnum();
	}

	private void OnDisable()
	{
		SearchField.text = string.Empty;
		ClearScreen();
	}

	private void ClearScreen()
	{
		if ((Object)(object)demoCard != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)demoCard).gameObject);
		}
		((Component)((TMP_Text)CardDescription).transform.parent).gameObject.SetActive(false);
		lastHoveredEntry = null;
		if ((Object)(object)CardopediaBackground != (Object)null)
		{
			((Component)CardopediaBackground).gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		HoveredEntry = null;
		if (GameCanvas.instance.ScreenIsInteractable<CardopediaScreen>())
		{
			foreach (CardopediaEntryElement entry in entries)
			{
				if (entry.Button.IsHovered || entry.Button.IsSelected)
				{
					HoveredEntry = entry;
				}
			}
		}
		if ((Object)(object)lastHoveredEntry != (Object)null)
		{
			((Graphic)lastHoveredEntry.Button.Image).color = ColorManager.instance.ButtonColor;
		}
		if ((Object)(object)HoveredEntry != (Object)null)
		{
			((Graphic)HoveredEntry.Button.Image).color = ColorManager.instance.HoverButtonColor;
		}
		UpdatePositions();
		if ((Object)(object)lastHoveredEntry != (Object)(object)HoveredEntry && (Object)(object)HoveredEntry != (Object)null)
		{
			if ((Object)(object)demoCard != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)demoCard).gameObject);
			}
			demoCard = Object.Instantiate<GameCard>(PrefabManager.instance.GameCardPrefab);
			CardData cardData = Object.Instantiate<CardData>(HoveredEntry.MyCardData);
			((Component)cardData).transform.SetParent(((Component)demoCard).transform);
			demoCard.CardData = cardData;
			cardData.MyGameCard = demoCard;
			demoCard.FaceUp = HoveredEntry.wasFound;
			demoCard.IsDemoCard = true;
			demoCard.SetDemoCardRotation();
			demoCard.CardData.UpdateCardText();
			demoCard.UpdateCardPalette();
			cardData.UpdateCard();
			demoCard.ForceUpdate();
		}
		if ((Object)(object)demoCard != (Object)null)
		{
			Vector3 position = TargetCardPos.position;
			((Component)demoCard).transform.position = (demoCard.TargetPosition = position);
		}
		if ((Object)(object)HoveredEntry != (Object)null)
		{
			((Component)((TMP_Text)CardDescription).transform.parent).gameObject.SetActive(true);
			if (HoveredEntry.wasFound)
			{
				demoCard.CardData.UpdateCardText();
				string dropSummaryFromCard = GetDropSummaryFromCard(HoveredEntry.MyCardData);
				string description = demoCard.CardData.Description;
				description = description.Replace("\\d", "\n\n");
				if (!string.IsNullOrEmpty(dropSummaryFromCard) && HoveredEntry.MyCardData.MyCardType != CardType.Locations)
				{
					description = description + "\n\n" + dropSummaryFromCard;
				}
				if (HoveredEntry.MyCardData is Blueprint blueprint)
				{
					description = blueprint.GetText();
				}
				((TMP_Text)CardDescription).text = description;
			}
			else
			{
				((TMP_Text)CardDescription).text = SokLoc.Translate("label_card_not_found");
			}
		}
		((Component)SearchField).gameObject.SetActive(!InputController.instance.CurrentSchemeIsController && !SearchDisabled);
		((TMP_Text)CardFoundAmount).text = SokLoc.Translate("label_cards_found", (LocParam[])(object)new LocParam[2]
		{
			LocParam.Create("found", totalFoundCount.ToString()),
			LocParam.Create("total", currentTotalCardCount.ToString())
		});
		lastHoveredEntry = HoveredEntry;
		UpdateTabs();
	}

	private void UpdateTabs()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		foreach (CustomButton tabButton in tabButtons)
		{
			if (((Component)tabButton).gameObject.activeInHierarchy)
			{
				bool flag = (Object)(object)tabButton == (Object)(object)activeTab;
				Color color = (tabButton.IsSelected ? ColorManager.instance.BackgroundColor2 : ((!flag) ? ColorManager.instance.InactiveBackgroundColor : ColorManager.instance.BackgroundColor));
				((Graphic)tabButton.Image).color = color;
			}
		}
	}

	public void UpdatePositions()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Vector2 sizeDelta = EntriesParent.sizeDelta;
		Vector2 val = Vector2.op_Implicit(((Transform)EntriesParent).localPosition);
		Rect rect = EntriesParent.rect;
		Rect rect2 = ((RectTransform)((Transform)EntriesParent).parent).rect;
		float height = ((Rect)(ref rect2)).height;
		float num2 = 0f - val.y - height * 0.5f;
		Vector3 position = default(Vector3);
		for (int i = 0; i < listChildren.Count; i++)
		{
			object obj = listChildren[i];
			bool flag = false;
			RectTransform val2 = null;
			if (obj is ExpandableLabelCardopedia expandableLabelCardopedia)
			{
				flag = ((Component)expandableLabelCardopedia).gameObject.activeInHierarchy;
				val2 = (RectTransform)((Component)expandableLabelCardopedia).transform;
			}
			if (obj is CardopediaEntryElement cardopediaEntryElement)
			{
				flag = cardopediaEntryElement.IsEnabled;
				val2 = (RectTransform)((Component)cardopediaEntryElement).transform;
				((Graphic)cardopediaEntryElement.Button.Image).raycastTarget = cardopediaEntryElement.IsEnabled;
			}
			if (flag)
			{
				Vector3 localPosition = ((Transform)val2).localPosition;
				localPosition.x = 0f;
				localPosition.y = (float)(-num) * 50f;
				((Transform)val2).localPosition = localPosition;
				Vector2 sizeDelta2 = val2.sizeDelta;
				sizeDelta2.x = ((Rect)(ref rect)).width;
				val2.sizeDelta = sizeDelta2;
				num++;
			}
			else
			{
				((Vector3)(ref position))._002Ector(1000f, 1000f);
				((Transform)val2).position = position;
			}
			if (obj is CardopediaEntryElement cardopediaEntryElement2)
			{
				bool flag2 = Mathf.Abs(((Transform)val2).localPosition.y - num2) < height * 0.75f;
				cardopediaEntryElement2.Cull(!cardopediaEntryElement2.IsEnabled || !flag2);
			}
		}
		sizeDelta.y = (float)num * 50f;
		EntriesParent.sizeDelta = sizeDelta;
	}

	private string GetDropSummaryFromCard(CardData cardData)
	{
		if (cardData is Harvestable)
		{
			return BoosterpackData.GetSummaryFromAllCards(cardData.GetPossibleDrops(), "label_can_drop");
		}
		if (cardData is Enemy)
		{
			return BoosterpackData.GetSummaryFromAllCards(cardData.GetPossibleDrops(), "label_can_drop");
		}
		return "";
	}

	private void SetTempDemoCard(CardData data)
	{
		if ((Object)(object)demoCard != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)demoCard).gameObject);
		}
		demoCard = Object.Instantiate<GameCard>(PrefabManager.instance.GameCardPrefab);
		CardData cardData = Object.Instantiate<CardData>(data);
		((Component)cardData).transform.SetParent(((Component)demoCard).transform);
		demoCard.CardData = cardData;
		cardData.MyGameCard = demoCard;
		demoCard.FaceUp = true;
		demoCard.IsDemoCard = true;
		demoCard.SetDemoCardRotation();
		demoCard.UpdateCardPalette();
		cardData.UpdateCard();
		demoCard.ForceUpdate();
		((Component)((TMP_Text)CardDescription).transform.parent).gameObject.SetActive(true);
		demoCard.CardData.UpdateCardText();
		string dropSummaryFromCard = GetDropSummaryFromCard(cardData);
		string description = demoCard.CardData.Description;
		description = description.Replace("\\d", "\n\n");
		if (cardData is Combatable combatable)
		{
			description += combatable.GetCombatableDescriptionAdvanced();
		}
		if (!string.IsNullOrEmpty(dropSummaryFromCard) && cardData.MyCardType != CardType.Locations)
		{
			description = description + "\n\n" + dropSummaryFromCard;
		}
		if (cardData is Blueprint blueprint)
		{
			description = blueprint.GetText();
		}
		((TMP_Text)CardDescription).text = description;
	}
}
