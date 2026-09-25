using TMPro;
using UnityEngine;

public class ScrollTextHorizontal : MonoBehaviour
{
	public float ScrollSpeed;

	private TextMeshProUGUI myText;

	private RectTransform myRect;

	private string lastText;

	private float startWaitTimer;

	private float endWaitTimer;

	private void Start()
	{
		myRect = ((Component)this).GetComponent<RectTransform>();
		myText = ((Component)this).GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		RectTransform val = (RectTransform)((Transform)myRect).parent;
		Vector2 anchoredPosition = myRect.anchoredPosition;
		startWaitTimer += Time.deltaTime;
		bool flag = anchoredPosition.x <= 0f - (myRect.sizeDelta.x - val.sizeDelta.x);
		if (startWaitTimer >= 0.75f && myRect.sizeDelta.x >= val.sizeDelta.x && !flag)
		{
			anchoredPosition.x -= ScrollSpeed * Time.deltaTime;
		}
		if (flag)
		{
			endWaitTimer += Time.deltaTime;
			if (endWaitTimer >= 1.5f)
			{
				anchoredPosition.x = 0f;
				startWaitTimer = 0f;
				endWaitTimer = 0f;
			}
		}
		myRect.anchoredPosition = anchoredPosition;
	}

	private void LateUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (lastText != ((TMP_Text)myText).text)
		{
			Vector2 anchoredPosition = myRect.anchoredPosition;
			anchoredPosition.x = 0f;
			myRect.anchoredPosition = anchoredPosition;
			startWaitTimer = 0f;
			endWaitTimer = 0f;
		}
		lastText = ((TMP_Text)myText).text;
	}
}
