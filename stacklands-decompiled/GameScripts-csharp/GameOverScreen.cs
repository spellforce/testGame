using TMPro;
using UnityEngine;

public class GameOverScreen : SokScreen
{
	public CustomButton BackButton;

	public TextMeshProUGUI StatsText;

	private float timer;

	private void Awake()
	{
		BackButton.Clicked += delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				WorldManager.instance.ClearRoundAndRestart();
			});
		};
	}

	private void OnEnable()
	{
		((TMP_Text)StatsText).maxVisibleLines = 0;
	}

	private void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		text = text + SokLoc.Translate("label_you_reached_moon", (LocParam[])(object)new LocParam[1] { LocParam.Create("moon", WorldManager.instance.CurrentMonth.ToString()) }) + "\n";
		text = text + SokLoc.Translate("label_quests_completed", (LocParam[])(object)new LocParam[1] { LocParam.Plural("count", WorldManager.instance.QuestsCompleted) }) + "\n";
		text = text + SokLoc.Translate("label_new_cards_found", (LocParam[])(object)new LocParam[1] { LocParam.Plural("count", WorldManager.instance.NewCardsFound) }) + "\n";
		((TMP_Text)StatsText).text = text;
		timer += Time.deltaTime;
		if (timer >= 0.3f)
		{
			timer = 0f;
			TextMeshProUGUI statsText = StatsText;
			int maxVisibleLines = ((TMP_Text)statsText).maxVisibleLines;
			((TMP_Text)statsText).maxVisibleLines = maxVisibleLines + 1;
		}
	}
}
