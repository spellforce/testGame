using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : SokScreen
{
	public CustomButton NewGameButton;

	public CustomButton ContinueButton;

	public CustomButton CardopediaButton;

	public CustomButton OptionsButton;

	public CustomButton ModsButton;

	public CustomButton QuitButton;

	public CustomButton JoinDiscordButton;

	public CustomButton UpdateInfoButton;

	public TextMeshProUGUI UpdateText;

	public RectTransform CardopediaNewElement;

	public UpdatePopup UpdatePopup;

	public TextMeshProUGUI TitleText2000;

	private float timer;

	private void StartNewRun()
	{
		GameCanvas.instance.SetScreen<RunOptionsScreen>();
	}

	private void Awake()
	{
		JoinDiscordButton.Clicked += delegate
		{
			Application.OpenURL("https://discord.gg/sokpop");
		};
		NewGameButton.Clicked += delegate
		{
			if (WorldManager.instance.CurrentSave.LastPlayedRound != null)
			{
				GameCanvas.instance.ShowStartNewRunModal(delegate
				{
					StartNewRun();
				});
			}
			else
			{
				StartNewRun();
			}
		};
		ContinueButton.Clicked += delegate
		{
			HashSet<string> hashSet = WorldManager.instance.FindMissingCardsInSave();
			if (hashSet.Count == 0)
			{
				TransitionScreen.instance.StartTransition(delegate
				{
					WorldManager.instance.LoadPreviousRound();
					WorldManager.instance.Play();
				}, 1f);
			}
			else
			{
				GameCanvas.instance.MissingCardsInSavePrompt(delegate
				{
					TransitionScreen.instance.StartTransition(delegate
					{
						WorldManager.instance.LoadPreviousRound();
						WorldManager.instance.Play();
					}, 1f);
				}, hashSet);
			}
		};
		OptionsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		if (!PlatformHelper.HasModdingSupport)
		{
			((Component)ModsButton).gameObject.SetActive(false);
		}
		ModsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModsScreen>();
		};
		CardopediaButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<CardopediaScreen>();
		};
		QuitButton.Clicked += delegate
		{
			Application.Quit();
		};
		UpdateInfoButton.Clicked += delegate
		{
			((Component)UpdatePopup).gameObject.SetActive(true);
		};
		((Component)UpdatePopup).gameObject.SetActive(false);
		if (PlayerPrefs.GetInt("showUpdatePopup", 4) <= 4)
		{
			((Component)UpdatePopup).gameObject.SetActive(true);
			PlayerPrefs.SetInt("showUpdatePopup", 5);
		}
		((Component)TitleText2000).gameObject.SetActive(WorldManager.instance.IsCitiesDlcActive());
	}

	private void Update()
	{
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		timer += Time.deltaTime;
		if (InputController.instance.GetKeyDown((Key)1) || InputController.instance.GetKeyDown((Key)60))
		{
			((Component)UpdatePopup).gameObject.SetActive(false);
		}
		((Component)JoinDiscordButton).gameObject.SetActive(GameCanvas.instance.ScreenIsInteractable<MainMenu>());
		((Component)CardopediaNewElement).gameObject.SetActive(WorldManager.instance.CurrentSave.NewCardopediaIds.Count > 0);
		if (WorldManager.instance.CurrentSave.LastPlayedRound != null)
		{
			string text = ((WorldManager.instance.CurrentSave.LastPlayedRound.SaveVersion <= 1) ? WorldManager.instance.CurrentSave.LastPlayedRound.CurrentMonth.ToString() : ((!(WorldManager.instance.CurrentSave.LastPlayedRound.CurrentBoardId == "cities")) ? (WorldManager.instance.CurrentSave.LastPlayedRound.BoardMonths.MainMonth + WorldManager.instance.CurrentSave.LastPlayedRound.BoardMonths.IslandMonth).ToString() : WorldManager.instance.CurrentSave.LastPlayedRound.BoardMonths.CitiesMonth.ToString()));
			((TMP_Text)ContinueButton.TextMeshPro).text = SokLoc.Translate("label_continue_run", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", text) });
		}
		if (WorldManager.instance.IsCitiesDlcActive())
		{
			((TMP_Text)UpdateText).text = SokLoc.Translate("label_menu_cities_title");
		}
		else
		{
			((TMP_Text)UpdateText).text = SokLoc.Translate("label_menu_cities_title_locked");
		}
		((Component)ContinueButton).gameObject.SetActive(WorldManager.instance.CurrentSave.LastPlayedRound != null);
		Vector3 val = default(Vector3);
		if (UpdateInfoButton.IsHovered)
		{
			((Vector3)(ref val))._002Ector(1.2f, 1.2f, 1.2f);
		}
		else
		{
			val = Vector3.one * (1f + Mathf.Sin(timer * 2f) * 0.1f);
		}
		((Component)UpdateInfoButton).transform.localScale = Vector3.Lerp(((Component)UpdateInfoButton).transform.localScale, val, Time.deltaTime * 18f);
	}
}
