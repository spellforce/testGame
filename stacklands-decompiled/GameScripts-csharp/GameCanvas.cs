using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
	public enum ScreenPosition
	{
		Left,
		Right,
		Bottom,
		Top,
		None
	}

	public static GameCanvas instance;

	public RectTransform FreeMoveParent;

	public SokScreen CurrentScreen;

	public RectTransform Modal;

	public Canvas Canvas;

	private List<ScreenPosition> screenPositions;

	private List<bool> screenInTransition;

	private Dictionary<RectTransform, GameObject> lastSelectedObject = new Dictionary<RectTransform, GameObject>();

	private List<SokScreen> screens;

	private List<RaycastResult> raycastResults = new List<RaycastResult>();

	public GameObject Transition;

	public bool ModalIsOpen;

	public GameObject MouseOverObject;

	private CustomButton previousHovered;

	private PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

	private GetComponentCacher<CustomButton> getCustomButtonCacher = new GetComponentCacher<CustomButton>();

	public Ease easeType = (Ease)10;

	private RectTransform rectTransform
	{
		get
		{
			Transform transform = ((Component)this).transform;
			return (RectTransform)(object)((transform is RectTransform) ? transform : null);
		}
	}

	public bool NothingSelected
	{
		get
		{
			if (!((Object)(object)EventSystem.current.currentSelectedGameObject == (Object)null))
			{
				return !EventSystem.current.currentSelectedGameObject.activeInHierarchy;
			}
			return true;
		}
	}

	public GameObject SelectedObject => EventSystem.current.currentSelectedGameObject;

	private void Awake()
	{
		instance = this;
		screens = ((Component)this).GetComponentsInChildren<SokScreen>(true).ToList();
		screenPositions = new List<ScreenPosition>();
		screenInTransition = new List<bool>();
		foreach (SokScreen screen in screens)
		{
			_ = screen;
			screenPositions.Add(ScreenPosition.Bottom);
			screenInTransition.Add(item: false);
		}
		foreach (SokScreen screen2 in screens)
		{
			((Component)screen2).gameObject.SetActive(true);
			((Component)screen2).gameObject.SetActive(false);
		}
		((Component)Modal).gameObject.SetActive(true);
		((Component)Modal).gameObject.SetActive(false);
		((Component)FreeMoveParent).gameObject.SetActive(true);
		Transition.gameObject.SetActive(true);
		SetScreen<MainMenu>();
	}

	private void Update()
	{
		if ((!(CurrentScreen is GameScreen) || GameScreen.instance.ControllerIsInUI) && InputController.instance.CurrentSchemeIsController && NothingSelected)
		{
			if ((Object)(object)EventSystem.current.currentSelectedGameObject != (Object)null)
			{
				_ = !EventSystem.current.currentSelectedGameObject.activeInHierarchy;
			}
			else
				_ = 0;
			SelectFirstSelectable(CurrentScreen);
		}
		if (InputController.instance.CancelTriggered() && !ModalIsOpen)
		{
			SokScreen previousScreen = GetPreviousScreen(CurrentScreen);
			if ((Object)(object)previousScreen != (Object)null)
			{
				SetScreen(previousScreen);
			}
		}
		PerformMouseRaycast();
		UpdateMouseOverObject();
		UpdateCustomButtonHovered();
	}

	private void UpdateMouseOverObject()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)EventSystem.current == (Object)null))
		{
			if (raycastResults.Count > 0)
			{
				RaycastResult val = raycastResults[0];
				MouseOverObject = ((RaycastResult)(ref val)).gameObject;
			}
			else
			{
				MouseOverObject = null;
			}
		}
	}

	private void PerformMouseRaycast()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((AbstractEventData)eventDataCurrentPosition).Reset();
		eventDataCurrentPosition.position = InputController.instance.ClampedMousePosition();
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
	}

	private void UpdateCustomButtonHovered()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)previousHovered != (Object)null)
		{
			previousHovered.IsHovered = false;
		}
		if (!InputController.instance.IsUsingMouse || TransitionScreen.InTransition || InputController.instance.MouseIsDragging || (Object)(object)EventSystem.current == (Object)null)
		{
			return;
		}
		foreach (RaycastResult raycastResult in raycastResults)
		{
			RaycastResult current = raycastResult;
			CustomButton component = getCustomButtonCacher.GetComponent(((RaycastResult)(ref current)).gameObject);
			if (!((Object)(object)component == (Object)null) && (!((Object)(object)component.parentScreen != (Object)null) || !ModalIsOpen) && RectTransformUtility.RectangleContainsScreenPoint(component.RectTransform, InputController.instance.ClampedMousePosition(), (Camera)null) && IsSelected((Transform)(object)component.RectTransform, raycastResults))
			{
				component.IsHovered = true;
				previousHovered = component;
				break;
			}
		}
	}

	private SokScreen GetPreviousScreen(SokScreen screen)
	{
		if (screen is CardopediaScreen)
		{
			if (WorldManager.instance.CurrentGameState == WorldManager.GameState.Paused)
			{
				return GetScreen<PauseScreen>();
			}
			return GetScreen<MainMenu>();
		}
		if (screen is CreditsScreen || screen is SelectResolutionScreen || screen is SelectLanguageScreen || screen is SelectSaveScreen || screen is ControlsScreen || screen is AccessibilityScreen || screen is AdvancedSettingsScreen)
		{
			return GetScreen<OptionsScreen>();
		}
		return null;
	}

	public static string FormatTimeLeft(float timeLeft)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return SokLoc.Translate("label_seconds_left_format", (LocParam[])(object)new LocParam[1] { LocParam.Create("seconds", timeLeft.ToString("0.0")) });
	}

	public static string FormatTime(float time)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return SokLoc.Translate("label_seconds_format", (LocParam[])(object)new LocParam[1] { LocParam.Create("seconds", time.ToString("0.0")) });
	}

	public static string FormatTimeShort(float time)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return SokLoc.Translate("label_seconds_format", (LocParam[])(object)new LocParam[1] { LocParam.Create("seconds", time.ToString("0")) });
	}

	public void SetScreen(SokScreen nextScreen)
	{
		if ((Object)(object)nextScreen == (Object)(object)CurrentScreen)
		{
			return;
		}
		SokScreen currentScreen = CurrentScreen;
		if ((Object)(object)currentScreen != (Object)null)
		{
			lastSelectedObject[currentScreen.Rect] = EventSystem.current.currentSelectedGameObject;
		}
		HandleTransition(CurrentScreen, nextScreen);
		CurrentScreen = nextScreen;
		foreach (SokScreen screen in screens)
		{
			if (!((Object)(object)screen == (Object)(object)currentScreen))
			{
				((Component)screen).gameObject.SetActive((Object)(object)nextScreen == (Object)(object)screen);
			}
		}
		if (lastSelectedObject.ContainsKey(nextScreen.Rect))
		{
			GameObject val = lastSelectedObject[nextScreen.Rect];
			if ((Object)(object)val != (Object)null && val.gameObject.activeInHierarchy)
			{
				EventSystem.current.SetSelectedGameObject(val);
			}
		}
		SetFrameRateCap(CurrentScreen);
	}

	public SokScreen GetScreen<T>() where T : SokScreen
	{
		return screens.Find((SokScreen x) => x is T);
	}

	public void SetScreen<T>() where T : SokScreen
	{
		SokScreen screen = GetScreen<T>();
		if ((Object)(object)screen == (Object)(object)CurrentScreen)
		{
			return;
		}
		SokScreen currentScreen = CurrentScreen;
		if ((Object)(object)currentScreen != (Object)null)
		{
			lastSelectedObject[currentScreen.Rect] = EventSystem.current.currentSelectedGameObject;
		}
		HandleTransition(CurrentScreen, screen);
		CurrentScreen = screen;
		foreach (SokScreen screen2 in screens)
		{
			if (!((Object)(object)screen2 == (Object)(object)currentScreen))
			{
				((Component)screen2).gameObject.SetActive((Object)(object)screen == (Object)(object)screen2);
			}
		}
		if (lastSelectedObject.ContainsKey(screen.Rect))
		{
			GameObject val = lastSelectedObject[screen.Rect];
			if ((Object)(object)val != (Object)null && val.activeInHierarchy)
			{
				EventSystem.current.SetSelectedGameObject(val);
			}
		}
		SetFrameRateCap(CurrentScreen);
	}

	private void SetFrameRateCap(SokScreen currentScreen)
	{
		if (currentScreen.IsFrameRateUncapped)
		{
			int num = PlayerPrefs.GetInt("framerate", -1);
			Application.targetFrameRate = Mathf.Max(-1, num);
			if (num == -1)
			{
				QualitySettings.vSyncCount = 1;
			}
			else
			{
				QualitySettings.vSyncCount = 0;
			}
		}
		else
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 1;
		}
	}

	private void HandleTransition(SokScreen prevScreen, SokScreen nextScreen)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!HasTransition(prevScreen, nextScreen))
		{
			if (prevScreen != null)
			{
				ShortcutExtensions.DOKill((Component)(object)prevScreen.Rect, true);
			}
			ShortcutExtensions.DOKill((Component)(object)nextScreen.Rect, true);
			if ((Object)(object)prevScreen != (Object)null)
			{
				((Component)prevScreen).gameObject.SetActive(false);
			}
			((Component)nextScreen).gameObject.SetActive(true);
			nextScreen.Rect.anchoredPosition = Vector2.zero;
		}
		else
		{
			ScreenPosition pos = ScreenPosition.Left;
			ScreenPosition pos2 = ScreenPosition.Left;
			LeaveTo(prevScreen, pos);
			EnterFrom(nextScreen, pos2);
		}
	}

	public bool MousePositionIsOverUI()
	{
		return raycastResults.Count > 0;
	}

	public bool PositionIsOverUI(Vector2 screenPos)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)EventSystem.current == (Object)null)
		{
			return false;
		}
		((AbstractEventData)eventDataCurrentPosition).Reset();
		eventDataCurrentPosition.position = screenPos;
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
		return raycastResults.Count > 0;
	}

	public Vector3 ScreenPosToLocalPos(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(FreeMoveParent, Vector2.op_Implicit(pos), (Camera)null, ref val);
		return Vector2.op_Implicit(val);
	}

	public bool AboveMeOrMyChildrenRaycast(Transform t, Vector2 position)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)EventSystem.current == (Object)null)
		{
			return false;
		}
		((AbstractEventData)eventDataCurrentPosition).Reset();
		eventDataCurrentPosition.position = position;
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
		foreach (RaycastResult raycastResult in raycastResults)
		{
			RaycastResult current = raycastResult;
			if (((RaycastResult)(ref current)).gameObject.transform.IsChildOf(t) || (Object)(object)((RaycastResult)(ref current)).gameObject.transform == (Object)(object)t)
			{
				return true;
			}
		}
		return false;
	}

	public bool AboveMeOrMyChildren(Transform t, GameObject mouseOverObject)
	{
		if ((Object)(object)mouseOverObject == (Object)null)
		{
			return false;
		}
		if (mouseOverObject.transform.IsChildOf(t) || (Object)(object)mouseOverObject.transform == (Object)(object)t)
		{
			return true;
		}
		return false;
	}

	public static Rect GetWorldRect2(RectTransform rt)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		rt.GetWorldCorners(array);
		Vector3 val = array[0];
		Vector2 val2 = Vector2.op_Implicit(array[2] - array[0]);
		return new Rect(Vector2.op_Implicit(val), val2);
	}

	public Rect GetWorldRect(RectTransform rt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetWorldRect2(rt);
	}

	public static void SetScrollRectPosition(ScrollRect scrollRect, RectTransform rectTransform, bool centerInView = false)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (centerInView)
		{
			scrollRect.ScrollToCenter(rectTransform);
			return;
		}
		Vector2 val = Vector2.op_Implicit(((Component)scrollRect).transform.InverseTransformPoint(((Transform)scrollRect.content).position));
		Rect worldRect = instance.GetWorldRect(rectTransform);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(((Rect)(ref worldRect)).xMin, ((Rect)(ref worldRect)).yMax);
		Vector2 val3 = Vector2.op_Implicit(((Component)scrollRect).transform.InverseTransformPoint(val2));
		Vector2 anchoredPosition = val - val3;
		anchoredPosition.x = scrollRect.content.anchoredPosition.x;
		scrollRect.content.anchoredPosition = anchoredPosition;
		scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
	}

	public void OpenModal()
	{
		((Component)Modal).gameObject.SetActive(true);
		ModalIsOpen = true;
	}

	public void CloseModal()
	{
		((Component)Modal).gameObject.SetActive(false);
		ModalIsOpen = false;
	}

	public void ShowDlcNotInstalledModal()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_spirit_dlc_not_installed_title"), SokLoc.Translate("label_spirit_dlc_not_installed_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowCantChangeBoardSpirit()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_change_board_disabled_title"), SokLoc.Translate("label_change_board_disabled_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowNameCombatableModal(CardData cb, Action onDone)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_name_villager_title"), SokLoc.Translate("label_name_villager_text"));
		TMP_InputField input = ModalScreen.instance.AddInputNoButton();
		if (!string.IsNullOrEmpty(cb.CustomName))
		{
			input.text = cb.CustomName;
		}
		else
		{
			input.text = cb.Name;
		}
		input.characterLimit = 12;
		ModalScreen.instance.AddOption(SokLoc.Translate("label_random_name"), delegate
		{
			input.text = GetRandomName();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			ProfanityChecker profanityChecker = WorldManager.instance.GameDataLoader.ProfanityChecker;
			string text = input.text;
			if (profanityChecker.IsProfanityInLanguage(SokLoc.instance.CurrentLanguage, text))
			{
				text = "Bobba";
			}
			text = StringUtils.RemoveRichText(text);
			cb.CustomName = text;
			CloseModal();
			onDone();
		});
		OpenModal();
	}

	public string GetRandomName()
	{
		return WorldManager.instance.GameDataLoader.VillagerNames.Choose();
	}

	public void ShowSimpleModal(string textTerm, string titleTerm)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(titleTerm, textTerm);
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowClearSaveModal()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_clear_save"), SokLoc.Translate("label_clear_save_confirm"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				WorldManager.instance.ClearSaveAndRestart();
				CloseModal();
			});
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowEarlyAccessModal()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		ModalScreen.instance.Clear();
		string text = SokLoc.Translate("label_youtube_cities_dlc_text", (LocParam[])(object)new LocParam[1] { LocParam.Create("saveDirectory", PlatformHelper.CurrentSavesDirectory) });
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_youtube_cities_dlc_title"), text);
		ModalScreen.instance.AddOption(SokLoc.Translate("label_load_run"), delegate
		{
			SaveGame saveFromResources = SaveManager.instance.GetSaveFromResources("Saves/2000_BetaSave");
			saveFromResources.SaveId = "4";
			SaveManager.instance.Save(saveFromResources);
			WorldManager.RestartGame();
			CloseModal();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_exit"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowStartNewRunModal(Action onYes)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_start_new_run"), SokLoc.Translate("label_new_run_confirm"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void OneVillagerNeedsToStayPrompt(string term)
	{
		ModalScreen.instance.Clear();
		if (WorldManager.instance.CurrentBoard.Id == "main")
		{
			ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_one_villager_needs_to_stay"));
		}
		else if (WorldManager.instance.CurrentBoard.Id == "island")
		{
			ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_one_villager_needs_to_stay_island"));
		}
		else
		{
			ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_one_villager_needs_to_stay"));
		}
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void MaxVillagerCountPrompt(string term, int amount)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_max_villager_in_portal", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", amount.ToString()) }));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void NotEnoughFoodToUsePortalPrompt()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_taking_portal_title"), SokLoc.Translate("label_not_enough_food_before_portal"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void NotEnoughFoodToSailOffPrompt()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_sailing_off_title"), SokLoc.Translate("label_not_enough_food_before_sailing"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void LeaveSpiritWorldPrompt(Action onYes, Action onNo)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_return_from_spirit_world_title"), SokLoc.Translate("label_return_from_spirit_world_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo();
		});
		OpenModal();
	}

	public void AbandonCityPrompt(Action onYes, Action onNo)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_abandon_city_title"), SokLoc.Translate("label_abandon_city_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo?.Invoke();
		});
		OpenModal();
	}

	public void GoToCityPrompt(Action onYes, Action onNo)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_go_to_city_title"), SokLoc.Translate("label_go_to_city_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo?.Invoke();
		});
		OpenModal();
	}

	public void MissingCardsInSavePrompt(Action onYes, HashSet<string> missingCardIds)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		ModalScreen.instance.Clear();
		string text = string.Join(", ", missingCardIds.Take(3));
		if (missingCardIds.Count > 3)
		{
			text = text + " " + SokLoc.Translate("label_missing_cards_more_text", (LocParam[])(object)new LocParam[1] { LocParam.Create("amount", (missingCardIds.Count - 3).ToString()) });
		}
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_missing_cards_title"), SokLoc.Translate("label_missing_cards_text", (LocParam[])(object)new LocParam[1] { LocParam.Create("missing", text) }));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ChangeLocationPrompt(Action onYes, Action onNo, string destinationBoard)
	{
		string text = "label_return_to_mainland_prompt";
		string text2 = "label_sailing_off_title";
		if (WorldManager.instance.CurrentBoard.Id == "main")
		{
			switch (destinationBoard)
			{
			case "island":
				text = "label_go_to_island_prompt";
				break;
			case "forest":
				text2 = "label_taking_portal_title";
				text = "label_go_to_forest_prompt";
				break;
			case "cities":
				text2 = "label_take_time_machine";
				text = "label_go_to_cities_mainland";
				break;
			}
		}
		else if (WorldManager.instance.CurrentBoard.Id == "island")
		{
			if (destinationBoard == "main")
			{
				text = "label_return_to_mainland_prompt";
			}
			else if (destinationBoard == "forest")
			{
				text2 = "label_taking_portal_title";
				text = "label_go_to_forest_prompt";
			}
		}
		else if (WorldManager.instance.CurrentBoard.Id == "cities" && destinationBoard == "main")
		{
			text2 = "label_take_time_machine";
			text = "label_go_to_mainland_cities";
		}
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate(text2), SokLoc.Translate(text));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo();
		});
		OpenModal();
	}

	private bool HasTransition(SokScreen prevScreen, SokScreen nextScreen)
	{
		if ((Object)(object)prevScreen == (Object)null)
		{
			return false;
		}
		if (nextScreen is GameScreen || prevScreen is GameScreen)
		{
			return false;
		}
		if (prevScreen is CreditsScreen || nextScreen is CreditsScreen)
		{
			return false;
		}
		return true;
	}

	private float GetEaseDuration(SokScreen screen, bool leaving)
	{
		return 0.25f;
	}

	private static Vector2 GetPosition(ScreenPosition pos, Vector2 size)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		switch (pos)
		{
		case ScreenPosition.Left:
			val = Vector2.left;
			break;
		case ScreenPosition.Right:
			val = Vector2.right;
			break;
		case ScreenPosition.Bottom:
			val = Vector2.down;
			break;
		case ScreenPosition.Top:
			val = Vector2.up;
			break;
		case ScreenPosition.None:
			((Vector2)(ref val))._002Ector(0f, 0f);
			break;
		default:
			throw new Exception($"Can't get screen position for {pos}");
		}
		return new Vector2(val.x * size.x, val.y * size.y);
	}

	private void EnterFrom(SokScreen screen, ScreenPosition pos)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		if (!((Object)(object)screen == (Object)null))
		{
			RectTransform rect = screen.Rect;
			TrackScreenPosition(screen, ScreenPosition.None);
			float easeDuration = GetEaseDuration(screen, leaving: false);
			screenInTransition[ScreenIndex(screen)] = true;
			ShortcutExtensions.DOKill((Component)(object)rect, false);
			TweenSettingsExtensions.OnComplete<TweenerCore<Vector2, Vector2, VectorOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(TweenSettingsExtensions.From<Vector2, Vector2, VectorOptions>(DOTweenModuleUI.DOAnchorPos(rect, Vector2.zero, easeDuration, true), GetPosition(pos, rectTransform.sizeDelta) * 1.2f, true, false), easeType), (TweenCallback)delegate
			{
				screenInTransition[ScreenIndex(screen)] = false;
			});
		}
	}

	private int ScreenIndex(SokScreen s)
	{
		return screens.IndexOf(s);
	}

	private void TrackScreenPosition(SokScreen screen, ScreenPosition pos)
	{
		if (!((Object)(object)screen == (Object)null))
		{
			screenPositions[screens.IndexOf(screen)] = pos;
		}
	}

	private void LeaveTo(SokScreen screen, ScreenPosition pos)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		RectTransform rect = screen.Rect;
		if (!((Object)(object)rect == (Object)null))
		{
			TrackScreenPosition(screen, pos);
			float easeDuration = GetEaseDuration(screen, leaving: true);
			screenInTransition[ScreenIndex(screen)] = true;
			ShortcutExtensions.DOKill((Component)(object)rect, false);
			TweenSettingsExtensions.OnComplete<TweenerCore<Vector2, Vector2, VectorOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOAnchorPos(rect, GetPosition(pos, rectTransform.sizeDelta), easeDuration, true), easeType), (TweenCallback)delegate
			{
				((Component)rect).gameObject.SetActive(false);
				screenInTransition[ScreenIndex(screen)] = false;
			});
		}
	}

	public void EaseRectTransformPosition(RectTransform rectTransform, ScreenPosition target, Vector2 size, Action onComplete = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)rectTransform, false);
		Vector2 position = GetPosition(target, size);
		TweenSettingsExtensions.OnComplete<TweenerCore<Vector2, Vector2, VectorOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOAnchorPos(rectTransform, position, 0.4f, false), easeType), (TweenCallback)delegate
		{
			onComplete?.Invoke();
		});
	}

	public void SetRectTransformPosition(RectTransform rectTransform, ScreenPosition target, Vector2 size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = GetPosition(target, size);
		rectTransform.anchoredPosition = position;
	}

	public SokScreen GetParentScreen(RectTransform obj)
	{
		return ((Component)obj).gameObject.GetComponentInParent<SokScreen>(true);
	}

	private bool ScreenInTransition(SokScreen screen)
	{
		return screenInTransition[ScreenIndex(screen)];
	}

	public bool AnyScreenInTransition()
	{
		for (int i = 0; i < screenInTransition.Count; i++)
		{
			if (screenInTransition[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool ScreenIsInteractable(SokScreen screen)
	{
		if ((Object)(object)CurrentScreen == (Object)(object)screen)
		{
			return !ScreenInTransition(screen);
		}
		return false;
	}

	public bool ScreenIsInteractable<T>() where T : SokScreen
	{
		SokScreen screen = GetScreen<T>();
		if ((Object)(object)CurrentScreen == (Object)(object)screen)
		{
			return !ScreenInTransition(screen);
		}
		return false;
	}

	public bool IsSelected(Transform t, Vector2 position)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)EventSystem.current == (Object)null)
		{
			return false;
		}
		((AbstractEventData)eventDataCurrentPosition).Reset();
		eventDataCurrentPosition.position = position;
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
		if (raycastResults.Count == 0)
		{
			return false;
		}
		RaycastResult val = raycastResults.OrderBy((RaycastResult x) => x.displayIndex).First();
		if (!((Object)(object)((RaycastResult)(ref val)).gameObject.transform == (Object)(object)t))
		{
			return IsDeepChildOf(((RaycastResult)(ref val)).gameObject.transform, t);
		}
		return true;
	}

	public bool IsSelected(Transform t, List<RaycastResult> raycastResults)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		RaycastResult val = raycastResults.OrderBy((RaycastResult x) => x.displayIndex).First();
		if (!((Object)(object)((RaycastResult)(ref val)).gameObject.transform == (Object)(object)t))
		{
			return IsDeepChildOf(((RaycastResult)(ref val)).gameObject.transform, t);
		}
		return true;
	}

	private bool IsDeepChildOf(Transform a, Transform parent)
	{
		Transform val = a;
		while ((Object)(object)val.parent != (Object)null)
		{
			if ((Object)(object)val.parent == (Object)(object)parent)
			{
				return true;
			}
			val = val.parent;
		}
		return false;
	}

	public void ReselectFirstSelectable()
	{
		if ((Object)(object)CurrentScreen != (Object)null)
		{
			SelectFirstSelectable(CurrentScreen);
		}
	}

	private void SelectFirstSelectable(SokScreen screen)
	{
		RectTransform val = screen.Rect;
		if (ModalIsOpen)
		{
			Transform transform = ((Component)ModalScreen.instance).transform;
			val = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		Selectable val2 = ((Component)val).GetComponentInChildren<Selectable>();
		if ((Object)(object)val2 == (Object)null)
		{
			if (Application.isEditor)
			{
				Debug.Log((object)("Could not find a Selectable child in " + ((Object)val).name));
			}
			return;
		}
		int num = 0;
		while ((Object)(object)val2.FindSelectableOnUp() != (Object)null && num < 100)
		{
			val2 = val2.FindSelectableOnUp();
			num++;
		}
		if (val2 is CustomButton && !(val2 as CustomButton).SelectableWithController)
		{
			CustomButton[] componentsInChildren = ((Component)val).GetComponentsInChildren<CustomButton>();
			foreach (CustomButton customButton in componentsInChildren)
			{
				if (customButton.SelectableWithController)
				{
					val2 = (Selectable)(object)customButton;
					break;
				}
			}
		}
		if (val2.interactable)
		{
			val2.Select();
			EventSystem.current.SetSelectedGameObject(((Component)val2).gameObject);
			val2.Select();
		}
	}

	public void SetUIToggle(bool enabled)
	{
		((Behaviour)Canvas).enabled = enabled;
	}
}
