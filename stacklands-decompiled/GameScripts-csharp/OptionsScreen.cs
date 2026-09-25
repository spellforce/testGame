using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsScreen : SokScreen
{
	public static int CurrentWidth;

	public static int CurrentHeight;

	public static bool CurrentFullScreen;

	public static int CurrentFrameRate;

	public static Vector2 CurrentUIScale;

	public const bool DEBUG_RESOLUTION = false;

	public CustomButton ResolutionButton;

	public CustomButton FullscreenButton;

	public CustomButton UIScaleButton;

	public CustomButton FrameRateCapButton;

	public CustomButton LanguageButton;

	public CustomButton ClearSaveButton;

	public CustomButton CreditsButton;

	public CustomButton ControlsButton;

	public CustomButton AccessibilityButton;

	public CustomButton BackButton;

	public CustomButton AdvancedSettingsButton;

	public Slider MusicSlider;

	public Slider SfxSlider;

	public CanvasScaler CanvasScaler;

	public TextMeshProUGUI VersionText;

	public TextMeshProUGUI MusicVolumeText;

	public TextMeshProUGUI SfxVolumeText;

	public CustomButton SelectSaveButton;

	private static List<int> frameRates = new List<int> { -2, -1, 30, 60, 120 };

	private static List<Vector2> UIScale = new List<Vector2>
	{
		new Vector2(1920f, 1080f),
		new Vector2(2500f, 1080f),
		new Vector2(3440f, 2169f)
	};

	public static bool MusicOn;

	public static bool SfxOn;

	public static float MusicVol;

	public static float SfxVol;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		SelectSaveButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<SelectSaveScreen>();
		};
		ResolutionButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<SelectResolutionScreen>();
		};
		ControlsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ControlsScreen>();
		};
		AccessibilityButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<AccessibilityScreen>();
		};
		AdvancedSettingsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<AdvancedSettingsScreen>();
		};
		FullscreenButton.Clicked += ToggleFullScreen;
		FrameRateCapButton.Clicked += ToggleFrameRateCap;
		UIScaleButton.Clicked += ToggleUIScale;
		ClearSaveButton.Clicked += delegate
		{
			GameCanvas.instance.ShowClearSaveModal();
		};
		LanguageButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<SelectLanguageScreen>();
		};
		((UnityEvent<float>)(object)MusicSlider.onValueChanged).AddListener((UnityAction<float>)OnMusicVolumeChange);
		((UnityEvent<float>)(object)SfxSlider.onValueChanged).AddListener((UnityAction<float>)OnSFXVolumeChange);
		CreditsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<CreditsScreen>();
		};
		BackButton.Clicked += delegate
		{
			GoBack();
		};
		LoadSettings();
		MusicSlider.value = MusicVol;
		SfxSlider.value = SfxVol;
		SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
	}

	public void OnMusicVolumeChange(float sliderValue)
	{
		MusicVol = sliderValue;
	}

	public void OnSFXVolumeChange(float sliderValue)
	{
		SfxVol = sliderValue;
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
		SetTexts();
	}

	private void GoBack()
	{
		if (WorldManager.instance.CurrentGameState == WorldManager.GameState.Paused)
		{
			GameCanvas.instance.SetScreen<PauseScreen>();
		}
		else
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		}
		SaveSettings();
	}

	private void Start()
	{
		SetTexts();
	}

	private static void ToggleUIScale()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		int num = UIScale.IndexOf(CurrentUIScale) + 1;
		if (num == UIScale.Count)
		{
			num = 0;
		}
		CurrentUIScale = UIScale[num];
		SetUIScale();
	}

	private static void ToggleFullScreen()
	{
		CurrentFullScreen = !CurrentFullScreen;
		SetResolution();
	}

	private void ToggleFrameRateCap()
	{
		int num = frameRates.IndexOf(CurrentFrameRate) + 1;
		if (num == frameRates.Count)
		{
			num = 0;
		}
		CurrentFrameRate = frameRates[num];
	}

	private void Update()
	{
		((TMP_Text)VersionText).text = "v" + Application.version;
		if (InputController.instance.CancelTriggered() && !GameCanvas.instance.ModalIsOpen)
		{
			GoBack();
		}
		SetTexts();
	}

	private void OnDisable()
	{
		SaveSettings();
	}

	private void SetTexts()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)ResolutionButton.TextMeshPro).text = SokLoc.Translate("label_resolution", (LocParam[])(object)new LocParam[2]
		{
			LocParam.Create("width", CurrentWidth.ToString()),
			LocParam.Create("height", CurrentHeight.ToString())
		});
		((TMP_Text)FullscreenButton.TextMeshPro).text = SokLoc.Translate("label_fullscreen", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(CurrentFullScreen)) });
		((TMP_Text)FrameRateCapButton.TextMeshPro).text = SokLoc.Translate("label_framerate_cap", (LocParam[])(object)new LocParam[1] { LocParam.Create("fps_cap", FramerateLabel(CurrentFrameRate)) });
		((TMP_Text)UIScaleButton.TextMeshPro).text = SokLoc.Translate("label_ui_scale", (LocParam[])(object)new LocParam[1] { LocParam.Create("scale", UIScaleLabel(UIScale.IndexOf(CurrentUIScale))) });
		((TMP_Text)BackButton.TextMeshPro).text = SokLoc.Translate("label_back");
		((TMP_Text)MusicVolumeText).text = $"{Mathf.RoundToInt(MusicVol * 100f)}%";
		((TMP_Text)SfxVolumeText).text = $"{Mathf.RoundToInt(SfxVol * 100f)}%";
	}

	public static string YesNo(bool a)
	{
		if (!a)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}

	public static string FramerateLabel(int i)
	{
		return i switch
		{
			-2 => SokLoc.Translate("label_framerate_unlimited"), 
			-1 => SokLoc.Translate("label_framerate_vsync"), 
			30 => SokLoc.Translate("label_framerate_30"), 
			60 => SokLoc.Translate("label_framerate_60"), 
			120 => SokLoc.Translate("label_framerate_120"), 
			_ => SokLoc.Translate("label_framerate_unlimited"), 
		};
	}

	public static string UIScaleLabel(int i)
	{
		return i switch
		{
			0 => SokLoc.Translate("label_ui_scale_100"), 
			1 => SokLoc.Translate("label_ui_scale_80"), 
			2 => SokLoc.Translate("label_ui_scale_60"), 
			_ => SokLoc.Translate("label_ui_scale_100"), 
		};
	}

	public static void LoadSettings()
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		MusicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
		SfxOn = PlayerPrefs.GetInt("sfxOn", 1) == 1;
		MusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
		SfxVol = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
		string languageName = SokLoc.DetermineSystemLanguage().LanguageName;
		string language = PlayerPrefs.GetString("language", languageName);
		SokLoc.instance.SetLanguage(language);
		int num = PlayerPrefs.GetInt("width", -1);
		int num2 = PlayerPrefs.GetInt("height", -1);
		CurrentFullScreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
		CurrentFrameRate = PlayerPrefs.GetInt("framerate", -1);
		int num3 = PlayerPrefs.GetInt("uiScaleIndex", 0);
		if (num3 >= 0 && num3 < UIScale.Count)
		{
			CurrentUIScale = UIScale[num3];
		}
		else
		{
			CurrentUIScale = UIScale[0];
		}
		SetUIScale();
		if (num != -1 && num2 != -1)
		{
			CurrentWidth = num;
			CurrentHeight = num2;
			SetResolution();
			Debug.Log((object)$"Loaded resolution {CurrentWidth}x{CurrentHeight}");
		}
		else
		{
			Resolution currentResolution = Screen.currentResolution;
			CurrentWidth = ((Resolution)(ref currentResolution)).width;
			currentResolution = Screen.currentResolution;
			CurrentHeight = ((Resolution)(ref currentResolution)).height;
			Debug.Log((object)$"Set current resolution to {CurrentWidth}x{CurrentHeight}");
		}
	}

	public static void SaveSettings()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		PlayerPrefs.SetInt("musicOn", MusicOn ? 1 : 0);
		PlayerPrefs.SetInt("sfxOn", SfxOn ? 1 : 0);
		PlayerPrefs.SetFloat("MusicVolume", MusicVol);
		PlayerPrefs.SetFloat("SfxVolume", SfxVol);
		PlayerPrefs.SetInt("fullscreen", CurrentFullScreen ? 1 : 0);
		PlayerPrefs.SetInt("framerate", CurrentFrameRate);
		PlayerPrefs.SetInt("uiScaleIndex", UIScale.IndexOf(CurrentUIScale));
		PlayerPrefs.SetInt("width", CurrentWidth);
		PlayerPrefs.SetInt("height", CurrentHeight);
		PlayerPrefs.SetString("language", SokLoc.instance.CurrentLanguage);
	}

	private static bool IsSameResolution(Resolution a, Resolution b)
	{
		if (((Resolution)(ref a)).width == ((Resolution)(ref b)).width)
		{
			return ((Resolution)(ref a)).height == ((Resolution)(ref b)).height;
		}
		return false;
	}

	public static List<Resolution> PossibleResolutions()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		List<Resolution> list = new List<Resolution>();
		List<Resolution> list2 = Screen.resolutions.ToList();
		list2.Sort((Resolution a, Resolution b) => ((Resolution)(ref a)).width - ((Resolution)(ref b)).width);
		for (int num = 0; num < list2.Count; num++)
		{
			Resolution val = list2[num];
			bool flag = false;
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				if (IsSameResolution(val, list[num2]))
				{
					flag = true;
					break;
				}
			}
			if (!flag && ((Resolution)(ref val)).height != 0 && ((Resolution)(ref val)).width != 0)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static void SetUIScale()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((Component)GameCanvas.instance.Canvas).GetComponent<CanvasScaler>().referenceResolution = CurrentUIScale;
	}

	public static void SetResolution()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		int currentWidth;
		Resolution val;
		if (CurrentWidth >= 100)
		{
			currentWidth = CurrentWidth;
		}
		else
		{
			val = PossibleResolutions()[0];
			currentWidth = ((Resolution)(ref val)).width;
		}
		CurrentWidth = currentWidth;
		int currentHeight;
		if (CurrentHeight >= 100)
		{
			currentHeight = CurrentHeight;
		}
		else
		{
			val = PossibleResolutions()[0];
			currentHeight = ((Resolution)(ref val)).height;
		}
		CurrentHeight = currentHeight;
		Screen.SetResolution(CurrentWidth, CurrentHeight, CurrentFullScreen);
	}
}
