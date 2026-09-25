using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Colorizer : MonoBehaviour
{
	public UIColor Color;

	private void OnValidate()
	{
		SetColors();
	}

	private void Start()
	{
		SetColors();
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			SetColors();
		}
	}

	private void SetColors()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)ColorManager.instance == (Object)null))
		{
			Image component = ((Component)this).GetComponent<Image>();
			if ((Object)(object)component != (Object)null)
			{
				((Graphic)component).color = ColorManager.instance.GetColor(Color);
			}
			TextMeshProUGUI component2 = ((Component)this).GetComponent<TextMeshProUGUI>();
			if ((Object)(object)component2 != (Object)null)
			{
				((Graphic)component2).color = ColorManager.instance.GetColor(Color);
			}
		}
	}
}
