using System;
using TMPro;
using UnityEngine;

public class NotificationElement : MonoBehaviour
{
	public CustomButton Button;

	public TextMeshProUGUI NotificationTitle;

	public TextMeshProUGUI NotificationText;

	public Action OnClicked;

	private float timer;

	private void Start()
	{
		Button.Clicked += delegate
		{
			OnClicked?.Invoke();
			Object.Destroy((Object)(object)((Component)this).gameObject);
		};
	}

	private void Update()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		timer += WorldManager.instance.TimeScale * Time.deltaTime;
		if (timer > 30f)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, Vector3.one, Time.deltaTime * 12f);
	}
}
