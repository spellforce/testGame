using UnityEngine;

public class ShowTooltip : MonoBehaviour
{
	public string MyTooltipTerm;

	public string MyTooltipText;

	private void Update()
	{
		if (InputController.instance.CurrentScheme != ControlScheme.KeyboardMouse)
		{
			return;
		}
		GameObject mouseOverObject = GameCanvas.instance.MouseOverObject;
		if ((Object)(object)mouseOverObject != (Object)null && (mouseOverObject.transform.IsChildOf(((Component)this).transform) || (Object)(object)mouseOverObject.transform == (Object)(object)((Component)this).transform))
		{
			if (!string.IsNullOrEmpty(MyTooltipTerm))
			{
				Tooltip.Text = SokLoc.Translate(MyTooltipTerm);
			}
			else
			{
				Tooltip.Text = MyTooltipText;
			}
		}
	}
}
