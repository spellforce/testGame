using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunOptionsScreen : SokScreen
{
	public CustomButton ShortMoon;

	public CustomButton NormalMoon;

	public CustomButton LongMoon;

	public CustomButton PeacefulModeOn;

	public CustomButton PeacefulModeOff;

	public CustomButton PlayButton;

	public CustomButton BackButton;

	public CustomButton HappinessButton;

	public CustomButton DeathButton;

	public RectTransform CurseOptions;

	public MoonLength CurMoonLength = MoonLength.Normal;

	public bool PeacefulMode;

	public bool EnableGreed;

	public bool EnableHappiness;

	public bool EnableDeath;

	private void Start()
	{
		PlayButton.Clicked += delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				WorldManager.instance.CurrentRunOptions = new RunOptions
				{
					MoonLength = CurMoonLength,
					IsPeacefulMode = PeacefulMode,
					IsGreedEnabled = EnableGreed,
					IsDeathEnabled = EnableDeath,
					IsHappinessEnabled = EnableHappiness
				};
				WorldManager.instance.CurrentSave?.LastPlayedRound?.SavedBoosterBoxes?.Clear();
				foreach (BuyBoosterBox allBoosterBox in WorldManager.instance.AllBoosterBoxes)
				{
					allBoosterBox.StoredCostAmount = 0;
				}
				WorldManager.instance.CurrentRunVariables = new RunVariables();
				WorldManager.instance.RoundExtraKeyValues = new List<SerializedKeyValuePair>();
				CitiesManager.instance.Wellbeing = CitiesManager.instance.WellbeingStart;
				WorldManager.instance.StartNewRound();
				WorldManager.instance.Play();
			});
		};
		BackButton.Clicked += delegate
		{
		};
		ShortMoon.Clicked += delegate
		{
			CurMoonLength = MoonLength.Short;
		};
		NormalMoon.Clicked += delegate
		{
			CurMoonLength = MoonLength.Normal;
		};
		LongMoon.Clicked += delegate
		{
			CurMoonLength = MoonLength.Long;
		};
		PeacefulModeOn.Clicked += delegate
		{
			PeacefulMode = true;
		};
		PeacefulModeOff.Clicked += delegate
		{
			PeacefulMode = false;
		};
		HappinessButton.Clicked += delegate
		{
			EnableHappiness = !EnableHappiness;
		};
		DeathButton.Clicked += delegate
		{
			EnableDeath = !EnableDeath;
		};
	}

	private void Update()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)ShortMoon.Image).color = ((CurMoonLength != MoonLength.Short) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)NormalMoon.Image).color = ((CurMoonLength != MoonLength.Normal) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)LongMoon.Image).color = ((CurMoonLength != MoonLength.Long) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)PeacefulModeOn.Image).color = ((!PeacefulMode) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Graphic)PeacefulModeOff.Image).color = (PeacefulMode ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		((Component)CurseOptions).gameObject.SetActive(WorldManager.instance.IsSpiritDlcActive());
		SetCurseButton(DeathButton, WorldManager.instance.CurrentSave.FinishedDeath, EnableDeath, "label_enable_death_curse");
		SetCurseButton(HappinessButton, WorldManager.instance.CurrentSave.FinishedHappiness, EnableHappiness, "label_enable_happiness_curse");
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		}
	}

	private void SetCurseButton(CustomButton but, bool curseUnlocked, bool curseEnabled, string mainTerm)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (curseUnlocked)
		{
			but.ButtonEnabled = true;
			but.TooltipText = "";
			((TMP_Text)but.TextMeshPro).text = SokLoc.Translate(mainTerm, (LocParam[])(object)new LocParam[1] { LocParam.Create("on_off", YesNo(curseEnabled)) });
		}
		else
		{
			but.ButtonEnabled = false;
			((TMP_Text)but.TextMeshPro).text = SokLoc.Translate("label_enable_curse_unknown");
			but.TooltipText = SokLoc.Translate("label_beat_curse_to_unlock");
		}
	}

	public static string YesNo(bool a)
	{
		if (!a)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}
}
