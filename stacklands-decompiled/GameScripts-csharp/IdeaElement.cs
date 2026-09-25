using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdeaElement : MonoBehaviour
{
	public CustomButton MyButton;

	[HideInInspector]
	public IKnowledge MyKnowledge;

	public TextMeshProUGUI MyText;

	public GameObject NewLabel;

	public bool IsNew;

	private void Update()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		NewLabel.gameObject.SetActive(IsNew);
		if (MyButton.IsHovered || MyButton.IsSelected)
		{
			if (IsNew)
			{
				WorldManager.instance.CurrentSave.NewKnowledgeIds.Remove(MyKnowledge.CardId);
				IsNew = false;
			}
			GameScreen.InfoBoxTitle = MyKnowledge.KnowledgeName;
			GameScreen.InfoBoxText = MyKnowledge.KnowledgeText;
		}
		((Graphic)MyButton.Image).color = ColorManager.instance.BackgroundColor;
	}

	public void SetKnowledge(IKnowledge knowledge)
	{
		MyKnowledge = knowledge;
		IsNew = WorldManager.instance.CurrentSave.NewKnowledgeIds.Contains(MyKnowledge.CardId);
		((TMP_Text)MyText).text = "• " + MyKnowledge.KnowledgeName;
	}
}
