using TMPro;
using UnityEngine;

public class ScrollText : MonoBehaviour
{
	public float ScrollSpeed;

	public TextMeshProUGUI myText;

	private RectTransform myRect;

	private string lastText;

	private float startWaitTimer;

	private float endWaitTimer;

	private void Start()
	{
		myRect = ((Component)this).GetComponent<RectTransform>();
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
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		RectTransform val = (RectTransform)((Transform)myRect).parent;
		Vector2 anchoredPosition = myRect.anchoredPosition;
		startWaitTimer += Time.deltaTime;
		bool flag = anchoredPosition.y >= myRect.sizeDelta.y - val.sizeDelta.y;
		if (startWaitTimer >= 0.75f && myRect.sizeDelta.y >= val.sizeDelta.y && !flag)
		{
			anchoredPosition.y += ScrollSpeed * Time.deltaTime;
		}
		if (flag)
		{
			endWaitTimer += Time.deltaTime;
			if (endWaitTimer >= 1.5f)
			{
				anchoredPosition.y = 0f;
				startWaitTimer = 0f;
				endWaitTimer = 0f;
			}
		}
		myRect.anchoredPosition = anchoredPosition;
	}

	public void ResetScroll()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 anchoredPosition = myRect.anchoredPosition;
		anchoredPosition.y = 0f;
		myRect.anchoredPosition = anchoredPosition;
		startWaitTimer = 0f;
		endWaitTimer = 0f;
	}

	private void LateUpdate()
	{
		if ((Object)(object)myText != (Object)null && lastText != ((TMP_Text)myText).text)
		{
			ResetScroll();
		}
		lastText = (((Object)(object)myText != (Object)null) ? ((TMP_Text)myText).text : "");
	}
}
