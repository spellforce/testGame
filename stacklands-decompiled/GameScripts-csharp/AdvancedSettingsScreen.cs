using TMPro;
using UnityEngine;

public class AdvancedSettingsScreen : SokScreen
{
	public CustomButton BackButton;

	public CustomButton AdvancedCombatStatsButton;

	public static bool AdvancedCombatStatsEnabled;

	private void Awake()
	{
		AdvancedCombatStatsButton.Clicked += delegate
		{
			AdvancedCombatStatsEnabled = !AdvancedCombatStatsEnabled;
			SaveSettings();
		};
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		LoadSettings();
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)AdvancedCombatStatsButton.TextMeshPro).text = SokLoc.Translate("label_advanced_combat", (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", OptionsScreen.YesNo(AdvancedCombatStatsEnabled)) });
	}

	private void OnApplicationQuit()
	{
		SaveSettings();
	}

	private void LoadSettings()
	{
		AdvancedCombatStatsEnabled = PlayerPrefs.GetInt("AdvancedCombatStatsEnabled", 0) == 1;
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetInt("AdvancedCombatStatsEnabled", AdvancedCombatStatsEnabled ? 1 : 0);
	}
}
