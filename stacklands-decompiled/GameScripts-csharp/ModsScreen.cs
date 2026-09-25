using System.Linq;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModsScreen : SokScreen
{
	public CustomButton BackButton;

	public CustomButton DisableModsButton;

	public CustomButton OpenWorkshopButton;

	public RectTransform ButtonsParent;

	private void Awake()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (!PlatformHelper.HasModdingSupport)
		{
			return;
		}
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		};
		DisableModsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModDisablingScreen>();
		};
		foreach (Mod mod in ModManager.LoadedMods.OrderBy((Mod m) => m.Manifest.Name))
		{
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab, (Transform)(object)ButtonsParent);
			((Component)customButton).transform.localScale = Vector3.one;
			((Component)customButton).transform.localPosition = Vector3.zero;
			((Component)customButton).transform.localRotation = Quaternion.identity;
			((TMP_Text)customButton.TextMeshPro).text = mod.Manifest.Name;
			customButton.Clicked += delegate
			{
				ModOptionsScreen.SelectedMod = mod;
				GameCanvas.instance.SetScreen<ModOptionsScreen>();
			};
		}
		foreach (ModManifest item in ModManager.DisabledModManifests.OrderBy((ModManifest m) => m.Name))
		{
			CustomButton customButton2 = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab, (Transform)(object)ButtonsParent);
			((Component)customButton2).transform.localScale = Vector3.one;
			((Component)customButton2).transform.localPosition = Vector3.zero;
			((Component)customButton2).transform.localRotation = Quaternion.identity;
			((TMP_Text)customButton2.TextMeshPro).text = "<color=#A1A1A1><s>" + item.Name + "</s>";
		}
		OpenWorkshopButton.Clicked += delegate
		{
			OpenWorkshop();
		};
		((Component)OpenWorkshopButton).gameObject.SetActive(PlatformHelper.UseSteam);
	}

	private void Update()
	{
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		}
	}

	private static void OpenWorkshop()
	{
		if (PlatformHelper.UseSteam && !InputController.instance.GetKey((Key)53))
		{
			SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/app/1948280/workshop", (EActivateGameOverlayToWebPageMode)0);
		}
	}
}
