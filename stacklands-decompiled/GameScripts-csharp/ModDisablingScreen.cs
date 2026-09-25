using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ModDisablingScreen : SokScreen
{
	public CustomButton BackButton;

	public RectTransform ButtonsParent;

	public bool ShouldRestart;

	private void Awake()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		BackButton.Clicked += delegate
		{
			SaveManager.instance.Save(saveRound: false);
			if (ShouldRestart)
			{
				WorldManager.RebootGame();
			}
			GameCanvas.instance.SetScreen<ModsScreen>();
		};
		List<ModManifest> list = new List<ModManifest>();
		if (ModManager.LoadedMods != null)
		{
			list.AddRange(ModManager.LoadedMods.Select((Mod m) => m.Manifest));
		}
		if (ModManager.DisabledModManifests != null)
		{
			list.AddRange(ModManager.DisabledModManifests);
		}
		list.Sort((ModManifest x, ModManifest y) => string.Compare(x.Name, y.Name));
		foreach (ModManifest manifest in list)
		{
			CustomButton btn = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab, (Transform)(object)ButtonsParent);
			((Component)btn).transform.localScale = Vector3.one;
			((Component)btn).transform.localPosition = Vector3.zero;
			((Component)btn).transform.localRotation = Quaternion.identity;
			if (ModManager.DisabledModManifests.Contains(manifest))
			{
				((TMP_Text)btn.TextMeshPro).text = "<color=#A1A1A1><s>" + manifest.Name + "</s>";
			}
			else
			{
				((TMP_Text)btn.TextMeshPro).text = manifest.Name;
			}
			btn.Clicked += delegate
			{
				if (ModManager.DisabledModManifests.Contains(manifest))
				{
					ModManager.DisabledModManifests.Remove(manifest);
					SaveManager.instance.CurrentSave.DisabledMods.Remove(manifest.Id);
					((TMP_Text)btn.TextMeshPro).text = manifest.Name;
				}
				else
				{
					ModManager.DisabledModManifests.Add(manifest);
					SaveManager.instance.CurrentSave.DisabledMods.Add(manifest.Id);
					((TMP_Text)btn.TextMeshPro).text = "<color=#A1A1A1><s>" + manifest.Name + "</s>";
				}
				ShouldRestart = true;
			};
		}
	}
}
