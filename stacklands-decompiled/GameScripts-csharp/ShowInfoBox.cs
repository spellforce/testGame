using UnityEngine;

public class ShowInfoBox : MonoBehaviour
{
	public string InfoBoxTitle;

	public string InfoBoxText;

	private RectTransform rectTransform;

	private void Start()
	{
		rectTransform = ((Component)this).GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (InputController.instance.CurrentScheme == ControlScheme.KeyboardMouse)
		{
			if (GameCanvas.instance.AboveMeOrMyChildren((Transform)(object)rectTransform, GameCanvas.instance.MouseOverObject))
			{
				GameScreen.InfoBoxTitle = InfoBoxTitle;
				GameScreen.InfoBoxText = InfoBoxText;
			}
		}
		else if (InputController.instance.CurrentSchemeIsController && (Object)(object)GameCanvas.instance.SelectedObject == (Object)(object)((Component)this).gameObject)
		{
			GameScreen.InfoBoxTitle = InfoBoxTitle;
			GameScreen.InfoBoxText = InfoBoxText;
		}
	}
}
