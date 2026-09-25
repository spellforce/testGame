using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
	public static string Text;

	public TextMeshProUGUI TextMesh;

	private RectTransform rectTransform;

	private float tooltipSameTime;

	private string lastTooltipText;

	private static Vector2[] pivots = (Vector2[])(object)new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	private void Awake()
	{
		rectTransform = ((Component)this).GetComponent<RectTransform>();
	}

	private void LateUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		bool num = Text != "";
		if (!num)
		{
			((Component)this).transform.localScale = Vector3.zero;
		}
		else
		{
			if (Text == lastTooltipText)
			{
				tooltipSameTime += Time.deltaTime;
			}
			else
			{
				tooltipSameTime = 0f;
			}
			if (tooltipSameTime >= 0f)
			{
				((Component)this).transform.localScale = Vector3.one;
			}
			else
			{
				((Component)this).transform.localScale = Vector3.zero;
			}
		}
		((TMP_Text)TextMesh).text = Text;
		if (num)
		{
			SetRectTransformToSafePosition(rectTransform);
		}
		lastTooltipText = Text;
		Text = "";
	}

	public static void SetRectTransformToSafePosition(RectTransform rect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localScale = ((Component)rect).transform.localScale;
		((Component)rect).transform.localScale = Vector3.one;
		Vector2[] array = pivots;
		foreach (Vector2 pivot in array)
		{
			rect.pivot = pivot;
			((Transform)rect).localPosition = GameCanvas.instance.ScreenPosToLocalPos(Vector2.op_Implicit(InputController.instance.ClampedMousePosition()));
			if (!IsOverflowing(rect))
			{
				break;
			}
		}
		((Component)rect).transform.localScale = localScale;
	}

	private static bool IsOverflowing(RectTransform rect)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, (float)Screen.width, (float)Screen.height);
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		rect.GetWorldCorners(array);
		bool result = false;
		Vector3[] array2 = array;
		foreach (Vector3 val2 in array2)
		{
			if (!((Rect)(ref val)).Contains(val2))
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
