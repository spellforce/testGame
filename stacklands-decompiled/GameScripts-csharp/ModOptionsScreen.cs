using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ModOptionsScreen : SokScreen
{
	public static ModOptionsScreen instance;

	public CustomButton BackButton;

	public CustomButton OpenFolderButton;

	public CustomButton OpenWorkshopButton;

	public CustomButton UploadButton;

	public RectTransform ButtonsParent;

	public static Mod SelectedMod;

	public bool ShouldRestart;

	public RectTransform SpacerPrefab;

	public RectTransform InputPrefab;

	public RectTransform Title;

	public RectTransform Version;

	private CallResult<SteamUGCQueryCompleted_t> crQueryCompleted;

	private UGCQueryHandle_t queryHandle;

	private void Awake()
	{
		instance = this;
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModsScreen>();
			SelectedMod?.Config.Save();
			if (ShouldRestart)
			{
				WorldManager.RebootGame();
			}
		};
		OpenFolderButton.Clicked += delegate
		{
			Application.OpenURL("file:///" + SelectedMod.Path);
		};
		UploadButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModUploadScreen>();
		};
	}

	private void Update()
	{
		if (!InputController.instance.DisableAllInput && InputController.instance.GetKeyDown((Key)35) && PlatformHelper.UseSteam)
		{
			GameCanvas.instance.SetScreen<ModUploadScreen>();
		}
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<ModsScreen>();
			SelectedMod?.Config.Save();
			if (ShouldRestart)
			{
				WorldManager.RebootGame();
			}
		}
	}

	private void OnEnable()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in (Transform)ButtonsParent)
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
		if ((Object)(object)SelectedMod == (Object)null)
		{
			return;
		}
		if (ModUploadScreen.GetWorkshopId(SelectedMod) != 0L)
		{
			((Component)OpenWorkshopButton).gameObject.SetActive(true);
			OpenWorkshopButton.Clicked += delegate
			{
				OpenWorkshop();
			};
		}
		else
		{
			((Component)OpenWorkshopButton).gameObject.SetActive(false);
		}
		((TMP_Text)((Component)Title).GetComponent<TextMeshProUGUI>()).text = SelectedMod.Manifest.Name;
		((TMP_Text)((Component)Version).GetComponent<TextMeshProUGUI>()).text = "v" + SelectedMod.Manifest.Version;
		foreach (ConfigEntryBase entry in SelectedMod.Config.Entries)
		{
			entry.UI.OnUI?.Invoke(entry);
			if (!entry.UI.Hidden)
			{
				Debug.Log((object)$"creating ui for {entry.Name} ({entry.ValueType})");
				if (entry.ValueType == typeof(bool))
				{
					CreateBoolConfig(entry);
				}
				else if (entry.ValueType == typeof(string))
				{
					CreateTextConfig(entry);
				}
				else if (entry.ValueType == typeof(int) || entry.ValueType == typeof(float))
				{
					CreateNumberConfig(entry, entry.ValueType);
				}
			}
		}
		((Component)UploadButton).gameObject.SetActive(false);
		if (ModManager.LocalModPaths.Contains(SelectedMod.Path))
		{
			((Component)UploadButton).gameObject.SetActive(true);
			return;
		}
		crQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create((APIDispatchDelegate<SteamUGCQueryCompleted_t>)OnQueryCompleted);
		ulong workshopId = ModUploadScreen.GetWorkshopId(SelectedMod);
		if (workshopId != 0L)
		{
			queryHandle = SteamUGC.CreateQueryUGCDetailsRequest((PublishedFileId_t[])(object)new PublishedFileId_t[1] { (PublishedFileId_t)workshopId }, 1u);
			SteamAPICall_t val = SteamUGC.SendQueryUGCRequest(queryHandle);
			crQueryCompleted.Set(val, (APIDispatchDelegate<SteamUGCQueryCompleted_t>)null);
		}
	}

	private void OnQueryCompleted(SteamUGCQueryCompleted_t result, bool failed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)result.m_eResult != 1)
		{
			return;
		}
		SteamUGCDetails_t val = default(SteamUGCDetails_t);
		if (SteamUGC.GetQueryUGCResult(queryHandle, 0u, ref val))
		{
			Debug.Log((object)$"owner of mod is {val.m_ulSteamIDOwner}");
			if (val.m_ulSteamIDOwner == (ulong)SteamUser.GetSteamID())
			{
				((Component)UploadButton).gameObject.SetActive(true);
			}
		}
		SteamUGC.ReleaseQueryUGCRequest(queryHandle);
	}

	private static void OpenWorkshop()
	{
		if (PlatformHelper.UseSteam && !InputController.instance.GetKey((Key)53))
		{
			SteamFriends.ActivateGameOverlayToWebPage($"https://steamcommunity.com/sharedfiles/filedetails/?id={ModUploadScreen.GetWorkshopId(SelectedMod)}", (EActivateGameOverlayToWebPageMode)0);
		}
		else
		{
			Application.OpenURL($"https://steamcommunity.com/sharedfiles/filedetails/?id={ModUploadScreen.GetWorkshopId(SelectedMod)}");
		}
	}

	private void CreateTextConfig(ConfigEntryBase entry)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab, (Transform)(object)ButtonsParent);
		((Component)customButton).transform.localScale = Vector3.one;
		((Component)customButton).transform.localPosition = Vector3.zero;
		((Component)customButton).transform.localRotation = Quaternion.identity;
		string name = entry.UI.GetName();
		string tooltip = entry.UI.GetTooltip();
		((TMP_Text)customButton.TextMeshPro).text = ((!string.IsNullOrEmpty(name)) ? name : entry.Name);
		if (!string.IsNullOrEmpty(tooltip))
		{
			customButton.TooltipText = tooltip;
		}
		TMP_InputField component = ((Component)Object.Instantiate<RectTransform>(InputPrefab, (Transform)(object)ButtonsParent)).GetComponent<TMP_InputField>();
		component.text = (string)entry.BoxedValue;
		component.characterLimit = 0;
		((TMP_Text)component.placeholder).text = entry.UI.PlaceholderText;
		((UnityEvent<string>)(object)component.onValueChanged).AddListener((UnityAction<string>)delegate(string newValue)
		{
			entry.BoxedValue = newValue;
			if (entry.UI.RestartAfterChange)
			{
				ShouldRestart = true;
			}
		});
		Object.Instantiate<RectTransform>(SpacerPrefab, (Transform)(object)ButtonsParent);
	}

	private void CreateNumberConfig(ConfigEntryBase entry, Type inputType)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab, (Transform)(object)ButtonsParent);
		((Component)customButton).transform.localScale = Vector3.one;
		((Component)customButton).transform.localPosition = Vector3.zero;
		((Component)customButton).transform.localRotation = Quaternion.identity;
		string name = entry.UI.GetName();
		string tooltip = entry.UI.GetTooltip();
		((TMP_Text)customButton.TextMeshPro).text = ((!string.IsNullOrEmpty(name)) ? name : entry.Name);
		if (!string.IsNullOrEmpty(tooltip))
		{
			customButton.TooltipText = tooltip;
		}
		TMP_InputField component = ((Component)Object.Instantiate<RectTransform>(InputPrefab, (Transform)(object)ButtonsParent)).GetComponent<TMP_InputField>();
		component.characterValidation = (CharacterValidation)((inputType == typeof(int)) ? 2 : 3);
		component.text = ((inputType == typeof(int)) ? ((int)entry.BoxedValue).ToString() : ((float)entry.BoxedValue).ToString());
		((UnityEvent<string>)(object)component.onValueChanged).AddListener((UnityAction<string>)delegate(string newValue)
		{
			entry.BoxedValue = ((inputType == typeof(int)) ? ((object)int.Parse(newValue)) : ((object)float.Parse(newValue)));
			if (entry.UI.RestartAfterChange)
			{
				ShouldRestart = true;
			}
		});
		Object.Instantiate<RectTransform>(SpacerPrefab, (Transform)(object)ButtonsParent);
	}

	private void CreateBoolConfig(ConfigEntryBase entry)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		CustomButton btn = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab, (Transform)(object)ButtonsParent);
		((Component)btn).transform.localScale = Vector3.one;
		((Component)btn).transform.localPosition = Vector3.zero;
		((Component)btn).transform.localRotation = Quaternion.identity;
		string name = entry.UI.GetName();
		string tooltip = entry.UI.GetTooltip();
		((TMP_Text)btn.TextMeshPro).text = ((!string.IsNullOrEmpty(name)) ? name : entry.Name) + ": " + BoolToLabel((bool)entry.BoxedValue);
		if (!string.IsNullOrEmpty(tooltip))
		{
			btn.TooltipText = tooltip;
		}
		btn.Clicked += delegate
		{
			entry.BoxedValue = !(bool)entry.BoxedValue;
			((TMP_Text)btn.TextMeshPro).text = ((!string.IsNullOrEmpty(name)) ? name : entry.Name) + ": " + BoolToLabel((bool)entry.BoxedValue);
			if (entry.UI.RestartAfterChange)
			{
				ShouldRestart = true;
			}
		};
	}

	private string BoolToLabel(bool b)
	{
		if (!b)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}
}
