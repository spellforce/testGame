using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using TMPro;
using UnityEngine;

public class ModUploadScreen : SokScreen
{
	public CustomButton BackButton;

	public CustomButton UploadButton;

	public CustomButton TogglePreviewButton;

	public CustomButton SelectTagsButton;

	public bool SetPreviewImage = true;

	public TextMeshProUGUI ChangeNotesText;

	public List<string> Tags = new List<string>();

	private CallResult<CreateItemResult_t> crCreateItemResult;

	private CallResult<SubmitItemUpdateResult_t> crSubmitItemUpdateResult;

	private UGCUpdateHandle_t ugcUpdateHandle = UGCUpdateHandle_t.Invalid;

	private EItemUpdateStatus lastUpdateStatus;

	private void Awake()
	{
		TogglePreviewButton.Clicked += delegate
		{
			SetPreviewImage = !SetPreviewImage;
			((TMP_Text)TogglePreviewButton.TextMeshPro).text = "Set preview image: " + OptionsScreen.YesNo(SetPreviewImage);
		};
		SelectTagsButton.Clicked += delegate
		{
			ModalScreen instance = ModalScreen.instance;
			Tags.Clear();
			instance.Clear();
			instance.SetTexts("Select Tags", "Click tags below to enable them.\n\n<color=#0000ff><u><link=\"https://modding.stacklands.co/en/latest/guides/publishing.html#adding-tags\">What are tags?</link></u></color>");
			CreateTagButton("Cards");
			CreateTagButton("Gameplay");
			CreateTagButton("Language");
			CreateTagButton("Quality of Life");
			CreateTagButton("Content");
			CreateTagButton("Development");
			instance.AddOption("Back", delegate
			{
				GameCanvas.instance.CloseModal();
			});
			GameCanvas.instance.OpenModal();
		};
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModOptionsScreen>();
		};
	}

	private void OnEnable()
	{
		Mod mod = ModOptionsScreen.SelectedMod;
		if ((Object)(object)mod == (Object)null)
		{
			return;
		}
		crCreateItemResult = CallResult<CreateItemResult_t>.Create((APIDispatchDelegate<CreateItemResult_t>)OnCreateItem);
		crSubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create((APIDispatchDelegate<SubmitItemUpdateResult_t>)OnUpdateItem);
		if (GetWorkshopId(mod) == 0L)
		{
			ModalScreen instance = ModalScreen.instance;
			instance.Clear();
			instance.SetTexts("Create Workshop Item?", "It appears this mod has not been uploaded to the Steam Workshop yet. Would you like to create a new Workshop Item now?\n\nBy submitting this item, you agree to the <color=#0000ff><u><link=\"https://steamcommunity.com/workshop/workshoplegalagreement/\">Steam Workshop Terms Of Service</link></u></color>");
			instance.AddOption("Create Item", delegate
			{
				if (!crCreateItemResult.IsActive())
				{
					CreateWorkshopItem();
				}
			});
			instance.AddOption("Back", delegate
			{
				GameCanvas.instance.SetScreen<ModOptionsScreen>();
				GameCanvas.instance.CloseModal();
			});
			GameCanvas.instance.OpenModal();
		}
		UploadButton.Clicked += delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!(ugcUpdateHandle != UGCUpdateHandle_t.Invalid))
			{
				ModalScreen modal = ModalScreen.instance;
				modal.Clear();
				modal.SetTexts("Upload mod?", "You are about to upload the contents of the following folder to the Steam Workshop: " + mod.Path);
				modal.AddOption("Upload", delegate
				{
					modal.Clear();
					UploadWorkshopItem();
				});
				modal.AddOption("Open Folder", delegate
				{
					Application.OpenURL("file:///" + mod.Path);
				});
				modal.AddOption("Back", delegate
				{
					GameCanvas.instance.CloseModal();
				});
				GameCanvas.instance.OpenModal();
			}
		};
	}

	private void Update()
	{
		UpdateUploading();
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<ModOptionsScreen>();
		}
	}

	private void UpdateUploading()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!(ugcUpdateHandle == UGCUpdateHandle_t.Invalid))
		{
			ModalScreen instance = ModalScreen.instance;
			ulong bytes = default(ulong);
			ulong num = default(ulong);
			EItemUpdateStatus itemUpdateProgress = SteamUGC.GetItemUpdateProgress(ugcUpdateHandle, ref bytes, ref num);
			if (num != 0L)
			{
				instance.SetTexts("Uploading...", $"Status: {itemUpdateProgress} ({FormatBytes(bytes)}/{FormatBytes(num)})");
			}
			else
			{
				instance.SetTexts("Uploading...", $"Status: {itemUpdateProgress}");
			}
			if (itemUpdateProgress != lastUpdateStatus)
			{
				Debug.Log((object)itemUpdateProgress);
				lastUpdateStatus = itemUpdateProgress;
			}
		}
	}

	private void CheckForConfig()
	{
		string configPath = Path.Combine(ModOptionsScreen.SelectedMod.Path, "config.json");
		ModalScreen modal = ModalScreen.instance;
		if (File.Exists(configPath))
		{
			modal.Clear();
			modal.SetTexts("config.json detected", "A config.json file has been detected in the mod folder. If this file is uploaded, all players will have your current settings instead of the default ones. Would you like to delete the file and proceed with the upload?");
			modal.AddOption("Delete config.json & Upload", delegate
			{
				File.Delete(configPath);
				UploadWorkshopItem();
				modal.Clear();
			});
			modal.AddOption("Keep config.json & Upload (Not Recommended)", delegate
			{
				UploadWorkshopItem();
				modal.Clear();
			});
			modal.AddOption("Open Folder", delegate
			{
				Application.OpenURL("file:///" + ModOptionsScreen.SelectedMod.Path);
			});
			modal.AddOption("Cancel", delegate
			{
				GameCanvas.instance.CloseModal();
			});
		}
		else
		{
			UploadWorkshopItem();
			modal.Clear();
		}
	}

	private void CreateWorkshopItem()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Creating workshop item..");
		SteamAPICall_t val = SteamUGC.CreateItem(new AppId_t(1948280u), (EWorkshopFileType)0);
		crCreateItemResult.Set(val, (APIDispatchDelegate<CreateItemResult_t>)null);
	}

	private void UploadWorkshopItem()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Mod selectedMod = ModOptionsScreen.SelectedMod;
		string text = Path.Combine(selectedMod.Path, "config.json");
		if (File.Exists(text))
		{
			File.Move(text, Path.Combine(Application.persistentDataPath, "Mods", "config.json"));
		}
		ugcUpdateHandle = SteamUGC.StartItemUpdate(new AppId_t(1948280u), new PublishedFileId_t(GetWorkshopId(selectedMod)));
		SteamUGC.SetItemTitle(ugcUpdateHandle, selectedMod.Manifest.Name);
		SteamUGC.SetItemContent(ugcUpdateHandle, selectedMod.Path);
		if (Tags.Count > 0)
		{
			SteamUGC.SetItemTags(ugcUpdateHandle, (IList<string>)Tags);
		}
		if (SetPreviewImage && File.Exists(Path.Combine(selectedMod.Path, "icon.png")))
		{
			SteamUGC.SetItemPreview(ugcUpdateHandle, Path.Combine(selectedMod.Path, "icon.png"));
		}
		SteamAPICall_t val = SteamUGC.SubmitItemUpdate(ugcUpdateHandle, ((TMP_Text)ChangeNotesText).text);
		crSubmitItemUpdateResult.Set(val, (APIDispatchDelegate<SubmitItemUpdateResult_t>)null);
	}

	private void OnCreateItem(CreateItemResult_t result, bool failed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if ((int)result.m_eResult != 1)
		{
			Debug.LogError((object)$"uh oh, result of CreateItem is {result.m_eResult}");
			return;
		}
		Debug.Log((object)$"Item has been created: {result.m_nPublishedFileId}");
		if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
		{
			SteamFriends.ActivateGameOverlayToWebPage($"steam://url/CommunityFilePage/{result.m_nPublishedFileId}", (EActivateGameOverlayToWebPageMode)0);
		}
		Mod mod = ModOptionsScreen.SelectedMod;
		SetWorkshopId(mod, (ulong)result.m_nPublishedFileId);
		ModalScreen instance = ModalScreen.instance;
		instance.Clear();
		instance.SetTexts("Item created", "A workshop.txt file has been created in the mod folder, be sure to copy this to your source folder!");
		instance.AddOption("Open folder", delegate
		{
			Application.OpenURL("file:///" + mod.Path);
		});
		instance.AddOption("Back", delegate
		{
			GameCanvas.instance.CloseModal();
		});
	}

	private void OnUpdateItem(SubmitItemUpdateResult_t result, bool failed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		ugcUpdateHandle = UGCUpdateHandle_t.Invalid;
		lastUpdateStatus = (EItemUpdateStatus)0;
		ModalScreen instance = ModalScreen.instance;
		instance.Clear();
		if ((int)result.m_eResult == 1)
		{
			instance.SetTexts("Upload finished", "The files have been successfully uploaded");
			instance.AddOption(SokLoc.Translate("label_okay"), delegate
			{
				GameCanvas.instance.CloseModal();
			});
		}
		else
		{
			instance.SetTexts("uh oh", $"something went wrong :( {result.m_eResult}");
			instance.AddOption(SokLoc.Translate("label_okay"), delegate
			{
				GameCanvas.instance.CloseModal();
			});
		}
		string text = Path.Combine(Application.persistentDataPath, "Mods", "config.json");
		if (File.Exists(text))
		{
			File.Move(text, Path.Combine(ModOptionsScreen.SelectedMod.Path, "config.json"));
		}
		SteamFriends.ActivateGameOverlayToWebPage($"https://steamcommunity.com/sharedfiles/filedetails/?id={result.m_nPublishedFileId}", (EActivateGameOverlayToWebPageMode)0);
	}

	private void CreateTagButton(string tag)
	{
		CustomButton but = Object.Instantiate<CustomButton>(ModalScreen.instance.ButtonPrefab);
		((Component)but).transform.SetParentClean((Transform)(object)ModalScreen.instance.ButtonParent);
		((TMP_Text)but.TextMeshPro).text = "<color=#A1A1A1><s>" + tag + "</s>";
		but.Clicked += delegate
		{
			if (Tags.Contains(tag))
			{
				Tags.Remove(tag);
				((TMP_Text)but.TextMeshPro).text = "<color=#A1A1A1><s>" + tag + "</s>";
			}
			else
			{
				Tags.Add(tag);
				((TMP_Text)but.TextMeshPro).text = tag;
			}
		};
	}

	public static string FormatBytes(ulong bytes)
	{
		if (bytes < 1024)
		{
			return $"{bytes} B";
		}
		int num = (int)(Math.Log(bytes) / Math.Log(1024.0));
		return string.Format("{0:F2} {1}B", (double)bytes / Math.Pow(1024.0, num), "KMGT"[num - 1]);
	}

	public static ulong GetWorkshopId(Mod mod)
	{
		string path = Path.Combine(mod.Path, "workshop.txt");
		if (!File.Exists(path))
		{
			return 0uL;
		}
		if (ulong.TryParse(File.ReadAllText(path), out var result))
		{
			return result;
		}
		return 0uL;
	}

	public static void SetWorkshopId(Mod mod, ulong id)
	{
		File.WriteAllText(Path.Combine(mod.Path, "workshop.txt"), id.ToString());
	}
}
