using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementElement : MonoBehaviour
{
	public Quest MyQuest;

	public CustomButton MyButton;

	public Image Checkmark;

	public Image Checkbox;

	public TextMeshProUGUI AchievementNameText;

	public GameObject NewLabel;

	private bool isVisible;

	private bool isComplete;

	private bool isNew;

	public bool IsNew => isNew;

	private void Update()
	{
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (!isVisible && !isComplete)
		{
			((TMP_Text)AchievementNameText).text = "???";
		}
		else
		{
			((TMP_Text)AchievementNameText).text = MyQuest.Description;
		}
		((Graphic)MyButton.Image).color = ColorManager.instance.BackgroundColor;
		if (MyButton.IsHovered || MyButton.IsSelected)
		{
			if (isNew || isComplete)
			{
				if (!WorldManager.instance.CurrentSave.SeenQuestIds.Contains(MyQuest.Id))
				{
					WorldManager.instance.CurrentSave.SeenQuestIds.Add(MyQuest.Id);
				}
				isNew = false;
			}
			if (isVisible || isComplete)
			{
				GameScreen.InfoBoxTitle = SokLoc.Translate("label_quest");
				GameScreen.InfoBoxText = MyQuest.Description;
			}
			else
			{
				GameScreen.InfoBoxTitle = SokLoc.Translate("label_quest");
				GameScreen.InfoBoxText = SokLoc.Translate("label_quests_complete_more_to_see");
			}
		}
		((Graphic)AchievementNameText).color = (isComplete ? ColorManager.instance.DisabledColor : Color.black);
		((Graphic)Checkmark).color = ((Graphic)AchievementNameText).color;
		((Graphic)Checkbox).color = ColorManager.instance.DisabledColor;
		if (isVisible && !MyQuest.PossibleInPeacefulMode && WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			MyButton.TooltipText = SokLoc.Translate("label_quest_not_possible_in_peaceful");
		}
		else
		{
			MyButton.TooltipText = "";
		}
		NewLabel.gameObject.SetActive(isNew);
		((Component)Checkmark).gameObject.SetActive(isComplete);
	}

	public void SetQuest(Quest ach)
	{
		MyQuest = ach;
		isComplete = QuestManager.instance.QuestIsComplete(MyQuest);
		isVisible = QuestManager.instance.QuestIsVisible(MyQuest);
		isNew = !WorldManager.instance.CurrentSave.SeenQuestIds.Contains(MyQuest.Id) && !isComplete;
		UpdateVisuals();
	}
}
