using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpdatePopup : MonoBehaviour
{
	public TextMeshProUGUI UpdateText;

	public TextMeshProUGUI UpdateTitle;

	public CustomButton CloseUpdateInfoButton;

	public CustomButton BuyDLCButton;

	private void Awake()
	{
		CloseUpdateInfoButton.Clicked += delegate
		{
			if (PlatformHelper.IsTestBuild && WorldManager.instance.IsCitiesDlcActive())
			{
				GameCanvas.instance.ShowEarlyAccessModal();
			}
			((Component)this).gameObject.SetActive(false);
		};
		CloseUpdateInfoButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			((Navigation)(ref nav)).selectOnUp = (Selectable)(object)(((Component)BuyDLCButton).gameObject.activeInHierarchy ? BuyDLCButton : null);
			Selectable val = (((Navigation)(ref nav)).selectOnRight = null);
			Selectable selectOnDown = (((Navigation)(ref nav)).selectOnLeft = val);
			((Navigation)(ref nav)).selectOnDown = selectOnDown;
			return nav;
		};
		BuyDLCButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			Selectable val = (((Navigation)(ref nav)).selectOnRight = null);
			Selectable selectOnUp = (((Navigation)(ref nav)).selectOnLeft = val);
			((Navigation)(ref nav)).selectOnUp = selectOnUp;
			((Navigation)(ref nav)).selectOnDown = (Selectable)(object)CloseUpdateInfoButton;
			return nav;
		};
		BuyDLCButton.Clicked += delegate
		{
			SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/2867570/Stacklands_2000", (EActivateGameOverlayToWebPageMode)0);
		};
		UpdatePopupText();
	}

	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(((Component)CloseUpdateInfoButton).gameObject);
	}

	private void Update()
	{
		UpdatePopupText();
	}

	private void UpdatePopupText()
	{
		((TMP_Text)UpdateTitle).text = SokLoc.Translate("label_update_title_cities");
		if (WorldManager.instance.IsCitiesDlcActive())
		{
			((TMP_Text)UpdateText).text = SokLoc.Translate("label_update_text_cities");
			((Component)BuyDLCButton).gameObject.SetActive(false);
		}
		else
		{
			((TMP_Text)UpdateText).text = SokLoc.Translate("label_update_text_cities_locked");
			((Component)BuyDLCButton).gameObject.SetActive(true);
		}
	}
}
