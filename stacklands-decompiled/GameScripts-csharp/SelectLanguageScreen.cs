using TMPro;
using UnityEngine;

public class SelectLanguageScreen : SokScreen
{
	public RectTransform ButtonsParent;

	public CustomButton BackButton;

	private void Start()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		SokLanguage[] languages = SokLoc.Languages;
		foreach (SokLanguage language in languages)
		{
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab);
			((Component)customButton).transform.SetParent((Transform)(object)ButtonsParent);
			((Component)customButton).transform.localScale = Vector3.one;
			((Component)customButton).transform.localPosition = Vector3.zero;
			((Component)customButton).transform.localRotation = Quaternion.identity;
			((Object)customButton).name = language.LanguageName;
			((TMP_Text)customButton.TextMeshPro).text = SokLoc.GetLocalLanguageName(language.LanguageName);
			((Component)customButton).GetComponentInChildren<FontSetter>().LanguageOverride = language.LanguageName;
			customButton.Clicked += delegate
			{
				SokLoc.instance.SetLanguage(language.LanguageName);
				OptionsScreen.SaveSettings();
			};
		}
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
	}
}
